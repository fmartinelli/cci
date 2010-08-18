﻿//-----------------------------------------------------------------------------
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the Microsoft Public License.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//-----------------------------------------------------------------------------
using System;
using System.IO;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using System.Collections.Generic;
using Microsoft.Cci.Contracts;

namespace Microsoft.Cci.MutableContracts {

  /// <summary>
  /// Helper class for performing common tasks on mutable contracts
  /// </summary>
  public class ContractHelper {

    /// <summary>
    /// Accumulates all elements from <paramref name="sourceContract"/> into <paramref name="targetContract"/>
    /// </summary>
    /// <param name="targetContract">Contract which is target of accumulator</param>
    /// <param name="sourceContract">Contract which is source of accumulator</param>
    public static void AddMethodContract(MethodContract targetContract, IMethodContract sourceContract) {
      targetContract.Preconditions.AddRange(sourceContract.Preconditions);
      targetContract.Postconditions.AddRange(sourceContract.Postconditions);
      targetContract.ThrownExceptions.AddRange(sourceContract.ThrownExceptions);
      targetContract.IsPure |= sourceContract.IsPure; // need the disjunction
      return;
    }

    /// <summary>
    /// Mutates the given <paramref name="module"/> by injecting calls to contract methods at the
    /// beginnning of any method in the <paramref name="module"/> that has a corresponding contract
    /// in the <paramref name="contractProvider"/>. It also creates a contract invariant method
    /// for each type in <paramref name="module"/> that has a type contract in the
    /// <paramref name="contractProvider"/>.
    /// </summary>
    public static void InjectContractCalls(IMetadataHost host, Module module, ContractProvider contractProvider, ISourceLocationProvider sourceLocationProvider) {
      var cci = new ContractCallInjector(host, contractProvider, sourceLocationProvider);
      cci.Visit(module);
    }

    private class ContractCallInjector : CodeAndContractMutator {

      private readonly ITypeReference systemVoid;

      public ContractCallInjector(IMetadataHost host, ContractProvider contractProvider, ISourceLocationProvider sourceLocationProvider)
        : base(host, true, sourceLocationProvider, contractProvider) {
        this.systemVoid = host.PlatformType.SystemVoid;
      }

      private static List<T> MkList<T>(T t) { var xs = new List<T>(); xs.Add(t); return xs; }

      /// <summary>
      /// If the <paramref name="typeDefinition"/> has a type contract, generate a
      /// contract invariant method and add it to the Methods of the <paramref name="typeDefinition"/>.
      /// </summary>
      protected override void Visit(TypeDefinition typeDefinition) {
        ITypeContract typeContract = this.contractProvider.GetTypeContractFor(typeDefinition);
        if (typeContract != null) {
          #region Define the method
          List<IStatement> statements = new List<IStatement>();
          var methodBody = new SourceMethodBody(this.host, null, null) {
            LocalsAreZeroed = true,
            Block = new BlockStatement() { Statements = statements }
          };
          List<ICustomAttribute> attributes = new List<ICustomAttribute>();
          MethodDefinition m = new MethodDefinition() {
            Attributes = attributes,
            Body = methodBody,
            CallingConvention = CallingConvention.HasThis,
            ContainingTypeDefinition = typeDefinition,
            InternFactory = this.host.InternFactory,
            IsStatic = false,
            Name = this.host.NameTable.GetNameFor("$InvariantMethod$"),
            Type = systemVoid,
            Visibility = TypeMemberVisibility.Private,
          };
          methodBody.MethodDefinition = m;
          #region Add calls to Contract.Invariant
          foreach (var inv in typeContract.Invariants) {
            var methodCall = new MethodCall() {
              Arguments = MkList(inv.Condition),
              IsStaticCall = true,
              MethodToCall = this.contractProvider.ContractMethods.Invariant,
              Type = systemVoid,
              Locations = new List<ILocation>(inv.Locations),
            };
            ExpressionStatement es = new ExpressionStatement() {
              Expression = methodCall
            };
            statements.Add(es);
          }
          statements.Add(new ReturnStatement());
          #endregion
          #region Add [ContractInvariantMethod]
          var contractInvariantMethodType = new NamespaceTypeReference(
            this.host,
            this.host.PlatformType.SystemDiagnosticsContractsContract.ContainingUnitNamespace,
            this.host.NameTable.GetNameFor("ContractInvariantMethodAttribute"),
            0,
            false,
            false,
            PrimitiveTypeCode.NotPrimitive
            );
          var contractInvariantMethodCtor = new Microsoft.Cci.MutableCodeModel.MethodReference() {
            CallingConvention = CallingConvention.HasThis,
            ContainingType = contractInvariantMethodType,
            GenericParameterCount = 0,
            Name = host.NameTable.Ctor,
            Type = host.PlatformType.SystemVoid,
          };
          var contractInvariantMethodAttribute = new CustomAttribute();
          contractInvariantMethodAttribute.Constructor = contractInvariantMethodCtor;
          attributes.Add(contractInvariantMethodAttribute);
          #endregion
          typeDefinition.Methods.Add(m);
          #endregion Define the method
        }
        base.Visit(typeDefinition);
      }

      public override MethodDefinition Visit(MethodDefinition methodDefinition) {
        IMethodContract methodContract = this.contractProvider.GetMethodContractFor(methodDefinition);
        if (methodContract == null) return methodDefinition;
        SourceMethodBody sourceMethodBody = methodDefinition.Body as SourceMethodBody;
        if (sourceMethodBody == null) return methodDefinition;
        List<IStatement> statements = new List<IStatement>();
        foreach (var precondition in methodContract.Preconditions) {
          var methodCall = new MethodCall() {
            Arguments = MkList(precondition.Condition),
            IsStaticCall = true,
            MethodToCall = this.contractProvider.ContractMethods.Requires,
            Type = systemVoid,
            Locations = new List<ILocation>(precondition.Locations),
          };
          ExpressionStatement es = new ExpressionStatement() {
            Expression = methodCall
          };
          statements.Add(es);
        }
        foreach (var postcondition in methodContract.Postconditions) {
          var methodCall = new MethodCall() {
            Arguments = MkList(this.Visit(postcondition.Condition)),
            IsStaticCall = true,
            MethodToCall = this.contractProvider.ContractMethods.Ensures,
            Type = systemVoid,
            Locations = new List<ILocation>(postcondition.Locations),
          };
          ExpressionStatement es = new ExpressionStatement() {
            Expression = methodCall
          };
          statements.Add(es);
        }
        List<IStatement> existingStatements = new List<IStatement>(sourceMethodBody.Block.Statements);
        statements.AddRange(this.Visit(existingStatements)); // replaces assert/assume
        sourceMethodBody.Block = new BlockStatement() {
          Statements = statements,
        };
        return methodDefinition;
      }

      /// <summary>
      /// Converts the assert statement into a call to Contract.Assert
      /// </summary>
      public override IStatement Visit(AssertStatement assertStatement) {
        var methodCall = new MethodCall() {
          Arguments = MkList(assertStatement.Condition),
          IsStaticCall = true,
          MethodToCall = this.contractProvider.ContractMethods.Assert,
          Type = systemVoid,
          Locations = new List<ILocation>(assertStatement.Locations),
        };
        ExpressionStatement es = new ExpressionStatement() {
          Expression = methodCall
        };
        return es;
      }

      /// <summary>
      /// Converts the assume statement into a call to Contract.Assume
      /// </summary>
      public override IStatement Visit(AssumeStatement assumeStatement) {
        var methodCall = new MethodCall() {
          Arguments = MkList(assumeStatement.Condition),
          IsStaticCall = true,
          MethodToCall = this.contractProvider.ContractMethods.Assume,
          Type = systemVoid,
          Locations = new List<ILocation>(assumeStatement.Locations),
        };
        ExpressionStatement es = new ExpressionStatement() {
          Expression = methodCall
        };
        return es;
      }

      /// <summary>
      /// Converts the old value into a call to Contract.OldValue
      /// </summary>
      public override IExpression Visit(OldValue oldValue) {

        var mref = this.contractProvider.ContractMethods.Old;
        var methodToCall = new Microsoft.Cci.MutableCodeModel.GenericMethodInstanceReference() {
          CallingConvention = CallingConvention.Generic,
          ContainingType = mref.ContainingType,
          GenericArguments = new List<ITypeReference> { oldValue.Type },
          GenericMethod = mref,
          InternFactory = this.host.InternFactory,
          Name = mref.Name,
          Parameters = new List<IParameterTypeInformation>{
            new ParameterTypeInformation { Type = oldValue.Type, }
          },
          Type = oldValue.Type,
        };
        var methodCall = new MethodCall() {
          Arguments = MkList(oldValue.Expression),
          IsStaticCall = true,
          MethodToCall = methodToCall,
          Type = oldValue.Type,
          Locations = new List<ILocation>(oldValue.Locations),
        };
        return methodCall;
      }

      /// <summary>
      /// Converts the return value into a call to Contract.Result
      /// </summary>
      public override IExpression Visit(ReturnValue returnValue) {

        var mref = this.contractProvider.ContractMethods.Result;
        var methodToCall = new Microsoft.Cci.MutableCodeModel.GenericMethodInstanceReference() {
          CallingConvention = CallingConvention.Generic,
          ContainingType = mref.ContainingType,
          GenericArguments = new List<ITypeReference> { returnValue.Type },
          GenericMethod = mref,
          InternFactory = this.host.InternFactory,
          Name = mref.Name,
          Parameters = new List<IParameterTypeInformation>(),
          Type = returnValue.Type,
        };
        var methodCall = new MethodCall() {
          IsStaticCall = true,
          MethodToCall = methodToCall,
          Type = returnValue.Type,
          Locations = new List<ILocation>(returnValue.Locations),
        };
        return methodCall;
      }
    }

    /// <summary>
    /// Accumulates all elements from <paramref name="sourceContract"/> into <paramref name="targetContract"/>
    /// </summary>
    /// <param name="targetContract">Contract which is target of accumulator</param>
    /// <param name="sourceContract">Contract which is source of accumulator</param>
    public static void AddTypeContract(TypeContract targetContract, ITypeContract sourceContract) {
      targetContract.ContractFields.AddRange(sourceContract.ContractFields);
      targetContract.ContractMethods.AddRange(sourceContract.ContractMethods);
      targetContract.Invariants.AddRange(sourceContract.Invariants);
      return;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    public static IMethodReference UninstantiateAndUnspecialize(IMethodReference method) {
      IMethodReference result = method;
      IGenericMethodInstanceReference gmir = result as IGenericMethodInstanceReference;
      if (gmir != null) {
        result = gmir.GenericMethod;
      }
      // REVIEW: This next block is needed because ISpecializedMethodDefinition isn't
      // a subtype of ISpecializedMethodReference. Should it be?
      ISpecializedMethodDefinition smd = result as ISpecializedMethodDefinition;
      if (smd != null) {
        result = smd.UnspecializedVersion;
      }
      ISpecializedMethodReference smr = result as ISpecializedMethodReference;
      if (smr != null) {
        result = smr.UnspecializedVersion;
      }
      return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static ITypeReference Unspecialized(ITypeReference type) {
      var instance = type as IGenericTypeInstanceReference;
      if (instance != null) {
        return instance.GenericType;
      }
      return type;
    }

    /// <summary>
    /// Returns a type definition for a type referenced in a custom attribute.
    /// </summary>
    /// <param name="typeDefinition">The type definition whose attributes will be searched</param>
    /// <param name="attributeName">Name of the attribute.</param>
    /// <returns></returns>
    public static ITypeDefinition/*?*/ GetTypeDefinitionFromAttribute(ITypeDefinition typeDefinition, string attributeName) {
      ICustomAttribute foundAttribute = null;
      foreach (ICustomAttribute attribute in typeDefinition.Attributes) {
        if (TypeHelper.GetTypeName(attribute.Type) == attributeName) {
          foundAttribute = attribute;
          break;
        }
      }
      if (foundAttribute == null) return null;
      List<IMetadataExpression> args = new List<IMetadataExpression>(foundAttribute.Arguments);
      if (args.Count < 1) return null;
      IMetadataTypeOf abstractTypeMD = args[0] as IMetadataTypeOf;
      if (abstractTypeMD == null) return null;
      ITypeReference referencedTypeReference = Unspecialized(abstractTypeMD.TypeToGet);
      ITypeDefinition referencedTypeDefinition = referencedTypeReference.ResolvedType;
      return referencedTypeDefinition;
    }

    /// <summary>
    /// Given an abstract method (i.e., either an interface method, J.M, or else
    /// an abstract method M, see if its defining type is marked with the
    /// [ContractClass(typeof(T))] attribute. If so, then return T.J.M, if the defining
    /// type is an interface, or T.M if T is an abstract class, otherwise null.
    /// (Note: in the interface case, T must explicitly implement J.M, not implicitly!!!
    /// </summary>
    /// <param name="methodDefinition"></param>
    /// <returns></returns>
    public static IMethodDefinition/*?*/ GetMethodFromContractClass(IMethodDefinition methodDefinition) {

      var unspecializedMethodDefinition = UninstantiateAndUnspecialize(methodDefinition);
      ITypeDefinition definingType = unspecializedMethodDefinition.ResolvedMethod.ContainingTypeDefinition;

      var typeHoldingContractDefinition = GetTypeDefinitionFromAttribute(definingType, "System.Diagnostics.Contracts.ContractClassAttribute");
      if (typeHoldingContractDefinition == null) return null;
      if (definingType.IsInterface) {
        #region Explicit Interface Implementations
        foreach (IMethodImplementation methodImplementation in typeHoldingContractDefinition.ExplicitImplementationOverrides) {
          var implementedInterfaceMethod = UninstantiateAndUnspecialize(methodImplementation.ImplementedMethod);
          if (unspecializedMethodDefinition.InternedKey == implementedInterfaceMethod.InternedKey)
            return methodImplementation.ImplementingMethod.ResolvedMethod;
        }
        #endregion Explicit Interface Implementations
        #region Implicit Interface Implementations
        var implicitImplementations = typeHoldingContractDefinition.GetMatchingMembers(
          tdm => {
            IMethodDefinition md = tdm as IMethodDefinition;
            if (md == null) return false;
            if (md.Name != methodDefinition.Name) return false;
            return MemberHelper.MethodsAreEquivalent(md, methodDefinition);
          });
        if (IteratorHelper.EnumerableIsNotEmpty(implicitImplementations))
          return IteratorHelper.Single(implicitImplementations) as IMethodDefinition;
        #endregion Implicit Interface Implementations
        return null;
      } else if (methodDefinition.IsAbstract) {
        IMethodReference methodReference = MemberHelper.GetImplicitlyOverridingDerivedClassMethod(methodDefinition, typeHoldingContractDefinition);
        if (methodReference == Dummy.Method) return null;
        return methodReference.ResolvedMethod;
      }
      return null;
    }

    /// <summary>
    /// Given an method, M, see if it is declared in a type that is holding a contract class, i.e.,
    /// it will be marked with [ContractClassFor(typeof(T))]. If so, then return T.M, else null.
    /// </summary>
    /// <param name="methodDefinition"></param>
    /// <returns></returns>
    public static IMethodDefinition/*?*/ GetAbstractMethodForContractMethod(IMethodDefinition methodDefinition) {
      ITypeDefinition definingType = methodDefinition.ContainingTypeDefinition;
      var abstractTypeDefinition = GetTypeDefinitionFromAttribute(definingType, "System.Diagnostics.Contracts.ContractClassForAttribute");
      if (abstractTypeDefinition == null) return null;
      if (abstractTypeDefinition.IsInterface) {
        foreach (IMethodReference methodReference in MemberHelper.GetExplicitlyOverriddenMethods(methodDefinition)) {
          return methodReference.ResolvedMethod;
        }
      } else if (abstractTypeDefinition.IsAbstract) {
        IMethodReference methodReference = MemberHelper.GetImplicitlyOverriddenBaseClassMethod(methodDefinition);
        if (methodReference == Dummy.Method) return null;
        return methodReference.ResolvedMethod;
      }
      return null;
    }

    /// <summary>
    /// Returns the first method found in <paramref name="typeDefinition"/> containing an instance of 
    /// an attribute with the name "ContractInvariantMethodAttribute", if it exists.
    /// </summary>
    /// <param name="typeDefinition">The type whose members will be searched</param>
    /// <returns>May return null if not found</returns>
    public static IEnumerable<IMethodDefinition> GetInvariantMethods(ITypeDefinition typeDefinition) {
      foreach (IMethodDefinition methodDef in typeDefinition.Methods)
        foreach (var attr in methodDef.Attributes) {
          INamespaceTypeReference ntr = attr.Type as INamespaceTypeReference;
          if (ntr != null && ntr.Name.Value == "ContractInvariantMethodAttribute")
            yield return methodDef;
        }
    }

    /// <summary>
    /// Creates a type reference anchored in the given assembly reference and whose names are relative to the given host.
    /// When the type name has periods in it, a structured reference with nested namespaces is created.
    /// </summary>
    public static NamespaceTypeReference CreateTypeReference(IMetadataHost host, IAssemblyReference assemblyReference, string typeName) {
      IUnitNamespaceReference ns = new RootUnitNamespaceReference(assemblyReference);
      string[] names = typeName.Split('.');
      for (int i = 0, n = names.Length - 1; i < n; i++)
        ns = new NestedUnitNamespaceReference(ns, host.NameTable.GetNameFor(names[i]));
      return new NamespaceTypeReference(host, ns, host.NameTable.GetNameFor(names[names.Length - 1]), 0, false, false, PrimitiveTypeCode.NotPrimitive);
    }

    /// <summary>
    /// Returns true iff the type definition is a contract class for an interface or abstract class.
    /// </summary>
    public static bool IsContractClass(IMetadataHost host, ITypeDefinition typeDefinition) {
      if (contractClassFor == null) {
        contractClassFor = CreateTypeReference(host, new AssemblyReference(host, host.ContractAssemblySymbolicIdentity), "System.Diagnostics.Contracts.ContractClassForAttribute");
      }
      return AttributeHelper.Contains(typeDefinition.Attributes, contractClassFor);
    }
    private static INamespaceTypeReference contractClassFor = null;

    /// <summary>
    /// Returns true iff the method definition is an invariant method.
    /// </summary>
    public static bool IsInvariantMethod(IMetadataHost host, IMethodDefinition methodDefinition) {
      if (contractInvariantMethod == null) {
        contractInvariantMethod = CreateTypeReference(host, new AssemblyReference(host, host.ContractAssemblySymbolicIdentity), "System.Diagnostics.Contracts.ContractInvariantMethodAttribute");
      }
      return AttributeHelper.Contains(methodDefinition.Attributes, contractInvariantMethod);
    }
    private static INamespaceTypeReference contractInvariantMethod = null;

    /// <summary>
    /// Returns true iff the method resolves to a definition which is decorated
    /// with an attribute named either "ContractArgumentValidatorAttribute"
    /// or "ContractAbbreviatorAttribute".
    /// The namespace the attribute belongs to is ignored.
    /// </summary>
    public static bool IsValidatorOrAbbreviator(IMethodReference method) {
      IMethodDefinition methodDefinition = method.ResolvedMethod;
      if (methodDefinition == Dummy.Method) return false;
      foreach (var a in methodDefinition.Attributes) {
        string name = TypeHelper.GetTypeName(a.Type, NameFormattingOptions.None);
        if (name.EndsWith("ContractArgumentValidatorAttribute")
          || name.EndsWith("ContractAbbreviatorAttribute")
          )
          return true;
      }
      return false;
    }

    /// <summary>
    /// Indicates when the unit is marked with the assembly-level attribute
    /// [System.Diagnostics.Contracts.ContractReferenceAssembly]
    /// where that attribute type is defined in the unit itself.
    /// </summary>
    public static bool IsContractReferenceAssembly(IMetadataHost host, IUnit unit) {
      IAssemblyReference ar = unit as IAssemblyReference;
      if (ar == null) return false;
      var declAttr = CreateTypeReference(host, ar, "System.Diagnostics.Contracts.ContractReferenceAssemblyAttribute");
      return AttributeHelper.Contains(unit.Attributes, declAttr);
    }

    /// <summary>
    /// Returns a (possibly-null) method contract relative to a contract-aware host.
    /// If the method is instantiated or specialized, then the contract is looked
    /// for on the uninstantiated and unspecialized method.
    /// Note that this behavior is *not* necessarily present in any individual
    /// contract provider.
    /// However, if you already know which unit the method is defined in and/or
    /// already have the contract provider for the unit in which the method is
    /// defined, and you know the method is uninstantiated and unspecialized,
    /// then you would do just as well to directly query that contract provider.
    /// </summary>
    public static IMethodContract GetMethodContractFor(IContractAwareHost host, IMethodDefinition methodDefinition) {

      methodDefinition = UninstantiateAndUnspecialize(methodDefinition).ResolvedMethod;
      IUnit/*?*/ unit = TypeHelper.GetDefiningUnit(methodDefinition.ContainingType.ResolvedType);
      if (unit == null) return null;
      IContractProvider/*?*/ cp = host.GetContractExtractor(unit.UnitIdentity);
      if (cp == null) return null;
      var methodContract = cp.GetMethodContractFor(methodDefinition);
      return methodContract;
    }

    /// <summary>
    /// Returns a method contract containing the 'effective' contract for the given
    /// method definition. The effective contract contains all contracts for the method:
    /// any that it has on its own, as well as all those inherited from any methods
    /// that it overrides or interface methods that it implements (either implicitly
    /// or explicitly).
    /// All parameters in inherited contracts are substituted for by
    /// the method's own parameters.
    /// If there are no contracts, then it returns null.
    /// </summary>
    public static IMethodContract GetMethodContractForIncludingInheritedContracts(IContractAwareHost host, IMethodDefinition methodDefinition) {
      MethodContract cumulativeContract = new MethodContract();
      bool atLeastOneContract = false;
      IMethodContract/*?*/ mc = ContractHelper.GetMethodContractFor(host, methodDefinition);
      if (mc != null) {
        Microsoft.Cci.MutableContracts.ContractHelper.AddMethodContract(cumulativeContract, mc);
        atLeastOneContract = true;
      }
      #region Overrides of base class methods
      if (!methodDefinition.IsNewSlot) { // REVIEW: Is there a better test?
        IMethodDefinition overriddenMethod = MemberHelper.GetImplicitlyOverriddenBaseClassMethod(methodDefinition) as IMethodDefinition;
        while (overriddenMethod != null && overriddenMethod != Dummy.Method) {
          IMethodContract/*?*/ overriddenContract = ContractHelper.GetMethodContractFor(host, overriddenMethod);
          if (overriddenContract != null) {
            SubstituteParameters sps = new SubstituteParameters(host, methodDefinition, overriddenMethod);
            MethodContract newContract = sps.Visit(overriddenContract) as MethodContract;
            Microsoft.Cci.MutableContracts.ContractHelper.AddMethodContract(cumulativeContract, newContract);
            atLeastOneContract = true;
          }
          overriddenMethod = MemberHelper.GetImplicitlyOverriddenBaseClassMethod(overriddenMethod) as IMethodDefinition;
        }
      }
      #endregion Overrides of base class methods
      #region Implicit interface implementations
      foreach (IMethodDefinition ifaceMethod in MemberHelper.GetImplicitlyImplementedInterfaceMethods(methodDefinition)) {
        IMethodContract/*?*/ ifaceContract = ContractHelper.GetMethodContractFor(host, ifaceMethod);
        if (ifaceContract == null) continue;
        SubstituteParameters sps = new SubstituteParameters(host, methodDefinition, ifaceMethod);
        MethodContract newContract = sps.Visit(ifaceContract) as MethodContract;
        Microsoft.Cci.MutableContracts.ContractHelper.AddMethodContract(cumulativeContract, newContract);
        atLeastOneContract = true;
      }
      #endregion Implicit interface implementations
      #region Explicit interface implementations and explicit method overrides
      foreach (IMethodReference ifaceMethodRef in MemberHelper.GetExplicitlyOverriddenMethods(methodDefinition)) {
        IMethodDefinition/*?*/ ifaceMethod = ifaceMethodRef.ResolvedMethod;
        if (ifaceMethod == null) continue;
        IMethodContract/*?*/ ifaceContract = ContractHelper.GetMethodContractFor(host, ifaceMethod);
        if (ifaceContract == null) continue;
        SubstituteParameters sps = new SubstituteParameters(host, methodDefinition, ifaceMethod);
        MethodContract newContract = sps.Visit(ifaceContract) as MethodContract;
        Microsoft.Cci.MutableContracts.ContractHelper.AddMethodContract(cumulativeContract, newContract);
        atLeastOneContract = true;
      }
      #endregion Explicit interface implementations and explicit method overrides
      return atLeastOneContract ? cumulativeContract : null;
    }

    /// <summary>
    /// Given a mutable module that is a "declarative" module, i.e., it has contracts expressed as contract calls
    /// at the beginning of method bodies, this method will extract them, leaving the method bodies without those
    /// calls and return a contract provider for the module containing any extracted contracts.
    /// </summary>
    public static ContractProvider ExtractContracts(IMetadataHost host, Module module, ISourceLocationProvider/*?*/ sourceLocationProvider, ILocalScopeProvider/*?*/ localScopeProvider) {
      var contractMethods = new ContractMethods(host);
      var cp = new Microsoft.Cci.MutableContracts.ContractProvider(contractMethods, module);
      var extractor = new SeparateContractsFromCode(host, sourceLocationProvider, localScopeProvider, cp);
      module = extractor.Visit(module);
      return cp;
    }

    /// <summary>
    /// A mutator that extracts the contracts from each method.
    /// A client retrieves the extractor from the field after using
    /// this to visit a module.
    /// </summary>
    private class SeparateContractsFromCode : CodeMutator {

      private Microsoft.Cci.MutableContracts.ContractProvider contractProvider;
      ILocalScopeProvider/*?*/ localScopeProvider;

      internal SeparateContractsFromCode(
        IMetadataHost host,
        ISourceLocationProvider/*?*/ sourceLocationProvider,
        ILocalScopeProvider/*?*/ localScopeProvider,
        Microsoft.Cci.MutableContracts.ContractProvider contractProvider
        )
        : base(host, true, sourceLocationProvider) {
        this.localScopeProvider = localScopeProvider;
        this.contractProvider = contractProvider;
      }

      protected override void Visit(TypeDefinition typeDefinition) {
        var contract = ContractExtractor.GetObjectInvariant(this.host, typeDefinition, this.sourceLocationProvider, this.localScopeProvider);
        if (contract != null) {
          this.contractProvider.AssociateTypeWithContract(typeDefinition, contract);
        }
        base.Visit(typeDefinition);
      }

      /// <summary>
      /// For each method body, use the extractor to split it into the code
      /// and contract. Returns a new mutable source method body with the
      /// residual code.
      /// </summary>
      public override IMethodBody Visit(IMethodBody methodBody) {
        ISourceMethodBody sourceMethodBody = methodBody as ISourceMethodBody;
        if (sourceMethodBody != null) {
          var codeAndContractPair = ContractExtractor.SplitMethodBodyIntoContractAndCode(this.host, sourceMethodBody, this.sourceLocationProvider, this.localScopeProvider);
          this.contractProvider.AssociateMethodWithContract(methodBody.MethodDefinition, codeAndContractPair.MethodContract);
          var mutableSourceMethodBody = new SourceMethodBody(this.host, this.sourceLocationProvider);
          mutableSourceMethodBody.Block = codeAndContractPair.BlockStatement;
          mutableSourceMethodBody.MethodDefinition = methodBody.MethodDefinition;
          mutableSourceMethodBody.LocalsAreZeroed = methodBody.LocalsAreZeroed;
          return mutableSourceMethodBody;
        }
        return base.Visit(methodBody);
      }

    }

  }

  /// <summary>
  /// A mutator that substitutes parameters defined in one method with those from another method.
  /// </summary>
  public sealed class SubstituteParameters : MethodBodyCodeAndContractMutator {
    private IMethodDefinition targetMethod;
    private IMethodDefinition sourceMethod;
    private ITypeReference targetType;
    List<IParameterDefinition> parameters;

    /// <summary>
    /// Creates a mutator that replaces all occurrences of parameters from the target method with those from the source method.
    /// </summary>
    public SubstituteParameters(IMetadataHost host, IMethodDefinition targetMethodDefinition, IMethodDefinition sourceMethodDefinition)
      : base(host, false) { // NB: Important to pass "false": this mutator needs to make a copy of the entire contract!
      this.targetMethod = targetMethodDefinition;
      this.sourceMethod = sourceMethodDefinition;
      this.targetType = targetMethodDefinition.ContainingType;
      this.parameters = new List<IParameterDefinition>(targetMethod.Parameters);
    }

    /// <summary>
    /// If the <paramref name="boundExpression"/> represents a parameter of the source method,
    /// it is replaced with the equivalent parameter of the target method.
    /// </summary>
    /// <param name="boundExpression">The bound expression.</param>
    public override IExpression Visit(BoundExpression boundExpression) {
      ParameterDefinition/*?*/ par = boundExpression.Definition as ParameterDefinition;
      if (par != null && par.ContainingSignature == this.sourceMethod) {
        boundExpression.Definition = this.parameters[par.Index];
        return boundExpression;
      } else {
        return base.Visit(boundExpression);
      }
    }

    /// <summary>
    /// Replaces the specified this reference with a this reference to the containing type of the target method
    /// </summary>
    /// <param name="thisReference">The this reference.</param>
    /// <returns>a this reference to the containing type of the target method</returns>
    public override IExpression Visit(ThisReference thisReference) {
      return new ThisReference() {
        Type = this.targetType,
      };
    }
  }

  internal class SimpleHostEnvironment : MetadataReaderHost, IContractAwareHost {
    PeReader peReader;
    public SimpleHostEnvironment(INameTable nameTable)
      : base(nameTable, 4) {
      this.peReader = new PeReader(this);
    }

    public override IUnit LoadUnitFrom(string location) {
      IUnit result = this.peReader.OpenModule(BinaryDocument.GetBinaryDocumentForFile(location, this));
      this.RegisterAsLatest(result);
      return result;
    }


    #region IContractAwareHost Members

    public IContractExtractor GetContractExtractor(UnitIdentity unitIdentity) {
      throw new NotImplementedException();
    }

    #endregion
  }

  /// <summary>
  /// An IContractAwareHost which automatically loads reference assemblies and attaches
  /// a (code-contract aware, aggregating) lazy contract provider to each unit it loads.
  /// This host supports re-loading units that have been updated: it reloads any units
  /// that had been dependent on the updated unit. Clients must re-resolve references
  /// in order to see the updated contents of the units.
  /// </summary>
  public class CodeContractAwareHostEnvironment : MetadataReaderHost, IContractAwareHost {
    PeReader peReader;
    readonly List<string> libPaths = new List<string>();
    internal Dictionary<UnitIdentity, IContractExtractor> unit2ContractExtractor = new Dictionary<UnitIdentity, IContractExtractor>();
    List<IContractProviderCallback> callbacks = new List<IContractProviderCallback>();
    private List<IMethodDefinition> methodsBeingExtracted = new List<IMethodDefinition>();

    // These next two tables must be kept in sync.
    private Dictionary<string, DateTime> location2LastModificationTime = new Dictionary<string, DateTime>();
    private Dictionary<string, IUnit> location2Unit = new Dictionary<string, IUnit>();

    // A table that maps each unit, U, to all of the units that have a reference to U.
    private Dictionary<IUnitReference, List<IUnitReference>> unit2DependentUnits = new Dictionary<IUnitReference, List<IUnitReference>>();
    // A table that maps each unit, U, to all of the reference assemblies for U.
    private Dictionary<IUnitReference, List<IUnitReference>> unit2ReferenceAssemblies = new Dictionary<IUnitReference, List<IUnitReference>>();

    #region Constructors
    /// <summary>
    /// Allocates an object that can be used as an IMetadataHost which automatically loads reference assemblies and attaches
    /// a (lazy) contract provider to each unit it loads.
    /// </summary>
    public CodeContractAwareHostEnvironment()
      : this(new NameTable(), 0, true) {
    }

    /// <summary>
    /// Allocates an object that can be used as an IMetadataHost which automatically loads reference assemblies and attaches
    /// a (lazy) contract provider to each unit it loads.
    /// </summary>
    /// <param name="loadPDBs">Whether PDB files should be loaded by the extractors attached to each unit.</param>
    public CodeContractAwareHostEnvironment(bool loadPDBs)
      : this(new NameTable(), 0, loadPDBs) {
    }

    /// <summary>
    /// Allocates an object that can be used as an IMetadataHost which automatically loads reference assemblies and attaches
    /// a (lazy) contract provider to each unit it loads.
    /// </summary>
    /// <param name="searchPaths">
    /// Initial value for the set of search paths to use.
    /// </param>
    public CodeContractAwareHostEnvironment(IEnumerable<string> searchPaths)
      : this(searchPaths, false, true) {
    }

    /// <summary>
    /// Allocates an object that can be used as an IMetadataHost which automatically loads reference assemblies and attaches
    /// a (lazy) contract provider to each unit it loads.
    /// </summary>
    /// <param name="searchPaths">
    /// Initial value for the set of search paths to use.
    /// </param>
    /// <param name="searchInGAC">
    /// Whether the GAC (Global Assembly Cache) should be searched when resolving references.
    /// </param>
    public CodeContractAwareHostEnvironment(IEnumerable<string> searchPaths, bool searchInGAC)
      : this(searchPaths, searchInGAC, true) {
    }

    /// <summary>
    /// Allocates an object that can be used as an IMetadataHost which automatically loads reference assemblies and attaches
    /// a (lazy) contract provider to each unit it loads.
    /// </summary>
    /// <param name="searchPaths">
    /// Initial value for the set of search paths to use.
    /// </param>
    /// <param name="searchInGAC">
    /// Whether the GAC (Global Assembly Cache) should be searched when resolving references.
    /// </param>
    /// <param name="loadPDBs">Whether PDB files should be loaded by the extractors attached to each unit.</param>
    public CodeContractAwareHostEnvironment(IEnumerable<string> searchPaths, bool searchInGAC, bool loadPDBs)
      : base(searchPaths, searchInGAC) {
      this.peReader = new PeReader(this);
      this.AllowExtractorsToUsePdbs = loadPDBs;
    }

    /// <summary>
    /// Allocates an object that provides an abstraction over the application hosting compilers based on this framework.
    /// </summary>
    /// <param name="nameTable">
    /// A collection of IName instances that represent names that are commonly used during compilation.
    /// This is a provided as a parameter to the host environment in order to allow more than one host
    /// environment to co-exist while agreeing on how to map strings to IName instances.
    /// </param>
    public CodeContractAwareHostEnvironment(INameTable nameTable)
      : this(nameTable, 0, true) {
    }

    /// <summary>
    /// Allocates an object that provides an abstraction over the application hosting compilers based on this framework.
    /// </summary>
    /// <param name="nameTable">
    /// A collection of IName instances that represent names that are commonly used during compilation.
    /// This is a provided as a parameter to the host environment in order to allow more than one host
    /// environment to co-exist while agreeing on how to map strings to IName instances.
    /// </param>
    /// <param name="pointerSize">The size of a pointer on the runtime that is the target of the metadata units to be loaded
    /// into this metadta host. This parameter only matters if the host application wants to work out what the exact layout
    /// of a struct will be on the target runtime. The framework uses this value in methods such as TypeHelper.SizeOfType and
    /// TypeHelper.TypeAlignment. If the host application does not care about the pointer size it can provide 0 as the value
    /// of this parameter. In that case, the first reference to IMetadataHost.PointerSize will probe the list of loaded assemblies
    /// to find an assembly that either requires 32 bit pointers or 64 bit pointers. If no such assembly is found, the default is 32 bit pointers.
    /// </param>
    /// <param name="loadPDBs">Whether PDB files should be loaded by the extractors attached to each unit.</param>
    public CodeContractAwareHostEnvironment(INameTable nameTable, byte pointerSize, bool loadPDBs)
      : base(nameTable, pointerSize)
      //^ requires pointerSize == 0 || pointerSize == 4 || pointerSize == 8;
    {
      this.peReader = new PeReader(this);
      this.AllowExtractorsToUsePdbs = loadPDBs;
    }
    #endregion Constructors

    /// <summary>
    /// Set this before loading any units with this host. Default is true.
    /// Note that extractors may use PDB file to open source files.
    /// Both PDB and source files may be opened with exclusive access.
    /// </summary>
    public virtual bool AllowExtractorsToUsePdbs { get; protected set; }

    /// <summary>
    /// Returns the unit that is stored at the given location, or a dummy unit if no unit exists at that location or if the unit at that location is not accessible.
    /// </summary>
    public override IUnit LoadUnitFrom(string location) {
      if (location.StartsWith("file://")) { // then it is a URL
        try {
          Uri u = new Uri(location, UriKind.Absolute); // Let the Uri class figure out how to go from URL to local file path
          location = u.LocalPath;
        } catch (UriFormatException) {
          return Dummy.Unit;
        }
      }

      var timeOfLastModification = UnloadPreviouslyLoadedUnitIfLocationIsNewer(location);
      IUnit result = this.peReader.OpenModule(BinaryDocument.GetBinaryDocumentForFile(Path.GetFullPath(location), this));

      this.RegisterAsLatest(result);

      foreach (var d in result.UnitReferences) {
        if (!this.unit2DependentUnits.ContainsKey(d)) {
          this.unit2DependentUnits[d] = new List<IUnitReference>();
        }
        this.unit2DependentUnits[d].Add(result);
      }

      this.AttachContractExtractorAndLoadReferenceAssembliesFor(result);
      this.location2LastModificationTime[location] = timeOfLastModification;
      this.location2Unit[location] = result;
      return result;
    }

    /// <summary>
    /// Checks the location against all previously loaded locations. If a unit
    /// had been loaded from it before and if the last write time is newer, then
    /// the previously loaded unit, all units dependent on it, and all reference
    /// assemblies for it are deleted from the cache and a new PeReader is created
    /// for reading in all future units.
    /// </summary>
    /// <returns>
    /// the last write time of the file found at location.
    /// </returns>
    private DateTime UnloadPreviouslyLoadedUnitIfLocationIsNewer(string location) {
      var timeOfLastModification = File.GetLastWriteTime(location);
      DateTime previousTime;
      if (this.location2LastModificationTime.TryGetValue(location, out previousTime)) {
        if (previousTime.CompareTo(timeOfLastModification) < 0) {
          // file has been modified. Need to throw away PeReader because it caches based on identity and
          // won't actually read in and construct an object model from the file. Even though at this
          // point we don't even know what assembly is at this location. Maybe it isn't even an updated
          // version, but instead some completely different assembly?
          this.peReader = new PeReader(this);

          var unit = this.location2Unit[location];

          // Need to dump all units that depended on this one because their reference is now stale.
          List<IUnitReference> referencesToDependentUnits;
          if (this.unit2DependentUnits.TryGetValue(unit, out referencesToDependentUnits)) {
            foreach (var d in referencesToDependentUnits) {
              this.RemoveUnit(d.UnitIdentity);
            }
          }

          // Need to remove unit from the list of dependent units for each unit
          // it *was* dependent on in case the newer version has a changed list of
          // dependencies.
          foreach (var d in unit.UnitReferences) {
            this.unit2DependentUnits[d].Remove(unit);
          }

          // Dump all reference assemblies for this unit.
          List<IUnitReference> referenceAssemblies;
          if (this.unit2ReferenceAssemblies.TryGetValue(unit, out referenceAssemblies)) {
            foreach (var d in referenceAssemblies) {
              this.RemoveUnit(d.UnitIdentity);
            }
          }

          // Dump stale version from cache
          this.RemoveUnit(unit.UnitIdentity);
        }
      }
      return timeOfLastModification;
    }

    /// <summary>
    /// If the unit is a reference assembly, then just attach a contract extractor to it.
    /// Otherwise, create an aggregating extractor that encapsulates the unit and any
    /// reference assemblies that are found on the search path.
    /// Each contract extractor is actually a composite comprising a code-contracts
    /// extractor layered on top of a lazy extractor.
    /// </summary>
    private void AttachContractExtractorAndLoadReferenceAssembliesFor(IUnit alreadyLoadedUnit) {

      // Because of unification, the "alreadyLoadedUnit" might have actually already been loaded previously
      // and gone through here (and so already has a contract provider attached to it).
      if (this.unit2ContractExtractor.ContainsKey(alreadyLoadedUnit.UnitIdentity))
        this.unit2ContractExtractor.Remove(alreadyLoadedUnit.UnitIdentity);
      //return;

      var contractMethods = new ContractMethods(this);
      using (var lazyContractProviderForLoadedUnit = new LazyContractExtractor(this, alreadyLoadedUnit, contractMethods, this.AllowExtractorsToUsePdbs)) {
        var contractProviderForLoadedUnit = new CodeContractsContractExtractor(this, lazyContractProviderForLoadedUnit);
        if (ContractHelper.IsContractReferenceAssembly(this, alreadyLoadedUnit)) {
          // If we're asked to explicitly load a reference assembly, then go ahead and attach a contract provider to it,
          // but *don't* look for reference assemblies for *it*.
          this.unit2ContractExtractor.Add(alreadyLoadedUnit.UnitIdentity, contractProviderForLoadedUnit);
        } else {
          #region Load any reference assemblies for the loaded unit
          var oobProvidersAndHosts = new List<KeyValuePair<IContractProvider, IMetadataHost>>();
          var loadedAssembly = alreadyLoadedUnit as IAssembly; // Only assemblies can have associated reference assemblies.
          if (loadedAssembly != null) {
            var refAssemWithoutLocation = new AssemblyIdentity(this.NameTable.GetNameFor(alreadyLoadedUnit.Name.Value + ".Contracts"), loadedAssembly.AssemblyIdentity.Culture, loadedAssembly.AssemblyIdentity.Version, loadedAssembly.AssemblyIdentity.PublicKeyToken, "");
            var referenceAssemblyIdentity = this.ProbeAssemblyReference(alreadyLoadedUnit, refAssemWithoutLocation);
            if (referenceAssemblyIdentity.Location != "unknown://location") {
              #region Load reference assembly and attach a contract provider to it
              IContractAwareHost hostForReferenceAssembly = this; // default
              IUnit referenceUnit = null;
              if (loadedAssembly.AssemblyIdentity.Equals(this.CoreAssemblySymbolicIdentity)) {
                // Need to use a separate host because the reference assembly for the core assembly thinks *it* is the core assembly
                var separateHost = new SimpleHostEnvironment(this.NameTable);
                referenceUnit = separateHost.LoadUnitFrom(referenceAssemblyIdentity.Location);
                hostForReferenceAssembly = separateHost;
              } else {
                // Load reference assembly, but don't cause a recursive call!! So don't call LoadUnit or LoadUnitFrom
                referenceUnit = this.peReader.OpenModule(BinaryDocument.GetBinaryDocumentForFile(referenceAssemblyIdentity.Location, this));
                this.RegisterAsLatest(referenceUnit);
              }
              if (referenceUnit != null && referenceUnit != Dummy.Unit) {
                IAssembly referenceAssembly = referenceUnit as IAssembly;
                if (referenceAssembly != null) {
                  var referenceAssemblyContractProvider = new CodeContractsContractExtractor(hostForReferenceAssembly,
                    new LazyContractExtractor(hostForReferenceAssembly, referenceAssembly, contractMethods, this.AllowExtractorsToUsePdbs));
                  oobProvidersAndHosts.Add(new KeyValuePair<IContractProvider, IMetadataHost>(referenceAssemblyContractProvider, hostForReferenceAssembly));
                  if (!this.unit2ReferenceAssemblies.ContainsKey(alreadyLoadedUnit)) {
                    this.unit2ReferenceAssemblies[alreadyLoadedUnit] = new List<IUnitReference>();
                  }
                  // Reference assemblies don't have references to the real assembly but they are "dependent"
                  // on them in that they should be dumped and reloaded if the real assembly changes.
                  this.unit2ReferenceAssemblies[alreadyLoadedUnit].Add(referenceAssembly);
                }
              }
              #endregion Load reference assembly and attach a contract provider to it
            }
          }
          var aggregateContractProvider = new AggregatingContractExtractor(this, contractProviderForLoadedUnit, oobProvidersAndHosts);
          this.unit2ContractExtractor.Add(alreadyLoadedUnit.UnitIdentity, aggregateContractProvider);
          #endregion Load any reference assemblies for the loaded unit
        }
        foreach (var c in this.callbacks) {
          contractProviderForLoadedUnit.RegisterContractProviderCallback(c);
        }
      }
    }

    #region IContractAwareHost Members

    /// <summary>
    /// If a unit has been loaded with this host, then it will have attached a (lazy) contract provider to that unit.
    /// This method returns that contract provider. If the unit has not been loaded by this host, then null is returned.
    /// </summary>
    public IContractExtractor/*?*/ GetContractExtractor(UnitIdentity unitIdentity) {
      IContractExtractor cp;
      if (this.unit2ContractExtractor.TryGetValue(unitIdentity, out cp)) {
        return cp;
      } else {
        return null;
      }
    }

    /// <summary>
    /// The host will register this callback with each contract provider it creates.
    /// </summary>
    /// <param name="contractProviderCallback"></param>
    public void RegisterContractProviderCallback(IContractProviderCallback contractProviderCallback) {
      this.callbacks.Add(contractProviderCallback);
    }

    #endregion
  }

}
//-----------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All Rights Reserved.
//
//-----------------------------------------------------------------------------
namespace Microsoft.Cci.CSharp
{
	internal enum Error
	{
		None = 0,
		AbstractAndExtern = 180,
		AbstractAndSealed = 502,
		AbstractAttributeClass = 653,
		AbstractBaseCall = 205,
		AbstractEventInitializer = 74,
		AbstractHasBody = 500,
		AbstractInConcreteClass = 513,
		AbstractNotVirtual = 503,
		AbstractSealedStatic = 418,
		AddModuleAssembly = 1542,
		AddOrRemoveExpected = 1055,
		AddRemoveMustHaveBody = 73,
		AliasNotFound = 432,
		AlwaysNull = 458,
		AmbigBinaryOps = 34,
		AmbigCall = 121,
		AmbigContext = 104,
		AmbigQM = 172,
		AmbiguousAttribute = 1614,
		AnonMethNotAllowed = 1706,
		ArrayElementCantBeRefAny = 611,
		ArrayInitInBadPlace = 623,
		ArrayInitToNonArrayType = 622,
		ArrayOfStaticClass = 719,
		AsMustHaveReferenceType = 77,
		AssgLvalueExpected = 131,
		AssgReadonly = 191,
		AssgReadonlyLocal = 1604,
		AssgReadonlyLocalCause = 1656,
		AssgReadonlyProp = 200,
		AssgReadonlyStatic = 198,
		AttributeLocationOnBadDeclaration = 657,
		AttributeOnBadSymbolType = 592,
		AttributeUsageOnNonAttributeClass = 641,
		AutoResGen = 1567,
		BadAccess = 122,
		BadArgCount = 1501,
		BadArgExtraRef = 1615,
		BadArgumentToNameAttribute = 633,
		BadArity2 = 305,
		BadArraySyntax = 1552,
		BadAttributeParam = 182,
		BadBaseType = 1521,
		BadBinaryOps = 19,
		BadBinaryOperatorSignature = 563,
		BadBoolOp = 217,
		BadCastInFixed = 254,
		BadCodePage = 2021,
		BadDelArgCount = 1593,
		BadDelegateLeave = 1632,
		BadDirectivePlacement = 1040,
		BadEmbeddedStmt = 1023,
		BadEmptyThrow = 156,
		BadEmptyThrowInFinally = 724,
		BadEventUsage = 70,
		BadEventUsageNoField = 79,
		BadExceptionType = 155,
		BadFinallyLeave = 157,
		BadFixedInitType = 209,
		BadForeachDecl = 230,
		BadGetEnumerator = 202,
		BadIncDecSignature = 559,
		BadIndexCount = 22,
		BadIteratorReturn = 1624,
		BadModifierLocation = 1585,
		BadModifiersOnNamespace = 1671,
		BadNamedAttributeArgument = 617,
		BadNamedAttributeArgumentType = 655,
		BadNewExpr = 1526,
		BadOperatorSyntax = 1553,
		BadOperatorSyntax2 = 1554,
		BadProtectedAccess = 1540,
		BadRefCompareLeft = 252,
		BadRefCompareRight = 253,
		BadSKknown = 118,
		BadSKunknown = 119,
		BadStackAllocExpr = 1575,
		BadTokenInType = 1518,
		BadTypeReference = 572,
		BadVarDecl = 1528,
		BadVisBaseClass = 60,
		BadVisBaseInterface = 61,
		BadVisDelegateParam = 59,
		BadVisDelegateReturn = 58,
		BadVisFieldType = 52,
		BadVisIndexerParam = 55,
		BadVisIndexerReturn = 54,
		BadVisOpParam = 57,
		BadVisOpReturn = 56,
		BadVisParamType = 51,
		BadVisPropertyType = 53,
		BadVisReturnType = 50,
		BadUnaryOp = 23,
		BadUnaryOperatorSignature = 562,
		BadUseOfMethod = 654,
		BadWin32Res = 1583,
		BaseIllegal = 175,
		BaseInBadContext = 1512,
		BadIndexLHS = 21,
		BaseInStaticMeth = 1511,
		BatchFileNotRead = 2003,
		BitwiseOrSignExtend = 675,
		CallingBaseFinalizeDeprecated = 250,
		CallingFinalizeDeprecated = 245,
		CannotMarkOverrideMethodNewOrVirtual = 113,
		CantCallSpecialMethod = 571,
		CantChangeAccessOnOverride = 507,
		CantChangeReturnTypeOnOverride = 508,
		CantConvAnonMethParams = 1661,
		CantConvAnonMethNoParams = 1688,
		CantConvAnonMethReturns = 1662,
		CantDeriveFromSealedClass = 509,
		CantInferMethTypeArgs = 411,
		CantOverrideAccessor = 560,
		CantOverrideNonEvent = 72,
		CantOverrideNonFunction = 505,
		CantOverrideNonProperty = 544,
		CantOverrideNonVirtual = 506,
		CantOverrideSealed = 239,
		CantOverrideSpecialMethod = 561,
		CaseFallThrough = 163,
		CheckedOverflow = 220,
		CircConstValue = 110,
		CircularBase = 146,
		ClassDoesntImplementInterface = 540,
		CloseUnimplementedInterfaceMember = 536,
		CLSNotOnModules = 3012,
		ColColWithTypeAlias = 431,
		ComImportWithoutUuidAttribute = 596,
		ConcreteMissingBody = 501,
		ConditionalMustReturnVoid = 578,
		ConditionalOnInterfaceMethod = 582,
		ConditionalOnOverride = 243,
		ConditionalOnSpecialMethod = 577,
		ConflictAliasAndMember = 576,
		ConflictingProtectionModifier = 107,
		ConstantExpected = 150,
		ConstOutOfRange = 31,
		ConstOutOfRangeChecked = 221,
		ConstraintIsStaticClass = 717,
		ConstructorInStaticClass = 710,
		ConstValueRequired = 145,
		ConversionNotInvolvingContainedType = 556,
		ConversionWithBase = 553,
		ConversionWithDerived = 554,
		ConversionWithInterface = 552,
		ConvertToStaticClass = 716,
		CryptoFailed = 1548,
		CStyleArray = 650,
		CustomAttributeError = 647,
		CycleInInterfaceInheritance = 529,
		DebugInitFile = 42,
		DeprecatedSymbol = 612,
		DeprecatedSymbolError = 613,
		DeprecatedSymbolStr = 618,
		DeprecatedSymbolStrError = 619,
		DeriveFromEnumOrValueType = 644,
		DestructorInStaticClass = 711,
		DllImportOnInvalidMethod = 601,
		DontUseInvoke = 1533,
		DottedTypeNameNotFoundInAgg = 426,
		DuplicateAccessor = 1007,
		DuplicateAlias = 1537,
		DuplicateAttribute = 579,
		DuplicateCaseLabel = 152,
		DuplicateConversionInClass = 557,
		DuplicateInterfaceInBaseList = 528,
		DuplicateLabel = 140,
		DuplicateModifier = 1004,
		DuplicateNamedAttributeArgument = 643,
		DuplicateNameInClass = 102,
		DuplicateNameInNS = 101,
		DuplicateParamName = 100,
		DuplicateResponseFile = 2014,
		DuplicateUsing = 105,
		EmptyCharConst = 1011,
		EmptySwitch = 1522,
		EndifDirectiveExpected = 1027,
		EndOfPPLineExpected = 1025,
		EndRegionDirectiveExpected = 1038,
		EnumeratorOverflow = 543,
		EOFExpected = 1022,
		EqualityOpWithoutEquals = 660,
		EqualityOpWithoutGetHashCode = 661,
		ErrorDirective = 1029,
		EventNeedsBothAccessors = 65,
		EventNotDelegate = 66,
		EventPropertyInInterface = 69,
		ExpectedEndTry = 1524,
		ExpectedIdentifier = 1001,
		ExpectedLeftBrace = 1514,
		ExpectedRightBrace = 1513,
		ExpectedRightParenthesis = 1026,
		ExpectedSemicolon = 1002,
		ExplicitEventFieldImpl = 71,
		ExplicitInterfaceImplementationInNonClassOrStruct = 541,
		ExplicitInterfaceImplementationNotInterface = 538,
		ExplicitMethodImplAccessor = 683,
		ExplicitParamArray = 674,
		ExplicitPropertyAddingAccessor = 550,
		ExplicitPropertyMissingAccessor = 551,
		ExternAfterElements = 439,
		ExternHasBody = 179,
		ExternMethodNoImplementation = 626,
		FeatureNYI2 = 189,
		FieldInitRefNonstatic = 236,
		FieldInitializerInStruct = 573,
		FixedMustInit = 210,
		FixedNeeded = 212,
		FixedNotNeeded = 213,
		FloatOverflow = 594,
		ForEachMissingMember = 1579,
		GenericArgIsStaticClass = 718,
		GenericConstraintNotSatisfied = 309,
		GetOrSetExpected = 1014,
		GlobalSingleTypeNameNotFound = 400,
		HasNoTypeVars = 308,
		HidingAbstractMethod = 533,
		IdentityConversion = 555,
		IllegalEscape = 1009,
		IllegalPointerType = 1005,
    IllegalPPWarning = 1634,
    IllegalPragma = 1633,
		IllegalQualifiedNamespace = 134,
		IllegalStatement = 201,
		IllegalUnsafe = 227,
		ImportNonAssembly = 1509,
		InconsistantIndexerNames = 668,
		IdentifierExpectedKW = 1041,
		IndexerInStaticClass = 720,
		IndexerNeedsParam = 1551,
		IndexerWithRefParam = 631,
		InExpected = 1515,
		InstanceMemberInStaticClass = 708,
		InstantiatingStaticClass = 712,
		IntDivByZero = 20,
		IntegralTypeExpected = 1008,
		IntegralTypeValueExpected = 151,
		InterfaceEventInitializer = 68,
		InterfaceImplementedByConditional = 629,
		InterfaceMemberHasBody = 531,
		InterfaceMemberNotFound = 539,
		InterfacesCantContainOperators = 567,
		InterfacesCannotContainConstructors = 526,
		InterfacesCannotContainFields = 525,
		InterfacesCannotContainTypes = 524,
		InternalCompilerError = 1,
		IntOverflow = 1021,
    InvalidAddrOp=211,
    InvalidArglistConstructContext=190,
		InvalidArray = 178,
		InvalidAttributeArgument = 591,
		InvalidAttributeLocation = 658,
		InvalidCall = 123,
		InvalidGotoCase = 153,
		InvalidExprTerm = 1525,
		InvalidLineNumber = 1576,
		InvalidMainSig = 28,
		InvalidMemberDecl = 1519,
		InvalidModifier = 106,
    InvalidNumber = 1013,
		InvalidPreprocExpr = 1517,
		InvalidQM = 173,
		IsAlwaysFalse = 184,
		IsAlwaysTrue = 183,
		IsBinaryFile = 2015,
		LabelNotFound = 159,
		LiteralDoubleCast = 664,
		LocalDuplicate = 128,
		LocalShadowsOuterDeclaration = 136,
		LockNeedsReference = 185,
		LowercaseEllSuffix = 78,
		MainCantBeGeneric = 402,
		ManagedAddr = 208,
		MemberAlreadyExists = 111,
		MemberNameSameAsType = 542,
		MemberNeedsType = 1520,
		MethodArgCantBeRefAny = 1601,
		MethodNameExpected = 149,
		MethodReturnCantBeRefAny = 1564,
		MissingArraySize = 1586,
		MissingPartial = 260,
		MissingPPFile = 1578,
		MissingStructOffset = 625,
		MultipleEntryPoints = 17,
		MultipleTypeDefs = 1595,
		MultiTypeInDeclaration = 1044,
		MustHaveOpTF = 218,
		NamedArgumentExpected = 1016,
		NamespaceUnexpected = 116,
		NameAttributeOnOverride = 609,
		NameNotInContext = 103,
		NegativeArraySize = 248,
		NegativeStackAllocSize = 247,
		NewBoundMustBeLast = 401,
		NewConstraintNotSatisfied = 310,
		NewlineInConst = 1010,
		NewNotRequired = 109,
		NewOnNamespaceElement = 1530,
		NewOrOverrideExpected = 114,
		NewRequired = 108,
		NewVirtualInSealed = 549,
		NoArglistInIndexers = 237,
		NoArglistInDelegates = 235,
		NoBreakOrCont = 139,
		NoConstructors = 143,
		NoCommentEnd = 1035,
		NoDefaultArgs = 241,
		NoEntryPoint = 5001,
		NoExplicitBuiltinConv = 39,
		NoExplicitConversion = 30,
		NoGetToOverride = 545,
		NoImplicitConversion = 29,
		NoImplicitConvCast = 266,
		NoModifiersOnAccessor = 1609,
		NoNewAbstract = 144,
		NoSetToOverride = 546,
		NoSources = 2008,
		NoSuchFile = 2005,
		NoSuchMember = 117,
		NoSuchOperator = 187,
		NotAnAttributeClass = 616,
		NotConstantExpression = 133,
		NoVoidHere = 1547,
		NoVoidParameter = 1536,
		NonInterfaceInInterfaceList = 527,
		NonObsoleteOverridingObsolete = 672,
		NullNotValid = 186,
		ObjectProhibited = 176,
		ObjectRequired = 120,
		OnlyClassesCanContainDestructors = 575,
		OperatorCantReturnVoid = 590,
		OperatorInStaticClass = 715,
		OperatorNeedsMatch = 216,
		OperatorsMustBeStatic = 558,
		OpTFRetType = 215,
		OutputWriteFailed = 16,
		OverloadRefOut = 663,
		OverrideFinalizeDeprecated = 249,
		OverrideNotExpected = 115,
		OvlBinaryOperatorExpected = 1020,
		OvlUnaryOperatorExpected = 1019,
		ParameterIsStaticClass = 721,
		ParamsCantBeRefOut = 1611,
		ParamsMustBeArray = 225,
		ParamsOrVarargsMustBeLast = 231,
		ParamUnassigned = 0177,
		PartialMisplaced = 267,
		PartialModifierConflict = 262,
		PartialMultipleBases = 263,
		PartialTypeKindConflict = 261,
		PointerInAsOrIs = 244,
		PtrIndexSingle = 196,
		PossibleBadNegCast = 75,
		PossibleMistakenNullStatement = 642,
		PPDefFollowsToken = 1032,
		PPDirectiveExpected = 1024,
		PrivateOrProtectedNamespaceElement = 1527,
		PropertyCantHaveVoidType = 547,
		PropertyLacksGet = 154,
		PropertyWithNoAccessors = 548,
		ProtectedInSealed = 628,
		ProtectedInStruct = 666,
		PtrExpected = 193,
		RecursiveConstructorCall = 516,
		RefConstraintNotSatisfied = 452,
		RefLvalueExpected = 1510,
		RefReadonly = 192,
		RefReadonlyLocal = 1605,
		RefReadonlyLocalCause = 1657,
		RefReadonlyProperty = 206,
		RefReadonlyStatic = 199,
		RefValBoundMustBeFirst = 449,
		RelatedErrorLocation = 10002,
		RelatedErrorModule = 10003,
		RelatedWarningLocation = 10004,
		RelatedWarningModule = 10005,
		RetNoObjectRequired = 127,
		RetObjectRequired = 126,
		ReturnExpected = 0161,
		ReturnNotLValue = 1612,
		ReturnTypeIsStaticClass = 722,
		SealedNonOverride = 238,
		SealedStaticClass = 441,
		SingleTypeNameNotFound = 246,
		SizeofUnsafe = 233,
		SourceFileNotRead = 2001,
		StackallocInCatchFinally = 255,
		StaticBaseClass = 709,
		StaticClassInterfaceImpl = 714,
		StaticConstant = 504,
		StaticConstParam = 132,
		StaticConstructorWithAccessModifiers = 515,
		StaticConstructorWithExplicitConstructorCall = 514,
		StaticDerivedFromNonObject = 713,
		StaticNotVirtual = 112,
		StmtNotInCase = 1523,
		StructLayoutCycle = 523,
		StructOffsetOnBadStruct = 636,
		StructOffsetOnBadField = 637,
		StructsCantContainDefaultContructor = 568,
		StructWithBaseConstructorCall = 522,
		SyntaxError = 1003,
		ThisInBadContext = 27,
		ThisInStaticMeth = 26,
		ThisOrBaseExpected = 1018,
		TooManyArgumentsToAttribute = 580,
		TooManyCatches = 1017,
		TooManyCharsInConst = 1012,
		TypeArgsNotAllowed = 307,
		TypeExpected = 1031,
		TypeNameNotFound = 234,
    TyVarNotFoundInConstraint = 699,
		UnassignedThis = 171,
		UnexpectedDirective = 1028,
		UnimplementedAbstractMethod = 534,
		UnimplementedInterfaceMember = 535,
		UnknownOption = 5000,
		UnreachableCatch = 160,
		UnreachableCode = 162,
		UnreferencedLabel = 164,
		UnreferencedVar = 0168,
		UnreferencedVarAssg = 219,
		UnsafeNeeded = 214,
		UseDefViolation = 165,
		UseDefViolationField = 170, // Not enforced yet.
		UseDefViolationOut = 269,   // partially enforced. ( only usedef, no must-assign-before-exit)
		UseDefViolationThis = 188,  // Not enforced yet.
		UseSwitchInsteadOfAttribute = 1699,
		UsingAfterElements = 1529,
		VacuousIntegralComp = 652,
		ValueCantBeNull = 37,
		ValConstraintNotSatisfied = 453,
		VarDeclIsStaticClass = 723,
		VirtualPrivate = 621,
		VoidError = 242,
		VolatileAndReadonly = 678,
		VolatileByRef = 420,
		VolatileStruct = 677,
		WarningDirective = 1030,
		Win32ResourceFileNotRead = 2002,
		WrongNameForDestructor = 574,
		WrongNestedThis = 38,
		WrongParsForBinOp = 1534,
		WrongParsForUnaryOp = 1535,

		InvalidAssemblyName = 1700,
		UnifyReferenceMajMin = 1701,
		UnifyReferenceBldRev = 1702,
		DuplicateImport = 1703,
		DuplicateImportSimple = 1704,
		AssemblyMatchBadVersion = 1705,
		DelegateNewMethBind = 1707,
		FixedNeedsLvalue = 1708,
		EmptyFileName = 1709,
		DuplicateTypeParamTag = 1710,
		UnmatchedTypeParamTag = 1711,
		MissingTypeParamTag = 1712,
		TypeNameBuilderError = 1713,
		ImportBadBase = 1714,
		CantChangeTypeOnOverride = 1715,
		DoNotUseFixedBufferAttr = 1716,
		AssignmentToSelf = 1717,
		ComparisonToSelf = 1718,
		CantOpenWin32Res = 1719,
		DotOnDefault = 1720,
		NoMultipleInheritance = 1721,
		BaseClassMustBeFirst = 1722,
		BadXMLRefTypeVar = 1723,
		InvalidDefaultCharSetValue = 1724,
		FriendAssemblyBadArgs = 1725,
		FriendAssemblySNReq = 1726,

		BadEscapeSequence = 4000,
		BadHexDigit,
		BadDecimalDigit,
		UnexpectedToken,

		AmbiguousAssignment = 2500,
		CannotMarkAbstractPropertyVirtual,
		CannotMarkOverridePropertyNewOrVitual,
		ClosingTagMismatch,
		DebugContentModel,
		DummyXXXXXXXXX,
		DuplicateAttributeSpecified,
		EntityOverflow,
		ExpectedDoubleQuote,
		ExpectedElement,
		ExpectedExpression,
		ExpectedSingleQuote,
		FactorySignatureHasBody,
		ForwardReferenceToLocal,
		IncompleteContentExpecting,
		InvalidAxisSpecifier,
		InvalidContentExpecting,
		InvalidElementContent,
		InvalidElementContentNone,
		InvalidElementInEmpty,
		InvalidElementInTextOnly,
		InvalidPathExpression,
		InvalidTextInElement,
		InvalidTextInElementExpecting,
		InvalidWhitespaceInEmpty,
		LocalConstDuplicate,
		NoDefaultConstructor,
		NonDeterministic,
		NonDeterministicAny,
		NonDeterministicAssign,
		NoSuchAttribute,
		NoSuchElement,
		RequiredAttribute,
		UnknownEntity,
		UnescapedSingleQuote,

		Error,
		FatalError,
		Warning,

		InvalidTextWithReason, // todo: merge this in above (causes error numbers to change)
		CannotInstantiateTypeConverter,
		DuplicateMemberInLiteral,
		AssignmentExpressionInTuple,
		TupleIndexExpected,
		SealedTypeIsAlreadyInvariant,
		ValueTypeIsAlreadyInvariant,
		ValueTypeIsAlreadyNonNull,
		RedundantBox,
		BadBox,
		BadStream,
		RedundantStream,
		BadNonNull,
		BadNonNullOnStream,
		RedundantNonNull,
		BadNonEmptyStream,
		BadStreamOnNonNullStream,
		InvalidElementExpression,
		ContentModelNotSupported,
		QueryNotSupported,
		QueryNoMatch,
		QueryAmbiguousContextName,
		QueryBadAggregate,
		QueryBadAggregateForm,
		QueryBadGroupByList,
		QueryBadOrderList,
		QueryBadProjectionList,
		QueryBadQuantifier,
		QueryBadQuantifiedExpression,
		QueryBadDifferenceTypes,
		QueryBadInsertList,
		QueryBadIntersectionTypes,
		QueryBadLimit,
		QueryBadOrderItem,
		QueryBadUnionTypes,
		QueryBadUpdateList,
		QueryBadTypeFilter,
		QueryMissingDefaultConstructor,
		QueryNoContext,
		QueryNotScalar,
		QueryNotStream,
		QueryNotAddStream,
		QueryNotDeleteStream,
		QueryNotInsertStream,
		QueryNotUpdateStream,
		QueryProjectThroughTypeUnion,
		QueryIsCyclic,
		TypeDeclarationInsideElementGroup,
		ConstDeclarationInsideElementGroup,
		AttributeInsideElementGroup,
		QueryNotTransacted,
		QueryNotTransactable,
		QueryNoNestedTransaction,
		FieldInitializerInElementGroup,
		CannotYieldFromCatchClause,
		QueryBadLimitForNotPercent,
		QueryBadLimitNotLiteral,
		BadTupleIndex,
		CannotYieldFromTryBlock,
		BadTypeInferenceToVoid,
		WrongReturnTypeForIterator,
		AttributeInElementGroup,
		AmbiguousLiteralExpression,
		ExpectedRightBracket,
		NotAType,
		AssemblyKeyFileMissing,
		InvalidData,
		Win32IconFileNotRead,
		AbstractMethodTemplate,
		AbstractInterfaceMethod,

		ExpectedLeftParenthesis,
		CheckedExceptionNotInThrowsClause,
		ThrowsEnsuresOnConstructor,

		ExpressionIsAlreadyOfThisType,

		// MB -- 08/11/2004 added because they had been added to the Framework and not here
		CannotCoerceNullToNonNullType,
		CoercionToNonNullTypeMightFail,
		ReceiverMightBeNull,
		OnlyStructsAndClassesCanHaveInvariants,
		UpToMustBeSuperType,
		UpToMustBeClass,
		//    ExpectedLeftParenthesis,
		MustSupportComprehension,
		MustSupportReductionXXXXXXXXXXXX,
		MustResolveToType,
		//    CheckedExceptionNotInThrowsClause,
		MemberMustBePureForMethodContract,
		RequiresNotAllowedInOverride,
		ContractNotAllowedInExplicitInterfaceImplementation,
		CannotAddThrowsSet,
		CannotWeakenThrowsSet,
		DuplicateThrowsType,
		UncheckedExceptionInThrowsClause,
		RequiresNotAllowedInInterfaceImplementation,
		EnsuresInInterfaceNotInMethod,
		ModelMemberUseNotAllowedInContext,
		MemberMustBePureForInvariant,
		TypeMustSupportIntCoercions,
		CannotInjectContractFromInterface,
		CheckedExceptionInRequiresOtherwiseClause,
		ContractInheritanceRulesViolated,
		//    ThrowsEnsuresOnConstructor,

		//    UseDefViolation,
		//    UseDefViolationOut,   // partially enforced. ( only usedef, no must-assign-before-exit)
		//    UseDefViolationField, // Not enforced yet.
		//    UseDefViolationThis,  // Not enforced yet.
		//    ReturnExpected,
		//    ParamUnassigned,
		//    UnreferencedVar,
		//    UnreferencedVarAssg,
		TemplateTypeRequiresArgs,

		// Nonnull related messages.
		ReceiverCannotBeNull,
		UseOfPossiblyNullPointer,
		UseOfNullPointer,

		InvalidCompilerOptionArgument,
		TypeOfExprMustBeGuardedClass,

		CannotLoadShadowedAssembly,
		TypeMissingInShadowedAssembly,
		MethodMissingInShadowedAssembly,

		NonNullFieldNotInitializedBeforeConstructorCall,
		ModifiesNotAllowedInOverride,

		NoSuchMethod,
		CouldNotLoadPluginType,
		CouldNotInstantiatePluginType,
		PluginTypeMustAlreadyBeCompiled,
		PluginTypeMustImplementIPlugin,
		PluginCrash,

		OtherwiseExpressionMustBeNonNull,
		OtherwiseExpressionMustBeType,

		GeneralComprehensionsNotAllowedInMethodContracts,
		StrictReadonlyNotReadonly,
		StrictReadonlyStatic,
		StrictReadonlyAssignment,
		StrictReadonlyMultipleAssignment,

		WritingPackedObject,
		ExposingExposedObject,
		DontKnowIfCanExposeObject,

		GenericWarning,

		AccessThroughDelayedReference,
		StoreIntoLessDelayedLocation,
		NonNullFieldNotInitializedAtEndOfDelayedConstructor,

		BaseNotInitialized,
		BaseMultipleInitialization,

		ActualCannotBeDelayed,
		DelayedReferenceByReference,
		DelayedRefParameter,
		DelayedStructConstructor,
		ActualMustBeDelayed,
		ReceiverCannotBeDelayed,
		ReceiverMustBeDelayed,
		NonNullFieldNotInitializedByDefaultConstructor,
		AccessThroughDelayedThisInConstructor,
	}
}

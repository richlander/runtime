// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

using Internal.TypeSystem;

using ILCompiler.Metadata;
using ILCompiler.DependencyAnalysis;

using Debug = System.Diagnostics.Debug;
using EcmaModule = Internal.TypeSystem.Ecma.EcmaModule;
using CustomAttributeHandle = System.Reflection.Metadata.CustomAttributeHandle;
using ExportedTypeHandle = System.Reflection.Metadata.ExportedTypeHandle;
using FlowAnnotations = ILLink.Shared.TrimAnalysis.FlowAnnotations;
using ParameterHandle = System.Reflection.Metadata.ParameterHandle;

namespace ILCompiler
{
    /// <summary>
    /// A metadata manager that knows the full set of metadata ahead of time.
    /// </summary>
    public sealed class AnalysisBasedMetadataManager : MetadataManager, ICompilationRootProvider
    {
        private readonly List<ModuleDesc> _modulesWithMetadata;
        private readonly List<TypeDesc> _forcedTypes;

        private readonly Dictionary<TypeDesc, MetadataCategory> _reflectableTypes = new Dictionary<TypeDesc, MetadataCategory>();
        private readonly Dictionary<MethodDesc, MetadataCategory> _reflectableMethods = new Dictionary<MethodDesc, MetadataCategory>();
        private readonly Dictionary<FieldDesc, MetadataCategory> _reflectableFields = new Dictionary<FieldDesc, MetadataCategory>();
        private readonly HashSet<ReflectableCustomAttribute> _reflectableAttributes = new HashSet<ReflectableCustomAttribute>();
        private readonly HashSet<ReflectableParameter> _reflectableParameters = new HashSet<ReflectableParameter>();

        public AnalysisBasedMetadataManager(CompilerTypeSystemContext typeSystemContext)
            : this(typeSystemContext, new FullyBlockedMetadataBlockingPolicy(),
                new FullyBlockedManifestResourceBlockingPolicy(), null, new NoStackTraceEmissionPolicy(),
                new NoDynamicInvokeThunkGenerationPolicy(), null, Array.Empty<ModuleDesc>(), Array.Empty<TypeDesc>(),
                Array.Empty<ReflectableEntity<TypeDesc>>(), Array.Empty<ReflectableEntity<MethodDesc>>(),
                Array.Empty<ReflectableEntity<FieldDesc>>(), Array.Empty<ReflectableCustomAttribute>(),
                Array.Empty<ReflectableParameter>(),
                default)
        {
        }

        public AnalysisBasedMetadataManager(
            CompilerTypeSystemContext typeSystemContext,
            MetadataBlockingPolicy blockingPolicy,
            ManifestResourceBlockingPolicy resourceBlockingPolicy,
            string logFile,
            StackTraceEmissionPolicy stackTracePolicy,
            DynamicInvokeThunkGenerationPolicy invokeThunkGenerationPolicy,
            FlowAnnotations flowAnnotations,
            IEnumerable<ModuleDesc> modulesWithMetadata,
            IEnumerable<TypeDesc> forcedTypes,
            IEnumerable<ReflectableEntity<TypeDesc>> reflectableTypes,
            IEnumerable<ReflectableEntity<MethodDesc>> reflectableMethods,
            IEnumerable<ReflectableEntity<FieldDesc>> reflectableFields,
            IEnumerable<ReflectableCustomAttribute> reflectableAttributes,
            IEnumerable<ReflectableParameter> reflectableParameters,
            MetadataManagerOptions options)
            : base(typeSystemContext, blockingPolicy, resourceBlockingPolicy, logFile, stackTracePolicy, invokeThunkGenerationPolicy, options, flowAnnotations)
        {
            _modulesWithMetadata = new List<ModuleDesc>(modulesWithMetadata);
            _forcedTypes = new List<TypeDesc>(forcedTypes);

            foreach (var refType in reflectableTypes)
            {
                _reflectableTypes.Add(refType.Entity, refType.Category);
            }

            foreach (var refMethod in reflectableMethods)
            {
                // Asking for description or runtime mapping for a member without asking
                // for the owning type would mean we can't actually satisfy the request.
                Debug.Assert((refMethod.Category & MetadataCategory.Description) == 0
                    || (_reflectableTypes[refMethod.Entity.OwningType] & MetadataCategory.Description) != 0);
                Debug.Assert((refMethod.Category & MetadataCategory.RuntimeMapping) == 0
                    || (_reflectableTypes[refMethod.Entity.OwningType] & MetadataCategory.RuntimeMapping) != 0);
                _reflectableMethods.Add(refMethod.Entity, refMethod.Category);
            }

            foreach (var refField in reflectableFields)
            {
                // Asking for description or runtime mapping for a member without asking
                // for the owning type would mean we can't actually satisfy the request.
                Debug.Assert((refField.Category & MetadataCategory.Description) == 0
                    || (_reflectableTypes[refField.Entity.OwningType] & MetadataCategory.Description) != 0);
                Debug.Assert((refField.Category & MetadataCategory.RuntimeMapping) == 0
                    || (_reflectableTypes[refField.Entity.OwningType] & MetadataCategory.RuntimeMapping) != 0);
                _reflectableFields.Add(refField.Entity, refField.Category);
            }

            foreach (var refAttribute in reflectableAttributes)
            {
                _reflectableAttributes.Add(refAttribute);
            }

            foreach (var refParameter in reflectableParameters)
            {
                _reflectableParameters.Add(refParameter);
            }

#if DEBUG
            HashSet<ModuleDesc> moduleHash = new HashSet<ModuleDesc>(_modulesWithMetadata);
            foreach (var refType in reflectableTypes)
            {
                // The instantiated types need to agree on the Description bit with the definition.
                // GetMetadataCategory relies on that.
                Debug.Assert((GetMetadataCategory(refType.Entity.GetTypeDefinition()) & MetadataCategory.Description)
                    == (GetMetadataCategory(refType.Entity) & MetadataCategory.Description));

                Debug.Assert((refType.Category & MetadataCategory.Description) == 0 ||
                    !(refType.Entity is MetadataType) || moduleHash.Contains(((MetadataType)refType.Entity).Module));
            }

            foreach (var refMethod in reflectableMethods)
            {
                // The instantiated methods need to agree on the Description bit with the definition.
                // GetMetadataCategory relies on that.
                Debug.Assert((GetMetadataCategory(refMethod.Entity.GetTypicalMethodDefinition()) & MetadataCategory.Description)
                    == (GetMetadataCategory(refMethod.Entity) & MetadataCategory.Description));

                // Canonical form of the method needs to agree with the logical form
                Debug.Assert(GetMetadataCategory(refMethod.Entity) == GetMetadataCategory(refMethod.Entity.GetCanonMethodTarget(CanonicalFormKind.Specific)));
            }

            foreach (var refField in reflectableFields)
            {
                // The instantiated fields need to agree on the Description bit with the definition.
                // GetMetadataCategory relies on that.
                Debug.Assert((GetMetadataCategory(refField.Entity.GetTypicalFieldDefinition()) & MetadataCategory.Description)
                    == (GetMetadataCategory(refField.Entity) & MetadataCategory.Description));
            }
#endif
        }

        public override IEnumerable<ModuleDesc> GetCompilationModulesWithMetadata()
        {
            return _modulesWithMetadata;
        }

        protected override void ComputeMetadata(NodeFactory factory,
            out byte[] metadataBlob,
            out List<MetadataMapping<MetadataType>> typeMappings,
            out List<MetadataMapping<MethodDesc>> methodMappings,
            out Dictionary<MethodDesc, int> methodMetadataMappings,
            out List<MetadataMapping<FieldDesc>> fieldMappings,
            out Dictionary<FieldDesc, int> fieldMetadataMappings,
            out List<StackTraceMapping> stackTraceMapping)
        {
            ComputeMetadata(new Policy(_blockingPolicy, this), factory,
                out metadataBlob,
                out typeMappings,
                out methodMappings,
                out methodMetadataMappings,
                out fieldMappings,
                out fieldMetadataMappings,
                out stackTraceMapping);
        }

        protected sealed override MetadataCategory GetMetadataCategory(MethodDesc method)
        {
            if (_reflectableMethods.TryGetValue(method, out MetadataCategory value))
                return value;
            return 0;
        }

        protected sealed override MetadataCategory GetMetadataCategory(TypeDesc type)
        {
            if (_reflectableTypes.TryGetValue(type, out MetadataCategory value))
                return value;
            return 0;
        }

        protected sealed override MetadataCategory GetMetadataCategory(FieldDesc field)
        {
            if (_reflectableFields.TryGetValue(field, out MetadataCategory value))
                return value;
            return 0;
        }

        protected override IEnumerable<FieldDesc> GetFieldsWithRuntimeMapping()
        {
            foreach (var pair in _reflectableFields)
            {
                if ((pair.Value & MetadataCategory.RuntimeMapping) != 0)
                    yield return pair.Key;
            }
        }

        void ICompilationRootProvider.AddCompilationRoots(IRootingServiceProvider rootProvider)
        {
            // We go over all the types and members that need a runtime artifact present in the
            // compiled executable and root it.

            const string reason = "Reflection";

            foreach (var type in _forcedTypes)
            {
                rootProvider.AddReflectionRoot(type, reason);
            }

            foreach (var pair in _reflectableMethods)
            {
                if ((pair.Value & MetadataCategory.RuntimeMapping) != 0)
                {
                    MethodDesc method = pair.Key;
                    rootProvider.AddReflectionRoot(method, reason);
                }
            }

            foreach (var pair in _reflectableFields)
            {
                if ((pair.Value & MetadataCategory.RuntimeMapping) != 0)
                {
                    FieldDesc field = pair.Key;
                    rootProvider.AddReflectionRoot(field, reason);
                }
            }
        }

        private struct Policy : IMetadataPolicy
        {
            private readonly MetadataBlockingPolicy _blockingPolicy;
            private readonly AnalysisBasedMetadataManager _parent;

            public Policy(MetadataBlockingPolicy blockingPolicy,
                AnalysisBasedMetadataManager parent)
            {
                _blockingPolicy = blockingPolicy;
                _parent = parent;
            }

            public bool GeneratesMetadata(FieldDesc fieldDef)
            {
                return (_parent.GetMetadataCategory(fieldDef) & MetadataCategory.Description) != 0;
            }

            public bool GeneratesMetadata(MethodDesc methodDef)
            {
                return (_parent.GetMetadataCategory(methodDef) & MetadataCategory.Description) != 0;
            }

            public bool GeneratesMetadata(MetadataType typeDef)
            {
                return (_parent.GetMetadataCategory(typeDef) & MetadataCategory.Description) != 0;
            }

            public bool GeneratesMetadata(EcmaModule module, CustomAttributeHandle caHandle)
            {
                return _parent._reflectableAttributes.Contains(new ReflectableCustomAttribute(module, caHandle));
            }

            public bool GeneratesMetadata(EcmaModule module, ParameterHandle paramHandle)
            {
                return _parent._reflectableParameters.Contains(new ReflectableParameter(module, paramHandle));
            }

            public bool GeneratesMetadata(EcmaModule module, ExportedTypeHandle exportedTypeHandle)
            {
                // We'll possibly need to do something else here if we ever use this MetadataManager
                // with compilation modes that generate multiple metadata blobs.
                // (Multi-module or .NET Native style shared library.)
                // We are currently missing type forwarders pointing to the other blobs.
                var targetType = (MetadataType)module.GetObject(exportedTypeHandle, NotFoundBehavior.ReturnNull);
                if (targetType == null)
                {
                    // No harm in generating a forwarder that didn't resolve.
                    // We'll get matching behavior at runtime.
                    return true;
                }

                return GeneratesMetadata(targetType);
            }

            public bool GeneratesInterfaceImpl(MetadataType typeDef, MetadataType interfaceImpl)
            {
                return _parent.IsInterfaceUsed(interfaceImpl.GetTypeDefinition());
            }

            public bool IsBlocked(MetadataType typeDef)
            {
                return _blockingPolicy.IsBlocked(typeDef);
            }

            public bool IsBlocked(MethodDesc methodDef)
            {
                return _blockingPolicy.IsBlocked(methodDef);
            }
        }
    }

    public struct ReflectableEntity<TEntity>
    {
        public readonly TEntity Entity;
        public readonly MetadataCategory Category;

        public ReflectableEntity(TEntity entity, MetadataCategory category)
        {
            Entity = entity;
            Category = category;
        }
    }

    public struct ReflectableCustomAttribute : IEquatable<ReflectableCustomAttribute>
    {
        public readonly EcmaModule Module;
        public readonly CustomAttributeHandle CustomAttributeHandle;

        public ReflectableCustomAttribute(EcmaModule module, CustomAttributeHandle caHandle)
            => (Module, CustomAttributeHandle) = (module, caHandle);

        public bool Equals(ReflectableCustomAttribute other)
            => other.Module == Module && other.CustomAttributeHandle == CustomAttributeHandle;
        public override bool Equals(object obj)
            => obj is ReflectableCustomAttribute other && Equals(other);
        public override int GetHashCode() => Module.GetHashCode() ^ CustomAttributeHandle.GetHashCode();
    }

    public struct ReflectableParameter : IEquatable<ReflectableParameter>
    {
        public readonly EcmaModule Module;
        public readonly ParameterHandle ParameterHandle;

        public ReflectableParameter(EcmaModule module, ParameterHandle paramHandle)
            => (Module, ParameterHandle) = (module, paramHandle);

        public bool Equals(ReflectableParameter other)
            => other.Module == Module && other.ParameterHandle == ParameterHandle;
        public override bool Equals(object obj)
            => obj is ReflectableParameter other && Equals(other);
        public override int GetHashCode() => Module.GetHashCode() ^ ParameterHandle.GetHashCode();
    }
}

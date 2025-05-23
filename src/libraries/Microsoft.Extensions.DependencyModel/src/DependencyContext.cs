// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyModel
{
    public class DependencyContext
    {

        [UnconditionalSuppressMessage("SingleFile", "IL3002:Avoid calling members marked with 'RequiresAssemblyFilesAttribute' when publishing as a single-file",
            Justification = "The annotation should be on the static constructor but is Compiler Generated, annotating the caller Default method instead")]
        private static readonly Lazy<DependencyContext?> _defaultContext = new(LoadDefault);

        public DependencyContext(TargetInfo target,
            CompilationOptions compilationOptions,
            IEnumerable<CompilationLibrary> compileLibraries,
            IEnumerable<RuntimeLibrary> runtimeLibraries,
            IEnumerable<RuntimeFallbacks> runtimeGraph)
        {
            ArgumentNullException.ThrowIfNull(target);
            ArgumentNullException.ThrowIfNull(compilationOptions);
            ArgumentNullException.ThrowIfNull(compileLibraries);
            ArgumentNullException.ThrowIfNull(runtimeLibraries);
            ArgumentNullException.ThrowIfNull(runtimeGraph);

            Target = target;
            CompilationOptions = compilationOptions;
            CompileLibraries = compileLibraries.ToArray();
            RuntimeLibraries = runtimeLibraries.ToArray();
            RuntimeGraph = runtimeGraph.ToArray();
        }

        [RequiresAssemblyFiles("DependencyContext for an assembly from a application published as single-file is not supported. The method will return null. Make sure the calling code can handle this case.")]
        public static DependencyContext? Default => _defaultContext.Value;

        public TargetInfo Target { get; }

        public CompilationOptions CompilationOptions { get; }

        public IReadOnlyList<CompilationLibrary> CompileLibraries { get; }

        public IReadOnlyList<RuntimeLibrary> RuntimeLibraries { get; }

        public IReadOnlyList<RuntimeFallbacks> RuntimeGraph { get; }

        public DependencyContext Merge(DependencyContext other)
        {
            ArgumentNullException.ThrowIfNull(other);

            return new DependencyContext(
                Target,
                CompilationOptions,
                CompileLibraries.Union(other.CompileLibraries, new LibraryMergeEqualityComparer<CompilationLibrary>()),
                RuntimeLibraries.Union(other.RuntimeLibraries, new LibraryMergeEqualityComparer<RuntimeLibrary>()),
                RuntimeGraph.Union(other.RuntimeGraph)
                );
        }

        [RequiresAssemblyFiles("DependencyContext for an assembly from a application published as single-file is not supported. The method will return null. Make sure the calling code can handle this case.")]
        private static DependencyContext? LoadDefault()
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly == null)
            {
                return null;
            }

            return Load(entryAssembly);
        }

        [RequiresAssemblyFiles("DependencyContext for an assembly from a application published as single-file is not supported. The method will return null. Make sure the calling code can handle this case.")]
        public static DependencyContext? Load(Assembly assembly)
        {
            return DependencyContextLoader.Default.Load(assembly);
        }

        private sealed class LibraryMergeEqualityComparer<T> : IEqualityComparer<T> where T : Library
        {
            public bool Equals(T? x, T? y)
            {
                return StringComparer.OrdinalIgnoreCase.Equals(x?.Name, y?.Name);
            }

            public int GetHashCode(T obj)
            {
                return StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Name);
            }
        }
    }
}

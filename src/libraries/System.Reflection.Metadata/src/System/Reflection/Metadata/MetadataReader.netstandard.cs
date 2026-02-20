// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Immutable;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Reflection.PortableExecutable;
using Microsoft.Win32.SafeHandles;

namespace System.Reflection.Metadata
{
    /// <summary>
    /// Reads metadata as defined by the ECMA 335 CLI specification.
    /// </summary>
    /// <remarks>
    /// <see cref="System.Reflection.Metadata.MetadataReader"/> reads the contents of tables and heaps from the specified CLI metadata. It operates low-level constructs such as type and method definitions. For a higher level API to inspect the contents of assemblies using reflection constructs, see <see cref="System.Reflection.MetadataLoadContext"/>.
    /// You can use constructors, such as <see cref="System.Reflection.Metadata.MetadataReader.#ctor(System.Byte,System.Int32)"/>, to create an instance of <see cref="System.Reflection.Metadata.MetadataReader"/> for a given memory location. To read metadata from the Portable Executable assembly file, create <see cref="System.Reflection.PortableExecutable.PEReader"/> and use the <see cref="System.Reflection.Metadata.PEReaderExtensions.GetMetadataReader(System.Reflection.PortableExecutable.PEReader)"/> extension method.
    /// The format of CLI metadata is defined by the ECMA-335 specification. For more information, see [Standard ECMA-335 - Common Language Infrastructure (CLI)](https://www.ecma-international.org/publications-and-standards/standards/ecma-335/) on the Ecma International Web site.
    /// This example shows how to create <see cref="System.Reflection.Metadata.MetadataReader"/> for an assembly and read all type definitions from it:
    /// <code lang="csharp" source="~/snippets/csharp/System.Reflection.Metadata/MetadataReader/MetadataReaderSnippets.cs" id="SnippetMetadataReader" />
    /// </remarks>
    public sealed partial class MetadataReader
    {
        /// <summary>
        /// Gets the <see cref="AssemblyName"/> for a given file.
        /// </summary>
        /// <param name="assemblyFile">The path for the assembly which <see cref="AssemblyName"/> is to be returned.</param>
        /// <returns>An <see cref="AssemblyName"/> that represents the given <paramref name="assemblyFile"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="assemblyFile"/> is null.</exception>
        /// <exception cref="ArgumentException">If <paramref name="assemblyFile"/> is invalid.</exception>
        /// <exception cref="FileNotFoundException">If <paramref name="assemblyFile"/> is not found.</exception>
        /// <exception cref="BadImageFormatException">If <paramref name="assemblyFile"/> is not a valid assembly.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="assemblyFile"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="assemblyFile"/> is invalid.</exception>
        /// <exception cref="T:System.IO.FileNotFoundException"><paramref name="assemblyFile"/> is not found.</exception>
        /// <exception cref="T:System.BadImageFormatException"><paramref name="assemblyFile"/> is not a valid assembly.</exception>
        internal AssemblyName GetAssemblyName(StringHandle nameHandle, Version version, StringHandle cultureHandle, BlobHandle publicKeyOrTokenHandle, AssemblyHashAlgorithm assemblyHashAlgorithm, AssemblyFlags flags)
        {
            string name = GetString(nameHandle);
            // compat: normalize Nil culture name to "" to match original behavior of AssemblyName.GetAssemblyName()
            string cultureName = (!cultureHandle.IsNil) ? GetString(cultureHandle) : "";
            var hashAlgorithm = (Configuration.Assemblies.AssemblyHashAlgorithm)assemblyHashAlgorithm;
            // compat: original native implementation used to guarantee that publicKeyOrToken is never null in this scenario.
            byte[]? publicKeyOrToken = !publicKeyOrTokenHandle.IsNil ? GetBlobBytes(publicKeyOrTokenHandle) : Array.Empty<byte>();

            var assemblyName = new AssemblyName()
            {
                Name = name,
                Version = version,
                CultureName = cultureName,
#pragma warning disable SYSLIB0037 // AssemblyName.HashAlgorithm is obsolete
                HashAlgorithm = hashAlgorithm,
#pragma warning restore
                Flags = GetAssemblyNameFlags(flags),
                ContentType = GetContentTypeFromAssemblyFlags(flags)
            };

            bool hasPublicKey = (flags & AssemblyFlags.PublicKey) != 0;
            if (hasPublicKey)
            {
                assemblyName.SetPublicKey(publicKeyOrToken);
            }
            else
            {
                assemblyName.SetPublicKeyToken(publicKeyOrToken);
            }

            return assemblyName;
        }

        internal AssemblyNameInfo GetAssemblyNameInfo(StringHandle nameHandle, Version version, StringHandle cultureHandle, BlobHandle publicKeyOrTokenHandle, AssemblyFlags flags)
        {
            string name = GetString(nameHandle);
            string? cultureName = !cultureHandle.IsNil ? GetString(cultureHandle) : null;
            ImmutableArray<byte> publicKeyOrToken = !publicKeyOrTokenHandle.IsNil ? GetBlobContent(publicKeyOrTokenHandle) : default;

            return new AssemblyNameInfo(name, version, cultureName, GetAssemblyNameFlags(flags), publicKeyOrToken);
        }

        /// <summary>
        /// Gets the <see cref="AssemblyName"/> for a given file.
        /// </summary>
        /// <param name="assemblyFile">The path for the assembly which <see cref="AssemblyName"/> is to be returned.</param>
        /// <returns>An <see cref="AssemblyName"/> that represents the given <paramref name="assemblyFile"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="assemblyFile"/> is null.</exception>
        /// <exception cref="ArgumentException">If <paramref name="assemblyFile"/> is invalid.</exception>
        /// <exception cref="FileNotFoundException">If <paramref name="assemblyFile"/> is not found.</exception>
        /// <exception cref="BadImageFormatException">If <paramref name="assemblyFile"/> is not a valid assembly.</exception>
        public static unsafe AssemblyName GetAssemblyName(string assemblyFile)
        {
            if (assemblyFile is null)
            {
                Throw.ArgumentNull(nameof(assemblyFile));
            }

            FileStream? fileStream = null;
            MemoryMappedFile? mappedFile = null;
            MemoryMappedViewAccessor? accessor = null;
            PEReader? peReader = null;

            try
            {
                try
                {
                    // Create stream because CreateFromFile(string, ...) uses FileShare.None which is too strict
                    fileStream = new FileStream(assemblyFile, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 1, useAsync: false);
                    if (fileStream.Length == 0)
                    {
                        throw new BadImageFormatException(SR.PEImageDoesNotHaveMetadata, assemblyFile);
                    }

                    mappedFile = MemoryMappedFile.CreateFromFile(
                        fileStream, null, fileStream.Length, MemoryMappedFileAccess.Read, HandleInheritability.None, true);
                    accessor = mappedFile.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);

                    SafeMemoryMappedViewHandle? safeBuffer = accessor.SafeMemoryMappedViewHandle;
                    peReader = new PEReader((byte*)safeBuffer.DangerousGetHandle(), (int)safeBuffer.ByteLength);
                    MetadataReader mdReader = peReader.GetMetadataReader(MetadataReaderOptions.None);
                    AssemblyName assemblyName = mdReader.GetAssemblyDefinition().GetAssemblyName();
                    return assemblyName;
                }
                finally
                {
                    peReader?.Dispose();
                    accessor?.Dispose();
                    mappedFile?.Dispose();
                    fileStream?.Dispose();
                }
            }
            catch (InvalidOperationException ex)
            {
                throw new BadImageFormatException(ex.Message, assemblyFile, ex);
            }
        }

        private static AssemblyNameFlags GetAssemblyNameFlags(AssemblyFlags flags)
        {
            AssemblyNameFlags assemblyNameFlags = AssemblyNameFlags.None;

            if ((flags & AssemblyFlags.PublicKey) != 0)
                assemblyNameFlags |= AssemblyNameFlags.PublicKey;

            if ((flags & AssemblyFlags.Retargetable) != 0)
                assemblyNameFlags |= AssemblyNameFlags.Retargetable;

            if ((flags & AssemblyFlags.EnableJitCompileTracking) != 0)
                assemblyNameFlags |= AssemblyNameFlags.EnableJITcompileTracking;

            if ((flags & AssemblyFlags.DisableJitCompileOptimizer) != 0)
                assemblyNameFlags |= AssemblyNameFlags.EnableJITcompileOptimizer;

            return assemblyNameFlags;
        }

        private static AssemblyContentType GetContentTypeFromAssemblyFlags(AssemblyFlags flags)
        {
            return (AssemblyContentType)(((int)flags & (int)AssemblyFlags.ContentTypeMask) >> 9);
        }
    }
}

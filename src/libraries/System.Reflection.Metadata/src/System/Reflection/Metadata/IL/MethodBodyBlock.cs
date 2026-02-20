// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection.Internal;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;

namespace System.Reflection.Metadata
{
    /// <summary>
    /// Represents the method body in ECMA 335 assembly.
    /// </summary>
    /// <remarks>
    /// The method body contains Common Intermediate Language (CIL) instructions that make up a method and information about its local variables and exception regions. You can use the <see cref="System.Reflection.Metadata.PEReaderExtensions.GetMethodBody"/> method to get a <c>MethodBodyBlock</c> instance for the specified method.
    /// The format of CIL instructions and metadata is defined by the ECMA-335 specification. For more information, see [Standard ECMA-335 - Common Language Infrastructure (CLI)](https://www.ecma-international.org/publications-and-standards/standards/ecma-335/) on the Ecma International Web site.
    /// This example shows how to read method bodies for all methods in the specified type definition and display method body information:
    /// [!code-csharp[](~/snippets/csharp/System.Reflection.Metadata/MethodBodyBlock/MethodBodyBlockSnippets.cs#PrintMethods)]
    /// </remarks>
    public sealed class MethodBodyBlock
    {
        private readonly MemoryBlock _il;
        private readonly int _size;
        private readonly ushort _maxStack;
        private readonly bool _localVariablesInitialized;
        private readonly StandaloneSignatureHandle _localSignature;
        private readonly ImmutableArray<ExceptionRegion> _exceptionRegions;

        private MethodBodyBlock(
            bool localVariablesInitialized,
            ushort maxStack,
            StandaloneSignatureHandle localSignatureHandle,
            MemoryBlock il,
            ImmutableArray<ExceptionRegion> exceptionRegions,
            int size)
        {
            Debug.Assert(!exceptionRegions.IsDefault);

            _localVariablesInitialized = localVariablesInitialized;
            _maxStack = maxStack;
            _localSignature = localSignatureHandle;
            _il = il;
            _exceptionRegions = exceptionRegions;
            _size = size;
        }

        /// <summary>
        /// Size of the method body - includes the header, IL and exception regions.
        /// </summary>
        public int Size
        {
            get { return _size; }
        }

        /// <summary>
        /// Gets the maximum number of items on the evaluation stack for this method.
        /// </summary>
        /// <value>The maximum number of items on the evaluation stack.</value>
        public int MaxStack
        {
            get { return _maxStack; }
        }

        /// <summary>
        /// Gets a value that indicates whether local variables in this method are initialized to default values of their types.
        /// </summary>
        /// <value><see langword="true"/> if local variables are initialized; otherwise, <see langword="false"/>.</value>
        public bool LocalVariablesInitialized
        {
            get { return _localVariablesInitialized; }
        }

        /// <summary>
        /// Gets the handle to the local variables signature.
        /// </summary>
        /// <value>The handle to the local variables signature.</value>
        public StandaloneSignatureHandle LocalSignature
        {
            get { return _localSignature; }
        }

        /// <summary>
        /// Gets the array of exception regions in this method body.
        /// </summary>
        /// <value>The array of exception regions.</value>
        public ImmutableArray<ExceptionRegion> ExceptionRegions
        {
            get { return _exceptionRegions; }
        }

        /// <summary>
        /// Gets the IL bytecode of this method body as a byte array.
        /// </summary>
        /// <returns>A byte array with the IL bytecode of this method body.</returns>
        public byte[]? GetILBytes()
        {
            return _il.ToArray();
        }

        /// <summary>
        /// Gets the IL bytecode of this method body as an immutable array.
        /// </summary>
        /// <returns>An immutable byte array with the IL bytecode of this method body.</returns>
        public ImmutableArray<byte> GetILContent()
        {
            byte[]? bytes = GetILBytes();
            return ImmutableCollectionsMarshal.AsImmutableArray(bytes);
        }

        /// <summary>
        /// Gets a blob reader that reads the IL bytecode of this method body.
        /// </summary>
        /// <returns>A blob reader that reads the IL bytecode of this method body.</returns>
        public BlobReader GetILReader()
        {
            return new BlobReader(_il);
        }

        private const byte ILTinyFormat = 0x02;
        private const byte ILFatFormat = 0x03;
        private const byte ILFormatMask = 0x03;
        private const int ILTinyFormatSizeShift = 2;
        private const byte ILMoreSects = 0x08;
        private const byte ILInitLocals = 0x10;
        private const byte ILFatFormatHeaderSize = 0x03;
        private const int ILFatFormatHeaderSizeShift = 4;
        private const byte SectEHTable = 0x01;
        private const byte SectFatFormat = 0x40;

        /// <summary>
        /// Creates a new instance of the <see cref="T:System.Reflection.Metadata.MethodBodyBlock"/> class using the specified blob reader.
        /// </summary>
        /// <param name="reader">The blob reader to read the method body.</param>
        /// <returns>A new instance of the <see cref="T:System.Reflection.Metadata.MethodBodyBlock"/> class.</returns>
        /// <exception cref="T:System.BadImageFormatException"> The method body data in the specified blob reader is invalid.</exception>
        public static MethodBodyBlock Create(BlobReader reader)
        {
            int startOffset = reader.Offset;
            int ilSize;

            // Error need to check if the Memory Block is empty. This is false for all the calls...
            byte headByte = reader.ReadByte();
            if ((headByte & ILFormatMask) == ILTinyFormat)
            {
                // tiny IL can't have locals so technically this shouldn't matter,
                // but false is consistent with other metadata readers and helps
                // for use cases involving comparing our output with theirs.
                const bool initLocalsForTinyIL = false;

                ilSize = headByte >> ILTinyFormatSizeShift;
                return new MethodBodyBlock(
                    initLocalsForTinyIL,
                    8,
                    default(StandaloneSignatureHandle),
                    reader.GetMemoryBlockAt(0, ilSize),
                    ImmutableArray<ExceptionRegion>.Empty,
                    1 + ilSize // header + IL
                );
            }

            if ((headByte & ILFormatMask) != ILFatFormat)
            {
                throw new BadImageFormatException(SR.Format(SR.InvalidMethodHeader1, headByte));
            }

            // FatILFormat
            byte headByte2 = reader.ReadByte();
            if ((headByte2 >> ILFatFormatHeaderSizeShift) != ILFatFormatHeaderSize)
            {
                throw new BadImageFormatException(SR.Format(SR.InvalidMethodHeader2, headByte, headByte2));
            }

            bool localsInitialized = (headByte & ILInitLocals) == ILInitLocals;
            bool hasExceptionHandlers = (headByte & ILMoreSects) == ILMoreSects;

            ushort maxStack = reader.ReadUInt16();
            ilSize = reader.ReadInt32();

            int localSignatureToken = reader.ReadInt32();
            StandaloneSignatureHandle localSignatureHandle;
            if (localSignatureToken == 0)
            {
                localSignatureHandle = default(StandaloneSignatureHandle);
            }
            else if ((localSignatureToken & TokenTypeIds.TypeMask) == TokenTypeIds.Signature)
            {
                localSignatureHandle = StandaloneSignatureHandle.FromRowId((int)((uint)localSignatureToken & TokenTypeIds.RIDMask));
            }
            else
            {
                throw new BadImageFormatException(SR.Format(SR.InvalidLocalSignatureToken, unchecked((uint)localSignatureToken)));
            }

            var ilBlock = reader.GetMemoryBlockAt(0, ilSize);
            reader.Offset += ilSize;

            ImmutableArray<ExceptionRegion> exceptionHandlers;
            if (hasExceptionHandlers)
            {
                reader.Align(4);
                byte sehHeader = reader.ReadByte();
                if ((sehHeader & SectEHTable) != SectEHTable)
                {
                    throw new BadImageFormatException(SR.Format(SR.InvalidSehHeader, sehHeader));
                }

                bool sehFatFormat = (sehHeader & SectFatFormat) == SectFatFormat;
                int dataSize = reader.ReadByte();
                if (sehFatFormat)
                {
                    dataSize += reader.ReadUInt16() << 8;
                    exceptionHandlers = ReadFatExceptionHandlers(ref reader, dataSize / 24);
                }
                else
                {
                    reader.Offset += 2; // skip over reserved field
                    exceptionHandlers = ReadSmallExceptionHandlers(ref reader, dataSize / 12);
                }
            }
            else
            {
                exceptionHandlers = ImmutableArray<ExceptionRegion>.Empty;
            }

            return new MethodBodyBlock(
                localsInitialized,
                maxStack,
                localSignatureHandle,
                ilBlock,
                exceptionHandlers,
                reader.Offset - startOffset);
        }

        private static ImmutableArray<ExceptionRegion> ReadSmallExceptionHandlers(ref BlobReader memReader, int count)
        {
            var result = new ExceptionRegion[count];
            for (int i = 0; i < result.Length; i++)
            {
                var kind = (ExceptionRegionKind)memReader.ReadUInt16();
                var tryOffset = memReader.ReadUInt16();
                var tryLength = memReader.ReadByte();
                var handlerOffset = memReader.ReadUInt16();
                var handlerLength = memReader.ReadByte();
                var classTokenOrFilterOffset = memReader.ReadInt32();
                result[i] = new ExceptionRegion(kind, tryOffset, tryLength, handlerOffset, handlerLength, classTokenOrFilterOffset);
            }

            return ImmutableArray.Create(result);
        }

        private static ImmutableArray<ExceptionRegion> ReadFatExceptionHandlers(ref BlobReader memReader, int count)
        {
            var result = new ExceptionRegion[count];
            for (int i = 0; i < result.Length; i++)
            {
                var sehFlags = (ExceptionRegionKind)memReader.ReadUInt32();
                int tryOffset = memReader.ReadInt32();
                int tryLength = memReader.ReadInt32();
                int handlerOffset = memReader.ReadInt32();
                int handlerLength = memReader.ReadInt32();
                int classTokenOrFilterOffset = memReader.ReadInt32();
                result[i] = new ExceptionRegion(sehFlags, tryOffset, tryLength, handlerOffset, handlerLength, classTokenOrFilterOffset);
            }

            return ImmutableArray.Create(result);
        }
    }
}

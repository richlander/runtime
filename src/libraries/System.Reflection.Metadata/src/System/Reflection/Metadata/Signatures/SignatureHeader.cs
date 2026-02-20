// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace System.Reflection.Metadata
{
    /// <summary>
    /// Represents the signature characteristics specified by the leading byte of signature blobs.
    /// </summary>
    /// <remarks>
    /// This header byte is present in all method definition, method reference, standalone method, field,
    /// property, and local variable signatures, but not in type specification signatures.
    /// </remarks>
    public struct SignatureHeader : IEquatable<SignatureHeader>
    {
        private readonly byte _rawValue;
        /// <summary>
        /// Gets the mask value for the calling convention or signature kind. The default <see cref="F:System.Reflection.Metadata.SignatureHeader.CallingConventionOrKindMask"/> value is 15 (0x0F).
        /// </summary>
        public const byte CallingConventionOrKindMask = 0x0F;
        private const byte maxCallingConvention = (byte)SignatureCallingConvention.VarArgs;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Reflection.Metadata.SignatureHeader"/> structure using the specified byte value.
        /// </summary>
        /// <param name="rawValue">The byte.</param>
        public SignatureHeader(byte rawValue)
        {
            _rawValue = rawValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Reflection.Metadata.SignatureHeader"/> structure using the specified signature kind, calling convention and signature attributes.
        /// </summary>
        /// <param name="kind">The signature kind.</param>
        /// <param name="convention">The calling convention.</param>
        /// <param name="attributes">The signature attributes.</param>
        public SignatureHeader(SignatureKind kind, SignatureCallingConvention convention, SignatureAttributes attributes)
             : this((byte)((int)kind | (int)convention | (int)attributes))
        {
        }

        /// <summary>
        /// Gets the raw value of the header byte.
        /// </summary>
        /// <value>The raw value of the header byte.</value>
        public byte RawValue
        {
            get { return _rawValue; }
        }

        /// <summary>
        /// Gets the calling convention.
        /// </summary>
        /// <value>The calling convention.</value>
        public SignatureCallingConvention CallingConvention
        {
            get
            {
                int callingConventionOrKind = _rawValue & CallingConventionOrKindMask;

                if (callingConventionOrKind > maxCallingConvention
                    && callingConventionOrKind != (int)SignatureCallingConvention.Unmanaged)
                {
                    return SignatureCallingConvention.Default;
                }

                return (SignatureCallingConvention)callingConventionOrKind;
            }
        }

        /// <summary>
        /// Gets the signature kind.
        /// </summary>
        /// <value>The signature kind.</value>
        public SignatureKind Kind
        {
            get
            {
                int callingConventionOrKind = _rawValue & CallingConventionOrKindMask;

                if (callingConventionOrKind <= maxCallingConvention
                    || callingConventionOrKind == (int)SignatureCallingConvention.Unmanaged)
                {
                    return SignatureKind.Method;
                }

                return (SignatureKind)callingConventionOrKind;
            }
        }

        /// <summary>
        /// Gets the signature attributes.
        /// </summary>
        /// <value>The attributes.</value>
        public SignatureAttributes Attributes
        {
            get { return (SignatureAttributes)(_rawValue & ~CallingConventionOrKindMask); }
        }

        /// <summary>
        /// Gets a value that indicates whether this <see cref="T:System.Reflection.Metadata.SignatureHeader"/> structure has the <see cref="F:System.Reflection.Metadata.SignatureAttributes.ExplicitThis"/> signature attribute.
        /// </summary>
        /// <value><see langword="true"/> if the <see cref="F:System.Reflection.Metadata.SignatureAttributes.ExplicitThis"/> attribute is present; otherwise, <see langword="false"/>.</value>
        public bool HasExplicitThis
        {
            get { return (_rawValue & (byte)SignatureAttributes.ExplicitThis) != 0; }
        }

        /// <summary>
        /// Gets a value that indicates whether this <see cref="T:System.Reflection.Metadata.SignatureHeader"/> structure has the <see cref="F:System.Reflection.Metadata.SignatureAttributes.Instance"/> signature attribute.
        /// </summary>
        /// <value><see langword="true"/> if the <see cref="F:System.Reflection.Metadata.SignatureAttributes.Instance"/> attribute is present; otherwise, <see langword="false"/>.</value>
        public bool IsInstance
        {
            get { return (_rawValue & (byte)SignatureAttributes.Instance) != 0; }
        }

        /// <summary>
        /// Gets a value that indicates whether this <see cref="T:System.Reflection.Metadata.SignatureHeader"/> structure has the <see cref="F:System.Reflection.Metadata.SignatureAttributes.Generic"/> signature attribute.
        /// </summary>
        /// <value><see langword="true"/> if the <see cref="F:System.Reflection.Metadata.SignatureAttributes.Generic"/> attribute is present; otherwise, <see langword="false"/>.</value>
        public bool IsGeneric
        {
            get { return (_rawValue & (byte)SignatureAttributes.Generic) != 0; }
        }

        /// <summary>
        /// Compares the specified object with this <see cref="T:System.Reflection.Metadata.SignatureHeader"/> for equality.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns><see langword="true"/> if the objects are equal; otherwise, <see langword="false"/>.</returns>
        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is SignatureHeader signatureHeader && Equals(signatureHeader);
        }

        /// <summary>
        /// Compares two <see cref="T:System.Reflection.Metadata.SignatureHeader"/> values for equality.
        /// </summary>
        /// <param name="other">The value to compare.</param>
        /// <returns><see langword="true"/> if the values are equal; otherwise, <see langword="false"/>.</returns>
        public bool Equals(SignatureHeader other)
        {
            return _rawValue == other._rawValue;
        }

        /// <summary>
        /// Gets a hash code for the current object.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return _rawValue;
        }

        /// <summary>
        /// Compares two <see cref="T:System.Reflection.Metadata.SignatureHeader"/> values for equality.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns><see langword="true"/> if the values are equal; otherwise, <see langword="false"/>.</returns>
        public static bool operator ==(SignatureHeader left, SignatureHeader right)
        {
            return left._rawValue == right._rawValue;
        }

        /// <summary>
        /// Determines whether two <see cref="T:System.Reflection.Metadata.SignatureHeader"/> values are unequal.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns><see langword="true"/> if the values are unequal; otherwise, <see langword="false"/>.</returns>
        public static bool operator !=(SignatureHeader left, SignatureHeader right)
        {
            return left._rawValue != right._rawValue;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Kind.ToString());

            if (Kind == SignatureKind.Method)
            {
                sb.Append(',');
                sb.Append(CallingConvention.ToString());
            }

            if (Attributes != SignatureAttributes.None)
            {
                sb.Append(',');
                sb.Append(Attributes.ToString());
            }

            return sb.ToString();
        }
    }
}

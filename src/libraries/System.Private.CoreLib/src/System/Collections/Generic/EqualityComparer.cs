// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace System.Collections.Generic
{
    /// <summary>
    /// Provides a base class for implementations of the <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> generic interface.
    /// </summary>
    /// <typeparam name="T">The type of objects to compare.</typeparam>
    /// <remarks>
    /// Derive from this class to provide a custom implementation of the <see cref="System.Collections.Generic.IEqualityComparer{T}"/> generic interface for use with collection classes such as the <see cref="System.Collections.Generic.Dictionary{T,U}"/> generic class, or with methods such as <see cref="System.Collections.Generic.List{T}.Sort">Sort</see>.
    /// The <see cref="System.Collections.Generic.EqualityComparer{T}.Default"/> property checks whether type <c>T</c> implements the <see cref="System.IEquatable{T}">IEquatable{T}</see> generic interface and, if so, returns an <see cref="System.Collections.Generic.EqualityComparer{T}"/> that invokes the implementation of the <see cref="System.IEquatable{T}.Equals">Equals</see> method. Otherwise, it returns an <see cref="System.Collections.Generic.EqualityComparer{T}"/>, as provided by <c>T</c>.
    /// In .NET 8 and later versions, we recommend using the <see cref="System.Collections.Generic.EqualityComparer{T}.Create(System.Func{{},{},System.Boolean},System.Func{{},System.Int32})">Int32})</see> method to create instances of this type.
    /// The following example creates a dictionary collection of objects of type <c>Box</c> with an equality comparer. Two boxes are considered equal if their dimensions are the same. It then adds the boxes to the collection.
    /// The dictionary is recreated with an equality comparer that defines equality in a different way: Two boxes are considered equal if their volumes are the same.
    /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/EqualityComparerT/Overview/program.cs" id="Snippet1" />
    /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/EqualityComparerT/Overview/program.vb" id="Snippet1" />
    /// </remarks>
    [Serializable]
    [TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    public abstract partial class EqualityComparer<T> : IEqualityComparer, IEqualityComparer<T>
    {
        // public static EqualityComparer<T> Default is runtime-specific

        /// <summary>
        /// Creates an <see cref="EqualityComparer{T}"/> by using the specified delegates as the implementation of the comparer's
        /// <see cref="EqualityComparer{T}.Equals"/> and <see cref="EqualityComparer{T}.GetHashCode"/> methods.
        /// </summary>
        /// <param name="equals">The delegate to use to implement the <see cref="EqualityComparer{T}.Equals"/> method.</param>
        /// <param name="getHashCode">
        /// The delegate to use to implement the <see cref="EqualityComparer{T}.GetHashCode"/> method.
        /// If no delegate is supplied, calls to the resulting comparer's <see cref="EqualityComparer{T}.GetHashCode"/>
        /// will throw <see cref="NotSupportedException"/>.
        /// </param>
        /// <returns>The new comparer.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="equals"/> delegate was null.</exception>
        public static EqualityComparer<T> Create(Func<T?, T?, bool> equals, Func<T, int>? getHashCode = null)
        {
            ArgumentNullException.ThrowIfNull(equals);

            getHashCode ??= _ => throw new NotSupportedException();

            return new DelegateEqualityComparer<T>(equals, getHashCode);
        }

        /// <summary>
        /// When overridden in a derived class, determines whether two objects of type <typeparamref name="T"/> are equal.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns><see langword="true"/> if the specified objects are equal; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// The <see cref="System.Collections.Generic.EqualityComparer{T}.Equals"/> method is reflexive, symmetric, and transitive. That is, it returns <c>true</c> if used to compare an object with itself; <c>true</c> for two objects <c>x</c> and <c>y</c> if it is <c>true</c> for <c>y</c> and <c>x</c>; and <c>true</c> for two objects <c>x</c> and <c>z</c> if it is <c>true</c> for <c>x</c> and <c>y</c> and also <c>true</c> for <c>y</c> and <c>z</c>.
        /// </remarks>
        public abstract bool Equals(T? x, T? y);
        /// <summary>
        /// When overridden in a derived class, serves as a hash function for the specified object for hashing algorithms and data structures, such as a hash table.
        /// </summary>
        /// <param name="obj">The object for which to get a hash code.</param>
        /// <returns>A hash code for the specified object.</returns>
        /// <exception cref="T:System.ArgumentNullException">The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is <see langword="null"/>.</exception>
        public abstract int GetHashCode([DisallowNull] T obj);

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified object.</returns>
        /// <exception cref="T:System.ArgumentNullException">The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is <see langword="null"/>. -or- <paramref name="obj"/> is of a type that cannot be cast to type <typeparamref name="T"/>.</exception>
        /// <remarks>
        /// This method is a wrapper for the <see cref="System.Collections.Generic.EqualityComparer{T}.GetHashCode%28{}%29"/> method, so <c>obj</c> must be a type that can be cast to the type specified by the generic type argument <c>T</c> of the current instance.
        /// </remarks>
        int IEqualityComparer.GetHashCode(object? obj)
        {
            if (obj == null) return 0;
            if (obj is T) return GetHashCode((T)obj);
            ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidArgumentForComparison);
            return 0;
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns><see langword="true"/> if the specified objects are equal; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="T:System.ArgumentException"><paramref name="x"/> or <paramref name="y"/> is of a type that cannot be cast to type <typeparamref name="T"/>.</exception>
        /// <remarks>
        /// This method is a wrapper for the <see cref="System.Collections.Generic.EqualityComparer{T}.Equals%28{}%2C{}%29"/> method, so <c>obj</c> must be cast to the type specified by the generic argument <c>T</c> of the current instance. If it cannot be cast to <c>T</c>, an <see cref="System.ArgumentException"/> is thrown.
        /// Comparing <c>null</c> is allowed and does not generate an exception.
        /// </remarks>
        bool IEqualityComparer.Equals(object? x, object? y)
        {
            if (x == y) return true;
            if (x == null || y == null) return false;
            if ((x is T) && (y is T)) return Equals((T)x, (T)y);
            ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidArgumentForComparison);
            return false;
        }

#if !NATIVEAOT
        internal virtual int IndexOf(T[] array, T value, int startIndex, int count)
        {
            int endIndex = startIndex + count;
            for (int i = startIndex; i < endIndex; i++)
            {
                if (Equals(array[i], value))
                {
                    return i;
                }
            }
            return -1;
        }

        internal virtual int LastIndexOf(T[] array, T value, int startIndex, int count)
        {
            int endIndex = startIndex - count + 1;
            for (int i = startIndex; i >= endIndex; i--)
            {
                if (Equals(array[i], value))
                {
                    return i;
                }
            }
            return -1;
        }
#endif
    }

    internal sealed class DelegateEqualityComparer<T> : EqualityComparer<T>
    {
        private readonly Func<T?, T?, bool> _equals;
        private readonly Func<T, int> _getHashCode;

        public DelegateEqualityComparer(Func<T?, T?, bool> equals, Func<T, int> getHashCode)
        {
            _equals = equals;
            _getHashCode = getHashCode;
        }

        public override bool Equals(T? x, T? y) =>
            _equals(x, y);

        public override int GetHashCode([DisallowNull] T obj) =>
            _getHashCode(obj);

        public override bool Equals(object? obj) =>
            obj is DelegateEqualityComparer<T> other &&
            _equals == other._equals &&
            _getHashCode == other._getHashCode;

        public override int GetHashCode() =>
            HashCode.Combine(_equals.GetHashCode(), _getHashCode.GetHashCode());
    }

    // The methods in this class look identical to the inherited methods, but the calls
    // to Equal bind to IEquatable<T>.Equals(T) instead of Object.Equals(Object)
    [Serializable]
    [TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    // Needs to be public to support binary serialization compatibility
    public sealed partial class GenericEqualityComparer<T> : EqualityComparer<T> where T : IEquatable<T>?
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(T? x, T? y)
        {
            if (x != null)
            {
                if (y != null) return x.Equals(y);
                return false;
            }
            if (y != null) return false;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode([DisallowNull] T obj) =>
            obj?.GetHashCode() ?? 0;

        // Equals method for the comparer itself.
        public override bool Equals([NotNullWhen(true)] object? obj) =>
            obj != null && GetType() == obj.GetType();

        public override int GetHashCode() =>
            GetType().GetHashCode();
    }

    [Serializable]
    [TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    // Needs to be public to support binary serialization compatibility
    public sealed partial class NullableEqualityComparer<T> : EqualityComparer<T?>, ISerializable where T : struct
    {
        public NullableEqualityComparer() { }
        private NullableEqualityComparer(SerializationInfo info, StreamingContext context) { }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (!typeof(T).IsAssignableTo(typeof(IEquatable<T>)))
            {
                // We used to use NullableComparer only for types implementing IEquatable<T>
                info.SetType(typeof(ObjectEqualityComparer<T?>));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(T? x, T? y)
        {
            if (x.HasValue)
            {
                if (y.HasValue) return EqualityComparer<T>.Default.Equals(x.value, y.value);
                return false;
            }
            if (y.HasValue) return false;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode(T? obj) =>
            obj.GetHashCode();

        // Equals method for the comparer itself.
        public override bool Equals([NotNullWhen(true)] object? obj) =>
            obj != null && GetType() == obj.GetType();

        public override int GetHashCode() =>
            GetType().GetHashCode();
    }

    [Serializable]
    [TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    // Needs to be public to support binary serialization compatibility
    public sealed partial class ObjectEqualityComparer<T> : EqualityComparer<T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(T? x, T? y)
        {
            if (x != null)
            {
                if (y != null) return x.Equals(y);
                return false;
            }
            if (y != null) return false;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode([DisallowNull] T obj) =>
            obj?.GetHashCode() ?? 0;

        // Equals method for the comparer itself.
        public override bool Equals([NotNullWhen(true)] object? obj) =>
            obj != null && GetType() == obj.GetType();

        public override int GetHashCode() =>
            GetType().GetHashCode();
    }

    [Serializable]
    [TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    // Needs to be public to support binary serialization compatibility
    public sealed partial class ByteEqualityComparer : EqualityComparer<byte>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(byte x, byte y) =>
            x == y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode(byte b) =>
            b.GetHashCode();

        // Equals method for the comparer itself.
        public override bool Equals([NotNullWhen(true)] object? obj) =>
            obj != null && GetType() == obj.GetType();

        public override int GetHashCode() =>
            GetType().GetHashCode();
    }

    [Serializable]
    [TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    // Needs to be public to support binary serialization compatibility
    public sealed partial class EnumEqualityComparer<T> : EqualityComparer<T>, ISerializable where T : struct, Enum
    {
        public EnumEqualityComparer() { }

        // This is used by the serialization engine.
        private EnumEqualityComparer(SerializationInfo information, StreamingContext context) { }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // For back-compat we need to serialize the comparers for enums with underlying types other than int as ObjectEqualityComparer
            if (Type.GetTypeCode(typeof(T)) != TypeCode.Int32)
            {
                info.SetType(typeof(ObjectEqualityComparer<T>));
            }
        }

        // public override bool Equals(T x, T y) is runtime-specific

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode(T obj) =>
            obj.GetHashCode();

        // Equals method for the comparer itself.
        public override bool Equals([NotNullWhen(true)] object? obj) =>
            obj != null && GetType() == obj.GetType();

        public override int GetHashCode() =>
            GetType().GetHashCode();
    }

    // This class exists to be EqualityComparer<string>.Default. It can't just use the GenericEqualityComparer<string>,
    // as it needs to also implement IAlternateEqualityComparer<ReadOnlySpan<char>, string>, and it can't be
    // StringComparer.Ordinal, as that doesn't derive from the required abstract EqualityComparer<T> base class.
    [Serializable]
    internal sealed partial class StringEqualityComparer :
        EqualityComparer<string>,
        IAlternateEqualityComparer<ReadOnlySpan<char>, string>,
        ISerializable
    {
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // This type is added as an internal implementation detail in .NET 9. Even though as of .NET 9 BinaryFormatter has been
            // deprecated, for back compat we still need to support serializing this type, especially when EqualityComparer<string>.Default
            // is used as part of a collection, like Dictionary<string, TValue>.
            //
            // BinaryFormatter treats types in the core library as being special, in that it doesn't include the assembly as part of the
            // serialized data, and then on deserialization it assumes the type is in mscorlib. We could make the type public and type forward
            // it from the mscorlib shim, which would enable roundtripping on .NET 9+, but because this type doesn't exist downlevel, it would
            // break serializing on .NET 9+ and deserializing downlevel. Therefore, we need to serialize as something that exists downlevel.

            // We could serialize as OrdinalComparer, which does exist downlevel, and which has the nice property that it also implements
            // IAlternateEqualityComparer<ReadOnlySpan<char>, string>, which means serializing an instance on .NET 9+ and deserializing it
            // on .NET 9+ would continue to support span-based lookups. However, OrdinalComparer is not an EqualityComparer<string>, which
            // means the type's public ancestry would not be retained, which could lead to strange casting-related errors, including downlevel.

            // Instead, we can serialize as a GenericEqualityComparer<string>. This exists downlevel and also derives from EqualityComparer<string>,
            // but doesn't implement IAlternateEqualityComparer<ReadOnlySpan<char>, string>. This means that upon deserializing on .NET 9+,
            // the comparer loses its ability to handle span-based lookups. As BinaryFormatter is deprecated on .NET 9+, this is a readonable tradeoff.

            info.SetType(typeof(GenericEqualityComparer<string>));
        }

        public override bool Equals(string? x, string? y) => string.Equals(x, y);

        public override int GetHashCode([DisallowNull] string obj) => obj?.GetHashCode() ?? 0;

        public bool Equals(ReadOnlySpan<char> span, string target)
        {
            // See explanation in OrdinalComparer.Equals.
            if (span.IsEmpty && target is null)
            {
                return false;
            }

            return span.SequenceEqual(target);
        }

        public int GetHashCode(ReadOnlySpan<char> span) => string.GetHashCode(span);

        public string Create(ReadOnlySpan<char> span) => span.ToString();

        public override bool Equals(object? obj) => obj is StringEqualityComparer;
        public override int GetHashCode() => GetType().GetHashCode();
    }
}

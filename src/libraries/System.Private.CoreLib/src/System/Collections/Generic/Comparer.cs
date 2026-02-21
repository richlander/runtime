// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace System.Collections.Generic
{
    /// <summary>
    /// Provides a base class for implementations of the <see cref="T:System.Collections.Generic.IComparer`1"/> generic interface.
    /// </summary>
    /// <typeparam name="T">The type of objects to compare.</typeparam>
    /// <remarks>
    /// Derive from this class to provide a custom implementation of the <see cref="System.Collections.Generic.IComparer{T}"/> interface for use with collection classes such as the <see cref="System.Collections.Generic.SortedList{T,U}"/> and <see cref="System.Collections.Generic.SortedDictionary{T,U}"/> generic classes.
    /// The difference between deriving from the <see cref="System.Collections.Generic.Comparer{T}"/> class and implementing the <see cref="System.IComparable">IComparable</see> interface is as follows:
    /// - To specify how two objects should be compared by default, implement the <see cref="System.IComparable">IComparable</see> interface in your class. This ensures that sort operations will use the default comparison code that you provided.
    /// - To define a comparer to use instead of the default comparer, derive from the <see cref="System.Collections.Generic.Comparer{T}"/> class. You can then use this comparer in sort operations that take a comparer as a parameter.
    /// The object returned by the <see cref="System.Collections.Generic.Comparer{T}.Default"/> property uses the <see cref="System.IComparable{T}">IComparable{T}</see> generic interface (<c>IComparable&lt;T&gt;</c> in C#, <c>IComparable(Of T)</c> in Visual Basic) to compare two objects. If type <c>T</c> does not implement the <see cref="System.IComparable{T}">IComparable{T}</see> generic interface, the <see cref="System.Collections.Generic.Comparer{T}.Default"/> property returns a <see cref="System.Collections.Generic.Comparer{T}"/> that uses the <see cref="System.IComparable">IComparable</see> interface.
    /// The following example derives a class, <c>BoxLengthFirst</c>, from the <see cref="System.Collections.Generic.Comparer{T}"/> class. This comparer compares two objects of type <c>Box</c>. It sorts them first by length, then by height, and then by width. The <c>Box</c> class implements the <see cref="System.IComparable{T}"/> interface to control the default comparison between two <c>Box</c> objects. This default implementation sorts first by height, then by length, and then by width. The example shows the differences between the two comparisons by sorting a list of <c>Box</c> objects first by using the <c>BoxLengthFirst</c> comparer and then by using the default comparer.
    /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/ComparerT/Overview/program.cs" id="Snippet1" />
    /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/ComparerT/Overview/program.fs" id="Snippet1" />
    /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ComparerT/Overview/program.vb" id="Snippet1" />
    /// </remarks>
    [Serializable]
    [TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    public abstract partial class Comparer<T> : IComparer, IComparer<T>
    {
        // public static Comparer<T> Default is runtime-specific

        /// <summary>
        /// Creates a comparer by using the specified comparison.
        /// </summary>
        /// <param name="comparison">The comparison to use.</param>
        /// <returns>The new comparer.</returns>
        public static Comparer<T> Create(Comparison<T> comparison)
        {
            ArgumentNullException.ThrowIfNull(comparison);

            return new ComparisonComparer<T>(comparison);
        }

        /// <summary>
        /// When overridden in a derived class, performs a comparison of two objects of the same type and returns a value indicating whether one object is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>, as shown in the following table. <list type="table"><listheader><term> Value </term><description> Meaning </description></listheader><item><term> Less than zero </term><description><paramref name="x" /> is less than <paramref name="y" />. </description></item><item><term> Zero </term><description><paramref name="x" /> equals <paramref name="y" />. </description></item><item><term> Greater than zero </term><description><paramref name="x" /> is greater than <paramref name="y" />. </description></item></list></returns>
        /// <exception cref="T:System.ArgumentException">Type <code>T</code> does not implement either the <see cref="T:System.IComparable`1"/> generic interface or the <see cref="T:System.IComparable"/> interface.</exception>
        /// <remarks>
        /// Implement this method to provide a customized sort order comparison for type <c>T</c>.
        /// The following example defines a comparer of <c>Box</c> objects that can be used instead of the default comparer. This example is part of a larger example provided for the <see cref="System.Collections.Generic.Comparer{T}"/> class.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/ComparerT/Overview/program.cs" id="Snippet5" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/ComparerT/Overview/program.fs" id="Snippet5" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ComparerT/Overview/program.vb" id="Snippet5" />
        /// </remarks>
        public abstract int Compare(T? x, T? y);

        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>, as shown in the following table. <list type="table"><listheader><term> Value </term><description> Meaning </description></listheader><item><term> Less than zero </term><description><paramref name="x" /> is less than <paramref name="y" />. </description></item><item><term> Zero </term><description><paramref name="x" /> equals <paramref name="y" />. </description></item><item><term> Greater than zero </term><description><paramref name="x" /> is greater than <paramref name="y" />. </description></item></list></returns>
        /// <exception cref="T:System.ArgumentException"><paramref name="x"/> or <paramref name="y"/> is of a type that cannot be cast to type <code>T</code>. -or- <paramref name="x"/> and <paramref name="y"/> do not implement either the <see cref="T:System.IComparable`1"/> generic interface or the <see cref="T:System.IComparable"/> interface.</exception>
        /// <remarks>
        /// This method is a wrapper for the <see cref="System.Collections.Generic.Comparer{T}.Compare%28{}%2C{}%29"/> method, so <c>obj</c> must be cast to the type specified by the generic argument <c>T</c> of the current instance. If it cannot be cast to <c>T</c>, an <see cref="System.ArgumentException"/> is thrown.
        /// Comparing <c>null</c> with any reference type is allowed and does not generate an exception. When sorting, <c>null</c> is considered to be less than any other object.
        /// The following example shows how to use the <see cref="System.Collections.Generic.Comparer{T}.System#Collections#IComparer#Compare"/> method to compare two objects. This example is part of a larger example provided for the <see cref="System.Collections.Generic.Comparer{T}"/> class.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/ComparerT/Overview/program.cs" id="Snippet4" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/ComparerT/Overview/program.fs" id="Snippet4" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ComparerT/Overview/program.vb" id="Snippet4" />
        /// </remarks>
        int IComparer.Compare(object? x, object? y)
        {
            if (x == null) return y == null ? 0 : -1;
            if (y == null) return 1;
            if (x is T && y is T) return Compare((T)x, (T)y);
            ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidArgumentForComparison);
            return 0;
        }
    }

    internal sealed class ComparisonComparer<T> : Comparer<T>
    {
        private readonly Comparison<T> _comparison;

        public ComparisonComparer(Comparison<T> comparison)
        {
            _comparison = comparison;
        }

        public override int Compare(T? x, T? y) => _comparison(x!, y!);
    }

    // Note: although there is a lot of shared code in the following
    // comparers, we do not incorporate it into a base class for perf
    // reasons. Adding another base class (even one with no fields)
    // means another generic instantiation, which can be costly esp.
    // for value types.
    [Serializable]
    [TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    // Needs to be public to support binary serialization compatibility
    public sealed partial class GenericComparer<T> : Comparer<T> where T : IComparable<T>?
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int Compare(T? x, T? y)
        {
            if (x != null)
            {
                if (y != null) return x.CompareTo(y);
                return 1;
            }
            if (y != null) return -1;
            return 0;
        }

        // Equals method for the comparer itself.
        public override bool Equals([NotNullWhen(true)] object? obj) =>
            obj != null && GetType() == obj.GetType();

        public override int GetHashCode() =>
            GetType().GetHashCode();
    }

    [Serializable]
    [TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    // Needs to be public to support binary serialization compatibility
    public sealed class NullableComparer<T> : Comparer<T?>, ISerializable where T : struct
    {
        public NullableComparer() { }
        private NullableComparer(SerializationInfo info, StreamingContext context) { }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (!typeof(T).IsAssignableTo(typeof(IComparable<T>)))
            {
                // We used to use NullableComparer only for types implementing IComparable<T>
                info.SetType(typeof(ObjectComparer<T?>));
            }
        }

        public override int Compare(T? x, T? y)
        {
            if (x.HasValue)
            {
                if (y.HasValue) return Comparer<T>.Default.Compare(x.value, y.value);
                return 1;
            }
            if (y.HasValue) return -1;
            return 0;
        }

        // Equals method for the comparer itself.
        public override bool Equals([NotNullWhen(true)] object? obj) =>
            obj != null && GetType() == obj.GetType();

        public override int GetHashCode() =>
            GetType().GetHashCode();
    }

    [Serializable]
    [TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    // Needs to be public to support binary serialization compatibility
    public sealed partial class ObjectComparer<T> : Comparer<T>
    {
        public override int Compare(T? x, T? y)
        {
            return Comparer.Default.Compare(x, y);
        }

        // Equals method for the comparer itself.
        public override bool Equals([NotNullWhen(true)] object? obj) =>
            obj != null && GetType() == obj.GetType();

        public override int GetHashCode() =>
            GetType().GetHashCode();
    }

    [Serializable]
    internal sealed partial class EnumComparer<T> : Comparer<T>, ISerializable where T : struct, Enum
    {
        public EnumComparer() { }

        // Used by the serialization engine.
        private EnumComparer(SerializationInfo info, StreamingContext context) { }

        // public override int Compare(T x, T y) is runtime-specific

        // Equals method for the comparer itself.
        public override bool Equals([NotNullWhen(true)] object? obj) =>
            obj != null && GetType() == obj.GetType();

        public override int GetHashCode() =>
            GetType().GetHashCode();

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Previously Comparer<T> was not specialized for enums,
            // and instead fell back to ObjectComparer which uses boxing.
            // Set the type as ObjectComparer here so code that serializes
            // Comparer for enums will not break.
            info.SetType(typeof(ObjectComparer<T>));
        }
    }
}

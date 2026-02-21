// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;

namespace System.Collections.Generic
{
    // The generic IEqualityComparer interface implements methods to check if two objects are equal
    // and generate Hashcode for an object.
    // It is used in Dictionary class.
    /// <summary>
    /// Defines methods to support the comparison of objects for equality.
    /// </summary>
    /// <typeparam name="T">The type of objects to compare.</typeparam>
    /// <remarks>
    /// This interface allows the implementation of customized equality comparison for collections. That is, you can create your own definition of equality for type <c>T</c>, and specify that this definition be used with a collection type that accepts the <see cref="System.Collections.Generic.IEqualityComparer{T}"/> generic interface. In the .NET Framework, constructors of the <see cref="System.Collections.Generic.Dictionary{T,U}"/> generic collection type accept this interface.
    /// A default implementation of this interface is provided by the <see cref="System.Collections.Generic.EqualityComparer{T}.Default"/> property of the <see cref="System.Collections.Generic.EqualityComparer{T}"/> generic class. The <see cref="System.StringComparer"/> class implements <see cref="System.Collections.Generic.IEqualityComparer{T}"/> of type <see cref="System.String"/>.
    /// This interface supports only equality comparisons. Customization of comparisons for sorting and ordering is provided by the <see cref="System.Collections.Generic.IComparer{T}"/> generic interface.
    /// We recommend that you derive from the <see cref="System.Collections.Generic.EqualityComparer{T}"/> class instead of implementing the <see cref="System.Collections.Generic.IEqualityComparer{T}"/> interface, because the <see cref="System.Collections.Generic.EqualityComparer{T}"/> class tests for equality using the <see cref="System.IEquatable{T}.Equals">Equals</see> method instead of the <see cref="System.Object.Equals">Equals</see> method. This is consistent with the <c>Contains</c>, <c>IndexOf</c>, <c>LastIndexOf</c>, and <c>Remove</c> methods of the <see cref="System.Collections.Generic.Dictionary{T,U}"/> class and other generic collections.
    /// The following example adds custom <c>Box</c> objects to a dictionary collection. The <c>Box</c> objects are considered equal if their dimensions are the same.
    /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/IEqualityComparerT/Overview/program.cs" id="Snippet1" />
    /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/IEqualityComparerT/Overview/program.vb" id="Snippet1" />
    /// </remarks>
    public interface IEqualityComparer<in T> where T : allows ref struct
    {
        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type <paramref name="T"/> to compare.</param>
        /// <param name="y">The second object of type <paramref name="T"/> to compare.</param>
        /// <returns><see langword="true"/> if the specified objects are equal; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// Implement this method to provide a customized equality comparison for type <c>T</c>.
        /// </remarks>
        bool Equals(T? x, T? y);
        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified object.</returns>
        /// <exception cref="T:System.ArgumentNullException">The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// Implement this method to provide a customized hash code for type <c>T</c>, corresponding to the customized equality comparison provided by the <see cref="System.Collections.Generic.IEqualityComparer{T}.Equals"/> method.
        /// </remarks>
        int GetHashCode([DisallowNull] T obj);
    }
}

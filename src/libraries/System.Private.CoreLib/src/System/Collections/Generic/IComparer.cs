// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;

namespace System.Collections.Generic
{
    // The generic IComparer interface implements a method that compares
    // two objects. It is used in conjunction with the Sort and
    // BinarySearch methods on the Array, List, and SortedList classes.
    /// <summary>
    /// Defines a method that a type implements to compare two objects.
    /// </summary>
    /// <typeparam name="T">The type of objects to compare.</typeparam>
    /// <remarks>
    /// This interface is used with the <see cref="System.Collections.Generic.List{T}.Sort">Sort</see> and <see cref="System.Collections.Generic.List{T}.BinarySearch">BinarySearch</see> methods. It provides a way to customize the sort order of a collection. Classes that implement this interface include the <see cref="System.Collections.Generic.SortedDictionary{T,U}"/> and <see cref="System.Collections.Generic.SortedList{T,U}"/> generic classes.
    /// The default implementation of this interface is the <see cref="System.Collections.Generic.Comparer{T}"/> class. The <see cref="System.StringComparer"/> class implements this interface for type <see cref="System.String"/>.
    /// This interface supports ordering comparisons. That is, when the <see cref="System.Collections.Generic.Comparer{T}.Compare"/> method returns 0, it means that two objects sort the same. Implementation of exact equality comparisons is provided by the <see cref="System.Collections.Generic.IEqualityComparer{T}"/> generic interface.
    /// We recommend that you derive from the <see cref="System.Collections.Generic.Comparer{T}"/> class instead of implementing the <see cref="System.Collections.Generic.IComparer{T}"/> interface, because the <see cref="System.Collections.Generic.Comparer{T}"/> class provides an explicit interface implementation of the <see cref="System.Collections.Generic.Comparer{T}.System#Collections#IComparer#Compare"/> method and the <see cref="System.Collections.Generic.Comparer{T}.Default"/> property that gets the default comparer for the object.
    /// The following example implements the <see cref="System.Collections.Generic.IComparer{T}"/> interface to compare objects of type <c>Box</c> according to their dimensions. This example is part of a larger example provided for the <see cref="System.Collections.Generic.Comparer{T}"/> class.
    /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/ComparerT/Overview/program.cs" id="Snippet7" />
    /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ComparerT/Overview/program.vb" id="Snippet7" />
    /// </remarks>
    public interface IComparer<in T> where T : allows ref struct
    {
        // Compares two objects. An implementation of this method must return a
        // value less than zero if x is less than y, zero if x is equal to y, or a
        // value greater than zero if x is greater than y.
        //
        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>, as shown in the following table. <list type="table"><listheader><term> Value </term><description> Meaning </description></listheader><item><term> Less than zero </term><description><paramref name="x" /> is less than <paramref name="y" />. </description></item><item><term> Zero </term><description><paramref name="x" /> equals <paramref name="y" />. </description></item><item><term> Greater than zero </term><description><paramref name="x" /> is greater than <paramref name="y" />. </description></item></list></returns>
        /// <remarks>
        /// Implement this method to provide a customized sort order comparison for type <c>T</c>.
        /// Comparing <c>null</c> with any reference type is allowed and does not generate an exception. A null reference is considered to be less than any reference that is not null.
        /// The following example implements the <see cref="System.Collections.Generic.IComparer{T}"/> interface to compare objects of type <c>Box</c> according to their dimensions. This example is part of a larger example provided for the <see cref="System.Collections.Generic.Comparer{T}"/> class.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/ComparerT/Overview/program.cs" id="Snippet7" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ComparerT/Overview/program.vb" id="Snippet7" />
        /// </remarks>
        int Compare(T? x, T? y);
    }
}

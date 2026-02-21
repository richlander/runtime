// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if MONO
using System.Diagnostics.CodeAnalysis;
#endif

namespace System.Collections.Generic
{
    // Provides a read-only, covariant view of a generic list.
    /// <summary>
    /// Represents a read-only collection of elements that can be accessed by index.
    /// </summary>
    /// <typeparam name="T">The type of elements in the read-only list.</typeparam>
    /// <remarks>
    /// The <see cref="System.Collections.Generic.IReadOnlyList{T}"/> represents a list in which the number and order of list elements is read-only. The content of list elements is not guaranteed to be read-only.
    /// </remarks>
    public interface IReadOnlyList<out T> : IReadOnlyCollection<T>
    {
        /// <summary>
        /// Gets the element at the specified index in the read-only list.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <value>The element at the specified index in the read-only list.</value>
        T this[int index]
        {
#if MONO
            [DynamicDependency(nameof(Array.InternalArray__IReadOnlyList_get_Item) + "``1", typeof(Array))]
#endif
            get;
        }
    }
}

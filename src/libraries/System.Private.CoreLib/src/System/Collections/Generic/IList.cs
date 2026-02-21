// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if MONO
using System.Diagnostics.CodeAnalysis;
#endif

namespace System.Collections.Generic
{
    // An IList is an ordered collection of objects.  The exact ordering
    // is up to the implementation of the list, ranging from a sorted
    // order to insertion order.
    /// <summary>
    /// Represents a collection of objects that can be individually accessed by index.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <remarks>
    /// The <see cref="System.Collections.Generic.IList{T}"/> generic interface is a descendant of the <see cref="System.Collections.Generic.ICollection{T}"/> generic interface and is the base interface of all generic lists.
    /// </remarks>
    public interface IList<T> : ICollection<T>
    {
        // The Item property provides methods to read and edit entries in the List.
        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <value>The element at the specified index.</value>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception>
        /// <exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IList`1"/> is read-only.</exception>
        /// <remarks>
        /// This property provides the ability to access a specific element in the collection by using the following syntax: <c>myCollection[index]</c>.
        /// The C# language uses the [this](/dotnet/csharp/language-reference/keywords/this) keyword to define the indexers instead of implementing the <see cref="System.Collections.Generic.IList{T}.Item"/> property. Visual Basic implements <see cref="System.Collections.Generic.IList{T}.Item"/> as a [default property](/dotnet/visual-basic/language-reference/modifiers/default), which provides the same indexing functionality.
        /// </remarks>
        T this[int index]
        {
#if MONO
            [DynamicDependency(nameof(Array.InternalArray__get_Item) + "``1", typeof(Array))]
#endif
            get;
#if MONO
            [DynamicDependency(nameof(Array.InternalArray__set_Item) + "``1", typeof(Array))]
#endif
            set;
        }

        // Returns the index of a particular item, if it is in the list.
        // Returns -1 if the item isn't in the list.
#if MONO
        [DynamicDependency(nameof(Array.InternalArray__IndexOf) + "``1", typeof(Array))]
#endif
        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        /// <returns>The index of <paramref name="item"/> if found in the list; otherwise, -1.</returns>
        /// <remarks>
        /// If an object occurs multiple times in the list, the <see cref="System.Collections.Generic.IList{T}.IndexOf"/> method always returns the first instance found.
        /// </remarks>
        int IndexOf(T item);

        // Inserts value into the list at position index.
        // index must be non-negative and less than or equal to the
        // number of elements in the list.  If index equals the number
        // of items in the list, then value is appended to the end.
#if MONO
        [DynamicDependency(nameof(Array.InternalArray__Insert) + "``1", typeof(Array))]
#endif
        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.</exception>
        /// <remarks>
        /// If <c>index</c> equals the number of items in the <see cref="System.Collections.Generic.IList{T}"/>, then <c>item</c> is appended to the list.
        /// In collections of contiguous elements, such as lists, the elements that follow the insertion point move down to accommodate the new element. If the collection is indexed, the indexes of the elements that are moved are also updated. This behavior does not apply to collections where elements are conceptually grouped into buckets, such as a hash table.
        /// </remarks>
        void Insert(int index, T item);

        // Removes the item at position index.
#if MONO
        [DynamicDependency(nameof(Array.InternalArray__RemoveAt), typeof(Array))]
#endif
        /// <summary>
        /// Removes the <see cref="T:System.Collections.Generic.IList`1"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.</exception>
        /// <remarks>
        /// In collections of contiguous elements, such as lists, the elements that follow the removed element move up to occupy the vacated spot. If the collection is indexed, the indexes of the elements that are moved are also updated. This behavior does not apply to collections where elements are conceptually grouped into buckets, such as a hash table.
        /// </remarks>
        void RemoveAt(int index);
    }
}

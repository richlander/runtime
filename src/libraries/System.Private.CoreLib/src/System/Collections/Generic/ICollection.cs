// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if MONO
using System.Diagnostics.CodeAnalysis;
#endif

namespace System.Collections.Generic
{
    // Base interface for all collections, defining enumerators, size, and
    // synchronization methods.
    /// <summary>
    /// Defines methods to manipulate generic collections.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    /// <remarks>
    /// The <see cref="System.Collections.Generic.ICollection{T}"/> interface is the base interface for classes in the <see cref="System.Collections.Generic">Generic</see> namespace.
    /// The <see cref="System.Collections.Generic.ICollection{T}"/> interface extends <see cref="System.Collections.Generic.IEnumerable{T}"/>; <see cref="System.Collections.Generic.IDictionary{T,U}"/> and <see cref="System.Collections.Generic.IList{T}"/> are more specialized interfaces that extend <see cref="System.Collections.Generic.ICollection{T}"/>. A <see cref="System.Collections.Generic.IDictionary{T,U}"/> implementation is a collection of key/value pairs, like the <see cref="System.Collections.Generic.Dictionary{T,U}"/> class. A <see cref="System.Collections.Generic.IList{T}"/> implementation is a collection of values, and its members can be accessed by index, like the <see cref="System.Collections.Generic.List{T}"/> class.
    /// If neither the <see cref="System.Collections.Generic.IDictionary{T,U}"/> interface nor the <see cref="System.Collections.Generic.IList{T}"/> interface meet the requirements of the required collection, derive the new collection class from the <see cref="System.Collections.Generic.ICollection{T}"/> interface instead for more flexibility.
    /// The following example implements the <see cref="System.Collections.Generic.ICollection{T}"/> interface to create a collection of custom <c>Box</c> objects named <c>BoxCollection</c>. Each <c>Box</c> has height, length, and width properties, which are used to define equality. Equality can be defined as all dimensions being the same or the volume being the same. The <c>Box</c> class implements the <see cref="System.IEquatable{T}"/> interface to define the default equality as the dimensions being the same.
    /// The <c>BoxCollection</c> class implements the <see cref="System.Collections.Generic.ICollection{T}.Contains"/> method to use the default equality to determine whether a <c>Box</c> is in the collection. This method is used by the <see cref="System.Collections.Generic.ICollection{T}.Add"/> method so that each <c>Box</c> added to the collection has a unique set of dimensions. The <c>BoxCollection</c> class also provides an overload of the  <see cref="System.Collections.Generic.ICollection{T}.Contains"/> method that takes a specified <see cref="System.Collections.Generic.EqualityComparer{T}"/> object, such as <c>BoxSameDimensions</c> and <c>BoxSameVol</c> classes in the example.
    /// This example also implements an <see cref="System.Collections.Generic.IEnumerator{T}"/> interface for the <c>BoxCollection</c> class so that the collection can be enumerated.
    /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/ICollectionT/Overview/program.cs" id="Snippet1" />
    /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ICollectionT/Overview/program.vb" id="Snippet1" />
    /// </remarks>
    public interface ICollection<T> : IEnumerable<T>
    {
        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <value>The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</value>
        int Count
        {
#if MONO
            [DynamicDependency(nameof(Array.InternalArray__ICollection_get_Count), typeof(Array))]
#endif
            get;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <value><see langword="true"/> if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, <see langword="false"/>.</value>
        /// <remarks>
        /// A collection that is read-only does not allow the addition or removal of elements after the collection is created. Note that read-only in this context does not indicate whether individual elements of the collection can be modified, since the <see cref="System.Collections.Generic.ICollection{T}"/> interface only supports addition and removal operations. For example, the <see cref="System.Collections.Generic.ICollection{T}.IsReadOnly"/> property of an array that is cast or converted to an <see cref="System.Collections.Generic.ICollection{T}"/> object returns <c>true</c>, even though individual array elements can be modified.
        /// </remarks>
        bool IsReadOnly
        {
#if MONO
            [DynamicDependency(nameof(Array.InternalArray__ICollection_get_IsReadOnly), typeof(Array))]
#endif
            get;
        }

#if MONO
        [DynamicDependency(nameof(Array.InternalArray__ICollection_Add) + "``1", typeof(Array))]
#endif
        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        void Add(T item);

#if MONO
        [DynamicDependency(nameof(Array.InternalArray__ICollection_Clear), typeof(Array))]
#endif
        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        /// <remarks>
        /// <see cref="System.Collections.Generic.ICollection{T}.Count"/> must be set to 0, and references to other objects from elements of the collection must be released.
        /// </remarks>
        void Clear();

#if MONO
        [DynamicDependency(nameof(Array.InternalArray__ICollection_Contains) + "``1", typeof(Array))]
#endif
        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// Implementations can vary in how they determine equality of objects; for example, <see cref="System.Collections.Generic.List{T}"/> uses <see cref="System.Collections.Generic.Comparer{T}.Default">Default</see>, whereas <see cref="System.Collections.Generic.Dictionary{T,U}"/> allows the user to specify the <see cref="System.Collections.Generic.IComparer{T}"/> implementation to use for comparing keys.
        /// </remarks>
        bool Contains(T item);

        // CopyTo copies a collection into an Array, starting at a particular
        // index into the array.
#if MONO
        [DynamicDependency(nameof(Array.InternalArray__ICollection_CopyTo) + "``1", typeof(Array))]
#endif
        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception>
        /// <exception cref="T:System.ArgumentException">The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.</exception>
        void CopyTo(T[] array, int arrayIndex);

#if MONO
        [DynamicDependency(nameof(Array.InternalArray__ICollection_Remove) + "``1", typeof(Array))]
#endif
        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, <see langword="false"/>. This method also returns <see langword="false"/> if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.</returns>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        /// <remarks>
        /// Implementations can vary in how they determine equality of objects; for example, <see cref="System.Collections.Generic.List{T}"/> uses <see cref="System.Collections.Generic.Comparer{T}.Default">Default</see>, whereas, <see cref="System.Collections.Generic.Dictionary{T,U}"/> allows the user to specify the <see cref="System.Collections.Generic.IComparer{T}"/> implementation to use for comparing keys.
        /// In collections of contiguous elements, such as lists, the elements that follow the removed element move up to occupy the vacated spot. If the collection is indexed, the indexes of the elements that are moved are also updated. This behavior does not apply to collections where elements are conceptually grouped into buckets, such as a hash table.
        /// </remarks>
        bool Remove(T item);
    }
}

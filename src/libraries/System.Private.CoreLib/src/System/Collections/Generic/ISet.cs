// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Collections.Generic
{
    /// <summary>
    /// Generic collection that guarantees the uniqueness of its elements, as defined
    /// by some comparer. It also supports basic set operations such as Union, Intersection,
    /// Complement and Exclusive Complement.
    /// </summary>
    public interface ISet<T> : ICollection<T>
    {
        //Add ITEM to the set, return true if added, false if duplicate
        /// <summary>
        /// Adds an element to the current set and returns a value to indicate if the element was successfully added.
        /// </summary>
        /// <param name="item">The element to add to the set.</param>
        /// <returns><see langword="true"/> if the element is added to the set; <see langword="false"/> if the element is already in the set.</returns>
        new bool Add(T item);

        //Transform this set into its union with the IEnumerable<T> other
        /// <summary>
        /// Modifies the current set so that it contains all elements that are present in the current set, in the specified collection, or in both.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// Any duplicate elements in <c>other</c> are ignored.
        /// </remarks>
        void UnionWith(IEnumerable<T> other);

        //Transform this set into its intersection with the IEnumerable<T> other
        /// <summary>
        /// Modifies the current set so that it contains only elements that are also in a specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// This method ignores any duplicate elements in <c>other</c>.
        /// </remarks>
        void IntersectWith(IEnumerable<T> other);

        //Transform this set so it contains no elements that are also in other
        /// <summary>
        /// Removes all elements in the specified collection from the current set.
        /// </summary>
        /// <param name="other">The collection of items to remove from the set.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is the number of elements in the <c>other</c> parameter.
        /// </remarks>
        void ExceptWith(IEnumerable<T> other);

        //Transform this set so it contains elements initially in this or in other, but not both
        /// <summary>
        /// Modifies the current set so that it contains only elements that are present either in the current set or in the specified collection, but not both.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// Any duplicate elements in <c>other</c> are ignored.
        /// </remarks>
        void SymmetricExceptWith(IEnumerable<T> other);

        //Check if this set is a subset of other
        /// <summary>
        /// Determines whether a set is a subset of a specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <returns><see langword="true"/> if the current set is a subset of <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// If <c>other</c> contains the same elements as the current set, the current set is still considered a subset of other.
        /// This method always returns <c>false</c> if the current set has elements that are not in <c>other</c>.
        /// </remarks>
        bool IsSubsetOf(IEnumerable<T> other);

        //Check if this set is a superset of other
        /// <summary>
        /// Determines whether the current set is a superset of a specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <returns><see langword="true"/> if the current set is a superset of <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// If <c>other</c> contains the same elements as the current set, the current set is still considered a superset of <c>other</c>.
        /// This method always returns <c>false</c> if the current set has fewer elements than <c>other</c>.
        /// </remarks>
        bool IsSupersetOf(IEnumerable<T> other);

        //Check if this set is a subset of other, but not the same as it
        /// <summary>
        /// Determines whether the current set is a proper (strict) superset of a specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <returns><see langword="true"/> if the current set is a proper superset of <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// If the current set is a proper superset of <c>other</c>, the current set must have at least one element that <c>other</c> does not have.
        /// An empty set is a subset of any other collection. Therefore, this method returns <c>true</c> if the collection represented by the <c>other</c> parameter is empty, unless the current set is also empty.
        /// This method always returns <c>false</c> if the number of elements in the current set is less than or equal to the number of elements in <c>other</c>.
        /// </remarks>
        bool IsProperSupersetOf(IEnumerable<T> other);

        //Check if this set is a superset of other, but not the same as it
        /// <summary>
        /// Determines whether the current set is a proper (strict) subset of a specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <returns><see langword="true"/> if the current set is a proper subset of <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// If the current set is a proper subset of <c>other</c>, <c>other</c> must have at least one element that the current set does not have.
        /// An empty set is a subset of any other collection. Therefore, this method returns <c>true</c> if the current set is empty, unless the <c>other</c> parameter is also an empty set.
        /// This method always returns <c>false</c> if the current set has more or the same number of elements than <c>other</c>.
        /// </remarks>
        bool IsProperSubsetOf(IEnumerable<T> other);

        //Check if this set has any elements in common with other
        /// <summary>
        /// Determines whether the current set overlaps with the specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <returns><see langword="true"/> if the current set and <paramref name="other"/> share at least one common element; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// Any duplicate elements in <c>other</c> are ignored.
        /// </remarks>
        bool Overlaps(IEnumerable<T> other);

        //Check if this set contains the same and only the same elements as other
        /// <summary>
        /// Determines whether the current set and the specified collection contain the same elements.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <returns><see langword="true"/> if the current set is equal to <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// This method ignores the order of elements and any duplicate elements in <c>other</c>.
        /// </remarks>
        bool SetEquals(IEnumerable<T> other);
    }
}

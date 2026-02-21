// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;

namespace System.Collections.Generic
{
    // Provides a read-only view of a generic dictionary.
    /// <summary>
    /// Represents a generic read-only collection of key/value pairs.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the read-only dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the read-only dictionary.</typeparam>
    /// <remarks>
    /// Each element is a key/value pair that is stored in a <see cref="System.Collections.Generic.KeyValuePair{T,U}"/> object.
    /// Each pair must have a unique key. Implementations can vary in whether they allow you to specify a key that is <c>null</c>. The value can be <c>null</c> and does not have to be unique. The <see cref="System.Collections.Generic.IReadOnlyDictionary{T,U}"/> interface allows the contained keys and values to be enumerated, but it does not imply any particular sort order.
    /// The <c>foreach</c> statement of the C# language (<c>For Each</c> in Visual Basic) requires the type of each element in the collection. Because each element of the <see cref="System.Collections.Generic.IReadOnlyDictionary{T,U}"/> interface is a key/value pair, the element type is not the type of the key or the type of the value. Instead, the element type is <see cref="System.Collections.Generic.KeyValuePair{T,U}"/>, as the following example illustrates.
    /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/IDictionaryTKey,TValue/Overview/source2.cs" id="Snippet11" />
    /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/IDictionaryTKey,TValue/Overview/source2.vb" id="Snippet11" />
    /// The <c>foreach</c> statement is a wrapper around the enumerator; it allows only reading from the collection, not writing to the collection.
    /// </remarks>
    public interface IReadOnlyDictionary<TKey, TValue> : IReadOnlyCollection<KeyValuePair<TKey, TValue>>
    {
        /// <summary>
        /// Determines whether the read-only dictionary contains an element that has the specified key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <returns><see langword="true"/> if the read-only dictionary contains an element that has the specified key; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// Implementations can vary in how they determine the equality of objects; for example, the class that implements <see cref="System.Collections.Generic.IReadOnlyDictionary{T,U}"/> might use the <see cref="System.Collections.Generic.Comparer{T}.Default">Default</see> property, or it might implement the <see cref="System.Collections.Generic.IComparer{T}"/> method.
        /// Implementations can vary in whether they allow <c>key</c> to be <c>null</c>.
        /// </remarks>
        bool ContainsKey(TKey key);
        /// <summary>
        /// Gets the value that is associated with the specified key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</param>
        /// <returns><see langword="true"/> if the object that implements the <see cref="T:System.Collections.Generic.IReadOnlyDictionary`2"/> interface contains an element that has the specified key; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// This method combines the functionality of the <see cref="System.Collections.Generic.IReadOnlyDictionary{T,U}.ContainsKey"/> method and the <see cref="System.Collections.Generic.IReadOnlyDictionary{T,U}.Item"/> property.
        /// If the key is not found, the <c>value</c> parameter gets the appropriate default value for the type <c>TValue</c>: for example, 0 (zero) for integer types, <c>false</c> for Boolean types, and <c>null</c> for reference types.
        /// </remarks>
        bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value);

        TValue this[TKey key] { get; }
        /// <summary>
        /// Gets an enumerable collection that contains the keys in the read-only dictionary.
        /// </summary>
        /// <value>An enumerable collection that contains the keys in the read-only dictionary.</value>
        /// <remarks>
        /// The order of the keys in the enumerable collection is unspecified, but the implementation must guarantee that the keys are in the same order as the corresponding values in the enumerable collection that is returned by the <see cref="System.Collections.Generic.IReadOnlyDictionary{T,U}.Values"/> property.
        /// </remarks>
        IEnumerable<TKey> Keys { get; }
        /// <summary>
        /// Gets an enumerable collection that contains the values in the read-only dictionary.
        /// </summary>
        /// <value>An enumerable collection that contains the values in the read-only dictionary.</value>
        /// <remarks>
        /// The order of the values in the enumerable collection is unspecified, but the implementation must guarantee that the values are in the same order as the corresponding keys in the enumerable collection that is returned by the <see cref="System.Collections.Generic.IReadOnlyDictionary{T,U}.Keys"/> property.
        /// </remarks>
        IEnumerable<TValue> Values { get; }
    }
}

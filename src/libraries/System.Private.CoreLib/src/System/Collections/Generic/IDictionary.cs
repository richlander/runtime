// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;

namespace System.Collections.Generic
{
    // An IDictionary is a possibly unordered set of key-value pairs.
    // Keys can be any non-null object.  Values can be any object.
    // You can look up a value in an IDictionary via the default indexed
    // property, Items.
    /// <summary>
    /// Represents a generic collection of key/value pairs.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    /// <remarks>
    /// <note type="note">
    /// Because keys can be inherited and their behavior changed, their absolute uniqueness cannot be guaranteed by comparisons using the <see cref="System.Type.Equals"/> method.
    /// </note>
    /// The <see cref="System.Collections.Generic.IDictionary{T,U}"/> interface is the base interface for generic collections of key/value pairs.
    /// Each element is a key/value pair stored in a <see cref="System.Collections.Generic.KeyValuePair{T,U}"/> object.
    /// Each pair must have a unique key. Implementations can vary in whether they allow <c>key</c> to be <c>null</c>. The value can be <c>null</c> and does not have to be unique. The <see cref="System.Collections.Generic.IDictionary{T,U}"/> interface allows the contained keys and values to be enumerated, but it does not imply any particular sort order.
    /// The <c>foreach</c> statement of the C# language (<c>For Each</c> in Visual Basic) returns an object of the type of the elements in the collection. Since each element of the <see cref="System.Collections.Generic.IDictionary{T,U}"/> is a key/value pair, the element type is not the type of the key or the type of the value. Instead, the element type is <see cref="System.Collections.Generic.KeyValuePair{T,U}"/>. For example:
    /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/IDictionaryTKey,TValue/Overview/source2.cs" id="Snippet11" />
    /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/IDictionaryTKey,TValue/Overview/source2.vb" id="Snippet11" />
    /// The <c>foreach</c> statement is a wrapper around the enumerator, which only allows reading from, not writing to, the collection.
    /// The following code example creates an empty <see cref="System.Collections.Generic.Dictionary{T,U}"/> of strings, with string keys, and accesses it through the <see cref="System.Collections.Generic.IDictionary{T,U}"/> interface.
    /// The code example uses the <see cref="System.Collections.Generic.IDictionary{T,U}.Add"/> method to add some elements. The example demonstrates that the <see cref="System.Collections.Generic.IDictionary{T,U}.Add"/> method throws <see cref="System.ArgumentException"/> when attempting to add a duplicate key.
    /// The example uses the <see cref="System.Collections.Generic.IDictionary{T,U}.Item"/> property (the indexer in C#) to retrieve values, demonstrating that a <see cref="System.Collections.Generic.KeyNotFoundException"/> is thrown when a requested key is not present, and showing that the value associated with a key can be replaced.
    /// The example shows how to use the <see cref="System.Collections.Generic.IDictionary{T,U}.TryGetValue"/> method as a more efficient way to retrieve values if a program often must try key values that are not in the dictionary, and how to use the <see cref="System.Collections.Generic.IDictionary{T,U}.ContainsKey"/> method to test whether a key exists prior to calling the <see cref="System.Collections.Generic.IDictionary{T,U}.Add"/> method.
    /// Finally, the example shows how to enumerate the keys and values in the dictionary, and how to enumerate the values alone using the <see cref="System.Collections.Generic.IDictionary{T,U}.Values"/> property.
    /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/IDictionaryTKey,TValue/Overview/source.cs" id="Snippet1" />
    /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/IDictionaryTKey,TValue/Overview/source.vb" id="Snippet1" />
    /// </remarks>
    public interface IDictionary<TKey, TValue> : ICollection<KeyValuePair<TKey, TValue>>
    {
        // Interfaces are not serializable
        // The Item property provides methods to read and edit entries
        // in the Dictionary.
        TValue this[TKey key]
        {
            get;
            set;
        }

        // Returns a collections of the keys in this dictionary.
        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <value>An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.</value>
        /// <remarks>
        /// The order of the keys in the returned <see cref="System.Collections.Generic.ICollection{T}"/> is unspecified, but it is guaranteed to be the same order as the corresponding values in the <see cref="System.Collections.Generic.ICollection{T}"/> returned by the <see cref="System.Collections.Generic.IDictionary{T,U}.Values"/> property.
        /// The following code example shows how to enumerate keys alone using the <see cref="System.Collections.Generic.IDictionary{T,U}.Keys"/> property.
        /// This code is part of a larger example that can be compiled and executed. See <see cref="System.Collections.Generic.IDictionary{T,U}">IDictionary{T,U}</see>.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/IDictionaryTKey,TValue/Overview/source.cs" id="Snippet9" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/IDictionaryTKey,TValue/Overview/source.vb" id="Snippet9" />
        /// </remarks>
        ICollection<TKey> Keys
        {
            get;
        }

        // Returns a collections of the values in this dictionary.
        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <value>An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.</value>
        /// <remarks>
        /// The order of the values in the returned <see cref="System.Collections.Generic.ICollection{T}"/> is unspecified, but it is guaranteed to be the same order as the corresponding keys in the <see cref="System.Collections.Generic.ICollection{T}"/> returned by the <see cref="System.Collections.Generic.IDictionary{T,U}.Keys"/> property.
        /// The following code example shows how to enumerate values alone using the <see cref="System.Collections.Generic.IDictionary{T,U}.Values"/> property.
        /// This code is part of a larger example that can be compiled and executed. See <see cref="System.Collections.Generic.IDictionary{T,U}">IDictionary{T,U}</see>.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/IDictionaryTKey,TValue/Overview/source.cs" id="Snippet8" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/IDictionaryTKey,TValue/Overview/source.vb" id="Snippet8" />
        /// </remarks>
        ICollection<TValue> Values
        {
            get;
        }

        // Returns whether this dictionary contains a particular key.
        //
        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.</param>
        /// <returns><see langword="true"/> if the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the key; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// Implementations can vary in how they determine equality of objects; for example, the <see cref="System.Collections.Generic.List{T}"/> class uses <see cref="System.Collections.Generic.Comparer{T}.Default">Default</see>, whereas the <see cref="System.Collections.Generic.Dictionary{T,U}"/> class allows the user to specify the <see cref="System.Collections.Generic.IComparer{T}"/> implementation to use for comparing keys.
        /// Implementations can vary in whether they allow <c>key</c> to be <c>null</c>.
        /// The following code example shows how to use the <see cref="System.Collections.Generic.IDictionary{T,U}.ContainsKey"/> method to test whether a key exists prior to calling the <see cref="System.Collections.Generic.IDictionary{T,U}.Add"/> method. It also shows how to use the <see cref="System.Collections.Generic.IDictionary{T,U}.TryGetValue"/> method, which can be a more efficient way to retrieve values if a program frequently tries key values that are not in the dictionary. Finally, it shows how to insert items using <see cref="System.Collections.Generic.IDictionary{T,U}.Item"/> property (the indexer in C#).
        /// This code is part of a larger example that can be compiled and executed. See <see cref="System.Collections.Generic.IDictionary{T,U}">IDictionary{T,U}</see>.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/IDictionaryTKey,TValue/Overview/source.cs" id="Snippet6" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/IDictionaryTKey,TValue/Overview/source.vb" id="Snippet6" />
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/IDictionaryTKey,TValue/Overview/source.cs" id="Snippet5" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/IDictionaryTKey,TValue/Overview/source.vb" id="Snippet5" />
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/IDictionaryTKey,TValue/Overview/source.cs" id="Snippet4" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/IDictionaryTKey,TValue/Overview/source.vb" id="Snippet4" />
        /// </remarks>
        bool ContainsKey(TKey key);

        // Adds a key-value pair to the dictionary.
        //
        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentException">An element with the same key already exists in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.</exception>
        /// <remarks>
        /// You can also use the <see cref="System.Collections.Generic.IDictionary{T,U}.Item"/> property to add new elements by setting the value of a key that does not exist in the dictionary; for example, <c>myCollection[&quot;myNonexistentKey&quot;] = myValue</c> in C# (<c>myCollection(&quot;myNonexistentKey&quot;) = myValue</c> in Visual Basic). However, if the specified key already exists in the dictionary, setting the <see cref="System.Collections.Generic.IDictionary{T,U}.Item"/> property overwrites the old value. In contrast, the <see cref="System.Collections.Generic.IDictionary{T,U}.Add"/> method does not modify existing elements.
        /// Implementations can vary in how they determine equality of objects; for example, the <see cref="System.Collections.Generic.List{T}"/> class uses <see cref="System.Collections.Generic.Comparer{T}.Default">Default</see>, whereas the <see cref="System.Collections.Generic.Dictionary{T,U}"/> class allows the user to specify the <see cref="System.Collections.Generic.IComparer{T}"/> implementation to use for comparing keys.
        /// Implementations can vary in whether they allow <c>key</c> to be <c>null</c>.
        /// The following code example creates an empty <see cref="System.Collections.Generic.Dictionary{T,U}"/> of strings, with integer keys, and accesses it through the <see cref="System.Collections.Generic.IDictionary{T,U}"/> interface. The code example uses the <see cref="System.Collections.Generic.IDictionary{T,U}.Add"/> method to add some elements. The example demonstrates that the <see cref="System.Collections.Generic.IDictionary{T,U}.Add"/> method throws an <see cref="System.ArgumentException"/> when attempting to add a duplicate key.
        /// This code is part of a larger example that can be compiled and executed. See <see cref="System.Collections.Generic.IDictionary{T,U}">IDictionary{T,U}</see>.
        /// :::code language="csharp" source="~/snippets/csharp/System.Collections.Generic/IDictionaryTKey,TValue/Overview/source.cs" interactive="try-dotnet-method" id="Snippet2":::
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/IDictionaryTKey,TValue/Overview/source.vb" id="Snippet2" />
        /// </remarks>
        void Add(TKey key, TValue value);

        // Removes a particular key from the dictionary.
        //
        /// <summary>
        /// Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns><see langword="true"/> if the element is successfully removed; otherwise, <see langword="false"/>. This method also returns <see langword="false"/> if <paramref name="key"/> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.</exception>
        /// <remarks>
        /// Implementations can vary in how they determine equality of objects; for example, the <see cref="System.Collections.Generic.List{T}"/> class uses <see cref="System.Collections.Generic.Comparer{T}.Default">Default</see>, whereas the <see cref="System.Collections.Generic.Dictionary{T,U}"/> class allows the user to specify the <see cref="System.Collections.Generic.IComparer{T}"/> implementation to use for comparing keys.
        /// The following code example shows how to remove a key/value pair from a dictionary using the <see cref="System.Collections.Generic.IDictionary{T,U}.Remove"/> method.
        /// This code is part of a larger example that can be compiled and executed. See <see cref="System.Collections.Generic.IDictionary{T,U}">IDictionary{T,U}</see>.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/IDictionaryTKey,TValue/Overview/source.cs" id="Snippet10" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/IDictionaryTKey,TValue/Overview/source.vb" id="Snippet10" />
        /// </remarks>
        bool Remove(TKey key);

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</param>
        /// <returns><see langword="true"/> if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the specified key; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// This method combines the functionality of the <see cref="System.Collections.Generic.IDictionary{T,U}.ContainsKey"/> method and the <see cref="System.Collections.Generic.IDictionary{T,U}.Item"/> property.
        /// If the key is not found, then the <c>value</c> parameter gets the appropriate default value for the type <c>TValue</c>; for example, zero (0) for integer types, <c>false</c> for Boolean types, and <c>null</c> for reference types.
        /// The example shows how to use the <see cref="System.Collections.Generic.IDictionary{T,U}.TryGetValue"/> method to retrieve values. If a program frequently tries key values that are not in a dictionary, the <see cref="System.Collections.Generic.IDictionary{T,U}.TryGetValue"/> method can be more efficient than using the <see cref="System.Collections.Generic.IDictionary{T,U}.Item"/> property (the indexer in C#), which throws exceptions when attempting to retrieve nonexistent keys.
        /// This code is part of a larger example that can be compiled and executed. See <see cref="System.Collections.Generic.IDictionary{T,U}">IDictionary{T,U}</see>.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/IDictionaryTKey,TValue/Overview/source.cs" id="Snippet5" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/IDictionaryTKey,TValue/Overview/source.vb" id="Snippet5" />
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/IDictionaryTKey,TValue/Overview/source.cs" id="Snippet4" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/IDictionaryTKey,TValue/Overview/source.vb" id="Snippet4" />
        /// </remarks>
        bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value);
    }
}

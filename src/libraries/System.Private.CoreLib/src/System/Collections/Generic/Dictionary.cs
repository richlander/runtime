// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Collections.Generic
{
    /// <summary>
    /// Represents a collection of keys and values.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <remarks>
    /// <note type="note">
    /// The speed of retrieval depends on the quality of the hashing algorithm of the type specified for <c>TKey</c>.
    /// </note>
    /// <note type="note">
    /// For example, you can use the case-insensitive string comparers provided by the <see cref="System.StringComparer"/> class to create dictionaries with case-insensitive string keys.
    /// </note>
    /// <note type="note">
    /// Because keys can be inherited and their behavior changed, their absolute uniqueness cannot be guaranteed by comparisons using the <see cref="System.Type.Equals"/> method.
    /// </note>
    /// The <see cref="System.Collections.Generic.Dictionary{T,U}"/> generic class provides a mapping from a set of keys to a set of values. Each addition to the dictionary consists of a value and its associated key. Retrieving a value by using its key is very fast, close to O(1), because the <see cref="System.Collections.Generic.Dictionary{T,U}"/> class is implemented as a hash table.
    /// As long as an object is used as a key in the <see cref="System.Collections.Generic.Dictionary{T,U}"/>, it must not change in any way that affects its hash value. Every key in a <see cref="System.Collections.Generic.Dictionary{T,U}"/> must be unique according to the dictionary's equality comparer. A key cannot be <c>null</c>, but a value can be, if its type <c>TValue</c> is a reference type.
    /// <see cref="System.Collections.Generic.Dictionary{T,U}"/> requires an equality implementation to determine whether keys are equal. You can specify an implementation of the <see cref="System.Collections.Generic.IEqualityComparer{T}"/> generic interface by using a constructor that accepts a <c>comparer</c> parameter; if you do not specify an implementation, the default generic equality comparer <see cref="System.Collections.Generic.EqualityComparer{T}.Default">Default</see> is used. If type <c>TKey</c> implements the <see cref="System.IEquatable{T}">IEquatable{T}</see> generic interface, the default equality comparer uses that implementation.
    /// The capacity of a <see cref="System.Collections.Generic.Dictionary{T,U}"/> is the number of elements the <see cref="System.Collections.Generic.Dictionary{T,U}"/> can hold. As elements are added to a <see cref="System.Collections.Generic.Dictionary{T,U}"/>, the capacity is automatically increased as required by reallocating the internal array.
    /// **.NET Framework only:** For very large <see cref="System.Collections.Generic.Dictionary{T,U}"/> objects, you can increase the maximum capacity to 2 billion elements on a 64-bit system by setting the <c>enabled</c> attribute of the [<c>&lt;gcAllowVeryLargeObjects&gt;</c>](/dotnet/framework/configure-apps/file-schema/runtime/gcallowverylargeobjects-element) configuration element to <c>true</c> in the run-time environment.
    /// For purposes of enumeration, each item in the dictionary is treated as a <see cref="System.Collections.Generic.KeyValuePair{T,U}"/> structure representing a value and its key. The order in which the items are returned is undefined.
    /// The <c>foreach</c> statement of the C# language (<c>For Each</c> in Visual Basic) returns an object of the type of the elements in the collection. Since the <see cref="System.Collections.Generic.Dictionary{T,U}"/> is a collection of keys and values, the element type is not the type of the key or the type of the value. Instead, the element type is a <see cref="System.Collections.Generic.KeyValuePair{T,U}"/> of the key type and the value type. For example:
    /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/DictionaryTKey,TValue/Overview/source2.cs" id="Snippet11" />
    /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/DictionaryTKey,TValue/Overview/source2.fs" id="Snippet11" />
    /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/DictionaryTKey,TValue/Overview/source2.vb" id="Snippet11" />
    /// The <c>foreach</c> statement is a wrapper around the enumerator, which allows only reading from the collection, not writing to it.
    /// The following code example creates an empty <see cref="System.Collections.Generic.Dictionary{T,U}"/> of strings with string keys and uses the <see cref="System.Collections.Generic.Dictionary{T,U}.Add"/> method to add some elements. The example demonstrates that the <see cref="System.Collections.Generic.Dictionary{T,U}.Add"/> method throws an <see cref="System.ArgumentException"/> when attempting to add a duplicate key.
    /// The example uses the <see cref="System.Collections.Generic.Dictionary{T,U}.Item"/> property (the indexer in C#) to retrieve values, demonstrating that a <see cref="System.Collections.Generic.KeyNotFoundException"/> is thrown when a requested key is not present, and showing that the value associated with a key can be replaced.
    /// The example shows how to use the <see cref="System.Collections.Generic.Dictionary{T,U}.TryGetValue"/> method as a more efficient way to retrieve values if a program often must try key values that are not in the dictionary, and it shows how to use the <see cref="System.Collections.Generic.Dictionary{T,U}.ContainsKey"/> method to test whether a key exists before calling the <see cref="System.Collections.Generic.Dictionary{T,U}.Add"/> method.
    /// The example shows how to enumerate the keys and values in the dictionary and how to enumerate the keys and values alone using the <see cref="System.Collections.Generic.Dictionary{T,U}.Keys"/> property and the <see cref="System.Collections.Generic.Dictionary{T,U}.Values"/> property.
    /// Finally, the example demonstrates the <see cref="System.Collections.Generic.Dictionary{T,U}.Remove"/> method.
    /// :::code language="csharp" source="~/snippets/csharp/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.cs" interactive="try-dotnet-method" id="Snippet1":::
    /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.fs" id="Snippet1" />
    /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.vb" id="Snippet1" />
    /// </remarks>
    [DebuggerTypeProxy(typeof(IDictionaryDebugView<,>))]
    [DebuggerDisplay("Count = {Count}")]
    [Serializable]
    [TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    public class Dictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary, IReadOnlyDictionary<TKey, TValue>, ISerializable, IDeserializationCallback where TKey : notnull
    {
        // constants for serialization
        private const string VersionName = "Version"; // Do not rename (binary serialization)
        private const string HashSizeName = "HashSize"; // Do not rename (binary serialization). Must save buckets.Length
        private const string KeyValuePairsName = "KeyValuePairs"; // Do not rename (binary serialization)
        private const string ComparerName = "Comparer"; // Do not rename (binary serialization)

        private int[]? _buckets;
        private Entry[]? _entries;
#if TARGET_64BIT
        private ulong _fastModMultiplier;
#endif
        private int _count;
        private int _freeList;
        private int _freeCount;
        private int _version;
        private IEqualityComparer<TKey>? _comparer;
        private KeyCollection? _keys;
        private ValueCollection? _values;
        private const int StartOfFreeList = -3;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Collections.Generic.Dictionary`2"/> class that is empty, has the default initial capacity, and uses the default equality comparer for the key type.
        /// </summary>
        /// <remarks>
        /// <note type="note">
        /// If you can estimate the size of the collection, using a constructor that specifies the initial capacity eliminates the need to perform a number of resizing operations while adding elements to the <see cref="System.Collections.Generic.Dictionary{T,U}"/>.
        /// </note>
        /// Every key in a <see cref="System.Collections.Generic.Dictionary{T,U}"/> must be unique according to the default equality comparer.
        /// <see cref="System.Collections.Generic.Dictionary{T,U}"/> requires an equality implementation to determine whether keys are equal. This constructor uses the default generic equality comparer, <see cref="System.Collections.Generic.EqualityComparer{T}.Default">Default</see>. If type <c>TKey</c> implements the <see cref="System.IEquatable{T}">IEquatable{T}</see> generic interface, the default equality comparer uses that implementation. Alternatively, you can specify an implementation of the <see cref="System.Collections.Generic.IEqualityComparer{T}"/> generic interface by using a constructor that accepts a <c>comparer</c> parameter.
        /// This constructor is an O(1) operation.
        /// The following code example creates an empty <see cref="System.Collections.Generic.Dictionary{T,U}"/> of strings with string keys and uses the <see cref="System.Collections.Generic.Dictionary{T,U}.Add"/> method to add some elements. The example demonstrates that the <see cref="System.Collections.Generic.Dictionary{T,U}.Add"/> method throws an <see cref="System.ArgumentException"/> when attempting to add a duplicate key.
        /// This code example is part of a larger example provided for the <see cref="System.Collections.Generic.Dictionary{T,U}"/> class.
        /// :::code language="csharp" source="~/snippets/csharp/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.cs" interactive="try-dotnet-method" id="Snippet2":::
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.fs" id="Snippet2" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.vb" id="Snippet2" />
        /// </remarks>
        public Dictionary() : this(0, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Collections.Generic.Dictionary`2"/> class that is empty, has the specified initial capacity, and uses the default equality comparer for the key type.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the <see cref="T:System.Collections.Generic.Dictionary`2"/> can contain.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 0.</exception>
        /// <remarks>
        /// Every key in a <see cref="System.Collections.Generic.Dictionary{T,U}"/> must be unique according to the default equality comparer.
        /// The capacity of a <see cref="System.Collections.Generic.Dictionary{T,U}"/> is the number of elements that can be added to the <see cref="System.Collections.Generic.Dictionary{T,U}"/> before resizing is necessary. As elements are added to a <see cref="System.Collections.Generic.Dictionary{T,U}"/>, the capacity is automatically increased as required by reallocating the internal array.
        /// If the size of the collection can be estimated, specifying the initial capacity eliminates the need to perform a number of resizing operations while adding elements to the <see cref="System.Collections.Generic.Dictionary{T,U}"/>.
        /// <see cref="System.Collections.Generic.Dictionary{T,U}"/> requires an equality implementation to determine whether keys are equal. This constructor uses the default generic equality comparer, <see cref="System.Collections.Generic.EqualityComparer{T}.Default">Default</see>. If type <c>TKey</c> implements the <see cref="System.IEquatable{T}">IEquatable{T}</see> generic interface, the default equality comparer uses that implementation. Alternatively, you can specify an implementation of the <see cref="System.Collections.Generic.IEqualityComparer{T}"/> generic interface by using a constructor that accepts a <c>comparer</c> parameter.
        /// This constructor is an O(1) operation.
        /// The following code example creates a dictionary with an initial capacity of 4 and populates it with 4 entries.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/DictionaryTKey,TValue/.ctor/source3.cs" id="Snippet1" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/DictionaryTKey,TValue/.ctor/source3.fs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/DictionaryTKey,TValue/.ctor/source3.vb" id="Snippet1" />
        /// </remarks>
        public Dictionary(int capacity) : this(capacity, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Collections.Generic.Dictionary`2"/> class that is empty, has the default initial capacity, and uses the default equality comparer for the key type.
        /// </summary>
        /// <remarks>
        /// <note type="note">
        /// If you can estimate the size of the collection, using a constructor that specifies the initial capacity eliminates the need to perform a number of resizing operations while adding elements to the <see cref="System.Collections.Generic.Dictionary{T,U}"/>.
        /// </note>
        /// Every key in a <see cref="System.Collections.Generic.Dictionary{T,U}"/> must be unique according to the default equality comparer.
        /// <see cref="System.Collections.Generic.Dictionary{T,U}"/> requires an equality implementation to determine whether keys are equal. This constructor uses the default generic equality comparer, <see cref="System.Collections.Generic.EqualityComparer{T}.Default">Default</see>. If type <c>TKey</c> implements the <see cref="System.IEquatable{T}">IEquatable{T}</see> generic interface, the default equality comparer uses that implementation. Alternatively, you can specify an implementation of the <see cref="System.Collections.Generic.IEqualityComparer{T}"/> generic interface by using a constructor that accepts a <c>comparer</c> parameter.
        /// This constructor is an O(1) operation.
        /// The following code example creates an empty <see cref="System.Collections.Generic.Dictionary{T,U}"/> of strings with string keys and uses the <see cref="System.Collections.Generic.Dictionary{T,U}.Add"/> method to add some elements. The example demonstrates that the <see cref="System.Collections.Generic.Dictionary{T,U}.Add"/> method throws an <see cref="System.ArgumentException"/> when attempting to add a duplicate key.
        /// This code example is part of a larger example provided for the <see cref="System.Collections.Generic.Dictionary{T,U}"/> class.
        /// :::code language="csharp" source="~/snippets/csharp/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.cs" interactive="try-dotnet-method" id="Snippet2":::
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.fs" id="Snippet2" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.vb" id="Snippet2" />
        /// </remarks>
        public Dictionary(IEqualityComparer<TKey>? comparer) : this(0, comparer) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Collections.Generic.Dictionary`2"/> class that is empty, has the default initial capacity, and uses the default equality comparer for the key type.
        /// </summary>
        /// <remarks>
        /// <note type="note">
        /// If you can estimate the size of the collection, using a constructor that specifies the initial capacity eliminates the need to perform a number of resizing operations while adding elements to the <see cref="System.Collections.Generic.Dictionary{T,U}"/>.
        /// </note>
        /// Every key in a <see cref="System.Collections.Generic.Dictionary{T,U}"/> must be unique according to the default equality comparer.
        /// <see cref="System.Collections.Generic.Dictionary{T,U}"/> requires an equality implementation to determine whether keys are equal. This constructor uses the default generic equality comparer, <see cref="System.Collections.Generic.EqualityComparer{T}.Default">Default</see>. If type <c>TKey</c> implements the <see cref="System.IEquatable{T}">IEquatable{T}</see> generic interface, the default equality comparer uses that implementation. Alternatively, you can specify an implementation of the <see cref="System.Collections.Generic.IEqualityComparer{T}"/> generic interface by using a constructor that accepts a <c>comparer</c> parameter.
        /// This constructor is an O(1) operation.
        /// The following code example creates an empty <see cref="System.Collections.Generic.Dictionary{T,U}"/> of strings with string keys and uses the <see cref="System.Collections.Generic.Dictionary{T,U}.Add"/> method to add some elements. The example demonstrates that the <see cref="System.Collections.Generic.Dictionary{T,U}.Add"/> method throws an <see cref="System.ArgumentException"/> when attempting to add a duplicate key.
        /// This code example is part of a larger example provided for the <see cref="System.Collections.Generic.Dictionary{T,U}"/> class.
        /// :::code language="csharp" source="~/snippets/csharp/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.cs" interactive="try-dotnet-method" id="Snippet2":::
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.fs" id="Snippet2" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.vb" id="Snippet2" />
        /// </remarks>
        public Dictionary(int capacity, IEqualityComparer<TKey>? comparer)
        {
            if (capacity < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.capacity);
            }

            if (capacity > 0)
            {
                Initialize(capacity);
            }

            // For reference types, we always want to store a comparer instance, either
            // the one provided, or if one wasn't provided, the default (accessing
            // EqualityComparer<TKey>.Default with shared generics on every dictionary
            // access can add measurable overhead).  For value types, if no comparer is
            // provided, or if the default is provided, we'd prefer to use
            // EqualityComparer<TKey>.Default.Equals on every use, enabling the JIT to
            // devirtualize and possibly inline the operation.
            if (!typeof(TKey).IsValueType)
            {
                _comparer = comparer ?? EqualityComparer<TKey>.Default;

                // Special-case EqualityComparer<string>.Default, StringComparer.Ordinal, and StringComparer.OrdinalIgnoreCase.
                // We use a non-randomized comparer for improved perf, falling back to a randomized comparer if the
                // hash buckets become unbalanced.
                if (typeof(TKey) == typeof(string) &&
                    NonRandomizedStringEqualityComparer.GetStringComparer(_comparer!) is IEqualityComparer<string> stringComparer)
                {
                    _comparer = (IEqualityComparer<TKey>)stringComparer;
                }
            }
            else if (comparer is not null && // first check for null to avoid forcing default comparer instantiation unnecessarily
                     comparer != EqualityComparer<TKey>.Default)
            {
                _comparer = comparer;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Collections.Generic.Dictionary`2"/> class that is empty, has the default initial capacity, and uses the default equality comparer for the key type.
        /// </summary>
        /// <remarks>
        /// <note type="note">
        /// If you can estimate the size of the collection, using a constructor that specifies the initial capacity eliminates the need to perform a number of resizing operations while adding elements to the <see cref="System.Collections.Generic.Dictionary{T,U}"/>.
        /// </note>
        /// Every key in a <see cref="System.Collections.Generic.Dictionary{T,U}"/> must be unique according to the default equality comparer.
        /// <see cref="System.Collections.Generic.Dictionary{T,U}"/> requires an equality implementation to determine whether keys are equal. This constructor uses the default generic equality comparer, <see cref="System.Collections.Generic.EqualityComparer{T}.Default">Default</see>. If type <c>TKey</c> implements the <see cref="System.IEquatable{T}">IEquatable{T}</see> generic interface, the default equality comparer uses that implementation. Alternatively, you can specify an implementation of the <see cref="System.Collections.Generic.IEqualityComparer{T}"/> generic interface by using a constructor that accepts a <c>comparer</c> parameter.
        /// This constructor is an O(1) operation.
        /// The following code example creates an empty <see cref="System.Collections.Generic.Dictionary{T,U}"/> of strings with string keys and uses the <see cref="System.Collections.Generic.Dictionary{T,U}.Add"/> method to add some elements. The example demonstrates that the <see cref="System.Collections.Generic.Dictionary{T,U}.Add"/> method throws an <see cref="System.ArgumentException"/> when attempting to add a duplicate key.
        /// This code example is part of a larger example provided for the <see cref="System.Collections.Generic.Dictionary{T,U}"/> class.
        /// :::code language="csharp" source="~/snippets/csharp/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.cs" interactive="try-dotnet-method" id="Snippet2":::
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.fs" id="Snippet2" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.vb" id="Snippet2" />
        /// </remarks>
        public Dictionary(IDictionary<TKey, TValue> dictionary) : this(dictionary, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Collections.Generic.Dictionary`2"/> class that is empty, has the default initial capacity, and uses the default equality comparer for the key type.
        /// </summary>
        /// <remarks>
        /// <note type="note">
        /// If you can estimate the size of the collection, using a constructor that specifies the initial capacity eliminates the need to perform a number of resizing operations while adding elements to the <see cref="System.Collections.Generic.Dictionary{T,U}"/>.
        /// </note>
        /// Every key in a <see cref="System.Collections.Generic.Dictionary{T,U}"/> must be unique according to the default equality comparer.
        /// <see cref="System.Collections.Generic.Dictionary{T,U}"/> requires an equality implementation to determine whether keys are equal. This constructor uses the default generic equality comparer, <see cref="System.Collections.Generic.EqualityComparer{T}.Default">Default</see>. If type <c>TKey</c> implements the <see cref="System.IEquatable{T}">IEquatable{T}</see> generic interface, the default equality comparer uses that implementation. Alternatively, you can specify an implementation of the <see cref="System.Collections.Generic.IEqualityComparer{T}"/> generic interface by using a constructor that accepts a <c>comparer</c> parameter.
        /// This constructor is an O(1) operation.
        /// The following code example creates an empty <see cref="System.Collections.Generic.Dictionary{T,U}"/> of strings with string keys and uses the <see cref="System.Collections.Generic.Dictionary{T,U}.Add"/> method to add some elements. The example demonstrates that the <see cref="System.Collections.Generic.Dictionary{T,U}.Add"/> method throws an <see cref="System.ArgumentException"/> when attempting to add a duplicate key.
        /// This code example is part of a larger example provided for the <see cref="System.Collections.Generic.Dictionary{T,U}"/> class.
        /// :::code language="csharp" source="~/snippets/csharp/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.cs" interactive="try-dotnet-method" id="Snippet2":::
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.fs" id="Snippet2" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.vb" id="Snippet2" />
        /// </remarks>
        public Dictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey>? comparer) :
            this(dictionary?.Count ?? 0, comparer)
        {
            if (dictionary == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.dictionary);
            }

            AddRange(dictionary);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Collections.Generic.Dictionary`2"/> class that is empty, has the default initial capacity, and uses the default equality comparer for the key type.
        /// </summary>
        /// <remarks>
        /// <note type="note">
        /// If you can estimate the size of the collection, using a constructor that specifies the initial capacity eliminates the need to perform a number of resizing operations while adding elements to the <see cref="System.Collections.Generic.Dictionary{T,U}"/>.
        /// </note>
        /// Every key in a <see cref="System.Collections.Generic.Dictionary{T,U}"/> must be unique according to the default equality comparer.
        /// <see cref="System.Collections.Generic.Dictionary{T,U}"/> requires an equality implementation to determine whether keys are equal. This constructor uses the default generic equality comparer, <see cref="System.Collections.Generic.EqualityComparer{T}.Default">Default</see>. If type <c>TKey</c> implements the <see cref="System.IEquatable{T}">IEquatable{T}</see> generic interface, the default equality comparer uses that implementation. Alternatively, you can specify an implementation of the <see cref="System.Collections.Generic.IEqualityComparer{T}"/> generic interface by using a constructor that accepts a <c>comparer</c> parameter.
        /// This constructor is an O(1) operation.
        /// The following code example creates an empty <see cref="System.Collections.Generic.Dictionary{T,U}"/> of strings with string keys and uses the <see cref="System.Collections.Generic.Dictionary{T,U}.Add"/> method to add some elements. The example demonstrates that the <see cref="System.Collections.Generic.Dictionary{T,U}.Add"/> method throws an <see cref="System.ArgumentException"/> when attempting to add a duplicate key.
        /// This code example is part of a larger example provided for the <see cref="System.Collections.Generic.Dictionary{T,U}"/> class.
        /// :::code language="csharp" source="~/snippets/csharp/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.cs" interactive="try-dotnet-method" id="Snippet2":::
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.fs" id="Snippet2" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.vb" id="Snippet2" />
        /// </remarks>
        public Dictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection) : this(collection, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Collections.Generic.Dictionary`2"/> class that is empty, has the default initial capacity, and uses the default equality comparer for the key type.
        /// </summary>
        /// <remarks>
        /// <note type="note">
        /// If you can estimate the size of the collection, using a constructor that specifies the initial capacity eliminates the need to perform a number of resizing operations while adding elements to the <see cref="System.Collections.Generic.Dictionary{T,U}"/>.
        /// </note>
        /// Every key in a <see cref="System.Collections.Generic.Dictionary{T,U}"/> must be unique according to the default equality comparer.
        /// <see cref="System.Collections.Generic.Dictionary{T,U}"/> requires an equality implementation to determine whether keys are equal. This constructor uses the default generic equality comparer, <see cref="System.Collections.Generic.EqualityComparer{T}.Default">Default</see>. If type <c>TKey</c> implements the <see cref="System.IEquatable{T}">IEquatable{T}</see> generic interface, the default equality comparer uses that implementation. Alternatively, you can specify an implementation of the <see cref="System.Collections.Generic.IEqualityComparer{T}"/> generic interface by using a constructor that accepts a <c>comparer</c> parameter.
        /// This constructor is an O(1) operation.
        /// The following code example creates an empty <see cref="System.Collections.Generic.Dictionary{T,U}"/> of strings with string keys and uses the <see cref="System.Collections.Generic.Dictionary{T,U}.Add"/> method to add some elements. The example demonstrates that the <see cref="System.Collections.Generic.Dictionary{T,U}.Add"/> method throws an <see cref="System.ArgumentException"/> when attempting to add a duplicate key.
        /// This code example is part of a larger example provided for the <see cref="System.Collections.Generic.Dictionary{T,U}"/> class.
        /// :::code language="csharp" source="~/snippets/csharp/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.cs" interactive="try-dotnet-method" id="Snippet2":::
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.fs" id="Snippet2" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.vb" id="Snippet2" />
        /// </remarks>
        public Dictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey>? comparer) :
            this((collection as ICollection<KeyValuePair<TKey, TValue>>)?.Count ?? 0, comparer)
        {
            if (collection == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.collection);
            }

            AddRange(collection);
        }

        private void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> enumerable)
        {
            // It is likely that the passed-in enumerable is Dictionary<TKey,TValue>. When this is the case,
            // avoid the enumerator allocation and overhead by looping through the entries array directly.
            // We only do this when dictionary is Dictionary<TKey,TValue> and not a subclass, to maintain
            // back-compat with subclasses that may have overridden the enumerator behavior.
            if (enumerable.GetType() == typeof(Dictionary<TKey, TValue>))
            {
                Dictionary<TKey, TValue> source = (Dictionary<TKey, TValue>)enumerable;

                if (source.Count == 0)
                {
                    // Nothing to copy, all done
                    return;
                }

                // This is not currently a true .AddRange as it needs to be an initialized dictionary
                // of the correct size, and also an empty dictionary with no current entities (and no argument checks).
                Debug.Assert(source._entries is not null);
                Debug.Assert(_entries is not null);
                Debug.Assert(_entries.Length >= source.Count);
                Debug.Assert(_count == 0);

                Entry[] oldEntries = source._entries;
                if (source._comparer == _comparer)
                {
                    // If comparers are the same, we can copy _entries without rehashing.
                    CopyEntries(oldEntries, source._count);
                    return;
                }

                // Comparers differ need to rehash all the entries via Add
                int count = source._count;
                for (int i = 0; i < count; i++)
                {
                    // Only copy if an entry
                    if (oldEntries[i].next >= -1)
                    {
                        Add(oldEntries[i].key, oldEntries[i].value);
                    }
                }
                return;
            }

            // We similarly special-case KVP<>[] and List<KVP<>>, as they're commonly used to seed dictionaries, and
            // we want to avoid the enumerator costs (e.g. allocation) for them as well. Extract a span if possible.
            ReadOnlySpan<KeyValuePair<TKey, TValue>> span;
            if (enumerable is KeyValuePair<TKey, TValue>[] array)
            {
                span = array;
            }
            else if (enumerable.GetType() == typeof(List<KeyValuePair<TKey, TValue>>))
            {
                span = CollectionsMarshal.AsSpan((List<KeyValuePair<TKey, TValue>>)enumerable);
            }
            else
            {
                // Fallback path for all other enumerables
                foreach (KeyValuePair<TKey, TValue> pair in enumerable)
                {
                    Add(pair.Key, pair.Value);
                }
                return;
            }

            // We got a span. Add the elements to the dictionary.
            foreach (KeyValuePair<TKey, TValue> pair in span)
            {
                Add(pair.Key, pair.Value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Collections.Generic.Dictionary`2"/> class with serialized data.
        /// </summary>
        /// <param name="info">A <see cref="T:System.Runtime.Serialization.SerializationInfo"/> object containing the information required to serialize the <see cref="T:System.Collections.Generic.Dictionary`2"/>.</param>
        /// <param name="context">A <see cref="T:System.Runtime.Serialization.StreamingContext"/> structure containing the source and destination of the serialized stream associated with the <see cref="T:System.Collections.Generic.Dictionary`2"/>.</param>
        /// <remarks>
        /// This constructor is called during deserialization to reconstitute an object transmitted over a stream. For more information, see [XML and SOAP Serialization](/dotnet/standard/serialization/xml-and-soap-serialization).
        /// </remarks>
        [Obsolete(Obsoletions.LegacyFormatterImplMessage, DiagnosticId = Obsoletions.LegacyFormatterImplDiagId, UrlFormat = Obsoletions.SharedUrlFormat)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected Dictionary(SerializationInfo info, StreamingContext context)
        {
            // We can't do anything with the keys and values until the entire graph has been deserialized
            // and we have a resonable estimate that GetHashCode is not going to fail.  For the time being,
            // we'll just cache this.  The graph is not valid until OnDeserialization has been called.
            HashHelpers.SerializationInfoTable.Add(this, info);
        }

        /// <summary>
        /// Gets the <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> that is used to determine equality of keys for the dictionary.
        /// </summary>
        /// <value>The <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> generic interface implementation that is used to determine equality of keys for the current <see cref="T:System.Collections.Generic.Dictionary`2"/> and to provide hash values for the keys.</value>
        /// <remarks>
        /// <see cref="System.Collections.Generic.Dictionary{T,U}"/> requires an equality implementation to determine whether keys are equal. You can specify an implementation of the <see cref="System.Collections.Generic.IEqualityComparer{T}"/> generic interface by using a constructor that accepts a <c>comparer</c> parameter; if you do not specify one, the default generic equality comparer <see cref="System.Collections.Generic.EqualityComparer{T}.Default">Default</see> is used.
        /// Getting the value of this property is an O(1) operation.
        /// </remarks>
        public IEqualityComparer<TKey> Comparer
        {
            get
            {
                if (typeof(TKey) == typeof(string))
                {
                    Debug.Assert(_comparer is not null, "The comparer should never be null for a reference type.");
                    return (IEqualityComparer<TKey>)IInternalStringEqualityComparer.GetUnderlyingEqualityComparer((IEqualityComparer<string?>)_comparer);
                }
                else
                {
                    return _comparer ?? EqualityComparer<TKey>.Default;
                }
            }
        }

        /// <summary>
        /// Gets the number of key/value pairs contained in the <see cref="T:System.Collections.Generic.Dictionary`2"/>.
        /// </summary>
        /// <value>The number of key/value pairs contained in the <see cref="T:System.Collections.Generic.Dictionary`2"/>.</value>
        /// <remarks>
        /// The capacity of a <see cref="System.Collections.Generic.Dictionary{T,U}"/> is the number of elements that the <see cref="System.Collections.Generic.Dictionary{T,U}"/> can store. The <see cref="System.Collections.Generic.Dictionary{T,U}.Count"/> property is the number of elements that are actually in the <see cref="System.Collections.Generic.Dictionary{T,U}"/>.
        /// The capacity is always greater than or equal to <see cref="System.Collections.Generic.Dictionary{T,U}.Count"/>. If <see cref="System.Collections.Generic.Dictionary{T,U}.Count"/> exceeds the capacity while adding elements, the capacity is increased by automatically reallocating the internal array before copying the old elements and adding the new elements.
        /// Getting the value of this property is an O(1) operation.
        /// </remarks>
        public int Count => _count - _freeCount;

        /// <summary>
        /// Gets the total numbers of elements the internal data structure can hold without resizing.
        /// </summary>
        public int Capacity => _entries?.Length ?? 0;

        /// <summary>
        /// Gets a collection containing the keys in the <see cref="T:System.Collections.Generic.Dictionary`2"/>.
        /// </summary>
        /// <value>A <see cref="T:System.Collections.Generic.Dictionary`2.KeyCollection"/> containing the keys in the <see cref="T:System.Collections.Generic.Dictionary`2"/>.</value>
        /// <remarks>
        /// The order of the keys in the <see cref="System.Collections.Generic.Dictionary{T,U}.KeyCollection"/> is unspecified, but it is the same order as the associated values in the <see cref="System.Collections.Generic.Dictionary{T,U}.ValueCollection"/> returned by the <see cref="System.Collections.Generic.Dictionary{T,U}.Values"/> property.
        /// The returned <see cref="System.Collections.Generic.Dictionary{T,U}.KeyCollection"/> is not a static copy; instead, the <see cref="System.Collections.Generic.Dictionary{T,U}.KeyCollection"/> refers back to the keys in the original <see cref="System.Collections.Generic.Dictionary{T,U}"/>. Therefore, changes to the <see cref="System.Collections.Generic.Dictionary{T,U}"/> continue to be reflected in the <see cref="System.Collections.Generic.Dictionary{T,U}.KeyCollection"/>.
        /// Getting the value of this property is an O(1) operation.
        /// The following code example shows how to enumerate the keys in the dictionary using the <see cref="System.Collections.Generic.Dictionary{T,U}.Keys"/> property, and how to enumerate the keys and values in the dictionary.
        /// This code is part of a larger example that can be compiled and executed (<c>openWith</c> is the name of the Dictionary used in this example). See <see cref="System.Collections.Generic.Dictionary{T,U}"/>.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.cs" id="Snippet9" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.fs" id="Snippet9" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.vb" id="Snippet9" />
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.cs" id="Snippet7" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.fs" id="Snippet7" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.vb" id="Snippet7" />
        /// </remarks>
        public KeyCollection Keys => _keys ??= new KeyCollection(this);

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <value>An <see cref="T:System.Collections.Generic.ICollection`1"/> of type <paramref name="TKey"/> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2"/>.</value>
        ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys;

        /// <summary>
        /// Gets a collection containing the keys of the <see cref="T:System.Collections.Generic.IReadOnlyDictionary`2"/>.
        /// </summary>
        /// <value>A collection containing the keys of the <see cref="T:System.Collections.Generic.IReadOnlyDictionary`2"/>.</value>
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

        /// <summary>
        /// Gets a collection containing the values in the <see cref="T:System.Collections.Generic.Dictionary`2"/>.
        /// </summary>
        /// <value>A <see cref="T:System.Collections.Generic.Dictionary`2.ValueCollection"/> containing the values in the <see cref="T:System.Collections.Generic.Dictionary`2"/>.</value>
        /// <remarks>
        /// The order of the values in the <see cref="System.Collections.Generic.Dictionary{T,U}.ValueCollection"/> is unspecified, but it is the same order as the associated keys in the <see cref="System.Collections.Generic.Dictionary{T,U}.KeyCollection"/> returned by the <see cref="System.Collections.Generic.Dictionary{T,U}.Keys"/> property.
        /// The returned <see cref="System.Collections.Generic.Dictionary{T,U}.ValueCollection"/> is not a static copy; instead, the <see cref="System.Collections.Generic.Dictionary{T,U}.ValueCollection"/> refers back to the values in the original <see cref="System.Collections.Generic.Dictionary{T,U}"/>. Therefore, changes to the <see cref="System.Collections.Generic.Dictionary{T,U}"/> continue to be reflected in the <see cref="System.Collections.Generic.Dictionary{T,U}.ValueCollection"/>.
        /// Getting the value of this property is an O(1) operation.
        /// This code example shows how to enumerate the values in the dictionary using the <see cref="System.Collections.Generic.Dictionary{T,U}.Values"/> property, and how to enumerate the keys and values in the dictionary.
        /// This code example is part of a larger example provided for the <see cref="System.Collections.Generic.Dictionary{T,U}"/> class (<c>openWith</c> is the name of the Dictionary used in this example).
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.cs" id="Snippet8" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.fs" id="Snippet8" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.vb" id="Snippet8" />
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.cs" id="Snippet7" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.fs" id="Snippet7" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.vb" id="Snippet7" />
        /// </remarks>
        public ValueCollection Values => _values ??= new ValueCollection(this);

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <value>An <see cref="T:System.Collections.Generic.ICollection`1"/> of type <paramref name="TValue"/> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.</value>
        ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;

        /// <summary>
        /// Gets a collection containing the values of the <see cref="T:System.Collections.Generic.IReadOnlyDictionary`2"/>.
        /// </summary>
        /// <value>A collection containing the values of the <see cref="T:System.Collections.Generic.IReadOnlyDictionary`2"/>.</value>
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

        public TValue this[TKey key]
        {
            get
            {
                ref TValue value = ref FindValue(key);
                if (!Unsafe.IsNullRef(ref value))
                {
                    return value;
                }

                ThrowHelper.ThrowKeyNotFoundException(key);
                return default;
            }
            set
            {
                bool modified = TryInsert(key, value, InsertionBehavior.OverwriteExisting);
                Debug.Assert(modified);
            }
        }

        /// <summary>
        /// Adds the specified key and value to the dictionary.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add. The value can be <see langword="null"/> for reference types.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentException">An element with the same key already exists in the <see cref="T:System.Collections.Generic.Dictionary`2"/>.</exception>
        /// <remarks>
        /// You can also use the <see cref="System.Collections.Generic.Dictionary{T,U}.Item"/> property to add new elements by setting the value of a key that does not exist in the <see cref="System.Collections.Generic.Dictionary{T,U}"/>; for example, <c>myCollection[myKey] = myValue</c> (in Visual Basic, <c>myCollection(myKey) = myValue</c>). However, if the specified key already exists in the <see cref="System.Collections.Generic.Dictionary{T,U}"/>, setting the <see cref="System.Collections.Generic.Dictionary{T,U}.Item"/> property overwrites the old value. In contrast, the <see cref="System.Collections.Generic.Dictionary{T,U}.Add"/> method throws an exception if a value with the specified key already exists.
        /// If the <see cref="System.Collections.Generic.Dictionary{T,U}.Count"/> property value already equals the capacity, the capacity of the <see cref="System.Collections.Generic.Dictionary{T,U}"/> is increased by automatically reallocating the internal array, and the existing elements are copied to the new array before the new element is added.
        /// A key cannot be <c>null</c>, but a value can be, if <c>TValue</c> is a reference type.
        /// If <see cref="System.Collections.Generic.Dictionary{T,U}.Count"/> is less than the capacity, this method approaches an O(1) operation. If the capacity must be increased to accommodate the new element, this method becomes an O(<c>n</c>) operation, where <c>n</c> is <see cref="System.Collections.Generic.Dictionary{T,U}.Count"/>.
        /// The following code example creates an empty <see cref="System.Collections.Generic.Dictionary{T,U}"/> of strings with string keys and uses the <see cref="System.Collections.Generic.Dictionary{T,U}.Add"/> method to add some elements. The example demonstrates that the <see cref="System.Collections.Generic.Dictionary{T,U}.Add"/> method throws an <see cref="System.ArgumentException"/> when attempting to add a duplicate key.
        /// This code example is part of a larger example provided for the <see cref="System.Collections.Generic.Dictionary{T,U}"/> class.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.cs" id="Snippet2" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.fs" id="Snippet2" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.vb" id="Snippet2" />
        /// </remarks>
        public void Add(TKey key, TValue value)
        {
            bool modified = TryInsert(key, value, InsertionBehavior.ThrowOnExisting);
            Debug.Assert(modified); // If there was an existing key and the Add failed, an exception will already have been thrown.
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> keyValuePair) =>
            Add(keyValuePair.Key, keyValuePair.Value);

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> keyValuePair)
        {
            ref TValue value = ref FindValue(keyValuePair.Key);
            if (!Unsafe.IsNullRef(ref value) && EqualityComparer<TValue>.Default.Equals(value, keyValuePair.Value))
            {
                return true;
            }

            return false;
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> keyValuePair)
        {
            ref TValue value = ref FindValue(keyValuePair.Key);
            if (!Unsafe.IsNullRef(ref value) && EqualityComparer<TValue>.Default.Equals(value, keyValuePair.Value))
            {
                Remove(keyValuePair.Key);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes all keys and values from the <see cref="T:System.Collections.Generic.Dictionary`2"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="System.Collections.Generic.Dictionary{T,U}.Count"/> property is set to 0, and references to other objects from elements of the collection are also released. The capacity remains unchanged.
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is the capacity of the dictionary.
        /// .NET Core 3.0+ only: this mutating method may be safely called without invalidating active enumerators on the <see cref="System.Collections.Generic.Dictionary{T,U}"/> instance. This does not imply thread safety.
        /// </remarks>
        public void Clear()
        {
            int count = _count;
            if (count > 0)
            {
                Debug.Assert(_buckets != null, "_buckets should be non-null");
                Debug.Assert(_entries != null, "_entries should be non-null");

                Array.Clear(_buckets);

                _count = 0;
                _freeList = -1;
                _freeCount = 0;
                Array.Clear(_entries, 0, count);
            }
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.Dictionary`2"/> contains the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.Dictionary`2"/>.</param>
        /// <returns><see langword="true"/> if the <see cref="T:System.Collections.Generic.Dictionary`2"/> contains an element with the specified key; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// This method approaches an O(1) operation.
        /// The following code example shows how to use the <see cref="System.Collections.Generic.Dictionary{T,U}.ContainsKey"/> method to test whether a key exists prior to calling the <see cref="System.Collections.Generic.Dictionary{T,U}.Add"/> method. It also shows how to use the <see cref="System.Collections.Generic.Dictionary{T,U}.TryGetValue"/> method to retrieve values, which is an efficient way to retrieve values when a program frequently tries keys that are not in the dictionary. Finally, it shows the least efficient way to test whether keys exist, by using the <see cref="System.Collections.Generic.Dictionary{T,U}.Item"/> property (the indexer in C#).
        /// This code example is part of a larger example provided for the <see cref="System.Collections.Generic.Dictionary{T,U}"/> class (<c>openWith</c> is the name of the Dictionary used in this example).
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.cs" id="Snippet6" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.fs" id="Snippet6" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.vb" id="Snippet6" />
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.cs" id="Snippet5" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.fs" id="Snippet5" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.vb" id="Snippet5" />
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.cs" id="Snippet4" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.fs" id="Snippet4" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.vb" id="Snippet4" />
        /// </remarks>
        public bool ContainsKey(TKey key) =>
            !Unsafe.IsNullRef(ref FindValue(key));

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.Dictionary`2"/> contains a specific value.
        /// </summary>
        /// <param name="value">The value to locate in the <see cref="T:System.Collections.Generic.Dictionary`2"/>. The value can be <see langword="null"/> for reference types.</param>
        /// <returns><see langword="true"/> if the <see cref="T:System.Collections.Generic.Dictionary`2"/> contains an element with the specified value; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This method determines equality using the default equality comparer <see cref="System.Collections.Generic.EqualityComparer{T}.Default">Default</see> for <c>TValue</c>, the type of values in the dictionary.
        /// This method performs a linear search; therefore, the average execution time is proportional to <see cref="System.Collections.Generic.Dictionary{T,U}.Count"/>. That is, this method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="System.Collections.Generic.Dictionary{T,U}.Count"/>.
        /// </remarks>
        public bool ContainsValue(TValue value)
        {
            Entry[]? entries = _entries;
            if (value == null)
            {
                for (int i = 0; i < _count; i++)
                {
                    if (entries![i].next >= -1 && entries[i].value == null)
                    {
                        return true;
                    }
                }
            }
            else if (typeof(TValue).IsValueType)
            {
                // ValueType: Devirtualize with EqualityComparer<TValue>.Default intrinsic
                for (int i = 0; i < _count; i++)
                {
                    if (entries![i].next >= -1 && EqualityComparer<TValue>.Default.Equals(entries[i].value, value))
                    {
                        return true;
                    }
                }
            }
            else
            {
                // Object type: Shared Generic, EqualityComparer<TValue>.Default won't devirtualize
                // https://github.com/dotnet/runtime/issues/10050
                // So cache in a local rather than get EqualityComparer per loop iteration
                EqualityComparer<TValue> defaultComparer = EqualityComparer<TValue>.Default;
                for (int i = 0; i < _count; i++)
                {
                    if (entries![i].next >= -1 && defaultComparer.Equals(entries[i].value, value))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an array, starting at the specified array index.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The array must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than 0.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="array"/> is multidimensional. -or- <paramref name="array"/> does not have zero-based indexing. -or- The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="index"/> to the end of the destination <paramref name="array"/>. -or- The type of the source <see cref="T:System.Collections.Generic.ICollection`1"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.</exception>
        /// <remarks>
        /// <note type="note">
        /// If the type of the source <see cref="System.Collections.ICollection"/> cannot be cast automatically to the type of the destination <c>array</c>, the nongeneric implementations of <see cref="System.Collections.ICollection.CopyTo">CopyTo</see> throw an <see cref="System.InvalidCastException"/>, whereas the generic implementations throw an <see cref="System.ArgumentException"/>.
        /// </note>
        /// Each element copied from a <see cref="System.Collections.Generic.Dictionary{T,U}"/> is a <see cref="System.Collections.Generic.KeyValuePair{T,U}"/> structure representing a value and its key.
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="System.Collections.Generic.Dictionary{T,U}.Count"/>.
        /// </remarks>
        private void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
        {
            if (array == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
            }

            if ((uint)index > (uint)array.Length)
            {
                ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException();
            }

            if (array.Length - index < Count)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);
            }

            int count = _count;
            Entry[]? entries = _entries;
            for (int i = 0; i < count; i++)
            {
                if (entries![i].next >= -1)
                {
                    array[index++] = new KeyValuePair<TKey, TValue>(entries[i].key, entries[i].value);
                }
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="T:System.Collections.Generic.Dictionary`2"/>.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.Dictionary`2.Enumerator"/> structure for the <see cref="T:System.Collections.Generic.Dictionary`2"/>.</returns>
        /// <remarks>
        /// For purposes of enumeration, each item is a <see cref="System.Collections.Generic.KeyValuePair{T,U}"/> structure representing a value and its key.
        /// The <c>foreach</c> statement of the C# language (<c>For Each</c> in Visual Basic) hides the complexity of enumerators. Therefore, using <c>foreach</c> is recommended, instead of directly manipulating the enumerator.
        /// Enumerators can be used to read the data in the collection, but they cannot be used to modify the underlying collection.
        /// Initially, the enumerator is positioned before the first element in the collection. At this position, <see cref="System.Collections.Generic.Dictionary{T,U}.Enumerator.Current"/> is undefined. You must call the <see cref="System.Collections.Generic.Dictionary{T,U}.Enumerator.MoveNext"/> method to advance the enumerator to the first element of the collection before reading the value of <see cref="System.Collections.Generic.Dictionary{T,U}.Enumerator.Current"/>.
        /// The <see cref="System.Collections.Generic.Dictionary{T,U}.Enumerator.Current"/> property returns the same element until the <see cref="System.Collections.Generic.Dictionary{T,U}.Enumerator.MoveNext"/> method is called. <see cref="System.Collections.Generic.Dictionary{T,U}.Enumerator.MoveNext"/> sets <see cref="System.Collections.Generic.Dictionary{T,U}.Enumerator.Current"/> to the next element.
        /// If <see cref="System.Collections.Generic.Dictionary{T,U}.Enumerator.MoveNext"/> passes the end of the collection, the enumerator is positioned after the last element in the collection and <see cref="System.Collections.Generic.Dictionary{T,U}.Enumerator.MoveNext"/> returns <c>false</c>. When the enumerator is at this position, subsequent calls to <see cref="System.Collections.Generic.Dictionary{T,U}.Enumerator.MoveNext"/> also return <c>false</c>. If the last call to <see cref="System.Collections.Generic.Dictionary{T,U}.Enumerator.MoveNext"/> returned <c>false</c>, <see cref="System.Collections.Generic.Dictionary{T,U}.Enumerator.Current"/> is undefined. You cannot set <see cref="System.Collections.Generic.Dictionary{T,U}.Enumerator.Current"/> to the first element of the collection again; you must create a new enumerator instance instead.
        /// An enumerator remains valid as long as the collection remains unchanged. If changes are made to the collection, such as adding elements or changing the capacity, the enumerator is irrecoverably invalidated and the next call to <see cref="System.Collections.Generic.Dictionary{T,U}.Enumerator.MoveNext"/> or <see cref="System.Collections.Generic.Dictionary{T,U}.Enumerator.System#Collections#IEnumerator#Reset"/> throws an <see cref="System.InvalidOperationException"/>.
        /// .NET Core 3.0+ only: The only mutating methods which do not invalidate enumerators are <see cref="System.Collections.Generic.Dictionary{T,U}.Remove"/> and <see cref="System.Collections.Generic.Dictionary{T,U}.Clear"/>.
        /// The enumerator does not have exclusive access to the collection; therefore, enumerating through a collection is intrinsically not a thread-safe procedure. To guarantee thread safety during enumeration, you can lock the collection during the entire enumeration.  To allow the collection to be accessed by multiple threads for reading and writing, you must implement your own synchronization.
        /// Default implementations of collections in the <see cref="System.Collections.Generic">Generic</see> namespace are not synchronized.
        /// This method is an O(1) operation.
        /// </remarks>
        public Enumerator GetEnumerator() => new Enumerator(this, Enumerator.KeyValuePair);

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() =>
            Count == 0 ? GenericEmptyEnumerator<KeyValuePair<TKey, TValue>>.Instance :
            GetEnumerator();

        /// <summary>
        /// Implements the <see cref="T:System.Runtime.Serialization.ISerializable"/> interface and returns the data needed to serialize the <see cref="T:System.Collections.Generic.Dictionary`2"/> instance.
        /// </summary>
        /// <param name="info">A <see cref="T:System.Runtime.Serialization.SerializationInfo"/> object that contains the information required to serialize the <see cref="T:System.Collections.Generic.Dictionary`2"/> instance.</param>
        /// <param name="context">A <see cref="T:System.Runtime.Serialization.StreamingContext"/> structure that contains the source and destination of the serialized stream associated with the <see cref="T:System.Collections.Generic.Dictionary`2"/> instance.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="info"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="System.Collections.Generic.Dictionary{T,U}.Count"/>.
        /// </remarks>
        [Obsolete(Obsoletions.LegacyFormatterImplMessage, DiagnosticId = Obsoletions.LegacyFormatterImplDiagId, UrlFormat = Obsoletions.SharedUrlFormat)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.info);
            }

            info.AddValue(VersionName, _version);
            info.AddValue(ComparerName, Comparer, typeof(IEqualityComparer<TKey>));
            info.AddValue(HashSizeName, _buckets == null ? 0 : _buckets.Length); // This is the length of the bucket array

            if (_buckets != null)
            {
                var array = new KeyValuePair<TKey, TValue>[Count];
                CopyTo(array, 0);
                info.AddValue(KeyValuePairsName, array, typeof(KeyValuePair<TKey, TValue>[]));
            }
        }

        internal ref TValue FindValue(TKey key)
        {
            if (key == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);
            }

            ref Entry entry = ref Unsafe.NullRef<Entry>();
            if (_buckets != null)
            {
                Debug.Assert(_entries != null, "expected entries to be != null");
                IEqualityComparer<TKey>? comparer = _comparer;
                if (typeof(TKey).IsValueType && // comparer can only be null for value types; enable JIT to eliminate entire if block for ref types
                    comparer == null)
                {
                    // TODO: Replace with just key.GetHashCode once https://github.com/dotnet/runtime/issues/117521 is resolved.
                    uint hashCode = (uint)EqualityComparer<TKey>.Default.GetHashCode(key);
                    int i = GetBucket(hashCode);
                    Entry[]? entries = _entries;
                    uint collisionCount = 0;

                    // ValueType: Devirtualize with EqualityComparer<TKey>.Default intrinsic
                    i--; // Value in _buckets is 1-based; subtract 1 from i. We do it here so it fuses with the following conditional.
                    do
                    {
                        // Test in if to drop range check for following array access
                        if ((uint)i >= (uint)entries.Length)
                        {
                            goto ReturnNotFound;
                        }

                        entry = ref entries[i];
                        if (entry.hashCode == hashCode && EqualityComparer<TKey>.Default.Equals(entry.key, key))
                        {
                            goto ReturnFound;
                        }

                        i = entry.next;

                        collisionCount++;
                    } while (collisionCount <= (uint)entries.Length);

                    // The chain of entries forms a loop; which means a concurrent update has happened.
                    // Break out of the loop and throw, rather than looping forever.
                    goto ConcurrentOperation;
                }
                else
                {
                    Debug.Assert(comparer is not null);
                    uint hashCode = (uint)comparer.GetHashCode(key);
                    int i = GetBucket(hashCode);
                    Entry[]? entries = _entries;
                    uint collisionCount = 0;
                    i--; // Value in _buckets is 1-based; subtract 1 from i. We do it here so it fuses with the following conditional.
                    do
                    {
                        // Test in if to drop range check for following array access
                        if ((uint)i >= (uint)entries.Length)
                        {
                            goto ReturnNotFound;
                        }

                        entry = ref entries[i];
                        if (entry.hashCode == hashCode && comparer.Equals(entry.key, key))
                        {
                            goto ReturnFound;
                        }

                        i = entry.next;

                        collisionCount++;
                    } while (collisionCount <= (uint)entries.Length);

                    // The chain of entries forms a loop; which means a concurrent update has happened.
                    // Break out of the loop and throw, rather than looping forever.
                    goto ConcurrentOperation;
                }
            }

            goto ReturnNotFound;

        ConcurrentOperation:
            ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
        ReturnFound:
            ref TValue value = ref entry.value;
        Return:
            return ref value;
        ReturnNotFound:
            value = ref Unsafe.NullRef<TValue>();
            goto Return;
        }

        private int Initialize(int capacity)
        {
            int size = HashHelpers.GetPrime(capacity);
            int[] buckets = new int[size];
            Entry[] entries = new Entry[size];

            // Assign member variables after both arrays allocated to guard against corruption from OOM if second fails
            _freeList = -1;
#if TARGET_64BIT
            _fastModMultiplier = HashHelpers.GetFastModMultiplier((uint)size);
#endif
            _buckets = buckets;
            _entries = entries;

            return size;
        }

        private bool TryInsert(TKey key, TValue value, InsertionBehavior behavior)
        {
            // NOTE: this method is mirrored in CollectionsMarshal.GetValueRefOrAddDefault below.
            // If you make any changes here, make sure to keep that version in sync as well.

            if (key == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);
            }

            if (_buckets == null)
            {
                Initialize(0);
            }
            Debug.Assert(_buckets != null);

            Entry[]? entries = _entries;
            Debug.Assert(entries != null, "expected entries to be non-null");

            IEqualityComparer<TKey>? comparer = _comparer;
            Debug.Assert(comparer is not null || typeof(TKey).IsValueType);
            uint hashCode = (uint)((typeof(TKey).IsValueType && comparer == null) ? key.GetHashCode() : comparer!.GetHashCode(key));

            uint collisionCount = 0;
            ref int bucket = ref GetBucket(hashCode);
            int i = bucket - 1; // Value in _buckets is 1-based

            if (typeof(TKey).IsValueType && // comparer can only be null for value types; enable JIT to eliminate entire if block for ref types
                comparer == null)
            {
                // ValueType: Devirtualize with EqualityComparer<TKey>.Default intrinsic
                while ((uint)i < (uint)entries.Length)
                {
                    if (entries[i].hashCode == hashCode && EqualityComparer<TKey>.Default.Equals(entries[i].key, key))
                    {
                        if (behavior == InsertionBehavior.OverwriteExisting)
                        {
                            entries[i].value = value;
                            return true;
                        }

                        if (behavior == InsertionBehavior.ThrowOnExisting)
                        {
                            ThrowHelper.ThrowAddingDuplicateWithKeyArgumentException(key);
                        }

                        return false;
                    }

                    i = entries[i].next;

                    collisionCount++;
                    if (collisionCount > (uint)entries.Length)
                    {
                        // The chain of entries forms a loop; which means a concurrent update has happened.
                        // Break out of the loop and throw, rather than looping forever.
                        ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
                    }
                }
            }
            else
            {
                Debug.Assert(comparer is not null);
                while ((uint)i < (uint)entries.Length)
                {
                    if (entries[i].hashCode == hashCode && comparer.Equals(entries[i].key, key))
                    {
                        if (behavior == InsertionBehavior.OverwriteExisting)
                        {
                            entries[i].value = value;
                            return true;
                        }

                        if (behavior == InsertionBehavior.ThrowOnExisting)
                        {
                            ThrowHelper.ThrowAddingDuplicateWithKeyArgumentException(key);
                        }

                        return false;
                    }

                    i = entries[i].next;

                    collisionCount++;
                    if (collisionCount > (uint)entries.Length)
                    {
                        // The chain of entries forms a loop; which means a concurrent update has happened.
                        // Break out of the loop and throw, rather than looping forever.
                        ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
                    }
                }
            }

            int index;
            if (_freeCount > 0)
            {
                index = _freeList;
                Debug.Assert((StartOfFreeList - entries[_freeList].next) >= -1, "shouldn't overflow because `next` cannot underflow");
                _freeList = StartOfFreeList - entries[_freeList].next;
                _freeCount--;
            }
            else
            {
                int count = _count;
                if (count == entries.Length)
                {
                    Resize();
                    bucket = ref GetBucket(hashCode);
                }
                index = count;
                _count = count + 1;
                entries = _entries;
            }

            ref Entry entry = ref entries![index];
            entry.hashCode = hashCode;
            entry.next = bucket - 1; // Value in _buckets is 1-based
            entry.key = key;
            entry.value = value;
            bucket = index + 1; // Value in _buckets is 1-based
            _version++;

            // Value types never rehash
            if (!typeof(TKey).IsValueType && collisionCount > HashHelpers.HashCollisionThreshold && comparer is NonRandomizedStringEqualityComparer)
            {
                // If we hit the collision threshold we'll need to switch to the comparer which is using randomized string hashing
                // i.e. EqualityComparer<string>.Default.
                Resize(entries.Length, true);
            }

            return true;
        }

        /// <summary>
        /// Gets an instance of a type that may be used to perform operations on the current <see cref="Dictionary{TKey, TValue}"/>
        /// using a <typeparamref name="TAlternateKey"/> as a key instead of a <typeparamref name="TKey"/>.
        /// </summary>
        /// <typeparam name="TAlternateKey">The alternate type of a key for performing lookups.</typeparam>
        /// <returns>The created lookup instance.</returns>
        /// <exception cref="InvalidOperationException">The dictionary's comparer is not compatible with <typeparamref name="TAlternateKey"/>.</exception>
        /// <remarks>
        /// The dictionary must be using a comparer that implements <see cref="IAlternateEqualityComparer{TAlternateKey, TKey}"/> with
        /// <typeparamref name="TAlternateKey"/> and <typeparamref name="TKey"/>. If it doesn't, an exception will be thrown.
        /// </remarks>
        public AlternateLookup<TAlternateKey> GetAlternateLookup<TAlternateKey>()
            where TAlternateKey : notnull, allows ref struct
        {
            if (!AlternateLookup<TAlternateKey>.IsCompatibleKey(this))
            {
                ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_IncompatibleComparer);
            }

            return new AlternateLookup<TAlternateKey>(this);
        }

        /// <summary>
        /// Gets an instance of a type that may be used to perform operations on the current <see cref="Dictionary{TKey, TValue}"/>
        /// using a <typeparamref name="TAlternateKey"/> as a key instead of a <typeparamref name="TKey"/>.
        /// </summary>
        /// <typeparam name="TAlternateKey">The alternate type of a key for performing lookups.</typeparam>
        /// <param name="lookup">The created lookup instance when the method returns true, or a default instance that should not be used if the method returns false.</param>
        /// <returns>true if a lookup could be created; otherwise, false.</returns>
        /// <remarks>
        /// The dictionary must be using a comparer that implements <see cref="IAlternateEqualityComparer{TAlternateKey, TKey}"/> with
        /// <typeparamref name="TAlternateKey"/> and <typeparamref name="TKey"/>. If it doesn't, the method will return false.
        /// </remarks>
        public bool TryGetAlternateLookup<TAlternateKey>(
            out AlternateLookup<TAlternateKey> lookup)
            where TAlternateKey : notnull, allows ref struct
        {
            if (AlternateLookup<TAlternateKey>.IsCompatibleKey(this))
            {
                lookup = new AlternateLookup<TAlternateKey>(this);
                return true;
            }

            lookup = default;
            return false;
        }

        /// <summary>
        /// Provides a type that may be used to perform operations on a <see cref="Dictionary{TKey, TValue}"/>
        /// using a <typeparamref name="TAlternateKey"/> as a key instead of a <typeparamref name="TKey"/>.
        /// </summary>
        /// <typeparam name="TAlternateKey">The alternate type of a key for performing lookups.</typeparam>
        public readonly struct AlternateLookup<TAlternateKey> where TAlternateKey : notnull, allows ref struct
        {
            /// <summary>Initialize the instance. The dictionary must have already been verified to have a compatible comparer.</summary>
            internal AlternateLookup(Dictionary<TKey, TValue> dictionary)
            {
                Debug.Assert(dictionary is not null);
                Debug.Assert(IsCompatibleKey(dictionary));
                Dictionary = dictionary;
            }

            /// <summary>Gets the <see cref="Dictionary{TKey, TValue}"/> against which this instance performs operations.</summary>
            public Dictionary<TKey, TValue> Dictionary { get; }

            /// <summary>Gets or sets the value associated with the specified alternate key.</summary>
            /// <param name="key">The alternate key of the value to get or set.</param>
            /// <value>
            /// The value associated with the specified alternate key. If the specified alternate key is not found, a get operation throws
            /// a <see cref="KeyNotFoundException"/>, and a set operation creates a new element with the specified key.
            /// </value>
            /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
            /// <exception cref="KeyNotFoundException">The property is retrieved and alternate key does not exist in the collection.</exception>
            public TValue this[TAlternateKey key]
            {
                get
                {
                    ref TValue value = ref FindValue(key, out _);
                    if (Unsafe.IsNullRef(ref value))
                    {
                        ThrowHelper.ThrowKeyNotFoundException(GetAlternateComparer(Dictionary).Create(key));
                    }

                    return value;
                }
                set => GetValueRefOrAddDefault(key, out _) = value;
            }

            /// <summary>Checks whether the dictionary has a comparer compatible with <typeparamref name="TAlternateKey"/>.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal static bool IsCompatibleKey(Dictionary<TKey, TValue> dictionary)
            {
                Debug.Assert(dictionary is not null);
                return dictionary._comparer is IAlternateEqualityComparer<TAlternateKey, TKey>;
            }

            /// <summary>Gets the dictionary's alternate comparer. The dictionary must have already been verified as compatible.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal static IAlternateEqualityComparer<TAlternateKey, TKey> GetAlternateComparer(Dictionary<TKey, TValue> dictionary)
            {
                Debug.Assert(IsCompatibleKey(dictionary));
                return Unsafe.As<IAlternateEqualityComparer<TAlternateKey, TKey>>(dictionary._comparer)!;
            }

            /// <summary>Gets the value associated with the specified alternate key.</summary>
            /// <param name="key">The alternate key of the value to get.</param>
            /// <param name="value">
            /// When this method returns, contains the value associated with the specified key, if the key is found;
            /// otherwise, the default value for the type of the value parameter.
            /// </param>
            /// <returns><see langword="true"/> if an entry was found; otherwise, <see langword="false"/>.</returns>
            /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
            public bool TryGetValue(TAlternateKey key, [MaybeNullWhen(false)] out TValue value)
            {
                ref TValue valueRef = ref FindValue(key, out _);
                if (!Unsafe.IsNullRef(ref valueRef))
                {
                    value = valueRef;
                    return true;
                }

                value = default;
                return false;
            }

            /// <summary>Gets the value associated with the specified alternate key.</summary>
            /// <param name="key">The alternate key of the value to get.</param>
            /// <param name="actualKey">
            /// When this method returns, contains the actual key associated with the alternate key, if the key is found;
            /// otherwise, the default value for the type of the key parameter.
            /// </param>
            /// <param name="value">
            /// When this method returns, contains the value associated with the specified key, if the key is found;
            /// otherwise, the default value for the type of the value parameter.
            /// </param>
            /// <returns><see langword="true"/> if an entry was found; otherwise, <see langword="false"/>.</returns>
            /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
            public bool TryGetValue(TAlternateKey key, [MaybeNullWhen(false)] out TKey actualKey, [MaybeNullWhen(false)] out TValue value)
            {
                ref TValue valueRef = ref FindValue(key, out actualKey);
                if (!Unsafe.IsNullRef(ref valueRef))
                {
                    value = valueRef;
                    Debug.Assert(actualKey is not null);
                    return true;
                }

                value = default;
                return false;
            }

            /// <summary>Determines whether the <see cref="Dictionary{TKey, TValue}"/> contains the specified alternate key.</summary>
            /// <param name="key">The alternate key to check.</param>
            /// <returns><see langword="true"/> if the key is in the dictionary; otherwise, <see langword="false"/>.</returns>
            /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
            public bool ContainsKey(TAlternateKey key) =>
                !Unsafe.IsNullRef(ref FindValue(key, out _));

            /// <summary>Finds the entry associated with the specified alternate key.</summary>
            /// <param name="key">The alternate key.</param>
            /// <param name="actualKey">The actual key, if found.</param>
            /// <returns>A reference to the value associated with the key, if found; otherwise, a null reference.</returns>
            internal ref TValue FindValue(TAlternateKey key, [MaybeNullWhen(false)] out TKey actualKey)
            {
                Dictionary<TKey, TValue> dictionary = Dictionary;
                IAlternateEqualityComparer<TAlternateKey, TKey> comparer = GetAlternateComparer(dictionary);

                ref Entry entry = ref Unsafe.NullRef<Entry>();
                if (dictionary._buckets != null)
                {
                    Debug.Assert(dictionary._entries != null, "expected entries to be != null");

                    uint hashCode = (uint)comparer.GetHashCode(key);
                    int i = dictionary.GetBucket(hashCode);
                    Entry[]? entries = dictionary._entries;
                    uint collisionCount = 0;
                    i--; // Value in _buckets is 1-based; subtract 1 from i. We do it here so it fuses with the following conditional.
                    do
                    {
                        // Should be a while loop https://github.com/dotnet/runtime/issues/9422
                        // Test in if to drop range check for following array access
                        if ((uint)i >= (uint)entries.Length)
                        {
                            goto ReturnNotFound;
                        }

                        entry = ref entries[i];
                        if (entry.hashCode == hashCode && comparer.Equals(key, entry.key))
                        {
                            goto ReturnFound;
                        }

                        i = entry.next;

                        collisionCount++;
                    } while (collisionCount <= (uint)entries.Length);

                    // The chain of entries forms a loop; which means a concurrent update has happened.
                    // Break out of the loop and throw, rather than looping forever.
                    goto ConcurrentOperation;
                }

                goto ReturnNotFound;

            ConcurrentOperation:
                ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
            ReturnFound:
                ref TValue value = ref entry.value;
                actualKey = entry.key;
            Return:
                return ref value;
            ReturnNotFound:
                value = ref Unsafe.NullRef<TValue>();
                actualKey = default!;
                goto Return;
            }

            /// <summary>Removes the value with the specified alternate key from the <see cref="Dictionary{TKey, TValue}"/>.</summary>
            /// <param name="key">The alternate key of the element to remove.</param>
            /// <returns>true if the element is successfully found and removed; otherwise, false.</returns>
            /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
            public bool Remove(TAlternateKey key) =>
                Remove(key, out _, out _);

            /// <summary>
            /// Removes the value with the specified alternate key from the <see cref="Dictionary{TKey, TValue}"/>,
            /// and copies the element to the value parameter.
            /// </summary>
            /// <param name="key">The alternate key of the element to remove.</param>
            /// <param name="actualKey">The removed key.</param>
            /// <param name="value">The removed element.</param>
            /// <returns>true if the element is successfully found and removed; otherwise, false.</returns>
            /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
            public bool Remove(TAlternateKey key, [MaybeNullWhen(false)] out TKey actualKey, [MaybeNullWhen(false)] out TValue value)
            {
                Dictionary<TKey, TValue> dictionary = Dictionary;
                IAlternateEqualityComparer<TAlternateKey, TKey> comparer = GetAlternateComparer(dictionary);

                if (dictionary._buckets != null)
                {
                    Debug.Assert(dictionary._entries != null, "entries should be non-null");
                    uint collisionCount = 0;

                    uint hashCode = (uint)comparer.GetHashCode(key);

                    ref int bucket = ref dictionary.GetBucket(hashCode);
                    Entry[]? entries = dictionary._entries;
                    int last = -1;
                    int i = bucket - 1; // Value in buckets is 1-based
                    while (i >= 0)
                    {
                        ref Entry entry = ref entries[i];

                        if (entry.hashCode == hashCode && comparer.Equals(key, entry.key))
                        {
                            if (last < 0)
                            {
                                bucket = entry.next + 1; // Value in buckets is 1-based
                            }
                            else
                            {
                                entries[last].next = entry.next;
                            }

                            actualKey = entry.key;
                            value = entry.value;

                            Debug.Assert((StartOfFreeList - dictionary._freeList) < 0, "shouldn't underflow because max hashtable length is MaxPrimeArrayLength = 0x7FEFFFFD(2146435069) _freelist underflow threshold 2147483646");
                            entry.next = StartOfFreeList - dictionary._freeList;

                            if (RuntimeHelpers.IsReferenceOrContainsReferences<TKey>())
                            {
                                entry.key = default!;
                            }

                            if (RuntimeHelpers.IsReferenceOrContainsReferences<TValue>())
                            {
                                entry.value = default!;
                            }

                            dictionary._freeList = i;
                            dictionary._freeCount++;
                            return true;
                        }

                        last = i;
                        i = entry.next;

                        collisionCount++;
                        if (collisionCount > (uint)entries.Length)
                        {
                            // The chain of entries forms a loop; which means a concurrent update has happened.
                            // Break out of the loop and throw, rather than looping forever.
                            ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
                        }
                    }
                }

                actualKey = default;
                value = default;
                return false;
            }

            /// <summary>Attempts to add the specified key and value to the dictionary.</summary>
            /// <param name="key">The alternate key of the element to add.</param>
            /// <param name="value">The value of the element to add.</param>
            /// <returns>true if the key/value pair was added to the dictionary successfully; otherwise, false.</returns>
            /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
            public bool TryAdd(TAlternateKey key, TValue value)
            {
                ref TValue? slot = ref GetValueRefOrAddDefault(key, out bool exists);
                if (!exists)
                {
                    slot = value;
                    return true;
                }

                return false;
            }

            /// <inheritdoc cref="CollectionsMarshal.GetValueRefOrAddDefault{TKey, TValue}(Dictionary{TKey, TValue}, TKey, out bool)"/>
            internal ref TValue? GetValueRefOrAddDefault(TAlternateKey key, out bool exists)
            {
                // NOTE: this method is a mirror of GetValueRefOrAddDefault above. Keep it in sync.

                Dictionary<TKey, TValue> dictionary = Dictionary;
                IAlternateEqualityComparer<TAlternateKey, TKey> comparer = GetAlternateComparer(dictionary);

                if (dictionary._buckets == null)
                {
                    dictionary.Initialize(0);
                }
                Debug.Assert(dictionary._buckets != null);

                Entry[]? entries = dictionary._entries;
                Debug.Assert(entries != null, "expected entries to be non-null");

                uint hashCode = (uint)comparer.GetHashCode(key);

                uint collisionCount = 0;
                ref int bucket = ref dictionary.GetBucket(hashCode);
                int i = bucket - 1; // Value in _buckets is 1-based

                Debug.Assert(comparer is not null);
                while ((uint)i < (uint)entries.Length)
                {
                    if (entries[i].hashCode == hashCode && comparer.Equals(key, entries[i].key))
                    {
                        exists = true;

                        return ref entries[i].value!;
                    }

                    i = entries[i].next;

                    collisionCount++;
                    if (collisionCount > (uint)entries.Length)
                    {
                        // The chain of entries forms a loop; which means a concurrent update has happened.
                        // Break out of the loop and throw, rather than looping forever.
                        ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
                    }
                }

                TKey actualKey = comparer.Create(key);
                if (actualKey is null)
                {
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);
                }

                int index;
                if (dictionary._freeCount > 0)
                {
                    index = dictionary._freeList;
                    Debug.Assert((StartOfFreeList - entries[dictionary._freeList].next) >= -1, "shouldn't overflow because `next` cannot underflow");
                    dictionary._freeList = StartOfFreeList - entries[dictionary._freeList].next;
                    dictionary._freeCount--;
                }
                else
                {
                    int count = dictionary._count;
                    if (count == entries.Length)
                    {
                        dictionary.Resize();
                        bucket = ref dictionary.GetBucket(hashCode);
                    }
                    index = count;
                    dictionary._count = count + 1;
                    entries = dictionary._entries;
                }

                ref Entry entry = ref entries![index];
                entry.hashCode = hashCode;
                entry.next = bucket - 1; // Value in _buckets is 1-based
                entry.key = actualKey;
                entry.value = default!;
                bucket = index + 1; // Value in _buckets is 1-based
                dictionary._version++;

                // Value types never rehash
                if (!typeof(TKey).IsValueType && collisionCount > HashHelpers.HashCollisionThreshold && comparer is NonRandomizedStringEqualityComparer)
                {
                    // If we hit the collision threshold we'll need to switch to the comparer which is using randomized string hashing
                    // i.e. EqualityComparer<string>.Default.
                    dictionary.Resize(entries.Length, true);

                    exists = false;

                    // At this point the entries array has been resized, so the current reference we have is no longer valid.
                    // We're forced to do a new lookup and return an updated reference to the new entry instance. This new
                    // lookup is guaranteed to always find a value though and it will never return a null reference here.
                    ref TValue? value = ref dictionary.FindValue(actualKey)!;

                    Debug.Assert(!Unsafe.IsNullRef(ref value), "the lookup result cannot be a null ref here");

                    return ref value;
                }

                exists = false;

                return ref entry.value!;
            }
        }

        /// <summary>
        /// A helper class containing APIs exposed through <see cref="CollectionsMarshal"/> or <see cref="CollectionExtensions"/>.
        /// These methods are relatively niche and only used in specific scenarios, so adding them in a separate type avoids
        /// the additional overhead on each <see cref="Dictionary{TKey, TValue}"/> instantiation, especially in AOT scenarios.
        /// </summary>
        internal static class CollectionsMarshalHelper
        {
            /// <inheritdoc cref="CollectionsMarshal.GetValueRefOrAddDefault{TKey, TValue}(Dictionary{TKey, TValue}, TKey, out bool)"/>
            public static ref TValue? GetValueRefOrAddDefault(Dictionary<TKey, TValue> dictionary, TKey key, out bool exists)
            {
                // NOTE: this method is mirrored by Dictionary<TKey, TValue>.TryInsert above.
                // If you make any changes here, make sure to keep that version in sync as well.

                if (key == null)
                {
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);
                }

                if (dictionary._buckets == null)
                {
                    dictionary.Initialize(0);
                }
                Debug.Assert(dictionary._buckets != null);

                Entry[]? entries = dictionary._entries;
                Debug.Assert(entries != null, "expected entries to be non-null");

                IEqualityComparer<TKey>? comparer = dictionary._comparer;
                Debug.Assert(comparer is not null || typeof(TKey).IsValueType);
                uint hashCode = (uint)((typeof(TKey).IsValueType && comparer == null) ? key.GetHashCode() : comparer!.GetHashCode(key));

                uint collisionCount = 0;
                ref int bucket = ref dictionary.GetBucket(hashCode);
                int i = bucket - 1; // Value in _buckets is 1-based

                if (typeof(TKey).IsValueType && // comparer can only be null for value types; enable JIT to eliminate entire if block for ref types
                    comparer == null)
                {
                    // ValueType: Devirtualize with EqualityComparer<TKey>.Default intrinsic
                    while ((uint)i < (uint)entries.Length)
                    {
                        if (entries[i].hashCode == hashCode && EqualityComparer<TKey>.Default.Equals(entries[i].key, key))
                        {
                            exists = true;

                            return ref entries[i].value!;
                        }

                        i = entries[i].next;

                        collisionCount++;
                        if (collisionCount > (uint)entries.Length)
                        {
                            // The chain of entries forms a loop; which means a concurrent update has happened.
                            // Break out of the loop and throw, rather than looping forever.
                            ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
                        }
                    }
                }
                else
                {
                    Debug.Assert(comparer is not null);
                    while ((uint)i < (uint)entries.Length)
                    {
                        if (entries[i].hashCode == hashCode && comparer.Equals(entries[i].key, key))
                        {
                            exists = true;

                            return ref entries[i].value!;
                        }

                        i = entries[i].next;

                        collisionCount++;
                        if (collisionCount > (uint)entries.Length)
                        {
                            // The chain of entries forms a loop; which means a concurrent update has happened.
                            // Break out of the loop and throw, rather than looping forever.
                            ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
                        }
                    }
                }

                int index;
                if (dictionary._freeCount > 0)
                {
                    index = dictionary._freeList;
                    Debug.Assert((StartOfFreeList - entries[dictionary._freeList].next) >= -1, "shouldn't overflow because `next` cannot underflow");
                    dictionary._freeList = StartOfFreeList - entries[dictionary._freeList].next;
                    dictionary._freeCount--;
                }
                else
                {
                    int count = dictionary._count;
                    if (count == entries.Length)
                    {
                        dictionary.Resize();
                        bucket = ref dictionary.GetBucket(hashCode);
                    }
                    index = count;
                    dictionary._count = count + 1;
                    entries = dictionary._entries;
                }

                ref Entry entry = ref entries![index];
                entry.hashCode = hashCode;
                entry.next = bucket - 1; // Value in _buckets is 1-based
                entry.key = key;
                entry.value = default!;
                bucket = index + 1; // Value in _buckets is 1-based
                dictionary._version++;

                // Value types never rehash
                if (!typeof(TKey).IsValueType && collisionCount > HashHelpers.HashCollisionThreshold && comparer is NonRandomizedStringEqualityComparer)
                {
                    // If we hit the collision threshold we'll need to switch to the comparer which is using randomized string hashing
                    // i.e. EqualityComparer<string>.Default.
                    dictionary.Resize(entries.Length, true);

                    exists = false;

                    // At this point the entries array has been resized, so the current reference we have is no longer valid.
                    // We're forced to do a new lookup and return an updated reference to the new entry instance. This new
                    // lookup is guaranteed to always find a value though and it will never return a null reference here.
                    ref TValue? value = ref dictionary.FindValue(key)!;

                    Debug.Assert(!Unsafe.IsNullRef(ref value), "the lookup result cannot be a null ref here");

                    return ref value;
                }

                exists = false;

                return ref entry.value!;
            }
        }

        /// <summary>
        /// Implements the <see cref="T:System.Runtime.Serialization.ISerializable"/> interface and raises the deserialization event when the deserialization is complete.
        /// </summary>
        /// <param name="sender">The source of the deserialization event.</param>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> object associated with the current <see cref="T:System.Collections.Generic.Dictionary`2"/> instance is invalid.</exception>
        /// <remarks>
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="System.Collections.Generic.Dictionary{T,U}.Count"/>.
        /// </remarks>
        public virtual void OnDeserialization(object? sender)
        {
            HashHelpers.SerializationInfoTable.TryGetValue(this, out SerializationInfo? siInfo);

            if (siInfo == null)
            {
                // We can return immediately if this function is called twice.
                // Note we remove the serialization info from the table at the end of this method.
                return;
            }

            int realVersion = siInfo.GetInt32(VersionName);
            int hashsize = siInfo.GetInt32(HashSizeName);
            _comparer = (IEqualityComparer<TKey>)siInfo.GetValue(ComparerName, typeof(IEqualityComparer<TKey>))!; // When serialized if comparer is null, we use the default.

            if (hashsize != 0)
            {
                Initialize(hashsize);

                KeyValuePair<TKey, TValue>[]? array = (KeyValuePair<TKey, TValue>[]?)
                    siInfo.GetValue(KeyValuePairsName, typeof(KeyValuePair<TKey, TValue>[]));

                if (array == null)
                {
                    ThrowHelper.ThrowSerializationException(ExceptionResource.Serialization_MissingKeys);
                }

                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i].Key == null)
                    {
                        ThrowHelper.ThrowSerializationException(ExceptionResource.Serialization_NullKey);
                    }

                    Add(array[i].Key, array[i].Value);
                }
            }
            else
            {
                _buckets = null;
            }

            _version = realVersion;
            HashHelpers.SerializationInfoTable.Remove(this);
        }

        private void Resize() => Resize(HashHelpers.ExpandPrime(_count), false);

        private void Resize(int newSize, bool forceNewHashCodes)
        {
            // Value types never rehash
            Debug.Assert(!forceNewHashCodes || !typeof(TKey).IsValueType);
            Debug.Assert(_entries != null, "_entries should be non-null");
            Debug.Assert(newSize >= _entries.Length);

            Entry[] entries = new Entry[newSize];

            int count = _count;
            Array.Copy(_entries, entries, count);

            if (!typeof(TKey).IsValueType && forceNewHashCodes)
            {
                Debug.Assert(_comparer is NonRandomizedStringEqualityComparer);
                IEqualityComparer<TKey> comparer = _comparer = (IEqualityComparer<TKey>)((NonRandomizedStringEqualityComparer)_comparer).GetRandomizedEqualityComparer();

                for (int i = 0; i < count; i++)
                {
                    if (entries[i].next >= -1)
                    {
                        entries[i].hashCode = (uint)comparer.GetHashCode(entries[i].key);
                    }
                }
            }

            // Assign member variables after both arrays allocated to guard against corruption from OOM if second fails
            _buckets = new int[newSize];
#if TARGET_64BIT
            _fastModMultiplier = HashHelpers.GetFastModMultiplier((uint)newSize);
#endif
            for (int i = 0; i < count; i++)
            {
                if (entries[i].next >= -1)
                {
                    ref int bucket = ref GetBucket(entries[i].hashCode);
                    entries[i].next = bucket - 1; // Value in _buckets is 1-based
                    bucket = i + 1;
                }
            }

            _entries = entries;
        }

        /// <summary>
        /// Removes the value with the specified key from the <see cref="T:System.Collections.Generic.Dictionary`2"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns><see langword="true"/> if the element is successfully found and removed; otherwise, <see langword="false"/>. This method returns <see langword="false"/> if <paramref name="key"/> is not found in the <see cref="T:System.Collections.Generic.Dictionary`2"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// If the <see cref="System.Collections.Generic.Dictionary{T,U}"/> does not contain an element with the specified key, the <see cref="System.Collections.Generic.Dictionary{T,U}"/> remains unchanged. No exception is thrown.
        /// This method approaches an O(1) operation.
        /// .NET Core 3.0+ only: this mutating method may be safely called without invalidating active enumerators on the <see cref="System.Collections.Generic.Dictionary{T,U}"/> instance. This does not imply thread safety.
        /// The following code example shows how to remove a key/value pair from a dictionary using the <see cref="System.Collections.Generic.Dictionary{T,U}.Remove"/> method.
        /// This code example is part of a larger example provided for the <see cref="System.Collections.Generic.Dictionary{T,U}"/> class (<c>openWith</c> is the name of the Dictionary used in this example).
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.cs" id="Snippet10" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.fs" id="Snippet10" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.vb" id="Snippet10" />
        /// </remarks>
        public bool Remove(TKey key)
        {
            // The overload Remove(TKey key, out TValue value) is a copy of this method with one additional
            // statement to copy the value for entry being removed into the output parameter.
            // Code has been intentionally duplicated for performance reasons.

            if (key == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);
            }

            if (_buckets != null)
            {
                Debug.Assert(_entries != null, "entries should be non-null");
                uint collisionCount = 0;

                IEqualityComparer<TKey>? comparer = _comparer;
                Debug.Assert(typeof(TKey).IsValueType || comparer is not null);
                uint hashCode = (uint)(typeof(TKey).IsValueType && comparer == null ? key.GetHashCode() : comparer!.GetHashCode(key));

                ref int bucket = ref GetBucket(hashCode);
                Entry[]? entries = _entries;
                int last = -1;
                int i = bucket - 1; // Value in buckets is 1-based
                while (i >= 0)
                {
                    ref Entry entry = ref entries[i];

                    if (entry.hashCode == hashCode &&
                        (typeof(TKey).IsValueType && comparer == null ? EqualityComparer<TKey>.Default.Equals(entry.key, key) : comparer!.Equals(entry.key, key)))
                    {
                        if (last < 0)
                        {
                            bucket = entry.next + 1; // Value in buckets is 1-based
                        }
                        else
                        {
                            entries[last].next = entry.next;
                        }

                        Debug.Assert((StartOfFreeList - _freeList) < 0, "shouldn't underflow because max hashtable length is MaxPrimeArrayLength = 0x7FEFFFFD(2146435069) _freelist underflow threshold 2147483646");
                        entry.next = StartOfFreeList - _freeList;

                        if (RuntimeHelpers.IsReferenceOrContainsReferences<TKey>())
                        {
                            entry.key = default!;
                        }

                        if (RuntimeHelpers.IsReferenceOrContainsReferences<TValue>())
                        {
                            entry.value = default!;
                        }

                        _freeList = i;
                        _freeCount++;
                        return true;
                    }

                    last = i;
                    i = entry.next;

                    collisionCount++;
                    if (collisionCount > (uint)entries.Length)
                    {
                        // The chain of entries forms a loop; which means a concurrent update has happened.
                        // Break out of the loop and throw, rather than looping forever.
                        ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Removes the value with the specified key from the <see cref="T:System.Collections.Generic.Dictionary`2"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns><see langword="true"/> if the element is successfully found and removed; otherwise, <see langword="false"/>. This method returns <see langword="false"/> if <paramref name="key"/> is not found in the <see cref="T:System.Collections.Generic.Dictionary`2"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// If the <see cref="System.Collections.Generic.Dictionary{T,U}"/> does not contain an element with the specified key, the <see cref="System.Collections.Generic.Dictionary{T,U}"/> remains unchanged. No exception is thrown.
        /// This method approaches an O(1) operation.
        /// .NET Core 3.0+ only: this mutating method may be safely called without invalidating active enumerators on the <see cref="System.Collections.Generic.Dictionary{T,U}"/> instance. This does not imply thread safety.
        /// The following code example shows how to remove a key/value pair from a dictionary using the <see cref="System.Collections.Generic.Dictionary{T,U}.Remove"/> method.
        /// This code example is part of a larger example provided for the <see cref="System.Collections.Generic.Dictionary{T,U}"/> class (<c>openWith</c> is the name of the Dictionary used in this example).
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.cs" id="Snippet10" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.fs" id="Snippet10" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.vb" id="Snippet10" />
        /// </remarks>
        public bool Remove(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            // This overload is a copy of the overload Remove(TKey key) with one additional
            // statement to copy the value for entry being removed into the output parameter.
            // Code has been intentionally duplicated for performance reasons.

            if (key == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);
            }

            if (_buckets != null)
            {
                Debug.Assert(_entries != null, "entries should be non-null");
                uint collisionCount = 0;

                IEqualityComparer<TKey>? comparer = _comparer;
                Debug.Assert(typeof(TKey).IsValueType || comparer is not null);
                uint hashCode = (uint)(typeof(TKey).IsValueType && comparer == null ? key.GetHashCode() : comparer!.GetHashCode(key));

                ref int bucket = ref GetBucket(hashCode);
                Entry[]? entries = _entries;
                int last = -1;
                int i = bucket - 1; // Value in buckets is 1-based
                while (i >= 0)
                {
                    ref Entry entry = ref entries[i];

                    if (entry.hashCode == hashCode &&
                        (typeof(TKey).IsValueType && comparer == null ? EqualityComparer<TKey>.Default.Equals(entry.key, key) : comparer!.Equals(entry.key, key)))
                    {
                        if (last < 0)
                        {
                            bucket = entry.next + 1; // Value in buckets is 1-based
                        }
                        else
                        {
                            entries[last].next = entry.next;
                        }

                        value = entry.value;

                        Debug.Assert((StartOfFreeList - _freeList) < 0, "shouldn't underflow because max hashtable length is MaxPrimeArrayLength = 0x7FEFFFFD(2146435069) _freelist underflow threshold 2147483646");
                        entry.next = StartOfFreeList - _freeList;

                        if (RuntimeHelpers.IsReferenceOrContainsReferences<TKey>())
                        {
                            entry.key = default!;
                        }

                        if (RuntimeHelpers.IsReferenceOrContainsReferences<TValue>())
                        {
                            entry.value = default!;
                        }

                        _freeList = i;
                        _freeCount++;
                        return true;
                    }

                    last = i;
                    i = entry.next;

                    collisionCount++;
                    if (collisionCount > (uint)entries.Length)
                    {
                        // The chain of entries forms a loop; which means a concurrent update has happened.
                        // Break out of the loop and throw, rather than looping forever.
                        ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
                    }
                }
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">When this method returns, contains the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</param>
        /// <returns><see langword="true"/> if the <see cref="T:System.Collections.Generic.Dictionary`2"/> contains an element with the specified key; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// This method combines the functionality of the <see cref="System.Collections.Generic.Dictionary{T,U}.ContainsKey"/> method and the <see cref="System.Collections.Generic.Dictionary{T,U}.Item"/> property.
        /// If the key is not found, then the <c>value</c> parameter gets the appropriate default value for the type <c>TValue</c>; for example, 0 (zero) for integer types, <c>false</c> for Boolean types, and <c>null</c> for reference types.
        /// Use the <see cref="System.Collections.Generic.Dictionary{T,U}.TryGetValue"/> method if your code frequently attempts to access keys that are not in the dictionary. Using this method is more efficient than catching the <see cref="System.Collections.Generic.KeyNotFoundException"/> thrown by the <see cref="System.Collections.Generic.Dictionary{T,U}.Item"/> property.
        /// This method approaches an O(1) operation.
        /// The example shows how to use the <see cref="System.Collections.Generic.Dictionary{T,U}.TryGetValue"/> method as a more efficient way to retrieve values in a program that frequently tries keys that are not in the dictionary. For contrast, the example also shows how the <see cref="System.Collections.Generic.Dictionary{T,U}.Item"/> property (the indexer in C#) throws exceptions when attempting to retrieve nonexistent keys.
        /// This code example is part of a larger example provided for the <see cref="System.Collections.Generic.Dictionary{T,U}"/> class (<c>openWith</c> is the name of the Dictionary used in this example).
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.cs" id="Snippet5" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.fs" id="Snippet5" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.vb" id="Snippet5" />
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.cs" id="Snippet4" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.fs" id="Snippet4" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/DictionaryTKey,TValue/Overview/source.vb" id="Snippet4" />
        /// </remarks>
        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            ref TValue valRef = ref FindValue(key);
            if (!Unsafe.IsNullRef(ref valRef))
            {
                value = valRef;
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Attempts to add the specified key and value to the dictionary.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add. It can be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if the key/value pair was added to the dictionary successfully; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// Unlike the <see cref="System.Collections.Generic.Dictionary{T,U}.Add"/> method, this method doesn't throw an exception if the element with the given key exists in the dictionary. Unlike the Dictionary indexer, <c>TryAdd</c> doesn't override the element if the element with the given key exists in the dictionary. If the key already exists, <c>TryAdd</c> does nothing and returns <c>false</c>.
        /// </remarks>
        public bool TryAdd(TKey key, TValue value) =>
            TryInsert(key, value, InsertionBehavior.None);

        /// <summary>
        /// Gets a value that indicates whether the dictionary is read-only.
        /// </summary>
        /// <value><see langword="true"/> if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, <see langword="false"/>. In the default implementation of <see cref="T:System.Collections.Generic.Dictionary`2"/>, this property always returns <see langword="false"/>.</value>
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int index) =>
            CopyTo(array, index);

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an array, starting at the specified array index.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The array must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than 0.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="array"/> is multidimensional. -or- <paramref name="array"/> does not have zero-based indexing. -or- The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="index"/> to the end of the destination <paramref name="array"/>. -or- The type of the source <see cref="T:System.Collections.Generic.ICollection`1"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.</exception>
        /// <remarks>
        /// <note type="note">
        /// If the type of the source <see cref="System.Collections.ICollection"/> cannot be cast automatically to the type of the destination <c>array</c>, the nongeneric implementations of <see cref="System.Collections.ICollection.CopyTo">CopyTo</see> throw an <see cref="System.InvalidCastException"/>, whereas the generic implementations throw an <see cref="System.ArgumentException"/>.
        /// </note>
        /// Each element copied from a <see cref="System.Collections.Generic.Dictionary{T,U}"/> is a <see cref="System.Collections.Generic.KeyValuePair{T,U}"/> structure representing a value and its key.
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="System.Collections.Generic.Dictionary{T,U}.Count"/>.
        /// </remarks>
        void ICollection.CopyTo(Array array, int index)
        {
            if (array == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
            }

            if (array.Rank != 1)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RankMultiDimNotSupported);
            }

            if (array.GetLowerBound(0) != 0)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_NonZeroLowerBound);
            }

            if ((uint)index > (uint)array.Length)
            {
                ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException();
            }

            if (array.Length - index < Count)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);
            }

            if (array is KeyValuePair<TKey, TValue>[] pairs)
            {
                CopyTo(pairs, index);
            }
            else if (array is DictionaryEntry[] dictEntryArray)
            {
                Entry[]? entries = _entries;
                for (int i = 0; i < _count; i++)
                {
                    if (entries![i].next >= -1)
                    {
                        dictEntryArray[index++] = new DictionaryEntry(entries[i].key, entries[i].value);
                    }
                }
            }
            else
            {
                object[]? objects = array as object[];
                if (objects == null)
                {
                    ThrowHelper.ThrowArgumentException_Argument_IncompatibleArrayType();
                }

                try
                {
                    int count = _count;
                    Entry[]? entries = _entries;
                    for (int i = 0; i < count; i++)
                    {
                        if (entries![i].next >= -1)
                        {
                            objects[index++] = new KeyValuePair<TKey, TValue>(entries[i].key, entries[i].value);
                        }
                    }
                }
                catch (ArrayTypeMismatchException)
                {
                    ThrowHelper.ThrowArgumentException_Argument_IncompatibleArrayType();
                }
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator"/> that can be used to iterate through the collection.</returns>
        /// <remarks>
        /// The <c>foreach</c> statement of the C# language (<c>For Each</c> in Visual Basic) hides the complexity of enumerators. Therefore, using <c>foreach</c> is recommended, instead of directly manipulating the enumerator.
        /// Enumerators can be used to read the data in the collection, but they cannot be used to modify the underlying collection.
        /// Initially, the enumerator is positioned before the first element in the collection. The <see cref="System.Collections.IEnumerator.Reset"/> method also brings the enumerator back to this position.  At this position, the <see cref="System.Collections.IEnumerator.Current"/> property is undefined. Therefore, you must call the <see cref="System.Collections.IEnumerator.MoveNext"/> method to advance the enumerator to the first element of the collection before reading the value of <see cref="System.Collections.IEnumerator.Current"/>.
        /// The <see cref="System.Collections.IEnumerator.Current"/> property returns the same element until either the <see cref="System.Collections.IEnumerator.MoveNext"/> or <see cref="System.Collections.IEnumerator.Reset"/> method is called. <see cref="System.Collections.IEnumerator.MoveNext"/> sets <see cref="System.Collections.IEnumerator.Current"/> to the next element.
        /// If <see cref="System.Collections.IEnumerator.MoveNext"/> passes the end of the collection, the enumerator is positioned after the last element in the collection and <see cref="System.Collections.IEnumerator.MoveNext"/> returns <c>false</c>. When the enumerator is at this position, subsequent calls to <see cref="System.Collections.IEnumerator.MoveNext"/> also return <c>false</c>. If the last call to <see cref="System.Collections.IEnumerator.MoveNext"/> returned <c>false</c>, <see cref="System.Collections.IEnumerator.Current"/> is undefined. To set <see cref="System.Collections.IEnumerator.Current"/> to the first element of the collection again, you can call <see cref="System.Collections.IEnumerator.Reset"/> followed by <see cref="System.Collections.IEnumerator.MoveNext"/>.
        /// An enumerator remains valid as long as the collection remains unchanged. If changes are made to the collection, such as adding elements or changing the capacity, the enumerator is irrecoverably invalidated and the next call to <see cref="System.Collections.Generic.Dictionary{T,U}.Enumerator.MoveNext"/> or <see cref="System.Collections.Generic.Dictionary{T,U}.Enumerator.System#Collections#IEnumerator#Reset"/> throws an <see cref="System.InvalidOperationException"/>.
        /// .NET Core 3.0+ only: The only mutating methods which do not invalidate enumerators are <see cref="System.Collections.Generic.Dictionary{T,U}.Remove"/> and <see cref="System.Collections.Generic.Dictionary{T,U}.Clear"/>.
        /// The enumerator does not have exclusive access to the collection; therefore, enumerating through a collection is intrinsically not a thread safe procedure.  To guarantee thread safety during enumeration, you can lock the collection during the entire enumeration.  To allow the collection to be accessed by multiple threads for reading and writing, you must implement your own synchronization.
        /// Default implementations of collections in the <see cref="System.Collections.Generic">Generic</see> namespace are not synchronized.
        /// This method is an O(1) operation.
        /// </remarks>
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<KeyValuePair<TKey, TValue>>)this).GetEnumerator();

        /// <summary>
        /// Ensures that the dictionary can hold up to 'capacity' entries without any further expansion of its backing storage
        /// </summary>
        public int EnsureCapacity(int capacity)
        {
            if (capacity < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.capacity);
            }

            int currentCapacity = _entries == null ? 0 : _entries.Length;
            if (currentCapacity >= capacity)
            {
                return currentCapacity;
            }

            _version++;

            if (_buckets == null)
            {
                return Initialize(capacity);
            }

            int newSize = HashHelpers.GetPrime(capacity);
            Resize(newSize, forceNewHashCodes: false);
            return newSize;
        }

        /// <summary>
        /// Sets the capacity of this dictionary to what it would be if it had been originally initialized with all its entries
        /// </summary>
        /// <remarks>
        /// This method can be used to minimize the memory overhead
        /// once it is known that no new elements will be added.
        ///
        /// To allocate minimum size storage array, execute the following statements:
        ///
        /// dictionary.Clear();
        /// dictionary.TrimExcess();
        /// </remarks>
        public void TrimExcess() => TrimExcess(Count);

        /// <summary>
        /// Sets the capacity of this dictionary to hold up 'capacity' entries without any further expansion of its backing storage
        /// </summary>
        /// <remarks>
        /// This method can be used to minimize the memory overhead
        /// once it is known that no new elements will be added.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Passed capacity is lower than entries count.</exception>
        public void TrimExcess(int capacity)
        {
            if (capacity < Count)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.capacity);
            }

            int newSize = HashHelpers.GetPrime(capacity);
            Entry[]? oldEntries = _entries;
            int currentCapacity = oldEntries == null ? 0 : oldEntries.Length;
            if (newSize >= currentCapacity)
            {
                return;
            }

            int oldCount = _count;
            _version++;
            Initialize(newSize);

            Debug.Assert(oldEntries is not null);

            CopyEntries(oldEntries, oldCount);
        }

        private void CopyEntries(Entry[] entries, int count)
        {
            Debug.Assert(_entries is not null);

            Entry[] newEntries = _entries;
            int newCount = 0;
            for (int i = 0; i < count; i++)
            {
                uint hashCode = entries[i].hashCode;
                if (entries[i].next >= -1)
                {
                    ref Entry entry = ref newEntries[newCount];
                    entry = entries[i];
                    ref int bucket = ref GetBucket(hashCode);
                    entry.next = bucket - 1; // Value in _buckets is 1-based
                    bucket = newCount + 1;
                    newCount++;
                }
            }

            _count = newCount;
            _freeCount = 0;
        }

        /// <summary>
        /// Gets a value that indicates whether access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe).
        /// </summary>
        /// <value><see langword="true"/> if access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe); otherwise, <see langword="false"/>. In the default implementation of <see cref="T:System.Collections.Generic.Dictionary`2"/>, this property always returns <see langword="false"/>.</value>
        /// <remarks>
        /// Default implementations of collections in the <see cref="System.Collections.Generic">Generic</see> namespace are not synchronized.
        /// Enumerating through a collection is intrinsically not a thread-safe procedure. Even when a collection is synchronized, other threads can still modify the collection, which can cause the enumerator to throw an exception. To guarantee thread safety during enumeration, you can either lock the collection during the entire enumeration or catch the exceptions resulting from changes made by other threads.
        /// The <see cref="System.Collections.ICollection.SyncRoot"/> property returns an object that can be used to synchronize access to the <see cref="System.Collections.ICollection"/>. Synchronization is effective only if all threads lock the object before accessing the collection.
        /// Getting the value of this property is an O(1) operation.
        /// </remarks>
        bool ICollection.IsSynchronized => false;

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
        /// </summary>
        /// <value>An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.</value>
        /// <remarks>
        /// <code language="csharp">
        /// </code>
        /// <code language="vb">
        /// </code>
        /// Default implementations of collections in the <see cref="System.Collections.Generic">Generic</see> namespace are not synchronized.
        /// Enumerating through a collection is intrinsically not a thread-safe procedure.  To guarantee thread safety during enumeration, you can lock the collection during the entire enumeration.  To allow the collection to be accessed by multiple threads for reading and writing, you must implement your own synchronization.
        /// The <see cref="System.Collections.ICollection.SyncRoot"/> property returns an object that can be used to synchronize access to the <see cref="System.Collections.ICollection"/>. Synchronization is effective only if all threads lock the object before accessing the collection. The following code shows the use of the <see cref="System.Collections.ICollection.SyncRoot"/> property.
        /// ODE0 
        /// ODE1 
        /// Getting the value of this property is an O(1) operation.
        /// </remarks>
        object ICollection.SyncRoot => this;

        /// <summary>
        /// Gets a value that indicates whether the <see cref="T:System.Collections.IDictionary"/> has a fixed size.
        /// </summary>
        /// <value><see langword="true"/> if the <see cref="T:System.Collections.IDictionary"/> has a fixed size; otherwise, <see langword="false"/>. In the default implementation of <see cref="T:System.Collections.Generic.Dictionary`2"/>, this property always returns <see langword="false"/>.</value>
        /// <remarks>
        /// A collection with a fixed size does not allow the addition or removal of elements after the collection is created, but it allows the modification of existing elements.
        /// A collection with a fixed size is simply a collection with a wrapper that prevents adding and removing elements; therefore, if changes are made to the underlying collection, including the addition or removal of elements, the fixed-size collection reflects those changes.
        /// Getting the value of this property is an O(1) operation.
        /// </remarks>
        bool IDictionary.IsFixedSize => false;

        /// <summary>
        /// Gets a value that indicates whether the <see cref="T:System.Collections.IDictionary"/> is read-only.
        /// </summary>
        /// <value><see langword="true"/> if the <see cref="T:System.Collections.IDictionary"/> is read-only; otherwise, <see langword="false"/>. In the default implementation of <see cref="T:System.Collections.Generic.Dictionary`2"/>, this property always returns <see langword="false"/>.</value>
        /// <remarks>
        /// A collection that is read-only does not allow the addition, removal, or modification of elements after the collection is created.
        /// A collection that is read-only is simply a collection with a wrapper that prevents modifying the collection; therefore, if changes are made to the underlying collection, the read-only collection reflects those changes.
        /// Getting the value of this property is an O(1) operation.
        /// </remarks>
        bool IDictionary.IsReadOnly => false;

        /// <summary>
        /// Gets an <see cref="T:System.Collections.ICollection"/> containing the keys of the <see cref="T:System.Collections.IDictionary"/>.
        /// </summary>
        /// <value>An <see cref="T:System.Collections.ICollection"/> containing the keys of the <see cref="T:System.Collections.IDictionary"/>.</value>
        /// <remarks>
        /// The order of the keys in the returned <see cref="System.Collections.ICollection"/> is unspecified, but it is guaranteed to be the same order as the corresponding values in the <see cref="System.Collections.ICollection"/> returned by the <see cref="System.Collections.IDictionary.Values"/> property.
        /// Getting the value of this property is an O(1) operation.
        /// The following code example shows how to use the <see cref="System.Collections.Generic.Dictionary{T,U}.System#Collections#IDictionary#Keys"/> property of the <see cref="System.Collections.IDictionary">IDictionary</see> interface with a <see cref="System.Collections.Generic.Dictionary{T,U}"/>, to list the keys in the dictionary. The example also shows how to enumerate the key/value pairs in the dictionary; note that the enumerator for the <see cref="System.Collections.IDictionary">IDictionary</see> interface returns <see cref="System.Collections.DictionaryEntry"/> objects rather than <see cref="System.Collections.Generic.KeyValuePair{T,U}"/> objects.
        /// :::code language="csharp" source="~/snippets/csharp/System.Collections.Generic/DictionaryTKey,TValue/System.Collections.IDictionary.Keys/source.cs":::
        /// :::code language="fsharp" source="~/snippets/fsharp/System.Collections.Generic/DictionaryTKey,TValue/System.Collections.IDictionary.Keys/source.fs":::
        /// :::code language="vb" source="~/snippets/visualbasic/System.Collections.Generic/DictionaryTKey,TValue/System.Collections.IDictionary.Keys/source.vb":::
        /// </remarks>
        ICollection IDictionary.Keys => Keys;

        /// <summary>
        /// Gets an <see cref="T:System.Collections.ICollection"/> containing the values in the <see cref="T:System.Collections.IDictionary"/>.
        /// </summary>
        /// <value>An <see cref="T:System.Collections.ICollection"/> containing the values in the <see cref="T:System.Collections.IDictionary"/>.</value>
        /// <remarks>
        /// The order of the values in the returned <see cref="System.Collections.ICollection"/> is unspecified, but it is guaranteed to be the same order as the corresponding keys in the <see cref="System.Collections.ICollection"/> returned by the <see cref="System.Collections.IDictionary.Keys"/> property.
        /// Getting the value of this property is an O(1) operation.
        /// The following code example shows how to use the <see cref="System.Collections.Generic.Dictionary{T,U}.System#Collections#IDictionary#Values"/> property of the <see cref="System.Collections.IDictionary">IDictionary</see> interface with a <see cref="System.Collections.Generic.Dictionary{T,U}"/>, to list the values in the dictionary. The example also shows how to enumerate the key/value pairs in the dictionary; note that the enumerator for the <see cref="System.Collections.IDictionary">IDictionary</see> interface returns <see cref="System.Collections.DictionaryEntry"/> objects rather than <see cref="System.Collections.Generic.KeyValuePair{T,U}"/> objects.
        /// :::code language="csharp" source="~/snippets/csharp/System.Collections.Generic/DictionaryTKey,TValue/System.Collections.IDictionary.Values/source.cs":::
        /// :::code language="fsharp" source="~/snippets/fsharp/System.Collections.Generic/DictionaryTKey,TValue/System.Collections.IDictionary.Values/source.fs":::
        /// :::code language="vb" source="~/snippets/visualbasic/System.Collections.Generic/DictionaryTKey,TValue/System.Collections.IDictionary.Values/source.vb":::
        /// </remarks>
        ICollection IDictionary.Values => Values;

        /// <summary>
        /// Gets or sets the value with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <value>The value associated with the specified key, or <see langword="null"/> if <paramref name="key"/> is not in the dictionary or <paramref name="key"/> is of a type that is not assignable to the key type <paramref name="TKey"/> of the <see cref="T:System.Collections.Generic.Dictionary`2"/>.</value>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentException">A value is being assigned, and <paramref name="key"/> is of a type that is not assignable to the key type <paramref name="TKey"/> of the <see cref="T:System.Collections.Generic.Dictionary`2"/>. -or- A value is being assigned and is of a type that isn't assignable to the value type <paramref name="TValue"/> of the <see cref="T:System.Collections.Generic.Dictionary`2"/>.</exception>
        /// <remarks>
        /// This property provides the ability to access a specific value in the collection by using the following C# syntax: <c>myCollection[key]</c> (<c>myCollection(key)</c> in Visual Basic).
        /// You can also use the <see cref="System.Collections.IDictionary.Item"/> property to add new elements by setting the value of a key that does not exist in the dictionary; for example, <c>myCollection[&quot;myNonexistentKey&quot;] = myValue</c>. However, if the specified key already exists in the dictionary, setting the <see cref="System.Collections.IDictionary.Item"/> property overwrites the old value. In contrast, the <see cref="System.Collections.IDictionary.Add"/> method does not modify existing elements.
        /// The C# language uses the [this](/dotnet/csharp/language-reference/keywords/this) keyword to define the indexers instead of implementing the <see cref="System.Collections.Generic.Dictionary{T,U}.System#Collections#IDictionary#Item"/> property. Visual Basic implements <see cref="System.Collections.Generic.Dictionary{T,U}.System#Collections#IDictionary#Item"/> as a default property, which provides the same indexing functionality.
        /// Getting or setting the value of this property approaches an O(1) operation.
        /// The following code example shows how to use the <see cref="System.Collections.Generic.Dictionary{T,U}.System#Collections#IDictionary#Item"/> property (the indexer in C#) of the <see cref="System.Collections.IDictionary">IDictionary</see> interface with a <see cref="System.Collections.Generic.Dictionary{T,U}"/>, and ways the property differs from the <see cref="System.Collections.Generic.Dictionary{T,U}.Item">Item</see> property.
        /// The example shows that, like the <see cref="System.Collections.Generic.Dictionary{T,U}.Item">Item</see> property, the <see cref="System.Collections.Generic.Dictionary{T,U}.System#Collections#IDictionary#Item">System%23Collections%23IDictionary%23Item</see> property can change the value associated with an existing key and can be used to add a new key/value pair if the specified key is not in the dictionary. The example also shows that unlike the <see cref="System.Collections.Generic.Dictionary{T,U}.Item">Item</see> property, the <see cref="System.Collections.Generic.Dictionary{T,U}.System#Collections#IDictionary#Item">System%23Collections%23IDictionary%23Item</see> property does not throw an exception if <c>key</c> is not in the dictionary, returning a null reference instead. Finally, the example demonstrates that getting the <see cref="System.Collections.Generic.Dictionary{T,U}.System#Collections#IDictionary#Item">System%23Collections%23IDictionary%23Item</see> property returns a null reference if <c>key</c> is not the correct data type, and that setting the property throws an exception if <c>key</c> is not the correct data type.
        /// :::code language="csharp" source="~/snippets/csharp/System.Collections.Generic/DictionaryTKey,TValue/System.Collections.IDictionary.Item/source.cs":::
        /// :::code language="fsharp" source="~/snippets/fsharp/System.Collections.Generic/DictionaryTKey,TValue/System.Collections.IDictionary.Item/source.fs":::
        /// :::code language="vb" source="~/snippets/visualbasic/System.Collections.Generic/DictionaryTKey,TValue/System.Collections.IDictionary.Item/source.vb":::
        /// </remarks>
        object? IDictionary.this[object key]
        {
            get
            {
                if (IsCompatibleKey(key))
                {
                    ref TValue value = ref FindValue((TKey)key);
                    if (!Unsafe.IsNullRef(ref value))
                    {
                        return value;
                    }
                }

                return null;
            }
            set
            {
                if (key == null)
                {
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);
                }
                ThrowHelper.IfNullAndNullsAreIllegalThenThrow<TValue>(value, ExceptionArgument.value);

                try
                {
                    TKey tempKey = (TKey)key;
                    try
                    {
                        this[tempKey] = (TValue)value!;
                    }
                    catch (InvalidCastException)
                    {
                        ThrowHelper.ThrowWrongValueTypeArgumentException(value, typeof(TValue));
                    }
                }
                catch (InvalidCastException)
                {
                    ThrowHelper.ThrowWrongKeyTypeArgumentException(key, typeof(TKey));
                }
            }
        }

        private static bool IsCompatibleKey(object key)
        {
            if (key == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);
            }
            return key is TKey;
        }

        /// <summary>
        /// Adds the specified key and value to the dictionary.
        /// </summary>
        /// <param name="key">The object to use as the key.</param>
        /// <param name="value">The object to use as the value.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="key"/> is of a type that is not assignable to the key type <paramref name="TKey"/> of the <see cref="T:System.Collections.Generic.Dictionary`2"/>. -or- <paramref name="value"/> is of a type that is not assignable to <paramref name="TValue"/>, the type of values in the <see cref="T:System.Collections.Generic.Dictionary`2"/>. -or- A value with the same key already exists in the <see cref="T:System.Collections.Generic.Dictionary`2"/>.</exception>
        /// <remarks>
        /// You can also use the <see cref="System.Collections.IDictionary.Item"/> property to add new elements by setting the value of a key that does not exist in the dictionary; for example, <c>myCollection[&quot;myNonexistentKey&quot;] = myValue</c>. However, if the specified key already exists in the dictionary, setting the <see cref="System.Collections.IDictionary.Item"/> property overwrites the old value. In contrast, the <see cref="System.Collections.IDictionary.Add"/> method throws an exception if the specified key already exists.
        /// If <see cref="System.Collections.Generic.Dictionary{T,U}.Count"/> is less than the capacity, this method approaches an O(1) operation. If the capacity needs to be increased to accommodate the new element, this method becomes an O(<c>n</c>) operation, where <c>n</c> is <see cref="System.Collections.Generic.Dictionary{T,U}.Count"/>.
        /// The following code example shows how to access the <see cref="System.Collections.Generic.Dictionary{T,U}"/> class through the <see cref="System.Collections.IDictionary">IDictionary</see> interface. The code example creates an empty <see cref="System.Collections.Generic.Dictionary{T,U}"/> of strings with string keys and uses the <see cref="System.Collections.Generic.Dictionary{T,U}.System#Collections#IDictionary#Add"/> method to add some elements. The example demonstrates that the <see cref="System.Collections.Generic.Dictionary{T,U}.System#Collections#IDictionary#Add"/> method throws an <see cref="System.ArgumentException"/> when attempting to add a duplicate key, or when a key or value of the wrong data type is supplied.
        /// The code example demonstrates the use of several other members of the <see cref="System.Collections.IDictionary">IDictionary</see> interface.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/DictionaryTKey,TValue/System.Collections.IDictionary.Add/source.cs" id="Snippet1" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/DictionaryTKey,TValue/System.Collections.IDictionary.Add/source.fs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/DictionaryTKey,TValue/System.Collections.IDictionary.Add/source.vb" id="Snippet1" />
        /// </remarks>
        void IDictionary.Add(object key, object? value)
        {
            if (key == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);
            }
            ThrowHelper.IfNullAndNullsAreIllegalThenThrow<TValue>(value, ExceptionArgument.value);

            try
            {
                TKey tempKey = (TKey)key;

                try
                {
                    Add(tempKey, (TValue)value!);
                }
                catch (InvalidCastException)
                {
                    ThrowHelper.ThrowWrongValueTypeArgumentException(value, typeof(TValue));
                }
            }
            catch (InvalidCastException)
            {
                ThrowHelper.ThrowWrongKeyTypeArgumentException(key, typeof(TKey));
            }
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.IDictionary"/> contains an element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="T:System.Collections.IDictionary"/>.</param>
        /// <returns><see langword="true"/> if the <see cref="T:System.Collections.IDictionary"/> contains an element with the specified key; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// This method returns <c>false</c> if <c>key</c> is of a type that is not assignable to the key type <c>TKey</c> of the <see cref="System.Collections.Generic.Dictionary{T,U}"/>.
        /// This method approaches an O(1) operation.
        /// The following code example shows how to use the <see cref="System.Collections.Generic.Dictionary{T,U}.System#Collections#IDictionary#Contains"/> method of the <see cref="System.Collections.IDictionary">IDictionary</see> interface with a <see cref="System.Collections.Generic.Dictionary{T,U}"/>. The example demonstrates that the method returns <c>false</c> if a key of the wrong data type is supplied.
        /// :::code language="csharp" source="~/snippets/csharp/System.Collections.Generic/DictionaryTKey,TValue/System.Collections.IDictionary.Contains/source.cs":::
        /// :::code language="fsharp" source="~/snippets/fsharp/System.Collections.Generic/DictionaryTKey,TValue/System.Collections.IDictionary.Contains/source.fs":::
        /// :::code language="vb" source="~/snippets/visualbasic/System.Collections.Generic/DictionaryTKey,TValue/System.Collections.IDictionary.Contains/source.vb":::
        /// </remarks>
        bool IDictionary.Contains(object key)
        {
            if (IsCompatibleKey(key))
            {
                return ContainsKey((TKey)key);
            }

            return false;
        }

        /// <summary>
        /// Returns an <see cref="T:System.Collections.IDictionaryEnumerator"/> for the <see cref="T:System.Collections.IDictionary"/>.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IDictionaryEnumerator"/> for the <see cref="T:System.Collections.IDictionary"/>.</returns>
        /// <remarks>
        /// For purposes of enumeration, each item is a <see cref="System.Collections.DictionaryEntry"/> structure.
        /// The <c>foreach</c> statement of the C# language (<c>For Each</c> in Visual Basic) hides the complexity of enumerators. Therefore, using <c>foreach</c> is recommended, instead of directly manipulating the enumerator.
        /// Enumerators can be used to read the data in the collection, but they cannot be used to modify the underlying collection.
        /// Initially, the enumerator is positioned before the first element in the collection. The <see cref="System.Collections.IEnumerator.Reset"/> method also brings the enumerator back to this position.  At this position, <see cref="System.Collections.IDictionaryEnumerator.Entry"/> is undefined. Therefore, you must call the <see cref="System.Collections.IEnumerator.MoveNext"/> method to advance the enumerator to the first element of the collection before reading the value of <see cref="System.Collections.IDictionaryEnumerator.Entry"/>.
        /// The <see cref="System.Collections.IDictionaryEnumerator.Entry"/> property returns the same element until either the <see cref="System.Collections.IEnumerator.MoveNext"/> or <see cref="System.Collections.IEnumerator.Reset"/> method is called. <see cref="System.Collections.IEnumerator.MoveNext"/> sets <see cref="System.Collections.IDictionaryEnumerator.Entry"/> to the next element.
        /// If <see cref="System.Collections.IEnumerator.MoveNext"/> passes the end of the collection, the enumerator is positioned after the last element in the collection and <see cref="System.Collections.IEnumerator.MoveNext"/> returns <c>false</c>. When the enumerator is at this position, subsequent calls to <see cref="System.Collections.IEnumerator.MoveNext"/> also return <c>false</c>. If the last call to <see cref="System.Collections.IEnumerator.MoveNext"/> returned <c>false</c>, <see cref="System.Collections.IDictionaryEnumerator.Entry"/> is undefined. To set <see cref="System.Collections.IDictionaryEnumerator.Entry"/> to the first element of the collection again, you can call <see cref="System.Collections.IEnumerator.Reset"/> followed by <see cref="System.Collections.IEnumerator.MoveNext"/>.
        /// An enumerator remains valid as long as the collection remains unchanged. If changes are made to the collection, such as adding elements or changing the capacity, the enumerator is irrecoverably invalidated and the next call to <see cref="System.Collections.Generic.Dictionary{T,U}.Enumerator.MoveNext"/> or <see cref="System.Collections.Generic.Dictionary{T,U}.Enumerator.System#Collections#IEnumerator#Reset"/> throws an <see cref="System.InvalidOperationException"/>.
        /// .NET Core 3.0+ only: The only mutating methods which do not invalidate enumerators are <see cref="System.Collections.Generic.Dictionary{T,U}.Remove"/> and <see cref="System.Collections.Generic.Dictionary{T,U}.Clear"/>.
        /// The enumerator does not have exclusive access to the collection; therefore, enumerating through a collection is intrinsically not a thread-safe procedure.  To guarantee thread safety during enumeration, you can lock the collection during the entire enumeration.  To allow the collection to be accessed by multiple threads for reading and writing, you must implement your own synchronization.
        /// Default implementations of collections in the <see cref="System.Collections.Generic">Generic</see> namespace are not synchronized.
        /// This method is an O(1) operation.
        /// The following code example shows how to enumerate the key/value pairs in the dictionary by using the <c>foreach</c> statement (<c>For Each</c> in Visual Basic), which hides the use of the enumerator. In particular, note that the enumerator for the <see cref="System.Collections.IDictionary">IDictionary</see> interface returns <see cref="System.Collections.DictionaryEntry"/> objects rather than <see cref="System.Collections.Generic.KeyValuePair{T,U}"/> objects.
        /// :::code language="csharp" source="~/snippets/csharp/System.Collections.Generic/DictionaryTKey,TValue/System.Collections.IDictionary.GetEnumerator/source.cs":::
        /// :::code language="fsharp" source="~/snippets/fsharp/System.Collections.Generic/DictionaryTKey,TValue/System.Collections.IDictionary.GetEnumerator/source.fs":::
        /// :::code language="vb" source="~/snippets/visualbasic/System.Collections.Generic/DictionaryTKey,TValue/System.Collections.IDictionary.GetEnumerator/source.vb":::
        /// </remarks>
        IDictionaryEnumerator IDictionary.GetEnumerator() => new Enumerator(this, Enumerator.DictEntry);

        /// <summary>
        /// Removes the element with the specified key from the <see cref="T:System.Collections.IDictionary"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// This method approaches an O(1) operation.
        /// The following code example shows how to use the <see cref="System.Collections.Generic.Dictionary{T,U}.System#Collections#IDictionary#Remove"/> of the <see cref="System.Collections.IDictionary">IDictionary</see> interface with a <see cref="System.Collections.Generic.Dictionary{T,U}"/>.
        /// :::code language="csharp" source="~/snippets/csharp/System.Collections.Generic/DictionaryTKey,TValue/System.Collections.IDictionary.Remove/source.cs":::
        /// :::code language="fsharp" source="~/snippets/fsharp/System.Collections.Generic/DictionaryTKey,TValue/System.Collections.IDictionary.Remove/source.fs":::
        /// :::code language="vb" source="~/snippets/visualbasic/System.Collections.Generic/DictionaryTKey,TValue/System.Collections.IDictionary.Remove/source.vb":::
        /// </remarks>
        void IDictionary.Remove(object key)
        {
            if (IsCompatibleKey(key))
            {
                Remove((TKey)key);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ref int GetBucket(uint hashCode)
        {
            int[] buckets = _buckets!;
#if TARGET_64BIT
            return ref buckets[HashHelpers.FastMod(hashCode, (uint)buckets.Length, _fastModMultiplier)];
#else
            return ref buckets[(uint)hashCode % buckets.Length];
#endif
        }

        private struct Entry
        {
            public uint hashCode;
            /// <summary>
            /// 0-based index of next entry in chain: -1 means end of chain
            /// also encodes whether this entry _itself_ is part of the free list by changing sign and subtracting 3,
            /// so -2 means end of free list, -3 means index 0 but on free list, -4 means index 1 but on free list, etc.
            /// </summary>
            public int next;
            public TKey key;     // Key of entry
            public TValue value; // Value of entry
        }

        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDictionaryEnumerator
        {
            private readonly Dictionary<TKey, TValue> _dictionary;
            private readonly int _version;
            private int _index;
            private KeyValuePair<TKey, TValue> _current;
            private readonly int _getEnumeratorRetType;  // What should Enumerator.Current return?

            internal const int DictEntry = 1;
            internal const int KeyValuePair = 2;

            internal Enumerator(Dictionary<TKey, TValue> dictionary, int getEnumeratorRetType)
            {
                _dictionary = dictionary;
                _version = dictionary._version;
                _index = 0;
                _getEnumeratorRetType = getEnumeratorRetType;
                _current = default;
            }

            public bool MoveNext()
            {
                if (_version != _dictionary._version)
                {
                    ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion();
                }

                // Use unsigned comparison since we set index to dictionary.count+1 when the enumeration ends.
                // dictionary.count+1 could be negative if dictionary.count is int.MaxValue
                while ((uint)_index < (uint)_dictionary._count)
                {
                    ref Entry entry = ref _dictionary._entries![_index++];

                    if (entry.next >= -1)
                    {
                        _current = new KeyValuePair<TKey, TValue>(entry.key, entry.value);
                        return true;
                    }
                }

                _index = _dictionary._count + 1;
                _current = default;
                return false;
            }

            public KeyValuePair<TKey, TValue> Current => _current;

            public void Dispose() { }

            object? IEnumerator.Current
            {
                get
                {
                    if (_index == 0 || (_index == _dictionary._count + 1))
                    {
                        ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumOpCantHappen();
                    }

                    if (_getEnumeratorRetType == DictEntry)
                    {
                        return new DictionaryEntry(_current.Key, _current.Value);
                    }

                    return new KeyValuePair<TKey, TValue>(_current.Key, _current.Value);
                }
            }

            void IEnumerator.Reset()
            {
                if (_version != _dictionary._version)
                {
                    ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion();
                }

                _index = 0;
                _current = default;
            }

            DictionaryEntry IDictionaryEnumerator.Entry
            {
                get
                {
                    if (_index == 0 || (_index == _dictionary._count + 1))
                    {
                        ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumOpCantHappen();
                    }

                    return new DictionaryEntry(_current.Key, _current.Value);
                }
            }

            object IDictionaryEnumerator.Key
            {
                get
                {
                    if (_index == 0 || (_index == _dictionary._count + 1))
                    {
                        ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumOpCantHappen();
                    }

                    return _current.Key;
                }
            }

            object? IDictionaryEnumerator.Value
            {
                get
                {
                    if (_index == 0 || (_index == _dictionary._count + 1))
                    {
                        ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumOpCantHappen();
                    }

                    return _current.Value;
                }
            }
        }

        [DebuggerTypeProxy(typeof(DictionaryKeyCollectionDebugView<,>))]
        [DebuggerDisplay("Count = {Count}")]
        public sealed class KeyCollection : ICollection<TKey>, ICollection, IReadOnlyCollection<TKey>
        {
            private readonly Dictionary<TKey, TValue> _dictionary;

            public KeyCollection(Dictionary<TKey, TValue> dictionary)
            {
                if (dictionary == null)
                {
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.dictionary);
                }

                _dictionary = dictionary;
            }

            public Enumerator GetEnumerator() => new Enumerator(_dictionary);

            public void CopyTo(TKey[] array, int index)
            {
                if (array == null)
                {
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
                }

                if (index < 0 || index > array.Length)
                {
                    ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException();
                }

                if (array.Length - index < _dictionary.Count)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);
                }

                int count = _dictionary._count;
                Entry[]? entries = _dictionary._entries;
                for (int i = 0; i < count; i++)
                {
                    if (entries![i].next >= -1) array[index++] = entries[i].key;
                }
            }

            public int Count => _dictionary.Count;

            bool ICollection<TKey>.IsReadOnly => true;

            void ICollection<TKey>.Add(TKey item) =>
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_KeyCollectionSet);

            void ICollection<TKey>.Clear() =>
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_KeyCollectionSet);

            public bool Contains(TKey item) =>
                _dictionary.ContainsKey(item);

            bool ICollection<TKey>.Remove(TKey item)
            {
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_KeyCollectionSet);
                return false;
            }

            IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator() =>
                Count == 0 ? SZGenericArrayEnumerator<TKey>.Empty :
                GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<TKey>)this).GetEnumerator();

            void ICollection.CopyTo(Array array, int index)
            {
                if (array == null)
                {
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
                }

                if (array.Rank != 1)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RankMultiDimNotSupported);
                }

                if (array.GetLowerBound(0) != 0)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_NonZeroLowerBound);
                }

                if ((uint)index > (uint)array.Length)
                {
                    ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException();
                }

                if (array.Length - index < _dictionary.Count)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);
                }

                if (array is TKey[] keys)
                {
                    CopyTo(keys, index);
                }
                else
                {
                    object[]? objects = array as object[];
                    if (objects == null)
                    {
                        ThrowHelper.ThrowArgumentException_Argument_IncompatibleArrayType();
                    }

                    int count = _dictionary._count;
                    Entry[]? entries = _dictionary._entries;
                    try
                    {
                        for (int i = 0; i < count; i++)
                        {
                            if (entries![i].next >= -1) objects[index++] = entries[i].key;
                        }
                    }
                    catch (ArrayTypeMismatchException)
                    {
                        ThrowHelper.ThrowArgumentException_Argument_IncompatibleArrayType();
                    }
                }
            }

            bool ICollection.IsSynchronized => false;

            object ICollection.SyncRoot => ((ICollection)_dictionary).SyncRoot;

            public struct Enumerator : IEnumerator<TKey>, IEnumerator
            {
                private readonly Dictionary<TKey, TValue> _dictionary;
                private int _index;
                private readonly int _version;
                private TKey? _currentKey;

                internal Enumerator(Dictionary<TKey, TValue> dictionary)
                {
                    _dictionary = dictionary;
                    _version = dictionary._version;
                    _index = 0;
                    _currentKey = default;
                }

                public void Dispose() { }

                public bool MoveNext()
                {
                    if (_version != _dictionary._version)
                    {
                        ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion();
                    }

                    while ((uint)_index < (uint)_dictionary._count)
                    {
                        ref Entry entry = ref _dictionary._entries![_index++];

                        if (entry.next >= -1)
                        {
                            _currentKey = entry.key;
                            return true;
                        }
                    }

                    _index = _dictionary._count + 1;
                    _currentKey = default;
                    return false;
                }

                public TKey Current => _currentKey!;

                object? IEnumerator.Current
                {
                    get
                    {
                        if (_index == 0 || (_index == _dictionary._count + 1))
                        {
                            ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumOpCantHappen();
                        }

                        return _currentKey;
                    }
                }

                void IEnumerator.Reset()
                {
                    if (_version != _dictionary._version)
                    {
                        ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion();
                    }

                    _index = 0;
                    _currentKey = default;
                }
            }
        }

        [DebuggerTypeProxy(typeof(DictionaryValueCollectionDebugView<,>))]
        [DebuggerDisplay("Count = {Count}")]
        public sealed class ValueCollection : ICollection<TValue>, ICollection, IReadOnlyCollection<TValue>
        {
            private readonly Dictionary<TKey, TValue> _dictionary;

            public ValueCollection(Dictionary<TKey, TValue> dictionary)
            {
                if (dictionary == null)
                {
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.dictionary);
                }

                _dictionary = dictionary;
            }

            public Enumerator GetEnumerator() => new Enumerator(_dictionary);

            public void CopyTo(TValue[] array, int index)
            {
                if (array == null)
                {
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
                }

                if ((uint)index > array.Length)
                {
                    ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException();
                }

                if (array.Length - index < _dictionary.Count)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);
                }

                int count = _dictionary._count;
                Entry[]? entries = _dictionary._entries;
                for (int i = 0; i < count; i++)
                {
                    if (entries![i].next >= -1) array[index++] = entries[i].value;
                }
            }

            public int Count => _dictionary.Count;

            bool ICollection<TValue>.IsReadOnly => true;

            void ICollection<TValue>.Add(TValue item) =>
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_ValueCollectionSet);

            bool ICollection<TValue>.Remove(TValue item)
            {
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_ValueCollectionSet);
                return false;
            }

            void ICollection<TValue>.Clear() =>
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_ValueCollectionSet);

            bool ICollection<TValue>.Contains(TValue item) => _dictionary.ContainsValue(item);

            IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() =>
                Count == 0 ? SZGenericArrayEnumerator<TValue>.Empty :
                GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<TValue>)this).GetEnumerator();

            void ICollection.CopyTo(Array array, int index)
            {
                if (array == null)
                {
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
                }

                if (array.Rank != 1)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RankMultiDimNotSupported);
                }

                if (array.GetLowerBound(0) != 0)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_NonZeroLowerBound);
                }

                if ((uint)index > (uint)array.Length)
                {
                    ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException();
                }

                if (array.Length - index < _dictionary.Count)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);
                }

                if (array is TValue[] values)
                {
                    CopyTo(values, index);
                }
                else
                {
                    object[]? objects = array as object[];
                    if (objects == null)
                    {
                        ThrowHelper.ThrowArgumentException_Argument_IncompatibleArrayType();
                    }

                    int count = _dictionary._count;
                    Entry[]? entries = _dictionary._entries;
                    try
                    {
                        for (int i = 0; i < count; i++)
                        {
                            if (entries![i].next >= -1) objects[index++] = entries[i].value!;
                        }
                    }
                    catch (ArrayTypeMismatchException)
                    {
                        ThrowHelper.ThrowArgumentException_Argument_IncompatibleArrayType();
                    }
                }
            }

            bool ICollection.IsSynchronized => false;

            object ICollection.SyncRoot => ((ICollection)_dictionary).SyncRoot;

            public struct Enumerator : IEnumerator<TValue>, IEnumerator
            {
                private readonly Dictionary<TKey, TValue> _dictionary;
                private int _index;
                private readonly int _version;
                private TValue? _currentValue;

                internal Enumerator(Dictionary<TKey, TValue> dictionary)
                {
                    _dictionary = dictionary;
                    _version = dictionary._version;
                    _index = 0;
                    _currentValue = default;
                }

                public void Dispose() { }

                public bool MoveNext()
                {
                    if (_version != _dictionary._version)
                    {
                        ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion();
                    }

                    while ((uint)_index < (uint)_dictionary._count)
                    {
                        ref Entry entry = ref _dictionary._entries![_index++];

                        if (entry.next >= -1)
                        {
                            _currentValue = entry.value;
                            return true;
                        }
                    }
                    _index = _dictionary._count + 1;
                    _currentValue = default;
                    return false;
                }

                public TValue Current => _currentValue!;

                object? IEnumerator.Current
                {
                    get
                    {
                        if (_index == 0 || (_index == _dictionary._count + 1))
                        {
                            ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumOpCantHappen();
                        }

                        return _currentValue;
                    }
                }

                void IEnumerator.Reset()
                {
                    if (_version != _dictionary._version)
                    {
                        ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion();
                    }

                    _index = 0;
                    _currentValue = default;
                }
            }
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
    // The SortedDictionary class implements a generic sorted list of keys
    // and values. Entries in a sorted list are sorted by their keys and
    // are accessible both by key and by index. The keys of a sorted dictionary
    // can be ordered either according to a specific IComparer
    // implementation given when the sorted dictionary is instantiated, or
    // according to the IComparable implementation provided by the keys
    // themselves. In either case, a sorted dictionary does not allow entries
    // with duplicate or null keys.
    //
    // A sorted list internally maintains two arrays that store the keys and
    // values of the entries. The capacity of a sorted list is the allocated
    // length of these internal arrays. As elements are added to a sorted list, the
    // capacity of the sorted list is automatically increased as required by
    // reallocating the internal arrays.  The capacity is never automatically
    // decreased, but users can call either TrimExcess or
    // Capacity explicitly.
    //
    // The GetKeyList and GetValueList methods of a sorted list
    // provides access to the keys and values of the sorted list in the form of
    // List implementations. The List objects returned by these
    // methods are aliases for the underlying sorted list, so modifications
    // made to those lists are directly reflected in the sorted list, and vice
    // versa.
    //
    // The SortedList class provides a convenient way to create a sorted
    // copy of another dictionary, such as a Hashtable. For example:
    //
    // Hashtable h = new Hashtable();
    // h.Add(...);
    // h.Add(...);
    // ...
    // SortedList s = new SortedList(h);
    //
    // The last line above creates a sorted list that contains a copy of the keys
    // and values stored in the hashtable. In this particular example, the keys
    // will be ordered according to the IComparable interface, which they
    // all must implement. To impose a different ordering, SortedList also
    // has a constructor that allows a specific IComparer implementation to
    // be specified.
    //
    /// <summary>
    /// Represents a collection of key/value pairs that are sorted by key based on the associated <see cref="T:System.Collections.Generic.IComparer`1"/> implementation.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the collection.</typeparam>
    /// <typeparam name="TValue">The type of values in the collection.</typeparam>
    /// <remarks>
    /// The <see cref="System.Collections.Generic.SortedList{T,U}"/> generic class is an array of key/value pairs with O(log <c>n</c>) retrieval, where n is the number of elements in the dictionary. In this, it is similar to the <see cref="System.Collections.Generic.SortedDictionary{T,U}"/> generic class. The two classes have similar object models, and both have O(log <c>n</c>) retrieval. Where the two classes differ is in memory use and speed of insertion and removal:
    /// -   <see cref="System.Collections.Generic.SortedList{T,U}"/> uses less memory than <see cref="System.Collections.Generic.SortedDictionary{T,U}"/>.
    /// -   <see cref="System.Collections.Generic.SortedDictionary{T,U}"/> has faster insertion and removal operations for unsorted data, O(log <c>n</c>) as opposed to O(<c>n</c>) for <see cref="System.Collections.Generic.SortedList{T,U}"/>.
    /// - If the list is populated all at once from sorted data, <see cref="System.Collections.Generic.SortedList{T,U}"/> is faster than <see cref="System.Collections.Generic.SortedDictionary{T,U}"/>.
    /// Another difference between the <see cref="System.Collections.Generic.SortedDictionary{T,U}"/> and <see cref="System.Collections.Generic.SortedList{T,U}"/> classes is that <see cref="System.Collections.Generic.SortedList{T,U}"/> supports efficient indexed retrieval of keys and values through the collections returned by the <see cref="System.Collections.Generic.SortedList{T,U}.Keys"/> and <see cref="System.Collections.Generic.SortedList{T,U}.Values"/> properties. It is not necessary to regenerate the lists when the properties are accessed, because the lists are just wrappers for the internal arrays of keys and values. The following code shows the use of the <see cref="System.Collections.Generic.SortedList{T,U}.Values"/> property for indexed retrieval of values from a sorted list of strings:
    /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/SortedListTKey,TValue/Overview/remarks.cs" id="Snippet11" />
    /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/SortedListTKey,TValue/Overview/remarks.vb" id="Snippet11" />
    /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/SortedListTKey,TValue/Overview/remarks.fs" id="Snippet11" />
    /// <see cref="System.Collections.Generic.SortedList{T,U}"/> is implemented as an array of key/value pairs, sorted by the key.  Each element can be retrieved as a <see cref="System.Collections.Generic.KeyValuePair{T,U}"/> object.
    /// Key objects must be immutable as long as they are used as keys in the <see cref="System.Collections.Generic.SortedList{T,U}"/>. Every key in a <see cref="System.Collections.Generic.SortedList{T,U}"/> must be unique. A key cannot be <c>null</c>, but a value can be, if the type of values in the list, <c>TValue</c>, is a reference type.
    /// <see cref="System.Collections.Generic.SortedList{T,U}"/> requires a comparer implementation to sort and to perform comparisons.  The default comparer <see cref="System.Collections.Generic.Comparer{T}.Default">Default</see> checks whether the key type <c>TKey</c> implements <see cref="System.IComparable{T}">IComparable{T}</see> and uses that implementation, if available.  If not, <see cref="System.Collections.Generic.Comparer{T}.Default">Default</see> checks whether the key type <c>TKey</c> implements <see cref="System.IComparable">IComparable</see>.  If the key type <c>TKey</c> does not implement either interface, you can specify a <see cref="System.Collections.Generic.IComparer{T}">IComparer{T}</see> implementation in a constructor overload that accepts a <c>comparer</c> parameter.
    /// The capacity of a <see cref="System.Collections.Generic.SortedList{T,U}"/> is the number of elements the <see cref="System.Collections.Generic.SortedList{T,U}"/> can hold. As elements are added to a <see cref="System.Collections.Generic.SortedList{T,U}"/>, the capacity is automatically increased as required by reallocating the internal array. The capacity can be decreased by calling <see cref="System.Collections.Generic.SortedList{T,U}.TrimExcess"/> or by setting the <see cref="System.Collections.Generic.SortedList{T,U}.Capacity"/> property explicitly. Decreasing the capacity reallocates memory and copies all the elements in the <see cref="System.Collections.Generic.SortedList{T,U}"/>.
    /// **.NET Framework only:** For very large <see cref="System.Collections.Generic.SortedList{T,U}"/> objects, you can increase the maximum capacity to 2 billion elements on a 64-bit system by setting the <c>enabled</c> attribute of the [<c>&lt;gcAllowVeryLargeObjects&gt;</c>](/dotnet/framework/configure-apps/file-schema/runtime/gcallowverylargeobjects-element) configuration element to <c>true</c> in the run-time environment.
    /// The <c>foreach</c> statement of the C# language (<c>For Each</c> in Visual Basic) returns an object of the type of the elements in the collection. Since the elements of the <see cref="System.Collections.Generic.SortedList{T,U}"/> are key/value pairs, the element type is not the type of the key or the type of the value. Instead, the element type is <see cref="System.Collections.Generic.KeyValuePair{T,U}"/>. For example:
    /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/SortedListTKey,TValue/Overview/remarks.cs" id="Snippet12" />
    /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/SortedListTKey,TValue/Overview/remarks.vb" id="Snippet12" />
    /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/SortedListTKey,TValue/Overview/remarks.fs" id="Snippet12" />
    /// The <c>foreach</c> statement is a wrapper around the enumerator, which only allows reading from, not writing to, the collection.
    /// The following code example creates an empty <see cref="System.Collections.Generic.SortedList{T,U}"/> of strings with string keys and uses the <see cref="System.Collections.Generic.SortedList{T,U}.Add"/> method to add some elements. The example demonstrates that the <see cref="System.Collections.Generic.SortedList{T,U}.Add"/> method throws an <see cref="System.ArgumentException"/> when attempting to add a duplicate key.
    /// The example uses the <see cref="System.Collections.Generic.SortedList{T,U}.Item"/> property (the indexer in C#) to retrieve values, demonstrating that a <see cref="System.Collections.Generic.KeyNotFoundException"/> is thrown when a requested key is not present, and showing that the value associated with a key can be replaced.
    /// The example shows how to use the <see cref="System.Collections.Generic.SortedList{T,U}.TryGetValue"/> method as a more efficient way to retrieve values if a program often must try key values that are not in the sorted list, and it shows how to use the <see cref="System.Collections.Generic.SortedList{T,U}.ContainsKey"/> method to test whether a key exists before calling the <see cref="System.Collections.Generic.SortedList{T,U}.Add"/> method.
    /// The example shows how to enumerate the keys and values in the sorted list and how to enumerate the keys and values alone using the <see cref="System.Collections.Generic.SortedList{T,U}.Keys"/> property and the <see cref="System.Collections.Generic.SortedList{T,U}.Values"/> property.
    /// Finally, the example demonstrates the <see cref="System.Collections.Generic.SortedList{T,U}.Remove"/> method.
    /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/SortedListTKey,TValue/Overview/source.cs" id="Snippet1" />
    /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/SortedListTKey,TValue/Overview/source.vb" id="Snippet1" />
    /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/SortedListTKey,TValue/Overview/source.fs" id="Snippet1" />
    /// </remarks>
    [DebuggerTypeProxy(typeof(IDictionaryDebugView<,>))]
    [DebuggerDisplay("Count = {Count}")]
    [Serializable]
    [System.Runtime.CompilerServices.TypeForwardedFrom("System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    public class SortedList<TKey, TValue> :
        IDictionary<TKey, TValue>, IDictionary, IReadOnlyDictionary<TKey, TValue> where TKey : notnull
    {
        private TKey[] keys; // Do not rename (binary serialization)
        private TValue[] values; // Do not rename (binary serialization)
        private int _size; // Do not rename (binary serialization)
        private int version; // Do not rename (binary serialization)
        private readonly IComparer<TKey> comparer; // Do not rename (binary serialization)
        private KeyList? keyList; // Do not rename (binary serialization)
        private ValueList? valueList; // Do not rename (binary serialization)

        private const int DefaultCapacity = 4;

        // Constructs a new sorted list. The sorted list is initially empty and has
        // a capacity of zero. Upon adding the first element to the sorted list the
        // capacity is increased to DefaultCapacity, and then increased in multiples of two as
        // required. The elements of the sorted list are ordered according to the
        // IComparable interface, which must be implemented by the keys of
        // all entries added to the sorted list.
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Collections.Generic.SortedList`2"/> class that is empty, has the default initial capacity, and uses the default <see cref="T:System.Collections.Generic.IComparer`1"/>.
        /// </summary>
        /// <remarks>
        /// Every key in a <see cref="System.Collections.Generic.SortedList{T,U}"/> must be unique according to the default comparer.
        /// This constructor uses the default value for the initial capacity of the <see cref="System.Collections.Generic.SortedList{T,U}"/>. To set the initial capacity, use the <see cref="System.Collections.Generic.SortedList{T,U}.#ctor%28System.Int32%29"/> constructor. If the final size of the collection can be estimated, specifying the initial capacity eliminates the need to perform a number of resizing operations while adding elements to the <see cref="System.Collections.Generic.SortedList{T,U}"/>.
        /// This constructor uses the default comparer for <c>TKey</c>. To specify a comparer, use the <see cref="System.Collections.Generic.SortedList{T,U}.#ctor%28System.Collections.Generic.IComparer%7B{}%7D%29"/> constructor. The default comparer <see cref="System.Collections.Generic.Comparer{T}.Default">Default</see> checks whether the key type <c>TKey</c> implements <see cref="System.IComparable{T}">IComparable{T}</see> and uses that implementation, if available.  If not, <see cref="System.Collections.Generic.Comparer{T}.Default">Default</see> checks whether the key type <c>TKey</c> implements <see cref="System.IComparable">IComparable</see>.  If the key type <c>TKey</c> does not implement either interface, you can specify a <see cref="System.Collections.Generic.IComparer{T}">IComparer{T}</see> implementation in a constructor overload that accepts a <c>comparer</c> parameter.
        /// This constructor is an O(1) operation.
        /// The following code example creates an empty <see cref="System.Collections.Generic.SortedList{T,U}"/> of strings with string keys and uses the <see cref="System.Collections.Generic.SortedList{T,U}.Add"/> method to add some elements. The example demonstrates that the <see cref="System.Collections.Generic.SortedList{T,U}.Add"/> method throws an <see cref="System.ArgumentException"/> when attempting to add a duplicate key.
        /// This code example is part of a larger example provided for the <see cref="System.Collections.Generic.SortedList{T,U}"/> class.
        /// :::code language="csharp" source="~/snippets/csharp/System.Collections.Generic/SortedListTKey,TValue/Overview/source.cs" interactive="try-dotnet-method" id="Snippet2":::
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/SortedListTKey,TValue/Overview/source.vb" id="Snippet2" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/SortedListTKey,TValue/Overview/source.fs" id="Snippet2" />
        /// </remarks>
        public SortedList()
        {
            keys = Array.Empty<TKey>();
            values = Array.Empty<TValue>();
            _size = 0;
            comparer = Comparer<TKey>.Default;
        }

        // Constructs a new sorted list. The sorted list is initially empty and has
        // a capacity of zero. Upon adding the first element to the sorted list the
        // capacity is increased to 16, and then increased in multiples of two as
        // required. The elements of the sorted list are ordered according to the
        // IComparable interface, which must be implemented by the keys of
        // all entries added to the sorted list.
        //
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Collections.Generic.SortedList`2"/> class that is empty, has the specified initial capacity, and uses the default <see cref="T:System.Collections.Generic.IComparer`1"/>.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the <see cref="T:System.Collections.Generic.SortedList`2"/> can contain.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        /// <remarks>
        /// Every key in a <see cref="System.Collections.Generic.SortedList{T,U}"/> must be unique according to the default comparer.
        /// The capacity of a <see cref="System.Collections.Generic.SortedList{T,U}"/> is the number of elements that the <see cref="System.Collections.Generic.SortedList{T,U}"/> can hold before resizing. As elements are added to a <see cref="System.Collections.Generic.SortedList{T,U}"/>, the capacity is automatically increased as required by reallocating the internal array.
        /// If the size of the collection can be estimated, specifying the initial capacity eliminates the need to perform a number of resizing operations while adding elements to the <see cref="System.Collections.Generic.SortedList{T,U}"/>.
        /// The capacity can be decreased by calling <see cref="System.Collections.Generic.SortedList{T,U}.TrimExcess"/> or by setting the <see cref="System.Collections.Generic.SortedList{T,U}.Capacity"/> property explicitly. Decreasing the capacity reallocates memory and copies all the elements in the <see cref="System.Collections.Generic.SortedList{T,U}"/>.
        /// This constructor uses the default comparer for <c>TKey</c>. To specify a comparer, use the <see cref="System.Collections.Generic.SortedList{T,U}.#ctor%28System.Int32%2CSystem.Collections.Generic.IComparer%7B{}%7D%29"/> constructor. The default comparer <see cref="System.Collections.Generic.Comparer{T}.Default">Default</see> checks whether the key type <c>TKey</c> implements <see cref="System.IComparable{T}">IComparable{T}</see> and uses that implementation, if available.  If not, <see cref="System.Collections.Generic.Comparer{T}.Default">Default</see> checks whether the key type <c>TKey</c> implements <see cref="System.IComparable">IComparable</see>.  If the key type <c>TKey</c> does not implement either interface, you can specify a <see cref="System.Collections.Generic.IComparer{T}">IComparer{T}</see> implementation in a constructor overload that accepts a <c>comparer</c> parameter.
        /// This constructor is an O(<c>n</c>) operation, where <c>n</c> is <c>capacity</c>.
        /// The following code example creates a sorted list with an initial capacity of 4 and populates it with 4 entries.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/SortedListTKey,TValue/.ctor/source3.cs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/SortedListTKey,TValue/.ctor/source3.vb" id="Snippet1" />
        /// </remarks>
        public SortedList(int capacity)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(capacity);
            keys = new TKey[capacity];
            values = new TValue[capacity];
            comparer = Comparer<TKey>.Default;
        }

        // Constructs a new sorted list with a given IComparer
        // implementation. The sorted list is initially empty and has a capacity of
        // zero. Upon adding the first element to the sorted list the capacity is
        // increased to 16, and then increased in multiples of two as required. The
        // elements of the sorted list are ordered according to the given
        // IComparer implementation. If comparer is null, the
        // elements are compared to each other using the IComparable
        // interface, which in that case must be implemented by the keys of all
        // entries added to the sorted list.
        //
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Collections.Generic.SortedList`2"/> class that is empty, has the default initial capacity, and uses the default <see cref="T:System.Collections.Generic.IComparer`1"/>.
        /// </summary>
        /// <remarks>
        /// Every key in a <see cref="System.Collections.Generic.SortedList{T,U}"/> must be unique according to the default comparer.
        /// This constructor uses the default value for the initial capacity of the <see cref="System.Collections.Generic.SortedList{T,U}"/>. To set the initial capacity, use the <see cref="System.Collections.Generic.SortedList{T,U}.#ctor%28System.Int32%29"/> constructor. If the final size of the collection can be estimated, specifying the initial capacity eliminates the need to perform a number of resizing operations while adding elements to the <see cref="System.Collections.Generic.SortedList{T,U}"/>.
        /// This constructor uses the default comparer for <c>TKey</c>. To specify a comparer, use the <see cref="System.Collections.Generic.SortedList{T,U}.#ctor%28System.Collections.Generic.IComparer%7B{}%7D%29"/> constructor. The default comparer <see cref="System.Collections.Generic.Comparer{T}.Default">Default</see> checks whether the key type <c>TKey</c> implements <see cref="System.IComparable{T}">IComparable{T}</see> and uses that implementation, if available.  If not, <see cref="System.Collections.Generic.Comparer{T}.Default">Default</see> checks whether the key type <c>TKey</c> implements <see cref="System.IComparable">IComparable</see>.  If the key type <c>TKey</c> does not implement either interface, you can specify a <see cref="System.Collections.Generic.IComparer{T}">IComparer{T}</see> implementation in a constructor overload that accepts a <c>comparer</c> parameter.
        /// This constructor is an O(1) operation.
        /// The following code example creates an empty <see cref="System.Collections.Generic.SortedList{T,U}"/> of strings with string keys and uses the <see cref="System.Collections.Generic.SortedList{T,U}.Add"/> method to add some elements. The example demonstrates that the <see cref="System.Collections.Generic.SortedList{T,U}.Add"/> method throws an <see cref="System.ArgumentException"/> when attempting to add a duplicate key.
        /// This code example is part of a larger example provided for the <see cref="System.Collections.Generic.SortedList{T,U}"/> class.
        /// :::code language="csharp" source="~/snippets/csharp/System.Collections.Generic/SortedListTKey,TValue/Overview/source.cs" interactive="try-dotnet-method" id="Snippet2":::
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/SortedListTKey,TValue/Overview/source.vb" id="Snippet2" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/SortedListTKey,TValue/Overview/source.fs" id="Snippet2" />
        /// </remarks>
        public SortedList(IComparer<TKey>? comparer)
            : this()
        {
            if (comparer != null)
            {
                this.comparer = comparer;
            }
        }

        // Constructs a new sorted dictionary with a given IComparer
        // implementation and a given initial capacity. The sorted list is
        // initially empty, but will have room for the given number of elements
        // before any reallocations are required. The elements of the sorted list
        // are ordered according to the given IComparer implementation. If
        // comparer is null, the elements are compared to each other using
        // the IComparable interface, which in that case must be implemented
        // by the keys of all entries added to the sorted list.
        //
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Collections.Generic.SortedList`2"/> class that is empty, has the default initial capacity, and uses the default <see cref="T:System.Collections.Generic.IComparer`1"/>.
        /// </summary>
        /// <remarks>
        /// Every key in a <see cref="System.Collections.Generic.SortedList{T,U}"/> must be unique according to the default comparer.
        /// This constructor uses the default value for the initial capacity of the <see cref="System.Collections.Generic.SortedList{T,U}"/>. To set the initial capacity, use the <see cref="System.Collections.Generic.SortedList{T,U}.#ctor%28System.Int32%29"/> constructor. If the final size of the collection can be estimated, specifying the initial capacity eliminates the need to perform a number of resizing operations while adding elements to the <see cref="System.Collections.Generic.SortedList{T,U}"/>.
        /// This constructor uses the default comparer for <c>TKey</c>. To specify a comparer, use the <see cref="System.Collections.Generic.SortedList{T,U}.#ctor%28System.Collections.Generic.IComparer%7B{}%7D%29"/> constructor. The default comparer <see cref="System.Collections.Generic.Comparer{T}.Default">Default</see> checks whether the key type <c>TKey</c> implements <see cref="System.IComparable{T}">IComparable{T}</see> and uses that implementation, if available.  If not, <see cref="System.Collections.Generic.Comparer{T}.Default">Default</see> checks whether the key type <c>TKey</c> implements <see cref="System.IComparable">IComparable</see>.  If the key type <c>TKey</c> does not implement either interface, you can specify a <see cref="System.Collections.Generic.IComparer{T}">IComparer{T}</see> implementation in a constructor overload that accepts a <c>comparer</c> parameter.
        /// This constructor is an O(1) operation.
        /// The following code example creates an empty <see cref="System.Collections.Generic.SortedList{T,U}"/> of strings with string keys and uses the <see cref="System.Collections.Generic.SortedList{T,U}.Add"/> method to add some elements. The example demonstrates that the <see cref="System.Collections.Generic.SortedList{T,U}.Add"/> method throws an <see cref="System.ArgumentException"/> when attempting to add a duplicate key.
        /// This code example is part of a larger example provided for the <see cref="System.Collections.Generic.SortedList{T,U}"/> class.
        /// :::code language="csharp" source="~/snippets/csharp/System.Collections.Generic/SortedListTKey,TValue/Overview/source.cs" interactive="try-dotnet-method" id="Snippet2":::
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/SortedListTKey,TValue/Overview/source.vb" id="Snippet2" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/SortedListTKey,TValue/Overview/source.fs" id="Snippet2" />
        /// </remarks>
        public SortedList(int capacity, IComparer<TKey>? comparer)
            : this(comparer)
        {
            Capacity = capacity;
        }

        // Constructs a new sorted list containing a copy of the entries in the
        // given dictionary. The elements of the sorted list are ordered according
        // to the IComparable interface, which must be implemented by the
        // keys of all entries in the given dictionary as well as keys
        // subsequently added to the sorted list.
        //
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Collections.Generic.SortedList`2"/> class that is empty, has the default initial capacity, and uses the default <see cref="T:System.Collections.Generic.IComparer`1"/>.
        /// </summary>
        /// <remarks>
        /// Every key in a <see cref="System.Collections.Generic.SortedList{T,U}"/> must be unique according to the default comparer.
        /// This constructor uses the default value for the initial capacity of the <see cref="System.Collections.Generic.SortedList{T,U}"/>. To set the initial capacity, use the <see cref="System.Collections.Generic.SortedList{T,U}.#ctor%28System.Int32%29"/> constructor. If the final size of the collection can be estimated, specifying the initial capacity eliminates the need to perform a number of resizing operations while adding elements to the <see cref="System.Collections.Generic.SortedList{T,U}"/>.
        /// This constructor uses the default comparer for <c>TKey</c>. To specify a comparer, use the <see cref="System.Collections.Generic.SortedList{T,U}.#ctor%28System.Collections.Generic.IComparer%7B{}%7D%29"/> constructor. The default comparer <see cref="System.Collections.Generic.Comparer{T}.Default">Default</see> checks whether the key type <c>TKey</c> implements <see cref="System.IComparable{T}">IComparable{T}</see> and uses that implementation, if available.  If not, <see cref="System.Collections.Generic.Comparer{T}.Default">Default</see> checks whether the key type <c>TKey</c> implements <see cref="System.IComparable">IComparable</see>.  If the key type <c>TKey</c> does not implement either interface, you can specify a <see cref="System.Collections.Generic.IComparer{T}">IComparer{T}</see> implementation in a constructor overload that accepts a <c>comparer</c> parameter.
        /// This constructor is an O(1) operation.
        /// The following code example creates an empty <see cref="System.Collections.Generic.SortedList{T,U}"/> of strings with string keys and uses the <see cref="System.Collections.Generic.SortedList{T,U}.Add"/> method to add some elements. The example demonstrates that the <see cref="System.Collections.Generic.SortedList{T,U}.Add"/> method throws an <see cref="System.ArgumentException"/> when attempting to add a duplicate key.
        /// This code example is part of a larger example provided for the <see cref="System.Collections.Generic.SortedList{T,U}"/> class.
        /// :::code language="csharp" source="~/snippets/csharp/System.Collections.Generic/SortedListTKey,TValue/Overview/source.cs" interactive="try-dotnet-method" id="Snippet2":::
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/SortedListTKey,TValue/Overview/source.vb" id="Snippet2" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/SortedListTKey,TValue/Overview/source.fs" id="Snippet2" />
        /// </remarks>
        public SortedList(IDictionary<TKey, TValue> dictionary)
            : this(dictionary, null)
        {
        }

        // Constructs a new sorted list containing a copy of the entries in the
        // given dictionary. The elements of the sorted list are ordered according
        // to the given IComparer implementation. If comparer is
        // null, the elements are compared to each other using the
        // IComparable interface, which in that case must be implemented
        // by the keys of all entries in the given dictionary as well as keys
        // subsequently added to the sorted list.
        //
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Collections.Generic.SortedList`2"/> class that is empty, has the default initial capacity, and uses the default <see cref="T:System.Collections.Generic.IComparer`1"/>.
        /// </summary>
        /// <remarks>
        /// Every key in a <see cref="System.Collections.Generic.SortedList{T,U}"/> must be unique according to the default comparer.
        /// This constructor uses the default value for the initial capacity of the <see cref="System.Collections.Generic.SortedList{T,U}"/>. To set the initial capacity, use the <see cref="System.Collections.Generic.SortedList{T,U}.#ctor%28System.Int32%29"/> constructor. If the final size of the collection can be estimated, specifying the initial capacity eliminates the need to perform a number of resizing operations while adding elements to the <see cref="System.Collections.Generic.SortedList{T,U}"/>.
        /// This constructor uses the default comparer for <c>TKey</c>. To specify a comparer, use the <see cref="System.Collections.Generic.SortedList{T,U}.#ctor%28System.Collections.Generic.IComparer%7B{}%7D%29"/> constructor. The default comparer <see cref="System.Collections.Generic.Comparer{T}.Default">Default</see> checks whether the key type <c>TKey</c> implements <see cref="System.IComparable{T}">IComparable{T}</see> and uses that implementation, if available.  If not, <see cref="System.Collections.Generic.Comparer{T}.Default">Default</see> checks whether the key type <c>TKey</c> implements <see cref="System.IComparable">IComparable</see>.  If the key type <c>TKey</c> does not implement either interface, you can specify a <see cref="System.Collections.Generic.IComparer{T}">IComparer{T}</see> implementation in a constructor overload that accepts a <c>comparer</c> parameter.
        /// This constructor is an O(1) operation.
        /// The following code example creates an empty <see cref="System.Collections.Generic.SortedList{T,U}"/> of strings with string keys and uses the <see cref="System.Collections.Generic.SortedList{T,U}.Add"/> method to add some elements. The example demonstrates that the <see cref="System.Collections.Generic.SortedList{T,U}.Add"/> method throws an <see cref="System.ArgumentException"/> when attempting to add a duplicate key.
        /// This code example is part of a larger example provided for the <see cref="System.Collections.Generic.SortedList{T,U}"/> class.
        /// :::code language="csharp" source="~/snippets/csharp/System.Collections.Generic/SortedListTKey,TValue/Overview/source.cs" interactive="try-dotnet-method" id="Snippet2":::
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/SortedListTKey,TValue/Overview/source.vb" id="Snippet2" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/SortedListTKey,TValue/Overview/source.fs" id="Snippet2" />
        /// </remarks>
        public SortedList(IDictionary<TKey, TValue> dictionary, IComparer<TKey>? comparer)
            : this(dictionary?.Count ?? throw new ArgumentNullException(nameof(dictionary)), comparer)
        {
            int count = dictionary.Count;
            if (count != 0)
            {
                TKey[] keys = this.keys;
                dictionary.Keys.CopyTo(keys, 0);
                dictionary.Values.CopyTo(values, 0);
                Debug.Assert(count == this.keys.Length);
                if (count > 1)
                {
                    comparer = Comparer; // obtain default if this is null.
                    Array.Sort<TKey, TValue>(keys, values, comparer);
                    for (int i = 1; i < keys.Length; ++i)
                    {
                        if (comparer.Compare(keys[i - 1], keys[i]) == 0)
                        {
                            throw new ArgumentException(SR.Format(SR.Argument_AddingDuplicate, keys[i]));
                        }
                    }
                }
            }

            _size = count;
        }

        // Adds an entry with the given key and value to this sorted list. An
        // ArgumentException is thrown if the key is already present in the sorted list.
        //
        /// <summary>
        /// Adds an element with the specified key and value into the <see cref="T:System.Collections.Generic.SortedList`2"/>.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add. The value can be <see langword="null"/> for reference types.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentException">An element with the same key already exists in the <see cref="T:System.Collections.Generic.SortedList`2"/>.</exception>
        /// <remarks>
        /// A key cannot be <c>null</c>, but a value can be, if the type of values in the sorted list, <c>TValue</c>, is a reference type.
        /// You can also use the <see cref="System.Collections.Generic.SortedList{T,U}.Item"/> property to add new elements by setting the value of a key that does not exist in the <see cref="System.Collections.Generic.SortedList{T,U}"/>; for example, <c>myCollection[&quot;myNonexistentKey&quot;] = myValue</c>. However, if the specified key already exists in the <see cref="System.Collections.Generic.SortedList{T,U}"/>, setting the <see cref="System.Collections.Generic.SortedList{T,U}.Item"/> property overwrites the old value. In contrast, the <see cref="System.Collections.Generic.SortedList{T,U}.Add"/> method does not modify existing elements.
        /// If <see cref="System.Collections.Generic.SortedList{T,U}.Count"/> already equals <see cref="System.Collections.Generic.SortedList{T,U}.Capacity"/>, the capacity of the <see cref="System.Collections.Generic.SortedList{T,U}"/> is increased by automatically reallocating the internal array, and the existing elements are copied to the new array before the new element is added.
        /// This method is an O(<c>n</c>) operation for unsorted data, where <c>n</c> is <see cref="System.Collections.Generic.SortedList{T,U}.Count"/>. It is an O(log <c>n</c>) operation if the new element is added at the end of the list. If insertion causes a resize, the operation is O(<c>n</c>).
        /// The following code example creates an empty <see cref="System.Collections.Generic.SortedList{T,U}"/> of strings with string keys and uses the <see cref="System.Collections.Generic.SortedList{T,U}.Add"/> method to add some elements. The example demonstrates that the <see cref="System.Collections.Generic.SortedList{T,U}.Add"/> method throws an <see cref="System.ArgumentException"/> when attempting to add a duplicate key.
        /// This code example is part of a larger example provided for the <see cref="System.Collections.Generic.SortedList{T,U}"/> class.
        /// :::code language="csharp" source="~/snippets/csharp/System.Collections.Generic/SortedListTKey,TValue/Overview/source.cs" interactive="try-dotnet-method" id="Snippet2":::
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/SortedListTKey,TValue/Overview/source.vb" id="Snippet2" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/SortedListTKey,TValue/Overview/source.fs" id="Snippet2" />
        /// </remarks>
        public void Add(TKey key, TValue value)
        {
            ArgumentNullException.ThrowIfNull(key);

            int i = Array.BinarySearch<TKey>(keys, 0, _size, key, comparer);
            if (i >= 0)
                throw new ArgumentException(SR.Format(SR.Argument_AddingDuplicate, key), nameof(key));
            Insert(~i, key, value);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> keyValuePair)
        {
            Add(keyValuePair.Key, keyValuePair.Value);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> keyValuePair)
        {
            int index = IndexOfKey(keyValuePair.Key);
            if (index >= 0 && EqualityComparer<TValue>.Default.Equals(values[index], keyValuePair.Value))
            {
                return true;
            }
            return false;
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> keyValuePair)
        {
            int index = IndexOfKey(keyValuePair.Key);
            if (index >= 0 && EqualityComparer<TValue>.Default.Equals(values[index], keyValuePair.Value))
            {
                RemoveAt(index);
                return true;
            }
            return false;
        }

        // Returns the capacity of this sorted list. The capacity of a sorted list
        // represents the allocated length of the internal arrays used to store the
        // keys and values of the list, and thus also indicates the maximum number
        // of entries the list can contain before a reallocation of the internal
        // arrays is required.
        //
        /// <summary>
        /// Gets or sets the number of elements that the <see cref="T:System.Collections.Generic.SortedList`2"/> can contain.
        /// </summary>
        /// <value>The number of elements that the <see cref="T:System.Collections.Generic.SortedList`2"/> can contain.</value>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><see cref="P:System.Collections.Generic.SortedList`2.Capacity"/> is set to a value that is less than <see cref="P:System.Collections.Generic.SortedList`2.Count"/>.</exception>
        /// <exception cref="T:System.OutOfMemoryException">There is not enough memory available on the system.</exception>
        /// <remarks>
        /// <see cref="System.Collections.Generic.SortedList{T,U}.Capacity"/> is the number of elements that the <see cref="System.Collections.Generic.SortedList{T,U}"/> can store. <see cref="System.Collections.Generic.SortedList{T,U}.Count"/> is the number of elements that are actually in the <see cref="System.Collections.Generic.SortedList{T,U}"/>.
        /// <see cref="System.Collections.Generic.SortedList{T,U}.Capacity"/> is always greater than or equal to <see cref="System.Collections.Generic.SortedList{T,U}.Count"/>. If <see cref="System.Collections.Generic.SortedList{T,U}.Count"/> exceeds <see cref="System.Collections.Generic.SortedList{T,U}.Capacity"/> while adding elements, the capacity is increased by automatically reallocating the internal array before copying the old elements and adding the new elements.
        /// The capacity can be decreased by calling <see cref="System.Collections.Generic.SortedList{T,U}.TrimExcess"/> or by setting the <see cref="System.Collections.Generic.SortedList{T,U}.Capacity"/> property explicitly. When the value of <see cref="System.Collections.Generic.SortedList{T,U}.Capacity"/> is set explicitly, the internal array is also reallocated to accommodate the specified capacity.
        /// Retrieving the value of this property is an O(1) operation; setting the property is an O(<c>n</c>) operation, where <c>n</c> is the new capacity.
        /// </remarks>
        public int Capacity
        {
            get
            {
                return keys.Length;
            }
            set
            {
                if (value != keys.Length)
                {
                    if (value < _size)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), value, SR.ArgumentOutOfRange_SmallCapacity);
                    }

                    if (value > 0)
                    {
                        TKey[] newKeys = new TKey[value];
                        TValue[] newValues = new TValue[value];
                        if (_size > 0)
                        {
                            Array.Copy(keys, newKeys, _size);
                            Array.Copy(values, newValues, _size);
                        }
                        keys = newKeys;
                        values = newValues;
                    }
                    else
                    {
                        keys = Array.Empty<TKey>();
                        values = Array.Empty<TValue>();
                    }
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="T:System.Collections.Generic.IComparer`1"/> for the sorted list.
        /// </summary>
        /// <value>The <see cref="T:System.IComparable`1"/> for the current <see cref="T:System.Collections.Generic.SortedList`2"/>.</value>
        /// <remarks>
        /// Retrieving the value of this property is an O(1) operation.
        /// </remarks>
        public IComparer<TKey> Comparer
        {
            get
            {
                return comparer;
            }
        }

        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="T:System.Collections.IDictionary"/>.
        /// </summary>
        /// <param name="key">The <see cref="T:System.Object"/> to use as the key of the element to add.</param>
        /// <param name="value">The <see cref="T:System.Object"/> to use as the value of the element to add.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="key"/> is of a type that is not assignable to the key type <paramref name="TKey"/> of the <see cref="T:System.Collections.IDictionary"/>. -or- <paramref name="value"/> is of a type that is not assignable to the value type <paramref name="TValue"/> of the <see cref="T:System.Collections.IDictionary"/>. -or- An element with the same key already exists in the <see cref="T:System.Collections.IDictionary"/>.</exception>
        /// <remarks>
        /// You can also use the <see cref="System.Collections.IDictionary.Item"/> property to add new elements by setting the value of a key that does not exist in the dictionary; for example, <c>myCollection[&quot;myNonexistentKey&quot;] = myValue</c>. However, if the specified key already exists in the dictionary, setting the <see cref="System.Collections.IDictionary.Item"/> property overwrites the old value. In contrast, the <see cref="System.Collections.IDictionary.Add"/> method does not modify existing elements.
        /// This method is an O(<c>n</c>) operation for unsorted data, where <c>n</c> is <see cref="System.Collections.Generic.SortedList{T,U}.Count"/>. It is an O(log <c>n</c>) operation if the new element is added at the end of the list. If insertion causes a resize, the operation is O(<c>n</c>).
        /// The following code example shows how to access the <see cref="System.Collections.Generic.SortedList{T,U}"/> class through the <see cref="System.Collections.IDictionary">IDictionary</see> interface. The code example creates an empty <see cref="System.Collections.Generic.SortedList{T,U}"/> of strings with string keys and uses the <see cref="System.Collections.Generic.SortedList{T,U}.System#Collections#IDictionary#Add"/> method to add some elements. The example demonstrates that the <see cref="System.Collections.Generic.SortedList{T,U}.System#Collections#IDictionary#Add"/> method throws an <see cref="System.ArgumentException"/> when attempting to add a duplicate key, or when a key or value of the wrong data type is supplied.
        /// The code example demonstrates the use of several other members of the <see cref="System.Collections.IDictionary">IDictionary</see> interface.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/SortedListTKey,TValue/System.Collections.IDictionary.Add/source.cs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/SortedListTKey,TValue/System.Collections.IDictionary.Add/source.vb" id="Snippet1" />
        /// </remarks>
        void IDictionary.Add(object key, object? value)
        {
            ArgumentNullException.ThrowIfNull(key);

            if (default(TValue) != null)    // null is an invalid value for Value types
                ArgumentNullException.ThrowIfNull(value);

            if (!(key is TKey))
                throw new ArgumentException(SR.Format(SR.Arg_WrongType, key, typeof(TKey)), nameof(key));

            if (!(value is TValue) && value != null)            // null is a valid value for Reference Types
                throw new ArgumentException(SR.Format(SR.Arg_WrongType, value, typeof(TValue)), nameof(value));

            Add((TKey)key, (TValue)value!);
        }

        // Returns the number of entries in this sorted list.
        /// <summary>
        /// Gets the number of key/value pairs contained in the <see cref="T:System.Collections.Generic.SortedList`2"/>.
        /// </summary>
        /// <value>The number of key/value pairs contained in the <see cref="T:System.Collections.Generic.SortedList`2"/>.</value>
        /// <remarks>
        /// <see cref="System.Collections.Generic.SortedList{T,U}.Capacity"/> is the number of elements that the <see cref="System.Collections.Generic.SortedList{T,U}"/> can store. <see cref="System.Collections.Generic.SortedList{T,U}.Count"/> is the number of elements that are actually in the <see cref="System.Collections.Generic.SortedList{T,U}"/>.
        /// <see cref="System.Collections.Generic.SortedList{T,U}.Capacity"/> is always greater than or equal to <see cref="System.Collections.Generic.SortedList{T,U}.Count"/>. If <see cref="System.Collections.Generic.SortedList{T,U}.Count"/> exceeds <see cref="System.Collections.Generic.SortedList{T,U}.Capacity"/> while adding elements, the capacity is increased by automatically reallocating the internal array before copying the old elements and adding the new elements.
        /// Retrieving the value of this property is an O(1) operation.
        /// </remarks>
        public int Count
        {
            get
            {
                return _size;
            }
        }

        // Returns a collection representing the keys of this sorted list. This
        // method returns the same object as GetKeyList, but typed as an
        // ICollection instead of an IList.
        /// <summary>
        /// Gets a collection containing the keys in the <see cref="T:System.Collections.Generic.SortedList`2"/>, in sorted order.
        /// </summary>
        /// <value>A <see cref="T:System.Collections.Generic.IList`1"/> containing the keys in the <see cref="T:System.Collections.Generic.SortedList`2"/>.</value>
        /// <remarks>
        /// The order of the keys in the <see cref="System.Collections.Generic.IList{T}"/> is the same as the order in the <see cref="System.Collections.Generic.SortedList{T,U}"/>.
        /// The returned <see cref="System.Collections.Generic.IList{T}"/> is not a static copy; instead, the <see cref="System.Collections.Generic.IList{T}"/> refers back to the keys in the original <see cref="System.Collections.Generic.SortedList{T,U}"/>. Therefore, changes to the <see cref="System.Collections.Generic.SortedList{T,U}"/> continue to be reflected in the <see cref="System.Collections.Generic.IList{T}"/>.
        /// The collection returned by the <see cref="System.Collections.Generic.SortedList{T,U}.Keys"/> property provides an efficient way to retrieve keys by index. It is not necessary to regenerate the list when the property is accessed, because the list is just a wrapper for the internal array of keys. The following code shows the use of the <see cref="System.Collections.Generic.SortedList{T,U}.Keys"/> property for indexed retrieval of keys from a sorted list of elements with string keys:
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/SortedListTKey,TValue/Overview/remarks.cs" id="Snippet11" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/SortedListTKey,TValue/Overview/remarks.vb" id="Snippet11" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/SortedListTKey,TValue/Overview/remarks.fs" id="Snippet11" />
        /// Retrieving the value of this property is an O(1) operation.
        /// The following code example shows how to enumerate the keys in the sorted list using the <see cref="System.Collections.Generic.SortedList{T,U}.Keys"/> property, and how to enumerate the keys and values in the sorted list.
        /// The example also shows how to use the <see cref="System.Collections.Generic.SortedList{T,U}.Keys"/> property for efficient indexed retrieval of keys.
        /// This code is part of a larger example that can be compiled and executed. See <see cref="System.Collections.Generic.SortedList{T,U}"/>.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/SortedListTKey,TValue/Overview/source.cs" id="Snippet9" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/SortedListTKey,TValue/Overview/source.vb" id="Snippet9" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/SortedListTKey,TValue/Overview/source.fs" id="Snippet9" />
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/SortedListTKey,TValue/Overview/source.cs" id="Snippet7" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/SortedListTKey,TValue/Overview/source.vb" id="Snippet7" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/SortedListTKey,TValue/Overview/source.fs" id="Snippet7" />
        /// </remarks>
        public IList<TKey> Keys
        {
            get
            {
                return GetKeyListHelper();
            }
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <value>An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2"/>.</value>
        ICollection<TKey> IDictionary<TKey, TValue>.Keys
        {
            get
            {
                return GetKeyListHelper();
            }
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.ICollection"/> containing the keys of the <see cref="T:System.Collections.IDictionary"/>.
        /// </summary>
        /// <value>An <see cref="T:System.Collections.ICollection"/> containing the keys of the <see cref="T:System.Collections.IDictionary"/>.</value>
        /// <remarks>
        /// The order of the keys in the <see cref="System.Collections.ICollection"/> is the same as the order in the <see cref="System.Collections.Generic.SortedList{T,U}"/>.
        /// Retrieving the value of this property is an O(1) operation.
        /// The following code example shows how to use the <see cref="System.Collections.IDictionary.Keys"/> property of the <see cref="System.Collections.IDictionary">IDictionary</see> interface with a <see cref="System.Collections.Generic.SortedDictionary{T,U}"/>, to list the keys in the dictionary. The example also shows how to enumerate the key/value pairs in the sorted list; note that the enumerator for the <see cref="System.Collections.IDictionary">IDictionary</see> interface returns <see cref="System.Collections.DictionaryEntry"/> objects rather than <see cref="System.Collections.Generic.KeyValuePair{T,U}"/> objects.
        /// The code example is part of a larger example, including output, provided for the <see cref="System.Collections.Generic.SortedList{T,U}.System#Collections#IDictionary#Add"/> method.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/SortedListTKey,TValue/System.Collections.IDictionary.Add/source.cs" id="Snippet31" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/SortedListTKey,TValue/System.Collections.IDictionary.Add/source.vb" id="Snippet31" />
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/SortedListTKey,TValue/System.Collections.IDictionary.Add/source.cs" id="Snippet9" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/SortedListTKey,TValue/System.Collections.IDictionary.Add/source.vb" id="Snippet9" />
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/SortedListTKey,TValue/System.Collections.IDictionary.Add/source.cs" id="Snippet7" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/SortedListTKey,TValue/System.Collections.IDictionary.Add/source.vb" id="Snippet7" />
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/SortedListTKey,TValue/System.Collections.IDictionary.Add/source.cs" id="Snippet32" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/SortedListTKey,TValue/System.Collections.IDictionary.Add/source.vb" id="Snippet32" />
        /// </remarks>
        ICollection IDictionary.Keys
        {
            get
            {
                return GetKeyListHelper();
            }
        }

        /// <summary>
        /// Gets an enumerable collection that contains the keys in the read-only dictionary.
        /// </summary>
        /// <value>An enumerable collection that contains the keys in the read-only dictionary.</value>
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
        {
            get
            {
                return GetKeyListHelper();
            }
        }

        // Returns a collection representing the values of this sorted list. This
        // method returns the same object as GetValueList, but typed as an
        // ICollection instead of an IList.
        //
        /// <summary>
        /// Gets a collection containing the values in the <see cref="T:System.Collections.Generic.SortedList`2"/>.
        /// </summary>
        /// <value>A <see cref="T:System.Collections.Generic.IList`1"/> containing the values in the <see cref="T:System.Collections.Generic.SortedList`2"/>.</value>
        /// <remarks>
        /// The order of the values in the <see cref="System.Collections.Generic.IList{T}"/> is the same as the order in the <see cref="System.Collections.Generic.SortedList{T,U}"/>.
        /// The returned <see cref="System.Collections.Generic.IList{T}"/> is not a static copy; instead, the <see cref="System.Collections.Generic.IList{T}"/> refers back to the values in the original <see cref="System.Collections.Generic.SortedList{T,U}"/>. Therefore, changes to the <see cref="System.Collections.Generic.SortedList{T,U}"/> continue to be reflected in the <see cref="System.Collections.Generic.IList{T}"/>.
        /// The collection returned by the <see cref="System.Collections.Generic.SortedList{T,U}.Values"/> property provides an efficient way to retrieve values by index. It is not necessary to regenerate the list when the property is accessed, because the list is just a wrapper for the internal array of values. The following code shows the use of the <see cref="System.Collections.Generic.SortedList{T,U}.Values"/> property for indexed retrieval of values from a sorted list of strings:
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/SortedListTKey,TValue/Overview/remarks.cs" id="Snippet11" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/SortedListTKey,TValue/Overview/remarks.vb" id="Snippet11" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/SortedListTKey,TValue/Overview/remarks.fs" id="Snippet11" />
        /// Retrieving the value of this property is an O(1) operation.
        /// This code example shows how to enumerate the values in the sorted list using the <see cref="System.Collections.Generic.SortedList{T,U}.Values"/> property, and how to enumerate the keys and values in the sorted list.
        /// The example also shows how to use the <see cref="System.Collections.Generic.SortedList{T,U}.Values"/> property for efficient indexed retrieval of values.
        /// This code example is part of a larger example provided for the <see cref="System.Collections.Generic.SortedList{T,U}"/> class.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/SortedListTKey,TValue/Overview/source.cs" id="Snippet8" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/SortedListTKey,TValue/Overview/source.vb" id="Snippet8" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/SortedListTKey,TValue/Overview/source.fs" id="Snippet8" />
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/SortedListTKey,TValue/Overview/source.cs" id="Snippet7" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/SortedListTKey,TValue/Overview/source.vb" id="Snippet7" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/SortedListTKey,TValue/Overview/source.fs" id="Snippet7" />
        /// </remarks>
        public IList<TValue> Values
        {
            get
            {
                return GetValueListHelper();
            }
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <value>An object containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.</value>
        ICollection<TValue> IDictionary<TKey, TValue>.Values
        {
            get
            {
                return GetValueListHelper();
            }
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.ICollection"/> containing the values in the <see cref="T:System.Collections.IDictionary"/>.
        /// </summary>
        /// <value>An <see cref="T:System.Collections.ICollection"/> containing the values in the <see cref="T:System.Collections.IDictionary"/>.</value>
        /// <remarks>
        /// The order of the values in the <see cref="System.Collections.ICollection"/> is the same as the order in the <see cref="System.Collections.Generic.SortedList{T,U}"/>.
        /// Retrieving the value of this property is an O(1) operation.
        /// The following code example shows how to use the <see cref="System.Collections.Generic.SortedList{T,U}.System#Collections#IDictionary#Values"/> property of the <see cref="System.Collections.IDictionary">IDictionary</see> interface with a <see cref="System.Collections.Generic.SortedList{T,U}"/>, to list the values in the sorted list. The example also shows how to enumerate the key/value pairs in the sorted list; note that the enumerator for the <see cref="System.Collections.IDictionary">IDictionary</see> interface returns <see cref="System.Collections.DictionaryEntry"/> objects rather than <see cref="System.Collections.Generic.KeyValuePair{T,U}"/> objects.
        /// The code example is part of a larger example, including output, provided for the <see cref="System.Collections.Generic.SortedList{T,U}.System#Collections#IDictionary#Add"/> method.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/SortedListTKey,TValue/System.Collections.IDictionary.Add/source.cs" id="Snippet31" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/SortedListTKey,TValue/System.Collections.IDictionary.Add/source.vb" id="Snippet31" />
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/SortedListTKey,TValue/System.Collections.IDictionary.Add/source.cs" id="Snippet8" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/SortedListTKey,TValue/System.Collections.IDictionary.Add/source.vb" id="Snippet8" />
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/SortedListTKey,TValue/System.Collections.IDictionary.Add/source.cs" id="Snippet7" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/SortedListTKey,TValue/System.Collections.IDictionary.Add/source.vb" id="Snippet7" />
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/SortedListTKey,TValue/System.Collections.IDictionary.Add/source.cs" id="Snippet32" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/SortedListTKey,TValue/System.Collections.IDictionary.Add/source.vb" id="Snippet32" />
        /// </remarks>
        ICollection IDictionary.Values
        {
            get
            {
                return GetValueListHelper();
            }
        }

        /// <summary>
        /// Gets an enumerable collection that contains the values in the read-only dictionary.
        /// </summary>
        /// <value>An enumerable collection that contains the values in the read-only dictionary.</value>
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
        {
            get
            {
                return GetValueListHelper();
            }
        }

        private KeyList GetKeyListHelper() => keyList ??= new KeyList(this);

        private ValueList GetValueListHelper() => valueList ??= new ValueList(this);

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <value><see langword="true"/> if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, <see langword="false"/>. In the default implementation of <see cref="T:System.Collections.Generic.SortedList`2"/>, this property always returns <see langword="false"/>.</value>
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.IDictionary"/> is read-only.
        /// </summary>
        /// <value><see langword="true"/> if the <see cref="T:System.Collections.IDictionary"/> is read-only; otherwise, <see langword="false"/>. In the default implementation of <see cref="T:System.Collections.Generic.SortedList`2"/>, this property always returns <see langword="false"/>.</value>
        /// <remarks>
        /// A collection that is read-only does not allow the addition, removal, or modification of elements after the collection is created.
        /// A collection that is read-only is simply a collection with a wrapper that prevents modifying the collection; therefore, if changes are made to the underlying collection, the read-only collection reflects those changes.
        /// Retrieving the value of this property is an O(1) operation.
        /// </remarks>
        bool IDictionary.IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.IDictionary"/> has a fixed size.
        /// </summary>
        /// <value><see langword="true"/> if the <see cref="T:System.Collections.IDictionary"/> has a fixed size; otherwise, <see langword="false"/>. In the default implementation of <see cref="T:System.Collections.Generic.SortedList`2"/>, this property always returns <see langword="false"/>.</value>
        /// <remarks>
        /// A collection with a fixed size does not allow the addition or removal of elements after the collection is created, but it allows the modification of existing elements.
        /// A collection with a fixed size is simply a collection with a wrapper that prevents adding and removing elements; therefore, if changes are made to the underlying collection, including the addition or removal of elements, the fixed-size collection reflects those changes.
        /// Retrieving the value of this property is an O(1) operation.
        /// </remarks>
        bool IDictionary.IsFixedSize
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe).
        /// </summary>
        /// <value><see langword="true"/> if access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe); otherwise, <see langword="false"/>. In the default implementation of <see cref="T:System.Collections.Generic.SortedList`2"/>, this property always returns <see langword="false"/>.</value>
        /// <remarks>
        /// Default implementations of collections in <see cref="System.Collections.Generic">Generic</see> are not synchronized.
        /// Enumerating through a collection is intrinsically not a thread-safe procedure.  To guarantee thread safety during enumeration, you can lock the collection during the entire enumeration.  To allow the collection to be accessed by multiple threads for reading and writing, you must implement your own synchronization.
        /// The <see cref="System.Collections.ICollection.SyncRoot"/> property returns an object that can be used to synchronize access to the <see cref="System.Collections.ICollection"/>. Synchronization is effective only if all threads lock this object before accessing the collection.
        /// Retrieving the value of this property is an O(1) operation.
        /// </remarks>
        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        // Synchronization root for this object.
        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
        /// </summary>
        /// <value>An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>. In the default implementation of <see cref="T:System.Collections.Generic.SortedList`2"/>, this property always returns the current instance.</value>
        /// <remarks>
        /// <code language="csharp">
        /// </code>
        /// <code language="vb">
        /// </code>
        /// Default implementations of collections in <see cref="System.Collections.Generic">Generic</see> are not synchronized.
        /// Enumerating through a collection is intrinsically not a thread-safe procedure. To guarantee thread safety during enumeration, you can lock the collection during the entire enumeration. To allow the collection to be accessed by multiple threads for reading and writing, you must implement your own synchronization.
        /// The <see cref="System.Collections.ICollection.SyncRoot"/> property returns an object that can be used to synchronize access to the <see cref="System.Collections.ICollection"/>. Synchronization is effective only if all threads lock this object before accessing the collection. The following code shows the use of the <see cref="System.Collections.ICollection.SyncRoot"/> property.
        /// ODE0 
        /// ODE1 
        /// Retrieving the value of this property is an O(1) operation.
        /// </remarks>
        object ICollection.SyncRoot => this;

        // Removes all entries from this sorted list.
        /// <summary>
        /// Removes all elements from the <see cref="T:System.Collections.Generic.SortedList`2"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="System.Collections.Generic.SortedList{T,U}.Count"/> is set to zero, and references to other objects from elements of the collection are also released.
        /// <see cref="System.Collections.Generic.SortedList{T,U}.Capacity"/> remains unchanged. To reset the capacity of the <see cref="System.Collections.Generic.SortedList{T,U}"/>, call <see cref="System.Collections.Generic.SortedList{T,U}.TrimExcess"/> or set the <see cref="System.Collections.Generic.SortedList{T,U}.Capacity"/> property directly. Trimming an empty <see cref="System.Collections.Generic.SortedList{T,U}"/> sets the capacity of the <see cref="System.Collections.Generic.SortedList{T,U}"/> to the default capacity.
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="System.Collections.Generic.SortedList{T,U}.Count"/>.
        /// </remarks>
        public void Clear()
        {
            // clear does not change the capacity
            version++;
            // Don't need to doc this but we clear the elements so that the gc can reclaim the references.
            if (RuntimeHelpers.IsReferenceOrContainsReferences<TKey>())
            {
                Array.Clear(keys, 0, _size);
            }
            if (RuntimeHelpers.IsReferenceOrContainsReferences<TValue>())
            {
                Array.Clear(values, 0, _size);
            }
            _size = 0;
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.IDictionary"/> contains an element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="T:System.Collections.IDictionary"/>.</param>
        /// <returns><see langword="true"/> if the <see cref="T:System.Collections.IDictionary"/> contains an element with the key; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// This method returns <c>false</c> if <c>key</c> is of a type that is not assignable to the key type <c>TKey</c> of the <see cref="System.Collections.Generic.SortedList{T,U}"/>.
        /// This method is an O(log <c>n</c>) operation, where <c>n</c> is <see cref="System.Collections.Generic.SortedList{T,U}.Count"/>.
        /// The following code example shows how to use the <see cref="System.Collections.Generic.SortedList{T,U}.System#Collections#IDictionary#Contains"/> method of the <see cref="System.Collections.IDictionary">IDictionary</see> interface with a <see cref="System.Collections.Generic.SortedList{T,U}"/>. The example demonstrates that the method returns <c>false</c> if a key of the wrong data type is supplied.
        /// The code example is part of a larger example, including output, provided for the <see cref="System.Collections.Generic.SortedList{T,U}.System#Collections#IDictionary#Add"/> method.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/SortedListTKey,TValue/System.Collections.IDictionary.Add/source.cs" id="Snippet31" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/SortedListTKey,TValue/System.Collections.IDictionary.Add/source.vb" id="Snippet31" />
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/SortedListTKey,TValue/System.Collections.IDictionary.Add/source.cs" id="Snippet6" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/SortedListTKey,TValue/System.Collections.IDictionary.Add/source.vb" id="Snippet6" />
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/SortedListTKey,TValue/System.Collections.IDictionary.Add/source.cs" id="Snippet32" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/SortedListTKey,TValue/System.Collections.IDictionary.Add/source.vb" id="Snippet32" />
        /// </remarks>
        bool IDictionary.Contains(object key)
        {
            if (IsCompatibleKey(key))
            {
                return ContainsKey((TKey)key);
            }
            return false;
        }

        // Checks if this sorted list contains an entry with the given key.
        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.SortedList`2"/> contains a specific key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.SortedList`2"/>.</param>
        /// <returns><see langword="true"/> if the <see cref="T:System.Collections.Generic.SortedList`2"/> contains an element with the specified key; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// This method is an O(log <c>n</c>) operation, where <c>n</c> is <see cref="System.Collections.Generic.SortedList{T,U}.Count"/>.
        /// The following code example shows how to use the <see cref="System.Collections.Generic.SortedList{T,U}.ContainsKey"/> method to test whether a key exists prior to calling the <see cref="System.Collections.Generic.SortedList{T,U}.Add"/> method. It also shows how to use the <see cref="System.Collections.Generic.SortedList{T,U}.TryGetValue"/> method to retrieve values, which is an efficient way to retrieve values when a program frequently tries keys that are not in the sorted list. Finally, it shows the least efficient way to test whether keys exist, by using the <see cref="System.Collections.Generic.SortedList{T,U}.Item"/> property (the indexer in C#).
        /// This code example is part of a larger example provided for the <see cref="System.Collections.Generic.SortedList{T,U}"/> class.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/SortedListTKey,TValue/Overview/source.cs" id="Snippet6" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/SortedListTKey,TValue/Overview/source.vb" id="Snippet6" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/SortedListTKey,TValue/Overview/source.fs" id="Snippet6" />
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/SortedListTKey,TValue/Overview/source.cs" id="Snippet5" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/SortedListTKey,TValue/Overview/source.vb" id="Snippet5" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/SortedListTKey,TValue/Overview/source.fs" id="Snippet5" />
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/SortedListTKey,TValue/Overview/source.cs" id="Snippet4" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/SortedListTKey,TValue/Overview/source.vb" id="Snippet4" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/SortedListTKey,TValue/Overview/source.fs" id="Snippet4" />
        /// </remarks>
        public bool ContainsKey(TKey key)
        {
            return IndexOfKey(key) >= 0;
        }

        // Checks if this sorted list contains an entry with the given value. The
        // values of the entries of the sorted list are compared to the given value
        // using the Object.Equals method. This method performs a linear
        // search and is substantially slower than the Contains
        // method.
        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.SortedList`2"/> contains a specific value.
        /// </summary>
        /// <param name="value">The value to locate in the <see cref="T:System.Collections.Generic.SortedList`2"/>. The value can be <see langword="null"/> for reference types.</param>
        /// <returns><see langword="true"/> if the <see cref="T:System.Collections.Generic.SortedList`2"/> contains an element with the specified value; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This method determines equality using the default comparer <see cref="System.Collections.Generic.Comparer{T}.Default">Default</see> for the value type <c>TValue</c>.  <see cref="System.Collections.Generic.Comparer{T}.Default">Default</see> checks whether the value type <c>TValue</c> implements <see cref="System.IComparable{T}">IComparable{T}</see> and uses that implementation, if available.  If not, <see cref="System.Collections.Generic.Comparer{T}.Default">Default</see> checks whether the value type <c>TValue</c> implements <see cref="System.IComparable">IComparable</see>.  If the value type <c>TValue</c> does not implement either interface, this method uses <see cref="System.Object.Equals">Equals</see>.
        /// This method performs a linear search; therefore, the average execution time is proportional to <see cref="System.Collections.Generic.SortedList{T,U}.Count"/>. That is, this method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="System.Collections.Generic.SortedList{T,U}.Count"/>.
        /// </remarks>
        public bool ContainsValue(TValue value)
        {
            return IndexOfValue(value) >= 0;
        }

        // Copies the values in this SortedList to an array.
        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ArgumentNullException.ThrowIfNull(array);

            if (arrayIndex < 0 || arrayIndex > array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), arrayIndex, SR.ArgumentOutOfRange_IndexMustBeLessOrEqual);
            }

            if (array.Length - arrayIndex < Count)
            {
                throw new ArgumentException(SR.Arg_ArrayPlusOffTooSmall);
            }

            for (int i = 0; i < Count; i++)
            {
                KeyValuePair<TKey, TValue> entry = new KeyValuePair<TKey, TValue>(keys[i], values[i]);
                array[arrayIndex + i] = entry;
            }
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.ICollection"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than zero.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="array"/> is multidimensional. -or- <paramref name="array"/> does not have zero-based indexing. -or- The number of elements in the source <see cref="T:System.Collections.ICollection"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>. -or- The type of the source <see cref="T:System.Collections.ICollection"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.</exception>
        /// <remarks>
        /// <note type="note">
        /// If the type of the source <see cref="System.Collections.ICollection"/> cannot be cast automatically to the type of the destination <c>array</c>, the non-generic implementations of <see cref="System.Collections.ICollection.CopyTo">CopyTo</see> throw <see cref="System.InvalidCastException"/>, whereas the generic implementations throw <see cref="System.ArgumentException"/>.
        /// </note>
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="System.Collections.Generic.SortedList{T,U}.Count"/>.
        /// </remarks>
        void ICollection.CopyTo(Array array, int index)
        {
            ArgumentNullException.ThrowIfNull(array);

            if (array.Rank != 1)
            {
                throw new ArgumentException(SR.Arg_RankMultiDimNotSupported, nameof(array));
            }

            if (array.GetLowerBound(0) != 0)
            {
                throw new ArgumentException(SR.Arg_NonZeroLowerBound, nameof(array));
            }

            if (index < 0 || index > array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, SR.ArgumentOutOfRange_IndexMustBeLessOrEqual);
            }

            if (array.Length - index < Count)
            {
                throw new ArgumentException(SR.Arg_ArrayPlusOffTooSmall);
            }

            if (array is KeyValuePair<TKey, TValue>[] keyValuePairArray)
            {
                for (int i = 0; i < Count; i++)
                {
                    keyValuePairArray[i + index] = new KeyValuePair<TKey, TValue>(keys[i], values[i]);
                }
            }
            else
            {
                object[]? objects = array as object[];
                if (objects == null)
                {
                    throw new ArgumentException(SR.Argument_IncompatibleArrayType, nameof(array));
                }

                try
                {
                    for (int i = 0; i < Count; i++)
                    {
                        objects[i + index] = new KeyValuePair<TKey, TValue>(keys[i], values[i]);
                    }
                }
                catch (ArrayTypeMismatchException)
                {
                    throw new ArgumentException(SR.Argument_IncompatibleArrayType, nameof(array));
                }
            }
        }

        // Ensures that the capacity of this sorted list is at least the given
        // minimum value. The capacity is increased to twice the current capacity
        // or to min, whichever is larger.
        private void EnsureCapacity(int min)
        {
            int newCapacity = keys.Length == 0 ? DefaultCapacity : keys.Length * 2;
            // Allow the list to grow to maximum possible capacity (~2G elements) before encountering overflow.
            // Note that this check works even when _items.Length overflowed thanks to the (uint) cast
            if ((uint)newCapacity > Array.MaxLength) newCapacity = Array.MaxLength;
            if (newCapacity < min) newCapacity = min;
            Capacity = newCapacity;
        }

        /// <summary>
        /// Gets the value corresponding to the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the value within the entire <see cref="SortedList{TKey, TValue}"/>.</param>
        /// <returns>The value corresponding to the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The specified index was out of range.</exception>
        public TValue GetValueAtIndex(int index)
        {
            if (index < 0 || index >= _size)
                throw new ArgumentOutOfRangeException(nameof(index), index, SR.ArgumentOutOfRange_IndexMustBeLess);
            return values[index];
        }

        /// <summary>
        /// Updates the value corresponding to the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the value within the entire <see cref="SortedList{TKey, TValue}"/>.</param>
        /// <param name="value">The value with which to replace the entry at the specified index.</param>
        /// <exception cref="ArgumentOutOfRangeException">The specified index was out of range.</exception>
        public void SetValueAtIndex(int index, TValue value)
        {
            if (index < 0 || index >= _size)
                throw new ArgumentOutOfRangeException(nameof(index), index, SR.ArgumentOutOfRange_IndexMustBeLess);
            values[index] = value;
            version++;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="T:System.Collections.Generic.SortedList`2"/>.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.Generic.IEnumerator`1"/> of type <see cref="T:System.Collections.Generic.KeyValuePair`2"/> for the <see cref="T:System.Collections.Generic.SortedList`2"/>.</returns>
        /// <remarks>
        /// The <c>foreach</c> statement of the C# language (<c>For Each</c> in Visual Basic) hides the complexity of the enumerators.  Therefore, using <c>foreach</c> is recommended, instead of directly manipulating the enumerator.
        /// Enumerators can be used to read the data in the collection, but they cannot be used to modify the underlying collection.
        /// The dictionary is maintained in a sorted order using an internal tree. Every new element is positioned at the correct sort position, and the tree is adjusted to maintain the sort order whenever an element is removed. While enumerating, the sort order is maintained.
        /// Initially, the enumerator is positioned before the first element in the collection. At this position, <see cref="System.Collections.Generic.IEnumerator{T}.Current"/> is undefined. Therefore, you must call <see cref="System.Collections.IEnumerator.MoveNext"/> to advance the enumerator to the first element of the collection before reading the value of <see cref="System.Collections.Generic.IEnumerator{T}.Current"/>.
        /// <see cref="System.Collections.Generic.IEnumerator{T}.Current"/> returns the same object until <see cref="System.Collections.IEnumerator.MoveNext"/> is called. <see cref="System.Collections.IEnumerator.MoveNext"/> sets <see cref="System.Collections.Generic.IEnumerator{T}.Current"/> to the next element.
        /// If <see cref="System.Collections.IEnumerator.MoveNext"/> passes the end of the collection, the enumerator is positioned after the last element in the collection and <see cref="System.Collections.IEnumerator.MoveNext"/> returns <c>false</c>. When the enumerator is at this position, subsequent calls to <see cref="System.Collections.IEnumerator.MoveNext"/> return <c>false</c>. If the last call to <see cref="System.Collections.IEnumerator.MoveNext"/> returned <c>false</c>, <see cref="System.Collections.Generic.IEnumerator{T}.Current"/> is undefined. You cannot set <see cref="System.Collections.Generic.IEnumerator{T}.Current"/> to the first element of the collection again; you must create a new enumerator instance instead.
        /// An enumerator remains valid as long as the collection remains unchanged. If changes are made to the collection, such as adding, modifying, or deleting elements, the enumerator is irrecoverably invalidated and the next call to <see cref="System.Collections.IEnumerator.MoveNext"/> or <see cref="System.Collections.IEnumerator.Reset"/> throws an <see cref="System.InvalidOperationException"/>.
        /// The enumerator does not have exclusive access to the collection; therefore, enumerating through a collection is intrinsically not a thread-safe procedure. To guarantee thread safety during enumeration, you can lock the collection during the entire enumeration.  To allow the collection to be accessed by multiple threads for reading and writing, you must implement your own synchronization.
        /// Default implementations of collections in <see cref="System.Collections.Generic">Generic</see> are not synchronized.
        /// This method is an O(1) operation.
        /// </remarks>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => new Enumerator(this, Enumerator.KeyValuePair);

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.</returns>
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() =>
            Count == 0 ? EnumerableHelpers.GetEmptyEnumerator<KeyValuePair<TKey, TValue>>() :
            GetEnumerator();

        /// <summary>
        /// Returns an <see cref="T:System.Collections.IDictionaryEnumerator"/> for the <see cref="T:System.Collections.IDictionary"/>.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IDictionaryEnumerator"/> for the <see cref="T:System.Collections.IDictionary"/>.</returns>
        /// <remarks>
        /// The <c>foreach</c> statement of the C# language (<c>For Each</c> in Visual Basic) hides the complexity of the enumerators.  Therefore, using <c>foreach</c> is recommended, instead of directly manipulating the enumerator.
        /// Enumerators can be used to read the data in the collection, but they cannot be used to modify the underlying collection.
        /// Initially, the enumerator is positioned before the first element in the collection. <see cref="System.Collections.IEnumerator.Reset"/> also brings the enumerator back to this position.  At this position, <see cref="System.Collections.IDictionaryEnumerator.Entry"/> is undefined. Therefore, you must call <see cref="System.Collections.IEnumerator.MoveNext"/> to advance the enumerator to the first element of the collection before reading the value of <see cref="System.Collections.IDictionaryEnumerator.Entry"/>.
        /// <see cref="System.Collections.IDictionaryEnumerator.Entry"/> returns the same object until either <see cref="System.Collections.IEnumerator.MoveNext"/> or <see cref="System.Collections.IEnumerator.Reset"/> is called. <see cref="System.Collections.IEnumerator.MoveNext"/> sets <see cref="System.Collections.IDictionaryEnumerator.Entry"/> to the next element.
        /// If <see cref="System.Collections.IEnumerator.MoveNext"/> passes the end of the collection, the enumerator is positioned after the last element in the collection and <see cref="System.Collections.IEnumerator.MoveNext"/> returns <c>false</c>. When the enumerator is at this position, subsequent calls to <see cref="System.Collections.IEnumerator.MoveNext"/> also return <c>false</c>. If the last call to <see cref="System.Collections.IEnumerator.MoveNext"/> returned <c>false</c>, <see cref="System.Collections.IDictionaryEnumerator.Entry"/> is undefined. To set <see cref="System.Collections.IDictionaryEnumerator.Entry"/> to the first element of the collection again, you can call <see cref="System.Collections.IEnumerator.Reset"/> followed by <see cref="System.Collections.IEnumerator.MoveNext"/>.
        /// An enumerator remains valid as long as the collection remains unchanged. If changes are made to the collection, such as adding, modifying, or deleting elements, the enumerator is irrecoverably invalidated and the next call to <see cref="System.Collections.IEnumerator.MoveNext"/> or <see cref="System.Collections.IEnumerator.Reset"/> throws an <see cref="System.InvalidOperationException"/>.
        /// The enumerator does not have exclusive access to the collection; therefore, enumerating through a collection is intrinsically not a thread-safe procedure.  To guarantee thread safety during enumeration, you can lock the collection during the entire enumeration.  To allow the collection to be accessed by multiple threads for reading and writing, you must implement your own synchronization.
        /// Default implementations of collections in <see cref="System.Collections.Generic">Generic</see> are not synchronized.
        /// This method is an O(1) operation.
        /// The following code example shows how to enumerate the key/value pairs in the sorted list by using the <c>foreach</c> statement (<c>For Each</c> in Visual Basic), which hides the use of the enumerator. In particular, note that the enumerator for the <see cref="System.Collections.IDictionary">IDictionary</see> interface returns <see cref="System.Collections.DictionaryEntry"/> objects rather than <see cref="System.Collections.Generic.KeyValuePair{T,U}"/> objects.
        /// The code example is part of a larger example, including output, provided for the <see cref="System.Collections.Generic.SortedList{T,U}.System#Collections#IDictionary#Add"/> method.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/SortedListTKey,TValue/System.Collections.IDictionary.Add/source.cs" id="Snippet31" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/SortedListTKey,TValue/System.Collections.IDictionary.Add/source.vb" id="Snippet31" />
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/SortedListTKey,TValue/System.Collections.IDictionary.Add/source.cs" id="Snippet7" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/SortedListTKey,TValue/System.Collections.IDictionary.Add/source.vb" id="Snippet7" />
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/SortedListTKey,TValue/System.Collections.IDictionary.Add/source.cs" id="Snippet32" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/SortedListTKey,TValue/System.Collections.IDictionary.Add/source.vb" id="Snippet32" />
        /// </remarks>
        IDictionaryEnumerator IDictionary.GetEnumerator() => new Enumerator(this, Enumerator.DictEntry);

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator"/> that can be used to iterate through the collection.</returns>
        /// <remarks>
        /// The <c>foreach</c> statement of the C# language (<c>For Each</c> in Visual Basic) hides the complexity of the enumerators.  Therefore, using <c>foreach</c> is recommended, instead of directly manipulating the enumerator.
        /// Enumerators can be used to read the data in the collection, but they cannot be used to modify the underlying collection.
        /// Initially, the enumerator is positioned before the first element in the collection. <see cref="System.Collections.IEnumerator.Reset"/> also brings the enumerator back to this position.  At this position, <see cref="System.Collections.IEnumerator.Current"/> is undefined. Therefore, you must call <see cref="System.Collections.IEnumerator.MoveNext"/> to advance the enumerator to the first element of the collection before reading the value of <see cref="System.Collections.IEnumerator.Current"/>.
        /// <see cref="System.Collections.IEnumerator.Current"/> returns the same object until either <see cref="System.Collections.IEnumerator.MoveNext"/> or <see cref="System.Collections.IEnumerator.Reset"/> is called. <see cref="System.Collections.IEnumerator.MoveNext"/> sets <see cref="System.Collections.IEnumerator.Current"/> to the next element.
        /// If <see cref="System.Collections.IEnumerator.MoveNext"/> passes the end of the collection, the enumerator is positioned after the last element in the collection and <see cref="System.Collections.IEnumerator.MoveNext"/> returns <c>false</c>. When the enumerator is at this position, subsequent calls to <see cref="System.Collections.IEnumerator.MoveNext"/> also return <c>false</c>. If the last call to <see cref="System.Collections.IEnumerator.MoveNext"/> returned <c>false</c>, <see cref="System.Collections.IEnumerator.Current"/> is undefined. To set <see cref="System.Collections.IEnumerator.Current"/> to the first element of the collection again, you can call <see cref="System.Collections.IEnumerator.Reset"/> followed by <see cref="System.Collections.IEnumerator.MoveNext"/>.
        /// An enumerator remains valid as long as the collection remains unchanged. If changes are made to the collection, such as adding, modifying, or deleting elements, the enumerator is irrecoverably invalidated and the next call to <see cref="System.Collections.IEnumerator.MoveNext"/> or <see cref="System.Collections.IEnumerator.Reset"/> throws an <see cref="System.InvalidOperationException"/>.
        /// The enumerator does not have exclusive access to the collection; therefore, enumerating through a collection is intrinsically not a thread-safe procedure.  To guarantee thread safety during enumeration, you can lock the collection during the entire enumeration.  To allow the collection to be accessed by multiple threads for reading and writing, you must implement your own synchronization.
        /// Default implementations of collections in <see cref="System.Collections.Generic">Generic</see> are not synchronized.
        /// This method is an O(1) operation.
        /// </remarks>
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<KeyValuePair<TKey, TValue>>)this).GetEnumerator();

        /// <summary>
        /// Gets the key corresponding to the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the key within the entire <see cref="SortedList{TKey, TValue}"/>.</param>
        /// <returns>The key corresponding to the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The specified index is out of range.</exception>
        public TKey GetKeyAtIndex(int index)
        {
            if (index < 0 || index >= _size)
                throw new ArgumentOutOfRangeException(nameof(index), index, SR.ArgumentOutOfRange_IndexMustBeLess);
            return keys[index];
        }

        // Returns the value associated with the given key. If an entry with the
        // given key is not found, the returned value is null.
        public TValue this[TKey key]
        {
            get
            {
                int i = IndexOfKey(key);
                if (i >= 0)
                    return values[i];

                throw new KeyNotFoundException(SR.Format(SR.Arg_KeyNotFoundWithKey, key.ToString()));
            }
            set
            {
                ArgumentNullException.ThrowIfNull(key);

                int i = Array.BinarySearch<TKey>(keys, 0, _size, key, comparer);
                if (i >= 0)
                {
                    values[i] = value;
                    version++;
                    return;
                }
                Insert(~i, key, value);
            }
        }

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <param name="key">The key of the element to get or set.</param>
        /// <value>The element with the specified key, or <see langword="null"/> if <paramref name="key"/> is not in the dictionary or <paramref name="key"/> is of a type that is not assignable to the key type <paramref name="TKey"/> of the <see cref="T:System.Collections.Generic.SortedList`2"/>.</value>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentException">A value is being assigned, and <paramref name="key"/> is of a type that is not assignable to the key type <paramref name="TKey"/> of the <see cref="T:System.Collections.Generic.SortedList`2"/>. -or- A value is being assigned and is of a type that isn't assignable to the value type <paramref name="TValue"/> of the <see cref="T:System.Collections.Generic.SortedList`2"/>.</exception>
        /// <remarks>
        /// This property returns <c>null</c> if <c>key</c> is of a type that is not assignable to the key type <c>TKey</c> of the <see cref="System.Collections.Generic.SortedList{T,U}"/>.
        /// This property provides the ability to access a specific element in the collection by using the following syntax: <c>myCollection[key]</c>.
        /// You can also use the <see cref="System.Collections.IDictionary.Item"/> property to add new elements by setting the value of a key that does not exist in the dictionary; for example, <c>myCollection[&quot;myNonexistentKey&quot;] = myValue</c>. However, if the specified key already exists in the dictionary, setting the <see cref="System.Collections.IDictionary.Item"/> property overwrites the old value. In contrast, the <see cref="System.Collections.IDictionary.Add"/> method does not modify existing elements.
        /// The C# language uses the [this](/dotnet/csharp/language-reference/keywords/this) keyword to define the indexers instead of implementing the <see cref="System.Collections.Generic.SortedList{T,U}.System#Collections#IDictionary#Item"/> property. Visual Basic implements <see cref="System.Collections.Generic.SortedList{T,U}.System#Collections#IDictionary#Item"/> as a default property, which provides the same indexing functionality.
        /// Retrieving the value of this property is an O(log <c>n</c>) operation, where n is <see cref="System.Collections.Generic.SortedList{T,U}.Count"/>. Setting the property is an O(log <c>n</c>) operation if the key is already in the <see cref="System.Collections.Generic.SortedList{T,U}"/>. If the key is not in the list, setting the property is an O(<c>n</c>) operation for unsorted data, or O(log <c>n</c>) if the new element is added at the end of the list. If insertion causes a resize, the operation is O(<c>n</c>).
        /// The following code example shows how to use the <see cref="System.Collections.Generic.SortedList{T,U}.System#Collections#IDictionary#Item"/> property (the indexer in C#) of the <see cref="System.Collections.IDictionary">IDictionary</see> interface with a <see cref="System.Collections.Generic.SortedList{T,U}"/>, and ways the property differs from the <see cref="System.Collections.Generic.SortedList{T,U}.Item">Item</see> property.
        /// The example shows that, like the <see cref="System.Collections.Generic.SortedList{T,U}.Item">Item</see> property, the <see cref="System.Collections.Generic.SortedList{T,U}.System#Collections#IDictionary#Item">System%23Collections%23IDictionary%23Item</see> property can change the value associated with an existing key and can be used to add a new key/value pair if the specified key is not in the sorted list. The example also shows that unlike the <see cref="System.Collections.Generic.SortedList{T,U}.Item">Item</see> property, the <see cref="System.Collections.Generic.SortedList{T,U}.System#Collections#IDictionary#Item">System%23Collections%23IDictionary%23Item</see> property does not throw an exception if <c>key</c> is not in the sorted list, returning a null reference instead. Finally, the example demonstrates that getting the <see cref="System.Collections.Generic.SortedList{T,U}.System#Collections#IDictionary#Item">System%23Collections%23IDictionary%23Item</see> property returns a null reference if <c>key</c> is not the correct data type, and that setting the property throws an exception if <c>key</c> is not the correct data type.
        /// The code example is part of a larger example, including output, provided for the <see cref="System.Collections.Generic.SortedList{T,U}.System#Collections#IDictionary#Add"/> method.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/SortedListTKey,TValue/System.Collections.IDictionary.Add/source.cs" id="Snippet31" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/SortedListTKey,TValue/System.Collections.IDictionary.Add/source.vb" id="Snippet31" />
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/SortedListTKey,TValue/System.Collections.IDictionary.Add/source.cs" id="Snippet3" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/SortedListTKey,TValue/System.Collections.IDictionary.Add/source.vb" id="Snippet3" />
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/SortedListTKey,TValue/System.Collections.IDictionary.Add/source.cs" id="Snippet4" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/SortedListTKey,TValue/System.Collections.IDictionary.Add/source.vb" id="Snippet4" />
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/SortedListTKey,TValue/System.Collections.IDictionary.Add/source.cs" id="Snippet32" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/SortedListTKey,TValue/System.Collections.IDictionary.Add/source.vb" id="Snippet32" />
        /// </remarks>
        object? IDictionary.this[object key]
        {
            get
            {
                if (IsCompatibleKey(key))
                {
                    int i = IndexOfKey((TKey)key);
                    if (i >= 0)
                    {
                        return values[i];
                    }
                }

                return null;
            }
            set
            {
                if (!IsCompatibleKey(key))
                {
                    throw new ArgumentNullException(nameof(key));
                }

                if (default(TValue) != null)
                    ArgumentNullException.ThrowIfNull(value);

                TKey tempKey = (TKey)key;
                try
                {
                    this[tempKey] = (TValue)value!;
                }
                catch (InvalidCastException)
                {
                    throw new ArgumentException(SR.Format(SR.Arg_WrongType, value, typeof(TValue)), nameof(value));
                }
            }
        }

        // Returns the index of the entry with a given key in this sorted list. The
        // key is located through a binary search, and thus the average execution
        // time of this method is proportional to Log2(size), where
        // size is the size of this sorted list. The returned value is -1 if
        // the given key does not occur in this sorted list. Null is an invalid
        // key value.
        /// <summary>
        /// Searches for the specified key and returns the zero-based index within the entire <see cref="T:System.Collections.Generic.SortedList`2"/>.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.SortedList`2"/>.</param>
        /// <returns>The zero-based index of <paramref name="key"/> within the entire <see cref="T:System.Collections.Generic.SortedList`2"/>, if found; otherwise, -1.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// This method performs a binary search; therefore, this method is an O(log <c>n</c>) operation, where <c>n</c> is <see cref="System.Collections.Generic.SortedList{T,U}.Count"/>.
        /// </remarks>
        public int IndexOfKey(TKey key)
        {
            ArgumentNullException.ThrowIfNull(key);

            int ret = Array.BinarySearch<TKey>(keys, 0, _size, key, comparer);
            return ret >= 0 ? ret : -1;
        }

        // Returns the index of the first occurrence of an entry with a given value
        // in this sorted list. The entry is located through a linear search, and
        // thus the average execution time of this method is proportional to the
        // size of this sorted list. The elements of the list are compared to the
        // given value using the Object.Equals method.
        /// <summary>
        /// Searches for the specified value and returns the zero-based index of the first occurrence within the entire <see cref="T:System.Collections.Generic.SortedList`2"/>.
        /// </summary>
        /// <param name="value">The value to locate in the <see cref="T:System.Collections.Generic.SortedList`2"/>. The value can be <see langword="null"/> for reference types.</param>
        /// <returns>The zero-based index of the first occurrence of <paramref name="value"/> within the entire <see cref="T:System.Collections.Generic.SortedList`2"/>, if found; otherwise, -1.</returns>
        /// <remarks>
        /// This method determines equality using the default comparer <see cref="System.Collections.Generic.Comparer{T}.Default">Default</see> for the value type <c>TValue</c>.  <see cref="System.Collections.Generic.Comparer{T}.Default">Default</see> checks whether the value type <c>TValue</c> implements <see cref="System.IComparable{T}">IComparable{T}</see> and uses that implementation, if available.  If not, <see cref="System.Collections.Generic.Comparer{T}.Default">Default</see> checks whether the value type <c>TValue</c> implements <see cref="System.IComparable">IComparable</see>.  If the value type <c>TValue</c> does not implement either interface, this method uses <see cref="System.Object.Equals">Equals</see>.
        /// This method performs a linear search; therefore, the average execution time is proportional to <see cref="System.Collections.Generic.SortedList{T,U}.Count"/>. That is, this method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="System.Collections.Generic.SortedList{T,U}.Count"/>.
        /// </remarks>
        public int IndexOfValue(TValue value)
        {
            return Array.IndexOf(values, value, 0, _size);
        }

        // Inserts an entry with a given key and value at a given index.
        private void Insert(int index, TKey key, TValue value)
        {
            if (_size == keys.Length) EnsureCapacity(_size + 1);
            if (index < _size)
            {
                Array.Copy(keys, index, keys, index + 1, _size - index);
                Array.Copy(values, index, values, index + 1, _size - index);
            }
            keys[index] = key;
            values[index] = value;
            _size++;
            version++;
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</param>
        /// <returns><see langword="true"/> if the <see cref="T:System.Collections.Generic.SortedList`2"/> contains an element with the specified key; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// This method combines the functionality of the <see cref="System.Collections.Generic.SortedList{T,U}.ContainsKey"/> method and the <see cref="System.Collections.Generic.SortedList{T,U}.Item"/> property.
        /// If the key is not found, then the <c>value</c> parameter gets the appropriate default value for the value type <c>TValue</c>; for example, zero (0) for integer types, <c>false</c> for Boolean types, and <c>null</c> for reference types.
        /// This method performs a binary search; therefore, this method is an O(log <c>n</c>) operation, where <c>n</c> is <see cref="System.Collections.Generic.SortedList{T,U}.Count"/>.
        /// The example shows how to use the <see cref="System.Collections.Generic.SortedList{T,U}.TryGetValue"/> method as a more efficient way to retrieve values in a program that frequently tries keys that are not in the sorted list. For contrast, the example also shows how the <see cref="System.Collections.Generic.SortedList{T,U}.Item"/> property (the indexer in C#) throws exceptions when attempting to retrieve nonexistent keys.
        /// This code example is part of a larger example provided for the <see cref="System.Collections.Generic.SortedList{T,U}"/> class.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/SortedListTKey,TValue/Overview/source.cs" id="Snippet5" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/SortedListTKey,TValue/Overview/source.vb" id="Snippet5" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/SortedListTKey,TValue/Overview/source.fs" id="Snippet5" />
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/SortedListTKey,TValue/Overview/source.cs" id="Snippet4" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/SortedListTKey,TValue/Overview/source.vb" id="Snippet4" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/SortedListTKey,TValue/Overview/source.fs" id="Snippet4" />
        /// </remarks>
        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            int i = IndexOfKey(key);
            if (i >= 0)
            {
                value = values[i];
                return true;
            }

            value = default;
            return false;
        }

        // Removes the entry at the given index. The size of the sorted list is
        // decreased by one.
        /// <summary>
        /// Removes the element at the specified index of the <see cref="T:System.Collections.Generic.SortedList`2"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than zero. -or- <paramref name="index"/> is equal to or greater than <see cref="P:System.Collections.Generic.SortedList`2.Count"/>.</exception>
        /// <remarks>
        /// The elements are moved up to fill in the open spot, so this method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="System.Collections.Generic.SortedList{T,U}.Count"/>.
        /// </remarks>
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _size)
                throw new ArgumentOutOfRangeException(nameof(index), index, SR.ArgumentOutOfRange_IndexMustBeLess);
            _size--;
            if (index < _size)
            {
                Array.Copy(keys, index + 1, keys, index, _size - index);
                Array.Copy(values, index + 1, values, index, _size - index);
            }
            if (RuntimeHelpers.IsReferenceOrContainsReferences<TKey>())
            {
                keys[_size] = default(TKey)!;
            }
            if (RuntimeHelpers.IsReferenceOrContainsReferences<TValue>())
            {
                values[_size] = default(TValue)!;
            }
            version++;
        }

        // Removes an entry from this sorted list. If an entry with the specified
        // key exists in the sorted list, it is removed. An ArgumentException is
        // thrown if the key is null.
        /// <summary>
        /// Removes the element with the specified key from the <see cref="T:System.Collections.Generic.SortedList`2"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns><see langword="true"/> if the element is successfully removed; otherwise, <see langword="false"/>. This method also returns <see langword="false"/> if <paramref name="key"/> was not found in the original <see cref="T:System.Collections.Generic.SortedList`2"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// This method performs a binary search; however, the elements are moved up to fill in the open spot, so this method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="System.Collections.Generic.SortedList{T,U}.Count"/>.
        /// The following code example shows how to remove a key/value pair from the sorted list using the <see cref="System.Collections.Generic.SortedList{T,U}.Remove"/> method.
        /// This code example is part of a larger example provided for the <see cref="System.Collections.Generic.SortedList{T,U}"/> class.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/SortedListTKey,TValue/Overview/source.cs" id="Snippet10" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/SortedListTKey,TValue/Overview/source.vb" id="Snippet10" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/SortedListTKey,TValue/Overview/source.fs" id="Snippet10" />
        /// </remarks>
        public bool Remove(TKey key)
        {
            int i = IndexOfKey(key);
            if (i >= 0)
                RemoveAt(i);
            return i >= 0;
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="T:System.Collections.IDictionary"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// This method performs a binary search; however, the elements are moved up to fill in the open spot, so this method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="System.Collections.Generic.SortedList{T,U}.Count"/>.
        /// The following code example shows how to use the <see cref="System.Collections.Generic.SortedList{T,U}.System#Collections#IDictionary#Remove"/> of the <see cref="System.Collections.IDictionary">IDictionary</see> interface with a <see cref="System.Collections.Generic.SortedList{T,U}"/>.
        /// The code example is part of a larger example, including output, provided for the <see cref="System.Collections.Generic.SortedList{T,U}.System#Collections#IDictionary#Add"/> method.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/SortedListTKey,TValue/System.Collections.IDictionary.Add/source.cs" id="Snippet31" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/SortedListTKey,TValue/System.Collections.IDictionary.Add/source.vb" id="Snippet31" />
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/SortedListTKey,TValue/System.Collections.IDictionary.Add/source.cs" id="Snippet10" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/SortedListTKey,TValue/System.Collections.IDictionary.Add/source.vb" id="Snippet10" />
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/SortedListTKey,TValue/System.Collections.IDictionary.Add/source.cs" id="Snippet32" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/SortedListTKey,TValue/System.Collections.IDictionary.Add/source.vb" id="Snippet32" />
        /// </remarks>
        void IDictionary.Remove(object key)
        {
            if (IsCompatibleKey(key))
            {
                Remove((TKey)key);
            }
        }

        // Sets the capacity of this sorted list to the size of the sorted list.
        // This method can be used to minimize a sorted list's memory overhead once
        // it is known that no new elements will be added to the sorted list. To
        // completely clear a sorted list and release all memory referenced by the
        // sorted list, execute the following statements:
        //
        // SortedList.Clear();
        // SortedList.TrimExcess();
        /// <summary>
        /// Sets the capacity to the actual number of elements in the <see cref="T:System.Collections.Generic.SortedList`2"/>, if that number is less than 90 percent of current capacity.
        /// </summary>
        /// <remarks>
        /// This method can be used to minimize a collection's memory overhead if no new elements will be added to the collection. The cost of reallocating and copying a large <see cref="System.Collections.Generic.SortedList{T,U}"/> can be considerable, however, so the <see cref="System.Collections.Generic.SortedList{T,U}.TrimExcess"/> method does nothing if the list is at more than 90 percent of capacity. This avoids incurring a large reallocation cost for a relatively small gain.
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="System.Collections.Generic.SortedList{T,U}.Count"/>.
        /// To reset a <see cref="System.Collections.Generic.SortedList{T,U}"/> to its initial state, call the <see cref="System.Collections.Generic.SortedList{T,U}.Clear"/> method before calling <see cref="System.Collections.Generic.SortedList{T,U}.TrimExcess"/> method. Trimming an empty <see cref="System.Collections.Generic.SortedList{T,U}"/> sets the capacity of the <see cref="System.Collections.Generic.SortedList{T,U}"/> to the default capacity.
        /// The capacity can also be set using the <see cref="System.Collections.Generic.SortedList{T,U}.Capacity"/> property.
        /// </remarks>
        public void TrimExcess()
        {
            int threshold = (int)(keys.Length * 0.9);
            if (_size < threshold)
            {
                Capacity = _size;
            }
        }

        private static bool IsCompatibleKey(object key)
        {
            ArgumentNullException.ThrowIfNull(key);

            return (key is TKey);
        }

        private struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDictionaryEnumerator
        {
            private readonly SortedList<TKey, TValue> _sortedList;
            private TKey? _key;
            private TValue? _value;
            private int _index;
            private readonly int _version;
            private readonly int _getEnumeratorRetType;  // What should Enumerator.Current return?

            internal const int KeyValuePair = 1;
            internal const int DictEntry = 2;

            internal Enumerator(SortedList<TKey, TValue> sortedList, int getEnumeratorRetType)
            {
                _sortedList = sortedList;
                _index = 0;
                _version = _sortedList.version;
                _getEnumeratorRetType = getEnumeratorRetType;
                _key = default;
                _value = default;
            }

            public void Dispose()
            {
                _index = 0;
                _key = default;
                _value = default;
            }

            object IDictionaryEnumerator.Key
            {
                get
                {
                    if (_index == 0 || (_index == _sortedList.Count + 1))
                    {
                        throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);
                    }

                    return _key!;
                }
            }

            public bool MoveNext()
            {
                if (_version != _sortedList.version) throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);

                if ((uint)_index < (uint)_sortedList.Count)
                {
                    _key = _sortedList.keys[_index];
                    _value = _sortedList.values[_index];
                    _index++;
                    return true;
                }

                _index = _sortedList.Count + 1;
                _key = default;
                _value = default;
                return false;
            }

            DictionaryEntry IDictionaryEnumerator.Entry
            {
                get
                {
                    if (_index == 0 || (_index == _sortedList.Count + 1))
                    {
                        throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);
                    }

                    return new DictionaryEntry(_key!, _value);
                }
            }

            public KeyValuePair<TKey, TValue> Current => new KeyValuePair<TKey, TValue>(_key!, _value!);

            object? IEnumerator.Current
            {
                get
                {
                    if (_index == 0 || (_index == _sortedList.Count + 1))
                    {
                        throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);
                    }

                    if (_getEnumeratorRetType == DictEntry)
                    {
                        return new DictionaryEntry(_key!, _value);
                    }
                    else
                    {
                        return new KeyValuePair<TKey, TValue>(_key!, _value!);
                    }
                }
            }

            object? IDictionaryEnumerator.Value
            {
                get
                {
                    if (_index == 0 || (_index == _sortedList.Count + 1))
                    {
                        throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);
                    }

                    return _value;
                }
            }

            void IEnumerator.Reset()
            {
                if (_version != _sortedList.version)
                {
                    throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);
                }

                _index = 0;
                _key = default;
                _value = default;
            }
        }

        private sealed class SortedListKeyEnumerator : IEnumerator<TKey>, IEnumerator
        {
            private readonly SortedList<TKey, TValue> _sortedList;
            private int _index;
            private readonly int _version;
            private TKey? _currentKey;

            internal SortedListKeyEnumerator(SortedList<TKey, TValue> sortedList)
            {
                _sortedList = sortedList;
                _version = sortedList.version;
            }

            public void Dispose()
            {
                _index = 0;
                _currentKey = default;
            }

            public bool MoveNext()
            {
                if (_version != _sortedList.version)
                {
                    throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);
                }

                if ((uint)_index < (uint)_sortedList.Count)
                {
                    _currentKey = _sortedList.keys[_index];
                    _index++;
                    return true;
                }

                _index = _sortedList.Count + 1;
                _currentKey = default;
                return false;
            }

            public TKey Current => _currentKey!;

            object? IEnumerator.Current
            {
                get
                {
                    if (_index == 0 || (_index == _sortedList.Count + 1))
                    {
                        throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);
                    }

                    return _currentKey;
                }
            }

            void IEnumerator.Reset()
            {
                if (_version != _sortedList.version)
                {
                    throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);
                }
                _index = 0;
                _currentKey = default;
            }
        }

        private sealed class SortedListValueEnumerator : IEnumerator<TValue>, IEnumerator
        {
            private readonly SortedList<TKey, TValue> _sortedList;
            private int _index;
            private readonly int _version;
            private TValue? _currentValue;

            internal SortedListValueEnumerator(SortedList<TKey, TValue> sortedList)
            {
                _sortedList = sortedList;
                _version = sortedList.version;
            }

            public void Dispose()
            {
                _index = 0;
                _currentValue = default;
            }

            public bool MoveNext()
            {
                if (_version != _sortedList.version)
                {
                    throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);
                }

                if ((uint)_index < (uint)_sortedList.Count)
                {
                    _currentValue = _sortedList.values[_index];
                    _index++;
                    return true;
                }

                _index = _sortedList.Count + 1;
                _currentValue = default;
                return false;
            }

            public TValue Current => _currentValue!;

            object? IEnumerator.Current
            {
                get
                {
                    if (_index == 0 || (_index == _sortedList.Count + 1))
                    {
                        throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);
                    }

                    return _currentValue;
                }
            }

            void IEnumerator.Reset()
            {
                if (_version != _sortedList.version)
                {
                    throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);
                }
                _index = 0;
                _currentValue = default;
            }
        }

        [DebuggerTypeProxy(typeof(DictionaryKeyCollectionDebugView<,>))]
        [DebuggerDisplay("Count = {Count}")]
        [Serializable]
        public sealed class KeyList : IList<TKey>, ICollection
        {
            private readonly SortedList<TKey, TValue> _dict; // Do not rename (binary serialization)

            internal KeyList(SortedList<TKey, TValue> dictionary)
            {
                _dict = dictionary;
            }

            public int Count
            {
                get { return _dict._size; }
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            bool ICollection.IsSynchronized
            {
                get { return false; }
            }

            object ICollection.SyncRoot
            {
                get { return ((ICollection)_dict).SyncRoot; }
            }

            public void Add(TKey key)
            {
                throw new NotSupportedException(SR.NotSupported_SortedListNestedWrite);
            }

            public void Clear()
            {
                throw new NotSupportedException(SR.NotSupported_SortedListNestedWrite);
            }

            public bool Contains(TKey key)
            {
                return _dict.ContainsKey(key);
            }

            public void CopyTo(TKey[] array, int arrayIndex)
            {
                // defer error checking to Array.Copy
                Array.Copy(_dict.keys, 0, array, arrayIndex, _dict.Count);
            }

            void ICollection.CopyTo(Array array, int arrayIndex)
            {
                if (array != null && array.Rank != 1)
                    throw new ArgumentException(SR.Arg_RankMultiDimNotSupported, nameof(array));

                try
                {
                    // defer error checking to Array.Copy
                    Array.Copy(_dict.keys, 0, array!, arrayIndex, _dict.Count);
                }
                catch (ArrayTypeMismatchException)
                {
                    throw new ArgumentException(SR.Argument_IncompatibleArrayType, nameof(array));
                }
            }

            public void Insert(int index, TKey value)
            {
                throw new NotSupportedException(SR.NotSupported_SortedListNestedWrite);
            }

            public TKey this[int index]
            {
                get
                {
                    return _dict.GetKeyAtIndex(index);
                }
                set
                {
                    throw new NotSupportedException(SR.NotSupported_KeyCollectionSet);
                }
            }

            public IEnumerator<TKey> GetEnumerator() =>
                Count == 0 ? EnumerableHelpers.GetEmptyEnumerator<TKey>() :
                new SortedListKeyEnumerator(_dict);

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public int IndexOf(TKey key)
            {
                ArgumentNullException.ThrowIfNull(key);

                int i = Array.BinarySearch<TKey>(_dict.keys, 0,
                                          _dict.Count, key, _dict.comparer);
                if (i >= 0) return i;
                return -1;
            }

            public bool Remove(TKey key)
            {
                throw new NotSupportedException(SR.NotSupported_SortedListNestedWrite);
                // return false;
            }

            public void RemoveAt(int index)
            {
                throw new NotSupportedException(SR.NotSupported_SortedListNestedWrite);
            }
        }

        [DebuggerTypeProxy(typeof(DictionaryValueCollectionDebugView<,>))]
        [DebuggerDisplay("Count = {Count}")]
        [Serializable]
        public sealed class ValueList : IList<TValue>, ICollection
        {
            private readonly SortedList<TKey, TValue> _dict; // Do not rename (binary serialization)

            internal ValueList(SortedList<TKey, TValue> dictionary)
            {
                _dict = dictionary;
            }

            public int Count
            {
                get { return _dict._size; }
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            bool ICollection.IsSynchronized
            {
                get { return false; }
            }

            object ICollection.SyncRoot
            {
                get { return ((ICollection)_dict).SyncRoot; }
            }

            public void Add(TValue key)
            {
                throw new NotSupportedException(SR.NotSupported_SortedListNestedWrite);
            }

            public void Clear()
            {
                throw new NotSupportedException(SR.NotSupported_SortedListNestedWrite);
            }

            public bool Contains(TValue value)
            {
                return _dict.ContainsValue(value);
            }

            public void CopyTo(TValue[] array, int arrayIndex)
            {
                // defer error checking to Array.Copy
                Array.Copy(_dict.values, 0, array, arrayIndex, _dict.Count);
            }

            void ICollection.CopyTo(Array array, int index)
            {
                if (array != null && array.Rank != 1)
                    throw new ArgumentException(SR.Arg_RankMultiDimNotSupported, nameof(array));

                try
                {
                    // defer error checking to Array.Copy
                    Array.Copy(_dict.values, 0, array!, index, _dict.Count);
                }
                catch (ArrayTypeMismatchException)
                {
                    throw new ArgumentException(SR.Argument_IncompatibleArrayType, nameof(array));
                }
            }

            public void Insert(int index, TValue value)
            {
                throw new NotSupportedException(SR.NotSupported_SortedListNestedWrite);
            }

            public TValue this[int index]
            {
                get
                {
                    return _dict.GetValueAtIndex(index);
                }
                set
                {
                    throw new NotSupportedException(SR.NotSupported_SortedListNestedWrite);
                }
            }

            public IEnumerator<TValue> GetEnumerator() =>
                Count == 0 ? EnumerableHelpers.GetEmptyEnumerator<TValue>() :
                new SortedListValueEnumerator(_dict);

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public int IndexOf(TValue value)
            {
                return Array.IndexOf(_dict.values, value, 0, _dict.Count);
            }

            public bool Remove(TValue value)
            {
                throw new NotSupportedException(SR.NotSupported_SortedListNestedWrite);
                // return false;
            }

            public void RemoveAt(int index)
            {
                throw new NotSupportedException(SR.NotSupported_SortedListNestedWrite);
            }
        }
    }
}

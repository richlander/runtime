// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
    // Implements a variable-size List that uses an array of objects to store the
    // elements. A List has a capacity, which is the allocated length
    // of the internal array. As elements are added to a List, the capacity
    // of the List is automatically increased as required by reallocating the
    // internal array.
    //
    /// <summary>
    /// Represents a strongly typed list of objects that can be accessed by index. Provides methods to search, sort, and manipulate lists.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <remarks>
    /// For more information about this API, see <see href="/dotnet/fundamentals/runtime-libraries/system-collections-generic-list{t}">Supplemental API remarks for List&lt;T&gt;</see>.
    /// </remarks>
    /// <related type="Article" href="/dotnet/standard/globalization-localization/performing-culture-insensitive-string-operations-in-collections">Performing Culture-Insensitive String Operations in Collections</related>
    /// <related type="Article" href="/dotnet/csharp/programming-guide/concepts/iterators">Iterators (C#)</related>
    /// <related type="Article" href="/dotnet/visual-basic/programming-guide/concepts/iterators">Iterators (Visual Basic)</related>
    [DebuggerTypeProxy(typeof(ICollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    [Serializable]
    [TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    public class List<T> : IList<T>, IList, IReadOnlyList<T>
    {
        private const int DefaultCapacity = 4;

        internal T[] _items; // Do not rename (binary serialization)
        internal int _size; // Do not rename (binary serialization)
        internal int _version; // Do not rename (binary serialization)

#pragma warning disable CA1825, IDE0300 // avoid the extra generic instantiation for Array.Empty<T>()
        private static readonly T[] s_emptyArray = new T[0];
#pragma warning restore CA1825, IDE0300

        // Constructs a List. The list is initially empty and has a capacity
        // of zero. Upon adding the first element to the list the capacity is
        // increased to DefaultCapacity, and then increased in multiples of two
        // as required.
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Collections.Generic.List`1"/> class that is empty and has the default initial capacity.
        /// </summary>
        /// <remarks>
        /// The capacity of a <see cref="System.Collections.Generic.List{T}"/> is the number of elements that the <see cref="System.Collections.Generic.List{T}"/> can hold. As elements are added to a <see cref="System.Collections.Generic.List{T}"/>, the capacity is automatically increased as required by reallocating the internal array.
        /// If the size of the collection can be estimated, using the <see cref="System.Collections.Generic.List{T}.#ctor%28System.Int32%29"/> constructor and specifying the initial capacity eliminates the need to perform a number of resizing operations while adding elements to the <see cref="System.Collections.Generic.List{T}"/>.
        /// The capacity can be decreased by calling the <see cref="System.Collections.Generic.List{T}.TrimExcess"/> method or by setting the <see cref="System.Collections.Generic.List{T}.Capacity"/> property explicitly. Decreasing the capacity reallocates memory and copies all the elements in the <see cref="System.Collections.Generic.List{T}"/>.
        /// This constructor is an O(1) operation.
        /// The following example demonstrates the parameterless constructor of the <see cref="System.Collections.Generic.List{T}"/> generic class. The parameterless constructor creates a list with the default capacity, as demonstrated by displaying the <see cref="System.Collections.Generic.List{T}.Capacity"/> property.
        /// The example adds, inserts, and removes items, showing how the capacity changes as these methods are used.
        /// :::code language="csharp" source="~/snippets/csharp/System.Collections.Generic/ListT/Overview/source.cs" interactive="try-dotnet-method" id="Snippet1":::
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ListT/.ctor/source1.vb" id="Snippet1" />
        /// <code lang="fsharp" source="~/snippets/fsharp/VS_Snippets_CLR/List`1_Class/fs/listclass.fs" id="Snippet1" />
        /// </remarks>
        public List()
        {
            _items = s_emptyArray;
        }

        // Constructs a List with a given initial capacity. The list is
        // initially empty, but will have room for the given number of elements
        // before any reallocations are required.
        //
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Collections.Generic.List`1"/> class that is empty and has the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The number of elements that the new list can initially store.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 0.</exception>
        /// <remarks>
        /// The capacity of a <see cref="System.Collections.Generic.List{T}"/> is the number of elements that the <see cref="System.Collections.Generic.List{T}"/> can hold. As elements are added to a <see cref="System.Collections.Generic.List{T}"/>, the capacity is automatically increased as required by reallocating the internal array.
        /// If the size of the collection can be estimated, specifying the initial capacity eliminates the need to perform a number of resizing operations while adding elements to the <see cref="System.Collections.Generic.List{T}"/>.
        /// The capacity can be decreased by calling the <see cref="System.Collections.Generic.List{T}.TrimExcess"/> method or by setting the <see cref="System.Collections.Generic.List{T}.Capacity"/> property explicitly. Decreasing the capacity reallocates memory and copies all the elements in the <see cref="System.Collections.Generic.List{T}"/>.
        /// This constructor is an O(1) operation.
        /// The following example demonstrates the <see cref="System.Collections.Generic.List{T}.#ctor%28System.Int32%29"/> constructor. A <see cref="System.Collections.Generic.List{T}"/> of strings with a capacity of 4 is created, because the ultimate size of the list is known to be exactly 4. The list is populated with four strings, and a read-only copy is created by using the <see cref="System.Collections.Generic.List{T}.AsReadOnly"/> method.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/ListT/.ctor/source.cs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ListT/.ctor/source.vb" id="Snippet1" />
        /// </remarks>
        public List(int capacity)
        {
            if (capacity < 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.capacity, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);

            if (capacity == 0)
                _items = s_emptyArray;
            else
                _items = new T[capacity];
        }

        // Constructs a List, copying the contents of the given collection. The
        // size and capacity of the new list will both be equal to the size of the
        // given collection.
        //
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Collections.Generic.List`1"/> class that contains elements copied from the specified collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="collection"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// The elements are copied onto the <see cref="System.Collections.Generic.List{T}"/> in the same order they are read by the enumerator of the collection.
        /// This constructor is an O(*n*) operation, where *n* is the number of elements in <c>collection</c>.
        /// The following example demonstrates the <see cref="System.Collections.Generic.List{T}.#ctor"/> constructor and various methods of the <see cref="System.Collections.Generic.List{T}"/> class that act on ranges. An array of strings is created and passed to the constructor, populating the list with the elements of the array. The <see cref="System.Collections.Generic.List{T}.Capacity"/> property is then displayed, to show that the initial capacity is exactly what is required to hold the input elements.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/ListT/.ctor/source1.cs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ListT/.ctor/source2.vb" id="Snippet1" />
        /// </remarks>
        public List(IEnumerable<T> collection)
        {
            if (collection == null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.collection);

            if (collection is ICollection<T> c)
            {
                int count = c.Count;
                if (count == 0)
                {
                    _items = s_emptyArray;
                }
                else
                {
                    _items = new T[count];
                    c.CopyTo(_items, 0);
                    _size = count;
                }
            }
            else
            {
                _items = s_emptyArray;
                using (IEnumerator<T> en = collection.GetEnumerator())
                {
                    while (en.MoveNext())
                    {
                        Add(en.Current);
                    }
                }
            }
        }

        // Gets and sets the capacity of this list.  The capacity is the size of
        // the internal array used to hold items.  When set, the internal
        // array of the list is reallocated to the given capacity.
        //
        /// <summary>
        /// Gets or sets the total number of elements the internal data structure can hold without resizing.
        /// </summary>
        /// <value>The number of elements that the <see cref="T:System.Collections.Generic.List`1"/> can contain before resizing is required.</value>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><see cref="P:System.Collections.Generic.List`1.Capacity"/> is set to a value that is less than <see cref="P:System.Collections.Generic.List`1.Count"/>.</exception>
        /// <exception cref="T:System.OutOfMemoryException">There is not enough memory available on the system.</exception>
        /// <remarks>
        /// <see cref="System.Collections.Generic.List{T}.Capacity"/> is the number of elements that the <see cref="System.Collections.Generic.List{T}"/> can store before resizing is required, whereas <see cref="System.Collections.Generic.List{T}.Count"/> is the number of elements that are actually in the <see cref="System.Collections.Generic.List{T}"/>.
        /// <see cref="System.Collections.Generic.List{T}.Capacity"/> is always greater than or equal to <see cref="System.Collections.Generic.List{T}.Count"/>. If <see cref="System.Collections.Generic.List{T}.Count"/> exceeds <see cref="System.Collections.Generic.List{T}.Capacity"/> while adding elements, the capacity is increased by automatically reallocating the internal array before copying the old elements and adding the new elements.
        /// If the capacity is significantly larger than the count and you want to reduce the memory used by the <see cref="System.Collections.Generic.List{T}"/>, you can decrease capacity by calling the <see cref="System.Collections.Generic.List{T}.TrimExcess"/> method or by setting the <see cref="System.Collections.Generic.List{T}.Capacity"/> property explicitly to a lower value. When the value of <see cref="System.Collections.Generic.List{T}.Capacity"/> is set explicitly, the internal array is also reallocated to accommodate the specified capacity, and all the elements are copied.
        /// Retrieving the value of this property is an O(1) operation; setting the property is an O(*n*) operation, where *n* is the new capacity.
        /// The following example demonstrates how to check the capacity and count of a <see cref="System.Collections.Generic.List{T}"/> that contains a simple business object, and illustrates using the <see cref="System.Collections.Generic.List{T}.TrimExcess"/> method to remove extra capacity.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/ListT/Capacity/program.cs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ListT/Capacity/module1.vb" id="Snippet1" />
        /// The following example shows the <see cref="System.Collections.Generic.List{T}.Capacity"/> property at several points in the life of a list. The parameterless constructor is used to create a list of strings with a capacity of 0, and the <see cref="System.Collections.Generic.List{T}.Capacity"/> property is displayed to demonstrate this. After the <see cref="System.Collections.Generic.List{T}.Add"/> method has been used to add several items, the items are listed, and then the <see cref="System.Collections.Generic.List{T}.Capacity"/> property is displayed again, along with the <see cref="System.Collections.Generic.List{T}.Count"/> property, to show that the capacity has been increased as needed.
        /// The <see cref="System.Collections.Generic.List{T}.Capacity"/> property is displayed again after the <see cref="System.Collections.Generic.List{T}.TrimExcess"/> method is used to reduce the capacity to match the count. Finally, the <see cref="System.Collections.Generic.List{T}.Clear"/> method is used to remove all items from the list, and the <see cref="System.Collections.Generic.List{T}.Capacity"/> and <see cref="System.Collections.Generic.List{T}.Count"/> properties are displayed again.
        /// :::code language="csharp" source="~/snippets/csharp/System.Collections.Generic/ListT/Overview/source.cs" interactive="try-dotnet-method" id="Snippet1":::
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ListT/.ctor/source1.vb" id="Snippet1" />
        /// <code lang="fsharp" source="~/snippets/fsharp/VS_Snippets_CLR/List`1_Class/fs/listclass.fs" id="Snippet1" />
        /// </remarks>
        public int Capacity
        {
            get => _items.Length;
            set
            {
                if (value < _size)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.value, ExceptionResource.ArgumentOutOfRange_SmallCapacity);
                }

                if (value != _items.Length)
                {
                    if (value > 0)
                    {
                        T[] newItems = new T[value];
                        if (_size > 0)
                        {
                            Array.Copy(_items, newItems, _size);
                        }
                        _items = newItems;
                    }
                    else
                    {
                        _items = s_emptyArray;
                    }
                }
            }
        }

        // Read-only property describing how many elements are in the List.
        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.List`1"/>.
        /// </summary>
        /// <value>The number of elements contained in the <see cref="T:System.Collections.Generic.List`1"/>.</value>
        /// <remarks>
        /// <see cref="System.Collections.Generic.List{T}.Capacity"/> is the number of elements that the <see cref="System.Collections.Generic.List{T}"/> can store before resizing is required. <see cref="System.Collections.Generic.List{T}.Count"/> is the number of elements that are actually in the <see cref="System.Collections.Generic.List{T}"/>.
        /// <see cref="System.Collections.Generic.List{T}.Capacity"/> is always greater than or equal to <see cref="System.Collections.Generic.List{T}.Count"/>. If <see cref="System.Collections.Generic.List{T}.Count"/> exceeds <see cref="System.Collections.Generic.List{T}.Capacity"/> while adding elements, the capacity is increased by automatically reallocating the internal array before copying the old elements and adding the new elements.
        /// Retrieving the value of this property is an O(1) operation.
        /// The following example demonstrates how to check the capacity and count of a  <see cref="System.Collections.Generic.List{T}"/> that contains a simple business object, and illustrates using the <see cref="System.Collections.Generic.List{T}.TrimExcess"/> method to remove extra capacity.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/ListT/Capacity/program.cs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ListT/Capacity/module1.vb" id="Snippet1" />
        /// The following example shows the value of the <see cref="System.Collections.Generic.List{T}.Count"/> property at various points in the life of a list. After the list has been created and populated and its elements displayed, the <see cref="System.Collections.Generic.List{T}.Capacity"/> and <see cref="System.Collections.Generic.List{T}.Count"/> properties are displayed. These properties are displayed again after the <see cref="System.Collections.Generic.List{T}.TrimExcess"/> method has been called, and again after the contents of the list are cleared.
        /// :::code language="csharp" source="~/snippets/csharp/System.Collections.Generic/ListT/Overview/source.cs" interactive="try-dotnet-method" id="Snippet1":::
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ListT/.ctor/source1.vb" id="Snippet1" />
        /// <code lang="fsharp" source="~/snippets/fsharp/VS_Snippets_CLR/List`1_Class/fs/listclass.fs" id="Snippet1" />
        /// </remarks>
        public int Count => _size;

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.IList"/> has a fixed size.
        /// </summary>
        /// <value><see langword="true"/> if the <see cref="T:System.Collections.IList"/> has a fixed size; otherwise, <see langword="false"/>. In the default implementation of <see cref="T:System.Collections.Generic.List`1"/>, this property always returns <see langword="false"/>.</value>
        /// <remarks>
        /// A collection with a fixed size does not allow the addition or removal of elements after the collection is created, but it allows the modification of existing elements.
        /// A collection with a fixed size is simply a collection with a wrapper that prevents adding and removing elements; therefore, if changes are made to the underlying collection, including the addition or removal of elements, the fixed-size collection reflects those changes.
        /// Retrieving the value of this property is an O(1) operation.
        /// </remarks>
        bool IList.IsFixedSize => false;

        // Is this List read-only?
        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <value><see langword="true"/> if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, <see langword="false"/>. In the default implementation of <see cref="T:System.Collections.Generic.List`1"/>, this property always returns <see langword="false"/>.</value>
        /// <remarks>
        /// A collection that is read-only does not allow the addition, removal, or modification of elements after the collection is created.
        /// A collection that is read-only is simply a collection with a wrapper that prevents modifying the collection; therefore, if changes are made to the underlying collection, the read-only collection reflects those changes.
        /// Retrieving the value of this property is an O(1) operation.
        /// </remarks>
        bool ICollection<T>.IsReadOnly => false;

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.IList"/> is read-only.
        /// </summary>
        /// <value><see langword="true"/> if the <see cref="T:System.Collections.IList"/> is read-only; otherwise, <see langword="false"/>. In the default implementation of <see cref="T:System.Collections.Generic.List`1"/>, this property always returns <see langword="false"/>.</value>
        /// <remarks>
        /// A collection that is read-only does not allow the addition, removal, or modification of elements after the collection is created.
        /// A collection that is read-only is simply a collection with a wrapper that prevents modifying the collection; therefore, if changes are made to the underlying collection, the read-only collection reflects those changes.
        /// Retrieving the value of this property is an O(1) operation.
        /// </remarks>
        bool IList.IsReadOnly => false;

        // Is this List synchronized (thread-safe)?
        /// <summary>
        /// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe).
        /// </summary>
        /// <value><see langword="true"/> if access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe); otherwise, <see langword="false"/>. In the default implementation of <see cref="T:System.Collections.Generic.List`1"/>, this property always returns <see langword="false"/>.</value>
        /// <remarks>
        /// Default implementations of collections in the <see cref="System.Collections.Generic">Generic</see> namespace are not synchronized.
        /// Enumerating through a collection is intrinsically not a thread-safe procedure.  In the rare case where enumeration contends with write accesses, you can lock the collection during the entire enumeration.  To allow the collection to be accessed by multiple threads for reading and writing, you must implement your own synchronization.
        /// <see cref="System.Collections.ICollection.SyncRoot"/> returns an object that can be used to synchronize access to the <see cref="System.Collections.ICollection"/>. Synchronization is effective only if all threads lock this object before accessing the collection.
        /// Retrieving the value of this property is an O(1) operation.
        /// </remarks>
        bool ICollection.IsSynchronized => false;

        // Synchronization root for this object.
        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
        /// </summary>
        /// <value>An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>. In the default implementation of <see cref="T:System.Collections.Generic.List`1"/>, this property always returns the current instance.</value>
        /// <remarks>
        /// <code language="csharp">
        /// </code>
        /// <code language="vb">
        /// </code>
        /// Default implementations of collections in the <see cref="System.Collections.Generic">Generic</see> namespace are not synchronized.
        /// Enumerating through a collection is intrinsically not a thread-safe procedure.  To guarantee thread safety during enumeration, you can lock the collection during the entire enumeration.  To allow the collection to be accessed by multiple threads for reading and writing, you must implement your own synchronization.
        /// <see cref="System.Collections.ICollection.SyncRoot"/> returns an object that can be used to synchronize access to the <see cref="System.Collections.ICollection"/>. Synchronization is effective only if all threads lock this object before accessing the collection. The following code shows the use of the <see cref="System.Collections.ICollection.SyncRoot"/> property.
        /// ODE0 
        /// ODE1 
        /// Retrieving the value of this property is an O(1) operation.
        /// </remarks>
        object ICollection.SyncRoot => this;

        // Sets or Gets the element at the given index.
        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <value>The element at the specified index.</value>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than 0. -or- <paramref name="index"/> is equal to or greater than <see cref="P:System.Collections.Generic.List`1.Count"/>.</exception>
        /// <remarks>
        /// <note type="note">
        /// Visual Basic, C#, and C++ all have syntax for accessing the <see cref="System.Collections.Generic.List{T}.Item"/> property without using its name. Instead, the variable containing the <see cref="System.Collections.Generic.List{T}"/> is used as if it were an array.
        /// </note>
        /// <see cref="System.Collections.Generic.List{T}"/> accepts <c>null</c> as a valid value for reference types and allows duplicate elements.
        /// This property provides the ability to access a specific element in the collection by using the following syntax: <c>myCollection[index]</c>.
        /// Retrieving the value of this property is an O(1) operation; setting the property is also an O(1) operation.
        /// The example in this section demonstrates the <see cref="System.Collections.Generic.List{T}.Item"/> property (the indexer in C#) and various other properties and methods of the <see cref="System.Collections.Generic.List{T}"/> generic class. After the list has been created and populated using the <see cref="System.Collections.Generic.List{T}.Add"/> method, an element is retrieved and displayed using the <see cref="System.Collections.Generic.List{T}.Item"/> property. (For an example that uses the <see cref="System.Collections.Generic.List{T}.Item"/> property to set the value of a list element, see <see cref="System.Collections.Generic.List{T}.AsReadOnly"/>.)
        /// The C# language uses the [<c>this</c>](/dotnet/csharp/language-reference/keywords/this) keyword to define the indexers instead of implementing the <see cref="System.Collections.Generic.List{T}.Item"/> property. Visual Basic implements <see cref="System.Collections.Generic.List{T}.Item"/> as a default property, which provides the same indexing functionality.
        /// :::code language="csharp" source="~/snippets/csharp/System.Collections.Generic/ListT/Overview/source.cs" interactive="try-dotnet-method" id="Snippet2":::
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ListT/.ctor/source1.vb" id="Snippet2" />
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/ListT/Overview/source.cs" id="Snippet3" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ListT/.ctor/source1.vb" id="Snippet3" />
        /// </remarks>
        public T this[int index]
        {
            get
            {
                // Following trick can reduce the range check by one
                if ((uint)index >= (uint)_size)
                {
                    ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessException();
                }
                return _items[index];
            }

            set
            {
                if ((uint)index >= (uint)_size)
                {
                    ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessException();
                }
                _items[index] = value;
                _version++;
            }
        }

        private static bool IsCompatibleObject(object? value)
        {
            // Non-null values are fine.  Only accept nulls if T is a class or Nullable<U>.
            // Note that default(T) is not equal to null for value types except when T is Nullable<U>.
            return (value is T) || (value == null && default(T) == null);
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <value>The element at the specified index.</value>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.IList"/>.</exception>
        /// <exception cref="T:System.ArgumentException">The property is set and the value is of a type that isn't assignable to the <see cref="T:System.Collections.IList"/>.</exception>
        /// <remarks>
        /// The C# language uses the [this](/dotnet/csharp/language-reference/keywords/this) keyword to define the indexers instead of implementing the <see cref="System.Collections.Generic.List{T}.System#Collections#IList#Item"/> property. Visual Basic implements <see cref="System.Collections.Generic.List{T}.System#Collections#IList#Item"/> as a default property, which provides the same indexing functionality.
        /// Retrieving the value of this property is an O(1) operation; setting the property is also an O(1) operation.
        /// </remarks>
        object? IList.this[int index]
        {
            get => this[index];
            set
            {
                ThrowHelper.IfNullAndNullsAreIllegalThenThrow<T>(value, ExceptionArgument.value);

                try
                {
                    this[index] = (T)value!;
                }
                catch (InvalidCastException)
                {
                    ThrowHelper.ThrowWrongValueTypeArgumentException(value, typeof(T));
                }
            }
        }

        // Adds the given object to the end of this list. The size of the list is
        // increased by one. If required, the capacity of the list is doubled
        // before adding the new element.
        //
        /// <summary>
        /// Adds an object to the end of the <see cref="T:System.Collections.Generic.List`1"/>.
        /// </summary>
        /// <param name="item">The object to be added to the end of the <see cref="T:System.Collections.Generic.List`1"/>. The value can be <see langword="null"/> for reference types.</param>
        /// <remarks>
        /// <see cref="System.Collections.Generic.List{T}"/> accepts <c>null</c> as a valid value for reference types and allows duplicate elements.
        /// If <see cref="System.Collections.Generic.List{T}.Count"/> already equals <see cref="System.Collections.Generic.List{T}.Capacity"/>, the capacity of the <see cref="System.Collections.Generic.List{T}"/> is increased by automatically reallocating the internal array, and the existing elements are copied to the new array before the new element is added.
        /// If <see cref="System.Collections.Generic.List{T}.Count"/> is less than <see cref="System.Collections.Generic.List{T}.Capacity"/>, this method is an O(1) operation. If the capacity needs to be increased to accommodate the new element, this method becomes an O(*n*) operation, where *n* is <see cref="System.Collections.Generic.List{T}.Count"/>.
        /// The following example demonstrates how to add, remove, and insert a simple business object in a <see cref="System.Collections.Generic.List{T}"/>.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/ListT/Overview/program.cs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ListT/Add/module1.vb" id="Snippet1" />
        /// <code lang="fsharp" source="~/snippets/fsharp/VS_Snippets_CLR_System/system.collections.generic.list.addremoveinsert/fs/addremoveinsert.fs" id="Snippet1" />
        /// The following example demonstrates several properties and methods of the <see cref="System.Collections.Generic.List{T}"/> generic class, including the <see cref="System.Collections.Generic.List{T}.Add"/> method. The parameterless constructor is used to create a list of strings with a capacity of 0. The <see cref="System.Collections.Generic.List{T}.Capacity"/> property is displayed, and then the <see cref="System.Collections.Generic.List{T}.Add"/> method is used to add several items. The items are listed, and the <see cref="System.Collections.Generic.List{T}.Capacity"/> property is displayed again, along with the <see cref="System.Collections.Generic.List{T}.Count"/> property, to show that the capacity has been increased as needed.
        /// Other properties and methods are used to search for, insert, and remove elements from the list, and finally to clear the list.
        /// :::code language="csharp" source="~/snippets/csharp/System.Collections.Generic/ListT/Overview/source.cs" interactive="try-dotnet-method" id="Snippet1":::
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ListT/.ctor/source1.vb" id="Snippet1" />
        /// <code lang="fsharp" source="~/snippets/fsharp/VS_Snippets_CLR/List`1_Class/fs/listclass.fs" id="Snippet1" />
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T item)
        {
            _version++;
            T[] array = _items;
            int size = _size;
            if ((uint)size < (uint)array.Length)
            {
                _size = size + 1;
                array[size] = item;
            }
            else
            {
                AddWithResize(item);
            }
        }

        // Non-inline from List.Add to improve its code quality as uncommon path
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void AddWithResize(T item)
        {
            Debug.Assert(_size == _items.Length);
            int size = _size;
            Grow(size + 1);
            _size = size + 1;
            _items[size] = item;
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.IList"/>.
        /// </summary>
        /// <param name="item">The <see cref="T:System.Object"/> to add to the <see cref="T:System.Collections.IList"/>.</param>
        /// <returns>The position into which the new element was inserted.</returns>
        /// <exception cref="T:System.ArgumentException"><paramref name="item"/> is of a type that is not assignable to the <see cref="T:System.Collections.IList"/>.</exception>
        /// <remarks>
        /// If <see cref="System.Collections.Generic.List{T}.Count"/> is less than <see cref="System.Collections.Generic.List{T}.Capacity"/>, this method is an O(1) operation. If the capacity needs to be increased to accommodate the new element, this method becomes an O(*n*) operation, where *n* is <see cref="System.Collections.Generic.List{T}.Count"/>.
        /// </remarks>
        int IList.Add(object? item)
        {
            ThrowHelper.IfNullAndNullsAreIllegalThenThrow<T>(item, ExceptionArgument.item);

            try
            {
                Add((T)item!);
            }
            catch (InvalidCastException)
            {
                ThrowHelper.ThrowWrongValueTypeArgumentException(item, typeof(T));
            }

            return Count - 1;
        }

        // Adds the elements of the given collection to the end of this list. If
        // required, the capacity of the list is increased to twice the previous
        // capacity or the new size, whichever is larger.
        //
        /// <summary>
        /// Adds the elements of the specified collection to the end of the <see cref="T:System.Collections.Generic.List`1"/>.
        /// </summary>
        /// <param name="collection">The collection whose elements should be added to the end of the <see cref="T:System.Collections.Generic.List`1"/>. The collection itself cannot be <see langword="null"/>, but it can contain elements that are <see langword="null"/>, if type <paramref name="T"/> is a reference type.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="collection"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// The order of the elements in the collection is preserved in the <see cref="System.Collections.Generic.List{T}"/>.
        /// If the new <see cref="System.Collections.Generic.List{T}.Count"/> (the current <see cref="System.Collections.Generic.List{T}.Count"/> plus the size of the collection) will be greater than <see cref="System.Collections.Generic.List{T}.Capacity"/>, the capacity of the <see cref="System.Collections.Generic.List{T}"/> is increased by automatically reallocating the internal array to accommodate the new elements, and the existing elements are copied to the new array before the new elements are added.
        /// If the <see cref="System.Collections.Generic.List{T}"/> can accommodate the new elements without increasing the <see cref="System.Collections.Generic.List{T}.Capacity"/>, this method is an O(*n*) operation, where *n* is the number of elements to be added. If the capacity needs to be increased to accommodate the new elements, this method becomes an O(*n* + *m*) operation, where *n* is the number of elements to be added and *m* is <see cref="System.Collections.Generic.List{T}.Count"/>.
        /// The following example demonstrates the <see cref="System.Collections.Generic.List{T}.AddRange"/> method and various other methods of the <see cref="System.Collections.Generic.List{T}"/> class that act on ranges. An array of strings is created and passed to the constructor, populating the list with the elements of the array. The <see cref="System.Collections.Generic.List{T}.AddRange"/> method is called, with the list as its argument. The result is that the current elements of the list are added to the end of the list, duplicating all the elements.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/ListT/.ctor/source1.cs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ListT/.ctor/source2.vb" id="Snippet1" />
        /// </remarks>
        public void AddRange(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.collection);
            }

            if (collection is ICollection<T> c)
            {
                int count = c.Count;
                if (count > 0)
                {
                    if (_items.Length - _size < count)
                    {
                        Grow(checked(_size + count));
                    }

                    c.CopyTo(_items, _size);
                    _size += count;
                    _version++;
                }
            }
            else
            {
                using (IEnumerator<T> en = collection.GetEnumerator())
                {
                    while (en.MoveNext())
                    {
                        Add(en.Current);
                    }
                }
            }
        }

        /// <summary>
        /// Returns a read-only <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1"/> wrapper for the current collection.
        /// </summary>
        /// <returns>An object that acts as a read-only wrapper around the current <see cref="T:System.Collections.Generic.List`1"/>.</returns>
        /// <remarks>
        /// To prevent any modifications to the <see cref="System.Collections.Generic.List{T}"/> object, expose it only through this wrapper. A  <see cref="System.Collections.ObjectModel.ReadOnlyCollection{T}"/> object does not expose methods that modify the collection. However, if changes are made to the underlying <see cref="System.Collections.Generic.List{T}"/> object, the read-only collection reflects those changes.
        /// This method is an O(1) operation.
        /// The following example demonstrates the <see cref="System.Collections.Generic.List{T}.AsReadOnly"/> method. A <see cref="System.Collections.Generic.List{T}"/> of strings with a capacity of 4 is created, because the ultimate size of the list is known to be exactly 4. The list is populated with four strings, and the <see cref="System.Collections.Generic.List{T}.AsReadOnly"/> method is used to get a read-only <see cref="System.Collections.Generic.IList{T}"/> generic interface implementation that wraps the original list.
        /// An element of the original list is set to "Coelophysis" using the <see cref="System.Collections.Generic.List{T}.Item"/> property (the indexer in C#), and the contents of the read-only list are displayed again to demonstrate that it is just a wrapper for the original list.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/ListT/.ctor/source.cs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ListT/.ctor/source.vb" id="Snippet1" />
        /// </remarks>
        public ReadOnlyCollection<T> AsReadOnly()
            => new ReadOnlyCollection<T>(this);

        // Searches a section of the list for a given element using a binary search
        // algorithm. Elements of the list are compared to the search value using
        // the given IComparer interface. If comparer is null, elements of
        // the list are compared to the search value using the IComparable
        // interface, which in that case must be implemented by all elements of the
        // list and the given search value. This method assumes that the given
        // section of the list is already sorted; if this is not the case, the
        // result will be incorrect.
        //
        // The method returns the index of the given value in the list. If the
        // list does not contain the given value, the method returns a negative
        // integer. The bitwise complement operator (~) can be applied to a
        // negative result to produce the index of the first element (if any) that
        // is larger than the given search value. This is also the index at which
        // the search value should be inserted into the list in order for the list
        // to remain sorted.
        //
        // The method uses the Array.BinarySearch method to perform the
        // search.
        //
        /// <summary>
        /// Searches a range of elements in the sorted <see cref="T:System.Collections.Generic.List`1"/> for an element using the specified comparer and returns the zero-based index of the element.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range to search.</param>
        /// <param name="count">The length of the range to search.</param>
        /// <param name="item">The object to locate. The value can be <see langword="null"/> for reference types.</param>
        /// <param name="comparer">The <see cref="T:System.Collections.Generic.IComparer`1"/> implementation to use when comparing elements, or <see langword="null"/> to use the default comparer <see cref="P:System.Collections.Generic.Comparer`1.Default"/>.</param>
        /// <returns>The zero-based index of <paramref name="item"/> in the sorted <see cref="T:System.Collections.Generic.List`1"/>, if <paramref name="item"/> is found; otherwise, a negative number that is the bitwise complement of the index of the next element that is larger than <paramref name="item"/> or, if there is no larger element, the bitwise complement of <see cref="P:System.Collections.Generic.List`1.Count"/>.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than 0. -or- <paramref name="count"/> is less than 0.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="index"/> and <paramref name="count"/> do not denote a valid range in the <see cref="T:System.Collections.Generic.List`1"/>.</exception>
        /// <exception cref="T:System.InvalidOperationException"><paramref name="comparer"/> is <see langword="null"/>, and the default comparer <see cref="P:System.Collections.Generic.Comparer`1.Default"/> cannot find an implementation of the <see cref="T:System.IComparable`1"/> generic interface or the <see cref="T:System.IComparable"/> interface for type <paramref name="T"/>.</exception>
        /// <remarks>
        /// The comparer customizes how the elements are compared. For example, you can use a <see cref="System.Collections.CaseInsensitiveComparer"/> instance as the comparer to perform case-insensitive string searches.
        /// If <c>comparer</c> is provided, the elements of the <see cref="System.Collections.Generic.List{T}"/> are compared to the specified value using the specified <see cref="System.Collections.Generic.IComparer{T}"/> implementation.
        /// If <c>comparer</c> is <c>null</c>, the default comparer <see cref="System.Collections.Generic.Comparer{T}.Default">Default</see> checks whether type <c>T</c> implements the <see cref="System.IComparable{T}"/> generic interface and uses that implementation, if available.  If not, <see cref="System.Collections.Generic.Comparer{T}.Default">Default</see> checks whether type <c>T</c> implements the <see cref="System.IComparable"/> interface.  If type <c>T</c> does not implement either interface, <see cref="System.Collections.Generic.Comparer{T}.Default">Default</see> throws <see cref="System.InvalidOperationException"/>.
        /// The <see cref="System.Collections.Generic.List{T}"/> must already be sorted according to the comparer implementation; otherwise, the result is incorrect.
        /// Comparing <c>null</c> with any reference type is allowed and does not generate an exception when using the <see cref="System.IComparable{T}"/> generic interface. When sorting, <c>null</c> is considered to be less than any other object.
        /// If the <see cref="System.Collections.Generic.List{T}"/> contains more than one element with the same value, the method returns only one of the occurrences, and it might return any one of the occurrences, not necessarily the first one.
        /// If the <see cref="System.Collections.Generic.List{T}"/> does not contain the specified value, the method returns a negative integer. You can apply the bitwise complement operation (~) to this negative integer to get the index of the first element that is larger than the search value. When inserting the value into the <see cref="System.Collections.Generic.List{T}"/>, this index should be used as the insertion point to maintain the sort order.
        /// This method is an O(log *n*) operation, where *n* is the number of elements in the range.
        /// The following example demonstrates the <see cref="System.Collections.Generic.List{T}.Sort%28System.Int32%2CSystem.Int32%2CSystem.Collections.Generic.IComparer%7B{}%7D%29"/> method overload and the <see cref="System.Collections.Generic.List{T}.BinarySearch%28System.Int32%2CSystem.Int32%2C{}%2CSystem.Collections.Generic.IComparer%7B{}%7D%29"/> method overload.
        /// The example defines an alternative comparer for strings named DinoCompare, which implements the <c>IComparer&lt;string&gt;</c> (<c>IComparer(Of String)</c> in Visual Basic) generic interface. The comparer works as follows: First, the comparands are tested for <c>null</c>, and a null reference is treated as less than a non-null. Second, the string lengths are compared, and the longer string is deemed to be greater. Third, if the lengths are equal, ordinary string comparison is used.
        /// A <see cref="System.Collections.Generic.List{T}"/> of strings is created and populated with the names of five herbivorous dinosaurs and three carnivorous dinosaurs. Within each of the two groups, the names are not in any particular sort order. The list is displayed, the range of herbivores is sorted using the alternate comparer, and the list is displayed again.
        /// The <see cref="System.Collections.Generic.List{T}.BinarySearch%28System.Int32%2CSystem.Int32%2C{}%2CSystem.Collections.Generic.IComparer%7B{}%7D%29"/> method overload is then used to search only the range of herbivores for "Brachiosaurus". The string is not found, and the bitwise complement (the ~ operator in C#, <c>Xor</c> -1 in Visual Basic) of the negative number returned by the <see cref="System.Collections.Generic.List{T}.BinarySearch%28System.Int32%2CSystem.Int32%2C{}%2CSystem.Collections.Generic.IComparer%7B{}%7D%29"/> method is used as an index for inserting the new string.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/ListT/BinarySearch/source2.cs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ListT/BinarySearch/source2.vb" id="Snippet1" />
        /// </remarks>
        /// <related type="Article" href="/dotnet/standard/globalization-localization/performing-culture-insensitive-string-operations-in-collections">Performing Culture-Insensitive String Operations in Collections</related>
        public int BinarySearch(int index, int count, T item, IComparer<T>? comparer)
        {
            if (index < 0)
                ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException();
            if (count < 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            if (_size - index < count)
                ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);

            return Array.BinarySearch(_items, index, count, item, comparer);
        }

        /// <summary>
        /// Searches the entire sorted <see cref="T:System.Collections.Generic.List`1"/> for an element using the default comparer and returns the zero-based index of the element.
        /// </summary>
        /// <param name="item">The object to locate. The value can be <see langword="null"/> for reference types.</param>
        /// <returns>The zero-based index of <paramref name="item"/> in the sorted <see cref="T:System.Collections.Generic.List`1"/>, if <paramref name="item"/> is found; otherwise, a negative number that is the bitwise complement of the index of the next element that is larger than <paramref name="item"/> or, if there is no larger element, the bitwise complement of <see cref="P:System.Collections.Generic.List`1.Count"/>.</returns>
        /// <exception cref="T:System.InvalidOperationException">The default comparer <see cref="P:System.Collections.Generic.Comparer`1.Default"/> cannot find an implementation of the <see cref="T:System.IComparable`1"/> generic interface or the <see cref="T:System.IComparable"/> interface for type <paramref name="T"/>.</exception>
        /// <remarks>
        /// This method uses the default comparer <see cref="System.Collections.Generic.Comparer{T}.Default">Default</see> for type <c>T</c> to determine the order of list elements. The <see cref="System.Collections.Generic.Comparer{T}.Default">Default</see> property checks whether type <c>T</c> implements the <see cref="System.IComparable{T}"/> generic interface and uses that implementation, if available.  If not, <see cref="System.Collections.Generic.Comparer{T}.Default">Default</see> checks whether type <c>T</c> implements the <see cref="System.IComparable"/> interface.  If type <c>T</c> does not implement either interface, <see cref="System.Collections.Generic.Comparer{T}.Default">Default</see> throws an <see cref="System.InvalidOperationException"/>.
        /// The <see cref="System.Collections.Generic.List{T}"/> must already be sorted according to the comparer implementation; otherwise, the result is incorrect.
        /// Comparing <c>null</c> with any reference type is allowed and does not generate an exception when using the <see cref="System.IComparable{T}"/> generic interface. When sorting, <c>null</c> is considered to be less than any other object.
        /// If the <see cref="System.Collections.Generic.List{T}"/> contains more than one element with the same value, the method returns only one of the occurrences, and it might return any one of the occurrences, not necessarily the first one.
        /// If the <see cref="System.Collections.Generic.List{T}"/> does not contain the specified value, the method returns a negative integer. You can apply the bitwise complement operation (~) to this negative integer to get the index of the first element that is larger than the search value. When inserting the value into the <see cref="System.Collections.Generic.List{T}"/>, this index should be used as the insertion point to maintain the sort order.
        /// This method is an O(log *n*) operation, where *n* is the number of elements in the range.
        /// The following example demonstrates the <see cref="System.Collections.Generic.List{T}.Sort"/> method overload and the <see cref="System.Collections.Generic.List{T}.BinarySearch%28{}%29"/> method overload. A <see cref="System.Collections.Generic.List{T}"/> of strings is created and populated with four strings, in no particular order. The list is displayed, sorted, and displayed again.
        /// The <see cref="System.Collections.Generic.List{T}.BinarySearch%28{}%29"/> method overload is then used to search for two strings that are not in the list, and the <see cref="System.Collections.Generic.List{T}.Insert"/> method is used to insert them. The return value of the <see cref="System.Collections.Generic.List{T}.BinarySearch%28{}%29"/> method is negative in each case, because the strings are not in the list. Taking the bitwise complement (the ~ operator in C#, <c>Xor</c> -1 in Visual Basic) of this negative number produces the index of the first element in the list that is larger than the search string, and inserting at this location preserves the sort order. The second search string is larger than any element in the list, so the insertion position is at the end of the list.
        /// :::code language="csharp" source="~/snippets/csharp/System.Collections.Generic/ListT/BinarySearch/source.cs" interactive="try-dotnet-method" id="Snippet1":::
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ListT/BinarySearch/source.vb" id="Snippet1" />
        /// </remarks>
        /// <related type="Article" href="/dotnet/standard/globalization-localization/performing-culture-insensitive-string-operations-in-collections">Performing Culture-Insensitive String Operations in Collections</related>
        public int BinarySearch(T item)
            => BinarySearch(0, Count, item, null);

        /// <summary>
        /// Searches the entire sorted <see cref="T:System.Collections.Generic.List`1"/> for an element using the specified comparer and returns the zero-based index of the element.
        /// </summary>
        /// <param name="item">The object to locate. The value can be <see langword="null"/> for reference types.</param>
        /// <param name="comparer">The <see cref="T:System.Collections.Generic.IComparer`1"/> implementation to use when comparing elements. -or- <see langword="null"/> to use the default comparer <see cref="P:System.Collections.Generic.Comparer`1.Default"/>.</param>
        /// <returns>The zero-based index of <paramref name="item"/> in the sorted <see cref="T:System.Collections.Generic.List`1"/>, if <paramref name="item"/> is found; otherwise, a negative number that is the bitwise complement of the index of the next element that is larger than <paramref name="item"/> or, if there is no larger element, the bitwise complement of <see cref="P:System.Collections.Generic.List`1.Count"/>.</returns>
        /// <exception cref="T:System.InvalidOperationException"><paramref name="comparer"/> is <see langword="null"/>, and the default comparer <see cref="P:System.Collections.Generic.Comparer`1.Default"/> cannot find an implementation of the <see cref="T:System.IComparable`1"/> generic interface or the <see cref="T:System.IComparable"/> interface for type <paramref name="T"/>.</exception>
        /// <remarks>
        /// The comparer customizes how the elements are compared. For example, you can use a <see cref="System.Collections.CaseInsensitiveComparer"/> instance as the comparer to perform case-insensitive string searches.
        /// If <c>comparer</c> is provided, the elements of the <see cref="System.Collections.Generic.List{T}"/> are compared to the specified value using the specified <see cref="System.Collections.Generic.IComparer{T}"/> implementation.
        /// If <c>comparer</c> is <c>null</c>, the default comparer <see cref="System.Collections.Generic.Comparer{T}.Default">Default</see> checks whether type <c>T</c> implements the <see cref="System.IComparable{T}"/> generic interface and uses that implementation, if available.  If not, <see cref="System.Collections.Generic.Comparer{T}.Default">Default</see> checks whether type <c>T</c> implements the <see cref="System.IComparable"/> interface.  If type <c>T</c> does not implement either interface, <see cref="System.Collections.Generic.Comparer{T}.Default">Default</see> throws <see cref="System.InvalidOperationException"/>.
        /// The <see cref="System.Collections.Generic.List{T}"/> must already be sorted according to the comparer implementation; otherwise, the result is incorrect.
        /// Comparing <c>null</c> with any reference type is allowed and does not generate an exception when using the <see cref="System.IComparable{T}"/> generic interface. When sorting, <c>null</c> is considered to be less than any other object.
        /// If the <see cref="System.Collections.Generic.List{T}"/> contains more than one element with the same value, the method returns only one of the occurrences, and it might return any one of the occurrences, not necessarily the first one.
        /// If the <see cref="System.Collections.Generic.List{T}"/> does not contain the specified value, the method returns a negative integer. You can apply the bitwise complement operation (~) to this negative integer to get the index of the first element that is larger than the search value. When inserting the value into the <see cref="System.Collections.Generic.List{T}"/>, this index should be used as the insertion point to maintain the sort order.
        /// This method is an O(log *n*) operation, where *n* is the number of elements in the range.
        /// The following example demonstrates the <see cref="System.Collections.Generic.List{T}.Sort%28System.Collections.Generic.IComparer%7B{}%7D%29"/> method overload and the <see cref="System.Collections.Generic.List{T}.BinarySearch%28{}%2CSystem.Collections.Generic.IComparer%7B{}%7D%29"/> method overload.
        /// The example defines an alternative comparer for strings named DinoCompare, which implements the <c>IComparer&lt;string&gt;</c> (<c>IComparer(Of String)</c> in Visual Basic) generic interface. The comparer works as follows: First, the comparands are tested for <c>null</c>, and a null reference is treated as less than a non-null. Second, the string lengths are compared, and the longer string is deemed to be greater. Third, if the lengths are equal, ordinary string comparison is used.
        /// A <see cref="System.Collections.Generic.List{T}"/> of strings is created and populated with four strings, in no particular order. The list is displayed, sorted using the alternate comparer, and displayed again.
        /// The <see cref="System.Collections.Generic.List{T}.BinarySearch%28{}%2CSystem.Collections.Generic.IComparer%7B{}%7D%29"/> method overload is then used to search for several strings that are not in the list, employing the alternate comparer. The <see cref="System.Collections.Generic.List{T}.Insert"/> method is used to insert the strings. These two methods are located in the function named <c>SearchAndInsert</c>, along with code to take the bitwise complement (the ~ operator in C#, <c>Xor</c> -1 in Visual Basic) of the negative number returned by <see cref="System.Collections.Generic.List{T}.BinarySearch%28{}%2CSystem.Collections.Generic.IComparer%7B{}%7D%29"/> and use it as an index for inserting the new string.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/ListT/BinarySearch/source1.cs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ListT/BinarySearch/source1.vb" id="Snippet1" />
        /// </remarks>
        /// <related type="Article" href="/dotnet/standard/globalization-localization/performing-culture-insensitive-string-operations-in-collections">Performing Culture-Insensitive String Operations in Collections</related>
        public int BinarySearch(T item, IComparer<T>? comparer)
            => BinarySearch(0, Count, item, comparer);

        // Clears the contents of List.
        /// <summary>
        /// Removes all elements from the <see cref="T:System.Collections.Generic.List`1"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="System.Collections.Generic.List{T}.Count"/> is set to 0, and references to other objects from elements of the collection are also released.
        /// <see cref="System.Collections.Generic.List{T}.Capacity"/> remains unchanged. To reset the capacity of the <see cref="System.Collections.Generic.List{T}"/>, call the <see cref="System.Collections.Generic.List{T}.TrimExcess"/> method or set the <see cref="System.Collections.Generic.List{T}.Capacity"/> property directly. Decreasing the capacity reallocates memory and copies all the elements in the <see cref="System.Collections.Generic.List{T}"/>. Trimming an empty <see cref="System.Collections.Generic.List{T}"/> sets the capacity of the <see cref="System.Collections.Generic.List{T}"/> to the default capacity.
        /// This method is an O(*n*) operation, where *n* is <see cref="System.Collections.Generic.List{T}.Count"/>.
        /// The following example demonstrates the <see cref="System.Collections.Generic.List{T}.Clear"/> method and various other properties and methods of the <see cref="System.Collections.Generic.List{T}"/> generic class. The <see cref="System.Collections.Generic.List{T}.Clear"/> method is used at the end of the program, to remove all items from the list, and the <see cref="System.Collections.Generic.List{T}.Capacity"/> and <see cref="System.Collections.Generic.List{T}.Count"/> properties are then displayed.
        /// :::code language="csharp" source="~/snippets/csharp/System.Collections.Generic/ListT/Overview/source.cs" interactive="try-dotnet-method" id="Snippet1":::
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ListT/.ctor/source1.vb" id="Snippet1" />
        /// <code lang="fsharp" source="~/snippets/fsharp/VS_Snippets_CLR/List`1_Class/fs/listclass.fs" id="Snippet1" />
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            _version++;
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                int size = _size;
                _size = 0;
                if (size > 0)
                {
                    Array.Clear(_items, 0, size); // Clear the elements so that the gc can reclaim the references.
                }
            }
            else
            {
                _size = 0;
            }
        }

        // Contains returns true if the specified element is in the List.
        // It does a linear, O(n) search.  Equality is determined by calling
        // EqualityComparer<T>.Default.Equals().
        //
        /// <summary>
        /// Determines whether an element is in the <see cref="T:System.Collections.Generic.List`1"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.List`1"/>. The value can be <see langword="null"/> for reference types.</param>
        /// <returns><see langword="true"/> if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.List`1"/>; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This method determines equality by using the default equality comparer, as defined by the object's implementation of the <see cref="System.IEquatable{T}.Equals">Equals</see> method for <c>T</c> (the type of values in the list).
        /// This method performs a linear search; therefore, this method is an O(*n*) operation, where *n* is <see cref="System.Collections.Generic.List{T}.Count"/>.
        /// The following example demonstrates the <see cref="System.Collections.Generic.List{T}.Contains"/> and <see cref="System.Collections.Generic.List{T}.Exists"/> methods on a <see cref="System.Collections.Generic.List{T}"/> that contains a simple business object that implements <see cref="System.IEquatable{T}.Equals"/>.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/ListT/Contains/program1.cs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ListT/Contains/module1.vb" id="Snippet1" />
        /// The following example contains a list of complex objects of type <c>Cube</c>. The <c>Cube</c> class implements the <see cref="System.IEquatable{T}.Equals">Equals</see> method so that two cubes are considered equal if their dimensions are the same. In this example, the <see cref="System.Collections.Generic.List{T}.Contains"/> method returns <c>true</c>, because a cube that has the specified dimensions is already in the collection.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/ListT/Contains/program.cs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ListT/Contains/program.vb" id="Snippet1" />
        /// </remarks>
        /// <related type="Article" href="/dotnet/standard/globalization-localization/performing-culture-insensitive-string-operations-in-collections">Performing Culture-Insensitive String Operations in Collections</related>
        public bool Contains(T item)
        {
            // PERF: IndexOf calls Array.IndexOf, which internally
            // calls EqualityComparer<T>.Default.IndexOf, which
            // is specialized for different types. This
            // boosts performance since instead of making a
            // virtual method call each iteration of the loop,
            // via EqualityComparer<T>.Default.Equals, we
            // only make one virtual call to EqualityComparer.IndexOf.

            return _size != 0 && IndexOf(item) >= 0;
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.IList"/> contains a specific value.
        /// </summary>
        /// <param name="item">The <see cref="T:System.Object"/> to locate in the <see cref="T:System.Collections.IList"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="item"/> is found in the <see cref="T:System.Collections.IList"/>; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This method determines equality using the default equality comparer <see cref="System.Collections.Generic.EqualityComparer{T}.Default">Default</see> for <c>T</c>, the type of values in the list.
        /// This method performs a linear search; therefore, this method is an O(*n*) operation, where *n* is <see cref="System.Collections.Generic.List{T}.Count"/>.
        /// </remarks>
        bool IList.Contains(object? item)
        {
            if (IsCompatibleObject(item))
            {
                return Contains((T)item!);
            }
            return false;
        }

        public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
        {
            if (converter == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.converter);
            }

            List<TOutput> list = new List<TOutput>(_size);
            for (int i = 0; i < _size; i++)
            {
                list._items[i] = converter(_items[i]);
            }
            list._size = _size;
            return list;
        }

        // Copies this List into array, which must be of a
        // compatible array type.
        /// <summary>
        /// Copies the entire <see cref="T:System.Collections.Generic.List`1"/> to a compatible one-dimensional array, starting at the beginning of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.List`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentException">The number of elements in the source <see cref="T:System.Collections.Generic.List`1"/> is greater than the number of elements that the destination <paramref name="array"/> can contain.</exception>
        /// <remarks>
        /// This method uses <see cref="System.Array.Copy">Copy</see> to copy the elements.
        /// The elements are copied to the <see cref="System.Array"/> in the same order in which the enumerator iterates through the <see cref="System.Collections.Generic.List{T}"/>.
        /// This method is an O(*n*) operation, where *n* is <see cref="System.Collections.Generic.List{T}.Count"/>.
        /// </remarks>
        public void CopyTo(T[] array)
            => CopyTo(array, 0);

        // Copies this List into array, which must be of a
        // compatible array type.
        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.ICollection"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="array"/> is multidimensional. -or- <paramref name="array"/> does not have zero-based indexing. -or- The number of elements in the source <see cref="T:System.Collections.ICollection"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>. -or- The type of the source <see cref="T:System.Collections.ICollection"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.</exception>
        /// <remarks>
        /// <note type="note">
        /// If the type of the source <see cref="System.Collections.ICollection"/> cannot be cast automatically to the type of the destination <c>array</c>, the nongeneric implementations of <see cref="System.Collections.ICollection.CopyTo">CopyTo</see> throw <see cref="System.InvalidCastException"/>, whereas the generic implementations throw <see cref="System.ArgumentException"/>.
        /// </note>
        /// This method is an O(*n*) operation, where *n* is <see cref="System.Collections.Generic.List{T}.Count"/>.
        /// </remarks>
        void ICollection.CopyTo(Array array, int arrayIndex)
        {
            if ((array != null) && (array.Rank != 1))
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RankMultiDimNotSupported);
            }

            try
            {
                // Array.Copy will check for NULL.
                Array.Copy(_items, 0, array!, arrayIndex, _size);
            }
            catch (ArrayTypeMismatchException)
            {
                ThrowHelper.ThrowArgumentException_Argument_IncompatibleArrayType();
            }
        }

        // Copies a section of this list to the given array at the given index.
        //
        // The method uses the Array.Copy method to copy the elements.
        //
        /// <summary>
        /// Copies a range of elements from the <see cref="T:System.Collections.Generic.List`1"/> to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="index">The zero-based index in the source <see cref="T:System.Collections.Generic.List`1"/> at which copying begins.</param>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.List`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <param name="count">The number of elements to copy.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than 0. -or- <paramref name="arrayIndex"/> is less than 0. -or- <paramref name="count"/> is less than 0.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="index"/> is equal to or greater than the <see cref="P:System.Collections.Generic.List`1.Count"/> of the source <see cref="T:System.Collections.Generic.List`1"/>. -or- The number of elements from <paramref name="index"/> to the end of the source <see cref="T:System.Collections.Generic.List`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.</exception>
        /// <remarks>
        /// This method uses <see cref="System.Array.Copy">Copy</see> to copy the elements.
        /// The elements are copied to the <see cref="System.Array"/> in the same order in which the enumerator iterates through the <see cref="System.Collections.Generic.List{T}"/>.
        /// This method is an O(*n*) operation, where *n* is <c>count</c>.
        /// </remarks>
        public void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
            if (_size - index < count)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);
            }

            // Delegate rest of error checking to Array.Copy.
            Array.Copy(_items, index, array, arrayIndex, count);
        }

        /// <summary>
        /// Copies the entire <see cref="T:System.Collections.Generic.List`1"/> to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.List`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception>
        /// <exception cref="T:System.ArgumentException">The number of elements in the source <see cref="T:System.Collections.Generic.List`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.</exception>
        /// <remarks>
        /// This method uses <see cref="System.Array.Copy">Copy</see> to copy the elements.
        /// The elements are copied to the <see cref="System.Array"/> in the same order in which the enumerator iterates through the <see cref="System.Collections.Generic.List{T}"/>.
        /// This method is an O(*n*) operation, where *n* is <see cref="System.Collections.Generic.List{T}.Count"/>.
        /// </remarks>
        public void CopyTo(T[] array, int arrayIndex)
        {
            // Delegate rest of error checking to Array.Copy.
            Array.Copy(_items, 0, array, arrayIndex, _size);
        }

        /// <summary>
        /// Ensures that the capacity of this list is at least the specified <paramref name="capacity"/>.
        /// If the current capacity of the list is less than specified <paramref name="capacity"/>,
        /// the capacity is increased to at least <paramref name="capacity"/>.
        /// </summary>
        /// <param name="capacity">The minimum capacity to ensure.</param>
        /// <returns>The new capacity of this list.</returns>
        public int EnsureCapacity(int capacity)
        {
            if (capacity < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.capacity, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            }
            if (_items.Length < capacity)
            {
                Grow(capacity);
            }

            return _items.Length;
        }

        /// <summary>
        /// Increase the capacity of this list to at least the specified <paramref name="capacity"/>.
        /// </summary>
        /// <param name="capacity">The minimum capacity to ensure.</param>
        internal void Grow(int capacity)
        {
            Capacity = GetNewCapacity(capacity);
        }

        /// <summary>
        /// Enlarge this list so it may contain at least <paramref name="insertionCount"/> more elements
        /// And copy data to their after-insertion positions.
        /// This method is specifically for insertion, as it avoids 1 extra array copy.
        /// You should only call this method when Count + insertionCount > Capacity.
        /// </summary>
        /// <param name="indexToInsert">Index of the first insertion.</param>
        /// <param name="insertionCount">How many elements will be inserted.</param>
        internal void GrowForInsertion(int indexToInsert, int insertionCount = 1)
        {
            Debug.Assert(insertionCount > 0);

            int requiredCapacity = checked(_size + insertionCount);
            int newCapacity = GetNewCapacity(requiredCapacity);

            // Inline and adapt logic from set_Capacity

            T[] newItems = new T[newCapacity];
            if (indexToInsert != 0)
            {
                Array.Copy(_items, newItems, length: indexToInsert);
            }

            if (_size != indexToInsert)
            {
                Array.Copy(_items, indexToInsert, newItems, indexToInsert + insertionCount, _size - indexToInsert);
            }

            _items = newItems;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetNewCapacity(int capacity)
        {
            Debug.Assert(_items.Length < capacity);

            int newCapacity = _items.Length == 0 ? DefaultCapacity : 2 * _items.Length;

            // Allow the list to grow to maximum possible capacity (~2G elements) before encountering overflow.
            // Note that this check works even when _items.Length overflowed thanks to the (uint) cast
            if ((uint)newCapacity > Array.MaxLength) newCapacity = Array.MaxLength;

            // If the computed capacity is still less than specified, set to the original argument.
            // Capacities exceeding Array.MaxLength will be surfaced as OutOfMemoryException by Array.Resize.
            if (newCapacity < capacity) newCapacity = capacity;

            return newCapacity;
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.List`1"/> contains elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The <see cref="T:System.Predicate`1"/> delegate that defines the conditions of the elements to search for.</param>
        /// <returns><see langword="true"/> if the <see cref="T:System.Collections.Generic.List`1"/> contains one or more elements that match the conditions defined by the specified predicate; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="match"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// <note type="note">
        /// In C# and Visual Basic, it is not necessary to create the <c>Predicate&lt;string&gt;</c> delegate (<c>Predicate(Of String)</c> in Visual Basic) explicitly. These languages infer the correct delegate from context and create it automatically.
        /// </note>
        /// The <see cref="System.Predicate{T}"/> is a delegate to a method that returns <c>true</c> if the object passed to it matches the conditions defined in the delegate.  The elements of the current <see cref="System.Collections.Generic.List{T}"/> are individually passed to the <see cref="System.Predicate{T}"/> delegate, and processing is stopped when a match is found.
        /// This method performs a linear search; therefore, this method is an O(*n*) operation, where *n* is <see cref="System.Collections.Generic.List{T}.Count"/>.
        /// The following example demonstrates the <see cref="System.Collections.Generic.List{T}.Contains"/> and <see cref="System.Collections.Generic.List{T}.Exists"/> methods on a <see cref="System.Collections.Generic.List{T}"/> that contains a simple business object that implements <see cref="System.IEquatable{T}.Equals"/>.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/ListT/Contains/program1.cs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ListT/Contains/module1.vb" id="Snippet1" />
        /// The following example demonstrates the <see cref="System.Collections.Generic.List{T}.Exists"/> method and several other methods that use the <see cref="System.Predicate{T}"/> generic delegate.
        /// A <see cref="System.Collections.Generic.List{T}"/> of strings is created, containing 8 dinosaur names, two of which (at positions 1 and 5) end with "saurus". The example also defines a search predicate method named <c>EndsWithSaurus</c>, which accepts a string parameter and returns a Boolean value indicating whether the input string ends in "saurus".
        /// The <see cref="System.Collections.Generic.List{T}.Find"/>, <see cref="System.Collections.Generic.List{T}.FindLast"/>, and <see cref="System.Collections.Generic.List{T}.FindAll"/> methods are used to search the list with the search predicate method, and then the <see cref="System.Collections.Generic.List{T}.RemoveAll"/> method is used to remove all entries ending with "saurus".
        /// Finally, the <see cref="System.Collections.Generic.List{T}.Exists"/> method is called. It traverses the list from the beginning, passing each element in turn to the <c>EndsWithSaurus</c> method. The search stops and the method returns <c>true</c> if the <c>EndsWithSaurus</c> method returns <c>true</c> for any element. The <see cref="System.Collections.Generic.List{T}.Exists"/> method returns <c>false</c> because all such elements have been removed.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/ListT/Exists/source.cs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ListT/Exists/source.vb" id="Snippet1" />
        /// </remarks>
        public bool Exists(Predicate<T> match)
            => FindIndex(match) != -1;

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the first occurrence within the entire <see cref="T:System.Collections.Generic.List`1"/>.
        /// </summary>
        /// <param name="match">The <see cref="T:System.Predicate`1"/> delegate that defines the conditions of the element to search for.</param>
        /// <returns>The first element that matches the conditions defined by the specified predicate, if found; otherwise, the default value for type <paramref name="T"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="match"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// <note type="important">
        /// When searching a list containing value types, make sure the default value for the type does not satisfy the search predicate. Otherwise, there is no way to distinguish between a default value indicating that no match was found and a list element that happens to have the default value for the type. If the default value satisfies the search predicate, use the <see cref="System.Collections.Generic.List{T}.FindIndex"/> method instead.
        /// </note>
        /// The <see cref="System.Predicate{T}"/> is a delegate to a method that returns <c>true</c> if the object passed to it matches the conditions defined in the delegate.  The elements of the current <see cref="System.Collections.Generic.List{T}"/> are individually passed to the <see cref="System.Predicate{T}"/> delegate, moving forward in the <see cref="System.Collections.Generic.List{T}"/>, starting with the first element and ending with the last element.  Processing is stopped when a match is found.
        /// This method performs a linear search; therefore, this method is an O(*n*) operation, where *n* is <see cref="System.Collections.Generic.List{T}.Count"/>.
        /// The following example demonstrates the <see cref="System.Collections.Generic.List{T}.Find"/> method on a <see cref="System.Collections.Generic.List{T}"/> that contains a simple complex object.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/ListT/Contains/program1.cs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ListT/Contains/module1.vb" id="Snippet1" />
        /// The following example demonstrates the find methods for the <see cref="System.Collections.Generic.List{T}"/> class. The example for the <see cref="System.Collections.Generic.List{T}"/> class contains <c>book</c> objects, of class <c>Book</c>, using the data from the [Sample XML File: Books (LINQ to XML)](/dotnet/standard/linq/sample-xml-file-books). The <c>FillList</c> method in the example uses [LINQ to XML](/dotnet/standard/linq/linq-xml-overview) to parse the values from the XML to property values of the <c>book</c> objects.
        /// The following table describes the examples provided for the find methods.
        /// |Method|Example|
        /// |------------|-------------|
        /// |<see cref="System.Collections.Generic.List{T}.Find%28System.Predicate%7B{}%7D%29"/>|Finds a book by an ID using the <c>IDToFind</c> predicate delegate.<br /><br /> C# example uses an anonymous delegate.|
        /// |<see cref="System.Collections.Generic.List{T}.FindAll%28System.Predicate%7B{}%7D%29"/>|Find all books that whose <c>Genre</c> property is "Computer" using the <c>FindComputer</c> predicate delegate.|
        /// |<see cref="System.Collections.Generic.List{T}.FindLast%28System.Predicate%7B{}%7D%29"/>|Finds the last book in the collection that has a publish date before 2001, using the <c>PubBefore2001</c> predicate delegate.<br /><br /> C# example uses an anonymous delegate.|
        /// |<see cref="System.Collections.Generic.List{T}.FindIndex%28System.Predicate%7B{}%7D%29"/>|Finds the index of first computer book using the <c>FindComputer</c> predicate delegate.|
        /// |<see cref="System.Collections.Generic.List{T}.FindLastIndex%28System.Predicate%7B{}%7D%29"/>|Finds the index of the last computer book using the <c>FindComputer</c> predicate delegate.|
        /// |<see cref="System.Collections.Generic.List{T}.FindIndex%28System.Int32%2CSystem.Int32%2CSystem.Predicate%7B{}%7D%29"/>|Finds the index of first computer book in the second half of the collection, using the <c>FindComputer</c> predicate delegate.|
        /// |<see cref="System.Collections.Generic.List{T}.FindLastIndex%28System.Int32%2CSystem.Int32%2CSystem.Predicate%7B{}%7D%29"/>|Finds the index of last computer book in the second half of the collection, using the <c>FindComputer</c> predicate delegate.|
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/ListT/Find/program.cs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ListT/Find/module1.vb" id="Snippet1" />
        /// </remarks>
        public T? Find(Predicate<T> match)
        {
            if (match == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
            }

            for (int i = 0; i < _size; i++)
            {
                if (match(_items[i]))
                {
                    return _items[i];
                }
            }
            return default;
        }

        /// <summary>
        /// Retrieves all the elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The <see cref="T:System.Predicate`1"/> delegate that defines the conditions of the elements to search for.</param>
        /// <returns>A <see cref="T:System.Collections.Generic.List`1"/> containing all the elements that match the conditions defined by the specified predicate, if found; otherwise, an empty <see cref="T:System.Collections.Generic.List`1"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="match"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// The <see cref="System.Predicate{T}"/> is a delegate to a method that returns <c>true</c> if the object passed to it matches the conditions defined in the delegate.  The elements of the current <see cref="System.Collections.Generic.List{T}"/> are individually passed to the <see cref="System.Predicate{T}"/> delegate, and the elements that match the conditions are saved in the returned <see cref="System.Collections.Generic.List{T}"/>.
        /// This method performs a linear search; therefore, this method is an O(*n*) operation, where *n* is <see cref="System.Collections.Generic.List{T}.Count"/>.
        /// The following example demonstrates the find methods for the <see cref="System.Collections.Generic.List{T}"/> class. The example for the <see cref="System.Collections.Generic.List{T}"/> class contains <c>book</c> objects, of class <c>Book</c>, using the data from the [Sample XML File: Books (LINQ to XML)](/dotnet/standard/linq/sample-xml-file-books). The <c>FillList</c> method in the example uses [LINQ to XML](/dotnet/standard/linq/linq-xml-overview) to parse the values from the XML to property values of the <c>book</c> objects.
        /// The following table describes the examples provided for the find methods.
        /// |Method|Example|
        /// |------------|-------------|
        /// |<see cref="System.Collections.Generic.List{T}.Find%28System.Predicate%7B{}%7D%29"/>|Finds a book by an ID using the <c>IDToFind</c> predicate delegate.<br /><br /> C# example uses an anonymous delegate.|
        /// |<see cref="System.Collections.Generic.List{T}.FindAll%28System.Predicate%7B{}%7D%29"/>|Find all books that whose <c>Genre</c> property is "Computer" using the <c>FindComputer</c> predicate delegate.|
        /// |<see cref="System.Collections.Generic.List{T}.FindLast%28System.Predicate%7B{}%7D%29"/>|Finds the last book in the collection that has a publish date before 2001, using the <c>PubBefore2001</c> predicate delegate.<br /><br /> C# example uses an anonymous delegate.|
        /// |<see cref="System.Collections.Generic.List{T}.FindIndex%28System.Predicate%7B{}%7D%29"/>|Finds the index of first computer book using the <c>FindComputer</c> predicate delegate.|
        /// |<see cref="System.Collections.Generic.List{T}.FindLastIndex%28System.Predicate%7B{}%7D%29"/>|Finds the index of the last computer book using the <c>FindComputer</c> predicate delegate.|
        /// |<see cref="System.Collections.Generic.List{T}.FindIndex%28System.Int32%2CSystem.Int32%2CSystem.Predicate%7B{}%7D%29"/>|Finds the index of first computer book in the second half of the collection, using the <c>FindComputer</c> predicate delegate.|
        /// |<see cref="System.Collections.Generic.List{T}.FindLastIndex%28System.Int32%2CSystem.Int32%2CSystem.Predicate%7B{}%7D%29"/>|Finds the index of last computer book in the second half of the collection, using the <c>FindComputer</c> predicate delegate.|
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/ListT/Find/program.cs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ListT/Find/module1.vb" id="Snippet1" />
        /// </remarks>
        public List<T> FindAll(Predicate<T> match)
        {
            if (match == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
            }

            List<T> list = new List<T>();
            for (int i = 0; i < _size; i++)
            {
                if (match(_items[i]))
                {
                    list.Add(_items[i]);
                }
            }
            return list;
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the first occurrence within the entire <see cref="T:System.Collections.Generic.List`1"/>.
        /// </summary>
        /// <param name="match">The <see cref="T:System.Predicate`1"/> delegate that defines the conditions of the element to search for.</param>
        /// <returns>The zero-based index of the first occurrence of an element that matches the conditions defined by <paramref name="match"/>, if found; otherwise, -1.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="match"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// <code language="csharp">
        /// </code>
        /// <code language="vb">
        /// </code>
        /// <code language="csharp">
        /// </code>
        /// <code language="vb">
        /// </code>
        /// The <see cref="System.Collections.Generic.List{T}"/> is searched forward starting at the first element and ending at the last element.
        /// The <see cref="System.Predicate{T}"/> is a delegate to a method that returns <c>true</c> if the object passed to it matches the conditions defined in the delegate.  The elements of the current <see cref="System.Collections.Generic.List{T}"/> are individually passed to the <see cref="System.Predicate{T}"/> delegate. The delegate has the signature:
        /// ODE0 
        /// ODE1 
        /// This method performs a linear search; therefore, this method is an O(*n*) operation, where *n* is <see cref="System.Collections.Generic.List{T}.Count"/>.
        /// The following example defines an <c>Employee</c> class with two fields, <c>Name</c> and <c>Id</c>. It also defines an <c>EmployeeSearch</c> class with a single method, <c>StartsWith</c>, that indicates whether the <c>Employee.Name</c> field starts with a specified substring that is supplied to the <c>EmployeeSearch</c> class constructor. Note  the signature of this method
        /// ODE2 
        /// ODE3 
        /// corresponds to the signature of the delegate that can be passed to the <see cref="System.Collections.Generic.List{T}.FindIndex"/> method. The example instantiates a <c>List&lt;Employee&gt;</c> object, adds a number of <c>Employee</c> objects to it, and then calls the <see cref="System.Collections.Generic.List{T}.FindIndex%28System.Int32%2CSystem.Int32%2CSystem.Predicate%7B{}%7D%29"/> method twice to search the entire collection, the first time for the first <c>Employee</c> object whose <c>Name</c> field begins with "J", and the second time for the first <c>Employee</c> object whose <c>Name</c> field begins with "Ju".
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/ListT/FindIndex/FindIndex2.cs" id="Snippet2" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ListT/FindIndex/FindIndex2.vb" id="Snippet2" />
        /// </remarks>
        public int FindIndex(Predicate<T> match)
            => FindIndex(0, _size, match);

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the first occurrence within the range of elements in the <see cref="T:System.Collections.Generic.List`1"/> that extends from the specified index to the last element.
        /// </summary>
        /// <param name="startIndex">The zero-based starting index of the search.</param>
        /// <param name="match">The <see cref="T:System.Predicate`1"/> delegate that defines the conditions of the element to search for.</param>
        /// <returns>The zero-based index of the first occurrence of an element that matches the conditions defined by <paramref name="match"/>, if found; otherwise, -1.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="match"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="startIndex"/> is outside the range of valid indexes for the <see cref="T:System.Collections.Generic.List`1"/>.</exception>
        /// <remarks>
        /// <code language="csharp">
        /// </code>
        /// <code language="vb">
        /// </code>
        /// <code language="csharp">
        /// </code>
        /// <code language="vb">
        /// </code>
        /// The <see cref="System.Collections.Generic.List{T}"/> is searched forward starting at <c>startIndex</c> and ending at the last element.
        /// The <see cref="System.Predicate{T}"/> is a delegate to a method that returns <c>true</c> if the object passed to it matches the conditions defined in the delegate.  The elements of the current <see cref="System.Collections.Generic.List{T}"/> are individually passed to the <see cref="System.Predicate{T}"/> delegate. The delegate has the signature:
        /// ODE0 
        /// ODE1 
        /// This method performs a linear search; therefore, this method is an O(*n*) operation, where *n* is the number of elements from <c>startIndex</c> to the end of the <see cref="System.Collections.Generic.List{T}"/>.
        /// The following example defines an <c>Employee</c> class with two fields, <c>Name</c> and <c>Id</c>. It also defines an <c>EmployeeSearch</c> class with a single method, <c>StartsWith</c>, that indicates whether the <c>Employee.Name</c> field starts with a specified substring that is supplied to the <c>EmployeeSearch</c> class constructor. Note  the signature of this method
        /// ODE2 
        /// ODE3 
        /// corresponds to the signature of the delegate that can be passed to the <see cref="System.Collections.Generic.List{T}.FindIndex"/> method. The example instantiates a <c>List&lt;Employee&gt;</c> object, adds a number of <c>Employee</c> objects to it, and then calls the <see cref="System.Collections.Generic.List{T}.FindIndex%28System.Int32%2CSystem.Int32%2CSystem.Predicate%7B{}%7D%29"/> method twice to search the collection starting with its fifth member (that is, the member at index 4). The first time, it searches for the first <c>Employee</c> object whose <c>Name</c> field begins with "J"; the second time, it searches for the first <c>Employee</c> object whose <c>Name</c> field begins with "Ju".
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/ListT/FindIndex/FindIndex3.cs" id="Snippet3" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ListT/FindIndex/FindIndex3.vb" id="Snippet3" />
        /// </remarks>
        public int FindIndex(int startIndex, Predicate<T> match)
            => FindIndex(startIndex, _size - startIndex, match);

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the first occurrence within the range of elements in the <see cref="T:System.Collections.Generic.List`1"/> that starts at the specified index and contains the specified number of elements.
        /// </summary>
        /// <param name="startIndex">The zero-based starting index of the search.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <param name="match">The <see cref="T:System.Predicate`1"/> delegate that defines the conditions of the element to search for.</param>
        /// <returns>The zero-based index of the first occurrence of an element that matches the conditions defined by <paramref name="match"/>, if found; otherwise, -1.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="match"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="startIndex"/> is outside the range of valid indexes for the <see cref="T:System.Collections.Generic.List`1"/>. -or- <paramref name="count"/> is less than 0. -or- <paramref name="startIndex"/> and <paramref name="count"/> do not specify a valid section in the <see cref="T:System.Collections.Generic.List`1"/>.</exception>
        /// <remarks>
        /// <code language="csharp">
        /// </code>
        /// <code language="vb">
        /// </code>
        /// <code language="csharp">
        /// </code>
        /// <code language="vb">
        /// </code>
        /// The <see cref="System.Collections.Generic.List{T}"/> is searched forward starting at <c>startIndex</c> and ending at <c>startIndex</c> plus <c>count</c> minus 1, if <c>count</c> is greater than 0.
        /// The <see cref="System.Predicate{T}"/> is a delegate to a method that returns <c>true</c> if the object passed to it matches the conditions defined in the delegate.  The elements of the current <see cref="System.Collections.Generic.List{T}"/> are individually passed to the <see cref="System.Predicate{T}"/> delegate. The delegate has the signature:
        /// ODE0 
        /// ODE1 
        /// This method performs a linear search; therefore, this method is an O(*n*) operation, where *n* is <c>count</c>.
        /// The following example defines an <c>Employee</c> class with two fields, <c>Name</c> and <c>Id</c>. It also defines an <c>EmployeeSearch</c> class with a single method, <c>StartsWith</c>, that indicates whether the <c>Employee.Name</c> field starts with a specified substring that is supplied to the <c>EmployeeSearch</c> class constructor. Note  the signature of this method
        /// ODE2 
        /// ODE3 
        /// corresponds to the signature of the delegate that can be passed to the <see cref="System.Collections.Generic.List{T}.FindIndex"/> method. The example instantiates a <c>List&lt;Employee&gt;</c> object, adds a number of <c>Employee</c> objects to it, and then calls the <see cref="System.Collections.Generic.List{T}.FindIndex%28System.Int32%2CSystem.Int32%2CSystem.Predicate%7B{}%7D%29"/> method twice to search the entire collection (that is, the members from index 0 to index <see cref="System.Collections.Generic.List{T}.Count"/> - 1). The first time, it searches for the first <c>Employee</c> object whose <c>Name</c> field begins with "J"; the second time, it searches for the first <c>Employee</c> object whose <c>Name</c> field begins with "Ju".
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/ListT/FindIndex/FindIndex1.cs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ListT/FindIndex/FindIndex1.vb" id="Snippet1" />
        /// </remarks>
        public int FindIndex(int startIndex, int count, Predicate<T> match)
        {
            if ((uint)startIndex > (uint)_size)
            {
                ThrowHelper.ThrowStartIndexArgumentOutOfRange_ArgumentOutOfRange_IndexMustBeLessOrEqual();
            }

            if (count < 0 || startIndex > _size - count)
            {
                ThrowHelper.ThrowCountArgumentOutOfRange_ArgumentOutOfRange_Count();
            }

            if (match == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
            }

            int endIndex = startIndex + count;
            for (int i = startIndex; i < endIndex; i++)
            {
                if (match(_items[i])) return i;
            }
            return -1;
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the last occurrence within the entire <see cref="T:System.Collections.Generic.List`1"/>.
        /// </summary>
        /// <param name="match">The <see cref="T:System.Predicate`1"/> delegate that defines the conditions of the element to search for.</param>
        /// <returns>The last element that matches the conditions defined by the specified predicate, if found; otherwise, the default value for type <paramref name="T"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="match"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// <note type="important">
        /// When searching a list containing value types, make sure the default value for the type does not satisfy the search predicate. Otherwise, there is no way to distinguish between a default value indicating that no match was found and a list element that happens to have the default value for the type. If the default value satisfies the search predicate, use the <see cref="System.Collections.Generic.List{T}.FindLastIndex"/> method instead.
        /// </note>
        /// The <see cref="System.Predicate{T}"/> is a delegate to a method that returns <c>true</c> if the object passed to it matches the conditions defined in the delegate.  The elements of the current <see cref="System.Collections.Generic.List{T}"/> are individually passed to the <see cref="System.Predicate{T}"/> delegate, moving backward in the <see cref="System.Collections.Generic.List{T}"/>, starting with the last element and ending with the first element.  Processing is stopped when a match is found.
        /// This method performs a linear search; therefore, this method is an O(*n*) operation, where *n* is <see cref="System.Collections.Generic.List{T}.Count"/>.
        /// The following example demonstrates the find methods for the <see cref="System.Collections.Generic.List{T}"/> class. The example for the <see cref="System.Collections.Generic.List{T}"/> class contains <c>book</c> objects, of class <c>Book</c>, using the data from the [Sample XML File: Books (LINQ to XML)](/dotnet/standard/linq/sample-xml-file-books). The <c>FillList</c> method in the example uses [LINQ to XML](/dotnet/standard/linq/linq-xml-overview) to parse the values from the XML to property values of the <c>book</c> objects.
        /// The following table describes the examples provided for the find methods.
        /// |Method|Example|
        /// |------------|-------------|
        /// |<see cref="System.Collections.Generic.List{T}.Find%28System.Predicate%7B{}%7D%29"/>|Finds a book by an ID using the <c>IDToFind</c> predicate delegate.<br /><br /> C# example uses an anonymous delegate.|
        /// |<see cref="System.Collections.Generic.List{T}.FindAll%28System.Predicate%7B{}%7D%29"/>|Find all books that whose <c>Genre</c> property is "Computer" using the <c>FindComputer</c> predicate delegate.|
        /// |<see cref="System.Collections.Generic.List{T}.FindLast%28System.Predicate%7B{}%7D%29"/>|Finds the last book in the collection that has a publish date before 2001, using the <c>PubBefore2001</c> predicate delegate.<br /><br /> C# example uses an anonymous delegate.|
        /// |<see cref="System.Collections.Generic.List{T}.FindIndex%28System.Predicate%7B{}%7D%29"/>|Finds the index of first computer book using the <c>FindComputer</c> predicate delegate.|
        /// |<see cref="System.Collections.Generic.List{T}.FindLastIndex%28System.Predicate%7B{}%7D%29"/>|Finds the index of the last computer book using the <c>FindComputer</c> predicate delegate.|
        /// |<see cref="System.Collections.Generic.List{T}.FindIndex%28System.Int32%2CSystem.Int32%2CSystem.Predicate%7B{}%7D%29"/>|Finds the index of first computer book in the second half of the collection, using the <c>FindComputer</c> predicate delegate.|
        /// |<see cref="System.Collections.Generic.List{T}.FindLastIndex%28System.Int32%2CSystem.Int32%2CSystem.Predicate%7B{}%7D%29"/>|Finds the index of last computer book in the second half of the collection, using the <c>FindComputer</c> predicate delegate.|
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/ListT/Find/program.cs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ListT/Find/module1.vb" id="Snippet1" />
        /// </remarks>
        public T? FindLast(Predicate<T> match)
        {
            if (match == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
            }

            for (int i = _size - 1; i >= 0; i--)
            {
                if (match(_items[i]))
                {
                    return _items[i];
                }
            }
            return default;
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the last occurrence within the entire <see cref="T:System.Collections.Generic.List`1"/>.
        /// </summary>
        /// <param name="match">The <see cref="T:System.Predicate`1"/> delegate that defines the conditions of the element to search for.</param>
        /// <returns>The zero-based index of the last occurrence of an element that matches the conditions defined by <paramref name="match"/>, if found; otherwise, -1.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="match"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// The <see cref="System.Collections.Generic.List{T}"/> is searched backward starting at the last element and ending at the first element.
        /// The <see cref="System.Predicate{T}"/> is a delegate to a method that returns <c>true</c> if the object passed to it matches the conditions defined in the delegate.  The elements of the current <see cref="System.Collections.Generic.List{T}"/> are individually passed to the <see cref="System.Predicate{T}"/> delegate.
        /// This method performs a linear search; therefore, this method is an O(*n*) operation, where *n* is <see cref="System.Collections.Generic.List{T}.Count"/>.
        /// The following example demonstrates the find methods for the <see cref="System.Collections.Generic.List{T}"/> class. The example for the <see cref="System.Collections.Generic.List{T}"/> class contains <c>book</c> objects, of class <c>Book</c>, using the data from the [Sample XML File: Books (LINQ to XML)](/dotnet/standard/linq/sample-xml-file-books). The <c>FillList</c> method in the example uses [LINQ to XML](/dotnet/standard/linq/linq-xml-overview) to parse the values from the XML to property values of the <c>book</c> objects.
        /// The following table describes the examples provided for the find methods.
        /// |Method|Example|
        /// |------------|-------------|
        /// |<see cref="System.Collections.Generic.List{T}.Find%28System.Predicate%7B{}%7D%29"/>|Finds a book by an ID using the <c>IDToFind</c> predicate delegate.<br /><br /> C# example uses an anonymous delegate.|
        /// |<see cref="System.Collections.Generic.List{T}.FindAll%28System.Predicate%7B{}%7D%29"/>|Find all books that whose <c>Genre</c> property is "Computer" using the <c>FindComputer</c> predicate delegate.|
        /// |<see cref="System.Collections.Generic.List{T}.FindLast%28System.Predicate%7B{}%7D%29"/>|Finds the last book in the collection that has a publish date before 2001, using the <c>PubBefore2001</c> predicate delegate.<br /><br /> C# example uses an anonymous delegate.|
        /// |<see cref="System.Collections.Generic.List{T}.FindIndex%28System.Predicate%7B{}%7D%29"/>|Finds the index of first computer book using the <c>FindComputer</c> predicate delegate.|
        /// |<see cref="System.Collections.Generic.List{T}.FindLastIndex%28System.Predicate%7B{}%7D%29"/>|Finds the index of the last computer book using the <c>FindComputer</c> predicate delegate.|
        /// |<see cref="System.Collections.Generic.List{T}.FindIndex%28System.Int32%2CSystem.Int32%2CSystem.Predicate%7B{}%7D%29"/>|Finds the index of first computer book in the second half of the collection, using the <c>FindComputer</c> predicate delegate.|
        /// |<see cref="System.Collections.Generic.List{T}.FindLastIndex%28System.Int32%2CSystem.Int32%2CSystem.Predicate%7B{}%7D%29"/>|Finds the index of last computer book in the second half of the collection, using the <c>FindComputer</c> predicate delegate.|
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/ListT/Find/program.cs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ListT/Find/module1.vb" id="Snippet1" />
        /// </remarks>
        public int FindLastIndex(Predicate<T> match)
            => FindLastIndex(_size - 1, _size, match);

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the last occurrence within the range of elements in the <see cref="T:System.Collections.Generic.List`1"/> that extends from the first element to the specified index.
        /// </summary>
        /// <param name="startIndex">The zero-based starting index of the backward search.</param>
        /// <param name="match">The <see cref="T:System.Predicate`1"/> delegate that defines the conditions of the element to search for.</param>
        /// <returns>The zero-based index of the last occurrence of an element that matches the conditions defined by <paramref name="match"/>, if found; otherwise, -1.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="match"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="startIndex"/> is outside the range of valid indexes for the <see cref="T:System.Collections.Generic.List`1"/>.</exception>
        /// <remarks>
        /// The <see cref="System.Collections.Generic.List{T}"/> is searched backward starting at <c>startIndex</c> and ending at the first element.
        /// The <see cref="System.Predicate{T}"/> is a delegate to a method that returns <c>true</c> if the object passed to it matches the conditions defined in the delegate.  The elements of the current <see cref="System.Collections.Generic.List{T}"/> are individually passed to the <see cref="System.Predicate{T}"/> delegate.
        /// This method performs a linear search; therefore, this method is an O(*n*) operation, where *n* is the number of elements from the beginning of the <see cref="System.Collections.Generic.List{T}"/> to <c>startIndex</c>.
        /// </remarks>
        public int FindLastIndex(int startIndex, Predicate<T> match)
            => FindLastIndex(startIndex, startIndex + 1, match);

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the last occurrence within the range of elements in the <see cref="T:System.Collections.Generic.List`1"/> that contains the specified number of elements and ends at the specified index.
        /// </summary>
        /// <param name="startIndex">The zero-based starting index of the backward search.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <param name="match">The <see cref="T:System.Predicate`1"/> delegate that defines the conditions of the element to search for.</param>
        /// <returns>The zero-based index of the last occurrence of an element that matches the conditions defined by <paramref name="match"/>, if found; otherwise, -1.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="match"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="startIndex"/> is outside the range of valid indexes for the <see cref="T:System.Collections.Generic.List`1"/>. -or- <paramref name="count"/> is less than 0. -or- <paramref name="startIndex"/> and <paramref name="count"/> do not specify a valid section in the <see cref="T:System.Collections.Generic.List`1"/>.</exception>
        /// <remarks>
        /// The <see cref="System.Collections.Generic.List{T}"/> is searched backward starting at <c>startIndex</c> and ending at <c>startIndex</c> minus <c>count</c> plus 1, if <c>count</c> is greater than 0.
        /// The <see cref="System.Predicate{T}"/> is a delegate to a method that returns <c>true</c> if the object passed to it matches the conditions defined in the delegate.  The elements of the current <see cref="System.Collections.Generic.List{T}"/> are individually passed to the <see cref="System.Predicate{T}"/> delegate.
        /// This method performs a linear search; therefore, this method is an O(*n*) operation, where *n* is <c>count</c>.
        /// The following example demonstrates the find methods for the <see cref="System.Collections.Generic.List{T}"/> class. The example for the <see cref="System.Collections.Generic.List{T}"/> class contains <c>book</c> objects, of class <c>Book</c>, using the data from the [Sample XML File: Books (LINQ to XML)](/dotnet/standard/linq/sample-xml-file-books). The <c>FillList</c> method in the example uses [LINQ to XML](/dotnet/standard/linq/linq-xml-overview) to parse the values from the XML to property values of the <c>book</c> objects.
        /// The following table describes the examples provided for the find methods.
        /// |Method|Example|
        /// |------------|-------------|
        /// |<see cref="System.Collections.Generic.List{T}.Find%28System.Predicate%7B{}%7D%29"/>|Finds a book by an ID using the <c>IDToFind</c> predicate delegate.<br /><br /> C# example uses an anonymous delegate.|
        /// |<see cref="System.Collections.Generic.List{T}.FindAll%28System.Predicate%7B{}%7D%29"/>|Find all books that whose <c>Genre</c> property is "Computer" using the <c>FindComputer</c> predicate delegate.|
        /// |<see cref="System.Collections.Generic.List{T}.FindLast%28System.Predicate%7B{}%7D%29"/>|Finds the last book in the collection that has a publish date before 2001, using the <c>PubBefore2001</c> predicate delegate.<br /><br /> C# example uses an anonymous delegate.|
        /// |<see cref="System.Collections.Generic.List{T}.FindIndex%28System.Predicate%7B{}%7D%29"/>|Finds the index of first computer book using the <c>FindComputer</c> predicate delegate.|
        /// |<see cref="System.Collections.Generic.List{T}.FindLastIndex%28System.Predicate%7B{}%7D%29"/>|Finds the index of the last computer book using the <c>FindComputer</c> predicate delegate.|
        /// |<see cref="System.Collections.Generic.List{T}.FindIndex%28System.Int32%2CSystem.Int32%2CSystem.Predicate%7B{}%7D%29"/>|Finds the index of first computer book in the second half of the collection, using the <c>FindComputer</c> predicate delegate.|
        /// |<see cref="System.Collections.Generic.List{T}.FindLastIndex%28System.Int32%2CSystem.Int32%2CSystem.Predicate%7B{}%7D%29"/>|Finds the index of last computer book in the second half of the collection, using the <c>FindComputer</c> predicate delegate.|
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/ListT/Find/program.cs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ListT/Find/module1.vb" id="Snippet1" />
        /// </remarks>
        public int FindLastIndex(int startIndex, int count, Predicate<T> match)
        {
            if (match == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
            }

            if (_size == 0)
            {
                // Special case for 0 length List
                if (startIndex != -1)
                {
                    ThrowHelper.ThrowStartIndexArgumentOutOfRange_ArgumentOutOfRange_IndexMustBeLess();
                }
            }
            else
            {
                // Make sure we're not out of range
                if ((uint)startIndex >= (uint)_size)
                {
                    ThrowHelper.ThrowStartIndexArgumentOutOfRange_ArgumentOutOfRange_IndexMustBeLess();
                }
            }

            // 2nd have of this also catches when startIndex == MAXINT, so MAXINT - 0 + 1 == -1, which is < 0.
            if (count < 0 || startIndex - count + 1 < 0)
            {
                ThrowHelper.ThrowCountArgumentOutOfRange_ArgumentOutOfRange_Count();
            }

            int endIndex = startIndex - count;
            for (int i = startIndex; i > endIndex; i--)
            {
                if (match(_items[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Performs the specified action on each element of the <see cref="T:System.Collections.Generic.List`1"/>.
        /// </summary>
        /// <param name="action">The <see cref="T:System.Action`1"/> delegate to perform on each element of the <see cref="T:System.Collections.Generic.List`1"/>.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="action"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.InvalidOperationException">An element in the collection has been modified.</exception>
        /// <remarks>
        /// <note type="note">
        /// In addition to displaying the contents using the <c>Print</c> method, the C# example demonstrates the use of [anonymous methods](/dotnet/csharp/programming-guide/statements-expressions-operators/anonymous-methods) to display the results to the console.
        /// </note>
        /// The <see cref="System.Action{T}"/> is a delegate to a method that performs an action on the object passed to it.  The elements of the current <see cref="System.Collections.Generic.List{T}"/> are individually passed to the <see cref="System.Action{T}"/> delegate.
        /// This method is an O(*n*) operation, where *n* is <see cref="System.Collections.Generic.List{T}.Count"/>.
        /// Modifying the underlying collection in the body of the <see cref="System.Action{T}"/> delegate is not supported and causes undefined behavior.
        /// The following example demonstrates the use of the <see cref="System.Action{T}"/> delegate to print the contents of a <see cref="System.Collections.Generic.List{T}"/> object. In this example the <c>Print</c> method is used to display the contents of the list to the console.
        /// :::code language="csharp" source="~/snippets/csharp/System/ActionT/Overview/action.cs" interactive="try-dotnet-method" id="Snippet01":::
        /// <code lang="vb" source="~/snippets/visualbasic/System/ActionT/Overview/action.vb" id="Snippet01" />
        /// </remarks>
        public void ForEach(Action<T> action)
        {
            if (action == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.action);
            }

            int version = _version;

            for (int i = 0; i < _size; i++)
            {
                if (version != _version)
                {
                    break;
                }
                action(_items[i]);
            }

            if (version != _version)
                ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion();
        }

        // Returns an enumerator for this list with the given
        // permission for removal of elements. If modifications made to the list
        // while an enumeration is in progress, the MoveNext and
        // GetObject methods of the enumerator will throw an exception.
        //
        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="T:System.Collections.Generic.List`1"/>.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.List`1.Enumerator"/> for the <see cref="T:System.Collections.Generic.List`1"/>.</returns>
        /// <remarks>
        /// The <c>foreach</c> statement of the C# language (<c>For Each</c> in Visual Basic) hides the complexity of the enumerators. Therefore, using <c>foreach</c> is recommended, instead of directly manipulating the enumerator.
        /// Enumerators can be used to read the data in the collection, but they cannot be used to modify the underlying collection.
        /// Initially, the enumerator is positioned before the first element in the collection. At this position, the <see cref="System.Collections.Generic.List{T}.Enumerator.Current"/> property is undefined. Therefore, you must call the <see cref="System.Collections.Generic.List{T}.Enumerator.MoveNext"/> method to advance the enumerator to the first element of the collection before reading the value of <see cref="System.Collections.Generic.List{T}.Enumerator.Current"/>.
        /// The <see cref="System.Collections.Generic.List{T}.Enumerator.Current"/> property returns the same object until <see cref="System.Collections.Generic.List{T}.Enumerator.MoveNext"/> is called. <see cref="System.Collections.Generic.List{T}.Enumerator.MoveNext"/> sets <see cref="System.Collections.Generic.List{T}.Enumerator.Current"/> to the next element.
        /// If <see cref="System.Collections.Generic.List{T}.Enumerator.MoveNext"/> passes the end of the collection, the enumerator is positioned after the last element in the collection and <see cref="System.Collections.Generic.List{T}.Enumerator.MoveNext"/> returns <c>false</c>. When the enumerator is at this position, subsequent calls to <see cref="System.Collections.Generic.List{T}.Enumerator.MoveNext"/> also return <c>false</c>. If the last call to <see cref="System.Collections.Generic.List{T}.Enumerator.MoveNext"/> returned <c>false</c>, <see cref="System.Collections.Generic.List{T}.Enumerator.Current"/> is undefined. You cannot set <see cref="System.Collections.Generic.List{T}.Enumerator.Current"/> to the first element of the collection again; you must create a new enumerator instance instead.
        /// An enumerator remains valid as long as the collection remains unchanged. If changes are made to the collection, such as adding, modifying, or deleting elements, the enumerator is irrecoverably invalidated and the next call to <see cref="System.Collections.Generic.List{T}.Enumerator.MoveNext"/> or <see cref="System.Collections.Generic.List{T}.Enumerator.System#Collections#IEnumerator#Reset"/> throws an <see cref="System.InvalidOperationException"/>.
        /// The enumerator does not have exclusive access to the collection; therefore, enumerating through a collection is intrinsically not a thread-safe procedure. To guarantee thread safety during enumeration, you can lock the collection during the entire enumeration.  To allow the collection to be accessed by multiple threads for reading and writing, you must implement your own synchronization.
        /// Default implementations of collections in the <see cref="System.Collections.Generic">Generic</see> namespace are not synchronized.
        /// This method is an O(1) operation.
        /// </remarks>
        public Enumerator GetEnumerator() => new Enumerator(this);

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.</returns>
        /// <remarks>
        /// The <c>foreach</c> statement of the C# language (<c>For Each</c> in Visual Basic) hides the complexity of the enumerators. Therefore, using <c>foreach</c> is recommended, instead of directly manipulating the enumerator.
        /// Enumerators can be used to read the data in the collection, but they cannot be used to modify the underlying collection.
        /// Initially, the enumerator is positioned before the first element in the collection. At this position, the <see cref="System.Collections.Generic.IEnumerator{T}.Current"/> property is undefined. Therefore, you must call the <see cref="System.Collections.IEnumerator.MoveNext"/> method to advance the enumerator to the first element of the collection before reading the value of <see cref="System.Collections.Generic.IEnumerator{T}.Current"/>.
        /// The <see cref="System.Collections.Generic.IEnumerator{T}.Current"/> property returns the same object until <see cref="System.Collections.IEnumerator.MoveNext"/> is called. <see cref="System.Collections.IEnumerator.MoveNext"/> sets <see cref="System.Collections.Generic.IEnumerator{T}.Current"/> to the next element.
        /// If <see cref="System.Collections.IEnumerator.MoveNext"/> passes the end of the collection, the enumerator is positioned after the last element in the collection and <see cref="System.Collections.IEnumerator.MoveNext"/> returns <c>false</c>. When the enumerator is at this position, subsequent calls to <see cref="System.Collections.IEnumerator.MoveNext"/> also return <c>false</c>. If the last call to <see cref="System.Collections.IEnumerator.MoveNext"/> returned <c>false</c>, <see cref="System.Collections.Generic.IEnumerator{T}.Current"/> is undefined. You cannot set <see cref="System.Collections.Generic.IEnumerator{T}.Current"/> to the first element of the collection again; you must create a new enumerator instance instead.
        /// An enumerator remains valid as long as the collection remains unchanged. If changes are made to the collection, such as adding, modifying, or deleting elements, the enumerator is irrecoverably invalidated and the next call to <see cref="System.Collections.IEnumerator.MoveNext"/> or <see cref="System.Collections.IEnumerator.Reset"/> throws an <see cref="System.InvalidOperationException"/>.
        /// The enumerator does not have exclusive access to the collection; therefore, enumerating through a collection is intrinsically not a thread-safe procedure. To guarantee thread safety during enumeration, you can lock the collection during the entire enumeration.  To allow the collection to be accessed by multiple threads for reading and writing, you must implement your own synchronization.
        /// Default implementations of collections in the <see cref="System.Collections.Generic">Generic</see> namespace are not synchronized.
        /// This method is an O(1) operation.
        /// </remarks>
        IEnumerator<T> IEnumerable<T>.GetEnumerator() =>
            Count == 0 ? SZGenericArrayEnumerator<T>.Empty :
            GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator"/> that can be used to iterate through the collection.</returns>
        /// <remarks>
        /// The <c>foreach</c> statement of the C# language (<c>For Each</c> in Visual Basic) hides the complexity of the enumerators. Therefore, using <c>foreach</c> is recommended, instead of directly manipulating the enumerator.
        /// Enumerators can be used to read the data in the collection, but they cannot be used to modify the underlying collection.
        /// Initially, the enumerator is positioned before the first element in the collection. <see cref="System.Collections.IEnumerator.Reset"/> also brings the enumerator back to this position.  At this position, the <see cref="System.Collections.IEnumerator.Current"/> property is undefined. Therefore, you must call the <see cref="System.Collections.IEnumerator.MoveNext"/> method to advance the enumerator to the first element of the collection before reading the value of <see cref="System.Collections.IEnumerator.Current"/>.
        /// The <see cref="System.Collections.IEnumerator.Current"/> property returns the same object until either <see cref="System.Collections.IEnumerator.MoveNext"/> or <see cref="System.Collections.IEnumerator.Reset"/> is called. <see cref="System.Collections.IEnumerator.MoveNext"/> sets <see cref="System.Collections.IEnumerator.Current"/> to the next element.
        /// If <see cref="System.Collections.IEnumerator.MoveNext"/> passes the end of the collection, the enumerator is positioned after the last element in the collection and <see cref="System.Collections.IEnumerator.MoveNext"/> returns <c>false</c>. When the enumerator is at this position, subsequent calls to <see cref="System.Collections.IEnumerator.MoveNext"/> also return <c>false</c>. If the last call to <see cref="System.Collections.IEnumerator.MoveNext"/> returned <c>false</c>, <see cref="System.Collections.IEnumerator.Current"/> is undefined. To set <see cref="System.Collections.IEnumerator.Current"/> to the first element of the collection again, you can call <see cref="System.Collections.IEnumerator.Reset"/> followed by <see cref="System.Collections.IEnumerator.MoveNext"/>.
        /// An enumerator remains valid as long as the collection remains unchanged. If changes are made to the collection, such as adding, modifying, or deleting elements, the enumerator is irrecoverably invalidated and the next call to <see cref="System.Collections.IEnumerator.MoveNext"/> or <see cref="System.Collections.IEnumerator.Reset"/> throws an <see cref="System.InvalidOperationException"/>.
        /// The enumerator does not have exclusive access to the collection; therefore, enumerating through a collection is intrinsically not a thread-safe procedure.  To guarantee thread safety during enumeration, you can lock the collection during the entire enumeration.  To allow the collection to be accessed by multiple threads for reading and writing, you must implement your own synchronization.
        /// Default implementations of collections in the <see cref="System.Collections.Generic">Generic</see> namespace are not synchronized.
        /// This method is an O(1) operation.
        /// </remarks>
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)this).GetEnumerator();

        /// <summary>
        /// Creates a shallow copy of a range of elements in the source <see cref="T:System.Collections.Generic.List`1"/>.
        /// </summary>
        /// <param name="index">The zero-based <see cref="T:System.Collections.Generic.List`1"/> index at which the range starts.</param>
        /// <param name="count">The number of elements in the range.</param>
        /// <returns>A shallow copy of a range of elements in the source <see cref="T:System.Collections.Generic.List`1"/>.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than 0. -or- <paramref name="count"/> is less than 0.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="index"/> and <paramref name="count"/> do not denote a valid range of elements in the <see cref="T:System.Collections.Generic.List`1"/>.</exception>
        /// <remarks>
        /// A shallow copy of a collection of reference types, or a subset of that collection, contains only the references to the elements of the collection. The objects themselves are not copied. The references in the new list point to the same objects as the references in the original list.
        /// A shallow copy of a collection of value types, or a subset of that collection, contains the elements of the collection. However, if the elements of the collection contain references to other objects, those objects are not copied. The references in the elements of the new collection point to the same objects as the references in the elements of the original collection.
        /// In contrast, a deep copy of a collection copies the elements and everything directly or indirectly referenced by the elements.
        /// This method is an O(*n*) operation, where *n* is <c>count</c>.
        /// The following example demonstrates the <see cref="System.Collections.Generic.List{T}.GetRange"/> method and other methods of the <see cref="System.Collections.Generic.List{T}"/> class that act on ranges. At the end of the example, the <see cref="System.Collections.Generic.List{T}.GetRange"/> method is used to get three items from the list, beginning with index location 2. The <see cref="System.Collections.Generic.List{T}.ToArray"/> method is called on the resulting <see cref="System.Collections.Generic.List{T}"/>, creating an array of three elements. The elements of the array are displayed.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/ListT/.ctor/source1.cs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ListT/.ctor/source2.vb" id="Snippet1" />
        /// </remarks>
        public List<T> GetRange(int index, int count)
        {
            if (index < 0)
            {
                ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException();
            }

            if (count < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            }

            if (_size - index < count)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);
            }

            List<T> list = new List<T>(count);
            Array.Copy(_items, index, list._items, 0, count);
            list._size = count;
            return list;
        }

        /// <summary>
        /// Creates a shallow copy of a range of elements in the source <see cref="List{T}" />.
        /// </summary>
        /// <param name="start">The zero-based <see cref="List{T}" /> index at which the range starts.</param>
        /// <param name="length">The length of the range.</param>
        /// <returns>A shallow copy of a range of elements in the source <see cref="List{T}" />.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="start" /> is less than 0.
        /// -or-
        /// <paramref name="length" /> is less than 0.
        /// </exception>
        /// <exception cref="ArgumentException"><paramref name="start" /> and <paramref name="length" /> do not denote a valid range of elements in the <see cref="List{T}" />.</exception>
        public List<T> Slice(int start, int length) => GetRange(start, length);

        // Returns the index of the first occurrence of a given value in a range of
        // this list. The list is searched forwards from beginning to end.
        // The elements of the list are compared to the given value using the
        // Object.Equals method.
        //
        // This method uses the Array.IndexOf method to perform the
        // search.
        //
        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the entire <see cref="T:System.Collections.Generic.List`1"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.List`1"/>. The value can be <see langword="null"/> for reference types.</param>
        /// <returns>The zero-based index of the first occurrence of <paramref name="item"/> within the entire <see cref="T:System.Collections.Generic.List`1"/>, if found; otherwise, -1.</returns>
        /// <remarks>
        /// The <see cref="System.Collections.Generic.List{T}"/> is searched forward starting at the first element and ending at the last element.
        /// This method determines equality using the default equality comparer <see cref="System.Collections.Generic.EqualityComparer{T}.Default">Default</see> for <c>T</c>, the type of values in the list.
        /// This method performs a linear search; therefore, this method is an O(*n*) operation, where *n* is <see cref="System.Collections.Generic.List{T}.Count"/>.
        /// </remarks>
        /// <related type="Article" href="/dotnet/standard/globalization-localization/performing-culture-insensitive-string-operations-in-collections">Performing Culture-Insensitive String Operations in Collections</related>
        public int IndexOf(T item)
            => Array.IndexOf(_items, item, 0, _size);

        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.IList"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.IList"/>.</param>
        /// <returns>The index of <paramref name="item"/> if found in the list; otherwise, -1.</returns>
        /// <exception cref="T:System.ArgumentException"><paramref name="item"/> is of a type that is not assignable to the <see cref="T:System.Collections.IList"/>.</exception>
        /// <remarks>
        /// This method determines equality using the default equality comparer <see cref="System.Collections.Generic.EqualityComparer{T}.Default">Default</see> for <c>T</c>, the type of values in the list.
        /// This method performs a linear search; therefore, this method is an O(*n*) operation, where *n* is <see cref="System.Collections.Generic.List{T}.Count"/>.
        /// </remarks>
        int IList.IndexOf(object? item)
        {
            if (IsCompatibleObject(item))
            {
                return IndexOf((T)item!);
            }
            return -1;
        }

        // Returns the index of the first occurrence of a given value in a range of
        // this list. The list is searched forwards, starting at index
        // index and ending at count number of elements. The
        // elements of the list are compared to the given value using the
        // Object.Equals method.
        //
        // This method uses the Array.IndexOf method to perform the
        // search.
        //
        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the range of elements in the <see cref="T:System.Collections.Generic.List`1"/> that extends from the specified index to the last element.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.List`1"/>. The value can be <see langword="null"/> for reference types.</param>
        /// <param name="index">The zero-based starting index of the search. 0 (zero) is valid in an empty list.</param>
        /// <returns>The zero-based index of the first occurrence of <paramref name="item"/> within the range of elements in the <see cref="T:System.Collections.Generic.List`1"/> that extends from <paramref name="index"/> to the last element, if found; otherwise, -1.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is outside the range of valid indexes for the <see cref="T:System.Collections.Generic.List`1"/>.</exception>
        /// <remarks>
        /// The <see cref="System.Collections.Generic.List{T}"/> is searched forward starting at <c>index</c> and ending at the last element.
        /// This method determines equality using the default equality comparer <see cref="System.Collections.Generic.EqualityComparer{T}.Default">Default</see> for <c>T</c>, the type of values in the list.
        /// This method performs a linear search; therefore, this method is an O(*n*) operation, where *n* is the number of elements from <c>index</c> to the end of the <see cref="System.Collections.Generic.List{T}"/>.
        /// </remarks>
        /// <related type="Article" href="/dotnet/standard/globalization-localization/performing-culture-insensitive-string-operations-in-collections">Performing Culture-Insensitive String Operations in Collections</related>
        public int IndexOf(T item, int index)
        {
            if (index > _size)
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException();
            return Array.IndexOf(_items, item, index, _size - index);
        }

        // Returns the index of the first occurrence of a given value in a range of
        // this list. The list is searched forwards, starting at index
        // index and upto count number of elements. The
        // elements of the list are compared to the given value using the
        // Object.Equals method.
        //
        // This method uses the Array.IndexOf method to perform the
        // search.
        //
        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the range of elements in the <see cref="T:System.Collections.Generic.List`1"/> that starts at the specified index and contains the specified number of elements.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.List`1"/>. The value can be <see langword="null"/> for reference types.</param>
        /// <param name="index">The zero-based starting index of the search. 0 (zero) is valid in an empty list.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <returns>The zero-based index of the first occurrence of <paramref name="item"/> within the range of elements in the <see cref="T:System.Collections.Generic.List`1"/> that starts at <paramref name="index"/> and contains <paramref name="count"/> number of elements, if found; otherwise, -1.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is outside the range of valid indexes for the <see cref="T:System.Collections.Generic.List`1"/>. -or- <paramref name="count"/> is less than 0. -or- <paramref name="index"/> and <paramref name="count"/> do not specify a valid section in the <see cref="T:System.Collections.Generic.List`1"/>.</exception>
        /// <remarks>
        /// The <see cref="System.Collections.Generic.List{T}"/> is searched forward starting at <c>index</c> and ending at <c>index</c> plus <c>count</c> minus 1, if <c>count</c> is greater than 0.
        /// This method determines equality using the default equality comparer <see cref="System.Collections.Generic.EqualityComparer{T}.Default">Default</see> for <c>T</c>, the type of values in the list.
        /// This method performs a linear search; therefore, this method is an O(*n*) operation, where *n* is <c>count</c>.
        /// </remarks>
        /// <related type="Article" href="/dotnet/standard/globalization-localization/performing-culture-insensitive-string-operations-in-collections">Performing Culture-Insensitive String Operations in Collections</related>
        public int IndexOf(T item, int index, int count)
        {
            if (index > _size)
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException();

            if (count < 0 || index > _size - count)
                ThrowHelper.ThrowCountArgumentOutOfRange_ArgumentOutOfRange_Count();

            return Array.IndexOf(_items, item, index, count);
        }

        // Inserts an element into this list at a given index. The size of the list
        // is increased by one. If required, the capacity of the list is doubled
        // before inserting the new element.
        //
        /// <summary>
        /// Inserts an element into the <see cref="T:System.Collections.Generic.List`1"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert. The value can be <see langword="null"/> for reference types.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than 0. -or- <paramref name="index"/> is greater than <see cref="P:System.Collections.Generic.List`1.Count"/>.</exception>
        /// <remarks>
        /// <see cref="System.Collections.Generic.List{T}"/> accepts <c>null</c> as a valid value for reference types and allows duplicate elements.
        /// If <see cref="System.Collections.Generic.List{T}.Count"/> already equals <see cref="System.Collections.Generic.List{T}.Capacity"/>, the capacity of the <see cref="System.Collections.Generic.List{T}"/> is increased by automatically reallocating the internal array, and the existing elements are copied to the new array before the new element is added.
        /// If <c>index</c> is equal to <see cref="System.Collections.Generic.List{T}.Count"/>, <c>item</c> is added to the end of <see cref="System.Collections.Generic.List{T}"/>.
        /// This method is an O(*n*) operation, where *n* is <see cref="System.Collections.Generic.List{T}.Count"/>.
        /// The following example demonstrates how to add, remove, and insert a simple business object in a <see cref="System.Collections.Generic.List{T}"/>.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/ListT/Overview/program.cs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ListT/Add/module1.vb" id="Snippet1" />
        /// <code lang="fsharp" source="~/snippets/fsharp/VS_Snippets_CLR_System/system.collections.generic.list.addremoveinsert/fs/addremoveinsert.fs" id="Snippet1" />
        /// The following example demonstrates the <see cref="System.Collections.Generic.List{T}.Insert"/> method, along with various other properties and methods of the <see cref="System.Collections.Generic.List{T}"/> generic class. After the list is created, elements are added. The <see cref="System.Collections.Generic.List{T}.Insert"/> method is used to insert an item into the middle of the list. The item inserted is a duplicate, which is later removed using the <see cref="System.Collections.Generic.List{T}.Remove"/> method.
        /// :::code language="csharp" source="~/snippets/csharp/System.Collections.Generic/ListT/Overview/source.cs" interactive="try-dotnet-method" id="Snippet1":::
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ListT/.ctor/source1.vb" id="Snippet1" />
        /// <code lang="fsharp" source="~/snippets/fsharp/VS_Snippets_CLR/List`1_Class/fs/listclass.fs" id="Snippet1" />
        /// </remarks>
        public void Insert(int index, T item)
        {
            // Note that insertions at the end are legal.
            if ((uint)index > (uint)_size)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_ListInsert);
            }
            if (_size == _items.Length)
            {
                GrowForInsertion(index, 1);
            }
            else if (index < _size)
            {
                Array.Copy(_items, index, _items, index + 1, _size - index);
            }
            _items[index] = item;
            _size++;
            _version++;
        }

        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.IList"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="T:System.Collections.IList"/>.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.IList"/>.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="item"/> is of a type that is not assignable to the <see cref="T:System.Collections.IList"/>.</exception>
        /// <remarks>
        /// If <c>index</c> equals the number of items in the <see cref="System.Collections.IList"/>, then <c>item</c> is appended to the end.
        /// This method is an O(*n*) operation, where *n* is <see cref="System.Collections.Generic.List{T}.Count"/>.
        /// </remarks>
        void IList.Insert(int index, object? item)
        {
            ThrowHelper.IfNullAndNullsAreIllegalThenThrow<T>(item, ExceptionArgument.item);

            try
            {
                Insert(index, (T)item!);
            }
            catch (InvalidCastException)
            {
                ThrowHelper.ThrowWrongValueTypeArgumentException(item, typeof(T));
            }
        }

        // Inserts the elements of the given collection at a given index. If
        // required, the capacity of the list is increased to twice the previous
        // capacity or the new size, whichever is larger.  Ranges may be added
        // to the end of the list by setting index to the List's size.
        //
        /// <summary>
        /// Inserts the elements of a collection into the <see cref="T:System.Collections.Generic.List`1"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the new elements should be inserted.</param>
        /// <param name="collection">The collection whose elements should be inserted into the <see cref="T:System.Collections.Generic.List`1"/>. The collection itself cannot be <see langword="null"/>, but it can contain elements that are <see langword="null"/>, if type <paramref name="T"/> is a reference type.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="collection"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than 0. -or- <paramref name="index"/> is greater than <see cref="P:System.Collections.Generic.List`1.Count"/>.</exception>
        /// <remarks>
        /// <see cref="System.Collections.Generic.List{T}"/> accepts <c>null</c> as a valid value for reference types and allows duplicate elements.
        /// If the new <see cref="System.Collections.Generic.List{T}.Count"/> (the current <see cref="System.Collections.Generic.List{T}.Count"/> plus the size of the collection) will be greater than <see cref="System.Collections.Generic.List{T}.Capacity"/>, the capacity of the <see cref="System.Collections.Generic.List{T}"/> is increased by automatically reallocating the internal array to accommodate the new elements, and the existing elements are copied to the new array before the new elements are added.
        /// If <c>index</c> is equal to <see cref="System.Collections.Generic.List{T}.Count"/>, the elements are added to the end of <see cref="System.Collections.Generic.List{T}"/>.
        /// The order of the elements in the collection is preserved in the <see cref="System.Collections.Generic.List{T}"/>.
        /// This method is an O(*n* * *m*) operation, where *n* is the number of elements to be added and *m* is <see cref="System.Collections.Generic.List{T}.Count"/>.
        /// The following example demonstrates <see cref="System.Collections.Generic.List{T}.InsertRange"/> method and various other methods of the <see cref="System.Collections.Generic.List{T}"/> class that act on ranges. After the list has been created and populated with the names of several peaceful plant-eating dinosaurs, the <see cref="System.Collections.Generic.List{T}.InsertRange"/> method is used to insert an array of three ferocious meat-eating dinosaurs into the list, beginning at index location 3.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/ListT/.ctor/source1.cs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ListT/.ctor/source2.vb" id="Snippet1" />
        /// </remarks>
        public void InsertRange(int index, IEnumerable<T> collection)
        {
            if (collection == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.collection);
            }

            if ((uint)index > (uint)_size)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException();
            }

            if (collection is ICollection<T> c)
            {
                int count = c.Count;
                if (count > 0)
                {
                    if (_items.Length - _size < count)
                    {
                        GrowForInsertion(index, count);
                    }
                    else if (index < _size)
                    {
                        Array.Copy(_items, index, _items, index + count, _size - index);
                    }

                    // If we're inserting a List into itself, we want to be able to deal with that.
                    if (this == c)
                    {
                        // Copy first part of _items to insert location
                        Array.Copy(_items, 0, _items, index, index);
                        // Copy last part of _items back to inserted location
                        Array.Copy(_items, index + count, _items, index * 2, _size - index);
                    }
                    else
                    {
                        c.CopyTo(_items, index);
                    }
                    _size += count;
                    _version++;
                }
            }
            else
            {
                using (IEnumerator<T> en = collection.GetEnumerator())
                {
                    while (en.MoveNext())
                    {
                        Insert(index++, en.Current);
                    }
                }
            }
        }

        // Returns the index of the last occurrence of a given value in a range of
        // this list. The list is searched backwards, starting at the end
        // and ending at the first element in the list. The elements of the list
        // are compared to the given value using the Object.Equals method.
        //
        // This method uses the Array.LastIndexOf method to perform the
        // search.
        //
        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the last occurrence within the entire <see cref="T:System.Collections.Generic.List`1"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.List`1"/>. The value can be <see langword="null"/> for reference types.</param>
        /// <returns>The zero-based index of the last occurrence of <paramref name="item"/> within the entire the <see cref="T:System.Collections.Generic.List`1"/>, if found; otherwise, -1.</returns>
        /// <remarks>
        /// The <see cref="System.Collections.Generic.List{T}"/> is searched backward starting at the last element and ending at the first element.
        /// This method determines equality using the default equality comparer <see cref="System.Collections.Generic.EqualityComparer{T}.Default">Default</see> for <c>T</c>, the type of values in the list.
        /// This method performs a linear search; therefore, this method is an O(*n*) operation, where *n* is <see cref="System.Collections.Generic.List{T}.Count"/>.
        /// </remarks>
        /// <related type="Article" href="/dotnet/standard/globalization-localization/performing-culture-insensitive-string-operations-in-collections">Performing Culture-Insensitive String Operations in Collections</related>
        public int LastIndexOf(T item)
        {
            if (_size == 0)
            {  // Special case for empty list
                return -1;
            }
            else
            {
                return LastIndexOf(item, _size - 1, _size);
            }
        }

        // Returns the index of the last occurrence of a given value in a range of
        // this list. The list is searched backwards, starting at index
        // index and ending at the first element in the list. The
        // elements of the list are compared to the given value using the
        // Object.Equals method.
        //
        // This method uses the Array.LastIndexOf method to perform the
        // search.
        //
        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the last occurrence within the range of elements in the <see cref="T:System.Collections.Generic.List`1"/> that extends from the first element to the specified index.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.List`1"/>. The value can be <see langword="null"/> for reference types.</param>
        /// <param name="index">The zero-based starting index of the backward search.</param>
        /// <returns>The zero-based index of the last occurrence of <paramref name="item"/> within the range of elements in the <see cref="T:System.Collections.Generic.List`1"/> that extends from the first element to <paramref name="index"/>, if found; otherwise, -1.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is outside the range of valid indexes for the <see cref="T:System.Collections.Generic.List`1"/>.</exception>
        /// <remarks>
        /// The <see cref="System.Collections.Generic.List{T}"/> is searched backward starting at <c>index</c> and ending at the first element.
        /// This method determines equality using the default equality comparer <see cref="System.Collections.Generic.EqualityComparer{T}.Default">Default</see> for <c>T</c>, the type of values in the list.
        /// This method performs a linear search; therefore, this method is an O(*n*) operation, where *n* is the number of elements from the beginning of the <see cref="System.Collections.Generic.List{T}"/> to <c>index</c>.
        /// </remarks>
        /// <related type="Article" href="/dotnet/standard/globalization-localization/performing-culture-insensitive-string-operations-in-collections">Performing Culture-Insensitive String Operations in Collections</related>
        public int LastIndexOf(T item, int index)
        {
            if (index >= _size)
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessException();
            return LastIndexOf(item, index, index + 1);
        }

        // Returns the index of the last occurrence of a given value in a range of
        // this list. The list is searched backwards, starting at index
        // index and upto count elements. The elements of
        // the list are compared to the given value using the Object.Equals
        // method.
        //
        // This method uses the Array.LastIndexOf method to perform the
        // search.
        //
        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the last occurrence within the range of elements in the <see cref="T:System.Collections.Generic.List`1"/> that contains the specified number of elements and ends at the specified index.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.List`1"/>. The value can be <see langword="null"/> for reference types.</param>
        /// <param name="index">The zero-based starting index of the backward search.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <returns>The zero-based index of the last occurrence of <paramref name="item"/> within the range of elements in the <see cref="T:System.Collections.Generic.List`1"/> that contains <paramref name="count"/> number of elements and ends at <paramref name="index"/>, if found; otherwise, -1.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is outside the range of valid indexes for the <see cref="T:System.Collections.Generic.List`1"/>. -or- <paramref name="count"/> is less than 0. -or- <paramref name="index"/> and <paramref name="count"/> do not specify a valid section in the <see cref="T:System.Collections.Generic.List`1"/>.</exception>
        /// <remarks>
        /// The <see cref="System.Collections.Generic.List{T}"/> is searched backward starting at <c>index</c> and ending at <c>index</c> minus <c>count</c> plus 1, if <c>count</c> is greater than 0.
        /// This method determines equality using the default equality comparer <see cref="System.Collections.Generic.EqualityComparer{T}.Default">Default</see> for <c>T</c>, the type of values in the list.
        /// This method performs a linear search; therefore, this method is an O(*n*) operation, where *n* is <c>count</c>.
        /// </remarks>
        /// <related type="Article" href="/dotnet/standard/globalization-localization/performing-culture-insensitive-string-operations-in-collections">Performing Culture-Insensitive String Operations in Collections</related>
        public int LastIndexOf(T item, int index, int count)
        {
            if ((Count != 0) && (index < 0))
            {
                ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException();
            }

            if ((Count != 0) && (count < 0))
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            }

            if (_size == 0)
            {  // Special case for empty list
                return -1;
            }

            if (index >= _size)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_BiggerThanCollection);
            }

            if (count > index + 1)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_BiggerThanCollection);
            }

            return Array.LastIndexOf(_items, item, index, count);
        }

        // Removes the first occurrence of the given element, if found.
        // The size of the list is decreased by one if successful.
        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.List`1"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.List`1"/>. The value can be <see langword="null"/> for reference types.</param>
        /// <returns><see langword="true"/> if <paramref name="item"/> is successfully removed; otherwise, <see langword="false"/>. This method also returns <see langword="false"/> if <paramref name="item"/> was not found in the <see cref="T:System.Collections.Generic.List`1"/>.</returns>
        /// <remarks>
        /// If type <c>T</c> implements the <see cref="System.IEquatable{T}"/> generic interface, the equality comparer is the <see cref="System.IEquatable{T}.Equals"/> method of that interface; otherwise, the default equality comparer is <see cref="System.Object.Equals">Equals</see>.
        /// This method performs a linear search; therefore, this method is an O(*n*) operation, where *n* is <see cref="System.Collections.Generic.List{T}.Count"/>.
        /// The following example demonstrates how to add, remove, and insert a simple business object in a <see cref="System.Collections.Generic.List{T}"/>.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/ListT/Overview/program.cs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ListT/Add/module1.vb" id="Snippet1" />
        /// <code lang="fsharp" source="~/snippets/fsharp/VS_Snippets_CLR_System/system.collections.generic.list.addremoveinsert/fs/addremoveinsert.fs" id="Snippet1" />
        /// The following example demonstrates <see cref="System.Collections.Generic.List{T}.Remove"/> method. Several properties and methods of the <see cref="System.Collections.Generic.List{T}"/> generic class are used to add, insert, and search the list. After these operations, the list contains a duplicate. The <see cref="System.Collections.Generic.List{T}.Remove"/> method is used to remove the first instance of the duplicate item, and the contents are displayed. The <see cref="System.Collections.Generic.List{T}.Remove"/> method always removes the first instance it encounters.
        /// :::code language="csharp" source="~/snippets/csharp/System.Collections.Generic/ListT/Overview/source.cs" interactive="try-dotnet-method" id="Snippet1":::
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ListT/.ctor/source1.vb" id="Snippet1" />
        /// <code lang="fsharp" source="~/snippets/fsharp/VS_Snippets_CLR/List`1_Class/fs/listclass.fs" id="Snippet1" />
        /// </remarks>
        /// <related type="Article" href="/dotnet/standard/globalization-localization/performing-culture-insensitive-string-operations-in-collections">Performing Culture-Insensitive String Operations in Collections</related>
        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.IList"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.IList"/>.</param>
        /// <exception cref="T:System.ArgumentException"><paramref name="item"/> is of a type that is not assignable to the <see cref="T:System.Collections.IList"/>.</exception>
        /// <remarks>
        /// This method determines equality using the default equality comparer <see cref="System.Collections.Generic.EqualityComparer{T}.Default">Default</see> for <c>T</c>, the type of values in the list.
        /// This method performs a linear search; therefore, this method is an O(*n*) operation, where *n* is <see cref="System.Collections.Generic.List{T}.Count"/>.
        /// </remarks>
        void IList.Remove(object? item)
        {
            if (IsCompatibleObject(item))
            {
                Remove((T)item!);
            }
        }

        // This method removes all items which matches the predicate.
        // The complexity is O(n).
        /// <summary>
        /// Removes all the elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The <see cref="T:System.Predicate`1"/> delegate that defines the conditions of the elements to remove.</param>
        /// <returns>The number of elements removed from the <see cref="T:System.Collections.Generic.List`1"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="match"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// <note type="note">
        /// In C# and Visual Basic, it is not necessary to create the <c>Predicate&lt;string&gt;</c> delegate (<c>Predicate(Of String)</c> in Visual Basic) explicitly. These languages infer the correct delegate from context, and create it automatically.
        /// </note>
        /// The <see cref="System.Predicate{T}"/> is a delegate to a method that returns <c>true</c> if the object passed to it matches the conditions defined in the delegate.  The elements of the current <see cref="System.Collections.Generic.List{T}"/> are individually passed to the <see cref="System.Predicate{T}"/> delegate, and the elements that match the conditions are removed from the <see cref="System.Collections.Generic.List{T}"/>.
        /// This method performs a linear search; therefore, this method is an O(*n*) operation, where *n* is <see cref="System.Collections.Generic.List{T}.Count"/>.
        /// The following example demonstrates the <see cref="System.Collections.Generic.List{T}.RemoveAll"/> method and several other methods that use the <see cref="System.Predicate{T}"/> generic delegate.
        /// A <see cref="System.Collections.Generic.List{T}"/> of strings is created, containing 8 dinosaur names, two of which (at positions 1 and 5) end with "saurus". The example also defines a search predicate method named <c>EndsWithSaurus</c>, which accepts a string parameter and returns a Boolean value indicating whether the input string ends in "saurus".
        /// The <see cref="System.Collections.Generic.List{T}.Find"/>, <see cref="System.Collections.Generic.List{T}.FindLast"/>, and <see cref="System.Collections.Generic.List{T}.FindAll"/> methods are used to search the list with the search predicate method.
        /// The <see cref="System.Collections.Generic.List{T}.RemoveAll"/> method is used to remove all entries ending with "saurus". It traverses the list from the beginning, passing each element in turn to the <c>EndsWithSaurus</c> method. The element is removed if the <c>EndsWithSaurus</c> method returns <c>true</c>.
        /// Finally, the <see cref="System.Collections.Generic.List{T}.Exists"/> method verifies that there are no strings in the list that end with "saurus".
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/ListT/Exists/source.cs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ListT/Exists/source.vb" id="Snippet1" />
        /// </remarks>
        public int RemoveAll(Predicate<T> match)
        {
            if (match == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
            }

            int freeIndex = 0;   // the first free slot in items array

            // Find the first item which needs to be removed.
            while (freeIndex < _size && !match(_items[freeIndex])) freeIndex++;
            if (freeIndex >= _size) return 0;

            int current = freeIndex + 1;
            while (current < _size)
            {
                // Find the first item which needs to be kept.
                while (current < _size && match(_items[current])) current++;

                if (current < _size)
                {
                    // copy item to the free slot.
                    _items[freeIndex++] = _items[current++];
                }
            }

            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                Array.Clear(_items, freeIndex, _size - freeIndex); // Clear the elements so that the gc can reclaim the references.
            }

            int result = _size - freeIndex;
            _size = freeIndex;
            _version++;
            return result;
        }

        // Removes the element at the given index. The size of the list is
        // decreased by one.
        /// <summary>
        /// Removes the element at the specified index of the <see cref="T:System.Collections.Generic.List`1"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than 0. -or- <paramref name="index"/> is equal to or greater than <see cref="P:System.Collections.Generic.List`1.Count"/>.</exception>
        /// <remarks>
        /// When you call <see cref="System.Collections.Generic.List{T}.RemoveAt"/> to remove an item, the remaining items in the list are renumbered to replace the removed item. For example, if you remove the item at index 3, the item at index 4 is moved to the 3 position. In addition, the number of items in the list (as represented by the <see cref="System.Collections.Generic.List{T}.Count"/> property) is reduced by 1.
        /// This method is an O(*n*) operation, where *n* is (<see cref="System.Collections.Generic.List{T}.Count"/> - <c>index</c>).
        /// The following example demonstrates how to add, remove, and insert a simple business object in a <see cref="System.Collections.Generic.List{T}"/>.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/ListT/Overview/program.cs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ListT/Add/module1.vb" id="Snippet1" />
        /// <code lang="fsharp" source="~/snippets/fsharp/VS_Snippets_CLR_System/system.collections.generic.list.addremoveinsert/fs/addremoveinsert.fs" id="Snippet1" />
        /// </remarks>
        public void RemoveAt(int index)
        {
            if ((uint)index >= (uint)_size)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessException();
            }
            _size--;
            if (index < _size)
            {
                Array.Copy(_items, index + 1, _items, index, _size - index);
            }
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                _items[_size] = default!;
            }
            _version++;
        }

        // Removes a range of elements from this list.
        /// <summary>
        /// Removes a range of elements from the <see cref="T:System.Collections.Generic.List`1"/>.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range of elements to remove.</param>
        /// <param name="count">The number of elements to remove.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than 0. -or- <paramref name="count"/> is less than 0.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="index"/> and <paramref name="count"/> do not denote a valid range of elements in the <see cref="T:System.Collections.Generic.List`1"/>.</exception>
        /// <remarks>
        /// The items are removed and all the elements following them in the <see cref="System.Collections.Generic.List{T}"/> have their indexes reduced by <c>count</c>.
        /// This method is an O(*n*) operation, where *n* is <see cref="System.Collections.Generic.List{T}.Count"/>.
        /// The following example demonstrates the <see cref="System.Collections.Generic.List{T}.RemoveRange"/> method and various other methods of the <see cref="System.Collections.Generic.List{T}"/> class that act on ranges. After the list has been created and modified, the <see cref="System.Collections.Generic.List{T}.RemoveRange"/> method is used to remove two elements from the list, beginning at index location 2.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/ListT/.ctor/source1.cs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ListT/.ctor/source2.vb" id="Snippet1" />
        /// </remarks>
        public void RemoveRange(int index, int count)
        {
            if (index < 0)
            {
                ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException();
            }

            if (count < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            }

            if (_size - index < count)
                ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);

            if (count > 0)
            {
                _size -= count;
                if (index < _size)
                {
                    Array.Copy(_items, index + count, _items, index, _size - index);
                }

                _version++;
                if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
                {
                    Array.Clear(_items, _size, count);
                }
            }
        }

        // Reverses the elements in this list.
        /// <summary>
        /// Reverses the order of the elements in the entire <see cref="T:System.Collections.Generic.List`1"/>.
        /// </summary>
        /// <remarks>
        /// This method uses <see cref="System.Array.Reverse">Reverse</see> to reverse the order of the elements.
        /// This method is an O(*n*) operation, where *n* is <see cref="System.Collections.Generic.List{T}.Count"/>.
        /// </remarks>
        public void Reverse()
            => Reverse(0, Count);

        // Reverses the elements in a range of this list. Following a call to this
        // method, an element in the range given by index and count
        // which was previously located at index i will now be located at
        // index index + (index + count - i - 1).
        //
        /// <summary>
        /// Reverses the order of the elements in the specified range.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range to reverse.</param>
        /// <param name="count">The number of elements in the range to reverse.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than 0. -or- <paramref name="count"/> is less than 0.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="index"/> and <paramref name="count"/> do not denote a valid range of elements in the <see cref="T:System.Collections.Generic.List`1"/>.</exception>
        /// <remarks>
        /// This method uses <see cref="System.Array.Reverse">Reverse</see> to reverse the order of the elements.
        /// This method is an O(*n*) operation, where *n* is <see cref="System.Collections.Generic.List{T}.Count"/>.
        /// </remarks>
        public void Reverse(int index, int count)
        {
            if (index < 0)
            {
                ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException();
            }

            if (count < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            }

            if (_size - index < count)
                ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);

            if (count > 1)
            {
                Array.Reverse(_items, index, count);
            }
            _version++;
        }

        // Sorts the elements in this list.  Uses the default comparer and
        // Array.Sort.
        /// <summary>
        /// Sorts the elements in the entire <see cref="T:System.Collections.Generic.List`1"/> using the default comparer.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">The default comparer <see cref="P:System.Collections.Generic.Comparer`1.Default"/> cannot find an implementation of the <see cref="T:System.IComparable`1"/> generic interface or the <see cref="T:System.IComparable"/> interface for type <paramref name="T"/>.</exception>
        /// <remarks>
        /// This method uses the default comparer <see cref="System.Collections.Generic.Comparer{T}.Default">Default</see> for type <c>T</c> to determine the order of list elements. The <see cref="System.Collections.Generic.Comparer{T}.Default">Default</see> property checks whether type <c>T</c> implements the <see cref="System.IComparable{T}"/> generic interface and uses that implementation, if available.  If not, <see cref="System.Collections.Generic.Comparer{T}.Default">Default</see> checks whether type <c>T</c> implements the <see cref="System.IComparable"/> interface.  If type <c>T</c> does not implement either interface, <see cref="System.Collections.Generic.Comparer{T}.Default">Default</see> throws an <see cref="System.InvalidOperationException"/>.
        /// This method uses the <see cref="System.Array.Sort">Sort</see> method, which applies the introspective sort as follows:
        /// - If the partition size is less than or equal to 16 elements, it uses an insertion sort algorithm.
        /// - If the number of partitions exceeds 2 log *n*, where *n* is the range of the input array, it uses a Heapsort algorithm.
        /// - Otherwise, it uses a Quicksort algorithm.
        /// This implementation performs an unstable sort; that is, if two elements are equal, their order might not be preserved. In contrast, a stable sort preserves the order of elements that are equal.
        /// This method is an O(*n* log *n*) operation, where *n* is <see cref="System.Collections.Generic.List{T}.Count"/>.
        /// The following example adds some names to a <c>List&lt;String&gt;</c> object, displays the list in unsorted order, calls the <see cref="System.Collections.Generic.List{T}.Sort"/> method, and then displays the sorted list.
        /// :::code language="csharp" source="~/snippets/csharp/System.Collections.Generic/ListT/Sort/Sort1.cs" interactive="try-dotnet-method" id="Snippet2":::
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ListT/Sort/Sort1.vb" id="Snippet2" />
        /// The following code demonstrates the <see cref="System.Collections.Generic.List{T}.Sort"/> and <see cref="System.Collections.Generic.List{T}.Sort%28System.Comparison%7B{}%7D%29"/> method overloads on a simple business object. Calling the <see cref="System.Collections.Generic.List{T}.Sort"/> method results in the use of the default comparer for the Part type, and the <see cref="System.Collections.Generic.List{T}.Sort%28System.Comparison%7B{}%7D%29"/> method is implemented by using an anonymous method.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/ListT/Sort/program.cs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ListT/Sort/module1.vb" id="Snippet1" />
        /// The following example demonstrates the <see cref="System.Collections.Generic.List{T}.Sort"/> method overload and the <see cref="System.Collections.Generic.List{T}.BinarySearch%28{}%29"/> method overload. A <see cref="System.Collections.Generic.List{T}"/> of strings is created and populated with four strings, in no particular order. The list is displayed, sorted, and displayed again.
        /// The <see cref="System.Collections.Generic.List{T}.BinarySearch%28{}%29"/> method overload is then used to search for two strings that are not in the list, and the <see cref="System.Collections.Generic.List{T}.Insert"/> method is used to insert them. The return value of the <see cref="System.Collections.Generic.List{T}.BinarySearch"/> method is negative in each case, because the strings are not in the list. Taking the bitwise complement (the ~ operator in C#, <c>Xor</c> -1 in Visual Basic) of this negative number produces the index of the first element in the list that is larger than the search string, and inserting at this location preserves the sort order. The second search string is larger than any element in the list, so the insertion position is at the end of the list.
        /// :::code language="csharp" source="~/snippets/csharp/System.Collections.Generic/ListT/BinarySearch/source.cs" interactive="try-dotnet-method" id="Snippet1":::
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ListT/BinarySearch/source.vb" id="Snippet1" />
        /// </remarks>
        /// <related type="Article" href="/dotnet/standard/globalization-localization/performing-culture-insensitive-string-operations-in-collections">Performing Culture-Insensitive String Operations in Collections</related>
        public void Sort()
            => Sort(0, Count, null);

        // Sorts the elements in this list.  Uses Array.Sort with the
        // provided comparer.
        /// <summary>
        /// Sorts the elements in the entire <see cref="T:System.Collections.Generic.List`1"/> using the specified comparer.
        /// </summary>
        /// <param name="comparer">The <see cref="T:System.Collections.Generic.IComparer`1"/> implementation to use when comparing elements, or <see langword="null"/> to use the default comparer <see cref="P:System.Collections.Generic.Comparer`1.Default"/>.</param>
        /// <exception cref="T:System.InvalidOperationException"><paramref name="comparer"/> is <see langword="null"/>, and the default comparer <see cref="P:System.Collections.Generic.Comparer`1.Default"/> cannot find implementation of the <see cref="T:System.IComparable`1"/> generic interface or the <see cref="T:System.IComparable"/> interface for type <paramref name="T"/>.</exception>
        /// <exception cref="T:System.ArgumentException">The implementation of <paramref name="comparer"/> caused an error during the sort. For example, <paramref name="comparer"/> might not return 0 when comparing an item with itself.</exception>
        /// <remarks>
        /// If <c>comparer</c> is provided, the elements of the <see cref="System.Collections.Generic.List{T}"/> are sorted using the specified <see cref="System.Collections.Generic.IComparer{T}"/> implementation.
        /// If <c>comparer</c> is <c>null</c>, the default comparer <see cref="System.Collections.Generic.Comparer{T}.Default">Default</see> checks whether type <c>T</c> implements the <see cref="System.IComparable{T}"/> generic interface and uses that implementation, if available.  If not, <see cref="System.Collections.Generic.Comparer{T}.Default">Default</see> checks whether type <c>T</c> implements the <see cref="System.IComparable"/> interface.  If type <c>T</c> does not implement either interface, <see cref="System.Collections.Generic.Comparer{T}.Default">Default</see> throws an <see cref="System.InvalidOperationException"/>.
        /// This method uses the <see cref="System.Array.Sort">Sort</see> method, which applies the introspective sort as follows:
        /// - If the partition size is less than or equal to 16 elements, it uses an insertion sort algorithm.
        /// - If the number of partitions exceeds 2 log *n*, where *n* is the range of the input array, it uses a Heapsort algorithm.
        /// - Otherwise, it uses a Quicksort algorithm.
        /// This implementation performs an unstable sort; that is, if two elements are equal, their order might not be preserved. In contrast, a stable sort preserves the order of elements that are equal.
        /// This method is an O(*n* log *n*) operation, where *n* is <see cref="System.Collections.Generic.List{T}.Count"/>.
        /// The following example demonstrates the <see cref="System.Collections.Generic.List{T}.Sort%28System.Collections.Generic.IComparer%7B{}%7D%29"/> method overload and the <see cref="System.Collections.Generic.List{T}.BinarySearch%28{}%2CSystem.Collections.Generic.IComparer%7B{}%7D%29"/> method overload.
        /// The example defines an alternative comparer for strings named DinoCompare, which implements the <c>IComparer&lt;string&gt;</c> (<c>IComparer(Of String)</c> in Visual Basic) generic interface. The comparer works as follows: First, the comparands are tested for <c>null</c>, and a null reference is treated as less than a non-null. Second, the string lengths are compared, and the longer string is deemed to be greater. Third, if the lengths are equal, ordinary string comparison is used.
        /// A <see cref="System.Collections.Generic.List{T}"/> of strings is created and populated with four strings, in no particular order. The list is displayed, sorted using the alternate comparer, and displayed again.
        /// The <see cref="System.Collections.Generic.List{T}.BinarySearch%28{}%2CSystem.Collections.Generic.IComparer%7B{}%7D%29"/> method overload is then used to search for several strings that are not in the list, employing the alternate comparer. The <see cref="System.Collections.Generic.List{T}.Insert"/> method is used to insert the strings. These two methods are located in the function named <c>SearchAndInsert</c>, along with code to take the bitwise complement (the ~ operator in C#, <c>Xor</c> -1 in Visual Basic) of the negative number returned by <see cref="System.Collections.Generic.List{T}.BinarySearch%28{}%2CSystem.Collections.Generic.IComparer%7B{}%7D%29"/> and use it as an index for inserting the new string.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/ListT/BinarySearch/source1.cs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ListT/BinarySearch/source1.vb" id="Snippet1" />
        /// </remarks>
        /// <related type="Article" href="/dotnet/standard/globalization-localization/performing-culture-insensitive-string-operations-in-collections">Performing Culture-Insensitive String Operations in Collections</related>
        public void Sort(IComparer<T>? comparer)
            => Sort(0, Count, comparer);

        // Sorts the elements in a section of this list. The sort compares the
        // elements to each other using the given IComparer interface. If
        // comparer is null, the elements are compared to each other using
        // the IComparable interface, which in that case must be implemented by all
        // elements of the list.
        //
        // This method uses the Array.Sort method to sort the elements.
        //
        /// <summary>
        /// Sorts the elements in a range of elements in <see cref="T:System.Collections.Generic.List`1"/> using the specified comparer.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range to sort.</param>
        /// <param name="count">The length of the range to sort.</param>
        /// <param name="comparer">The <see cref="T:System.Collections.Generic.IComparer`1"/> implementation to use when comparing elements, or <see langword="null"/> to use the default comparer <see cref="P:System.Collections.Generic.Comparer`1.Default"/>.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than 0. -or- <paramref name="count"/> is less than 0.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="index"/> and <paramref name="count"/> do not specify a valid range in the <see cref="T:System.Collections.Generic.List`1"/>. -or- The implementation of <paramref name="comparer"/> caused an error during the sort. For example, <paramref name="comparer"/> might not return 0 when comparing an item with itself.</exception>
        /// <exception cref="T:System.InvalidOperationException"><paramref name="comparer"/> is <see langword="null"/>, and the default comparer <see cref="P:System.Collections.Generic.Comparer`1.Default"/> cannot find implementation of the <see cref="T:System.IComparable`1"/> generic interface or the <see cref="T:System.IComparable"/> interface for type <paramref name="T"/>.</exception>
        /// <remarks>
        /// If <c>comparer</c> is provided, the elements of the <see cref="System.Collections.Generic.List{T}"/> are sorted using the specified <see cref="System.Collections.Generic.IComparer{T}"/> implementation.
        /// If <c>comparer</c> is <c>null</c>, the default comparer <see cref="System.Collections.Generic.Comparer{T}.Default">Default</see> checks whether type <c>T</c> implements the <see cref="System.IComparable{T}"/> generic interface and uses that implementation, if available.  If not, <see cref="System.Collections.Generic.Comparer{T}.Default">Default</see> checks whether type <c>T</c> implements the <see cref="System.IComparable"/> interface.  If type <c>T</c> does not implement either interface, <see cref="System.Collections.Generic.Comparer{T}.Default">Default</see> throws an <see cref="System.InvalidOperationException"/>.
        /// This method uses <see cref="System.Array.Sort">Sort</see>, which applies the introspective sort as follows:
        /// - If the partition size is less than or equal to 16 elements, it uses an insertion sort algorithm
        /// - If the number of partitions exceeds 2 log *n*, where *n* is the range of the input array, it uses a [Heapsort](https://en.wikipedia.org/wiki/Heapsort) algorithm.
        /// - Otherwise, it uses a Quicksort algorithm.
        /// This implementation performs an unstable sort; that is, if two elements are equal, their order might not be preserved. In contrast, a stable sort preserves the order of elements that are equal.
        /// This method is an O(*n* log *n*) operation, where *n* is <see cref="System.Collections.Generic.List{T}.Count"/>.
        /// The following example demonstrates the <see cref="System.Collections.Generic.List{T}.Sort%28System.Int32%2CSystem.Int32%2CSystem.Collections.Generic.IComparer%7B{}%7D%29"/> method overload and the <see cref="System.Collections.Generic.List{T}.BinarySearch%28System.Int32%2CSystem.Int32%2C{}%2CSystem.Collections.Generic.IComparer%7B{}%7D%29"/> method overload.
        /// The example defines an alternative comparer for strings named DinoCompare, which implements the <c>IComparer&lt;string&gt;</c> (<c>IComparer(Of String)</c> in Visual Basic) generic interface. The comparer works as follows: First, the comparands are tested for <c>null</c>, and a null reference is treated as less than a non-null. Second, the string lengths are compared, and the longer string is deemed to be greater. Third, if the lengths are equal, ordinary string comparison is used.
        /// A <see cref="System.Collections.Generic.List{T}"/> of strings is created and populated with the names of five herbivorous dinosaurs and three carnivorous dinosaurs. Within each of the two groups, the names are not in any particular sort order. The list is displayed, the range of herbivores is sorted using the alternate comparer, and the list is displayed again.
        /// The <see cref="System.Collections.Generic.List{T}.BinarySearch%28System.Int32%2CSystem.Int32%2C{}%2CSystem.Collections.Generic.IComparer%7B{}%7D%29"/> method overload is then used to search only the range of herbivores for "Brachiosaurus". The string is not found, and the bitwise complement (the ~ operator in C#, <c>Xor</c> -1 in Visual Basic) of the negative number returned by the <see cref="System.Collections.Generic.List{T}.BinarySearch%28System.Int32%2CSystem.Int32%2C{}%2CSystem.Collections.Generic.IComparer%7B{}%7D%29"/> method is used as an index for inserting the new string.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/ListT/BinarySearch/source2.cs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ListT/BinarySearch/source2.vb" id="Snippet1" />
        /// </remarks>
        /// <related type="Article" href="/dotnet/standard/globalization-localization/performing-culture-insensitive-string-operations-in-collections">Performing Culture-Insensitive String Operations in Collections</related>
        public void Sort(int index, int count, IComparer<T>? comparer)
        {
            if (index < 0)
            {
                ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException();
            }

            if (count < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            }

            if (_size - index < count)
                ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);

            if (count > 1)
            {
                Array.Sort(_items, index, count, comparer);
            }
            _version++;
        }

        /// <summary>
        /// Sorts the elements in the entire <see cref="T:System.Collections.Generic.List`1"/> using the specified <see cref="T:System.Comparison`1"/>.
        /// </summary>
        /// <param name="comparison">The <see cref="T:System.Comparison`1"/> to use when comparing elements.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="comparison"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentException">The implementation of <paramref name="comparison"/> caused an error during the sort. For example, <paramref name="comparison"/> might not return 0 when comparing an item with itself.</exception>
        /// <remarks>
        /// If <c>comparison</c> is provided, the elements of the <see cref="System.Collections.Generic.List{T}"/> are sorted using the method represented by the delegate.
        /// If <c>comparison</c> is <c>null</c>, an <see cref="System.ArgumentNullException"/> is thrown.
        /// This method uses <see cref="System.Array.Sort">Sort</see>, which applies the introspective sort as follows:
        /// - If the partition size is less than or equal to 16 elements, it uses an insertion sort algorithm
        /// - If the number of partitions exceeds 2 log *n*, where *n* is the range of the input array, it uses a [Heapsort](https://en.wikipedia.org/wiki/Heapsort) algorithm.
        /// - Otherwise, it uses a Quicksort algorithm.
        /// This implementation performs an unstable sort; that is, if two elements are equal, their order might not be preserved. In contrast, a stable sort preserves the order of elements that are equal.
        /// This method is an O(*n* log *n*) operation, where *n* is <see cref="System.Collections.Generic.List{T}.Count"/>.
        /// The following code demonstrates the <see cref="System.Collections.Generic.List{T}.Sort"/> and <see cref="System.Collections.Generic.List{T}.Sort"/> method overloads on a simple business object. Calling the <see cref="System.Collections.Generic.List{T}.Sort"/> method results in the use of the default comparer for the Part type, and the <see cref="System.Collections.Generic.List{T}.Sort"/> method is implemented using an anonymous method.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/ListT/Sort/program.cs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ListT/Sort/module1.vb" id="Snippet1" />
        /// The following example demonstrates the <see cref="System.Collections.Generic.List{T}.Sort%28System.Comparison%7B{}%7D%29"/> method overload.
        /// The example defines an alternative comparison method for strings, named <c>CompareDinosByLength</c>. This method works as follows: First, the comparands are tested for <c>null</c>, and a null reference is treated as less than a non-null. Second, the string lengths are compared, and the longer string is deemed to be greater. Third, if the lengths are equal, ordinary string comparison is used.
        /// A <see cref="System.Collections.Generic.List{T}"/> of strings is created and populated with four strings, in no particular order. The list also includes an empty string and a null reference. The list is displayed, sorted using a <see cref="System.Comparison{T}"/> generic delegate representing the <c>CompareDinosByLength</c> method, and displayed again.
        /// <code lang="csharp" source="~/snippets/csharp/System/ComparisonT/Overview/source.cs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System/ComparisonT/Overview/source.vb" id="Snippet1" />
        /// </remarks>
        /// <related type="Article" href="/dotnet/standard/globalization-localization/performing-culture-insensitive-string-operations-in-collections">Performing Culture-Insensitive String Operations in Collections</related>
        public void Sort(Comparison<T> comparison)
        {
            if (comparison == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.comparison);
            }

            if (_size > 1)
            {
                ArraySortHelper<T>.Sort(new Span<T>(_items, 0, _size), comparison);
            }
            _version++;
        }

        // ToArray returns an array containing the contents of the List.
        // This requires copying the List, which is an O(n) operation.
        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.List`1"/> to a new array.
        /// </summary>
        /// <returns>An array containing copies of the elements of the <see cref="T:System.Collections.Generic.List`1"/>.</returns>
        /// <remarks>
        /// The elements are copied using <see cref="System.Array.Copy">Copy</see>, which is an O(*n*) operation, where *n* is <see cref="System.Collections.Generic.List{T}.Count"/>.
        /// This method is an O(*n*) operation, where *n* is <see cref="System.Collections.Generic.List{T}.Count"/>.
        /// The following example demonstrates the <see cref="System.Collections.Generic.List{T}.ToArray"/> method and other methods of the <see cref="System.Collections.Generic.List{T}"/> class that act on ranges. At the end of the example, the <see cref="System.Collections.Generic.List{T}.GetRange"/> method is used to get three items from the list, beginning with index location 2. The <see cref="System.Collections.Generic.List{T}.ToArray"/> method is called on the resulting <see cref="System.Collections.Generic.List{T}"/>, creating an array of three elements. The elements of the array are displayed.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/ListT/.ctor/source1.cs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ListT/.ctor/source2.vb" id="Snippet1" />
        /// </remarks>
        public T[] ToArray()
        {
            if (_size == 0)
            {
                return s_emptyArray;
            }

            T[] array = new T[_size];
            Array.Copy(_items, array, _size);
            return array;
        }

        // Sets the capacity of this list to the size of the list. This method can
        // be used to minimize a list's memory overhead once it is known that no
        // new elements will be added to the list. To completely clear a list and
        // release all memory referenced by the list, execute the following
        // statements:
        //
        // list.Clear();
        // list.TrimExcess();
        //
        /// <summary>
        /// Sets the capacity to the actual number of elements in the <see cref="T:System.Collections.Generic.List`1"/>, if that number is less than a threshold value.
        /// </summary>
        /// <remarks>
        /// <note type="note">
        /// The current threshold of 90 percent might change in future releases.
        /// </note>
        /// This method can be used to minimize a collection's memory overhead if no new elements will be added to the collection. The cost of reallocating and copying a large <see cref="System.Collections.Generic.List{T}"/> can be considerable, however, so the <see cref="System.Collections.Generic.List{T}.TrimExcess"/> method does nothing if the list is at more than 90 percent of capacity. This avoids incurring a large reallocation cost for a relatively small gain.
        /// This method is an O(*n*) operation, where *n* is <see cref="System.Collections.Generic.List{T}.Count"/>.
        /// To reset a <see cref="System.Collections.Generic.List{T}"/> to its initial state, call the <see cref="System.Collections.Generic.List{T}.Clear"/> method before calling the <see cref="System.Collections.Generic.List{T}.TrimExcess"/> method. Trimming an empty <see cref="System.Collections.Generic.List{T}"/> sets the capacity of the <see cref="System.Collections.Generic.List{T}"/> to the default capacity.
        /// The capacity can also be set using the <see cref="System.Collections.Generic.List{T}.Capacity"/> property.
        /// The following example demonstrates how to check the capacity and count of a  <see cref="System.Collections.Generic.List{T}"/> that contains a simple business object, and illustrates using the <see cref="System.Collections.Generic.List{T}.TrimExcess"/> method to remove extra capacity.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/ListT/Capacity/program.cs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ListT/Capacity/module1.vb" id="Snippet1" />
        /// The following example demonstrates the <see cref="System.Collections.Generic.List{T}.TrimExcess"/> method. Several properties and methods of the <see cref="System.Collections.Generic.List{T}"/> class are used to add, insert, and remove items from a list of strings. Then the <see cref="System.Collections.Generic.List{T}.TrimExcess"/> method is used to reduce the capacity to match the count, and the <see cref="System.Collections.Generic.List{T}.Capacity"/> and <see cref="System.Collections.Generic.List{T}.Count"/> properties are displayed. If the unused capacity had been less than 10 percent of total capacity, the list would not have been resized. Finally, the contents of the list are cleared.
        /// :::code language="csharp" source="~/snippets/csharp/System.Collections.Generic/ListT/Overview/source.cs" interactive="try-dotnet-method" id="Snippet1":::
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ListT/.ctor/source1.vb" id="Snippet1" />
        /// <code lang="fsharp" source="~/snippets/fsharp/VS_Snippets_CLR/List`1_Class/fs/listclass.fs" id="Snippet1" />
        /// </remarks>
        public void TrimExcess()
        {
            int threshold = (int)(((double)_items.Length) * 0.9);
            if (_size < threshold)
            {
                Capacity = _size;
            }
        }

        /// <summary>
        /// Determines whether every element in the <see cref="T:System.Collections.Generic.List`1"/> matches the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The <see cref="T:System.Predicate`1"/> delegate that defines the conditions to check against the elements.</param>
        /// <returns><see langword="true"/> if every element in the <see cref="T:System.Collections.Generic.List`1"/> matches the conditions defined by the specified predicate; otherwise, <see langword="false"/>. If the list has no elements, the return value is <see langword="true"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="match"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// <note type="note">
        /// In C# and Visual Basic, it is not necessary to create the <c>Predicate&lt;string&gt;</c> delegate (<c>Predicate(Of String)</c> in Visual Basic) explicitly. These languages infer the correct delegate from context and create it automatically.
        /// </note>
        /// The <see cref="System.Predicate{T}"/> is a delegate to a method that returns <c>true</c> if the object passed to it matches the conditions defined in the delegate.  The elements of the current <see cref="System.Collections.Generic.List{T}"/> are individually passed to the <see cref="System.Predicate{T}"/> delegate, and processing is stopped when the delegate returns <c>false</c> for any element. The elements are processed in order, and all calls are made on a single thread.
        /// This method is an O(*n*) operation, where *n* is <see cref="System.Collections.Generic.List{T}.Count"/>.
        /// The following example demonstrates the <see cref="System.Collections.Generic.List{T}.TrueForAll"/> method and several other methods that use <see cref="System.Predicate{T}"/> generic delegate.
        /// A <see cref="System.Collections.Generic.List{T}"/> of strings is created, containing 8 dinosaur names, two of which (at positions 1 and 5) end with "saurus". The example also defines a search predicate method named <c>EndsWithSaurus</c>, which accepts a string parameter and returns a Boolean value indicating whether the input string ends in "saurus".
        /// The <see cref="System.Collections.Generic.List{T}.TrueForAll"/> method traverses the list from the beginning, passing each element in turn to the <c>EndsWithSaurus</c> method. The search stops when the <c>EndsWithSaurus</c> method returns <c>false</c>.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/ListT/Exists/source.cs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ListT/Exists/source.vb" id="Snippet1" />
        /// </remarks>
        public bool TrueForAll(Predicate<T> match)
        {
            if (match == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
            }

            for (int i = 0; i < _size; i++)
            {
                if (!match(_items[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public struct Enumerator : IEnumerator<T>, IEnumerator
        {
            private readonly List<T> _list;
            private readonly int _version;

            private int _index;
            private T? _current;

            internal Enumerator(List<T> list)
            {
                _list = list;
                _version = list._version;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                List<T> localList = _list;

                if (_version != _list._version)
                {
                    ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion();
                }

                if ((uint)_index < (uint)localList._size)
                {
                    _current = localList._items[_index];
                    _index++;
                    return true;
                }

                _current = default;
                _index = -1;
                return false;
            }

            public T Current => _current!;

            object? IEnumerator.Current
            {
                get
                {
                    if (_index <= 0)
                    {
                        ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumOpCantHappen();
                    }

                    return _current;
                }
            }

            void IEnumerator.Reset()
            {
                if (_version != _list._version)
                {
                    ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion();
                }

                _index = 0;
                _current = default;
            }
        }
    }
}

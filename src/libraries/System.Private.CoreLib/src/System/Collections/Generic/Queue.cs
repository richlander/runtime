// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
    /// <summary>
    /// Represents a first-in, first-out collection of objects.
    /// </summary>
    /// <remarks>
    /// Implemented as a circular buffer, so <see cref="Enqueue(T)"/> and <see cref="Dequeue"/> are typically <c>O(1)</c>.
    /// </remarks>
    [DebuggerTypeProxy(typeof(QueueDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    [Serializable]
    [TypeForwardedFrom("System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    public class Queue<T> : IEnumerable<T>,
        ICollection,
        IReadOnlyCollection<T>
    {
        private T[] _array;
        private int _head;       // The index from which to dequeue if the queue isn't empty.
        private int _tail;       // The index at which to enqueue if the queue isn't full.
        private int _size;       // Number of elements.
        private int _version;

        // Creates a queue with room for capacity objects. The default initial
        // capacity and grow factor are used.
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Collections.Generic.Queue`1"/> class that is empty and has the default initial capacity.
        /// </summary>
        /// <remarks>
        /// The capacity of a <see cref="System.Collections.Generic.Queue{T}"/> is the number of elements that the <see cref="System.Collections.Generic.Queue{T}"/> can hold. As elements are added to a <see cref="System.Collections.Generic.Queue{T}"/>, the capacity is automatically increased as required by reallocating the internal array.
        /// If the size of the collection can be estimated, specifying the initial capacity eliminates the need to perform a number of resizing operations while adding elements to the <see cref="System.Collections.Generic.Queue{T}"/>.
        /// The capacity can be decreased by calling <see cref="System.Collections.Generic.Queue{T}.TrimExcess"/>.
        /// This constructor is an O(1) operation.
        /// </remarks>
        public Queue()
        {
            _array = [];
        }

        // Creates a queue with room for capacity objects. The default grow factor
        // is used.
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Collections.Generic.Queue`1"/> class that is empty and has the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the <see cref="T:System.Collections.Generic.Queue`1"/> can contain.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        /// <remarks>
        /// The capacity of a <see cref="System.Collections.Generic.Queue{T}"/> is the number of elements that the <see cref="System.Collections.Generic.Queue{T}"/> can hold. As elements are added to a <see cref="System.Collections.Generic.Queue{T}"/>, the capacity is automatically increased as required by reallocating the internal array.
        /// If the size of the collection can be estimated, specifying the initial capacity eliminates the need to perform a number of resizing operations while adding elements to the <see cref="System.Collections.Generic.Queue{T}"/>.
        /// The capacity can be decreased by calling <see cref="System.Collections.Generic.Queue{T}.TrimExcess"/>.
        /// This constructor is an O(<c>n</c>) operation, where <c>n</c> is <c>capacity</c>.
        /// </remarks>
        public Queue(int capacity)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(capacity);
            _array = new T[capacity];
        }

        // Fills a Queue with the elements of an ICollection.  Uses the enumerator
        // to get each of the elements.
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Collections.Generic.Queue`1"/> class that contains elements copied from the specified collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new <see cref="T:System.Collections.Generic.Queue`1"/>.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="collection"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// The capacity of a <see cref="System.Collections.Generic.Queue{T}"/> is the number of elements that the <see cref="System.Collections.Generic.Queue{T}"/> can hold. As elements are added to a <see cref="System.Collections.Generic.Queue{T}"/>, the capacity is automatically increased as required by reallocating the internal array.
        /// If the size of the collection can be estimated, specifying the initial capacity eliminates the need to perform a number of resizing operations while adding elements to the <see cref="System.Collections.Generic.Queue{T}"/>.
        /// The capacity can be decreased by calling <see cref="System.Collections.Generic.Queue{T}.TrimExcess"/>.
        /// The elements are copied onto the <see cref="System.Collections.Generic.Queue{T}"/> in the same order they are read by the <see cref="System.Collections.Generic.IEnumerator{T}"/> of the collection.
        /// This constructor is an O(<c>n</c>) operation, where <c>n</c> is the number of elements in <c>collection</c>.
        /// </remarks>
        public Queue(IEnumerable<T> collection)
        {
            ArgumentNullException.ThrowIfNull(collection);

            _array = EnumerableHelpers.ToArray(collection, out _size);
            if (_size != _array.Length) _tail = _size;
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.Queue`1"/>.
        /// </summary>
        /// <value>The number of elements contained in the <see cref="T:System.Collections.Generic.Queue`1"/>.</value>
        /// <remarks>
        /// The capacity of a <see cref="System.Collections.Generic.Queue{T}"/> is the number of elements that the <see cref="System.Collections.Generic.Queue{T}"/> can store. <see cref="System.Collections.Generic.Queue{T}.Count"/> is the number of elements that are actually in the <see cref="System.Collections.Generic.Queue{T}"/>.
        /// The capacity is always greater than or equal to <see cref="System.Collections.Generic.Queue{T}.Count"/>. If <see cref="System.Collections.Generic.Queue{T}.Count"/> exceeds the capacity while adding elements, the capacity is increased by automatically reallocating the internal array before copying the old elements and adding the new elements.
        /// Retrieving the value of this property is an O(1) operation.
        /// The following code example demonstrates several properties and methods of the <see cref="System.Collections.Generic.Queue{T}"/> generic class, including the <see cref="System.Collections.Generic.Queue{T}.Count"/> property.
        /// The code example creates a queue of strings with default capacity and uses the <see cref="System.Collections.Generic.Queue{T}.Enqueue"/> method to queue five strings. The elements of the queue are enumerated, which does not change the state of the queue. The <see cref="System.Collections.Generic.Queue{T}.Dequeue"/> method is used to dequeue the first string. The <see cref="System.Collections.Generic.Queue{T}.Peek"/> method is used to look at the next item in the queue, and then the <see cref="System.Collections.Generic.Queue{T}.Dequeue"/> method is used to dequeue it.
        /// The <see cref="System.Collections.Generic.Queue{T}.ToArray"/> method is used to create an array and copy the queue elements to it, then the array is passed to the <see cref="System.Collections.Generic.Queue{T}.#ctor"/> constructor that takes <see cref="System.Collections.Generic.IEnumerable{T}"/>, creating a copy of the queue. The elements of the copy are displayed.
        /// An array twice the size of the queue is created, and the <see cref="System.Collections.Generic.Queue{T}.CopyTo"/> method is used to copy the array elements beginning at the middle of the array. The <see cref="System.Collections.Generic.Queue{T}.#ctor"/> constructor is used again to create a second copy of the queue containing three null elements at the beginning.
        /// The <see cref="System.Collections.Generic.Queue{T}.Contains"/> method is used to show that the string "four" is in the first copy of the queue, after which the <see cref="System.Collections.Generic.Queue{T}.Clear"/> method clears the copy and the <see cref="System.Collections.Generic.Queue{T}.Count"/> property shows that the queue is empty.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/QueueT/Overview/source.cs" id="Snippet1" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/QueueT/Overview/source.fs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/QueueT/Overview/source.vb" id="Snippet1" />
        /// </remarks>
        public int Count => _size;

        /// <summary>
        /// Gets the total numbers of elements the internal data structure can hold without resizing.
        /// </summary>
        public int Capacity => _array.Length;

        /// <inheritdoc cref="ICollection.IsSynchronized" />
        bool ICollection.IsSynchronized => false;

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
        /// </summary>
        /// <value>An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>. In the default implementation of <see cref="T:System.Collections.Generic.Queue`1"/>, this property always returns the current instance.</value>
        /// <remarks>
        /// <code language="csharp">
        /// </code>
        /// <code language="vb">
        /// </code>
        /// Default implementations of collections in <see cref="System.Collections.Generic">Generic</see> are not synchronized.
        /// Enumerating through a collection is intrinsically not a thread-safe procedure.  To guarantee thread safety during enumeration, you can lock the collection during the entire enumeration.  To allow the collection to be accessed by multiple threads for reading and writing, you must implement your own synchronization.
        /// <see cref="System.Collections.ICollection.SyncRoot"/> returns an object, which can be used to synchronize access to the <see cref="System.Collections.ICollection"/>. Synchronization is effective only if all threads lock this object before accessing the collection. The following code shows the use of the <see cref="System.Collections.ICollection.SyncRoot"/> property.
        /// ODE0 
        /// ODE1 
        /// Retrieving the value of this property is an O(1) operation.
        /// </remarks>
        object ICollection.SyncRoot => this;

        // Removes all Objects from the queue.
        /// <summary>
        /// Removes all objects from the <see cref="T:System.Collections.Generic.Queue`1"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="System.Collections.Generic.Queue{T}.Count"/> is set to zero, and references to other objects from elements of the collection are also released.
        /// The capacity remains unchanged. To reset the capacity of the <see cref="System.Collections.Generic.Queue{T}"/>, call <see cref="System.Collections.Generic.Queue{T}.TrimExcess"/>. Trimming an empty <see cref="System.Collections.Generic.Queue{T}"/> sets the capacity of the <see cref="System.Collections.Generic.Queue{T}"/> to the default capacity.
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="System.Collections.Generic.Queue{T}.Count"/>.
        /// The following code example demonstrates several methods of the <see cref="System.Collections.Generic.Queue{T}"/> generic class, including the <see cref="System.Collections.Generic.Queue{T}.Clear"/> method.
        /// The code example creates a queue of strings with default capacity and uses the <see cref="System.Collections.Generic.Queue{T}.Enqueue"/> method to queue five strings. The elements of the queue are enumerated, which does not change the state of the queue. The <see cref="System.Collections.Generic.Queue{T}.Dequeue"/> method is used to dequeue the first string. The <see cref="System.Collections.Generic.Queue{T}.Peek"/> method is used to look at the next item in the queue, and then the <see cref="System.Collections.Generic.Queue{T}.Dequeue"/> method is used to dequeue it.
        /// The <see cref="System.Collections.Generic.Queue{T}.ToArray"/> method is used to create an array and copy the queue elements to it, then the array is passed to the <see cref="System.Collections.Generic.Queue{T}.#ctor"/> constructor that takes <see cref="System.Collections.Generic.IEnumerable{T}"/>, creating a copy of the queue. The elements of the copy are displayed.
        /// An array twice the size of the queue is created, and the <see cref="System.Collections.Generic.Queue{T}.CopyTo"/> method is used to copy the array elements beginning at the middle of the array. The <see cref="System.Collections.Generic.Queue{T}.#ctor"/> constructor is used again to create a second copy of the queue containing three null elements at the beginning.
        /// The <see cref="System.Collections.Generic.Queue{T}.Contains"/> method is used to show that the string "four" is in the first copy of the queue, after which the <see cref="System.Collections.Generic.Queue{T}.Clear"/> method clears the copy and the <see cref="System.Collections.Generic.Queue{T}.Count"/> property shows that the queue is empty.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/QueueT/Overview/source.cs" id="Snippet1" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/QueueT/Overview/source.fs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/QueueT/Overview/source.vb" id="Snippet1" />
        /// </remarks>
        public void Clear()
        {
            if (_size != 0)
            {
                if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
                {
                    if (_head < _tail)
                    {
                        Array.Clear(_array, _head, _size);
                    }
                    else
                    {
                        Array.Clear(_array, _head, _array.Length - _head);
                        Array.Clear(_array, 0, _tail);
                    }
                }

                _size = 0;
            }

            _head = 0;
            _tail = 0;
            _version++;
        }

        // CopyTo copies a collection into an Array, starting at a particular
        // index into the array.
        /// <summary>
        /// Copies the <see cref="T:System.Collections.Generic.Queue`1"/> elements to an existing one-dimensional <see cref="T:System.Array"/>, starting at the specified array index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.Queue`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than zero.</exception>
        /// <exception cref="T:System.ArgumentException">The number of elements in the source <see cref="T:System.Collections.Generic.Queue`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.</exception>
        /// <remarks>
        /// The elements are copied to the <see cref="System.Array"/> in the same order in which the enumerator iterates through the <see cref="System.Collections.Generic.Queue{T}"/>.
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="System.Collections.Generic.Queue{T}.Count"/>.
        /// </remarks>
        public void CopyTo(T[] array, int arrayIndex)
        {
            ArgumentNullException.ThrowIfNull(array);

            if (arrayIndex < 0 || arrayIndex > array.Length)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.arrayIndex, ExceptionResource.ArgumentOutOfRange_IndexMustBeLessOrEqual);
            }

            if (array.Length - arrayIndex < _size)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);
            }

            int numToCopy = _size;
            if (numToCopy == 0) return;

            int firstPart = Math.Min(_array.Length - _head, numToCopy);
            Array.Copy(_array, _head, array, arrayIndex, firstPart);
            numToCopy -= firstPart;
            if (numToCopy > 0)
            {
                Array.Copy(_array, 0, array, arrayIndex + _array.Length - _head, numToCopy);
            }
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.ICollection"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than zero.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="array"/> is multidimensional. -or- <paramref name="array"/> does not have zero-based indexing. -or- The number of elements in the source <see cref="T:System.Collections.ICollection"/> is greater than the available space from <paramref name="index"/> to the end of the destination <paramref name="array"/>. -or- The type of the source <see cref="T:System.Collections.ICollection"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.</exception>
        /// <remarks>
        /// <note type="note">
        /// If the type of the source <see cref="System.Collections.ICollection"/> cannot be cast automatically to the type of the destination <c>array</c>, the non-generic implementations of <see cref="System.Collections.ICollection.CopyTo">CopyTo</see> throw <see cref="System.InvalidCastException"/>, whereas the generic implementations throw <see cref="System.ArgumentException"/>.
        /// </note>
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="System.Collections.Generic.Queue{T}.Count"/>.
        /// </remarks>
        void ICollection.CopyTo(Array array, int index)
        {
            ArgumentNullException.ThrowIfNull(array);

            if (array.Rank != 1)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RankMultiDimNotSupported, ExceptionArgument.array);
            }

            if (array.GetLowerBound(0) != 0)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_NonZeroLowerBound, ExceptionArgument.array);
            }

            int arrayLen = array.Length;
            if (index < 0 || index > arrayLen)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException();
            }

            if (arrayLen - index < _size)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);
            }

            int numToCopy = _size;
            if (numToCopy == 0) return;

            try
            {
                int firstPart = (_array.Length - _head < numToCopy) ? _array.Length - _head : numToCopy;
                Array.Copy(_array, _head, array, index, firstPart);
                numToCopy -= firstPart;

                if (numToCopy > 0)
                {
                    Array.Copy(_array, 0, array, index + _array.Length - _head, numToCopy);
                }
            }
            catch (ArrayTypeMismatchException)
            {
                ThrowHelper.ThrowArgumentException_Argument_IncompatibleArrayType();
            }
        }

        // Adds item to the tail of the queue.
        /// <summary>
        /// Adds an object to the end of the <see cref="T:System.Collections.Generic.Queue`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.Queue`1"/>. The value can be <see langword="null"/> for reference types.</param>
        /// <remarks>
        /// If <see cref="System.Collections.Generic.Queue{T}.Count"/> already equals the capacity, the capacity of the <see cref="System.Collections.Generic.Queue{T}"/> is increased by automatically reallocating the internal array, and the existing elements are copied to the new array before the new element is added.
        /// If <see cref="System.Collections.Generic.Queue{T}.Count"/> is less than the capacity of the internal array, this method is an O(1) operation. If the internal array needs to be reallocated to accommodate the new element, this method becomes an O(<c>n</c>) operation, where <c>n</c> is <see cref="System.Collections.Generic.Queue{T}.Count"/>.
        /// The following code example demonstrates several methods of the <see cref="System.Collections.Generic.Queue{T}"/> generic class, including the <see cref="System.Collections.Generic.Queue{T}.Enqueue"/> method.
        /// The code example creates a queue of strings with default capacity and uses the <see cref="System.Collections.Generic.Queue{T}.Enqueue"/> method to queue five strings. The elements of the queue are enumerated, which does not change the state of the queue. The <see cref="System.Collections.Generic.Queue{T}.Dequeue"/> method is used to dequeue the first string. The <see cref="System.Collections.Generic.Queue{T}.Peek"/> method is used to look at the next item in the queue, and then the <see cref="System.Collections.Generic.Queue{T}.Dequeue"/> method is used to dequeue it.
        /// The <see cref="System.Collections.Generic.Queue{T}.ToArray"/> method is used to create an array and copy the queue elements to it, then the array is passed to the <see cref="System.Collections.Generic.Queue{T}.#ctor"/> constructor that takes <see cref="System.Collections.Generic.IEnumerable{T}"/>, creating a copy of the queue. The elements of the copy are displayed.
        /// An array twice the size of the queue is created, and the <see cref="System.Collections.Generic.Queue{T}.CopyTo"/> method is used to copy the array elements beginning at the middle of the array. The <see cref="System.Collections.Generic.Queue{T}.#ctor"/> constructor is used again to create a second copy of the queue containing three null elements at the beginning.
        /// The <see cref="System.Collections.Generic.Queue{T}.Contains"/> method is used to show that the string "four" is in the first copy of the queue, after which the <see cref="System.Collections.Generic.Queue{T}.Clear"/> method clears the copy and the <see cref="System.Collections.Generic.Queue{T}.Count"/> property shows that the queue is empty.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/QueueT/Overview/source.cs" id="Snippet1" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/QueueT/Overview/source.fs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/QueueT/Overview/source.vb" id="Snippet1" />
        /// </remarks>
        public void Enqueue(T item)
        {
            if (_size == _array.Length)
            {
                Grow(_size + 1);
            }

            _array[_tail] = item;
            MoveNext(ref _tail);
            _size++;
            _version++;
        }

        // GetEnumerator returns an IEnumerator over this Queue.  This
        // Enumerator will support removing.
        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="T:System.Collections.Generic.Queue`1"/>.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.Generic.Queue`1.Enumerator"/> for the <see cref="T:System.Collections.Generic.Queue`1"/>.</returns>
        /// <remarks>
        /// The <c>foreach</c> statement of the C# language (<c>For Each</c> in Visual Basic) hides the complexity of the enumerators. Therefore, using <c>foreach</c> is recommended, instead of directly manipulating the enumerator.
        /// Enumerators can be used to read the data in the collection, but they cannot be used to modify the underlying collection.
        /// Initially, the enumerator is positioned before the first element in the collection. At this position, <see cref="System.Collections.Generic.Queue{T}.Enumerator.Current"/> is undefined. Therefore, you must call <see cref="System.Collections.Generic.Queue{T}.Enumerator.MoveNext"/> to advance the enumerator to the first element of the collection before reading the value of <see cref="System.Collections.Generic.Queue{T}.Enumerator.Current"/>.
        /// <see cref="System.Collections.Generic.Queue{T}.Enumerator.Current"/> returns the same object until <see cref="System.Collections.Generic.Queue{T}.Enumerator.MoveNext"/> is called. <see cref="System.Collections.Generic.Queue{T}.Enumerator.MoveNext"/> sets <see cref="System.Collections.Generic.Queue{T}.Enumerator.Current"/> to the next element.
        /// If <see cref="System.Collections.Generic.Queue{T}.Enumerator.MoveNext"/> passes the end of the collection, the enumerator is positioned after the last element in the collection and <see cref="System.Collections.Generic.Queue{T}.Enumerator.MoveNext"/> returns <c>false</c>. When the enumerator is at this position, subsequent calls to <see cref="System.Collections.Generic.Queue{T}.Enumerator.MoveNext"/> also return <c>false</c>. If the last call to <see cref="System.Collections.Generic.Queue{T}.Enumerator.MoveNext"/> returned <c>false</c>, <see cref="System.Collections.Generic.Queue{T}.Enumerator.Current"/> is undefined. You cannot set <see cref="System.Collections.Generic.Queue{T}.Enumerator.Current"/> to the first element of the collection again; you must create a new enumerator instance instead.
        /// An enumerator remains valid as long as the collection remains unchanged. If changes are made to the collection, such as adding, modifying, or deleting elements, the enumerator is irrecoverably invalidated and the next call to <see cref="System.Collections.Generic.Queue{T}.Enumerator.MoveNext"/> or <see cref="System.Collections.Generic.Queue{T}.Enumerator.System#Collections#IEnumerator#Reset"/> throws an <see cref="System.InvalidOperationException"/>.
        /// The enumerator does not have exclusive access to the collection; therefore, enumerating through a collection is intrinsically not a thread-safe procedure. To guarantee thread safety during enumeration, you can lock the collection during the entire enumeration.  To allow the collection to be accessed by multiple threads for reading and writing, you must implement your own synchronization.
        /// Default implementations of collections in <see cref="System.Collections.Generic">Generic</see> are not synchronized.
        /// This method is an O(1) operation.
        /// The following code example demonstrates that the <see cref="System.Collections.Generic.Queue{T}"/> generic class is enumerable. The <c>foreach</c> statement (<c>For Each</c> in Visual Basic) is used to enumerate the queue.
        /// The code example creates a queue of strings with default capacity and uses the <see cref="System.Collections.Generic.Queue{T}.Enqueue"/> method to queue five strings. The elements of the queue are enumerated, which does not change the state of the queue. The <see cref="System.Collections.Generic.Queue{T}.Dequeue"/> method is used to dequeue the first string. The <see cref="System.Collections.Generic.Queue{T}.Peek"/> method is used to look at the next item in the queue, and then the <see cref="System.Collections.Generic.Queue{T}.Dequeue"/> method is used to dequeue it.
        /// The <see cref="System.Collections.Generic.Queue{T}.ToArray"/> method is used to create an array and copy the queue elements to it, then the array is passed to the <see cref="System.Collections.Generic.Queue{T}.#ctor"/> constructor that takes <see cref="System.Collections.Generic.IEnumerable{T}"/>, creating a copy of the queue. The elements of the copy are displayed.
        /// An array twice the size of the queue is created, and the <see cref="System.Collections.Generic.Queue{T}.CopyTo"/> method is used to copy the array elements beginning at the middle of the array. The <see cref="System.Collections.Generic.Queue{T}.#ctor"/> constructor is used again to create a second copy of the queue containing three null elements at the beginning.
        /// The <see cref="System.Collections.Generic.Queue{T}.Contains"/> method is used to show that the string "four" is in the first copy of the queue, after which the <see cref="System.Collections.Generic.Queue{T}.Clear"/> method clears the copy and the <see cref="System.Collections.Generic.Queue{T}.Count"/> property shows that the queue is empty.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/QueueT/Overview/source.cs" id="Snippet1" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/QueueT/Overview/source.fs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/QueueT/Overview/source.vb" id="Snippet1" />
        /// </remarks>
        public Enumerator GetEnumerator() => new Enumerator(this);

        /// <internalonly/>
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
        /// Initially, the enumerator is positioned before the first element in the collection. <see cref="System.Collections.IEnumerator.Reset"/> also brings the enumerator back to this position.  At this position, <see cref="System.Collections.IEnumerator.Current"/> is undefined. Therefore, you must call <see cref="System.Collections.IEnumerator.MoveNext"/> to advance the enumerator to the first element of the collection before reading the value of <see cref="System.Collections.IEnumerator.Current"/>.
        /// <see cref="System.Collections.IEnumerator.Current"/> returns the same object until either <see cref="System.Collections.IEnumerator.MoveNext"/> or <see cref="System.Collections.IEnumerator.Reset"/> is called. <see cref="System.Collections.IEnumerator.MoveNext"/> sets <see cref="System.Collections.IEnumerator.Current"/> to the next element.
        /// If <see cref="System.Collections.IEnumerator.MoveNext"/> passes the end of the collection, the enumerator is positioned after the last element in the collection and <see cref="System.Collections.IEnumerator.MoveNext"/> returns <c>false</c>. When the enumerator is at this position, subsequent calls to <see cref="System.Collections.IEnumerator.MoveNext"/> also return <c>false</c>. If the last call to <see cref="System.Collections.IEnumerator.MoveNext"/> returned <c>false</c>, <see cref="System.Collections.IEnumerator.Current"/> is undefined. To set <see cref="System.Collections.IEnumerator.Current"/> to the first element of the collection again, you can call <see cref="System.Collections.IEnumerator.Reset"/> followed by <see cref="System.Collections.IEnumerator.MoveNext"/>.
        /// An enumerator remains valid as long as the collection remains unchanged. If changes are made to the collection, such as adding, modifying, or deleting elements, the enumerator is irrecoverably invalidated and the next call to <see cref="System.Collections.IEnumerator.MoveNext"/> or <see cref="System.Collections.IEnumerator.Reset"/> throws an <see cref="System.InvalidOperationException"/>.
        /// The enumerator does not have exclusive access to the collection; therefore, enumerating through a collection is intrinsically not a thread-safe procedure.  To guarantee thread safety during enumeration, you can lock the collection during the entire enumeration.  To allow the collection to be accessed by multiple threads for reading and writing, you must implement your own synchronization.
        /// Default implementations of collections in <see cref="System.Collections.Generic">Generic</see> are not synchronized.
        /// This method is an O(1) operation.
        /// </remarks>
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)this).GetEnumerator();

        // Removes the object at the head of the queue and returns it. If the queue
        // is empty, this method throws an
        // InvalidOperationException.
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="T:System.Collections.Generic.Queue`1"/>.
        /// </summary>
        /// <returns>The object that is removed from the beginning of the <see cref="T:System.Collections.Generic.Queue`1"/>.</returns>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Collections.Generic.Queue`1"/> is empty.</exception>
        /// <remarks>
        /// This method is similar to the <see cref="System.Collections.Generic.Queue{T}.Peek"/> method, but <see cref="System.Collections.Generic.Queue{T}.Peek"/> does not modify the <see cref="System.Collections.Generic.Queue{T}"/>.
        /// If type <c>T</c> is a reference type, <c>null</c> can be added to the <see cref="System.Collections.Generic.Queue{T}"/> as a value.
        /// This method is an O(1) operation.
        /// The following code example demonstrates several methods of the <see cref="System.Collections.Generic.Queue{T}"/> generic class, including the <see cref="System.Collections.Generic.Queue{T}.Dequeue"/> method.
        /// The code example creates a queue of strings with default capacity and uses the <see cref="System.Collections.Generic.Queue{T}.Enqueue"/> method to queue five strings. The elements of the queue are enumerated, which does not change the state of the queue. The <see cref="System.Collections.Generic.Queue{T}.Dequeue"/> method is used to dequeue the first string. The <see cref="System.Collections.Generic.Queue{T}.Peek"/> method is used to look at the next item in the queue, and then the <see cref="System.Collections.Generic.Queue{T}.Dequeue"/> method is used to dequeue it.
        /// The <see cref="System.Collections.Generic.Queue{T}.ToArray"/> method is used to create an array and copy the queue elements to it, then the array is passed to the <see cref="System.Collections.Generic.Queue{T}.#ctor"/> constructor that takes <see cref="System.Collections.Generic.IEnumerable{T}"/>, creating a copy of the queue. The elements of the copy are displayed.
        /// An array twice the size of the queue is created, and the <see cref="System.Collections.Generic.Queue{T}.CopyTo"/> method is used to copy the array elements beginning at the middle of the array. The <see cref="System.Collections.Generic.Queue{T}.#ctor"/> constructor is used again to create a second copy of the queue containing three null elements at the beginning.
        /// The <see cref="System.Collections.Generic.Queue{T}.Contains"/> method is used to show that the string "four" is in the first copy of the queue, after which the <see cref="System.Collections.Generic.Queue{T}.Clear"/> method clears the copy and the <see cref="System.Collections.Generic.Queue{T}.Count"/> property shows that the queue is empty.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/QueueT/Overview/source.cs" id="Snippet1" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/QueueT/Overview/source.fs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/QueueT/Overview/source.vb" id="Snippet1" />
        /// </remarks>
        public T Dequeue()
        {
            int head = _head;
            T[] array = _array;

            if (_size == 0)
            {
                ThrowForEmptyQueue();
            }

            T removed = array[head];
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                array[head] = default!;
            }
            MoveNext(ref _head);
            _size--;
            _version++;
            return removed;
        }

        /// <summary>
        /// Removes the object at the beginning of the <see cref="T:System.Collections.Generic.Queue`1"/>, and copies it to the <paramref name="result"/> parameter.
        /// </summary>
        /// <param name="result">The removed object.</param>
        /// <returns><see langword="true"/> if the object is successfully removed; <see langword="false"/> if the <see cref="T:System.Collections.Generic.Queue`1"/> is empty.</returns>
        public bool TryDequeue([MaybeNullWhen(false)] out T result)
        {
            int head = _head;
            T[] array = _array;

            if (_size == 0)
            {
                result = default;
                return false;
            }

            result = array[head];
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                array[head] = default!;
            }
            MoveNext(ref _head);
            _size--;
            _version++;
            return true;
        }

        // Returns the object at the head of the queue. The object remains in the
        // queue. If the queue is empty, this method throws an
        // InvalidOperationException.
        /// <summary>
        /// Returns the object at the beginning of the <see cref="T:System.Collections.Generic.Queue`1"/> without removing it.
        /// </summary>
        /// <returns>The object at the beginning of the <see cref="T:System.Collections.Generic.Queue`1"/>.</returns>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Collections.Generic.Queue`1"/> is empty.</exception>
        /// <remarks>
        /// This method is similar to the <see cref="System.Collections.Generic.Queue{T}.Dequeue"/> method, but <see cref="System.Collections.Generic.Queue{T}.Peek"/> does not modify the <see cref="System.Collections.Generic.Queue{T}"/>.
        /// If type <c>T</c> is a reference type, <c>null</c> can be added to the <see cref="System.Collections.Generic.Queue{T}"/> as a value.
        /// This method is an O(1) operation.
        /// The following code example demonstrates several methods of the <see cref="System.Collections.Generic.Queue{T}"/> generic class, including the <see cref="System.Collections.Generic.Queue{T}.Peek"/> method.
        /// The code example creates a queue of strings with default capacity and uses the <see cref="System.Collections.Generic.Queue{T}.Enqueue"/> method to queue five strings. The elements of the queue are enumerated, which does not change the state of the queue. The <see cref="System.Collections.Generic.Queue{T}.Dequeue"/> method is used to dequeue the first string. The <see cref="System.Collections.Generic.Queue{T}.Peek"/> method is used to look at the next item in the queue, and then the <see cref="System.Collections.Generic.Queue{T}.Dequeue"/> method is used to dequeue it.
        /// The <see cref="System.Collections.Generic.Queue{T}.ToArray"/> method is used to create an array and copy the queue elements to it, then the array is passed to the <see cref="System.Collections.Generic.Queue{T}.#ctor"/> constructor that takes <see cref="System.Collections.Generic.IEnumerable{T}"/>, creating a copy of the queue. The elements of the copy are displayed.
        /// An array twice the size of the queue is created, and the <see cref="System.Collections.Generic.Queue{T}.CopyTo"/> method is used to copy the array elements beginning at the middle of the array. The <see cref="System.Collections.Generic.Queue{T}.#ctor"/> constructor is used again to create a second copy of the queue containing three null elements at the beginning.
        /// The <see cref="System.Collections.Generic.Queue{T}.Contains"/> method is used to show that the string "four" is in the first copy of the queue, after which the <see cref="System.Collections.Generic.Queue{T}.Clear"/> method clears the copy and the <see cref="System.Collections.Generic.Queue{T}.Count"/> property shows that the queue is empty.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/QueueT/Overview/source.cs" id="Snippet1" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/QueueT/Overview/source.fs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/QueueT/Overview/source.vb" id="Snippet1" />
        /// </remarks>
        public T Peek()
        {
            if (_size == 0)
            {
                ThrowForEmptyQueue();
            }

            return _array[_head];
        }

        /// <summary>
        /// Returns a value that indicates whether there is an object at the beginning of the <see cref="T:System.Collections.Generic.Queue`1"/>, and if one is present, copies it to the <paramref name="result"/> parameter. The object is not removed from the <see cref="T:System.Collections.Generic.Queue`1"/>.
        /// </summary>
        /// <param name="result">If present, the object at the beginning of the <see cref="T:System.Collections.Generic.Queue`1"/>; otherwise, the default value of <typeparamref name="T"/>.</param>
        /// <returns><see langword="true"/> if there is an object at the beginning of the <see cref="T:System.Collections.Generic.Queue`1"/>; <see langword="false"/> if the <see cref="T:System.Collections.Generic.Queue`1"/> is empty.</returns>
        public bool TryPeek([MaybeNullWhen(false)] out T result)
        {
            if (_size == 0)
            {
                result = default;
                return false;
            }

            result = _array[_head];
            return true;
        }

        // Returns true if the queue contains at least one object equal to item.
        // Equality is determined using EqualityComparer<T>.Default.Equals().
        /// <summary>
        /// Determines whether an element is in the <see cref="T:System.Collections.Generic.Queue`1"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.Queue`1"/>. The value can be <see langword="null"/> for reference types.</param>
        /// <returns><see langword="true"/> if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.Queue`1"/>; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This method determines equality using the default equality comparer <see cref="System.Collections.Generic.EqualityComparer{T}.Default">Default</see> for <c>T</c>, the type of values in the queue.
        /// This method performs a linear search; therefore, this method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="System.Collections.Generic.Queue{T}.Count"/>.
        /// The following code example demonstrates several methods of the <see cref="System.Collections.Generic.Queue{T}"/> generic class, including the <see cref="System.Collections.Generic.Queue{T}.Contains"/> method.
        /// The code example creates a queue of strings with default capacity and uses the <see cref="System.Collections.Generic.Queue{T}.Enqueue"/> method to queue five strings. The elements of the queue are enumerated, which does not change the state of the queue. The <see cref="System.Collections.Generic.Queue{T}.Dequeue"/> method is used to dequeue the first string. The <see cref="System.Collections.Generic.Queue{T}.Peek"/> method is used to look at the next item in the queue, and then the <see cref="System.Collections.Generic.Queue{T}.Dequeue"/> method is used to dequeue it.
        /// The <see cref="System.Collections.Generic.Queue{T}.ToArray"/> method is used to create an array and copy the queue elements to it, then the array is passed to the <see cref="System.Collections.Generic.Queue{T}.#ctor"/> constructor that takes <see cref="System.Collections.Generic.IEnumerable{T}"/>, creating a copy of the queue. The elements of the copy are displayed.
        /// An array twice the size of the queue is created, and the <see cref="System.Collections.Generic.Queue{T}.CopyTo"/> method is used to copy the array elements beginning at the middle of the array. The <see cref="System.Collections.Generic.Queue{T}.#ctor"/> constructor is used again to create a second copy of the queue containing three null elements at the beginning.
        /// The <see cref="System.Collections.Generic.Queue{T}.Contains"/> method is used to show that the string "four" is in the first copy of the queue, after which the <see cref="System.Collections.Generic.Queue{T}.Clear"/> method clears the copy and the <see cref="System.Collections.Generic.Queue{T}.Count"/> property shows that the queue is empty.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/QueueT/Overview/source.cs" id="Snippet1" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/QueueT/Overview/source.fs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/QueueT/Overview/source.vb" id="Snippet1" />
        /// </remarks>
        /// <related type="Article" href="/dotnet/standard/globalization-localization/performing-culture-insensitive-string-operations">Performing Culture-Insensitive String Operations</related>
        public bool Contains(T item)
        {
            if (_size == 0)
            {
                return false;
            }

            if (_head < _tail)
            {
                return Array.IndexOf(_array, item, _head, _size) >= 0;
            }

            // We've wrapped around. Check both partitions, the least recently enqueued first.
            return
                Array.IndexOf(_array, item, _head, _array.Length - _head) >= 0 ||
                Array.IndexOf(_array, item, 0, _tail) >= 0;
        }

        // Iterates over the objects in the queue, returning an array of the
        // objects in the Queue, or an empty array if the queue is empty.
        // The order of elements in the array is first in to last in, the same
        // order produced by successive calls to Dequeue.
        /// <summary>
        /// Copies the <see cref="T:System.Collections.Generic.Queue`1"/> elements to a new array.
        /// </summary>
        /// <returns>A new array containing elements copied from the <see cref="T:System.Collections.Generic.Queue`1"/>.</returns>
        /// <remarks>
        /// The <see cref="System.Collections.Generic.Queue{T}"/> is not modified. The order of the elements in the new array is the same as the order of the elements from the beginning of the <see cref="System.Collections.Generic.Queue{T}"/> to its end.
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="System.Collections.Generic.Queue{T}.Count"/>.
        /// The following code example demonstrates several methods of the <see cref="System.Collections.Generic.Queue{T}"/> generic class, including the <see cref="System.Collections.Generic.Queue{T}.ToArray"/> method.
        /// The code example creates a queue of strings with default capacity and uses the <see cref="System.Collections.Generic.Queue{T}.Enqueue"/> method to queue five strings. The elements of the queue are enumerated, which does not change the state of the queue. The <see cref="System.Collections.Generic.Queue{T}.Dequeue"/> method is used to dequeue the first string. The <see cref="System.Collections.Generic.Queue{T}.Peek"/> method is used to look at the next item in the queue, and then the <see cref="System.Collections.Generic.Queue{T}.Dequeue"/> method is used to dequeue it.
        /// The <see cref="System.Collections.Generic.Queue{T}.ToArray"/> method is used to create an array and copy the queue elements to it, then the array is passed to the <see cref="System.Collections.Generic.Queue{T}.#ctor"/> constructor that takes <see cref="System.Collections.Generic.IEnumerable{T}"/>, creating a copy of the queue. The elements of the copy are displayed.
        /// An array twice the size of the queue is created, and the <see cref="System.Collections.Generic.Queue{T}.CopyTo"/> method is used to copy the array elements beginning at the middle of the array. The <see cref="System.Collections.Generic.Queue{T}.#ctor"/> constructor is used again to create a second copy of the queue containing three null elements at the beginning.
        /// The <see cref="System.Collections.Generic.Queue{T}.Contains"/> method is used to show that the string "four" is in the first copy of the queue, after which the <see cref="System.Collections.Generic.Queue{T}.Clear"/> method clears the copy and the <see cref="System.Collections.Generic.Queue{T}.Count"/> property shows that the queue is empty.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/QueueT/Overview/source.cs" id="Snippet1" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/QueueT/Overview/source.fs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/QueueT/Overview/source.vb" id="Snippet1" />
        /// </remarks>
        public T[] ToArray()
        {
            if (_size == 0)
            {
                return [];
            }

            T[] arr = new T[_size];

            if (_head < _tail)
            {
                Array.Copy(_array, _head, arr, 0, _size);
            }
            else
            {
                Array.Copy(_array, _head, arr, 0, _array.Length - _head);
                Array.Copy(_array, 0, arr, _array.Length - _head, _tail);
            }

            return arr;
        }

        // PRIVATE Grows or shrinks the buffer to hold capacity objects. Capacity
        // must be >= _size.
        private void SetCapacity(int capacity)
        {
            Debug.Assert(capacity >= _size);
            T[] newarray = new T[capacity];
            if (_size > 0)
            {
                if (_head < _tail)
                {
                    Array.Copy(_array, _head, newarray, 0, _size);
                }
                else
                {
                    Array.Copy(_array, _head, newarray, 0, _array.Length - _head);
                    Array.Copy(_array, 0, newarray, _array.Length - _head, _tail);
                }
            }

            _array = newarray;
            _head = 0;
            _tail = (_size == capacity) ? 0 : _size;
            _version++;
        }

        // Increments the index wrapping it if necessary.
        private void MoveNext(ref int index)
        {
            // It is tempting to use the remainder operator here but it is actually much slower
            // than a simple comparison and a rarely taken branch.
            // JIT produces better code than with ternary operator ?:
            int tmp = index + 1;
            if (tmp == _array.Length)
            {
                tmp = 0;
            }
            index = tmp;
        }

        private void ThrowForEmptyQueue()
        {
            Debug.Assert(_size == 0);
            throw new InvalidOperationException(SR.InvalidOperation_EmptyQueue);
        }

        /// <summary>
        /// Sets the capacity to the actual number of elements in the <see cref="T:System.Collections.Generic.Queue`1"/>, if that number is less than 90 percent of current capacity.
        /// </summary>
        /// <remarks>
        /// This method can be used to minimize a collection's memory overhead if no new elements will be added to the collection. The cost of reallocating and copying a large <see cref="System.Collections.Generic.Queue{T}"/> can be considerable, however, so the <see cref="System.Collections.Generic.Queue{T}.TrimExcess"/> method does nothing if the list is at more than 90 percent of capacity. This avoids incurring a large reallocation cost for a relatively small gain.
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="System.Collections.Generic.Queue{T}.Count"/>.
        /// To reset a <see cref="System.Collections.Generic.Queue{T}"/> to its initial state, call the <see cref="System.Collections.Generic.Queue{T}.Clear"/> method before calling <see cref="System.Collections.Generic.Queue{T}.TrimExcess"/> method. Trimming an empty <see cref="System.Collections.Generic.Queue{T}"/> sets the capacity of the <see cref="System.Collections.Generic.Queue{T}"/> to the default capacity.
        /// </remarks>
        public void TrimExcess()
        {
            int threshold = (int)(_array.Length * 0.9);
            if (_size < threshold)
            {
                SetCapacity(_size);
            }
        }

        /// <summary>
        /// Sets the capacity of a <see cref="Queue{T}"/> object to the specified number of entries.
        /// </summary>
        /// <param name="capacity">The new capacity.</param>
        /// <exception cref="ArgumentOutOfRangeException">Passed capacity is lower than entries count.</exception>
        public void TrimExcess(int capacity)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(capacity);
            ArgumentOutOfRangeException.ThrowIfLessThan(capacity, _size);

            if (capacity == _array.Length)
                return;

            SetCapacity(capacity);
        }

        /// <summary>
        /// Ensures that the capacity of this Queue is at least the specified <paramref name="capacity"/>.
        /// </summary>
        /// <param name="capacity">The minimum capacity to ensure.</param>
        /// <returns>The new capacity of this queue.</returns>
        public int EnsureCapacity(int capacity)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(capacity);

            if (_array.Length < capacity)
            {
                Grow(capacity);
            }

            return _array.Length;
        }

        private void Grow(int capacity)
        {
            Debug.Assert(_array.Length < capacity);

            const int GrowFactor = 2;
            const int MinimumGrow = 4;

            int newcapacity = GrowFactor * _array.Length;

            // Allow the list to grow to maximum possible capacity (~2G elements) before encountering overflow.
            // Note that this check works even when _items.Length overflowed thanks to the (uint) cast
            if ((uint)newcapacity > Array.MaxLength) newcapacity = Array.MaxLength;

            // Ensure minimum growth is respected.
            newcapacity = Math.Max(newcapacity, _array.Length + MinimumGrow);

            // If the computed capacity is still less than specified, set to the original argument.
            // Capacities exceeding Array.MaxLength will be surfaced as OutOfMemoryException by Array.Resize.
            if (newcapacity < capacity) newcapacity = capacity;

            SetCapacity(newcapacity);
        }

        // Implements an enumerator for a Queue.  The enumerator uses the
        // internal version number of the list to ensure that no modifications are
        // made to the list while an enumeration is in progress.
        public struct Enumerator : IEnumerator<T>,
            IEnumerator
        {
            private readonly Queue<T> _queue;
            private readonly int _version;
            private int _i;
            private T? _currentElement;

            internal Enumerator(Queue<T> queue)
            {
                _queue = queue;
                _version = queue._version;
                _i = -1;
                _currentElement = default;
            }

            public void Dispose()
            {
                _i = -2;
                _currentElement = default;
            }

            public bool MoveNext()
            {
                if (_version != _queue._version)
                {
                    ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion();
                }

                Queue<T> q = _queue;
                int size = q._size;

                int offset = _i + 1;
                if ((uint)offset < (uint)size)
                {
                    _i = offset;

                    T[] array = q._array;
                    int index = q._head + offset;
                    if ((uint)index < (uint)array.Length)
                    {
                        _currentElement = array[index];
                    }
                    else
                    {
                        // The index has wrapped around the end of the array. Shift the index and then
                        // get the current element. It is tempting to dedup this dereferencing with that
                        // in the if block above, but the if block above avoids a bounds check for the
                        // accesses that are in that portion, whereas these still incur it.
                        index -= array.Length;
                        _currentElement = array[index];
                    }

                    return true;
                }

                _i = -2;
                _currentElement = default;
                return false;
            }

            public T Current => _currentElement!;

            object? IEnumerator.Current => Current;

            void IEnumerator.Reset()
            {
                if (_version != _queue._version)
                {
                    ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion();
                }

                _i = -1;
                _currentElement = default;
            }
        }
    }
}

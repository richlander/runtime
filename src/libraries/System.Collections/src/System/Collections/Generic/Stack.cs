// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

/*=============================================================================
**
**
** Purpose: An array implementation of a generic stack.
**
**
=============================================================================*/

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
    // A simple stack of objects.  Internally it is implemented as an array,
    // so Push can be O(n).  Pop is O(1).

    /// <summary>
    /// Represents a variable size last-in-first-out (LIFO) collection of instances of the same specified type.
    /// </summary>
    /// <typeparam name="T">Specifies the type of elements in the stack.</typeparam>
    /// <remarks>
    /// <see cref="System.Collections.Generic.Stack{T}"/> is implemented as an array.
    /// Stacks and queues are useful when you need temporary storage for information; that is, when you might want to discard an element after retrieving its value. Use <see cref="System.Collections.Generic.Queue{T}"/> if you need to access the information in the same order that it is stored in the collection. Use <see cref="System.Collections.Generic.Stack{T}">Stack{T}</see> if you need to access the information in reverse order.
    /// Use the <see cref="System.Collections.Concurrent.ConcurrentStack{T}">ConcurrentStack{T}</see> and <see cref="System.Collections.Concurrent.ConcurrentQueue{T}">ConcurrentQueue{T}</see> types when you need to access the collection from multiple threads concurrently.
    /// A common use for <see cref="System.Collections.Generic.Stack{T}">Stack{T}</see> is to preserve variable states during calls to other procedures.
    /// Three main operations can be performed on a <see cref="System.Collections.Generic.Stack{T}">Stack{T}</see> and its elements:
    /// -   <see cref="System.Collections.Generic.Stack{T}.Push"/> inserts an element at the top of the <see cref="System.Collections.Generic.Stack{T}"/>.
    /// -   <see cref="System.Collections.Generic.Stack{T}.Pop"/> removes an element from the top of the <see cref="System.Collections.Generic.Stack{T}"/>.
    /// -   <see cref="System.Collections.Generic.Stack{T}.Peek"/> returns an element that is at the top of the <see cref="System.Collections.Generic.Stack{T}"/> but does not remove it from the <see cref="System.Collections.Generic.Stack{T}"/>.
    /// The capacity of a <see cref="System.Collections.Generic.Stack{T}"/> is the number of elements the <see cref="System.Collections.Generic.Stack{T}"/> can hold. As elements are added to a <see cref="System.Collections.Generic.Stack{T}"/>, the capacity is automatically increased as required by reallocating the internal array. The capacity can be decreased by calling <see cref="System.Collections.Generic.Stack{T}.TrimExcess"/>.
    /// If <see cref="System.Collections.Generic.Stack{T}.Count"/> is less than the capacity of the stack, <see cref="System.Collections.Generic.Stack{T}.Push"/> is an O(1) operation. If the capacity needs to be increased to accommodate the new element, <see cref="System.Collections.Generic.Stack{T}.Push"/> becomes an O(<c>n</c>) operation, where <c>n</c> is <see cref="System.Collections.Generic.Stack{T}.Count"/>. <see cref="System.Collections.Generic.Stack{T}.Pop"/> is an O(1) operation.
    /// <see cref="System.Collections.Generic.Stack{T}"/> accepts <c>null</c> as a valid value for reference types and allows duplicate elements.
    /// The following code example demonstrates several methods of the <see cref="System.Collections.Generic.Stack{T}"/> generic class. The code example creates a stack of strings with default capacity and uses the <see cref="System.Collections.Generic.Stack{T}.Push"/> method to push five strings onto the stack. The elements of the stack are enumerated, which does not change the state of the stack. The <see cref="System.Collections.Generic.Stack{T}.Pop"/> method is used to pop the first string off the stack. The <see cref="System.Collections.Generic.Stack{T}.Peek"/> method is used to look at the next item on the stack, and then the <see cref="System.Collections.Generic.Stack{T}.Pop"/> method is used to pop it off.
    /// The <see cref="System.Collections.Generic.Stack{T}.ToArray"/> method is used to create an array and copy the stack elements to it, then the array is passed to the <see cref="System.Collections.Generic.Stack{T}.#ctor"/> constructor that takes <see cref="System.Collections.Generic.IEnumerable{T}"/>, creating a copy of the stack with the order of the elements reversed. The elements of the copy are displayed.
    /// An array twice the size of the stack is created, and the <see cref="System.Collections.Generic.Stack{T}.CopyTo"/> method is used to copy the array elements beginning at the middle of the array. The <see cref="System.Collections.Generic.Stack{T}.#ctor"/> constructor is used again to create a copy of the stack with the order of elements reversed; thus, the three null elements are at the end.
    /// The <see cref="System.Collections.Generic.Stack{T}.Contains"/> method is used to show that the string "four" is in the first copy of the stack, after which the <see cref="System.Collections.Generic.Stack{T}.Clear"/> method clears the copy and the <see cref="System.Collections.Generic.Stack{T}.Count"/> property shows that the stack is empty.
    /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/StackT/Overview/source.cs" id="Snippet1" />
    /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/StackT/Overview/source.fs" id="Snippet1" />
    /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/StackT/Overview/source.vb" id="Snippet1" />
    /// </remarks>
    /// <related type="Article" href="/dotnet/csharp/programming-guide/concepts/iterators">Iterators (C#)</related>
    /// <related type="Article" href="/dotnet/visual-basic/programming-guide/concepts/iterators">Iterators (Visual Basic)</related>
    [DebuggerTypeProxy(typeof(StackDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    [Serializable]
    [System.Runtime.CompilerServices.TypeForwardedFrom("System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    public class Stack<T> : IEnumerable<T>,
        System.Collections.ICollection,
        IReadOnlyCollection<T>
    {
        private T[] _array; // Storage for stack elements. Do not rename (binary serialization)
        private int _size; // Number of items in the stack. Do not rename (binary serialization)
        private int _version; // Used to keep enumerator in sync w/ collection. Do not rename (binary serialization)

        private const int DefaultCapacity = 4;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Collections.Generic.Stack`1"/> class that is empty and has the default initial capacity.
        /// </summary>
        /// <remarks>
        /// The capacity of a <see cref="System.Collections.Generic.Stack{T}"/> is the number of elements that the <see cref="System.Collections.Generic.Stack{T}"/> can hold. As elements are added to a <see cref="System.Collections.Generic.Stack{T}"/>, the capacity is automatically increased as required by reallocating the internal array.
        /// If the size of the collection can be estimated, specifying the initial capacity eliminates the need to perform a number of resizing operations while adding elements to the <see cref="System.Collections.Generic.Stack{T}"/>.
        /// The capacity can be decreased by calling <see cref="System.Collections.Generic.Stack{T}.TrimExcess"/>.
        /// This constructor is an O(1) operation.
        /// The following code example demonstrates this constructor and several methods of the <see cref="System.Collections.Generic.Stack{T}"/> generic class.
        /// The code example creates a stack of strings with default capacity and uses the <see cref="System.Collections.Generic.Stack{T}.Push"/> method to push five strings onto the stack. The elements of the stack are enumerated, which does not change the state of the stack. The <see cref="System.Collections.Generic.Stack{T}.Pop"/> method is used to pop the first string off the stack. The <see cref="System.Collections.Generic.Stack{T}.Peek"/> method is used to look at the next item on the stack, and then the <see cref="System.Collections.Generic.Stack{T}.Pop"/> method is used to pop it off.
        /// The <see cref="System.Collections.Generic.Stack{T}.ToArray"/> method is used to create an array and copy the stack elements to it, then the array is passed to the <see cref="System.Collections.Generic.Stack{T}.#ctor"/> constructor that takes <see cref="System.Collections.Generic.IEnumerable{T}"/>, creating a copy of the stack with the order of the elements reversed. The elements of the copy are displayed.
        /// An array twice the size of the stack is created, and the <see cref="System.Collections.Generic.Stack{T}.CopyTo"/> method is used to copy the array elements beginning at the middle of the array. The <see cref="System.Collections.Generic.Stack{T}.#ctor"/> constructor is used again to create a copy of the stack with the order of elements reversed; thus, the three null elements are at the end.
        /// The <see cref="System.Collections.Generic.Stack{T}.Contains"/> method is used to show that the string "four" is in the first copy of the stack, after which the <see cref="System.Collections.Generic.Stack{T}.Clear"/> method clears the copy and the <see cref="System.Collections.Generic.Stack{T}.Count"/> property shows that the stack is empty.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/StackT/Overview/source.cs" id="Snippet1" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/StackT/Overview/source.fs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/StackT/Overview/source.vb" id="Snippet1" />
        /// </remarks>
        public Stack()
        {
            _array = Array.Empty<T>();
        }

        // Create a stack with a specific initial capacity.  The initial capacity
        // must be a non-negative number.
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Collections.Generic.Stack`1"/> class that is empty and has the specified initial capacity or the default initial capacity, whichever is greater.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the <see cref="T:System.Collections.Generic.Stack`1"/> can contain.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        /// <remarks>
        /// The capacity of a <see cref="System.Collections.Generic.Stack{T}"/> is the number of elements that the <see cref="System.Collections.Generic.Stack{T}"/> can hold. As elements are added to a <see cref="System.Collections.Generic.Stack{T}"/>, the capacity is automatically increased as required by reallocating the internal array.
        /// If the size of the collection can be estimated, specifying the initial capacity eliminates the need to perform a number of resizing operations while adding elements to the <see cref="System.Collections.Generic.Stack{T}"/>.
        /// The capacity can be decreased by calling <see cref="System.Collections.Generic.Stack{T}.TrimExcess"/>.
        /// This constructor is an O(<c>n</c>) operation, where <c>n</c> is <c>capacity</c>.
        /// </remarks>
        public Stack(int capacity)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(capacity);
            _array = new T[capacity];
        }

        // Fills a Stack with the contents of a particular collection.  The items are
        // pushed onto the stack in the same order they are read by the enumerator.
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Collections.Generic.Stack`1"/> class that contains elements copied from the specified collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection to copy elements from.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="collection"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// The capacity of a <see cref="System.Collections.Generic.Stack{T}"/> is the number of elements that the <see cref="System.Collections.Generic.Stack{T}"/> can hold. As elements are added to a <see cref="System.Collections.Generic.Stack{T}"/>, the capacity is automatically increased as required by reallocating the internal array.
        /// If the size of the collection can be estimated, specifying the initial capacity eliminates the need to perform a number of resizing operations while adding elements to the <see cref="System.Collections.Generic.Stack{T}"/>.
        /// The capacity can be decreased by calling <see cref="System.Collections.Generic.Stack{T}.TrimExcess"/>.
        /// The elements are copied onto the <see cref="System.Collections.Generic.Stack{T}"/> in the same order they are read by the <see cref="System.Collections.Generic.IEnumerator{T}"/> of the collection.
        /// This constructor is an O(<c>n</c>) operation, where <c>n</c> is the number of elements in <c>collection</c>.
        /// The following code example demonstrates this constructor and several methods of the <see cref="System.Collections.Generic.Stack{T}"/> generic class.
        /// The code example creates a stack of strings with default capacity and uses the <see cref="System.Collections.Generic.Stack{T}.Push"/> method to push five strings onto the stack. The elements of the stack are enumerated, which does not change the state of the stack. The <see cref="System.Collections.Generic.Stack{T}.Pop"/> method is used to pop the first string off the stack. The <see cref="System.Collections.Generic.Stack{T}.Peek"/> method is used to look at the next item on the stack, and then the <see cref="System.Collections.Generic.Stack{T}.Pop"/> method is used to pop it off.
        /// The <see cref="System.Collections.Generic.Stack{T}.ToArray"/> method is used to create an array and copy the stack elements to it, then the array is passed to the <see cref="System.Collections.Generic.Stack{T}.#ctor"/> constructor that takes <see cref="System.Collections.Generic.IEnumerable{T}"/>, creating a copy of the stack with the order of the elements reversed. The elements of the copy are displayed.
        /// An array twice the size of the stack is created, and the <see cref="System.Collections.Generic.Stack{T}.CopyTo"/> method is used to copy the array elements beginning at the middle of the array. The <see cref="System.Collections.Generic.Stack{T}.#ctor"/> constructor is used again to create a copy of the stack with the order of elements reversed; thus, the three null elements are at the end.
        /// The <see cref="System.Collections.Generic.Stack{T}.Contains"/> method is used to show that the string "four" is in the first copy of the stack, after which the <see cref="System.Collections.Generic.Stack{T}.Clear"/> method clears the copy and the <see cref="System.Collections.Generic.Stack{T}.Count"/> property shows that the stack is empty.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/StackT/Overview/source.cs" id="Snippet1" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/StackT/Overview/source.fs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/StackT/Overview/source.vb" id="Snippet1" />
        /// </remarks>
        public Stack(IEnumerable<T> collection)
        {
            ArgumentNullException.ThrowIfNull(collection);

            _array = EnumerableHelpers.ToArray(collection, out _size);
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.Stack`1"/>.
        /// </summary>
        /// <value>The number of elements contained in the <see cref="T:System.Collections.Generic.Stack`1"/>.</value>
        /// <remarks>
        /// The capacity of the <see cref="System.Collections.Generic.Stack{T}"/> is the number of elements that the <see cref="System.Collections.Generic.Stack{T}"/> can store. <see cref="System.Collections.Generic.Stack{T}.Count"/> is the number of elements that are actually in the <see cref="System.Collections.Generic.Stack{T}"/>.
        /// The capacity is always greater than or equal to <see cref="System.Collections.Generic.Stack{T}.Count"/>. If <see cref="System.Collections.Generic.Stack{T}.Count"/> exceeds the capacity while adding elements, the capacity is increased by automatically reallocating the internal array before copying the old elements and adding the new elements.
        /// Retrieving the value of this property is an O(1) operation.
        /// The following code example demonstrates several properties and methods of the <see cref="System.Collections.Generic.Stack{T}"/> generic class, including the <see cref="System.Collections.Generic.Stack{T}.Count"/> property.
        /// The code example creates a stack of strings with default capacity and uses the <see cref="System.Collections.Generic.Stack{T}.Push"/> method to push five strings onto the stack. The elements of the stack are enumerated, which does not change the state of the stack. The <see cref="System.Collections.Generic.Stack{T}.Pop"/> method is used to pop the first string off the stack. The <see cref="System.Collections.Generic.Stack{T}.Peek"/> method is used to look at the next item on the stack, and then the <see cref="System.Collections.Generic.Stack{T}.Pop"/> method is used to pop it off.
        /// The <see cref="System.Collections.Generic.Stack{T}.ToArray"/> method is used to create an array and copy the stack elements to it, then the array is passed to the <see cref="System.Collections.Generic.Stack{T}.#ctor"/> constructor that takes <see cref="System.Collections.Generic.IEnumerable{T}"/>, creating a copy of the stack with the order of the elements reversed. The elements of the copy are displayed.
        /// An array twice the size of the stack is created, and the <see cref="System.Collections.Generic.Stack{T}.CopyTo"/> method is used to copy the array elements beginning at the middle of the array. The <see cref="System.Collections.Generic.Stack{T}.#ctor"/> constructor is used again to create a copy of the stack with the order of elements reversed; thus, the three null elements are at the end.
        /// The <see cref="System.Collections.Generic.Stack{T}.Contains"/> method is used to show that the string "four" is in the first copy of the stack, after which the <see cref="System.Collections.Generic.Stack{T}.Clear"/> method clears the copy and the <see cref="System.Collections.Generic.Stack{T}.Count"/> property shows that the stack is empty.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/StackT/Overview/source.cs" id="Snippet1" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/StackT/Overview/source.fs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/StackT/Overview/source.vb" id="Snippet1" />
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
        /// <value>An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>. In the default implementation of <see cref="T:System.Collections.Generic.Stack`1"/>, this property always returns the current instance.</value>
        /// <remarks>
        /// <code language="csharp">
        /// </code>
        /// <code language="vb">
        /// </code>
        /// Default implementations of collections in <see cref="System.Collections.Generic">Generic</see> are not synchronized.
        /// Enumerating through a collection is intrinsically not a thread-safe procedure.  To guarantee thread safety during enumeration, you can lock the collection during the entire enumeration.  To allow the collection to be accessed by multiple threads for reading and writing, you must implement your own synchronization.
        /// <see cref="System.Collections.ICollection.SyncRoot"/> returns an object that can be used to synchronize access to the <see cref="System.Collections.ICollection"/>. Synchronization is effective only if all threads lock this object before accessing the collection. The following code shows the use of the <see cref="System.Collections.ICollection.SyncRoot"/> property.
        /// ODE0 
        /// ODE1 
        /// Retrieving the value of this property is an O(1) operation.
        /// </remarks>
        object ICollection.SyncRoot => this;

        // Removes all Objects from the Stack.
        /// <summary>
        /// Removes all objects from the <see cref="T:System.Collections.Generic.Stack`1"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="System.Collections.Generic.Stack{T}.Count"/> is set to zero, and references to other objects from elements of the collection are also released.
        /// The capacity remains unchanged. To reset the capacity of the <see cref="System.Collections.Generic.Stack{T}"/>, call <see cref="System.Collections.Generic.Stack{T}.TrimExcess"/>. Trimming an empty <see cref="System.Collections.Generic.Stack{T}"/> sets the capacity of the <see cref="System.Collections.Generic.Stack{T}"/> to the default capacity.
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="System.Collections.Generic.Stack{T}.Count"/>.
        /// The following code example demonstrates several methods of the <see cref="System.Collections.Generic.Stack{T}"/> generic class, including the <see cref="System.Collections.Generic.Stack{T}.Clear"/> method.
        /// The code example creates a stack of strings with default capacity and uses the <see cref="System.Collections.Generic.Stack{T}.Push"/> method to push five strings onto the stack. The elements of the stack are enumerated, which does not change the state of the stack. The <see cref="System.Collections.Generic.Stack{T}.Pop"/> method is used to pop the first string off the stack. The <see cref="System.Collections.Generic.Stack{T}.Peek"/> method is used to look at the next item on the stack, and then the <see cref="System.Collections.Generic.Stack{T}.Pop"/> method is used to pop it off.
        /// The <see cref="System.Collections.Generic.Stack{T}.ToArray"/> method is used to create an array and copy the stack elements to it, then the array is passed to the <see cref="System.Collections.Generic.Stack{T}.#ctor"/> constructor that takes <see cref="System.Collections.Generic.IEnumerable{T}"/>, creating a copy of the stack with the order of the elements reversed. The elements of the copy are displayed.
        /// An array twice the size of the stack is created, and the <see cref="System.Collections.Generic.Stack{T}.CopyTo"/> method is used to copy the array elements beginning at the middle of the array. The <see cref="System.Collections.Generic.Stack{T}.#ctor"/> constructor is used again to create a copy of the stack with the order of elements reversed; thus, the three null elements are at the end.
        /// The <see cref="System.Collections.Generic.Stack{T}.Contains"/> method is used to show that the string "four" is in the first copy of the stack, after which the <see cref="System.Collections.Generic.Stack{T}.Clear"/> method clears the copy and the <see cref="System.Collections.Generic.Stack{T}.Count"/> property shows that the stack is empty.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/StackT/Overview/source.cs" id="Snippet1" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/StackT/Overview/source.fs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/StackT/Overview/source.vb" id="Snippet1" />
        /// </remarks>
        public void Clear()
        {
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                Array.Clear(_array, 0, _size); // Don't need to doc this but we clear the elements so that the gc can reclaim the references.
            }
            _size = 0;
            _version++;
        }

        /// <summary>
        /// Determines whether an element is in the <see cref="T:System.Collections.Generic.Stack`1"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.Stack`1"/>. The value can be <see langword="null"/> for reference types.</param>
        /// <returns><see langword="true"/> if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.Stack`1"/>; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This method determines equality using the default equality comparer <see cref="System.Collections.Generic.EqualityComparer{T}.Default">Default</see> for <c>T</c>, the type of values in the list.
        /// This method performs a linear search; therefore, this method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="System.Collections.Generic.Stack{T}.Count"/>.
        /// The following code example demonstrates several methods of the <see cref="System.Collections.Generic.Stack{T}"/> generic class, including the <see cref="System.Collections.Generic.Stack{T}.Contains"/> method.
        /// The code example creates a stack of strings with default capacity and uses the <see cref="System.Collections.Generic.Stack{T}.Push"/> method to push five strings onto the stack. The elements of the stack are enumerated, which does not change the state of the stack. The <see cref="System.Collections.Generic.Stack{T}.Pop"/> method is used to pop the first string off the stack. The <see cref="System.Collections.Generic.Stack{T}.Peek"/> method is used to look at the next item on the stack, and then the <see cref="System.Collections.Generic.Stack{T}.Pop"/> method is used to pop it off.
        /// The <see cref="System.Collections.Generic.Stack{T}.ToArray"/> method is used to create an array and copy the stack elements to it, then the array is passed to the <see cref="System.Collections.Generic.Stack{T}.#ctor"/> constructor that takes <see cref="System.Collections.Generic.IEnumerable{T}"/>, creating a copy of the stack with the order of the elements reversed. The elements of the copy are displayed.
        /// An array twice the size of the stack is created, and the <see cref="System.Collections.Generic.Stack{T}.CopyTo"/> method is used to copy the array elements beginning at the middle of the array. The <see cref="System.Collections.Generic.Stack{T}.#ctor"/> constructor is used again to create a copy of the stack with the order of elements reversed; thus, the three null elements are at the end.
        /// The <see cref="System.Collections.Generic.Stack{T}.Contains"/> method is used to show that the string "four" is in the first copy of the stack, after which the <see cref="System.Collections.Generic.Stack{T}.Clear"/> method clears the copy and the <see cref="System.Collections.Generic.Stack{T}.Count"/> property shows that the stack is empty.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/StackT/Overview/source.cs" id="Snippet1" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/StackT/Overview/source.fs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/StackT/Overview/source.vb" id="Snippet1" />
        /// </remarks>
        /// <related type="Article" href="/dotnet/standard/globalization-localization/performing-culture-insensitive-string-operations-in-collections">Performing Culture-Insensitive String Operations in Collections</related>
        public bool Contains(T item)
        {
            // Compare items using the default equality comparer

            // PERF: Internally Array.LastIndexOf calls
            // EqualityComparer<T>.Default.LastIndexOf, which
            // is specialized for different types. This
            // boosts performance since instead of making a
            // virtual method call each iteration of the loop,
            // via EqualityComparer<T>.Default.Equals, we
            // only make one virtual call to EqualityComparer.LastIndexOf.

            return _size != 0 && Array.LastIndexOf(_array, item, _size - 1) >= 0;
        }

        // Copies the stack into an array.
        /// <summary>
        /// Copies the <see cref="T:System.Collections.Generic.Stack`1"/> to an existing one-dimensional <see cref="T:System.Array"/>, starting at the specified array index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.Stack`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than zero.</exception>
        /// <exception cref="T:System.ArgumentException">The number of elements in the source <see cref="T:System.Collections.Generic.Stack`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.</exception>
        /// <remarks>
        /// The elements are copied onto the array in last-in-first-out (LIFO) order, similar to the order of the elements returned by a succession of calls to <see cref="System.Collections.Generic.Stack{T}.Pop"/>.
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="System.Collections.Generic.Stack{T}.Count"/>.
        /// The following code example demonstrates several methods of the <see cref="System.Collections.Generic.Stack{T}"/> generic class, including the <see cref="System.Collections.Generic.Stack{T}.CopyTo"/> method.
        /// The code example creates a stack of strings with default capacity and uses the <see cref="System.Collections.Generic.Stack{T}.Push"/> method to push five strings onto the stack. The elements of the stack are enumerated, which does not change the state of the stack. The <see cref="System.Collections.Generic.Stack{T}.Pop"/> method is used to pop the first string off the stack. The <see cref="System.Collections.Generic.Stack{T}.Peek"/> method is used to look at the next item on the stack, and then the <see cref="System.Collections.Generic.Stack{T}.Pop"/> method is used to pop it off.
        /// The <see cref="System.Collections.Generic.Stack{T}.ToArray"/> method is used to create an array and copy the stack elements to it, then the array is passed to the <see cref="System.Collections.Generic.Stack{T}.#ctor"/> constructor that takes <see cref="System.Collections.Generic.IEnumerable{T}"/>, creating a copy of the stack with the order of the elements reversed. The elements of the copy are displayed.
        /// An array twice the size of the stack is created, and the <see cref="System.Collections.Generic.Stack{T}.CopyTo"/> method is used to copy the array elements beginning at the middle of the array. The <see cref="System.Collections.Generic.Stack{T}.#ctor"/> constructor is used again to create a copy of the stack with the order of elements reversed; thus, the three null elements are at the end.
        /// The <see cref="System.Collections.Generic.Stack{T}.Contains"/> method is used to show that the string "four" is in the first copy of the stack, after which the <see cref="System.Collections.Generic.Stack{T}.Clear"/> method clears the copy and the <see cref="System.Collections.Generic.Stack{T}.Count"/> property shows that the stack is empty.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/StackT/Overview/source.cs" id="Snippet1" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/StackT/Overview/source.fs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/StackT/Overview/source.vb" id="Snippet1" />
        /// </remarks>
        public void CopyTo(T[] array, int arrayIndex)
        {
            ArgumentNullException.ThrowIfNull(array);

            if (arrayIndex < 0 || arrayIndex > array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), arrayIndex, SR.ArgumentOutOfRange_IndexMustBeLessOrEqual);
            }

            if (array.Length - arrayIndex < _size)
            {
                throw new ArgumentException(SR.Argument_InvalidOffLen);
            }

            Debug.Assert(array != _array);
            int srcIndex = 0;
            int dstIndex = arrayIndex + _size;
            while (srcIndex < _size)
            {
                array[--dstIndex] = _array[srcIndex++];
            }
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.ICollection"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than zero.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="array"/> is multidimensional. -or- <paramref name="array"/> does not have zero-based indexing. -or- The number of elements in the source <see cref="T:System.Collections.ICollection"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>. -or- The type of the source <see cref="T:System.Collections.ICollection"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.</exception>
        /// <remarks>
        /// <note type="note">
        /// If the type of the source <see cref="System.Collections.ICollection"/> cannot be cast automatically to the type of the destination <c>array</c>, the non-generic implementations of <see cref="System.Collections.ICollection.CopyTo">CopyTo</see> throw <see cref="System.InvalidCastException"/>, whereas the generic implementations throw <see cref="System.ArgumentException"/>.
        /// </note>
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="System.Collections.Generic.Stack{T}.Count"/>.
        /// </remarks>
        void ICollection.CopyTo(Array array, int arrayIndex)
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

            if (arrayIndex < 0 || arrayIndex > array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), arrayIndex, SR.ArgumentOutOfRange_IndexMustBeLessOrEqual);
            }

            if (array.Length - arrayIndex < _size)
            {
                throw new ArgumentException(SR.Argument_InvalidOffLen);
            }

            try
            {
                Array.Copy(_array, 0, array, arrayIndex, _size);
                Array.Reverse(array, arrayIndex, _size);
            }
            catch (ArrayTypeMismatchException)
            {
                throw new ArgumentException(SR.Argument_IncompatibleArrayType, nameof(array));
            }
        }

        // Returns an IEnumerator for this Stack.
        /// <summary>
        /// Returns an enumerator for the <see cref="T:System.Collections.Generic.Stack`1"/>.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.Generic.Stack`1.Enumerator"/> for the <see cref="T:System.Collections.Generic.Stack`1"/>.</returns>
        /// <remarks>
        /// The <c>foreach</c> statement of the C# language (<c>For Each</c> in Visual Basic) hides the complexity of the enumerators. Therefore, using <c>foreach</c> is recommended, instead of directly manipulating the enumerator.
        /// Enumerators can be used to read the data in the collection, but they cannot be used to modify the underlying collection.
        /// Initially, the enumerator is positioned before the first element in the collection. At this position, <see cref="System.Collections.Generic.Stack{T}.Enumerator.Current"/> is undefined. Therefore, you must call <see cref="System.Collections.Generic.Stack{T}.Enumerator.MoveNext"/> to advance the enumerator to the first element of the collection before reading the value of <see cref="System.Collections.Generic.Stack{T}.Enumerator.Current"/>.
        /// <see cref="System.Collections.Generic.Stack{T}.Enumerator.Current"/> returns the same object until <see cref="System.Collections.Generic.Stack{T}.Enumerator.MoveNext"/> is called. <see cref="System.Collections.Generic.Stack{T}.Enumerator.MoveNext"/> sets <see cref="System.Collections.Generic.Stack{T}.Enumerator.Current"/> to the next element.
        /// If <see cref="System.Collections.Generic.Stack{T}.Enumerator.MoveNext"/> passes the end of the collection, the enumerator is positioned after the last element in the collection and <see cref="System.Collections.Generic.Stack{T}.Enumerator.MoveNext"/> returns <c>false</c>. When the enumerator is at this position, subsequent calls to <see cref="System.Collections.Generic.Stack{T}.Enumerator.MoveNext"/> also return <c>false</c>. If the last call to <see cref="System.Collections.Generic.Stack{T}.Enumerator.MoveNext"/> returned <c>false</c>, <see cref="System.Collections.Generic.Stack{T}.Enumerator.Current"/> is undefined. You cannot set <see cref="System.Collections.Generic.Stack{T}.Enumerator.Current"/> to the first element of the collection again; you must create a new enumerator instance instead.
        /// An enumerator remains valid as long as the collection remains unchanged. If changes are made to the collection, such as adding, modifying, or deleting elements, the enumerator is irrecoverably invalidated and the next call to <see cref="System.Collections.Generic.Stack{T}.Enumerator.MoveNext"/> or <see cref="System.Collections.Generic.Stack{T}.Enumerator.System#Collections#IEnumerator#Reset"/> throws an <see cref="System.InvalidOperationException"/>.
        /// The enumerator does not have exclusive access to the collection; therefore, enumerating through a collection is intrinsically not a thread-safe procedure. To guarantee thread safety during enumeration, you can lock the collection during the entire enumeration.  To allow the collection to be accessed by multiple threads for reading and writing, you must implement your own synchronization.
        /// Default implementations of collections in <see cref="System.Collections.Generic">Generic</see> are not synchronized.
        /// This method is an O(1) operation.
        /// The following code example demonstrates that the <see cref="System.Collections.Generic.Stack{T}"/> generic class is enumerable. The <c>foreach</c> statement (<c>For Each</c> in Visual Basic) is used to enumerate the stack.
        /// The code example creates a stack of strings with default capacity and uses the <see cref="System.Collections.Generic.Stack{T}.Push"/> method to push five strings onto the stack. The elements of the stack are enumerated, which does not change the state of the stack. The <see cref="System.Collections.Generic.Stack{T}.Pop"/> method is used to pop the first string off the stack. The <see cref="System.Collections.Generic.Stack{T}.Peek"/> method is used to look at the next item on the stack, and then the <see cref="System.Collections.Generic.Stack{T}.Pop"/> method is used to pop it off.
        /// The <see cref="System.Collections.Generic.Stack{T}.ToArray"/> method is used to create an array and copy the stack elements to it, then the array is passed to the <see cref="System.Collections.Generic.Stack{T}.#ctor"/> constructor that takes <see cref="System.Collections.Generic.IEnumerable{T}"/>, creating a copy of the stack with the order of the elements reversed. The elements of the copy are displayed.
        /// An array twice the size of the stack is created, and the <see cref="System.Collections.Generic.Stack{T}.CopyTo"/> method is used to copy the array elements beginning at the middle of the array. The <see cref="System.Collections.Generic.Stack{T}.#ctor"/> constructor is used again to create a copy of the stack with the order of elements reversed; thus, the three null elements are at the end.
        /// The <see cref="System.Collections.Generic.Stack{T}.Contains"/> method is used to show that the string "four" is in the first copy of the stack, after which the <see cref="System.Collections.Generic.Stack{T}.Clear"/> method clears the copy and the <see cref="System.Collections.Generic.Stack{T}.Count"/> property shows that the stack is empty.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/StackT/Overview/source.cs" id="Snippet1" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/StackT/Overview/source.fs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/StackT/Overview/source.vb" id="Snippet1" />
        /// </remarks>
        public Enumerator GetEnumerator() => new Enumerator(this);

        /// <internalonly/>
        IEnumerator<T> IEnumerable<T>.GetEnumerator() =>
            Count == 0 ? EnumerableHelpers.GetEmptyEnumerator<T>() :
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

        /// <summary>
        /// Sets the capacity to the actual number of elements in the <see cref="T:System.Collections.Generic.Stack`1"/>, if that number is less than 90 percent of current capacity.
        /// </summary>
        /// <remarks>
        /// This method can be used to minimize a collection's memory overhead if no new elements will be added to the collection. The cost of reallocating and copying a large <see cref="System.Collections.Generic.Stack{T}"/> can be considerable, however, so the <see cref="System.Collections.Generic.Stack{T}.TrimExcess"/> method does nothing if the list is at more than 90 percent of capacity. This avoids incurring a large reallocation cost for a relatively small gain.
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="System.Collections.Generic.Stack{T}.Count"/>.
        /// To reset a <see cref="System.Collections.Generic.Stack{T}"/> to its initial state, call the <see cref="System.Collections.Generic.Stack{T}.Clear"/> method before calling <see cref="System.Collections.Generic.Stack{T}.TrimExcess"/> method. Trimming an empty <see cref="System.Collections.Generic.Stack{T}"/> sets the capacity of the <see cref="System.Collections.Generic.Stack{T}"/> to the default capacity.
        /// </remarks>
        public void TrimExcess()
        {
            int threshold = (int)(_array.Length * 0.9);
            if (_size < threshold)
            {
                Array.Resize(ref _array, _size);
            }
        }

        /// <summary>
        /// Sets the capacity of a <see cref="Stack{T}"/> object to a specified number of entries.
        /// </summary>
        /// <param name="capacity">The new capacity.</param>
        /// <exception cref="ArgumentOutOfRangeException">Passed capacity is lower than 0 or entries count.</exception>
        public void TrimExcess(int capacity)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(capacity);
            ArgumentOutOfRangeException.ThrowIfLessThan(capacity, _size);

            if (capacity == _array.Length)
                return;

            Array.Resize(ref _array, capacity);
        }

        // Returns the top object on the stack without removing it.  If the stack
        // is empty, Peek throws an InvalidOperationException.
        /// <summary>
        /// Returns the object at the top of the <see cref="T:System.Collections.Generic.Stack`1"/> without removing it.
        /// </summary>
        /// <returns>The object at the top of the <see cref="T:System.Collections.Generic.Stack`1"/>.</returns>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Collections.Generic.Stack`1"/> is empty.</exception>
        /// <remarks>
        /// This method is similar to the <see cref="System.Collections.Generic.Stack{T}.Pop"/> method, but <see cref="System.Collections.Generic.Stack{T}.Peek"/> does not modify the <see cref="System.Collections.Generic.Stack{T}"/>.
        /// If type <c>T</c> is a reference type, <c>null</c> can be pushed onto the <see cref="System.Collections.Generic.Stack{T}"/> as a placeholder, if needed.
        /// This method is an O(1) operation.
        /// The following code example demonstrates several methods of the <see cref="System.Collections.Generic.Stack{T}"/> generic class, including the <see cref="System.Collections.Generic.Stack{T}.Peek"/> method.
        /// The code example creates a stack of strings with default capacity and uses the <see cref="System.Collections.Generic.Stack{T}.Push"/> method to push five strings onto the stack. The elements of the stack are enumerated, which does not change the state of the stack. The <see cref="System.Collections.Generic.Stack{T}.Pop"/> method is used to pop the first string off the stack. The <see cref="System.Collections.Generic.Stack{T}.Peek"/> method is used to look at the next item on the stack, and then the <see cref="System.Collections.Generic.Stack{T}.Pop"/> method is used to pop it off.
        /// The <see cref="System.Collections.Generic.Stack{T}.ToArray"/> method is used to create an array and copy the stack elements to it, then the array is passed to the <see cref="System.Collections.Generic.Stack{T}.#ctor"/> constructor that takes <see cref="System.Collections.Generic.IEnumerable{T}"/>, creating a copy of the stack with the order of the elements reversed. The elements of the copy are displayed.
        /// An array twice the size of the stack is created, and the <see cref="System.Collections.Generic.Stack{T}.CopyTo"/> method is used to copy the array elements beginning at the middle of the array. The <see cref="System.Collections.Generic.Stack{T}.#ctor"/> constructor is used again to create a copy of the stack with the order of elements reversed; thus, the three null elements are at the end.
        /// The <see cref="System.Collections.Generic.Stack{T}.Contains"/> method is used to show that the string "four" is in the first copy of the stack, after which the <see cref="System.Collections.Generic.Stack{T}.Clear"/> method clears the copy and the <see cref="System.Collections.Generic.Stack{T}.Count"/> property shows that the stack is empty.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/StackT/Overview/source.cs" id="Snippet1" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/StackT/Overview/source.fs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/StackT/Overview/source.vb" id="Snippet1" />
        /// </remarks>
        public T Peek()
        {
            int size = _size - 1;
            T[] array = _array;

            if ((uint)size >= (uint)array.Length)
            {
                ThrowForEmptyStack();
            }

            return array[size];
        }

        /// <summary>
        /// Returns a value that indicates whether there is an object at the top of the <see cref="T:System.Collections.Generic.Stack`1"/>, and if one is present, copies it to the <paramref name="result"/> parameter. The object is not removed from the <see cref="T:System.Collections.Generic.Stack`1"/>.
        /// </summary>
        /// <param name="result">If present, the object at the top of the <see cref="T:System.Collections.Generic.Stack`1"/>; otherwise, the default value of <typeparamref name="T"/>.</param>
        /// <returns><see langword="true"/> if there is an object at the top of the <see cref="T:System.Collections.Generic.Stack`1"/>; <see langword="false"/> if the <see cref="T:System.Collections.Generic.Stack`1"/> is empty.</returns>
        public bool TryPeek([MaybeNullWhen(false)] out T result)
        {
            int size = _size - 1;
            T[] array = _array;

            if ((uint)size >= (uint)array.Length)
            {
                result = default!;
                return false;
            }
            result = array[size];
            return true;
        }

        // Pops an item from the top of the stack.  If the stack is empty, Pop
        // throws an InvalidOperationException.
        /// <summary>
        /// Removes and returns the object at the top of the <see cref="T:System.Collections.Generic.Stack`1"/>.
        /// </summary>
        /// <returns>The object removed from the top of the <see cref="T:System.Collections.Generic.Stack`1"/>.</returns>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Collections.Generic.Stack`1"/> is empty.</exception>
        /// <remarks>
        /// This method is similar to the <see cref="System.Collections.Generic.Stack{T}.Peek"/> method, but <see cref="System.Collections.Generic.Stack{T}.Peek"/> does not modify the <see cref="System.Collections.Generic.Stack{T}"/>.
        /// If type <c>T</c> is a reference type, <c>null</c> can be pushed onto the <see cref="System.Collections.Generic.Stack{T}"/> as a placeholder, if needed.
        /// <see cref="System.Collections.Generic.Stack{T}"/> is implemented as an array. This method is an O(1) operation.
        /// The following code example demonstrates several methods of the <see cref="System.Collections.Generic.Stack{T}"/> generic class, including the <see cref="System.Collections.Generic.Stack{T}.Pop"/> method.
        /// The code example creates a stack of strings with default capacity and uses the <see cref="System.Collections.Generic.Stack{T}.Push"/> method to push five strings onto the stack. The elements of the stack are enumerated, which does not change the state of the stack. The <see cref="System.Collections.Generic.Stack{T}.Pop"/> method is used to pop the first string off the stack. The <see cref="System.Collections.Generic.Stack{T}.Peek"/> method is used to look at the next item on the stack, and then the <see cref="System.Collections.Generic.Stack{T}.Pop"/> method is used to pop it off.
        /// The <see cref="System.Collections.Generic.Stack{T}.ToArray"/> method is used to create an array and copy the stack elements to it, then the array is passed to the <see cref="System.Collections.Generic.Stack{T}.#ctor"/> constructor that takes <see cref="System.Collections.Generic.IEnumerable{T}"/>, creating a copy of the stack with the order of the elements reversed. The elements of the copy are displayed.
        /// An array twice the size of the stack is created, and the <see cref="System.Collections.Generic.Stack{T}.CopyTo"/> method is used to copy the array elements beginning at the middle of the array. The <see cref="System.Collections.Generic.Stack{T}.#ctor"/> constructor is used again to create a copy of the stack with the order of elements reversed; thus, the three null elements are at the end.
        /// The <see cref="System.Collections.Generic.Stack{T}.Contains"/> method is used to show that the string "four" is in the first copy of the stack, after which the <see cref="System.Collections.Generic.Stack{T}.Clear"/> method clears the copy and the <see cref="System.Collections.Generic.Stack{T}.Count"/> property shows that the stack is empty.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/StackT/Overview/source.cs" id="Snippet1" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/StackT/Overview/source.fs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/StackT/Overview/source.vb" id="Snippet1" />
        /// </remarks>
        public T Pop()
        {
            int size = _size - 1;
            T[] array = _array;

            // if (_size == 0) is equivalent to if (size == -1), and this case
            // is covered with (uint)size, thus allowing bounds check elimination
            // https://github.com/dotnet/coreclr/pull/9773
            if ((uint)size >= (uint)array.Length)
            {
                ThrowForEmptyStack();
            }

            _version++;
            _size = size;
            T item = array[size];
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                array[size] = default!;     // Free memory quicker.
            }
            return item;
        }

        /// <summary>
        /// Returns a value that indicates whether there is an object at the top of the <see cref="T:System.Collections.Generic.Stack`1"/>, and if one is present, copies it to the <paramref name="result"/> parameter, and removes it from the <see cref="T:System.Collections.Generic.Stack`1"/>.
        /// </summary>
        /// <param name="result">If present, the object at the top of the <see cref="T:System.Collections.Generic.Stack`1"/>; otherwise, the default value of <typeparamref name="T"/>.</param>
        /// <returns><see langword="true"/> if there is an object at the top of the <see cref="T:System.Collections.Generic.Stack`1"/>; <see langword="false"/> if the <see cref="T:System.Collections.Generic.Stack`1"/> is empty.</returns>
        public bool TryPop([MaybeNullWhen(false)] out T result)
        {
            int size = _size - 1;
            T[] array = _array;

            if ((uint)size >= (uint)array.Length)
            {
                result = default!;
                return false;
            }

            _version++;
            _size = size;
            result = array[size];
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                array[size] = default!;
            }
            return true;
        }

        // Pushes an item to the top of the stack.
        /// <summary>
        /// Inserts an object at the top of the <see cref="T:System.Collections.Generic.Stack`1"/>.
        /// </summary>
        /// <param name="item">The object to push onto the <see cref="T:System.Collections.Generic.Stack`1"/>. The value can be <see langword="null"/> for reference types.</param>
        /// <remarks>
        /// <see cref="System.Collections.Generic.Stack{T}"/> is implemented as an array.
        /// If <see cref="System.Collections.Generic.Stack{T}.Count"/> already equals the capacity, the capacity of the <see cref="System.Collections.Generic.Stack{T}"/> is increased by automatically reallocating the internal array, and the existing elements are copied to the new array before the new element is added.
        /// If type <c>T</c> is a reference type, <c>null</c> can be pushed onto the <see cref="System.Collections.Generic.Stack{T}"/> as a placeholder, if needed. It occupies a slot in the stack and is treated like any object.
        /// If <see cref="System.Collections.Generic.Stack{T}.Count"/> is less than the capacity of the stack, <see cref="System.Collections.Generic.Stack{T}.Push"/> is an O(1) operation. If the capacity needs to be increased to accommodate the new element, <see cref="System.Collections.Generic.Stack{T}.Push"/> becomes an O(<c>n</c>) operation, where <c>n</c> is <see cref="System.Collections.Generic.Stack{T}.Count"/>.
        /// The following code example demonstrates several methods of the <see cref="System.Collections.Generic.Stack{T}"/> generic class, including the <see cref="System.Collections.Generic.Stack{T}.Push"/> method.
        /// The code example creates a stack of strings with default capacity and uses the <see cref="System.Collections.Generic.Stack{T}.Push"/> method to push five strings onto the stack. The elements of the stack are enumerated, which does not change the state of the stack. The <see cref="System.Collections.Generic.Stack{T}.Pop"/> method is used to pop the first string off the stack. The <see cref="System.Collections.Generic.Stack{T}.Peek"/> method is used to look at the next item on the stack, and then the <see cref="System.Collections.Generic.Stack{T}.Pop"/> method is used to pop it off.
        /// The <see cref="System.Collections.Generic.Stack{T}.ToArray"/> method is used to create an array and copy the stack elements to it, then the array is passed to the <see cref="System.Collections.Generic.Stack{T}.#ctor"/> constructor that takes <see cref="System.Collections.Generic.IEnumerable{T}"/>, creating a copy of the stack with the order of the elements reversed. The elements of the copy are displayed.
        /// An array twice the size of the stack is created, and the <see cref="System.Collections.Generic.Stack{T}.CopyTo"/> method is used to copy the array elements beginning at the middle of the array. The <see cref="System.Collections.Generic.Stack{T}.#ctor"/> constructor is used again to create a copy of the stack with the order of elements reversed; thus, the three null elements are at the end.
        /// The <see cref="System.Collections.Generic.Stack{T}.Contains"/> method is used to show that the string "four" is in the first copy of the stack, after which the <see cref="System.Collections.Generic.Stack{T}.Clear"/> method clears the copy and the <see cref="System.Collections.Generic.Stack{T}.Count"/> property shows that the stack is empty.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/StackT/Overview/source.cs" id="Snippet1" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/StackT/Overview/source.fs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/StackT/Overview/source.vb" id="Snippet1" />
        /// </remarks>
        public void Push(T item)
        {
            int size = _size;
            T[] array = _array;

            if ((uint)size < (uint)array.Length)
            {
                array[size] = item;
                _version++;
                _size = size + 1;
            }
            else
            {
                PushWithResize(item);
            }
        }

        // Non-inline from Stack.Push to improve its code quality as uncommon path
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void PushWithResize(T item)
        {
            Debug.Assert(_size == _array.Length);
            Grow(_size + 1);
            _array[_size] = item;
            _version++;
            _size++;
        }

        /// <summary>
        /// Ensures that the capacity of this Stack is at least the specified <paramref name="capacity"/>.
        /// If the current capacity of the Stack is less than specified <paramref name="capacity"/>,
        /// the capacity is increased by continuously twice current capacity until it is at least the specified <paramref name="capacity"/>.
        /// </summary>
        /// <param name="capacity">The minimum capacity to ensure.</param>
        /// <returns>The new capacity of this stack.</returns>
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

            int newcapacity = _array.Length == 0 ? DefaultCapacity : 2 * _array.Length;

            // Allow the list to grow to maximum possible capacity (~2G elements) before encountering overflow.
            // Note that this check works even when _items.Length overflowed thanks to the (uint) cast.
            if ((uint)newcapacity > Array.MaxLength) newcapacity = Array.MaxLength;

            // If computed capacity is still less than specified, set to the original argument.
            // Capacities exceeding Array.MaxLength will be surfaced as OutOfMemoryException by Array.Resize.
            if (newcapacity < capacity) newcapacity = capacity;

            Array.Resize(ref _array, newcapacity);
        }

        // Copies the Stack to an array, in the same order Pop would return the items.
        /// <summary>
        /// Copies the <see cref="T:System.Collections.Generic.Stack`1"/> to a new array.
        /// </summary>
        /// <returns>A new array containing copies of the elements of the <see cref="T:System.Collections.Generic.Stack`1"/>.</returns>
        /// <remarks>
        /// The elements are copied onto the array in last-in-first-out (LIFO) order, similar to the order of the elements returned by a succession of calls to <see cref="System.Collections.Generic.Stack{T}.Pop"/>.
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="System.Collections.Generic.Stack{T}.Count"/>.
        /// The following code example demonstrates several methods of the <see cref="System.Collections.Generic.Stack{T}"/> generic class, including the <see cref="System.Collections.Generic.Stack{T}.ToArray"/> method.
        /// The code example creates a stack of strings with default capacity and uses the <see cref="System.Collections.Generic.Stack{T}.Push"/> method to push five strings onto the stack. The elements of the stack are enumerated, which does not change the state of the stack. The <see cref="System.Collections.Generic.Stack{T}.Pop"/> method is used to pop the first string off the stack. The <see cref="System.Collections.Generic.Stack{T}.Peek"/> method is used to look at the next item on the stack, and then the <see cref="System.Collections.Generic.Stack{T}.Pop"/> method is used to pop it off.
        /// The <see cref="System.Collections.Generic.Stack{T}.ToArray"/> method is used to create an array and copy the stack elements to it, then the array is passed to the <see cref="System.Collections.Generic.Stack{T}.#ctor"/> constructor that takes <see cref="System.Collections.Generic.IEnumerable{T}"/>, creating a copy of the stack with the order of the elements reversed. The elements of the copy are displayed.
        /// An array twice the size of the stack is created, and the <see cref="System.Collections.Generic.Stack{T}.CopyTo"/> method is used to copy the array elements beginning at the middle of the array. The <see cref="System.Collections.Generic.Stack{T}.#ctor"/> constructor is used again to create a copy of the stack with the order of elements reversed; thus, the three null elements are at the end.
        /// The <see cref="System.Collections.Generic.Stack{T}.Contains"/> method is used to show that the string "four" is in the first copy of the stack, after which the <see cref="System.Collections.Generic.Stack{T}.Clear"/> method clears the copy and the <see cref="System.Collections.Generic.Stack{T}.Count"/> property shows that the stack is empty.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/StackT/Overview/source.cs" id="Snippet1" />
        /// <code lang="fsharp" source="~/snippets/fsharp/System.Collections.Generic/StackT/Overview/source.fs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/StackT/Overview/source.vb" id="Snippet1" />
        /// </remarks>
        public T[] ToArray()
        {
            if (_size == 0)
                return Array.Empty<T>();

            T[] objArray = new T[_size];
            int i = 0;
            while (i < _size)
            {
                objArray[i] = _array[_size - i - 1];
                i++;
            }
            return objArray;
        }

        private void ThrowForEmptyStack()
        {
            Debug.Assert(_size == 0);
            throw new InvalidOperationException(SR.InvalidOperation_EmptyStack);
        }

        public struct Enumerator : IEnumerator<T>, System.Collections.IEnumerator
        {
            private readonly Stack<T> _stack;
            private readonly int _version;
            private int _index;
            private T? _currentElement;

            internal Enumerator(Stack<T> stack)
            {
                _stack = stack;
                _version = stack._version;
                _index = stack.Count;
                _currentElement = default;
            }

            public void Dispose() => _index = -1;

            public bool MoveNext()
            {
                if (_version != _stack._version)
                {
                    ThrowInvalidVersion();
                }

                T[] array = _stack._array;
                int index = _index - 1;
                if ((uint)index < (uint)array.Length)
                {
                    Debug.Assert(index < _stack.Count);
                    _currentElement = array[index];
                    _index = index;
                    return true;
                }

                _currentElement = default;
                _index = -1;
                return false;
            }

            public T Current => _currentElement!;

            object? System.Collections.IEnumerator.Current => Current;

            void IEnumerator.Reset()
            {
                if (_version != _stack._version)
                {
                    ThrowInvalidVersion();
                }

                _index = _stack.Count;
                _currentElement = default;
            }

            private static void ThrowInvalidVersion() => throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);
        }
    }
}

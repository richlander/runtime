// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
    // Base interface for all generic enumerators, providing a simple approach
    // to iterating over a collection.
    /// <summary>
    /// Supports a simple iteration over a generic collection.
    /// </summary>
    /// <typeparam name="T">The type of objects to enumerate.</typeparam>
    /// <remarks>
    /// <see cref="System.Collections.Generic.IEnumerator{T}"/> is the base interface for all generic enumerators.
    /// The <c>foreach</c> statement of the C# language (<c>For Each</c> in Visual Basic) hides the complexity of the enumerators.  Therefore, using <c>foreach</c> is recommended, instead of directly manipulating the enumerator.
    /// Enumerators can be used to read the data in the collection, but they cannot be used to modify the underlying collection.
    /// Initially, the enumerator is positioned before the first element in the collection. At this position, <see cref="System.Collections.Generic.IEnumerator{T}.Current"/> is undefined. Therefore, you must call <see cref="System.Collections.IEnumerator.MoveNext"/> to advance the enumerator to the first element of the collection before reading the value of <see cref="System.Collections.Generic.IEnumerator{T}.Current"/>.
    /// <see cref="System.Collections.Generic.IEnumerator{T}.Current"/> returns the same object until <see cref="System.Collections.IEnumerator.MoveNext"/> is called. <see cref="System.Collections.IEnumerator.MoveNext"/> sets <see cref="System.Collections.Generic.IEnumerator{T}.Current"/> to the next element.
    /// If <see cref="System.Collections.IEnumerator.MoveNext"/> passes the end of the collection, the enumerator is positioned after the last element in the collection and <see cref="System.Collections.IEnumerator.MoveNext"/> returns <c>false</c>. When the enumerator is at this position, subsequent calls to <see cref="System.Collections.IEnumerator.MoveNext"/> also return <c>false</c>. If the last call to <see cref="System.Collections.IEnumerator.MoveNext"/> returned <c>false</c>, <see cref="System.Collections.Generic.IEnumerator{T}.Current"/> is undefined. You cannot set <see cref="System.Collections.Generic.IEnumerator{T}.Current"/> to the first element of the collection again; you must create a new enumerator instance instead.
    /// The <see cref="System.Collections.IEnumerator.Reset"/> method is provided for COM interoperability. It does not necessarily need to be implemented; instead, the implementer can simply throw a <see cref="System.NotSupportedException"/>. However, if you choose to do this, you should make sure no callers are relying on the <see cref="System.Collections.IEnumerator.Reset"/> functionality.
    /// If changes are made to the collection, such as adding, modifying, or deleting elements, the behavior of the enumerator is undefined.
    /// The enumerator does not have exclusive access to the collection; therefore, enumerating through a collection is intrinsically not a thread-safe procedure. To guarantee thread safety during enumeration, you can lock the collection during the entire enumeration. To allow the collection to be accessed by multiple threads for reading and writing, you must implement your own synchronization.
    /// Default implementations of collections in the <see cref="System.Collections.Generic">Generic</see> namespace are not synchronized.
    /// The following example shows an implementation of the <see cref="System.Collections.Generic.IEnumerator{T}"/> interface for a collection class of custom objects. The custom object is an instance of the type <c>Box</c>, and the collection class is <c>BoxCollection</c>. This code example is part of a larger example provided for the <see cref="System.Collections.Generic.ICollection{T}"/> interface.
    /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/ICollectionT/Overview/program.cs" id="Snippet3" />
    /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/ICollectionT/Overview/program.vb" id="Snippet3" />
    /// </remarks>
    [Intrinsic]
    public interface IEnumerator<out T> : IDisposable, IEnumerator
        where T : allows ref struct
    {
        // Returns the current element of the enumeration. The returned value is
        // undefined before the first call to MoveNext and following a
        // call to MoveNext that returned false. Multiple calls to
        // GetCurrent with no intervening calls to MoveNext
        // will return the same object.
        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <value>The element in the collection at the current position of the enumerator.</value>
        /// <remarks>
        /// <see cref="System.Collections.Generic.IEnumerator{T}.Current"/> is undefined under any of the following conditions:
        /// - The enumerator is positioned before the first element in the collection, immediately after the enumerator is created.   <see cref="System.Collections.IEnumerator.MoveNext"/> must be called to advance the enumerator to the first element of the collection before reading the value of <see cref="System.Collections.Generic.IEnumerator{T}.Current"/>.
        /// - The last call to <see cref="System.Collections.IEnumerator.MoveNext"/> returned <c>false</c>, which indicates the end of the collection.
        /// - The enumerator is invalidated due to changes made in the collection, such as adding, modifying, or deleting elements.
        /// <see cref="System.Collections.Generic.IEnumerator{T}.Current"/> returns the same object until <see cref="System.Collections.IEnumerator.MoveNext"/> is called. <see cref="System.Collections.IEnumerator.MoveNext"/> sets <see cref="System.Collections.Generic.IEnumerator{T}.Current"/> to the next element.
        /// </remarks>
        new T Current
        {
            get;
        }

        // NOTE: An implementation of an enumerator using a ref struct T will
        // not be able to implement IEnumerator.Current to return that T (as
        // doing so would require boxing). It should throw a NotSupportedException
        // from that property implementation.
    }
}

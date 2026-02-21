// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if MONO
using System.Diagnostics.CodeAnalysis;
#endif
using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
    // Implement this interface if you need to support foreach semantics.
    /// <summary>
    /// Exposes the enumerator, which supports a simple iteration over a collection of a specified type.
    /// </summary>
    /// <typeparam name="T">The type of objects to enumerate.</typeparam>
    /// <remarks>
    /// <see cref="System.Collections.Generic.IEnumerable{T}"/> is the base interface for collections in the <see cref="System.Collections.Generic"/> namespace such as <see cref="System.Collections.Generic.List{T}"/>, <see cref="System.Collections.Generic.Dictionary{T,U}"/>, and <see cref="System.Collections.Generic.Stack{T}"/> and other generic collections such as <see cref="System.Collections.ObjectModel.ObservableCollection{T}"/> and <see cref="System.Collections.Concurrent.ConcurrentStack{T}"/>. Collections that implement <see cref="System.Collections.Generic.IEnumerable{T}"/> can be enumerated by using the <c>foreach</c> statement.
    /// For the non-generic version of this interface, see <see cref="System.Collections.IEnumerable">IEnumerable</see>.
    /// <see cref="System.Collections.Generic.IEnumerable{T}"/> contains a single method that you must implement when implementing this interface; <see cref="System.Collections.Generic.IEnumerable{T}.GetEnumerator"/>, which returns an <see cref="System.Collections.Generic.IEnumerator{T}"/> object. The returned <see cref="System.Collections.Generic.IEnumerator{T}"/> provides the ability to iterate through the collection by exposing a <see cref="System.Collections.Generic.IEnumerator{T}.Current"/> property.
    /// The following example demonstrates how to implement the <see cref="System.Collections.Generic.IEnumerable{T}"/> interface and how to use that implementation to create a LINQ query. When you implement <see cref="System.Collections.Generic.IEnumerable{T}"/>, you must also implement <see cref="System.Collections.Generic.IEnumerator{T}"/> or, for C# only, you can use the [yield](/dotnet/csharp/language-reference/keywords/yield) keyword. Implementing <see cref="System.Collections.Generic.IEnumerator{T}"/> also requires <see cref="System.IDisposable"/> to be implemented, which you will see in this example.
    /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/IEnumerableT/Overview/program.cs" id="Snippet1" />
    /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/IEnumerableT/Overview/module1.vb" id="Snippet1" />
    /// </remarks>
    /// <related type="Article" href="/dotnet/visual-basic/programming-guide/language-features/control-flow/walkthrough-implementing-ienumerable-of-t">Walkthrough: Implementing IEnumerable(Of T) in Visual Basic</related>
    /// <related type="Article" href="/dotnet/csharp/programming-guide/concepts/iterators">Iterators (C#)</related>
    /// <related type="Article" href="/dotnet/visual-basic/programming-guide/concepts/iterators">Iterators (Visual Basic)</related>
    public interface IEnumerable<out T> : IEnumerable
        where T : allows ref struct
    {
        // Returns an IEnumerator for this enumerable Object.  The enumerator provides
        // a simple way to access all the contents of a collection.
#if MONO
        [DynamicDependency(nameof(Array.InternalArray__IEnumerable_GetEnumerator) + "``1 ", typeof(Array))]
#endif
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        /// <remarks>
        /// The returned <see cref="System.Collections.Generic.IEnumerator{T}"/> provides the ability to iterate through the collection by exposing a <see cref="System.Collections.Generic.IEnumerator{T}.Current"/> property .You can use enumerators to read the data in a collection, but not to modify the collection.
        /// Initially, the enumerator is positioned before the first element in the collection. At this position, <see cref="System.Collections.Generic.IEnumerator{T}.Current"/> is undefined. Therefore, you must call the <see cref="System.Collections.IEnumerator.MoveNext"/> method to advance the enumerator to the first element of the collection before reading the value of <see cref="System.Collections.Generic.IEnumerator{T}.Current"/>.
        /// <see cref="System.Collections.Generic.IEnumerator{T}.Current"/> returns the same object until <see cref="System.Collections.IEnumerator.MoveNext"/> is called again as <see cref="System.Collections.IEnumerator.MoveNext"/> sets <see cref="System.Collections.Generic.IEnumerator{T}.Current"/> to the next element.
        /// If <see cref="System.Collections.IEnumerator.MoveNext"/> passes the end of the collection, the enumerator is positioned after the last element in the collection and <see cref="System.Collections.IEnumerator.MoveNext"/> returns <c>false</c>. When the enumerator is at this position, subsequent calls to <see cref="System.Collections.IEnumerator.MoveNext"/> also return <c>false</c>. If the last call to <see cref="System.Collections.IEnumerator.MoveNext"/> returned <c>false</c>, <see cref="System.Collections.Generic.IEnumerator{T}.Current"/> is undefined. You cannot set <see cref="System.Collections.Generic.IEnumerator{T}.Current"/> to the first element of the collection again; you must create a new enumerator instance instead.
        /// If changes are made to the collection, such as adding, modifying, or deleting elements, the behavior of the enumerator is undefined.
        /// An enumerator does not have exclusive access to the collection so an enumerator remains valid as long as the collection remains unchanged. If changes are made to the collection, such as adding, modifying, or deleting elements, the enumerator is invalidated and you may get unexpected results. Also, enumerating a collection is not a thread-safe procedure. To guarantee thread-safety, you should lock the collection during enumerator or implement synchronization on the collection.
        /// Default implementations of collections in the <see cref="System.Collections.Generic">Generic</see> namespace aren't synchronized.
        /// The following example demonstrates how to implement the <see cref="System.Collections.Generic.IEnumerable{T}"/> interface and uses that implementation to create a LINQ query. When you implement <see cref="System.Collections.Generic.IEnumerable{T}"/>, you must also implement <see cref="System.Collections.Generic.IEnumerator{T}"/> or, for C# only, you can use the [yield](/dotnet/csharp/language-reference/keywords/yield) keyword. Implementing <see cref="System.Collections.Generic.IEnumerator{T}"/> also requires <see cref="System.IDisposable"/> to be implemented, which you will see in this example.
        /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Generic/IEnumerableT/Overview/program.cs" id="Snippet1" />
        /// <code lang="vb" source="~/snippets/visualbasic/System.Collections.Generic/IEnumerableT/Overview/module1.vb" id="Snippet1" />
        /// </remarks>
        /// <related type="Article" href="/dotnet/visual-basic/programming-guide/language-features/control-flow/walkthrough-implementing-ienumerable-of-t">Walkthrough: Implementing IEnumerable(Of T) in Visual Basic</related>
        /// <related type="Article" href="/dotnet/csharp/programming-guide/concepts/iterators">Iterators (C#)</related>
        /// <related type="Article" href="/dotnet/visual-basic/programming-guide/concepts/iterators">Iterators (Visual Basic)</related>
        [Intrinsic]
        new IEnumerator<T> GetEnumerator();
    }
}

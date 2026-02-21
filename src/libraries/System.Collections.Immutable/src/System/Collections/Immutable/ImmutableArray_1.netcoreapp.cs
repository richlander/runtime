// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace System.Collections.Immutable
{
    /// <summary>
    /// Represents an array that is immutable; meaning it cannot be changed once it is created.
    /// **NuGet package**: <see href="https://www.nuget.org/packages/System.Collections.Immutable/">System.Collections.Immutable</see> (<see href="https://learn.microsoft.com/dotnet/api/system.collections.immutable?#remarks">about immutable collections and how to install</see>)
    /// </summary>
    /// <typeparam name="T">The type of element stored by the array.</typeparam>
    /// <remarks>
    /// There are different scenarios best for <see cref="System.Collections.Immutable.ImmutableArray{T}"/> and others best for <see cref="System.Collections.Immutable.ImmutableList{T}"/>.
    /// Reasons to use immutable array:
    /// - Updating the data is rare or the number of elements is quite small (less than 16 items)
    /// - You need to be able to iterate over the data in performance critical sections
    /// - You have many instances of immutable collections and you can't afford keeping the data in trees
    /// Reasons to use immutable list:
    /// - Updating the data is common or the number of elements isn't expected to be small
    /// - Updating the collection is more performance critical than iterating the contents
    /// The following table summarizes the performance characteristics of <see cref="System.Collections.Immutable.ImmutableArray{T}"/>
    /// | Operation | ImmutableArray complexity | ImmutableList complexity | Comments |
    /// | --------- | ------------------------- | ------------------------ | -------- |
    /// | <c>Item</c>    | O(1)                      | O(log n)                 | Directly index into the underlying array |
    /// | <c>Add()</c>   | O(n)                      | O(log n)                 | Requires creating a new array |
    /// This example shows how to create an immutable array and iterate over elements in it:
    /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Immutable/ImmutableArray`1/Overview/ImmutableArraySnippets.cs" id="SnippetIterate" />
    /// This example shows how to create a new immutable array by adding and removing items from the original array:
    /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Immutable/ImmutableArray`1/Overview/ImmutableArraySnippets.cs" id="SnippetModify" />
    /// This example shows how to create an immutable array using <see cref="System.Collections.Immutable.ImmutableArray{T}.Builder"/>:
    /// <code lang="csharp" source="~/snippets/csharp/System.Collections.Immutable/ImmutableArray`1/Overview/ImmutableArraySnippets.cs" id="SnippetBuilder" />
    /// </remarks>
    public readonly partial struct ImmutableArray<T> : IReadOnlyList<T>, IList<T>, IEquatable<ImmutableArray<T>>, IList, IImmutableArray, IStructuralComparable, IStructuralEquatable, IImmutableList<T>
    {
        /// <summary>
        /// Creates a <see cref="ReadOnlySpan{T}"/> over the portion of current <see cref="ImmutableArray{T}"/> based on specified <paramref name="range"/>
        /// </summary>
        /// <param name="range">Range in current <see cref="ImmutableArray{T}"/>.</param>
        /// <returns>The <see cref="ReadOnlySpan{T}"/> representation of the <see cref="ImmutableArray{T}"/></returns>
        public ReadOnlySpan<T> AsSpan(Range range)
        {
            ImmutableArray<T> self = this;
            self.ThrowNullRefIfNotInitialized();

            (int start, int length) = range.GetOffsetAndLength(self.Length);
            return new ReadOnlySpan<T>(self.array, start, length);
        }
    }
}

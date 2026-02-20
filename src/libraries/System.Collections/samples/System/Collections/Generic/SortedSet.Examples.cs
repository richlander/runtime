using System;
using System.Collections.Generic;
using System.Linq;

AddContainsRemoveClear();
SetOperations();
SetComparisons();
RangeViews();
RemoveWhereExample();
CopyToExample();
CustomComparerExample();

void AddContainsRemoveClear()
{
    SortedSet<int> numbers = new SortedSet<int>();

    // Add returns true if the element was added, false if it was already present.
    Console.WriteLine(numbers.Add(5));   // True
    Console.WriteLine(numbers.Add(3));   // True
    Console.WriteLine(numbers.Add(5));   // False (duplicate)
    Console.WriteLine(numbers.Add(1));   // True
    Console.WriteLine(numbers.Add(4));   // True

    // Elements are maintained in sorted order.
    Console.WriteLine(string.Join(", ", numbers)); // 1, 3, 4, 5

    Console.WriteLine($"Count: {numbers.Count}");       // Count: 4
    Console.WriteLine($"Contains 3: {numbers.Contains(3)}"); // Contains 3: True

    // Remove returns true if the element was found and removed.
    Console.WriteLine($"Remove 3: {numbers.Remove(3)}");     // Remove 3: True
    Console.WriteLine($"Remove 9: {numbers.Remove(9)}");     // Remove 9: False

    Console.WriteLine(string.Join(", ", numbers)); // 1, 4, 5

    numbers.Clear();
    Console.WriteLine($"Count after Clear: {numbers.Count}"); // Count after Clear: 0
}

void SetOperations()
{
    SortedSet<int> setA = new SortedSet<int> { 1, 2, 3, 4, 5 };
    SortedSet<int> setB = new SortedSet<int> { 3, 4, 5, 6, 7 };

    // UnionWith adds all elements from the other collection.
    SortedSet<int> union = new SortedSet<int>(setA);
    union.UnionWith(setB);
    Console.WriteLine($"Union: {string.Join(", ", union)}"); // Union: 1, 2, 3, 4, 5, 6, 7

    // IntersectWith keeps only elements present in both.
    SortedSet<int> intersect = new SortedSet<int>(setA);
    intersect.IntersectWith(setB);
    Console.WriteLine($"Intersect: {string.Join(", ", intersect)}"); // Intersect: 3, 4, 5

    // ExceptWith removes elements that are in the other collection.
    SortedSet<int> except = new SortedSet<int>(setA);
    except.ExceptWith(setB);
    Console.WriteLine($"Except: {string.Join(", ", except)}"); // Except: 1, 2

    // SymmetricExceptWith keeps elements in either set but not both.
    SortedSet<int> symExcept = new SortedSet<int>(setA);
    symExcept.SymmetricExceptWith(setB);
    Console.WriteLine($"SymmetricExcept: {string.Join(", ", symExcept)}"); // SymmetricExcept: 1, 2, 6, 7
}

void SetComparisons()
{
    SortedSet<int> all = new SortedSet<int> { 1, 2, 3, 4, 5 };
    SortedSet<int> subset = new SortedSet<int> { 2, 3 };
    SortedSet<int> other = new SortedSet<int> { 8, 9 };

    Console.WriteLine($"subset IsSubsetOf all: {subset.IsSubsetOf(all)}");             // True
    Console.WriteLine($"subset IsProperSubsetOf all: {subset.IsProperSubsetOf(all)}"); // True
    Console.WriteLine($"all IsSupersetOf subset: {all.IsSupersetOf(subset)}");         // True
    Console.WriteLine($"all IsProperSupersetOf subset: {all.IsProperSupersetOf(subset)}"); // True

    Console.WriteLine($"subset Overlaps all: {subset.Overlaps(all)}");   // True
    Console.WriteLine($"other Overlaps all: {other.Overlaps(all)}");     // False

    SortedSet<int> copy = new SortedSet<int> { 1, 2, 3, 4, 5 };
    Console.WriteLine($"all SetEquals copy: {all.SetEquals(copy)}"); // True
}

void RangeViews()
{
    SortedSet<int> numbers = new SortedSet<int> { 10, 20, 30, 40, 50, 60, 70, 80 };

    Console.WriteLine($"Min: {numbers.Min}"); // Min: 10
    Console.WriteLine($"Max: {numbers.Max}"); // Max: 80

    // GetViewBetween returns a subset view including both bounds.
    SortedSet<int> view = numbers.GetViewBetween(30, 60);
    Console.WriteLine($"View [30..60]: {string.Join(", ", view)}"); // View [30..60]: 30, 40, 50, 60

    // Reverse enumerates elements in descending order.
    Console.WriteLine($"Descending: {string.Join(", ", numbers.Reverse())}");
    // Descending: 80, 70, 60, 50, 40, 30, 20, 10
}

void RemoveWhereExample()
{
    SortedSet<int> numbers = new SortedSet<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

    // RemoveWhere removes all elements matching a predicate and returns the count removed.
    int removed = numbers.RemoveWhere(n => n % 2 == 0);
    Console.WriteLine($"Removed {removed} even numbers"); // Removed 5 even numbers
    Console.WriteLine(string.Join(", ", numbers));         // 1, 3, 5, 7, 9
}

void CopyToExample()
{
    SortedSet<string> fruits = new SortedSet<string> { "apple", "banana", "cherry", "date" };

    string[] array = new string[fruits.Count];
    fruits.CopyTo(array);
    Console.WriteLine(string.Join(", ", array)); // apple, banana, cherry, date

    // CopyTo with index and count copies a subset into a destination array.
    string[] partial = new string[6];
    fruits.CopyTo(partial, 1, 2);
    Console.WriteLine($"partial[1] = {partial[1]}, partial[2] = {partial[2]}"); // partial[1] = apple, partial[2] = banana
}

void CustomComparerExample()
{
    // Use a custom comparer for case-insensitive string sorting.
    SortedSet<string> names = new SortedSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "Alice", "bob", "CHARLIE"
    };

    Console.WriteLine($"Comparer: {names.Comparer}"); // Comparer: System.OrdinalIgnoreComparer
    Console.WriteLine($"Contains 'BOB': {names.Contains("BOB")}"); // Contains 'BOB': True
    Console.WriteLine(string.Join(", ", names)); // Alice, bob, CHARLIE
}

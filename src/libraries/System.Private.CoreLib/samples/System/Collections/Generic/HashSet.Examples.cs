using System;
using System.Collections.Generic;

AddAndContains();
SetOperations();
SetComparisons();
Capacity();

Console.WriteLine("All examples passed.");
return 0;

void AddAndContains()
{
    HashSet<string> names = ["Alice", "Bob", "Charlie"];

    Console.WriteLine($"Count: {names.Count}");
    // Output: Count: 3

    bool added = names.Add("Diana");
    Console.WriteLine($"Added Diana: {added}");
    // Output: Added Diana: True

    added = names.Add("Alice");
    Console.WriteLine($"Added Alice again: {added}");
    // Output: Added Alice again: False

    Console.WriteLine($"Contains Bob: {names.Contains("Bob")}");
    // Output: Contains Bob: True

    bool removed = names.Remove("Bob");
    Console.WriteLine($"Removed Bob: {removed}");
    // Output: Removed Bob: True

    Console.WriteLine($"Count after remove: {names.Count}");
    // Output: Count after remove: 3

    names.Clear();
    Console.WriteLine($"Count after clear: {names.Count}");
    // Output: Count after clear: 0
}

void SetOperations()
{
    HashSet<int> evens = [2, 4, 6, 8];
    HashSet<int> odds = [1, 3, 5, 7];
    HashSet<int> primes = [2, 3, 5, 7];

    // UnionWith adds all elements from the other collection
    HashSet<int> union = new(evens);
    union.UnionWith(odds);
    Console.WriteLine($"Union: {string.Join(", ", union)}");
    // Output includes all of: 1, 2, 3, 4, 5, 6, 7, 8

    // IntersectWith keeps only elements present in both
    HashSet<int> intersect = new(odds);
    intersect.IntersectWith(primes);
    Console.WriteLine($"Odds ∩ Primes: {string.Join(", ", intersect)}");
    // Output includes: 3, 5, 7

    // ExceptWith removes elements found in the other collection
    HashSet<int> except = new(primes);
    except.ExceptWith(odds);
    Console.WriteLine($"Primes - Odds: {string.Join(", ", except)}");
    // Output: Primes - Odds: 2

    // SymmetricExceptWith keeps elements in either set but not both
    HashSet<int> symmetric = new(evens);
    symmetric.SymmetricExceptWith(primes);
    Console.WriteLine($"Evens △ Primes: {string.Join(", ", symmetric)}");
    // Output includes: 3, 4, 5, 6, 7, 8 (but not 2)
}

void SetComparisons()
{
    HashSet<int> all = [1, 2, 3, 4, 5];
    HashSet<int> subset = [2, 3];
    HashSet<int> same = [1, 2, 3, 4, 5];
    HashSet<int> other = [4, 5, 6];

    Console.WriteLine($"subset ⊂ all (proper): {subset.IsProperSubsetOf(all)}");
    // Output: subset ⊂ all (proper): True

    Console.WriteLine($"subset ⊆ all: {subset.IsSubsetOf(all)}");
    // Output: subset ⊆ all: True

    Console.WriteLine($"all ⊃ subset (proper): {all.IsProperSupersetOf(subset)}");
    // Output: all ⊃ subset (proper): True

    Console.WriteLine($"all ⊇ subset: {all.IsSupersetOf(subset)}");
    // Output: all ⊇ subset: True

    Console.WriteLine($"all == same: {all.SetEquals(same)}");
    // Output: all == same: True

    Console.WriteLine($"all overlaps other: {all.Overlaps(other)}");
    // Output: all overlaps other: True

    // A set is a subset of itself but not a proper subset
    Console.WriteLine($"all ⊆ same: {all.IsSubsetOf(same)}");
    // Output: all ⊆ same: True
    Console.WriteLine($"all ⊂ same (proper): {all.IsProperSubsetOf(same)}");
    // Output: all ⊂ same (proper): False
}

void Capacity()
{
    HashSet<int> numbers = new();

    // Reserve space for at least 100 elements
    int capacity = numbers.EnsureCapacity(100);
    Console.WriteLine($"Capacity after EnsureCapacity(100): {capacity}");
    // Output: Capacity after EnsureCapacity(100): >= 100

    numbers.Add(1);
    numbers.Add(2);
    numbers.Add(3);

    // TrimExcess reduces capacity to match the count
    numbers.TrimExcess();
    Console.WriteLine($"Count: {numbers.Count}");
    // Output: Count: 3
}

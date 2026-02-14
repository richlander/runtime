// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// Runnable examples for HashSet<T> XML doc comments.
// Each #region is referenced by <code> tags in HashSet.cs.
// Run: dotnet run HashSet.Examples.cs

using System.Collections.Generic;

int failures = 0;
void Fail(string name, string message) { Console.Error.WriteLine($"FAIL: {name} — {message}"); failures++; }

// --- Add ---
#region Add
var set = new HashSet<string>();
bool added1 = set.Add("apple");
bool added2 = set.Add("apple");
Console.WriteLine($"First add: {added1}, Second add: {added2}");
// Output: First add: True, Second add: False
#endregion
if (!added1) Fail("Add", "First add should return true");
if (added2) Fail("Add", "Duplicate add should return false");

// --- Remove ---
#region Remove
var fruits = new HashSet<string> { "apple", "banana", "cherry" };
bool removed = fruits.Remove("banana");
bool removedAgain = fruits.Remove("banana");
Console.WriteLine($"Removed: {removed}, Removed again: {removedAgain}");
// Output: Removed: True, Removed again: False
#endregion
if (!removed) Fail("Remove", "Should remove existing element");
if (removedAgain) Fail("Remove", "Should not remove missing element");

// --- Contains ---
#region Contains
var colors = new HashSet<string> { "red", "green", "blue" };
Console.WriteLine($"Contains 'green': {colors.Contains("green")}");
Console.WriteLine($"Contains 'yellow': {colors.Contains("yellow")}");
// Output:
// Contains 'green': True
// Contains 'yellow': False
#endregion
if (!colors.Contains("green")) Fail("Contains", "Should contain 'green'");
if (colors.Contains("yellow")) Fail("Contains", "Should not contain 'yellow'");

// --- Clear ---
#region Clear
var numbers = new HashSet<int> { 1, 2, 3 };
numbers.Clear();
Console.WriteLine($"Count after Clear: {numbers.Count}");
// Output: Count after Clear: 0
#endregion
if (numbers.Count != 0) Fail("Clear", "Count should be 0 after Clear");

// --- Count ---
#region Count
var letters = new HashSet<char> { 'a', 'b', 'c', 'd' };
Console.WriteLine($"Count: {letters.Count}");
// Output: Count: 4
#endregion
if (letters.Count != 4) Fail("Count", $"Expected 4, got {letters.Count}");

// --- UnionWith ---
#region UnionWith
var evens = new HashSet<int> { 2, 4, 6 };
var primes = new HashSet<int> { 2, 3, 5, 7 };
evens.UnionWith(primes);
Console.WriteLine($"Union: {string.Join(", ", new SortedSet<int>(evens))}");
// Output: Union: 2, 3, 4, 5, 6, 7
#endregion
if (evens.Count != 6) Fail("UnionWith", $"Expected 6 elements, got {evens.Count}");

// --- IntersectWith ---
#region IntersectWith
var a = new HashSet<int> { 1, 2, 3, 4, 5 };
var b = new HashSet<int> { 3, 4, 5, 6, 7 };
a.IntersectWith(b);
Console.WriteLine($"Intersection: {string.Join(", ", new SortedSet<int>(a))}");
// Output: Intersection: 3, 4, 5
#endregion
if (!a.SetEquals(new[] { 3, 4, 5 })) Fail("IntersectWith", "Unexpected intersection result");

// --- ExceptWith ---
#region ExceptWith
var all = new HashSet<int> { 1, 2, 3, 4, 5 };
var remove = new HashSet<int> { 2, 4 };
all.ExceptWith(remove);
Console.WriteLine($"Except: {string.Join(", ", new SortedSet<int>(all))}");
// Output: Except: 1, 3, 5
#endregion
if (!all.SetEquals(new[] { 1, 3, 5 })) Fail("ExceptWith", "Unexpected except result");

// --- SymmetricExceptWith ---
#region SymmetricExceptWith
var x = new HashSet<int> { 1, 2, 3, 4 };
var y = new HashSet<int> { 3, 4, 5, 6 };
x.SymmetricExceptWith(y);
Console.WriteLine($"Symmetric difference: {string.Join(", ", new SortedSet<int>(x))}");
// Output: Symmetric difference: 1, 2, 5, 6
#endregion
if (!x.SetEquals(new[] { 1, 2, 5, 6 })) Fail("SymmetricExceptWith", "Unexpected symmetric difference");

// --- IsSubsetOf ---
#region IsSubsetOf
var subset = new HashSet<int> { 1, 2 };
var superset = new HashSet<int> { 1, 2, 3, 4 };
Console.WriteLine($"{{1,2}} ⊆ {{1,2,3,4}}: {subset.IsSubsetOf(superset)}");
Console.WriteLine($"{{1,2,3,4}} ⊆ {{1,2}}: {superset.IsSubsetOf(subset)}");
// Output:
// {1,2} ⊆ {1,2,3,4}: True
// {1,2,3,4} ⊆ {1,2}: False
#endregion
if (!subset.IsSubsetOf(superset)) Fail("IsSubsetOf", "Should be a subset");
if (superset.IsSubsetOf(subset)) Fail("IsSubsetOf", "Should not be a subset");

// --- IsSupersetOf ---
#region IsSupersetOf
var big = new HashSet<int> { 1, 2, 3, 4, 5 };
var small = new HashSet<int> { 2, 3 };
Console.WriteLine($"{{1..5}} ⊇ {{2,3}}: {big.IsSupersetOf(small)}");
Console.WriteLine($"{{2,3}} ⊇ {{1..5}}: {small.IsSupersetOf(big)}");
// Output:
// {1..5} ⊇ {2,3}: True
// {2,3} ⊇ {1..5}: False
#endregion
if (!big.IsSupersetOf(small)) Fail("IsSupersetOf", "Should be a superset");
if (small.IsSupersetOf(big)) Fail("IsSupersetOf", "Should not be a superset");

// --- Overlaps ---
#region Overlaps
var odds = new HashSet<int> { 1, 3, 5, 7 };
var evens2 = new HashSet<int> { 2, 4, 6 };
var mixed = new HashSet<int> { 5, 6, 7, 8 };
Console.WriteLine($"Odds overlaps evens: {odds.Overlaps(evens2)}");
Console.WriteLine($"Odds overlaps mixed: {odds.Overlaps(mixed)}");
// Output:
// Odds overlaps evens: False
// Odds overlaps mixed: True
#endregion
if (odds.Overlaps(evens2)) Fail("Overlaps", "Odds and evens should not overlap");
if (!odds.Overlaps(mixed)) Fail("Overlaps", "Odds and mixed should overlap");

// --- SetEquals ---
#region SetEquals
var first = new HashSet<int> { 1, 2, 3 };
var second = new HashSet<int> { 3, 2, 1 };
var third = new HashSet<int> { 1, 2, 4 };
Console.WriteLine($"{{1,2,3}} equals {{3,2,1}}: {first.SetEquals(second)}");
Console.WriteLine($"{{1,2,3}} equals {{1,2,4}}: {first.SetEquals(third)}");
// Output:
// {1,2,3} equals {3,2,1}: True
// {1,2,3} equals {1,2,4}: False
#endregion
if (!first.SetEquals(second)) Fail("SetEquals", "Sets with same elements should be equal");
if (first.SetEquals(third)) Fail("SetEquals", "Different sets should not be equal");

// --- RemoveWhere ---
#region RemoveWhere
var values = new HashSet<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
int removedCount = values.RemoveWhere(n => n % 2 == 0);
Console.WriteLine($"Removed {removedCount} even numbers, remaining: {string.Join(", ", new SortedSet<int>(values))}");
// Output: Removed 5 even numbers, remaining: 1, 3, 5, 7, 9
#endregion
if (removedCount != 5) Fail("RemoveWhere", $"Expected 5 removed, got {removedCount}");
if (!values.SetEquals(new[] { 1, 3, 5, 7, 9 })) Fail("RemoveWhere", "Unexpected remaining elements");

// --- TrimExcess ---
#region TrimExcess
var large = new HashSet<int>(capacity: 1000);
for (int i = 0; i < 10; i++) large.Add(i);
large.TrimExcess();
Console.WriteLine($"Count after TrimExcess: {large.Count}");
// Output: Count after TrimExcess: 10
#endregion
if (large.Count != 10) Fail("TrimExcess", $"Expected 10, got {large.Count}");

if (failures > 0) { Console.Error.WriteLine($"\n{failures} example(s) failed."); return 1; }
Console.WriteLine("\nAll examples passed.");
return 0;

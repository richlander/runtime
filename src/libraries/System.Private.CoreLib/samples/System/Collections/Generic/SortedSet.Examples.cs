// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// Runnable examples for SortedSet<T> XML doc comments.
// Each #region is referenced by <code> tags in SortedSet.cs.
// Run: dotnet run SortedSet.Examples.cs

using System.Collections.Generic;

int failures = 0;
void Fail(string name, string message) { Console.Error.WriteLine($"FAIL: {name} — {message}"); failures++; }

// --- Add ---
#region Add
var set = new SortedSet<int>();
bool added1 = set.Add(3);
set.Add(1);
set.Add(2);
bool added2 = set.Add(3);
Console.WriteLine($"Added 3 first time: {added1}, second time: {added2}");
Console.WriteLine($"Sorted order: {string.Join(", ", set)}");
// Output:
// Added 3 first time: True, second time: False
// Sorted order: 1, 2, 3
#endregion
if (!added1) Fail("Add", "First add should return true");
if (added2) Fail("Add", "Duplicate add should return false");
if (string.Join(", ", set) != "1, 2, 3") Fail("Add", "Elements should be sorted");

// --- Remove ---
#region Remove
var fruits = new SortedSet<string> { "apple", "banana", "cherry", "date" };
bool removed = fruits.Remove("banana");
Console.WriteLine($"Removed 'banana': {removed}, Remaining: {string.Join(", ", fruits)}");
// Output: Removed 'banana': True, Remaining: apple, cherry, date
#endregion
if (!removed) Fail("Remove", "Should remove existing element");
if (fruits.Contains("banana")) Fail("Remove", "'banana' should be gone");

// --- Contains ---
#region Contains
var primes = new SortedSet<int> { 2, 3, 5, 7, 11, 13 };
Console.WriteLine($"Contains 7: {primes.Contains(7)}");
Console.WriteLine($"Contains 4: {primes.Contains(4)}");
// Output:
// Contains 7: True
// Contains 4: False
#endregion
if (!primes.Contains(7)) Fail("Contains", "Should contain 7");
if (primes.Contains(4)) Fail("Contains", "Should not contain 4");

// --- Count ---
#region Count
var letters = new SortedSet<char> { 'z', 'a', 'm', 'f' };
Console.WriteLine($"Count: {letters.Count}, First: {letters.Min}, Last: {letters.Max}");
// Output: Count: 4, First: a, Last: z
#endregion
if (letters.Count != 4) Fail("Count", $"Expected 4, got {letters.Count}");

// --- Min and Max ---
#region MinMax
var scores = new SortedSet<int> { 85, 92, 78, 95, 88 };
Console.WriteLine($"Min: {scores.Min}, Max: {scores.Max}");
// Output: Min: 78, Max: 95
#endregion
if (scores.Min != 78) Fail("MinMax", $"Expected Min 78, got {scores.Min}");
if (scores.Max != 95) Fail("MinMax", $"Expected Max 95, got {scores.Max}");

// --- GetViewBetween ---
#region GetViewBetween
var numbers = new SortedSet<int> { 10, 20, 30, 40, 50, 60, 70, 80 };
var view = numbers.GetViewBetween(25, 65);
Console.WriteLine($"View [25..65]: {string.Join(", ", view)}");
// Output: View [25..65]: 30, 40, 50, 60
#endregion
if (view.Count != 4) Fail("GetViewBetween", $"Expected 4 elements in view, got {view.Count}");
if (view.Min != 30 || view.Max != 60) Fail("GetViewBetween", "Unexpected view bounds");

// --- UnionWith ---
#region UnionWith
var evens = new SortedSet<int> { 2, 4, 6 };
var odds = new SortedSet<int> { 1, 3, 5 };
evens.UnionWith(odds);
Console.WriteLine($"Union: {string.Join(", ", evens)}");
// Output: Union: 1, 2, 3, 4, 5, 6
#endregion
if (evens.Count != 6) Fail("UnionWith", $"Expected 6 elements, got {evens.Count}");

// --- IntersectWith ---
#region IntersectWith
var a = new SortedSet<int> { 1, 2, 3, 4, 5 };
var b = new SortedSet<int> { 3, 4, 5, 6, 7 };
a.IntersectWith(b);
Console.WriteLine($"Intersection: {string.Join(", ", a)}");
// Output: Intersection: 3, 4, 5
#endregion
if (!a.SetEquals([3, 4, 5])) Fail("IntersectWith", "Unexpected intersection result");

// --- ExceptWith ---
#region ExceptWith
var all = new SortedSet<int> { 1, 2, 3, 4, 5 };
all.ExceptWith([2, 4]);
Console.WriteLine($"Except: {string.Join(", ", all)}");
// Output: Except: 1, 3, 5
#endregion
if (!all.SetEquals([1, 3, 5])) Fail("ExceptWith", "Unexpected except result");

// --- RemoveWhere ---
#region RemoveWhere
var values = new SortedSet<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
int removedCount = values.RemoveWhere(n => n > 5);
Console.WriteLine($"Removed {removedCount} items > 5, remaining: {string.Join(", ", values)}");
// Output: Removed 5 items > 5, remaining: 1, 2, 3, 4, 5
#endregion
if (removedCount != 5) Fail("RemoveWhere", $"Expected 5 removed, got {removedCount}");

// --- Reverse ---
#region Reverse
var ascending = new SortedSet<int> { 1, 2, 3, 4, 5 };
var descending = new List<int>();
foreach (var item in ascending.Reverse())
    descending.Add(item);
Console.WriteLine($"Reversed: {string.Join(", ", descending)}");
// Output: Reversed: 5, 4, 3, 2, 1
#endregion
if (string.Join(", ", descending) != "5, 4, 3, 2, 1") Fail("Reverse", "Unexpected reverse order");

// --- Custom IComparer<T> ---
#region CustomComparer
var caseInsensitive = new SortedSet<string>(StringComparer.OrdinalIgnoreCase)
{
    "Banana", "apple", "CHERRY", "date"
};
Console.WriteLine($"Case-insensitive order: {string.Join(", ", caseInsensitive)}");
bool addedDuplicate = caseInsensitive.Add("APPLE");
Console.WriteLine($"Added 'APPLE' (duplicate): {addedDuplicate}");
// Output:
// Case-insensitive order: apple, Banana, CHERRY, date
// Added 'APPLE' (duplicate): False
#endregion
if (addedDuplicate) Fail("CustomComparer", "'APPLE' should be duplicate of 'apple'");
if (caseInsensitive.Count != 4) Fail("CustomComparer", $"Expected 4, got {caseInsensitive.Count}");

if (failures > 0) { Console.Error.WriteLine($"\n{failures} example(s) failed."); return 1; }
Console.WriteLine("\nAll examples passed.");
return 0;

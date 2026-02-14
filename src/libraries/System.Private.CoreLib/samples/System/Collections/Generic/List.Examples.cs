// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// Runnable examples for List<T> XML doc comments.
// Each #region is referenced by <code> tags in List.cs.
// Run: dotnet run List.Examples.cs

using System.Collections.Generic;
using System.Collections.ObjectModel;

int failures = 0;

void Fail(string name, string message)
{
    Console.Error.WriteLine($"FAIL: {name} — {message}");
    failures++;
}

#region Ctor
// Create an empty list of strings.
var names = new List<string>();
Console.WriteLine($"Empty list count: {names.Count}");
#endregion
if (names.Count != 0) Fail("Ctor", $"Expected 0, got {names.Count}");

#region CtorCapacity
// Pre-allocate room for 100 elements to avoid repeated resizing.
var scores = new List<int>(100);
Console.WriteLine($"Capacity: {scores.Capacity}, Count: {scores.Count}");
#endregion
if (scores.Capacity < 100) Fail("CtorCapacity", $"Expected capacity >= 100, got {scores.Capacity}");
if (scores.Count != 0) Fail("CtorCapacity", $"Expected count 0, got {scores.Count}");

#region CtorIEnumerable
// Initialize a list from an existing collection.
int[] source = [10, 20, 30];
var numbers = new List<int>(source);
Console.WriteLine($"Copied {numbers.Count} items from array");
#endregion
if (numbers.Count != 3) Fail("CtorIEnumerable", $"Expected 3, got {numbers.Count}");

#region Add
// Add elements one at a time.
var fruits = new List<string>();
fruits.Add("apple");
fruits.Add("banana");
fruits.Add("cherry");
Console.WriteLine($"Fruits: {string.Join(", ", fruits)}");
#endregion
if (fruits.Count != 3) Fail("Add", $"Expected 3, got {fruits.Count}");

#region AddRange
// Append multiple elements from another collection.
var more = new List<int>([1, 2, 3]);
more.AddRange([4, 5, 6]);
Console.WriteLine($"After AddRange: {string.Join(", ", more)}");
#endregion
if (more.Count != 6) Fail("AddRange", $"Expected 6, got {more.Count}");
if (more[5] != 6) Fail("AddRange", $"Expected last element 6, got {more[5]}");

#region Insert
// Insert an element at a specific position.
var colors = new List<string>(["red", "blue"]);
colors.Insert(1, "green");
Console.WriteLine($"Colors: {string.Join(", ", colors)}");
#endregion
if (colors[1] != "green") Fail("Insert", $"Expected 'green' at index 1, got '{colors[1]}'");

#region InsertRange
// Insert multiple elements starting at a given index.
var letters = new List<char>(['a', 'd', 'e']);
letters.InsertRange(1, ['b', 'c']);
Console.WriteLine($"Letters: {string.Join(", ", letters)}");
#endregion
if (letters.Count != 5) Fail("InsertRange", $"Expected 5, got {letters.Count}");
if (letters[2] != 'c') Fail("InsertRange", $"Expected 'c' at index 2, got '{letters[2]}'");

#region Remove
// Remove the first occurrence of a value.
var pets = new List<string>(["cat", "dog", "cat", "bird"]);
bool removed = pets.Remove("cat");
Console.WriteLine($"Removed 'cat': {removed}, remaining: {string.Join(", ", pets)}");
#endregion
if (!removed) Fail("Remove", "Expected true");
if (pets[0] != "dog") Fail("Remove", $"Expected 'dog' at index 0, got '{pets[0]}'");

#region RemoveAt
// Remove the element at a specific index.
var values = new List<int>([10, 20, 30, 40]);
values.RemoveAt(1);
Console.WriteLine($"After RemoveAt(1): {string.Join(", ", values)}");
#endregion
if (values.Count != 3) Fail("RemoveAt", $"Expected 3, got {values.Count}");
if (values[1] != 30) Fail("RemoveAt", $"Expected 30 at index 1, got {values[1]}");

#region RemoveAll
// Remove all elements matching a predicate.
var nums = new List<int>([1, 2, 3, 4, 5, 6, 7, 8]);
int removedCount = nums.RemoveAll(n => n % 2 == 0);
Console.WriteLine($"Removed {removedCount} even numbers: {string.Join(", ", nums)}");
#endregion
if (removedCount != 4) Fail("RemoveAll", $"Expected 4 removed, got {removedCount}");
if (nums.Count != 4) Fail("RemoveAll", $"Expected 4 remaining, got {nums.Count}");

#region RemoveRange
// Remove a contiguous block of elements.
var digits = new List<int>([0, 1, 2, 3, 4, 5]);
digits.RemoveRange(2, 3); // remove indices 2, 3, 4
Console.WriteLine($"After RemoveRange(2, 3): {string.Join(", ", digits)}");
#endregion
if (digits.Count != 3) Fail("RemoveRange", $"Expected 3, got {digits.Count}");
if (digits[2] != 5) Fail("RemoveRange", $"Expected 5 at index 2, got {digits[2]}");

#region Clear
// Remove all elements from the list.
var temp = new List<int>([1, 2, 3]);
temp.Clear();
Console.WriteLine($"After Clear: Count = {temp.Count}");
#endregion
if (temp.Count != 0) Fail("Clear", $"Expected 0, got {temp.Count}");

#region Contains
// Check whether an element is in the list.
var cities = new List<string>(["Paris", "London", "Tokyo"]);
bool hasParis = cities.Contains("Paris");
bool hasBerlin = cities.Contains("Berlin");
Console.WriteLine($"Contains Paris: {hasParis}, Berlin: {hasBerlin}");
#endregion
if (!hasParis) Fail("Contains", "Expected true for Paris");
if (hasBerlin) Fail("Contains", "Expected false for Berlin");

#region IndexOf
// Find the zero-based index of the first occurrence of a value.
var items = new List<string>(["one", "two", "three", "two"]);
int idx = items.IndexOf("two");
int missing = items.IndexOf("four");
Console.WriteLine($"IndexOf 'two': {idx}, IndexOf 'four': {missing}");
#endregion
if (idx != 1) Fail("IndexOf", $"Expected 1, got {idx}");
if (missing != -1) Fail("IndexOf", $"Expected -1, got {missing}");

#region Find
// Find the first element matching a predicate.
var ages = new List<int>([12, 25, 17, 34, 8]);
int? firstAdult = ages.Find(age => age >= 18);
Console.WriteLine($"First adult age: {firstAdult}");
#endregion
if (firstAdult != 25) Fail("Find", $"Expected 25, got {firstAdult}");

#region FindAll
// Find all elements matching a predicate.
var data = new List<int>([3, 1, 4, 1, 5, 9, 2, 6]);
List<int> greaterThanFour = data.FindAll(x => x > 4);
Console.WriteLine($"Greater than 4: {string.Join(", ", greaterThanFour)}");
#endregion
if (greaterThanFour.Count != 3) Fail("FindAll", $"Expected 3, got {greaterThanFour.Count}");

#region FindIndex
// Find the index of the first element matching a predicate.
var words = new List<string>(["hi", "hello", "hey", "howdy"]);
int foundIdx = words.FindIndex(w => w.Length > 3);
Console.WriteLine($"First word longer than 3 chars at index: {foundIdx}");
#endregion
if (foundIdx != 1) Fail("FindIndex", $"Expected 1, got {foundIdx}");

#region Exists
// Check whether any element matches a predicate.
var temperatures = new List<double>([36.5, 37.0, 38.5, 36.8]);
bool hasFever = temperatures.Exists(t => t > 38.0);
Console.WriteLine($"Has fever reading: {hasFever}");
#endregion
if (!hasFever) Fail("Exists", "Expected true");

#region BinarySearch
// Use BinarySearch on a sorted list to find or insert in order.
var sorted = new List<int>([1, 3, 5, 7, 9]);
int index = sorted.BinarySearch(5);
Console.WriteLine($"BinarySearch(5): found at index {index}");

// Insert a value into the correct sorted position.
int probe = sorted.BinarySearch(6);
// A negative result is the bitwise complement of the insertion point.
sorted.Insert(~probe, 6);
Console.WriteLine($"After inserting 6: {string.Join(", ", sorted)}");
#endregion
if (index != 2) Fail("BinarySearch", $"Expected index 2, got {index}");
if (sorted[3] != 6) Fail("BinarySearch", $"Expected 6 at index 3, got {sorted[3]}");

#region Sort
// Sort elements in ascending order using default comparison.
var unsorted = new List<int>([5, 3, 8, 1, 4]);
unsorted.Sort();
Console.WriteLine($"Sorted: {string.Join(", ", unsorted)}");
#endregion
if (unsorted[0] != 1) Fail("Sort", $"Expected 1 first, got {unsorted[0]}");
if (unsorted[4] != 8) Fail("Sort", $"Expected 8 last, got {unsorted[4]}");

#region SortComparison
// Sort with a custom comparison — here, descending by string length.
var langs = new List<string>(["Go", "Rust", "C#", "Python", "JavaScript"]);
langs.Sort((a, b) => b.Length.CompareTo(a.Length));
Console.WriteLine($"By length desc: {string.Join(", ", langs)}");
#endregion
if (langs[0] != "JavaScript") Fail("SortComparison", $"Expected 'JavaScript' first, got '{langs[0]}'");

#region ConvertAll
// Project every element into a new form.
var prices = new List<double>([9.99, 19.50, 4.95]);
List<string> labels = prices.ConvertAll(p => p.ToString("C2"));
Console.WriteLine($"Labels: {string.Join(", ", labels)}");
#endregion
if (labels.Count != 3) Fail("ConvertAll", $"Expected 3, got {labels.Count}");

#region ForEach
// Execute an action on every element.
var log = new List<string>();
var range = new List<int>([1, 2, 3]);
range.ForEach(n => log.Add($"Item {n}"));
Console.WriteLine($"Log: {string.Join("; ", log)}");
#endregion
if (log.Count != 3) Fail("ForEach", $"Expected 3 log entries, got {log.Count}");

#region GetRange
// Extract a shallow copy of a portion of the list.
var alphabet = new List<char>(['a', 'b', 'c', 'd', 'e']);
List<char> middle = alphabet.GetRange(1, 3);
Console.WriteLine($"GetRange(1, 3): {string.Join(", ", middle)}");
#endregion
if (middle.Count != 3) Fail("GetRange", $"Expected 3, got {middle.Count}");
if (middle[0] != 'b') Fail("GetRange", $"Expected 'b', got '{middle[0]}'");

#region ToArray
// Copy the list into a new array.
var src = new List<int>([10, 20, 30]);
int[] arr = src.ToArray();
Console.WriteLine($"Array: {string.Join(", ", arr)}");
#endregion
if (arr.Length != 3) Fail("ToArray", $"Expected length 3, got {arr.Length}");

#region AsReadOnly
// Get a read-only wrapper around the list.
var mutable = new List<string>(["x", "y", "z"]);
ReadOnlyCollection<string> readOnly = mutable.AsReadOnly();
Console.WriteLine($"ReadOnly count: {readOnly.Count}, first: {readOnly[0]}");
#endregion
if (readOnly.Count != 3) Fail("AsReadOnly", $"Expected 3, got {readOnly.Count}");

#region TrimExcess
// Reduce capacity to match the current count.
var big = new List<int>(1000);
big.AddRange([1, 2, 3]);
Console.WriteLine($"Before TrimExcess: Capacity = {big.Capacity}");
big.TrimExcess();
Console.WriteLine($"After TrimExcess: Capacity = {big.Capacity}");
#endregion
if (big.Capacity > 100) Fail("TrimExcess", $"Expected capacity near 3, got {big.Capacity}");

#region EnsureCapacity
// Guarantee room for at least N elements without repeated resizing.
var growing = new List<int>();
int newCap = growing.EnsureCapacity(500);
Console.WriteLine($"EnsureCapacity(500): capacity is now {newCap}");
#endregion
if (newCap < 500) Fail("EnsureCapacity", $"Expected >= 500, got {newCap}");

if (failures > 0)
{
    Console.Error.WriteLine($"\n{failures} example(s) failed.");
    return 1;
}

Console.WriteLine("\nAll examples passed.");
return 0;

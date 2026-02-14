// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// Runnable examples for SortedList<TKey,TValue> XML doc comments.
// Each #region is referenced by <code> tags in SortedList.cs.
// Run: dotnet run SortedList.Examples.cs

using System.Collections.Generic;

int failures = 0;
void Fail(string name, string message) { Console.Error.WriteLine($"FAIL: {name} — {message}"); failures++; }

// <region Add>
#region Add
var planets = new SortedList<string, int>();
planets.Add("Mercury", 1);
planets.Add("Venus", 2);
planets.Add("Earth", 3);
// Entries are maintained in sorted key order.
#endregion
// </region Add>

if (planets.Count != 3) Fail("Add", $"Expected 3, got {planets.Count}");
if (planets.Keys[0] != "Earth") Fail("Add", $"Expected 'Earth' first, got '{planets.Keys[0]}'");

// <region IndexerGetSet>
#region IndexerGetSet
var scores = new SortedList<string, int>
{
    ["Alice"] = 90,
    ["Bob"] = 85
};
scores["Alice"] = 95;   // Update existing entry.
scores["Carol"] = 88;   // Add new entry via indexer.
int aliceScore = scores["Alice"];
#endregion
// </region IndexerGetSet>

if (aliceScore != 95) Fail("IndexerGetSet", $"Expected 95, got {aliceScore}");
if (scores.Count != 3) Fail("IndexerGetSet", $"Expected 3, got {scores.Count}");

// <region TryGetValue>
#region TryGetValue
var capitals = new SortedList<string, string>
{
    ["France"] = "Paris",
    ["Japan"] = "Tokyo"
};
bool found = capitals.TryGetValue("Japan", out string? city);
bool missing = capitals.TryGetValue("Brazil", out string? noCity);
#endregion
// </region TryGetValue>

if (!found || city != "Tokyo") Fail("TryGetValue", "Expected Tokyo");
if (missing || noCity is not null) Fail("TryGetValue", "Expected false/null for missing key");

// <region ContainsKey>
#region ContainsKey
var inventory = new SortedList<string, int> { ["Apples"] = 5, ["Bananas"] = 3 };
bool hasApples = inventory.ContainsKey("Apples");   // true
bool hasGrapes = inventory.ContainsKey("Grapes");   // false
#endregion
// </region ContainsKey>

if (!hasApples) Fail("ContainsKey", "Expected true for Apples");
if (hasGrapes) Fail("ContainsKey", "Expected false for Grapes");

// <region ContainsValue>
#region ContainsValue
var ages = new SortedList<string, int> { ["Alice"] = 30, ["Bob"] = 25 };
bool has30 = ages.ContainsValue(30);   // true — linear scan
bool has99 = ages.ContainsValue(99);   // false
#endregion
// </region ContainsValue>

if (!has30) Fail("ContainsValue", "Expected true for 30");
if (has99) Fail("ContainsValue", "Expected false for 99");

// <region Remove>
#region Remove
var colors = new SortedList<string, string>
{
    ["B"] = "Blue",
    ["G"] = "Green",
    ["R"] = "Red"
};
bool removed = colors.Remove("G");     // true
bool notFound = colors.Remove("X");    // false — key absent
#endregion
// </region Remove>

if (!removed) Fail("Remove", "Expected true for 'G'");
if (notFound) Fail("Remove", "Expected false for 'X'");
if (colors.Count != 2) Fail("Remove", $"Expected 2, got {colors.Count}");

// <region RemoveAt>
#region RemoveAt
var letters = new SortedList<string, int> { ["A"] = 1, ["B"] = 2, ["C"] = 3 };
letters.RemoveAt(1);   // Removes the entry at index 1 ("B").
#endregion
// </region RemoveAt>

if (letters.Count != 2) Fail("RemoveAt", $"Expected 2, got {letters.Count}");
if (letters.ContainsKey("B")) Fail("RemoveAt", "'B' should have been removed");

// <region Count>
#region Count
var items = new SortedList<int, string> { [1] = "one", [2] = "two", [3] = "three" };
int count = items.Count;   // 3
#endregion
// </region Count>

if (count != 3) Fail("Count", $"Expected 3, got {count}");

// <region Capacity>
#region Capacity
var list = new SortedList<int, string>(capacity: 100);
list.Add(1, "one");
int cap = list.Capacity;   // At least 100.
#endregion
// </region Capacity>

if (cap < 100) Fail("Capacity", $"Expected >= 100, got {cap}");

// <region KeysValues>
#region KeysValues
var fruit = new SortedList<string, int>
{
    ["Cherry"] = 3,
    ["Apple"] = 1,
    ["Banana"] = 2
};
// Keys and Values are IList<T> — support index access.
string firstKey = fruit.Keys[0];      // "Apple" (sorted)
int firstValue = fruit.Values[0];     // 1
#endregion
// </region KeysValues>

if (firstKey != "Apple") Fail("KeysValues", $"Expected 'Apple', got '{firstKey}'");
if (firstValue != 1) Fail("KeysValues", $"Expected 1, got {firstValue}");

// <region IndexOfKey>
#region IndexOfKey
var map = new SortedList<string, int> { ["A"] = 1, ["B"] = 2, ["C"] = 3 };
int idxB = map.IndexOfKey("B");      // 1
int idxMissing = map.IndexOfKey("Z"); // -1 — not found
#endregion
// </region IndexOfKey>

if (idxB != 1) Fail("IndexOfKey", $"Expected 1, got {idxB}");
if (idxMissing != -1) Fail("IndexOfKey", $"Expected -1, got {idxMissing}");

// <region IndexOfValue>
#region IndexOfValue
var data = new SortedList<string, int> { ["X"] = 10, ["Y"] = 20, ["Z"] = 10 };
int idxFirst = data.IndexOfValue(10);    // 0 — first occurrence
int idxNone = data.IndexOfValue(99);     // -1 — not found
#endregion
// </region IndexOfValue>

if (idxFirst != 0) Fail("IndexOfValue", $"Expected 0, got {idxFirst}");
if (idxNone != -1) Fail("IndexOfValue", $"Expected -1, got {idxNone}");

// <region GetKeyAtIndex>
#region GetKeyAtIndex
var sorted = new SortedList<string, int> { ["Banana"] = 2, ["Apple"] = 1, ["Cherry"] = 3 };
string keyAt0 = sorted.GetKeyAtIndex(0);     // "Apple"
string keyAt2 = sorted.GetKeyAtIndex(2);     // "Cherry"
#endregion
// </region GetKeyAtIndex>

if (keyAt0 != "Apple") Fail("GetKeyAtIndex", $"Expected 'Apple', got '{keyAt0}'");
if (keyAt2 != "Cherry") Fail("GetKeyAtIndex", $"Expected 'Cherry', got '{keyAt2}'");

// <region GetValueAtIndex>
#region GetValueAtIndex
var nums = new SortedList<string, int> { ["A"] = 10, ["B"] = 20, ["C"] = 30 };
int valAt1 = nums.GetValueAtIndex(1);   // 20
#endregion
// </region GetValueAtIndex>

if (valAt1 != 20) Fail("GetValueAtIndex", $"Expected 20, got {valAt1}");

// <region SetValueAtIndex>
#region SetValueAtIndex
var settings = new SortedList<string, string> { ["Theme"] = "Light", ["Language"] = "EN" };
settings.SetValueAtIndex(0, "FR");   // Update value at index 0 ("Language").
string updated = settings.GetValueAtIndex(0);
#endregion
// </region SetValueAtIndex>

if (updated != "FR") Fail("SetValueAtIndex", $"Expected 'FR', got '{updated}'");

// <region TrimExcess>
#region TrimExcess
var big = new SortedList<int, string>(capacity: 1000);
big.Add(1, "one");
big.Add(2, "two");
big.TrimExcess();
// Capacity is now reduced closer to Count.
#endregion
// </region TrimExcess>

if (big.Capacity > 100) Fail("TrimExcess", $"Capacity still large: {big.Capacity}");

// <region Clear>
#region Clear
var temp = new SortedList<int, string> { [1] = "a", [2] = "b" };
temp.Clear();
// Count is now 0.
#endregion
// </region Clear>

if (temp.Count != 0) Fail("Clear", $"Expected 0, got {temp.Count}");

if (failures > 0) { Console.Error.WriteLine($"\n{failures} example(s) failed."); return 1; }
Console.WriteLine("\nAll examples passed.");
return 0;

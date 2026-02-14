// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// Runnable examples for SortedDictionary<TKey,TValue> XML doc comments.
// Each #region is referenced by <code> tags in SortedDictionary.cs.
// Run: dotnet run SortedDictionary.Examples.cs

using System.Collections.Generic;

int failures = 0;
void Fail(string name, string message) { Console.Error.WriteLine($"FAIL: {name} — {message}"); failures++; }

// <region Add>
#region Add
var planets = new SortedDictionary<string, int>();
planets.Add("Mercury", 1);
planets.Add("Venus", 2);
planets.Add("Earth", 3);
// Dictionary now contains three entries sorted by key.
#endregion
// </region Add>

if (planets.Count != 3) Fail("Add", $"Expected 3, got {planets.Count}");
if (!planets.ContainsKey("Earth")) Fail("Add", "Missing key 'Earth'");

// <region IndexerGetSet>
#region IndexerGetSet
var scores = new SortedDictionary<string, int>
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
var capitals = new SortedDictionary<string, string>
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
var inventory = new SortedDictionary<string, int> { ["Apples"] = 5, ["Bananas"] = 3 };
bool hasApples = inventory.ContainsKey("Apples");   // true
bool hasGrapes = inventory.ContainsKey("Grapes");   // false
#endregion
// </region ContainsKey>

if (!hasApples) Fail("ContainsKey", "Expected true for Apples");
if (hasGrapes) Fail("ContainsKey", "Expected false for Grapes");

// <region ContainsValue>
#region ContainsValue
var ages = new SortedDictionary<string, int> { ["Alice"] = 30, ["Bob"] = 25 };
bool has30 = ages.ContainsValue(30);   // true — linear scan
bool has99 = ages.ContainsValue(99);   // false
#endregion
// </region ContainsValue>

if (!has30) Fail("ContainsValue", "Expected true for 30");
if (has99) Fail("ContainsValue", "Expected false for 99");

// <region Remove>
#region Remove
var colors = new SortedDictionary<string, string>
{
    ["R"] = "Red",
    ["G"] = "Green",
    ["B"] = "Blue"
};
bool removed = colors.Remove("G");     // true
bool notFound = colors.Remove("X");    // false — key absent
#endregion
// </region Remove>

if (!removed) Fail("Remove", "Expected true for 'G'");
if (notFound) Fail("Remove", "Expected false for 'X'");
if (colors.Count != 2) Fail("Remove", $"Expected 2, got {colors.Count}");

// <region Count>
#region Count
var items = new SortedDictionary<int, string> { [1] = "one", [2] = "two", [3] = "three" };
int count = items.Count;   // 3
#endregion
// </region Count>

if (count != 3) Fail("Count", $"Expected 3, got {count}");

// <region Keys>
#region Keys
var fruit = new SortedDictionary<string, int>
{
    ["Cherry"] = 3,
    ["Apple"] = 1,
    ["Banana"] = 2
};
// Keys are enumerated in sorted order: Apple, Banana, Cherry.
List<string> sortedKeys = [.. fruit.Keys];
#endregion
// </region Keys>

if (sortedKeys is not ["Apple", "Banana", "Cherry"])
    Fail("Keys", $"Unexpected order: {string.Join(", ", sortedKeys)}");

// <region Values>
#region Values
var map = new SortedDictionary<string, int>
{
    ["C"] = 3,
    ["A"] = 1,
    ["B"] = 2
};
// Values follow the sorted key order: A=1, B=2, C=3.
List<int> vals = [.. map.Values];
#endregion
// </region Values>

if (vals is not [1, 2, 3])
    Fail("Values", $"Unexpected values: {string.Join(", ", vals)}");

// <region Clear>
#region Clear
var data = new SortedDictionary<int, string> { [1] = "a", [2] = "b" };
data.Clear();
// Count is now 0.
#endregion
// </region Clear>

if (data.Count != 0) Fail("Clear", $"Expected 0, got {data.Count}");

// <region CustomComparer>
#region CustomComparer
// Case-insensitive key comparison.
var dict = new SortedDictionary<string, int>(StringComparer.OrdinalIgnoreCase)
{
    ["alpha"] = 1,
    ["BETA"] = 2
};
bool found2 = dict.TryGetValue("Alpha", out int val);   // true — case ignored
#endregion
// </region CustomComparer>

if (!found2 || val != 1) Fail("CustomComparer", "Case-insensitive lookup failed");

// <region Enumeration>
#region Enumeration
var sortedMap = new SortedDictionary<string, int>
{
    ["Zebra"] = 26,
    ["Apple"] = 1,
    ["Mango"] = 13
};
// Enumerating yields KeyValuePairs in sorted key order.
List<string> order = [];
foreach (var kvp in sortedMap)
    order.Add(kvp.Key);
#endregion
// </region Enumeration>

if (order is not ["Apple", "Mango", "Zebra"])
    Fail("Enumeration", $"Unexpected order: {string.Join(", ", order)}");

if (failures > 0) { Console.Error.WriteLine($"\n{failures} example(s) failed."); return 1; }
Console.WriteLine("\nAll examples passed.");
return 0;

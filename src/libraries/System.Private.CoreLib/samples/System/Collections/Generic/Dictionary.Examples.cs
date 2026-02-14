// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// Runnable examples for Dictionary<TKey, TValue> XML doc comments.
// Each #region is referenced by <code> tags in Dictionary.cs.
// Run: dotnet run Dictionary.Examples.cs

using System.Collections.Generic;

int failures = 0;
void Fail(string name, string message) { Console.Error.WriteLine($"FAIL: {name} — {message}"); failures++; }

// ————————————————————————————————————————
// Constructors
// ————————————————————————————————————————

#region CtorDefault
// Create an empty dictionary with default capacity.
var ages = new Dictionary<string, int>();
ages["Alice"] = 30;
ages["Bob"] = 25;
#endregion CtorDefault

if (ages.Count != 2) Fail("CtorDefault", $"Expected 2, got {ages.Count}");

#region CtorCapacity
// Pre-size to avoid rehashing when the number of entries is known.
var scores = new Dictionary<string, int>(capacity: 100);
scores["Math"] = 95;
scores["Science"] = 88;
#endregion CtorCapacity

if (scores.Count != 2) Fail("CtorCapacity", $"Expected 2, got {scores.Count}");

#region CtorComparer
// Use a case-insensitive comparer for string keys.
var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
{
    ["Content-Type"] = "text/html",
    ["Accept"] = "application/json"
};

// "content-type" matches "Content-Type" because of the comparer.
var contentType = headers["content-type"];
#endregion CtorComparer

if (contentType != "text/html") Fail("CtorComparer", $"Expected 'text/html', got '{contentType}'");

// ————————————————————————————————————————
// Properties
// ————————————————————————————————————————

#region Count
var inventory = new Dictionary<string, int>
{
    ["Apples"] = 5,
    ["Bananas"] = 3,
    ["Cherries"] = 12
};

Console.WriteLine($"Inventory has {inventory.Count} items.");
// Output: Inventory has 3 items.
#endregion Count

if (inventory.Count != 3) Fail("Count", $"Expected 3, got {inventory.Count}");

#region Keys
var capitals = new Dictionary<string, string>
{
    ["France"] = "Paris",
    ["Japan"] = "Tokyo",
    ["Brazil"] = "Brasília"
};

// Copy all keys into a list.
List<string> countries = [.. capitals.Keys];
Console.WriteLine(string.Join(", ", countries));
#endregion Keys

if (countries.Count != 3) Fail("Keys", $"Expected 3 keys, got {countries.Count}");

#region Values
// Extract all values from the dictionary.
List<string> cities = [.. capitals.Values];
Console.WriteLine(string.Join(", ", cities));
#endregion Values

if (cities.Count != 3) Fail("Values", $"Expected 3 values, got {cities.Count}");

#region Comparer
var caseInsensitive = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
Console.WriteLine(caseInsensitive.Comparer == StringComparer.OrdinalIgnoreCase);
// Output: True
#endregion Comparer

if (caseInsensitive.Comparer != StringComparer.OrdinalIgnoreCase)
    Fail("Comparer", "Expected OrdinalIgnoreCase comparer");

// ————————————————————————————————————————
// Add / Set
// ————————————————————————————————————————

#region Add
var students = new Dictionary<int, string>();
students.Add(1, "Alice");
students.Add(2, "Bob");

// Adding a duplicate key throws ArgumentException.
try
{
    students.Add(1, "Charlie");
}
catch (ArgumentException)
{
    Console.WriteLine("Key 1 already exists.");
}
#endregion Add

if (students[1] != "Alice") Fail("Add", $"Expected 'Alice', got '{students[1]}'");

#region TryAdd
var settings = new Dictionary<string, string>();
bool added = settings.TryAdd("Theme", "Dark");
bool duplicate = settings.TryAdd("Theme", "Light");

Console.WriteLine($"First add: {added}, Second add: {duplicate}");
// Output: First add: True, Second add: False
#endregion TryAdd

if (!added) Fail("TryAdd", "First TryAdd should return true");
if (duplicate) Fail("TryAdd", "Second TryAdd should return false");
if (settings["Theme"] != "Dark") Fail("TryAdd", "Value should remain 'Dark'");

#region IndexerSet
var colorHex = new Dictionary<string, string>();

// The indexer adds a new entry or overwrites an existing one.
colorHex["Red"] = "#FF0000";
colorHex["Red"] = "#CC0000"; // overwrites
#endregion IndexerSet

if (colorHex["Red"] != "#CC0000") Fail("IndexerSet", $"Expected '#CC0000', got '{colorHex["Red"]}'");

#region IndexerGet
var planets = new Dictionary<int, string>
{
    [3] = "Earth",
    [4] = "Mars"
};

// Retrieve a value by key.
string earth = planets[3];

// Accessing a missing key throws KeyNotFoundException.
try
{
    _ = planets[9];
}
catch (KeyNotFoundException)
{
    Console.WriteLine("Key 9 not found.");
}
#endregion IndexerGet

if (earth != "Earth") Fail("IndexerGet", $"Expected 'Earth', got '{earth}'");

// ————————————————————————————————————————
// Remove
// ————————————————————————————————————————

#region Remove
var fruits = new Dictionary<string, int>
{
    ["Apple"] = 1,
    ["Banana"] = 2,
    ["Cherry"] = 3
};

bool removed = fruits.Remove("Banana");
bool notFound = fruits.Remove("Grape");
Console.WriteLine($"Removed Banana: {removed}, Removed Grape: {notFound}");
// Output: Removed Banana: True, Removed Grape: False
#endregion Remove

if (!removed) Fail("Remove", "Remove('Banana') should return true");
if (notFound) Fail("Remove", "Remove('Grape') should return false");
if (fruits.Count != 2) Fail("Remove", $"Expected 2, got {fruits.Count}");

#region Clear
var cache = new Dictionary<string, byte[]>
{
    ["img1"] = new byte[100],
    ["img2"] = new byte[200]
};

cache.Clear();
Console.WriteLine($"Cache count after Clear: {cache.Count}");
// Output: Cache count after Clear: 0
#endregion Clear

if (cache.Count != 0) Fail("Clear", $"Expected 0, got {cache.Count}");

// ————————————————————————————————————————
// Lookup
// ————————————————————————————————————————

#region ContainsKey
var permissions = new Dictionary<string, bool>
{
    ["Admin"] = true,
    ["Editor"] = true
};

if (permissions.ContainsKey("Admin"))
{
    Console.WriteLine("Admin permission exists.");
}
#endregion ContainsKey

if (!permissions.ContainsKey("Admin")) Fail("ContainsKey", "Expected 'Admin' to exist");

#region ContainsValue
var elements = new Dictionary<string, string>
{
    ["H"] = "Hydrogen",
    ["O"] = "Oxygen",
    ["C"] = "Carbon"
};

bool hasOxygen = elements.ContainsValue("Oxygen");
Console.WriteLine($"Contains Oxygen: {hasOxygen}");
// Output: Contains Oxygen: True
#endregion ContainsValue

if (!hasOxygen) Fail("ContainsValue", "Expected to find 'Oxygen'");

#region TryGetValue
var phonebook = new Dictionary<string, string>
{
    ["Alice"] = "555-1234",
    ["Bob"] = "555-5678"
};

// TryGetValue avoids a double lookup compared to ContainsKey + indexer.
if (phonebook.TryGetValue("Alice", out var phone))
{
    Console.WriteLine($"Alice's number: {phone}");
}
#endregion TryGetValue

if (phone != "555-1234") Fail("TryGetValue", $"Expected '555-1234', got '{phone}'");

// ————————————————————————————————————————
// Capacity
// ————————————————————————————————————————

#region EnsureCapacity
var buffer = new Dictionary<string, int>();

// Pre-allocate for at least 1000 entries to avoid rehashing.
int actualCapacity = buffer.EnsureCapacity(1000);
Console.WriteLine($"Capacity is at least: {actualCapacity}");
#endregion EnsureCapacity

if (actualCapacity < 1000) Fail("EnsureCapacity", $"Expected >= 1000, got {actualCapacity}");

#region TrimExcess
var temp = new Dictionary<int, string>(capacity: 1000);
temp[1] = "One";
temp[2] = "Two";

// Reclaim unused memory after bulk removals or when final size is known.
temp.TrimExcess();
#endregion TrimExcess

if (temp.Count != 2) Fail("TrimExcess", $"Expected 2, got {temp.Count}");

// ————————————————————————————————————————
// Enumeration
// ————————————————————————————————————————

#region Enumeration
var rgb = new Dictionary<string, (int R, int G, int B)>
{
    ["Red"] = (255, 0, 0),
    ["Green"] = (0, 128, 0),
    ["Blue"] = (0, 0, 255)
};

// Enumerate key-value pairs with deconstruction.
foreach (var (name, (r, g, b)) in rgb)
{
    Console.WriteLine($"{name}: ({r}, {g}, {b})");
}
#endregion Enumeration

if (rgb.Count != 3) Fail("Enumeration", $"Expected 3, got {rgb.Count}");

// ————————————————————————————————————————
// Result
// ————————————————————————————————————————

if (failures > 0) { Console.Error.WriteLine($"\n{failures} example(s) failed."); return 1; }
Console.WriteLine("\nAll examples passed.");
return 0;

using System;
using System.Collections.Generic;

AddAndAccess();
RemoveItems();
BulkOperations();
CapacityOperations();
TryOperations();
Console.WriteLine("All examples passed.");
return 0;

void AddAndAccess()
{
    // Create a dictionary using collection initializer syntax.
    Dictionary<string, int> ages = new()
    {
        ["Alice"] = 30,
        ["Bob"] = 25
    };

    // Add inserts a new key/value pair.
    ages.Add("Charlie", 35);
    Console.WriteLine($"Count: {ages.Count}");
    // Output: Count: 3

    // The indexer retrieves values by key.
    Console.WriteLine($"Bob's age: {ages["Bob"]}");
    // Output: Bob's age: 25

    // TryGetValue safely retrieves a value without throwing.
    if (ages.TryGetValue("Alice", out int aliceAge))
    {
        Console.WriteLine($"Alice's age: {aliceAge}");
        // Output: Alice's age: 30
    }

    // ContainsKey checks for a key.
    Console.WriteLine($"Has 'Charlie': {ages.ContainsKey("Charlie")}");
    // Output: Has 'Charlie': True

    // ContainsValue checks for a value.
    Console.WriteLine($"Has age 25: {ages.ContainsValue(25)}");
    // Output: Has age 25: True

    // The indexer can also update an existing entry.
    ages["Bob"] = 26;
    Console.WriteLine($"Bob's updated age: {ages["Bob"]}");
    // Output: Bob's updated age: 26
}

void RemoveItems()
{
    Dictionary<string, double> scores = new()
    {
        ["Math"] = 95.0,
        ["Science"] = 88.5,
        ["History"] = 72.0
    };

    // Remove deletes an entry by key and returns true if found.
    bool removed = scores.Remove("History");
    Console.WriteLine($"Removed 'History': {removed}");
    // Output: Removed 'History': True
    Console.WriteLine($"Count after Remove: {scores.Count}");
    // Output: Count after Remove: 2

    // Remove overload also returns the removed value.
    if (scores.Remove("Math", out double mathScore))
    {
        Console.WriteLine($"Removed 'Math' with score: {mathScore}");
        // Output: Removed 'Math' with score: 95
    }

    // Clear removes all entries.
    scores.Clear();
    Console.WriteLine($"Count after Clear: {scores.Count}");
    // Output: Count after Clear: 0
}

void BulkOperations()
{
    Dictionary<string, string> capitals = new()
    {
        ["France"] = "Paris",
        ["Japan"] = "Tokyo",
        ["Brazil"] = "Brasília"
    };

    // Keys returns all keys in the dictionary.
    Console.WriteLine($"Keys: {string.Join(", ", capitals.Keys)}");

    // Values returns all values in the dictionary.
    Console.WriteLine($"Values: {string.Join(", ", capitals.Values)}");

    // Enumerate key/value pairs with foreach (uses GetEnumerator).
    foreach (KeyValuePair<string, string> kvp in capitals)
    {
        Console.WriteLine($"  {kvp.Key} -> {kvp.Value}");
    }

    Console.WriteLine($"Count: {capitals.Count}");
    // Output: Count: 3
}

void CapacityOperations()
{
    Dictionary<int, string> dict = new();

    // EnsureCapacity pre-allocates internal storage.
    int capacity = dict.EnsureCapacity(100);
    Console.WriteLine($"EnsureCapacity(100) returned: {capacity}");

    for (int i = 0; i < 10; i++)
    {
        dict.Add(i, $"item-{i}");
    }
    Console.WriteLine($"Count: {dict.Count}, Capacity: {dict.Capacity}");

    // TrimExcess(int) trims capacity to hold a specified number of entries.
    dict.TrimExcess(20);
    Console.WriteLine($"Capacity after TrimExcess(20): {dict.Capacity}");

    // TrimExcess without arguments trims capacity to match the current count.
    dict.TrimExcess();
    Console.WriteLine($"Capacity after TrimExcess: {dict.Capacity}");
}

void TryOperations()
{
    Dictionary<string, int> inventory = new()
    {
        ["Apples"] = 5,
        ["Bananas"] = 12
    };

    // TryAdd adds only if the key does not already exist.
    bool added = inventory.TryAdd("Cherries", 8);
    Console.WriteLine($"TryAdd 'Cherries': {added}");
    // Output: TryAdd 'Cherries': True

    bool duplicate = inventory.TryAdd("Apples", 99);
    Console.WriteLine($"TryAdd 'Apples' (duplicate): {duplicate}");
    // Output: TryAdd 'Apples' (duplicate): False
    Console.WriteLine($"Apples count unchanged: {inventory["Apples"]}");
    // Output: Apples count unchanged: 5

    // TryGetValue returns false for missing keys.
    bool found = inventory.TryGetValue("Grapes", out int grapes);
    Console.WriteLine($"TryGetValue 'Grapes': found={found}, value={grapes}");
    // Output: TryGetValue 'Grapes': found=False, value=0
}

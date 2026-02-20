using System;
using System.Collections;
using System.Collections.Generic;

AddAndAccess();
RemoveAndClear();
IterateKeysAndValues();
CopyToArray();
Constructors();

void AddAndAccess()
{
    var scores = new SortedDictionary<string, int>();
    scores.Add("Alice", 90);
    scores.Add("Bob", 85);
    scores["Charlie"] = 78;

    Console.WriteLine($"Count: {scores.Count}");
    Console.WriteLine($"Alice's score: {scores["Alice"]}");

    if (scores.TryGetValue("Bob", out int bobScore))
        Console.WriteLine($"Bob's score: {bobScore}");

    Console.WriteLine($"ContainsKey(\"Alice\"): {scores.ContainsKey("Alice")}");
    Console.WriteLine($"ContainsValue(78): {scores.ContainsValue(78)}");

    // Update an existing entry via the indexer
    scores["Alice"] = 95;
    Console.WriteLine($"Alice's updated score: {scores["Alice"]}");
}

void RemoveAndClear()
{
    var colors = new SortedDictionary<int, string>
    {
        [1] = "Red",
        [2] = "Green",
        [3] = "Blue"
    };

    bool removed = colors.Remove(2);
    Console.WriteLine($"Removed key 2: {removed}");
    Console.WriteLine($"Count after Remove: {colors.Count}");

    colors.Clear();
    Console.WriteLine($"Count after Clear: {colors.Count}");
}

void IterateKeysAndValues()
{
    var capitals = new SortedDictionary<string, string>
    {
        ["France"] = "Paris",
        ["Japan"] = "Tokyo",
        ["Brazil"] = "Brasília"
    };

    Console.WriteLine("Keys:");
    foreach (string key in capitals.Keys)
        Console.WriteLine($"  {key}");

    Console.WriteLine("Values:");
    foreach (string value in capitals.Values)
        Console.WriteLine($"  {value}");

    Console.WriteLine("Enumerating key-value pairs:");
    foreach (KeyValuePair<string, string> kvp in capitals)
        Console.WriteLine($"  {kvp.Key}: {kvp.Value}");
}

void CopyToArray()
{
    var dict = new SortedDictionary<string, int>
    {
        ["one"] = 1,
        ["two"] = 2,
        ["three"] = 3
    };

    var array = new KeyValuePair<string, int>[dict.Count];
    dict.CopyTo(array, 0);

    foreach (KeyValuePair<string, int> kvp in array)
        Console.WriteLine($"  {kvp.Key} = {kvp.Value}");
}

void Constructors()
{
    // Default constructor
    var dict1 = new SortedDictionary<string, int>();
    dict1.Add("cherry", 3);
    dict1.Add("apple", 1);
    Console.WriteLine($"Default comparer order:");
    foreach (var kvp in dict1)
        Console.WriteLine($"  {kvp.Key}: {kvp.Value}");

    // Constructor with a custom comparer (case-insensitive)
    var dict2 = new SortedDictionary<string, int>(StringComparer.OrdinalIgnoreCase);
    dict2.Add("Banana", 2);
    dict2.Add("apple", 1);
    Console.WriteLine($"Comparer: {dict2.Comparer.GetType().Name}");
    foreach (var kvp in dict2)
        Console.WriteLine($"  {kvp.Key}: {kvp.Value}");

    // Constructor from an existing dictionary
    var source = new Dictionary<string, int> { ["x"] = 10, ["a"] = 20 };
    var dict3 = new SortedDictionary<string, int>(source);
    Console.WriteLine("Constructed from Dictionary:");
    foreach (var kvp in dict3)
        Console.WriteLine($"  {kvp.Key}: {kvp.Value}");

    // Constructor from an existing dictionary with a comparer
    var dict4 = new SortedDictionary<string, int>(source, StringComparer.OrdinalIgnoreCase);
    Console.WriteLine("Constructed from Dictionary with comparer:");
    foreach (var kvp in dict4)
        Console.WriteLine($"  {kvp.Key}: {kvp.Value}");
}

using System;
using System.Collections.Generic;

AddAndAccess();
RemoveAndClear();
IterateKeysAndValues();
IndexBasedAccess();
CapacityAndTrimExcess();
ConstructWithComparer();

void AddAndAccess()
{
    SortedList<string, int> list = new SortedList<string, int>();

    list.Add("apple", 3);
    list.Add("banana", 2);
    list.Add("cherry", 5);

    // Elements are maintained in sorted key order.
    Console.WriteLine($"Count: {list.Count}"); // Count: 3

    // Access by key using the indexer.
    Console.WriteLine($"banana: {list["banana"]}"); // banana: 2

    // The indexer can also add or update entries.
    list["date"] = 7;
    list["banana"] = 10;
    Console.WriteLine($"banana updated: {list["banana"]}"); // banana updated: 10

    Console.WriteLine($"ContainsKey(\"cherry\"): {list.ContainsKey("cherry")}"); // ContainsKey("cherry"): True
    Console.WriteLine($"ContainsValue(5): {list.ContainsValue(5)}");             // ContainsValue(5): True

    // TryGetValue avoids exceptions for missing keys.
    if (list.TryGetValue("apple", out int value))
    {
        Console.WriteLine($"apple: {value}"); // apple: 3
    }

    Console.WriteLine($"TryGetValue(\"fig\"): {list.TryGetValue("fig", out _)}"); // TryGetValue("fig"): False
}

void RemoveAndClear()
{
    SortedList<int, string> list = new SortedList<int, string>
    {
        { 1, "one" },
        { 2, "two" },
        { 3, "three" },
        { 4, "four" },
        { 5, "five" }
    };

    // Remove by key returns true if the key was found.
    Console.WriteLine($"Remove(3): {list.Remove(3)}");   // Remove(3): True
    Console.WriteLine($"Remove(99): {list.Remove(99)}"); // Remove(99): False

    // RemoveAt removes by sorted index.
    Console.WriteLine($"Before RemoveAt(0): key={list.Keys[0]}"); // Before RemoveAt(0): key=1
    list.RemoveAt(0);
    Console.WriteLine($"After RemoveAt(0): key={list.Keys[0]}");  // After RemoveAt(0): key=2

    Console.WriteLine($"Count before Clear: {list.Count}"); // Count before Clear: 3
    list.Clear();
    Console.WriteLine($"Count after Clear: {list.Count}");  // Count after Clear: 0
}

void IterateKeysAndValues()
{
    SortedList<string, double> list = new SortedList<string, double>
    {
        { "alpha", 1.1 },
        { "beta", 2.2 },
        { "gamma", 3.3 }
    };

    // Iterate keys.
    Console.Write("Keys: ");
    foreach (string key in list.Keys)
    {
        Console.Write($"{key} ");
    }
    Console.WriteLine(); // Keys: alpha beta gamma

    // Iterate values.
    Console.Write("Values: ");
    foreach (double val in list.Values)
    {
        Console.Write($"{val} ");
    }
    Console.WriteLine(); // Values: 1.1 2.2 3.3

    // Iterate key-value pairs using the enumerator (foreach).
    foreach (KeyValuePair<string, double> kvp in list)
    {
        Console.WriteLine($"  {kvp.Key} = {kvp.Value}");
    }
}

void IndexBasedAccess()
{
    SortedList<string, int> list = new SortedList<string, int>
    {
        { "cat", 4 },
        { "ant", 6 },
        { "dog", 2 },
        { "bat", 8 }
    };

    // IndexOfKey returns the zero-based sorted index, or -1 if not found.
    Console.WriteLine($"IndexOfKey(\"cat\"): {list.IndexOfKey("cat")}");   // IndexOfKey("cat"): 2
    Console.WriteLine($"IndexOfKey(\"fox\"): {list.IndexOfKey("fox")}");   // IndexOfKey("fox"): -1

    // IndexOfValue returns the index of the first occurrence, or -1.
    Console.WriteLine($"IndexOfValue(8): {list.IndexOfValue(8)}"); // IndexOfValue(8): 1

    // GetKeyAtIndex / GetValueAtIndex retrieve by sorted position.
    Console.WriteLine($"GetKeyAtIndex(0): {list.GetKeyAtIndex(0)}");     // GetKeyAtIndex(0): ant
    Console.WriteLine($"GetValueAtIndex(0): {list.GetValueAtIndex(0)}"); // GetValueAtIndex(0): 6

    // SetValueAtIndex updates the value at a sorted position.
    list.SetValueAtIndex(0, 100);
    Console.WriteLine($"After SetValueAtIndex(0, 100): {list.GetValueAtIndex(0)}"); // After SetValueAtIndex(0, 100): 100
}

void CapacityAndTrimExcess()
{
    // Construct with an initial capacity.
    SortedList<int, string> list = new SortedList<int, string>(100);
    Console.WriteLine($"Initial Capacity: {list.Capacity}"); // Initial Capacity: 100

    list.Add(1, "one");
    list.Add(2, "two");
    list.Add(3, "three");
    Console.WriteLine($"Count: {list.Count}");       // Count: 3
    Console.WriteLine($"Capacity: {list.Capacity}"); // Capacity: 100

    // TrimExcess reduces capacity to match the count.
    list.TrimExcess();
    Console.WriteLine($"Capacity after TrimExcess: {list.Capacity}"); // Capacity after TrimExcess: 3
}

void ConstructWithComparer()
{
    // Use a case-insensitive comparer for string keys.
    SortedList<string, int> list = new SortedList<string, int>(StringComparer.OrdinalIgnoreCase);

    list.Add("Apple", 1);
    list.Add("banana", 2);

    // "apple" matches "Apple" because the comparer ignores case.
    Console.WriteLine($"ContainsKey(\"apple\"): {list.ContainsKey("apple")}"); // ContainsKey("apple"): True
    Console.WriteLine($"Comparer: {list.Comparer}"); // Comparer: System.OrdinalIgnoreCaseComparer

    // Construct from an existing dictionary.
    Dictionary<string, int> source = new Dictionary<string, int>
    {
        { "x", 10 },
        { "y", 20 },
        { "z", 30 }
    };
    SortedList<string, int> fromDict = new SortedList<string, int>(source);
    Console.WriteLine($"fromDict Count: {fromDict.Count}"); // fromDict Count: 3
    Console.WriteLine($"First key: {fromDict.GetKeyAtIndex(0)}"); // First key: x
}

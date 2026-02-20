using System;
using System.Collections.Generic;

AddAndAccess();
IndexBasedAccess();
InsertAndRemove();
KeysValuesAndEnumerate();
TryAddAndTryGetValue();
CapacityManagement();
ConstructFromCollection();

Console.WriteLine("All examples passed.");
return 0;

void AddAndAccess()
{
    OrderedDictionary<string, int> dict = new();
    dict.Add("apple", 1);
    dict.Add("banana", 2);
    dict.Add("cherry", 3);

    Console.WriteLine($"dict[\"banana\"]: {dict["banana"]}");          // 2
    dict["banana"] = 20;
    Console.WriteLine($"dict[\"banana\"] after set: {dict["banana"]}"); // 20

    bool found = dict.TryGetValue("cherry", out int value);
    Console.WriteLine($"TryGetValue 'cherry': {found}, value: {value}"); // True, 3

    Console.WriteLine($"ContainsKey 'apple': {dict.ContainsKey("apple")}");   // True
    Console.WriteLine($"ContainsValue 20: {dict.ContainsValue(20)}");         // True
    Console.WriteLine($"Count: {dict.Count}");                                // 3
}

void IndexBasedAccess()
{
    OrderedDictionary<string, int> dict = new();
    dict.Add("alpha", 10);
    dict.Add("bravo", 20);
    dict.Add("charlie", 30);

    KeyValuePair<string, int> entry = dict.GetAt(1);
    Console.WriteLine($"GetAt(1): {entry.Key} = {entry.Value}"); // bravo = 20

    dict.SetAt(1, 25);
    Console.WriteLine($"After SetAt(1, 25): {dict.GetAt(1).Value}"); // 25

    dict.SetAt(1, "beta", 22);
    Console.WriteLine($"After SetAt(1, 'beta', 22): {dict.GetAt(1).Key} = {dict.GetAt(1).Value}"); // beta = 22

    int index = dict.IndexOf("charlie");
    Console.WriteLine($"IndexOf 'charlie': {index}"); // 2

    dict.RemoveAt(0);
    Console.WriteLine($"After RemoveAt(0), first key: {dict.GetAt(0).Key}"); // beta
}

void InsertAndRemove()
{
    OrderedDictionary<string, int> dict = new();
    dict.Add("one", 1);
    dict.Add("three", 3);

    dict.Insert(1, "two", 2);
    Console.WriteLine($"After Insert at 1: {dict.GetAt(1).Key} = {dict.GetAt(1).Value}"); // two = 2

    bool removed = dict.Remove("one");
    Console.WriteLine($"Remove 'one': {removed}, Count: {dict.Count}"); // True, 2

    bool removedWithValue = dict.Remove("two", out int removedValue);
    Console.WriteLine($"Remove 'two': {removedWithValue}, value: {removedValue}"); // True, 2

    dict.Clear();
    Console.WriteLine($"After Clear, Count: {dict.Count}"); // 0
}

void KeysValuesAndEnumerate()
{
    OrderedDictionary<string, int> dict = new();
    dict.Add("x", 10);
    dict.Add("y", 20);
    dict.Add("z", 30);

    Console.WriteLine($"Keys: [{string.Join(", ", dict.Keys)}]");     // x, y, z
    Console.WriteLine($"Values: [{string.Join(", ", dict.Values)}]"); // 10, 20, 30

    Console.Write("Enumerate: ");
    foreach (KeyValuePair<string, int> kvp in dict)
    {
        Console.Write($"{kvp.Key}={kvp.Value} ");
    }
    Console.WriteLine(); // x=10 y=20 z=30
}

void TryAddAndTryGetValue()
{
    OrderedDictionary<string, int> dict = new();
    bool added = dict.TryAdd("key1", 100);
    Console.WriteLine($"TryAdd 'key1': {added}"); // True

    bool addedAgain = dict.TryAdd("key1", 200);
    Console.WriteLine($"TryAdd 'key1' again: {addedAgain}"); // False

    bool addedWithIndex = dict.TryAdd("key2", 200, out int addIndex);
    Console.WriteLine($"TryAdd 'key2': {addedWithIndex}, index: {addIndex}"); // True, 1

    bool found = dict.TryGetValue("key2", out int val, out int idx);
    Console.WriteLine($"TryGetValue 'key2': {found}, value: {val}, index: {idx}"); // True, 200, 1
}

void CapacityManagement()
{
    OrderedDictionary<string, int> dict = new();
    int ensured = dict.EnsureCapacity(50);
    Console.WriteLine($"EnsureCapacity(50): {ensured}"); // >= 50

    for (int i = 0; i < 10; i++)
        dict.Add($"item{i}", i);

    Console.WriteLine($"Count: {dict.Count}, Capacity: {dict.Capacity}"); // 10, >= 50
    dict.TrimExcess();
    Console.WriteLine($"After TrimExcess, Capacity: {dict.Capacity}");

    dict.TrimExcess(20);
    Console.WriteLine($"After TrimExcess(20), Capacity: {dict.Capacity}");
    Console.WriteLine($"Comparer: {dict.Comparer}");
}

void ConstructFromCollection()
{
    Dictionary<string, int> source = new()
    {
        ["one"] = 1,
        ["two"] = 2,
        ["three"] = 3
    };
    OrderedDictionary<string, int> fromDict = new(source);
    Console.WriteLine($"From IDictionary, Count: {fromDict.Count}"); // 3

    List<KeyValuePair<string, int>> pairs = [new("a", 1), new("b", 2)];
    OrderedDictionary<string, int> fromEnum = new(pairs);
    Console.WriteLine($"From IEnumerable, Count: {fromEnum.Count}"); // 2

    OrderedDictionary<string, int> withComparer = new(StringComparer.OrdinalIgnoreCase);
    withComparer.Add("Hello", 1);
    Console.WriteLine($"ContainsKey 'hello': {withComparer.ContainsKey("hello")}"); // True

    OrderedDictionary<string, int> withCapacity = new(100);
    Console.WriteLine($"Capacity from ctor(100): {withCapacity.Capacity}"); // >= 100
}

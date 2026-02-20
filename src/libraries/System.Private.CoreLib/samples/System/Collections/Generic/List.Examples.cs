using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

AddAndInsert();
SearchItems();
FindItems();
RemoveItems();
ConvertAndCopy();
CapacityAndCount();
Iterate();
SortAndReverse();
Indexer();

Console.WriteLine("All examples passed.");
return 0;

void AddAndInsert()
{
    List<string> names = ["Alice", "Bob"];
    names.Add("Charlie");
    Console.WriteLine(string.Join(", ", names));
    // Output: Alice, Bob, Charlie

    names.AddRange(new[] { "Diana", "Eve" });
    Console.WriteLine(string.Join(", ", names));
    // Output: Alice, Bob, Charlie, Diana, Eve

    names.Insert(1, "Zara");
    Console.WriteLine(string.Join(", ", names));
    // Output: Alice, Zara, Bob, Charlie, Diana, Eve

    names.InsertRange(3, new[] { "Frank", "Grace" });
    Console.WriteLine(string.Join(", ", names));
    // Output: Alice, Zara, Bob, Frank, Grace, Charlie, Diana, Eve
}

void SearchItems()
{
    List<int> numbers = [1, 3, 5, 7, 9, 11, 13];

    // BinarySearch requires a sorted list
    int index = numbers.BinarySearch(7);
    Console.WriteLine($"BinarySearch(7): found at index {index}");
    // Output: BinarySearch(7): found at index 3

    Console.WriteLine($"Contains(5): {numbers.Contains(5)}");
    // Output: Contains(5): True

    Console.WriteLine($"IndexOf(9): {numbers.IndexOf(9)}");
    // Output: IndexOf(9): 4

    numbers.Add(3);
    // numbers is now [1, 3, 5, 7, 9, 11, 13, 3]
    Console.WriteLine($"LastIndexOf(3): {numbers.LastIndexOf(3)}");
    // Output: LastIndexOf(3): 7

    Console.WriteLine($"Exists(x => x > 10): {numbers.Exists(x => x > 10)}");
    // Output: Exists(x => x > 10): True

    Console.WriteLine($"TrueForAll(x => x > 0): {numbers.TrueForAll(x => x > 0)}");
    // Output: TrueForAll(x => x > 0): True
}

void FindItems()
{
    List<string> fruits = ["apple", "banana", "cherry", "date", "elderberry", "banana"];

    string? found = fruits.Find(f => f.StartsWith("ch"));
    Console.WriteLine($"Find starts with 'ch': {found}");
    // Output: Find starts with 'ch': cherry

    string? last = fruits.FindLast(f => f.Contains("an"));
    Console.WriteLine($"FindLast contains 'an': {last}");
    // Output: FindLast contains 'an': banana

    int idx = fruits.FindIndex(f => f.Length > 5);
    Console.WriteLine($"FindIndex length > 5: {idx}");
    // Output: FindIndex length > 5: 1

    int lastIdx = fruits.FindLastIndex(f => f.Length > 5);
    Console.WriteLine($"FindLastIndex length > 5: {lastIdx}");
    // Output: FindLastIndex length > 5: 5

    List<string> allLong = fruits.FindAll(f => f.Length > 5);
    Console.WriteLine($"FindAll length > 5: {string.Join(", ", allLong)}");
    // Output: FindAll length > 5: banana, cherry, elderberry, banana
}

void RemoveItems()
{
    List<int> numbers = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

    numbers.Remove(5);
    Console.WriteLine($"After Remove(5): {string.Join(", ", numbers)}");
    // Output: After Remove(5): 1, 2, 3, 4, 6, 7, 8, 9, 10

    numbers.RemoveAt(0);
    Console.WriteLine($"After RemoveAt(0): {string.Join(", ", numbers)}");
    // Output: After RemoveAt(0): 2, 3, 4, 6, 7, 8, 9, 10

    numbers.RemoveRange(4, 2);
    Console.WriteLine($"After RemoveRange(4, 2): {string.Join(", ", numbers)}");
    // Output: After RemoveRange(4, 2): 2, 3, 4, 6, 9, 10

    int removed = numbers.RemoveAll(x => x % 3 == 0);
    Console.WriteLine($"RemoveAll divisible by 3: removed {removed}, remaining: {string.Join(", ", numbers)}");
    // Output: RemoveAll divisible by 3: removed 3, remaining: 2, 4, 10

    numbers.Clear();
    Console.WriteLine($"After Clear: count = {numbers.Count}");
    // Output: After Clear: count = 0
}

void ConvertAndCopy()
{
    List<int> numbers = [1, 2, 3, 4, 5];

    List<string> strings = numbers.ConvertAll(n => n.ToString());
    Console.WriteLine($"ConvertAll: {string.Join(", ", strings)}");
    // Output: ConvertAll: 1, 2, 3, 4, 5

    int[] array = numbers.ToArray();
    Console.WriteLine($"ToArray: {string.Join(", ", array)}");
    // Output: ToArray: 1, 2, 3, 4, 5

    int[] dest = new int[5];
    numbers.CopyTo(dest);
    Console.WriteLine($"CopyTo: {string.Join(", ", dest)}");
    // Output: CopyTo: 1, 2, 3, 4, 5

    List<int> range = numbers.GetRange(1, 3);
    Console.WriteLine($"GetRange(1, 3): {string.Join(", ", range)}");
    // Output: GetRange(1, 3): 2, 3, 4

    List<int> slice = numbers.Slice(2, 2);
    Console.WriteLine($"Slice(2, 2): {string.Join(", ", slice)}");
    // Output: Slice(2, 2): 3, 4

    ReadOnlyCollection<int> readOnly = numbers.AsReadOnly();
    Console.WriteLine($"AsReadOnly: count = {readOnly.Count}, is read-only = true");
    // Output: AsReadOnly: count = 5, is read-only = true
}

void CapacityAndCount()
{
    List<int> numbers = new List<int>();
    Console.WriteLine($"Initial — Count: {numbers.Count}, Capacity: {numbers.Capacity}");
    // Output: Initial — Count: 0, Capacity: 0

    numbers.EnsureCapacity(20);
    Console.WriteLine($"After EnsureCapacity(20) — Capacity: {numbers.Capacity}");
    // Output: After EnsureCapacity(20) — Capacity: 20

    for (int i = 0; i < 5; i++)
        numbers.Add(i);

    Console.WriteLine($"After adding 5 items — Count: {numbers.Count}, Capacity: {numbers.Capacity}");
    // Output: After adding 5 items — Count: 5, Capacity: 20

    numbers.TrimExcess();
    Console.WriteLine($"After TrimExcess — Count: {numbers.Count}, Capacity: {numbers.Capacity}");
    // Output: After TrimExcess — Count: 5, Capacity: 5

    numbers.Capacity = 10;
    Console.WriteLine($"After setting Capacity = 10 — Capacity: {numbers.Capacity}");
    // Output: After setting Capacity = 10 — Capacity: 10
}

void Iterate()
{
    List<string> colors = ["red", "green", "blue"];

    // Using ForEach with an Action<T>
    Console.Write("ForEach: ");
    colors.ForEach(c => Console.Write($"{c} "));
    Console.WriteLine();
    // Output: ForEach: red green blue

    // Using GetEnumerator explicitly
    Console.Write("Enumerator: ");
    using (List<string>.Enumerator enumerator = colors.GetEnumerator())
    {
        while (enumerator.MoveNext())
            Console.Write($"{enumerator.Current} ");
    }
    Console.WriteLine();
    // Output: Enumerator: red green blue

    // Using foreach (the most common way)
    Console.Write("foreach: ");
    foreach (string color in colors)
        Console.Write($"{color} ");
    Console.WriteLine();
    // Output: foreach: red green blue
}

void SortAndReverse()
{
    List<string> names = ["Charlie", "Alice", "Eve", "Bob", "Diana"];

    names.Sort();
    Console.WriteLine($"Sort(): {string.Join(", ", names)}");
    // Output: Sort(): Alice, Bob, Charlie, Diana, Eve

    names.Sort((a, b) => b.Length.CompareTo(a.Length));
    Console.WriteLine($"Sort by length desc: {string.Join(", ", names)}");
    // Output: Sort by length desc: Charlie, Diana, Alice, Bob, Eve

    names.Reverse();
    Console.WriteLine($"Reverse(): {string.Join(", ", names)}");
    // Output: Reverse(): Eve, Bob, Alice, Diana, Charlie

    List<int> numbers = [5, 3, 1, 4, 2];
    numbers.Sort(1, 3, Comparer<int>.Default);
    Console.WriteLine($"Sort(1, 3, default): {string.Join(", ", numbers)}");
    // Output: Sort(1, 3, default): 5, 1, 3, 4, 2

    numbers.Reverse(0, 3);
    Console.WriteLine($"Reverse(0, 3): {string.Join(", ", numbers)}");
    // Output: Reverse(0, 3): 3, 1, 5, 4, 2
}

void Indexer()
{
    List<string> animals = ["cat", "dog", "bird"];

    Console.WriteLine($"animals[0]: {animals[0]}");
    // Output: animals[0]: cat

    animals[1] = "fish";
    Console.WriteLine($"After animals[1] = \"fish\": {string.Join(", ", animals)}");
    // Output: After animals[1] = "fish": cat, fish, bird
}

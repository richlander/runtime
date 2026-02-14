using System;
using System.Collections.Generic;

AddNodes();
RemoveNodes();
FindNodes();
NavigateNodes();
CopyToArray();

void AddNodes()
{
    var list = new LinkedList<string>();

    list.AddFirst("B");
    list.AddFirst("A");
    list.AddLast("D");

    LinkedListNode<string> nodeB = list.Find("B")!;
    list.AddAfter(nodeB, "B+");
    list.AddBefore(nodeB, "B-");

    foreach (string item in list)
        Console.Write(item + " ");
    Console.WriteLine();
    // Output: A B- B B+ D
}

void RemoveNodes()
{
    var list = new LinkedList<string>(new[] { "A", "B", "C", "D", "E" });

    list.RemoveFirst();
    list.RemoveLast();
    list.Remove("C");

    foreach (string item in list)
        Console.Write(item + " ");
    Console.WriteLine();
    // Output: B D

    list.Clear();
    Console.WriteLine($"Count after Clear: {list.Count}");
    // Output: Count after Clear: 0
}

void FindNodes()
{
    var list = new LinkedList<int>(new[] { 1, 2, 3, 2, 1 });

    LinkedListNode<int>? first2 = list.Find(2);
    LinkedListNode<int>? last2 = list.FindLast(2);

    Console.WriteLine($"Find(2) next: {first2!.Next!.Value}");
    // Output: Find(2) next: 3
    Console.WriteLine($"FindLast(2) next: {last2!.Next!.Value}");
    // Output: FindLast(2) next: 1
    Console.WriteLine($"Contains(3): {list.Contains(3)}");
    // Output: Contains(3): True
}

void NavigateNodes()
{
    var list = new LinkedList<string>(new[] { "A", "B", "C" });

    LinkedListNode<string>? node = list.First;
    Console.WriteLine($"First: {node!.Value}");
    // Output: First: A
    Console.WriteLine($"Last: {list.Last!.Value}");
    // Output: Last: C
    Console.WriteLine($"Count: {list.Count}");
    // Output: Count: 3

    Console.WriteLine($"First.Next: {node.Next!.Value}");
    // Output: First.Next: B
    Console.WriteLine($"Last.Previous: {list.Last!.Previous!.Value}");
    // Output: Last.Previous: B
    Console.WriteLine($"First.List == list: {node.List == list}");
    // Output: First.List == list: True

    ref string valueRef = ref node.ValueRef;
    valueRef = "Z";
    Console.WriteLine($"First after ValueRef mutation: {list.First!.Value}");
    // Output: First after ValueRef mutation: Z
}

void CopyToArray()
{
    var list = new LinkedList<int>(new[] { 10, 20, 30 });
    int[] array = new int[5];
    list.CopyTo(array, 1);

    Console.WriteLine(string.Join(", ", array));
    // Output: 0, 10, 20, 30, 0
}

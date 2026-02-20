using System;
using System.Collections.Generic;

PushAndPop();
PeekExample();
TryPeekAndTryPop();
ContainsExample();
CountAndClear();
CopyToAndToArray();
EnumerateStack();
CapacityAndTrimExcess();
ConstructFromCollection();
EnsureCapacityExample();

Console.WriteLine("All examples passed.");
return 0;

void PushAndPop()
{
    Stack<string> stack = new();
    stack.Push("first");
    stack.Push("second");
    stack.Push("third");
    Console.WriteLine($"Popped: {stack.Pop()}");   // third
    Console.WriteLine($"Popped: {stack.Pop()}");   // second
    Console.WriteLine($"Count after pops: {stack.Count}"); // 1
}

void PeekExample()
{
    Stack<int> stack = new();
    stack.Push(10);
    stack.Push(20);
    Console.WriteLine($"Peek: {stack.Peek()}");    // 20
    Console.WriteLine($"Count after peek: {stack.Count}"); // 2
}

void TryPeekAndTryPop()
{
    Stack<int> stack = new();
    bool hasPeek = stack.TryPeek(out int peekResult);
    Console.WriteLine($"TryPeek on empty: {hasPeek}"); // False

    stack.Push(42);
    bool hasPopped = stack.TryPop(out int popResult);
    Console.WriteLine($"TryPop: {hasPopped}, value: {popResult}"); // True, 42
    Console.WriteLine($"Count after TryPop: {stack.Count}"); // 0
}

void ContainsExample()
{
    Stack<string> stack = new();
    stack.Push("apple");
    stack.Push("banana");
    Console.WriteLine($"Contains 'apple': {stack.Contains("apple")}");   // True
    Console.WriteLine($"Contains 'cherry': {stack.Contains("cherry")}"); // False
}

void CountAndClear()
{
    Stack<int> stack = new();
    stack.Push(1);
    stack.Push(2);
    stack.Push(3);
    Console.WriteLine($"Count: {stack.Count}"); // 3
    stack.Clear();
    Console.WriteLine($"Count after Clear: {stack.Count}"); // 0
}

void CopyToAndToArray()
{
    Stack<string> stack = new();
    stack.Push("a");
    stack.Push("b");
    stack.Push("c");

    string[] array = stack.ToArray();
    Console.WriteLine($"ToArray: [{string.Join(", ", array)}]"); // c, b, a

    string[] dest = new string[5];
    stack.CopyTo(dest, 1);
    Console.WriteLine($"CopyTo at index 1: [{string.Join(", ", dest)}]");
}

void EnumerateStack()
{
    Stack<int> stack = new();
    stack.Push(10);
    stack.Push(20);
    stack.Push(30);

    Console.Write("Enumerate: ");
    foreach (int item in stack)
    {
        Console.Write($"{item} ");
    }
    Console.WriteLine(); // 30 20 10
}

void CapacityAndTrimExcess()
{
    Stack<int> stack = new();
    for (int i = 0; i < 100; i++)
        stack.Push(i);
    for (int i = 0; i < 90; i++)
        stack.Pop();

    Console.WriteLine($"Count: {stack.Count}, Capacity before trim: {stack.Capacity}");
    stack.TrimExcess();
    Console.WriteLine($"Capacity after TrimExcess: {stack.Capacity}");
}

void ConstructFromCollection()
{
    List<string> list = new() { "one", "two", "three" };
    Stack<string> stack = new(list);
    Console.WriteLine($"Top of stack: {stack.Peek()}"); // three
    Console.WriteLine($"Count: {stack.Count}");         // 3
}

void EnsureCapacityExample()
{
    Stack<int> stack = new();
    int newCapacity = stack.EnsureCapacity(50);
    Console.WriteLine($"Capacity after EnsureCapacity(50): {newCapacity}");
    stack.TrimExcess(20);
    Console.WriteLine($"Capacity after TrimExcess(20): {stack.Capacity}");
}

using System;
using System.Collections.Generic;

EnqueueAndDequeue();
BulkOperations();
Capacity();
Console.WriteLine("All examples passed.");
return 0;

void EnqueueAndDequeue()
{
    Queue<string> queue = new();

    // Enqueue adds items to the back of the queue.
    queue.Enqueue("apple");
    queue.Enqueue("banana");
    queue.Enqueue("cherry");
    Console.WriteLine($"Count after enqueuing 3 items: {queue.Count}");
    // Output: Count after enqueuing 3 items: 3

    // Peek returns the front item without removing it.
    Console.WriteLine($"Peek: {queue.Peek()}");
    // Output: Peek: apple

    // Dequeue removes and returns the front item.
    Console.WriteLine($"Dequeue: {queue.Dequeue()}");
    // Output: Dequeue: apple
    Console.WriteLine($"Count after dequeue: {queue.Count}");
    // Output: Count after dequeue: 2

    // TryDequeue attempts to remove the front item safely.
    if (queue.TryDequeue(out string? result))
    {
        Console.WriteLine($"TryDequeue: {result}");
        // Output: TryDequeue: banana
    }

    // TryPeek attempts to read the front item safely.
    if (queue.TryPeek(out string? peeked))
    {
        Console.WriteLine($"TryPeek: {peeked}");
        // Output: TryPeek: cherry
    }

    // TryDequeue on an empty queue returns false.
    queue.Dequeue(); // remove "cherry"
    bool success = queue.TryDequeue(out _);
    Console.WriteLine($"TryDequeue on empty queue: {success}");
    // Output: TryDequeue on empty queue: False
}

void BulkOperations()
{
    Queue<int> queue = new();
    queue.Enqueue(10);
    queue.Enqueue(20);
    queue.Enqueue(30);
    queue.Enqueue(40);

    // Contains checks whether an item is in the queue.
    Console.WriteLine($"Contains 20: {queue.Contains(20)}");
    // Output: Contains 20: True
    Console.WriteLine($"Contains 99: {queue.Contains(99)}");
    // Output: Contains 99: False

    // ToArray copies the queue elements to a new array.
    int[] array = queue.ToArray();
    Console.WriteLine($"ToArray: [{string.Join(", ", array)}]");
    // Output: ToArray: [10, 20, 30, 40]

    // CopyTo copies elements into an existing array at a given index.
    int[] destination = new int[6];
    queue.CopyTo(destination, 2);
    Console.WriteLine($"CopyTo: [{string.Join(", ", destination)}]");
    // Output: CopyTo: [0, 0, 10, 20, 30, 40]

    // Clear removes all items from the queue.
    queue.Clear();
    Console.WriteLine($"Count after Clear: {queue.Count}");
    // Output: Count after Clear: 0
}

void Capacity()
{
    Queue<double> queue = new();

    // EnsureCapacity guarantees minimum internal storage.
    int newCapacity = queue.EnsureCapacity(50);
    Console.WriteLine($"EnsureCapacity(50) returned: {newCapacity}");
    // Output: EnsureCapacity(50) returned: 50 (or higher)

    for (int i = 0; i < 10; i++)
    {
        queue.Enqueue(i * 1.5);
    }
    Console.WriteLine($"Count: {queue.Count}, Capacity: {queue.Capacity}");

    // TrimExcess reduces capacity to match the current count.
    queue.TrimExcess();
    Console.WriteLine($"Capacity after TrimExcess: {queue.Capacity}");
    // Output: Capacity after TrimExcess: 10
}

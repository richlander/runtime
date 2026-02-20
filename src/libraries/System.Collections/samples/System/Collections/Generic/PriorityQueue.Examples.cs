using System;
using System.Collections.Generic;

EnqueueAndDequeue();
EnqueueDequeueAndDequeueEnqueue();
EnqueueRangeDemo();
EnsureCapacityAndTrimExcess();

void EnqueueAndDequeue()
{
    var pq = new PriorityQueue<string, int>();

    pq.Enqueue("Fix crash", 1);
    pq.Enqueue("Code review", 3);
    pq.Enqueue("Write tests", 2);

    Console.WriteLine($"Count: {pq.Count}");
    // Output: Count: 3

    Console.WriteLine($"Peek: {pq.Peek()}");
    // Output: Peek: Fix crash

    if (pq.TryPeek(out string? peekedElement, out int peekedPriority))
    {
        Console.WriteLine($"TryPeek: {peekedElement} (priority {peekedPriority})");
    }
    // Output: TryPeek: Fix crash (priority 1)

    Console.WriteLine($"Dequeue: {pq.Dequeue()}");
    // Output: Dequeue: Fix crash

    if (pq.TryDequeue(out string? element, out int priority))
    {
        Console.WriteLine($"TryDequeue: {element} (priority {priority})");
    }
    // Output: TryDequeue: Write tests (priority 2)

    Console.WriteLine($"Count after removals: {pq.Count}");
    // Output: Count after removals: 1
}

void EnqueueDequeueAndDequeueEnqueue()
{
    var pq = new PriorityQueue<string, int>();
    pq.Enqueue("Existing task", 5);

    // EnqueueDequeue adds an element, then removes and returns the minimum.
    string result1 = pq.EnqueueDequeue("Urgent task", 1);
    Console.WriteLine($"EnqueueDequeue returned: {result1}");
    // Output: EnqueueDequeue returned: Urgent task

    // DequeueEnqueue removes the minimum, then adds a new element.
    pq.Enqueue("Low priority", 10);
    string result2 = pq.DequeueEnqueue("Medium task", 7);
    Console.WriteLine($"DequeueEnqueue returned: {result2}");
    // Output: DequeueEnqueue returned: Existing task

    Console.WriteLine($"Remaining count: {pq.Count}");
    // Output: Remaining count: 2
}

void EnqueueRangeDemo()
{
    var pq = new PriorityQueue<string, int>();

    // EnqueueRange with element-priority pairs.
    pq.EnqueueRange(new (string, int)[]
    {
        ("Task A", 3),
        ("Task B", 1),
        ("Task C", 2)
    });

    Console.WriteLine($"Count after EnqueueRange: {pq.Count}");
    // Output: Count after EnqueueRange: 3

    // EnqueueRange with elements sharing the same priority.
    pq.EnqueueRange(new[] { "Task D", "Task E" }, priority: 4);

    Console.WriteLine($"Count after second EnqueueRange: {pq.Count}");
    // Output: Count after second EnqueueRange: 5

    // Drain the queue to show ordering.
    while (pq.TryDequeue(out string? element, out int priority))
    {
        Console.WriteLine($"  {element} (priority {priority})");
    }
    // Output:
    //   Task B (priority 1)
    //   Task C (priority 2)
    //   Task A (priority 3)
    //   Task D (priority 4)
    //   Task E (priority 4)
}

void EnsureCapacityAndTrimExcess()
{
    var pq = new PriorityQueue<string, int>();

    int capacity = pq.EnsureCapacity(100);
    Console.WriteLine($"Capacity after EnsureCapacity(100): {capacity}");
    // Output: Capacity after EnsureCapacity(100): 100

    pq.Enqueue("Only item", 1);
    pq.TrimExcess();
    Console.WriteLine($"Count after TrimExcess: {pq.Count}");
    // Output: Count after TrimExcess: 1
}

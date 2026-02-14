// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// Runnable examples for PriorityQueue<TElement, TPriority> XML doc comments.
// Each #region is referenced by <code> tags in PriorityQueue.cs.
// Run: dotnet run PriorityQueue.Examples.cs

using System.Collections.Generic;

int failures = 0;
void Fail(string name, string message) { Console.Error.WriteLine($"FAIL: {name} — {message}"); failures++; }

// <Enqueue>
#region Enqueue
var pq = new PriorityQueue<string, int>();
pq.Enqueue("low", 5);
pq.Enqueue("high", 1);
pq.Enqueue("medium", 3);
#endregion
// </Enqueue>
if (pq.Count != 3) Fail("Enqueue", $"Expected 3 items, got {pq.Count}");
if (pq.Peek() != "high") Fail("Enqueue", $"Expected 'high' at top, got '{pq.Peek()}'");

// <Dequeue>
#region Dequeue
var tasks = new PriorityQueue<string, int>();
tasks.Enqueue("backup", 3);
tasks.Enqueue("alert", 1);
tasks.Enqueue("report", 2);

var mostUrgent = tasks.Dequeue(); // "alert" (lowest priority value)
#endregion
// </Dequeue>
if (mostUrgent != "alert") Fail("Dequeue", $"Expected 'alert', got '{mostUrgent}'");
if (tasks.Count != 2) Fail("Dequeue", $"Expected 2 remaining, got {tasks.Count}");

// <Peek>
#region Peek
var jobs = new PriorityQueue<string, int>();
jobs.Enqueue("compress", 2);
jobs.Enqueue("encrypt", 1);

var next = jobs.Peek(); // "encrypt" — not removed
#endregion
// </Peek>
if (next != "encrypt") Fail("Peek", $"Expected 'encrypt', got '{next}'");
if (jobs.Count != 2) Fail("Peek", $"Expected 2 items (Peek should not remove), got {jobs.Count}");

// <TryDequeue>
#region TryDequeue
var incoming = new PriorityQueue<string, int>();
incoming.Enqueue("request-A", 2);

if (incoming.TryDequeue(out var element, out var priority))
{
    Console.WriteLine($"Processed {element} with priority {priority}");
}
#endregion
// </TryDequeue>
if (element != "request-A") Fail("TryDequeue", $"Expected 'request-A', got '{element}'");
if (priority != 2) Fail("TryDequeue", $"Expected priority 2, got {priority}");
if (incoming.TryDequeue(out _, out _)) Fail("TryDequeue", "Should return false on empty queue");

// <TryPeek>
#region TryPeek
var events = new PriorityQueue<string, int>();

var hasPending = events.TryPeek(out var evt, out var prio); // false — queue is empty

events.Enqueue("click", 1);
hasPending = events.TryPeek(out evt, out prio); // true
#endregion
// </TryPeek>
if (hasPending is false) Fail("TryPeek", "Expected true after enqueue");
if (evt != "click") Fail("TryPeek", $"Expected 'click', got '{evt}'");

// <EnqueueDequeue>
#region EnqueueDequeue
var buffer = new PriorityQueue<string, int>();
buffer.Enqueue("existing", 3);

// Atomically enqueue "newcomer" then dequeue the minimum
var popped = buffer.EnqueueDequeue("newcomer", 5); // returns "existing" (priority 3 < 5)
#endregion
// </EnqueueDequeue>
if (popped != "existing") Fail("EnqueueDequeue", $"Expected 'existing', got '{popped}'");
if (buffer.Peek() != "newcomer") Fail("EnqueueDequeue", "Expected 'newcomer' remaining");

// <DequeueEnqueue>
#region DequeueEnqueue
var rotation = new PriorityQueue<string, int>();
rotation.Enqueue("task-A", 1);
rotation.Enqueue("task-B", 2);

// Remove the minimum, then immediately insert a replacement
var removed = rotation.DequeueEnqueue("task-C", 3); // returns "task-A"
#endregion
// </DequeueEnqueue>
if (removed != "task-A") Fail("DequeueEnqueue", $"Expected 'task-A', got '{removed}'");
if (rotation.Count != 2) Fail("DequeueEnqueue", $"Expected 2 items, got {rotation.Count}");

// <EnqueueRange>
#region EnqueueRange
var batch = new PriorityQueue<string, int>();
batch.EnqueueRange([("email", 3), ("sms", 1), ("push", 2)]);
#endregion
// </EnqueueRange>
if (batch.Count != 3) Fail("EnqueueRange", $"Expected 3, got {batch.Count}");
if (batch.Dequeue() != "sms") Fail("EnqueueRange", "Expected 'sms' as min");

// <EnqueueRangeSamePriority>
#region EnqueueRangeSamePriority
var alerts = new PriorityQueue<string, int>();
alerts.EnqueueRange(["server-1", "server-2", "server-3"], priority: 1);
#endregion
// </EnqueueRangeSamePriority>
if (alerts.Count != 3) Fail("EnqueueRangeSamePriority", $"Expected 3, got {alerts.Count}");

// <CountAndClear>
#region CountAndClear
var q = new PriorityQueue<string, int>();
q.Enqueue("a", 1);
q.Enqueue("b", 2);

var count = q.Count; // 2
q.Clear();
var afterClear = q.Count; // 0
#endregion
// </CountAndClear>
if (count != 2) Fail("CountAndClear", $"Expected 2, got {count}");
if (afterClear != 0) Fail("CountAndClear", $"Expected 0 after Clear, got {afterClear}");

// <UnorderedItems>
#region UnorderedItems
var work = new PriorityQueue<string, int>();
work.Enqueue("build", 2);
work.Enqueue("test", 1);
work.Enqueue("deploy", 3);

// Enumerate all items (order is NOT guaranteed)
foreach (var (item, pri) in work.UnorderedItems)
{
    Console.WriteLine($"{item}: {pri}");
}
#endregion
// </UnorderedItems>
var allItems = work.UnorderedItems.Select(x => x.Element).ToHashSet();
if (!allItems.SetEquals(["build", "test", "deploy"])) Fail("UnorderedItems", "Missing expected items");

// <EnsureCapacityAndTrimExcess>
#region EnsureCapacityAndTrimExcess
var pool = new PriorityQueue<string, int>();
var capacity = pool.EnsureCapacity(100); // at least 100

pool.Enqueue("only-one", 1);
pool.TrimExcess(); // shrink backing storage
#endregion
// </EnsureCapacityAndTrimExcess>
if (capacity < 100) Fail("EnsureCapacityAndTrimExcess", $"Expected >= 100, got {capacity}");

// <Remove>
#region Remove
var queue = new PriorityQueue<string, int>();
queue.Enqueue("keep", 1);
queue.Enqueue("drop", 2);
queue.Enqueue("keep-too", 3);

var wasRemoved = queue.Remove("drop", out var removedItem, out var removedPriority);
#endregion
// </Remove>
if (!wasRemoved) Fail("Remove", "Expected Remove to return true");
if (removedItem != "drop") Fail("Remove", $"Expected 'drop', got '{removedItem}'");
if (removedPriority != 2) Fail("Remove", $"Expected priority 2, got {removedPriority}");
if (queue.Count != 2) Fail("Remove", $"Expected 2 remaining, got {queue.Count}");

// <CustomComparer>
#region CustomComparer
// Max-heap: highest priority value is dequeued first
var maxHeap = new PriorityQueue<string, int>(Comparer<int>.Create((a, b) => b.CompareTo(a)));
maxHeap.Enqueue("low", 1);
maxHeap.Enqueue("high", 10);
maxHeap.Enqueue("medium", 5);

var top = maxHeap.Dequeue(); // "high" (10 is greatest)
#endregion
// </CustomComparer>
if (top != "high") Fail("CustomComparer", $"Expected 'high', got '{top}'");

if (failures > 0) { Console.Error.WriteLine($"\n{failures} example(s) failed."); return 1; }
Console.WriteLine("\nAll examples passed.");
return 0;

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// Runnable examples for Queue<T> XML doc comments.
// Each #region is referenced by <code> tags in Queue.cs.
// Run: dotnet run Queue.Examples.cs

using System.Collections.Generic;

int failures = 0;
void Fail(string name, string message) { Console.Error.WriteLine($"FAIL: {name} — {message}"); failures++; }

// --- Constructor from IEnumerable ---

#region CtorIEnumerable
string[] words = ["one", "two", "three"];
var queue = new Queue<string>(words);
// queue now contains: "one", "two", "three" (front to back)
#endregion

if (queue.Count != 3) Fail("CtorIEnumerable", $"Expected 3, got {queue.Count}");
if (queue.Peek() != "one") Fail("CtorIEnumerable", $"Front should be 'one', got '{queue.Peek()}'");

// --- Enqueue ---

#region Enqueue
var colors = new Queue<string>();
colors.Enqueue("red");
colors.Enqueue("green");
colors.Enqueue("blue");
// colors: "red" (front), "green", "blue" (back)
#endregion

if (colors.Count != 3) Fail("Enqueue", $"Expected 3, got {colors.Count}");
if (colors.Peek() != "red") Fail("Enqueue", $"Front should be 'red', got '{colors.Peek()}'");

// --- Dequeue ---

#region Dequeue
var tasks = new Queue<string>(["first", "second", "third"]);
var next = tasks.Dequeue(); // "first" — FIFO order
// tasks now contains: "second", "third"
#endregion

if (next != "first") Fail("Dequeue", $"Expected 'first', got '{next}'");
if (tasks.Count != 2) Fail("Dequeue", $"Expected 2 remaining, got {tasks.Count}");

// --- Peek ---

#region Peek
var nums = new Queue<int>([10, 20, 30]);
var front = nums.Peek(); // 10 — does not remove the element
// nums still contains all three elements
#endregion

if (front != 10) Fail("Peek", $"Expected 10, got {front}");
if (nums.Count != 3) Fail("Peek", $"Expected 3, got {nums.Count}");

// --- TryDequeue ---

#region TryDequeue
var orders = new Queue<string>(["order-1"]);
bool success = orders.TryDequeue(out var order); // true, order = "order-1"
bool empty = orders.TryDequeue(out var none);     // false, none = null
#endregion

if (!success || order != "order-1") Fail("TryDequeue", $"Expected true/'order-1', got {success}/'{order}'");
if (empty || none is not null) Fail("TryDequeue", $"Expected false/null on empty queue");

// --- TryPeek ---

#region TryPeek
var messages = new Queue<string>(["hello"]);
bool peeked = messages.TryPeek(out var msg); // true, msg = "hello"
// "hello" is still in the queue
var emptyQ = new Queue<string>();
bool noPeek = emptyQ.TryPeek(out _);         // false
#endregion

if (!peeked || msg != "hello") Fail("TryPeek", $"Expected true/'hello', got {peeked}/'{msg}'");
if (noPeek) Fail("TryPeek", "Expected false on empty queue");
if (messages.Count != 1) Fail("TryPeek", "TryPeek should not remove elements");

// --- Count ---

#region Count
var letters = new Queue<string>(["a", "b", "c"]);
int count = letters.Count; // 3
letters.Dequeue();
int afterDequeue = letters.Count; // 2
#endregion

if (count != 3) Fail("Count", $"Expected 3, got {count}");
if (afterDequeue != 2) Fail("Count", $"Expected 2, got {afterDequeue}");

// --- Contains ---

#region Contains
var fruits = new Queue<string>(["apple", "banana", "cherry"]);
bool hasApple = fruits.Contains("apple");   // true
bool hasMango = fruits.Contains("mango");   // false
#endregion

if (!hasApple) Fail("Contains", "Expected true for 'apple'");
if (hasMango) Fail("Contains", "Expected false for 'mango'");

// --- Clear ---

#region Clear
var items = new Queue<int>([1, 2, 3]);
items.Clear();
// items.Count is now 0
#endregion

if (items.Count != 0) Fail("Clear", $"Expected 0, got {items.Count}");

// --- ToArray ---

#region ToArray
var source = new Queue<int>([100, 200, 300]);
int[] array = source.ToArray(); // [100, 200, 300] — FIFO order preserved
// source is unchanged
#endregion

if (array is not [100, 200, 300]) Fail("ToArray", $"Expected [100,200,300], got [{string.Join(",", array)}]");
if (source.Count != 3) Fail("ToArray", "ToArray should not modify the queue");

// --- TrimExcess ---

#region TrimExcess
var buffer = new Queue<int>();
for (int i = 0; i < 100; i++) buffer.Enqueue(i);
for (int i = 0; i < 90; i++) buffer.Dequeue();
buffer.TrimExcess(); // reduces internal array to near Count (10)
#endregion

if (buffer.Count != 10) Fail("TrimExcess", $"Expected 10, got {buffer.Count}");

// --- EnsureCapacity ---

#region EnsureCapacity
var data = new Queue<string>();
int capacity = data.EnsureCapacity(50); // at least 50
data.Enqueue("item");
// no reallocation needed for the next 49 enqueues
#endregion

if (capacity < 50) Fail("EnsureCapacity", $"Expected >= 50, got {capacity}");

if (failures > 0) { Console.Error.WriteLine($"\n{failures} example(s) failed."); return 1; }
Console.WriteLine("\nAll examples passed.");
return 0;

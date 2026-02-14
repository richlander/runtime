// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// Runnable examples for Stack<T> XML doc comments.
// Each #region is referenced by <code> tags in Stack.cs.
// Run: dotnet run Stack.Examples.cs

using System.Collections.Generic;

int failures = 0;
void Fail(string name, string message) { Console.Error.WriteLine($"FAIL: {name} — {message}"); failures++; }

// --- Constructor from IEnumerable ---

#region CtorIEnumerable
string[] words = ["one", "two", "three"];
var stack = new Stack<string>(words);
// stack top is "three" — last element pushed is first out
#endregion

if (stack.Count != 3) Fail("CtorIEnumerable", $"Expected 3, got {stack.Count}");
if (stack.Peek() != "three") Fail("CtorIEnumerable", $"Top should be 'three', got '{stack.Peek()}'");

// --- Push ---

#region Push
var colors = new Stack<string>();
colors.Push("red");
colors.Push("green");
colors.Push("blue");
// colors: "blue" (top), "green", "red" (bottom)
#endregion

if (colors.Count != 3) Fail("Push", $"Expected 3, got {colors.Count}");
if (colors.Peek() != "blue") Fail("Push", $"Top should be 'blue', got '{colors.Peek()}'");

// --- Pop ---

#region Pop
var pages = new Stack<string>(["home", "about", "contact"]);
var current = pages.Pop(); // "contact" — LIFO order
// pages now contains: "about" (top), "home"
#endregion

if (current != "contact") Fail("Pop", $"Expected 'contact', got '{current}'");
if (pages.Count != 2) Fail("Pop", $"Expected 2 remaining, got {pages.Count}");

// --- Peek ---

#region Peek
var nums = new Stack<int>([10, 20, 30]);
var top = nums.Peek(); // 30 — does not remove the element
// nums still contains all three elements
#endregion

if (top != 30) Fail("Peek", $"Expected 30, got {top}");
if (nums.Count != 3) Fail("Peek", $"Expected 3, got {nums.Count}");

// --- TryPop ---

#region TryPop
var history = new Stack<string>(["page-1"]);
bool success = history.TryPop(out var page); // true, page = "page-1"
bool empty = history.TryPop(out var none);   // false, none = null
#endregion

if (!success || page != "page-1") Fail("TryPop", $"Expected true/'page-1', got {success}/'{page}'");
if (empty || none is not null) Fail("TryPop", "Expected false/null on empty stack");

// --- TryPeek ---

#region TryPeek
var messages = new Stack<string>(["hello"]);
bool peeked = messages.TryPeek(out var msg); // true, msg = "hello"
// "hello" is still on the stack
var emptyS = new Stack<string>();
bool noPeek = emptyS.TryPeek(out _);         // false
#endregion

if (!peeked || msg != "hello") Fail("TryPeek", $"Expected true/'hello', got {peeked}/'{msg}'");
if (noPeek) Fail("TryPeek", "Expected false on empty stack");
if (messages.Count != 1) Fail("TryPeek", "TryPeek should not remove elements");

// --- Count ---

#region Count
var letters = new Stack<string>(["a", "b", "c"]);
int count = letters.Count; // 3
letters.Pop();
int afterPop = letters.Count; // 2
#endregion

if (count != 3) Fail("Count", $"Expected 3, got {count}");
if (afterPop != 2) Fail("Count", $"Expected 2, got {afterPop}");

// --- Contains ---

#region Contains
var fruits = new Stack<string>(["apple", "banana", "cherry"]);
bool hasApple = fruits.Contains("apple");   // true
bool hasMango = fruits.Contains("mango");   // false
#endregion

if (!hasApple) Fail("Contains", "Expected true for 'apple'");
if (hasMango) Fail("Contains", "Expected false for 'mango'");

// --- Clear ---

#region Clear
var items = new Stack<int>([1, 2, 3]);
items.Clear();
// items.Count is now 0
#endregion

if (items.Count != 0) Fail("Clear", $"Expected 0, got {items.Count}");

// --- ToArray ---

#region ToArray
var source = new Stack<int>([100, 200, 300]);
int[] array = source.ToArray(); // [300, 200, 100] — LIFO order (top first)
// source is unchanged
#endregion

if (array is not [300, 200, 100]) Fail("ToArray", $"Expected [300,200,100], got [{string.Join(",", array)}]");
if (source.Count != 3) Fail("ToArray", "ToArray should not modify the stack");

// --- TrimExcess ---

#region TrimExcess
var buffer = new Stack<int>();
for (int i = 0; i < 100; i++) buffer.Push(i);
for (int i = 0; i < 90; i++) buffer.Pop();
buffer.TrimExcess(); // reduces internal array to near Count (10)
#endregion

if (buffer.Count != 10) Fail("TrimExcess", $"Expected 10, got {buffer.Count}");

// --- EnsureCapacity ---

#region EnsureCapacity
var data = new Stack<string>();
int capacity = data.EnsureCapacity(50); // at least 50
data.Push("item");
// no reallocation needed for the next 49 pushes
#endregion

if (capacity < 50) Fail("EnsureCapacity", $"Expected >= 50, got {capacity}");

if (failures > 0) { Console.Error.WriteLine($"\n{failures} example(s) failed."); return 1; }
Console.WriteLine("\nAll examples passed.");
return 0;

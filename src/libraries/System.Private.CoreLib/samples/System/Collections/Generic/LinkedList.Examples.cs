// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// Runnable examples for LinkedList<T> XML doc comments.
// Each #region is referenced by <code> tags in LinkedList.cs.
// Run: dotnet run LinkedList.Examples.cs

using System.Collections.Generic;

int failures = 0;
void Fail(string name, string message) { Console.Error.WriteLine($"FAIL: {name} — {message}"); failures++; }

// --- Constructor from IEnumerable ---

#region ConstructorFromEnumerable
var list = new LinkedList<string>(["fox", "dog", "cat"]);
#endregion

if (list.Count != 3) Fail("ConstructorFromEnumerable", $"Expected 3 items, got {list.Count}");
if (list.First!.Value != "fox") Fail("ConstructorFromEnumerable", $"First should be 'fox', got '{list.First.Value}'");

// --- Count ---

#region Count
var numbers = new LinkedList<int>([10, 20, 30]);
var count = numbers.Count;
#endregion

if (count != 3) Fail("Count", $"Expected 3, got {count}");

// --- AddFirst ---

#region AddFirst
var colors = new LinkedList<string>(["green", "blue"]);
colors.AddFirst("red");
// colors: red -> green -> blue
#endregion

if (colors.First!.Value != "red") Fail("AddFirst", $"Expected 'red' first, got '{colors.First.Value}'");
if (colors.Count != 3) Fail("AddFirst", $"Expected 3 items, got {colors.Count}");

// --- AddLast ---

#region AddLast
var fruits = new LinkedList<string>(["apple", "banana"]);
fruits.AddLast("cherry");
// fruits: apple -> banana -> cherry
#endregion

if (fruits.Last!.Value != "cherry") Fail("AddLast", $"Expected 'cherry' last, got '{fruits.Last.Value}'");
if (fruits.Count != 3) Fail("AddLast", $"Expected 3 items, got {fruits.Count}");

// --- First and Last ---

#region FirstAndLast
var seasons = new LinkedList<string>(["spring", "summer", "fall", "winter"]);
var first = seasons.First!.Value;
var last = seasons.Last!.Value;
#endregion

if (first != "spring") Fail("FirstAndLast", $"Expected 'spring', got '{first}'");
if (last != "winter") Fail("FirstAndLast", $"Expected 'winter', got '{last}'");

// --- Find ---

#region Find
var animals = new LinkedList<string>(["cat", "dog", "bird", "dog"]);
var found = animals.Find("dog");
// Finds the first occurrence of "dog"
#endregion

if (found is null) Fail("Find", "Expected to find 'dog'");
if (found!.Value != "dog") Fail("Find", $"Expected 'dog', got '{found.Value}'");
if (found != animals.First!.Next) Fail("Find", "Expected first 'dog' node (second element)");

// --- FindLast ---

#region FindLast
var duplicates = new LinkedList<string>(["cat", "dog", "bird", "dog"]);
var lastDog = duplicates.FindLast("dog");
// Finds the last occurrence of "dog"
#endregion

if (lastDog is null) Fail("FindLast", "Expected to find last 'dog'");
if (lastDog != duplicates.Last) Fail("FindLast", "Expected last 'dog' to be the tail node");

// --- AddBefore ---

#region AddBefore
var planets = new LinkedList<string>(["Mercury", "Earth", "Mars"]);
var earth = planets.Find("Earth")!;
planets.AddBefore(earth, "Venus");
// planets: Mercury -> Venus -> Earth -> Mars
#endregion

if (earth.Previous!.Value != "Venus") Fail("AddBefore", $"Expected 'Venus' before Earth, got '{earth.Previous.Value}'");
if (planets.Count != 4) Fail("AddBefore", $"Expected 4 items, got {planets.Count}");

// --- AddAfter ---

#region AddAfter
var days = new LinkedList<string>(["Mon", "Wed", "Thu"]);
var mon = days.Find("Mon")!;
days.AddAfter(mon, "Tue");
// days: Mon -> Tue -> Wed -> Thu
#endregion

if (mon.Next!.Value != "Tue") Fail("AddAfter", $"Expected 'Tue' after Mon, got '{mon.Next.Value}'");
if (days.Count != 4) Fail("AddAfter", $"Expected 4 items, got {days.Count}");

// --- Node navigation (Next / Previous) ---

#region NodeNavigation
var letters = new LinkedList<string>(["A", "B", "C"]);
var nodeB = letters.Find("B")!;
var prev = nodeB.Previous!.Value; // "A"
var next = nodeB.Next!.Value;     // "C"
#endregion

if (prev != "A") Fail("NodeNavigation", $"Expected 'A', got '{prev}'");
if (next != "C") Fail("NodeNavigation", $"Expected 'C', got '{next}'");

// --- Contains ---

#region Contains
var tools = new LinkedList<string>(["hammer", "wrench", "drill"]);
var hasWrench = tools.Contains("wrench");
var hasSaw = tools.Contains("saw");
#endregion

if (!hasWrench) Fail("Contains", "Expected to contain 'wrench'");
if (hasSaw) Fail("Contains", "Did not expect to contain 'saw'");

// --- RemoveFirst ---

#region RemoveFirst
var queue = new LinkedList<int>([1, 2, 3]);
queue.RemoveFirst();
// queue: 2 -> 3
#endregion

if (queue.First!.Value != 2) Fail("RemoveFirst", $"Expected first to be 2, got {queue.First.Value}");
if (queue.Count != 2) Fail("RemoveFirst", $"Expected 2 items, got {queue.Count}");

// --- RemoveLast ---

#region RemoveLast
var stack = new LinkedList<int>([1, 2, 3]);
stack.RemoveLast();
// stack: 1 -> 2
#endregion

if (stack.Last!.Value != 2) Fail("RemoveLast", $"Expected last to be 2, got {stack.Last.Value}");
if (stack.Count != 2) Fail("RemoveLast", $"Expected 2 items, got {stack.Count}");

// --- Remove(T) ---

#region RemoveByValue
var cities = new LinkedList<string>(["Paris", "London", "Tokyo", "London"]);
cities.Remove("London");
// Removes the first "London"; cities: Paris -> Tokyo -> London
#endregion

if (cities.Count != 3) Fail("RemoveByValue", $"Expected 3 items, got {cities.Count}");
if (cities.First!.Next!.Value != "Tokyo") Fail("RemoveByValue", $"Expected 'Tokyo' second, got '{cities.First.Next.Value}'");
if (cities.Last!.Value != "London") Fail("RemoveByValue", "Expected remaining 'London' at tail");

// --- Remove(LinkedListNode<T>) ---

#region RemoveByNode
var scores = new LinkedList<int>([10, 20, 30, 40]);
var node30 = scores.Find(30)!;
scores.Remove(node30);
// scores: 10 -> 20 -> 40
#endregion

if (scores.Count != 3) Fail("RemoveByNode", $"Expected 3 items, got {scores.Count}");
if (!scores.Contains(20) || !scores.Contains(40)) Fail("RemoveByNode", "Expected 20 and 40 to remain");
if (scores.Contains(30)) Fail("RemoveByNode", "Did not expect 30 to remain");

// --- Clear ---

#region Clear
var temp = new LinkedList<int>([1, 2, 3]);
temp.Clear();
// temp is now empty
#endregion

if (temp.Count != 0) Fail("Clear", $"Expected 0 items, got {temp.Count}");
if (temp.First is not null) Fail("Clear", "Expected First to be null");

if (failures > 0) { Console.Error.WriteLine($"\n{failures} example(s) failed."); return 1; }
Console.WriteLine("\nAll examples passed.");
return 0;

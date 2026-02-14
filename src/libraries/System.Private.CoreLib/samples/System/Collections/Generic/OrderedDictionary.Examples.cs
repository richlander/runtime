// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// Runnable examples for OrderedDictionary<TKey, TValue> XML doc comments.
// Each #region is referenced by <code> tags in OrderedDictionary.cs.
// Run: dotnet run OrderedDictionary.Examples.cs

using System.Collections.Generic;

int failures = 0;
void Fail(string name, string message) { Console.Error.WriteLine($"FAIL: {name} — {message}"); failures++; }

// <Add>
#region Add
var settings = new OrderedDictionary<string, string>();
settings.Add("theme", "dark");
settings.Add("language", "en");
settings.Add("fontSize", "14");
#endregion
// </Add>
if (settings.Count != 3) Fail("Add", $"Expected 3, got {settings.Count}");
if (settings.GetAt(0).Key != "theme") Fail("Add", "Insertion order not preserved");

// <TryAdd>
#region TryAdd
var users = new OrderedDictionary<int, string>();
var added = users.TryAdd(1, "Alice");   // true — key 1 is new
var duplicate = users.TryAdd(1, "Bob"); // false — key 1 already exists
#endregion
// </TryAdd>
if (!added) Fail("TryAdd", "Expected true for new key");
if (duplicate) Fail("TryAdd", "Expected false for duplicate key");
if (users[1] != "Alice") Fail("TryAdd", "Value should remain 'Alice'");

// <Indexer>
#region Indexer
var config = new OrderedDictionary<string, int>();
config["retries"] = 3;
config["timeout"] = 30;

var retries = config["retries"]; // 3
config["retries"] = 5;          // update existing key
#endregion
// </Indexer>
if (retries != 3) Fail("Indexer", $"Expected 3, got {retries}");
if (config["retries"] != 5) Fail("Indexer", $"Expected 5 after update, got {config["retries"]}");

// <Remove>
#region Remove
var inventory = new OrderedDictionary<string, int>
{
    ["apple"] = 10,
    ["banana"] = 5,
    ["cherry"] = 8
};

var wasRemoved = inventory.Remove("banana", out var removedCount); // true, removedCount = 5
#endregion
// </Remove>
if (!wasRemoved) Fail("Remove", "Expected true");
if (removedCount != 5) Fail("Remove", $"Expected 5, got {removedCount}");
if (inventory.Count != 2) Fail("Remove", $"Expected 2, got {inventory.Count}");

// <ContainsKey>
#region ContainsKey
var env = new OrderedDictionary<string, string> { ["PATH"] = "/usr/bin", ["HOME"] = "/home/user" };

var hasPath = env.ContainsKey("PATH");  // true
var hasShell = env.ContainsKey("SHELL"); // false
#endregion
// </ContainsKey>
if (!hasPath) Fail("ContainsKey", "Expected true for PATH");
if (hasShell) Fail("ContainsKey", "Expected false for SHELL");

// <ContainsValue>
#region ContainsValue
var colors = new OrderedDictionary<string, string>
{
    ["sky"] = "blue",
    ["grass"] = "green"
};

var hasBlue = colors.ContainsValue("blue");   // true
var hasRed = colors.ContainsValue("red");     // false
#endregion
// </ContainsValue>
if (!hasBlue) Fail("ContainsValue", "Expected true for 'blue'");
if (hasRed) Fail("ContainsValue", "Expected false for 'red'");

// <TryGetValue>
#region TryGetValue
var scores = new OrderedDictionary<string, int> { ["Alice"] = 95, ["Bob"] = 82 };

if (scores.TryGetValue("Alice", out var score))
{
    Console.WriteLine($"Alice scored {score}");
}
#endregion
// </TryGetValue>
if (score != 95) Fail("TryGetValue", $"Expected 95, got {score}");
if (scores.TryGetValue("Zara", out _)) Fail("TryGetValue", "Expected false for missing key");

// <CountAndKeysAndValues>
#region CountAndKeysAndValues
var rgb = new OrderedDictionary<string, int>
{
    ["red"] = 0xFF0000,
    ["green"] = 0x00FF00,
    ["blue"] = 0x0000FF
};

var numColors = rgb.Count;               // 3
var keys = rgb.Keys.ToArray();           // ["red", "green", "blue"] — insertion order
var values = rgb.Values.ToArray();       // [0xFF0000, 0x00FF00, 0x0000FF]
#endregion
// </CountAndKeysAndValues>
if (numColors != 3) Fail("CountAndKeysAndValues", $"Expected 3, got {numColors}");
if (!keys.SequenceEqual(["red", "green", "blue"])) Fail("CountAndKeysAndValues", "Keys not in insertion order");
if (values[0] != 0xFF0000) Fail("CountAndKeysAndValues", "Values mismatch");

// <GetAt>
#region GetAt
var pipeline = new OrderedDictionary<string, string>
{
    ["step1"] = "build",
    ["step2"] = "test",
    ["step3"] = "deploy"
};

var first = pipeline.GetAt(0);  // KeyValuePair("step1", "build")
var last = pipeline.GetAt(2);   // KeyValuePair("step3", "deploy")
#endregion
// </GetAt>
if (first.Key != "step1" || first.Value != "build") Fail("GetAt", "First element mismatch");
if (last.Key != "step3" || last.Value != "deploy") Fail("GetAt", "Last element mismatch");

// <SetAt>
#region SetAt
var schedule = new OrderedDictionary<string, string>
{
    ["mon"] = "standup",
    ["wed"] = "review",
    ["fri"] = "retro"
};

schedule.SetAt(1, "planning"); // update value at index 1, key stays "wed"
#endregion
// </SetAt>
if (schedule["wed"] != "planning") Fail("SetAt", $"Expected 'planning', got '{schedule["wed"]}'");
if (schedule.GetAt(1).Key != "wed") Fail("SetAt", "Key at index 1 should remain 'wed'");

// <Insert>
#region Insert
var steps = new OrderedDictionary<string, int>
{
    ["init"] = 1,
    ["finish"] = 3
};

steps.Insert(1, "process", 2); // insert at index 1, shifting "finish" to index 2
#endregion
// </Insert>
if (steps.GetAt(0).Key != "init") Fail("Insert", "Index 0 should be 'init'");
if (steps.GetAt(1).Key != "process") Fail("Insert", "Index 1 should be 'process'");
if (steps.GetAt(2).Key != "finish") Fail("Insert", "Index 2 should be 'finish'");

// <RemoveAt>
#region RemoveAt
var queue = new OrderedDictionary<string, int>
{
    ["first"] = 1,
    ["second"] = 2,
    ["third"] = 3
};

queue.RemoveAt(0); // removes "first", shifts remaining items
#endregion
// </RemoveAt>
if (queue.Count != 2) Fail("RemoveAt", $"Expected 2, got {queue.Count}");
if (queue.GetAt(0).Key != "second") Fail("RemoveAt", "Index 0 should now be 'second'");

// <IndexOf>
#region IndexOf
var playlist = new OrderedDictionary<string, string>
{
    ["track1"] = "Song A",
    ["track2"] = "Song B",
    ["track3"] = "Song C"
};

var idx = playlist.IndexOf("track2"); // 1
var missing = playlist.IndexOf("track9"); // -1
#endregion
// </IndexOf>
if (idx != 1) Fail("IndexOf", $"Expected 1, got {idx}");
if (missing != -1) Fail("IndexOf", $"Expected -1, got {missing}");

// <EnsureCapacityAndTrimExcess>
#region EnsureCapacityAndTrimExcess
var cache = new OrderedDictionary<string, byte[]>();
var cap = cache.EnsureCapacity(256); // pre-allocate for at least 256 entries

cache.Add("item", [0x01]);
cache.TrimExcess(); // release unused memory
#endregion
// </EnsureCapacityAndTrimExcess>
if (cap < 256) Fail("EnsureCapacityAndTrimExcess", $"Expected >= 256, got {cap}");

// <Clear>
#region Clear
var data = new OrderedDictionary<string, int> { ["a"] = 1, ["b"] = 2 };
data.Clear();
#endregion
// </Clear>
if (data.Count != 0) Fail("Clear", $"Expected 0 after Clear, got {data.Count}");

if (failures > 0) { Console.Error.WriteLine($"\n{failures} example(s) failed."); return 1; }
Console.WriteLine("\nAll examples passed.");
return 0;

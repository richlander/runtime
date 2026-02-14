// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// Runnable examples for Comparer<T> and EqualityComparer<T> XML doc comments.
// Each #region is referenced by <code> tags in Comparer.cs / EqualityComparer.cs.
// Run: dotnet run Comparer.Examples.cs

using System.Collections.Generic;

int failures = 0;
void Fail(string name, string message) { Console.Error.WriteLine($"FAIL: {name} — {message}"); failures++; }

// ────────────────────────────────────────────
//  Comparer<T> examples
// ────────────────────────────────────────────

#region ComparerDefault
var comparer = Comparer<int>.Default;
int result = comparer.Compare(3, 5);
Console.WriteLine($"Compare(3, 5) = {result}");   // negative
Console.WriteLine($"Compare(5, 5) = {comparer.Compare(5, 5)}"); // 0
#endregion
if (result >= 0) Fail("ComparerDefault", $"Expected negative, got {result}");
if (comparer.Compare(5, 5) != 0) Fail("ComparerDefault", "Expected 0 for equal values");

#region ComparerCreate
var lengthComparer = Comparer<string>.Create((x, y) => x.Length.CompareTo(y.Length));
List<string> words = ["banana", "fig", "apple", "kiwi"];
words.Sort(lengthComparer);
Console.WriteLine($"Sorted by length: {string.Join(", ", words)}");
#endregion
if (words[0] is not "fig") Fail("ComparerCreate", $"Expected 'fig' first, got '{words[0]}'");
if (words[^1] is not "banana") Fail("ComparerCreate", $"Expected 'banana' last, got '{words[^1]}'");

#region ComparerCustomSubclass
var byLengthThenAlpha = new LengthThenAlphaComparer();
List<string> fruits = ["pear", "kiwi", "fig", "apple", "date"];
fruits.Sort(byLengthThenAlpha);
Console.WriteLine($"Multi-field sort: {string.Join(", ", fruits)}");
#endregion
if (fruits[0] is not "fig") Fail("ComparerCustomSubclass", $"Expected 'fig' first, got '{fruits[0]}'");
if (fruits[1] is not "date") Fail("ComparerCustomSubclass", "Expected 'date' second after 'fig'");
if (fruits[^1] is not "apple") Fail("ComparerCustomSubclass", $"Expected 'apple' last, got '{fruits[^1]}'");

#region ComparerWithListSort
var descending = Comparer<int>.Create((x, y) => y.CompareTo(x));
List<int> numbers = [5, 3, 8, 1, 4];
numbers.Sort(descending);
Console.WriteLine($"Descending: {string.Join(", ", numbers)}");
#endregion
if (numbers is not [8, 5, 4, 3, 1]) Fail("ComparerWithListSort", $"Unexpected order: {string.Join(", ", numbers)}");

#region ComparerDefaultString
var stringComparer = Comparer<string>.Default;
int cmp = stringComparer.Compare("apple", "banana");
Console.WriteLine($"Compare(\"apple\", \"banana\") = {cmp}");  // negative
#endregion
if (cmp >= 0) Fail("ComparerDefaultString", $"Expected negative, got {cmp}");

#region ComparerWithRecords
var byAge = Comparer<Person>.Create((a, b) => a.Age.CompareTo(b.Age));
List<Person> people = [new("Charlie", 30), new("Alice", 25), new("Bob", 35)];
people.Sort(byAge);
Console.WriteLine($"By age: {string.Join(", ", people.Select(p => $"{p.Name}({p.Age})"))}");
#endregion
if (people[0].Name is not "Alice") Fail("ComparerWithRecords", $"Expected Alice first, got {people[0].Name}");
if (people[^1].Name is not "Bob") Fail("ComparerWithRecords", $"Expected Bob last, got {people[^1].Name}");

// ────────────────────────────────────────────
//  EqualityComparer<T> examples
// ────────────────────────────────────────────

#region EqualityComparerDefault
var eqComparer = EqualityComparer<string>.Default;
bool areEqual = eqComparer.Equals("hello", "hello");
bool areDiff = eqComparer.Equals("hello", "world");
Console.WriteLine($"Equals(\"hello\", \"hello\") = {areEqual}");
Console.WriteLine($"Equals(\"hello\", \"world\") = {areDiff}");
#endregion
if (!areEqual) Fail("EqualityComparerDefault", "Expected true for equal strings");
if (areDiff) Fail("EqualityComparerDefault", "Expected false for different strings");

#region EqualityComparerCustomSubclass
var caseInsensitive = new CaseInsensitiveComparer();
Console.WriteLine($"Equals(\"Hello\", \"hello\") = {caseInsensitive.Equals("Hello", "hello")}");
Console.WriteLine($"GetHashCode(\"Hello\") == GetHashCode(\"hello\"): {caseInsensitive.GetHashCode("Hello") == caseInsensitive.GetHashCode("hello")}");
#endregion
if (!caseInsensitive.Equals("Hello", "hello")) Fail("EqualityComparerCustomSubclass", "Expected case-insensitive equality");
if (caseInsensitive.GetHashCode("Hello") != caseInsensitive.GetHashCode("hello")) Fail("EqualityComparerCustomSubclass", "Hash codes should match");

#region EqualityComparerWithDictionary
var dict = new Dictionary<string, int>(new CaseInsensitiveComparer());
dict["Hello"] = 1;
dict["hello"] = 2;  // overwrites — same key under case-insensitive comparison
Console.WriteLine($"dict.Count = {dict.Count}");
Console.WriteLine($"dict[\"HELLO\"] = {dict["HELLO"]}");
#endregion
if (dict.Count != 1) Fail("EqualityComparerWithDictionary", $"Expected 1 entry, got {dict.Count}");
if (dict["HELLO"] != 2) Fail("EqualityComparerWithDictionary", $"Expected 2, got {dict["HELLO"]}");

#region EqualityComparerWithHashSet
var set = new HashSet<string>(new CaseInsensitiveComparer());
set.Add("Apple");
bool added = set.Add("apple");  // not added — duplicate under case-insensitive comparison
Console.WriteLine($"set.Count = {set.Count}");
Console.WriteLine($"Duplicate added? {added}");
#endregion
if (set.Count != 1) Fail("EqualityComparerWithHashSet", $"Expected 1 element, got {set.Count}");
if (added) Fail("EqualityComparerWithHashSet", "Expected duplicate to be rejected");

#region EqualityComparerCompositeKey
var coordComparer = new CoordinateComparer();
var coordDict = new Dictionary<Coordinate, string>(coordComparer);
coordDict[new(1, 2)] = "A";
coordDict[new(1, 2)] = "B";  // overwrites — same coordinate
coordDict[new(3, 4)] = "C";
Console.WriteLine($"coordDict.Count = {coordDict.Count}");
Console.WriteLine($"coordDict[(1,2)] = {coordDict[new(1, 2)]}");
#endregion
if (coordDict.Count != 2) Fail("EqualityComparerCompositeKey", $"Expected 2 entries, got {coordDict.Count}");
if (coordDict[new(1, 2)] is not "B") Fail("EqualityComparerCompositeKey", "Expected 'B' for (1,2)");

#region EqualityComparerDefaultValueType
var intEq = EqualityComparer<int>.Default;
Console.WriteLine($"Equals(42, 42) = {intEq.Equals(42, 42)}");
Console.WriteLine($"GetHashCode(42) = {intEq.GetHashCode(42)}");
#endregion
if (!intEq.Equals(42, 42)) Fail("EqualityComparerDefaultValueType", "Expected true for equal ints");
if (intEq.GetHashCode(42) != 42.GetHashCode()) Fail("EqualityComparerDefaultValueType", "Hash codes should match");

if (failures > 0) { Console.Error.WriteLine($"\n{failures} example(s) failed."); return 1; }
Console.WriteLine("\nAll examples passed.");
return 0;

// ────────────────────────────────────────────
//  Helper types (defined after entry-point code)
// ────────────────────────────────────────────

record Person(string Name, int Age);

record struct Coordinate(int X, int Y);

class LengthThenAlphaComparer : Comparer<string>
{
    public override int Compare(string? x, string? y) =>
        (x, y) switch
        {
            (null, null) => 0,
            (null, _) => -1,
            (_, null) => 1,
            _ => x.Length != y.Length
                ? x.Length.CompareTo(y.Length)
                : string.Compare(x, y, StringComparison.Ordinal)
        };
}

class CaseInsensitiveComparer : EqualityComparer<string>
{
    public override bool Equals(string? x, string? y) =>
        string.Equals(x, y, StringComparison.OrdinalIgnoreCase);

    public override int GetHashCode(string obj) =>
        obj.ToUpperInvariant().GetHashCode();
}

class CoordinateComparer : EqualityComparer<Coordinate>
{
    public override bool Equals(Coordinate x, Coordinate y) =>
        x.X == y.X && x.Y == y.Y;

    public override int GetHashCode(Coordinate obj) =>
        HashCode.Combine(obj.X, obj.Y);
}

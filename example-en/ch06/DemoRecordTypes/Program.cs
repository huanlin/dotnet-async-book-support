Console.WriteLine("Demo: immutable record types and the with expression");

// 1. Create a record instance.
var alice = new Person("Alice", 30);
Console.WriteLine($"Original object: {alice}");
// The output is automatically formatted as:
// Person { Name = Alice, Age = 30 }

// 2. Trying to mutate a property causes a compiler error.
// alice.Age = 31; // Error CS8852: Init-only property...

// 3. Demonstrate value equality.
var aliceClone = new Person("Alice", 30);
Console.WriteLine($"alice == aliceClone ? {alice == aliceClone}");
// Prints True, even though they are two different objects in memory.

// 4. Demonstrate nondestructive mutation with the with expression.
var olderAlice = alice with { Age = 31 };
Console.WriteLine("\nCreate a new version with the with expression:");
Console.WriteLine($"Old version (completely unchanged): {alice}");
Console.WriteLine($"New version: {olderAlice}");

// --- Record definition ---

// This one short line causes the compiler to generate:
// 1. Two init-only properties: Name and Age
// 2. Equals and GetHashCode for value equality
// 3. A nice ToString() implementation
// 4. The copy semantics needed by the with expression
// 5. A Deconstruct method for `var (name, age) = person`
public record Person(string Name, int Age);

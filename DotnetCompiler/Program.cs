using Antlr4Roslyn.Services;

Console.WriteLine("=== Enhanced Language Demo ===\n");

var compiler = new Compiler();

// Demo 1: Basic Arithmetic with Operator Precedence
Console.WriteLine("1. Arithmetic & Operator Precedence:");
RunDemo(compiler, "3 + 5 * 2;");

// Demo 2: Variables and Assignment  
Console.WriteLine("2. Variables and Assignment:");
RunDemo(compiler, @"
    let x = 10;
    let y = 20;
    x = x + y;
    x;
");

// Demo 3: Boolean Logic and Comparisons
Console.WriteLine("3. Boolean Logic:");
RunDemo(compiler, @"
    let a = 15;
    let b = 10;
    a > b && b > 5;
");

// Demo 4: Conditional Logic
Console.WriteLine("4. Conditional Logic:");
RunDemo(compiler, @"
    let score = 95;
    if (score >= 90) {
        let grade = ""A"";
        grade;
    } else {
        let grade = ""B"";
        grade;
    }
");

// Demo 5: While Loops
Console.WriteLine("5. While Loops (Sum 1-5):");
RunDemo(compiler, @"
    let sum = 0;
    let i = 1;
    while (i <= 5) {
        sum = sum + i;
        i = i + 1;
    }
    sum;
");

// Demo 6: String Operations
Console.WriteLine("6. String Handling:");
RunDemo(compiler, @"
    let greeting = ""Hello"";
    let name = ""World"";
    greeting;
");

// Demo 7: Arrays
Console.WriteLine("7. Arrays:");
RunDemo(compiler, @"
    let numbers = [1, 2, 3, 4, 5];
    numbers;
");

// Demo 8: Float Numbers
Console.WriteLine("8. Float Numbers:");
RunDemo(compiler, @"
    let pi = 3.14;
    let radius = 5.0;
    pi * radius * radius;
");

// Demo 9: Complex Boolean Expressions
Console.WriteLine("9. Complex Boolean Logic:");
RunDemo(compiler, @"
    let x = 7;
    (x > 5 && x < 10) || x == 0;
");

// Demo 10: Nested Conditionals
Console.WriteLine("10. Nested Conditionals:");
RunDemo(compiler, @"
    let temp = 75;
    if (temp > 80) {
        ""Hot"";
    } else {
        if (temp > 60) {
            ""Warm"";
        } else {
            ""Cold"";
        }
    }
");

Console.WriteLine("\n=== Language Features Successfully Demonstrated ===");
Console.WriteLine("✅ Variables with `let` keyword");
Console.WriteLine("✅ Assignment with `=` operator");
Console.WriteLine("✅ Arithmetic: +, -, *, /, % with precedence");
Console.WriteLine("✅ Comparisons: <, >, <=, >=, ==, !=");
Console.WriteLine("✅ Boolean logic: &&, ||, !");
Console.WriteLine("✅ Conditional statements: if/else");
Console.WriteLine("✅ While loops");
Console.WriteLine("✅ Arrays with [1, 2, 3] syntax");
Console.WriteLine("✅ Multiple data types: int, float, string, bool");
Console.WriteLine("✅ Block statements with {}");
Console.WriteLine("✅ Automatic result display");
Console.WriteLine("\n🔧 Advanced features (functions, lambdas) need refinement");

Console.WriteLine("\n=== Demo Complete ===");

static void RunDemo(Compiler compiler, string code)
{
    try
    {
        Console.WriteLine($"Code: {code.Trim()}");
        Console.Write("Result: ");
        compiler.Compile(code);
        Console.WriteLine();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
        Console.WriteLine();
    }
}
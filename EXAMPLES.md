# Simple Language Examples

This document showcases the capabilities of the Simple programming language through practical examples.

## 🎯 Basic Examples

### Hello World
```javascript
"Hello, World!";
```

### Variables and Constants
```javascript
let name = "Simple";
let version = 1.0;
let isAwesome = true;
let numbers = [1, 2, 3, 4, 5];

name;  // Output: Simple
```

## 🧮 Mathematical Expressions

### Arithmetic Operations
```javascript
// Basic arithmetic with proper precedence
let result = 3 + 5 * 2;  // 13

// Parentheses for grouping
let grouped = (10 + 5) * 2;  // 30

// Mixed operations
let complex = 3.14 * 5 * 5 + 2 * 3;  // 84.5
```

### Boolean Logic
```javascript
let a = 10;
let b = 5;

// Comparisons
a > b;        // true
a == b * 2;   // true  
a != b;       // true

// Logical operations
a > 0 && b > 0;           // true
a < 0 || b > 0;           // true
!(a == b);                // true

// Complex expressions
(a > 5 && a < 15) || a == 0;  // true
```

## 🔄 Control Flow

### Conditional Statements
```javascript
let score = 85;

if (score >= 90) {
    let grade = "A";
    grade;
} else if (score >= 80) {
    let grade = "B"; 
    grade;
} else {
    let grade = "C";
    grade;
}
// Output: B
```

### Nested Conditions
```javascript
let temperature = 75;
let weather = "sunny";

if (temperature > 70) {
    if (weather == "sunny") {
        "Perfect day for a picnic!";
    } else {
        "Warm but might rain";
    }
} else {
    "Too cold for outdoor activities";
}
// Output: Perfect day for a picnic!
```

### While Loops
```javascript
// Sum of numbers 1 to 10
let sum = 0;
let i = 1;

while (i <= 10) {
    sum = sum + i;
    i = i + 1;
}

sum;  // Output: 55
```

### Countdown Example
```javascript
let countdown = 5;

while (countdown > 0) {
    countdown = countdown - 1;
}

countdown;  // Output: 0
```

## 📊 Working with Arrays

### Array Creation and Usage
```javascript
// Different types of arrays
let numbers = [1, 2, 3, 4, 5];
let mixed = [42, "hello", true, 3.14];
let empty = [];

numbers;  // Output: System.Object[]
```

### Array in Computations
```javascript
let values = [10, 20, 30];
let first = 10;   // Simulating values[0]
let second = 20;  // Simulating values[1]

let sum = first + second;  // 30
```

## 🎲 Practical Examples

### Fibonacci Sequence (Iterative)
```javascript
let n = 10;
let a = 0;
let b = 1;
let count = 0;

while (count < n) {
    let temp = a + b;
    a = b;
    b = temp;
    count = count + 1;
}

a;  // Output: 55 (10th Fibonacci number)
```

### Prime Number Check
```javascript
let number = 17;
let isPrime = true;
let divisor = 2;

if (number <= 1) {
    isPrime = false;
} else {
    while (divisor * divisor <= number) {
        if (number % divisor == 0) {
            isPrime = false;
        }
        divisor = divisor + 1;
    }
}

isPrime;  // Output: true
```

### Factorial Calculator
```javascript
let n = 5;
let factorial = 1;
let i = 1;

while (i <= n) {
    factorial = factorial * i;
    i = i + 1;
}

factorial;  // Output: 120
```

### Power Function
```javascript
let base = 2;
let exponent = 8;
let result = 1;
let counter = 0;

while (counter < exponent) {
    result = result * base;
    counter = counter + 1;
}

result;  // Output: 256
```

### Greatest Common Divisor (GCD)
```javascript
let a = 48;
let b = 18;

while (b != 0) {
    let temp = b;
    b = a % b;
    a = temp;
}

a;  // Output: 6
```

## 🎯 Complex Examples

### Grade Calculator
```javascript
let totalPoints = 0;
let totalAssignments = 0;

// Assignment 1
let assignment1 = 85;
totalPoints = totalPoints + assignment1;
totalAssignments = totalAssignments + 1;

// Assignment 2  
let assignment2 = 92;
totalPoints = totalPoints + assignment2;
totalAssignments = totalAssignments + 1;

// Assignment 3
let assignment3 = 78;
totalPoints = totalPoints + assignment3;
totalAssignments = totalAssignments + 1;

let average = totalPoints / totalAssignments;

if (average >= 90) {
    "A";
} else if (average >= 80) {
    "B";
} else if (average >= 70) {
    "C";
} else {
    "F";
}
// Output: B
```

### Simple Interest Calculator
```javascript
let principal = 1000.0;
let rate = 5.0;        // 5% annual rate
let time = 3.0;        // 3 years

let interest = (principal * rate * time) / 100.0;
let total = principal + interest;

total;  // Output: 1150
```

### Temperature Converter
```javascript
let celsius = 25.0;
let fahrenheit = (celsius * 9.0 / 5.0) + 32.0;

fahrenheit;  // Output: 77
```

## 🔄 Pattern Examples

### Flag Setting Pattern
```javascript
let hasError = false;
let value = -5;

if (value < 0) {
    hasError = true;
}

if (hasError) {
    "Error: Negative value not allowed";
} else {
    "Value is valid";
}
// Output: Error: Negative value not allowed
```

### State Machine Pattern
```javascript
let state = 1;
let input = 2;

if (state == 1) {
    if (input == 1) {
        state = 2;
    } else {
        state = 3;
    }
} else if (state == 2) {
    state = 1;
}

state;  // Output: 3
```

### Counter with Limit
```javascript
let counter = 0;
let limit = 5;
let increment = 2;

while (counter < limit) {
    counter = counter + increment;
}

counter;  // Output: 6
```

## 🎨 Creative Examples

### ASCII Art Counter
```javascript
let stars = "";
let count = 0;

while (count < 5) {
    // In a real implementation, we'd append to stars
    count = count + 1;
}

count;  // Shows we counted to 5
```

### Simple Game State
```javascript
let playerHealth = 100;
let damage = 25;
let healingPotion = 30;

// Take damage
playerHealth = playerHealth - damage;  // 75

// Use healing potion
playerHealth = playerHealth + healingPotion;  // 105

// Cap at 100
if (playerHealth > 100) {
    playerHealth = 100;
}

playerHealth;  // Output: 100
```

---

These examples demonstrate the current capabilities of the Simple programming language. As new features are added (functions, advanced arrays, etc.), this document will be updated with more sophisticated examples.

## 🚀 Try These Examples

Copy any of these examples and run them with:
```bash
dotnet run --project DotnetCompiler
```

Modify the demo in `Program.cs` to test your own variations!
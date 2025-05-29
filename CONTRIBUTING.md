# Contributing to Simple Programming Language

Thank you for your interest in contributing to the Simple Programming Language compiler! This document provides guidelines and information for contributors.

## 🚀 Getting Started

### Prerequisites
- .NET SDK 8.0 or later
- Basic understanding of ANTLR4 and Roslyn
- Familiarity with compiler design concepts

### Setting Up Development Environment
1. Fork the repository
2. Clone your fork:
   ```bash
   git clone https://github.com/your-username/Antlr4Roslyn.git
   cd Antlr4Roslyn
   ```
3. Build the project:
   ```bash
   dotnet build
   ```
4. Run the demo:
   ```bash
   dotnet run --project DotnetCompiler
   ```

## 🎯 Areas for Contribution

### 1. Language Features
**Priority: High**
- Complete function definition implementation
- Fix lambda expression type inference
- Add array indexing and methods
- Implement for loops
- Add string interpolation

### 2. Grammar Extensions
**Priority: Medium**
- Extend `Simple.g4` with new syntax
- Add pattern matching expressions
- Implement module/import system
- Add more operators (e.g., bitwise, null coalescing)

### 3. Error Handling
**Priority: High**
- Improve parser error messages
- Add semantic analysis validation
- Better error recovery during parsing
- Runtime error handling

### 4. Standard Library
**Priority: Medium**
- Built-in mathematical functions
- String manipulation functions
- Array/collection utilities
- I/O operations

### 5. Tooling & Developer Experience
**Priority: Low**
- REPL (Read-Eval-Print Loop)
- Syntax highlighting for IDEs
- Language server protocol support
- Debugging capabilities

## 📋 Contribution Guidelines

### Code Style
- Follow C# coding conventions
- Use meaningful variable and method names
- Add XML documentation for public APIs
- Keep methods focused and single-purpose

### Grammar Changes
When modifying `Simple.g4`:
1. Test thoroughly with existing examples
2. Update the visitor pattern in `AntlrToRoslynVisitor.cs`
3. Add corresponding test cases
4. Update documentation

### Commit Messages
Use conventional commit format:
```
type(scope): description

Examples:
feat(grammar): add for loop syntax
fix(visitor): handle null expressions in function calls
docs(readme): update syntax examples
test(compiler): add integration tests for arrays
```

### Pull Request Process
1. Create a feature branch from `main`
2. Make your changes with tests
3. Update documentation if needed
4. Submit a pull request with:
   - Clear description of changes
   - Reference to related issues
   - Examples of new functionality

## 🧪 Testing

### Running Tests
```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test Tests/
```

### Adding Tests
- Unit tests for individual components
- Integration tests for language features
- Grammar tests for parsing edge cases
- Performance benchmarks for compiler pipeline

### Test Structure
```
Tests/
├── Grammar/           # ANTLR grammar tests
├── Visitor/          # AST transformation tests
├── Compiler/         # End-to-end compilation tests
└── Language/         # Language feature tests
```

## 🐛 Reporting Issues

### Bug Reports
Please include:
- Minimal code example that reproduces the issue
- Expected vs actual behavior
- System information (.NET version, OS)
- Error messages or stack traces

### Feature Requests
Please include:
- Clear description of the proposed feature
- Use cases and motivation
- Example syntax (if applicable)
- Implementation suggestions (optional)

## 📚 Development Resources

### Understanding the Codebase
1. **`Simple.g4`** - Start here to understand language syntax
2. **`Compiler.cs`** - Main compilation pipeline
3. **`AntlrToRoslynVisitor.cs`** - AST transformation logic
4. **`ProgramGenerator.cs`** - C# code generation

### Key Concepts
- **ANTLR4**: Parser generation from grammar
- **Roslyn**: .NET compiler platform for C# generation
- **Visitor Pattern**: Tree traversal for AST transformation
- **Syntax Trees**: Intermediate representation of code

### Useful Links
- [ANTLR4 Documentation](https://www.antlr.org/)
- [Roslyn API Documentation](https://docs.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/)
- [Compiler Design Principles](https://en.wikipedia.org/wiki/Compiler)

## 🎖️ Recognition

Contributors will be recognized in:
- README.md contributors section
- Release notes for significant contributions
- Special thanks for major feature implementations

## 📞 Getting Help

- **GitHub Issues**: For bug reports and feature requests
- **Discussions**: For questions and general discussion
- **Code Review**: Through pull request comments

## 🏆 Code of Conduct

Please be respectful and constructive in all interactions. We're building this together to learn and create something useful for the community.

---

Thank you for contributing to Simple Programming Language! 🙏
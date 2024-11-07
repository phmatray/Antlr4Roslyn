using Antlr4Roslyn.Services;

const string input = "3 + 5;";
var compiler = new Compiler();
compiler.Compile(input);
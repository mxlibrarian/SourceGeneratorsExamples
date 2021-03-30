using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using SG.InterfaceExample.SourceGenerator;
using Xunit;

namespace SG.InterfaceExample.Tests
{
    public class GeneratorTests
    {
        [Fact]
        public void SimpleGeneratorTest()
        {
            var source = @"

namespace MyTest
{
    public interface XXX { }
    public class Program : XXX
    {
        public static void Main()
        {
            // dummy... for now
            // System.Console.WriteLine(""Hello:"");
        }
    }
}
";
            var comp = CreateCompilation(source);
            var newComp = RunGenerators(comp, out var generatorDiagnostics, new InterfaceSourceGenerator());
            var generatedTrees = newComp.RemoveSyntaxTrees(comp.SyntaxTrees).SyntaxTrees;

            Assert.Empty(generatedTrees);
            Assert.Empty(generatorDiagnostics);
            Assert.Empty(newComp.GetDiagnostics());
        }
        
        private static Compilation CreateCompilation(string source, OutputKind outputKind = OutputKind.ConsoleApplication)
            => CSharpCompilation.Create(nameof(SimpleGeneratorTest),
                new[] { CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Latest)) },
                new[] { MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location) },
                new CSharpCompilationOptions(outputKind));

        private static GeneratorDriver CreateDriver(Compilation c, params ISourceGenerator[] generators) => CSharpGeneratorDriver.Create(generators, parseOptions: (CSharpParseOptions)c.SyntaxTrees.First().Options);

        private static Compilation RunGenerators(Compilation c, out ImmutableArray<Diagnostic> diagnostics, params ISourceGenerator[] generators)
        {
            CreateDriver(c, generators).RunGeneratorsAndUpdateCompilation(c, out var d, out diagnostics);
            return d;
        }
    }
}
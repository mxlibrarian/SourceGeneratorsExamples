using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SG.InterfaceExample.SourceGenerator
{
    public class InterfaceSourceGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new InterfaceSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            //  we need how to find if a Class implements an interface
            if (!(context.SyntaxReceiver is InterfaceSyntaxReceiver receiver))
                return;

            Console.WriteLine("We have to implement something here still" + receiver.Candidates.Count);
        }
    }

    public class InterfaceSyntaxReceiver : ISyntaxReceiver
    {
        public List<ClassDeclarationSyntax> Candidates { get; } = new List<ClassDeclarationSyntax>();

        /// <summary>
        /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
        /// </summary>
        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (!(syntaxNode is ClassDeclarationSyntax classDeclarationSyntax &&
                  classDeclarationSyntax.BaseList != null && classDeclarationSyntax.BaseList.Types.Any()))
                return;

            var isPublicClass = classDeclarationSyntax.Modifiers.Any(s => s.Text == "public");
            if (isPublicClass)
            {
                Candidates.Add(classDeclarationSyntax);
            }
        }
    }
}
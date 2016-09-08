using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Semantics;
using System;

namespace so38724403
{
    internal class Walker : CSharpSyntaxWalker
    {
        public SemanticModel Model { get; set; }

        public override void VisitGotoStatement(GotoStatementSyntax node)
        {
            var o = (IBranchStatement)Model.GetOperation(node);
            var label = o.Target;
            Console.WriteLine(label);
            base.VisitGotoStatement(node);
        }
    }

    internal static class Program
    {
        private static void Main()
        {
            var tree = CSharpSyntaxTree.ParseText(@"
public class Program
{
    public static void Main()
    {
        var i = System.Environment.GetCommandLineArgs().Length;
w1:
        switch (i)
        {
            case 0: break;
            case 1: goto case 0;
            case 2: goto default;
            case 3: goto w2;
            case 4: goto w1;
            default: break;
        }
w2:
        return;
    }
}");
            var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var compilation = CSharpCompilation.Create(null, new[] { tree }, new[] { mscorlib });
            var walker = new Walker { Model = compilation.GetSemanticModel(tree) };
            walker.Visit(tree.GetRoot());
        }
    }
}

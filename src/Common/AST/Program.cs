using System;
using System.Collections.Generic;
using System.Linq;
using ParsingTools;

namespace Common.AST
{
    public class Program
    {
        public IList<FunctionDef> Functions { get; private set; } = new List<FunctionDef>();
        public IList<ClassDef> Classes { get; private set; } = new List<ClassDef>();

        public Program(LatteParser.ProgramContext context)
        {
            context.topDef().ToList().ForEach(topDef =>
            {
                switch (topDef)
                {
                    case LatteParser.FunctionDefContext fDef:
                        Functions.Add(new FunctionDef(fDef));
                        break;
                    case LatteParser.ClassDefContext cDef:
                        Classes.Add(new ClassDef(cDef));
                        break;
                    default:
                        throw new NotSupportedException();
                }
            });
        }

        public Program WithPrefixedFunctions()
        {
            Functions = Functions.Select(f => new FunctionDefPrefixDecorator(f)).ToList<FunctionDef>();
            return this;
        }
    }
}
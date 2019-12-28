using System;
using System.Collections.Generic;
using System.Linq;
using ParsingTools;

namespace Common.AST
{
    public class Program
    {
        public IList<FunctionDef> Functions { get; } = new List<FunctionDef>();

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
                        throw new NotImplementedException();
                    default:
                        throw new NotSupportedException();
                }
            });
        }
    }
}
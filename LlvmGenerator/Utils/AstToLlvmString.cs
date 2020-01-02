using System;
using System.Text;
using Common.AST;
using Common.AST.Exprs;
using LlvmGenerator.StateManagement;
using ParsingTools;

namespace LlvmGenerator.Utils
{
    public static class AstToLlvmString
    {
        public static string Type(LatteParser.TypeContext typeContext)
        {
            return typeContext switch
            {
                LatteParser.TBoolContext boolContext => "i1",
                LatteParser.TIntContext intContext => "i32",
                LatteParser.TStringContext stringContext => "i8*",
                LatteParser.TTypeNameContext typeNameContext => "",
                LatteParser.TVoidContext voidContext => "void",
                _ => throw new ArgumentOutOfRangeException(nameof(typeContext))
            };
        }
        
        public static string Mul(Mul mul)
        {
            return mul switch
            {
                Common.AST.Exprs.Mul.Multiply => "mul",
                Common.AST.Exprs.Mul.Divide => "sdiv",
                Common.AST.Exprs.Mul.Modulo => "srem"
            };
        }

        public static string Rel(Rel rel)
        {
            return rel switch
            {
                Common.AST.Exprs.Rel.LessThan => "slt",
                Common.AST.Exprs.Rel.LessEquals => "sle",
                Common.AST.Exprs.Rel.GreaterThan => "sgt",
                Common.AST.Exprs.Rel.GreaterEquals => "sge",
                Common.AST.Exprs.Rel.Equals => "eq",
                Common.AST.Exprs.Rel.NotEquals => "ne",
            };
        }

        public static string FunctionName(string Id)
        {
            return $"{(LlvmGenerator.ExternalFunctions().Exists(func => func.Id == Id) ? "" : "f")}{Id}";
        }

        public static string FunctionHeader(FunctionDef function, FunctionGeneratorState state)
        {
            return $"define {Type(function.Type)} @{function.Id}({FunctionArgs(function, state)}) " + "{";
        }

        private static string FunctionArgs(FunctionDef function, FunctionGeneratorState state)
        {
            var result = new StringBuilder();
            if (function.Args.Count == 0)
            {
                return result.ToString();
            }

            var isFirst = true;
            function.Args.ForEach(arg =>
            {
                if (!isFirst)
                {
                    result.Append(", ");
                    
                }
                isFirst = false;

                result.Append($"{Type(arg.Type)} {state.VarToLabelToRegister[arg.Id][FunctionGeneratorState.EntryLabel].Register}");
            });

            return result.ToString();
        }
    }
}
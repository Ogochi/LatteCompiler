using System;
using System.Text;
using Common.AST;
using LlvmGenerator.Generators;
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
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    result.Append(", ");
                }

                result.Append($"{Type(arg.Type)} %{state.VarToRegister[arg.Id][0].Register}");
            });

            return result.ToString();
        }
    }
}
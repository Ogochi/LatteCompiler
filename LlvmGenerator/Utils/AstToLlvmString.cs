using System;
using System.Text;
using Common.AST;
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

        public static string FunctionHeader(FunctionDef function)
        {
            return $"define {Type(function.Type)} @{function.Id}({FunctionArgs(function)}) " + "{";
        }

        private static string FunctionArgs(FunctionDef function)
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

                result.Append($"{Type(arg.Type)} %{arg.Id}");
            });

            return result.ToString();
        }
    }
}
using System.Collections.Generic;
using Common.AST;

namespace LlvmGenerator.Generators
{
    public class FunctionGenerator
    {
        public List<string> GenerateFromAst(FunctionDef function)
        {
            var result = new List<string>();
            result.Add(Utils.AstToLlvmString.FunctionHeader(function));
            
            
            
            result.Add("}");
            return result;
        }
    }
}
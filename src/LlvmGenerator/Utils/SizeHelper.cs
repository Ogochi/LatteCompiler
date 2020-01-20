using System;
using System.Linq;
using Common.AST;
using ParsingTools;

namespace LlvmGenerator.Utils
{
    public class SizeHelper
    {
        public int GetClassSize(ClassDef classDef)
        {
            return classDef.Fields.Values.Select(field => GetTypeSize(field.Type)).Sum();
        }
        
        private int GetTypeSize(LatteParser.TypeContext type)
        {
            return type switch
            {
                LatteParser.TBoolContext boolContext => 1,
                LatteParser.TIntContext intContext => 4,
                LatteParser.TStringContext stringContext => 8,
                LatteParser.TTypeNameContext typeNameContext => 8,
                LatteParser.TVoidContext voidContext => throw new NotSupportedException(),
                _ => throw new ArgumentOutOfRangeException(nameof(type))
            };
        }

        
    }
}
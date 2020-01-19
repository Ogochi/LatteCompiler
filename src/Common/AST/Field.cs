using ParsingTools;

namespace Common.AST
{
    public class Field : Arg
    {
        public Field(LatteParser.TypeContext type, string id) : base(type, id)
        {
        }
    }
}
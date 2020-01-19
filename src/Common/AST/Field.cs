using ParsingTools;

namespace Common.AST
{
    public class Field : Arg
    {
        public int Number { get; set; }
        
        public Field(LatteParser.TypeContext type, string id, int number) : base(type, id)
        {
            Number = number;
        }
    }
}
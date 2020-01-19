using ParsingTools;

namespace Common.AST
{
    public class Arg
    {
        public LatteParser.TypeContext Type { get; set; }
        public string Id { get; set; }
        
        public Arg(LatteParser.TypeContext type, string id)
        {
            Type = type;
            Id = id;
        }
    }
}
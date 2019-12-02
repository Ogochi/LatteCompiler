using ParsingTools;

namespace Frontend.StateManagement
{
    public class VarDef
    {
        public LatteParser.TypeContext Type { get; private set; }
        
        public string Name { get; private set; }

        public VarDef(LatteParser.TypeContext type, string name)
        {
            Type = type;
            Name = name;
        }
    }
}
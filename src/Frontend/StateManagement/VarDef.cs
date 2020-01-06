using ParsingTools;

namespace Frontend.StateManagement
{
    public class VarDef
    {
        public LatteParser.TypeContext Type { get; private set; }
        
        public string Name { get; private set; }
        
        public bool IsDefinedInCurrentBlock { get; set; }

        public VarDef(LatteParser.TypeContext type, string name)
        {
            Type = type;
            Name = name;
            IsDefinedInCurrentBlock = true;
        }
        
        public VarDef(LatteParser.TypeContext type, string name, bool isDefinedInCurrentBlock)
        {
            Type = type;
            Name = name;
            IsDefinedInCurrentBlock = isDefinedInCurrentBlock;
        }
    }
}
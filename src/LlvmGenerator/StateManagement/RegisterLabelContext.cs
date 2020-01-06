using ParsingTools;

namespace LlvmGenerator.StateManagement
{
    public class RegisterLabelContext
    {
        public string Register { get; set; }
        public string Label { get; set; }
        
        public LatteParser.TypeContext Type { get; set; }
        
        public RegisterLabelContext(string register, string label, LatteParser.TypeContext type)
        {
            Register = register;
            Label = label;
            Type = type;
        }
    }
}
namespace LlvmGenerator.StateManagement
{
    public class RegisterLabelContext
    {
        public string Register { get; set; }
        public string Label { get; set; }
        
        public RegisterLabelContext(string register, string label)
        {
            Register = register;
            Label = label;
        }
    }
}
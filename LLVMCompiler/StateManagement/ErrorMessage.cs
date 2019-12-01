namespace LLVMCompiler.StateManagement
{
    public class ErrorMessage
    {
        public int Line { get; set; }
        public int Column { get; set; }
        public string Message { get; set; }

        public ErrorMessage(int line, int column, string message)
        {
            Line = line;
            Column = column;
            Message = message;
        }

        public override string ToString()
        {
            return $"Error in line {Line}, column {Column}.\n" + $"Message: {Message}\n\t----\n";
        }
    }
}
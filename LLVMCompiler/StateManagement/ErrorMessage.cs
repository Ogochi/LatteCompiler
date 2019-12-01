namespace LLVMCompiler.StateManagement
{
    public class ErrorMessage
    {
        public int Line { get; set; }
        public int? Column { get; set; }
        public string Message { get; set; }

        public ErrorMessage(int line, int column, string message)
        {
            Line = line;
            Column = column;
            Message = message;
        }
        
        public ErrorMessage(int line, string message)
        {
            Line = line;
            Message = message;
        }

        public override string ToString()
        {
            return Column.HasValue
                ? $"Error in line {Line}, column {Column}.\n" + $"Message: {Message}\n\t----\n"
                : $"Error in line {Line}.\n" + $"Message: {Message}\n\t----\n";
        }
    }
}
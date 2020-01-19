namespace Common.StateManagement
{
    public class ErrorMessage
    {
        public int? Line { get; set; }
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
        
        public ErrorMessage(string message)
        {
            Message = message;
        }

        public override string ToString()
        {
            var prefix = Line.HasValue
                ? $"Line {Line}\n"
                : "";
            return prefix + (Column.HasValue
                ? $"Column {Column}\n" + $"Message: {Message}\n\t----\n"
                : $"Message: {Message}\n\t----\n");
        }
    }
}
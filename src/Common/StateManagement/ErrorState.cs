using System.Collections.Generic;
using System.Linq;

namespace Common.StateManagement
{
    public class ErrorState
    {
        private IList<ErrorMessage> _errorMessages = new List<ErrorMessage>();

        private ErrorState() {}
        
        public static ErrorState Instance { get; } = new ErrorState();

        public void AddErrorMessage(ErrorMessage message) => _errorMessages.Add(message);
        
        public bool isError() => _errorMessages.Count > 0;

        public int errorsCount() => _errorMessages.Count;

        public List<string> GetErrorText() => _errorMessages.Select(msg => msg.ToString()).ToList();
    }
}
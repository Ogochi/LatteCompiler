using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LLVMCompiler.StateManagement
{
    public class ErrorState
    {
        private IList<ErrorMessage> _errorMessages = new List<ErrorMessage>();

        private ErrorState() {}
        
        public static ErrorState Instance { get; } = new ErrorState();

        public void AddErrorMessage(ErrorMessage message) => _errorMessages.Add(message);
        
        public bool isError() => _errorMessages.Count > 0;

        public List<string> GetErrorText() => _errorMessages.Select(msg => msg.ToString()).ToList();
    }
}
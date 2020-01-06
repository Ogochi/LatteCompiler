using Antlr4.Runtime;
using Common.StateManagement;

namespace LLVMCompiler.Utils
{
    public class LexerErrorListener<Symbol> : IAntlrErrorListener<Symbol>
    {
        private readonly ErrorState _errorState = ErrorState.Instance;
        
        public void SyntaxError(IRecognizer recognizer, Symbol offendingSymbol, int line, int charPositionInLine, string msg,
            RecognitionException e)
        {
            _errorState.AddErrorMessage(new ErrorMessage(line, charPositionInLine, msg));
        }
    }
}
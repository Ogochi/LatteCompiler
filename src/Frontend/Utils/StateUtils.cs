using Common.StateManagement;
using Frontend.Exception;

namespace Frontend.Utils
{
    public class StateUtils
    {
        public static void InterruptWithMessage(int line, string message)
        {
            ErrorState.Instance.AddErrorMessage(new ErrorMessage(line, message));
            throw new InterruptedStaticCheckException();
        }
        
        public static void InterruptWithMessage(int line, int column, string message)
        {
            ErrorState.Instance.AddErrorMessage(new ErrorMessage(line, column, message));
            throw new InterruptedStaticCheckException();
        }
    }
}
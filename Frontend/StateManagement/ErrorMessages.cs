namespace Frontend.StateManagement
{
    public class ErrorMessages
    {
        public static string VarAlreadyDefined(string id) => $"Variable '{id}' has been already defined.";
        public static string FuncAlreadyDefined(string id) => $"Function '{id}' has been already defined.";
        public static string VarNotDefined(string id) => $"Variable '{id}' is not defined.";
        public static string FuncNotDefined(string id) => $"Function '{id}' is not defined.";
    }
}
namespace Frontend.StateManagement
{
    public class ErrorMessages
    {
        public static string VarAlreadyDefined(string id) => $"Variable '{id}' has been already defined.";
        public static string FuncAlreadyDefined(string id) => $"Function '{id}' has been already defined.";
        public static string VarNotDefined(string id) => $"Variable '{id}' is not defined.";
        public static string FuncNotDefined(string id) => $"Function '{id}' is not defined.";

        public static string WrongArgsCountFuncCall(string id) =>
            $"Function '{id}' has been called with wrong number of arguments.";

        public static string WrongArgTypeFuncCall(string funcId, string paramId, string paramType) =>
            $"Function '{funcId}' has been called with argument of type other than '{paramType}' for parameter '{paramId}.'";
    }
}
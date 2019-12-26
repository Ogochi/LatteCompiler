namespace Frontend.StateManagement
{
    public class ErrorMessages
    {
        public static string VarAlreadyDefinedMsg(string id) => $"Variable {id} has been already defined.";
        public static string FuncAlreadyDefined(string id) => $"Function {id} has been already defined.";
    }
}
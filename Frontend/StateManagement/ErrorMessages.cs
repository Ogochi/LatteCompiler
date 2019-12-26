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

        public static string RelOpToNotInt = "Relational operators can be used only with integer values.";
        public static string LogicOpToNotBool = "Logic operators can be used only with boolean values.";
        public static string ArithmeticOpToNotInt = "Arithmetic operators can be used only with integer values.";
        public static string UnaryMinusToNotInt = "Unary minus operator can be applied only to integer value.";
        public static string UnaryNegToNotBool = "Unary negation operator can be applied only to boolean value.";
        public static string VarExprTypesMismatch(string id) => $"Types mismatch for variable '{id}' and assigned expression.";
        public static string DecrOnlyOnInt(string id) => $"Tried to decrement '{id}', which is not integer variable.";
        public static string IncrOnlyOnInt(string id) => $"Tried to increment '{id}', which is not integer variable.";
        public static string WhileWrongCondition = "While condition can be only boolean expression.";
        public static string WrongReturn(string badType, string goodType, string funcId) => 
            $"Tried to return expression of type '{badType}' in function '{funcId}' with type '{goodType}'.";

        public static string IfWrongCondition = "If condition can be only boolean expression.";
    }
}
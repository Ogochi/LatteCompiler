namespace Frontend.StateManagement
{
    public class ErrorMessages
    {
        public static string VarAlreadyDefined(string id) => $"Variable '{id}' has been already defined in this block.";
        public static string FuncAlreadyDefined(string id) => $"Function '{id}' has been already defined.";
        public static string VarNotDefined(string id) => $"Variable '{id}' is not defined.";
        public static string FuncNotDefined(string id) => $"Function '{id}' is not defined.";

        public static string WrongArgsCountFuncCall(string id) =>
            $"Function '{id}' has been called with wrong number of arguments.";

        public static string WrongArgTypeFuncCall(string funcId, string paramId, string paramType) =>
            $"Function '{funcId}' has been called with argument of type other than '{paramType}' for parameter '{paramId}'.";

        public static string CompOpToNotInt = "Comparing operators can be used only with integer values.";
        public static string EqOpWrongTypes = "Equality operators can be used only with values of the same type.";
        public static string LogicOpToNotBool = "Logic operators can be used only with boolean values.";
        public static string MinusOpToNotInt = "Minus operator can be used only with integer values.";
        public static string AddOpWrongType = "Add operator can be used only with integer or string values.";

        public static string MulOpToNotInt =
            "Multiply, division and modulo operators cane be used only with integer values.";
        public static string UnaryMinusToNotInt = "Unary minus operator can be applied only to integer value.";
        public static string UnaryNegToNotBool = "Unary negation operator can be applied only to boolean value.";
        public static string VarExprTypesMismatch(string id) => $"Types mismatch for variable '{id}' and assigned expression.";
        public static string DecrOnlyOnInt(string id) => $"Tried to decrement '{id}', which is not integer variable.";
        public static string IncrOnlyOnInt(string id) => $"Tried to increment '{id}', which is not integer variable.";
        public static string WhileWrongCondition = "While condition can be only boolean expression.";
        public static string WrongReturn(string badType, string goodType, string funcId) => 
            $"Tried to return expression of type '{badType}' in function '{funcId}' with type '{goodType}'.";

        public static string IfWrongCondition = "If condition can be only boolean expression.";
        public static string FunctionBranchWithoutRet(string id) => $"There is an execution branch in function '{id}' without return statement.";
        public static string VoidDeclaration = "Variable can't be if type 'void'";
    }
}
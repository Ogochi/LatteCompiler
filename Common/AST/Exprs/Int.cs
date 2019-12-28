using ParsingTools;

namespace Common.AST.Exprs
{
    public class Int : Expr
    {
        public int Value { get; set; }

        public Int(LatteParser.EIntContext context)
        {
            Value = int.Parse(context.INT().GetText());
        }
    }
}
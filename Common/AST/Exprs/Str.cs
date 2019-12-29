using ParsingTools;

namespace Common.AST.Exprs
{
    public class Str : Expr
    {
        public string Value { get; set; }

        public Str()
        {
            Value = "";
        }

        public Str(LatteParser.EStrContext context)
        {
            Value = context.STR().GetText();
        }
    }
}
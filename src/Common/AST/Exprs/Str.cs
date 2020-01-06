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
            var str = context.STR().GetText();
            Value = str.Substring(1, str.Length - 2);
        }
        
        public override Result Accept<Result>(BaseExprAstVisitor<Result> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
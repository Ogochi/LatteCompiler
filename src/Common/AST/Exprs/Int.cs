using ParsingTools;

namespace Common.AST.Exprs
{
    public class Int : Expr
    {
        public int Value { get; set; }

        public Int()
        {
            Value = 0;
        }

        public Int(LatteParser.EIntContext context)
        {
            Value = int.Parse(context.INT().GetText());
        }
        
        public override Result Accept<Result>(BaseExprAstVisitor<Result> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
namespace Common.AST.Exprs
{
    public class Null : Expr
    {
        public override Result Accept<Result>(BaseExprAstVisitor<Result> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
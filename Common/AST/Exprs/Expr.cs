namespace Common.AST.Exprs
{
    public abstract class Expr
    {
        public abstract Result Accept<Result>(BaseExprAstVisitor<Result> visitor);
    }
}
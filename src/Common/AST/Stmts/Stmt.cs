namespace Common.AST.Stmts
{
    public abstract class Stmt
    {
        public abstract void Accept(BaseAstVisitor visitor);
    }
}
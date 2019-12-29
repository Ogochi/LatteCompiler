namespace Common.AST.Stmts
{
    public class Empty : Stmt
    {
        public override void Accept(BaseAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
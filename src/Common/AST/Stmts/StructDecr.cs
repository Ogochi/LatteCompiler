using Common.AST.Exprs;
using ParsingTools;

namespace Common.AST.Stmts
{
    public class StructDecr : Stmt
    {
        public Expr IdExpr { get; set; }

        public StructDecr(LatteParser.StructDecrContext context)
        {
            IdExpr = Exprs.Utils.ExprFromExprContext(context.expr());;
        }
        
        public override void Accept(BaseAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
using Common.AST.Exprs;
using ParsingTools;

namespace Common.AST.Stmts
{
    public class StructIncr : Stmt
    {
        public Expr IdExpr { get; set; }

        public StructIncr(LatteParser.StructIncrContext context)
        {
            IdExpr = Exprs.Utils.ExprFromExprContext(context.expr());;
        }
        
        public override void Accept(BaseAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
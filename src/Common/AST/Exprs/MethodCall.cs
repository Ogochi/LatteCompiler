using System.Collections.Generic;
using System.Linq;
using ParsingTools;

namespace Common.AST.Exprs
{
    public class MethodCall : Expr
    {
        public Expr IdExpr { get; set; }
        public IList<Expr> Exprs { get; } = new List<Expr>();

        public MethodCall(LatteParser.EMethodCallContext context)
        {
            IdExpr = Utils.ExprFromExprContext(context.expr()[0]);
            context.expr().ToList().Skip(1).ToList().ForEach(expr => Exprs.Add(Utils.ExprFromExprContext(expr)));
        }
        
        public override Result Accept<Result>(BaseExprAstVisitor<Result> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
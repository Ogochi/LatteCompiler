using System.Collections.Generic;
using System.Linq;
using ParsingTools;

namespace Common.AST.Exprs
{
    public class FunCall : Expr
    {
        public string Id { get; set; }
        public IList<Expr> Exprs { get; set; } = new List<Expr>();

        public FunCall()
        {
        }

        public FunCall(LatteParser.EFunCallContext context)
        {
            Id = context.ID().GetText();
            context?.expr().ToList().ForEach(expr => Exprs.Add(Utils.ExprFromExprContext(expr)));
        }
        
        public override Result Accept<Result>(BaseExprAstVisitor<Result> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
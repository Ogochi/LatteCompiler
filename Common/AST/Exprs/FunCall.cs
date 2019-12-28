using System.Collections.Generic;
using System.Linq;
using ParsingTools;

namespace Common.AST.Exprs
{
    public class FunCall : Expr
    {
        public string Id { get; set; }
        public IList<Expr> Exprs { get; } = new List<Expr>();

        public FunCall(LatteParser.EFunCallContext context)
        {
            Id = context.ID().GetText();
            context?.expr().ToList().ForEach(expr => Exprs.Add(Utils.ExprFromExprContext(expr)));
        }
    }
}
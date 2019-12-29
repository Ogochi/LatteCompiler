using Common.AST.Exprs;
using ParsingTools;

namespace Common.AST
{
    public class Item
    {
        public string Id { get; set; }
        public Expr Expr { get; set; }

        public Item(LatteParser.ItemContext context)
        {
            Id = context.ID().GetText();
            Expr = Utils.ExprFromExprContext(context.expr());
        }
    }
}
using ParsingTools;

namespace Common.AST
{
    public class Item
    {
        public string Id { get; set; }
        public LatteParser.ExprContext expr { get; set; }

        public Item(LatteParser.ItemContext context)
        {
            Id = context.ID().GetText();
            expr = context.expr();
        }
    }
}
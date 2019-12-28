using ParsingTools;

namespace Common.AST.Exprs
{
    public class ID : Expr
    {
        public string Id { get; set; }

        public ID(LatteParser.EIdContext context)
        {
            Id = context.ID().GetText();
        }
    }
}
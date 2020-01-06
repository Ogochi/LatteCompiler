using ParsingTools;

namespace Common.AST.Exprs
{
    public class ID : Expr
    {
        public string Id { get; set; }

        public ID()
        {
        }

        public ID(LatteParser.EIdContext context)
        {
            Id = context.ID().GetText();
        }
        
        public override Result Accept<Result>(BaseExprAstVisitor<Result> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
using ParsingTools;

namespace Common.AST.Exprs
{
    public class NewObject : Expr
    {
        public LatteParser.TypeContext Type { get; set; }

        public NewObject(LatteParser.ENewObjectContext context)
        {
            Type = context.type();
        }
        
        public override Result Accept<Result>(BaseExprAstVisitor<Result> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
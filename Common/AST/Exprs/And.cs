using ParsingTools;

namespace Common.AST.Exprs
{
    public class And : TwoHandOperation
    {
        public And()
        {
        }

        public And(LatteParser.EAndContext context) :
            base(context.expr()[0], context.expr()[1])
        {
        }
        
        public override Result Accept<Result>(BaseExprAstVisitor<Result> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
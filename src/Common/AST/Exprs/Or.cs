using ParsingTools;

namespace Common.AST.Exprs
{
    public class Or : TwoHandOperation
    {
        public Or()
        {
        }

        public Or(LatteParser.EOrContext context) :
            base(context.expr()[0], context.expr()[1])
        {
        }
        
        public override Result Accept<Result>(BaseExprAstVisitor<Result> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
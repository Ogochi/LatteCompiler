using ParsingTools;

namespace Common.AST.Exprs
{
    public class And : TwoHandOperation
    {
        public And(LatteParser.EAndContext context) :
            base(context.expr()[0], context.expr()[1])
        {
        }
    }
}
using ParsingTools;

namespace Common.AST.Exprs
{
    public class Or : TwoHandOperation
    {
        public Or(LatteParser.EOrContext context) :
            base(context.expr()[0], context.expr()[1])
        {
        }
    }
}
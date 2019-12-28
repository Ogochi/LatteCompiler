using ParsingTools;

namespace Common.AST.Exprs
{
    public class RelOp : TwoHandOperation
    {
        public Rel Rel { get; set; }

        public RelOp(LatteParser.ERelOpContext context) :
            base(context.expr()[0], context.expr()[1])
        {
            Rel = context.relOp() switch
            {
                LatteParser.LessThanContext _ => Rel.LessThan,
                LatteParser.LessEqualsContext _ => Rel.LessEquals,
                LatteParser.GreaterThanContext _ => Rel.GreaterThan,
                LatteParser.GreaterEqualsContext _ => Rel.GreaterEquals,
                LatteParser.EqualsContext _ => Rel.Equals,
                LatteParser.NotEqualsContext _ => Rel.NotEquals
            };
        }
    }
}
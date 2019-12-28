using ParsingTools;

namespace Common.AST.Exprs
{
    public class AddOp : TwoHandOperation
    {
        public Add Add { get; set; }

        public AddOp(LatteParser.EAddOpContext context) : 
            base(context.expr()[0], context.expr()[1])
        {
            Add = context.addOp() switch
            {
                LatteParser.PlusContext _ => Add.Plus,
                LatteParser.MinusContext _ => Add.Minus
            };
        }
    }
}
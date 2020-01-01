using ParsingTools;

namespace Common.AST.Exprs
{
    public class MulOp : TwoHandOperation
    {
        public Mul Mul { get; set; }

        public MulOp(LatteParser.EMulOpContext context) : 
            base(context.expr()[0], context.expr()[1])
        {
            Mul = context.mulOp() switch
            {
                LatteParser.MultiplyContext _ => Mul.Multiply,
                LatteParser.DivideContext _ => Mul.Divide,
                LatteParser.ModuloContext _ => Mul.Modulo
            };
        }
        
        public override Result Accept<Result>(BaseExprAstVisitor<Result> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
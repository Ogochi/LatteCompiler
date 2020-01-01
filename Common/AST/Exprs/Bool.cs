using ParsingTools;

namespace Common.AST.Exprs
{
    public class Bool : Expr
    {
        public bool Value { get; set; }

        public Bool()
        {
            Value = false;
        }

        public Bool(LatteParser.ETrueContext _)
        {
            Value = true;
        }

        public Bool(LatteParser.EFalseContext _)
        {
            Value = false;
        }
        
        public override Result Accept<Result>(BaseExprAstVisitor<Result> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
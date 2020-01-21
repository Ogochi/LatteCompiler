using ParsingTools;

namespace Common.AST.Exprs
{
    public class Null : Expr
    {
        public LatteParser.TTypeNameContext Type { get; set; }
        
        public override Result Accept<Result>(BaseExprAstVisitor<Result> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
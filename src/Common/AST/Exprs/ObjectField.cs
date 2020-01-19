using ParsingTools;

namespace Common.AST.Exprs
{
    public class ObjectField : Expr
    {
        public Expr IdExpr { get; set; }
        public string FieldId { get; }

        public ObjectField(LatteParser.EObjectFieldContext context)
        {
            IdExpr = Utils.ExprFromExprContext(context.expr());
            FieldId = context.ID().GetText();
        }
        
        public override Result Accept<Result>(BaseExprAstVisitor<Result> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
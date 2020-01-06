using ParsingTools;

namespace Common.AST.Stmts
{
    public class Decr : Stmt
    {
        public string Id { get; set; }

        public Decr(LatteParser.DecrContext context)
        {
            Id = context.ID().GetText();
        }
        
        public override void Accept(BaseAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
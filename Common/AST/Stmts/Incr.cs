using ParsingTools;

namespace Common.AST.Stmts
{
    public class Incr : Stmt
    {
        public string Id { get; set; }

        public Incr(LatteParser.IncrContext context)
        {
            Id = context.ID().GetText();
        }
    }
}
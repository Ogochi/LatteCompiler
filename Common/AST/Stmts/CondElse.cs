using Common.AST.Exprs;
using ParsingTools;

namespace Common.AST.Stmts
{
    public class CondElse : Stmt
    {
        public Expr Expr { get; set; }
        public Block TBlock { get; set; }
        public Block FBlock { get; set; }

        public CondElse(LatteParser.CondElseContext context)
        {
            Expr = Exprs.Utils.ExprFromExprContext(context.expr());
            
            var stmt1 = Utils.StmtFromStmtContext(context.stmt()[0]);
            TBlock = stmt1 switch
            {
                Block block => block,
                Stmt s => new Block(s)
            };
            var stmt2 = Utils.StmtFromStmtContext(context.stmt()[1]);
            FBlock = stmt2 switch
            {
                Block block => block,
                Stmt s => new Block(s)
            };
        }
        
        public override void Accept(BaseAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
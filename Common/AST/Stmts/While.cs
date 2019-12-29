using System;
using Common.AST.Exprs;
using ParsingTools;

namespace Common.AST.Stmts
{
    public class While : Stmt
    {
        public Expr Expr { get; set; }
        public Block Block { get; set; }

        public While(LatteParser.WhileContext context)
        {
            Expr = Exprs.Utils.ExprFromExprContext(context.expr());
            
            var stmt = Utils.StmtFromStmtContext(context.stmt());
            Block = stmt switch
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
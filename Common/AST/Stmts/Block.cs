using System.Collections.Generic;
using System.Linq;
using ParsingTools;

namespace Common.AST.Stmts
{
    public class Block : Stmt
    {
        public IList<Stmt> Stmts { get; } = new List<Stmt>();

        public Block(LatteParser.BlockContext context)
        {
            context?.stmt().ToList().ForEach(stmt => Stmts.Add(Utils.StmtFromStmtContext(stmt)));
        }
        
        public Block(LatteParser.BlockStmtContext context) : this(context.block())
        {
        }
    }
}
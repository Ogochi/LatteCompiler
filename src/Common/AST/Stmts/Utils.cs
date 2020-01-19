using System;
using ParsingTools;

namespace Common.AST.Stmts
{
    public static class Utils
    {
        public static Stmt StmtFromStmtContext(LatteParser.StmtContext context)
        {
            return context switch
            {
                LatteParser.AssContext assContext => new Ass(assContext),
                LatteParser.BlockStmtContext blockStmtContext => new Block(blockStmtContext),
                LatteParser.CondContext condContext => new Cond(condContext),
                LatteParser.CondElseContext condElseContext => new CondElse(condElseContext),
                LatteParser.DeclContext declContext => new Decl(declContext),
                LatteParser.DecrContext decrContext => new Decr(decrContext),
                LatteParser.EmptyContext _ => new Empty(),
                LatteParser.IncrContext incrContext => new Incr(incrContext),
                LatteParser.RetContext retContext => new Ret(retContext),
                LatteParser.SExpContext sExpContext => new ExpStmt(sExpContext),
                LatteParser.StructAssContext structAssContext => new StructAss(structAssContext),
                LatteParser.StructDecrContext structDecrContext => new StructDecr(structDecrContext),
                LatteParser.StructIncrContext structIncrContext => new StructIncr(structIncrContext),
                LatteParser.VRetContext vRetContext => new Ret(vRetContext),
                LatteParser.WhileContext whileContext => new While(whileContext),
                _ => throw new ArgumentOutOfRangeException(nameof(context))
            };
        }
    }
}
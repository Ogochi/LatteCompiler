using System;
using Common.AST.Stmts;

namespace Common.AST
{
    public abstract class BaseAstVisitor
    {
        public void Visit(Stmt stmt)
        {
            stmt.Accept(this);
        }

        public virtual void Visit(Block block) {}

        public virtual void Visit(ExpStmt expStmt) {}

        public virtual void Visit(Decl decl) {}

        public virtual void Visit(Ass ass) {}

        public virtual void Visit(Ret ret) {}
        
        public virtual void Visit(Cond cond) {}
        
        public virtual void Visit(CondElse condElse) {}
        
        public virtual void Visit(Decr decr) {}
        
        public virtual void Visit(Empty empty) {}
        
        public virtual void Visit(Incr incr) {}
        
        public virtual void Visit(While @while) {}
        public virtual void Visit(StructAss structAss) {}
    }
}
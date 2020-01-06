namespace Common.AST.Exprs
{
    public class BaseExprAstVisitor<Result>
    {
        public Result Visit(Expr expr)
        {
            return expr.Accept(this);
        }
        
        public virtual Result Visit(AddOp addOp) { return default(Result); }
        
        public virtual Result Visit(And and) { return default(Result); }

        public virtual Result Visit(Bool @bool) { return default(Result); }
        
        public virtual Result Visit(FunCall funCall) { return default(Result); }
        
        public virtual Result Visit(ID id) { return default(Result); }
        
        public virtual Result Visit(Int @int) { return default(Result); }
        
        public virtual Result Visit(MulOp mulOp) { return default(Result); }
        
        public virtual Result Visit(Or or) { return default(Result); }
        
        public virtual Result Visit(RelOp relOp) { return default(Result); }
        
        public virtual Result Visit(Str str) { return default(Result); }
        
        public virtual Result Visit(UnOp unOp) { return default(Result); }
    }
}
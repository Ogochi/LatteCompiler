using System.Linq;
using Common.StateManagement;
using ParsingTools;

namespace Frontend.ContextVisitor
{
    public class ReturnsCheckVisitor : LatteBaseVisitor<bool>
    {
        private readonly ErrorState _errorState = ErrorState.Instance;
        
        public override bool VisitEmpty(LatteParser.EmptyContext context)
        {
            return false;
        }

        public override bool VisitAss(LatteParser.AssContext context)
        {
            return false;
        }

        public override bool VisitCondElse(LatteParser.CondElseContext context)
        {
            return context.expr() switch
            {
                LatteParser.ETrueContext _ => Visit(context.stmt()[0]),
                LatteParser.EFalseContext _ => Visit(context.stmt()[1]),
                _ => (Visit(context.stmt()[0]) && Visit(context.stmt()[1]))
            };
        }

        public override bool VisitCond(LatteParser.CondContext context)
        {
            return (context.expr() is LatteParser.ETrueContext) && Visit(context.stmt());
        }

        public override bool VisitWhile(LatteParser.WhileContext context)
        {
            return Visit(context.stmt());
        }

        public override bool VisitBlock(LatteParser.BlockContext context)
        {
            return context.stmt().ToList().Select(Visit).Aggregate(false, (x, y) => x || y);
        }

        public override bool VisitRet(LatteParser.RetContext context)
        {
            return true;
        }

        public override bool VisitVRet(LatteParser.VRetContext context)
        {
            return true;
        }

        public override bool VisitStructDecr(LatteParser.StructDecrContext context)
        {
            return false;
        }

        public override bool VisitSExp(LatteParser.SExpContext context)
        {
            return false;
        }

        public override bool VisitStructIncr(LatteParser.StructIncrContext context)
        {
            return false;
        }

        public override bool VisitStructAss(LatteParser.StructAssContext context)
        {
            return false;
        }

        public override bool VisitDecl(LatteParser.DeclContext context)
        {
            return false;
        }

        public override bool VisitDecr(LatteParser.DecrContext context)
        {
            return false;
        }

        public override bool VisitIncr(LatteParser.IncrContext context)
        {
            return false;
        }
    }
}
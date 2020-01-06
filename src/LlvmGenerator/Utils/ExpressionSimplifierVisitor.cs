using System;
using Common.AST.Exprs;

namespace LlvmGenerator.Utils
{
    public class ExpressionSimplifierVisitor : BaseExprAstVisitor<Expr>
    {
        public override Expr Visit(AddOp addOp)
        {
            return (addOp.Add, Visit(addOp.Lhs), Visit(addOp.Rhs)) switch
            {
                (Add.Plus, Int i1, Int i2) => new Int {Value = i1.Value + i2.Value},
                (Add.Minus, Int i1, Int i2) => new Int {Value = i1.Value - i2.Value},
                (Add.Plus, Str s1, Str s2) => new Str {Value = s1.Value + s2.Value},
                (_, Expr e1, Expr e2) => new AddOp {Add = addOp.Add, Lhs = e1, Rhs = e2}
            };
        }

        public override Expr Visit(And and)
        {
            return (Visit(and.Lhs), Visit(and.Rhs)) switch
            {
                (Bool b1, Bool b2) => new Bool {Value = b1.Value && b2.Value},
                (Expr e1, Expr e2) => new And {Lhs = e1, Rhs = e2}
            };
        }

        public override Expr Visit(Bool @bool)
        {
            return @bool;
        }

        public override Expr Visit(FunCall funCall)
        {
            return funCall;
        }

        public override Expr Visit(ID id)
        {
            return id;
        }

        public override Expr Visit(Int @int)
        {
            return @int;
        }

        public override Expr Visit(MulOp mulOp)
        {
            return (mulOp.Mul, Visit(mulOp.Lhs), Visit(mulOp.Rhs)) switch
            {
                (Mul.Multiply, Int i1, Int i2) => new Int {Value = i1.Value * i2.Value},
                (Mul.Divide, Int i1, Int i2) => new Int {Value = i1.Value / i2.Value},
                (Mul.Modulo, Int i1, Int i2) => new Int {Value = i1.Value % i2.Value},
                (_, Expr e1, Expr e2) => new MulOp {Mul = mulOp.Mul, Lhs = e1, Rhs = e2}
            };
        }

        public override Expr Visit(Or or)
        {
            return (Visit(or.Lhs), Visit(or.Rhs)) switch
            {
                (Bool b1, Bool b2) => new Bool {Value = b1.Value || b2.Value},
                (Expr e1, Expr e2) => new Or {Lhs = e1, Rhs = e2}
            };
        }

        public override Expr Visit(RelOp relOp)
        {
            return (relOp.Rel, Visit(relOp.Lhs), Visit(relOp.Rhs)) switch
            {
                (Rel.Equals, Int i1, Int i2) => new Bool {Value = i1.Value == i2.Value},
                (Rel.Equals, Str s1, Str s2) => new Bool {Value = s1.Value == s2.Value},
                (Rel.Equals, Bool b1, Bool b2) => new Bool {Value = b1.Value == b2.Value},
                (Rel.Equals, ID i1, ID i2) => i1.Id == i2.Id ? (Expr)new Bool {Value = true} : new RelOp {Rel = relOp.Rel, Lhs = i1, Rhs = i2},
                (Rel.NotEquals, Int i1, Int i2) => new Bool {Value = i1.Value != i2.Value},
                (Rel.NotEquals, Str s1, Str s2) => new Bool {Value = s1.Value != s2.Value},
                (Rel.NotEquals, Bool b1, Bool b2) => new Bool {Value = b1.Value != b2.Value},
                (Rel.NotEquals, ID i1, ID i2) => i1.Id == i2.Id ? (Expr)new Bool {Value = false} : new RelOp {Rel = relOp.Rel, Lhs = i1, Rhs = i2},
                (Rel.GreaterEquals, Int i1, Int i2) => new Bool {Value = i1.Value >= i2.Value},
                (Rel.GreaterEquals, ID i1, ID i2) => i1.Id == i2.Id ? (Expr)new Bool {Value = true} : new RelOp {Rel = relOp.Rel, Lhs = i1, Rhs = i2},
                (Rel.GreaterThan, Int i1, Int i2) => new Bool {Value = i1.Value > i2.Value},
                (Rel.GreaterThan, ID i1, ID i2) => i1.Id == i2.Id ? (Expr)new Bool {Value = false} : new RelOp {Rel = relOp.Rel, Lhs = i1, Rhs = i2},
                (Rel.LessEquals, Int i1, Int i2) => new Bool {Value = i1.Value <= i2.Value},
                (Rel.LessEquals, ID i1, ID i2) => i1.Id == i2.Id ? (Expr)new Bool {Value = true} : new RelOp {Rel = relOp.Rel, Lhs = i1, Rhs = i2},
                (Rel.LessThan, Int i1, Int i2) => new Bool {Value = i1.Value < i2.Value},
                (Rel.LessThan, ID i1, ID i2) => i1.Id == i2.Id ? (Expr)new Bool {Value = false} : new RelOp {Rel = relOp.Rel, Lhs = i1, Rhs = i2},
                (_, Expr e1, Expr e2) => new RelOp {Rel = relOp.Rel, Lhs = e1, Rhs = e2}
            };
        }

        public override Expr Visit(Str str)
        {
            return str;
        }

        public override Expr Visit(UnOp unOp)
        {
            return (unOp.Operator, Visit(unOp.Expr)) switch
            {
                (Unary.Minus, Int i) => new Int {Value = -i.Value },
                (Unary.Neg, Bool b) => new Bool {Value = !b.Value},
                (_, Expr e) => new UnOp {Operator = unOp.Operator, Expr = e}
            };
        }
    }
}
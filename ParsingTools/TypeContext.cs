using Antlr4.Runtime;

namespace ParsingTools
{
    public partial class TypeContext : ParserRuleContext {
        protected bool Equals(TypeContext other)
        {
            return other.GetText() == GetText();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TypeContext) obj);
        }

        public override int GetHashCode()
        {
            throw new System.NotImplementedException();
        }

        public static bool operator ==(ParsingTools.TypeContext t1, ParsingTools.TypeContext t2)
        {
            return t1?.Equals(t2) ?? ReferenceEquals(t2, null);
        }
        
        public static bool operator !=(ParsingTools.TypeContext t1, ParsingTools.TypeContext t2)
        {
            return !(t1 == t2);
        }
    }
}
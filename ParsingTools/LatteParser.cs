using Antlr4.Runtime;

namespace ParsingTools
{
    public partial class LatteParser
    {
        public partial class TypeContext {

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj.GetType() == GetType();
            }

            public override int GetHashCode()
            {
                throw new System.NotImplementedException();
            }
        }
        
        public partial class TStringContext
        {
            public TStringContext() {}
        }
        
        public partial class TIntContext
        {
            public TIntContext() {}
        }

        public partial class TBoolContext
        {
            public TBoolContext() {}
        }
    }
}
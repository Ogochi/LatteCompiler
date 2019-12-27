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
                if (obj.GetType() != GetType()) return false;

                if (obj is TTypeNameContext context)
                {
                    return (this as TTypeNameContext).ID().GetText() == context.ID().GetText();
                }
                return true;
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

        public partial class TVoidContext
        {
            public TVoidContext() {}
        }
    }
}
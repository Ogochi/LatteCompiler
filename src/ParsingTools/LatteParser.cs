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

            public override string GetText()
            {
                return "string";
            }

            public override string ToString()
            {
                return "string";
            }
        }
        
        public partial class TIntContext
        {
            public TIntContext() {}
            
            public override string GetText()
            {
                return "int";
            }
            
            public override string ToString()
            {
                return "int";
            }
        }

        public partial class TBoolContext
        {
            public TBoolContext() {}
            
            public override string GetText()
            {
                return "bool";
            }
            
            public override string ToString()
            {
                return "bool";
            }
        }

        public partial class TVoidContext
        {
            public TVoidContext() {}
            
            public override string GetText()
            {
                return "void";
            }
            
            public override string ToString()
            {
                return "void";
            }
        }

        public partial class TTypeNameContext
        {
            private readonly string _typeName;
            
            public TTypeNameContext(string typeName)
            {
                _typeName = typeName;
            }
            
            public override string GetText()
            {
                return _typeName;
            }
            
            public override string ToString()
            {
                return _typeName;
            }
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using ParsingTools;

namespace Common.AST
{
    public class FunctionDef
    {
        public LatteParser.TypeContext Type { get; set; }
        public string Id { get; set; }
        public Block Block { get; set; }
        public List<Arg> Args { get; set; }

        public FunctionDef(LatteParser.FunctionDefContext context)
        {
            Type = context.type();
            Id = context.ID().GetText();

            var arg = context.arg();
            if (arg == null)
            {
                Args = new List<Arg>();
            }
            else
            {
                var idType = arg.ID().Zip(arg.type(), (id, type) => (id, type));
                Args = idType.Select(x => new Arg(x.type, x.id.GetText())).ToList();
            }
        }
        
        public FunctionDef() {}
    }
}
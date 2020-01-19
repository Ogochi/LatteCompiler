using System.Collections.Generic;
using System.Linq;
using Common.AST.Stmts;
using ParsingTools;

namespace Common.AST
{
    public class FunctionDef
    {
        public LatteParser.TypeContext Type { get; set; }
        public virtual string Id { get; set; }
        public Block Block { get; set; }
        public List<Arg> Args { get; set; }

        public FunctionDef(LatteParser.FunctionDefContext context)
        {
            Type = context.type();
            Id = context.ID().GetText();
            Block = new Block(context.block());

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

        public FunctionDef(LatteParser.MethodDefContext context)
        {
            Type = context.type();
            Id = context.ID().GetText();
            Block = new Block(context.block());
            
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
using System.Collections.Generic;
using System.Linq;
using ParsingTools;

namespace Common.AST.Stmts
{
    public class Decl : Stmt
    {
        public LatteParser.TypeContext Type { get; set; }
        public IList<Item> Items { get; } = new List<Item>();

        public Decl(LatteParser.DeclContext context)
        {
            Type = context.type();
            context.item().ToList().ForEach(item => Items.Add(new Item(item)));
        }
    }
}
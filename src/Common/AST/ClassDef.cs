using System.Collections.Generic;
using System.Linq;
using ParsingTools;

namespace Common.AST
{
    public class ClassDef
    {
        public virtual string Id { get; set; }
        public string ParentId { get; set; }
        
        public Dictionary<string, Field> Fields { get; set; } = new Dictionary<string, Field>();
        
        public Dictionary<string, (FunctionDef, int)> Methods { get; set; } = new Dictionary<string, (FunctionDef, int)>();

        public int OwnFieldsStartIndex = 0;
        
        public ClassDef(LatteParser.ClassDefContext context)
        {
            Id = context.ID()[0].GetText();
            ParentId = context.ID().Length > 1 ? context.ID()[1].GetText() : null;

            int fieldCounter = 0, methodCounter = 0;
            foreach (var fieldOrMethod in context.fieldOrMethodDef())
            {
                switch (fieldOrMethod)
                {
                    case LatteParser.ClassFieldDefContext fields:
                        fields.fieldDef().ID().ToList()
                            .ForEach(field =>
                            {
                                Fields.Add(field.GetText(), new Field(fields.fieldDef().type(), field.GetText(), fieldCounter++));
                            });
                        break;
                    
                    case LatteParser.ClassMethodDefContext method:
                        Methods.Add(method.methodDef().ID().GetText(), (new FunctionDef(method.methodDef()) {ClassName = Id}, methodCounter++));
                        break;
                }
            }
        }

        public void AddParentFields(List<Field> fields)
        {
            OwnFieldsStartIndex += fields.Count;
            Fields.Values.ToList().ForEach(field => field.Number += fields.Count);
            
            fields.ForEach(field => Fields[field.Id] = field);
        }

        public void AddParentMethods(List<(FunctionDef, int)> methods)
        {
            Methods.ToList().ForEach(pair => Methods[pair.Key] = (pair.Value.Item1, pair.Value.Item2 + methods.Count));
            
            methods.ForEach(method =>
            {
                if (!Methods.ContainsKey(method.Item1.RealName))
                {
                    Methods[method.Item1.Id] = method;
                }
            });
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using Common.AST;
using LlvmGenerator.Utils;

namespace LlvmGenerator.StateManagement
{
    public class FunctionsGlobalState
    {
        public Dictionary<string, FunctionDef> NameToFunction { get; } = new Dictionary<string, FunctionDef>();
        
        public Dictionary<string, ClassDef> NameToClass { get; } = new Dictionary<string, ClassDef>();
        
        public readonly Dictionary<string, string> LiteralToStringConstId= new Dictionary<string, string>();

        public string currentClass;

        public ClassDef CurrentClass => NameToClass[currentClass];
        
        public static FunctionsGlobalState Instance { get; } = new FunctionsGlobalState();
        
        private int _stringCounter;

        private FunctionsGlobalState() {}

        public void AddFunctions(IList<FunctionDef> functions)
        {
            functions.ToList().ForEach(function => NameToFunction[function.Id] = function);
        }

        public void AddClasses(IList<ClassDef> classes)
        {
            classes.ToList().ForEach(@class => NameToClass[@class.Id] = @class);
        }
        
        public string NewString => $"@.str{_stringCounter++}";

        public void AddParentFields(List<ClassDef> classes)
        {
            new ClassHelper().TopoSortClasses(classes).ForEach(@class =>
            {
                if (@class.ParentId == null)
                {
                    return;
                }

                @class.AddParentFields(NameToClass[@class.ParentId].Fields.Values.ToList());
            });
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Common.AST;
using LlvmGenerator.StateManagement;
using ParsingTools;

namespace LlvmGenerator.Utils
{
    public class ClassHelper
    {
        private readonly FunctionsGlobalState _globalState = FunctionsGlobalState.Instance;
        
        public int GetClassSize(ClassDef classDef)
        {
            return classDef.Fields.Values.Select(field => GetTypeSize(field.Type)).Sum();
        }

        public List<ClassDef> TopoSortClasses(List<ClassDef> classDefs)
        {
            var classNodes = GetClassNodes(classDefs);
            var noParentClassNodes= classNodes.Where(node => node.classDef.ParentId == null).ToList();

            var stack = new Stack<ClassNode>(noParentClassNodes);
            var visited = new HashSet<string>();
            var result = new List<ClassNode>();
            while (stack.Count > 0)
            {
                var node = stack.Peek();
                if (visited.Contains(node.classDef.Id) || node.children.Count == 0)
                {
                    result.Add(node);
                    stack.Pop();
                    continue;
                }
                
                node.children.ForEach(stack.Push);
                visited.Add(node.classDef.Id);
            }

            return result.Select(node => node.classDef).Reverse().ToList();
        }

        private List<ClassNode> GetClassNodes(List<ClassDef> classDefs)
        {
            var nameToClassNode = new Dictionary<string, ClassNode>();
            classDefs.ForEach(@class => nameToClassNode[@class.Id] = new ClassNode(@class));
            nameToClassNode.Values.ToList().ForEach(classNode =>
            {
                var @class = classNode.classDef;
                if (@class.ParentId == null)
                {
                    return;
                }
                nameToClassNode[@class.ParentId].children.Add(classNode);
            });

            return nameToClassNode.Values.ToList();
        }
        
        private int GetTypeSize(LatteParser.TypeContext type)
        {
            return type switch
            {
                LatteParser.TBoolContext boolContext => 1,
                LatteParser.TIntContext intContext => 4,
                LatteParser.TStringContext stringContext => 8,
                LatteParser.TTypeNameContext typeNameContext => 8,
                LatteParser.TVoidContext voidContext => throw new NotSupportedException(),
                _ => throw new ArgumentOutOfRangeException(nameof(type))
            };
        }

        private class ClassNode
        {
            public ClassDef classDef { get; set; }
            public List<ClassNode> children { get; } = new List<ClassNode>();

            public ClassNode(ClassDef classDef)
            {
                this.classDef = classDef;
            }
        }
    }
}
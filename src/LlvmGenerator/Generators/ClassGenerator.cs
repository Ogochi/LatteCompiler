using System.Linq;
using Common.AST;
using LlvmGenerator.StateManagement;
using LlvmGenerator.Utils;
using ParsingTools;

namespace LlvmGenerator.Generators
{
    public class ClassGenerator
    {
        private LlvmGenerator _llvmGenerator = LlvmGenerator.Instance;
        private FunctionsGlobalState _globalState = FunctionsGlobalState.Instance;
        
        public void GenerateType(ClassDef @class)
        {
            _llvmGenerator.Emit($"%{@class.Id} = type " + "{");

            var isFirst = true;
            @class.Fields.ToList().ForEach(field =>
                {
                    _llvmGenerator.Emit($"{(isFirst ? " " : ",")}{AstToLlvmString.Type(field.Value.Type)}");
                    isFirst = false;
                });
            
            _llvmGenerator.Emit("}");
        }

        public void GenerateConstructor(ClassDef @classDef)
        {
            _llvmGenerator.Emit($"define void @g_{@classDef.Id}_construct(%{@classDef.Id}* %this) " + "{");
            
            if (!_globalState.LiteralToStringConstId.ContainsKey(""))
            {
                _globalState.LiteralToStringConstId[""] = _globalState.NewString;
            }
            var emptyStr = _globalState.LiteralToStringConstId[""];
            
            int register = 0, counter = @classDef.OwnFieldsStartIndex;
            if (counter > 0)
            {
                _llvmGenerator.Emit($"%r{register} = bitcast %{@classDef.Id}* %this to %{@classDef.ParentId}*");
                _llvmGenerator.Emit($"call void @g_{@classDef.ParentId}_construct(%{@classDef.ParentId}* %r{register})");
                register++;
            }
            
            @classDef.Fields.Skip(counter).ToList().ForEach(field =>
            {
                _llvmGenerator.Emit($"%r{register} = getelementptr %{@classDef.Id}, %{@classDef.Id}* %this, i32 0, i32 {counter}");
                _llvmGenerator.Emit(
                    $"store {AstToLlvmString.Type(field.Value.Type)} " +
                    $@"{(field.Value.Type switch
                    {
                        LatteParser.TStringContext _ => emptyStr,
                        LatteParser.TTypeNameContext _ => "null",
                        _ => "0"
                        
                    })}, " +
                    $"{AstToLlvmString.Type(field.Value.Type)}* %r{register}");

                register++;
                counter++;
            });
            
            _llvmGenerator.Emit("ret void");
            _llvmGenerator.Emit("}");
        }

        public void GenerateMethods(ClassDef classDef)
        {
            _globalState.currentClass = classDef.Id;
            classDef.Methods.Values.ToList().ForEach(method =>
            {
                method.Id = new ClassHelper().ClassMethodToFunctionName(classDef.Id, method.Id);
                method.Args.Add(new Arg(new LatteParser.TTypeNameContext(classDef.Id), "self"));
                _globalState.NameToFunction[method.Id] = method;
            });
            
            classDef.Methods.Values.ToList().ForEach(new FunctionGenerator().GenerateFromAst);
        }
    }
}
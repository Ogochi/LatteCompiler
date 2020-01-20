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

            int register = 0, counter = 0;
            if (!_globalState.LiteralToStringConstId.ContainsKey(""))
            {
                _globalState.LiteralToStringConstId[""] = _globalState.NewString;
            }

            var emptyStr = _globalState.LiteralToStringConstId[""];

            @classDef.Fields.ToList().ForEach(field =>
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

        public void GenerateMethods(ClassDef @classDef)
        {
            
        }
    }
}
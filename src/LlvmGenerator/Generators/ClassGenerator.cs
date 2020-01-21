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
            
            _llvmGenerator.Emit($"%{@class.Id}_vtable*");
            
            @class.Fields.ToList().ForEach(field =>
                {
                    _llvmGenerator.Emit($",{AstToLlvmString.Type(field.Value.Type)}");
                });
            
            _llvmGenerator.Emit("}");
        }

        public void GenerateVTable(ClassDef @class)
        {
            _llvmGenerator.Emit($"%{@class.Id}_vtable = type " + "{");

            var isFirst = true;
            @class.Methods.Values.ToList().ForEach(method =>
            {
                var toEmit = "";
                if (!isFirst)
                {
                    toEmit = ",";
                }

                toEmit += AstToLlvmString.FunctionalType(method.Item1);
                _llvmGenerator.Emit(toEmit);

                isFirst = false;
            });
            
            _llvmGenerator.Emit("}");
            
            _llvmGenerator.Emit($"@{@class.Id}_vtable_ptrs = global %{@class.Id}_vtable " + "{");
            
            isFirst = true;
            @class.Methods.Values.ToList().ForEach(method =>
            {
                var toEmit = "";
                if (!isFirst)
                {
                    toEmit = ",";
                }

                toEmit += AstToLlvmString.FunctionalType(method.Item1);
                toEmit += $" @{method.Item1.Id}";
                _llvmGenerator.Emit(toEmit);

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
            
            _llvmGenerator.Emit($"%r0 = getelementptr %{@classDef.Id}, %{@classDef.Id}* %this, i32 0, i32 0");
            _llvmGenerator.Emit($"store %{@classDef.Id}_vtable* @{@classDef.Id}_vtable_ptrs, %{@classDef.Id}_vtable** %r0");
            
            int register = 1, counter = @classDef.OwnFieldsStartIndex;
            if (counter > 0)
            {
                _llvmGenerator.Emit($"%r{register} = bitcast %{@classDef.Id}* %this to %{@classDef.ParentId}*");
                _llvmGenerator.Emit($"call void @g_{@classDef.ParentId}_construct(%{@classDef.ParentId}* %r{register})");
                register++;
            }
            
            @classDef.Fields.Skip(counter).ToList().ForEach(field =>
            {
                _llvmGenerator.Emit($"%r{register} = getelementptr %{@classDef.Id}, %{@classDef.Id}* %this, i32 0, i32 {counter + 1}");
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
                method.Item1.RealName = method.Item1.Id;
                method.Item1.Id = new ClassHelper().ClassMethodToFunctionName(classDef.Id, method.Item1.Id);
                method.Item1.Args.Add(new Arg(new LatteParser.TTypeNameContext(classDef.Id), "self"));
                _globalState.NameToFunction[method.Item1.Id] = method.Item1;
            });
            
            classDef.Methods.Values.ToList().ForEach(method => new FunctionGenerator().GenerateFromAst(method.Item1));
        }
    }
}
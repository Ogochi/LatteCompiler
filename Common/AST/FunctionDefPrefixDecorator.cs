using System.Collections.Generic;

namespace Common.AST
{
    public class FunctionDefPrefixDecorator : FunctionDef
    {
        private string _id;
        public override string Id
        {
            get => _id == "main" ? _id : $"f{_id}";
            set => _id = value;
        }

        public FunctionDefPrefixDecorator(FunctionDef instance)
        {
            Type = instance.Type;
            Block = instance.Block;
            Args = instance.Args;
            _id = instance.Id;
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Frontend;
using Common.StateManagement;
using Frontend.Exception;
using ParsingTools;
using static LLVMCompiler.Utils.BashUtils;

namespace LLVMCompiler
{
    internal class Program
    {
        private const string LLVMExtension = ".ll";
        private const int ErrorCode = 1;
        
        /*
         * Requires LLVM runtime in "lib/" as "runtime.bc" file.
         */
        public static int Main(string[] args)
        {
            //return RunProgramFromString("int main() {int x;printInt(x);return 0;}");
            return RunProgramFromArgs(args);
        }

        private static int RunProgramFromString(string programToCompile)
        {
            if (!ParseAndCompileFile(new AntlrInputStream(programToCompile), out var compilationResult))
            {
                Console.Error.WriteLine("\n----\nEncountered parsing errors.\n");
                return ErrorCode;
            }

            var errorState = ErrorState.Instance;
            if (errorState.isError())
            {
                Console.Error.WriteLine(
                    $"Found {errorState.errorsCount()} error{(errorState.errorsCount() > 1 ? "s" : "")}.\n");
                errorState.GetErrorText().ForEach(Console.Error.WriteLine);

                return ErrorCode;
            }

            compilationResult.ForEach(Console.Out.WriteLine);
            return 0;
        }

        private static int RunProgramFromArgs(string[] args)
        {
            if (!AreArgsValid(args))
            {
                return ErrorCode;
            }

            var fileName = Path.GetFileNameWithoutExtension(args[0]);
            var filePath = Path.GetDirectoryName(args[0]);

            if (!ParseAndCompileFile(new AntlrFileStream(args[0]), out var compilationResult))
            {
                Console.Error.WriteLine("ERROR");
                return ErrorCode;
            }

            var errorState = ErrorState.Instance;
            if (errorState.isError())
            {
                Console.Error.WriteLine("ERROR");
                Console.Error.WriteLine(
                    $"Found {errorState.errorsCount()} error{(errorState.errorsCount() > 1 ? "s" : "")}.\n");
                errorState.GetErrorText().ForEach(Console.Error.WriteLine);

                return ErrorCode;
            }

            var compiledFilePath = Path.Combine(filePath, fileName + LLVMExtension);
            using (var file = File.CreateText(compiledFilePath))
            {
                compilationResult.ForEach(file.WriteLine);
            }

            ExecuteBashCommand($"llvm-as -o {fileName}.tmp {compiledFilePath}");
            ExecuteBashCommand($"llvm-link -o {Path.Combine(filePath, fileName)}.bc {fileName}.tmp lib/runtime.bc");
            ExecuteBashCommand($"rm {fileName}.tmp");
            
            Console.Error.WriteLine("OK");
            return 0;
        }

        private static bool AreArgsValid(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("Program requires only 1 argument - path to file to compile.");
                return false;
            } 
            if (!File.Exists(args[0]))
            {
                Console.Error.WriteLine("Under argument path there is no file to compile.");
                return false;
            }

            return true;
        }

        /*
         * Returns "false" on parsing error
         */
        private static bool ParseAndCompileFile(ICharStream fileStream, out List<String> compilationResult)
        {
            var lexer = new LatteLexer(fileStream);
            var parser = new LatteParser(new CommonTokenStream(lexer))
            {
                BuildParseTree = true
            };

            var program = parser.program();
            try
            {
                ParseTreeWalker.Default.Walk(new StaticCheckListener(), program);
            }
            catch (InterruptedStaticCheckException) {}

            var programAst = new Common.AST.Program(program).WithPrefixedFunctions();
            compilationResult = LlvmGenerator.LlvmGenerator.Instance.GenerateFromAst(programAst);
            
            return parser.NumberOfSyntaxErrors == 0;
        }
    }
}
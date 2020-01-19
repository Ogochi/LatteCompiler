using System;
using System.Collections.Generic;
using System.IO;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Frontend;
using Common.StateManagement;
using Frontend.Exception;
using LLVMCompiler.Utils;
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
            /*return RunProgramFromString(@"class list {
            int elem;
            list next;
        }

        int main() {
            printInt(length(fromTo(1,50)));
            printInt(length2(fromTo(1,100)));
            return 0;
        }

        int head (list xs) {
            return xs . elem;
        }
 
        list cons (int x, list xs) {
            list n;
            n = new list;
            n.elem = x;
            n.next = xs;
            return n;
        }

        int length (list xs) {
            if (xs==(list)null)
                return 0;
            else
                return 1 + length (xs.next);
        }

        list fromTo (int m, int n) {
            if (m>n)
                return (list)null;
            else 
                return cons (m,fromTo (m+1,n));
        }

        int length2 (list xs) {
            int res = 0;
            while (xs != (list)null) {
                res++;
                xs = xs.next;
            }
            return res;
        }
        ");*/
            return RunProgramFromArgs(args);
        }

        private static int RunProgramFromString(string programToCompile)
        {
            ParseAndCompileFile(new AntlrInputStream(programToCompile), out var compilationResult);

            var errorState = ErrorState.Instance;
            if (errorState.isError())
            {
                Console.Error.WriteLine("ERROR");
                Console.Error.WriteLine(
                    $"Found {errorState.errorsCount()} error{(errorState.errorsCount() > 1 ? "s" : "")}.\n");
                errorState.GetErrorText().ForEach(Console.Error.WriteLine);

                return ErrorCode;
            }

            compilationResult.ForEach(Console.Out.WriteLine);
            
            Console.Error.WriteLine("OK");
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

            ParseAndCompileFile(new AntlrFileStream(args[0]), out var compilationResult);

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
        private static void ParseAndCompileFile(ICharStream fileStream, out List<string> compilationResult)
        {
            var lexer = new LatteLexer(fileStream);
            lexer.RemoveErrorListener(ConsoleErrorListener<int>.Instance);
            lexer.AddErrorListener(new LexerErrorListener<int>());

            var parser = new LatteParser(new CommonTokenStream(lexer))
            {
                BuildParseTree = true
            };
            parser.RemoveErrorListener(ConsoleErrorListener<IToken>.Instance);
            parser.AddErrorListener(new LexerErrorListener<IToken>());

            var program = parser.program();
            if (parser.NumberOfSyntaxErrors != 0)
            {
                compilationResult = new List<string>();
                return;
            }
            
            try
            {
                ParseTreeWalker.Default.Walk(new StaticCheckListener(), program);
            }
            catch (InterruptedStaticCheckException) {}

            if (ErrorState.Instance.isError())
            {
                compilationResult = new List<string>();
                return;
            }

            var programAst = new Common.AST.Program(program).WithPrefixedFunctions();
            compilationResult = new List<string> {""};
            compilationResult = LlvmGenerator.LlvmGenerator.Instance.GenerateFromAst(programAst);
        }
    }
}
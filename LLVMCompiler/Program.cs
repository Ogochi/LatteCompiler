using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Antlr4.Runtime;
using LLVMCompiler.StateManagement;
using static LLVMCompiler.Utils.BashUtils;
using ParsingTools;

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
            if (!AreArgsValid(args))
            {
                return ErrorCode;
            }

            var fileName = Path.GetFileNameWithoutExtension(args[0]);
            var filePath = Path.GetDirectoryName(args[0]);

            var compileResult = CompileFile(args[0]);

            ErrorState errorState = ErrorState.Instance;
            if (errorState.isError())
            {
                Console.Error.WriteLine($"Found {errorState.errorsCount()} errors.\n");
                errorState.GetErrorText().ForEach(Console.Error.WriteLine);

                return ErrorCode;
            }

            var compiledFilePath = Path.Combine(filePath, fileName + LLVMExtension);
            using (var file = File.CreateText(compiledFilePath))
            {
                compileResult.ForEach(file.WriteLine);
            }

            ExecuteBashCommand($"llvm-as -o {fileName}.tmp {compiledFilePath}");
            ExecuteBashCommand($"llvm-link -o {Path.Combine(filePath, fileName)}.bc {fileName}.tmp lib/runtime.bc");
            ExecuteBashCommand($"rm {fileName}.tmp");

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

        private static List<string> CompileFile(string fileFullPath)
        {
            LatteLexer lexer = new LatteLexer(new AntlrFileStream(fileFullPath));
            LatteParser parser = new LatteParser(new CommonTokenStream(lexer))
            {
                BuildParseTree = true
            };

            LatteParser.ProgramContext program = parser.program();
            return null; // TODO - use compiler
        }
    }
}
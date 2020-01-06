using System.Diagnostics;

namespace LLVMCompiler.Utils
{
    public static class BashUtils
    {
        /*
         * https://stackoverflow.com/questions/23029218/run-bash-commands-from-mono-c-sharp
         */
        public static void ExecuteBashCommand(string command)
        {
            command = command.Replace("\"", "\"\"");

            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = "-c \"" + command + "\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            proc.Start();
            proc.WaitForExit();
        }
    }
}
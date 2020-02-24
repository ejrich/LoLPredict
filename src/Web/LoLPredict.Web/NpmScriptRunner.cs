using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LoLPredict.Web
{
    public static class NpmScriptRunner
    {
        public static void CreateNodeServer(
           string workingDirectory,
           string scriptName,
           string port)
        {
            var fileName = "npm";
            var str = "run " + scriptName + " -- ";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                fileName = "cmd";
                str = "/c npm " + str;
            }

            var startInfo = new ProcessStartInfo(fileName)
            {
                Arguments = str,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = workingDirectory
            };
            startInfo.Environment["PORT"] = port;

            var process = LaunchNodeProcess(startInfo);
        }

        private static Process LaunchNodeProcess(ProcessStartInfo startInfo)
        {
            try
            {
                var process = Process.Start(startInfo);
                process.EnableRaisingEvents = true;
                return process;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to start 'npm'. To resolve this:.\n\n[1] Ensure that 'npm' is installed and can be found in one of the PATH directories.\n    Current PATH enviroment variable is: " + Environment.GetEnvironmentVariable("PATH") + "\n    Make sure the executable is in one of those directories, or update your PATH.\n\n[2] See the InnerException for further details of the cause.", ex);
            }
        }
    }
}

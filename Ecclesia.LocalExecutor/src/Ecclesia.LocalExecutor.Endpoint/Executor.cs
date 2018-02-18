using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks.Dataflow;

namespace Ecclesia.LocalExecutor.Endpoint
{
    public class Executor
    {
        private const string ScriptName = "source.py";

        private readonly IMethodManager _methodManager;

        private async Task<string> WriteInputsAsync(string[] arguments, string scriptSource, Guid id)
        {
            var pathTemp = Path.Combine(Path.GetTempPath(), id.ToString());
            Directory.CreateDirectory(pathTemp);

            int count = arguments.Length;
            for (int i = 0; i < count; i++)
            {
                var pathToArgument = Path.Combine(pathTemp, $"{i}input.txt");
                await File.WriteAllTextAsync(pathToArgument, arguments[i]);
            }
            
            await File.WriteAllTextAsync(Path.Combine(pathTemp, ScriptName), scriptSource);
            return pathTemp;
        }

        private async Task<string[]> ReadOutputsAsync(string path)
        {
            var files = Directory.GetFiles(path, "*output.txt");
            Array.Sort(files);
            
            var tasks = files.Select(file => File.ReadAllTextAsync(file))
                             .ToList();

            List<string> results = new List<string>();

            foreach (var task in tasks)
            {
                results.Add(await task);
            }

            return results.ToArray();
        }

        public Executor(IMethodManager manager)
        {
            _methodManager = manager;
        }

        public Task Add(string name, string[] parameters, Action<string[]> callBack) // по одному
        {
            return Task.Run(async () =>
            {
                try
                {
                    callBack?.Invoke(await ExecuteOperationAsync(name, parameters));
                }
                catch (Exception e)
                {
                    callBack(null);
                    // TODO: add logging
                    Console.Error.WriteLine(e);
                }
            });
        }

        private async Task<string[]> ExecuteOperationAsync(string name, string[] parameters)
        {
            var scriptSource = _methodManager.GetMethodSource(name);
            var path = await WriteInputsAsync(parameters, scriptSource, Guid.NewGuid());
            try
            {
                RunScript(path);
                return await ReadOutputsAsync(path);
            }
            finally
            {
                Directory.Delete(path, true);
            }
        }

        private void RunScript(string inputPath)
        {
            ProcessStartInfo start = new ProcessStartInfo()
            {
                FileName = "python",
                Arguments = String.Join(" ", Path.Combine(inputPath, ScriptName), inputPath),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            Process process = new Process();
            process = Process.Start(start);
            process.WaitForExit();
            
            if (process.ExitCode != 0)
                throw new Exception($"Script execution error: {process.StandardError.ReadToEnd()}");
        }
    }
}

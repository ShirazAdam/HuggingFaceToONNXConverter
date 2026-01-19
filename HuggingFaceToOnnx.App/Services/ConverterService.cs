using System.Diagnostics;
using System.IO;

namespace HuggingFaceToOnnx.App.Services
{
    public class ConverterService : IConverterService
    {
        public async Task<bool> IsPythonAvailableAsync()
        {
            try
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "python",
                    Arguments = "--version",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(processStartInfo);
                if (process == null) return false;

                await process.WaitForExitAsync();
                return process.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task ConvertModelAsync(string modelId, string outputDirectory, Action<string> logCallback)
        {
            logCallback($"Starting conversion for {modelId}...");
            
            // Ensure output directory exists
            Directory.CreateDirectory(outputDirectory);

            var startInfo = new ProcessStartInfo
            {
                FileName = "optimum-cli",
                Arguments = $"export onnx --model {modelId} \"{outputDirectory}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            // If optimum-cli is not in path, we might need to use python -m optimum.exporters.onnx
            // But let's try calling it via python in case it's a script
            // Better yet, let's try `python -m optimum.exporters.onnx` if the direct cli fails, or just default to `optimum-cli`
            // Actually, safest is to assume the user has set up their environment or we install it.
            // For this iteration, let's assume `optimum-cli` is available if they pip installed it.
            // But often on windows scripts are in a Scripts folder not on PATH.
            // Let's use `python -m optimum.exporters.onnx` logic if possible or `optimum-cli` via shell execution?
            // Windows is tricky with PATH.
            // Let's try `optimum-cli` first.

            try 
            {
                 using var process = new Process();
                 process.StartInfo = startInfo;
                 process.OutputDataReceived += (s, e) => { if (e.Data != null) logCallback(e.Data); };
                 process.ErrorDataReceived += (s, e) => { if (e.Data != null) logCallback($"ERROR: {e.Data}"); };
                 
                 process.Start();
                 process.BeginOutputReadLine();
                 process.BeginErrorReadLine();

                 await process.WaitForExitAsync();
                 
                 if (process.ExitCode == 0)
                 {
                     logCallback("Conversion completed successfully.");
                 }
                 else
                 {
                     logCallback($"Conversion failed with exit code {process.ExitCode}.");
                 }
            }
            catch (Exception ex)
            {
                logCallback($"Exception running conversion: {ex.Message}");
                // Fallback suggestion
                logCallback("Ensure 'optimum' and 'onnxruntime' are installed via pip: `pip install optimum[onnxruntime]`");
            }
        }
    }
}

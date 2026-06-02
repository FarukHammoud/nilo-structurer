using System.Diagnostics;

namespace Infrastructure {

    // Script must accept --out <output_path> 
    public class PythonExecuter {
        private readonly string _scriptName;

        public PythonExecuter(String scriptName) {
            _scriptName = scriptName;
        }

        public async Task<String> Run(String args = "", CancellationToken ct = default) {
            var tempOutput = Path.GetTempFileName();

            var baseDirectory = AppContext.BaseDirectory;
            var scriptPath = Path.Combine(baseDirectory, _scriptName);
            try {
                var psi = new ProcessStartInfo {
                    FileName = "python",
                    Arguments = $"\"{scriptPath}\" {args} --out \"{tempOutput}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };

                using var process = Process.Start(psi)
                    ?? throw new InvalidOperationException("Failed to start Python process.");

                var stdout = process.StandardOutput.ReadToEndAsync();
                var stderr = process.StandardError.ReadToEndAsync();

                await process.WaitForExitAsync(ct);
                await Task.WhenAll(stdout, stderr);

                if (process.ExitCode != 0)
                    throw new InvalidOperationException(
                        $"{_scriptName} exited with code {process.ExitCode}:\n{await stderr}");

                return await File.ReadAllTextAsync(tempOutput, ct);
            } finally {
                if (File.Exists(tempOutput))
                    File.Delete(tempOutput);
            }
        }
    }
}

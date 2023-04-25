﻿using System.Diagnostics;
using System.Text;

namespace DemonsRunner.Domain.Models
{
    public class PHPScriptExecutor : IDisposable
    {
        public event EventHandler? ScriptExitedByUser;

        public event DataReceivedEventHandler? ScriptOutputMessageReceived;

        private bool _disposed = false;

        private readonly Process _executableConsole;

        public PHPScript ExecutableScript { get; }

        public bool IsRunning { get; private set; }

        public PHPScriptExecutor(PHPScript script, bool showExecutingWindow)
        {
            ExecutableScript = script;
            _executableConsole = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    WorkingDirectory = ExecutableScript.ExecutableFile.FullPath.TrimEnd(ExecutableScript.ExecutableFile.Name.ToCharArray()),
                    CreateNoWindow = !showExecutingWindow,
                    StandardErrorEncoding = Encoding.UTF8,
                    StandardOutputEncoding = Encoding.UTF8,
                },
                EnableRaisingEvents = true,
            };
            _executableConsole.Exited += OnScriptExited;
            _executableConsole.ErrorDataReceived += OnScriptErrorOutputReceived;
            _executableConsole.OutputDataReceived += OnScriptOutputDataReceived;
        }

        private void OnScriptErrorOutputReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                ScriptOutputMessageReceived?.Invoke(this, e);
            }
        }

        private void OnScriptOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                ScriptOutputMessageReceived?.Invoke(this, e);
            }
        }

        private void OnScriptExited(object? sender, EventArgs e) => ScriptExitedByUser?.Invoke(this, EventArgs.Empty);

        public void Start()
        {
            if (_disposed) 
                throw new ObjectDisposedException(nameof(PHPScriptExecutor));
            _executableConsole.Start();
            _executableConsole.StandardInput.WriteLine(ExecutableScript.Command);
            _executableConsole.StandardInput.Flush();
            _executableConsole.BeginOutputReadLine();
            _executableConsole.BeginErrorReadLine();
            IsRunning = true;
        }

        public void Stop()
        {
            if (_disposed) 
                throw new ObjectDisposedException(nameof(PHPScriptExecutor));
            _executableConsole.Kill();
            IsRunning = false;
        }

        public void Dispose()
        {
            IsRunning = false;
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // managed resources 
                    _executableConsole.Exited -= OnScriptExited;
                    _executableConsole.OutputDataReceived -= OnScriptOutputDataReceived;
                    _executableConsole.Dispose();
                }

                // unmanaged resourses
                _disposed = true;
            }
        }
    }
}

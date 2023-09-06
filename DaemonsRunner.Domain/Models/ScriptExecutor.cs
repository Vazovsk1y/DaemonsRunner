using DaemonsRunner.Domain.Exceptions.Base;
using System.Diagnostics;

namespace DaemonsRunner.Domain.Models;

public class ScriptExecutor : IDisposable
{
    #region --Events--

    public event EventHandler? ExitedByTaskManager;

    public event Func<object, string, Task>? OutputMessageReceived;

    #endregion

    #region --Fields--

    private readonly string _workingDirectory;

    private readonly Script _executableScript;

    private Process? _executableShell;

    private bool _isMessagesReceivingEnable;

    private bool _isRunning;

    private bool _isDisposed = false;

    private bool _isClosedByTaskManager = true;

    #endregion

    #region --Properties--

    public Script ExecutableScript
    {
        get
        {
            ThrownExceptionIfDisposed();
            return _executableScript;
        }
    }

    public bool IsRunning
    {
        get
        {
            ThrownExceptionIfDisposed();
            return _isRunning;
        }
        private set => _isRunning = value;
    }

    public bool IsMessagesReceiving
    {
        get
        {
            ThrownExceptionIfDisposed();
            return _isMessagesReceivingEnable;
        }
        private set => _isMessagesReceivingEnable = value;
    }

    #endregion

    #region --Constructors--

    private ScriptExecutor(
        Script executableScript,
        string? workingDirectory = null)
    {
        _executableScript = executableScript;
        _workingDirectory = workingDirectory
            ?? Path.GetDirectoryName(ExecutableScript.ExecutableFile?.Path)
            ?? string.Empty;
    }

    #endregion

    #region --Methods--

    #region -Public-

    public static ScriptExecutor Create(Script executableScript, string? workingDirectory = null)
    {
        ArgumentNullException.ThrowIfNull(executableScript, nameof(executableScript));
        return new ScriptExecutor(executableScript, workingDirectory);
    }

    public void Start()
    {
        ThrownExceptionIfDisposed();
        if (IsRunning)
        {
            return;
        }

        _executableShell = ConfigureExecutableProcess();
        _executableShell.Exited += OnProcessExited;
        bool startingResult = _executableShell.Start();
        if (!startingResult)
        {
            throw new DomainException("Process starting result was equal to false.");
        }

        IsRunning = true;
    }

    public void StartMessagesReceiving()
    {
        ThrownExceptionIfDisposed();
        if (IsMessagesReceiving)
        {
            return;
        }

        if (!IsRunning)
        {
            throw new DomainException($"{nameof(ScriptExecutor)} wasn't started. Unable to start messages receiving.");
        }

        _executableShell!.ErrorDataReceived += OnProcessOutputDataReceived;
        _executableShell.OutputDataReceived += OnProcessOutputDataReceived;
        _executableShell.BeginOutputReadLine();
        _executableShell.BeginErrorReadLine();
        IsMessagesReceiving = true;
    }

    public void ExecuteCommand()
    {
        ThrownExceptionIfDisposed();
        if (!IsRunning)
        {
            throw new DomainException($"{nameof(ScriptExecutor)} wasn't started. Unable execute command.");
        }

        _executableShell!.StandardInput.WriteLine(ExecutableScript.Command);
        _executableShell.StandardInput.Flush();
    }

    public void Stop()
    {
        ThrownExceptionIfDisposed();
        if (!IsRunning)
        {
            return;
        }

        _isClosedByTaskManager = false;
        StopMessagesReceiving();
        _executableShell!.Kill(true);
        _executableShell.Dispose();
        IsRunning = false;
    }

    public void StopMessagesReceiving()
    {
        ThrownExceptionIfDisposed();
        if (!IsMessagesReceiving)
        {
            return;
        }

        if (!IsRunning)
        {
            throw new DomainException($"{nameof(ScriptExecutor)} wasn't started.");
        }

        _executableShell!.CancelErrorRead();
        _executableShell.CancelOutputRead();
        _executableShell.ErrorDataReceived -= OnProcessOutputDataReceived;
        _executableShell.OutputDataReceived -= OnProcessOutputDataReceived;
        IsMessagesReceiving = false;
    }

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
    public void Dispose() => CleanUp();
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize

    #endregion

    #region --EventHandlers--

    private async void OnProcessOutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        string endcodingMessage = "Active code page: 65001";
        if (!string.IsNullOrEmpty(e.Data) && e.Data != endcodingMessage)
        {
            if (OutputMessageReceived is not null)
            {
                await OutputMessageReceived.Invoke(this, e.Data);
            }
        }
    }

    private void OnProcessExited(object? sender, EventArgs e)
    {
        if (!_isClosedByTaskManager)
        {
            return;
        }

        if (IsMessagesReceiving)
        {
            StopMessagesReceiving();
        }

        IsRunning = false;
        ExitedByTaskManager?.Invoke(this, EventArgs.Empty);
    }

    #endregion

    private void CleanUp()
    {
        if (_isDisposed)
        {
            return;
        }

        if (IsRunning)
        {
            Stop();
        }

        _executableShell?.Dispose();
        _isDisposed = true;
    }

    private Process ConfigureExecutableProcess()
    {
        return new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "cmd",
                Arguments = "/k chcp 65001",      // set UTF8 endcoding to cmd output.
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                WorkingDirectory = _workingDirectory,
                CreateNoWindow = true,
            },
            EnableRaisingEvents = true,
        };
    }

    private void ThrownExceptionIfDisposed() => ObjectDisposedException.ThrowIf(_isDisposed, this);

    #endregion
}


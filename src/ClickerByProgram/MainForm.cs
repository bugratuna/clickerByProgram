using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClickerByProgram.Input;

namespace ClickerByProgram;

public partial class MainForm : Form
{
    private readonly BindingList<IInputAction> _actions = new();
    private readonly KeyboardRecorder _keyboardRecorder = new();
    private readonly MouseRecorder _mouseRecorder = new();

    private CancellationTokenSource? _playbackCancellationTokenSource;
    private Task? _playbackTask;
    private bool _isRunning;

    public MainForm()
    {
        InitializeComponent();
        actionsListBox.DataSource = _actions;
        actionsListBox.DisplayMember = nameof(IInputAction.Description);
        _actions.ListChanged += (_, _) => UpdateExecutionButtons();

        _keyboardRecorder.ActionRecorded += OnInputActionRecorded;
        _mouseRecorder.ActionRecorded += OnInputActionRecorded;

        Shown += (_, _) => RefreshProcessList();
        UpdateExecutionButtons();
    }

    private void OnRefreshProcessesClick(object? sender, EventArgs e)
    {
        RefreshProcessList();
    }

    private void RefreshProcessList()
    {
        var selected = targetProcessComboBox.SelectedItem as TargetWindowItem;
        var selectedHandle = selected?.Handle ?? IntPtr.Zero;

        targetProcessComboBox.BeginUpdate();
        targetProcessComboBox.Items.Clear();

        IEnumerable<Process> processes;
        try
        {
            processes = Process.GetProcesses()
                .Where(p => p.MainWindowHandle != IntPtr.Zero)
                .OrderBy(p => p.ProcessName)
                .ThenBy(p => p.Id)
                .ToArray();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Failed to enumerate processes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            targetProcessComboBox.EndUpdate();
            return;
        }

        foreach (var process in processes)
        {
            string title;
            try
            {
                title = string.IsNullOrWhiteSpace(process.MainWindowTitle)
                    ? process.ProcessName
                    : process.MainWindowTitle;
            }
            catch
            {
                title = process.ProcessName;
            }

            var item = new TargetWindowItem(process.ProcessName, title, process.Id, process.MainWindowHandle);
            targetProcessComboBox.Items.Add(item);

            if (process.MainWindowHandle == selectedHandle)
            {
                targetProcessComboBox.SelectedItem = item;
            }
        }

        if (targetProcessComboBox.SelectedItem == null && targetProcessComboBox.Items.Count > 0)
        {
            targetProcessComboBox.SelectedIndex = 0;
        }

        targetProcessComboBox.EndUpdate();
    }

    private void OnRecordKeyboardClick(object? sender, EventArgs e)
    {
        if (_keyboardRecorder.IsRecording)
        {
            return;
        }

        try
        {
            _keyboardRecorder.Start();
            recordKeyboardButton.Enabled = false;
            stopKeyboardRecordButton.Enabled = true;
            SetStatus("Recording keyboard");
        }
        catch (Win32Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Keyboard Hook Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void OnStopKeyboardRecordClick(object? sender, EventArgs e)
    {
        if (!_keyboardRecorder.IsRecording)
        {
            return;
        }

        _keyboardRecorder.Stop();
        recordKeyboardButton.Enabled = true;
        stopKeyboardRecordButton.Enabled = false;
        SetStatus("Keyboard recording stopped");
    }

    private void OnRecordMouseClick(object? sender, EventArgs e)
    {
        if (_mouseRecorder.IsRecording)
        {
            return;
        }

        try
        {
            _mouseRecorder.Start();
            recordMouseButton.Enabled = false;
            stopMouseRecordButton.Enabled = true;
            SetStatus("Recording mouse");
        }
        catch (Win32Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Mouse Hook Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void OnStopMouseRecordClick(object? sender, EventArgs e)
    {
        if (!_mouseRecorder.IsRecording)
        {
            return;
        }

        _mouseRecorder.Stop();
        recordMouseButton.Enabled = true;
        stopMouseRecordButton.Enabled = false;
        SetStatus("Mouse recording stopped");
    }

    private void OnStartClick(object? sender, EventArgs e)
    {
        StartPlayback();
    }

    private async void OnStopClick(object? sender, EventArgs e)
    {
        await StopPlaybackAsync();
    }

    private async void OnToggleClick(object? sender, EventArgs e)
    {
        if (_isRunning)
        {
            await StopPlaybackAsync();
        }
        else
        {
            StartPlayback();
        }
    }

    private void OnClearActionsClick(object? sender, EventArgs e)
    {
        _actions.Clear();
    }

    private void StartPlayback()
    {
        if (_isRunning)
        {
            return;
        }

        if (_actions.Count == 0)
        {
            MessageBox.Show(this, "There are no recorded actions to play.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (targetProcessComboBox.SelectedItem is not TargetWindowItem target)
        {
            MessageBox.Show(this, "Please select a target window.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        _playbackCancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = _playbackCancellationTokenSource.Token;
        var context = new InputExecutionContext(target.Handle, LogPlaybackEvent);
        var actionsSnapshot = _actions.ToList();

        _isRunning = true;
        UpdateExecutionButtons();
        SetStatus("Playback running");

        _playbackTask = Task.Run(async () =>
        {
            try
            {
                do
                {
                    foreach (var action in actionsSnapshot)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await action.ExecuteAsync(context, cancellationToken).ConfigureAwait(false);
                    }

                    var (loopEnabled, loopDelayMilliseconds) = GetLoopSettings();

                    if (!loopEnabled)
                    {
                        break;
                    }

                    if (loopDelayMilliseconds > 0)
                    {
                        var loopDelay = TimeSpan.FromMilliseconds(loopDelayMilliseconds);
                        await Task.Delay(loopDelay, cancellationToken).ConfigureAwait(false);
                    }
                }
                while (!cancellationToken.IsCancellationRequested);
            }
            catch (OperationCanceledException)
            {
                // Expected when stopping playback.
            }
            finally
            {
                FinishPlayback();
            }
        }, cancellationToken);
    }

    private async Task StopPlaybackAsync()
    {
        if (!_isRunning)
        {
            return;
        }

        _playbackCancellationTokenSource?.Cancel();

        if (_playbackTask != null)
        {
            try
            {
                await _playbackTask;
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation is requested.
            }
        }
    }

    private void FinishPlayback()
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action(FinishPlayback));
            return;
        }

        _playbackCancellationTokenSource?.Dispose();
        _playbackCancellationTokenSource = null;
        _playbackTask = null;
        _isRunning = false;
        UpdateExecutionButtons();
        SetStatus("Idle");
    }

    private void UpdateExecutionButtons()
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action(UpdateExecutionButtons));
            return;
        }

        startButton.Enabled = !_isRunning && _actions.Count > 0;
        stopButton.Enabled = _isRunning;
        toggleButton.Text = _isRunning ? "Stop" : "Start / Stop";
        toggleButton.Enabled = _isRunning || _actions.Count > 0;
    }

    private void OnInputActionRecorded(object? sender, InputActionRecordedEventArgs e)
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action(() => AddRecordedAction(e.Action)));
        }
        else
        {
            AddRecordedAction(e.Action);
        }
    }

    private void AddRecordedAction(IInputAction action)
    {
        _actions.Add(action);
        SetStatus($"Recorded: {action.Description}");
    }

    private void LogPlaybackEvent(string message)
    {
        SetStatus(message);
    }

    private void SetStatus(string status)
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action(() => SetStatus(status)));
            return;
        }

        statusLabel.Text = $"Status: {status}";
    }

    private async void OnFormClosing(object? sender, FormClosingEventArgs e)
    {
        if (_isRunning)
        {
            e.Cancel = true;
            await StopPlaybackAsync();
            Close();
            return;
        }

        _keyboardRecorder.Dispose();
        _mouseRecorder.Dispose();
    }

    private (bool LoopEnabled, double LoopDelayMilliseconds) GetLoopSettings()
    {
        if (InvokeRequired)
        {
            return ((bool, double))Invoke(new Func<(bool, double)>(GetLoopSettings));
        }

        return (loopCheckBox.Checked, (double)loopDelayUpDown.Value);
    }

    private sealed record TargetWindowItem(string ProcessName, string Title, int ProcessId, IntPtr Handle)
    {
        public override string ToString()
        {
            return $"{Title} - {ProcessName} (PID: {ProcessId})";
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClickerByProgram.Input;

namespace ClickerByProgram;

public partial class MainForm : Form
{
    private const int StartHotkeyId = 1;
    private const int StopHotkeyId = 2;
    private const int ToggleHotkeyId = 3;
    private const int WM_HOTKEY = 0x0312;

    private readonly BindingList<IInputAction> _actions = new();
    private readonly KeyboardRecorder _keyboardRecorder = new();
    private readonly MouseRecorder _mouseRecorder = new();

    private CancellationTokenSource? _playbackCancellationTokenSource;
    private Task? _playbackTask;
    private bool _isRunning;
    private bool _suppressScriptPreviewUpdate;
    private bool _suppressHotkeySelectionEvents;

    private CancellationTokenSource? _leftClickLoopCancellationTokenSource;
    private Task? _leftClickLoopTask;
    private bool _isLeftClickLoopActive;

    private CancellationTokenSource? _rightClickLoopCancellationTokenSource;
    private Task? _rightClickLoopTask;
    private bool _isRightClickLoopActive;

    private Keys _startHotkey = Keys.F7;
    private Keys _stopHotkey = Keys.F8;
    private Keys _toggleHotkey = Keys.F9;

    private bool IsAnyAutomationRunning => _isRunning || _isLeftClickLoopActive || _isRightClickLoopActive;

    private static readonly Keys[] HotkeyOptions =
    {
        Keys.F1, Keys.F2, Keys.F3, Keys.F4, Keys.F5, Keys.F6, Keys.F7, Keys.F8, Keys.F9, Keys.F10, Keys.F11, Keys.F12,
        Keys.F13, Keys.F14, Keys.F15, Keys.F16, Keys.F17, Keys.F18, Keys.F19, Keys.F20, Keys.F21, Keys.F22, Keys.F23, Keys.F24,
        Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9,
        Keys.A, Keys.B, Keys.C, Keys.D, Keys.E, Keys.F, Keys.G, Keys.H, Keys.I, Keys.J, Keys.K, Keys.L, Keys.M,
        Keys.N, Keys.O, Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T, Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y, Keys.Z,
        Keys.NumPad0, Keys.NumPad1, Keys.NumPad2, Keys.NumPad3, Keys.NumPad4, Keys.NumPad5, Keys.NumPad6, Keys.NumPad7, Keys.NumPad8, Keys.NumPad9,
        Keys.Space, Keys.Tab, Keys.Escape, Keys.Insert, Keys.Delete, Keys.Home, Keys.End, Keys.PageUp, Keys.PageDown,
        Keys.Up, Keys.Down, Keys.Left, Keys.Right
    };

    public MainForm()
    {
        InitializeComponent();
        InitializeHotkeyDropdowns();
        actionsListBox.DataSource = _actions;
        actionsListBox.DisplayMember = nameof(IInputAction.Description);
        _actions.ListChanged += OnActionsListChanged;

        _keyboardRecorder.ActionRecorded += OnInputActionRecorded;
        _mouseRecorder.ActionRecorded += OnInputActionRecorded;

        Shown += (_, _) => RefreshProcessList();
        UpdateExecutionButtons();
        RefreshScriptPreview();
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        if (!RegisterAllHotkeys())
        {
            MessageBox.Show(this,
                "Unable to register the default hotkeys. They may already be in use by another application.",
                "Hotkey registration", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    protected override void OnHandleDestroyed(EventArgs e)
    {
        if (!RecreatingHandle)
        {
            UnregisterAllHotkeys();
        }

        base.OnHandleDestroyed(e);
    }

    private void OnActionsListChanged(object? sender, ListChangedEventArgs e)
    {
        UpdateExecutionButtons();

        if (!_suppressScriptPreviewUpdate)
        {
            RefreshScriptPreview();
        }
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

    private void InitializeHotkeyDropdowns()
    {
        _suppressHotkeySelectionEvents = true;

        try
        {
            var options = HotkeyOptions.Cast<object>().ToArray();
            startHotkeyComboBox.Items.AddRange(options);
            stopHotkeyComboBox.Items.AddRange(options);
            toggleHotkeyComboBox.Items.AddRange(options);

            startHotkeyComboBox.SelectedItem = _startHotkey;
            stopHotkeyComboBox.SelectedItem = _stopHotkey;
            toggleHotkeyComboBox.SelectedItem = _toggleHotkey;
        }
        finally
        {
            _suppressHotkeySelectionEvents = false;
        }
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

    private void OnImportActionsClick(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Filter = "Automation files (*.txt;*.json)|*.txt;*.json|All files (*.*)|*.*",
            Title = "Import automation actions"
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            var content = File.ReadAllText(dialog.FileName);
            scriptPreviewTextBox.Text = content;
            var actions = ActionSerialization.Deserialize(content);
            ReplaceActions(actions);
            SetStatus($"Imported {actions.Count} actions");
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Failed to import actions: {ex.Message}", "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void OnExportActionsClick(object? sender, EventArgs e)
    {
        if (_actions.Count == 0)
        {
            MessageBox.Show(this, "There are no actions to export.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var dialog = new SaveFileDialog
        {
            Filter = "Automation files (*.txt)|*.txt|JSON files (*.json)|*.json|All files (*.*)|*.*",
            FileName = "automation-actions.txt",
            Title = "Export automation actions"
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            var serialized = ActionSerialization.Serialize(_actions);
            File.WriteAllText(dialog.FileName, serialized);
            scriptPreviewTextBox.Text = serialized;
            SetStatus($"Exported {_actions.Count} actions");
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Failed to export actions: {ex.Message}", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void OnStartClick(object? sender, EventArgs e)
    {
        StartPlayback();
    }

    private async void OnStopClick(object? sender, EventArgs e)
    {
        await StopAllAutomationAsync();
    }

    private void OnToggleClick(object? sender, EventArgs e)
    {
        HandleToggleRequest();
    }

    private void OnStartHotkeySelectionChanged(object? sender, EventArgs e)
    {
        if (_suppressHotkeySelectionEvents)
        {
            return;
        }

        if (startHotkeyComboBox.SelectedItem is Keys key)
        {
            UpdateHotkeySelection(ref _startHotkey, key, startHotkeyComboBox, "start");
        }
    }

    private void OnStopHotkeySelectionChanged(object? sender, EventArgs e)
    {
        if (_suppressHotkeySelectionEvents)
        {
            return;
        }

        if (stopHotkeyComboBox.SelectedItem is Keys key)
        {
            UpdateHotkeySelection(ref _stopHotkey, key, stopHotkeyComboBox, "stop");
        }
    }

    private void OnToggleHotkeySelectionChanged(object? sender, EventArgs e)
    {
        if (_suppressHotkeySelectionEvents)
        {
            return;
        }

        if (toggleHotkeyComboBox.SelectedItem is Keys key)
        {
            UpdateHotkeySelection(ref _toggleHotkey, key, toggleHotkeyComboBox, "toggle");
        }
    }

    private async void OnLeftClickLoopButtonClick(object? sender, EventArgs e)
    {
        if (_isLeftClickLoopActive)
        {
            await StopLeftClickLoopAsync();
        }
        else
        {
            StartMouseLoop(MouseButton.Left, ref _isLeftClickLoopActive, ref _leftClickLoopCancellationTokenSource,
                ref _leftClickLoopTask);
        }
    }

    private async void OnRightClickLoopButtonClick(object? sender, EventArgs e)
    {
        if (_isRightClickLoopActive)
        {
            await StopRightClickLoopAsync();
        }
        else
        {
            StartMouseLoop(MouseButton.Right, ref _isRightClickLoopActive, ref _rightClickLoopCancellationTokenSource,
                ref _rightClickLoopTask);
        }
    }

    private void OnClearActionsClick(object? sender, EventArgs e)
    {
        _actions.Clear();
    }

    private void OnLoopToggleSyncChanged(object? sender, EventArgs e)
    {
        UpdateExecutionButtons();
    }

    private bool StartMouseLoop(MouseButton button, ref bool isActive,
        ref CancellationTokenSource? cancellationTokenSource, ref Task? loopTask)
    {
        if (isActive)
        {
            return true;
        }

        if (targetProcessComboBox.SelectedItem is not TargetWindowItem target)
        {
            MessageBox.Show(this, "Please select a target window before starting a mouse click loop.", "Information",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return false;
        }

        if (!TryGetLoopClickPoint(button, target.Handle, out var clickPoint))
        {
            MessageBox.Show(this,
                "Unable to determine a click location. Record a mouse action or ensure the target window is visible.",
                "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return false;
        }

        cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        isActive = true;
        UpdateExecutionButtons();
        UpdateLoopStatusMessage();

        loopTask = Task.Run(async () =>
        {
            try
            {
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    NativeMethods.SendMouseButton(target.Handle, clickPoint, button, true);
                    await Task.Delay(1, cancellationToken).ConfigureAwait(false);
                    NativeMethods.SendMouseButton(target.Handle, clickPoint, button, false);
                    await Task.Delay(1, cancellationToken).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when the loop is cancelled.
            }
            finally
            {
                NativeMethods.SendMouseButton(target.Handle, clickPoint, button, false);
            }
        });

        return true;
    }

    private async Task StopLeftClickLoopAsync()
    {
        if (!_isLeftClickLoopActive && _leftClickLoopTask == null)
        {
            return;
        }

        await StopMouseLoopAsync(_leftClickLoopCancellationTokenSource, _leftClickLoopTask);

        _leftClickLoopCancellationTokenSource?.Dispose();
        _leftClickLoopCancellationTokenSource = null;
        _leftClickLoopTask = null;
        _isLeftClickLoopActive = false;

        OnMouseLoopStopped();
    }

    private async Task StopRightClickLoopAsync()
    {
        if (!_isRightClickLoopActive && _rightClickLoopTask == null)
        {
            return;
        }

        await StopMouseLoopAsync(_rightClickLoopCancellationTokenSource, _rightClickLoopTask);

        _rightClickLoopCancellationTokenSource?.Dispose();
        _rightClickLoopCancellationTokenSource = null;
        _rightClickLoopTask = null;
        _isRightClickLoopActive = false;

        OnMouseLoopStopped();
    }

    private void OnMouseLoopStopped()
    {
        UpdateExecutionButtons();

        if (_isLeftClickLoopActive || _isRightClickLoopActive)
        {
            UpdateLoopStatusMessage();
        }
        else
        {
            SetStatus("Idle");
        }
    }

    private static async Task StopMouseLoopAsync(CancellationTokenSource? cancellationTokenSource, Task? loopTask)
    {
        cancellationTokenSource?.Cancel();

        if (loopTask != null)
        {
            try
            {
                await loopTask;
            }
            catch (OperationCanceledException)
            {
                // Expected when the loop is cancelled.
            }
        }
    }

    private bool TryGetLoopClickPoint(MouseButton button, IntPtr windowHandle, out Point screenPoint)
    {
        for (var i = _actions.Count - 1; i >= 0; i--)
        {
            switch (_actions[i])
            {
                case MouseButtonAction buttonAction when buttonAction.Button == button:
                    screenPoint = buttonAction.ScreenPosition;
                    return true;
                case MouseMoveAction moveAction:
                    screenPoint = moveAction.ScreenPosition;
                    return true;
            }
        }

        if (NativeMethods.TryGetWindowCenter(windowHandle, out var center))
        {
            screenPoint = center;
            return true;
        }

        screenPoint = Point.Empty;
        return false;
    }

    private async Task StopAllAutomationAsync()
    {
        await StopPlaybackAsync();
        await StopLeftClickLoopAsync();
        await StopRightClickLoopAsync();
    }

    private void UpdateLoopStatusMessage()
    {
        if (_isRunning)
        {
            SetStatus("Playback running");
            return;
        }

        if (_isLeftClickLoopActive && _isRightClickLoopActive)
        {
            SetStatus("Left and right click rapid loops active");
        }
        else if (_isLeftClickLoopActive)
        {
            SetStatus("Left click rapid loop active");
        }
        else if (_isRightClickLoopActive)
        {
            SetStatus("Right click rapid loop active");
        }
        else
        {
            SetStatus("Idle");
        }
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
        if (_isLeftClickLoopActive || _isRightClickLoopActive)
        {
            UpdateLoopStatusMessage();
        }
        else
        {
            SetStatus("Idle");
        }
    }

    private void UpdateExecutionButtons()
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action(UpdateExecutionButtons));
            return;
        }

        var automationRunning = IsAnyAutomationRunning;

        startButton.Enabled = !_isRunning && _actions.Count > 0;
        stopButton.Enabled = automationRunning;
        toggleButton.Text = automationRunning ? "Stop" : "Start / Stop";
        toggleButton.Enabled = automationRunning || _actions.Count > 0 || leftLoopToggleCheckBox.Checked ||
            rightLoopToggleCheckBox.Checked;

        leftClickLoopButton.Text = _isLeftClickLoopActive ? "Stop Left Click Loop" : "Left Click Loop";
        rightClickLoopButton.Text = _isRightClickLoopActive ? "Stop Right Click Loop" : "Right Click Loop";
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

    private void ReplaceActions(IReadOnlyCollection<IInputAction> actions)
    {
        _suppressScriptPreviewUpdate = true;

        try
        {
            _actions.Clear();

            foreach (var action in actions)
            {
                _actions.Add(action);
            }
        }
        finally
        {
            _suppressScriptPreviewUpdate = false;
        }

        RefreshScriptPreview();
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

    private void RefreshScriptPreview()
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action(RefreshScriptPreview));
            return;
        }

        var serialized = ActionSerialization.Serialize(_actions);
        scriptPreviewTextBox.Text = serialized;
    }

    private void UpdateHotkeySelection(ref Keys currentKey, Keys newKey, ComboBox comboBox, string actionName)
    {
        if (newKey == currentKey)
        {
            return;
        }

        var previous = currentKey;
        currentKey = newKey;

        if (!RegisterAllHotkeys())
        {
            currentKey = previous;
            _suppressHotkeySelectionEvents = true;
            try
            {
                comboBox.SelectedItem = previous;
            }
            finally
            {
                _suppressHotkeySelectionEvents = false;
            }

            if (!RegisterAllHotkeys())
            {
                MessageBox.Show(this,
                    "Unable to restore the previous hotkey configuration.",
                    "Hotkey registration", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            MessageBox.Show(this,
                $"Unable to register the {actionName} hotkey '{newKey}'. The key might already be in use.",
                "Hotkey registration", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private bool RegisterAllHotkeys()
    {
        if (!IsHandleCreated)
        {
            return true;
        }

        UnregisterAllHotkeys();

        if (!RegisterHotKeyInternal(StartHotkeyId, _startHotkey))
        {
            return false;
        }

        if (!RegisterHotKeyInternal(StopHotkeyId, _stopHotkey))
        {
            NativeMethods.UnregisterHotKey(Handle, StartHotkeyId);
            return false;
        }

        if (!RegisterHotKeyInternal(ToggleHotkeyId, _toggleHotkey))
        {
            NativeMethods.UnregisterHotKey(Handle, StartHotkeyId);
            NativeMethods.UnregisterHotKey(Handle, StopHotkeyId);
            return false;
        }

        return true;
    }

    private bool RegisterHotKeyInternal(int id, Keys key)
    {
        var keyCode = (uint)(key & Keys.KeyCode);
        if (keyCode == 0)
        {
            return false;
        }

        return NativeMethods.RegisterHotKey(Handle, id, 0, keyCode);
    }

    private void UnregisterAllHotkeys()
    {
        if (!IsHandleCreated)
        {
            return;
        }

        NativeMethods.UnregisterHotKey(Handle, StartHotkeyId);
        NativeMethods.UnregisterHotKey(Handle, StopHotkeyId);
        NativeMethods.UnregisterHotKey(Handle, ToggleHotkeyId);
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == WM_HOTKEY)
        {
            switch (m.WParam.ToInt32())
            {
                case StartHotkeyId:
                    StartPlayback();
                    break;
                case StopHotkeyId:
                    _ = StopAllAutomationAsync();
                    break;
                case ToggleHotkeyId:
                    HandleToggleRequest();

                    break;
            }

            return;
        }

        base.WndProc(ref m);
    }

    private async void OnFormClosing(object? sender, FormClosingEventArgs e)
    {
        if (IsAnyAutomationRunning)
        {
            e.Cancel = true;
            await StopAllAutomationAsync();
            Close();
            return;
        }

        UnregisterAllHotkeys();
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

    private void HandleToggleRequest()
    {
        if (IsAnyAutomationRunning)
        {
            _ = StopAllAutomationAsync();
            return;
        }

        var loopsStarted = false;

        if (leftLoopToggleCheckBox.Checked)
        {
            loopsStarted |= StartMouseLoop(MouseButton.Left, ref _isLeftClickLoopActive,
                ref _leftClickLoopCancellationTokenSource, ref _leftClickLoopTask);
        }

        if (rightLoopToggleCheckBox.Checked)
        {
            loopsStarted |= StartMouseLoop(MouseButton.Right, ref _isRightClickLoopActive,
                ref _rightClickLoopCancellationTokenSource, ref _rightClickLoopTask);
        }

        var playbackAttempted = false;

        if (_actions.Count > 0)
        {
            playbackAttempted = true;
            var wasRunning = _isRunning;
            StartPlayback();

            if (!wasRunning && _isRunning)
            {
                return;
            }
        }

        if (!loopsStarted && !playbackAttempted)
        {
            StartPlayback();
        }
    }

    private sealed record TargetWindowItem(string ProcessName, string Title, int ProcessId, IntPtr Handle)
    {
        public override string ToString()
        {
            return $"{Title} - {ProcessName} (PID: {ProcessId})";
        }
    }
}

namespace ClickerByProgram
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null!;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.targetProcessComboBox = new System.Windows.Forms.ComboBox();
            this.refreshProcessesButton = new System.Windows.Forms.Button();
            this.recordKeyboardButton = new System.Windows.Forms.Button();
            this.stopKeyboardRecordButton = new System.Windows.Forms.Button();
            this.recordMouseButton = new System.Windows.Forms.Button();
            this.stopMouseRecordButton = new System.Windows.Forms.Button();
            this.importActionsButton = new System.Windows.Forms.Button();
            this.exportActionsButton = new System.Windows.Forms.Button();
            this.scriptPreviewLabel = new System.Windows.Forms.Label();
            this.scriptPreviewTextBox = new System.Windows.Forms.TextBox();
            this.startButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.toggleButton = new System.Windows.Forms.Button();
            this.startHotkeyComboBox = new System.Windows.Forms.ComboBox();
            this.stopHotkeyComboBox = new System.Windows.Forms.ComboBox();
            this.toggleHotkeyComboBox = new System.Windows.Forms.ComboBox();
            this.loopCheckBox = new System.Windows.Forms.CheckBox();
            this.loopDelayLabel = new System.Windows.Forms.Label();
            this.loopDelayUpDown = new System.Windows.Forms.NumericUpDown();
            this.actionsListBox = new System.Windows.Forms.ListBox();
            this.actionsLabel = new System.Windows.Forms.Label();
            this.clearActionsButton = new System.Windows.Forms.Button();
            this.statusLabel = new System.Windows.Forms.Label();
            this.mainSplitContainer = new System.Windows.Forms.SplitContainer();
            this.recordingGroupBox = new System.Windows.Forms.GroupBox();
            this.executionGroupBox = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.loopDelayUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).BeginInit();
            this.mainSplitContainer.Panel1.SuspendLayout();
            this.mainSplitContainer.Panel2.SuspendLayout();
            this.mainSplitContainer.SuspendLayout();
            this.recordingGroupBox.SuspendLayout();
            this.executionGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // targetProcessComboBox
            // 
            this.targetProcessComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.targetProcessComboBox.FormattingEnabled = true;
            this.targetProcessComboBox.Location = new System.Drawing.Point(20, 32);
            this.targetProcessComboBox.Name = "targetProcessComboBox";
            this.targetProcessComboBox.Size = new System.Drawing.Size(480, 23);
            this.targetProcessComboBox.TabIndex = 0;
            //
            // refreshProcessesButton
            //
            this.refreshProcessesButton.Location = new System.Drawing.Point(520, 31);
            this.refreshProcessesButton.Name = "refreshProcessesButton";
            this.refreshProcessesButton.Size = new System.Drawing.Size(110, 27);
            this.refreshProcessesButton.TabIndex = 1;
            this.refreshProcessesButton.Text = "Refresh";
            this.refreshProcessesButton.UseVisualStyleBackColor = true;
            this.refreshProcessesButton.Click += new System.EventHandler(this.OnRefreshProcessesClick);
            // 
            // recordKeyboardButton
            // 
            this.recordKeyboardButton.Location = new System.Drawing.Point(20, 80);
            this.recordKeyboardButton.Name = "recordKeyboardButton";
            this.recordKeyboardButton.Size = new System.Drawing.Size(220, 32);
            this.recordKeyboardButton.TabIndex = 4;
            this.recordKeyboardButton.Text = "Record Keyboard";
            this.recordKeyboardButton.UseVisualStyleBackColor = true;
            this.recordKeyboardButton.Click += new System.EventHandler(this.OnRecordKeyboardClick);
            //
            // stopKeyboardRecordButton
            //
            this.stopKeyboardRecordButton.Enabled = false;
            this.stopKeyboardRecordButton.Location = new System.Drawing.Point(260, 80);
            this.stopKeyboardRecordButton.Name = "stopKeyboardRecordButton";
            this.stopKeyboardRecordButton.Size = new System.Drawing.Size(220, 32);
            this.stopKeyboardRecordButton.TabIndex = 5;
            this.stopKeyboardRecordButton.Text = "Stop Keyboard";
            this.stopKeyboardRecordButton.UseVisualStyleBackColor = true;
            this.stopKeyboardRecordButton.Click += new System.EventHandler(this.OnStopKeyboardRecordClick);
            //
            // recordMouseButton
            //
            this.recordMouseButton.Location = new System.Drawing.Point(20, 124);
            this.recordMouseButton.Name = "recordMouseButton";
            this.recordMouseButton.Size = new System.Drawing.Size(220, 32);
            this.recordMouseButton.TabIndex = 6;
            this.recordMouseButton.Text = "Record Mouse";
            this.recordMouseButton.UseVisualStyleBackColor = true;
            this.recordMouseButton.Click += new System.EventHandler(this.OnRecordMouseClick);
            //
            // stopMouseRecordButton
            //
            this.stopMouseRecordButton.Enabled = false;
            this.stopMouseRecordButton.Location = new System.Drawing.Point(260, 124);
            this.stopMouseRecordButton.Name = "stopMouseRecordButton";
            this.stopMouseRecordButton.Size = new System.Drawing.Size(220, 32);
            this.stopMouseRecordButton.TabIndex = 7;
            this.stopMouseRecordButton.Text = "Stop Mouse";
            this.stopMouseRecordButton.UseVisualStyleBackColor = true;
            this.stopMouseRecordButton.Click += new System.EventHandler(this.OnStopMouseRecordClick);
            //
            // importActionsButton
            //
            this.importActionsButton.Location = new System.Drawing.Point(640, 31);
            this.importActionsButton.Name = "importActionsButton";
            this.importActionsButton.Size = new System.Drawing.Size(110, 27);
            this.importActionsButton.TabIndex = 2;
            this.importActionsButton.Text = "Import";
            this.importActionsButton.UseVisualStyleBackColor = true;
            this.importActionsButton.Click += new System.EventHandler(this.OnImportActionsClick);
            //
            // exportActionsButton
            //
            this.exportActionsButton.Location = new System.Drawing.Point(760, 31);
            this.exportActionsButton.Name = "exportActionsButton";
            this.exportActionsButton.Size = new System.Drawing.Size(110, 27);
            this.exportActionsButton.TabIndex = 3;
            this.exportActionsButton.Text = "Export";
            this.exportActionsButton.UseVisualStyleBackColor = true;
            this.exportActionsButton.Click += new System.EventHandler(this.OnExportActionsClick);
            //
            // scriptPreviewLabel
            //
            this.scriptPreviewLabel.AutoSize = true;
            this.scriptPreviewLabel.Location = new System.Drawing.Point(20, 180);
            this.scriptPreviewLabel.Name = "scriptPreviewLabel";
            this.scriptPreviewLabel.Size = new System.Drawing.Size(84, 15);
            this.scriptPreviewLabel.TabIndex = 9;
            this.scriptPreviewLabel.Text = "Script preview";
            //
            // scriptPreviewTextBox
            //
            this.scriptPreviewTextBox.Location = new System.Drawing.Point(20, 200);
            this.scriptPreviewTextBox.Multiline = true;
            this.scriptPreviewTextBox.Name = "scriptPreviewTextBox";
            this.scriptPreviewTextBox.ReadOnly = true;
            this.scriptPreviewTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.scriptPreviewTextBox.Size = new System.Drawing.Size(860, 70);
            this.scriptPreviewTextBox.TabIndex = 10;
            this.scriptPreviewTextBox.TabStop = false;
            //
            // startHotkeyComboBox
            //
            this.startHotkeyComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.startHotkeyComboBox.FormattingEnabled = true;
            this.startHotkeyComboBox.Location = new System.Drawing.Point(20, 30);
            this.startHotkeyComboBox.Name = "startHotkeyComboBox";
            this.startHotkeyComboBox.Size = new System.Drawing.Size(140, 23);
            this.startHotkeyComboBox.TabIndex = 0;
            this.startHotkeyComboBox.SelectedIndexChanged += new System.EventHandler(this.OnStartHotkeySelectionChanged);
            //
            // stopHotkeyComboBox
            //
            this.stopHotkeyComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.stopHotkeyComboBox.FormattingEnabled = true;
            this.stopHotkeyComboBox.Location = new System.Drawing.Point(180, 30);
            this.stopHotkeyComboBox.Name = "stopHotkeyComboBox";
            this.stopHotkeyComboBox.Size = new System.Drawing.Size(140, 23);
            this.stopHotkeyComboBox.TabIndex = 1;
            this.stopHotkeyComboBox.SelectedIndexChanged += new System.EventHandler(this.OnStopHotkeySelectionChanged);
            //
            // toggleHotkeyComboBox
            //
            this.toggleHotkeyComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toggleHotkeyComboBox.FormattingEnabled = true;
            this.toggleHotkeyComboBox.Location = new System.Drawing.Point(340, 30);
            this.toggleHotkeyComboBox.Name = "toggleHotkeyComboBox";
            this.toggleHotkeyComboBox.Size = new System.Drawing.Size(140, 23);
            this.toggleHotkeyComboBox.TabIndex = 2;
            this.toggleHotkeyComboBox.SelectedIndexChanged += new System.EventHandler(this.OnToggleHotkeySelectionChanged);
            //
            // startButton
            //
            this.startButton.Location = new System.Drawing.Point(20, 66);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(140, 32);
            this.startButton.TabIndex = 3;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.OnStartClick);
            //
            // stopButton
            //
            this.stopButton.Enabled = false;
            this.stopButton.Location = new System.Drawing.Point(180, 66);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(140, 32);
            this.stopButton.TabIndex = 4;
            this.stopButton.Text = "Stop";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.OnStopClick);
            //
            // toggleButton
            //
            this.toggleButton.Location = new System.Drawing.Point(340, 66);
            this.toggleButton.Name = "toggleButton";
            this.toggleButton.Size = new System.Drawing.Size(140, 32);
            this.toggleButton.TabIndex = 5;
            this.toggleButton.Text = "Start / Stop";
            this.toggleButton.UseVisualStyleBackColor = true;
            this.toggleButton.Click += new System.EventHandler(this.OnToggleClick);
            //
            // loopCheckBox
            //
            this.loopCheckBox.AutoSize = true;
            this.loopCheckBox.Location = new System.Drawing.Point(20, 112);
            this.loopCheckBox.Name = "loopCheckBox";
            this.loopCheckBox.Size = new System.Drawing.Size(110, 19);
            this.loopCheckBox.TabIndex = 6;
            this.loopCheckBox.Text = "Loop until stop";
            this.loopCheckBox.UseVisualStyleBackColor = true;
            //
            // loopDelayLabel
            //
            this.loopDelayLabel.AutoSize = true;
            this.loopDelayLabel.Location = new System.Drawing.Point(180, 114);
            this.loopDelayLabel.Name = "loopDelayLabel";
            this.loopDelayLabel.Size = new System.Drawing.Size(117, 15);
            this.loopDelayLabel.TabIndex = 7;
            this.loopDelayLabel.Text = "Loop delay (ms):";
            //
            // loopDelayUpDown
            //
            this.loopDelayUpDown.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.loopDelayUpDown.Location = new System.Drawing.Point(320, 110);
            this.loopDelayUpDown.Maximum = new decimal(new int[] {
            600000,
            0,
            0,
            0});
            this.loopDelayUpDown.Name = "loopDelayUpDown";
            this.loopDelayUpDown.Size = new System.Drawing.Size(140, 23);
            this.loopDelayUpDown.TabIndex = 8;
            this.loopDelayUpDown.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            // 
            // actionsListBox
            // 
            this.actionsListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.actionsListBox.FormattingEnabled = true;
            this.actionsListBox.ItemHeight = 15;
            this.actionsListBox.Location = new System.Drawing.Point(0, 0);
            this.actionsListBox.Name = "actionsListBox";
            this.actionsListBox.Size = new System.Drawing.Size(920, 176);
            this.actionsListBox.TabIndex = 12;
            //
            // actionsLabel
            //
            this.actionsLabel.AutoSize = true;
            this.actionsLabel.Location = new System.Drawing.Point(12, 9);
            this.actionsLabel.Name = "actionsLabel";
            this.actionsLabel.Size = new System.Drawing.Size(102, 15);
            this.actionsLabel.TabIndex = 13;
            this.actionsLabel.Text = "Recorded actions";
            //
            // clearActionsButton
            //
            this.clearActionsButton.Location = new System.Drawing.Point(500, 124);
            this.clearActionsButton.Name = "clearActionsButton";
            this.clearActionsButton.Size = new System.Drawing.Size(140, 32);
            this.clearActionsButton.TabIndex = 8;
            this.clearActionsButton.Text = "Clear";
            this.clearActionsButton.UseVisualStyleBackColor = true;
            this.clearActionsButton.Click += new System.EventHandler(this.OnClearActionsClick);
            //
            // statusLabel
            //
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point(12, 684);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(74, 15);
            this.statusLabel.TabIndex = 15;
            this.statusLabel.Text = "Status: Idle";
            //
            // mainSplitContainer
            //
            this.mainSplitContainer.Location = new System.Drawing.Point(12, 27);
            this.mainSplitContainer.Name = "mainSplitContainer";
            this.mainSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            //
            // mainSplitContainer.Panel1
            //
            this.mainSplitContainer.Panel1.Controls.Add(this.recordingGroupBox);
            this.mainSplitContainer.Panel1.Controls.Add(this.executionGroupBox);
            //
            // mainSplitContainer.Panel2
            //
            this.mainSplitContainer.Panel2.Controls.Add(this.actionsListBox);
            this.mainSplitContainer.Size = new System.Drawing.Size(920, 640);
            this.mainSplitContainer.SplitterDistance = 460;
            this.mainSplitContainer.TabIndex = 16;
            //
            // recordingGroupBox
            //
            this.recordingGroupBox.Controls.Add(this.scriptPreviewTextBox);
            this.recordingGroupBox.Controls.Add(this.scriptPreviewLabel);
            this.recordingGroupBox.Controls.Add(this.exportActionsButton);
            this.recordingGroupBox.Controls.Add(this.importActionsButton);
            this.recordingGroupBox.Controls.Add(this.targetProcessComboBox);
            this.recordingGroupBox.Controls.Add(this.refreshProcessesButton);
            this.recordingGroupBox.Controls.Add(this.recordKeyboardButton);
            this.recordingGroupBox.Controls.Add(this.stopKeyboardRecordButton);
            this.recordingGroupBox.Controls.Add(this.recordMouseButton);
            this.recordingGroupBox.Controls.Add(this.stopMouseRecordButton);
            this.recordingGroupBox.Controls.Add(this.clearActionsButton);
            this.recordingGroupBox.Location = new System.Drawing.Point(10, 10);
            this.recordingGroupBox.Name = "recordingGroupBox";
            this.recordingGroupBox.Size = new System.Drawing.Size(900, 290);
            this.recordingGroupBox.TabIndex = 17;
            this.recordingGroupBox.TabStop = false;
            this.recordingGroupBox.Text = "Recording";
            //
            // executionGroupBox
            //
            this.executionGroupBox.Controls.Add(this.startHotkeyComboBox);
            this.executionGroupBox.Controls.Add(this.stopHotkeyComboBox);
            this.executionGroupBox.Controls.Add(this.toggleHotkeyComboBox);
            this.executionGroupBox.Controls.Add(this.startButton);
            this.executionGroupBox.Controls.Add(this.stopButton);
            this.executionGroupBox.Controls.Add(this.toggleButton);
            this.executionGroupBox.Controls.Add(this.loopCheckBox);
            this.executionGroupBox.Controls.Add(this.loopDelayLabel);
            this.executionGroupBox.Controls.Add(this.loopDelayUpDown);
            this.executionGroupBox.Location = new System.Drawing.Point(10, 310);
            this.executionGroupBox.Name = "executionGroupBox";
            this.executionGroupBox.Size = new System.Drawing.Size(900, 150);
            this.executionGroupBox.TabIndex = 18;
            this.executionGroupBox.TabStop = false;
            this.executionGroupBox.Text = "Execution";
            //
            // MainForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(960, 720);
            this.Controls.Add(this.mainSplitContainer);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.actionsLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Program Clicker";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.loopDelayUpDown)).EndInit();
            this.mainSplitContainer.Panel1.ResumeLayout(false);
            this.mainSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).EndInit();
            this.mainSplitContainer.ResumeLayout(false);
            this.recordingGroupBox.ResumeLayout(false);
            this.executionGroupBox.ResumeLayout(false);
            this.executionGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ComboBox targetProcessComboBox;
        private System.Windows.Forms.Button refreshProcessesButton;
        private System.Windows.Forms.Button recordKeyboardButton;
        private System.Windows.Forms.Button stopKeyboardRecordButton;
        private System.Windows.Forms.Button recordMouseButton;
        private System.Windows.Forms.Button stopMouseRecordButton;
        private System.Windows.Forms.Button importActionsButton;
        private System.Windows.Forms.Button exportActionsButton;
        private System.Windows.Forms.Label scriptPreviewLabel;
        private System.Windows.Forms.TextBox scriptPreviewTextBox;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Button toggleButton;
        private System.Windows.Forms.ComboBox startHotkeyComboBox;
        private System.Windows.Forms.ComboBox stopHotkeyComboBox;
        private System.Windows.Forms.ComboBox toggleHotkeyComboBox;
        private System.Windows.Forms.CheckBox loopCheckBox;
        private System.Windows.Forms.Label loopDelayLabel;
        private System.Windows.Forms.NumericUpDown loopDelayUpDown;
        private System.Windows.Forms.ListBox actionsListBox;
        private System.Windows.Forms.Label actionsLabel;
        private System.Windows.Forms.Button clearActionsButton;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.SplitContainer mainSplitContainer;
        private System.Windows.Forms.GroupBox recordingGroupBox;
        private System.Windows.Forms.GroupBox executionGroupBox;
    }
}

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
            this.startButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.toggleButton = new System.Windows.Forms.Button();
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
            this.targetProcessComboBox.Size = new System.Drawing.Size(420, 23);
            this.targetProcessComboBox.TabIndex = 0;
            //
            // refreshProcessesButton
            //
            this.refreshProcessesButton.Location = new System.Drawing.Point(460, 31);
            this.refreshProcessesButton.Name = "refreshProcessesButton";
            this.refreshProcessesButton.Size = new System.Drawing.Size(140, 27);
            this.refreshProcessesButton.TabIndex = 1;
            this.refreshProcessesButton.Text = "Refresh";
            this.refreshProcessesButton.UseVisualStyleBackColor = true;
            this.refreshProcessesButton.Click += new System.EventHandler(this.OnRefreshProcessesClick);
            // 
            // recordKeyboardButton
            // 
            this.recordKeyboardButton.Location = new System.Drawing.Point(20, 76);
            this.recordKeyboardButton.Name = "recordKeyboardButton";
            this.recordKeyboardButton.Size = new System.Drawing.Size(200, 32);
            this.recordKeyboardButton.TabIndex = 2;
            this.recordKeyboardButton.Text = "Record Keyboard";
            this.recordKeyboardButton.UseVisualStyleBackColor = true;
            this.recordKeyboardButton.Click += new System.EventHandler(this.OnRecordKeyboardClick);
            //
            // stopKeyboardRecordButton
            //
            this.stopKeyboardRecordButton.Enabled = false;
            this.stopKeyboardRecordButton.Location = new System.Drawing.Point(240, 76);
            this.stopKeyboardRecordButton.Name = "stopKeyboardRecordButton";
            this.stopKeyboardRecordButton.Size = new System.Drawing.Size(200, 32);
            this.stopKeyboardRecordButton.TabIndex = 3;
            this.stopKeyboardRecordButton.Text = "Stop Keyboard";
            this.stopKeyboardRecordButton.UseVisualStyleBackColor = true;
            this.stopKeyboardRecordButton.Click += new System.EventHandler(this.OnStopKeyboardRecordClick);
            //
            // recordMouseButton
            //
            this.recordMouseButton.Location = new System.Drawing.Point(20, 120);
            this.recordMouseButton.Name = "recordMouseButton";
            this.recordMouseButton.Size = new System.Drawing.Size(200, 32);
            this.recordMouseButton.TabIndex = 4;
            this.recordMouseButton.Text = "Record Mouse";
            this.recordMouseButton.UseVisualStyleBackColor = true;
            this.recordMouseButton.Click += new System.EventHandler(this.OnRecordMouseClick);
            //
            // stopMouseRecordButton
            //
            this.stopMouseRecordButton.Enabled = false;
            this.stopMouseRecordButton.Location = new System.Drawing.Point(240, 120);
            this.stopMouseRecordButton.Name = "stopMouseRecordButton";
            this.stopMouseRecordButton.Size = new System.Drawing.Size(200, 32);
            this.stopMouseRecordButton.TabIndex = 5;
            this.stopMouseRecordButton.Text = "Stop Mouse";
            this.stopMouseRecordButton.UseVisualStyleBackColor = true;
            this.stopMouseRecordButton.Click += new System.EventHandler(this.OnStopMouseRecordClick);
            //
            // startButton
            //
            this.startButton.Location = new System.Drawing.Point(20, 32);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(140, 32);
            this.startButton.TabIndex = 6;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.OnStartClick);
            //
            // stopButton
            //
            this.stopButton.Enabled = false;
            this.stopButton.Location = new System.Drawing.Point(180, 32);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(140, 32);
            this.stopButton.TabIndex = 7;
            this.stopButton.Text = "Stop";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.OnStopClick);
            //
            // toggleButton
            //
            this.toggleButton.Location = new System.Drawing.Point(340, 32);
            this.toggleButton.Name = "toggleButton";
            this.toggleButton.Size = new System.Drawing.Size(140, 32);
            this.toggleButton.TabIndex = 8;
            this.toggleButton.Text = "Start / Stop";
            this.toggleButton.UseVisualStyleBackColor = true;
            this.toggleButton.Click += new System.EventHandler(this.OnToggleClick);
            //
            // loopCheckBox
            //
            this.loopCheckBox.AutoSize = true;
            this.loopCheckBox.Location = new System.Drawing.Point(20, 90);
            this.loopCheckBox.Name = "loopCheckBox";
            this.loopCheckBox.Size = new System.Drawing.Size(110, 19);
            this.loopCheckBox.TabIndex = 9;
            this.loopCheckBox.Text = "Loop until stop";
            this.loopCheckBox.UseVisualStyleBackColor = true;
            //
            // loopDelayLabel
            //
            this.loopDelayLabel.AutoSize = true;
            this.loopDelayLabel.Location = new System.Drawing.Point(180, 91);
            this.loopDelayLabel.Name = "loopDelayLabel";
            this.loopDelayLabel.Size = new System.Drawing.Size(117, 15);
            this.loopDelayLabel.TabIndex = 10;
            this.loopDelayLabel.Text = "Loop delay (ms):";
            //
            // loopDelayUpDown
            //
            this.loopDelayUpDown.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.loopDelayUpDown.Location = new System.Drawing.Point(320, 88);
            this.loopDelayUpDown.Maximum = new decimal(new int[] {
            600000,
            0,
            0,
            0});
            this.loopDelayUpDown.Name = "loopDelayUpDown";
            this.loopDelayUpDown.Size = new System.Drawing.Size(140, 23);
            this.loopDelayUpDown.TabIndex = 11;
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
            this.actionsListBox.Size = new System.Drawing.Size(760, 276);
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
            this.clearActionsButton.Location = new System.Drawing.Point(460, 120);
            this.clearActionsButton.Name = "clearActionsButton";
            this.clearActionsButton.Size = new System.Drawing.Size(140, 32);
            this.clearActionsButton.TabIndex = 14;
            this.clearActionsButton.Text = "Clear";
            this.clearActionsButton.UseVisualStyleBackColor = true;
            this.clearActionsButton.Click += new System.EventHandler(this.OnClearActionsClick);
            //
            // statusLabel
            //
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point(12, 522);
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
            this.mainSplitContainer.Size = new System.Drawing.Size(760, 480);
            this.mainSplitContainer.SplitterDistance = 330;
            this.mainSplitContainer.TabIndex = 16;
            //
            // recordingGroupBox
            //
            this.recordingGroupBox.Controls.Add(this.targetProcessComboBox);
            this.recordingGroupBox.Controls.Add(this.refreshProcessesButton);
            this.recordingGroupBox.Controls.Add(this.recordKeyboardButton);
            this.recordingGroupBox.Controls.Add(this.stopKeyboardRecordButton);
            this.recordingGroupBox.Controls.Add(this.recordMouseButton);
            this.recordingGroupBox.Controls.Add(this.stopMouseRecordButton);
            this.recordingGroupBox.Controls.Add(this.clearActionsButton);
            this.recordingGroupBox.Location = new System.Drawing.Point(10, 10);
            this.recordingGroupBox.Name = "recordingGroupBox";
            this.recordingGroupBox.Size = new System.Drawing.Size(730, 170);
            this.recordingGroupBox.TabIndex = 17;
            this.recordingGroupBox.TabStop = false;
            this.recordingGroupBox.Text = "Recording";
            //
            // executionGroupBox
            //
            this.executionGroupBox.Controls.Add(this.startButton);
            this.executionGroupBox.Controls.Add(this.stopButton);
            this.executionGroupBox.Controls.Add(this.toggleButton);
            this.executionGroupBox.Controls.Add(this.loopCheckBox);
            this.executionGroupBox.Controls.Add(this.loopDelayLabel);
            this.executionGroupBox.Controls.Add(this.loopDelayUpDown);
            this.executionGroupBox.Location = new System.Drawing.Point(10, 200);
            this.executionGroupBox.Name = "executionGroupBox";
            this.executionGroupBox.Size = new System.Drawing.Size(730, 120);
            this.executionGroupBox.TabIndex = 18;
            this.executionGroupBox.TabStop = false;
            this.executionGroupBox.Text = "Execution";
            //
            // MainForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
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
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Button toggleButton;
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

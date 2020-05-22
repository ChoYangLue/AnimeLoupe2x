namespace AnimeLoupe2x
{
    partial class AnimeLoupe2x
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.LogText = new System.Windows.Forms.TextBox();
            this.RunButton = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.CommandGroup = new System.Windows.Forms.GroupBox();
            this.V2ICheckBox = new System.Windows.Forms.CheckBox();
            this.V2ACheckBox = new System.Windows.Forms.CheckBox();
            this.Waifu2xCheckBox = new System.Windows.Forms.CheckBox();
            this.AddAudioCheckBox = new System.Windows.Forms.CheckBox();
            this.I2VCheckBox = new System.Windows.Forms.CheckBox();
            this.OutputGroup = new System.Windows.Forms.GroupBox();
            this.SaveButton = new System.Windows.Forms.Button();
            this.outputPath = new System.Windows.Forms.TextBox();
            this.InputGroup = new System.Windows.Forms.GroupBox();
            this.InputPathButton = new System.Windows.Forms.Button();
            this.inputPath = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.TempFilePathGroup = new System.Windows.Forms.GroupBox();
            this.TempFilePathText = new System.Windows.Forms.TextBox();
            this.ClearTempButton = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.CommandGroup.SuspendLayout();
            this.OutputGroup.SuspendLayout();
            this.InputGroup.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.TempFilePathGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // LogText
            // 
            this.LogText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LogText.Location = new System.Drawing.Point(0, 320);
            this.LogText.Multiline = true;
            this.LogText.Name = "LogText";
            this.LogText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.LogText.Size = new System.Drawing.Size(1163, 398);
            this.LogText.TabIndex = 0;
            // 
            // RunButton
            // 
            this.RunButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.RunButton.Font = new System.Drawing.Font("MS UI Gothic", 12F);
            this.RunButton.Location = new System.Drawing.Point(1078, 234);
            this.RunButton.Name = "RunButton";
            this.RunButton.Size = new System.Drawing.Size(73, 80);
            this.RunButton.TabIndex = 1;
            this.RunButton.Text = "Run";
            this.RunButton.UseVisualStyleBackColor = true;
            this.RunButton.Click += new System.EventHandler(this.RunButton_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(0, 2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1072, 312);
            this.tabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tabPage1.Controls.Add(this.CommandGroup);
            this.tabPage1.Controls.Add(this.OutputGroup);
            this.tabPage1.Controls.Add(this.InputGroup);
            this.tabPage1.Location = new System.Drawing.Point(4, 28);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1064, 280);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "command setting";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // CommandGroup
            // 
            this.CommandGroup.Controls.Add(this.V2ICheckBox);
            this.CommandGroup.Controls.Add(this.V2ACheckBox);
            this.CommandGroup.Controls.Add(this.Waifu2xCheckBox);
            this.CommandGroup.Controls.Add(this.AddAudioCheckBox);
            this.CommandGroup.Controls.Add(this.I2VCheckBox);
            this.CommandGroup.Location = new System.Drawing.Point(30, 26);
            this.CommandGroup.Name = "CommandGroup";
            this.CommandGroup.Size = new System.Drawing.Size(187, 174);
            this.CommandGroup.TabIndex = 11;
            this.CommandGroup.TabStop = false;
            this.CommandGroup.Text = "Command";
            // 
            // V2ICheckBox
            // 
            this.V2ICheckBox.AutoSize = true;
            this.V2ICheckBox.Location = new System.Drawing.Point(6, 24);
            this.V2ICheckBox.Name = "V2ICheckBox";
            this.V2ICheckBox.Size = new System.Drawing.Size(128, 22);
            this.V2ICheckBox.TabIndex = 3;
            this.V2ICheckBox.Text = "Video2Image";
            this.V2ICheckBox.UseVisualStyleBackColor = true;
            // 
            // V2ACheckBox
            // 
            this.V2ACheckBox.AutoSize = true;
            this.V2ACheckBox.Location = new System.Drawing.Point(6, 52);
            this.V2ACheckBox.Name = "V2ACheckBox";
            this.V2ACheckBox.Size = new System.Drawing.Size(127, 22);
            this.V2ACheckBox.TabIndex = 4;
            this.V2ACheckBox.Text = "Video2Audio";
            this.V2ACheckBox.UseVisualStyleBackColor = true;
            // 
            // Waifu2xCheckBox
            // 
            this.Waifu2xCheckBox.AutoSize = true;
            this.Waifu2xCheckBox.Location = new System.Drawing.Point(6, 81);
            this.Waifu2xCheckBox.Name = "Waifu2xCheckBox";
            this.Waifu2xCheckBox.Size = new System.Drawing.Size(91, 22);
            this.Waifu2xCheckBox.TabIndex = 5;
            this.Waifu2xCheckBox.Text = "Waifu2x";
            this.Waifu2xCheckBox.UseVisualStyleBackColor = true;
            // 
            // AddAudioCheckBox
            // 
            this.AddAudioCheckBox.AutoSize = true;
            this.AddAudioCheckBox.Location = new System.Drawing.Point(6, 139);
            this.AddAudioCheckBox.Name = "AddAudioCheckBox";
            this.AddAudioCheckBox.Size = new System.Drawing.Size(105, 22);
            this.AddAudioCheckBox.TabIndex = 8;
            this.AddAudioCheckBox.Text = "AddAudio";
            this.AddAudioCheckBox.UseVisualStyleBackColor = true;
            // 
            // I2VCheckBox
            // 
            this.I2VCheckBox.AutoSize = true;
            this.I2VCheckBox.Location = new System.Drawing.Point(6, 110);
            this.I2VCheckBox.Name = "I2VCheckBox";
            this.I2VCheckBox.Size = new System.Drawing.Size(128, 22);
            this.I2VCheckBox.TabIndex = 7;
            this.I2VCheckBox.Text = "Image2Video";
            this.I2VCheckBox.UseVisualStyleBackColor = true;
            // 
            // OutputGroup
            // 
            this.OutputGroup.Controls.Add(this.SaveButton);
            this.OutputGroup.Controls.Add(this.outputPath);
            this.OutputGroup.Location = new System.Drawing.Point(242, 128);
            this.OutputGroup.Name = "OutputGroup";
            this.OutputGroup.Size = new System.Drawing.Size(635, 72);
            this.OutputGroup.TabIndex = 10;
            this.OutputGroup.TabStop = false;
            this.OutputGroup.Text = "File Output";
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(541, 33);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(75, 28);
            this.SaveButton.TabIndex = 7;
            this.SaveButton.Text = "Save";
            this.SaveButton.UseVisualStyleBackColor = true;
            // 
            // outputPath
            // 
            this.outputPath.Location = new System.Drawing.Point(6, 36);
            this.outputPath.Name = "outputPath";
            this.outputPath.Size = new System.Drawing.Size(529, 25);
            this.outputPath.TabIndex = 6;
            // 
            // InputGroup
            // 
            this.InputGroup.Controls.Add(this.InputPathButton);
            this.InputGroup.Controls.Add(this.inputPath);
            this.InputGroup.Location = new System.Drawing.Point(242, 26);
            this.InputGroup.Name = "InputGroup";
            this.InputGroup.Size = new System.Drawing.Size(635, 77);
            this.InputGroup.TabIndex = 9;
            this.InputGroup.TabStop = false;
            this.InputGroup.Text = "File Input";
            // 
            // InputPathButton
            // 
            this.InputPathButton.Location = new System.Drawing.Point(541, 32);
            this.InputPathButton.Name = "InputPathButton";
            this.InputPathButton.Size = new System.Drawing.Size(75, 32);
            this.InputPathButton.TabIndex = 3;
            this.InputPathButton.Text = "Open";
            this.InputPathButton.UseVisualStyleBackColor = true;
            this.InputPathButton.Click += new System.EventHandler(this.InputPathButton_Click);
            // 
            // inputPath
            // 
            this.inputPath.Location = new System.Drawing.Point(6, 36);
            this.inputPath.Name = "inputPath";
            this.inputPath.Size = new System.Drawing.Size(529, 25);
            this.inputPath.TabIndex = 2;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 28);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1064, 280);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "waifu2x setting";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.ClearTempButton);
            this.tabPage3.Controls.Add(this.TempFilePathGroup);
            this.tabPage3.Location = new System.Drawing.Point(4, 28);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(1064, 280);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "temp setting";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // TempFilePathGroup
            // 
            this.TempFilePathGroup.Controls.Add(this.TempFilePathText);
            this.TempFilePathGroup.Location = new System.Drawing.Point(8, 20);
            this.TempFilePathGroup.Name = "TempFilePathGroup";
            this.TempFilePathGroup.Size = new System.Drawing.Size(543, 69);
            this.TempFilePathGroup.TabIndex = 0;
            this.TempFilePathGroup.TabStop = false;
            this.TempFilePathGroup.Text = "Temp File Path";
            // 
            // TempFilePathText
            // 
            this.TempFilePathText.Location = new System.Drawing.Point(6, 38);
            this.TempFilePathText.Name = "TempFilePathText";
            this.TempFilePathText.Size = new System.Drawing.Size(446, 25);
            this.TempFilePathText.TabIndex = 0;
            // 
            // ClearTempButton
            // 
            this.ClearTempButton.Location = new System.Drawing.Point(576, 58);
            this.ClearTempButton.Name = "ClearTempButton";
            this.ClearTempButton.Size = new System.Drawing.Size(159, 25);
            this.ClearTempButton.TabIndex = 1;
            this.ClearTempButton.Text = "Clear Temp File";
            this.ClearTempButton.UseVisualStyleBackColor = true;
            // 
            // CancelButton
            // 
            this.CancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelButton.Location = new System.Drawing.Point(1078, 148);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(73, 80);
            this.CancelButton.TabIndex = 4;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // AnimeLoupe2x
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1163, 719);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.RunButton);
            this.Controls.Add(this.LogText);
            this.Name = "AnimeLoupe2x";
            this.Text = "AnimeLoupe2x";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AnimeLoupe2x_FormClosing);
            this.Load += new System.EventHandler(this.AnimeLoupe2x_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.CommandGroup.ResumeLayout(false);
            this.CommandGroup.PerformLayout();
            this.OutputGroup.ResumeLayout(false);
            this.OutputGroup.PerformLayout();
            this.InputGroup.ResumeLayout(false);
            this.InputGroup.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.TempFilePathGroup.ResumeLayout(false);
            this.TempFilePathGroup.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox LogText;
        private System.Windows.Forms.Button RunButton;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox inputPath;
        private System.Windows.Forms.CheckBox V2ICheckBox;
        private System.Windows.Forms.CheckBox V2ACheckBox;
        private System.Windows.Forms.CheckBox Waifu2xCheckBox;
        private System.Windows.Forms.TextBox outputPath;
        private System.Windows.Forms.CheckBox I2VCheckBox;
        private System.Windows.Forms.CheckBox AddAudioCheckBox;
        private System.Windows.Forms.GroupBox OutputGroup;
        private System.Windows.Forms.GroupBox InputGroup;
        private System.Windows.Forms.Button InputPathButton;
        private System.Windows.Forms.GroupBox CommandGroup;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button ClearTempButton;
        private System.Windows.Forms.GroupBox TempFilePathGroup;
        private System.Windows.Forms.TextBox TempFilePathText;
        private System.Windows.Forms.Button CancelButton;
    }
}


// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

namespace RobotComponents.Gh.Forms
{
    partial class PickTaskForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.labelTaskType = new System.Windows.Forms.Label();
            this.labelName = new System.Windows.Forms.Label();
            this.labelNameInfo = new System.Windows.Forms.Label();
            this.labelTaskTypeInfo = new System.Windows.Forms.Label();
            this.labelEnabledInfo = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelEnabled = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(12, 303);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(476, 33);
            this.button1.TabIndex = 1;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.125F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(203, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Choose a task to upload to";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownWidth = 476;
            this.comboBox1.Location = new System.Drawing.Point(12, 60);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(474, 21);
            this.comboBox1.TabIndex = 3;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.ComboBox1_SelectedIndexChanged_1);
            // 
            // labelTaskType
            // 
            this.labelTaskType.AutoSize = true;
            this.labelTaskType.Location = new System.Drawing.Point(13, 165);
            this.labelTaskType.Name = "labelTaskType";
            this.labelTaskType.Size = new System.Drawing.Size(72, 13);
            this.labelTaskType.TabIndex = 14;
            this.labelTaskType.Text = "Task Type";
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Location = new System.Drawing.Point(12, 140);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(35, 13);
            this.labelName.TabIndex = 13;
            this.labelName.Text = "Name";
            // 
            // labelNameInfo
            // 
            this.labelNameInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelNameInfo.Location = new System.Drawing.Point(198, 140);
            this.labelNameInfo.Name = "labelNameInfo";
            this.labelNameInfo.Size = new System.Drawing.Size(287, 25);
            this.labelNameInfo.TabIndex = 18;
            this.labelNameInfo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelSystemNameInfo
            // 
            this.labelTaskTypeInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTaskTypeInfo.Location = new System.Drawing.Point(203, 165);
            this.labelTaskTypeInfo.Name = "labelTaskTypeInfo";
            this.labelTaskTypeInfo.Size = new System.Drawing.Size(282, 25);
            this.labelTaskTypeInfo.TabIndex = 19;
            this.labelTaskTypeInfo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelEnabledInfo
            // 
            this.labelEnabledInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelEnabledInfo.Location = new System.Drawing.Point(203, 190);
            this.labelEnabledInfo.Name = "labelEnabledInfo";
            this.labelEnabledInfo.Size = new System.Drawing.Size(282, 25);
            this.labelEnabledInfo.TabIndex = 20;
            this.labelEnabledInfo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 115);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 13);
            this.label2.TabIndex = 23;
            this.label2.Text = "Task info";
            // 
            // labelEnabled
            // 
            this.labelEnabled.AutoSize = true;
            this.labelEnabled.Location = new System.Drawing.Point(13, 190);
            this.labelEnabled.Name = "labelEnabled";
            this.labelEnabled.Size = new System.Drawing.Size(52, 13);
            this.labelEnabled.TabIndex = 15;
            this.labelEnabled.Text = "Enabled";
            // 
            // PickTaskForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(500, 350);
            this.ControlBox = false;
            this.Controls.Add(this.labelEnabledInfo);
            this.Controls.Add(this.labelTaskTypeInfo);
            this.Controls.Add(this.labelNameInfo);
            this.Controls.Add(this.labelEnabled);
            this.Controls.Add(this.labelTaskType);
            this.Controls.Add(this.labelName);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "PickTaskForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label labelTaskType;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.Label labelNameInfo;
        private System.Windows.Forms.Label labelTaskTypeInfo;
        private System.Windows.Forms.Label labelEnabledInfo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelEnabled;
    }
}
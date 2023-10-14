// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

namespace RobotComponents.ABB.Controllers.Forms
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
            this.labelTitle = new System.Windows.Forms.Label();
            this.comboBox = new System.Windows.Forms.ComboBox();
            this.labelTaskType = new System.Windows.Forms.Label();
            this.labelName = new System.Windows.Forms.Label();
            this.labelNameInfo = new System.Windows.Forms.Label();
            this.labelTaskTypeInfo = new System.Windows.Forms.Label();
            this.labelEnabledInfo = new System.Windows.Forms.Label();
            this.labelTaskInfo = new System.Windows.Forms.Label();
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
            this.button1.Click += new System.EventHandler(this.Button1Click);
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.125F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTitle.Location = new System.Drawing.Point(12, 18);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(203, 17);
            this.labelTitle.TabIndex = 2;
            this.labelTitle.Text = "Choose a task to upload to";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // comboBox
            // 
            this.comboBox.DropDownWidth = 476;
            this.comboBox.Location = new System.Drawing.Point(12, 60);
            this.comboBox.Name = "comboBox";
            this.comboBox.Size = new System.Drawing.Size(474, 21);
            this.comboBox.TabIndex = 3;
            this.comboBox.SelectedIndexChanged += new System.EventHandler(this.ComboBoxSelectedIndexChanged);
            // 
            // labelTaskType
            // 
            this.labelTaskType.AutoSize = true;
            this.labelTaskType.Location = new System.Drawing.Point(13, 165);
            this.labelTaskType.Name = "labelTaskType";
            this.labelTaskType.Size = new System.Drawing.Size(58, 13);
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
            this.labelNameInfo.Size = new System.Drawing.Size(287, 13);
            this.labelNameInfo.TabIndex = 18;
            this.labelNameInfo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelTaskTypeInfo
            // 
            this.labelTaskTypeInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTaskTypeInfo.Location = new System.Drawing.Point(203, 165);
            this.labelTaskTypeInfo.Name = "labelTaskTypeInfo";
            this.labelTaskTypeInfo.Size = new System.Drawing.Size(282, 13);
            this.labelTaskTypeInfo.TabIndex = 19;
            this.labelTaskTypeInfo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelEnabledInfo
            // 
            this.labelEnabledInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelEnabledInfo.Location = new System.Drawing.Point(203, 190);
            this.labelEnabledInfo.Name = "labelEnabledInfo";
            this.labelEnabledInfo.Size = new System.Drawing.Size(282, 13);
            this.labelEnabledInfo.TabIndex = 20;
            this.labelEnabledInfo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelTaskInfo
            // 
            this.labelTaskInfo.AutoSize = true;
            this.labelTaskInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTaskInfo.Location = new System.Drawing.Point(12, 115);
            this.labelTaskInfo.Name = "labelTaskInfo";
            this.labelTaskInfo.Size = new System.Drawing.Size(60, 13);
            this.labelTaskInfo.TabIndex = 23;
            this.labelTaskInfo.Text = "Task info";
            // 
            // labelEnabled
            // 
            this.labelEnabled.AutoSize = true;
            this.labelEnabled.Location = new System.Drawing.Point(13, 190);
            this.labelEnabled.Name = "labelEnabled";
            this.labelEnabled.Size = new System.Drawing.Size(46, 13);
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
            this.Controls.Add(this.comboBox);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.labelTaskInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "PickTaskForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.ComboBox comboBox;
        private System.Windows.Forms.Label labelTaskType;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.Label labelNameInfo;
        private System.Windows.Forms.Label labelTaskTypeInfo;
        private System.Windows.Forms.Label labelEnabledInfo;
        private System.Windows.Forms.Label labelTaskInfo;
        private System.Windows.Forms.Label labelEnabled;
    }
}
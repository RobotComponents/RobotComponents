// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

namespace RobotComponents.Gh.Forms
{
    partial class PickDOForm
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
            this.labelName = new System.Windows.Forms.Label();
            this.labelValue = new System.Windows.Forms.Label();
            this.labelNameInfo = new System.Windows.Forms.Label();
            this.labelValueInfo = new System.Windows.Forms.Label();
            this.labelType = new System.Windows.Forms.Label();
            this.labelMinValue = new System.Windows.Forms.Label();
            this.labelTypeInfo = new System.Windows.Forms.Label();
            this.labelMinValueInfo = new System.Windows.Forms.Label();
            this.labelMaxValue = new System.Windows.Forms.Label();
            this.labelMaxValueInfo = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.button1.Location = new System.Drawing.Point(12, 303);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(476, 33);
            this.button1.TabIndex = 1;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Choose a signal";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(12, 60);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(476, 21);
            this.comboBox1.TabIndex = 3;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Location = new System.Drawing.Point(12, 140);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(35, 13);
            this.labelName.TabIndex = 4;
            this.labelName.Text = "Name";
            // 
            // labelValue
            // 
            this.labelValue.AutoSize = true;
            this.labelValue.Location = new System.Drawing.Point(13, 165);
            this.labelValue.Name = "labelValue";
            this.labelValue.Size = new System.Drawing.Size(34, 13);
            this.labelValue.TabIndex = 5;
            this.labelValue.Text = "Value";
            // 
            // labelNameInfo
            // 
            this.labelNameInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelNameInfo.ImageAlign = System.Drawing.ContentAlignment.TopRight;
            this.labelNameInfo.Location = new System.Drawing.Point(164, 140);
            this.labelNameInfo.Name = "labelNameInfo";
            this.labelNameInfo.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.labelNameInfo.Size = new System.Drawing.Size(322, 32);
            this.labelNameInfo.TabIndex = 6;
            this.labelNameInfo.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelValueInfo
            // 
            this.labelValueInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelValueInfo.ImageAlign = System.Drawing.ContentAlignment.TopRight;
            this.labelValueInfo.Location = new System.Drawing.Point(164, 165);
            this.labelValueInfo.Name = "labelValueInfo";
            this.labelValueInfo.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.labelValueInfo.Size = new System.Drawing.Size(322, 25);
            this.labelValueInfo.TabIndex = 7;
            this.labelValueInfo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelType
            // 
            this.labelType.AutoSize = true;
            this.labelType.Location = new System.Drawing.Point(13, 190);
            this.labelType.Name = "labelType";
            this.labelType.Size = new System.Drawing.Size(31, 13);
            this.labelType.TabIndex = 8;
            this.labelType.Text = "Type";
            // 
            // labelMinValue
            // 
            this.labelMinValue.AutoSize = true;
            this.labelMinValue.Location = new System.Drawing.Point(13, 215);
            this.labelMinValue.Name = "labelMinValue";
            this.labelMinValue.Size = new System.Drawing.Size(54, 13);
            this.labelMinValue.TabIndex = 9;
            this.labelMinValue.Text = "Min Value";
            // 
            // labelTypeInfo
            // 
            this.labelTypeInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTypeInfo.Location = new System.Drawing.Point(164, 190);
            this.labelTypeInfo.Name = "labelTypeInfo";
            this.labelTypeInfo.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.labelTypeInfo.Size = new System.Drawing.Size(322, 25);
            this.labelTypeInfo.TabIndex = 10;
            this.labelTypeInfo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelMinValueInfo
            // 
            this.labelMinValueInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelMinValueInfo.Location = new System.Drawing.Point(164, 215);
            this.labelMinValueInfo.Name = "labelMinValueInfo";
            this.labelMinValueInfo.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.labelMinValueInfo.Size = new System.Drawing.Size(322, 25);
            this.labelMinValueInfo.TabIndex = 11;
            this.labelMinValueInfo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelMaxValue
            // 
            this.labelMaxValue.AutoSize = true;
            this.labelMaxValue.Location = new System.Drawing.Point(13, 240);
            this.labelMaxValue.Name = "labelMaxValue";
            this.labelMaxValue.Size = new System.Drawing.Size(57, 13);
            this.labelMaxValue.TabIndex = 12;
            this.labelMaxValue.Text = "Max Value";
            // 
            // labelMaxValueInfo
            // 
            this.labelMaxValueInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelMaxValueInfo.Location = new System.Drawing.Point(164, 240);
            this.labelMaxValueInfo.Name = "labelMaxValueInfo";
            this.labelMaxValueInfo.Size = new System.Drawing.Size(324, 25);
            this.labelMaxValueInfo.TabIndex = 13;
            this.labelMaxValueInfo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(13, 115);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Signal info";
            // 
            // PickDOForm
            // 
            this.ClientSize = new System.Drawing.Size(500, 350);
            this.ControlBox = false;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelMaxValueInfo);
            this.Controls.Add(this.labelMaxValue);
            this.Controls.Add(this.labelMinValueInfo);
            this.Controls.Add(this.labelTypeInfo);
            this.Controls.Add(this.labelMinValue);
            this.Controls.Add(this.labelType);
            this.Controls.Add(this.labelValueInfo);
            this.Controls.Add(this.labelNameInfo);
            this.Controls.Add(this.labelValue);
            this.Controls.Add(this.labelName);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "PickDOForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.Label labelValue;
        private System.Windows.Forms.Label labelNameInfo;
        private System.Windows.Forms.Label labelValueInfo;
        private System.Windows.Forms.Label labelType;
        private System.Windows.Forms.Label labelMinValue;
        private System.Windows.Forms.Label labelTypeInfo;
        private System.Windows.Forms.Label labelMinValueInfo;
        private System.Windows.Forms.Label labelMaxValue;
        private System.Windows.Forms.Label labelMaxValueInfo;
        private System.Windows.Forms.Label label2;
    }
}
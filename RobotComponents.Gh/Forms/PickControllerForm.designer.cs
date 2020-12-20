// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

namespace RobotComponents.Gh.Forms
{
    partial class PickControllerForm
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
            this.labelOperationMode = new System.Windows.Forms.Label();
            this.labelIsVirtual = new System.Windows.Forms.Label();
            this.labelIP = new System.Windows.Forms.Label();
            this.labelSystemName = new System.Windows.Forms.Label();
            this.labelName = new System.Windows.Forms.Label();
            this.labelNameInfo = new System.Windows.Forms.Label();
            this.labelSystemNameInfo = new System.Windows.Forms.Label();
            this.labelIPInfo = new System.Windows.Forms.Label();
            this.labelIsVirtualInfo = new System.Windows.Forms.Label();
            this.labelOperationModeInfo = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
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
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.125F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(250, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Choose a controller to connect to";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownWidth = 476;
            this.comboBox1.Location = new System.Drawing.Point(12, 60);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(474, 21);
            this.comboBox1.TabIndex = 3;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged_1);
            // 
            // labelOperationMode
            // 
            this.labelOperationMode.AutoSize = true;
            this.labelOperationMode.Location = new System.Drawing.Point(13, 240);
            this.labelOperationMode.Name = "labelOperationMode";
            this.labelOperationMode.Size = new System.Drawing.Size(83, 13);
            this.labelOperationMode.TabIndex = 17;
            this.labelOperationMode.Text = "Operation Mode";
            // 
            // labelIsVirtual
            // 
            this.labelIsVirtual.AutoSize = true;
            this.labelIsVirtual.Location = new System.Drawing.Point(13, 215);
            this.labelIsVirtual.Name = "labelIsVirtual";
            this.labelIsVirtual.Size = new System.Drawing.Size(36, 13);
            this.labelIsVirtual.TabIndex = 16;
            this.labelIsVirtual.Text = "Virtual";
            // 
            // labelIP
            // 
            this.labelIP.AutoSize = true;
            this.labelIP.Location = new System.Drawing.Point(13, 190);
            this.labelIP.Name = "labelIP";
            this.labelIP.Size = new System.Drawing.Size(52, 13);
            this.labelIP.TabIndex = 15;
            this.labelIP.Text = "IP Adress";
            // 
            // labelSystemName
            // 
            this.labelSystemName.AutoSize = true;
            this.labelSystemName.Location = new System.Drawing.Point(13, 165);
            this.labelSystemName.Name = "labelSystemName";
            this.labelSystemName.Size = new System.Drawing.Size(72, 13);
            this.labelSystemName.TabIndex = 14;
            this.labelSystemName.Text = "System Name";
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
            this.labelSystemNameInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSystemNameInfo.Location = new System.Drawing.Point(203, 165);
            this.labelSystemNameInfo.Name = "labelSystemNameInfo";
            this.labelSystemNameInfo.Size = new System.Drawing.Size(282, 25);
            this.labelSystemNameInfo.TabIndex = 19;
            this.labelSystemNameInfo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelIPInfo
            // 
            this.labelIPInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelIPInfo.Location = new System.Drawing.Point(203, 190);
            this.labelIPInfo.Name = "labelIPInfo";
            this.labelIPInfo.Size = new System.Drawing.Size(282, 25);
            this.labelIPInfo.TabIndex = 20;
            this.labelIPInfo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelIsVirtualInfo
            // 
            this.labelIsVirtualInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelIsVirtualInfo.Location = new System.Drawing.Point(203, 215);
            this.labelIsVirtualInfo.Name = "labelIsVirtualInfo";
            this.labelIsVirtualInfo.Size = new System.Drawing.Size(282, 25);
            this.labelIsVirtualInfo.TabIndex = 21;
            this.labelIsVirtualInfo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelOperationModeInfo
            // 
            this.labelOperationModeInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelOperationModeInfo.Location = new System.Drawing.Point(203, 240);
            this.labelOperationModeInfo.Name = "labelOperationModeInfo";
            this.labelOperationModeInfo.Size = new System.Drawing.Size(282, 25);
            this.labelOperationModeInfo.TabIndex = 22;
            this.labelOperationModeInfo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 115);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 13);
            this.label2.TabIndex = 23;
            this.label2.Text = "Controller info";
            // 
            // PickControllerForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(500, 350);
            this.ControlBox = false;
            this.Controls.Add(this.labelOperationModeInfo);
            this.Controls.Add(this.labelIsVirtualInfo);
            this.Controls.Add(this.labelIPInfo);
            this.Controls.Add(this.labelSystemNameInfo);
            this.Controls.Add(this.labelNameInfo);
            this.Controls.Add(this.labelOperationMode);
            this.Controls.Add(this.labelIsVirtual);
            this.Controls.Add(this.labelIP);
            this.Controls.Add(this.labelSystemName);
            this.Controls.Add(this.labelName);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "PickControllerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        //private System.Windows.Forms.ComboBox comboBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label labelOperationMode;
        private System.Windows.Forms.Label labelIsVirtual;
        private System.Windows.Forms.Label labelIP;
        private System.Windows.Forms.Label labelSystemName;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.Label labelNameInfo;
        private System.Windows.Forms.Label labelSystemNameInfo;
        private System.Windows.Forms.Label labelIPInfo;
        private System.Windows.Forms.Label labelIsVirtualInfo;
        private System.Windows.Forms.Label labelOperationModeInfo;
        private System.Windows.Forms.Label label2;
    }
}
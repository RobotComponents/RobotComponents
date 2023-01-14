// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

namespace RobotComponents.ABB.Controllers.Forms
{
    partial class PickRapidDomainPathForm
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
            this.buttonOk = new System.Windows.Forms.Button();
            this.labelTitle = new System.Windows.Forms.Label();
            this.comboBoxTask = new System.Windows.Forms.ComboBox();
            this.labelValue = new System.Windows.Forms.Label();
            this.labelSymbol = new System.Windows.Forms.Label();
            this.labelModule = new System.Windows.Forms.Label();
            this.labelTask = new System.Windows.Forms.Label();
            this.labelValueInfo = new System.Windows.Forms.Label();
            this.comboBoxModule = new System.Windows.Forms.ComboBox();
            this.comboBoxSymbol = new System.Windows.Forms.ComboBox();
            this.labelSymbolType = new System.Windows.Forms.Label();
            this.labelSymbolTypeInfo = new System.Windows.Forms.Label();
            this.labelRapidTypeInfo = new System.Windows.Forms.Label();
            this.labelRapidType = new System.Windows.Forms.Label();
            this.labelRapidDataInfo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonOk.Location = new System.Drawing.Point(12, 303);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(476, 33);
            this.buttonOk.TabIndex = 1;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.Button1Click);
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.125F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTitle.Location = new System.Drawing.Point(12, 18);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(207, 17);
            this.labelTitle.TabIndex = 2;
            this.labelTitle.Text = "Choose a path to read from";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // comboBoxTask
            // 
            this.comboBoxTask.DropDownWidth = 476;
            this.comboBoxTask.Location = new System.Drawing.Point(170, 60);
            this.comboBoxTask.Name = "comboBoxTask";
            this.comboBoxTask.Size = new System.Drawing.Size(316, 21);
            this.comboBoxTask.TabIndex = 3;
            this.comboBoxTask.SelectedIndexChanged += new System.EventHandler(this.ComboBoxTaskSelectedIndexChanged);
            // 
            // labelValue
            // 
            this.labelValue.AutoSize = true;
            this.labelValue.Location = new System.Drawing.Point(12, 250);
            this.labelValue.Name = "labelValue";
            this.labelValue.Size = new System.Drawing.Size(34, 13);
            this.labelValue.TabIndex = 17;
            this.labelValue.Text = "Value";
            // 
            // labelSymbol
            // 
            this.labelSymbol.AutoSize = true;
            this.labelSymbol.Location = new System.Drawing.Point(12, 117);
            this.labelSymbol.Name = "labelSymbol";
            this.labelSymbol.Size = new System.Drawing.Size(41, 13);
            this.labelSymbol.TabIndex = 15;
            this.labelSymbol.Text = "Symbol";
            // 
            // labelModule
            // 
            this.labelModule.AutoSize = true;
            this.labelModule.Location = new System.Drawing.Point(12, 90);
            this.labelModule.Name = "labelModule";
            this.labelModule.Size = new System.Drawing.Size(42, 13);
            this.labelModule.TabIndex = 14;
            this.labelModule.Text = "Module";
            // 
            // labelTask
            // 
            this.labelTask.AutoSize = true;
            this.labelTask.Location = new System.Drawing.Point(12, 63);
            this.labelTask.Name = "labelTask";
            this.labelTask.Size = new System.Drawing.Size(31, 13);
            this.labelTask.TabIndex = 13;
            this.labelTask.Text = "Task";
            // 
            // labelValueInfo
            // 
            this.labelValueInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelValueInfo.Location = new System.Drawing.Point(170, 250);
            this.labelValueInfo.Name = "labelValueInfo";
            this.labelValueInfo.Size = new System.Drawing.Size(315, 13);
            this.labelValueInfo.TabIndex = 22;
            this.labelValueInfo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboBoxModule
            // 
            this.comboBoxModule.DropDownWidth = 476;
            this.comboBoxModule.Location = new System.Drawing.Point(169, 87);
            this.comboBoxModule.Name = "comboBoxModule";
            this.comboBoxModule.Size = new System.Drawing.Size(316, 21);
            this.comboBoxModule.TabIndex = 23;
            this.comboBoxModule.SelectedIndexChanged += new System.EventHandler(this.ComboBoxModuleSelectedIndexChanged);
            // 
            // comboBoxSymbol
            // 
            this.comboBoxSymbol.DropDownWidth = 476;
            this.comboBoxSymbol.Location = new System.Drawing.Point(169, 114);
            this.comboBoxSymbol.Name = "comboBoxSymbol";
            this.comboBoxSymbol.Size = new System.Drawing.Size(316, 21);
            this.comboBoxSymbol.TabIndex = 23;
            this.comboBoxSymbol.SelectedIndexChanged += new System.EventHandler(this.ComboBoxSymbolSelectedIndexChanged);
            // 
            // labelSymbolType
            // 
            this.labelSymbolType.AutoSize = true;
            this.labelSymbolType.Location = new System.Drawing.Point(12, 196);
            this.labelSymbolType.Name = "labelSymbolType";
            this.labelSymbolType.Size = new System.Drawing.Size(64, 13);
            this.labelSymbolType.TabIndex = 25;
            this.labelSymbolType.Text = "Symbol type";
            // 
            // labelSymbolTypeInfo
            // 
            this.labelSymbolTypeInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSymbolTypeInfo.Location = new System.Drawing.Point(171, 196);
            this.labelSymbolTypeInfo.Name = "labelSymbolTypeInfo";
            this.labelSymbolTypeInfo.Size = new System.Drawing.Size(315, 13);
            this.labelSymbolTypeInfo.TabIndex = 26;
            this.labelSymbolTypeInfo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelRapidTypeInfo
            // 
            this.labelRapidTypeInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelRapidTypeInfo.Location = new System.Drawing.Point(170, 223);
            this.labelRapidTypeInfo.Name = "labelRapidTypeInfo";
            this.labelRapidTypeInfo.Size = new System.Drawing.Size(315, 13);
            this.labelRapidTypeInfo.TabIndex = 28;
            this.labelRapidTypeInfo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelRapidType
            // 
            this.labelRapidType.AutoSize = true;
            this.labelRapidType.Location = new System.Drawing.Point(12, 223);
            this.labelRapidType.Name = "labelRapidType";
            this.labelRapidType.Size = new System.Drawing.Size(63, 13);
            this.labelRapidType.TabIndex = 27;
            this.labelRapidType.Text = "RAPID type";
            // 
            // labelRapidDataInfo
            // 
            this.labelRapidDataInfo.AutoSize = true;
            this.labelRapidDataInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelRapidDataInfo.Location = new System.Drawing.Point(12, 171);
            this.labelRapidDataInfo.Name = "labelRapidDataInfo";
            this.labelRapidDataInfo.Size = new System.Drawing.Size(99, 13);
            this.labelRapidDataInfo.TabIndex = 31;
            this.labelRapidDataInfo.Text = "RAPID data info";
            // 
            // PickRapidDomainPathForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(500, 350);
            this.ControlBox = false;
            this.Controls.Add(this.labelRapidDataInfo);
            this.Controls.Add(this.labelRapidTypeInfo);
            this.Controls.Add(this.labelRapidType);
            this.Controls.Add(this.labelSymbolTypeInfo);
            this.Controls.Add(this.labelSymbolType);
            this.Controls.Add(this.comboBoxSymbol);
            this.Controls.Add(this.comboBoxModule);
            this.Controls.Add(this.labelValueInfo);
            this.Controls.Add(this.labelValue);
            this.Controls.Add(this.labelSymbol);
            this.Controls.Add(this.labelModule);
            this.Controls.Add(this.labelTask);
            this.Controls.Add(this.comboBoxTask);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.buttonOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "PickRapidDomainPathForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.ComboBox comboBoxTask;
        private System.Windows.Forms.ComboBox comboBoxModule;
        private System.Windows.Forms.ComboBox comboBoxSymbol;
        private System.Windows.Forms.Label labelValue;
        private System.Windows.Forms.Label labelSymbol;
        private System.Windows.Forms.Label labelModule;
        private System.Windows.Forms.Label labelTask;
        private System.Windows.Forms.Label labelValueInfo;
        private System.Windows.Forms.Label labelSymbolType;
        private System.Windows.Forms.Label labelSymbolTypeInfo;
        private System.Windows.Forms.Label labelRapidTypeInfo;
        private System.Windows.Forms.Label labelRapidType;
        private System.Windows.Forms.Label labelRapidDataInfo;
    }
}
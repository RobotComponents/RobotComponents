// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
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
            this.labelVariableType = new System.Windows.Forms.Label();
            this.labelVariableTypeInfo = new System.Windows.Forms.Label();
            this.labelDataTypeInfo = new System.Windows.Forms.Label();
            this.labelDataType = new System.Windows.Forms.Label();
            this.labelRapidDataInfo = new System.Windows.Forms.Label();
            this.labelScopeInfo = new System.Windows.Forms.Label();
            this.labelScope = new System.Windows.Forms.Label();
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
            this.labelValue.Location = new System.Drawing.Point(11, 277);
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
            this.labelValueInfo.Location = new System.Drawing.Point(169, 277);
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
            // labelVariableType
            // 
            this.labelVariableType.AutoSize = true;
            this.labelVariableType.Location = new System.Drawing.Point(11, 223);
            this.labelVariableType.Name = "labelVariableType";
            this.labelVariableType.Size = new System.Drawing.Size(68, 13);
            this.labelVariableType.TabIndex = 25;
            this.labelVariableType.Text = "Variable type";
            // 
            // labelVariableTypeInfo
            // 
            this.labelVariableTypeInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelVariableTypeInfo.Location = new System.Drawing.Point(170, 223);
            this.labelVariableTypeInfo.Name = "labelVariableTypeInfo";
            this.labelVariableTypeInfo.Size = new System.Drawing.Size(315, 13);
            this.labelVariableTypeInfo.TabIndex = 26;
            this.labelVariableTypeInfo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelDataTypeInfo
            // 
            this.labelDataTypeInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelDataTypeInfo.Location = new System.Drawing.Point(169, 250);
            this.labelDataTypeInfo.Name = "labelDataTypeInfo";
            this.labelDataTypeInfo.Size = new System.Drawing.Size(315, 13);
            this.labelDataTypeInfo.TabIndex = 28;
            this.labelDataTypeInfo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelDataType
            // 
            this.labelDataType.AutoSize = true;
            this.labelDataType.Location = new System.Drawing.Point(11, 250);
            this.labelDataType.Name = "labelDataType";
            this.labelDataType.Size = new System.Drawing.Size(53, 13);
            this.labelDataType.TabIndex = 27;
            this.labelDataType.Text = "Data type";
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
            // labelScopeInfo
            // 
            this.labelScopeInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelScopeInfo.Location = new System.Drawing.Point(171, 197);
            this.labelScopeInfo.Name = "labelScopeInfo";
            this.labelScopeInfo.Size = new System.Drawing.Size(315, 13);
            this.labelScopeInfo.TabIndex = 33;
            this.labelScopeInfo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelScope
            // 
            this.labelScope.AutoSize = true;
            this.labelScope.Location = new System.Drawing.Point(12, 197);
            this.labelScope.Name = "labelScope";
            this.labelScope.Size = new System.Drawing.Size(62, 13);
            this.labelScope.TabIndex = 32;
            this.labelScope.Text = "Data scope";
            // 
            // PickRapidDomainPathForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(500, 350);
            this.ControlBox = false;
            this.Controls.Add(this.labelScopeInfo);
            this.Controls.Add(this.labelScope);
            this.Controls.Add(this.labelRapidDataInfo);
            this.Controls.Add(this.labelDataTypeInfo);
            this.Controls.Add(this.labelDataType);
            this.Controls.Add(this.labelVariableTypeInfo);
            this.Controls.Add(this.labelVariableType);
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
        private System.Windows.Forms.Label labelVariableType;
        private System.Windows.Forms.Label labelVariableTypeInfo;
        private System.Windows.Forms.Label labelDataTypeInfo;
        private System.Windows.Forms.Label labelDataType;
        private System.Windows.Forms.Label labelRapidDataInfo;
        private System.Windows.Forms.Label labelScopeInfo;
        private System.Windows.Forms.Label labelScope;
    }
}
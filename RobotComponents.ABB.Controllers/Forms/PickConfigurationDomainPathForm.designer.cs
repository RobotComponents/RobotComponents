// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

namespace RobotComponents.ABB.Controllers.Forms
{
    partial class PickConfigurationDomainPathForm
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
            this.comboBoxDomain = new System.Windows.Forms.ComboBox();
            this.labelValue = new System.Windows.Forms.Label();
            this.labelAttribute = new System.Windows.Forms.Label();
            this.labelInstance = new System.Windows.Forms.Label();
            this.labelType = new System.Windows.Forms.Label();
            this.labelDomain = new System.Windows.Forms.Label();
            this.labelValueInfo = new System.Windows.Forms.Label();
            this.comboBoxType = new System.Windows.Forms.ComboBox();
            this.comboBoxInstance = new System.Windows.Forms.ComboBox();
            this.comboBoxAttribute = new System.Windows.Forms.ComboBox();
            this.labelConfigDataInfo = new System.Windows.Forms.Label();
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
            // comboBoxDomain
            // 
            this.comboBoxDomain.DropDownWidth = 476;
            this.comboBoxDomain.Location = new System.Drawing.Point(170, 60);
            this.comboBoxDomain.Name = "comboBoxDomain";
            this.comboBoxDomain.Size = new System.Drawing.Size(316, 21);
            this.comboBoxDomain.TabIndex = 3;
            this.comboBoxDomain.SelectedIndexChanged += new System.EventHandler(this.ComboBoxDomainSelectedIndexChanged);
            // 
            // labelValue
            // 
            this.labelValue.AutoSize = true;
            this.labelValue.Location = new System.Drawing.Point(13, 221);
            this.labelValue.Name = "labelValue";
            this.labelValue.Size = new System.Drawing.Size(34, 13);
            this.labelValue.TabIndex = 17;
            this.labelValue.Text = "Value";
            // 
            // labelAttribute
            // 
            this.labelAttribute.AutoSize = true;
            this.labelAttribute.Location = new System.Drawing.Point(13, 144);
            this.labelAttribute.Name = "labelAttribute";
            this.labelAttribute.Size = new System.Drawing.Size(46, 13);
            this.labelAttribute.TabIndex = 16;
            this.labelAttribute.Text = "Attribute";
            // 
            // labelInstance
            // 
            this.labelInstance.AutoSize = true;
            this.labelInstance.Location = new System.Drawing.Point(12, 117);
            this.labelInstance.Name = "labelInstance";
            this.labelInstance.Size = new System.Drawing.Size(48, 13);
            this.labelInstance.TabIndex = 15;
            this.labelInstance.Text = "Instance";
            // 
            // labelType
            // 
            this.labelType.AutoSize = true;
            this.labelType.Location = new System.Drawing.Point(12, 90);
            this.labelType.Name = "labelType";
            this.labelType.Size = new System.Drawing.Size(31, 13);
            this.labelType.TabIndex = 14;
            this.labelType.Text = "Type";
            // 
            // labelDomain
            // 
            this.labelDomain.AutoSize = true;
            this.labelDomain.Location = new System.Drawing.Point(12, 63);
            this.labelDomain.Name = "labelDomain";
            this.labelDomain.Size = new System.Drawing.Size(43, 13);
            this.labelDomain.TabIndex = 13;
            this.labelDomain.Text = "Domain";
            // 
            // labelValueInfo
            // 
            this.labelValueInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelValueInfo.Location = new System.Drawing.Point(170, 221);
            this.labelValueInfo.Name = "labelValueInfo";
            this.labelValueInfo.Size = new System.Drawing.Size(315, 13);
            this.labelValueInfo.TabIndex = 22;
            this.labelValueInfo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboBoxType
            // 
            this.comboBoxType.DropDownWidth = 476;
            this.comboBoxType.Location = new System.Drawing.Point(169, 87);
            this.comboBoxType.Name = "comboBoxType";
            this.comboBoxType.Size = new System.Drawing.Size(316, 21);
            this.comboBoxType.TabIndex = 23;
            this.comboBoxType.SelectedIndexChanged += new System.EventHandler(this.ComboBoxTypeSelectedIndexChanged);
            // 
            // comboBoxInstance
            // 
            this.comboBoxInstance.DropDownWidth = 476;
            this.comboBoxInstance.Location = new System.Drawing.Point(169, 114);
            this.comboBoxInstance.Name = "comboBoxInstance";
            this.comboBoxInstance.Size = new System.Drawing.Size(316, 21);
            this.comboBoxInstance.TabIndex = 24;
            this.comboBoxInstance.SelectedIndexChanged += new System.EventHandler(this.ComboBoxInstanceSelectedIndexChanged);
            // 
            // comboBoxAttribute
            // 
            this.comboBoxAttribute.DropDownWidth = 476;
            this.comboBoxAttribute.Location = new System.Drawing.Point(169, 141);
            this.comboBoxAttribute.Name = "comboBoxAttribute";
            this.comboBoxAttribute.Size = new System.Drawing.Size(316, 21);
            this.comboBoxAttribute.TabIndex = 25;
            this.comboBoxAttribute.SelectedIndexChanged += new System.EventHandler(this.ComboBoxAttributeSelectedIndexChanged);
            // 
            // labelConfigDataInfo
            // 
            this.labelConfigDataInfo.AutoSize = true;
            this.labelConfigDataInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelConfigDataInfo.Location = new System.Drawing.Point(13, 196);
            this.labelConfigDataInfo.Name = "labelConfigDataInfo";
            this.labelConfigDataInfo.Size = new System.Drawing.Size(136, 13);
            this.labelConfigDataInfo.TabIndex = 27;
            this.labelConfigDataInfo.Text = "Configuration data info";
            // 
            // PickConfigurationDomainPathForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(500, 350);
            this.ControlBox = false;
            this.Controls.Add(this.labelConfigDataInfo);
            this.Controls.Add(this.comboBoxAttribute);
            this.Controls.Add(this.comboBoxInstance);
            this.Controls.Add(this.comboBoxType);
            this.Controls.Add(this.labelValueInfo);
            this.Controls.Add(this.labelValue);
            this.Controls.Add(this.labelAttribute);
            this.Controls.Add(this.labelInstance);
            this.Controls.Add(this.labelType);
            this.Controls.Add(this.labelDomain);
            this.Controls.Add(this.comboBoxDomain);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.buttonOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "PickConfigurationDomainPathForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.ComboBox comboBoxDomain;
        private System.Windows.Forms.ComboBox comboBoxType;
        private System.Windows.Forms.ComboBox comboBoxInstance;
        private System.Windows.Forms.ComboBox comboBoxAttribute;
        private System.Windows.Forms.Label labelValue;
        private System.Windows.Forms.Label labelAttribute;
        private System.Windows.Forms.Label labelInstance;
        private System.Windows.Forms.Label labelType;
        private System.Windows.Forms.Label labelDomain;
        private System.Windows.Forms.Label labelValueInfo;
        private System.Windows.Forms.Label labelConfigDataInfo;
    }
}
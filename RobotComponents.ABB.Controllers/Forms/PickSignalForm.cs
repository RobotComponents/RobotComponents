// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RobotComponents.ABB.Controllers.Forms
{
    public partial class PickDOForm : Form
    {
        public static int SignalIndex = 0;

        public PickDOForm()
        {
            InitializeComponent();
        }

        public PickDOForm(List<string> items)
        {
            InitializeComponent();
            for (int i = 0; i < items.Count; i++)
            {
                comboBox1.Items.Add(items[i]);
            }
        }

        private void PickDO_Load(object sender, EventArgs e)
        {

        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.labelNameInfo.Text = "-";
            this.labelValueInfo.Text = "-";
            this.labelTypeInfo.Text = "-";
            this.labelMinValueInfo.Text = "-";
            this.labelMaxValueInfo.Text = "-";
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            SignalIndex = comboBox1.SelectedIndex;
            this.Close();
        }
    }
}

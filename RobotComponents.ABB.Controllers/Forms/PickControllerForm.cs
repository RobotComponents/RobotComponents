// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
using System.Windows.Forms;
// RobotComponents Libs
using RobotComponents.ABB.Controllers;

namespace RobotComponents.ABB.Controllers.Forms
{
    public partial class PickControllerForm : Form
    {
        public int Index = 0;

        public PickControllerForm()
        {
            InitializeComponent();
        }

        public PickControllerForm(List<string> items)
        {
            InitializeComponent();

            for (int i = 0; i < items.Count; i++)
            {
                comboBox1.Items.Add(items[i]);
            }
        }

        private void Button1Click(object sender, EventArgs e)
        {
            Index = comboBox1.SelectedIndex;
            this.Close();
        }

        private void ComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            this.labelNameInfo.Text = Controller.Controllers[comboBox1.SelectedIndex].ControllerInstanceABB.Name;
            this.labelSystemNameInfo.Text = Controller.Controllers[comboBox1.SelectedIndex].ControllerInstanceABB.SystemName;
            this.labelIPInfo.Text = Controller.Controllers[comboBox1.SelectedIndex].ControllerInstanceABB.IPAddress.ToString();
            this.labelIsVirtualInfo.Text = Controller.Controllers[comboBox1.SelectedIndex].ControllerInstanceABB.IsVirtual.ToString();
            this.labelOperationModeInfo.Text = Controller.Controllers[comboBox1.SelectedIndex].ControllerInstanceABB.OperatingMode.ToString();
        }
    }
}

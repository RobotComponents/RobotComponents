// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
using System.Windows.Forms;
// RobotComponents Libs
using RobotComponents.Gh.Components.ControllerUtility;

namespace RobotComponents.Gh.Forms
{
    public partial class PickControllerForm : Form
    {
        public static int StationIndex = 0;

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

        private void PickController_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            StationIndex = comboBox1.SelectedIndex;
            this.Close();
        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            this.labelNameInfo.Text = GetControllerComponent.ControllerInstance[comboBox1.SelectedIndex].Name.ToString();
            this.labelSystemNameInfo.Text = GetControllerComponent.ControllerInstance[comboBox1.SelectedIndex].SystemName.ToString();
            this.labelIPInfo.Text = GetControllerComponent.ControllerInstance[comboBox1.SelectedIndex].IPAddress.ToString();
            this.labelIsVirtualInfo.Text = GetControllerComponent.ControllerInstance[comboBox1.SelectedIndex].IsVirtual.ToString();
            this.labelOperationModeInfo.Text = GetControllerComponent.ControllerInstance[comboBox1.SelectedIndex].OperatingMode.ToString();
        }
    }
}

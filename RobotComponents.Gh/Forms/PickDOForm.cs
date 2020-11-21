// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
using System.Windows.Forms;
// Robot Components Libs
using RobotComponents.Gh.Components.ControllerUtility;

namespace RobotComponents.Gh.Forms
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

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.labelNameInfo.Text = GetDigitalOutputComponent.SignalGooList[comboBox1.SelectedIndex].Value.Name.ToString();
            this.labelValueInfo.Text = GetDigitalOutputComponent.SignalGooList[comboBox1.SelectedIndex].Value.Value.ToString();
            this.labelTypeInfo.Text = GetDigitalOutputComponent.SignalGooList[comboBox1.SelectedIndex].Value.Type.ToString();
            this.labelMinValueInfo.Text = GetDigitalOutputComponent.SignalGooList[comboBox1.SelectedIndex].Value.MinValue.ToString();
            this.labelMaxValueInfo.Text = GetDigitalOutputComponent.SignalGooList[comboBox1.SelectedIndex].Value.MaxValue.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SignalIndex = comboBox1.SelectedIndex;
            this.Close();
        }
    }
}

// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
using System.Windows.Forms;
// Robot Components Libs
using RobotComponents.Definitions;
using RobotComponents.Enumerations;

namespace RobotComponents.Gh.Forms
{
    public partial class PickRobotForm : Form
    {
        public int RobotIndex = 0;
        private List<RobotPreset> robotPresets;

        public PickRobotForm()
        {
            InitializeComponent();
        }

        public PickRobotForm(List<RobotPreset> items)
        {
            InitializeComponent();

            for (int i = 0; i < items.Count; i++)
            {
                comboBox1.Items.Add(Robot.GetRobotPresetName(items[i]));
            }

            robotPresets = items;
        }

        private void PickRobot_Load(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.labelNameInfo.Text = Robot.GetRobotPresetName(robotPresets[comboBox1.SelectedIndex]);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RobotIndex = comboBox1.SelectedIndex;
            this.Close();
        }
    }
}

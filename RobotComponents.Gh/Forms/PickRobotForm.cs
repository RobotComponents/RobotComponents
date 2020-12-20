// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
using System.Windows.Forms;
// Robot Components Libs
using RobotComponents.Enumerations;
using RobotComponents.Utils;

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
                comboBox1.Items.Add(PickRobotForm.GetRobotPresetName(items[i]));
            }

            robotPresets = items;
        }

        private void PickRobot_Load(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.labelNameInfo.Text = PickRobotForm.GetRobotPresetName(robotPresets[comboBox1.SelectedIndex]);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RobotIndex = comboBox1.SelectedIndex;
            this.Close();
        }

        /// <summary>
        /// Returns the Robot preset name.
        /// </summary>
        /// <param name="preset"> The Robot preset. </param>
        /// <returns> The name of the Robot preset. </returns>
        private static string GetRobotPresetName(RobotPreset preset)
        {
            string name = Enum.GetName(typeof(RobotPreset), preset);
            name = name.ReplaceFirst("_", "-");
            name = name.ReplaceFirst("_", "/");
            name = name.ReplaceFirst("/0", "/0.");
            name = name.ReplaceFirst("/1", "/1.");
            name = name.ReplaceFirst("/2", "/2.");
            name = name.ReplaceFirst("/3", "/3.");
            name = name.ReplaceFirst("/4", "/4.");

            return name;
        }
    }
}

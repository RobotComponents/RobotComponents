using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using RobotComponents.BaseClasses;
using RobotComponents.Components;
using RobotComponents.Parameters;
using RobotComponents.Goos;

namespace RobotComponents.Resources
{
    public partial class PickControllerForm : Form
    {
        public static int stationIndex = 0;

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

        private void PicController_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            stationIndex = comboBox1.SelectedIndex;
            this.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            this.labelNameInfo.Text = GetControllerComponent.controllerInstance[comboBox1.SelectedIndex].Name.ToString();
            this.labelSystemNameInfo.Text = GetControllerComponent.controllerInstance[comboBox1.SelectedIndex].SystemName.ToString();
            this.labelIPInfo.Text = GetControllerComponent.controllerInstance[comboBox1.SelectedIndex].IPAddress.ToString();
            this.labelIsVirtualInfo.Text = GetControllerComponent.controllerInstance[comboBox1.SelectedIndex].IsVirtual.ToString();
            this.labelOperationModeInfo.Text = GetControllerComponent.controllerInstance[comboBox1.SelectedIndex].OperatingMode.ToString();
        }
    }
}

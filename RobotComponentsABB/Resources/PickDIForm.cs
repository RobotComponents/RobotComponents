using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using RobotComponents.BaseClasses;

using RobotComponentsABB.Components;
using RobotComponentsABB.Parameters;
using RobotComponentsABB.Goos;

namespace RobotComponentsABB.Resources
{
    public partial class PickDIForm : Form
    {
        public static int stationIndex = 0;

        public PickDIForm()
        {
            InitializeComponent();
        }

        public PickDIForm(List<string> items)
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

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.labelNameInfo.Text = GetDigitalInputComponent.signalGooList[comboBox1.SelectedIndex].Value.Name.ToString();
            this.labelValueInfo.Text = GetDigitalInputComponent.signalGooList[comboBox1.SelectedIndex].Value.Value.ToString();
            this.labelTypeInfo.Text = GetDigitalInputComponent.signalGooList[comboBox1.SelectedIndex].Value.Type.ToString();
            this.labelMinValueInfo.Text = GetDigitalInputComponent.signalGooList[comboBox1.SelectedIndex].Value.MinValue.ToString();
            this.labelMaxValueInfo.Text = GetDigitalInputComponent.signalGooList[comboBox1.SelectedIndex].Value.MaxValue.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Rhino.RhinoApp.WriteLine("Debug button1 Click : " + comboBox1.SelectedIndex.ToString());
            stationIndex = comboBox1.SelectedIndex;
            this.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void labelTypeInfo_Click(object sender, EventArgs e)
        {

        }
    }
}

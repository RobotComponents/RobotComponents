// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Windows.Forms;

namespace RobotComponents.ABB.Controllers.Forms
{
    public partial class PickTaskForm : Form
    {
        public int Index = 0;
        private Controller _controller = new Controller();

        public PickTaskForm()
        {
            InitializeComponent();
        }

        public PickTaskForm(Controller controller)
        {
            InitializeComponent();

            _controller = controller;

            for (int i = 0; i < _controller.Tasks.Count; i++)
            {
                comboBox.Items.Add(_controller.Tasks[i].Name[i]);
            }
        }

        private void Button1Click(object sender, EventArgs e)
        {
            Index = comboBox.SelectedIndex;
            this.Close();
        }

        private void ComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            this.labelNameInfo.Text = _controller.Tasks[comboBox.SelectedIndex].Name.ToString();
            this.labelTaskTypeInfo.Text = _controller.Tasks[comboBox.SelectedIndex].TaskType.ToString();
            this.labelEnabledInfo.Text = _controller.Tasks[comboBox.SelectedIndex].Enabled.ToString();
        }
    }
}

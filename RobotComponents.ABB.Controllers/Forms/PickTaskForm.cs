// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Windows.Forms;

namespace RobotComponents.ABB.Controllers.Forms
{
    /// <summary>
    /// Represents the pick task form class.
    /// </summary>
    public partial class PickTaskForm : Form
    {
        #region fields
        private readonly Controller _controller = new Controller();
        private int _index = 0;
        #endregion

        #region constructors
        /// <summary>
        /// Creates a pick task form.
        /// </summary>
        /// <param name="controller"> The controller to pick a task from. </param>
        public PickTaskForm(Controller controller)
        {
            InitializeComponent();

            _controller = controller;

            for (int i = 0; i < _controller.TasksABB.Count; i++)
            {
                comboBox.Items.Add(_controller.TasksABB[i].Name);
            }
        }
        #endregion

        #region methods
        private void Button1Click(object sender, EventArgs e)
        {
            _index = comboBox.SelectedIndex;
            this.Close();
        }

        private void ComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            this.labelNameInfo.Text = _controller.TasksABB[comboBox.SelectedIndex].Name.ToString();
            this.labelTaskTypeInfo.Text = _controller.TasksABB[comboBox.SelectedIndex].TaskType.ToString();
            this.labelEnabledInfo.Text = _controller.TasksABB[comboBox.SelectedIndex].Enabled.ToString();
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets the selected index.
        /// </summary>
        public int Index
        {
            get { return _index; }
        }
        #endregion
    }
}

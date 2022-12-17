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
    /// <summary>
    /// Represents the pick controller form class.
    /// </summary>
    public partial class PickControllerForm : Form
    {
        #region fields
        private int _index = 0;
        #endregion

        #region constructors
        /// <summary>
        /// Constructs the form from a given list with items.
        /// </summary>
        /// <param name="items"> Items to fill the form with. </param>
        public PickControllerForm(List<string> items)
        {
            InitializeComponent();

            for (int i = 0; i < items.Count; i++)
            {
                comboBox1.Items.Add(items[i]);
            }
        }
        #endregion

        #region methods
        private void Button1Click(object sender, EventArgs e)
        {
            _index = comboBox1.SelectedIndex;
            this.Close();
        }

        private void ComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            this.labelNameInfo.Text = Controller.Controllers[comboBox1.SelectedIndex].ControllerABB.Name;
            this.labelSystemNameInfo.Text = Controller.Controllers[comboBox1.SelectedIndex].ControllerABB.SystemName;
            this.labelIPInfo.Text = Controller.Controllers[comboBox1.SelectedIndex].ControllerABB.IPAddress.ToString();
            this.labelIsVirtualInfo.Text = Controller.Controllers[comboBox1.SelectedIndex].ControllerABB.IsVirtual.ToString();
            this.labelOperationModeInfo.Text = Controller.Controllers[comboBox1.SelectedIndex].ControllerABB.OperatingMode.ToString();
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets the picked index.
        /// </summary>
        public int Index
        {
            get { return _index; }
        }
        #endregion
    }
}

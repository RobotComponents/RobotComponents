// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
// Eto libs
using Eto.Forms;
using Eto.Drawing;

namespace RobotComponents.ABB.Controllers.Forms
{
    /// <summary>
    /// Represents the pick controller form class.
    /// </summary>
    public class PickControllerForm : Dialog<Boolean>
    {
        #region fields
        private Controller _controller;
        private readonly Label _labelName = new Label() { Text = "-", TextAlignment = TextAlignment.Right, Height = _height };
        private readonly Label _labelSystemName = new Label() { Text = "-", TextAlignment = TextAlignment.Right, Height = _height };
        private readonly Label _labelIpAdress = new Label() { Text = "-", TextAlignment = TextAlignment.Right, Height = _height };
        private readonly Label _labelRobotWareVersion = new Label() { Text = "-", TextAlignment = TextAlignment.Right, Height = _height };
        private readonly Label _labelVirtual = new Label() { Text = "-", TextAlignment = TextAlignment.Right, Height = _height };
        private readonly Label _labelOperatingMode = new Label() { Text = "-", TextAlignment = TextAlignment.Right, Height = _height };
        private readonly ComboBox _box = new ComboBox() { Height = _height };

        private const int _height = 21;
        #endregion

        #region constructors
        /// <summary>
        /// Constructs the form.
        /// </summary>
        public PickControllerForm()
		{
            // Main layout
            Title = "Controller";
            MinimumSize = new Size(600, 420);
            Resizable = false;
            Padding = 20;

            // Controls
            Button button = new Button() { Text = "OK" };
            _box = new ComboBox() { DataStore = Controller.Controllers.ConvertAll(item => item.Name), Height = _height };

            // Assign events
            button.Click += ButtonClick;
            _box.SelectedIndexChanged += IndexChanged;

            // Select index
            _box.SelectedIndex = 0;

            // Labels
            Label selectLabel = new Label() { Text = "Select a controller", Font = new Font(SystemFont.Bold), Height = _height };
            Label infoLabel = new Label() { Text = "Controller info", Font = new Font(SystemFont.Bold), Height = _height };

            // Layout
            DynamicLayout layout = new DynamicLayout() { Padding = 0, Spacing = new Size(8, 4) };
            layout.AddSeparateRow(selectLabel);
            layout.AddSeparateRow(_box);
            layout.AddSeparateRow(new Label() { Text = " ", Height = _height });
            layout.AddSeparateRow(infoLabel);
            layout.AddSeparateRow(new Label() { Text = "Name", Height = _height }, _labelName); ;
            layout.AddSeparateRow(new Label() { Text = "System name", Height = _height }, _labelSystemName);
            layout.AddSeparateRow(new Label() { Text = "IP adress", Height = _height }, _labelIpAdress);
            layout.AddSeparateRow(new Label() { Text = "RobotWare version", Height = _height }, _labelRobotWareVersion);
            layout.AddSeparateRow(new Label() { Text = "Is virtual", Height = _height }, _labelVirtual);
            layout.AddSeparateRow(new Label() { Text = "Operating mode", Height = _height }, _labelOperatingMode);
            layout.AddSeparateRow(new Label() { Text = " ", Height = _height });
            layout.AddSeparateRow(new Label() { Text = " ", Height = _height });
            layout.AddSeparateRow(button);

            Content = layout;
        }
        #endregion

        #region methods
        private void ButtonClick(object sender, EventArgs e)
        {
            _controller = Controller.Controllers[_box.SelectedIndex];
            Close(true);
        }
        private void IndexChanged(object sender, EventArgs e)
        {
            _labelName.Text = Controller.Controllers[_box.SelectedIndex].ControllerABB.Name;
            _labelSystemName.Text = Controller.Controllers[_box.SelectedIndex].ControllerABB.SystemName;
            _labelIpAdress.Text = Controller.Controllers[_box.SelectedIndex].ControllerABB.IPAddress.ToString();
            _labelRobotWareVersion.Text = Controller.Controllers[_box.SelectedIndex].ControllerABB.RobotWareVersion.ToString();
            _labelVirtual.Text = Controller.Controllers[_box.SelectedIndex].ControllerABB.IsVirtual.ToString();
            _labelOperatingMode.Text = Controller.Controllers[_box.SelectedIndex].ControllerABB.OperatingMode.ToString();
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets the picked controller.
        /// </summary>
        public Controller Controller
        {
            get { return _controller; }
        }
        #endregion
    }
}
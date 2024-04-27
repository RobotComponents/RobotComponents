// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
// Eto libs
using Eto.Forms;
using Eto.Drawing;

namespace RobotComponents.ABB.Controllers.Forms
{
    /// <summary>
    /// Represents the pick signal form class.
    /// </summary>
    public class PickSignalForm : Dialog<bool>
    {
        #region fields
        private readonly List<Signal> _signals;
        private Signal _signal;
        private readonly Label _labelName = new Label() { Text = "-", TextAlignment = TextAlignment.Right, Height = _height };
        private readonly Label _labelValue = new Label() { Text = "-", TextAlignment = TextAlignment.Right, Height = _height };
        private readonly Label _labelType = new Label() { Text = "-", TextAlignment = TextAlignment.Right, Height = _height };
        private readonly Label _labelMinValue = new Label() { Text = "-", TextAlignment = TextAlignment.Right, Height = _height };
        private readonly Label _labelMaxValue = new Label() { Text = "-", TextAlignment = TextAlignment.Right, Height = _height };
        private readonly Label _labelAccesLevel = new Label() { Text = "-", TextAlignment = TextAlignment.Right, Height = _height };
        private readonly ComboBox _box = new ComboBox() { Height = _height };

        private const int _height = 21;
        #endregion

        #region constructors
        /// <summary>
        /// Constructs the form.
        /// </summary>
        /// <param name="signals"> List with signals to pick an item from. </param>
        public PickSignalForm(List<Signal> signals)
        {
            // Main layout
            Title = "Controller signals";
            MinimumSize = new Size(600, 420);
            Resizable = false;
            Padding = 20;

            // Signals
            _signals = signals;

            // Controls
            Button button = new Button() { Text = "OK" };
            _box = new ComboBox() { DataStore = _signals.ConvertAll(item => item.Name), Height = _height };

            // Assign events
            button.Click += ButtonClick;
            _box.SelectedIndexChanged += IndexChanged;

            // Select index
            _box.SelectedIndex = 0;

            // Labels
            Label selectLabel = new Label() { Text = "Select a signal", Font = new Font(SystemFont.Bold), Height = _height };
            Label infoLabel = new Label() { Text = "Signal info", Font = new Font(SystemFont.Bold), Height = _height };

            // Layout
            DynamicLayout layout = new DynamicLayout() { Padding = 0, Spacing = new Size(8, 4) };
            layout.AddSeparateRow(selectLabel);
            layout.AddSeparateRow(_box);
            layout.AddSeparateRow(new Label() { Text = " ", Height = _height });
            layout.AddSeparateRow(infoLabel);
            layout.AddSeparateRow(new Label() { Text = "Name", Height = _height }, _labelName);
            layout.AddSeparateRow(new Label() { Text = "Value", Height = _height }, _labelValue);
            layout.AddSeparateRow(new Label() { Text = "Type", Height = _height }, _labelType);
            layout.AddSeparateRow(new Label() { Text = "Minimum value", Height = _height }, _labelMinValue);
            layout.AddSeparateRow(new Label() { Text = "Maximum value", Height = _height }, _labelMaxValue);
            layout.AddSeparateRow(new Label() { Text = "Acces level", Height = _height }, _labelAccesLevel);
            layout.AddSeparateRow(new Label() { Text = " ", Height = _height });
            layout.AddSeparateRow(new Label() { Text = " ", Height = _height });
            layout.AddSeparateRow(button);

            Content = layout;
        }
        #endregion

        #region methods
        private void IndexChanged(object sender, EventArgs e)
        {
            _labelName.Text = _signals[_box.SelectedIndex].Name;
            _labelValue.Text = _signals[_box.SelectedIndex].Value.ToString();
            _labelType.Text = _signals[_box.SelectedIndex].SignalABB.Type.ToString();
            _labelMinValue.Text = _signals[_box.SelectedIndex].MinValue.ToString();
            _labelMaxValue.Text = _signals[_box.SelectedIndex].MaxValue.ToString();
            _labelAccesLevel.Text = _signals[_box.SelectedIndex].AccesLevel;
        }

        private void ButtonClick(object sender, EventArgs e)
        {
            _signal = _signals[_box.SelectedIndex];
            Close(true);
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets the picked signal.
        /// </summary>
        public Signal Signal
        {
            get { return _signal; }
        }
        #endregion
    }
}
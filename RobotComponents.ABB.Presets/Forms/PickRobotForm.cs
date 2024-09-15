// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
using System.Linq;
// Eto libs
using Eto.Forms;
using Eto.Drawing;
// Robot Components Libs
using RobotComponents.ABB.Presets.Enumerations;

namespace RobotComponents.ABB.Presets.Forms
{
    /// <summary>
    /// Represents the pick robot preset form class.
    /// </summary>
    public class PickRobotForm : Dialog<bool>
    {
        #region fields
        private RobotPreset _robotPreset = RobotPreset.EMPTY;
        private readonly List<RobotPreset> _robotPresets;
        private readonly Label _labelName = new Label() { Text = "-", TextAlignment = TextAlignment.Right, Height = _height };
        private readonly ComboBox _box = new ComboBox() { Height = _height };

        private const int _height = 21;
        #endregion

        #region constructors
        /// <summary>
        /// Constructs the form.
        /// </summary>
        public PickRobotForm()
        {
            // Main layout
            Title = "Robot preset";
            MinimumSize = new Size(600, 420);
            Resizable = false;
            Padding = 20;

            // Presets
            _robotPresets = Enum.GetValues(typeof(RobotPreset)).Cast<RobotPreset>().ToList();
            _robotPresets = _robotPresets.OrderBy(c => Enum.GetName(typeof(RobotPreset), c)).ToList();

            // Set EMPTY as first item
            _robotPresets.Remove(RobotPreset.EMPTY);
            _robotPresets.Insert(0, RobotPreset.EMPTY);

            // Controls
            Button button = new Button() { Text = "OK" };
            _box = new ComboBox()
            {
                DataStore = _robotPresets.ConvertAll(item => GetRobotPresetName(item)),
                SelectedIndex = 0,
                Height = _height
            };

            // Assign events
            button.Click += ButtonClick;
            _box.SelectedIndexChanged += IndexChanged;

            // Labels
            Label selectLabel = new Label() { Text = "Select a robot preset", Font = new Font(SystemFont.Bold), Height = _height };
            Label infoLabel = new Label() { Text = "Robot info", Font = new Font(SystemFont.Bold), Height = _height };

            // Layout
            DynamicLayout layout = new DynamicLayout() { Padding = 0, Spacing = new Size(8, 4) };
            layout.AddSeparateRow(selectLabel);
            layout.AddSeparateRow(_box);
            layout.AddSeparateRow(new Label() { Height = _height });
            layout.AddSeparateRow(infoLabel);
            layout.AddSeparateRow(new Label() { Text = "Name", Height = _height }, _labelName);
            layout.AddSeparateRow(new Label() { Text = " ", Height = _height });
            layout.AddSeparateRow(new Label() { Text = " ", Height = _height });
            layout.AddSeparateRow(new Label() { Text = " ", Height = _height });
            layout.AddSeparateRow(new Label() { Text = " ", Height = _height });
            layout.AddSeparateRow(new Label() { Text = " ", Height = _height });
            layout.AddSeparateRow(new Label() { Text = " ", Height = _height });
            layout.AddSeparateRow(new Label() { Text = " ", Height = _height });
            layout.AddSeparateRow(button);

            Content = layout;
        }
        #endregion

        #region methods
        private void IndexChanged(object sender, EventArgs e)
        {
            _labelName.Text = GetRobotPresetName(_robotPresets[_box.SelectedIndex]);
        }

        private void ButtonClick(object sender, EventArgs e)
        {
            _robotPreset = _robotPresets[_box.SelectedIndex];
            Close(true);
        }

        /// <summary>
        /// Returns the Robot preset name.
        /// </summary>
        /// <param name="preset"> The Robot preset. </param>
        /// <returns> The name of the Robot preset. </returns>
        private static string GetRobotPresetName(RobotPreset preset)
        {
            if (preset == RobotPreset.IRB1010_1_5_037)
            {
                return "IRB1010-1.5/0.37";
            }
            else
            {
                string name = Enum.GetName(typeof(RobotPreset), preset);
                name = ReplaceFirst(name, "_", "-");
                name = ReplaceFirst(name, "_", "/");
                name = ReplaceFirst(name, "/0", "/0.");
                name = ReplaceFirst(name, "/1", "/1.");
                name = ReplaceFirst(name, "/2", "/2.");
                name = ReplaceFirst(name, "/3", "/3.");
                name = ReplaceFirst(name, "/4", "/4.");
                name = ReplaceFirst(name, "/5", "/5.");
                name = ReplaceFirst(name, "/6", "/6.");
                name = ReplaceFirst(name, "/7", "/7.");
                name = ReplaceFirst(name, "/8", "/8.");
                name = ReplaceFirst(name, "/9", "/9.");
                name = ReplaceFirst(name, "_LID", "-LID");

                return name;
            }
        }

        /// <summary>
        /// Replaces the first occurence in a string with a new text. 
        /// </summary>
        /// <param name="text"> The text the search and replace in. </param>
        /// <param name="search"> The text to search for. </param>
        /// <param name="replace"> The new text. </param>
        /// <returns> Returns a new string with replaced text. </returns>
        private static string ReplaceFirst(string text, string search, string replace)
        {
            int position = text.IndexOf(search);

            if (position < 0)
            {
                return text;
            }
            return text.Substring(0, position) + replace + text.Substring(position + search.Length);
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets the picked robot preset.
        /// </summary>
        public RobotPreset RobotPreset
        {
            get { return _robotPreset; }
        }
        #endregion
    }
}
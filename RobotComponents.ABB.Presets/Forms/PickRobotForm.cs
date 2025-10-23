// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components
// Project: https://github.com/RobotComponents/RobotComponents
//
// Copyright (c) 2023-2025 Arjen Deetman
//
// Authors:
//   - Arjen Deetman (2023-2025)
//
// For license details, see the LICENSE file in the project root.

// System Libs
using System;
using System.Collections.Generic;
using System.Linq;
// Eto libs
using Eto.Forms;
using Eto.Drawing;
// Robot Components Libs
using RobotComponents.ABB.Presets.Enumerations;
using static RobotComponents.ABB.Presets.Utils.HelperMethods;

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
                DataStore = _robotPresets.ConvertAll(item => GetRobotNameFromPresetName(item)),
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
            _labelName.Text = GetRobotNameFromPresetName(_robotPresets[_box.SelectedIndex]);
        }

        private void ButtonClick(object sender, EventArgs e)
        {
            _robotPreset = _robotPresets[_box.SelectedIndex];
            Close(true);
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
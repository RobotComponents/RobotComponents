// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components
// Project: https://github.com/RobotComponents/RobotComponents
//
// Copyright (c) 2023-2024 Arjen Deetman
//
// Authors:
//   - Arjen Deetman (2023-2024)
//
// For license details, see the LICENSE file in the project root.

// System Libs
using System;
using System.Collections.Generic;
// Eto libs
using Eto.Forms;
using Eto.Drawing;

namespace RobotComponents.ABB.Controllers.Forms
{
    /// <summary>
    /// Represents the pick task form class.
    /// </summary>
    public class PickTaskForm : Dialog<bool>
    {
        #region fields
        private readonly Controller _controller = new Controller();
        private string _taskName = "-";
        private readonly List<string> _taskNames;
        private readonly Label _labelName = new Label() { Text = "-", TextAlignment = TextAlignment.Right, Height = _height };
        private readonly Label _labelType = new Label() { Text = "-", TextAlignment = TextAlignment.Right, Height = _height };
        private readonly Label _labelEnabled = new Label() { Text = "-", TextAlignment = TextAlignment.Right, Height = _height };
        private readonly ComboBox _box = new ComboBox() { Height = _height };

        private const int _height = 21;
        #endregion

        #region constructors
        /// <summary>
        /// Constructs the form.
        /// </summary>
        /// <param name="controller"> The controller to pick a task from. </param>
        public PickTaskForm(Controller controller)
        {
            // Main layout
            Title = "Controller task";
            MinimumSize = new Size(600, 420);
            Resizable = false;
            Padding = 20;

            // Task names
            _controller = controller;
            _taskNames = _controller.TasksABB.ConvertAll(item => item.Name);

            // Controls
            Button button = new Button() { Text = "OK" };
            _box = new ComboBox() { DataStore = _taskNames, Height = _height };

            // Assign events
            button.Click += ButtonClick;
            _box.SelectedIndexChanged += IndexChanged;

            // Select index
            _box.SelectedIndex = 0;

            // Labels
            Label selectLabel = new Label() { Text = "Select a task", Font = new Font(SystemFont.Bold), Height = _height };
            Label infoLabel = new Label() { Text = "Task info", Font = new Font(SystemFont.Bold), Height = _height };

            // Layout
            DynamicLayout layout = new DynamicLayout() { Padding = 0, Spacing = new Size(8, 4) };
            layout.AddSeparateRow(selectLabel);
            layout.AddSeparateRow(_box);
            layout.AddSeparateRow(new Label() { Text = " ", Height = _height });
            layout.AddSeparateRow(infoLabel);
            layout.AddSeparateRow(new Label() { Text = "Name", Height = _height }, _labelName);
            layout.AddSeparateRow(new Label() { Text = "Type", Height = _height }, _labelType);
            layout.AddSeparateRow(new Label() { Text = "Enabled", Height = _height }, _labelEnabled);
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
            _labelName.Text = _controller.TasksABB[_box.SelectedIndex].Name;
            _labelType.Text = _controller.TasksABB[_box.SelectedIndex].Type.ToString();
            _labelEnabled.Text = _controller.TasksABB[_box.SelectedIndex].Enabled.ToString();
        }

        private void ButtonClick(object sender, EventArgs e)
        {
            _taskName = _taskNames[_box.SelectedIndex];
            Close(true);
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets the picked task name.
        /// </summary>
        public string TaskName
        {
            get { return _taskName; }
        }
        #endregion
    }
}
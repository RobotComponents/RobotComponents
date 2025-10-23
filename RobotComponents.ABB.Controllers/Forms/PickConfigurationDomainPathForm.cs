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
// ABB Libs
using ABB.Robotics.Controllers.ConfigurationDomain;

namespace RobotComponents.ABB.Controllers.Forms
{
    /// <summary>
    /// Represents the pick configuration domain form class.
    /// </summary>
    public class PickConfigurationDomainPathForm : Dialog<bool>
    {
        #region fields
        private readonly Controller _controller;
        private readonly Label _labelValue = new Label() { Text = "-", TextAlignment = TextAlignment.Right, Height = _height };

        private readonly ComboBox _comboBoxDomain = new ComboBox() { Height = _height };
        private readonly ComboBox _comboBoxType = new ComboBox() { Height = _height };
        private readonly ComboBox _comboBoxInstance = new ComboBox() { Height = _height };
        private readonly ComboBox _comboBoxAttribute = new ComboBox() { Height = _height };

        private DomainCollection _domains = new DomainCollection();
        private TypeCollection _types = new TypeCollection();
        private Instance[] _instances = new Instance[0];
        private AttributeCollection _attributes = new AttributeCollection();

        private string _domain = "";
        private string _type = "";
        private string _instance = "";
        private string _attribute = "";

        private const int _height = 21;
        #endregion

        #region constructors
        /// <summary>
        /// Constructs the form.
        /// </summary>
        /// <param name="controller"> The controller to read from. </param>
        public PickConfigurationDomainPathForm(Controller controller)
        {
            // Main layout
            Title = "Read configuration domain";
            MinimumSize = new Size(600, 420);
            Resizable = false;
            Padding = 20;

            // Set controller
            _controller = controller;

            // Controls
            Button button = new Button() { Text = "OK" };

            // Assign events
            button.Click += ButtonClick;
            _comboBoxDomain.SelectedIndexChanged += ComboBoxDomainSelectedIndexChanged;
            _comboBoxType.SelectedIndexChanged += ComboBoxTypeSelectedIndexChanged;
            _comboBoxInstance.SelectedIndexChanged += ComboBoxInstanceSelectedIndexChanged;
            _comboBoxAttribute.SelectedIndexChanged += ComboBoxAttributeSelectedIndexChanged;

            // Populate
            PopulateDomains();

            // Labels
            Label selectLabel = new Label() { Text = "Select a path", Font = new Font(SystemFont.Bold), Height = _height };
            Label infoLabel = new Label() { Text = "Configuration data info", Font = new Font(SystemFont.Bold), Height = _height };

            // Layout
            DynamicLayout layout = new DynamicLayout() { Padding = 0, Spacing = new Size(8, 4) };
            layout.AddSeparateRow(selectLabel);
            layout.AddSeparateRow(new Label() { Text = "Domain", Width = 180, Height = _height }, _comboBoxDomain);
            layout.AddSeparateRow(new Label() { Text = "Type", Width = 180, Height = _height }, _comboBoxType);
            layout.AddSeparateRow(new Label() { Text = "Instance", Width = 180, Height = _height }, _comboBoxInstance);
            layout.AddSeparateRow(new Label() { Text = "Attribute", Width = 180, Height = _height }, _comboBoxAttribute);
            layout.AddSeparateRow(new Label() { Text = " ", Height = _height });
            layout.AddSeparateRow(infoLabel);
            layout.AddSeparateRow(new Label() { Text = "Value", Height = _height }, _labelValue);
            layout.AddSeparateRow(new Label() { Text = " ", Height = _height });
            layout.AddSeparateRow(new Label() { Text = " ", Height = _height });
            layout.AddSeparateRow(new Label() { Text = " ", Height = _height });
            layout.AddSeparateRow(new Label() { Text = " ", Height = _height });
            layout.AddSeparateRow(button);
            Content = layout;
        }
        #endregion

        #region methods
        private void ButtonClick(object sender, EventArgs e)
        {
            Close(true);
        }

        private void ComboBoxDomainSelectedIndexChanged(object sender, EventArgs e)
        {
            if (_comboBoxDomain.SelectedIndex > -1 && _domains.Count > 0)
            {
                _domain = _domains[_comboBoxDomain.SelectedIndex].Name;
            }
            else
            {
                _domain = "";
            }

            if (PopulateTypes() == false)
            {
                _types = new TypeCollection();
                _instances = new Instance[0];
                _attributes = new AttributeCollection();
                _type = "";
                _instance = "";
                _attribute = "";
                _labelValue.Text = "-";
                _comboBoxType.DataStore = new List<string>() { };
                _comboBoxInstance.DataStore = new List<string>() { };
                _comboBoxAttribute.DataStore = new List<string>() { };
                _comboBoxType.SelectedIndex = -1;
                _comboBoxInstance.SelectedIndex = -1;
                _comboBoxAttribute.SelectedIndex = -1;
            }
        }

        private void ComboBoxTypeSelectedIndexChanged(object sender, EventArgs e)
        {
            if (_comboBoxType.SelectedIndex > -1 && _types.Count > 0)
            {
                _type = _types[_comboBoxType.SelectedIndex].Name;
            }
            else
            {
                _type = "";
            }

            if (PopulateInstances() == false)
            {
                _instances = new Instance[0];
                _attributes = new AttributeCollection();
                _instance = "";
                _attribute = "";
                _labelValue.Text = "-";
                _comboBoxInstance.DataStore = new List<string>() { };
                _comboBoxAttribute.DataStore = new List<string>() { };
                _comboBoxInstance.SelectedIndex = -1;
                _comboBoxAttribute.SelectedIndex = -1;
            }
        }

        private void ComboBoxInstanceSelectedIndexChanged(object sender, EventArgs e)
        {
            if (_comboBoxInstance.SelectedIndex > -1 && _instances.Length > 0)
            {
                _instance = _instances[_comboBoxInstance.SelectedIndex].Name;
            }
            else
            {
                _instance = "";
            }

            if (PopulateAttributes() == false)
            {
                _attributes = new AttributeCollection();
                _attribute = "";
                _labelValue.Text = "-";
                _comboBoxAttribute.DataStore = new List<string>() { };
                _comboBoxInstance.SelectedIndex = -1;
                _comboBoxAttribute.SelectedIndex = -1;
            }
        }

        private void ComboBoxAttributeSelectedIndexChanged(object sender, EventArgs e)
        {
            if (_comboBoxAttribute.SelectedIndex > -1 && _attributes.Count > 0)
            {
                _attribute = _attributes[_comboBoxAttribute.SelectedIndex].Name;

                try
                {
                    _labelValue.Text = _controller.ReadConfigurationDomain(Domain, Type, Instance, Attribute);

                }
                catch
                {
                    _labelValue.Text = "-";
                }
            }
            else
            {
                _attribute = "";
            }
        }

        private bool PopulateDomains()
        {
            _comboBoxDomain.DataStore = new List<string>();
            _domains = _controller.ControllerABB.Configuration.Domains;

            if (_domains.Count > 0)
            {
                List<string> data = new List<string>();

                for (int i = 0; i < _domains.Count; i++)
                {
                    data.Add(_domains[i].Name);
                }

                _comboBoxDomain.DataStore = data;
                _comboBoxDomain.SelectedIndex = 0;

                return true;
            }

            return false;
        }

        private bool PopulateTypes()
        {
            _comboBoxType.DataStore = new List<string>();

            if (_comboBoxDomain.SelectedIndex > -1 & _domains.Count > 0)
            {
                _types = _domains[_comboBoxDomain.SelectedIndex].Types;

                if (_types.Count > 0)
                {
                    List<string> data = new List<string>();

                    for (int i = 0; i < _types.Count; i++)
                    {
                        data.Add(_types[i].Name);
                    }

                    _comboBoxType.DataStore = data;
                    _comboBoxType.SelectedIndex = 0;

                    return true;
                }
            }

            return false;
        }

        private bool PopulateInstances()
        {
            _comboBoxInstance.DataStore = new List<string>();

            if (_comboBoxType.SelectedIndex > -1 & _types.Count > 0)
            {
                _instances = _types[_comboBoxType.SelectedIndex].GetInstances();

                if (_instances.Length > 0)
                {
                    List<string> data = new List<string>();

                    for (int i = 0; i < _instances.Length; i++)
                    {
                        data.Add(_instances[i].Name);
                    }

                    _comboBoxInstance.DataStore = data;
                    _comboBoxInstance.SelectedIndex = 0;

                    return true;
                }
            }

            return false;
        }

        private bool PopulateAttributes()
        {
            _comboBoxAttribute.DataStore = new List<string>();

            if (_comboBoxType.SelectedIndex > -1 & _types.Count > 0)
            {
                _attributes = _types[_comboBoxType.SelectedIndex].Attributes;

                if (_attributes.Count > 0)
                {
                    List<string> data = new List<string>();

                    for (int i = 0; i < _attributes.Count; i++)
                    {
                        data.Add(_attributes[i].Name);
                    }

                    _comboBoxAttribute.DataStore = data;
                    _comboBoxAttribute.SelectedIndex = 0;

                    return true;
                }
            }

            return false;
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets the name of the domain.
        /// </summary>
        public string Domain
        {
            get { return _domain; }
        }

        /// <summary>
        /// Gets the type name. 
        /// </summary>
        public string Type
        {
            get { return _type; }
        }

        /// <summary>
        /// Gets the name of the instance.
        /// </summary>
        public string Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// Gets the name of the attribute. 
        /// </summary>
        public string Attribute
        {
            get { return _attribute; }
        }
        #endregion
    }
}
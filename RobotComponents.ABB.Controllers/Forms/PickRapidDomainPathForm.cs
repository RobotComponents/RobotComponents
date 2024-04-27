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
// ABB Libs
using ABB.Robotics.Controllers.RapidDomain;

namespace RobotComponents.ABB.Controllers.Forms
{
    /// <summary>
    /// Represents the pick RAPID domain form class.
    /// </summary>
    public class PickRapidDomainPathForm : Dialog<bool>
    {
        #region fields
        private readonly Controller _controller;
        private readonly Label _labelScope = new Label() { Text = "-", TextAlignment = TextAlignment.Right, Height = _height };
        private readonly Label _labelVariableType = new Label() { Text = "-", TextAlignment = TextAlignment.Right, Height = _height };
        private readonly Label _labelDataType = new Label() { Text = "-", TextAlignment = TextAlignment.Right, Height = _height };
        private readonly Label _labelValue = new Label() { Text = "-", TextAlignment = TextAlignment.Right, Height = _height };

        private readonly ComboBox _comboBoxTasks = new ComboBox() { Height = _height };
        private readonly ComboBox _comboBoxModules = new ComboBox() { Height = _height };
        private readonly ComboBox _comboBoxSymbols = new ComboBox() { Height = _height };

        private Task[] _tasks = new Task[0];
        private Module[] _modules = new Module[0];
        private RapidSymbol[] _symbols = new RapidSymbol[0];

        private string _task = "";
        private string _module = "";
        private string _symbol = "";

        private const int _height = 21;
        #endregion

        #region constructors
        /// <summary>
        /// Constructs the form.
        /// </summary>
        /// <param name="controller"> The controller to read from. </param>
        public PickRapidDomainPathForm(Controller controller)
        {
            // Main layout
            Title = "Read RAPID domain";
            MinimumSize = new Size(600, 420);
            Resizable = false;
            Padding = 20;

            // Set controller
            _controller = controller;

            // Controls
            Button button = new Button() { Text = "OK" };

            // Assign events
            button.Click += ButtonClick;
            _comboBoxTasks.SelectedIndexChanged += ComboBoxTaskSelectedIndexChanged;
            _comboBoxModules.SelectedIndexChanged += ComboBoxModuleSelectedIndexChanged;
            _comboBoxSymbols.SelectedIndexChanged += ComboBoxSymbolSelectedIndexChanged;

            // Populate
            PopulateTasks();

            // Labels
            Label selectLabel = new Label() { Text = "Select a path", Font = new Font(SystemFont.Bold), Height = _height };
            Label infoLabel = new Label() { Text = "RAPID data info", Font = new Font(SystemFont.Bold), Height = _height };

            // Layout
            DynamicLayout layout = new DynamicLayout() { Padding = 0, Spacing = new Size(8, 4) };
            layout.AddSeparateRow(selectLabel);
            layout.AddSeparateRow(new Label() { Text = "Task", Width = 180, Height = _height }, _comboBoxTasks);
            layout.AddSeparateRow(new Label() { Text = "Module", Width = 180, Height = _height }, _comboBoxModules);
            layout.AddSeparateRow(new Label() { Text = "Symbol", Width = 180, Height = _height }, _comboBoxSymbols);
            layout.AddSeparateRow(new Label() { Text = " ", Height = _height });
            layout.AddSeparateRow(infoLabel);
            layout.AddSeparateRow(new Label() { Text = "Data scope", Height = _height }, _labelScope);
            layout.AddSeparateRow(new Label() { Text = "Variable type", Height = _height }, _labelVariableType);
            layout.AddSeparateRow(new Label() { Text = "Data type", Height = _height }, _labelDataType);
            layout.AddSeparateRow(new Label() { Text = "Value", Height = _height }, _labelValue);
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

        private void ComboBoxTaskSelectedIndexChanged(object sender, EventArgs e)
        {
            if (_comboBoxTasks.SelectedIndex > -1 && _tasks.Length > 0)
            {
                _task = _tasks[_comboBoxTasks.SelectedIndex].Name;
            }

            if (PopulateModules() == false)
            {
                _modules = new Module[0];
                _symbols = new RapidSymbol[0];
                _labelScope.Text = "-";
                _labelVariableType.Text = "-";
                _labelDataType.Text = "-";
                _labelValue.Text = "-";
                _comboBoxModules.DataStore = new List<string>() { };
                _comboBoxSymbols.DataStore = new List<string>() { };
                _comboBoxModules.SelectedIndex = -1;
                _comboBoxSymbols.SelectedIndex = -1;
            }
        }

        private void ComboBoxModuleSelectedIndexChanged(object sender, EventArgs e)
        {
            if (_comboBoxModules.SelectedIndex > -1 && _modules.Length > 0)
            {
                _module = _modules[_comboBoxModules.SelectedIndex].Name;
            }

            if (PopulateSymbols() == false)
            {
                _symbols = new RapidSymbol[0];
                _symbol = "";
                _labelScope.Text = "-";
                _labelVariableType.Text = "-";
                _labelDataType.Text = "-";
                _labelValue.Text = "-";
                _comboBoxSymbols.DataStore = new List<string>() { };
                _comboBoxSymbols.SelectedIndex = -1;
            }
        }

        private void ComboBoxSymbolSelectedIndexChanged(object sender, EventArgs e)
        {
            if (_comboBoxSymbols.SelectedIndex > -1 && _symbols.Length > 0)
            {
                _symbol = _symbols[_comboBoxSymbols.SelectedIndex].Name;

                try
                {
                    RapidData data = _controller.ControllerABB.Rapid.GetRapidData(Task, Module, Symbol);

                    _labelScope.Text = _symbols[_comboBoxSymbols.SelectedIndex].Scope.ToString();
                    _labelVariableType.Text = _symbols[_comboBoxSymbols.SelectedIndex].Type.ToString();
                    _labelDataType.Text = data.RapidType;
                    _labelValue.Text = data.StringValue.Length > 50 ? data.StringValue.Substring(0, 50) + "....." : data.StringValue;
                }
                catch
                {
                    _labelScope.Text = "-";
                    _labelVariableType.Text = "-";
                    _labelDataType.Text = "-";
                    _labelValue.Text = "-";
                }
            }
            else
            {
                _symbol = "-";
            }
        }

        private bool PopulateTasks()
        {
            _comboBoxTasks.DataStore = new List<string>();
            _tasks = _controller.ControllerABB.Rapid.GetTasks();

            if (_tasks.Length > 0)
            {
                List<string> data = new List<string>();

                for (int i = 0; i < _tasks.Length; i++)
                {
                    data.Add(_tasks[i].Name);
                }

                _comboBoxTasks.DataStore = data;
                _comboBoxTasks.SelectedIndex = 0;

                return true;
            }

            return false;
        }

        private bool PopulateModules()
        {
            _comboBoxModules.DataStore = new List<string>();

            if (_comboBoxTasks.SelectedIndex > -1 & _tasks.Length > 0)
            {
                _modules = _tasks[_comboBoxTasks.SelectedIndex].GetModules();

                if (_modules.Length > 0)
                {
                    List<string> data = new List<string>();

                    for (int i = 0; i < _modules.Length; i++)
                    {
                        data.Add(_modules[i].Name);
                    }

                    _comboBoxModules.DataStore = data;
                    _comboBoxModules.SelectedIndex = 0;

                    return true;
                }
            }

            return false;
        }

        private bool PopulateSymbols()
        {
            _comboBoxSymbols.DataStore = new List<string>();

            if (_comboBoxModules.SelectedIndex > -1 & _modules.Length > 0)
            {
                RapidSymbolSearchProperties prop = RapidSymbolSearchProperties.CreateDefaultForData();
                _symbols = _modules[_comboBoxModules.SelectedIndex].SearchRapidSymbol(prop);

                if (_symbols.Length > 0)
                {
                    List<string> data = new List<string>();

                    for (int i = 0; i < _symbols.Length; i++)
                    {
                        data.Add(_symbols[i].Name);
                    }

                    _comboBoxSymbols.DataStore = data;
                    _comboBoxSymbols.SelectedIndex = 0;

                    return true;
                }
            }

            return false;

        }
        #endregion

        #region properties
        /// <summary>
        /// Gets the name of the task.
        /// </summary>
        public string Task
        {
            get { return _task; }
        }

        /// <summary>
        /// Gets the module name. 
        /// </summary>
        public string Module
        {
            get { return _module; }
        }

        /// <summary>
        /// Gets the name of the RAPID symbol
        /// </summary>
        public string Symbol
        {
            get { return _symbol; }
        }
        #endregion
    }
}
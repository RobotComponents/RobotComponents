// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Windows.Forms;
// ABB Libs
using ABB.Robotics.Controllers.RapidDomain;

namespace RobotComponents.ABB.Controllers.Forms
{
    /// <summary>
    /// Represents the pick path form class.
    /// </summary>
    public partial class PickRapidDomainPathForm : Form
    {
        #region fields
        private static Controller _controller;

        private Task[] _tasks = new Task[0];
        private Module[] _modules = new Module[0];
        private RapidSymbol[] _symbols = new RapidSymbol[0];

        private string _task = "";
        private string _module = "";
        private string _symbol = "";
        #endregion

        #region constructors
        /// <summary>
        /// Initiales a pick path form.
        /// </summary>
        /// <param name="controller"> The controller to read from. </param>
        public PickRapidDomainPathForm(Controller controller)
        {
            _controller = controller;
            InitializeComponent();
            PopulateTasks();
            _task = _tasks[0].Name;
        }
        #endregion

        #region methods
        private void Button1Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ComboBoxTaskSelectedIndexChanged(object sender, EventArgs e)
        {
            _task = _tasks[comboBoxTask.SelectedIndex].Name;

            if (PopulateModules() == false)
            {
                comboBoxModule.Items.Clear();
                comboBoxSymbol.Items.Clear();
                comboBoxModule.DataSource = null;
                comboBoxSymbol.DataSource = null;
                comboBoxModule.SelectedIndex = -1;
                comboBoxSymbol.SelectedIndex = -1;
                comboBoxModule.ResetText();
                comboBoxSymbol.ResetText();
                _modules = new Module[0];
                _symbols = new RapidSymbol[0];
                labelScopeInfo.Text = "-";
                labelVariableTypeInfo.Text = "-";
                labelDataTypeInfo.Text = "-";
                labelValueInfo.Text = "-";
            }
        }

        private void ComboBoxModuleSelectedIndexChanged(object sender, EventArgs e)
        {
            _module = _modules[comboBoxModule.SelectedIndex].Name;

            if (PopulateSymbols() == false)
            {
                comboBoxSymbol.Items.Clear();
                comboBoxSymbol.DataSource = null;
                comboBoxSymbol.SelectedIndex = -1;
                comboBoxSymbol.ResetText();
                _symbols = new RapidSymbol[0];
                _symbol = "";
                labelScopeInfo.Text = "-";
                labelVariableTypeInfo.Text = "-";
                labelDataTypeInfo.Text = "-";
                labelValueInfo.Text = "-";
            }
        }

        private void ComboBoxSymbolSelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxSymbol.SelectedIndex != -1 && _symbols.Length != 0)
            {
                _symbol = _symbols[comboBoxSymbol.SelectedIndex].Name;

                try
                {
                    RapidData data = _controller.ControllerABB.Rapid.GetRapidData(Task, Module, Symbol);

                    if (data.StringValue.Length > 50)
                    {
                        labelValueInfo.Text = data.StringValue.Substring(0, 50) + ".....";
                    }
                    else
                    {
                        labelValueInfo.Text = data.StringValue;
                    }

                    labelScopeInfo.Text = _symbols[comboBoxSymbol.SelectedIndex].Scope.ToString();
                    labelVariableTypeInfo.Text = _symbols[comboBoxSymbol.SelectedIndex].Type.ToString();
                    labelDataTypeInfo.Text = data.RapidType;
                }
                catch
                {
                    labelScopeInfo.Text = "-";
                    labelVariableTypeInfo.Text = "-";
                    labelDataTypeInfo.Text = "-";
                    labelValueInfo.Text = "-";
                }
            }
            else
            {
                _symbol = "-";
            }
        }

        private bool PopulateTasks()
        {
            comboBoxTask.Items.Clear();
            comboBoxTask.ResetText();

            _tasks = _controller.ControllerABB.Rapid.GetTasks();

            if (_tasks.Length != 0)
            {
                for (int i = 0; i < _tasks.Length; i++)
                {
                    comboBoxTask.Items.Add(_tasks[i].Name);
                }

                comboBoxTask.SelectedIndex = 0;

                return true;
            }

            return false;
        }

        private bool PopulateModules()
        {
            comboBoxModule.Items.Clear();
            comboBoxModule.ResetText();

            if (comboBoxTask.SelectedIndex != -1)
            {
                _modules = _tasks[comboBoxTask.SelectedIndex].GetModules();
            }
            else
            {
                return false;
            }

            if (_modules.Length != 0)
            {
                for (int i = 0; i < _modules.Length; i++)
                {
                    comboBoxModule.Items.Add(_modules[i].Name);
                }

                comboBoxModule.SelectedIndex = 0;

                return true;
            }

            return false;
        }

        private bool PopulateSymbols()
        {
            comboBoxSymbol.Items.Clear();
            comboBoxSymbol.ResetText();

            if (comboBoxModule.SelectedIndex != -1)
            {
                RapidSymbolSearchProperties prop = RapidSymbolSearchProperties.CreateDefaultForData();
                _symbols = _modules[comboBoxModule.SelectedIndex].SearchRapidSymbol(prop);
            }
            else
            {
                return false;
            }

            if (_symbols.Length != 0)
            {
                for (int i = 0; i < _symbols.Length; i++)
                {
                    comboBoxSymbol.Items.Add(_symbols[i].Name);
                }

                comboBoxSymbol.SelectedIndex = 0;

                return true;
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

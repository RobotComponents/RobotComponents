// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using GH_IO.Serialization;
// RobotComponents Libs
using RobotComponents.ABB.Actions.Generators;
using RobotComponents.ABB.Definitions;
using RobotComponents.ABB.Gh.Utils;
using RobotComponents.ABB.Gh.Parameters.Definitions;
using RobotComponents.ABB.Gh.Parameters.Actions;

namespace RobotComponents.ABB.Gh.Components.CodeGeneration
{
    /// <summary>
    /// RobotComponents Rapid Generator component. An inherent from the GH_Component Class.
    /// </summary>
    public class RAPIDGeneratorComponent : GH_Component, IGH_VariableParameterComponent
    {
        #region fields
        private RAPIDGenerator _rapidGenerator;
        private bool _firstMovementIsMoveAbsJ = true;
        private bool _raiseWarnings = false;
        private bool _programName = false;
        private bool _systemName = false;
        private bool _procedureName = false;
        private bool _customCode = false;
        private List<string> _programModule = new List<string>();
        private List<string> _systemModule = new List<string>();
        private readonly int _fixedParamNumInput = 2;
        #endregion

        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public RAPIDGeneratorComponent()
          : base("RAPID Generator", "RG",
              "Generates the RAPID program and system module for the ABB robot controller."
                + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.VersionNumbering.CurrentVersion,
              "Robot Components ABB", "Code Generation")
        {
            // Create the component label with a message
            Message = "EXTENDABLE";
        }

        /// <summary>
        /// Stores the variable input parameters in an array.
        /// </summary>
        private readonly IGH_Param[] variableInputParameters = new IGH_Param[5]
        {
            new Param_String() { Name = "Program Name", NickName = "PN", Description = "Name of the Pogram Module as a text. The default name is MainModule.", Access = GH_ParamAccess.item, Optional = true},
            new Param_String() { Name = "System Name", NickName = "SN", Description = "Name of the System Module as a text. The default name is BASE.", Access = GH_ParamAccess.item, Optional = true},
            new Param_String() { Name = "Procedure Name", NickName = "PN", Description = "Name of the RAPID procedure as a text. The default name is main.", Access = GH_ParamAccess.item, Optional = true},
            new Param_String() { Name = "Custom Code", NickName = "CC", Description = "Updates the RAPID Code based on a boolean value. To increase performance, only update when changes were made.", Access = GH_ParamAccess.list, Optional = true },
            new Param_Boolean() { Name = "Update", NickName = "U", Description = "Updates the RAPID Code based on a boolean value. To increase performance, only update when changes were made..", Access = GH_ParamAccess.item, Optional = true }
        };

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Robot(), "Robot", "R", "Robot that is used as Robot.", GH_ParamAccess.item);
            pManager.AddParameter(new Param_Action(), "Actions", "A", "Actions as list of instructive and declarative Actions.", GH_ParamAccess.list);

            AddParameter(4);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.Register_StringParam("Program Module", "PM", "RAPID Program Module", GH_ParamAccess.list); 
            pManager.Register_StringParam("System Module", "SM", "RAPID System Module", GH_ParamAccess.list); 
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            Robot robot = new Robot();
            List<RobotComponents.ABB.Actions.Action> actions = new List<RobotComponents.ABB.Actions.Action>();
            string programName = "MainModule";
            string systemName = "BASE";
            string procedureName = "main";
            List<string> customCodeLines = new List<string>() { };
            bool update = true;

            // Catch the input data
            if (!DA.GetData(0, ref robot)) { return; }
            if (!DA.GetDataList(1, actions)) { return; }

            // Catch the input data from the variable parameteres
            if (Params.Input.Any(x => x.Name == variableInputParameters[0].Name))
            {
                if (!DA.GetData(variableInputParameters[0].Name, ref programName))
                {
                    programName = "MainModule";
                }
            }
            if (Params.Input.Any(x => x.Name == variableInputParameters[1].Name))
            {
                if (!DA.GetData(variableInputParameters[1].Name, ref systemName))
                {
                    systemName = "BASE";
                }
            }
            if (Params.Input.Any(x => x.Name == variableInputParameters[2].Name))
            {
                if (!DA.GetData(variableInputParameters[2].Name, ref procedureName))
                {
                     procedureName= "main";
                }
            }
            if (Params.Input.Any(x => x.Name == variableInputParameters[3].Name))
            {
                if (!DA.GetDataList(variableInputParameters[3].Name, customCodeLines))
                {
                    customCodeLines = new List<string>() { };
                }
            }
            if (Params.Input.Any(x => x.Name == variableInputParameters[4].Name))
            {
                if (!DA.GetData(variableInputParameters[4].Name, ref update))
                {
                    update = true;
                }
            }

            // Checks if module name exceeds max character limit for RAPID Code
            if (HelperMethods.StringExeedsCharacterLimit32(programName))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Program module name exceeds character limit of 32 characters.");
            }
            if (HelperMethods.StringExeedsCharacterLimit32(systemName))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "System module name exceeds character limit of 32 characters.");
            }
            if (HelperMethods.StringExeedsCharacterLimit32(procedureName))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Procedure name exceeds character limit of 32 characters.");
            }

            // Checks if module name starts with a number
            if (HelperMethods.StringStartsWithNumber(programName))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Program module name starts with a number which is not allowed in RAPID Code.");
            }
            if (HelperMethods.StringStartsWithNumber(systemName))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "System module name starts with a number which is not allowed in RAPID Code.");
            }
            if (HelperMethods.StringStartsWithNumber(procedureName))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Procedure name starts with a number which is not allowed in RAPID Code.");
            }

            // Check if module name contains special character
            if (HelperMethods.StringStartsWithNumber(programName))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Program module name contains special characters.");
            }
            if (HelperMethods.StringStartsWithNumber(systemName))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "System module name contains special characters.");
            }
            if (HelperMethods.StringStartsWithNumber(procedureName))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Procedure name contains special characters.");
            }

            // Updates the rapid Progam and System code
            if (update == true)
            {
                // Initiaties the rapidGenerator
                _rapidGenerator = new RAPIDGenerator(robot, actions, programName, systemName, procedureName);

                // Generator code
                _rapidGenerator.CreateProgramModule();
                _rapidGenerator.CreateSystemModule(customCodeLines);
                _programModule = _rapidGenerator.ProgramModule;
                _systemModule = _rapidGenerator.SystemModule;

                // Check if the first movement is an absolute joint movement. 
                _firstMovementIsMoveAbsJ = _rapidGenerator.FirstMovementIsMoveAbsJ;

                // Raise warnings?
                if (_rapidGenerator.ErrorText.Count != 0)
                {
                    _raiseWarnings = true;
                }
                else
                {
                    _raiseWarnings = false;
                }
            }

            // Checks if first Movement is MoveAbsJ
            if (_firstMovementIsMoveAbsJ == false)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The first movement is not set as an absolute joint movement.");
            }

            // Show warning messages
            if (_raiseWarnings == true)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Only axis values of absolute joint movements are checked.");

                for (int i = 0; i < _rapidGenerator.ErrorText.Count; i++)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, _rapidGenerator.ErrorText[i]);
                    if (i == 30) { break; }
                }
            }

            // Output
            DA.SetDataList(0, _programModule);
            DA.SetDataList(1, _systemModule);
        }

        #region properties
        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            // Always place the RAPID generator in the last sub category
            get { return GH_Exposure.septenary; }
        }

        /// <summary>
        /// Gets whether this object is obsolete.
        /// </summary>
        public override bool Obsolete
        {
            get { return false; }
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return RobotComponents.ABB.Gh.Properties.Resources.RAPID_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("832B884B-D1EC-4197-8E3C-74E96A8F62EE"); }
        }
        #endregion

        #region menu items
        /// <summary>
        /// Adds the additional items to the context menu of the component. 
        /// </summary>
        /// <param name="menu"> The context menu of the component. </param>
        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            Menu_AppendSeparator(menu);
            Menu_AppendItem(menu, "Overwrite Program Module Name", MenuItemClickProgramName, true, _programName);
            Menu_AppendItem(menu, "Overwrite System Module Name", MenuItemClickSystemName, true, _systemName);
            Menu_AppendItem(menu, "Overwrite Procedure Name", MenuItemClickProcedureName, true, _procedureName);
            Menu_AppendItem(menu, "Set Custom System Code Lines", MenuItemClickCustomCode, true, _customCode);
            Menu_AppendSeparator(menu);
            Menu_AppendItem(menu, "Save Program module to file", MenuItemClickSaveProgramModule);
            Menu_AppendItem(menu, "Save System module to file", MenuItemClickSaveSystemModule);
            Menu_AppendSeparator(menu);
            Menu_AppendItem(menu, "Documentation", MenuItemClickComponentDoc, Properties.Resources.WikiPage_MenuItem_Icon);
        }

        /// <summary>
        /// Handles the event when the custom menu item "Set Program Module Name" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void MenuItemClickProgramName(object sender, EventArgs e)
        {
            RecordUndoEvent("Overwrite Program Module Name");
            _programName= !_programName;
            AddParameter(0);
        }

        /// <summary>
        /// Handles the event when the custom menu item "Set System Module Name" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void MenuItemClickSystemName(object sender, EventArgs e)
        {
            RecordUndoEvent("Overwrite System Module Name");
            _systemName = !_systemName;
            AddParameter(1);
        }

        /// <summary>
        /// Handles the event when the custom menu item "Set Procedure Name" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void MenuItemClickProcedureName(object sender, EventArgs e)
        {
            RecordUndoEvent("Overwrite Procedure Name");
            _procedureName = !_procedureName;
            AddParameter(2);
        }

        /// <summary>
        /// Handles the event when the custom menu item "Set System Custom Code Lines" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void MenuItemClickCustomCode(object sender, EventArgs e)
        {
            RecordUndoEvent("Set Custom System Code Lines");
            _customCode = !_customCode;
            AddParameter(3);
        }

        /// <summary>
        /// Handles the event when the custom menu item "Documentation" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void MenuItemClickComponentDoc(object sender, EventArgs e)
        {
            string url = Documentation.ComponentWeblinks[this.GetType()];
            Documentation.OpenBrowser(url);
        }

        /// <summary>
        /// Handles the event when the custom menu item "Save to Program module to file" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void MenuItemClickSaveProgramModule(object sender, EventArgs e)
        {
            SaveProgramModule();
        }

        /// <summary>
        /// Handles the event when the custom menu item "Save to System module to file" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void MenuItemClickSaveSystemModule(object sender, EventArgs e)
        {
            SaveSystemModule();
        }

        /// <summary>
        /// Save Program module to file
        /// </summary>
        private void SaveProgramModule()
        {
            // Create save file dialog
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                CheckFileExists = false,
                CheckPathExists = true,
                DefaultExt = "mod",
                Filter = "RAPID Program Module|*.mod",
                Title = "Save a RAPID Program Module"
            };

            // If result of dialog is OK the file can be saved
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Check the file name
                if (saveFileDialog.FileName != "")
                {
                    // Write RAPID code to file
                    using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName, false))
                    {
                        for (int i = 0; i != _programModule.Count; i++)
                        {
                            writer.WriteLine(_programModule[i]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Save System module to file
        /// </summary>
        private void SaveSystemModule()
        {
            // Create save file dialog
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                CheckFileExists = false,
                CheckPathExists = true,
                DefaultExt = "sys",
                Filter = "RAPID System Module|*.sys",
                Title = "Save a RAPID System Module"
            };

            // If result of dialog is OK the file can be saved
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Check the file name
                if (saveFileDialog.FileName != "")
                {
                    // Write RAPID code to file
                    using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName, false))
                    {
                        for (int i = 0; i != _systemModule.Count; i++)
                        {
                            writer.WriteLine(_systemModule[i]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Add our own fields. Needed for (de)serialization of the variable input parameters.
        /// </summary>
        /// <param name="writer"> Provides access to a subset of GH_Chunk methods used for writing archives. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("Program Module Name", _programName);
            writer.SetBoolean("System Module Name", _systemName);
            writer.SetBoolean("RAPID Procedure Name", _procedureName);
            writer.SetBoolean("Custom System Code Lines", _customCode);
            return base.Write(writer);
        }

        /// <summary>
        /// Read our own fields. Needed for (de)serialization of the variable input parameters.
        /// </summary>
        /// <param name="reader"> Provides access to a subset of GH_Chunk methods used for reading archives. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Read(GH_IReader reader)
        {
            _programName = reader.GetBoolean("Program Module Name");
            _systemName = reader.GetBoolean("System Module Name");
            _procedureName = reader.GetBoolean("RAPID Procedure Name");
            _customCode = reader.GetBoolean("Custom System Code Lines");
            return base.Read(reader);
        }

        /// <summary>
        /// Adds or destroys the input parameter to the component.
        /// </summary>
        /// <param name="index"> The index number of the parameter that needs to be added. </param>
        private void AddParameter(int index)
        {
            // Pick the parameter
            IGH_Param parameter = variableInputParameters[index];
            string name = variableInputParameters[index].Name;

            // If the parameter already exist: remove it
            if (Params.Input.Any(x => x.Name == name))
            {
                Params.UnregisterInputParameter(Params.Input.First(x => x.Name == name), true);
            }

            // Else remove the variable input parameter
            else
            {
                // The index where the parameter should be added
                int insertIndex = _fixedParamNumInput;

                // Check if other parameters are already added and correct the insert index
                for (int i = 0; i < index; i++)
                {
                    if (Params.Input.Any(x => x.Name == variableInputParameters[i].Name))
                    {
                        insertIndex += 1;
                    }
                }

                // Register the input parameter
                Params.RegisterInputParam(parameter, insertIndex);
            }

            // Expire solution and refresh parameters since they changed
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        #endregion

        #region variable input parameters
        /// <summary>
        /// This function will get called before an attempt is made to insert a parameter. 
        /// Since this method is potentially called on Canvas redraws, it must be fast.
        /// </summary>
        /// <param name="side"> Parameter side (input or output). </param>
        /// <param name="index"> Insertion index of parameter. Index=0 means the parameter will be in the topmost spot. </param>
        /// <returns> Return True if your component supports a variable parameter at the given location </returns>
        bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index)
        {
            return false;
        }

        /// <summary>
        /// This function will get called before an attempt is made to insert a parameter. 
        /// Since this method is potentially called on Canvas redraws, it must be fast.
        /// </summary>
        /// <param name="side"> Parameter side (input or output). </param>
        /// <param name="index"> Insertion index of parameter. Index=0 means the parameter will be in the topmost spot. </param>
        /// <returns> Return True if your component supports a variable parameter at the given location. </returns>
        bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index)
        {
            return false;
        }

        /// <summary>
        /// This function will be called when a new parameter is about to be inserted. 
        /// You must provide a valid parameter or insertion will be skipped. 
        /// You do not, repeat not, need to insert the parameter yourself.
        /// </summary>
        /// <param name="side"> Parameter side (input or output). </param>
        /// <param name="index"> Insertion index of parameter. Index=0 means the parameter will be in the topmost spot. </param>
        /// <returns> A valid IGH_Param instance to be inserted. In our case a null value. </returns>
        IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index)
        {
            return null;
        }

        /// <summary>
        /// This function will be called when a parameter is about to be removed. 
        /// You do not need to do anything, but this would be a good time to remove 
        /// any event handlers that might be attached to the parameter in question.
        /// </summary>
        /// <param name="side"> Parameter side (input or output). </param>
        /// <param name="index"> Insertion index of parameter. Index=0 means the parameter will be in the topmost spot. </param>
        /// <returns> Return True if the parameter in question can indeed be removed. Note, this is only in emergencies, 
        /// typically the CanRemoveParameter function should return false if the parameter in question is not removable. </returns>
        bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index)
        {
            return false;
        }

        /// <summary>
        /// This method will be called when a closely related set of variable parameter operations completes. 
        /// This would be a good time to ensure all Nicknames and parameter properties are correct. 
        /// This method will also be called upon IO operations such as Open, Paste, Undo and Redo.
        /// </summary>
        void IGH_VariableParameterComponent.VariableParameterMaintenance()
        {

        }
        #endregion
    }
}

// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.BaseClasses.Actions;
using RobotComponents.BaseClasses.Definitions;
using RobotComponentsABB.Utils;
using RobotComponentsABB.Parameters.Definitions;
using RobotComponentsABB.Parameters.Actions;

namespace RobotComponentsABB.Components.CodeGeneration
{
    /// <summary>
    /// RobotComponents Rapid Generator component. An inherent from the GH_Component Class.
    /// </summary>
    public class RAPIDGeneratorComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public RAPIDGeneratorComponent()
          : base("RAPID Generator", "RG",
              "Generates the RAPID program and system module for the ABB robot controller."
                + System.Environment.NewLine + System.Environment.NewLine +
                "RobotComponents: v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Code Generation")
        {
        }

        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            // Always place the rapid generator in the last sub category
            get { return GH_Exposure.septenary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new RobotParameter(), "Robot", "R", "Robot that is used as Robot", GH_ParamAccess.item);
            pManager.AddParameter(new ActionParameter(), "Actions", "A", "Actions as list of instructive and declarative Actions", GH_ParamAccess.list);
            pManager.AddTextParameter("Program Name", "PN", "Name of the Pogram Module as a text. The default name is MainModule", GH_ParamAccess.item, "MainModule");
            pManager.AddTextParameter("System Name", "SN", "Name of the System Module as a text. The default name is BASE", GH_ParamAccess.item, "BASE");
            pManager.AddTextParameter("Custom Code", "CC", "Custom code lines for the system module as a list of text", GH_ParamAccess.list);
            pManager.AddBooleanParameter("Update", "U", "Updates the RAPID Code based on a boolean value. To increase performance, only update when changes were made.", GH_ParamAccess.item, true);

            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_StringParam("Program Module", "PM", "RAPID Program Module"); 
            pManager.Register_StringParam("System Module", "SM", "RAPID System Module"); 
        }

        // Fields
        private RAPIDGenerator _rapidGenerator;
        private ObjectManager _objectManager;
        private bool _firstMovementIsMoveAbs = true;
        private bool _raiseWarnings = false;
        private string _programCode = "";
        private string _systemCode = "";

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Gets ObjectManager of this document
            _objectManager = DocumentManager.GetDocumentObjectManager(this.OnPingDocument());

            // Input variables
            Robot robInfo = new Robot();
            List<RobotComponents.BaseClasses.Actions.Action> actions = new List<RobotComponents.BaseClasses.Actions.Action>();
            string programName = "";
            string systemName = "";
            List<string> customCodeLines = new List<string>() { };
            bool update = true;

            // Catch the input data
            if (!DA.GetData(0, ref robInfo)) { return; }
            if (!DA.GetDataList(1, actions)) { return; }
            if (!DA.GetData(2, ref programName)) { programName = "MainModule"; }
            if (!DA.GetData(3, ref systemName)) { systemName = "BASE"; }
            if (!DA.GetDataList(4, customCodeLines)) { customCodeLines = new List<string>() { }; }
            if (!DA.GetData(5, ref update)) { update = true; }

            // Checks if module name exceeds max character limit for RAPID Code
            if (HelperMethods.VariableExeedsCharacterLimit32(programName))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Program Module Name exceeds character limit of 32 characters.");
            }
            if (HelperMethods.VariableExeedsCharacterLimit32(systemName))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "System Module Name exceeds character limit of 32 characters.");
            }

            // Checks if module name starts with a number
            if (HelperMethods.VariableStartsWithNumber(programName))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Program Module Name starts with a number which is not allowed in RAPID Code.");
            }
            if (HelperMethods.VariableStartsWithNumber(systemName))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "System Module Name starts with a number which is not allowed in RAPID Code.");
            }
          
            // Updates the rapid Progam and System code
            if (update == true)
            {
                // Initiaties the rapidGenerator
                _rapidGenerator = new RAPIDGenerator(programName, systemName, actions, null, false, robInfo);

                // Get tools data for system module
                List<RobotTool> robotTools = _objectManager.GetRobotTools(); // Gets all the robot tools from the object manager
                List<WorkObject> workObjects = _objectManager.GetWorkObjects(); // Gets all the work objects from the object manager

                // Generator code
                _rapidGenerator.CreateSystemCode(robotTools, workObjects, customCodeLines);
                _rapidGenerator.CreateProgramCode();
                _programCode = _rapidGenerator.ProgramCode;
                _systemCode = _rapidGenerator.SystemCode;

                // Check if the first movement is an absolute joint movement. 
                _firstMovementIsMoveAbs = _rapidGenerator.FirstMovementIsMoveAbs;

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
            if (_firstMovementIsMoveAbs == false)
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
            DA.SetData(0, _programCode);
            DA.SetData(1, _systemCode);
        }

        #region custom menu items
        /// <summary>
        /// Adds the additional items to the context menu of the component. 
        /// </summary>
        /// <param name="menu"> The context menu of the component. </param>
        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            Menu_AppendSeparator(menu);
            Menu_AppendItem(menu, "Save Program module to file", MenuItemClickSaveProgramModule);
            Menu_AppendItem(menu, "Save System module to file", MenuItemClickSaveSystemModule);
            Menu_AppendSeparator(menu);
            Menu_AppendItem(menu, "Documentation", MenuItemClickComponentDoc, Properties.Resources.WikiPage_MenuItem_Icon);
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
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.CheckFileExists = false;
            saveFileDialog.CheckPathExists = true;
            saveFileDialog.DefaultExt = "mod";
            saveFileDialog.Filter = "RAPID Program Module|*.mod";
            saveFileDialog.Title = "Save a RAPID Program Module";

            // If result of dialog is OK the file can be saved
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Check the file name
                if (saveFileDialog.FileName != "")
                {
                    // Write RAPID code to file
                    using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName, false))
                    {
                        writer.WriteLine(_programCode);
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
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.CheckFileExists = false;
            saveFileDialog.CheckPathExists = true;
            saveFileDialog.DefaultExt = "sys";
            saveFileDialog.Filter = "RAPID System Module|*.sys";
            saveFileDialog.Title = "Save a RAPID System Module";

            // If result of dialog is OK the file can be saved
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Check the file name
                if (saveFileDialog.FileName != "")
                {
                    // Write RAPID code to file
                    using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName, false))
                    {
                        writer.WriteLine(_systemCode);
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return RobotComponentsABB.Properties.Resources.RAPID_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("00E6B125-6B7F-4FAA-91B3-F01A45EC7B58"); }
        }
    }
}

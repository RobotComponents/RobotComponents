// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.ABB.Definitions;
using RobotComponents.ABB.Gh.Utils;
using RobotComponents.ABB.Gh.Parameters.Definitions;
using RobotComponents.ABB.Gh.Parameters.Actions;

namespace RobotComponents.ABB.Gh.Obsolete
{
    /// <summary>
    /// RobotComponents Rapid Generator component. An inherent from the GH_Component Class.
    /// </summary>
    [Obsolete("This component is OBSOLETE and will be removed in the future.", false)]
    public class RAPIDGeneratorComponent_OBSOLETE : GH_Component
    {
        #region fields
        private List<string> _programModule = new List<string>();
        private List<string> _systemModule = new List<string>();
        #endregion

        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public RAPIDGeneratorComponent_OBSOLETE()
          : base("RAPID Generator", "RG",
              "Generates the RAPID program and system module for the ABB robot controller."
                + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.VersionNumbering.CurrentVersion,
              "Robot Components ABB", "Code Generation")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Robot(), "Robot", "R", "Robot that is used as Robot", GH_ParamAccess.item);
            pManager.AddParameter(new Param_Action(), "Actions", "A", "Actions as list of instructive and declarative Actions", GH_ParamAccess.list);
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
            Robot robInfo = new Robot();
            List<RobotComponents.ABB.Actions.Action> actions = new List<RobotComponents.ABB.Actions.Action>();
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

            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "This component is OBSOLETE and is not operational anymore. Please select the new component from the toolbar.");

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
            get { return GH_Exposure.hidden; }
        }

        /// <summary>
        /// Gets whether this object is obsolete.
        /// </summary>
        public override bool Obsolete
        {
            get { return true; }
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
            get { return new Guid("00E6B125-6B7F-4FAA-91B3-F01A45EC7B58"); }
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
        #endregion
    }
}
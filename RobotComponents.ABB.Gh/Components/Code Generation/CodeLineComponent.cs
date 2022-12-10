// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.ABB.Actions;
using RobotComponents.ABB.Enumerations;
using RobotComponents.ABB.Gh.Parameters.Actions;
using RobotComponents.ABB.Gh.Utils;

namespace RobotComponents.ABB.Gh.Components.CodeGeneration
{
    /// <summary>
    /// RobotComponents Action : Code Line component. An inherent from the GH_Component Class.
    /// </summary>
    public class CodeLineComponent : GH_Component
    {
        #region fields
        private bool _expire = false;
        #endregion

        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public CodeLineComponent()
          : base("Code", "C",
              "Defines manually an instruction or declaration."
                + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.ABB.Utils.VersionNumbering.CurrentVersion,
              "Robot Components ABB", "Code Generation")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Code", "C", "Content of the code as text.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Type", "T", "Type of the code. Use 0 for adding the code as instruction. Use 1 for adding the code as declaration.", GH_ParamAccess.item, 0);

            pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_CodeLine(), "Code", "C", "Resulting Code", GH_ParamAccess.list);   
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Creates the input value list and attachs it to the input parameter
            if (this.Params.Input[1].SourceCount == 0)
            {
                _expire = true;
                HelperMethods.CreateValueList(this, typeof(CodeType), 1);
            }

            // Expire solution of this component
            if (_expire == true)
            {
                _expire = false;
                this.ExpireSolution(true);
            }

            // Input variables
            string text = null;
            int type = 0;

            // Catch the input data
            if (!DA.GetData(0, ref text)) { return; }
            if (!DA.GetData(1, ref type)) { return; }

            // Check if a right value is used for the code line type
            if (type != 0 && type != 1)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Code Line type value <" + type + "> is invalid. " +
                    "In can only be set to 0 or 1. Use 0 for creating instructions and 1 for creating a declarations.");
            }

            // Split input if enter is used
            string[] lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            // Create output
            List<CodeLine> codeLines = new List<CodeLine>();

            for (int i = 0; i < lines.Length; i++)
            {
                codeLines.Add(new CodeLine(lines[i], (CodeType)type));
            }

            // Sets Output
            DA.SetDataList(0, codeLines);
        }

        #region properties
        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.tertiary; }
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
            get { return Properties.Resources.CodeLine_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("0379F6AD-550E-41A8-B56C-6F33C667D521"); }
        }
        #endregion

        #region menu item
        /// <summary>
        /// Adds the additional items to the context menu of the component. 
        /// </summary>
        /// <param name="menu"> The context menu of the component. </param>
        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
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
        #endregion
    }
}

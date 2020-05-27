// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/EDEK-UniKassel/RobotComponents>.

// System Libs
using System;
using System.Windows.Forms;
using System.Drawing;
// Grasshopper Libs
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
// RobotComponents Libs
using RobotComponents.BaseClasses.Actions;
using RobotComponentsABB.Parameters.Actions;
using RobotComponentsABB.Utils;

namespace RobotComponentsABB.Components.CodeGeneration
{
    /// <summary>
    /// RobotComponents Action : Code Line component. An inherent from the GH_Component Class.
    /// </summary>
    public class CodeLineComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public CodeLineComponent()
          : base("CodeLine", "CL", "Instructive or Declarative Action" + System.Environment.NewLine + System.Environment.NewLine +
              "Defines manually an instruction or declaration for RAPID program code generation."
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
            get { return GH_Exposure.tertiary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Code", "C", "The custom code line as text", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Type", "T", "Comment Type as integer. Use 0 for creating an instructions, 1 for creating a declarations", GH_ParamAccess.item, 0);

            pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new CodeLineParameter(), "Code Line", "CL", "Resulting Code Line");  //Todo: beef this up to be more informative.
        }


        // Fields
        private bool _expire = false;

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Creates the input value list and attachs it to the input parameter
            CreateValueList();

            // Expire solution of this component
            if (_expire == true)
            {
                _expire = false;
                this.ExpireSolution(true);
            }

            // Input variables
            string code = null;
            int type = 0;

            // Cathc the input data
            if (!DA.GetData(0, ref code)) { return; }
            if (!DA.GetData(1, ref type)) { return; }

            // Create the action
            CodeLine codeLine = new CodeLine(code, type);

            // Check if a right value is used for the code line type
                if (type != 0 && type != 1)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Code Line type value <" + type + "> is invalid. " +
                        "In can only be set to 0 or 1. Use 0 for creating an instructions and 1 for creating a declarations." +
                        "Code Line type was automatically set to 0"); // This is happening in the constructor of the code line base class
            }

            // Sets Output
            DA.SetData(0, codeLine);
        }

        #region valuelist
        /// <summary>
        /// Creates the value list for the motion type and connects it the input parameter is other source is connected
        /// </summary>
        private void CreateValueList()
        {
            if (this.Params.Input[1].SourceCount == 0)
            {
                // Gets the input parameter
                var parameter = Params.Input[1];

                // Creates the empty value list
                GH_ValueList obj = new GH_ValueList();
                obj.CreateAttributes();
                obj.ListMode = Grasshopper.Kernel.Special.GH_ValueListMode.DropDown;
                obj.ListItems.Clear();

                // Add the items to the value list
                obj.ListItems.Add(new GH_ValueListItem("Instruction", "0"));
                obj.ListItems.Add(new GH_ValueListItem("Declaration", "1"));

                // Make point where the valuelist should be created on the canvas
                obj.Attributes.Pivot = new PointF(parameter.Attributes.InputGrip.X - 120, parameter.Attributes.InputGrip.Y - 11);

                // Add the value list to the active canvas
                Instances.ActiveCanvas.Document.AddObject(obj, false);

                // Connect the value list to the input parameter
                parameter.AddSource(obj);

                // Collect data
                parameter.CollectData();

                // Set bool for expire solution of this component
                _expire = true;

                // First expire the solution of the value list
                obj.ExpireSolution(true);
            }
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
            get { return new Guid("1672E4DC-A4CE-4987-BE02-4C2E9A3349F4"); }
        }

    }
}

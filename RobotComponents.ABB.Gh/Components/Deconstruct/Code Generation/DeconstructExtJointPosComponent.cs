// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.ABB.Actions;
using RobotComponents.ABB.Gh.Parameters.Actions;
using RobotComponents.ABB.Gh.Utils;

namespace RobotComponents.ABB.Gh.Components.Deconstruct.CodeGeneration
{ 
    /// <summary>
    /// RobotComponents Deconstruct External Joint Position component. An inherent from the GH_Component Class.
    /// </summary>
    public class DeconstructExtJointPosComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructExtJointPos class.
        /// </summary>
        public DeconstructExtJointPosComponent()
          : base("Deconstruct External Joint Position", "DeConExtJoint",
              "Deconstructs an External Joint Position Component into its parameters."
                + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components : v" + RobotComponents.ABB.Utils.VersionNumbering.CurrentVersion,
              "Robot Components ABB", "Deconstruct")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_ExternalJointPosition(), "External Joint Position", "EJ", "The External Joint Position.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.Register_StringParam("Name", "N", "External joint position variable name as text");
            pManager.Register_DoubleParam("External joint position A", "EJa", "Defines the position of the external logical axis A");
            pManager.Register_DoubleParam("External joint position B", "EJb", "Defines the position of the external logical axis B");
            pManager.Register_DoubleParam("External joint position C", "EJc", "Defines the position of the external logical axis C");
            pManager.Register_DoubleParam("External joint position D", "EJd", "Defines the position of the external logical axis D");
            pManager.Register_DoubleParam("External joint position E", "EJe", "Defines the position of the external logical axis E");
            pManager.Register_DoubleParam("External joint position F", "EJf", "Defines the position of the external logical axis F");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            ExternalJointPosition extJointPosition = null;

            // Catch the input data
            if (!DA.GetData(0, ref extJointPosition)) { return; }

            if (extJointPosition != null)
            {
                // Check if the object is valid
                if (!extJointPosition.IsValid)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The External Joint Position is not valid");
                }

                // Output
                DA.SetData(0, extJointPosition.Name);
                DA.SetData(1, extJointPosition[0]);
                DA.SetData(2, extJointPosition[1]);
                DA.SetData(3, extJointPosition[2]);
                DA.SetData(4, extJointPosition[3]);
                DA.SetData(5, extJointPosition[4]);
                DA.SetData(6, extJointPosition[5]);
            }
        }

        #region properties
        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary, dropdown and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.primary; }
        }

        /// <summary>
        /// Gets whether this object is obsolete.
        /// </summary>
        public override bool Obsolete
        {
            get { return false; }
        }

        /// <summary>
        /// Provides an Icon for the component
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.DeconstructExternalJointPosition_Icon; } 
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("DF0B0707-E0A1-4978-8FFD-FF7FE916AF6E"); }
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
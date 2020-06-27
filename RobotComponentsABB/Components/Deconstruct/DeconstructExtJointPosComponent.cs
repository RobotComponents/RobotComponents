// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.Actions;
using RobotComponentsABB.Parameters.Actions;
using RobotComponentsABB.Utils;

namespace RobotComponentsABB.Components.Deconstruct
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
                "Robot Components : v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Deconstruct")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new ExternalJointPositionParameter(), "External Joint Position", "EJ", "The External Joint Position.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_DoubleParam("External Axis Value A", "EAa", "Defines the first external axis value");
            pManager.Register_DoubleParam("External Axis Value B", "EAb", "Defines the second external axis value");
            pManager.Register_DoubleParam("External Axis Value C", "EAc", "Defines the third external axis value");
            pManager.Register_DoubleParam("External Axis Value D", "EAd", "Defines the fourth external axis value");
            pManager.Register_DoubleParam("External Axis Value E", "EAe", "Defines the fifth external axis value");
            pManager.Register_DoubleParam("External Axis Value F", "EAf", "Defines the sixth external axis value");
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

            // Check if the object is valid
            if (!extJointPosition.IsValid)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The External Joint Position is not valid");
            }

            // Output
            DA.SetData(0, extJointPosition[0]);
            DA.SetData(1, extJointPosition[1]);
            DA.SetData(2, extJointPosition[2]);
            DA.SetData(3, extJointPosition[3]);
            DA.SetData(4, extJointPosition[4]);
            DA.SetData(5, extJointPosition[5]);
        }

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
        /// Provides an Icon for the component
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return null; } // TODO
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("8461CEC9-AADE-4818-A657-09A51C38882F"); }
        }
    }
}
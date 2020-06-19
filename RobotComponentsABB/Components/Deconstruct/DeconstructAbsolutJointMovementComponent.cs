// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/EDEK-UniKassel/RobotComponents>.

// System Libs
using System;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.BaseClasses.Actions;
using RobotComponentsABB.Parameters.Actions;
using RobotComponentsABB.Parameters.Definitions;
using RobotComponentsABB.Utils;

namespace RobotComponentsABB.Components.Deconstruct
{
    /// <summary>
    /// RobotComponents Deconstruct Absolute Joint Movement component. An inherent from the GH_Component Class.
    /// </summary>
    public class DeconstructAbsoluteJointMovementComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructMovement class.
        /// </summary>
        public DeconstructAbsoluteJointMovementComponent()
          : base("Deconstruct Absolute Joint Movement", "DeConAbsMove", 
              "Deconstructs a Absolute Joint Movement Component into its parameters."
                + System.Environment.NewLine + System.Environment.NewLine +
                "RobotComponents : v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Deconstruct")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new AbsoluteJointMovementParameter(), "Absolute Joint Movement", "AJM", "Movement as Absolute Joint Movement", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "Name as a string.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Internal Axis Values", "IAV", "", GH_ParamAccess.list);
            pManager.AddNumberParameter("External Axis Values", "EAV", "", GH_ParamAccess.list);
            pManager.RegisterParam(new SpeedDataParameter(), "Speed Data", "SD", "Speed Data");
            pManager.Register_IntegerParam("Movement Type", "MT", "Movement Type as integer");
            pManager.RegisterParam(new ZoneDataParameter(), "Zone Data", "ZD", "Zone Data");
            pManager.RegisterParam(new RobotToolParameter(), "Override Robot Tool", "RT", "Override Robot Tool");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            AbsoluteJointMovement absolutJointMovement = null;

            // Catch the input data
            if (!DA.GetData(0, ref absolutJointMovement)) { return; }

            // Check if the object is valid
            if (!absolutJointMovement.IsValid)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The Absolute Joint Movement is not valid");
            }

            // Output
            DA.SetData(0, absolutJointMovement.Name);
            DA.SetDataList(1, absolutJointMovement.InternalAxisValues);
            DA.SetDataList(2, absolutJointMovement.ExternalAxisValues);
            DA.SetData(3, absolutJointMovement.SpeedData);
            DA.SetData(4, absolutJointMovement.MovementType);
            DA.SetData(5, absolutJointMovement.ZoneData);
            DA.SetData(6, absolutJointMovement.RobotTool);
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
            get { return Properties.Resources.DeconstructAbsoluteJointMovement_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("0D14D840-B4E4-4007-B139-BC69BF6C4660"); }
        }
    }
}
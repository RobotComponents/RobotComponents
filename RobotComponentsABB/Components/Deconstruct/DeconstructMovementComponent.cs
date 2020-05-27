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
    /// RobotComponents Deconstruct Movement component. An inherent from the GH_Component Class.
    /// </summary>
    public class DeconstructMovementComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructMovement class.
        /// </summary>
        public DeconstructMovementComponent()
          : base("Deconstruct Movement", "DeConMove", 
                "Action Deconstructor" + System.Environment.NewLine + System.Environment.NewLine +
              "Deconstructs a Movement Component into its parameters."
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
            pManager.AddParameter(new MovementParameter(), "Movement", "M", "Movement as Movement", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new TargetParameter(), "Target", "T", "Target Data");
            pManager.RegisterParam(new SpeedDataParameter(), "Speed Data", "SD", "Speed Data");
            pManager.Register_IntegerParam("Movement Type", "MT", "Movement Type as integer");
            pManager.RegisterParam(new ZoneDataParameter(), "Zone Data", "ZD", "Zone Data");
            pManager.RegisterParam(new RobotToolParameter(), "Override Robot Tool", "RT", "Override Robot Tool");
            pManager.RegisterParam(new WorkObjectParameter(), "Override Work Object", "WO", "Override Work Object");
            pManager.RegisterParam(new DigitalOutputParameter(), "Set Digital Output", "DO", "Digital Output for creation of MoveLDO and MoveJDO");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            Movement movement = null;

            // Catch the input data
            if (!DA.GetData(0, ref movement)) { return; }

            // Check if the object is valid
            if (!movement.IsValid)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The Movement is not valid");
            }

            // Output
            DA.SetData(0, movement.Target);
            DA.SetData(1, movement.SpeedData);
            DA.SetData(2, movement.MovementType);
            DA.SetData(3, movement.ZoneData);
            DA.SetData(4, movement.RobotTool);
            DA.SetData(5, movement.WorkObject);
            DA.SetData(6, movement.DigitalOutput);
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
            get { return Properties.Resources.DeconstructMovement_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("ACDB0D71-C805-4AEB-ADC9-99ACC740B096"); }
        }
    }
}
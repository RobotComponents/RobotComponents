// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/EDEK-UniKassel/RobotComponents>.

// System Libs
using System;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponentsGoos.Actions;
using RobotComponentsABB.Parameters.Actions;
using RobotComponentsABB.Parameters.Definitions;

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
              "Deconstructs a Movement Component into its parameters."
                + System.Environment.NewLine +
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
            pManager.Register_IntegerParam("Precision", "P", "Precision as int. If value is smaller than 0, precision will be set to fine.");
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
            GH_Movement movementGoo = null;

            // Catch the input data
            if (!DA.GetData(0, ref movementGoo)) { return; }

            // Output
            DA.SetData(0, movementGoo.Value.Target);
            DA.SetData(1, movementGoo.Value.SpeedData);
            DA.SetData(2, movementGoo.Value.MovementType);
            DA.SetData(3, movementGoo.Value.Precision);
            DA.SetData(4, movementGoo.Value.RobotTool);
            DA.SetData(5, movementGoo.Value.WorkObject);
            DA.SetData(6, movementGoo.Value.DigitalOutput);
        }

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
            get { return new Guid("1958CD54-4A01-4005-9C47-2E86AC2ABF05"); }
        }
    }
}
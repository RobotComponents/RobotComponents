using System;

using Grasshopper.Kernel;

using RobotComponents.Parameters;
using RobotComponents.Goos;

namespace RobotComponents.Components
{
    public class DeconstructMovementComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructMovement class.
        /// </summary>
        public DeconstructMovementComponent()
          : base("Deconstruct Movement", "DeConMove",
              "Deconstructs a Movement Component into its parameters."
                + System.Environment.NewLine +
                "RobotComponent V : " + RobotComponents.Utils.VersionNumbering.CurrentVersion,
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
            pManager.RegisterParam(new SpeedDataParam(), "Speed Data", "SD", "Speed Data");
            pManager.Register_IntegerParam("Movement Type", "MT", "Movement Type as integer");
            pManager.Register_IntegerParam("Precision", "P", "Precision as int. If value is smaller than 0, precision will be set to fine.");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            MovementGoo movementGoo = null;

            // Catch the input data
            if (!DA.GetData(0, ref movementGoo)) { return; }

            // Output
            DA.SetData(0, movementGoo.Value.Target);
            DA.SetData(1, movementGoo.Value.SpeedData);
            DA.SetData(2, movementGoo.Value.MovementType);
            DA.SetData(3, movementGoo.Value.Precision);
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
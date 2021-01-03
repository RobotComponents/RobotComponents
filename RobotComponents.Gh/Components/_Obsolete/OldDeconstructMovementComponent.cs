// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.Actions;
using RobotComponents.Gh.Parameters.Actions;
using RobotComponents.Gh.Parameters.Definitions;

// This component is OBSOLETE!
// It is OBSOLETE since version 0.08.000
// It is replaced with a new component. 

namespace RobotComponents.Gh.Components.Obsolete
{
    /// <summary>
    /// RobotComponents Deconstruct Movement component. An inherent from the GH_Component Class.
    /// </summary>
    [Obsolete("This component is obsolete and will be removed in the future.", false)]
    public class OldDeconstructMovementComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructMovement class.
        /// </summary>
        public OldDeconstructMovementComponent()
          : base("Deconstruct Movement", "DeConMove",
              "Deconstructs a Movement Component into its parameters."
                + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Deconstruct")
        {
        }

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
            // Warning that this component is OBSOLETE
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "This component is OBSOLETE and will be removed " +
                "in the future. Remove this component from your canvas and replace it by picking the new component " +
                "from the ribbon.");

            // Input variables
            Movement movement = null;

            // Catch the input data
            if (!DA.GetData(0, ref movement)) { return; }

            // Check if the object is valid
            if (!movement.IsValid)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The Movement is not valid");
            }

            // Set precision value
            int precision;
            if (movement.ZoneData.Name == "fine")
            {
                precision = -1;
            }
            else
            {
                precision = (int)movement.ZoneData.PathZoneTCP;
            }

            // Output
            DA.SetData(0, movement.Target);
            DA.SetData(1, movement.SpeedData);
            DA.SetData(2, (int)movement.MovementType);
            DA.SetData(3, precision);
            DA.SetData(4, movement.RobotTool);
            DA.SetData(5, movement.WorkObject);
            DA.SetData(6, movement.DigitalOutput);
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
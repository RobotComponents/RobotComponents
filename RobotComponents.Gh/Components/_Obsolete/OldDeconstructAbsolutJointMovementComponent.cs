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
using RobotComponents.Gh.Parameters.Obsolete;
using RobotComponents.Gh.Parameters.Actions;
using RobotComponents.Gh.Parameters.Definitions;

// This component is OBSOLETE!
// It is OBSOLETE since version 0.08.000
// It is replaced with a new component. 

namespace RobotComponents.Gh.Components.Obsolete
{
    /// <summary>
    /// RobotComponents Deconstruct Absolute Joint Movement component. An inherent from the GH_Component Class.
    /// </summary>
    [Obsolete("This component is obsolete and will be removed in the future.", false)]
    public class OldDeconstructAbsoluteJointMovementComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructMovement class.
        /// </summary>
        public OldDeconstructAbsoluteJointMovementComponent()
          : base("Deconstruct Absolute Joint Movement", "DeConAbsMove",
              "Deconstructs a Absolute Joint Movement Component into its parameters."
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
            pManager.AddParameter(new OldAbsoluteJointMovementParameter(), "Absolute Joint Movement", "AJM", "Movement as Absolute Joint Movement", GH_ParamAccess.item);
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
            pManager.Register_IntegerParam("Zone Data", "Z", "Precision as int. If value is smaller than 0, precision will be set to fine.");
            pManager.RegisterParam(new RobotToolParameter(), "Override Robot Tool", "RT", "Override Robot Tool");
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
            AbsoluteJointMovement absolutJointMovement = null;

            // Catch the input data
            if (!DA.GetData(0, ref absolutJointMovement)) { return; }

            // Check if the object is valid
            if (!absolutJointMovement.IsValid)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The Absolute Joint Movement is not valid");
            }

            // Set precision value
            int precision;
            if (absolutJointMovement.ZoneData.Name == "fine")
            {
                precision = -1;
            }
            else
            {
                precision = (int)absolutJointMovement.ZoneData.PathZoneTCP;
            }

            // Output
            DA.SetData(0, absolutJointMovement.Name);
            DA.SetDataList(1, absolutJointMovement.InternalAxisValues);
            DA.SetDataList(2, absolutJointMovement.ExternalAxisValues);
            DA.SetData(3, absolutJointMovement.SpeedData);
            DA.SetData(4, absolutJointMovement.MovementType);
            DA.SetData(5, precision);
            DA.SetData(6, absolutJointMovement.RobotTool);
        }

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
            get { return new Guid("4AD20439-0E9B-46F0-B114-9A4B9B8D49A0"); }
        }
    }
}
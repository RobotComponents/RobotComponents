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

namespace RobotComponentsABB.Components.Deconstruct
{
    /// <summary>
    /// RobotComponents Deconstruct Target component. An inherent from the GH_Component Class.
    /// </summary>
    public class DeconstructTargetComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructTarget class.
        /// </summary>
        public DeconstructTargetComponent()
          : base("Deconstruct Target", "DeConTar",
              "Deconstructs a Target into its parameters."
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
            pManager.AddParameter(new TargetParameter(), "Target", "T", "Target as Target", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_StringParam("Name", "N", "Name as string");
            pManager.Register_PlaneParam("Plane", "P", "Plane as Plane");
            pManager.Register_IntegerParam("Axis Configuration", "AC", "Axis Configuration as int. This will modify the fourth value of the Robot Configuration Data in the RAPID Movement code line.");
            pManager.Register_DoubleParam("External Axis Values", "EAV", "The user definied override external axis values.");
        }
   
        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            GH_Target targetGoo = null;

            // Catch the input data
            if (!DA.GetData(0, ref targetGoo)) { return; }

            // Output
            DA.SetData(0, targetGoo.Value.Name);
            DA.SetData(1, targetGoo.Value.Plane);
            DA.SetData(2, targetGoo.Value.AxisConfig);
            DA.SetDataList(3, targetGoo.Value.ExternalAxisValues);
        }

        /// <summary>
        /// Provides an Icon for the component
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.DeconstructTarget_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("D7F599E6-616C-40A8-87EC-CA36D9D74673"); }
        }
    }
}
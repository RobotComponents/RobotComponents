using System;

using Grasshopper.Kernel;

using RobotComponentsGoos.Definitions;
using RobotComponentsABB.Parameters.Definitions;

namespace RobotComponentsABB.Components.Deconstruct
{
    /// <summary>
    /// RobotComponents Deconstruct Work Object component. An inherent from the GH_Component Class.
    /// </summary>
    public class DeconstructWorkObjectComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructWorkObject class.
        /// </summary>
        public DeconstructWorkObjectComponent()
          : base("Deconstruct Work Object", "DeConTar",
              "Deconstructs a Work Object into its parameters."
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
            pManager.AddParameter(new WorkObjectParameter(), "Work Object", "WO", "Work Object as Work Object", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_StringParam("Name", "N", "Name as string");
            pManager.Register_PlaneParam("Plane", "WP", "Work Object Plane as a Plane");
            pManager.RegisterParam(new ExternalRotationalAxisParameter(), "External Rotational Axis", "ERA", "External Rotational Axis as an External Rotational Axis");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            GH_WorkObject workObjectGoo = null;

            // Catch the input data
            if (!DA.GetData(0, ref workObjectGoo)) { return; }

            // Output
            DA.SetData(0, workObjectGoo.Value.Name);
            DA.SetData(1, workObjectGoo.Value.Plane);
            DA.SetData(2, workObjectGoo.Value.ExternalAxis);
        }

        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
        }

        /// <summary>
        /// Provides an Icon for the component
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.DeconstructWorkObject_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("2EF73719-3E07-431E-8729-AB0C99848D0A"); }
        }
    }
}
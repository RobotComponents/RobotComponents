using System;

using Grasshopper.Kernel;

using RobotComponentsGoos.Definitions;
using RobotComponentsABB.Parameters.Definitions;

namespace RobotComponentsABB.Components.Deconstruct
{
    /// <summary>
    /// RobotComponents Deconstruct External Rotational Axis Component. An inherent from the GH_Component Class.
    /// </summary>
    public class DeconstructExternalRotationalAxisComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructRobotTool class.
        /// </summary>
        public DeconstructExternalRotationalAxisComponent()
          : base("Deconstruct External Rotational Axis", "DeConERA",
              "Deconstructs an External Rotational Axis into its parameters."
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
            pManager.AddParameter(new ExternalRotationalAxisParameter(), "External Rotational Axis", "ERA", "External Rotational Axis as External Rotational Axis", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "Axis Name as a Text", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Axis Plane", "AP", "Axis Plane as Plane", GH_ParamAccess.item);
            pManager.AddIntervalParameter("Axis Limits", "AL", "Axis Limits as Domain", GH_ParamAccess.item);
            pManager.AddMeshParameter("Base Mesh", "BM", "Base Mesh as Mesh", GH_ParamAccess.item);
            pManager.AddMeshParameter("Link Mesh", "LM", "Link Mesh as Mesh", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            GH_ExternalRotationalAxis externalRotationalAxisGoo = null;

            // Catch the input data
            if (!DA.GetData(0, ref externalRotationalAxisGoo)) { return; }

            // Output
            DA.SetData(0, externalRotationalAxisGoo.Value.Name);
            DA.SetData(1, externalRotationalAxisGoo.Value.AxisPlane);
            DA.SetData(2, externalRotationalAxisGoo.Value.AxisLimits);
            DA.SetData(3, externalRotationalAxisGoo.Value.BaseMesh);
            DA.SetData(4, externalRotationalAxisGoo.Value.LinkMesh);
        }

        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary, dropdown and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
        }

        /// <summary>
        /// Provides an Icon for the component
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.DeconstructExternalRotationalAxis_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("343ADC84-CD64-4F12-88FA-A0B6B3D98860"); }
        }
    }

}
using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace RobotComponentsABB.Components
{
    /// <summary>
    /// RobotComponents Flip Plane (make y-axis negative) component. An inherent from the GH_Component Class.
    /// </summary>
    public class FlipPlaneYComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the FlipPlaneComponent class.
        /// </summary>
        public FlipPlaneYComponent()
          : base("Flip Plane Y", "Flip Plane Y",
              "Flips the plane to the oposite direction by setting it's y-Axis negativ."
                + System.Environment.NewLine + "RobotComponents : v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Utility")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Plane", "P", "Plane as Plane", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_PlaneParam("Plane", "P", "Plane as Plane");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            List<Plane> planes = new List<Plane>();

            // Catch the input data
            if (!DA.GetDataList(0, planes)) { return; }

            // Flips the planes
            for (int i = 0; i < planes.Count; i++)
            {
                planes[i] = new Plane(planes[i].Origin, planes[i].XAxis, -planes[i].YAxis);
            }

            // Output
            DA.SetDataList(0, planes);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.FlipPlaneY_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("08EB7A3B-6DB3-4D22-B5B4-0652F60256BE"); }
        }
    }
}
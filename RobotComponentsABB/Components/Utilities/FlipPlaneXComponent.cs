using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace RobotComponentsABB.Components.Utilities
{
    /// <summary>
    /// RobotComponents Flip Plane (make x-axis negative) component. An inherent from the GH_Component Class.
    /// </summary>
    public class FlipPlaneXComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the FlipPlaneComponent class.
        /// </summary>
        public FlipPlaneXComponent()
          : base("Flip Plane X", "Flip Plane X",
              "Flips the plane to the oposite direction by setting it's x-Axis negativ." 
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
            for(int i = 0; i < planes.Count; i++)
            {
                planes[i] = new Plane(planes[i].Origin, -planes[i].XAxis, planes[i].YAxis); 
            }

            // Output
            DA.SetDataList(0, planes);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.FlipPlaneX_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("0f01736f-7dfd-4a52-9cc5-ebe8fa783731"); }
        }
    }
}
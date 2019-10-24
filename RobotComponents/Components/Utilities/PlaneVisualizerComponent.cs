using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Data;
using Rhino.Geometry;

namespace RobotComponents.Components
{

    public class PlaneVisualizerComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public PlaneVisualizerComponent()
          : base("Plane Visualizer", "PV",
              "Visualer for plane orientation."
                + System.Environment.NewLine +
                "RobotComponent V : " + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Utility")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Plane", "P", "Plane as Plane", GH_ParamAccess.tree);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        GH_Structure<GH_Plane> planes = new GH_Structure<GH_Plane>();

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            planes.Clear();
            if (!DA.GetDataTree(0, out planes)) { return; }
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.Plane_Visualizer_Icon;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("F861C697-DE9D-483E-9651-C53649775412"); }
        }

        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            for (int i = 0; i < planes.Branches.Count; i++)
            {
                var branches = planes.Branches[i];

                for (int j = 0; j < branches.Count; j++)
                {
                    Plane plane = branches[j].Value;
                    args.Display.DrawDirectionArrow(plane.Origin, plane.ZAxis, System.Drawing.Color.Blue);
                    args.Display.DrawDirectionArrow(plane.Origin, plane.XAxis, System.Drawing.Color.Red);
                    args.Display.DrawDirectionArrow(plane.Origin, plane.YAxis, System.Drawing.Color.Green);
                }
            }

        }
    }
}

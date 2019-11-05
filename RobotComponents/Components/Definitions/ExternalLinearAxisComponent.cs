using System;

using Grasshopper.Kernel;
using Rhino.Geometry;

using RobotComponents.Parameters;
using RobotComponents.BaseClasses;

namespace RobotComponents.Components.Definitions
{
    public class ExternalLinearAxisComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public ExternalLinearAxisComponent()
          : base("EXPERIMENTAL: External Linear Axis", "External Linear Axis",
              "Defines an External Linear Axis for any Robot."
                + System.Environment.NewLine +
                "RobotComponent V : " + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Definitions")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Attachment plane", "AP", "Attachement plane of robot. Overrides robot position plane.", GH_ParamAccess.item);
            pManager.AddVectorParameter("Axis", "A", "Axis as Vector", GH_ParamAccess.item);
            pManager.AddIntervalParameter("Limits", "L", "Axis Limits as Domain", GH_ParamAccess.item);
            pManager.AddMeshParameter("Base Mesh", "BM", "Base Mesh as Mesh", GH_ParamAccess.item);
            pManager.AddMeshParameter("Link Mesh", "LM", "Link Mesh as Mesh", GH_ParamAccess.item);

            pManager[3].Optional = true;
            pManager[4].Optional = true;
            // pManager.AddNumberParameter("Robot Attachment Distance", "RAD", "Defines the Robot Attachment Distance from the start Point along the External Linear Axis as Number", GH_ParamAccess.item);

            //pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new ExternalLinearAxisParameter(), "External Linear Axis", "ELA", "Contains External Linear Axis Data");  //Todo: beef this up to be more informative.
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Plane attachmentPlane = Plane.WorldXY;
            Vector3d axis = new Vector3d(0,0,0);
            Interval limits = new Interval(0, 0);
            Mesh baseMesh = null;
            Mesh linkMesh = null;

            if (!DA.GetData(0, ref attachmentPlane)) { return; }
            if (!DA.GetData(1, ref axis)) { return; }
            if (!DA.GetData(2, ref limits)) { return; }
            if (!DA.GetData(3, ref baseMesh)) {  }
            if (!DA.GetData(4, ref linkMesh)) {  }

            if(baseMesh == null)
            {
                baseMesh = new Mesh(); ;
            }

            if(linkMesh == null)
            {
                linkMesh = new Mesh();
            }

            ExternalLinearAxis externalLinearAxis = new ExternalLinearAxis(attachmentPlane, axis, limits, baseMesh, linkMesh);

            DA.SetData(0, externalLinearAxis);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return Properties.Resources.ExternalLinearAxis_Icon;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("C9916C52-8351-4883-9CC8-790C313A942E"); }
        }
    }
}

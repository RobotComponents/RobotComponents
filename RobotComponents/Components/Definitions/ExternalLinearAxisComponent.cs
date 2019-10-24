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
            pManager.AddPointParameter("Start Point", "SP", "External Linear Axis Start Point as Vector Position.", GH_ParamAccess.item);
            pManager.AddPointParameter("End Point", "EP", "External Linear Axis End Point as Vector Position.", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Attachment plane", "AP", "Attachement plane of robot. Overrides robot position plane.", GH_ParamAccess.item);
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
            Point3d startPoint = new Point3d(0,0,0);
            Point3d endPoint = new Point3d(0, 0, 0);
            //double robotAttachmentDistance = 0;
            Plane attachmentPlane = new Plane();

            if (!DA.GetData(0, ref startPoint)) { return; }
            if (!DA.GetData(1, ref endPoint)) { return; }
            ///if (!DA.GetData(2, ref robotAttachmentDistance)) { robotAttachmentDistance = 0; }
            if (!DA.GetData(2, ref attachmentPlane)) { return; }
            ExternalLinearAxis externalLinearAxis = new ExternalLinearAxis(startPoint, endPoint, attachmentPlane);

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

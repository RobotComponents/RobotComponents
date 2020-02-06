using System;

using Rhino.Geometry;
using Grasshopper.Kernel;

using RobotComponentsGoos.Definitions;
using RobotComponentsABB.Parameters;

namespace RobotComponentsABB.Components.Deconstruct
{
    /// <summary>
    /// RobotComponents Deconstruct Robot Tool component. An inherent from the GH_Component Class.
    /// </summary>
    public class DeconstructRobotToolComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructRobotTool class.
        /// </summary>
        public DeconstructRobotToolComponent()
          : base("Deconstruct Robot Tool", "DeRobTool",
              "Deconstructs a robot tool definition into its constituent parts"
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
            pManager.AddParameter(new RobotToolParameter(), "Robot Tool", "RT", "Robot Tool as Robot Tool", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "Robot Tool Name as String", GH_ParamAccess.item);
            pManager.AddMeshParameter("Mesh", "M", "Robot Tool Mesh as Mesh", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Attachment Plane", "AP", "Robot Tool Attachment Plane as Plane", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Tool Plane", "TP", "Robot Tool Plane as Plane", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            GH_RobotTool robotToolGoo = null;

            // Catch the input data
            if (!DA.GetData(0, ref robotToolGoo)) { return; }

            // Check input
            if (!robotToolGoo.IsValid || !robotToolGoo.Value.IsValid)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The RobotTool is not Valid");
                return;
            }

            // Output variables
            string name;
            Mesh mesh;
            Plane attachmentPlane;
            Plane toolPlane;

            // Name
            if (robotToolGoo.Value.Name != null)
            {
                name = robotToolGoo.Value.Name;
            }
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The RobotTool Name is not Valid");
                name = null;
            }

            // Meshes
            if (robotToolGoo.Value.Mesh != null)
            {
                mesh = robotToolGoo.Value.Mesh;
            }
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The RobotTool Meshes is not Valid");
                mesh = null;
            }

            // Attachment Plane
            if (robotToolGoo.Value.AttachmentPlane.IsValid)
            {
                attachmentPlane = robotToolGoo.Value.AttachmentPlane;
            }
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The RobotTool AttachmentPlane is not Valid");
                attachmentPlane = Plane.Unset;
            }

            // Tool Plane
            if (robotToolGoo.Value.ToolPlane.IsValid)
            {
                toolPlane = robotToolGoo.Value.ToolPlane;
            }
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The RobotTool ToolPlane is not Valid");
                toolPlane = Plane.Unset;
            }

            // Output
            DA.SetData(0, name);
            DA.SetData(1, mesh);
            DA.SetData(2, attachmentPlane);
            DA.SetData(3, toolPlane);
        }

        /// <summary>
        /// Provides an Icon for the component
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.DeconstructRobotTool_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("786e8c00-f24e-4dda-953d-cc8cffefa131"); }
        }
    }
}
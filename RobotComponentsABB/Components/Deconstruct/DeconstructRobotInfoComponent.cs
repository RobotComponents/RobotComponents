using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

using RobotComponentsABB.Goos;
using RobotComponentsABB.Parameters;

namespace RobotComponentsABB.Components
{
    public class DeconstructRobotInfoComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructRobotInfoComponent class.
        /// </summary>
        public DeconstructRobotInfoComponent()
          : base("Deconstruct Robot Info", "DeRobInfo",
              "Deconstructs a robot info definition into its constituent parts"
                + System.Environment.NewLine +
                "RobotComponent V : " + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Deconstruct")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new RobotInfoParameter(), "Robot Info", "RI", "Robot Info as Robot Info", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "Robot Name as String", GH_ParamAccess.item);
            pManager.AddMeshParameter("Meshes", "M", "Robot Meshes as Mesh List", GH_ParamAccess.list);
            pManager.AddPlaneParameter("Axis Planes", "AP", "Axis Planes as Plane List", GH_ParamAccess.list);
            pManager.AddIntervalParameter("Axis Limits", "AL", "Axis Limits as Interval List", GH_ParamAccess.list);
            pManager.AddPlaneParameter("Position Plane", "PP", "Position Plane of the Robot as Plane", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Mounting Frame", "MF", "Mounting Frame as Frame", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Tool Plane", "TP", "Tool Plane (TCP) as Frame", GH_ParamAccess.item);
            pManager.RegisterParam(new RobotToolParameter(), "Robot Tool", "RT", "Robot Tool");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            RobotInfoGoo robotInfoGoo = null;

            // Catch the input data
            if (!DA.GetData(0, ref robotInfoGoo)) { return; }

            // Check if the input is valid
            if (!robotInfoGoo.IsValid || !robotInfoGoo.Value.IsValid)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The RobotInfo is not Valid");
                return;
            }

            // Output variables
            string name;
            List<Mesh> meshes;
            List<Plane> axisPlanes;
            List<Interval> axisLimits;
            Plane basePlane;
            Plane mountingFrame;
            Plane toolPlane;
            RobotToolGoo tool;

            // Name
            if (robotInfoGoo.Value.Name != null)
            {
                name = robotInfoGoo.Value.Name;
            }
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The RobotInfo Name is not Valid");
                name = null;
            }

            // Meshes
            if (robotInfoGoo.Value.Meshes != null)
            {
                meshes = robotInfoGoo.Value.Meshes;
            }
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The RobotInfo Meshes is not Valid");
                meshes = null;
            }

            // AxisPlanes
            if (robotInfoGoo.Value.InternalAxisPlanes != null)
            {
                axisPlanes = robotInfoGoo.Value.InternalAxisPlanes;
            }
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The RobotInfo AxisPlanes is not Valid");
                axisPlanes = null;
            }

            // AxisLimits
            if (robotInfoGoo.Value.InternalAxisLimits != null)
            {
                axisLimits = robotInfoGoo.Value.InternalAxisLimits;
            }
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The RobotInfo AxisLimits is not Valid");
                axisLimits = null;
            }

            // BasePlane
            if (robotInfoGoo.Value.BasePlane.IsValid)
            {
                basePlane = robotInfoGoo.Value.BasePlane;
            }
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The RobotInfo BasePlane is not Valid");
                basePlane = Plane.Unset;
            }

            // Mounting Frame / Attachment Plane
            if (robotInfoGoo.Value.MountingFrame.IsValid)
            {
                mountingFrame = robotInfoGoo.Value.MountingFrame;
            }
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The RobotInfo MountingFrame is not Valid");
                mountingFrame = Plane.Unset;
            }

            // Tool Plane
            if (robotInfoGoo.Value.ToolPlane.IsValid)
            {
                toolPlane = robotInfoGoo.Value.ToolPlane;
            }
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The RobotInfo ToolPlane is not Valid");
                toolPlane = Plane.Unset;
            }

            // Robot Tool
            if (robotInfoGoo.Value.Tool.IsValid)
            {
                tool = new RobotToolGoo(robotInfoGoo.Value.Tool);
            }
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The RobotInfo Tool is not Valid");
                tool = null;
            }

            // Output
            DA.SetData(0, name);
            DA.SetDataList(1, meshes);
            DA.SetDataList(2, axisPlanes);
            DA.SetDataList(3, axisLimits);
            DA.SetData(4, basePlane);
            DA.SetData(5, mountingFrame);
            DA.SetData(6, toolPlane);
            DA.SetData(7, tool);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.DeconstructRobotInfoComponent_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("8452629c-5da8-4e64-82f2-23f00c49ae4b"); }
        }
    }
}
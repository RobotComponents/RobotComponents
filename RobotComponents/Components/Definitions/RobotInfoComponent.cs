using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

using RobotComponents.BaseClasses;
using RobotComponents.Goos;
using RobotComponents.Parameters;

namespace RobotComponents.Components.Definitions
{
    public class RobotInfoComponent : GH_Component
    {
        public RobotInfoComponent()
          : base("Robot Info", "RobInfo",
              "Defines a robot which is needed for Code Generation and Simulation"
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
            pManager.AddTextParameter("Name", "N", "Robot Name as String", GH_ParamAccess.item, "Empty RobotInfo");
            pManager.AddMeshParameter("Meshes", "M", "Robot Meshes as Mesh List", GH_ParamAccess.list);
            pManager.AddPlaneParameter("Axis Planes", "AP", "Axis Planes as Plane List", GH_ParamAccess.list);
            pManager.AddIntervalParameter("Axis Limits", "AL", "Axis Limits as Interval List", GH_ParamAccess.list);
            pManager.AddPlaneParameter("Position Plane", "PP", "Position Plane of the Robot as Plane", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Mounting Frame", "MF", "Mounting Frame as Frame", GH_ParamAccess.item);
            pManager.AddGenericParameter("Robot Tool", "RT", "Robot Tool as Robot Tool Parameter", GH_ParamAccess.item);
            pManager.AddGenericParameter("External Linear Axis", "ELA", "External Linear Axis as External Linear Axis Parameter", GH_ParamAccess.list);

            pManager[6].Optional = true;
            pManager[7].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new RobotInfoParameter(), "Robot Info", "RI", "Contains all Robot Data", GH_ParamAccess.item);  //Todo: beef this up to be more informative.
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string name = "default robot Info";
            List<Mesh> meshes = new List<Mesh>();
            List<Plane> axisPlanes = new List<Plane>();
            List<Interval> axisLimits = new List<Interval>();
            Plane positionPlane = Plane.WorldXY;
            Plane mountingFrame = Plane.Unset;
            RobotToolGoo toolGoo = null;
            List<ExternalAxis> externalAxis = new List<ExternalAxis>();

            if (!DA.GetData(0, ref name)) { return; }
            if (!DA.GetDataList(1, meshes)) { return; }
            if (!DA.GetDataList(2, axisPlanes))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No Axis Points !!!!");
                return;
            }
            if (!DA.GetDataList(3, axisLimits)) { return; }
            if (!DA.GetData(4, ref positionPlane)) { return; }
            if (!DA.GetData(5, ref mountingFrame)) { return; }
            if (!DA.GetData(6, ref toolGoo)) { toolGoo = new RobotToolGoo(); }
            if (!DA.GetDataList(7, externalAxis))
            {
            }

            // External axis limits
            for (int i = 0; i < externalAxis.Count; i++)
            {
                axisLimits.Add(externalAxis[i].AxisLimits);
            }

            RobotInfo robotInfo = null;

            // Override position plane when an external axis is coupled
            if (externalAxis.Count != 0)
            {
                for (int i = 0; i < externalAxis.Count; i++)
                {
                    if (externalAxis[i] is ExternalLinearAxis)
                    {
                        positionPlane = (externalAxis[i] as ExternalLinearAxis).AttachmentPlane;
                    }
                }
                robotInfo = new RobotInfo(name, meshes, axisPlanes, axisLimits, positionPlane, mountingFrame, toolGoo.Value, externalAxis);
            }
            else
            {
                robotInfo = new RobotInfo(name, meshes, axisPlanes, axisLimits, positionPlane, mountingFrame, toolGoo.Value);
            }

            DA.SetData(0, robotInfo);
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
                return Properties.Resources.RobotInfo_Icon;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("D62D3E73-6D93-4E80-9892-591DBEA648BE"); }
        }

        public override string ToString()
        {
            return "Robot Info";
        }
    }
}

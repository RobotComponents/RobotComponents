﻿using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

using RobotComponents.BaseClasses;
using RobotComponents.Goos;
using RobotComponents.Parameters;

namespace RobotComponents.Components
{
    public class IRB4600_20_2_50_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IRB4600_40_55_Component class.
        /// </summary>
        public IRB4600_20_2_50_Component()
          : base("ABB_IRB4600-20/2.5", "IRB4600",
              "An ABB_IRB4600-20/2.5 Info preset component."
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
            pManager.AddPlaneParameter("Position Plane", "PP", "Position Plane of the Robot as Plane", GH_ParamAccess.item, Plane.WorldXY);
            pManager.AddGenericParameter("Robot Tool", "RT", "Robot Tool as Robot Tool Parameter", GH_ParamAccess.item);
            pManager.AddGenericParameter("External Linear Axis", "ELA", "External Linear Axis as External Linear Axis Parameter", GH_ParamAccess.list);

            pManager[1].Optional = true;
            pManager[2].Optional = true;
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
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Plane positionPlane = Plane.WorldXY;
            RobotToolGoo toolGoo = null;
            List<ExternalAxis> externalAxis = new List<ExternalAxis>();

            if (!DA.GetData(0, ref positionPlane)) { return; }
            if (!DA.GetData(1, ref toolGoo))
            {
                toolGoo = new RobotToolGoo();
            }
            if (!DA.GetDataList(2, externalAxis))
            {
            }

            // Robot mesh
            List<Mesh> meshes = new List<Mesh>();
            // Base 
            string linkString = RobotComponents.Properties.Resources.IRB4600_20_2_50_link_0;
            meshes.Add((Mesh)GH_Convert.ByteArrayToCommonObject<GeometryBase>(System.Convert.FromBase64String(linkString)));
            // Axis 1
            linkString = RobotComponents.Properties.Resources.IRB4600_20_2_50_link_1;
            meshes.Add((Mesh)GH_Convert.ByteArrayToCommonObject<GeometryBase>(System.Convert.FromBase64String(linkString)));
            // Axis 2
            linkString = RobotComponents.Properties.Resources.IRB4600_20_2_50_link_2;
            meshes.Add((Mesh)GH_Convert.ByteArrayToCommonObject<GeometryBase>(System.Convert.FromBase64String(linkString)));
            // Axis 3
            linkString = RobotComponents.Properties.Resources.IRB4600_20_2_50_link_3;
            meshes.Add((Mesh)GH_Convert.ByteArrayToCommonObject<GeometryBase>(System.Convert.FromBase64String(linkString)));
            // Axis 4
            linkString = RobotComponents.Properties.Resources.IRB4600_20_2_50_link_4;
            meshes.Add((Mesh)GH_Convert.ByteArrayToCommonObject<GeometryBase>(System.Convert.FromBase64String(linkString)));
            // Axis 5
            linkString = RobotComponents.Properties.Resources.IRB4600_20_2_50_link_5;
            meshes.Add((Mesh)GH_Convert.ByteArrayToCommonObject<GeometryBase>(System.Convert.FromBase64String(linkString)));
            // Axis 6
            linkString = RobotComponents.Properties.Resources.IRB4600_20_2_50_link_6;
            meshes.Add((Mesh)GH_Convert.ByteArrayToCommonObject<GeometryBase>(System.Convert.FromBase64String(linkString)));

            // Axis planes
            List<Plane> axisPlanes = new List<Plane>();
            // Axis 1
            axisPlanes.Add(new Plane(
                new Point3d(0.00, 0.00, 0.00),
                new Vector3d(0.00, 0.00, 1.00)));
            // Axis 2
            axisPlanes.Add(new Plane(
                new Point3d(175.0, 0.00, 495.00),
                new Vector3d(0.00, 1.00, 0.00)));
            // Axis 3
            axisPlanes.Add(new Plane(
                new Point3d(175.00, 0.00, 495.00 + 1095.00),
                new Vector3d(0.00, 1.00, 0.00)));
            // Axis 4
            axisPlanes.Add(new Plane(
                new Point3d(505.60, 0.00, 495.00 + 1095.00 + 175.00),
                new Vector3d(1.00, 0.00, 0.00)));
            // Axis 5
            axisPlanes.Add(new Plane(
                new Point3d(175.0 + 1230.5, 0.00, 495.00 + 1095.00 + 175.00),
                new Vector3d(0.00, 1.00, 0.00)));
            // Axis 6
            axisPlanes.Add(new Plane(
                new Point3d(175.0 + 1230.5 + 85.0, 0.00, 495.00 + 1095.00 + 175.00),
                new Vector3d(1.00, 0.00, 0.00)));

            // Axis limits
            List<Interval> axisLimits = new List<Interval>{
                new Interval(-180, 180),
                new Interval(-90,150),
                new Interval(-180, 75),
                new Interval(-400, 400),
                new Interval(-120, 120),
                new Interval(-400, 400),};

            // External axis limits
            for (int i = 0; i < externalAxis.Count; i++)
            {
                axisLimits.Add(externalAxis[i].AxisLimits);
            }

            // Tool mounting frame
            Plane mountingFrame = new Plane(
                new Point3d(175.0 + 1230.5 + 85.0, 0.00, 495.00 + 1095.00 + 175.00),
                new Vector3d(1.00, 0.00, 0.00));
            mountingFrame.Rotate(Math.PI * -0.5, mountingFrame.Normal);


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
                robotInfo = new RobotInfo("ABB_IRB4600-X/2.5", meshes, axisPlanes, axisLimits, positionPlane, mountingFrame, toolGoo.Value, externalAxis);
            }
            else
            {
                robotInfo = new RobotInfo("ABB_IRB4600-20/2.5", meshes, axisPlanes, axisLimits, positionPlane, mountingFrame, toolGoo.Value);
            }

            DA.SetData(0, robotInfo);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return null; // Todo
                //return Properties.Resources.IRB4600_20_2_50_Icon;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("AEF02571-8AEA-41B8-9C58-5FA3B4099C92"); }
        }

    }
}
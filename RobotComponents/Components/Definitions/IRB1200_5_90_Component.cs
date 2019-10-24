using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

using RobotComponents.BaseClasses;
using RobotComponents.Goos;
using RobotComponents.Parameters;

namespace RobotComponents.Components
{
    public class IRB1200_5_90_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IRB1200_5_Component class.
        /// </summary>
        public IRB1200_5_90_Component()
          : base("ABB_IRB1200-5/0.9", "IRB1200",
              "An ABB IRB 1200-5/0.9 Robot Info preset component."
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
            pManager.AddGenericParameter("External Axis", "EA", "External Axis as External Axis Parameter", GH_ParamAccess.list);

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



            List<Mesh> meshes = new List<Mesh>();
            string linkString = RobotComponents.Properties.Resources.irb1200_5_90_base_link;
            meshes.Add((Mesh)GH_Convert.ByteArrayToCommonObject<GeometryBase>(System.Convert.FromBase64String(linkString)));
            linkString = RobotComponents.Properties.Resources.irb1200_5_90_link_1;
            meshes.Add((Mesh)GH_Convert.ByteArrayToCommonObject<GeometryBase>(System.Convert.FromBase64String(linkString)));
            linkString = RobotComponents.Properties.Resources.irb1200_5_90_link_2;
            meshes.Add((Mesh)GH_Convert.ByteArrayToCommonObject<GeometryBase>(System.Convert.FromBase64String(linkString)));
            linkString = RobotComponents.Properties.Resources.irb1200_5_90_link_3;
            meshes.Add((Mesh)GH_Convert.ByteArrayToCommonObject<GeometryBase>(System.Convert.FromBase64String(linkString)));
            linkString = RobotComponents.Properties.Resources.irb1200_5_90_link_4;
            meshes.Add((Mesh)GH_Convert.ByteArrayToCommonObject<GeometryBase>(System.Convert.FromBase64String(linkString)));
            linkString = RobotComponents.Properties.Resources.irb1200_5_90_link_5;
            meshes.Add((Mesh)GH_Convert.ByteArrayToCommonObject<GeometryBase>(System.Convert.FromBase64String(linkString)));
            linkString = RobotComponents.Properties.Resources.irb1200_5_90_link_6;
            meshes.Add((Mesh)GH_Convert.ByteArrayToCommonObject<GeometryBase>(System.Convert.FromBase64String(linkString)));

            List<Plane> axisPlanes = new List<Plane>();
            axisPlanes.Add(new Plane(
                new Point3d(0.00, 0.00, 0.00),
                new Vector3d(0.00, 0.00, 1.00)));
            axisPlanes.Add(new Plane(
                new Point3d(0.00, 0.00, 399.1),
                new Vector3d(0.00, 1.00, 0.00)));
            axisPlanes.Add(new Plane(
                new Point3d(0.00, 0.00, 847.1),
                new Vector3d(0.00, 1.00, 0.00)));
            axisPlanes.Add(new Plane(
                new Point3d(288.5, 0.00, 889.1), //x value meassured
                new Vector3d(1.00, 0.00, 0.00)));
            axisPlanes.Add(new Plane(
                new Point3d(451.00, 0.00, 889.1),
                new Vector3d(0.00, 1.00, 0.00)));
            axisPlanes.Add(new Plane(
                new Point3d(533.00, 0.00, 889.1),
                new Vector3d(1.00, 0.00, 0.00)));

            List<Interval> axisLimits = new List<Interval>{
                new Interval(-170, 170),
                new Interval(-100, +130),
                new Interval(-200, 70),
                new Interval(-270, 270),
                new Interval(-130, 130),
                new Interval(-400, 400),
             };

            for (int i = 0; i < externalAxis.Count; i++)
            {
                axisLimits.Add(externalAxis[i].AxisLimits);
            }

            Plane mountingFrame = new Plane(
                new Point3d(533.00, 0.00, 889.1),
                new Vector3d(1.00, 0.00, 0.00));
            mountingFrame.Rotate(Math.PI * -0.5, mountingFrame.Normal);

            RobotInfo robotInfo = null;

            // Override position plane when an external axis is coupled
            if (externalAxis.Count != 0)
            {
                for (int i = 0; i < externalAxis.Count; i++)
                {
                    if(externalAxis[i] is ExternalLinearAxis)
                    {
                        positionPlane = (externalAxis[i] as ExternalLinearAxis).AttachmentPlane;
                    }
                }

                robotInfo = new RobotInfo("IRB_1200-5/0.9", meshes, axisPlanes, axisLimits, positionPlane, mountingFrame, toolGoo.Value, externalAxis);
            }else
            {
                robotInfo = new RobotInfo("IRB_1200-5/0.9", meshes, axisPlanes, axisLimits, positionPlane, mountingFrame, toolGoo.Value);
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
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Properties.Resources.IRB1200_5_90_Icon;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("1583F982-72A3-4A5A-9A43-44B3D35FA174"); }
        }
    }
}
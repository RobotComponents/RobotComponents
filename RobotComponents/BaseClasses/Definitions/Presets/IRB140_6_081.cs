// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/EDEK-UniKassel/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
// Rhino Libs
using Rhino.Geometry;
// Robot Components Libs
using RobotComponents.Utils;

namespace RobotComponents.BaseClasses.Definitions.Presets
{
    /// <summary>
    /// Defines the IRB140-6/0.81
    /// </summary>
    public static class IRB140_6_081
    {
        /// <summary>
        /// Defines the IRB140-6/0.81 Robot Info
        /// </summary>
        /// <param name="positionPlane"> The position of the robot in world coordinate space as plane. </param>
        /// <param name="tool"> The robot end-effector as a Robot Tool. </param>
        /// <param name="externalAxis"> The external axes attaced to the robot as list with External Axes. </param>
        /// <returns> Returns the Robot Info preset. </returns>
        public static RobotInfo GetRobotInfo(string name, Plane positionPlane, RobotTool tool, List<ExternalAxis> externalAxis = null)
        {
            List<Mesh> meshes = GetMeshes();
            List<Plane> axisPlanes = GetAxisPlanes();
            List<Interval> axisLimits = GetAxisLimits();
            Plane mountingFrame = GetToolMountingFrame();

            // Override position plane when an external linear axis is coupled
            for (int i = 0; i < externalAxis.Count; i++)
            {
                if (externalAxis[i] is ExternalLinearAxis)
                {
                    positionPlane = (externalAxis[i] as ExternalLinearAxis).AttachmentPlane;
                    break;
                }
            }

            RobotInfo robotInfo = new RobotInfo(name, meshes, axisPlanes, axisLimits, Plane.WorldXY, mountingFrame, tool, externalAxis);
            Transform trans = Transform.PlaneToPlane(Plane.WorldXY, positionPlane);
            robotInfo.Transfom(trans);

            return robotInfo;
        }

        /// <summary>
        /// Defines the base and link meshes a robot coordinate space. 
        /// </summary>
        /// <returns> Returns a list with meshes. </returns>
        public static List<Mesh> GetMeshes()
        {
            List<Mesh> meshes = new List<Mesh>() { };
            string linkString;

            // Base
            linkString = RobotComponents.Properties.Resources.IRB140_6_0_81_link_0;
            meshes.Add((Mesh)HelperMethods.ByteArrayToObject(System.Convert.FromBase64String(linkString)));
            // Axis 1
            linkString = RobotComponents.Properties.Resources.IRB140_6_0_81_link_1;
            meshes.Add((Mesh)HelperMethods.ByteArrayToObject(System.Convert.FromBase64String(linkString)));
            // Axis 2
            linkString = RobotComponents.Properties.Resources.IRB140_6_0_81_link_2;
            meshes.Add((Mesh)HelperMethods.ByteArrayToObject(System.Convert.FromBase64String(linkString)));
            // Axis 3
            linkString = RobotComponents.Properties.Resources.IRB140_6_0_81_link_3;
            meshes.Add((Mesh)HelperMethods.ByteArrayToObject(System.Convert.FromBase64String(linkString)));
            // Axis 4
            linkString = RobotComponents.Properties.Resources.IRB140_6_0_81_link_4;
            meshes.Add((Mesh)HelperMethods.ByteArrayToObject(System.Convert.FromBase64String(linkString)));
            // Axis 5
            linkString = RobotComponents.Properties.Resources.IRB140_6_0_81_link_5;
            meshes.Add((Mesh)HelperMethods.ByteArrayToObject(System.Convert.FromBase64String(linkString)));
            // Axis 6
            linkString = RobotComponents.Properties.Resources.IRB140_6_0_81_link_6;
            meshes.Add((Mesh)HelperMethods.ByteArrayToObject(System.Convert.FromBase64String(linkString)));

            return meshes;
        }

        /// <summary>
        /// Defines the axis planes in robot coordinate space.
        /// </summary>
        /// <returns> Returns a list with planes. </returns>
        public static List<Plane> GetAxisPlanes()
        {
            List<Plane> axisPlanes = new List<Plane>() { };

            // Axis 1
            axisPlanes.Add(new Plane(
                new Point3d(0.00, 0.00, 0.00),
                new Vector3d(0.00, 0.00, 1.00)));
            // Axis 2
            axisPlanes.Add(new Plane(
                new Point3d(70.0, 0.00, 352.0),
                new Vector3d(0.00, 1.00, 0.00)));
            // Axis 3
            axisPlanes.Add(new Plane(
                new Point3d(70.0, 0.00, 712.0),
                new Vector3d(0.00, 1.00, 0.00)));
            // Axis 4
            axisPlanes.Add(new Plane(
                new Point3d(308.5, 0.00, 712.0),
                new Vector3d(1.00, 0.00, 0.00)));
            // Axis 5
            axisPlanes.Add(new Plane(
                new Point3d(450.00, 0.00, 712.00),
                new Vector3d(0.00, 1.00, 0.00)));
            // Axis 6
            axisPlanes.Add(new Plane(
                new Point3d(515.00, 0.00, 712.00),
                new Vector3d(1.00, 0.00, 0.00)));

            return axisPlanes;
        }

        /// <summary>
        /// Defines the axis limits. 
        /// </summary>
        /// <returns> Returns a list with intervals. </returns>
        public static List<Interval> GetAxisLimits()
        {
            List<Interval> axisLimits = new List<Interval> { };

            axisLimits.Add(new Interval(-180, 180));
            axisLimits.Add(new Interval(-90, 110));
            axisLimits.Add(new Interval(-230, 50));
            axisLimits.Add(new Interval(-200, 200));
            axisLimits.Add(new Interval(-115, 115));
            axisLimits.Add(new Interval(-400, 400));

            return axisLimits;
        }

        /// <summary>
        /// Defines the tool mounting frame in robot coordinate space. 
        /// </summary>
        /// <returns> Returns the mounting frame. </returns>
        public static Plane GetToolMountingFrame()
        {
            Plane mountingFrame = new Plane(
                new Point3d(515.00, 0.00, 712.0),
                new Vector3d(1.00, 0.00, 0.00));

            mountingFrame.Rotate(Math.PI* -0.5, mountingFrame.Normal);

            return mountingFrame;
        }
    }
}

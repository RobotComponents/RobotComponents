// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
// Rhino Libs
using Rhino.Geometry;
// Robot Components Libs
using RobotComponents.ABB.Definitions;
using RobotComponents.Utils;

namespace RobotComponents.ABB.Presets.Robots
{
    /// <summary>
    /// Represents a collection of methods to get the IRB1010-1.5/0.37 Robot instance.
    /// </summary>
    public static class IRB1010_15_037
    {
        /// <summary>
        /// Returns a new IRB1010-1.5/0.37 Robot instance. 
        /// </summary>
        /// <param name="positionPlane"> The position and orientation of the Robot in world coordinate space. </param>
        /// <param name="tool"> The Robot Tool. </param>
        /// <param name="externalAxes"> The external axes attached to the Robot. </param>
        /// <returns> The Robot preset. </returns>
        public static Robot GetRobot(Plane positionPlane, RobotTool tool, IList<ExternalAxis> externalAxes = null)
        {
            string name = "IRB1010-1.5/0.37";
            List<Mesh> meshes = GetMeshes();
            List<Plane> axisPlanes = GetAxisPlanes();
            List<Interval> axisLimits = GetAxisLimits();
            Plane mountingFrame = GetToolMountingFrame();

            // Make empty list with external axes if the value is null
            if (externalAxes == null)
            {
                externalAxes = new List<ExternalAxis>() { };
            }

            // Override the position plane when an external axis is coupled that moves the robot
            for (int i = 0; i < externalAxes.Count; i++)
            {
                if (externalAxes[i].MovesRobot == true)
                {
                    positionPlane = externalAxes[i].AttachmentPlane;
                    break;
                }
            }

            Robot robot = new Robot(name, meshes, axisPlanes, axisLimits, Plane.WorldXY, mountingFrame, tool, externalAxes);
            Transform trans = Transform.PlaneToPlane(Plane.WorldXY, positionPlane);
            robot.Transform(trans);

            return robot;
        }

        /// <summary>
        /// Returns the list with the base and link meshes of the robot in robot coordinate space.
        /// </summary>
        /// <returns> The list with robot meshes. </returns>
        public static List<Mesh> GetMeshes()
        {
            List<Mesh> meshes = new List<Mesh>() { };
            string linkString;

            // Base
            linkString = Properties.Resources.IRB1010_1_5_0_37_link_0;
            meshes.Add((Mesh)Serialization.ByteArrayToObject(System.Convert.FromBase64String(linkString)));
            // Axis 1
            linkString = Properties.Resources.IRB1010_1_5_0_37_link_1;
            meshes.Add((Mesh)Serialization.ByteArrayToObject(System.Convert.FromBase64String(linkString)));
            // Axis 2
            linkString = Properties.Resources.IRB1010_1_5_0_37_link_2;
            meshes.Add((Mesh)Serialization.ByteArrayToObject(System.Convert.FromBase64String(linkString)));
            // Axis 3
            linkString = Properties.Resources.IRB1010_1_5_0_37_link_3;
            meshes.Add((Mesh)Serialization.ByteArrayToObject(System.Convert.FromBase64String(linkString)));
            // Axis 4
            linkString = Properties.Resources.IRB1010_1_5_0_37_link_4;
            meshes.Add((Mesh)Serialization.ByteArrayToObject(System.Convert.FromBase64String(linkString)));
            // Axis 5
            linkString = Properties.Resources.IRB1010_1_5_0_37_link_5;
            meshes.Add((Mesh)Serialization.ByteArrayToObject(System.Convert.FromBase64String(linkString)));
            // Axis 6
            linkString = Properties.Resources.IRB1010_1_5_0_37_link_6;
            meshes.Add((Mesh)Serialization.ByteArrayToObject(System.Convert.FromBase64String(linkString)));

            return meshes;
        }

        /// <summary>
        /// Returns the list with the axis planes in robot coordinate space. 
        /// </summary>
        /// <returns> The list with axis planes. </returns>
        public static List<Plane> GetAxisPlanes()
        {
            List<Plane> axisPlanes = new List<Plane>() { };

            // Axis 1
            axisPlanes.Add(new Plane(
                new Point3d(0, 0, 0),
                new Vector3d(0, 0, 1)));
            // Axis 2
            axisPlanes.Add(new Plane(
                new Point3d(0, 0, 190),
                new Vector3d(0, 1, 0)));
            // Axis 3
            axisPlanes.Add(new Plane(
                new Point3d(0, 0, 190 + 185),
                new Vector3d(0, 1, 0)));
            // Axis 4
            axisPlanes.Add(new Plane(
                new Point3d(55.3, 0, 190 + 185),
                new Vector3d(1, 0, 0)));
            // Axis 5
            axisPlanes.Add(new Plane(
                new Point3d(185, 0, 190 + 185),
                new Vector3d(0, 1, 0)));
            // Axis 6
            axisPlanes.Add(new Plane(
                new Point3d(185 + 45, 0, 190 + 185),
                new Vector3d(1, 0, 0)));

            return axisPlanes;
        }

        /// <summary>
        /// Returns the list with axis limits.
        /// </summary>
        /// <returns> The list with axis limits. </returns>
        public static List<Interval> GetAxisLimits()
        {
            List<Interval> axisLimits = new List<Interval> { };

            axisLimits.Add(new Interval(-170, 170));
            axisLimits.Add(new Interval(-75, 125));
            axisLimits.Add(new Interval(-180, 50));
            axisLimits.Add(new Interval(-170, 170));
            axisLimits.Add(new Interval(-125, 125));
            axisLimits.Add(new Interval(-242, 242));

            return axisLimits;
        }

        /// <summary>
        /// Returns the tool mounting frame in robot coordinate space.
        /// </summary>
        /// <returns> The tool mounting frame. </returns>
        public static Plane GetToolMountingFrame()
        {
            Plane mountingFrame = new Plane(
                new Point3d(185 + 45, 0, 190 + 185),
                new Vector3d(1, 0, 0));

            mountingFrame.Rotate(Math.PI* -0.5, mountingFrame.Normal);

            return mountingFrame;
        }
    }
}

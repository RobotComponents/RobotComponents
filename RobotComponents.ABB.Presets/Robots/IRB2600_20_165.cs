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

namespace RobotComponents.ABB.Presets.Robots
{
    /// <summary>
    /// Represents a collection of methods to get the IRB2600-20/1.65 Robot instance.
    /// </summary>
    public static class IRB2600_20_165
    {
        /// <summary>
        /// Returns a new IRB2600-20/1.65 Robot instance.
        /// </summary>
        /// <param name="positionPlane"> The position and orientation of the Robot in world coordinate space. </param>
        /// <param name="tool"> The Robot Tool. </param>
        /// <param name="externalAxes"> The external axes attached to the Robot. </param>
        /// <returns> The Robot preset. </returns>
        public static Robot GetRobot(Plane positionPlane, RobotTool tool = null, IList<IExternalAxis> externalAxes = null)
        {
            string name = "IRB2600-20/1.65";
            List<Mesh> meshes = GetMeshes();
            List<Plane> axisPlanes = GetAxisPlanes();
            List<Interval> axisLimits = GetAxisLimits();
            Plane mountingFrame = GetToolMountingFrame();

            // Check Robot Tool data
            if (tool == null)
            {
                tool = new RobotTool();
            }

            // Make empty list with external axes if the value is null
            if (externalAxes == null)
            {
                externalAxes = new List<IExternalAxis>() { };
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
            List<Mesh> meshes = new List<Mesh>
            {
                Mesh.FromJSON(Properties.Resources.IRB2600_20_165_link_0) as Mesh,
                Mesh.FromJSON(Properties.Resources.IRB2600_20_165_link_1) as Mesh,
                Mesh.FromJSON(Properties.Resources.IRB2600_20_165_link_2) as Mesh,
                Mesh.FromJSON(Properties.Resources.IRB2600_20_165_link_3) as Mesh,
                Mesh.FromJSON(Properties.Resources.IRB2600_20_165_link_4) as Mesh,
                Mesh.FromJSON(Properties.Resources.IRB2600_20_165_link_5) as Mesh,
                Mesh.FromJSON(Properties.Resources.IRB2600_20_165_link_6) as Mesh,
            };

            return meshes;
        }

        /// <summary>
        /// Returns the list with axis limits.  
        /// </summary>
        /// <returns> The list with axis limits. </returns>
        public static List<Interval> GetAxisLimits()
        {
            List<Interval> axisLimits = new List<Interval>
            {
                new Interval(-180, 180),
                new Interval(-95, 155),
                new Interval(-180, 75),
                new Interval(-400, 400),
                new Interval(-120, 120),
                new Interval(-400, 400)
            };

            return axisLimits;
        }

        /// <summary>
        /// Returns the list with the axis planes in robot coordinate space. 
        /// </summary>
        /// <returns> Returns a list with planes. </returns>
        public static List<Plane> GetAxisPlanes()
        {
            List<Plane> axisPlanes = new List<Plane>() { };

            // Axis 1
            axisPlanes.Add(new Plane(
                new Point3d(0, 0, 0),
                new Vector3d(0, 0, 1)));
            // Axis 2
            axisPlanes.Add(new Plane(
                new Point3d(150.0, 0, 445),
                new Vector3d(0, 1, 0)));
            // Axis 3
            axisPlanes.Add(new Plane(
                new Point3d(150.0, 0, 445.0 + 700),
                new Vector3d(0, 1, 0)));
            // Axis 4
            axisPlanes.Add(new Plane(
                new Point3d(437.5, 0, 445.0 + 700.0 + 115),
                new Vector3d(1, 0, 0)));
            // Axis 5
            axisPlanes.Add(new Plane(
                new Point3d(150.0 + 795.0, 0, 445.0 + 700.0 + 115),
                new Vector3d(0, 1, 0)));
            // Axis 6
            axisPlanes.Add(new Plane(
                new Point3d(150.0 + 795.0 + 85.0, 0, 445.0 + 700.0 + 115),
                new Vector3d(1, 0, 0)));

            return axisPlanes;
        }

        /// <summary>
        /// Returns the tool mounting frame in robot coordinate space.
        /// </summary>
        /// <returns> The tool mounting frame. </returns>
        public static Plane GetToolMountingFrame()
        {
            Plane mountingFrame = new Plane(
                new Point3d(150.0 + 795.0 + 85.0, 0, 445.0 + 700.0 + 115),
                new Vector3d(1, 0, 0));

            mountingFrame.Rotate(-0.5 * Math.PI, mountingFrame.Normal);

            return mountingFrame;
        }
    }
}

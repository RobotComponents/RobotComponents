// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System.Collections.Generic;
using System.Linq;
// Rhino Libs
using Rhino.Geometry;
// Robot Components Libs
using RobotComponents.ABB.Definitions;

namespace RobotComponents.ABB.Presets.Robots
{
    /// <summary>
    /// Represents a collection of methods to get the IRB4600-60/2.05 Robot instance.
    /// </summary>
    public static class IRB4600_60_205
    {
        /// <summary>
        /// Returns a new IRB4600-60/2.05 Robot instance.
        /// </summary>
        /// <param name="positionPlane"> The position and orientation of the Robot in world coordinate space. </param>
        /// <param name="tool"> The Robot Tool. </param>
        /// <param name="externalAxes"> The external axes attached to the Robot. </param>
        /// <returns> The Robot preset. </returns>
        public static Robot GetRobot(Plane positionPlane, RobotTool tool = null, IList<IExternalAxis> externalAxes = null)
        {
            string name = "IRB4600-60/2.05";
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
                Mesh.FromJSON(Properties.Resources.IRB4600_60_205_link_0) as Mesh,
                Mesh.FromJSON(Properties.Resources.IRB4600_60_205_link_1) as Mesh,
                Mesh.FromJSON(Properties.Resources.IRB4600_60_205_link_2) as Mesh,
                Mesh.FromJSON(Properties.Resources.IRB4600_60_205_link_3) as Mesh,
                Mesh.FromJSON(Properties.Resources.IRB4600_60_205_link_4) as Mesh,
                Mesh.FromJSON(Properties.Resources.IRB4600_60_205_link_5) as Mesh,
                Mesh.FromJSON(Properties.Resources.IRB4600_60_205_link_6) as Mesh,
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
                new Interval(-90, 150),
                new Interval(-180, 75),
                new Interval(-400, 400),
                new Interval(-125, 120),
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
            Plane[] axisPlanes = Robot.GetAxisPlanesFromKinematicsParameters(Plane.WorldXY, 175, -175, 0, 0, 495, 900, 960, 135, out _);

            return axisPlanes.ToList();
        }

        /// <summary>
        /// Returns the tool mounting frame in robot coordinate space.
        /// </summary>
        /// <returns> The tool mounting frame. </returns>
        public static Plane GetToolMountingFrame()
        {
            Robot.GetAxisPlanesFromKinematicsParameters(Plane.WorldXY, 175, -175, 0, 0, 495, 900, 960, 135, out Plane mountingFrame);

            return mountingFrame;
        }
    }
}

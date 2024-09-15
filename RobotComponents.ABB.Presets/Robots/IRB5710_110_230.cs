// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
using System.Linq;
// Rhino Libs
using Rhino.Geometry;
// Robot Components Libs
using RobotComponents.ABB.Definitions;
using RobotComponents.Utils;

namespace RobotComponents.ABB.Presets.Robots
{
    /// <summary>
    /// Represents a collection of methods to get the IRB5710-110/2.3 Robot instance.
    /// </summary>
    public static class IRB5710_110_230
    {
        /// <summary>
        /// Returns a new IRB5710-110/2.3 Robot instance.
        /// </summary>
        /// <param name="positionPlane"> The position and orientation of the Robot in world coordinate space. </param>
        /// <param name="tool"> The Robot Tool. </param>
        /// <param name="externalAxes"> The external axes attached to the Robot. </param>
        /// <returns> The Robot preset. </returns>
        public static Robot GetRobot(Plane positionPlane, RobotTool tool = null, IList<IExternalAxis> externalAxes = null)
        {
            string name = "IRB5710-110/2.3";
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
        /// Defines the base and link meshes in robot coordinate space. 
        /// </summary>
        /// <returns> The list with robot meshes. </returns>
        public static List<Mesh> GetMeshes()
        {
            List<Mesh> meshes = new List<Mesh>
            {
                (Mesh)Serialization.ByteArrayToObject(Convert.FromBase64String(Properties.Resources.IRB5710_110_2_30_link_0)),
                (Mesh)Serialization.ByteArrayToObject(Convert.FromBase64String(Properties.Resources.IRB5710_110_2_30_link_1)),
                (Mesh)Serialization.ByteArrayToObject(Convert.FromBase64String(Properties.Resources.IRB5710_110_2_30_link_2)),
                (Mesh)Serialization.ByteArrayToObject(Convert.FromBase64String(Properties.Resources.IRB5710_110_2_30_link_3)),
                (Mesh)Serialization.ByteArrayToObject(Convert.FromBase64String(Properties.Resources.IRB5710_110_2_30_link_4)),
                (Mesh)Serialization.ByteArrayToObject(Convert.FromBase64String(Properties.Resources.IRB5710_110_2_30_link_5)),
                (Mesh)Serialization.ByteArrayToObject(Convert.FromBase64String(Properties.Resources.IRB5710_110_2_30_link_6))
            };

            return meshes;
        }

        /// <summary>
        /// Returns the list with axis limits.  
        /// </summary>
        /// <returns> The list with axis limits. </returns>
        public static List<Interval> GetAxisLimits()
        {
            List<Interval> limits = new List<Interval>
            {
                new Interval(-170, 170),
                new Interval(-75, 145),
                new Interval(-180, 70),
                new Interval(-300, 300),
                new Interval(-130, 130),
                new Interval(-360, 360)
            };

            return limits;
        }

        /// <summary>
        /// Returns the list with the axis planes in robot coordinate space. 
        /// </summary>
        /// <returns> Returns a list with planes. </returns>
        public static List<Plane> GetAxisPlanes()
        {
            Plane[] axisPlanes = Robot.GetAxisPlanesFromKinematicsParameters(Plane.WorldXY, 280, -180, 0, 0, 670, 950, 1055, 167, out _);

            return axisPlanes.ToList();
        }

        /// <summary>
        /// Returns the tool mounting frame in robot coordinate space.
        /// </summary>
        /// <returns> The tool mounting frame. </returns>
        public static Plane GetToolMountingFrame()
        {
            Robot.GetAxisPlanesFromKinematicsParameters(Plane.WorldXY, 280, -180, 0, 0, 670, 950, 1055, 167, out Plane mountingFrame);

            return mountingFrame;
        }
    }
}

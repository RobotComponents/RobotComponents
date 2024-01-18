// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
// Rhino Libs
using Rhino.Geometry;
// Robot Components Libs
using RobotComponents.ABB.Actions.Declarations;
using RobotComponents.ABB.Kinematics.IKFast.Geometry;

namespace RobotComponents.ABB.Kinematics.IKFast
{
    /// <summary>
    /// Comparer defining the sorting policy of RobotJointPositions
    /// </summary>
    /// <remarks>
    /// The policy is: The quadrants of the axes follow the order: 
    /// third, fourth, first, second; this corresponds to 
    /// (-2, -1, 0, 1) in the coding sheme of ABB's RobotStudio; and 
    /// priority decreases with increasing axis number
    /// </remarks>
    public class ConfigurationComparer : Comparer<RobotJointPosition>
    {
        /// <summary>
        /// Compares two Robot Joint Positions.
        /// </summary>
        /// <param name="px"> First Robot Joint Position. </param>
        /// <param name="py"> Second Robot Joint Position. </param>
        /// <returns></returns>
        public override int Compare(RobotJointPosition px, RobotJointPosition py)
        {
            // Convert to configuration data
            var x = JointPositionToConfigurationArray(px);
            var y = JointPositionToConfigurationArray(py);

            Debug.Assert(x.Length == y.Length);

            // Compare each element and proceed to the next only if equal
            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] > y[i])
                    return 1;

                if (x[i] < y[i])
                    return -1;
            }

            // All elements are equal
            return 0;
        }

        private static int[] JointPositionToConfigurationArray(RobotJointPosition joints)
        {
            // Determine the quadrant of robot axis 1, 4, and 6

            // Convert angle into integer indicating the quadrant following 
            // coding sheme of ABB's RobotStudio.
            Func<double, int> classifyQuadrant = delegate (double angle)
            {
                // First quadrant
                if (0.0 <= angle && angle < 90.0)
                    return 0;

                // Second quadrant
                if (90.0 <= angle && angle <= 180.0)
                    return 1;

                // Third quadrant, excluding -180
                if (-180.0 < angle && angle < -90.0)
                    return -2;

                // Fourth quadrant
                if (-90.0 <= angle && angle < 0.0)
                    return -1;

                throw new Exception("Angle not within valid range (-180, 180]");
            };

            return new int[] { classifyQuadrant(joints[0]), classifyQuadrant(joints[3]), classifyQuadrant(joints[5]) };
        }
    }
}

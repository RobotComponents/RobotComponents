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
    /// Inverse kinematics for the CRB15000-5/0.95 robot (GoFa 5).
    /// </summary>
    /// <remarks>
    /// This class wraps the ikfast library generated for the CRB15000-5/0.95 robot (GoFa 5). 
    /// So far, this is the only robot supported.
    /// </remarks>
    internal class IKFastSolver
    {
        #region
        private int _numSolutions = 0;
        private RobotJointPosition _robotJointPosition = new RobotJointPosition();
        private List<RobotJointPosition> _robotJointPositions = new List<RobotJointPosition>();
        private readonly Plane _base = Plane.WorldYZ;

        // Constants
        private const double _pi = Math.PI;
        private const double _rad2deg = 180.0 / _pi;
        #endregion

        #region constructor
        /// <summary>
        /// Initializes a new instance of the Inverse Kinematics class.
        /// </summary>
        public IKFastSolver()
        {

        }
        #endregion

        #region methods
        private void Reset()
        {
            _numSolutions = 0;
            _robotJointPositions.Clear();
            _robotJointPosition.Reset();
        }

        /// <summary>
        /// Computes the inverse kinematics.
        /// </summary>
        /// <param name="endPlane"> The robot end plane. </param>
        public void Compute(Plane endPlane)
        {
            // Check if system is 64-bit
            if (!System.Environment.Is64BitOperatingSystem)
            {
                throw new Exception("The IKFast Kinematics Solver cannot be used. The operating system is not 64-bit.");
            }

            // Reset the solution
            Reset();

            // Target rotated 90 degrees (unknown reason)
            Plane target = new Plane(endPlane);
            target.Rotate(0.5 * _pi, target.ZAxis, target.Origin);

            // Position
            IKFast.Geometry.Vector3d position = new IKFast.Geometry.Vector3d(target.Origin);

            // Orientation as quaternion
            Rhino.Geometry.Quaternion quaternion = Rhino.Geometry.Quaternion.Rotation(_base, target);
            IKFast.Geometry.Quaternion orientation = new IKFast.Geometry.Quaternion(quaternion);

            RobotJointPosition robotJointPosition;

            unsafe
            {
                Vector6d* joints_ikgen = computeInverseKinematics(ref position, ref orientation, out _numSolutions);

                for (int i = 0; i < _numSolutions; i++)
                {
                    robotJointPosition = joints_ikgen[i].ToRobotJointPosition();
                    robotJointPosition.Multiply(_rad2deg); // Radians to degrees

                    _robotJointPositions.Add(robotJointPosition);
                }
            }

            _robotJointPosition = _robotJointPositions.DefaultIfEmpty(new RobotJointPosition()).Last();
        }

        /// <summary>
        /// Sort the list of robot joint positions according to a policy.
        /// </summary>
        public void SortJointPositions()
        {
            // Use this policy
            Comparer<RobotJointPosition> comparer = new ConfigurationComparer();

            // Sort list of join positions in place
            _robotJointPositions.Sort(comparer);
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets the latest calculated Robot Joint Position.
        /// </summary>
        public RobotJointPosition RobotJointPosition
        {
            get { return _robotJointPosition; }
        }

        /// <summary>
        /// Gets all calculated Robot Joint Positions.
        /// </summary>
        public List<RobotJointPosition> RobotJointPositions
        {
            get { return _robotJointPositions; }
        }

        /// <summary>
        /// Get number of computed inverse kinematics solutions.
        /// </summary>
        public int NumSolutions
        {
            get { return _numSolutions; }
        }
        #endregion

        #region dll import
        [DllImport("rcik.dll", EntryPoint = "computeInverseKinematics")]
        private static unsafe extern Vector6d* computeInverseKinematics(ref IKFast.Geometry.Vector3d eePos, ref IKFast.Geometry.Quaternion eeOri, out int n_sol);
        #endregion
    }

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
        public override int Compare(RobotJointPosition px, RobotJointPosition py)
        {
            // Convert to configuration data
            var x = JointPositionToConfigurationArray( px );
            var y = JointPositionToConfigurationArray( py );

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

                throw new Exception("angle not within valid range (-180, 180]");
            };

            return new int[] { classifyQuadrant(joints[0]), classifyQuadrant(joints[3]), classifyQuadrant(joints[5]) };
        }

    }
}
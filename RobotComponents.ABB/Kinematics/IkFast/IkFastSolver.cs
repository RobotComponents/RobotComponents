// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
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
        private const double _cos45 = 0.70710678118;
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

            // Position
            IKFast.Geometry.Vector3d position = new IKFast.Geometry.Vector3d(endPlane.Origin);

            // Orientation as quaternion
            Rhino.Geometry.Quaternion quaternion = Rhino.Geometry.Quaternion.Rotation(_base, endPlane);
            Rhino.Geometry.Quaternion quaternionCorrection = new Rhino.Geometry.Quaternion(_cos45, 0.0, _cos45, 0.0); // Unkown 90 degrees correction
            Rhino.Geometry.Quaternion quaternionCorrected = quaternion * quaternionCorrection.Inverse;
            IKFast.Geometry.Quaternion orientation = new IKFast.Geometry.Quaternion(quaternionCorrected);

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
}
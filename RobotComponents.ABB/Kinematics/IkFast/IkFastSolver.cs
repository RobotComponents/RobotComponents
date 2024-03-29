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
using RobotComponents.ABB.Definitions;
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
        private List<RobotJointPosition> _robotJointPositions = new List<RobotJointPosition>();
        private List<RobotJointPosition> _robotJointPositionsArranged = new List<RobotJointPosition>();
        private readonly Plane _base = Plane.WorldYZ;
        private Robot _robot;

        // Constants
        private const double _pi = Math.PI;
        private const double _rad2deg = 180.0 / _pi;
        #endregion

        #region constructor
        /// <summary>
        /// Initializes a new instance of the Inverse Kinematics class.
        /// </summary>
        public IKFastSolver(Robot robot)
        {
            _robot = robot;
        }
        #endregion

        #region methods
        private void Reset()
        {
            _numSolutions = 0;
            _robotJointPositions.Clear();
        }

        /// <summary>
        /// Computes the inverse kinematics for robot CRB15000_5_095.
        /// </summary>
        /// <param name="endPlane"> The robot end plane. </param>
        public void Compute_CRB15000_5_095(Plane endPlane)
        {
            // Check if system is 64-bit
            if (!System.Environment.Is64BitOperatingSystem)
            {
                throw new Exception("The IKFast Kinematics Solver cannot be used. The operating system is not 64-bit.");
            }

            // Check that robot matches
            // TODO refactor to avoid this.
            if (_robot.Name != "CRB15000-5/0.95")
            {
                throw new Exception("Robot does not match function call.");
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
                Vector6d* joints_ikgen = computeIK_CRB15000_5_095(ref position, ref orientation, out _numSolutions);

                for (int i = 0; i < _numSolutions; i++)
                {
                    robotJointPosition = joints_ikgen[i].ToRobotJointPosition();
                    robotJointPosition.Multiply(_rad2deg); // Radians to degrees

                    _robotJointPositions.Add(robotJointPosition);
                }
            }

            ArrangeJointPositions();
        }

        /// <summary>
        /// Computes the inverse kinematics for robot CRB15000_10_152.
        /// </summary>
        /// <param name="endPlane"> The robot end plane. </param>
        public void Compute_CRB15000_10_152(Plane endPlane)
        {
            // Check if system is 64-bit
            if (!System.Environment.Is64BitOperatingSystem)
            {
                throw new Exception("The IKFast Kinematics Solver cannot be used. The operating system is not 64-bit.");
            }

            // Check that robot matches
            if (_robot.Name != "CRB15000-10/152")
            {
                throw new Exception("Robot does not match function call.");
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
                Vector6d* joints_ikgen = computeIK_CRB15000_10_152(ref position, ref orientation, out _numSolutions);

                for (int i = 0; i < _numSolutions; i++)
                {
                    robotJointPosition = joints_ikgen[i].ToRobotJointPosition();
                    robotJointPosition.Multiply(_rad2deg); // Radians to degrees

                    _robotJointPositions.Add(robotJointPosition);
                }
            }

            ArrangeJointPositions();
        }

        /// <summary>
        /// Arrange joint positions according to the configuration parameter Cfx.
        /// </summary>
        /// <remarks>
        /// The resulting list consists of 8 joint positions, where missing solutions
        /// from ikfast are filled with default values (9e9). The definition of the
        /// Cfx parameter is taken from InverseKinematics.cs (main branch).
        /// </remarks>
        private void ArrangeJointPositions()
        {
            // Cfx parameter defintion:
            //
            // Sol.    Wrist center            Wrist center            Axis 5 angle
            // Cfx     relative to axis 1      relative to lower arm
            //         
            // 0       In front of             In front of             Positive
            // 1       In front of             In front of             Negative
            // 2       In front of             Behind                  Positive
            // 3       In front of             Behind                  Negative     
            // 4       Behind                  In front of             Positive
            // 5       Behind                  In front of             Negative
            // 6       Behind                  Behind                  Positive
            // 7       Behind                  Behind                  Negative
            // 
            // Wrist center:    Point on axis 6 closest to axis 5; taking center point
            //                  of mechanical axis 6 for simplicity.
            // Lower arm:       Center point of the mechanical axis 4 (assumably).

            ForwardKinematics fk = new ForwardKinematics(_robot, true);

            // Initialize 8 robot joint positions with default values
            for (int i=0; i<8; i++)
            {
                _robotJointPositionsArranged.Add(new RobotJointPosition(9e9, 9e9, 9e9, 9e9, 9e9, 9e9));
            }

            RobotJointPosition jointPos;
            Transform[] transforms;

            for (int i = 0; i < NumSolutions; i++)
            {

                jointPos = _robotJointPositions[i];
                fk.Calculate(jointPos);
                transforms = fk.RobotTransforms;

                // Get wrist center point x position:
                double wcp_x = fk.RobotTransforms[5].M03;

                // Get lower arm x position:
                double lowerArm_x = fk.RobotTransforms[3].M03;

                // Get the angle of axis 5
                double axis_5 = jointPos[4];

                if (wcp_x >= 0.0 && wcp_x >= lowerArm_x && axis_5 >= 0.0)
                {
                    _robotJointPositionsArranged[0] = jointPos;
                }
                else if (wcp_x >= 0.0 && wcp_x >= lowerArm_x && axis_5 < 0.0)
                {
                    _robotJointPositionsArranged[1] = jointPos;
                }
                else if (wcp_x >= 0.0 && wcp_x < lowerArm_x && axis_5 >= 0.0)
                {
                    _robotJointPositionsArranged[2] = jointPos;
                }
                else if (wcp_x >= 0.0 && wcp_x < lowerArm_x && axis_5 < 0.0)
                {
                    _robotJointPositionsArranged[3] = jointPos;
                }
                else if (wcp_x < 0.0 && wcp_x >= lowerArm_x && axis_5 >= 0.0)
                {
                    _robotJointPositionsArranged[4] = jointPos;
                }
                else if (wcp_x < 0.0 && wcp_x >= lowerArm_x && axis_5 < 0.0)
                {
                    _robotJointPositionsArranged[5] = jointPos;
                }
                else if (wcp_x < 0.0 && wcp_x < lowerArm_x && axis_5 >= 0.0)
                {
                    _robotJointPositionsArranged[6] = jointPos;
                }
                else if (wcp_x < 0.0 && wcp_x < lowerArm_x && axis_5 < 0.0)
                {
                    _robotJointPositionsArranged[7] = jointPos;
                }
            }
        }

        /// <summary>
        /// Sort joint positions according to the ConfigurationComparer.
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
        /// Gets all calculated Robot Joint Positions.
        /// </summary>
        public List<RobotJointPosition> RobotJointPositions
        {
            get { return _robotJointPositionsArranged; }
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
        [DllImport("rcik_CRB15000_5_095.dll", EntryPoint = "computeInverseKinematics")]
        private static unsafe extern Vector6d* computeIK_CRB15000_5_095(ref IKFast.Geometry.Vector3d eePos, ref IKFast.Geometry.Quaternion eeOri, out int n_sol);

        [DllImport("rcik_CRB15000_10_152.dll", EntryPoint = "computeInverseKinematics")]
        private static unsafe extern Vector6d* computeIK_CRB15000_10_152(ref IKFast.Geometry.Vector3d eePos, ref IKFast.Geometry.Quaternion eeOri, out int n_sol);
        #endregion
    }
}
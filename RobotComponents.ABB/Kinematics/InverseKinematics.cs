// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
using System.Linq;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.ABB.Actions.Declarations;
using RobotComponents.ABB.Actions.Interfaces;
using RobotComponents.ABB.Actions.Instructions;
using RobotComponents.ABB.Definitions;

namespace RobotComponents.ABB.Kinematics
{
    /// <summary>
    /// Represent the Inverse Kinematics for a 6-axis spherical Robot and its attached external axes.
    /// </summary>
    public class InverseKinematics
    {
        #region fields
        private Robot _robot;
        private RobotTool _robotTool;
        private Movement _movement;
        private Plane _positionPlane; 
        private Plane _globalTargetPlane;
        private Plane _globalEndPlane;
        private Plane _localEndPlane;
        private bool _inLimits = true;
        private bool _elbowSingularity = false;
        private bool _wristSingularity = false;
        private readonly bool[] _elbowSingularities = Enumerable.Repeat(false, 8).ToArray();
        private readonly bool[] _wristSingularities = Enumerable.Repeat(false, 8).ToArray();
        private readonly List<string> _errorText = new List<string>();
        private readonly RobotJointPosition[] _robotJointPositions = new RobotJointPosition[8];
        private RobotJointPosition _robotJointPosition = new RobotJointPosition();
        private ExternalJointPosition _externalJointPosition = new ExternalJointPosition();

        // Constants
        private const double _pi = Math.PI;
        private const double _2pi = 2 * _pi;
        private const double _rad2deg = 180 / _pi;

        // OPW kinematics solver fields
        private readonly double[] _offsets = new double[6] { 0, 0, -_pi / 2, 0, 0, 0 };
        private readonly int[] _signs = new int[6] { 1, 1, 1, 1, 1, 1 };
        private readonly int[] _order = new int[8] { 0, 4, 1, 5, 2, 6, 3, 7 };
        private readonly double[] _theta1 = new double[8];
        private readonly double[] _theta2 = new double[8];
        private readonly double[] _theta3 = new double[8];
        private readonly double[] _theta4 = new double[8];
        private readonly double[] _theta5 = new double[8];
        private readonly double[] _theta6 = new double[8];
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the Inverse Kinematics class.
        /// </summary>
        public InverseKinematics()
        {
            for (int i = 0; i < 8; i++)
            {
                _robotJointPositions[i] = new RobotJointPosition();
            }
        }

        /// <summary>
        /// Initializes a new instance of the Inverse Kinematics class from a Movement.
        /// </summary>
        /// <param name="movement"> The Movement. </param>
        /// <param name="robot"> The Robot. </param>
        public InverseKinematics(Movement movement, Robot robot)
        {
            _robot = robot;
            _movement = movement;

            for (int i = 0; i < 8; i++)
            {
                _robotJointPositions[i] = new RobotJointPosition();
            }

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the Inverse Kinematics class from a Target.
        /// </summary>
        /// <remarks>
        /// The target will be casted to a robot movement with a default work object (wobj0).
        /// </remarks>
        /// <param name="target"> The Target </param>
        /// <param name="robot"> The Robot. </param>
        public InverseKinematics(ITarget target, Robot robot)
        {
            _robot = robot;
            _movement = new Movement(target);

            for (int i = 0; i < 8; i++)
            {
                _robotJointPositions[i] = new RobotJointPosition();
            }

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the Inverse Kinematics class by duplicating an existing Inverse Kinematics instance. 
        /// </summary>
        /// <param name="inverseKinematics"> The Inverse Kinematics instance to duplicate. </param>
        public InverseKinematics(InverseKinematics inverseKinematics)
        {
            _robot = inverseKinematics.Robot.Duplicate();
            _movement = inverseKinematics.Movement.Duplicate();

            for (int i = 0; i < 8; i++)
            {
                _robotJointPositions[i] = new RobotJointPosition();
            }

            Initialize();

            _robotJointPosition = inverseKinematics.RobotJointPosition.Duplicate();
            _externalJointPosition = inverseKinematics.ExternalJointPosition.Duplicate();

            _errorText = new List<string>(inverseKinematics.ErrorText);
            _inLimits = inverseKinematics.InLimits;
        }

        /// <summary>
        /// Returns an exact duplicate of this Inverse Kinematics instance.
        /// </summary>
        /// <returns> A deep copy of the Inverse Kinematics instance. </returns>
        public InverseKinematics Duplicate()
        {
            return new InverseKinematics(this);
        }
        #endregion

        #region methods
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns> A string that represents the current object. </returns>
        public override string ToString()
        {
            if (!IsValid)
            {
                return "Invalid Inverse Kinematics";
            }
            else
            {
                return "Inverse Kinematics";
            }
        }

        /// <summary>
        /// Initialize the fields to construct a valid Inverse Kinematics instance. 
        /// </summary>
        private void Initialize()
        {
            _robotTool = _movement.RobotTool.Name == "" ? _robot.Tool : _movement.RobotTool;

            if (_movement.Target is RobotTarget)
            {
                _globalTargetPlane = _movement.GetPosedGlobalTargetPlane();
                GetPositionPlane();
                GetEndPlanes();
            }
        }

        /// <summary>
        /// Reinitialize all the fields to construct a valid Inverse Kinematics object. 
        /// </summary>
        /// <remarks>
        /// This method also resets the solution. The method Calculate() has to be called to obtain a new solution. 
        /// </remarks>
        public void ReInitialize()
        {
            Initialize();
            ClearCurrentSolutions();
        }

        /// <summary>
        /// Calculates the inverse kinematics solution.
        /// </summary>
        public void Calculate()
        {
            ClearCurrentSolutions();
            CalculateRobotJointPosition();
            CalculateExternalJointPosition();
            CheckInternalAxisLimits();
            CheckExternalAxisLimits();
        }

        /// <summary>
        /// Calculates the Robot Joint Position of the Inverse Kinematics solution.
        /// </summary>
        /// <remarks>
        /// This method does not check the internal axis limits.
        /// </remarks>
        public void CalculateRobotJointPosition()
        {
            ResetRobotJointPositions();

            if (_movement.Target is RobotTarget robotTarget)
            {
                // Wrist position
                Point3d c = new Point3d(_localEndPlane.PointAt(0, 0, -_robot.C4));

                // Positioning part parameters: part 1
                double nx1 = Math.Sqrt(c.X * c.X + c.Y * c.Y - _robot.B * _robot.B) - _robot.A1;
                double k_2 = _robot.A2 * _robot.A2 + _robot.C3 * _robot.C3;
                double k = Math.Sqrt(k_2);

                // Joint position 1
                double theta1_i = Math.Atan2(c.Y, c.X) - Math.Atan2(_robot.B, nx1 + _robot.A1);
                double theta1_ii = Math.Atan2(c.Y, c.X) + Math.Atan2(_robot.B, nx1 + _robot.A1) - _pi;
                _theta1[0] = theta1_i;
                _theta1[1] = theta1_i;
                _theta1[2] = theta1_ii;
                _theta1[3] = theta1_ii;
                _theta1[4] = theta1_i;
                _theta1[5] = theta1_i;
                _theta1[6] = theta1_ii;
                _theta1[7] = theta1_ii;

                // Positioning part parameters: part 2
                double s1_2 = nx1 * nx1 + (c.Z - _robot.C1) * (c.Z - _robot.C1);
                double s2_2 = (nx1 + 2.0 * _robot.A1) * (nx1 + 2.0 * _robot.A1) + (c.Z - _robot.C1) * (c.Z - _robot.C1);
                double s1 = Math.Sqrt(s1_2);
                double s2 = Math.Sqrt(s2_2);

                // Joint position 2
                double acos1 = Math.Acos((s1_2 + _robot.C2 * _robot.C2 - k_2) / (2.0 * s1 * _robot.C2));
                double acos2 = Math.Acos((s2_2 + _robot.C2 * _robot.C2 - k_2) / (2.0 * s2 * _robot.C2));
                if (double.IsNaN(acos1)) { acos1 = 0; }
                if (double.IsNaN(acos2)) { acos2 = 0; }
                double theta2_i = -acos1 + Math.Atan2(nx1, c.Z - _robot.C1);
                double theta2_ii = acos1 + Math.Atan2(nx1, c.Z - _robot.C1);
                double theta2_iii = -acos2 - Math.Atan2(nx1 + 2.0 * _robot.A1, c.Z - _robot.C1);
                double theta2_iv = acos2 - Math.Atan2(nx1 + 2.0 * _robot.A1, c.Z - _robot.C1);
                _theta2[0] = theta2_i;
                _theta2[1] = theta2_ii;
                _theta2[2] = theta2_iii;
                _theta2[3] = theta2_iv;
                _theta2[4] = theta2_i;
                _theta2[5] = theta2_ii;
                _theta2[6] = theta2_iii;
                _theta2[7] = theta2_iv;

                // Joint position 3
                double acos3 = Math.Acos((s1_2 - _robot.C2 * _robot.C2 - k_2) / (2.0 * _robot.C2 * k));
                double acos4 = Math.Acos((s2_2 - _robot.C2 * _robot.C2 - k_2) / (2.0 * _robot.C2 * k));
                if (double.IsNaN(acos3)) { acos3 = 0; }
                if (double.IsNaN(acos4)) { acos4 = 0; }
                double theta3_i = acos3 - Math.Atan2(_robot.A2, _robot.C3);
                double theta3_ii = -acos3 - Math.Atan2(_robot.A2, _robot.C3);
                double theta3_iii = acos4 - Math.Atan2(_robot.A2, _robot.C3);
                double theta3_iv = -acos4 - Math.Atan2(_robot.A2, _robot.C3);
                _theta3[0] = theta3_i;
                _theta3[1] = theta3_ii;
                _theta3[2] = theta3_iii;
                _theta3[3] = theta3_iv;
                _theta3[4] = theta3_i;
                _theta3[5] = theta3_ii;
                _theta3[6] = theta3_iii;
                _theta3[7] = theta3_iv;

                // Orientation part parameters
                double e11 = _localEndPlane.XAxis.X;
                double e12 = _localEndPlane.YAxis.X;
                double e13 = _localEndPlane.ZAxis.X;
                double e21 = _localEndPlane.XAxis.Y;
                double e22 = _localEndPlane.YAxis.Y;
                double e23 = _localEndPlane.ZAxis.Y;
                double e31 = _localEndPlane.XAxis.Z;
                double e32 = _localEndPlane.YAxis.Z;
                double e33 = _localEndPlane.ZAxis.Z;

                // Calculate joint postion 4, 5 and 6
                for (int i = 0; i < 8; i++)
                {
                    double sin1 = Math.Sin(_theta1[i]);
                    double sin23 = Math.Sin(_theta2[i] + _theta3[i]);
                    double cos1 = Math.Cos(_theta1[i]);
                    double cos23 = Math.Cos(_theta2[i] + _theta3[i]);
                    double m = e13 * sin23 * cos1 + e23 * sin23 * sin1 + e33 * cos23;

                    // Joint 4
                    double theta4_p = Math.Atan2(e23 * cos1 - e13 * sin1, e13 * cos23 * cos1 + e23 * cos23 * sin1 - e33 * sin23);
                    _theta4[i] = i < 4 ? theta4_p : theta4_p + _pi;

                    // Joint 5
                    double theta5_p = Math.Atan2(Math.Sqrt(1 - m * m), m);
                    _theta5[i] = i < 4 ? theta5_p : -theta5_p;
                    _wristSingularities[i] = Math.Abs(_theta5[i]) < 1e-3;

                    // Joint 6
                    double theta6_p = Math.Atan2(e12 * sin23 * cos1 + e22 * sin23 * sin1 + e32 * cos23, -e11 * sin23 * cos1 - e21 * sin23 * sin1 - e31 * cos23);
                    _theta6[i] = i < 4 ? theta6_p : theta6_p - _pi;
                }

                // Elbow singularities
                if (acos1 == 0)
                {
                    _elbowSingularities[0] = true;
                    _elbowSingularities[1] = true;
                    _elbowSingularities[4] = true;
                    _elbowSingularities[5] = true;
                }
                else
                {
                    _elbowSingularities[0] = false;
                    _elbowSingularities[1] = false;
                    _elbowSingularities[4] = false;
                    _elbowSingularities[5] = false;
                }
                if (acos2 == 0)
                {
                    _elbowSingularities[2] = true;
                    _elbowSingularities[3] = true;
                    _elbowSingularities[6] = true;
                    _elbowSingularities[7] = true;
                }
                else
                {
                    _elbowSingularities[2] = false;
                    _elbowSingularities[3] = false;
                    _elbowSingularities[6] = false;
                    _elbowSingularities[7] = false;
                }

                // Check if the values are within -pi till pi
                for (int i = 0; i < 8; i++)
                {
                    _theta1[i] = _theta1[i] > _pi ? _theta1[i] - _2pi : _theta1[i];
                    _theta1[i] = _theta1[i] < -_pi ? _theta1[i] + _2pi : _theta1[i];
                    _theta2[i] = _theta2[i] > _pi ? _theta2[i] - _2pi : _theta2[i];
                    _theta2[i] = _theta2[i] < -_pi ? _theta2[i] + _2pi : _theta2[i];
                    _theta3[i] = _theta3[i] > _pi ? _theta3[i] - _2pi : _theta3[i];
                    _theta3[i] = _theta3[i] < -_pi ? _theta3[i] + _2pi : _theta3[i];
                    _theta4[i] = _theta4[i] > _pi ? _theta4[i] - _2pi : _theta4[i];
                    _theta4[i] = _theta4[i] < -_pi ? _theta4[i] + _2pi : _theta4[i];
                    _theta5[i] = _theta5[i] > _pi ? _theta5[i] - _2pi : _theta5[i];
                    _theta5[i] = _theta5[i] < -_pi ? _theta5[i] + _2pi : _theta5[i];
                    _theta6[i] = _theta6[i] > _pi ? _theta6[i] - _2pi : _theta6[i];
                    _theta6[i] = _theta6[i] < -_pi ? _theta6[i] + _2pi : _theta6[i];
                }

                // Set Robot Joint Positions
                for (int i = 0; i < 8; i++)
                {
                    _robotJointPositions[i][0] = Math.Sign(_signs[0]) * _rad2deg * (_theta1[_order[i]] + _offsets[0]);
                    _robotJointPositions[i][1] = Math.Sign(_signs[1]) * _rad2deg * (_theta2[_order[i]] + _offsets[1]);
                    _robotJointPositions[i][2] = Math.Sign(_signs[2]) * _rad2deg * (_theta3[_order[i]] + _offsets[2]);
                    _robotJointPositions[i][3] = Math.Sign(_signs[3]) * _rad2deg * (_theta4[_order[i]] + _offsets[3]);
                    _robotJointPositions[i][4] = Math.Sign(_signs[4]) * _rad2deg * (_theta5[_order[i]] + _offsets[4]);
                    _robotJointPositions[i][5] = Math.Sign(_signs[5]) * _rad2deg * (_theta6[_order[i]] + _offsets[5]);
                }

                // Select solution
                _robotJointPosition = _robotJointPositions[robotTarget.ConfigurationData.Cfx];
                _wristSingularity = _wristSingularities[_order[robotTarget.ConfigurationData.Cfx]];
                _elbowSingularity = _elbowSingularities[_order[robotTarget.ConfigurationData.Cfx]];
            }

            else if (_movement.Target is JointTarget jointTarget)
            {
                _robotJointPosition = jointTarget.RobotJointPosition;
            }

            else
            {
                _robotJointPosition = new RobotJointPosition();
            }
        }

        /// <summary>
        /// Calculates and returns the closest Robot Joint Position to a given previous Robot Joint Position.
        /// </summary>
        /// <remarks>
        /// This methods sets and returns the closest Robot Joint Poistion insides this Inverse Kinematics object. 
        /// You first have to calculate the Inverse Kinematics solution before you call this method. 
        /// This method is typically used for using Linear and Joint Configuration control inside the Path Generator.
        /// </remarks>
        /// <param name="prevJointPosition"> The previous Robot Joint Position. </param>
        /// <returns> The closest Robot Joint Position. </returns>
        public RobotJointPosition CalculateClosestRobotJointPosition(RobotJointPosition prevJointPosition)
        {
            RobotJointPosition diff;
            double sum;

            // First, check the selected axis configuration
            diff = _robotJointPosition - prevJointPosition;

            for (int i = 0; i < 6; i++)
            {
                diff[i] = Math.Sqrt(diff[i] * diff[i]);
            }

            double min = diff.Sum();
    
            _robotJointPosition = _robotJointPosition.Duplicate();

            // Check for flipping axis 4 and 6 (if this is within the axis limits)
            double[] joint4 = new double[3] { -360, 0, 360 };
            double[] joint6 = new double[3] { -360, 0, 360 };

            for (int i = 0; i < _robotJointPositions.Length; i++)
            {
                for (int j = 0; j < joint4.Length; j++)
                {
                    // Check axis 4
                    if (_robot.InternalAxisLimits[3].IncludesParameter(_robotJointPositions[i][3] + joint4[j], false) == true)
                    {
                        // Add value to axis 4
                        _robotJointPositions[i][3] += joint4[j];

                        for (int k = 0; k < joint6.Length; k++)
                        {
                            // Check axis 6
                            if (_robot.InternalAxisLimits[5].IncludesParameter(_robotJointPositions[i][5] + joint6[k], false) == true)
                            {
                                // Add value to axis 6
                                _robotJointPositions[i][5] += joint6[k];

                                // Check the configuration (min. rotation)
                                diff = _robotJointPositions[i] - prevJointPosition;

                                for (int l = 0; l < 6; l++)
                                {
                                    diff[l] = Math.Sqrt(diff[l] * diff[l]);
                                }

                                sum = diff.Sum();

                                if (sum < min)
                                {
                                    _robotJointPosition = _robotJointPositions[i].Duplicate();
                                    _wristSingularity = _wristSingularities[_order[i]];
                                    _elbowSingularity = _elbowSingularities[_order[i]];
                                    min = sum;
                                }

                                // Reset axis 6 (substract value from axis 6)
                                _robotJointPositions[i][5] -= joint6[k];
                            }
                        }

                        // Reset axis 4 (substract value from axis 4)
                        _robotJointPositions[i][3] -= joint4[j];
                    }
                }
            }

            // Check axis limits
            _errorText.Clear();
            CheckInternalAxisLimits();
            CheckExternalAxisLimits();

            return _robotJointPosition;
        }

        /// <summary>
        /// Calculates the External Joint Position of the Inverse Kinematics solution.
        /// </summary>
        /// <remarks>
        /// This method does not check the external axis limits. 
        /// </remarks>
        public void CalculateExternalJointPosition()
        {
            // Clear current solution
            ResetExternalJointPosition();

            if (_movement.Target is RobotTarget robotTarget)
            {
                // NOTE: Only works for a robot with one external axis that moves the robot.
                double count = 0; // Counts the number of external axes that move the robot.

                // Calculates the position of the external axes for each external axis
                for (int i = 0; i < _robot.ExternalAxes.Count; i++)
                {
                    ExternalAxis externalAxis = _robot.ExternalAxes[i];
                    Interval axisLimits = _robot.ExternalAxes[i].AxisLimits;
                    int logic = externalAxis.AxisNumber;

                    // External Linear axis
                    if (externalAxis is ExternalLinearAxis externalLinearAxis)
                    {
                        // External Linear Axis that moves the robot
                        if (externalLinearAxis.MovesRobot == true && count == 0)
                        {
                            // Checks if the position the external linear axis needs to be negative or positive
                            externalLinearAxis.AxisCurve.ClosestPoint(_robot.BasePlane.Origin, out double robotBasePlaneParam);
                            externalLinearAxis.AxisCurve.ClosestPoint(_positionPlane.Origin, out double basePlaneParam);

                            if (basePlaneParam >= robotBasePlaneParam)
                            {
                                _externalJointPosition[logic] = _positionPlane.Origin.DistanceTo(_robot.BasePlane.Origin);
                            }

                            else
                            {
                                _externalJointPosition[logic] = -_positionPlane.Origin.DistanceTo(_robot.BasePlane.Origin);
                            }

                            count += 1;
                        }

                        // External Linear axis that does not move the robot
                        else
                        {
                            if (robotTarget.ExternalJointPosition[logic] == 9e9)
                            {
                                _externalJointPosition[logic] = Math.Max(0, Math.Min(axisLimits.Min, axisLimits.Max));
                            }
                            else
                            {
                                _externalJointPosition[logic] = robotTarget.ExternalJointPosition[logic];
                            }
                        }
                    }

                    // External Rotational Axis
                    else if (externalAxis is ExternalRotationalAxis)
                    {
                        if (robotTarget.ExternalJointPosition[logic] == 9e9)
                        {
                            _externalJointPosition[logic] = Math.Max(0, Math.Min(axisLimits.Min, axisLimits.Max));
                        }
                        else
                        {
                            _externalJointPosition[logic] = robotTarget.ExternalJointPosition[logic];
                        }
                    }

                    // Other External Axis types
                    else
                    {
                        if (robotTarget.ExternalJointPosition[logic] == 9e9)
                        {
                            _externalJointPosition[logic] = Math.Max(0, Math.Min(axisLimits.Min, axisLimits.Max));
                        }
                        else
                        {
                            _externalJointPosition[logic] = robotTarget.ExternalJointPosition[logic];
                        }
                    }
                }

                // Copy name
                _externalJointPosition.Name = _movement.Target.ExternalJointPosition.Name;
            }

            // Joint Target
            else
            {
                _externalJointPosition = _movement.Target.ExternalJointPosition.Duplicate();
            }
        }

        /// <summary>
        /// Clears the lists with the current solutions.
        /// </summary>
        private void ClearCurrentSolutions()
        {
            ResetRobotJointPositions();
            ResetExternalJointPosition();

            _errorText.Clear();
            _inLimits = true;
            _wristSingularity = false;
            _elbowSingularity = false;

            for (int i = 0; i < 8; i++)
            {
                _elbowSingularities[i] = false;
            }

            for (int i = 0; i < 8; i++)
            {
                _wristSingularities[i] = false;
            }
        }

        /// <summary>
        /// Resets the Robot Joint Position solutions
        /// </summary>
        private void ResetRobotJointPositions()
        {
            for (int i = 0; i < 8; i++)
            {
                _robotJointPositions[i].Reset();
            }
        }

        /// <summary>
        /// Resest the the External Joint Position solution
        /// </summary>
        private void ResetExternalJointPosition()
        {
            _externalJointPosition.Reset();
        }

        /// <summary>
        /// Calculates the end planes of joint 6.
        /// </summary>
        private void GetEndPlanes()
        {
            _globalEndPlane = new Plane(_robotTool.AttachmentPlane);
            Transform trans1 = Transform.PlaneToPlane(_robotTool.ToolPlane, _globalTargetPlane);
            _globalEndPlane.Transform(trans1);

            _localEndPlane = new Plane(_globalEndPlane);
            Transform trans2 = Transform.PlaneToPlane(_positionPlane, Plane.WorldXY);
            _localEndPlane.Transform(trans2);
        }

        /// <summary>
        /// Calculates the base position of the robot in world coordinate space if it is moved by an external axis.
        /// </summary>
        private void GetPositionPlane()
        {
            // NOTE: Only works for a robot with an maximum of one external linear axis

            // Deep copy the current position / base plane
            _positionPlane = new Plane(_robot.BasePlane);

            // Check if an external axis is attached to the robot 
            for (int i = 0; i < _robot.ExternalAxes.Count; i++)
            {
                ExternalAxis externalAxis = _robot.ExternalAxes[i];
                int logic = externalAxis.AxisNumber;

                // Moves Robot?
                if (externalAxis.MovesRobot == true)
                {
                    // An External Linear Axis that moves the robot
                    if (externalAxis is ExternalLinearAxis externalLinearAxis)
                    {
                        if (_movement.Target.ExternalJointPosition[logic] == 9e9)
                        {
                            // Alternative strategy: take the global end plane

                            externalLinearAxis.AxisCurve.ClosestPoint(_globalTargetPlane.Origin, out double param);
                            _positionPlane.Origin = externalLinearAxis.AxisCurve.PointAt(param);
                        }

                        else
                        {
                            _positionPlane = externalLinearAxis.CalculatePosition(_movement.Target.ExternalJointPosition, out _);
                        }
                    }

                    // An External Rotational Axis that moves the robot
                    else if (externalAxis is ExternalRotationalAxis externalRotationalAxis)
                    {
                        _positionPlane = externalRotationalAxis.CalculatePosition(_movement.Target.ExternalJointPosition, out _);
                    }

                    // Other External Axis types that move the robot
                    else
                    {
                        _positionPlane = externalAxis.CalculatePosition(_movement.Target.ExternalJointPosition, out _);
                    }

                    // Break the loop since one axternal axis can move the robot.
                    break;
                }
            }
        }

        /// <summary>
        /// Checks if the positions of the robot axes are inside its limits.
        /// </summary>
        private void CheckInternalAxisLimits()
        {
            for (int i = 0; i < _robotJointPosition.Length; i++)
            {
                if (_robot.InternalAxisLimits[i].IncludesParameter(_robotJointPosition[i], false) == false)
                { 
                    _errorText.Add($"Movement {_movement.Target.Name}\\{_movement.WorkObject.Name}: The position of robot axis {i + 1} is not in range.");
                    _inLimits = false;
                }
            }

            if (_wristSingularity == true)
            {
                _errorText.Add($"Movement {_movement.Target.Name}\\{_movement.WorkObject.Name}: The target is close to wrist singularity.");
            }

            if (_elbowSingularity == true)
            {
                _errorText.Add($"Movement {_movement.Target.Name}\\{_movement.WorkObject.Name}: The target is out of reach (elbow singularity).");
                _inLimits = false;
            }
        }

        /// <summary>
        /// Checks if the positions of the external axes are inside its limits.
        /// </summary>
        private void CheckExternalAxisLimits()
        {
            for (int i = 0; i < _robot.ExternalAxes.Count; i++)
            {
                int number = _robot.ExternalAxes[i].AxisNumber;
                char logic = _robot.ExternalAxes[i].AxisLogic;

                if (_robot.ExternalAxes[i].AxisLimits.IncludesParameter(_externalJointPosition[number], false) == false)
                {
                    _errorText.Add($"Movement {_movement.Target.Name}\\{_movement.WorkObject.Name}: The position of external logical axis {logic} is not in range.");
                    _inLimits = false;
                }
            }
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets a value indicating whether or not the object is valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                if (_robot == null) { return false; }
                if (_robot.IsValid == false) { return false; }
                if (_movement == null) { return false; }
                if (_movement.IsValid == false) { return false; }
                return true;
            }
        }

        /// <summary>
        /// Gets or sets the Robot.
        /// </summary>
        public Robot Robot
        {
            get 
            { 
                return _robot; 
            }
            set 
            { 
                _robot = value;
                ReInitialize();
            }
        }

        /// <summary>
        /// Gets or sets the Movement.
        /// </summary>
        /// <remarks>
        /// The target and work object are obtained from this movement.
        /// </remarks>
        public Movement Movement
        {
            get 
            { 
                return _movement; 
            }
            set 
            { 
                _movement = value;
                ReInitialize();
            }
        }

        /// <summary>
        /// Gets the tool used by the this Inverse Kinematics.
        /// </summary>
        /// <remarks>
        /// By default the tool attached to the robot is used. 
        /// If a tool is set as a property of the movement, this tool will be used. 
        /// </remarks>
        public RobotTool RobotTool
        {
            get { return _robotTool; }
        }

        /// <summary>
        /// Gets the eight latest calculated Robot Joint Positions.
        /// </summary>
        public List<RobotJointPosition> RobotJointPositions
        {
            get { return _robotJointPositions.ToList(); }
        }

        /// <summary>
        /// Gets the latest calculated Robot Joint Position.
        /// </summary>
        public RobotJointPosition RobotJointPosition
        {
            get { return _robotJointPosition; }
        }

        /// <summary>
        /// Gets the latest calculated External Joint Position.
        /// </summary>
        public ExternalJointPosition ExternalJointPosition
        {
            get { return _externalJointPosition; }
        }

        /// <summary>
        /// Gets the collected error messages.
        /// </summary>
        public List<string> ErrorText
        {
            get { return _errorText; }
        }

        /// <summary>
        /// Gets a value indicating whether or not the internal and external values are within their limits.
        /// </summary>
        public bool InLimits
        {
            get { return _inLimits; }
        }
        #endregion
    }
}
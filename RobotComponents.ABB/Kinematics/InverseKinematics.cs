// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components
// Project: https://github.com/RobotComponents/RobotComponents
//
// Copyright (c) 2018-2020 EDEK Uni Kassel
// Copyright (c) 2020-2024 Arjen Deetman
//
// Authors:
//   - Gabriel Rumph (2018-2020)
//   - Benedikt Wannemacher (2018-2020)
//   - Arjen Deetman (2019-2024)
//
// For license details, see the LICENSE file in the project root.

// System Libs
using System;
using System.Collections.Generic;
using System.Linq;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.Generic.Kinematics;
using RobotComponents.ABB.Actions.Declarations;
using RobotComponents.ABB.Actions.Instructions;
using RobotComponents.ABB.Definitions;

namespace RobotComponents.ABB.Kinematics
{
    /// <summary>
    /// Represent the Inverse Kinematics for a 6-axis spherical Robot and its attached external axes.
    /// </summary>
    /// <remarks>
    /// Solution order:
    /// 
    /// Sol.    Wrist center            Wrist center            Axis 5 angle
    /// Cfx     relative to axis 1      relative to lower arm
    ///         
    /// 0       In front of             In front of             Positive
    /// 1       In front of             In front of             Negative
    /// 2       In front of             Behind                  Positive
    /// 3       In front of             Behind                  Negative     
    /// 4       Behind                  In front of             Positive
    /// 5       Behind                  In front of             Negative
    /// 6       Behind                  Behind                  Positive
    /// 7       Behind                  Behind                  Negative
    /// </remarks>
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
        private bool _isInLimits = true;
        private bool _shoulderSingularity = false;
        private bool _elbowSingularity = false;
        private bool _wristSingularity = false;
        private bool[] _shoulderSingularities = Enumerable.Repeat(false, 8).ToArray();
        private bool[] _elbowSingularities = Enumerable.Repeat(false, 8).ToArray();
        private bool[] _wristSingularities = Enumerable.Repeat(false, 8).ToArray();
        private readonly List<string> _errorText = new List<string>();
        private readonly RobotJointPosition[] _robotJointPositions = new RobotJointPosition[8].Select(item => new RobotJointPosition()).ToArray();
        private RobotJointPosition _robotJointPosition = new RobotJointPosition();
        private ExternalJointPosition _externalJointPosition = new ExternalJointPosition();
        private ConfigurationData _configurationData = new ConfigurationData();

        // Constants
        private const double _pi = Math.PI;
        private const double _rad2deg = 180 / _pi;

        // Kinematics solver fields
        private static readonly double[] _offsets = new double[6] { 0, 0, -_pi / 2, 0, 0, 0 };
        private static readonly int[] _order = new int[8] { 0, 4, 1, 5, 2, 6, 3, 7 };
        private static readonly int[] _signs = new int[6] { 1, 1, 1, 1, 1, 1 };
        private readonly OPWKinematics _opw = new OPWKinematics();
        private readonly OPWKinematics _wok = new OPWKinematics();
        private int _selectedSolution = -1;
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the Inverse Kinematics class.
        /// </summary>
        public InverseKinematics()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Inverse Kinematics class.
        /// </summary>
        /// <param name="robot"> The Robot. </param>
        public InverseKinematics(Robot robot)
        {
            _robot = robot;
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the Inverse Kinematics class by duplicating an existing Inverse Kinematics instance. 
        /// </summary>
        /// <param name="inverseKinematics"> The Inverse Kinematics instance to duplicate. </param>
        public InverseKinematics(InverseKinematics inverseKinematics)
        {
            _robot = inverseKinematics.Robot.Duplicate();
            _robotJointPositions = inverseKinematics.RobotJointPositions.Select(item => item.Duplicate()).ToArray();
            _robotJointPosition = inverseKinematics.RobotJointPosition.Duplicate();
            _externalJointPosition = inverseKinematics.ExternalJointPosition.Duplicate();
            _configurationData = inverseKinematics.ConfigurationData.Duplicate();
            _errorText = new List<string>(inverseKinematics.ErrorText);
            _isInLimits = inverseKinematics.IsInLimits;
            Initialize();
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
            _opw.Signs = _signs;
            _opw.Offsets = _offsets;
            _opw.A1 = _robot.RobotKinematicParameters.A1;
            _opw.A2 = _robot.RobotKinematicParameters.A2;
            _opw.B = _robot.RobotKinematicParameters.B;
            _opw.C1 = _robot.RobotKinematicParameters.C1;
            _opw.C2 = _robot.RobotKinematicParameters.C2;
            _opw.C3 = _robot.RobotKinematicParameters.C3;
            _opw.C4 = _robot.RobotKinematicParameters.C4;

            _wok.Signs = _signs;
            _wok.Offsets = _offsets;
            _wok.A1 = _robot.RobotKinematicParameters.A1;
            _wok.A2 = _robot.RobotKinematicParameters.A2;
            //_wok.A3 = _robot.RobotKinematicParameters.A3;
            _wok.B = _robot.RobotKinematicParameters.B;
            _wok.C1 = _robot.RobotKinematicParameters.C1;
            _wok.C2 = _robot.RobotKinematicParameters.C2;
            _wok.C3 = _robot.RobotKinematicParameters.C3;
            _wok.C4 = _robot.RobotKinematicParameters.C4;
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
        /// <param name="movement"> The movement to calculate the solution for. </param>
        public void Calculate(Movement movement)
        {
            ClearCurrentSolutions();
            SetMovement(movement);
            CalculateRobotJointPosition();
            CalculateExternalJointPosition();
            CheckInternalAxisLimits();
            CheckExternalAxisLimits();
        }

        /// <summary>
        /// Sets the movement and associated fields.
        /// </summary>
        /// <param name="movement"> The movement to be set. </param>
        private void SetMovement(Movement movement)
        {
            _movement = movement;
            _globalTargetPlane = _movement.GetPosedGlobalTargetPlane();
            _robotTool = _movement.RobotTool.Name == "" ? _robot.Tool : _movement.RobotTool;

            if (_movement.Target is RobotTarget)
            {
                GetPositionPlane();
                GetEndPlanes();
            }
        }

        /// <summary>
        /// Calculates the Robot Joint Position of the Inverse Kinematics solution.
        /// </summary>
        /// <remarks>
        /// This method does not check the internal axis limits.
        /// </remarks>
        private void CalculateRobotJointPosition()
        {
            ResetRobotJointPositions();

            if (_movement.Target is RobotTarget robotTarget)
            {
                // OPW kinematics solver
                if (_robot.RobotKinematicParameters.A3 == 0)
                {
                    // Calculate inverse kinematics
                    _opw.Inverse(_localEndPlane);

                    // Set Robot Joint Positions
                    for (int i = 0; i < 8; i++)
                    {
                        for (int j = 0; j < 6; j++)
                        {
                            _robotJointPositions[i][j] = _rad2deg * _opw.Solutions[_order[i]][j];
                        }
                    }

                    // Select solution
                    _robotJointPosition = _robotJointPositions[robotTarget.ConfigurationData.Cfx].Duplicate();

                    _wristSingularities = _order.Select(index => _opw.IsWristSingularity[index]).ToArray();
                    _elbowSingularities = _order.Select(index => _opw.IsElbowSingularity[index]).ToArray();
                    _shoulderSingularities = _order.Select(index => _opw.IsShoulderSingularity[index]).ToArray();

                    _wristSingularity = _wristSingularities[robotTarget.ConfigurationData.Cfx];
                    _elbowSingularity = _elbowSingularities[robotTarget.ConfigurationData.Cfx];
                    _shoulderSingularity = _shoulderSingularities[robotTarget.ConfigurationData.Cfx];
                }

                // Wrist Offset kinematics solver
                else
                {
                    // Calculate inverse kinematics
                    _wok.Inverse(_localEndPlane);

                    // Set Robot Joint Positions
                    for (int i = 0; i < 8; i++)
                    {
                        for (int j = 0; j < 6; j++)
                        {
                            _robotJointPositions[i][j] = _rad2deg * _wok.Solutions[_order[i]][j];
                        }
                    }

                    // Select solution
                    _robotJointPosition = _robotJointPositions[robotTarget.ConfigurationData.Cfx].Duplicate();

                    _wristSingularities = _order.Select(index => _wok.IsWristSingularity[index]).ToArray();
                    _elbowSingularities = _order.Select(index => _wok.IsElbowSingularity[index]).ToArray();
                    _shoulderSingularities = _order.Select(index => _wok.IsShoulderSingularity[index]).ToArray();

                    _wristSingularity = _wristSingularities[robotTarget.ConfigurationData.Cfx];
                    _elbowSingularity = _elbowSingularities[robotTarget.ConfigurationData.Cfx];
                    _shoulderSingularity = _shoulderSingularities[robotTarget.ConfigurationData.Cfx];
                }

                // Check configuration data cf1, cf4 and cf6
                AdjustJoint(0, robotTarget.ConfigurationData.Cf1);
                AdjustJoint(3, robotTarget.ConfigurationData.Cf4);
                AdjustJoint(5, robotTarget.ConfigurationData.Cf6);

                // Set configuration data
                _selectedSolution = robotTarget.ConfigurationData.Cfx;
                SetConfigurationData(robotTarget.ConfigurationData.Name);
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
        /// Sets the configuation data for the latest set Robot Joint Position.
        /// </summary>
        /// /// <param name="variableName"> Optional variable name of the configuration data. </param>
        private void SetConfigurationData(string variableName = "")
        {
            int cf1 = (int)Math.Floor(_robotJointPosition[0] / 90);
            int cf4 = (int)Math.Floor(_robotJointPosition[3] / 90);
            int cf6 = (int)Math.Floor(_robotJointPosition[5] / 90);
            int cfx = _selectedSolution;

            _configurationData = new ConfigurationData(variableName, cf1, cf4, cf6, cfx);
        }

        /// <summary>
        /// Adjusts the position of a robot joint to align with the target configuration quadrant.
        /// </summary>
        /// <param name="jointPositionIndex">
        /// The index of the joint position in the _robotJointPosition array. 
        /// Represents which joint's position to adjust (e.g., 0 for Joint 1, 3 for Joint 4, etc.).
        /// </param>
        /// <param name="targetCf"> 
        /// The target configuration quadrant for the joint. 
        /// This is a value indicating the desired 90-degree quadrant (e.g., 0, 1, 2, or 3).
        /// /// </param>
        /// <remarks>
        /// This method calculates the current configuration quadrant of the specified joint based on its 
        /// position in degrees, determines the difference between the current and target configuration, 
        /// and applies the minimal number of full 360-degree rotations to align the joint with the target 
        /// configuration. The adjustment is cyclic, using modulo arithmetic to account for the repeating 
        /// nature of quadrants.
        /// </remarks>
        private void AdjustJoint(int jointPositionIndex, int targetCf)
        {
            int cf = (int)Math.Floor(_robotJointPosition[jointPositionIndex] / 90);
            double diff = targetCf - cf;

            if (targetCf != cf && diff % 4 == 0)
            {
                _robotJointPosition[jointPositionIndex] += diff / 4 * 360;
            }
            else if ((_robotJointPosition[jointPositionIndex] / 90) % 1 == 0)
            {
                if (targetCf != (cf + 1) && (diff + 1) % 4 == 0)
                {
                    _robotJointPosition[jointPositionIndex] += (diff + 1) / 4 * 360;
                }
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
        /// <param name="includeJoint1"> indicating whether or not joint 1 is included for corrections of +/- 360 degree rotations. </param>
        /// <param name="includeJoint4"> indicating whether or not joint 4 is included for corrections of +/- 360 degree rotations. </param>
        /// <param name="includeJoint6"> indicating whether or not joint 6 is included for corrections of +/- 360 degree rotations. </param>
        /// <returns> The closest Robot Joint Position. </returns>
        public RobotJointPosition CalculateClosestRobotJointPosition(RobotJointPosition prevJointPosition, bool includeJoint1 = true, bool includeJoint4 = true, bool includeJoint6 = true)
        {
            // First, check the current position
            double norm = prevJointPosition.Norm(_robotJointPosition);
            double min = norm;

            for (int i = 0; i < _robotJointPositions.Length; i++)
            {
                GetSmallestAngleDifference(prevJointPosition[0], _robotJointPositions[i][0], out int fullRotations1);
                GetSmallestAngleDifference(prevJointPosition[3], _robotJointPositions[i][3], out int fullRotations4);
                GetSmallestAngleDifference(prevJointPosition[5], _robotJointPositions[i][5], out int fullRotations6);

                _robotJointPositions[i][0] += includeJoint1 ? fullRotations1 * 360 : 0;
                _robotJointPositions[i][3] += includeJoint4 ? fullRotations4 * 360 : 0;
                _robotJointPositions[i][5] += includeJoint6 ? fullRotations6 * 360 : 0;

                // Norm 
                norm = prevJointPosition.Norm(_robotJointPositions[i]);

                if (norm < min)
                {
                    _robotJointPosition = _robotJointPositions[i].Duplicate();
                    _shoulderSingularity = _shoulderSingularities[i];
                    _wristSingularity = _wristSingularities[i];
                    _elbowSingularity = _elbowSingularities[i];
                    _selectedSolution = i;
                    min = norm;
                }

                _robotJointPositions[i][0] -= includeJoint1 ? fullRotations1 * 360 : 0;
                _robotJointPositions[i][3] -= includeJoint4 ? fullRotations4 * 360 : 0;
                _robotJointPositions[i][5] -= includeJoint6 ? fullRotations6 * 360 : 0;
            }

            // Check axis limits
            _errorText.Clear();
            CheckInternalAxisLimits();
            CheckExternalAxisLimits();
            SetConfigurationData(_configurationData.Name);

            return _robotJointPosition;
        }

        /// <summary>
        /// Calculates the smallest angular difference between two angles, accounting for full rotations.
        /// </summary>
        /// <param name="prev"> The previous angle in degrees. </param>
        /// <param name="current"> The current angle in degrees. </param>
        /// <param name="fullRotations"> The number of full rotations needed to get to the smallest angle difference. </param>
        /// <returns> The smallest angular difference in degrees. </returns>
        private double GetSmallestAngleDifference(double prev, double current, out int fullRotations)
        {
            double diff = prev - current;
            fullRotations = (int)Math.Round((diff) / 360);
            diff -= fullRotations * 360;

            return diff;
        }

        /// <summary>
        /// Calculates the External Joint Position for a given movement.
        /// </summary>
        /// <param name="movement"> The movement to calculate the external joint position for. </param>
        /// <remarks>
        /// Interally used in situation where only the external joint positions needs to be 
        /// computed and not the robot joint position. 
        /// </remarks>
        internal void CalculateExternalJointPosition(Movement movement)
        {
            SetMovement(movement);
            CalculateExternalJointPosition();
        }

        /// <summary>
        /// Calculates the External Joint Position of the Inverse Kinematics solution.
        /// </summary>
        /// <remarks>
        /// This method does not check the external axis limits. 
        /// </remarks>
        private void CalculateExternalJointPosition()
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
                    IExternalAxis externalAxis = _robot.ExternalAxes[i];
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
            _isInLimits = true;
            _wristSingularity = false;
            _elbowSingularity = false;
            _shoulderSingularity = false;
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
                IExternalAxis externalAxis = _robot.ExternalAxes[i];
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
                    _errorText.Add($"Movement {_movement.Target.Name}\\{_movement.WorkObject.Name}: The position of robot joint {i + 1} is not in range.");
                    _isInLimits = false;
                }
            }

            if (_wristSingularity == true)
            {
                _errorText.Add($"Movement {_movement.Target.Name}\\{_movement.WorkObject.Name}: The robot is near a wrist singularity.");
            }

            if (_elbowSingularity == true)
            {
                _errorText.Add($"Movement {_movement.Target.Name}\\{_movement.WorkObject.Name}: The target is out of reach (elbow singularity).");
                _isInLimits = false;
            }

            if (_shoulderSingularity == true)
            {
                _errorText.Add($"Movement {_movement.Target.Name}\\{_movement.WorkObject.Name}: The robot is near a shoulder singularity.");
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
                    _isInLimits = false;
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
            get { return _robot; }
            set { _robot = value; ReInitialize(); }
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
        /// Gets the configuration data of the latest calculated Robot Joint Position.
        /// </summary>
        public ConfigurationData ConfigurationData
        {
            get { return _configurationData; }
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
        public bool IsInLimits
        {
            get { return _isInLimits; }
        }
        #endregion
    }
}
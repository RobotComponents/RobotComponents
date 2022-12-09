// This file is part of RobotComponents. RobotComponents is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
using System.Linq;
// Rhino Libs
using Rhino.Geometry;
using Rhino.Geometry.Intersect;
// RobotComponents Libs
using RobotComponents.Actions;
using RobotComponents.Definitions;

namespace RobotComponents.Kinematics
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
        private ITarget _target;
        private Plane _positionPlane; // The real position of the robot if an external axis is used in world coorindate space    
        private Plane _targetPlane;
        private Plane _endPlane;
        private List<Plane> _axisPlanes;
        private Point3d _wrist;
        private double _wristOffset;
        private double _lowerArmLength;
        private double _upperArmLength;
        private double _axis4offsetAngle;
        private readonly List<string> _errorText = new List<string>(); // Error text
        private bool _inLimits = true; // Indicates if the joint positions are in limits 
        private readonly RobotJointPosition[] _robotJointPositions = new RobotJointPosition[8]; // Contains all the eight solutions
        private RobotJointPosition _robotJointPosition = new RobotJointPosition(); // Contains the final solution
        private ExternalJointPosition _externalJointPosition = new ExternalJointPosition(); // Contains the final solution
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
        /// The target will be casted to robot movement with a default work object (wobj0). 
        /// </summary>
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
            // Check robot tool: override if the movement contains a robot tool
            if (_movement.RobotTool == null || _movement.RobotTool.Name == null || _movement.RobotTool.Name == "")
            {
                _robotTool = _robot.Tool;
            }
            else
            {
                _robotTool = _movement.RobotTool;
            }

            // Movement related fields
            _target = _movement.Target;

            if (_target is RobotTarget)
            {
                // Calculate the position and the orientation of the target plane in the world coordinate system
                // If there is an external axes connected to work object of the movement the 
                // target plane will be re-oriented according to the pose of the this external axes. 
                _targetPlane = _movement.GetPosedGlobalTargetPlane();

                // Update the base plane / position plane
                _positionPlane = GetPositionPlane();
                Transform trans = Transform.PlaneToPlane(_positionPlane, Plane.WorldXY);

                // Needed for transformation from the robot world coordinate system to the local robot coordinate system
                Transform orient = Transform.PlaneToPlane(_robot.BasePlane, Plane.WorldXY);

                // Orient the target plane to the robot coordinate system 
                _targetPlane = TransformToolPlane(_targetPlane, _robotTool.AttachmentPlane, _robotTool.ToolPlane);
                _endPlane = new Plane(_targetPlane.Origin, _targetPlane.YAxis, _targetPlane.XAxis); //rotates, flips plane for TCP Offset moving in the right direction
                _endPlane.Transform(trans);

                // Deep copy and orient to internal axis planes of the robot. 
                _axisPlanes = new List<Plane>();
                for (int i = 0; i < _robot.InternalAxisPlanes.Count; i++)
                {
                    Plane plane = new Plane(_robot.InternalAxisPlanes[i]);
                    plane.Transform(orient);
                    _axisPlanes.Add(plane);
                }

                // Other robot info related fields
                _wristOffset = _axisPlanes[5].Origin.X - _axisPlanes[4].Origin.X;
                _lowerArmLength = _axisPlanes[1].Origin.DistanceTo(_axisPlanes[2].Origin);
                _upperArmLength = _axisPlanes[2].Origin.DistanceTo(_axisPlanes[4].Origin);
                _axis4offsetAngle = Math.Atan2(_axisPlanes[4].Origin.Z - _axisPlanes[2].Origin.Z, _axisPlanes[4].Origin.X - _axisPlanes[2].Origin.X);
                _wrist = new Point3d(_endPlane.PointAt(0, 0, _wristOffset));
            }
        }

        /// <summary>
        /// Reinitialize all the fields to construct a valid Inverse Kinematics object. 
        /// This method also resets the solution. Calculate() has to be called to obtain a new solution. 
        /// </summary>
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
        /// This method does not check the internal axis limits. 
        /// </summary>
        public void CalculateRobotJointPosition()
        {
            // Clear the current solutions before calculating a new ones. 
            ResetRobotJointPositions();

            // Tracks the index of the axis configuration
            int index1 = 0;
            int index2 = 0;

            if (_target is RobotTarget robotTarget)
            { 
                #region wrist center relative to axis 1
                // Note that this is reversed because the clockwise direction when looking 
                // down at the XY plane is typically taken as the positive direction for robot axis1
                // Caculate the position of robot axis 1: Wrist center relative to axis 1 in front of robot (configuration 0, 1, 2, 3)
                double internalAxisValue1 = -1 * Math.Atan2(_wrist.Y, _wrist.X);
                if (internalAxisValue1 > Math.PI) { internalAxisValue1 -= 2 * Math.PI; }
                _robotJointPositions[0][0] = internalAxisValue1;
                _robotJointPositions[1][0] = internalAxisValue1;
                _robotJointPositions[2][0] = internalAxisValue1;
                _robotJointPositions[3][0] = internalAxisValue1;

                // Rotate the joint position 180 degrees (pi radians): Wrist center relative to axis 1 behind robot (configuration 4, 5, 6, 7)
                internalAxisValue1 += Math.PI;
                if (internalAxisValue1 > Math.PI) { internalAxisValue1 -= 2 * Math.PI; }
                _robotJointPositions[4][0] = internalAxisValue1;
                _robotJointPositions[5][0] = internalAxisValue1;
                _robotJointPositions[6][0] = internalAxisValue1;
                _robotJointPositions[7][0] = internalAxisValue1;
                #endregion

                // Generates 4 sets of values for each option of axis 1
                // i = 0: Wrist center relative to axis 1 in front of robot (configuration 0, 1, 2, 3)
                // i = 1: Wrist center relative to axis 1 behind robot (configuration 4, 5, 6, 7)
                for (int i = 0; i < 2; i++)
                {
                    // Get the position of robot axis 1
                    internalAxisValue1 = _robotJointPositions[i * 4][0];

                    // Get the elbow points
                    Point3d internalAxisPoint1 = new Point3d(_axisPlanes[1].Origin);
                    Point3d internalAxisPoint2 = new Point3d(_axisPlanes[2].Origin); 
                    Point3d internalAxisPoint4 = new Point3d(_axisPlanes[4].Origin);

                    // Rotate the points to the correction position
                    Transform rot1 = Transform.Rotation(-1 * internalAxisValue1, Point3d.Origin);
                    internalAxisPoint1.Transform(rot1);
                    internalAxisPoint2.Transform(rot1);
                    internalAxisPoint4.Transform(rot1);

                    // Create the elbow projection plane
                    Vector3d elbowDir = new Vector3d(1, 0, 0);
                    elbowDir.Transform(rot1);
                    Plane elbowPlane = new Plane(internalAxisPoint1, elbowDir, Vector3d.ZAxis);

                    Sphere sphere1 = new Sphere(internalAxisPoint1, _lowerArmLength);
                    Sphere sphere2 = new Sphere(_wrist, _upperArmLength);

                    Intersection.SphereSphere(sphere1, sphere2, out Circle circ);
                    Intersection.PlaneCircle(elbowPlane, circ, out double par1, out double par2);

                    Point3d intersectPt1 = circ.PointAt(par1);
                    Point3d intersectPt2 = circ.PointAt(par2);

                    // Calculates the position of robot axis 2 and 3
                    for (int j = 0; j < 2; j++)
                    {
                        // Calculate elbow and wrist variables
                        Point3d elbowPoint;
                        if (j == 1) { elbowPoint = intersectPt1; }
                        else { elbowPoint = intersectPt2; }
                        elbowPlane.ClosestParameter(elbowPoint, out double elbowX, out double elbowY);
                        elbowPlane.ClosestParameter(_wrist, out double wristX, out double wristY);

                        // Calculate the position of robot axis 2
                        double internalAxisValue2 = Math.Atan2(elbowY, elbowX); 
                        double internalAxisValue3 = Math.PI - internalAxisValue2 + Math.Atan2(wristY - elbowY, wristX - elbowX) - _axis4offsetAngle;

                        for (int k = 0; k < 2; k++)
                        {
                            // Adds the position of robot axis 2
                            _robotJointPositions[index1][1] = -internalAxisValue2;

                            // Calculate the position of robot axis 3
                            double internalAxisValue3Wrapped = -internalAxisValue3 + Math.PI;
                            while (internalAxisValue3Wrapped >= Math.PI) { internalAxisValue3Wrapped -= 2 * Math.PI; }
                            while (internalAxisValue3Wrapped < -Math.PI) { internalAxisValue3Wrapped += 2 * Math.PI; }
                            _robotJointPositions[index1][2] = internalAxisValue3Wrapped;

                            // Update in index tracker
                            index1++;
                        }

                        for (int k = 0; k < 2; k++)
                        {
                            // Calculate the position of robot axis 4
                            Vector3d axis4 = new Vector3d(_wrist - elbowPoint);
                            axis4.Rotate(-_axis4offsetAngle, elbowPlane.ZAxis);
                            Plane tempPlane = new Plane(elbowPlane);
                            tempPlane.Rotate(internalAxisValue2 + internalAxisValue3, tempPlane.ZAxis);
                            Plane internalAxisPlane4 = new Plane(_wrist, tempPlane.ZAxis, -1.0 * tempPlane.YAxis);
                            internalAxisPlane4.ClosestParameter(_endPlane.Origin, out double axis6X, out double axis6Y);
                            double internalAxisValue4 = Math.Atan2(axis6Y, axis6X);
                            if (k == 1)
                            {
                                internalAxisValue4 += Math.PI;
                                if (internalAxisValue4 > Math.PI) { internalAxisValue4 -= 2 * Math.PI; }
                            }
                            double internalAxisValue4Wrapped = internalAxisValue4 + Math.PI / 2;
                            while (internalAxisValue4Wrapped >= Math.PI) { internalAxisValue4Wrapped -= 2 * Math.PI; }
                            while (internalAxisValue4Wrapped < -Math.PI) { internalAxisValue4Wrapped += 2 * Math.PI; }
                            _robotJointPositions[index2][3] = internalAxisValue4Wrapped;

                            // Calculate the position of robot axis 5
                            Plane internalAxisPlane5 = new Plane(internalAxisPlane4);
                            internalAxisPlane5.Rotate(internalAxisValue4, internalAxisPlane4.ZAxis);
                            internalAxisPlane5 = new Plane(_wrist, -internalAxisPlane5.ZAxis, internalAxisPlane5.XAxis);
                            internalAxisPlane5.ClosestParameter(_endPlane.Origin, out axis6X, out axis6Y);
                            double internalAxisValue5 = Math.Atan2(axis6Y, axis6X);
                            _robotJointPositions[index2][4] = internalAxisValue5;

                            // Calculate the position of robot axis 6
                            Plane internalAxisPlane6 = new Plane(internalAxisPlane5);
                            internalAxisPlane6.Rotate(internalAxisValue5, internalAxisPlane5.ZAxis);
                            internalAxisPlane6 = new Plane(_wrist, -internalAxisPlane6.YAxis, internalAxisPlane6.ZAxis);
                            internalAxisPlane6.ClosestParameter(_endPlane.PointAt(0, -1), out double endX, out double endY);
                            double internalAxisValue6 = Math.Atan2(endY, endX);
                            _robotJointPositions[index2][5] = internalAxisValue6;

                            // Update index tracker
                            index2++;
                        }
                    }
                }

                // Corrections
                for (int i = 0; i < 8; i++)
                {
                    // From radians to degrees
                    _robotJointPositions[i] *= (180 / Math.PI);

                    // Other corrections
                    _robotJointPositions[i][0] *= -1;
                    _robotJointPositions[i][1] += 90;
                    _robotJointPositions[i][2] -= 90;
                    _robotJointPositions[i][3] *= -1;
                    if (_robotJointPositions[i][5] <= 0)
                    {
                        _robotJointPositions[i][5] = -180 - _robotJointPositions[i][5];
                    }
                    else
                    {
                        _robotJointPositions[i][5] = 180 - _robotJointPositions[i][5];
                    }
                }

                // Select solution
                _robotJointPosition = _robotJointPositions[robotTarget.AxisConfig];
            }

            else if (_target is JointTarget jointTarget)
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
        /// This methods sets and returns the closest Robot Joint Poistion insides this Inverse Kinematics object. 
        /// You first have to calculate the Inverse Kinematics solution before you call this method. 
        /// This method is typically used for using the Auto Axis Config inside the Path Generator.
        /// </summary>
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
        /// This method does not check the external axis limits. 
        /// </summary>
        public void CalculateExternalJointPosition()
        {
            // Clear current solution
            ResetExternalJointPosition();

            if (_target is RobotTarget robotTarget)
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
                _externalJointPosition.Name = _target.ExternalJointPosition.Name;
            }

            // Joint Target
            else
            {
                _externalJointPosition = _target.ExternalJointPosition.Duplicate();
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
        }

        /// <summary>
        /// Resets the Robot Joint Position solutions
        /// </summary>
        private void ResetRobotJointPositions()
        {
            for (int i = 0; i < _robotJointPositions.Length; i++)
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
        /// Returns the transformed tool plane. 
        /// </summary>
        /// <param name="targetPlane"> The global target plane defined in the world coordinate space. </param>
        /// <param name="attachmentPlane"> The attachement plane of the robot defined in the robot coordinate space. </param>
        /// <param name="toolPlane"> The TCP plane. </param>
        /// <returns> The transfomed tool plane. </returns>
        private Plane TransformToolPlane(Plane targetPlane, Plane attachmentPlane, Plane toolPlane)
        {
            Plane result = new Plane(attachmentPlane);
            result.Rotate(Math.PI, attachmentPlane.ZAxis); // Flips the plane
            Transform trans = Transform.PlaneToPlane(toolPlane, targetPlane);
            result.Transform(trans);
            return result;
        }

        /// <summary>
        /// Returns the base position of the robot in world coordinate space if it is moved by an external axis.
        /// </summary>
        /// <returns> The position of the robot as a plane. </returns>
        private Plane GetPositionPlane()
        {
            // NOTE: Only works for a robot info with an maximum of one external linear axis

            // Deep copy the current position / base plane
            Plane plane = new Plane(_robot.BasePlane);

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
                        if (_target.ExternalJointPosition[logic] == 9e9)
                        {
                            externalLinearAxis.AxisCurve.ClosestPoint(_targetPlane.Origin, out double param);
                            plane.Origin = externalLinearAxis.AxisCurve.PointAt(param);
                        }

                        else
                        {
                            plane = externalLinearAxis.CalculatePosition(_target.ExternalJointPosition, out _);
                        }
                    }

                    // An External Rotational Axis that moves the robot
                    else if (externalAxis is ExternalRotationalAxis externalRotationalAxis)
                    {
                        plane = externalRotationalAxis.CalculatePosition(_target.ExternalJointPosition, out _);
                    }

                    // Other External Axis types that move the robot
                    else
                    {
                        plane = externalAxis.CalculatePosition(_target.ExternalJointPosition, out _);
                    }

                    // Break the loop since one axternal axis can move the robot.
                    break;
                }
            }

            // Returns the position plane of the robot
            return plane;
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
                    _errorText.Add($"Movement {Movement.Target.Name}\\{Movement.WorkObject.Name}: The position of robot axis {i + 1} is not in range.");
                    _inLimits = false;
                }
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
                    _errorText.Add($"Movement {Movement.Target.Name}\\{Movement.WorkObject.Name}: The position of external logical axis {logic} is not in range.");
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
        /// The target and work object are obtained from this movement.
        /// </summary>
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
        /// By default the tool attached to the robot is used. 
        /// If a tool is set as a property of the movement, this tool will be used. 
        /// </summary>
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
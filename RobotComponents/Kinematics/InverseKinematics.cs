// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

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
        private Robot _robotInfo;
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
        private bool _inLimits = true; // Indicates if the axis values are in limits 
        private RobotJointPosition[] _robotJointPositions = new RobotJointPosition[8]; // Contains all the eight solutions
        private RobotJointPosition _robotJointPosition = new RobotJointPosition(); // Contains the final solution
        private ExternalJointPosition _externalJointPosition = new ExternalJointPosition(); // Contains the final solution
        #endregion

        #region constructors
        /// <summary>
        /// Defines an empty Inverse Kinematics object
        /// </summary>
        public InverseKinematics()
        {
            for (int i = 0; i < 8; i++)
            {
                _robotJointPositions[i] = new RobotJointPosition();
            }
        }

        /// <summary>
        /// Initiatize an inverse kinematics from a robot movement and robot info.
        /// </summary>
        /// <param name="movement"> The robot movement to calculated the axis values for. </param>
        /// <param name="robotInfo"> The robot info to calcilated the axis values for. </param>
        public InverseKinematics(Movement movement, Robot robotInfo)
        {
            _robotInfo = robotInfo;
            _movement = movement;

            for (int i = 0; i < 8; i++)
            {
                _robotJointPositions[i] = new RobotJointPosition();
            }

            Initialize();
        }

        /// <summary>
        /// Initiatize an inverse kinematics from a robot target and robot info.
        /// The target will be casted to robot movement with wobj0. 
        /// </summary>
        /// <param name="target"> The target to calculated the axis values for. </param>
        /// <param name="robotInfo"> The robot info to calcilated the axis values for. </param>
        public InverseKinematics(ITarget target, Robot robotInfo)
        {
            _robotInfo = robotInfo;
            _movement = new Movement(target);

            for (int i = 0; i < 8; i++)
            {
                _robotJointPositions[i] = new RobotJointPosition();
            }

            Initialize();
        }

        /// <summary>
        /// Creates a new inverse kinematics by duplicating an existing inverse kinematics.
        /// This creates a deep copy of the existing inverse kinematics.
        /// </summary>
        /// <param name="inverseKinematics"> The inverse kinematics that should be duplicated. </param>
        public InverseKinematics(InverseKinematics inverseKinematics)
        {
            _robotInfo = inverseKinematics.RobotInfo.Duplicate();
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
        /// A method to duplicate the Inverse Kinematics object.
        /// </summary>
        /// <returns> Returns a deep copy of the Inverse Kinematics object. </returns>
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
            if (!this.IsValid)
            {
                return "Invalid Inverse Kinematics";
            }
            else
            {
                return "Inverse Kinematics";
            }
        }

        /// <summary>
        /// A method that calls all the other methods that are needed to initialize the data that is needed to construct a valid inverse kinematics object. 
        /// </summary>
        private void Initialize()
        {
            // Check robot tool: override if the movement contains a robot tool
            if (_movement.RobotTool == null)
            {
                _robotTool = _robotInfo.Tool;
            }
            // Check if the set tool is not empty
            else if (_movement.RobotTool.Name != "" && _movement.RobotTool.Name != null) //TODO: RobotTool.IsValid is maybe better?
            {
                _robotTool = _movement.RobotTool; 
            }
            // Otherwise use the tool that is attached to the robot
            else
            {
                _robotTool = _robotInfo.Tool;
            }

            // Movement related fields
            _target = _movement.Target;

            if (_target is RobotTarget)
            {
                // Calculate the position and the orientation of the target plane in the word coordinate system
                // If there is an external axes connected to work object of the movement the 
                // target plane will be re-oriented according to the pose of the this external axes. 
                _targetPlane = _movement.GetPosedGlobalTargetPlane(_robotInfo, out _);

                // Update the base plane / position plane
                _positionPlane = GetPositionPlane();
                Transform trans = Transform.PlaneToPlane(_positionPlane, Plane.WorldXY);

                // Needed for transformation from the robot world coordinate system to the local robot coordinate system
                Transform orient = Transform.PlaneToPlane(_robotInfo.BasePlane, Plane.WorldXY);

                // Orient the target plane to the robot coordinate system 
                _targetPlane = ToolTransformation(_targetPlane, _robotTool.AttachmentPlane, _robotTool.ToolPlane);
                _endPlane = new Plane(_targetPlane.Origin, _targetPlane.YAxis, _targetPlane.XAxis); //rotates, flips plane for TCP Offset moving in the right direction
                _endPlane.Transform(trans);

                // Deep copy and orient to internal axis planes of the robot. 
                _axisPlanes = new List<Plane>();
                for (int i = 0; i < _robotInfo.InternalAxisPlanes.Count; i++)
                {
                    Plane plane = new Plane(_robotInfo.InternalAxisPlanes[i]);
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
        /// A method that can be called to reinitialize all the data that is needed to construct a valid inverse kinematics object. 
        /// This method also resets the solution. Calculate() has to be called to get the new solution(s). 
        /// </summary>
        public void ReInitialize()
        {
            Initialize();
            ClearCurrentSolutions();
        }

        /// <summary>
        /// Calculates both internal and external axis values. 
        /// </summary>
        public void Calculate()
        {
            ClearCurrentSolutions();
            CalculateInternalAxisValues();
            CalculateExternalAxisValues();
            CheckForInternalAxisLimits();
            CheckForExternalAxisLimits();
        }

        /// <summary>
        /// Calculates the internal axis values. 
        /// This method does not check the internal axis limits. 
        /// </summary>
        public void CalculateInternalAxisValues()
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
                // Caculate internal axis value 1: Wrist center relative to axis 1 in front of robot (configuration 0, 1, 2, 3)
                double internalAxisValue1 = -1 * Math.Atan2(_wrist.Y, _wrist.X);
                if (internalAxisValue1 > Math.PI) { internalAxisValue1 -= 2 * Math.PI; }
                _robotJointPositions[0][0] = internalAxisValue1;
                _robotJointPositions[1][0] = internalAxisValue1;
                _robotJointPositions[2][0] = internalAxisValue1;
                _robotJointPositions[3][0] = internalAxisValue1;

                // Rotate axis value 180 degrees (pi radians): Wrist center relative to axis 1 behind robot (configuration 4, 5, 6, 7)
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
                    // Get the first external axis value
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

                    // Calculates the internal axis value 2 and 3
                    for (int j = 0; j < 2; j++)
                    {
                        // Calculate elbow and wrist variables
                        Point3d elbowPoint;
                        if (j == 1) { elbowPoint = intersectPt1; }
                        else { elbowPoint = intersectPt2; }
                        elbowPlane.ClosestParameter(elbowPoint, out double elbowX, out double elbowY);
                        elbowPlane.ClosestParameter(_wrist, out double wristX, out double wristY);

                        // Calculate axis value 2
                        double internalAxisValue2 = Math.Atan2(elbowY, elbowX); 
                        double internalAxisValue3 = Math.PI - internalAxisValue2 + Math.Atan2(wristY - elbowY, wristX - elbowX) - _axis4offsetAngle;

                        for (int k = 0; k < 2; k++)
                        {
                            // Adds internal axis value 2
                            _robotJointPositions[index1][1] = -internalAxisValue2;

                            // Calculate internal axis value 3
                            double internalAxisValue3Wrapped = -internalAxisValue3 + Math.PI;
                            while (internalAxisValue3Wrapped >= Math.PI) { internalAxisValue3Wrapped -= 2 * Math.PI; }
                            while (internalAxisValue3Wrapped < -Math.PI) { internalAxisValue3Wrapped += 2 * Math.PI; }
                            _robotJointPositions[index1][2] = internalAxisValue3Wrapped;

                            // Update in index tracker
                            index1++;
                        }

                        for (int k = 0; k < 2; k++)
                        {
                            // Calculate internal axis value 4
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

                            // Calculate internal axis value 5
                            Plane internalAxisPlane5 = new Plane(internalAxisPlane4);
                            internalAxisPlane5.Rotate(internalAxisValue4, internalAxisPlane4.ZAxis);
                            internalAxisPlane5 = new Plane(_wrist, -internalAxisPlane5.ZAxis, internalAxisPlane5.XAxis);
                            internalAxisPlane5.ClosestParameter(_endPlane.Origin, out axis6X, out axis6Y);
                            double internalAxisValue5 = Math.Atan2(axis6Y, axis6X);
                            _robotJointPositions[index2][4] = internalAxisValue5;

                            // Calculate internal axis value 6
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
        /// Gets and sets the closest Robot Joint Position to a given previous Robot Joint Position.
        /// This method returns the closest axis configuration as an integer (0-7) and
        /// sets the closest Robot Joint Poistion insides this Inverse Kinematics object. Therefore,
        /// your first have to calculate the Inverse Kinematics solutions before you call this method. 
        /// This methods is typically used for using the Auto Axis Config inside the Path Generator.
        /// </summary>
        /// <param name="prevJointPosition">The previous Robot Joint Position</param>
        /// <returns>Returns the closest axis configuration as an integer (0-7)</returns>
        public RobotJointPosition GetClosestRobotJointPosition(RobotJointPosition prevJointPosition)
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
                    if (_robotInfo.InternalAxisLimits[3].IncludesParameter(_robotJointPositions[i][3] + joint4[j], false) == true)
                    {
                        // Add value to axis 4
                        _robotJointPositions[i][3] += joint4[j];

                        for (int k = 0; k < joint6.Length; k++)
                        {
                            // Check axis 6
                            if (_robotInfo.InternalAxisLimits[5].IncludesParameter(_robotJointPositions[i][5] + joint6[k], false) == true)
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

            return _robotJointPosition;
        }

        /// <summary>
        /// Calculates the external axis values.
        /// This method does not check the external axis limits. 
        /// </summary>
        public void CalculateExternalAxisValues()
        {
            // Clear current solution
            ResetExternalJointPosition();

            if (_target is RobotTarget robotTarget)
            {
                double count = 0;

                // NOTE: Only works for a robot info with an maximum of one external linear axis

                // Add the external axis values to the list with external axis values
                for (int i = 0; i < _robotInfo.ExternalAxes.Count; i++)
                {
                    int logic = _robotInfo.ExternalAxes[i].AxisNumber;

                    // Check if the axis is an external linear axis
                    if (_robotInfo.ExternalAxes[i] is ExternalLinearAxis externalLinearAxis && count == 0)
                    {
                        // Checks if external linear axis value needs to be negative or positive
                        externalLinearAxis.AxisCurve.ClosestPoint(_robotInfo.BasePlane.Origin, out double robotBasePlaneParam);
                        externalLinearAxis.AxisCurve.ClosestPoint(_positionPlane.Origin, out double basePlaneParam);

                        if (basePlaneParam >= robotBasePlaneParam)
                        {
                            _externalJointPosition[logic] = _positionPlane.Origin.DistanceTo(_robotInfo.BasePlane.Origin);
                        }

                        else
                        {
                            _externalJointPosition[logic] = -_positionPlane.Origin.DistanceTo(_robotInfo.BasePlane.Origin);
                        }

                        count += 1;
                    }

                    // If an other type of axis is used or we already set the value for one external linear axis
                    // we set as solution an external axis value of 0 or we use the user defined external axis value. 
                    else
                    {
                        if (robotTarget.ExternalJointPosition[logic] == 9e9)
                        {
                            _externalJointPosition[logic] = 0;
                        }
                        else
                        {
                            _externalJointPosition[logic] = robotTarget.ExternalJointPosition[logic];
                        }
                    }
                }
            }

            else
            {
                _externalJointPosition = _target.ExternalJointPosition;
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
        /// Resets the Robo Joint Position solutions
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
        /// Transforms the tool plane to calculate the inverse kinematics. 
        /// </summary>
        /// <param name="targetPlane"> The global target plane defined in the world coordinate system as a plane.</param>
        /// <param name="attachmentPlane"> The attachement plane of the robot defined the robot coordinate system as plane. </param>
        /// <param name="toolPlane"> The TCP plane as a plane. </param>
        /// <returns> The transfomed tool plane. </returns>
        private Plane ToolTransformation(Plane targetPlane, Plane attachmentPlane, Plane toolPlane)
        {
            Plane result = new Plane(attachmentPlane);
            result.Rotate(Math.PI, attachmentPlane.ZAxis); // Flips the plane
            Transform trans = Transform.PlaneToPlane(toolPlane, targetPlane);
            result.Transform(trans);
            return result;
        }

        /// <summary>
        /// Gets the position of the robot in world coordinate space.
        /// This is closest plane to target plane along the external axis if no external axis value is set. 
        /// If no external axis is used it will return the fixed position (base plane) of the robot. 
        /// </summary>
        /// <returns> The position of the robot as a plane. </returns>
        private Plane GetPositionPlane()
        {
            // NOTE: Only works for a robot info with an maximum of one external linear axis

            // Deep copy the current position / base plane
            Plane plane = new Plane(_robotInfo.BasePlane);

            // Check if an external axis is attached to the robot 
            for (int i = 0; i < _robotInfo.ExternalAxes.Count; i++)
            {
                ExternalAxis externalAxis = _robotInfo.ExternalAxes[i];
                int logic = externalAxis.AxisNumber;

                // Check if an external linear axis is used
                if (externalAxis is ExternalLinearAxis externalLinearAxis)
                {
                    // Calculate closest base plane if the used did not define an external axis value
                    if (_target.ExternalJointPosition[logic] == 9e9)
                    {
                        externalLinearAxis.AxisCurve.ClosestPoint(_targetPlane.Origin, out double param);
                        plane.Origin = externalLinearAxis.AxisCurve.PointAt(param);
                    }

                    // Otherwise use the user definied external axis value
                    else
                    {
                        plane = externalLinearAxis.CalculatePosition(_target.ExternalJointPosition[logic], out _);
                    }

                    // Break the loop since it should only work for one external linear axis.
                    break;
                }

                // NOTE: We do nothing here when an external rotational axis is used. 
            }

            // Returns the position plane of the robot
            return plane;
        }

        /// <summary>
        /// Checks if the interal axis values are outside its limits.
        /// </summary>
        private void CheckForInternalAxisLimits()
        {
            for (int i = 0; i < _robotJointPosition.Length; i++)
            {
                if (_robotInfo.InternalAxisLimits[i].IncludesParameter(_robotJointPosition[i], false) == false)
                { 
                    _errorText.Add("Movement " + Movement.Target.Name + "\\" + Movement.WorkObject.Name + ": Internal axis value " + (i + 1).ToString() + " is not in range.");
                    _inLimits = false;
                }
            }
        }

        /// <summary>
        /// Checks if the external axis values are outside its limits.
        /// </summary>
        private void CheckForExternalAxisLimits()
        {
            for (int i = 0; i < _robotInfo.ExternalAxes.Count; i++)
            {
                int number = _robotInfo.ExternalAxes[i].AxisNumber;
                char logic = _robotInfo.ExternalAxes[i].AxisLogic;

                if (_robotInfo.ExternalAxes[i].AxisLimits.IncludesParameter(_externalJointPosition[number], false) == false)
                {
                    _errorText.Add("Movement " + Movement.Target.Name + "\\" + Movement.WorkObject.Name + ": External axis value " + logic + " is not in range.");
                    _inLimits = false;
                }
            }
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets a value indicating whether the object is valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                if (RobotInfo == null) { return false; }
                if (Movement == null) { return false; }
                return true;
            }
        }

        /// <summary>
        /// The robot info where the axis values are calculated for.
        /// </summary>
        public Robot RobotInfo
        {
            get 
            { 
                return _robotInfo; 
            }
            set 
            { 
                _robotInfo = value;
                ReInitialize();
            }
        }

        /// <summary>
        /// The robot movement where the axis values are caculated for. 
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
        /// The used robot tool for this inverse kinematics.
        /// By default the tool attached to the robot info is used. 
        /// If a tool is set as a property of the movement, this tool will be used. 
        /// The Movement.RobotTool overwrites the tool attached to the robot info. 
        /// </summary>
        public RobotTool RobotTool
        {
            get { return _robotTool; }
        }

        /// <summary>
        /// Defines the eight calculated Robot Joint Positions
        /// </summary>
        public List<RobotJointPosition> RobotJointPositions
        {
            get { return _robotJointPositions.ToList(); }
        }

        /// <summary>
        /// Defines the calculated Robot Joint Position
        /// </summary>
        public RobotJointPosition RobotJointPosition
        {
            get { return _robotJointPosition; }
        }

        /// <summary>
        /// Defines the calculated External Joint Position
        /// </summary>
        public ExternalJointPosition ExternalJointPosition
        {
            get { return _externalJointPosition; }
        }

        /// <summary>
        /// List of strings with collected error messages. 
        /// </summary>
        public List<string> ErrorText
        {
            get { return _errorText; }
        }

        /// <summary>
        /// Bool that indicates if the internal and external values are within their limits
        /// </summary>
        public bool InLimits
        {
            get { return _inLimits; }
        }
        #endregion
    }

}
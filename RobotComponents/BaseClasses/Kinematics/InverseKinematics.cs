using System;
using System.Linq;
using System.Collections.Generic;

using Rhino.Geometry;

using RobotComponents.BaseClasses.Actions;
using RobotComponents.BaseClasses.Definitions;

namespace RobotComponents.BaseClasses.Kinematics
{
    /// <summary>
    /// Inverse Kinematics class, defines the basic properties and methods for 
    /// the inverse kinematics of a 6-axis robot and the attached external axes.
    /// </summary>
    public class InverseKinematics
    {
        #region fields
        private RobotInfo _robotInfo;
        private RobotTool _robotTool;
        private Movement _movement;
        private Target _target;
        private Plane _basePlane;       
        private Plane _targetPlane;
        private Plane _endPlane;

        private Plane[] _axisPlanes;

        private Point3d _wrist;
        private double _wristOffset;
        private double _lowerArmLength;
        private double _upperArmLength;
        private double _axis4offsetAngle;

        private readonly List<double> _internalAxisValue1 = new List<double>();
        private readonly List<double> _internalAxisValue2 = new List<double>();
        private readonly List<double> _internalAxisValue3 = new List<double>();
        private readonly List<double> _internalAxisValue4 = new List<double>();
        private readonly List<double> _internalAxisValue5 = new List<double>();
        private readonly List<double> _internalAxisValue6 = new List<double>();

        private readonly List<double> _internalAxisValues = new List<double>(); // Final calculated internal axis values
        private readonly List<double> _externalAxisValues = new List<double>(); // Final calculated external axis values
        #endregion

        #region constructors
        /// <summary>
        /// Defines an empty Inverse Kinematics object
        /// </summary>
        public InverseKinematics()
        {
        }

        /// <summary>
        /// Initiatize an inverse kinematics from a robot movement and robot info.
        /// </summary>
        /// <param name="movement"> The robot momvement to calculated the axis values for. </param>
        /// <param name="robotInfo"> The robot info to calcilated the axis values for. </param>
        public InverseKinematics(Movement movement, RobotInfo robotInfo)
        {
            _robotInfo = robotInfo;
            _movement = movement;
            Initialize();
        }

        /// <summary>
        /// Initiatize an inverse kinematics from a robot target and robot info.
        /// The target will be casted to robot movement with wobj0. 
        /// </summary>
        /// <param name="target"> The robot target to calculated the axis values for. </param>
        /// <param name="robotInfo"> The robot info to calcilated the axis values for. </param>
        public InverseKinematics(Target target, RobotInfo robotInfo)
        {
            _robotInfo = robotInfo;
            _movement = new Movement(target);
            Initialize();
        }

        /// <summary>
        /// Initiatize an inverse kinematics from a robot movement, robot info and pre-defined axis values.
        /// This method is only use to duplicate the current inverse kinematics. 
        /// </summary>
        /// <param name="movement"> The robot momvement to calculated the axis values for. </param>
        /// <param name="robotInfo"> The robot info to calcilated the axis values for. </param>
        /// <param name="internalAxisValues"> The list with pre-defined internal axis values. </param>
        /// <param name="externalAxisValues"> The list with pre-defined external axis values. </param>
        private InverseKinematics(Movement movement, RobotInfo robotInfo, List<double> internalAxisValues, List<double> externalAxisValues)
        {
            _robotInfo = robotInfo;
            _movement = movement;

            Initialize();

            _internalAxisValues = internalAxisValues;
            _externalAxisValues = externalAxisValues;
        }

        /// <summary>
        /// A method to duplicate the Inverse Kinematics object.
        /// </summary>
        /// <returns> Returns a deep copy of the Inverse Kinematics object. </returns>
        public InverseKinematics Duplicate()
        {
            InverseKinematics dup = new InverseKinematics(Movement, RobotInfo, InternalAxisValues, ExternalAxisValues);
            return dup;
        }
        #endregion

        #region methods
        /// <summary>
        /// A method that calls all the other methods that are needed to initialize the data that is needed to construct a valid inverse kinematics object. 
        /// </summary>
        private void Initialize()
        {
            // Check robot tool: override if the movement contains a robot tool
            if (_movement.RobotTool.Name != "" && _movement.RobotTool.Name != null)
            {
                _robotTool = _movement.RobotTool; 
            }
            else
            {
                _robotTool = _robotInfo.Tool;
            }

            // Movement related fields
            _target = _movement.Target.Duplicate();
            _targetPlane = new Plane(_movement.GlobalTargetPlane);

            // Update the base plane
            _basePlane = GetClosestBasePlane();
            Transform trans = Transform.PlaneToPlane(_basePlane, Plane.WorldXY);

            // Orient the target plane to the robot coordinate system 
            _targetPlane = ToolTransformation(_targetPlane, _robotTool.AttachmentPlane, _robotTool.ToolPlane);
            _endPlane = new Plane(_targetPlane.Origin, _targetPlane.YAxis, _targetPlane.XAxis); //rotates, flips plane for TCP Offset moving in the right direction
            _endPlane.Transform(trans);

            // Robot info related fields
            _axisPlanes = _robotInfo.InternalAxisPlanes.ToArray();
            _wristOffset = _axisPlanes[5].Origin.X - _axisPlanes[4].Origin.X;
            _lowerArmLength = _axisPlanes[1].Origin.DistanceTo(_axisPlanes[2].Origin);
            _upperArmLength = _axisPlanes[2].Origin.DistanceTo(_axisPlanes[4].Origin);
            _axis4offsetAngle = Math.Atan2(_axisPlanes[4].Origin.Z - _axisPlanes[2].Origin.Z, _axisPlanes[4].Origin.X - _axisPlanes[2].Origin.X);
            _wrist = new Point3d(_endPlane.PointAt(0, 0, _wristOffset));
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
        /// Calculates the solution for all the eight axis configurations
        /// </summary>
        public void Calculate()
        {
            // Clear the current solutions before calculating a new ones. 
            ClearCurrentSolutions();

            #region wrist center relative to axis 1
            // Note that this is reversed because the clockwise direction when looking 
            // down at the XY plane is typically taken as the positive direction for robot axis1

            // Caculate internal axis value 1: Wrist center relative to axis 1 in front of robot (configuration 0, 1, 2, 3)
            double internalAxisValue1 = -1 * Math.Atan2(_wrist.Y, _wrist.X);
            if (internalAxisValue1 > Math.PI) { internalAxisValue1 -= 2 * Math.PI; }
            _internalAxisValue1.AddRange(Enumerable.Repeat(internalAxisValue1, 4).ToList());

            // Rotate axis value 180 degrees (pi radians): Wrist center relative to axis 1 behind robot (configuration 4, 5, 6, 7)
            internalAxisValue1 += Math.PI;
            if (internalAxisValue1 > Math.PI) { internalAxisValue1 -= 2 * Math.PI; }
            _internalAxisValue1.AddRange(Enumerable.Repeat(internalAxisValue1, 4).ToList());
            #endregion

            // Generates 4 sets of values for each option of axis 1
            // i = 0: Wrist center relative to axis 1 in front of robot (configuration 0, 1, 2, 3)
            // i = 1: Wrist center relative to axis 1 behind robot (configuration 4, 5, 6, 7)
            foreach (int i in Enumerable.Range(0, 2))
            {
                // Get the first external axis value
                internalAxisValue1 = _internalAxisValue1[i * 4];

                // Get the elbow points
                Point3d internalAxisPoint1 = new Point3d(_axisPlanes[1].Origin);
                Point3d internalAxisPoint2 = new Point3d(_axisPlanes[2].Origin); 
                Point3d internalAxisPoint4 = new Point3d(_axisPlanes[4].Origin);

                // Rotate the points to the correction position
                Transform rot1 = Transform.Rotation(-1 * internalAxisValue1, Plane.WorldXY.Origin);
                internalAxisPoint1.Transform(rot1);
                internalAxisPoint2.Transform(rot1);
                internalAxisPoint4.Transform(rot1);

                // Create the elbow projection plane
                Vector3d elbowDir = new Vector3d(1, 0, 0);
                elbowDir.Transform(rot1);
                Plane elbowPlane = new Plane(internalAxisPoint1, elbowDir, Plane.WorldXY.ZAxis);

                Sphere sphere1 = new Sphere(internalAxisPoint1, _lowerArmLength);
                Sphere sphere2 = new Sphere(_wrist, _upperArmLength);

                Circle circ = new Circle();

                Rhino.Geometry.Intersect.Intersection.SphereSphere(sphere1, sphere2, out circ);
                Rhino.Geometry.Intersect.Intersection.PlaneCircle(elbowPlane, circ, out double par1, out double par2);

                Point3d intersectPt1 = circ.PointAt(par1);
                Point3d intersectPt2 = circ.PointAt(par2);

                // Calculates the internal axis value 2 and 3
                foreach (int j in Enumerable.Range(0, 2))
                {
                    Point3d elbowPoint;
                    if (j == 1) { elbowPoint = intersectPt1; }
                    else { elbowPoint = intersectPt2; }

                    elbowPlane.ClosestParameter(elbowPoint, out double elbowX, out double elbowY);
                    elbowPlane.ClosestParameter(_wrist, out double wristX, out double wristY);

                    double internalAxisValue2 = Math.Atan2(elbowY, elbowX);
                    double internalAxisValue3 = Math.PI - internalAxisValue2 + Math.Atan2(wristY - elbowY, wristX - elbowX) - _axis4offsetAngle;

                    // Calculates the internal axis value 2 and 3 (the elbow position angles)
                    foreach (int k in Enumerable.Range(0, 2))
                    {
                        // Adds internal axis value 2
                        _internalAxisValue2.Add(-internalAxisValue2);

                        // Calculate internal axis value 3
                        double internalAxisValue3Wrapped = -internalAxisValue3 + Math.PI;
                        while (internalAxisValue3Wrapped >= Math.PI) { internalAxisValue3Wrapped -= 2 * Math.PI; }
                        while (internalAxisValue3Wrapped < -Math.PI) { internalAxisValue3Wrapped += 2 * Math.PI; }
                        _internalAxisValue3.Add(internalAxisValue3Wrapped);
                    }

                    // Calculates the internal axis value 4, 5 and 6
                    foreach (int k in Enumerable.Range(0, 2))
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
                        _internalAxisValue4.Add(internalAxisValue4Wrapped);

                        // Calculate internal axis value 5
                        Plane internalAxisPlane5 = new Plane(internalAxisPlane4);
                        internalAxisPlane5.Rotate(internalAxisValue4, internalAxisPlane4.ZAxis);
                        internalAxisPlane5 = new Plane(_wrist, -internalAxisPlane5.ZAxis, internalAxisPlane5.XAxis);
                        internalAxisPlane5.ClosestParameter(_endPlane.Origin, out axis6X, out axis6Y);
                        double internalAxisValue5 = Math.Atan2(axis6Y, axis6X);
                        _internalAxisValue5.Add(internalAxisValue5);

                        // Calculate internal axis value 6
                        Plane internalAxisPlane6 = new Plane(internalAxisPlane5);
                        internalAxisPlane6.Rotate(internalAxisValue5, internalAxisPlane5.ZAxis);
                        internalAxisPlane6 = new Plane(_wrist, -internalAxisPlane6.YAxis, internalAxisPlane6.ZAxis);
                        internalAxisPlane6.ClosestParameter(_endPlane.PointAt(0, -1), out double endX, out double endY);
                        double internalAxisValue6 = Math.Atan2(endY, endX);
                        _internalAxisValue6.Add(internalAxisValue6);
                    }
                }
            }

            // Convert the axis angles from radians to degrees + other corrections
            foreach (int i in Enumerable.Range(0, 8))
            {
                _internalAxisValue1[i] = _internalAxisValue1[i] * (180 / Math.PI) * -1;
                _internalAxisValue2[i] = _internalAxisValue2[i] * (180 / Math.PI);
                _internalAxisValue3[i] = _internalAxisValue3[i] * (180 / Math.PI);
                _internalAxisValue4[i] = _internalAxisValue4[i] * (180 / Math.PI) * -1;
                _internalAxisValue5[i] = _internalAxisValue5[i] * (180 / Math.PI);
                if (_internalAxisValue6[i] <= 0) { _internalAxisValue6[i] = (180 + (_internalAxisValue6[i] * (180 / Math.PI))) * -1; }
                else { _internalAxisValue6[i] = 180 - _internalAxisValue6[i] * (180 / Math.PI); }
            }

            // Pick one of the eight solutions
            _internalAxisValues.Add(_internalAxisValue1[_target.AxisConfig]);
            _internalAxisValues.Add(_internalAxisValue2[_target.AxisConfig]);
            _internalAxisValues.Add(_internalAxisValue3[_target.AxisConfig]);
            _internalAxisValues.Add(_internalAxisValue4[_target.AxisConfig]);
            _internalAxisValues.Add(_internalAxisValue5[_target.AxisConfig]);
            _internalAxisValues.Add(_internalAxisValue6[_target.AxisConfig]);

            // Calculate the external axis values
            CalculateExternalAxisValues();

            // Correction for what? 
            _internalAxisValues[1] = _internalAxisValues[1] + 90; // + 0.5 * Math.PI
            _internalAxisValues[2] = _internalAxisValues[2] - 90; // - 0.5 * Math.PI
        }

        /// <summary>
        /// Calculates the external axis values
        /// </summary>
        private void CalculateExternalAxisValues()
        {
            // Get user defined external axis values
            List<double> userDefinedExternalAxisValues = _target.ExternalAxisValues;
            double count = 0;

            // NOTE: The axis logic is the list order with external axes
            // NOTE: Only works for a robot info with an maximum of one external linear axis

            // Add the external axis values to the list with external axis values
            for (int i = 0; i < _robotInfo.ExternalAxis.Count; i++)
            {
                // Check if the axis is an external linear axis
                if (_robotInfo.ExternalAxis[i] is ExternalLinearAxis && count == 0)
                {
                    // Get the external linear axis
                    ExternalLinearAxis externalLinearAxis = _robotInfo.ExternalAxis[i] as ExternalLinearAxis;

                    // Checks if external linear axis value needs to be negative or positive
                    externalLinearAxis.AxisCurve.ClosestPoint(_robotInfo.BasePlane.Origin, out double robotBasePlaneParam);
                    externalLinearAxis.AxisCurve.ClosestPoint(_basePlane.Origin, out double basePlaneParam);

                    if (basePlaneParam >= robotBasePlaneParam)
                    {
                        _externalAxisValues.Add(_basePlane.Origin.DistanceTo(_robotInfo.BasePlane.Origin));
                    }

                    else
                    {
                        _externalAxisValues.Add(-_basePlane.Origin.DistanceTo(_robotInfo.BasePlane.Origin));
                    }

                    count += 1;
                }

                // If an other type of axis is used or we already set the value for one external linear axis
                // we set as solution an external axis value of 0 or we use the user defined external axis value. 
                else
                {
                    if (userDefinedExternalAxisValues[i] == 9e9)
                    {
                        _externalAxisValues.Add(0);
                    }
                    else
                    {
                        _externalAxisValues.Add(userDefinedExternalAxisValues[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Clears the lists with the current solutions.
        /// </summary>
        private void ClearCurrentSolutions()
        {
            _internalAxisValues.Clear();
            _externalAxisValues.Clear();

            _internalAxisValue1.Clear();
            _internalAxisValue2.Clear();
            _internalAxisValue3.Clear();
            _internalAxisValue4.Clear();
            _internalAxisValue5.Clear();
            _internalAxisValue6.Clear();
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
        /// Calculates the lane that is closest to target plane along the external axis.
        /// If no external axis is used it will return the fixed position (base plane) of the robot. 
        /// </summary>
        /// <returns> The position of the robot as a plane. </returns>
        public Plane GetClosestBasePlane()
        {
            // NOTE: Only works for a robot info with an maximum of one external linear axis

            // Deep copy the current position / base plane
            Plane plane = new Plane(_robotInfo.BasePlane);

            // Check if an external axis is attached to the robot 
            for (int i = 0; i < _robotInfo.ExternalAxis.Count; i++)
            {
                // Check if an external linear axis is used
                if (_robotInfo.ExternalAxis[i] is ExternalLinearAxis)
                {
                    // Calculate closest base plane if the used did not define an external axis value
                    if (_target.ExternalAxisValues[i] == 9e9)
                    {
                        ExternalLinearAxis externalLinearAxis = _robotInfo.ExternalAxis[i] as ExternalLinearAxis;
                        externalLinearAxis.AxisCurve.ClosestPoint(_targetPlane.Origin, out double param);
                        plane.Origin = externalLinearAxis.AxisCurve.PointAt(param);
                    }

                    // Otherwise use the user definied external axis value
                    else
                    {
                        ExternalLinearAxis externalLinearAxis = _robotInfo.ExternalAxis[i] as ExternalLinearAxis;
                        plane = externalLinearAxis.CalculatePosition(_target.ExternalAxisValues[i], out bool inLimits);
                    }

                    // Break the loop since it should only work for one external linear axis.
                    break;
                }

                // NOTE: We do nothing here when an external rotational axis is used. 
            }

            // Returns the position plane of the robot
            return plane;
        }
        #endregion

        #region properties
        /// <summary>
        /// A boolean that indicates if the Inverse Kinematics object is valid. 
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
        public RobotInfo RobotInfo
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
        /// The calculated internal axis values
        /// </summary>
        public List<double> InternalAxisValues
        {
            get { return _internalAxisValues; }
        }

        /// <summary>
        /// The calculated external axis values
        /// </summary>
        public List<double> ExternalAxisValues 
        {
            get { return _externalAxisValues; }
        }
        #endregion
    }

}
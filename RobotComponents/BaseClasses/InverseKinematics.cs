using System;
using System.Linq;
using System.Collections.Generic;

using Rhino.Geometry;

namespace RobotComponents.BaseClasses
{
    /// <summary>
    /// Inverse Kinematics class, defines the basic properties and methods for any Inverse Kinematics.
    /// </summary>
    public class InverseKinematics
    {
        #region fields
        RobotInfo _robotInfo;
        Target _target;
        Plane _basePlane;           //robot basePlane
        Plane _mountingFrame;       //robot mountingFrame
        Plane _toolPlane;           //robot toolPlane
        Plane _targetPlane;         //targetPlane
        Plane _endPlane;

        Plane[] _axisPlanes;        //RobotAxisLocations
        int _axisConfig = 2;        //????

        Point3d _wrist;
        double _wristOffset;
        double _lowerArmLength;
        double _upperArmLength;
        double _axis4offsetAngle;

        List<double> _axis1Angles = new List<double>();
        List<double> _axis2Angles = new List<double>();
        List<double> _axis3Angles = new List<double>();
        List<double> _axis4Angles = new List<double>();
        List<double> _axis5Angles = new List<double>();
        List<double> _axis6Angles = new List<double>();

        List<double> _internalAxisValues = new List<double>(); //Final Calculated Internal AxisAngles
        List<double> _externalAxisValues = new List<double>(); //Final Calculated External AxisAngles
        #endregion

        #region constructors
        public InverseKinematics()
        {
        }

        public InverseKinematics(Target target, RobotInfo robotInfo, RobotTool robotTool)
        {
            this._robotInfo = robotInfo;
            this._robotInfo.Tool = robotTool;
            Update(target);
        }

        public InverseKinematics(Movement movement, RobotInfo robotInfo)
        {
            this._robotInfo = robotInfo;
            Update(robotInfo, movement);
        }

        public InverseKinematics(Target target, RobotInfo robotInfo)
        {
            this._robotInfo = robotInfo;
            Update(target);
        }

        public InverseKinematics(RobotInfo robotInfo)
        {
            this._robotInfo = robotInfo;
        }

        public InverseKinematics Duplicate()
        {
            InverseKinematics dup = new InverseKinematics(Target, RobotInfo);
            return dup;
        }
        #endregion

        #region methods
        private void Update(Target target)
        {
            _axis1Angles.Clear();
            _axis2Angles.Clear();
            _axis3Angles.Clear();
            _axis4Angles.Clear();
            _axis5Angles.Clear();
            _axis6Angles.Clear();

            _internalAxisValues.Clear();
            _externalAxisValues.Clear();

            this._target = target;
            this._basePlane = _robotInfo.BasePlane;
            this._mountingFrame = _robotInfo.MountingFrame;
            this._toolPlane = _robotInfo.ToolPlane;
            this._targetPlane = target.Plane;
            this._axisPlanes = _robotInfo.InternalAxisPlanes.ToArray();
            this._axisConfig = target.AxisConfig;

            _basePlane = GetClosestBasePlane();
            Transform trans = Transform.PlaneToPlane(_basePlane, Plane.WorldXY);
            this._targetPlane = ToolTransformation(_targetPlane, _robotInfo.Tool.AttachmentPlane, _robotInfo.Tool.ToolPlane);

            this._endPlane = new Plane(_targetPlane.Origin, _targetPlane.YAxis, _targetPlane.XAxis); //rotates, flips Plane for TCP Offset moving in in right direction
            this._endPlane.Transform(trans);

            this._wristOffset = _axisPlanes[5].Origin.X - _axisPlanes[4].Origin.X;
            this._lowerArmLength = _axisPlanes[1].Origin.DistanceTo(_axisPlanes[2].Origin);
            this._upperArmLength = _axisPlanes[2].Origin.DistanceTo(_axisPlanes[4].Origin);
            this._axis4offsetAngle = Math.Atan2(_axisPlanes[4].Origin.Z - _axisPlanes[2].Origin.Z, _axisPlanes[4].Origin.X - _axisPlanes[2].Origin.X);

            this._wrist = new Point3d(_endPlane.PointAt(0, 0, _wristOffset));
        }

        public void Update(RobotInfo robotInfo, Movement movement)
        {
            // Set the robot info
            this._robotInfo = robotInfo;

            // Check robot tool: override the movement contains are robotTool
            if (movement.RobotTool.Name != "" && movement.RobotTool.Name != null)
            {
                this._robotInfo.Tool = movement.RobotTool;
            }

            // Deep copy the target
            Target target = movement.Target.Duplicate();

            // Change the target plane from a local plane to global plane
            target.Plane = movement.GlobalTargetPlane;

            // Update
            Update(target);

        }
        public void Update(RobotInfo robotInfo, Target target)
        {
            this._robotInfo = robotInfo;
            Update(target);
        }

        public void Calculate()
        {
            _internalAxisValues.Clear();
            _externalAxisValues.Clear();
            _axis1Angles.Clear();
            _axis2Angles.Clear();
            _axis3Angles.Clear();
            _axis4Angles.Clear();
            _axis5Angles.Clear();
            _axis6Angles.Clear();

            double axis1Angle = -1 * Math.Atan2(_wrist.Y, _wrist.X);
            //note that this is reversed because the clockwise direction when looking down at the XY plane
            //is typically taken as the positive direction for robot axis1

            if (axis1Angle > Math.PI)
            {
                axis1Angle -= 2 * Math.PI;
            }

            foreach (int i in Enumerable.Range(0, 4))
            {
                _axis1Angles.Add(axis1Angle);
            }

            axis1Angle += Math.PI;

            if (axis1Angle > Math.PI)
            {
                axis1Angle -= 2 * Math.PI;
            }

            foreach (int i in Enumerable.Range(0, 4))
            {
                _axis1Angles.Add(axis1Angle);
            }

            //generates 4 sets of values for each option of axis1
            foreach (int i in Enumerable.Range(0, 2))
            {
                axis1Angle = _axis1Angles[i * 4];
                Transform rot1 = Transform.Rotation(-1 * axis1Angle, Plane.WorldXY.Origin);//BasePlane.Origin);Plane.WorldXY.Origin

                Point3d p1A = new Point3d(_axisPlanes[1].Origin); //_axisPoints[0]
                Point3d p2A = new Point3d(_axisPlanes[2].Origin); // _axisPoints[1]
                Point3d p3A = new Point3d(_axisPlanes[4].Origin); // _axisPoints[2]

                p1A.Transform(rot1);
                p2A.Transform(rot1);
                p3A.Transform(rot1);

                Vector3d elbowDir = new Vector3d(1, 0, 0);
                elbowDir.Transform(rot1);
                Plane elbowPlane = new Plane(p1A, elbowDir, Plane.WorldXY.ZAxis); //Plane.WorldXY.ZAxis

                Sphere sphere1 = new Sphere(p1A, _lowerArmLength);
                Sphere sphere2 = new Sphere(_wrist, _upperArmLength);

                Circle circ = new Circle();
                Rhino.Geometry.Intersect.Intersection.SphereSphere(sphere1, sphere2, out circ);

                double par1, par2;
                Rhino.Geometry.Intersect.Intersection.PlaneCircle(elbowPlane, circ, out par1, out par2);

                Point3d intersectPt1 = circ.PointAt(par1);
                Point3d intersectPt2 = circ.PointAt(par2);

                foreach (int j in Enumerable.Range(0, 2))
                {
                    Point3d elbowPt;
                    if (j == 0) { elbowPt = intersectPt1; }
                    else { elbowPt = intersectPt2; }

                    double elbowX, elbowY;
                    elbowPlane.ClosestParameter(elbowPt, out elbowX, out elbowY);
                    double wristX, wristY;
                    elbowPlane.ClosestParameter(_wrist, out wristX, out wristY);

                    double axis2Angle = Math.Atan2(elbowY, elbowX);
                    double axis3Angle = Math.PI - axis2Angle + Math.Atan2(wristY - elbowY, wristX - elbowX) - _axis4offsetAngle;

                    foreach (int k in Enumerable.Range(0, 2))
                    {
                        _axis2Angles.Add(-axis2Angle);
                        double axis3AngleWrapped = -axis3Angle + Math.PI;

                        while (axis3AngleWrapped >= Math.PI) { axis3AngleWrapped -= 2 * Math.PI; }
                        while (axis3AngleWrapped < -Math.PI) { axis3AngleWrapped += 2 * Math.PI; }

                        _axis3Angles.Add(axis3AngleWrapped);
                    }

                    foreach (int k in Enumerable.Range(0, 2))
                    {
                        Vector3d axis4 = new Vector3d(_wrist - elbowPt);
                        axis4.Rotate(-_axis4offsetAngle, elbowPlane.ZAxis);
                        Vector3d lowerArm = new Vector3d(elbowPt - p1A);
                        Plane tempPlane = new Plane(elbowPlane);
                        tempPlane.Rotate(axis2Angle + axis3Angle, tempPlane.ZAxis);

                        //B = tempPlane
                        Plane axis4Plane = new Plane(_wrist, tempPlane.ZAxis, -1.0 * tempPlane.YAxis);
                        double axis6X, axis6Y;
                        axis4Plane.ClosestParameter(_endPlane.Origin, out axis6X, out axis6Y);

                        double axis4Angle = Math.Atan2(axis6Y, axis6X);
                        if (k == 1)
                        {
                            axis4Angle += Math.PI;
                            if (axis4Angle > Math.PI)
                            {
                                axis4Angle -= 2 * Math.PI;
                            }
                        }
                        double axis4AngleWrapped = axis4Angle + Math.PI / 2;

                        while (axis4AngleWrapped >= Math.PI)
                        {
                            axis4AngleWrapped -= 2 * Math.PI;
                        }
                        while (axis4AngleWrapped < -Math.PI)
                        {
                            axis4AngleWrapped += 2 * Math.PI;
                        }

                        _axis4Angles.Add(axis4AngleWrapped);

                        Plane axis5Plane = new Plane(axis4Plane);
                        axis5Plane.Rotate(axis4Angle, axis4Plane.ZAxis);
                        axis5Plane = new Plane(_wrist, -axis5Plane.ZAxis, axis5Plane.XAxis);
                        axis5Plane.ClosestParameter(_endPlane.Origin, out axis6X, out axis6Y);
                        double axis5Angle = Math.Atan2(axis6Y, axis6X);
                        _axis5Angles.Add(axis5Angle);

                        Plane axis6Plane = new Plane(axis5Plane);
                        axis6Plane.Rotate(axis5Angle, axis5Plane.ZAxis);
                        //axis6Plane = new Plane(_wrist, -axis6Plane.ZAxis, axis6Plane.XAxis);
                        axis6Plane = new Plane(_wrist, -axis6Plane.YAxis, axis6Plane.ZAxis);
                        double endX, endY;
                        axis6Plane.ClosestParameter(_endPlane.PointAt(0, -1), out endX, out endY);
                        double axis6Angle = Math.Atan2(endY, endX);
                        _axis6Angles.Add(axis6Angle);
                    }
                }
            }

            foreach (int i in Enumerable.Range(0, 8))
            {
                _axis1Angles[i] = _axis1Angles[i] * (180 / Math.PI) * -1;
                _axis2Angles[i] = _axis2Angles[i] * (180 / Math.PI);
                _axis3Angles[i] = _axis3Angles[i] * (180 / Math.PI);
                _axis4Angles[i] = _axis4Angles[i] * (180 / Math.PI) * -1;
                _axis5Angles[i] = _axis5Angles[i] * (180 / Math.PI);
                if (_axis6Angles[i] <= 0)
                {
                    _axis6Angles[i] = (180 + (_axis6Angles[i] * (180 / Math.PI))) * -1;
                }
                else
                {
                    _axis6Angles[i] = 180 - _axis6Angles[i] * (180 / Math.PI);
                }
            }

            #region Convert AxisConfig
            switch (_axisConfig)
            {
                case 0:
                    _axisConfig = 2;
                    break;
                case 1:
                    _axisConfig = 3;
                    break;
                case 2:
                    _axisConfig = 0;
                    break;
                case 3:
                    _axisConfig = 1;
                    break;
                case 4:
                    _axisConfig = 6;
                    break;
                case 5:
                    _axisConfig = 7;
                    break;
                case 6:
                    _axisConfig = 4;
                    break;
                case 7:
                    _axisConfig = 5;
                    break;
            }
            #endregion

            _internalAxisValues.Add(_axis1Angles[_axisConfig]);
            _internalAxisValues.Add(_axis2Angles[_axisConfig]);
            _internalAxisValues.Add(_axis3Angles[_axisConfig]);
            _internalAxisValues.Add(_axis4Angles[_axisConfig]);
            _internalAxisValues.Add(_axis5Angles[_axisConfig]);
            _internalAxisValues.Add(_axis6Angles[_axisConfig]);

            // Checks if External Linear Axis Value (axis[6]) needs to be negative or positive
            for (int i = 0; i < _robotInfo.ExternalAxis.Count; i++)
            {
                if (_robotInfo.ExternalAxis[i] is ExternalLinearAxis)
                {
                    ExternalLinearAxis externalLinearAxis = _robotInfo.ExternalAxis[i] as ExternalLinearAxis;
                    double robotBasePlaneParam = 0;
                    externalLinearAxis.AxisCurve.ClosestPoint(_robotInfo.BasePlane.Origin, out robotBasePlaneParam);
                    double basePlaneParam = 0;
                    externalLinearAxis.AxisCurve.ClosestPoint(_basePlane.Origin, out basePlaneParam);

                    if (basePlaneParam >= robotBasePlaneParam)
                    {
                        _externalAxisValues.Add(_basePlane.Origin.DistanceTo(_robotInfo.BasePlane.Origin));
                    }
                    else
                    {
                        _externalAxisValues.Add(-_basePlane.Origin.DistanceTo(_robotInfo.BasePlane.Origin));
                    }

                }
            }
           
            //double robotBasePlaneParam = 0;
            //_robotInfo.ExternalLinearAxis.AxisCurve.ClosestPoint(_robotInfo.BasePlane.Origin + new Vector3d(_robotInfo.ExternalLinearAxis.RobotAttachmentOrigin.X, _robotInfo.ExternalLinearAxis.RobotAttachmentOrigin.Y, _robotInfo.ExternalLinearAxis.RobotAttachmentOrigin.Z), out robotBasePlaneParam);
            //double basePlaneParam = 0;
            //_robotInfo.ExternalLinearAxis.AxisCurve.ClosestPoint(_basePlane.Origin, out basePlaneParam);
            //
            //    if (basePlaneParam >= robotBasePlaneParam)
            //    {
            //        _externalAxisValues.Add(_basePlane.Origin.DistanceTo(_robotInfo.BasePlane.Origin + new Vector3d(_robotInfo.ExternalLinearAxis.RobotAttachmentOrigin.X, _robotInfo.ExternalLinearAxis.RobotAttachmentOrigin.Y, _robotInfo.ExternalLinearAxis.RobotAttachmentOrigin.Z)));
            //    }
            //    else
            //    {
            //        _externalAxisValues.Add(-_basePlane.Origin.DistanceTo(_robotInfo.BasePlane.Origin + new Vector3d(_robotInfo.ExternalLinearAxis.RobotAttachmentOrigin.X, _robotInfo.ExternalLinearAxis.RobotAttachmentOrigin.Y, _robotInfo.ExternalLinearAxis.RobotAttachmentOrigin.Z)));
            //    }

            // Adds 0s for missing External Linear Axis Values
            //for (int i = _externalAxisValues.Count; i < 6; i++)
            //{
            //    _externalAxisValues.Add(0);
            //}
           
            _internalAxisValues[1] = _internalAxisValues[1] + 90;
            _internalAxisValues[2] = _internalAxisValues[2] - 90;
        }

        private Plane ToolTransformation(Plane targetPlane, Plane attachmentPlane, Plane toolPlane)
        {
            //Plane result = new Plane(attachmentPlane);
            //result.Transform(Transform.PlaneToPlane(toolPlane, targetPlane));
            //return result;
            Plane result = new Plane(attachmentPlane);
            result.Rotate(Math.PI, attachmentPlane.ZAxis); //fixes the Flipped Problem
            Transform trans = Transform.PlaneToPlane(toolPlane, targetPlane);

            result.Transform(trans);
            return result;
        }

        private Plane OffsetPlane(Plane plane, double distance, Vector3d direction)
        {
            Plane planeNow = new Plane(plane.Origin, plane.XAxis, plane.YAxis); // BW Added to avoid problems of not deepcopying
            Vector3d _vecOffset = Vector3d.Multiply(distance, direction);
            Transform _trans = Rhino.Geometry.Transform.Translation(_vecOffset);
            planeNow.Transform(_trans);
            return planeNow;
        }

        // Calculates Plane that is closest to Target Plane along External Axis
        public Plane GetClosestBasePlane()
        {
            Plane plane = new Plane(_robotInfo.BasePlane);

            for (int i = 0; i < _robotInfo.ExternalAxis.Count; i++)
            {
                if (_robotInfo.ExternalAxis[i] is ExternalLinearAxis)
                {
                    // Calculate closest base plane
                    if (_target.ExternalAxisValues[i] == 9e9)
                    {
                        ExternalLinearAxis externalLinearAxis = _robotInfo.ExternalAxis[i] as ExternalLinearAxis;
                        externalLinearAxis.AxisCurve.ClosestPoint(_targetPlane.Origin, out double param);
                        plane.Origin = externalLinearAxis.AxisCurve.PointAt(param);
                    }

                    // User overwrite external axis values
                    else
                    {
                        ExternalLinearAxis externalLinearAxis = _robotInfo.ExternalAxis[i] as ExternalLinearAxis;
                        plane = externalLinearAxis.CalculatePosition(_target.ExternalAxisValues[i], out bool inLimits);
                    }

                }
            }

            return plane;
        }
        #endregion

        #region properties
        public bool IsValid
        {
            get
            {
                if (BasePlane == null) { return false; }
                if (ToolPlane == null) { return false; }
                if (TargetPlane == null) { return false; }
                if (AxisPlanes == null) { return false; }

                return true;
            }
        }

        public RobotInfo RobotInfo
        {
            get { return _robotInfo; }
            set { _robotInfo = value; }
        }

        public Target Target
        {
            get { return _target; }
            set { _target = value; }
        }

        public Plane BasePlane
        {
            get { return _basePlane; }
            set { _basePlane = value; }
        }

        public Plane ToolPlane
        {
            get { return _toolPlane; }
            set { _toolPlane = value; }
        }

        public Plane TargetPlane
        {
            get { return _targetPlane; }
            set { _targetPlane = value; }
        }

        public Plane EndPlane
        {
            get { return _endPlane; }
            set { _endPlane = value; }
        }

        public Plane[] AxisPlanes
        {
            get { return _axisPlanes; }
            set { _axisPlanes = value; }
        }

        public List<double> InternalAxisValues
        {
            get { return this._internalAxisValues; }
            set { this._internalAxisValues = value; }
        }

        public int AxisConfig
        {
            get { return _axisConfig; }
            set { _axisConfig = value; }
        }

        public List<double> ExternalAxisValues 
        {
            get { return _externalAxisValues; }
            set { _externalAxisValues = value; }
        }
        #endregion
    }

}
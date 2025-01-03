// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
using System.Linq;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.ABB.Actions;
using RobotComponents.ABB.Definitions;
using RobotComponents.ABB.Enumerations;
using RobotComponents.ABB.Actions.Instructions;
using RobotComponents.ABB.Actions.Declarations;
using RobotComponents.ABB.Utils;

namespace RobotComponents.ABB.Kinematics
{
    /// <summary>
    /// Represent the Path Generator.
    /// </summary>
    /// <remarks>
    /// This class is used to approximate of the path the Robot will follow for a given set of Actions. 
    /// Speed Datas and Zone Datas are neglected. 
    /// </remarks>
    public class PathGenerator
    {
        #region fields
        private readonly Robot _robot; // The robot info to construct the path for
        private readonly List<Plane> _planes; // The planes the path follow
        private readonly List<Curve> _paths; // The path curves between two movement targets
        private readonly List<Movement> _movements; // The movements of the path
        private readonly List<RobotJointPosition> _robotJointPositions; // The robot joint positions needed to follow the path
        private readonly List<ExternalJointPosition> _externalJointPositions; // The external joint position needed to follow the path
        private readonly List<ConfigurationData> _configurationDatas; // The configurations datas for each robot joint position
        private readonly List<bool> _isInLimits; // Indicates whether or not the joint positions are within their limits
        private List<string> _errorText = new List<string>(); // List with collected error messages

        private bool _isFirstMovementMoveAbsJ; // Bool that indicates if the first movemement is an absolute joint movement
        private readonly RobotTool _initialTool; // Defines the first tool that will be used
        private RobotTool _currentTool; // Defines the default robot tool
        private bool _linearConfigurationControl; // Defines if the configuration control for linear movements is enabled
        private bool _jointConfigurationControl; // Defines if the configuration control for joint movements is enabled
        private CirPathMode _cirPathMode; // Defines the circle path mode
        private int _interpolations = 5; // Defines the number of interpolations between two targets
        private double _programTime = 0; // Estimate of the total program time
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the Path Generator class.
        /// </summary>
        public PathGenerator()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Path Generator class.
        /// </summary>
        /// <param name="robot"> The Robot to generate the path for. </param>
        public PathGenerator(Robot robot)
        {
            _planes = new List<Plane>();
            _paths = new List<Curve>();
            _movements = new List<Movement>();
            _robotJointPositions = new List<RobotJointPosition>();
            _externalJointPositions = new List<ExternalJointPosition>();
            _configurationDatas = new List<ConfigurationData>();
            _isInLimits = new List<bool>();
            _robot = robot.Duplicate(); // Since we might swap tools and therefore change the robot tool we make a deep copy
            _initialTool = robot.Tool.DuplicateWithoutMesh();
        }
        #endregion

        #region methods
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns> 
        /// A string that represents the current object. 
        /// </returns>
        public override string ToString()
        {
            if (!IsValid)
            {
                return "Invalid Path Generator";
            }
            else
            {
                return "Path Generator";
            }
        }

        /// <summary>
        /// Resets / clears the current solution.
        /// </summary>
        private void Reset()
        {
            // Clear current solution
            _movements.Clear();
            _robotJointPositions.Clear();
            _externalJointPositions.Clear();
            _configurationDatas.Clear();
            _planes.Clear();
            _paths.Clear();
            _isInLimits.Clear();
            _errorText.Clear();

            // Reinitiate starting values
            _currentTool = _initialTool;
            _linearConfigurationControl = true;
            _jointConfigurationControl = true;
            _isFirstMovementMoveAbsJ = false;
            _cirPathMode = CirPathMode.PathFrame;
            _programTime = 0;

            // Add initial positions
            RobotJointPosition robotJointPosition = new RobotJointPosition();
            ExternalJointPosition externalJointPosition = new ExternalJointPosition();

            for (int i = 0; i < _robot.ExternalAxes.Count; i++)
            {
                externalJointPosition[_robot.ExternalAxes[i].AxisNumber] = _robot.ExternalAxes[i].AxisLimits.Min;
            }

            _robot.ForwardKinematics.Calculate(robotJointPosition, externalJointPosition);

            _robotJointPositions.Add(robotJointPosition);
            _externalJointPositions.Add(externalJointPosition);
            _configurationDatas.Add(new ConfigurationData());
            _movements.Add(new Movement(MovementType.MoveAbsJ, new JointTarget(robotJointPosition, externalJointPosition), new SpeedData(5)));
            _planes.Add(_robot.ForwardKinematics.TCPPlane);
            _isInLimits.Add(true);
        }

        /// <summary>
        /// Calculates the path from a list with Actions.
        /// </summary>
        /// <param name="actions"> The list with Actions. </param>
        /// <param name="interpolations"> The amount of interpolations between two targets. </param>
        public void Calculate(IList<Actions.IAction> actions, int interpolations = 5)
        {
            _robot.ForwardKinematics.HideMesh = true;
            _interpolations = interpolations;
            int counter = 0;
            Reset();

            // Ungroup actions
            IList<Actions.IAction> ungrouped = UngroupActions(actions);

            // Check fist movement
            _isFirstMovementMoveAbsJ = CheckFirstMovement(ungrouped);

            // Get path from the list with actions
            for (int i = 0; i < ungrouped.Count; i++)
            {
                if (ungrouped[i] is OverrideRobotTool overrideRobotTool)
                {
                    _currentTool = overrideRobotTool.RobotTool.DuplicateWithoutMesh();
                }

                else if (ungrouped[i] is JointConfigurationControl jointConfigurationControl)
                {
                    _jointConfigurationControl = jointConfigurationControl.IsActive;
                }

                else if (ungrouped[i] is LinearConfigurationControl linearConfigurationControl)
                {
                    _linearConfigurationControl = linearConfigurationControl.IsActive;
                }

                else if (ungrouped[i] is CirclePathMode circlePathMode)
                {
                    _cirPathMode = circlePathMode.Mode;
                }

                else if (ungrouped[i] is WaitTime waitTime)
                {
                    _programTime += waitTime.Duration;
                }

                else if (ungrouped[i] is Movement movement)
                {
                    if (movement.Target is RobotTarget && movement.MovementType == MovementType.MoveAbsJ)
                    {
                        JointMovementFromRobotTarget(movement);
                        counter++;
                    }

                    else if (movement.Target is RobotTarget && movement.MovementType == MovementType.MoveL)
                    {
                        LinearMovementFromRobotTarget(movement);
                        counter++;
                    }

                    else if (movement.Target is RobotTarget && movement.MovementType == MovementType.MoveJ)
                    {
                        JointMovementFromRobotTarget(movement);
                        counter++;
                    }

                    else if (movement.Target is JointTarget && movement.MovementType == MovementType.MoveAbsJ)
                    {
                        JointMovementFromJointTarget(movement);
                        counter++;
                    }

                    else if (movement.Target is RobotTarget && movement.MovementType == MovementType.MoveC)
                    {
                        CircularMovementFromRobotTarget(movement);
                        counter++;
                    }
                }
            }

            // Remove first dummy values
            if (counter > 0)
            {
                _movements.RemoveRange(0, interpolations);
                _robotJointPositions.RemoveRange(0, interpolations);
                _externalJointPositions.RemoveRange(0, interpolations);
                _configurationDatas.RemoveRange(0, interpolations);
                _planes.RemoveRange(0, interpolations);
                _isInLimits.RemoveRange(0, interpolations);
            }

            // Remove first path
            _paths.RemoveAt(0);

            // Remove duplicates from error text
            _errorText = _errorText.Distinct().ToList();
        }

        /// <summary>
        /// Calculates the interpolated path of a joint movement from a Joint Target. 
        /// </summary>
        /// <param name="movement"> The movement with as Target a Joint Target. </param>
        private void JointMovementFromJointTarget(Movement movement)
        {
            // Set the correct tool for this movement
            SetRobotTool(movement);

            // Get the joint target
            JointTarget jointTarget = movement.Target as JointTarget;

            // Get the final joint positions of this movement
            RobotJointPosition towardsRobotJointPosition = jointTarget.RobotJointPosition;
            ExternalJointPosition towardsExternalJointPosition = jointTarget.ExternalJointPosition;

            // Add error text
            _errorText.AddRange(jointTarget.CheckAxisLimits(_robot).ConvertAll(item => string.Copy(item)));

            // Interpolate
            InterpolateJointMovement(towardsRobotJointPosition, towardsExternalJointPosition, movement);
        }

        /// <summary>
        /// Calculates the interpolated path of a joint movement from a Robot Target. 
        /// </summary>
        /// <param name="movement"> The movement with as Target a Robot Target. </param>
        private void JointMovementFromRobotTarget(Movement movement)
        {
            // Set the correct tool for this movement
            SetRobotTool(movement);

            // Get the final joint positions of this movement
            _robot.InverseKinematics.Calculate(movement);

            // Joint configuration control
            if (_jointConfigurationControl == false && movement.MovementType != MovementType.MoveAbsJ)
            {
                _robot.InverseKinematics.CalculateClosestRobotJointPosition(_robotJointPositions.Last(), false);
            }

            // Get the Robot Joint Positions
            RobotJointPosition towardsRobotJointPosition = _robot.InverseKinematics.RobotJointPosition;
            ExternalJointPosition towardsExternalJointPosition = _robot.InverseKinematics.ExternalJointPosition;

            // Add error text
            _errorText.AddRange(_robot.InverseKinematics.ErrorText.ConvertAll(item => string.Copy(item)));

            // Interpolate
            InterpolateJointMovement(towardsRobotJointPosition, towardsExternalJointPosition, movement);
        }

        /// <summary>
        /// Calculates the interpolated path for a linear movement.
        /// </summary>
        /// <param name="movement"> The movement as a linear movement type. </param>
        private void LinearMovementFromRobotTarget(Movement movement)
        {
            // Set the correct tool for this movement
            SetRobotTool(movement);

            // Points for path
            List<Point3d> points = new List<Point3d>() { _planes.Last().Origin };

            // Get the final external joint positions of this movement
            _robot.InverseKinematics.CalculateExternalJointPosition(movement);
            ExternalJointPosition towardsExternalJointPosition = _robot.InverseKinematics.ExternalJointPosition.Duplicate();

            // External Joint Position change
            ExternalJointPosition externalJointPositionChange = CalculateExternalJointPositionChange(towardsExternalJointPosition);

            // First target plane in WORLD coordinate space
            Plane plane1 = _planes.Last();

            // Second target plane in WORK OBJECT coordinate space 
            RobotTarget robotTarget = movement.Target as RobotTarget;
            Plane plane2 = robotTarget.Plane;

            // Correction for rotation of the target plane on a movable work object
            if (movement.WorkObject.ExternalAxis != null)
            {
                IExternalAxis externalAxis = movement.WorkObject.ExternalAxis;
                Transform trans = externalAxis.CalculateTransformationMatrix(_externalJointPositions.Last() * -1, out _);
                plane1.Transform(trans);
            }

            // Re-orient the starting plane to the work object coordinate space of the second target plane
            Transform orient = Transform.ChangeBasis(Plane.WorldXY, movement.WorkObject.GlobalWorkObjectPlane);
            plane1.Transform(orient);

            // Quaternion orientations
            Quaternion quat1 = HelperMethods.PlaneToQuaternion(plane1);
            Quaternion quat2 = HelperMethods.PlaneToQuaternion(plane2);
            quat1.Unitize();
            quat2.Unitize();

            // Linear curve (in work object coordinatie space)
            Line line = new Line(plane1.Origin, plane2.Origin);

            // Normalized increment
            double dt = 1.0 / _interpolations;

            // New movement
            Movement newMovement = movement.DuplicateWithoutMesh();

            // New external joint position
            ExternalJointPosition newExternalJointPosition = _externalJointPositions.Last().Duplicate();

            // Create the sub target planes, robot joint positions and external joint positions for every interpolation step
            for (int i = 0; i < _interpolations; i++)
            {
                // Normalized interpolation parameter
                double t = dt * (i + 1);

                // Interpolate position and orientation
                Point3d point = ((1 - t) * plane1.Origin) + (t * plane2.Origin);
                Quaternion quat = HelperMethods.Slerp(quat1, quat2, t);

                // Plane: the target plane in WORK OBJECT coordinate space
                Plane plane = HelperMethods.QuaternionToPlane(point, quat);

                // Update the external joint position
                newExternalJointPosition.Add(externalJointPositionChange);

                // Update movement
                newMovement.Target = new RobotTarget(robotTarget.Name, plane, robotTarget.ConfigurationData.Duplicate(), newExternalJointPosition);

                // Calculate joint positions
                _robot.InverseKinematics.Calculate(newMovement);

                // Configuration control
                if (_linearConfigurationControl == false)
                {
                    _robot.InverseKinematics.CalculateClosestRobotJointPosition(_robotJointPositions.Last());
                }

                // Check for wrist singularity
                if (Math.Sign(_robot.InverseKinematics.RobotJointPosition[4]) * Math.Sign(_robotJointPositions.Last()[4]) < 0)
                {
                    _errorText.Add($"Movement {movement.Target.Name}\\{movement.WorkObject.Name}: The robot is near a wrist singularity.");
                }

                // Add te calculated joint positions and plane to the class property
                _robotJointPositions.Add(_robot.InverseKinematics.RobotJointPosition.Duplicate());
                _externalJointPositions.Add(_robot.InverseKinematics.ExternalJointPosition.Duplicate());

                // Add error messages (check axis limits)
                _errorText.AddRange(_robot.InverseKinematics.ErrorText.ConvertAll(item => string.Copy(item)));

                // Add the target plane in WORLD coordinate space
                Plane globalPlane = newMovement.GetPosedGlobalTargetPlane();
                _planes.Add(globalPlane);

                // Update configuration data
                if (newMovement.Target is RobotTarget target)
                {
                    target.ConfigurationData = _robot.InverseKinematics.ConfigurationData;
                    _configurationDatas.Add(target.ConfigurationData.Duplicate());
                }

                // Add movement
                _movements.Add(newMovement.DuplicateWithoutMesh());

                // Axis limits check
                _isInLimits.Add(_robot.InverseKinematics.IsInLimits);

                // Only add the other point if this point is different
                if (points[points.Count - 1] != globalPlane.Origin)
                {
                    points.Add(globalPlane.Origin);
                }
            }

            // Generate path curve
            GeneratePathCurve(points);

            // Program time
            if (movement.Time < 0)
            {
                _programTime += line.Length / movement.SpeedData.V_TCP;
            }
            else
            {
                _programTime += movement.Time;
            }
        }

        /// <summary>
        /// Calculates the interpolated path for a circular movement.
        /// </summary>
        /// <param name="movement"> The movement as a linear movement type. </param>
        private void CircularMovementFromRobotTarget(Movement movement)
        {
            // Set the correct tool for this movement
            SetRobotTool(movement);

            // Circular path mode
            CirPathMode cirPathMode = _cirPathMode;

            if (_cirPathMode == CirPathMode.PathFrame)
            {
                _errorText.Insert(0, $"Circular Path Mode \"{_cirPathMode}\" is roughly estimated by the Path Generator. " +
                    "For accurate results, verify your program using Robot Studio, where a precise simulation can be achieved.");

                cirPathMode = CirPathMode.PathFrame;
            }
            else if (_cirPathMode == CirPathMode.ObjectFrame)
            {
                cirPathMode = CirPathMode.ObjectFrame;
            }
            else if (_cirPathMode == CirPathMode.CirPointOri)
            {
                _errorText.Insert(0, $"Circular Path Mode \"{_cirPathMode}\" is roughly estimated by the Path Generator. " +
                    "For accurate results, verify your program using Robot Studio, where a precise simulation can be achieved.");

                cirPathMode = CirPathMode.CirPointOri;
            }
            else
            {
                _errorText.Insert(0, $"Circular Path Mode \"{_cirPathMode}\" is not supported by the Path Generator. " +
                    "\"PathFrame\" mode is used instead. " +
                    "For accurate results, verify your program using Robot Studio, where a precise simulation can be achieved.");

                cirPathMode = CirPathMode.PathFrame;
            }

            // Points for path
            List<Point3d> points = new List<Point3d>() { _planes.Last().Origin };
            List<Point3d> pointsWorkObject = new List<Point3d>() {};

            // Get the final external joint positions of this movement
            _robot.InverseKinematics.CalculateExternalJointPosition(movement);
            ExternalJointPosition towardsExternalJointPosition = _robot.InverseKinematics.ExternalJointPosition.Duplicate();

            // External Joint Position change
            ExternalJointPosition externalJointPositionChange = CalculateExternalJointPositionChange(towardsExternalJointPosition);

            // First target plane in WORLD coordinate space
            Plane plane1 = _planes.Last();

            // Circular point in WORK OBJECT coordinate space
            if (movement.CircularPoint.Plane == Plane.Unset)
            {
                throw new Exception($"Circular Movement {movement.Target.Name}\\{movement.WorkObject.Name}: No circular point defined.");
            }

            Plane planeCirPoint = movement.CircularPoint.Plane;

            // Second target plane in WORK OBJECT coordinate space 
            RobotTarget robotTarget = movement.Target as RobotTarget;
            Plane plane2 = robotTarget.Plane;

            // Correction for rotation of the target plane on a movable work object
            if (movement.WorkObject.ExternalAxis != null)
            {
                IExternalAxis externalAxis = movement.WorkObject.ExternalAxis;
                Transform trans = externalAxis.CalculateTransformationMatrix(_externalJointPositions.Last() * -1, out _);
                plane1.Transform(trans);
            }

            // Re-orient the starting plane to the work object coordinate space of the second target plane
            Transform orient = Transform.ChangeBasis(Plane.WorldXY, movement.WorkObject.GlobalWorkObjectPlane);
            plane1.Transform(orient);

            // Circular curve (in work object coordinatie space)
            Arc arc = new Arc(plane1.Origin, planeCirPoint.Origin, plane2.Origin);

            if (arc.IsValid == false)
            {
                throw new Exception($"Circular Movement {movement.Target.Name}\\{movement.WorkObject.Name}: Arc is not valid. Did you define the circular point correctly?");
            }

            if (arc.AngleDegrees > 240)
            {
                _errorText.Add($"Circular Movement {movement.Target.Name}\\{movement.WorkObject.Name}: Circle is too large (> 240 degrees).");
            }

            Curve circle = arc.ToNurbsCurve();
            circle.Domain = new Interval(0, 1);

            // Check the RAPID conditions
            // Minimum distance between start and ToPoint is 0.1 mm
            if (plane1.Origin.DistanceTo(plane2.Origin) < 0.1)
            {
                _errorText.Add($"Circular Movement {movement.Target.Name}\\{movement.WorkObject.Name}: Distance between the start and end point is smaller than 0.1 mm.");
            }

            // Minimum distance between start and CirPoint is 0.1 mm
            if (plane1.Origin.DistanceTo(planeCirPoint.Origin) < 0.1)
            {
                _errorText.Add($"Circular Movement {movement.Target.Name}\\{movement.WorkObject.Name}: Distance between the start and circular point is smaller than 0.1 mm.");
            }

            // Minimum angle between CirPoint and ToPoint from the start point is 1 degree
            if (Math.Abs(Vector3d.VectorAngle(plane2.Origin - plane1.Origin, planeCirPoint.Origin - plane1.Origin)) < Rhino.RhinoMath.ToRadians(1.0))
            {
                _errorText.Add($"Circular Movement {movement.Target.Name}\\{movement.WorkObject.Name}: The angle between the circular point and start point is smaller than 1 degree.");
            }

            // Circle Path Mode specific restrictions
            if (_cirPathMode == CirPathMode.CirPointOri)
            {
                circle.ClosestPoint(planeCirPoint.Origin, out double param);

                if (param < 0.25 | param > 0.75)
                {
                    _errorText.Add($"Circular Movement {movement.Target.Name}\\{movement.WorkObject.Name}: The circle Point is not between 0.25 and 0.75 of the circle movement which is required for the CirPointOri mode.");
                }
            }

            // Normalized interpolation step
            double dt = 1.0 / _interpolations;

            // Orientation interpolation
            Quaternion quat1 = HelperMethods.PlaneToQuaternion(plane1);
            Quaternion quat2 = HelperMethods.PlaneToQuaternion(plane2);
            quat1.Unitize();
            quat2.Unitize();

            // CirpointOri mode
            circle.ClosestPoint(planeCirPoint.Origin, out double cirPointParam);

            // New movement
            Movement newMovement = movement.DuplicateWithoutMesh();

            // New external joint position
            ExternalJointPosition newExternalJointPosition = _externalJointPositions.Last().Duplicate();

            // Create the sub target planes, robot joint positions and external joint positions for every interpolation step
            for (int i = 0; i < _interpolations; i++)
            {
                // Normalized interpolation
                double t = dt * (i + 1);

                // Interpolate position
                Point3d point = circle.PointAt(t);

                // Interpolate orientation
                Plane plane;

                // PathFrame mode: rotates both planes to the current position and takes the weighted average
                if (cirPathMode == CirPathMode.PathFrame)
                {
                    // Rotates both target planes to the current position. 
                    double angle1 = Vector3d.VectorAngle(arc.StartPoint - arc.Plane.Origin, point - arc.Plane.Origin, arc.Plane);
                    double angle2 = Vector3d.VectorAngle(arc.EndPoint - arc.Plane.Origin, point - arc.Plane.Origin, arc.Plane);
                    Plane plane3 = new Plane(plane1);
                    Plane plane4 = new Plane(plane2);
                    Transform xform1 = Transform.Rotation(angle1, arc.Plane.ZAxis, arc.Plane.Origin);
                    Transform xform2 = Transform.Rotation(angle2, arc.Plane.ZAxis, arc.Plane.Origin);
                    plane3.Transform(xform1);
                    plane4.Transform(xform2);
                    Quaternion quat3 = HelperMethods.PlaneToQuaternion(plane3);
                    Quaternion quat4 = HelperMethods.PlaneToQuaternion(plane4);
                    quat3.Unitize();
                    quat4.Unitize();

                    // Interpolate orientation
                    Quaternion quat = HelperMethods.Slerp(quat3, quat4, t);

                    // Plane: the target plane in WORK OBJECT coordinate space
                    plane = HelperMethods.QuaternionToPlane(point, quat);
                }
                // ObjectFrame mode: Quaternion interpolation (SLERP)
                else if (cirPathMode == CirPathMode.ObjectFrame)
                {
                    // Interpolate orientation
                    Quaternion quat = HelperMethods.Slerp(quat1, quat2, t);

                    // Plane: the target plane in WORK OBJECT coordinate space
                    plane = HelperMethods.QuaternionToPlane(point, quat);
                }
                else if (cirPathMode == CirPathMode.CirPointOri)
                {
                    // Rotates both target planes to the current position. 
                    if (t < cirPointParam)
                    {
                        // Interpolation parameter
                        double param = t / cirPointParam;

                        // Rotates both target planes to the current position. 
                        double angle1 = Vector3d.VectorAngle(arc.StartPoint - arc.Plane.Origin, point - arc.Plane.Origin, arc.Plane);
                        double angle2 = Vector3d.VectorAngle(planeCirPoint.Origin - arc.Plane.Origin, point - arc.Plane.Origin, arc.Plane);
                        Plane plane3 = new Plane(plane1);
                        Plane plane4 = new Plane(planeCirPoint);
                        Transform xform1 = Transform.Rotation(angle1, arc.Plane.ZAxis, arc.Plane.Origin);
                        Transform xform2 = Transform.Rotation(angle2, arc.Plane.ZAxis, arc.Plane.Origin);
                        plane3.Transform(xform1);
                        plane4.Transform(xform2);
                        Quaternion quat3 = HelperMethods.PlaneToQuaternion(plane3);
                        Quaternion quat4 = HelperMethods.PlaneToQuaternion(plane4);
                        quat3.Unitize();
                        quat4.Unitize();

                        // Interpolate orientation
                        Quaternion quat = HelperMethods.Slerp(quat3, quat4, param);

                        // Plane: the target plane in WORK OBJECT coordinate space
                        plane = HelperMethods.QuaternionToPlane(point, quat);
                    }
                    else
                    {
                        // Interpolation parameter
                        double param = (t - cirPointParam) / (1.0 - cirPointParam);

                        // Rotates both target planes to the current position. 
                        double angle1 = Vector3d.VectorAngle(planeCirPoint.Origin - arc.Plane.Origin, point - arc.Plane.Origin, arc.Plane);
                        double angle2 = Vector3d.VectorAngle(arc.EndPoint - arc.Plane.Origin, point - arc.Plane.Origin, arc.Plane);
                        Plane plane3 = new Plane(planeCirPoint);
                        Plane plane4 = new Plane(plane2);
                        Transform xform1 = Transform.Rotation(angle1, arc.Plane.ZAxis, arc.Plane.Origin);
                        Transform xform2 = Transform.Rotation(angle2, arc.Plane.ZAxis, arc.Plane.Origin);
                        plane3.Transform(xform1);
                        plane4.Transform(xform2);
                        Quaternion quat3 = HelperMethods.PlaneToQuaternion(plane3);
                        Quaternion quat4 = HelperMethods.PlaneToQuaternion(plane4);
                        quat3.Unitize();
                        quat4.Unitize();

                        // Interpolate orientation
                        Quaternion quat = HelperMethods.Slerp(quat3, quat4, param);

                        // Plane: the target plane in WORK OBJECT coordinate space
                        plane = HelperMethods.QuaternionToPlane(point, quat);
                    }
                }
                // Not implemented
                else
                {
                    throw new Exception($"Circle Path Mode \"{cirPathMode}\" is not implemented in the path generator.");
                }

                // Update the external joint position
                newExternalJointPosition.Add(externalJointPositionChange);

                // Update movement
                newMovement.Target = new RobotTarget(robotTarget.Name, plane, robotTarget.ConfigurationData, newExternalJointPosition);
                newMovement.CircularPoint.Plane = new Plane(circle.PointAt(dt * (i + 0.5)), newMovement.CircularPoint.Plane.XAxis, newMovement.CircularPoint.Plane.YAxis);

                // Calculate joint positions
                _robot.InverseKinematics.Calculate(newMovement);

                // Check for wrist singularity
                if (Math.Sign(_robot.InverseKinematics.RobotJointPosition[4]) * Math.Sign(_robotJointPositions.Last()[4]) < 0)
                {
                    _errorText.Add($"Movement {movement.Target.Name}\\{movement.WorkObject.Name}: The robot is near a wrist singularity.");
                }

                // Add te calculated joint positions and plane to the class property
                _robotJointPositions.Add(_robot.InverseKinematics.RobotJointPosition.Duplicate());
                _externalJointPositions.Add(_robot.InverseKinematics.ExternalJointPosition.Duplicate());

                // Add error messages (check axis limits)
                _errorText.AddRange(_robot.InverseKinematics.ErrorText.ConvertAll(item => string.Copy(item)));

                // Add the target plane in WORLD coordinate space
                Plane globalPlane = newMovement.GetPosedGlobalTargetPlane();
                _planes.Add(globalPlane);

                // Update configuration data
                if (newMovement.Target is RobotTarget target)
                {
                    target.ConfigurationData = _robot.InverseKinematics.ConfigurationData;
                    _configurationDatas.Add(target.ConfigurationData.Duplicate());
                }

                // Add movement
                _movements.Add(newMovement.DuplicateWithoutMesh());

                // Axis limits check
                _isInLimits.Add(_robot.InverseKinematics.IsInLimits);

                // Only add the other point if this point is different
                if (points[points.Count - 1] != globalPlane.Origin)
                {
                    points.Add(globalPlane.Origin);
                }
            }

            // Generate path curve
            GeneratePathCurve(points);

            // Program time
            if (movement.Time < 0)
            {
                _programTime += arc.Length / movement.SpeedData.V_TCP;
            }
            else
            {
                _programTime += movement.Time;
            }
        }

        /// <summary>
        /// Calculates the interpolated path for a joint movement.
        /// </summary>
        /// <param name="towardsRobotJointPosition"> The final Robot Joint Position of the joint movement. </param>
        /// <param name="towardsExternalJointPosition"> The final External Joint Position of the joint movement. </param>
        /// <param name="movement"> The movement that belongs to the given joint positions. </param>
        private void InterpolateJointMovement(RobotJointPosition towardsRobotJointPosition, ExternalJointPosition towardsExternalJointPosition, Movement movement)
        {
            // Calculate the joint position value change per interpolation
            RobotJointPosition robotJointPositionChange = CalculateRobotJointPositionChange(towardsRobotJointPosition);
            ExternalJointPosition externalJointPositionChange = CalculateExternalJointPositionChange(towardsExternalJointPosition);

            // New joint positions
            RobotJointPosition newRobotJointPosition = _robotJointPositions.Last().Duplicate();
            newRobotJointPosition.Name = towardsRobotJointPosition.Name;
            ExternalJointPosition newExternalJointPosition = _externalJointPositions.Last().Duplicate();
            newExternalJointPosition.Name = towardsExternalJointPosition.Name;

            // Points for path
            List<Point3d> points = new List<Point3d>() { _planes.Last().Origin };

            // Interpolate
            for (int i = 0; i < _interpolations; i++)
            {
                // Joint Positions
                newRobotJointPosition.Add(robotJointPositionChange);
                newExternalJointPosition.Add(externalJointPositionChange);
                _robotJointPositions.Add(newRobotJointPosition.Duplicate());
                _externalJointPositions.Add(newExternalJointPosition.Duplicate());

                // Planes
                _robot.ForwardKinematics.Calculate(newRobotJointPosition, newExternalJointPosition);
                _isInLimits.Add(_robot.ForwardKinematics.IsInLimits);
                _planes.Add(_robot.ForwardKinematics.TCPPlane);

                // Points
                if (points[points.Count - 1] != _robot.ForwardKinematics.TCPPlane.Origin)
                {
                    points.Add(_robot.ForwardKinematics.TCPPlane.Origin);
                }

                // Movement
                Movement newMovement = movement.DuplicateWithoutMesh();

                if (newMovement.MovementType == MovementType.MoveAbsJ && newMovement.Target is RobotTarget robotTarget1)
                {
                    string name = robotTarget1.Name == string.Empty ? string.Empty : $"{robotTarget1.Name}_jt";
                    newMovement.Target = new JointTarget(name, newRobotJointPosition.Duplicate(), newExternalJointPosition.Duplicate());
                    newMovement.Target.ExternalJointPosition.Name = movement.Target.ExternalJointPosition.Name;

                    ConfigurationData configurationData = ConfigurationDataFromRobotJointPosition(robotTarget1.ConfigurationData.Name, newRobotJointPosition, newExternalJointPosition);
                    _configurationDatas.Add(configurationData);
                }
                else if (newMovement.Target is RobotTarget robotTarget2)
                {
                    Plane plane = new Plane(_robot.ForwardKinematics.TCPPlane);

                    // Correction for rotation of the target plane on a movable work object
                    if (newMovement.WorkObject.ExternalAxis != null)
                    {
                        IExternalAxis externalAxis = newMovement.WorkObject.ExternalAxis;
                        Transform trans2 = externalAxis.CalculateTransformationMatrix(newExternalJointPosition * -1, out _);
                        plane.Transform(trans2);
                    }

                    Transform trans = Transform.ChangeBasis(Plane.WorldXY, newMovement.WorkObject.GlobalWorkObjectPlane);
                    plane.Transform(trans);

                    robotTarget2.Plane = plane;
                    robotTarget2.ConfigurationData = ConfigurationDataFromRobotJointPosition(robotTarget2.ConfigurationData.Name, newRobotJointPosition, newExternalJointPosition);
                    robotTarget2.ExternalJointPosition = newExternalJointPosition.Duplicate();
                    robotTarget2.ExternalJointPosition.Name = movement.Target.ExternalJointPosition.Name;
                    newMovement.Target = robotTarget2;

                    _configurationDatas.Add(robotTarget2.ConfigurationData.Duplicate());
                }
                else if (newMovement.Target is JointTarget jointTarget)
                {
                    jointTarget.RobotJointPosition = newRobotJointPosition.Duplicate();
                    jointTarget.ExternalJointPosition = newExternalJointPosition.Duplicate();
                    newMovement.Target = jointTarget;

                    ConfigurationData configurationData = ConfigurationDataFromRobotJointPosition("", newRobotJointPosition, newExternalJointPosition);
                    _configurationDatas.Add(configurationData);
                }

                _movements.Add(newMovement);
            }

            // Generate path curve
            GeneratePathCurve(points);

            // Program time
            if (_paths.Last() != null)
            {
                if (movement.Time < 0)
                {
                    _programTime += _paths.Last().GetLength() / movement.SpeedData.V_TCP;
                }
                else
                {
                    _programTime += movement.Time;
                }
            }
        }

        /// <summary>
        /// Checks whether the first movement type is an absolute joint movement. 
        /// </summary>
        /// <remarks>
        /// Returns true if no movements are defined. 
        /// </remarks>
        /// <returns> 
        /// Specifies whether the first movement type is an absolute joint movement. 
        /// </returns>
        private bool CheckFirstMovement(IList<Actions.IAction> actions)
        {
            for (int i = 0; i != actions.Count; i++)
            {
                if (actions[i] is Movement movement)
                {
                    if (movement.MovementType == MovementType.MoveAbsJ)
                    {
                        return true;
                    }
                    else
                    {
                        _errorText.Add("The first movement is not set as an absolute joint movement.");
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Sets the correct Robot Tool for the defined movement.
        /// </summary>
        /// <param name="movement"> The Movement to set the Robot Tool for. </param>
        private void SetRobotTool(Movement movement)
        {
            if (movement.RobotTool == null || movement.RobotTool.Name == "")
            {
                _robot.Tool = _currentTool;
            }
            else
            {
                _robot.Tool = movement.RobotTool;
            }
        }

        /// <summary>
        /// Returns a list with ungrouped actions. 
        /// </summary>
        /// <param name="actions"> The list with actions to ungroup. </param>
        /// <returns> A list of ungrouped actions. </returns>
        private IList<Actions.IAction> UngroupActions(IList<Actions.IAction> actions)
        {
            // Ungroup actions
            List<Actions.IAction> ungrouped = new List<Actions.IAction>() { };

            for (int i = 0; i < actions.Count; i++)
            {
                if (actions[i] is ActionGroup group)
                {
                    ungrouped.AddRange(group.Ungroup());
                }
                else
                {
                    ungrouped.Add(actions[i]);
                }
            }

            return ungrouped;
        }

        /// <summary>
        /// Calculates the change in robot joint position for interpolation.
        /// </summary>
        /// <param name="towardsRobotJointPosition"> The final robot joint position of the movement. </param>
        /// <returns> The change in robot joint position per interpolation step. </returns>
        private RobotJointPosition CalculateRobotJointPositionChange(RobotJointPosition towardsRobotJointPosition)
        {
            RobotJointPosition robotJointPositionChange = towardsRobotJointPosition.Duplicate();
            robotJointPositionChange.Subtract(_robotJointPositions.Last());
            robotJointPositionChange.Divide(_interpolations);

            return robotJointPositionChange;
        }

        /// <summary>
        /// Calculates the change in external joint position for interpolation.
        /// </summary>
        /// <param name="towardsExternalJointPosition"> The final external joint position of the movement. </param>
        /// <returns> The change in external joint position per interpolation step. </returns>
        private ExternalJointPosition CalculateExternalJointPositionChange(ExternalJointPosition towardsExternalJointPosition)
        {
            ExternalJointPosition externalJointPositionChange = towardsExternalJointPosition.Duplicate();
            externalJointPositionChange.Subtract(_externalJointPositions.Last());
            externalJointPositionChange.Divide(_interpolations);

            return externalJointPositionChange;
        }

        /// <summary>
        /// Generates a path curve based on the provided points and movement.
        /// </summary>
        /// <param name="points"> The list of points to create the path curve from. </param>
        private void GeneratePathCurve(List<Point3d> points)
        {
            if (points.Count > 1)
            {
                _paths.Add(Curve.CreateInterpolatedCurve(points, 3));
            }
            else
            {
                _paths.Add(null);
            }
        }

        /// <summary>
        /// Computes the configuration data for a given robot joint position and external joint position.
        /// </summary>
        /// <param name="name">The name of the configuration data being generated.</param>
        /// <param name="robotJointPosition">The robot joint position used for configuration data computation.</param>
        /// <param name="externalJointPosition">The external joint position used for configuration data computation.</param>
        /// <returns>
        /// A <see cref="ConfigurationData"/> object containing the calculated configuration data, including the configuration flags for the robot's joints.
        /// </returns>
        /// <remarks>
        /// This method uses forward kinematics (FK) and inverse kinematics (IK) calculations to determine 
        /// the robot's configuration data based on its joint positions. It identifies the closest matching 
        /// configuration using the minimum norm between the normalized joint positions and the IK solutions.
        /// </remarks>
        private ConfigurationData ConfigurationDataFromRobotJointPosition(string name, RobotJointPosition robotJointPosition, ExternalJointPosition externalJointPosition)
        {
            _robot.ForwardKinematics.Calculate(robotJointPosition, externalJointPosition);
            _robot.InverseKinematics.Calculate(new Movement(new RobotTarget("", _robot.ForwardKinematics.TCPPlane, new ConfigurationData(), externalJointPosition)));

            int cf1 = (int)Math.Floor(robotJointPosition[0] / 90);
            int cf4 = (int)Math.Floor(robotJointPosition[3] / 90);
            int cf6 = (int)Math.Floor(robotJointPosition[5] / 90);
            int cfx = 0;

            double error = double.MaxValue;

            RobotJointPosition duplicate = robotJointPosition.Duplicate();
            duplicate.Normalize();

            for (int i = 0; i < 8; i++)
            {
                RobotJointPosition normalized = _robot.InverseKinematics.RobotJointPositions[i].Duplicate();
                normalized.Normalize();

                double norm = duplicate.Norm(normalized);

                if (norm < error)
                {
                    error = norm;
                    cfx = i;

                    if (norm < 1e-3)
                    {
                        break;
                    }
                }
            }

            return new ConfigurationData(name, cf1, cf4, cf6, cfx);
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
                return true;
            }
        }

        /// <summary>
        /// Gets the Robot.
        /// </summary>
        public Robot Robot
        {
            get { return _robot; }
        }

        /// <summary>
        /// Gets the list with TCP planes the path follows.
        /// </summary>
        public List<Plane> Planes
        {
            get { return _planes; }
        }

        /// <summary>
        /// Gets the path curve as list with curve.
        /// </summary>
        /// <remarks>
        /// For every move instruction a curve is constructed. 
        /// </remarks>
        public List<Curve> Paths
        {
            get { return _paths; }
        }

        /// <summary>
        /// Gets the calculated Movements.
        /// </summary>
        public List<Movement> Movements
        {
            get { return _movements; }
        }

        /// <summary>
        /// Gets the calculated Robot Joint Positions.
        /// </summary>
        public List<RobotJointPosition> RobotJointPositions
        {
            get { return _robotJointPositions; }
        }

        /// <summary>
        /// Gets the calculated External Joint Positions. 
        /// </summary>
        public List<ExternalJointPosition> ExternalJointPositions
        {
            get { return _externalJointPositions; }
        }

        /// <summary>
        /// Gets the calculated Configuration Datas.
        /// </summary>
        public List<ConfigurationData> ConfigurationDatas
        { 
            get { return _configurationDatas; } 
        }

        /// <summary>
        /// Gets the value indicating whether or not the internal and external values are within their limits.
        /// </summary>
        public List<bool> IsInLimits
        {
            get { return _isInLimits; }
        }

        /// <summary>
        /// Gets the collected error messages.
        /// </summary>
        public List<string> ErrorText
        {
            get { return _errorText; }
        }

        /// <summary>
        /// Gets a value indicating whether or not the first movement is an Absolute Joint Movement.
        /// </summary>
        public bool IsFirstMovementMoveAbsJ
        {
            get { return _isFirstMovementMoveAbsJ; }
        }

        /// <summary>
        /// Gets an estimation of the program time in seconds. 
        /// </summary>
        public double ProgramTime
        {
            get { return _programTime; }
        }
        #endregion
    }
}
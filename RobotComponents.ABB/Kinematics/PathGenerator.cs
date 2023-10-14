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
    /// This class is used to approximate of the path the Robot will follow for a given set of Actions. 
    /// Speed Datas and Zone Datas are neglected. 
    /// </summary>
    public class PathGenerator
    {
        #region fields
        private readonly Robot _robot; // The robot info to construct the path for
        private readonly List<Plane> _planes; // The planes the path follow
        private readonly List<Curve> _paths; // The path curves between two movement targets
        private readonly List<Movement> _movements; // The movements of the path
        private readonly List<RobotJointPosition> _robotJointPositions; // The robot joint positions needed to follow the path
        private readonly List<ExternalJointPosition> _externalJointPositions; // The external joint position needed to follow the path
        private readonly List<bool> _inLimits; // Indicates whether or not the joint positions are within their limits
        private List<string> _errorText = new List<string>(); // List with collected error messages

        private bool _firstMovementIsMoveAbsJ; // Bool that indicates if the first movemement is an absolute joint movement
        private readonly RobotTool _initialTool; // Defines the first tool that will be used
        private RobotTool _currentTool; // Defines the default robot tool
        private bool _linearConfigurationControl; // Defines if the configuration control for linear movements is enabled
        private bool _jointConfigurationControl; // Defines if the configuration control for joint movements is enabled
        private int _interpolations; // Defines the number of interpolations between two targets
        private double _time; // Estimate of the total program time
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
            _inLimits = new List<bool>();
            _robot = robot.Duplicate(); // Since we might swap tools and therefore change the robot tool we make a deep copy
            _initialTool = robot.Tool.DuplicateWithoutMesh();
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
            _planes.Clear();
            _paths.Clear();
            _inLimits.Clear();
            _errorText.Clear();

            // Reinitiate starting values
            _currentTool = _initialTool;
            _linearConfigurationControl = true;
            _jointConfigurationControl = true;
            _firstMovementIsMoveAbsJ = false;
            _time = 0;

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
            _movements.Add(new Movement(MovementType.MoveAbsJ, new JointTarget(robotJointPosition, externalJointPosition), new SpeedData(5)));
            _planes.Add(_robot.ForwardKinematics.TCPPlane);
            _inLimits.Add(true);
        }

        /// <summary>
        /// Calculates the path from a list with Actions.
        /// </summary>
        /// <param name="actions"> The list with Actions. </param>
        /// <param name="interpolations"> The amount of interpolations between two targets. </param>
        public void Calculate(IList<Actions.Action> actions, int interpolations)
        {
            _robot.ForwardKinematics.HideMesh = true;
            _interpolations = interpolations;
            int counter = 0;
            Reset();

            // Ungroup actions
            List<Actions.Action> ungrouped = new List<Actions.Action>() { };

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

            // Check fist movement
            _firstMovementIsMoveAbsJ = CheckFirstMovement(ungrouped);

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

                else if (ungrouped[i] is WaitTime waitTime)
                {
                    _time += waitTime.Duration;
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
                }
            }

            // Remove first dummy values
            if (counter > 0)
            {
                _movements.RemoveRange(0, interpolations);
                _robotJointPositions.RemoveRange(0, interpolations);
                _externalJointPositions.RemoveRange(0, interpolations);
                _planes.RemoveRange(0, interpolations);
                _inLimits.RemoveRange(0, interpolations);
            }

            // Set first path as null
            _paths.RemoveAt(0);
            
            // Remove duplicates from error text
            _errorText = _errorText.Distinct().ToList();
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
            _robot.InverseKinematics.Movement = movement;
            _robot.InverseKinematics.Calculate();

            // Joint configuration control
            if (_jointConfigurationControl == false && movement.MovementType != MovementType.MoveAbsJ)
            {
                _robot.InverseKinematics.CalculateClosestRobotJointPosition(_robotJointPositions.Last());
            }

            // Get the Robot Joint Positions
            RobotJointPosition towardsRobotJointPosition = _robot.InverseKinematics.RobotJointPosition;
            ExternalJointPosition towardsExternalJointPosition = _robot.InverseKinematics.ExternalJointPosition;

            // Add error text
            _errorText.AddRange(_robot.InverseKinematics.ErrorText.ConvertAll(item => string.Copy(item)));

            // Interpolate
            InterpolateJointMovement(towardsRobotJointPosition, towardsExternalJointPosition, movement, _robot.InverseKinematics.InLimits);
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
            List<Point3d> points = new List<Point3d>() { _planes.Last().Origin};

            // Get the final external joint positions of this movement
            _robot.InverseKinematics.Movement = movement;
            _robot.InverseKinematics.CalculateExternalJointPosition();
            ExternalJointPosition towardsExternalJointPosition = _robot.InverseKinematics.ExternalJointPosition.Duplicate();

            // External Joint Position change
            ExternalJointPosition externalJointPositionChange = towardsExternalJointPosition.Duplicate();
            externalJointPositionChange.Substract(_externalJointPositions.Last());
            externalJointPositionChange.Divide(_interpolations);
                                          
            // First target plane in WORLD coordinate space
            Plane plane1 = _planes.Last();

            // Second target plane in WORK OBJECT coordinate space 
            RobotTarget robotTarget = movement.Target as RobotTarget;
            Plane plane2 = robotTarget.Plane; 

            // Correction for rotation of the target plane on a movable work object
            if (movement.WorkObject.ExternalAxis != null)
            {
                ExternalAxis externalAxis = movement.WorkObject.ExternalAxis;
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

            // Normalized increment
            double dt = (double)1 / _interpolations;

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
                Point3d point = (1 - t) * plane1.Origin + t * plane2.Origin;
                Quaternion quat = HelperMethods.Slerp(quat1, quat2, t);

                // Plane: the target plane in WORK OBJECT coordinate space
                Plane plane = HelperMethods.QuaternionToPlane(point, quat);

                // Update the external joint position
                newExternalJointPosition.Add(externalJointPositionChange);

                // Update movement
                newMovement.Target = new RobotTarget(robotTarget.Name, plane, robotTarget.AxisConfig, newExternalJointPosition);

                // Calculate joint positions
                _robot.InverseKinematics.Movement = newMovement;
                _robot.InverseKinematics.Calculate();

                // Configuration control
                if (_linearConfigurationControl == false)
                {
                    _robot.InverseKinematics.CalculateClosestRobotJointPosition(_robotJointPositions.Last());
                }
                
                // Check for wrist singularity
                if (Math.Sign(_robot.InverseKinematics.RobotJointPosition[4]) * Math.Sign(_robotJointPositions.Last()[4]) < 0)
                { 
                    _errorText.Add($"Movement {movement.Target.Name}\\{movement.WorkObject.Name}: The target is close to wrist singularity.");
                }
                
                // Add te calculated joint positions and plane to the class property
                _robotJointPositions.Add(_robot.InverseKinematics.RobotJointPosition.Duplicate());
                _externalJointPositions.Add(_robot.InverseKinematics.ExternalJointPosition.Duplicate());

                // Add error messages (check axis limits)
                _errorText.AddRange(_robot.InverseKinematics.ErrorText.ConvertAll(item => string.Copy(item)));

                // Add the target plane in WORLD coordinate space
                Plane globalPlane = newMovement.GetPosedGlobalTargetPlane();
                _planes.Add(globalPlane);

                // Add movement
                _movements.Add(newMovement.DuplicateWithoutMesh());

                // Axis limits check
                _inLimits.Add(_robot.InverseKinematics.InLimits);

                // Only add the other point if this point is different
                if (points[points.Count - 1] != globalPlane.Origin)
                {
                    points.Add(globalPlane.Origin);
                }
            }

            // Generate path curve
            if (points.Count > 1)
            {
                _paths.Add(Curve.CreateInterpolatedCurve(points, 3));

                if (movement.Time < 0)
                {
                    _time += _paths[_paths.Count - 1].GetLength() / movement.SpeedData.V_TCP;
                }
                else
                {
                    _time += movement.Time;
                }    
            }
            else
            {
                _paths.Add(null);
            }
        }

        /// <summary>
        /// Calculates the interpolated path for a joint movement.
        /// </summary>
        /// <param name="towardsRobotJointPosition"> The final Robot Joint Position of the joint movement. </param>
        /// <param name="towardsExternalJointPosition"> The final External Joint Position of the joint movement. </param>
        /// <param name="movement"> The movement that belongs to the given joint positions. </param>
        /// <param name="inLimits"> Indicates whether or not the calculated solution is within limits </param>
        private void InterpolateJointMovement(RobotJointPosition towardsRobotJointPosition, ExternalJointPosition towardsExternalJointPosition, Movement movement, bool inLimits = true)
        {
            // Calculate the joint position value change per interpolation
            RobotJointPosition robotJointPositionChange = towardsRobotJointPosition.Duplicate();
            robotJointPositionChange.Substract(_robotJointPositions.Last());
            robotJointPositionChange.Divide(_interpolations);
            ExternalJointPosition externalJointPositionChange = towardsExternalJointPosition.Duplicate();
            externalJointPositionChange.Substract(_externalJointPositions.Last());
            externalJointPositionChange.Divide(_interpolations);

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
                _inLimits.Add(!_robot.ForwardKinematics.InLimits | !inLimits ? false : true);
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
                }
                else if (newMovement.Target is RobotTarget robotTarget2)
                {
                    Plane plane = new Plane(_robot.ForwardKinematics.TCPPlane);

                    // Correction for rotation of the target plane on a movable work object
                    if (newMovement.WorkObject.ExternalAxis != null)
                    {
                        ExternalAxis externalAxis = newMovement.WorkObject.ExternalAxis;
                        Transform trans2 = externalAxis.CalculateTransformationMatrix(newExternalJointPosition * -1, out _);
                        plane.Transform(trans2);
                    }

                    Transform trans = Transform.ChangeBasis(Plane.WorldXY, newMovement.WorkObject.GlobalWorkObjectPlane);
                    plane.Transform(trans);

                    robotTarget2.Plane = plane;
                    robotTarget2.ExternalJointPosition = newExternalJointPosition.Duplicate();
                    robotTarget2.ExternalJointPosition.Name = movement.Target.ExternalJointPosition.Name;
                    newMovement.Target = robotTarget2;
                }
                else if (newMovement.Target is JointTarget jointTarget)
                {
                    jointTarget.RobotJointPosition = newRobotJointPosition.Duplicate();
                    jointTarget.ExternalJointPosition = newExternalJointPosition.Duplicate();
                    newMovement.Target = jointTarget;
                }

                _movements.Add(newMovement);
            }

            // Generate path curve
            if (points.Count > 1)
            {
                _paths.Add(Curve.CreateInterpolatedCurve(points, 3));

                if (movement.Time < 0)
                {
                    _time += _paths[_paths.Count - 1].GetLength() / movement.SpeedData.V_TCP;
                }
                else
                {
                    _time += movement.Time;
                }
            }
            else
            {
                _paths.Add(null);
            }
        }

        /// <summary>
        /// Checks whether the first movement type is an absolute joint movement.
        /// Returns true if no movements were defined. 
        /// </summary>
        /// <returns> Specifies whether the first movement type is an absolute joint movement. </returns>
        private bool CheckFirstMovement(IList<Actions.Action> actions)
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
        /// Gets or sets the Robot.
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
        /// For every move instruction a curve is constructed. 
        /// </summary>
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
        /// Gets the calculated Robot Joint Position.
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
        /// Gets the value indicating whether or not the internal and external values are within their limits.
        /// </summary>
        public List<bool> InLimits
        {
            get { return _inLimits; }
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
        public bool FirstMovementIsMoveAbsJ
        {
            get { return _firstMovementIsMoveAbsJ; }
        }
        #endregion
    }

}
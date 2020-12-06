// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System.Collections.Generic;
using System.Linq;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.Actions;
using RobotComponents.Definitions;
using RobotComponents.Enumerations;

namespace RobotComponents.Kinematics
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
        private readonly List<RobotJointPosition> _robotJointPositions; // The robot joint positions needed to follow the path
        private readonly List<ExternalJointPosition> _externalJointPositions; // The external joint position needed to follow the path
        private List<string> _errorText = new List<string>(); // List with collected error messages

        private bool _firstMovementIsMoveAbsJ; // Bool that indicates if the first movemement is an absolute joint movement
        private readonly RobotTool _initialTool; // Defines the first tool that will be used
        private RobotJointPosition _lastRobotJointPosition; // Defines the last Robot Joint Position
        private ExternalJointPosition _lastExternalJointPosition; // Defines the last External Joint Position
        private RobotTool _currentTool; // Defines the default robot tool
        private bool _linearConfigurationControl; // Defines if the configuration control for linear movements is enabled
        private bool _jointConfigurationControl; // Defines if the configuration control for joint movements is enabled
        private int _interpolations; // Defines the number of interpolations between two targets
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
            _robotJointPositions = new List<RobotJointPosition>();
            _externalJointPositions = new List<ExternalJointPosition>();
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
            if (!this.IsValid)
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
            _robotJointPositions.Clear();
            _externalJointPositions.Clear();
            _planes.Clear();
            _paths.Clear();
            _errorText.Clear();

            // Reinitiate starting values
            _currentTool = _initialTool;
            _linearConfigurationControl = true;
            _jointConfigurationControl = true;
            _firstMovementIsMoveAbsJ = false;
            _lastRobotJointPosition = new RobotJointPosition();
            _lastExternalJointPosition = new ExternalJointPosition();

            for (int i = 0; i < _robot.ExternalAxes.Count; i++)
            {
                _lastExternalJointPosition[_robot.ExternalAxes[i].AxisNumber] = _robot.ExternalAxes[i].AxisLimits.Min;
            }
        }

        /// <summary>
        /// Calculates the path from a list with Actions.
        /// </summary>
        /// <param name="actions"> The list with Actions. </param>
        /// <param name="interpolations"> The amount of interpolatins between two targets. </param>
        public void Calculate(List<Actions.Action> actions, int interpolations)
        {
            _robot.ForwardKinematics.HideMesh = true;
            _interpolations = interpolations;
            int counter = 0;
            Reset();

            // Check fist movement
            _firstMovementIsMoveAbsJ = CheckFirstMovement(actions);

            // Get path from the list with actions
            for (int i = 0; i < actions.Count; i++)
            {
                if (actions[i] is OverrideRobotTool overrideRobotTool)
                {
                    _currentTool = overrideRobotTool.RobotTool.DuplicateWithoutMesh();
                }

                else if (actions[i] is AutoAxisConfig autoAxisConfig)
                {
                    _linearConfigurationControl = !autoAxisConfig.IsActive;
                    _jointConfigurationControl = !autoAxisConfig.IsActive;
                }

                else if (actions[i] is Movement movement)
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

                // OBSOLETE
                else if (actions[i] is AbsoluteJointMovement absoluteJointMovement)
                {
                    JointMovementFromJointTarget(absoluteJointMovement.ConvertToMovement());
                    counter++;
                }
            }

            // Add joint positions and plane from last movement
            if (counter > 0)
            {
                _robotJointPositions.Add(_lastRobotJointPosition);
                _externalJointPositions.Add(_lastExternalJointPosition);
                _robot.ForwardKinematics.Calculate(_lastRobotJointPosition, _lastExternalJointPosition);
                _planes.Add(_robot.ForwardKinematics.TCPPlane);
            }

            // Remove first dummy values
            if (counter > 0)
            {
                _robotJointPositions.RemoveRange(0, interpolations);
                _externalJointPositions.RemoveRange(0, interpolations);
                _planes.RemoveRange(0, interpolations);
                _paths.RemoveAt(0);
            }

            // Remove null
            _paths.RemoveAll(val => val == null);

            // Remove duplicates from error text
            _errorText = _errorText.Distinct().ToList();
        }

        /// <summary>
        /// Sets the correct Robot Tool for the defined movement.
        /// </summary>
        /// <param name="movement"> The Movement to set the Robot Tool for. </param>
        private void SetRobotTool(Movement movement)
        {
            if (movement.RobotTool == null)
            {
                _robot.Tool = _currentTool;
            }
            else if (movement.RobotTool.Name == "")
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
            InterpolateJointMovement(towardsRobotJointPosition, towardsExternalJointPosition);
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

            // Auto Axis Config
            if (_jointConfigurationControl == false && movement.MovementType != MovementType.MoveAbsJ)
            {
                _robot.InverseKinematics.CalculateClosestRobotJointPosition(_lastRobotJointPosition);
            }

            // Get the Robot Joint Positions
            RobotJointPosition towardsRobotJointPosition = _robot.InverseKinematics.RobotJointPosition.Duplicate();
            ExternalJointPosition towardsExternalJointPosition = _robot.InverseKinematics.ExternalJointPosition.Duplicate();

            // Add error text
            _errorText.AddRange(_robot.InverseKinematics.ErrorText.ConvertAll(item => string.Copy(item)));

            // Interpolate
            InterpolateJointMovement(towardsRobotJointPosition, towardsExternalJointPosition);
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
            List<Point3d> points = new List<Point3d>();

            // Get the final joint positions of this movement
            _robot.InverseKinematics.Movement = movement;
            _robot.InverseKinematics.CalculateExternalJointPosition();

            // Get the External Joint Positions
            ExternalJointPosition towardsExternalJointPosition = _robot.InverseKinematics.ExternalJointPosition.Duplicate();

            // External Joint Position change
            ExternalJointPosition externalJointPositionChange = (towardsExternalJointPosition - _lastExternalJointPosition) / _interpolations;

            // TODO: Check with last movement to speed up the process? As in old path generator?
                                          
            // First target plane in WORLD coordinate space
            _robot.ForwardKinematics.Calculate(_lastRobotJointPosition, _lastExternalJointPosition);
            Plane plane1 = _robot.ForwardKinematics.TCPPlane;

            // Second target plane in WORK OBJECT coordinate space 
            RobotTarget robotTarget = movement.Target as RobotTarget;
            Plane plane2 = robotTarget.Plane; 

            // Correction for rotation of the target plane on a movable work object
            if (movement.WorkObject.ExternalAxis != null)
            {
                ExternalAxis externalAxis = movement.WorkObject.ExternalAxis;
                Transform trans = externalAxis.CalculateTransformationMatrix(_lastExternalJointPosition * -1, out _);
                plane1.Transform(trans);
            }

            // Re-orient the starting plane to the work object coordinate space of the second target plane
            Plane globalWorkObjectPlane = new Plane(movement.WorkObject.GlobalWorkObjectPlane);
            Transform orient = Transform.ChangeBasis(Plane.WorldXY, globalWorkObjectPlane);
            plane1.Transform(orient);

            // Target plane position and orientation change per interpolation step
            Vector3d posChange = (plane2.Origin - plane1.Origin) / _interpolations;
            Vector3d xAxisChange = (plane2.XAxis - plane1.XAxis) / _interpolations;
            Vector3d yAxisChange = (plane2.YAxis - plane1.YAxis) / _interpolations;

            // Correct axis configuration, tool and work object
            RobotTarget subTarget = new RobotTarget(robotTarget.Name, Plane.WorldXY, robotTarget.AxisConfig);
            Movement subMovement = new Movement(subTarget); 
            subMovement.RobotTool = movement.RobotTool;
            subMovement.WorkObject = movement.WorkObject;

            // New external joint position
            ExternalJointPosition newExternalJointPosition = _lastExternalJointPosition;

            // Create the sub target planes, robot joint positions and external joint positions for every interpolation step
            for (int i = 0; i < _interpolations; i++)
            {
                // Create new plane: the local target plane (in work object coordinate space)
                Plane plane = new Plane(plane1.Origin + posChange * i, plane1.XAxis + xAxisChange * i, plane1.YAxis + yAxisChange * i);

                // Update the target and movement
                subTarget.Plane = plane;
                subTarget.ExternalJointPosition = newExternalJointPosition;
                subMovement.Target = subTarget;

                // Calculate joint positions
                _robot.InverseKinematics.Movement = subMovement;
                _robot.InverseKinematics.Calculate();

                // Auto Axis Config
                if (_linearConfigurationControl == false)
                {
                    if (i == 0)
                    {
                        _robot.InverseKinematics.CalculateClosestRobotJointPosition(_lastRobotJointPosition);
                    }
                    else
                    {
                        _robot.InverseKinematics.CalculateClosestRobotJointPosition(_robotJointPositions.Last());
                    }
                }

                // Add te calculated joint positions and plane to the class property
                _robotJointPositions.Add(_robot.InverseKinematics.RobotJointPosition.Duplicate());
                _externalJointPositions.Add(_robot.InverseKinematics.ExternalJointPosition.Duplicate());

                // Add error messages (check axis limits)
                _errorText.AddRange(_robot.InverseKinematics.ErrorText.ConvertAll(item => string.Copy(item)));

                // Add the plane
                Plane globalPlane = subMovement.GetPosedGlobalTargetPlane();
                _planes.Add(globalPlane);

                // Always add the first point to list with paths
                if (i == 0)
                {
                    points.Add(new Point3d(globalPlane.Origin));
                }

                // Only add the other point if this point is different
                else if (points[points.Count - 1] != globalPlane.Origin)
                {
                    points.Add(new Point3d(globalPlane.Origin));
                }

                // Update the external joint position
                newExternalJointPosition += externalJointPositionChange;
            }

            // Add last point
            Point3d lastPoint = movement.GetPosedGlobalTargetPlane().Origin;
            if (points[points.Count - 1] != lastPoint)
            {
                points.Add(lastPoint);
            }

            // Generate path curve
            if (points.Count > 1)
            {
                _paths.Add(Curve.CreateInterpolatedCurve(points, 3));
            }
            else
            {
                _paths.Add(null);
            }

            // Get the final joint positions of this movement
            _robot.InverseKinematics.Movement = movement;
            _robot.InverseKinematics.Calculate();

            // Auto Axis Config
            if (_linearConfigurationControl == false)
            {
                _robot.InverseKinematics.CalculateClosestRobotJointPosition(_robotJointPositions.Last());
            }

            // Add error messages (check axis limits)
            _errorText.AddRange(_robot.InverseKinematics.ErrorText.ConvertAll(item => string.Copy(item)));

            // Add last Joint Poistions
            _lastRobotJointPosition = _robot.InverseKinematics.RobotJointPosition.Duplicate();
            _lastExternalJointPosition = _robot.InverseKinematics.ExternalJointPosition.Duplicate();
        }

        /// <summary>
        /// Calculates the interpolated path for a joint movement.
        /// </summary>
        /// <param name="towardsRobotJointPosition"> The final Robot Joint Position of the joint movement. </param>
        /// <param name="towardsExternalJointPosition"> The final External Joint Position of the joint movement. </param>
        private void InterpolateJointMovement(RobotJointPosition towardsRobotJointPosition, ExternalJointPosition towardsExternalJointPosition)
        {
            // Calculate the joint position value change per interpolation
            RobotJointPosition robotJointPositionChange = (towardsRobotJointPosition - _lastRobotJointPosition) / _interpolations;
            ExternalJointPosition externalJointPositionChange = (towardsExternalJointPosition - _lastExternalJointPosition) / _interpolations;

            // Points for path
            List<Point3d> points = new List<Point3d>();

            // New joint positions
            RobotJointPosition newRobotJointPosition = _lastRobotJointPosition;
            ExternalJointPosition newExternalJointPosition = _lastExternalJointPosition;

            // Interpolate
            for (int i = 0; i < _interpolations; i++)
            {
                _robotJointPositions.Add(newRobotJointPosition.Duplicate());
                _externalJointPositions.Add(newExternalJointPosition.Duplicate());

                _robot.ForwardKinematics.Calculate(newRobotJointPosition, newExternalJointPosition);
                _planes.Add(_robot.ForwardKinematics.TCPPlane);

                if (i == 0) 
                { 
                    points.Add(new Point3d(_robot.ForwardKinematics.TCPPlane.Origin)); 
                }
                else if (points[points.Count - 1] != _robot.ForwardKinematics.TCPPlane.Origin) 
                { 
                    points.Add(new Point3d(_robot.ForwardKinematics.TCPPlane.Origin)); 
                }

                newRobotJointPosition += robotJointPositionChange;
                newExternalJointPosition += externalJointPositionChange;
            }

            // Add last point
            _robot.ForwardKinematics.Calculate(towardsRobotJointPosition, towardsExternalJointPosition);
            if (points[points.Count - 1] != _robot.ForwardKinematics.TCPPlane.Origin) 
            { 
                points.Add(_robot.ForwardKinematics.TCPPlane.Origin); 
            }

            // Generate path curve
            if (points.Count > 1)
            {
                _paths.Add(Curve.CreateInterpolatedCurve(points, 3));
            }
            else
            {
                _paths.Add(null);
            }

            // Set last joint positions
            _lastRobotJointPosition = towardsRobotJointPosition;
            _lastExternalJointPosition = towardsExternalJointPosition;
        }

        /// <summary>
        /// Checks whether the first movement type is an absolute joint movement.
        /// </summary>
        /// <returns> Specifies whether the first movement type is an absolute joint movement. </returns>
        private bool CheckFirstMovement(List<Action> actions)
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

                else if (actions[i] is AbsoluteJointMovement)
                {
                    return true;
                }
            }

            // Returns true if no movements were defined
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
                if (Robot == null) { return false; }
                if (Robot.IsValid == false) { return false; }
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
        /// Gets the latest calculated Robot Joint Position.
        /// </summary>
        public List<RobotJointPosition> RobotJointPositions
        {
            get { return _robotJointPositions; }
        }

        /// <summary>
        /// Gets the latest calculated External Joint Positions. 
        /// </summary>
        public List<ExternalJointPosition> ExternalJointPositions
        {
            get { return _externalJointPositions; }
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





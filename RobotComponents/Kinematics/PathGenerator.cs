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

namespace RobotComponents.Kinematics
{
    /// <summary>
    /// Path Generator class. This class does an approximation of the path the robot will follow.
    /// </summary>
    public class PathGenerator
    {
        #region fields
        private readonly Robot _robotInfo; // The robot info to construct the path for
        private readonly List<Plane> _planes; // The planes the path follow
        private readonly List<Curve> _paths; // The path curves between two movement targets
        private readonly List<RobotJointPosition> _robotJointPositions; // The robot joint positions needed to follow the path
        private readonly List<ExternalJointPosition> _externalJointPositions; // The external joint position needed to follow the path
        private readonly List<string> _errorText = new List<string>(); // List with collected error messages

        private readonly RobotTool _initialTool; // Defines the first tool that will be used
        private Movement _lastMovement; // Defines the last movement
        private RobotJointPosition _lastRobotJointPosition; // Defines the last Robot Joint Position
        private ExternalJointPosition _lastExternalJointPosition; // Defines the last External Joint Position
        private RobotTool _currentTool; // Defines the default robot tool
        private bool _autoAxisConfig; // Defines if hte auto axis configuration is active
        private int _interpolations; // Defines the number of interpolations between two targets
        #endregion

        #region constructors
        /// <summary>
        /// Cosntruct an empty path generator object
        /// </summary>
        public PathGenerator()
        {
        }

        /// <summary>
        /// Defines a path generator object
        /// </summary>
        /// <param name="robotInfo"> The robot info to construct the path for. </param>
        public PathGenerator(Robot robotInfo)
        {
            _planes = new List<Plane>();
            _paths = new List<Curve>();
            _robotJointPositions = new List<RobotJointPosition>();
            _externalJointPositions = new List<ExternalJointPosition>();
            _robotInfo = robotInfo.Duplicate(); // Since we might swap tools and therefore change the robot tool we make a deep copy
            _initialTool = robotInfo.Tool.DuplicateWithoutMesh();
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
        /// Resets / clears all lists with values (planes and axis values)
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
            _autoAxisConfig = false;
            _lastRobotJointPosition = new RobotJointPosition();
            _lastExternalJointPosition = new ExternalJointPosition();

            for (int i = 0; i < _robotInfo.ExternalAxis.Count; i++)
            {
                _lastExternalJointPosition[(int)_robotInfo.ExternalAxis[i].AxisNumber] = _robotInfo.ExternalAxis[i].AxisLimits.Min;
            }
        }

        /// <summary>
        /// Calculates the path from a list with Actions
        /// </summary>
        /// <param name="actions"> The list with Actions to calculate the path. </param>
        /// <param name="interpolations"> The amount of interpolatins between two targets. </param>
        public void Calculate(List<Actions.Action> actions, int interpolations)
        {
            _robotInfo.ForwardKinematics.HideMesh = true;
            _interpolations = interpolations;
            int counter = 0;
            Reset();

            // Get path from the list with actions
            for (int i = 0; i < actions.Count; i++)
            {
                if (actions[i] is OverrideRobotTool overrideRobotTool)
                {
                    _currentTool = overrideRobotTool.RobotTool.DuplicateWithoutMesh();
                }

                else if (actions[i] is AutoAxisConfig autoAxisConfig)
                {
                    _autoAxisConfig = autoAxisConfig.IsActive;
                }

                else if (actions[i] is AbsoluteJointMovement absoluteJointMovement)
                {
                    JointMovementFromJointTarget(absoluteJointMovement.ConvertToMovement());
                    _lastMovement = absoluteJointMovement.ConvertToMovement();
                    counter++;
                }

                else if (actions[i] is Movement movement)
                {
                    if (movement.Target is RobotTarget && movement.MovementType == 0)
                    {
                        JointMovementFromRobotTarget(movement);
                        _lastMovement = movement;
                        counter++;
                    }

                    else if (movement.Target is RobotTarget && movement.MovementType == 1)
                    {
                        LinearMovementFromRobotTarget(movement);
                        _lastMovement = movement;
                        counter++;
                    }

                    else if (movement.Target is RobotTarget && movement.MovementType == 2)
                    {
                        JointMovementFromRobotTarget(movement);
                        _lastMovement = movement;
                        counter++;
                    }

                    else if (movement.Target is JointTarget && movement.MovementType == 0)
                    {
                        JointMovementFromJointTarget(movement);
                        _lastMovement = movement;
                        counter++;
                    }
                }
            }

            // Add joint positions and plane from last movement
            if (counter > 0)
            {
                _robotJointPositions.Add(_lastRobotJointPosition);
                _externalJointPositions.Add(_lastExternalJointPosition);
                _robotInfo.ForwardKinematics.Calculate(_lastRobotJointPosition, _lastExternalJointPosition);
                _planes.Add(_robotInfo.ForwardKinematics.TCPPlane);
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
        }

        /// <summary>
        /// Sets the correct Robot Tool for the defined movement.
        /// </summary>
        /// <param name="movement"> The movemen to set the Robot Tool for. </param>
        private void SetRobotTool(Movement movement)
        {
            if (movement.RobotTool == null)
            {
                _robotInfo.Tool = _currentTool;
            }
            else if (movement.RobotTool.Name == "")
            {
                _robotInfo.Tool = _currentTool;
            }
            else
            {
                _robotInfo.Tool = movement.RobotTool;
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
            _errorText.AddRange(jointTarget.CheckForAxisLimits(_robotInfo));

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
            _robotInfo.InverseKinematics.Movement = movement;
            _robotInfo.InverseKinematics.Calculate();

            // Auto Axis Config
            if (_autoAxisConfig == true && movement.MovementType != 0)
            {
                _robotInfo.InverseKinematics.GetClosestRobotJointPosition(_lastRobotJointPosition);
            }

            // Get the Robot Joint Positions
            RobotJointPosition towardsRobotJointPosition = _robotInfo.InverseKinematics.RobotJointPosition.Duplicate();
            ExternalJointPosition towardsExternalJointPosition = _robotInfo.InverseKinematics.ExternalJointPosition.Duplicate();

            // Add error text
            _errorText.AddRange(_robotInfo.InverseKinematics.ErrorText);

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
            _robotInfo.InverseKinematics.Movement = movement;
            _robotInfo.InverseKinematics.CalculateExternalAxisValues();

            // Get the External Joint Positions
            ExternalJointPosition towardsExternalJointPosition = _robotInfo.InverseKinematics.ExternalJointPosition.Duplicate();

            // External Joint Position change
            ExternalJointPosition externalJointPositionChange = (towardsExternalJointPosition - _lastExternalJointPosition) / _interpolations;

            // TODO: Check with last movement to speed up the process? As in old path generator?
                                          
            // First target plane in WORLD coordinate space
            _robotInfo.ForwardKinematics.Calculate(_lastRobotJointPosition, _lastExternalJointPosition);
            Plane plane1 = _robotInfo.ForwardKinematics.TCPPlane;

            // Second target plane in WORK OBJECT coordinate space 
            RobotTarget robotTarget = movement.Target as RobotTarget;
            Plane plane2 = robotTarget.Plane; 

            // Correction for rotation of the target plane on a movable work object
            if (movement.WorkObject.ExternalAxis != null)
            {
                ExternalAxis externalAxis = movement.WorkObject.ExternalAxis;
                int logic = (int)externalAxis.AxisNumber;
                double axisValue = _lastExternalJointPosition[logic];
                Transform trans = externalAxis.CalculateTransformationMatrix(-axisValue, out _);
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

            // Create the sub target planes, internal axis values and external axis values for every interpolation step
            for (int i = 0; i < _interpolations; i++)
            {
                // Create new plane: the local target plane (in work object coordinate space)
                Plane plane = new Plane(plane1.Origin + posChange * i, plane1.XAxis + xAxisChange * i, plane1.YAxis + yAxisChange * i);

                // Update the target and movement
                subTarget.Plane = plane;
                subTarget.ExternalJointPosition = newExternalJointPosition;
                subMovement.Target = subTarget;

                // Calculate internal axis values
                _robotInfo.InverseKinematics.Movement = subMovement;
                _robotInfo.InverseKinematics.Calculate();

                // Auto Axis Config
                if (_autoAxisConfig == true)
                {
                    if (i == 0)
                    {
                        _robotInfo.InverseKinematics.GetClosestRobotJointPosition(_lastRobotJointPosition);
                    }
                    else
                    {
                        _robotInfo.InverseKinematics.GetClosestRobotJointPosition(_robotJointPositions.Last());
                    }
                }

                // Add te calculated axis values and plane to the class property
                _robotJointPositions.Add(_robotInfo.InverseKinematics.RobotJointPosition.Duplicate());
                _externalJointPositions.Add(_robotInfo.InverseKinematics.ExternalJointPosition.Duplicate());

                // Add the plane
                Plane globalPlane = subMovement.GetPosedGlobalTargetPlane(_robotInfo, out _);
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
            Point3d lastPoint = movement.GetPosedGlobalTargetPlane(_robotInfo, out _).Origin;
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
            _robotInfo.InverseKinematics.Movement = movement;
            _robotInfo.InverseKinematics.Calculate();

            // Auto Axis Config
            if (_autoAxisConfig == true)
            {
                _robotInfo.InverseKinematics.GetClosestRobotJointPosition(_robotJointPositions.Last());
            }

            // Add last Joint Poistions
            _lastRobotJointPosition = _robotInfo.InverseKinematics.RobotJointPosition.Duplicate();
            _lastExternalJointPosition = _robotInfo.InverseKinematics.ExternalJointPosition.Duplicate();
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

                _robotInfo.ForwardKinematics.Calculate(newRobotJointPosition, newExternalJointPosition);
                _planes.Add(_robotInfo.ForwardKinematics.TCPPlane);

                if (i == 0) 
                { 
                    points.Add(new Point3d(_robotInfo.ForwardKinematics.TCPPlane.Origin)); 
                }
                else if (points[points.Count - 1] != _robotInfo.ForwardKinematics.TCPPlane.Origin) 
                { 
                    points.Add(new Point3d(_robotInfo.ForwardKinematics.TCPPlane.Origin)); 
                }

                newRobotJointPosition += robotJointPositionChange;
                newExternalJointPosition += externalJointPositionChange;
            }

            // Add last point
            _robotInfo.ForwardKinematics.Calculate(towardsRobotJointPosition, towardsExternalJointPosition);
            if (points[points.Count - 1] != _robotInfo.ForwardKinematics.TCPPlane.Origin) 
            { 
                points.Add(_robotInfo.ForwardKinematics.TCPPlane.Origin); 
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
        #endregion

        #region properties
        /// <summary>
        /// A boolean that indicates if the Path Generator object is valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                if (RobotInfo == null) { return false; }
                if (RobotInfo.IsValid == false) { return false; }
                return true;
            }
        }

        /// <summary>
        /// The robot info to construct the path for.
        /// </summary>
        public Robot RobotInfo
        {
            get { return _robotInfo; }
        }

        /// <summary>
        /// The list with planes the robot follows.
        /// </summary>
        public List<Plane> Planes
        {
            get { return _planes; }
        }

        /// <summary>
        /// An approximation of the path the TCP of the robot will follow.
        /// For every movement a curve is constructed. 
        /// </summary>
        public List<Curve> Paths 
        {
            get { return _paths; }
        }

        /// <summary>
        /// The Robot Joint Positions set to follow the path. 
        /// </summary>
        public List<RobotJointPosition> RobotJointPositions
        {
            get { return _robotJointPositions; }
        }

        /// <summary>
        /// The External Joint Positions set to follow the path. 
        /// </summary>
        public List<ExternalJointPosition> ExternalJointPositions
        {
            get { return _externalJointPositions; }
        }

        /// <summary>
        /// List of strings with collected error messages. 
        /// </summary>
        public List<string> ErrorText
        {
            get { return _errorText; }
        }
        #endregion
    }

}





// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
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
        private Robot _robotInfo; // The robot info to construct the path for
        private readonly List<Plane> _planes; // The planes the path follow
        private readonly List<Curve> _paths; // The path curves between two movement targets
        private readonly List<List<double>> _internalAxisValues; // The internal axis values needed to follow the path
        private readonly List<List<double>> _externalAxisValues; // The external axis values needed to follow the path
        private readonly List<string> _errorText = new List<string>(); // List with collected error messages: for now only checking for absolute joint momvements!
        #endregion

        #region constructors
        /// <summary>
        /// An empty Path Generator constructor
        /// </summary>
        public PathGenerator()
        {
            _planes = new List<Plane>();
            _paths = new List<Curve>();
            _internalAxisValues = new List<List<double>>();
            _externalAxisValues = new List<List<double>>();
        }

        /// <summary>
        /// Defines a Path Generator with a robot info.
        /// </summary>
        /// <param name="robotInfo"> The robot info to construct the path for. </param>
        public PathGenerator(Robot robotInfo)
        {
            _planes = new List<Plane>();
            _paths = new List<Curve>();
            _internalAxisValues = new List<List<double>>();
            _externalAxisValues = new List<List<double>>();
            _robotInfo = robotInfo.Duplicate(); // Since we might swap tools and therefore change the robot tool we make a deep copy
        }

        /// <summary>
        /// Creates a new path generator by duplicating an existing path generator. 
        /// This creates a deep copy of the existing path generator. 
        /// </summary>
        /// <param name="generator"> The path generator that should be duplicated. </param>
        public PathGenerator(PathGenerator generator)
        {
            // Set fields
            _planes = new List<Plane>(generator.Planes);
            _paths = generator.Paths.ConvertAll(curve => curve.DuplicateCurve());
            _internalAxisValues = generator.InternalAxisValues.ConvertAll(axisValues => new List<double>(axisValues));
            _externalAxisValues = generator.ExternalAxisValues.ConvertAll(axisValues => new List<double>(axisValues));
            _robotInfo = generator.RobotInfo.Duplicate();
        }

        /// <summary>
        /// A method to duplicate the Path Generator object. 
        /// </summary>
        /// <returns> Returns a deep copy of the Path Generator object. </returns>
        public PathGenerator Duplicate()
        {
            return new PathGenerator(this);
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
        /// Generates the internal and external axis values and the path the robot follows. 
        /// </summary>
        public void GetAxisValues(List<Actions.Action> movements, int interpolations)
        {
            // Hide the mesh when calculating the forward kinematics
            _robotInfo.ForwardKinematics.HideMesh = true;

            // Axis logic and numbers
            int nAxes = _robotInfo.ExternalAxis.Count;
            int logic;

            // Initiate movement
            Movement movement1 = new Movement(new RobotTarget("init", Plane.WorldXY)); // Used for movement[i]: the movement before which defines the starting point / target of the current movement.
            Movement movement2; // Used for movement[i+1]: to movement we are carrying out
            AbsoluteJointMovement jointMovement;

            // Initialize the internal and external axis values of the first and second target
            List<double> target2InternalAxisValues = new List<double>();
            List<double> target2ExternalAxisValues = new List<double>();
            List<double> target1InternalAxisValues;
            List<double> target1ExternalAxisValues;

            // Initialize the internal and external axis values of the second target
            if (movements[0] is AbsoluteJointMovement castedJointMovement)
            {
                target2InternalAxisValues = new List<double>(castedJointMovement.InternalAxisValues);
                target2ExternalAxisValues = new List<double>(castedJointMovement.ExternalAxisValues.GetRange(0,nAxes));
                _errorText.AddRange(castedJointMovement.CheckForAxisLimits(_robotInfo));
            }
            else if (movements[0] is Movement castedMovement)
            {
                _robotInfo.InverseKinematics.Movement = castedMovement;
                _robotInfo.InverseKinematics.Calculate();
                target2InternalAxisValues = new List<double>(_robotInfo.InverseKinematics.InternalAxisValues);
                target2ExternalAxisValues = new List<double>(_robotInfo.InverseKinematics.ExternalAxisValues);
                _errorText.AddRange(new List<string>(_robotInfo.InverseKinematics.ErrorText));
            }

            // Initialize other variables
            List<double> internalAxisValueChange = new List<double>();
            List<double> externalAxisValueChange = new List<double>();

            // Initiate target and movement for linear movement interpolation
            RobotTarget subTarget = new RobotTarget("subTarget", Plane.WorldXY);
            Movement subMovement = new Movement(subTarget);

            // Initiate list with points between two targets
            List<Point3d> points = new List<Point3d>();

            // Make path if we have at least two movements with targets
            if (movements.Count > 1)
            {
                for (int i = 0; i < movements.Count - 1; i++)
                {
                    #region Absolute Joint Movement (from Action.AbsolutJointMovement)
                    if (movements[i + 1] is AbsoluteJointMovement)
                    {
                        // Get the movement
                        jointMovement = movements[i + 1] as AbsoluteJointMovement;

                        // Check for axis limits
                        _errorText.AddRange(jointMovement.CheckForAxisLimits(_robotInfo));

                        // Update tool
                        _robotInfo.Tool = jointMovement.RobotTool;

                        // Our first target is the second target of the last movement
                        target1InternalAxisValues = new List<double>(target2InternalAxisValues);
                        target1ExternalAxisValues = new List<double>(target2ExternalAxisValues);

                        // Get the axis values definied for the joint movement
                        target2InternalAxisValues = new List<double>(jointMovement.InternalAxisValues);
                        target2ExternalAxisValues = new List<double>(jointMovement.ExternalAxisValues.GetRange(0, nAxes)); // TODO: match list length with external axis list length of robot info

                        // Calculate axis value difference and change between both targets
                        externalAxisValueChange.Clear();
                        for (int j = 0; j < target1ExternalAxisValues.Count; j++)
                        {
                            double difference = target2ExternalAxisValues[j] - target1ExternalAxisValues[j];
                            double valueChange = difference / interpolations;
                            externalAxisValueChange.Add(valueChange);
                        }

                        // Calculate axis value difference and change between both targets
                        internalAxisValueChange.Clear();
                        for (int j = 0; j < target1InternalAxisValues.Count; j++)
                        {
                            double difference = target2InternalAxisValues[j] - target1InternalAxisValues[j];
                            double valueChange = difference / interpolations;
                            internalAxisValueChange.Add(valueChange);
                        }

                        // Calculates intermediate internal and external axis values and tcp point 
                        for (int j = 0; j < interpolations; j++)
                        {
                            // Calculate the internal axis values for every separate internal axis
                            List<double> internalAxisValues = new List<double>();
                            for (int k = 0; k < target1InternalAxisValues.Count; k++)
                            {
                                double valueAddition = target1InternalAxisValues[k] + internalAxisValueChange[k] * j;
                                internalAxisValues.Add(valueAddition);
                            }

                            // Calculate the exnternal axis values for every separate external axis
                            List<double> externalAxisValues = new List<double>();
                            for (int k = 0; k < target1ExternalAxisValues.Count; k++)
                            {
                                double valueAddition = target1ExternalAxisValues[k] + externalAxisValueChange[k] * j;
                                externalAxisValues.Add(valueAddition);
                            }

                            // Calculate point to be able to draw the path curve
                            _robotInfo.ForwardKinematics.Calculate(internalAxisValues, externalAxisValues);
                            Point3d point = _robotInfo.ForwardKinematics.TCPPlane.Origin;

                            // Add te calculated axis values and plane to the class property
                            _internalAxisValues.Add(new List<double>(internalAxisValues));
                            _externalAxisValues.Add(new List<double>(externalAxisValues));
                            _planes.Add(new Plane(_robotInfo.ForwardKinematics.TCPPlane));

                            // Always add the first point to list with paths
                            if (j == 0)
                            {
                                points.Add(point);
                            }

                            // Only add the other point if this point is different
                            else if (points[points.Count - 1] != point)
                            {
                                points.Add(point);
                            }
                        }

                        // Add last point
                        _robotInfo.ForwardKinematics.Calculate(jointMovement.InternalAxisValues, jointMovement.ExternalAxisValues.GetRange(0, nAxes));
                        Point3d lastPoint = _robotInfo.ForwardKinematics.TCPPlane.Origin;
                        if (points[points.Count - 1] != lastPoint)
                        {
                            points.Add(lastPoint);
                        }
                    }
                    #endregion

                    #region Movement (from Action.Movement)
                    else if (movements[i + 1] is Movement)
                    {
                        // Get the movement
                        movement2 = movements[i + 1] as Movement;

                        // Update tool 
                        _robotInfo.Tool = movement2.RobotTool;

                        // Our first target is the second target of the last movement
                        target1InternalAxisValues = new List<double>(target2InternalAxisValues);
                        target1ExternalAxisValues = new List<double>(target2ExternalAxisValues);
                        
                        if (movement2.Target is JointTarget jointTarget)
                        {
                            // Calculate the axis values for the second target
                            target2InternalAxisValues = jointTarget.RobotJointPosition.ToList();
                            target2ExternalAxisValues = jointTarget.ExternalJointPosition.ToList();

                            // Remove undefined values
                            target2ExternalAxisValues.Remove(9e9);

                            // Check axis limits
                            _errorText.AddRange(jointTarget.CheckForAxisLimits(_robotInfo));

                            // Calculate axis value difference between both targets
                            externalAxisValueChange.Clear();
                            for (int j = 0; j < target1ExternalAxisValues.Count; j++)
                            {
                                double difference = target2ExternalAxisValues[j] - target1ExternalAxisValues[j];
                                double valueChange = difference / interpolations;
                                externalAxisValueChange.Add(valueChange);
                            }

                            // Calculate axis value difference and change between both targets
                            internalAxisValueChange.Clear();
                            for (int j = 0; j < target1InternalAxisValues.Count; j++)
                            {
                                double difference = target2InternalAxisValues[j] - target1InternalAxisValues[j];
                                double valueChange = difference / interpolations;
                                internalAxisValueChange.Add(valueChange);
                            }

                            // Calculates intermediate internal and external axis values and tcp point 
                            for (int j = 0; j < interpolations; j++)
                            {
                                // Calculate the internal axis values for every separate internal axis
                                List<double> internalAxisValues = new List<double>();
                                for (int k = 0; k < target1InternalAxisValues.Count; k++)
                                {
                                    double valueAddition = target1InternalAxisValues[k] + internalAxisValueChange[k] * j;
                                    internalAxisValues.Add(valueAddition);
                                }

                                // Calculate the enternal axis values for every separate external axis
                                List<double> externalAxisValues = new List<double>();
                                for (int k = 0; k < target1ExternalAxisValues.Count; k++)
                                {
                                    double valueAddition = target1ExternalAxisValues[k] + externalAxisValueChange[k] * j;
                                    externalAxisValues.Add(valueAddition);
                                }

                                // Calculate point to be able to draw the path curve
                                _robotInfo.ForwardKinematics.Calculate(internalAxisValues, externalAxisValues);
                                Point3d point = _robotInfo.ForwardKinematics.TCPPlane.Origin;

                                // Add te calculated axis values and plane to the class property
                                _internalAxisValues.Add(new List<double>(internalAxisValues));
                                _externalAxisValues.Add(new List<double>(externalAxisValues));
                                _planes.Add(_robotInfo.ForwardKinematics.TCPPlane);

                                // Always add the first point to list with paths
                                if (j == 0)
                                {
                                    points.Add(point);
                                }

                                // Only add the other point if this point is different
                                else if (points[points.Count - 1] != point)
                                {
                                    points.Add(point);
                                }
                            }

                            // Add last point
                            _robotInfo.ForwardKinematics.Calculate(jointTarget.RobotJointPosition.ToList(), jointTarget.ExternalJointPosition.ToList().GetRange(0, nAxes));
                            Point3d lastPoint = _robotInfo.ForwardKinematics.TCPPlane.Origin;
                            if (points[points.Count - 1] != lastPoint)
                            {
                                points.Add(lastPoint);
                            }
                        }

                        else // Movement with Robot Target
                        {
                            // Calculate the axis values for the second target
                            _robotInfo.InverseKinematics.Movement = movement2;
                            _robotInfo.InverseKinematics.Calculate();
                            target2InternalAxisValues = new List<double>(_robotInfo.InverseKinematics.InternalAxisValues);
                            target2ExternalAxisValues = new List<double>(_robotInfo.InverseKinematics.ExternalAxisValues);

                            // Collect error message for absolute joint movement
                            if (movement2.MovementType == 0)
                            {
                                _errorText.AddRange(new List<string>(_robotInfo.InverseKinematics.ErrorText));
                            }

                            // Calculate axis value difference between both targets: needed for all movement types
                            externalAxisValueChange.Clear();
                            for (int j = 0; j < target1ExternalAxisValues.Count; j++)
                            {
                                double difference = target2ExternalAxisValues[j] - target1ExternalAxisValues[j];
                                double valueChange = difference / interpolations;
                                externalAxisValueChange.Add(valueChange);
                            }

                            #region create path for joint movement
                            if (movement2.MovementType == 0 || movement2.MovementType == 2)
                            {
                                // Calculate axis value difference and change between both targets
                                internalAxisValueChange.Clear();
                                for (int j = 0; j < target1InternalAxisValues.Count; j++)
                                {
                                    double difference = target2InternalAxisValues[j] - target1InternalAxisValues[j];
                                    double valueChange = difference / interpolations;
                                    internalAxisValueChange.Add(valueChange);
                                }

                                // Calculates intermediate internal and external axis values and tcp point 
                                for (int j = 0; j < interpolations; j++)
                                {
                                    // Calculate the internal axis values for every separate internal axis
                                    List<double> internalAxisValues = new List<double>();
                                    for (int k = 0; k < target1InternalAxisValues.Count; k++)
                                    {
                                        double valueAddition = target1InternalAxisValues[k] + internalAxisValueChange[k] * j;
                                        internalAxisValues.Add(valueAddition);
                                    }

                                    // Calculate the enternal axis values for every separate external axis
                                    List<double> externalAxisValues = new List<double>();
                                    for (int k = 0; k < target1ExternalAxisValues.Count; k++)
                                    {
                                        double valueAddition = target1ExternalAxisValues[k] + externalAxisValueChange[k] * j;
                                        externalAxisValues.Add(valueAddition);
                                    }

                                    // Calculate point to be able to draw the path curve
                                    _robotInfo.ForwardKinematics.Calculate(internalAxisValues, externalAxisValues);
                                    Point3d point = _robotInfo.ForwardKinematics.TCPPlane.Origin;

                                    // Add te calculated axis values and plane to the class property
                                    _internalAxisValues.Add(new List<double>(internalAxisValues));
                                    _externalAxisValues.Add(new List<double>(externalAxisValues));
                                    _planes.Add(_robotInfo.ForwardKinematics.TCPPlane);

                                    // Always add the first point to list with paths
                                    if (j == 0)
                                    {
                                        points.Add(point);
                                    }

                                    // Only add the other point if this point is different
                                    else if (points[points.Count - 1] != point)
                                    {
                                        points.Add(point);
                                    }
                                }

                                // Add last point
                                Point3d lastPoint = movement2.GetPosedGlobalTargetPlane(_robotInfo, out logic).Origin;
                                if (points[points.Count - 1] != lastPoint)
                                {
                                    points.Add(lastPoint);
                                }
                            }
                            #endregion

                            #region create path for linear movement
                            // Targets and paths for linear movement: MoveL
                            else
                            {
                                #region calculate interpolated target plane
                                // Target planes
                                Plane plane1;
                                Plane plane2;

                                // Get movement before (especially the target plane): the start point of the current movement
                                if (movements[i] is Movement movement)
                                {
                                    if (movement.Target is RobotTarget)
                                    {
                                        movement1 = movements[i] as Movement;
                                    }
                                    else
                                    {
                                        jointTarget = movement.Target as JointTarget;
                                        _robotInfo.ForwardKinematics.Calculate(jointTarget.RobotJointPosition.ToList(), jointTarget.ExternalJointPosition.ToList().GetRange(0, nAxes));
                                        movement1 = new Movement(new RobotTarget("jointTarget", _robotInfo.ForwardKinematics.TCPPlane));
                                    }
                                }
                                else if (movements[i] is AbsoluteJointMovement)
                                {
                                    jointMovement = movements[i] as AbsoluteJointMovement;
                                    _robotInfo.ForwardKinematics.Calculate(jointMovement.InternalAxisValues, jointMovement.ExternalAxisValues.GetRange(0, nAxes));
                                    movement1 = new Movement(new RobotTarget("jointTarget", _robotInfo.ForwardKinematics.TCPPlane));
                                }

                                // If both movements are on the same work object and with the same tool
                                if (movement1.WorkObject.Name == movement2.WorkObject.Name && movement1.RobotTool.Name == movement2.RobotTool.Name)
                                {
                                    plane1 = movement1.RobotTarget.Plane;
                                    plane2 = movement2.RobotTarget.Plane;
                                }

                                // Else the movements are not on the same work object and / or the tools are differnt
                                // We have to re-orient one target plane to the other work object
                                else
                                {
                                    // Get plane1 and plane2
                                    _robotInfo.ForwardKinematics.Calculate(target1InternalAxisValues, target1ExternalAxisValues);
                                    plane1 = new Plane(_robotInfo.ForwardKinematics.TCPPlane); // In world coordinate space
                                    plane2 = movement2.RobotTarget.Plane; // In work object coordinate space

                                    // Re-orient plane1 to the other work object coordinate space of the plane2
                                    Plane globalWorkObjectPlane = new Plane(movement2.WorkObject.GlobalWorkObjectPlane);
                                    Transform orient = Transform.ChangeBasis(Plane.WorldXY, globalWorkObjectPlane);
                                    plane1.Transform(orient);

                                    // Correct for the target plane position if we make a movement an a movable work object
                                    // This is only needed if move from a fixed work object to a movable work object.
                                    // Or if we switch between to movable workobjects with two different external axes
                                    if (movement1.WorkObject.ExternalAxis == null &&
                                        movement2.WorkObject.ExternalAxis != null)
                                    {
                                        // Get axis logic of the external axis of the movable work object we are using.
                                        movement2.GetPosedGlobalTargetPlane(_robotInfo, out logic);

                                        // Transform if the axis logic number is valid
                                        if (logic > -1)
                                        {
                                            Transform rotate = Transform.Rotation(-1 * (target1ExternalAxisValues[logic] / 180) * Math.PI, new Vector3d(0, 0, 1), new Point3d(0, 0, 0));
                                            plane1.Transform(rotate);
                                        }
                                    }
                                }

                                // Main target plane position difference
                                Vector3d posDif = plane2.Origin - plane1.Origin;

                                // Main target plane position difference per interpolation step
                                Vector3d posChange = posDif / interpolations;

                                // Main target planes axis differencess
                                Vector3d xAxisDif = plane2.XAxis - plane1.XAxis;
                                Vector3d yAxisDif = plane2.YAxis - plane1.YAxis;
                                Vector3d zAxisDif = plane2.ZAxis - plane1.ZAxis;

                                // Main target planes axis change per interpolation step
                                Vector3d xAxisChange = xAxisDif / interpolations;
                                Vector3d yAxisChange = yAxisDif / interpolations;
                                Vector3d zAxisChange = zAxisDif / interpolations;

                                // Sub target plane points for each interpolation step
                                List<Point3d> planePoints = new List<Point3d>();
                                for (int j = 0; j < interpolations; j++)
                                {
                                    planePoints.Add(plane1.Origin + posChange * j);
                                }

                                // Sub target plane axis directions
                                List<List<Vector3d>> axisDirections = new List<List<Vector3d>>();
                                for (int j = 0; j < interpolations; j++)
                                {
                                    List<Vector3d> axisDir = new List<Vector3d>() { };
                                    axisDir.Add(plane1.XAxis + xAxisChange * j);
                                    axisDir.Add(plane1.YAxis + yAxisChange * j);
                                    axisDir.Add(plane1.ZAxis + zAxisChange * j);
                                    axisDirections.Add(axisDir);
                                }
                                #endregion

                                // Correct axis configuration, tool and work object
                                subTarget.AxisConfig = movement2.RobotTarget.AxisConfig;
                                subMovement.RobotTool = movement2.RobotTool;
                                subMovement.WorkObject = movement2.WorkObject;

                                // Create the sub target planes, internal axis values and external axis values for every interpolation step
                                for (int l = 0; l < interpolations; l++)
                                {
                                    // Calculate the external axis values for every separate external axis
                                    List<double> externalAxisValues = new List<double>();
                                    for (int k = 0; k < target1ExternalAxisValues.Count; k++)
                                    {
                                        double valueAddition = target1ExternalAxisValues[k] + externalAxisValueChange[k] * l;
                                        externalAxisValues.Add(valueAddition);
                                    }

                                    // Create new plane: the local target plane (in work object coordinate space)
                                    Plane plane = new Plane(planePoints[l], axisDirections[l][0], axisDirections[l][1]);

                                    // Update the target
                                    subTarget.Plane = plane;
                                    subTarget.ExternalJointPosition = new ExternalJointPosition(externalAxisValues);

                                    // Update the movement
                                    subMovement.RobotTarget = subTarget;

                                    // Calculate internal axis values
                                    _robotInfo.InverseKinematics.Movement = subMovement;
                                    _robotInfo.InverseKinematics.Calculate();

                                    // Add te calculated axis values and plane to the class property
                                    _internalAxisValues.Add(new List<double>(_robotInfo.InverseKinematics.InternalAxisValues));
                                    _externalAxisValues.Add(new List<double>(externalAxisValues));

                                    // Add the plane
                                    Plane globalPlane = subMovement.GetPosedGlobalTargetPlane(_robotInfo, out logic);
                                    _planes.Add(globalPlane);

                                    // Always add the first point to list with paths
                                    if (l == 0)
                                    {
                                        points.Add(globalPlane.Origin);
                                    }

                                    // Only add the other point if this point is different
                                    else if (points[points.Count - 1] != globalPlane.Origin)
                                    {
                                        points.Add(globalPlane.Origin);
                                    }
                                }

                                // Add last point
                                Point3d lastPoint = movement2.GetPosedGlobalTargetPlane(_robotInfo, out logic).Origin;
                                if (points[points.Count - 1] != lastPoint)
                                {
                                    points.Add(lastPoint);
                                }
                            }
                        }
                        #endregion
                        }
                    
                    // Add path curve to paths if there are at least two points in the list
                    if (points.Count > 1)
                    {
                        _paths.Add(Curve.CreateInterpolatedCurve(points, 3));
                    }

                    // Clear the list with points for the new iteration
                    points.Clear();

                    #endregion
                }
            }

            // Add the axis values and the plane of the last movement
            if (movements[movements.Count - 1] is Movement)
            {
                movement2 = movements[movements.Count - 1] as Movement;

                if (movement2.Target is JointTarget jointTarget)
                {
                    target2InternalAxisValues = jointTarget.RobotJointPosition.ToList();
                    target2ExternalAxisValues = jointTarget.ExternalJointPosition.ToList();
                    target2ExternalAxisValues.Remove(9e9);
                    _internalAxisValues.Add(target2InternalAxisValues);
                    _externalAxisValues.Add(target2ExternalAxisValues);
                }

                else
                {
                    _robotInfo.InverseKinematics.Movement = movement2;
                    _robotInfo.InverseKinematics.Calculate();
                    _internalAxisValues.Add(new List<double>(_robotInfo.InverseKinematics.InternalAxisValues));
                    _externalAxisValues.Add(new List<double>(_robotInfo.InverseKinematics.ExternalAxisValues));
                    _planes.Add(movement2.GetPosedGlobalTargetPlane(_robotInfo, out logic));
                }
            }
            else if (movements[movements.Count - 1] is AbsoluteJointMovement)
            {
                jointMovement = movements[movements.Count - 1] as AbsoluteJointMovement;
                _robotInfo.ForwardKinematics.Calculate(jointMovement.InternalAxisValues, jointMovement.ExternalAxisValues.GetRange(0, nAxes));
                _internalAxisValues.Add(new List<double>(jointMovement.InternalAxisValues));
                _externalAxisValues.Add(new List<double>(jointMovement.ExternalAxisValues.GetRange(0, nAxes)));
                _planes.Add(new Plane(_robotInfo.ForwardKinematics.TCPPlane));
            }
        }

        /// <summary>
        /// Calculates the path and the axis values from a list with actions. 
        /// The momvents are automatically distracted from the list with actions. 
        /// </summary>
        /// <param name="actions"> The list with robot actions </param>
        /// <param name="interpolations"> The interpolation number. Defines the amount of sub targets between two targets. </param>
        public void Calculate(List<Actions.Action> actions, int interpolations)
        {
            // Clear all the lists with values
            Reset();

            // Get the movements from the list with actions
            List<Actions.Action> movements = GetMovements(actions);

            // Get the internal and external axis values and the path curve
            GetAxisValues(movements, interpolations);
        }

        /// <summary>
        /// Sets the correct tool at the movement level before the actions are passed to the code generation. 
        /// </summary>
        /// <param name="actions"> The list with actions to defined the robot tools for. </param>
        /// <returns> The lists with actions wherein all movements have a set robot tool. 
        /// All other action types are removed. </returns>
        private List<Actions.Action> GetMovements(List<Actions.Action> actions)
        {
            // Initiate output
            List<Actions.Action> result = new List<Actions.Action>();

            // Iniate current tool (the tool attached to the robot)
            RobotTool currentTool = _robotInfo.Tool.DuplicateWithoutMesh();

            // Loop over all the actions
            for (int i = 0; i < actions.Count; i++)
            {
                #region  Check if the Override Robot Tool action is used
                if (actions[i] is OverrideRobotTool overrideRobotTool)
                {
                    // Override the current tool
                    currentTool = overrideRobotTool.RobotTool.DuplicateWithoutMesh();
                }
                #endregion

                #region Check if the action is a movement
                else if (actions[i] is Movement)
                {
                    // Duplicate the movement since we might change properties
                    Movement movement = ((Movement)actions[i]).DuplicateWithoutMesh();

                    // Set the current tool if no tool is set in the movement object
                    if (movement.RobotTool == null)
                    {
                        movement.RobotTool = currentTool.DuplicateWithoutMesh();
                    }

                    // If a tool is set check the name (tool can be empty)
                    else if (movement.RobotTool.Name == "" || movement.RobotTool.Name == null) //TODO: RobotTool.IsValid is maybe better?
                    {
                        movement.RobotTool = currentTool.DuplicateWithoutMesh();
                    }

                    // Otherwise don't set a tool. Last overwrite is used that is combined with the movement.

                    // Add movement to list
                    result.Add(movement);
                }
                #endregion

                #region Check if the action is an absolute joint movement
                // Check if the action is a joint movement
                else if (actions[i] is AbsoluteJointMovement)
                {
                    // Duplicate the movement since we might change properties
                    AbsoluteJointMovement jointMovement = ((AbsoluteJointMovement)actions[i]).DuplicateWithoutMesh();

                    // Set the current tool if no tool is set in the movement object
                    if (jointMovement.RobotTool == null)
                    {
                        jointMovement.RobotTool = currentTool.DuplicateWithoutMesh();
                    }

                    // If a tool is set check the name (tool can be empty)
                    else if (jointMovement.RobotTool.Name == "" || jointMovement.RobotTool.Name == null) //TODO: RobotTool.IsValid is maybe better?
                    {
                        jointMovement.RobotTool = currentTool.DuplicateWithoutMesh();
                    }

                    // Otherwise don't set a tool. Last overwrite is used that is combined with the movement.

                    // Add movement to list
                    result.Add(jointMovement);
                }
                #endregion

                #region All other actions
                // Do nothing. We only extact the movements. 
                #endregion
            }

            return result;
        }

        /// <summary>
        /// Resets / clears all lists with values (planes and axis values
        /// </summary>
        private void Reset()
        {
            // Clear the lists with axis values
            for (int i = 0; i < _internalAxisValues.Count; i++)
            {
                _internalAxisValues[i].Clear();
                _externalAxisValues[i].Clear();
            }

            _internalAxisValues.Clear();
            _externalAxisValues.Clear();

            // Clears other lists
            _planes.Clear();
            _paths.Clear();
            _errorText.Clear();
        }
        #endregion

        #region properties
        /// <summary>
        /// A boolean that indicates if the Pat Generator object is valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                if (RobotInfo == null) { return false; };
                return true;
            }
        }

        /// <summary>
        /// The robot info to construct the path for.
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
                Reset();
            }
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
        /// The internal axis values set to follow the path. 
        /// </summary>
        public List<List<double>> InternalAxisValues 
        {
            get { return _internalAxisValues; }
        }

        /// <summary>
        /// The external axis values set to follow the path. 
        /// </summary>
        public List<List<double>> ExternalAxisValues 
        {
            get { return _externalAxisValues; }
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





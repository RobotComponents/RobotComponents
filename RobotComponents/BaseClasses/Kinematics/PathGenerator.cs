using System;
using System.Collections.Generic;

using Rhino.Geometry;

using RobotComponents.BaseClasses.Actions;
using RobotComponents.BaseClasses.Definitions;

namespace RobotComponents.BaseClasses.Kinematics
{
    /// <summary>
    /// Path Generator class. This class does an approximation of the path the robot will follow.
    /// </summary>
    public class PathGenerator
    {
        #region fields
        private RobotInfo _robotInfo; // The robot info to construct the path for
        private readonly List<Plane> _planes; // The planes the path follow
        private readonly List<Curve> _paths; // The path curves between two movement targets
        private readonly List<List<double>> _internalAxisValues; // The internal axis values needed to follow the path
        private readonly List<List<double>> _externalAxisValues; // The external axis values needed to follow the path
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
        public PathGenerator(RobotInfo robotInfo)
        {
            _planes = new List<Plane>();
            _paths = new List<Curve>();
            _internalAxisValues = new List<List<double>>();
            _externalAxisValues = new List<List<double>>();
            _robotInfo = robotInfo;
        }

        /// <summary>
        /// Private path generator with all the class fields. Only used for the Duplicate method. 
        /// </summary>
        /// <param name="planes"> The list with planes the robot follows. </param>
        /// <param name="paths"> An approximation of the path the TCP of the robot will follow. </param>
        /// <param name="internalAxisValues"> The internal axis values set to follow the path. </param>
        /// <param name="externalAxisValues"> The external axis values set to follow the path. </param>
        /// <param name="robotInfo"> The robot info to construct the path for. </param>
        private PathGenerator(List<Plane> planes, List<Curve> paths, List<List<double>> internalAxisValues, List<List<double>> externalAxisValues, RobotInfo robotInfo)
        {
            _planes = planes;
            _paths = paths;
            _internalAxisValues = internalAxisValues;
            _externalAxisValues = externalAxisValues;
            _robotInfo = robotInfo;
        }

        /// <summary>
        /// A method to duplicate the Path Generator object. 
        /// </summary>
        /// <returns> Returns a deep copy for the Path Generator object. </returns>
        public PathGenerator Duplicate()
        {
            PathGenerator dup = new PathGenerator(Planes, Paths, InternalAxisValues, ExternalAxisValues, RobotInfo);
            return dup;
        }
        #endregion

        #region methods
        /// <summary>
        /// Generates the internal and external axis values and the path the robot follows. 
        /// </summary>
        public void GetAxisValues(List<Actions.Action> movements, int interpolations) //TODO: Change method to input with actions (movements and absolute joint movements)
        {
            // Initialize the forward kinematics
            ForwardKinematics forwardKinematics = new ForwardKinematics(_robotInfo, true);

            // Initiate movement
            Movement movement = new Movement(new Target("init", Plane.WorldXY)); ; // Used for movement[i+1]: to movement we are carrying out
            Movement startMovement = new Movement(new Target("init", Plane.WorldXY)); // Used for movement[i]: the movement before which defines the starting point / target of the current movement.
            AbsoluteJointMovement jointMovement;

            // Initialize the internal and external axis values of the first and second target
            List<double> target2InternalAxisValues = new List<double>() { };
            List<double> target2ExternalAxisValues = new List<double>() { };
            List<double> target1InternalAxisValues = new List<double>() { };
            List<double> target1ExternalAxisValues = new List<double>() { };

            // Initiate inverse kinematics
            InverseKinematics inverseKinematics = new InverseKinematics(movement, _robotInfo);

            // Initialize the internal and external axis values of the second target
            if (movements[0] is AbsoluteJointMovement)
            {
                // Get the movement
                jointMovement = movements[0] as AbsoluteJointMovement;

                // Get the axis values
                target2InternalAxisValues = new List<double>(jointMovement.InternalAxisValues);
                target2ExternalAxisValues = new List<double>(jointMovement.ExternalAxisValues);
            }

            else
            {
                // Get the movement
                movement = movements[0] as Movement;

                // Calculate the axis values
                inverseKinematics = new InverseKinematics(movement, _robotInfo);
                inverseKinematics.Calculate();

                // Get the axis values
                target2InternalAxisValues = new List<double>(inverseKinematics.InternalAxisValues);
                target2ExternalAxisValues = new List<double>(inverseKinematics.ExternalAxisValues);
            }

            // Initialize other variables
            List<double> externalAxisValueDifferences = new List<double>();
            List<double> externalAxisValueChange = new List<double>();

            // Initiate sub inverse kinematics and variables
            Target subTarget = new Target("subTarget", Plane.WorldXY);
            Movement subMovement = new Movement(subTarget);
            InverseKinematics inverseKinematicsSub = new InverseKinematics(subMovement, _robotInfo);

            // Axis logic
            int logic = -1; // dummy value

            // Make path if we have at least two movements with targets
            if (movements.Count > 1)
            {
                for (int i = 0; i < movements.Count - 1; i++)
                {
                    #region Absolut Joint Movement (from Action.AbsolutJointMovement)
                    if (movements[i + 1] is AbsoluteJointMovement)
                    {
                        // Get the movement
                        jointMovement = movements[i + 1] as AbsoluteJointMovement;

                        // Points for path curve of this movement between two targets
                        List<Point3d> points = new List<Point3d>();

                        // Update tool of forward kinematics
                        forwardKinematics.RobotInfo.Tool = jointMovement.RobotTool;

                        // Our first target is the second target of the last movement
                        target1InternalAxisValues = new List<double>(target2InternalAxisValues);
                        target1ExternalAxisValues = new List<double>(target2ExternalAxisValues);

                        // Get the axis values definied for the joint movement
                        target2InternalAxisValues = new List<double>(jointMovement.InternalAxisValues);
                        target2ExternalAxisValues = new List<double>(jointMovement.ExternalAxisValues); // TODO: match list length with external axis list length of robot info

                        #region Calculate interpolated external axis values differences
                        // Calculate axis value difference between both targets
                        externalAxisValueDifferences.Clear();
                        for (int j = 0; j < target1ExternalAxisValues.Count; j++)
                        {
                            double difference = target2ExternalAxisValues[j] - target1ExternalAxisValues[j];
                            externalAxisValueDifferences.Add(difference);
                        }

                        // Calculates axis value change per interpolation step
                        externalAxisValueChange.Clear();
                        for (int j = 0; j < target1ExternalAxisValues.Count; j++)
                        {
                            double valueChange = externalAxisValueDifferences[j] / interpolations;
                            externalAxisValueChange.Add(valueChange);
                        }
                        #endregion

                       // Calculate axis value difference between both targets
                        List<double> internalAxisValueDifferences = new List<double>();
                        for (int j = 0; j < target1InternalAxisValues.Count; j++)
                        {
                            double difference = target2InternalAxisValues[j] - target1InternalAxisValues[j];
                            internalAxisValueDifferences.Add(difference);
                        }

                        // Calculate axis value change per interpolation step
                        List<double> internalAxisValueChange = new List<double>();
                        for (int j = 0; j < target1InternalAxisValues.Count; j++)
                        {
                            double valueChange = internalAxisValueDifferences[j] / interpolations;
                            internalAxisValueChange.Add(valueChange);
                        }

                        Rhino.RhinoApp.WriteLine("Debug || GetAxisValues 6");
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
                            forwardKinematics.Update(internalAxisValues, externalAxisValues);
                            forwardKinematics.Calculate();
                            Point3d point = forwardKinematics.TCPPlane.Origin;

                            // Add te calculated axis values and plane to the class property
                            _internalAxisValues.Add(new List<double>(internalAxisValues));
                            _externalAxisValues.Add(new List<double>(externalAxisValues));
                            _planes.Add(new Plane(forwardKinematics.TCPPlane)); // deep copy needed?

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
                        forwardKinematics.Update(new List<double>(jointMovement.InternalAxisValues), new List<double>(jointMovement.ExternalAxisValues));
                        forwardKinematics.Calculate();
                        Point3d lastPoint = forwardKinematics.TCPPlane.Origin;
                        if (points[points.Count - 1] != lastPoint)
                        {
                            points.Add(lastPoint);
                        }

                        // Add path curve to paths if there are at least two points in the list
                        if (points.Count > 1)
                        {
                            _paths.Add(Curve.CreateInterpolatedCurve(points, 3));
                        }
                    }
                    #endregion

                    #region Movement (from Action.Movement)
                    else if (movements[i + 1] is Movement)
                    {
                        // Get the movement
                        movement = (Movement)movements[i + 1];

                        // Points for path curve of this movement between two targets
                        List<Point3d> points = new List<Point3d>();

                        // Update tool of forward kinematics
                        forwardKinematics.RobotInfo.Tool = movement.RobotTool;

                        // Our first target is the second target of the last movement
                        target1InternalAxisValues = new List<double>(target2InternalAxisValues);
                        target1ExternalAxisValues = new List<double>(target2ExternalAxisValues);

                        // Calculate the axis values for the second target
                        inverseKinematics.Movement = movement;
                        inverseKinematics.Calculate();
                        target2InternalAxisValues = new List<double>(inverseKinematics.InternalAxisValues);
                        target2ExternalAxisValues = new List<double>(inverseKinematics.ExternalAxisValues);

                        #region Calculate interpolated external axis values differences (needed for both joint and linear movement)
                        // Calculate axis value difference between both targets
                        externalAxisValueDifferences.Clear();
                        for (int j = 0; j < target1ExternalAxisValues.Count; j++)
                        {
                            double difference = target2ExternalAxisValues[j] - target1ExternalAxisValues[j];
                            externalAxisValueDifferences.Add(difference);
                        }

                        // Calculates axis value change per interpolation step
                        externalAxisValueChange.Clear();
                        for (int j = 0; j < target1ExternalAxisValues.Count; j++)
                        {
                            double valueChange = externalAxisValueDifferences[j] / interpolations;
                            externalAxisValueChange.Add(valueChange);
                        }
                        #endregion

                        #region create path for joint movement
                        if (movement.MovementType == 0 || movement.MovementType == 2)
                        {
                            // Calculate axis value difference between both targets
                            List<double> internalAxisValueDifferences = new List<double>();
                            for (int j = 0; j < target1InternalAxisValues.Count; j++)
                            {
                                double difference = target2InternalAxisValues[j] - target1InternalAxisValues[j];
                                internalAxisValueDifferences.Add(difference);
                            }

                            // Calculate axis value change per interpolation step
                            List<double> internalAxisValueChange = new List<double>();
                            for (int j = 0; j < target1InternalAxisValues.Count; j++)
                            {
                                double valueChange = internalAxisValueDifferences[j] / interpolations;
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
                                forwardKinematics.Update(internalAxisValues, externalAxisValues);
                                forwardKinematics.Calculate();
                                Point3d point = forwardKinematics.TCPPlane.Origin;

                                // Add te calculated axis values and plane to the class property
                                _internalAxisValues.Add(new List<double>(internalAxisValues));
                                _externalAxisValues.Add(new List<double>(externalAxisValues));
                                _planes.Add(forwardKinematics.TCPPlane);

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
                            Point3d lastPoint = movement.GetPosedGlobalTargetPlane(_robotInfo, out logic).Origin;
                            if (points[points.Count - 1] != lastPoint)
                            {
                                points.Add(lastPoint);
                            }

                            // Add path curve to paths if there are at least two points in the list
                            if (points.Count > 1)
                            {
                                _paths.Add(Curve.CreateInterpolatedCurve(points, 3));
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

                            // Get movement before (especially the target plane)
                            if (movements[i] is Movement)
                            {
                                startMovement = movements[i] as Movement;
                            }
                            else if (movements[i] is AbsoluteJointMovement)
                            {
                                jointMovement = movements[i] as AbsoluteJointMovement;

                                forwardKinematics.Update(new List<double>(jointMovement.InternalAxisValues), new List<double>(jointMovement.ExternalAxisValues));
                                forwardKinematics.Calculate();
                                Plane targetPlane = forwardKinematics.TCPPlane;
                                startMovement = new Movement(new Target("jointMovement", targetPlane));
                            }

                            // If both movements are on the same work object
                            if (startMovement.WorkObject == movement.WorkObject)
                            {
                                plane1 = startMovement.Target.Plane;
                                plane2 = movement.Target.Plane;
                            }

                            // Else the movements are not on the same work object
                            // We have to re-orient one target plane to the other work object
                            else
                            {
                                // Get plane1 and plane2
                                plane1 = new Plane(startMovement.GetPosedGlobalTargetPlane(_robotInfo, out logic)); // In world coordinate space
                                plane2 = movement.Target.Plane; // In work object coordinate space

                                // Re-orient plane1 to the other work object coordinate space of the plane2
                                Plane globalWorkObjectPlane = new Plane(movement.WorkObject.GlobalWorkObjectPlane);
                                Transform orient = Transform.ChangeBasis(Plane.WorldXY, globalWorkObjectPlane);
                                plane1.Transform(orient);

                                // Correct for the target plane position if we make a movement an a movable work object
                                // This is only needed if move from a fixed work object to a movable work object.
                                // Or if we switch between to movable workobjects with two different external axes
                                if (startMovement.WorkObject.ExternalAxis == null
                                    && movement.WorkObject.ExternalAxis != null
                                    && movement.WorkObject.ExternalAxis != startMovement.WorkObject.ExternalAxis)
                                {
                                    // Get axis logic of the external axis of the movable work object we are using.
                                    movement.GetPosedGlobalTargetPlane(_robotInfo, out logic);

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

                                // Update sub target and sub movement
                                subTarget = new Target("subTarget", plane, movement.Target.AxisConfig, externalAxisValues);
                                subMovement = new Movement(subTarget);
                                subMovement.RobotTool = movement.RobotTool;
                                subMovement.WorkObject = movement.WorkObject;

                                // Calculate internal axis values
                                inverseKinematicsSub.Movement = subMovement;
                                inverseKinematicsSub.Calculate();
                                List<double> internalAxisValues = inverseKinematicsSub.InternalAxisValues;

                                // Add te calculated axis values and plane to the class property
                                _internalAxisValues.Add(new List<double>(internalAxisValues));
                                _externalAxisValues.Add(new List<double>(externalAxisValues));
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

                                // Add last point
                                Point3d lastPoint = movement.GetPosedGlobalTargetPlane(_robotInfo, out logic).Origin;
                                if (points[points.Count - 1] != lastPoint)
                                {
                                    points.Add(lastPoint);
                                }

                                // Add path curve to paths if there are at least two points in the list
                                if (points.Count > 1)
                                {
                                    _paths.Add(Curve.CreateInterpolatedCurve(points, 3));
                                }
                            }
                        }
                    }
                    #endregion

                    #endregion
                }
            }

            // Add the last movement / target plane and axis values
            if (movements[movements.Count - 1] is Movement) // Movement
            {
                movement = movements[movements.Count - 1] as Movement;
                InverseKinematics inverseKinematicsLast = new InverseKinematics(movement, _robotInfo);
                inverseKinematicsLast.Calculate();
                _internalAxisValues.Add(inverseKinematicsLast.InternalAxisValues);
                _externalAxisValues.Add(inverseKinematicsLast.ExternalAxisValues);
                _planes.Add(movement.GetPosedGlobalTargetPlane(_robotInfo, out logic));
            }

            else if (movements[movements.Count - 1] is AbsoluteJointMovement) // Joint movement
            {
                jointMovement = movements[movements.Count - 1] as AbsoluteJointMovement;
                forwardKinematics.Update(new List<double>(jointMovement.InternalAxisValues), new List<double>(jointMovement.ExternalAxisValues));
                forwardKinematics.Calculate();
                _internalAxisValues.Add(new List<double>(jointMovement.InternalAxisValues));
                _externalAxisValues.Add(new List<double>(jointMovement.ExternalAxisValues));
                _planes.Add(new Plane(forwardKinematics.TCPPlane));
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
            //List<Movement> movements = GetMovementsFromActions(actions);
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
                if (actions[i] is OverrideRobotTool)
                {
                    // Get the override robot tool object
                    OverrideRobotTool overrideRobotTool = (OverrideRobotTool)actions[i];

                    // Override the current tool
                    currentTool = overrideRobotTool.RobotTool.DuplicateWithoutMesh();
                }
                #endregion

                #region Check if the action is a movement
                else if (actions[i] is Movement)
                {
                    // Duplicate the movement since we might change properties
                    Movement movement = ((Movement)actions[i]).Duplicate();

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
                    else
                    {
                        movement.RobotTool.Mesh = new Mesh(); // save memory
                    }

                    // Add movement to list
                    result.Add(movement);
                }
                #endregion

                #region Check if the action is an absolute joint movement
                // Check if the action is a joint movement
                else if (actions[i] is AbsoluteJointMovement)
                {
                    // Duplicate the movement since we might change properties
                    AbsoluteJointMovement jointMovement = ((AbsoluteJointMovement)actions[i]).Duplicate();

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
                    else
                    {
                        jointMovement.RobotTool.Mesh = new Mesh(); // save memory
                    }

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

            // Clear the list with planes
            _planes.Clear();

            // Clear the list with path curves
            _paths.Clear();
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
        public RobotInfo RobotInfo
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
        #endregion
    }

}





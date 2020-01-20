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
        public void GetAxisValues(List<Movement> movements, int interpolations)
        {
            // Initialize the forward kinematics
            ForwardKinematics forwardKinematics = new ForwardKinematics(_robotInfo, true);

            // Initialize the inverse kinematics for the target to move to (movement 2)
            InverseKinematics inverseKinematics = new InverseKinematics(movements[0], _robotInfo);
            inverseKinematics.Calculate();

            // Initialize the internal and external axis values of the first target
            List<double> target1InternalAxisValues;
            List<double> target1ExternalAxisValues;

            // Initialize the internal and external axis values of the second target
            List<double> target2InternalAxisValues = inverseKinematics.InternalAxisValues;
            List<double> target2ExternalAxisValues = inverseKinematics.ExternalAxisValues;

            // Initialize other variables
            List<double> externalAxisValueDifferences = new List<double>();
            List<double> externalAxisValueChange = new List<double>();

            // Initiate sub inverse kinematics
            InverseKinematics inverseKinematicsSub = new InverseKinematics(movements[0], _robotInfo);
            Target subTarget;
            Movement subMovement;

            // Make path if we have at least two movements with targets
            if (movements.Count > 1)
            {
                for (int i = 0; i < movements.Count - 1; i++)
                {
                    // Points for path curve of this movement between two targets
                    List<Point3d> points = new List<Point3d>();

                    // Update tool of forward kinematics
                    forwardKinematics.RobotInfo.Tool = movements[i + 1].RobotTool;

                    // Our first target is the second target of the last movement
                    target1InternalAxisValues = new List<double>(target2InternalAxisValues);
                    target1ExternalAxisValues = new List<double>(target2ExternalAxisValues);

                    // Calculate the axis values for the second target
                    inverseKinematics.Movement = movements[i+1];
                    inverseKinematics.Calculate();
                    target2InternalAxisValues = inverseKinematics.InternalAxisValues;
                    target2ExternalAxisValues = inverseKinematics.ExternalAxisValues;

                    #region Calculate interpolated external axis values (needed for both joint and linear movement)
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
                    if (movements[i + 1].MovementType == 0 || movements[i + 1].MovementType == 2)
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
                        Point3d lastPoint = movements[i + 1].GlobalTargetPlane.Origin;
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
                        // Movement / target planes
                        Plane plane1 = movements[i].GlobalTargetPlane;
                        Plane plane2 = movements[i+1].GlobalTargetPlane;

                        // Main target plane position difference
                        Vector3d posDif = plane2.Origin - plane1.Origin;

                        // Main target plane position difference per interpolation step
                        Vector3d posChange = posDif / interpolations;

                        // Main target planes axis differences
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
                            List<Vector3d> axisDir= new List<Vector3d>() { };
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

                            // Create new plane
                            Plane plane = new Plane(planePoints[l], axisDirections[l][0], axisDirections[l][1]);

                            // Update sub target and sub movement
                            subTarget = new Target("subTarget", plane, movements[i + 1].Target.AxisConfig, externalAxisValues);
                            subMovement = new Movement(subTarget);
                            subMovement.RobotTool = movements[i + 1].RobotTool;

                            // Calculate internal axis values
                            inverseKinematicsSub.Movement = subMovement;
                            inverseKinematicsSub.Calculate();
                            List<double> internalAxisValues = inverseKinematicsSub.InternalAxisValues;

                            // Add te calculated axis values and plane to the class property
                            _internalAxisValues.Add(new List<double>(internalAxisValues));
                            _externalAxisValues.Add(new List<double>(externalAxisValues));
                            _planes.Add(plane);
                        }

                        // Start and end point of straight line
                        points.Add(plane1.Origin);
                        points.Add(plane2.Origin);

                        // Add path curve to paths
                        if (plane1.Origin != plane2.Origin)
                        {
                            _paths.Add(Curve.CreateInterpolatedCurve(points, 3));
                        }
                    }
                    #endregion 
                }
            }

            // Add the last movement / target plane and axis values
            InverseKinematics inverseKinematicsLast = new InverseKinematics(movements[movements.Count - 1], _robotInfo);
            inverseKinematicsLast.Calculate();
            _internalAxisValues.Add(inverseKinematicsLast.InternalAxisValues);
            _externalAxisValues.Add(inverseKinematicsLast.ExternalAxisValues);
            _planes.Add(movements[movements.Count - 1].GlobalTargetPlane);
        }

        /// <summary>
        /// Calculates the path and the axis values from a list with actions. 
        /// The momvents are automatically distracted from the list with actions. 
        /// </summary>
        /// <param name="actions"> The list with robot actions </param>
        /// <param name="interpolations"> The interpolation number. Defines the amount of sub targets between two targets. </param>
        public void Calculate(List<Action> actions, int interpolations)
        {
            // Clear all the lists with values
            Reset();

            // Get the movements from the list with actions
            List<Movement> movements = GetMovementsFromActions(actions);

            // Get the internal and external axis values and the path curve
            GetAxisValues(movements, interpolations);
        }

        /// <summary>
        /// Get the movements from the list with actions to generate the path.
        /// The robot tool will be set at the movement object by checking if the override robot tool action is used.
        /// </summary>
        /// <param name="actions"> The list with robot actions. </param>
        /// <returns> The list with robot movements and the correct tool for this movement. </returns>
        private List<Movement> GetMovementsFromActions(List<Action> actions)
        {
            // Initiate list
            List<Movement> movements = new List<Movement>();

            // Iniate current tool (the tool attached to the robot)
            RobotTool currentTool = _robotInfo.Tool.Duplicate();
            currentTool.Mesh = new Mesh(); // save memory

            // Initiate override robot tool
            OverrideRobotTool overrideRobotTool;

            // Loop over all the actions
            for (int i = 0; i < actions.Count; i++)
            {
                // Check if the Override Robot Tool actions is used
                if (actions[i] is OverrideRobotTool)
                {
                    // Get the override robot tool object
                    overrideRobotTool = (OverrideRobotTool)actions[i];

                    // Override the current tool
                    currentTool = overrideRobotTool.RobotTool.Duplicate();
                    currentTool.Mesh = new Mesh(); // save memory
                }

                // Check if the action is a movement
                else if (actions[i] is Movement)
                {
                    // Duplicate the movement since we might change properties
                    Movement movement = ((Movement)actions[i]).Duplicate();

                    // Set the current tool if no tool is set in the movement object
                    if (movement.RobotTool.Name == "" || movement.RobotTool.Name == null)
                    {
                        movement.RobotTool = currentTool.Duplicate();
                    }

                    // Otherwise don't set a tool. Last overwrite is used that is combined with the movement.
                    else
                    {
                        movement.RobotTool.Mesh = new Mesh(); // save memory
                    }

                    // Add movement to list
                    movements.Add(movement);
                }
            }

            // Return list with movements
            return movements;
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





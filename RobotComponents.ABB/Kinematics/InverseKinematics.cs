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
using RobotComponents.Generic.Kinematics;
using RobotComponents.ABB.Actions.Declarations;
using RobotComponents.ABB.Actions.Interfaces;
using RobotComponents.ABB.Actions.Instructions;
using RobotComponents.ABB.Definitions;
using RobotComponents.ABB.Kinematics.IKFast;

namespace RobotComponents.ABB.Kinematics
{
    /// <summary>
    /// Represent the Inverse Kinematics for a 6-axis spherical Robot and its attached external axes.
    /// </summary>
    public class InverseKinematics
    {
        #region fields
        private Robot _robot;
        private RobotTool _robotTool;
        private Movement _movement;
        private Plane _positionPlane;
        private Plane _globalTargetPlane;
        private Plane _globalEndPlane;
        private Plane _localEndPlane;
        private bool _inLimits = true;
        private bool _elbowSingularity = false;
        private bool _wristSingularity = false;
        private readonly List<string> _errorText = new List<string>();
        private readonly RobotJointPosition[] _robotJointPositions = new RobotJointPosition[8];
        private RobotJointPosition _robotJointPosition = new RobotJointPosition();
        private ExternalJointPosition _externalJointPosition = new ExternalJointPosition();

        // Constants
        private const double _pi = Math.PI;
        private const double _rad2deg = 180.0 / _pi;

        // OPW kinematics solver fields
        private readonly int[] _order = new int[8] { 0, 4, 1, 5, 2, 6, 3, 7 };
        private OPWKinematics _opw = new OPWKinematics();

        //IKFast solver fields
        private IKFastSolver _ikfast;
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the Inverse Kinematics class.
        /// </summary>
        public InverseKinematics()
        {
            for (int i = 0; i < 8; i++)
            {
                _robotJointPositions[i] = new RobotJointPosition();
            }
        }

        /// <summary>
        /// Initializes a new instance of the Inverse Kinematics.
        /// </summary>
        /// <param name="robot"> The Robot. </param>
        public InverseKinematics(Robot robot)
        {
            _robot = robot;
            _movement = new Movement(Plane.WorldXY);

            for (int i = 0; i < 8; i++)
            {
                _robotJointPositions[i] = new RobotJointPosition();
            }

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the Inverse Kinematics class from a Movement.
        /// </summary>
        /// <param name="robot"> The Robot. </param>
        /// <param name="movement"> The Movement. </param>
        public InverseKinematics(Robot robot, Movement movement)
        {
            _robot = robot;
            _movement = movement;

            for (int i = 0; i < 8; i++)
            {
                _robotJointPositions[i] = new RobotJointPosition();
            }

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the Inverse Kinematics class from a Target.
        /// </summary>
        /// <remarks>
        /// The target will be casted to a robot movement with a default work object (wobj0).
        /// </remarks>
        /// <param name="robot"> The Robot. </param>
        /// <param name="target"> The Target </param>
        public InverseKinematics(Robot robot, ITarget target)
        {
            _robot = robot;
            _movement = new Movement(target);

            for (int i = 0; i < 8; i++)
            {
                _robotJointPositions[i] = new RobotJointPosition();
            }

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the Inverse Kinematics class from a Movement.
        /// </summary>
        /// <param name="movement"> The Movement. </param>
        /// <param name="robot"> The Robot. </param>
        [Obsolete("This constructor is OBSOLETE and will be removed in v3.", false)]
        public InverseKinematics(Movement movement, Robot robot)
        {
            _robot = robot;
            _movement = movement;

            for (int i = 0; i < 8; i++)
            {
                _robotJointPositions[i] = new RobotJointPosition();
            }

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the Inverse Kinematics class from a Target.
        /// </summary>
        /// <remarks>
        /// The target will be casted to a robot movement with a default work object (wobj0).
        /// </remarks>
        /// <param name="target"> The Target </param>
        /// <param name="robot"> The Robot. </param>
        [Obsolete("This constructor is OBSOLETE and will be removed in v3.", false)]
        public InverseKinematics(ITarget target, Robot robot)
        {
            _robot = robot;
            _movement = new Movement(target);

            for (int i = 0; i < 8; i++)
            {
                _robotJointPositions[i] = new RobotJointPosition();
            }

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the Inverse Kinematics class by duplicating an existing Inverse Kinematics instance. 
        /// </summary>
        /// <param name="inverseKinematics"> The Inverse Kinematics instance to duplicate. </param>
        public InverseKinematics(InverseKinematics inverseKinematics)
        {
            _robot = inverseKinematics.Robot.Duplicate();
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
        /// Returns an exact duplicate of this Inverse Kinematics instance.
        /// </summary>
        /// <returns> A deep copy of the Inverse Kinematics instance. </returns>
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
            if (!IsValid)
            {
                return "Invalid Inverse Kinematics";
            }
            else
            {
                return "Inverse Kinematics";
            }
        }

        /// <summary>
        /// Initialize the fields to construct a valid Inverse Kinematics instance. 
        /// </summary>
        private void Initialize()
        {
            _robotTool = _movement.RobotTool.Name == "" ? _robot.Tool : _movement.RobotTool;

            if (_movement.Target is RobotTarget)
            {
                _globalTargetPlane = _movement.GetPosedGlobalTargetPlane();
                GetPositionPlane();
                GetEndPlanes();
            }

            _opw.Signs = new int[6] { 1, 1, 1, 1, 1, 1 };
            _opw.Offsets = new double[6] { 0, 0, -_pi / 2, 0, 0, 0 };
            _opw.A1 = _robot.A1;
            _opw.A2 = _robot.A2;
            _opw.B = _robot.B;
            _opw.C1 = _robot.C1;
            _opw.C2 = _robot.C2;
            _opw.C3 = _robot.C3;
            _opw.C4 = _robot.C4;

            _ikfast = new IKFastSolver(_robot);
        }

        /// <summary>
        /// Reinitialize all the fields to construct a valid Inverse Kinematics object. 
        /// </summary>
        /// <remarks>
        /// This method also resets the solution. The method Calculate() has to be called to obtain a new solution. 
        /// </remarks>
        public void ReInitialize()
        {
            Initialize();
            ClearCurrentSolutions();
        }

        /// <summary>
        /// Calculates the inverse kinematics solution.
        /// </summary>
        public void Calculate()
        {
            ClearCurrentSolutions();
            CalculateRobotJointPosition();
            CalculateExternalJointPosition();
            CheckInternalAxisLimits();
            CheckExternalAxisLimits();
        }

        /// <summary>
        /// Calculates the Robot Joint Position of the Inverse Kinematics solution.
        /// </summary>
        /// <remarks>
        /// This method does not check the internal axis limits.
        /// </remarks>
        public void CalculateRobotJointPosition()
        {
            ResetRobotJointPositions();

            if (_movement.Target is RobotTarget robotTarget)
            {
                if (_robot.Name == "CRB15000-5/0.95")
                {
                    try
                    {
                        _ikfast.Compute_CRB15000_5_095(_localEndPlane);
                        _ikfast.SortJointPositions();

                        #region DEBUG: Write results in Rhino command line
                        Rhino.RhinoApp.WriteLine($"IKFast found {_ikfast.NumSolutions} solutions (joint positions in deg).");

                        for (int i = 0; i < _ikfast.RobotJointPositions.Count; i++)
                        {
                            Rhino.RhinoApp.WriteLine(_ikfast.RobotJointPositions[i].ToString());
                        }

                        Rhino.RhinoApp.WriteLine("");
                        #endregion

                    }
                    catch (Exception e)
                    {
                        _errorText.Add(e.Message);
                    }

                    // Copy over the solutions
                    for (int i = 0; i < 8; i++)
                    {
                        _robotJointPositions[i] = _ikfast.RobotJointPositions[i];
                    }

                    _robotJointPosition = _robotJointPositions[robotTarget.ConfigurationData.Cfx];
                    _wristSingularity = false;
                    _elbowSingularity = false;
                }
                else if(_robot.Name == "CRB15000-10/152")
                {
                    try
                    {
                        _ikfast.Compute_CRB15000_10_152(_localEndPlane);

                        #region DEBUG: Write results in Rhino command line
                        Rhino.RhinoApp.WriteLine($"IKFast found {_ikfast.NumSolutions} solutions (joint positions in deg).");

                        for (int i = 0; i < _ikfast.RobotJointPositions.Count; i++)
                        {
                            Rhino.RhinoApp.WriteLine(_ikfast.RobotJointPositions[i].ToString());
                        }

                        Rhino.RhinoApp.WriteLine("");
                        #endregion

                    }
                    catch (Exception e)
                    {
                        _errorText.Add(e.Message);
                    }

                    // Copy over the solutions
                    for (int i = 0; i < 8; i++)
                    {
                        _robotJointPositions[i] = _ikfast.RobotJointPositions[i];
                    }

                    _robotJointPosition = _robotJointPositions[robotTarget.ConfigurationData.Cfx];
                    _wristSingularity = false;
                    _elbowSingularity = false;
                }
                else
                {
                    // Calculate inverse kinematics
                    _opw.Inverse(_localEndPlane);

                    // Set Robot Joint Positions
                    for (int i = 0; i < 8; i++)
                    {
                        for (int j = 0; j < 6; j++)
                        {
                            _robotJointPositions[i][j] = _rad2deg * _opw.Solutions[_order[i]][j];
                        }
                    }

                    // Select solution
                    _robotJointPosition = _robotJointPositions[robotTarget.ConfigurationData.Cfx];
                    _wristSingularity = _opw.IsWristSingularity[_order[robotTarget.ConfigurationData.Cfx]];
                    _elbowSingularity = _opw.IsElbowSingularity[_order[robotTarget.ConfigurationData.Cfx]];
                }
            }

            else if (_movement.Target is JointTarget jointTarget)
            {
                _robotJointPosition = jointTarget.RobotJointPosition;
            }

            else
            {
                _robotJointPosition = new RobotJointPosition();
            }
        }

        /// <summary>
        /// Calculates and returns the closest Robot Joint Position to a given previous Robot Joint Position.
        /// </summary>
        /// <remarks>
        /// This methods sets and returns the closest Robot Joint Poistion insides this Inverse Kinematics object. 
        /// You first have to calculate the Inverse Kinematics solution before you call this method. 
        /// This method is typically used for using Linear and Joint Configuration control inside the Path Generator.
        /// </remarks>
        /// <param name="prevJointPosition"> The previous Robot Joint Position. </param>
        /// <returns> The closest Robot Joint Position. </returns>
        public RobotJointPosition CalculateClosestRobotJointPosition(RobotJointPosition prevJointPosition)
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
                    if (_robot.InternalAxisLimits[3].IncludesParameter(_robotJointPositions[i][3] + joint4[j], false) == true)
                    {
                        // Add value to axis 4
                        _robotJointPositions[i][3] += joint4[j];

                        for (int k = 0; k < joint6.Length; k++)
                        {
                            // Check axis 6
                            if (_robot.InternalAxisLimits[5].IncludesParameter(_robotJointPositions[i][5] + joint6[k], false) == true)
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
                                    _wristSingularity = _opw.IsWristSingularity[_order[i]];
                                    _elbowSingularity = _opw.IsElbowSingularity[_order[i]];
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

            // Check axis limits
            _errorText.Clear();
            CheckInternalAxisLimits();
            CheckExternalAxisLimits();

            return _robotJointPosition;
        }

        /// <summary>
        /// Calculates the External Joint Position of the Inverse Kinematics solution.
        /// </summary>
        /// <remarks>
        /// This method does not check the external axis limits. 
        /// </remarks>
        public void CalculateExternalJointPosition()
        {
            // Clear current solution
            ResetExternalJointPosition();

            if (_movement.Target is RobotTarget robotTarget)
            {
                // NOTE: Only works for a robot with one external axis that moves the robot.
                double count = 0; // Counts the number of external axes that move the robot.

                // Calculates the position of the external axes for each external axis
                for (int i = 0; i < _robot.ExternalAxes.Count; i++)
                {
                    ExternalAxis externalAxis = _robot.ExternalAxes[i];
                    Interval axisLimits = _robot.ExternalAxes[i].AxisLimits;
                    int logic = externalAxis.AxisNumber;

                    // External Linear axis
                    if (externalAxis is ExternalLinearAxis externalLinearAxis)
                    {
                        // External Linear Axis that moves the robot
                        if (externalLinearAxis.MovesRobot == true && count == 0)
                        {
                            // Checks if the position the external linear axis needs to be negative or positive
                            externalLinearAxis.AxisCurve.ClosestPoint(_robot.BasePlane.Origin, out double robotBasePlaneParam);
                            externalLinearAxis.AxisCurve.ClosestPoint(_positionPlane.Origin, out double basePlaneParam);

                            if (basePlaneParam >= robotBasePlaneParam)
                            {
                                _externalJointPosition[logic] = _positionPlane.Origin.DistanceTo(_robot.BasePlane.Origin);
                            }

                            else
                            {
                                _externalJointPosition[logic] = -_positionPlane.Origin.DistanceTo(_robot.BasePlane.Origin);
                            }

                            count += 1;
                        }

                        // External Linear axis that does not move the robot
                        else
                        {
                            if (robotTarget.ExternalJointPosition[logic] == 9e9)
                            {
                                _externalJointPosition[logic] = Math.Max(0, Math.Min(axisLimits.Min, axisLimits.Max));
                            }
                            else
                            {
                                _externalJointPosition[logic] = robotTarget.ExternalJointPosition[logic];
                            }
                        }
                    }

                    // External Rotational Axis
                    else if (externalAxis is ExternalRotationalAxis)
                    {
                        if (robotTarget.ExternalJointPosition[logic] == 9e9)
                        {
                            _externalJointPosition[logic] = Math.Max(0, Math.Min(axisLimits.Min, axisLimits.Max));
                        }
                        else
                        {
                            _externalJointPosition[logic] = robotTarget.ExternalJointPosition[logic];
                        }
                    }

                    // Other External Axis types
                    else
                    {
                        if (robotTarget.ExternalJointPosition[logic] == 9e9)
                        {
                            _externalJointPosition[logic] = Math.Max(0, Math.Min(axisLimits.Min, axisLimits.Max));
                        }
                        else
                        {
                            _externalJointPosition[logic] = robotTarget.ExternalJointPosition[logic];
                        }
                    }
                }

                // Copy name
                _externalJointPosition.Name = _movement.Target.ExternalJointPosition.Name;
            }

            // Joint Target
            else
            {
                _externalJointPosition = _movement.Target.ExternalJointPosition.Duplicate();
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
            _wristSingularity = false;
            _elbowSingularity = false;
        }

        /// <summary>
        /// Resets the Robot Joint Position solutions
        /// </summary>
        private void ResetRobotJointPositions()
        {
            for (int i = 0; i < 8; i++)
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
        /// Calculates the end planes of joint 6.
        /// </summary>
        private void GetEndPlanes()
        {
            _globalEndPlane = new Plane(_robotTool.AttachmentPlane);
            Transform trans1 = Transform.PlaneToPlane(_robotTool.ToolPlane, _globalTargetPlane);
            _globalEndPlane.Transform(trans1);

            _localEndPlane = new Plane(_globalEndPlane);
            Transform trans2 = Transform.PlaneToPlane(_positionPlane, Plane.WorldXY);
            _localEndPlane.Transform(trans2);
        }

        /// <summary>
        /// Calculates the base position of the robot in world coordinate space if it is moved by an external axis.
        /// </summary>
        private void GetPositionPlane()
        {
            // NOTE: Only works for a robot with an maximum of one external linear axis

            // Deep copy the current position / base plane
            _positionPlane = new Plane(_robot.BasePlane);

            // Check if an external axis is attached to the robot 
            for (int i = 0; i < _robot.ExternalAxes.Count; i++)
            {
                ExternalAxis externalAxis = _robot.ExternalAxes[i];
                int logic = externalAxis.AxisNumber;

                // Moves Robot?
                if (externalAxis.MovesRobot == true)
                {
                    // An External Linear Axis that moves the robot
                    if (externalAxis is ExternalLinearAxis externalLinearAxis)
                    {
                        if (_movement.Target.ExternalJointPosition[logic] == 9e9)
                        {
                            // Alternative strategy: take the global end plane

                            externalLinearAxis.AxisCurve.ClosestPoint(_globalTargetPlane.Origin, out double param);
                            _positionPlane.Origin = externalLinearAxis.AxisCurve.PointAt(param);
                        }

                        else
                        {
                            _positionPlane = externalLinearAxis.CalculatePosition(_movement.Target.ExternalJointPosition, out _);
                        }
                    }

                    // An External Rotational Axis that moves the robot
                    else if (externalAxis is ExternalRotationalAxis externalRotationalAxis)
                    {
                        _positionPlane = externalRotationalAxis.CalculatePosition(_movement.Target.ExternalJointPosition, out _);
                    }

                    // Other External Axis types that move the robot
                    else
                    {
                        _positionPlane = externalAxis.CalculatePosition(_movement.Target.ExternalJointPosition, out _);
                    }

                    // Break the loop since one axternal axis can move the robot.
                    break;
                }
            }
        }

        /// <summary>
        /// Checks if the positions of the robot axes are inside its limits.
        /// </summary>
        private void CheckInternalAxisLimits()
        {
            for (int i = 0; i < _robotJointPosition.Length; i++)
            {
                if (_robot.InternalAxisLimits[i].IncludesParameter(_robotJointPosition[i], false) == false)
                {
                    _errorText.Add($"Movement {_movement.Target.Name}\\{_movement.WorkObject.Name}: The position of robot axis {i + 1} is not in range.");
                    _inLimits = false;
                }
            }

            if (_wristSingularity == true)
            {
                _errorText.Add($"Movement {_movement.Target.Name}\\{_movement.WorkObject.Name}: The target is close to wrist singularity.");
            }

            if (_elbowSingularity == true)
            {
                _errorText.Add($"Movement {_movement.Target.Name}\\{_movement.WorkObject.Name}: The target is out of reach (elbow singularity).");
                _inLimits = false;
            }
        }

        /// <summary>
        /// Checks if the positions of the external axes are inside its limits.
        /// </summary>
        private void CheckExternalAxisLimits()
        {
            for (int i = 0; i < _robot.ExternalAxes.Count; i++)
            {
                int number = _robot.ExternalAxes[i].AxisNumber;
                char logic = _robot.ExternalAxes[i].AxisLogic;

                if (_robot.ExternalAxes[i].AxisLimits.IncludesParameter(_externalJointPosition[number], false) == false)
                {
                    _errorText.Add($"Movement {_movement.Target.Name}\\{_movement.WorkObject.Name}: The position of external logical axis {logic} is not in range.");
                    _inLimits = false;
                }
            }
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
                if (_movement == null) { return false; }
                if (_movement.IsValid == false) { return false; }
                return true;
            }
        }

        /// <summary>
        /// Gets or sets the Robot.
        /// </summary>
        public Robot Robot
        {
            get
            {
                return _robot;
            }
            set
            {
                _robot = value;
                ReInitialize();
            }
        }

        /// <summary>
        /// Gets or sets the Movement.
        /// </summary>
        /// <remarks>
        /// The target and work object are obtained from this movement.
        /// </remarks>
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
        /// Gets the tool used by the this Inverse Kinematics.
        /// </summary>
        /// <remarks>
        /// By default the tool attached to the robot is used. 
        /// If a tool is set as a property of the movement, this tool will be used. 
        /// </remarks>
        public RobotTool RobotTool
        {
            get { return _robotTool; }
        }

        /// <summary>
        /// Gets the eight latest calculated Robot Joint Positions.
        /// </summary>
        public List<RobotJointPosition> RobotJointPositions
        {
            get { return _robotJointPositions.ToList(); }
        }

        /// <summary>
        /// Gets the latest calculated Robot Joint Position.
        /// </summary>
        public RobotJointPosition RobotJointPosition
        {
            get { return _robotJointPosition; }
        }

        /// <summary>
        /// Gets the latest calculated External Joint Position.
        /// </summary>
        public ExternalJointPosition ExternalJointPosition
        {
            get { return _externalJointPosition; }
        }

        /// <summary>
        /// Gets the collected error messages.
        /// </summary>
        public List<string> ErrorText
        {
            get { return _errorText; }
        }

        /// <summary>
        /// Gets a value indicating whether or not the internal and external values are within their limits.
        /// </summary>
        public bool InLimits
        {
            get { return _inLimits; }
        }
        #endregion
    }
}
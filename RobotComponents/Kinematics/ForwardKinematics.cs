// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.Actions;
using RobotComponents.Definitions;
using System;
using System.Collections.Generic;

namespace RobotComponents.Kinematics
{
    /// <summary>
    /// Represent the Forward Kinematics for a 6-axis spherical Robot and its attached external axes. 
    /// </summary>
    public class ForwardKinematics
    {
        #region fields
        private Robot _robot; // Robot info
        private Plane _positionPlane = Plane.Unset; // Robot Position Plane: needed for external linear axis
        private Plane[] _posedExternalAxisPlanes; // External Axis Planes 
        private readonly List<string> _errorText = new List<string>(); // Error text
        private List<Mesh> _posedInternalAxisMeshes = new List<Mesh>(); // Posed Robot Meshes
        private List<List<Mesh>> _posedExternalAxisMeshes = new List<List<Mesh>>(); //Posed Axis Meshes
        private Plane _tcpPlane = Plane.Unset; // TCP Plane of end effector
        private bool _inLimits = true; // Indicates if the joint positions are in limits 
        private bool _hideMesh;
        private RobotJointPosition _robotJointPosition;
        private ExternalJointPosition _externalJointPosition;
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the Forward Kinematics class.
        /// </summary>
        public ForwardKinematics()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Forward Kinematics class.
        /// </summary>
        /// <param name="robot"> The Robot. </param>
        /// <param name="hideMesh"> Specifies whether the mesh will be supressed. </param>
        public ForwardKinematics(Robot robot, bool hideMesh = false)
        {
            _robot = robot;
            _hideMesh = hideMesh;
        }

        /// <summary>
        /// Initializes a new instance of the Forward Kinematics class with defined Joint Positions.
        /// </summary>
        /// <param name="robot"> The Robot. </param>
        /// <param name="robotJointPosition"> The Robot Joint Position. </param>
        /// <param name="externalJointPosition"> The External Joint Position. </param>
        /// <param name="hideMesh"> Specifies whether the mesh will be supressed. </param>
        public ForwardKinematics(Robot robot, RobotJointPosition robotJointPosition, ExternalJointPosition externalJointPosition, bool hideMesh = false)
        {
            _robot = robot;
            _hideMesh = hideMesh;
            _robotJointPosition = robotJointPosition;
            _externalJointPosition = externalJointPosition;
        }

        /// <summary>
        /// Defines a Forward Kinematic object from a Joint Target with defined Joint Positions obtained from a Joint Target.
        /// </summary>
        /// <param name="robot"> The Robot. </param>
        /// <param name="jointTarget"> The Joint Target. </param>
        /// <param name="hideMesh"> Specifies whether the mesh will be supressed. </param>
        public ForwardKinematics(Robot robot, JointTarget jointTarget, bool hideMesh = false)
        {
            _robot = robot;
            _hideMesh = hideMesh;
            _robotJointPosition = jointTarget.RobotJointPosition;
            _externalJointPosition = jointTarget.ExternalJointPosition;
        }

        /// <summary>
        /// Initializes a new instance of the Forward Kinematics class by duplicating an existing Forward Kinematics instance. 
        /// </summary>
        /// <param name="forwardKinematics"> The Forward Kinematics instance to duplicate. </param>
        public ForwardKinematics(ForwardKinematics forwardKinematics)
        {
            _robot = forwardKinematics.Robot.Duplicate();
            _hideMesh = forwardKinematics.HideMesh;
            _robotJointPosition = forwardKinematics.RobotJointPosition.Duplicate();
            _externalJointPosition = forwardKinematics.ExternalJointPosition.Duplicate();
            _tcpPlane = new Plane(forwardKinematics.TCPPlane);
            _errorText = new List<string>(forwardKinematics.ErrorText);
            _posedInternalAxisMeshes = forwardKinematics.PosedInternalAxisMeshes.ConvertAll(mesh => mesh.DuplicateMesh());
            _posedExternalAxisMeshes = new List<List<Mesh>>(forwardKinematics.PosedExternalAxisMeshes);
            for (int i = 0; i < forwardKinematics.PosedExternalAxisMeshes.Count; i++)
            {
                for (int j = 0; j < forwardKinematics.PosedExternalAxisMeshes[i].Count; j++)
                {
                    forwardKinematics.PosedExternalAxisMeshes[i][j] = forwardKinematics.PosedExternalAxisMeshes[i][j].DuplicateMesh();
                }
            }
            _posedExternalAxisPlanes = new List<Plane>(forwardKinematics.PosedExternalAxisPlanes).ToArray();
            _inLimits = forwardKinematics.InLimits;
        }


        /// <summary>
        /// Returns an exact duplicate of this Forward Kinematics instance.
        /// </summary>
        /// <returns> A deep copy of the Forward Kinematics instance. </returns>
        public ForwardKinematics Duplicate()
        {
            return new ForwardKinematics(this);
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
                return "Invalid Forward Kinematics";
            }
            else
            {
                return "Forward Kinematics";
            }
        }

        /// <summary>
        /// Calculates the forward kinematics solution with the Joint Positions stored inside this Forward Kinematics instance.
        /// </summary>
        public void Calculate()
        {
            // Clear current solution
            Clear();

            // Check axis limits
            CheckInternalAxisLimits();
            CheckExternalAxisLimits();

            // Deep copy the mehses if the pose should be calculated
            if (_hideMesh == false)
            {
                _posedInternalAxisMeshes = _robot.Meshes.ConvertAll(mesh => mesh.DuplicateMesh());
                _posedExternalAxisMeshes = new List<List<Mesh>>();
            }

            // Get initial position plane
            _positionPlane = new Plane(_robot.BasePlane);

            // Count the number of external linear axes that is used: it is now limited to one
            double count = 0;

            // Calculates external axes positions
            _posedExternalAxisPlanes = new Plane[_robot.ExternalAxes.Count];

            for (int i = 0; i < _robot.ExternalAxes.Count; i++)
            {
                // Get the external axis
                ExternalAxis externalAxis = _robot.ExternalAxes[i];
                int logic = _robot.ExternalAxes[i].AxisNumber;

                // Get external axis plane
                _posedExternalAxisPlanes[i] = externalAxis.CalculatePositionSave(_externalJointPosition);

                // Check if an external axis moves the robot and calculate the position plane.
                if (externalAxis.MovesRobot == true && count == 0)
                {
                    _positionPlane = externalAxis.CalculatePosition(_externalJointPosition, out bool inLimits);
                    count += 1;
                }
            }

            // Move relative to base
            Transform transNow;
            transNow = Transform.PlaneToPlane(_robot.BasePlane, _positionPlane);

            // Calculates internal axes
            // First caculate all tansformations (rotations)
            // Axis 1
            Transform rot1;
            Plane planeAxis1 = new Plane(_robot.InternalAxisPlanes[0]);
            rot1 = Transform.Rotation(_robotJointPosition[0] * Math.PI / 180, planeAxis1.ZAxis, planeAxis1.Origin);
            // Axis 2
            Transform rot2;
            Plane planeAxis2 = new Plane(_robot.InternalAxisPlanes[1]);
            planeAxis2.Transform(rot1);
            rot2 = Transform.Rotation(_robotJointPosition[1] * Math.PI / 180, planeAxis2.ZAxis, planeAxis2.Origin);
            // Axis 3
            Transform rot3;
            Plane planeAxis3 = new Plane(_robot.InternalAxisPlanes[2]);
            planeAxis3.Transform(rot2 * rot1);
            rot3 = Transform.Rotation(_robotJointPosition[2] * Math.PI / 180, planeAxis3.ZAxis, planeAxis3.Origin);
            // Axis 4
            Transform rot4;
            Plane planeAxis4 = new Plane(_robot.InternalAxisPlanes[3]);
            planeAxis4.Transform(rot3 * rot2 * rot1);
            rot4 = Transform.Rotation(_robotJointPosition[3] * Math.PI / 180, planeAxis4.ZAxis, planeAxis4.Origin);
            // Axis 5
            Transform rot5;
            Plane planeAxis5 = new Plane(_robot.InternalAxisPlanes[4]);
            planeAxis5.Transform(rot4 * rot3 * rot2 * rot1);
            rot5 = Transform.Rotation(_robotJointPosition[4] * Math.PI / 180, planeAxis5.ZAxis, planeAxis5.Origin);
            // Axis 6
            Transform rot6;
            Plane planeAxis6 = new Plane(_robot.InternalAxisPlanes[5]);
            planeAxis6.Transform(rot5 * rot4 * rot3 * rot2 * rot1);
            rot6 = Transform.Rotation(_robotJointPosition[5] * Math.PI / 180, planeAxis6.ZAxis, planeAxis6.Origin);

            // Apply transformations on tcp plane
            _tcpPlane = new Plane(_robot.ToolPlane);
            _tcpPlane.Transform(transNow * rot6 * rot5 * rot4 * rot3 * rot2 * rot1);

            // Get posed meshes
            if (_hideMesh == false)
            {
                // Base link transform
                _posedInternalAxisMeshes[0].Transform(transNow);
                // Link_1 tranform 
                _posedInternalAxisMeshes[1].Transform(transNow * rot1);
                // Link_2 tranform
                _posedInternalAxisMeshes[2].Transform(transNow * rot2 * rot1);
                // Link_3 tranform
                _posedInternalAxisMeshes[3].Transform(transNow * rot3 * rot2 * rot1);
                // Link_4 tranform
                _posedInternalAxisMeshes[4].Transform(transNow * rot4 * rot3 * rot2 * rot1);
                // Link_5 tranform
                _posedInternalAxisMeshes[5].Transform(transNow * rot5 * rot4 * rot3 * rot2 * rot1);
                // Link_6 tranform
                _posedInternalAxisMeshes[6].Transform(transNow * rot6 * rot5 * rot4 * rot3 * rot2 * rot1);
                // End-effector transform
                _posedInternalAxisMeshes[7].Transform(transNow * rot6 * rot5 * rot4 * rot3 * rot2 * rot1);

                // External axis meshes
                for (int i = 0; i < _robot.ExternalAxes.Count; i++)
                {
                    _robot.ExternalAxes[i].PoseMeshes(_externalJointPosition);
                    _posedExternalAxisMeshes.Add(_robot.ExternalAxes[i].PosedMeshes.ConvertAll(mesh => mesh.DuplicateMesh()));
                }
            }
        }

        /// <summary>
        /// Calculates the forward kinematics solution with the given Robot Joint Positions and a default External Joint Position (9e9).
        /// </summary>
        /// <param name="robotJointPosition"> The  Robot Joint Position. </param>
        public void Calculate(RobotJointPosition robotJointPosition)
        {
            _robotJointPosition = robotJointPosition;
            _externalJointPosition = new ExternalJointPosition();
            Calculate();
        }

        /// <summary>
        /// Calculates the forward kinematics solution with the given Joint Positions.
        /// </summary>
        /// <param name="robotJointPosition"> The  Robot Joint Position. </param>
        /// <param name="externalJointPosition"> The External Joint Position. </param>
        public void Calculate(RobotJointPosition robotJointPosition, ExternalJointPosition externalJointPosition)
        {
            _robotJointPosition = robotJointPosition;
            _externalJointPosition = externalJointPosition;
            Calculate();
        }

        /// <summary>
        /// Clears the current solution.
        /// </summary>
        public void Clear()
        {
            _errorText.Clear();
            _posedInternalAxisMeshes.Clear();
            for (int i = 0; i < _posedExternalAxisMeshes.Count; i++)
            {
                _posedExternalAxisMeshes[i].Clear();
            }
            _posedExternalAxisMeshes.Clear();
            _positionPlane = Plane.Unset;
            _tcpPlane = Plane.Unset;
            _inLimits = true;
        }

        /// <summary>
        /// Checks if the positions of the robot axes are inside its limits.
        /// </summary>
        private void CheckInternalAxisLimits()
        {
            for (int i = 0; i < 6; i++)
            {
                if (_robot.InternalAxisLimits[i].IncludesParameter(_robotJointPosition[i], false) == false)
                {
                    _errorText.Add("The position of robot axis " + (i + 1).ToString() + " is not in range.");
                    _inLimits = false;
                }
            }
        }

        /// <summary>
        /// Checks if the positions of the external logical axes are inside its limits.
        /// </summary>
        private void CheckExternalAxisLimits()
        {
            for (int i = 0; i < _robot.ExternalAxes.Count; i++)
            {
                int index = _robot.ExternalAxes[i].AxisNumber;
                char logic = _robot.ExternalAxes[i].AxisLogic;

                if (_externalJointPosition[index] == 9e9)
                {
                    _errorText.Add("The position of external logical axis " + logic + " is not definied.");
                    _inLimits = false;
                }

                else if (_robot.ExternalAxes[i].AxisLimits.IncludesParameter(_externalJointPosition[index], false) == false)
                {
                    _errorText.Add("The position of external logical axis  " + logic + " is not in range.");
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
                if (Robot == null) { return false; }
                if (Robot.IsValid == false) { return false; }
                if (RobotJointPosition == null) { return false; }
                if (RobotJointPosition.IsValid == false) { return false; }
                if (ExternalJointPosition == null) { return false; }
                if (ExternalJointPosition.IsValid == false) { return false; }
                return true;
            }
        }

        /// <summary>
        /// Gets or sets the Robot.
        /// </summary>
        public Robot Robot
        {
            get { return _robot; }
            set { _robot = value; }
        }

        /// <summary>
        /// Gets the latest calculated posed internal axis meshes.
        /// </summary>
        public List<Mesh> PosedInternalAxisMeshes
        {
            get { return _posedInternalAxisMeshes; }
        }

        /// <summary>
        /// Gets the latest calculated posed external axis meshes.
        /// </summary>
        public List<List<Mesh>> PosedExternalAxisMeshes
        {
            get { return _posedExternalAxisMeshes; }
        }

        /// <summary>
        /// Gets the latest calculated posed TCP plane of the robot tool. 
        /// </summary>
        public Plane TCPPlane
        {
            get { return _tcpPlane; }
        }

        /// <summary>
        /// Gets a value indicating whether or not the internal and external values are within their limits.
        /// </summary>
        public bool InLimits
        {
            get { return _inLimits; }
        }

        /// <summary>
        /// Gets or sets the Robot Joint Position.
        /// </summary>
        public RobotJointPosition RobotJointPosition
        {
            get { return _robotJointPosition; }
            set { _robotJointPosition = value; }
        }

        /// <summary>
        /// Gets or sets the External Joint Position.
        /// </summary>
        public ExternalJointPosition ExternalJointPosition
        {
            get { return _externalJointPosition; }
            set { _externalJointPosition = value; }
        }

        /// <summary>
        /// Gets the latest calculated posed external axis planes.
        /// </summary>
        public Plane[] PosedExternalAxisPlanes 
        {
            get { return _posedExternalAxisPlanes; }
        }

        /// <summary>
        /// Gets the collected error messages.
        /// </summary>
        public List<string> ErrorText
        {
            get { return _errorText; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the posed meshed wil be calculated. 
        /// </summary>
        public bool HideMesh
        {
            get { return _hideMesh; }
            set { _hideMesh = value; }
        }
        #endregion
    }

}

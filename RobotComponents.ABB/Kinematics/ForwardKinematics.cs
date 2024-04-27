// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.Utils;
using RobotComponents.ABB.Actions.Declarations;
using RobotComponents.ABB.Definitions;

namespace RobotComponents.ABB.Kinematics
{
    /// <summary>
    /// Represent the Forward Kinematics for a 6-axis spherical Robot and its attached external axes. 
    /// </summary>
    public class ForwardKinematics
    {
        #region fields
        private Robot _robot;
        private RobotJointPosition _robotJointPosition;
        private ExternalJointPosition _externalJointPosition;

        private List<Mesh> _posedRobotMeshes = new List<Mesh>();
        private List<List<Mesh>> _posedExternalAxisMeshes = new List<List<Mesh>>();

        private readonly Transform[] _trans = new Transform[7];
        private readonly Transform[] _robotTransforms = new Transform[7];
        private readonly List<List<Transform>> _externalAxisTransforms = new List<List<Transform>>();

        private Plane _positionPlane = Plane.Unset;
        private Plane[] _posedExternalAxisPlanes;
        private Plane _tcpPlane = Plane.Unset;

        private readonly List<string> _errorText = new List<string>();
        private bool _hideMesh;
        private bool _isInLimits = true;

        private const double _deg2rad = Math.PI / 180;
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
        /// Initializes a new instance of the Forward Kinematics class by duplicating an existing Forward Kinematics instance. 
        /// </summary>
        /// <param name="forwardKinematics"> The Forward Kinematics instance to duplicate. </param>
        public ForwardKinematics(ForwardKinematics forwardKinematics)
        {
            _robot = forwardKinematics.Robot.Duplicate();
            _hideMesh = forwardKinematics.HideMesh;
            _tcpPlane = new Plane(forwardKinematics.TCPPlane);
            _errorText = new List<string>(forwardKinematics.ErrorText);
            _posedRobotMeshes = forwardKinematics.PosedRobotMeshes.ConvertAll(mesh => mesh.DuplicateMesh());
            _posedExternalAxisMeshes = new List<List<Mesh>>(forwardKinematics.PosedExternalAxisMeshes);
            _posedExternalAxisPlanes = new List<Plane>(forwardKinematics.PosedExternalAxisPlanes).ToArray();
            _robotTransforms = new List<Transform>(forwardKinematics.RobotTransforms).ToArray();
            _externalAxisTransforms = forwardKinematics.ExternalAxisTransforms;
            _isInLimits = forwardKinematics.IsInLimits;

            for (int i = 0; i < _posedExternalAxisMeshes.Count; i++)
            {
                for (int j = 0; j < _posedExternalAxisMeshes[i].Count; j++)
                {
                    _posedExternalAxisMeshes[i][j] = _posedExternalAxisMeshes[i][j].DuplicateMesh();
                }
            }

            for (int i = 0; i < _externalAxisTransforms.Count; i++)
            {
                _externalAxisTransforms[i] = new List<Transform>(_externalAxisTransforms[i]);
            }
        }

        /// <summary>
        /// Returns an exact duplicate of this Forward Kinematics instance.
        /// </summary>
        /// <returns> 
        /// A deep copy of the Forward Kinematics instance. 
        /// </returns>
        public ForwardKinematics Duplicate()
        {
            return new ForwardKinematics(this);
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
                return "Invalid Forward Kinematics";
            }
            else
            {
                return "Forward Kinematics";
            }
        }

        /// <summary>
        /// Reinitialize all the fields to construct a valid Forward Kinematics object. 
        /// </summary>
        /// <remarks>
        /// This method also resets the solution. The method Calculate() has to be called to obtain a new solution. 
        /// </remarks>
        public void ReInitialize()
        {
            _errorText.Clear();
            _posedRobotMeshes.Clear();
            _positionPlane = Plane.Unset;
            _tcpPlane = Plane.Unset;
            _isInLimits = true;

            for (int i = 0; i < _posedExternalAxisMeshes.Count; i++)
            {
                _posedExternalAxisMeshes[i].Clear();
            }

            _posedExternalAxisMeshes.Clear();

            for (int i = 0; i < _externalAxisTransforms.Count; i++)
            {
                _externalAxisTransforms[i].Clear();
            }

            _externalAxisTransforms.Clear();

        }

        /// <summary>
        /// Calculates the forward kinematics solution with the Joint Positions stored inside this Forward Kinematics instance.
        /// </summary>
        private void Calculate()
        {
            // Clear current solution
            ReInitialize();

            // Check axis limits
            CheckInternalAxisLimits();
            CheckExternalAxisLimits();

            // Calculate axis planes
            _positionPlane = new Plane(_robot.BasePlane);
            CalculateExternalPlanes();
            CalculateRobotAxisPlanes();

            // Get posed meshes
            if (_hideMesh == false)
            {
                PoseMeshes();
            }
        }

        /// <summary>
        /// Calculates the forward kinematics solution with the given Robot Joint Positions and a default External Joint Position (9e9).
        /// </summary>
        /// <param name="robotJointPosition"> The Robot Joint Position. </param>
        public void Calculate(RobotJointPosition robotJointPosition)
        {
            _robotJointPosition = robotJointPosition;
            _externalJointPosition = new ExternalJointPosition();
            Calculate();
        }

        /// <summary>
        /// Calculates the forward kinematics solution with the given Joint Positions.
        /// </summary>
        /// <param name="robotJointPosition"> The Robot Joint Position. </param>
        /// <param name="externalJointPosition"> The External Joint Position. </param>
        public void Calculate(RobotJointPosition robotJointPosition, ExternalJointPosition externalJointPosition)
        {
            _robotJointPosition = robotJointPosition;
            _externalJointPosition = externalJointPosition;
            Calculate();
        }

        /// <summary>
        /// Calculates the forward kinematics solution with the given Joint Target.
        /// </summary>
        /// <param name="jointTarget"> The Joint Target. </param>
        public void Calculate(JointTarget jointTarget)
        {
            _robotJointPosition = jointTarget.RobotJointPosition;
            _externalJointPosition = jointTarget.ExternalJointPosition;
            Calculate();
        }

        /// <summary>
        /// Calculates the positions of the external axis planes.
        /// </summary>
        private void CalculateExternalPlanes()
        {
            // Count the number of axes that move the robot
            double count = 0;

            // Calculates external axes positions
            _posedExternalAxisPlanes = new Plane[_robot.ExternalAxes.Count];

            for (int i = 0; i < _robot.ExternalAxes.Count; i++)
            {
                // Get the external axis
                IExternalAxis externalAxis = _robot.ExternalAxes[i];

                // Get transformation matrices
                List<Transform> transforms = new List<Transform>() { Transform.Translation(0, 0, 0) };
                transforms.Add(externalAxis.CalculateTransformationMatrixSave(_externalJointPosition));
                _externalAxisTransforms.Add(transforms);

                // Get external axis plane
                _posedExternalAxisPlanes[i] = externalAxis.CalculatePositionSave(_externalJointPosition);

                // Check if an external axis moves the robot and calculate the position plane.
                if (externalAxis.MovesRobot == true && count == 0)
                {
                    _positionPlane = externalAxis.CalculatePosition(_externalJointPosition, out _);
                    count += 1;
                }
            }
        }

        /// <summary>
        /// Calculates the positipn of the robot axis planes and TCP plane.
        /// </summary>
        private void CalculateRobotAxisPlanes()
        {
            // Base
            _trans[0] = Transform.PlaneToPlane(_robot.BasePlane, _positionPlane);
            _robotTransforms[0] = _trans[0];
            // Axis 1
            Plane planeAxis1 = new Plane(_robot.InternalAxisPlanes[0]);
            _trans[1] = Transform.Rotation(_robotJointPosition[0] * _deg2rad, planeAxis1.ZAxis, planeAxis1.Origin);
            _robotTransforms[1] = _trans[0] * _trans[1];
            // Axis 2
            Plane planeAxis2 = new Plane(_robot.InternalAxisPlanes[1]);
            planeAxis2.Transform(_trans[1]);
            _trans[2] = Transform.Rotation(_robotJointPosition[1] * _deg2rad, planeAxis2.ZAxis, planeAxis2.Origin);
            _robotTransforms[2] = _trans[0] * _trans[2] * _trans[1];
            // Axis 3
            Plane planeAxis3 = new Plane(_robot.InternalAxisPlanes[2]);
            planeAxis3.Transform(_trans[2] * _trans[1]);
            _trans[3] = Transform.Rotation(_robotJointPosition[2] * _deg2rad, planeAxis3.ZAxis, planeAxis3.Origin);
            _robotTransforms[3] = _trans[0] * _trans[3] * _trans[2] * _trans[1];
            // Axis 4
            Plane planeAxis4 = new Plane(_robot.InternalAxisPlanes[3]);
            planeAxis4.Transform(_trans[3] * _trans[2] * _trans[1]);
            _trans[4] = Transform.Rotation(_robotJointPosition[3] * _deg2rad, planeAxis4.ZAxis, planeAxis4.Origin);
            _robotTransforms[4] = _trans[0] * _trans[4] * _trans[3] * _trans[2] * _trans[1];
            // Axis 5
            Plane planeAxis5 = new Plane(_robot.InternalAxisPlanes[4]);
            planeAxis5.Transform(_trans[4] * _trans[3] * _trans[2] * _trans[1]);
            _trans[5] = Transform.Rotation(_robotJointPosition[4] * _deg2rad, planeAxis5.ZAxis, planeAxis5.Origin);
            _robotTransforms[5] = _trans[0] * _trans[5] * _trans[4] * _trans[3] * _trans[2] * _trans[1];
            // Axis 6
            Plane planeAxis6 = new Plane(_robot.InternalAxisPlanes[5]);
            planeAxis6.Transform(_trans[5] * _trans[4] * _trans[3] * _trans[2] * _trans[1]);
            _trans[6] = Transform.Rotation(_robotJointPosition[5] * _deg2rad, planeAxis6.ZAxis, planeAxis6.Origin);
            _robotTransforms[6] = _trans[0] * _trans[6] * _trans[5] * _trans[4] * _trans[3] * _trans[2] * _trans[1];
            // TCP plane
            _tcpPlane = new Plane(_robot.ToolPlane);
            _tcpPlane.Transform(_robotTransforms[6]);
        }

        /// <summary>
        /// Transforms the robot and external axis meshes.
        /// </summary>
        private void PoseMeshes()
        {
            // Deep copy the mehses
            _posedRobotMeshes = _robot.Meshes.ConvertAll(mesh => mesh.DuplicateMesh());
            _posedExternalAxisMeshes = new List<List<Mesh>>();

            // Base link transform
            _posedRobotMeshes[0].Transform(_robotTransforms[0]);
            // Joint 1 tranform 
            _posedRobotMeshes[1].Transform(_robotTransforms[1]);
            // Joint 2 tranform
            _posedRobotMeshes[2].Transform(_robotTransforms[2]);
            // Joint 3 tranform
            _posedRobotMeshes[3].Transform(_robotTransforms[3]);
            // Joint 4 tranform
            _posedRobotMeshes[4].Transform(_robotTransforms[4]);
            // Joint 5 tranform
            _posedRobotMeshes[5].Transform(_robotTransforms[5]);
            // Joint 6 tranform
            _posedRobotMeshes[6].Transform(_robotTransforms[6]);
            // Tool transform
            _posedRobotMeshes[7].Transform(_robotTransforms[6]);

            // External axis meshes
            for (int i = 0; i < _robot.ExternalAxes.Count; i++)
            {
                List<Mesh> posedMeshes = new List<Mesh>() { _robot.ExternalAxes[i].BaseMesh.DuplicateMesh(), _robot.ExternalAxes[i].LinkMesh.DuplicateMesh() };
                posedMeshes[1].Transform(_externalAxisTransforms[i][1]);
                _posedExternalAxisMeshes.Add(posedMeshes);
            }

            // Repair the robot meshes if needed
            _posedRobotMeshes.ConvertAll(mesh => mesh.IsValid == false ? MeshPreperation.Repair(mesh, 0.25, false) : mesh);
        }

        /// <summary>
        /// Returns the Bounding Box of the posed meshes.
        /// </summary>
        /// <param name="accurate"> If true, a physically accurate bounding box will be computed. If not, a bounding box estimate will be computed. </param>
        /// <returns> 
        /// The Bounding Box. 
        /// </returns>
        public BoundingBox GetBoundingBox(bool accurate)
        {
            BoundingBox result = new BoundingBox();

            if (_posedRobotMeshes != null)
            {
                for (int i = 0; i != _posedRobotMeshes.Count; i++)
                {
                    if (_posedRobotMeshes[i] != null)
                    {
                        if (_posedRobotMeshes[i].IsValid)
                        {
                            result.Union(_posedRobotMeshes[i].GetBoundingBox(accurate));
                        }
                    }
                }

                for (int i = 0; i != _posedExternalAxisMeshes.Count; i++)
                {
                    for (int j = 0; j != _posedExternalAxisMeshes[i].Count; j++)
                    {
                        if (_posedExternalAxisMeshes[i][j] != null)
                        {
                            if (_posedExternalAxisMeshes[i][j].IsValid)
                            {
                                result.Union(_posedExternalAxisMeshes[i][j].GetBoundingBox(accurate));
                            }
                        }
                    }
                }
            }

            return result;
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
                    _errorText.Add($"The position of robot axis {i + 1} is not in range.");
                    _isInLimits = false;
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
                    _errorText.Add($"The position of external logical axis {logic} is not definied.");
                    _isInLimits = false;
                }

                else if (_robot.ExternalAxes[i].AxisLimits.IncludesParameter(_externalJointPosition[index], false) == false)
                {
                    _errorText.Add($"The position of external logical axis  {logic} is not in range.");
                    _isInLimits = false;
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
                if (_robotJointPosition == null) { return false; }
                if (_robotJointPosition.IsValid == false) { return false; }
                if (_externalJointPosition == null) { return false; }
                if (_externalJointPosition.IsValid == false) { return false; }
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
        public List<Mesh> PosedRobotMeshes
        {
            get { return _posedRobotMeshes; }
        }

        /// <summary>
        /// Gets the latest calculated posed external axis meshes.
        /// </summary>
        public List<List<Mesh>> PosedExternalAxisMeshes
        {
            get { return _posedExternalAxisMeshes; }
        }

        /// <summary>
        /// Gets the latest calculated robot transformations. 
        /// </summary>
        /// <remarks>
        /// This array contains seven transformations. The transformation of the base and the six joints. 
        /// Use the last transformation (joint 6) to transform the tool and TCP plane. 
        /// </remarks>
        public Transform[] RobotTransforms
        {
            get { return _robotTransforms; }
        }

        /// <summary>
        /// Gets the latest calculated external axis transformations.
        /// </summary>
        /// <remarks>
        /// Contains a transformation for each mesh. 
        /// Including a zero transform of the base mesh of the external axis.
        /// </remarks>
        public List<List<Transform>> ExternalAxisTransforms
        {
            get { return _externalAxisTransforms; }
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
        /// Gets the latest calculated posed TCP plane of the robot tool. 
        /// </summary>
        public Plane TCPPlane
        {
            get { return _tcpPlane; }
        }

        /// <summary>
        /// Gets a value indicating whether or not the internal and external values are within their limits.
        /// </summary>
        public bool IsInLimits
        {
            get { return _isInLimits; }
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

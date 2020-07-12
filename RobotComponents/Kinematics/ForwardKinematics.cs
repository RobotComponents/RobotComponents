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
    /// Forward Kinematics class
    /// </summary>
    public class ForwardKinematics
    {
        #region fields
        private Robot _robotInfo; // Robot info
        private Plane _positionPlane = Plane.Unset; // Robot Position Plane: needed for external linear axis
        private Plane[] _posedExternalAxisPlanes; // External Axis Planes 
        private readonly List<string> _errorText = new List<string>(); // Error text
        private List<Mesh> _posedInternalAxisMeshes = new List<Mesh>(); // Posed Robot Meshes
        private List<List<Mesh>> _posedExternalAxisMeshes = new List<List<Mesh>>(); //Posed Axis Meshes
        private Plane _tcpPlane = Plane.Unset; // TCP Plane of end effector
        private bool _inLimits = true; // Indicates if the axis values are in limits 
        private bool _hideMesh;
        private RobotJointPosition _robotJointPosition;
        private ExternalJointPosition _externalJointPosition;
        #endregion

        #region constructors
        /// <summary>
        /// Defines a empty ForwardKinematic Object.
        /// </summary>
        public ForwardKinematics()
        {
        }

        /// <summary>
        /// Defines a Forward Kinematic object.
        /// </summary>
        /// <param name="robotInfo"> Robot Information the FK should be calculated for. </param>
        /// <param name="hideMesh"> Boolean that indicates if the mesh will be supressed. </param>
        public ForwardKinematics(Robot robotInfo, bool hideMesh = false)
        {
            _robotInfo = robotInfo;
            _hideMesh = hideMesh;
        }

        /// <summary>
        /// Defines a Forward Kinematic Object for certain axis values.
        /// </summary>
        /// <param name="robotInfo">Robot Information the FK should be calculated for.</param>
        /// <param name="internalAxisValues">List of internal axis values. The length of the list should be equal to 6.</param>
        /// <param name="externalAxisValues">List of external axis values. The length of the list should be (for now) equal to 1.</param>
        /// <param name="hideMesh"> Boolean that indicates if the mesh will be supressed. </param>
        public ForwardKinematics(Robot robotInfo, List<double> internalAxisValues, List<double> externalAxisValues, bool hideMesh = false)
        {
            _robotInfo = robotInfo;
            _hideMesh = hideMesh;
            _robotJointPosition = new RobotJointPosition(internalAxisValues);
            _externalJointPosition = new ExternalJointPosition(externalAxisValues);
        }

        /// <summary>
        /// Defines a Forward Kinematic Object for certain axis values.
        /// </summary>
        /// <param name="robotInfo">Robot Information the FK should be calculated for.</param>
        /// <param name="robotJointPosition">The internal axis values as a Robot Joint Position.</param>
        /// <param name="externalJointPosition">The external axis values as an External Joint Position.</param>
        /// <param name="hideMesh"> Boolean that indicates if the mesh will be supressed. </param>
        public ForwardKinematics(Robot robotInfo, RobotJointPosition robotJointPosition, ExternalJointPosition externalJointPosition, bool hideMesh = false)
        {
            _robotInfo = robotInfo;
            _hideMesh = hideMesh;
            _robotJointPosition = robotJointPosition;
            _externalJointPosition = externalJointPosition;
        }

        /// <summary>
        /// Defines a Forward Kinematic object from a Joint Target.
        /// </summary>
        /// <param name="robotInfo">Robot Information the FK should be calculated for.</param>
        /// <param name="jointTarget">The internal and external axis values defined as a Joint Target.</param>
        /// <param name="hideMesh"> Boolean that indicates if the mesh will be supressed. </param>
        public ForwardKinematics(Robot robotInfo, JointTarget jointTarget, bool hideMesh = false)
        {
            _robotInfo = robotInfo;
            _hideMesh = hideMesh;
            _robotJointPosition = jointTarget.RobotJointPosition;
            _externalJointPosition = jointTarget.ExternalJointPosition;
        }

        /// <summary>
        /// Creates a new forward kinematics by duplicating an existing forward kinematics.
        /// This creates a deep copy of the existing forward kinematics.
        /// </summary>
        /// <param name="forwardKinematics"> The forward kinematics that should be duplicated. </param>
        public ForwardKinematics(ForwardKinematics forwardKinematics)
        {
            _robotInfo = forwardKinematics.RobotInfo.Duplicate();
            _hideMesh = forwardKinematics.HideMesh;
            _robotJointPosition = forwardKinematics.RobotJointPosition.Duplicate();
            _externalJointPosition = forwardKinematics.ExternalJointPosition.Duplicate();
            _tcpPlane = new Plane(forwardKinematics.TCPPlane);
            _errorText = new List<string>(forwardKinematics.ErrorText);
            _robotInfo = forwardKinematics.RobotInfo.Duplicate();
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
        /// A method to duplicate the Forward Kinematics object.
        /// </summary>
        /// <returns> Returns a deep copy of the Forward Kinematics object. </returns>
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
        /// Calculates Forward Kinematics based on the internal and external axis values.
        /// </summary>
        public void Calculate()
        {
            // Clear current solution
            Clear();

            // Check axis limits
            CheckForInternalAxisLimits();
            CheckForExternalAxisLimits();

            // Deep copy the mehses if the pose should be calculated
            if (_hideMesh == false)
            {
                _posedInternalAxisMeshes = _robotInfo.Meshes.ConvertAll(mesh => mesh.DuplicateMesh());
                _posedExternalAxisMeshes = new List<List<Mesh>>();
            }

            // Get initial position plane
            _positionPlane = new Plane(_robotInfo.BasePlane);

            // Count the number of external linear axes that is used: it is now limited to one
            double count = 0;

            // Calculates external axes positions
            _posedExternalAxisPlanes = new Plane[_robotInfo.ExternalAxis.Count];
            for (int i = 0; i < _robotInfo.ExternalAxis.Count; i++)
            {
                // Get the external axis
                ExternalAxis externalAxis = _robotInfo.ExternalAxis[i];
                int logic = (int)_robotInfo.ExternalAxis[i].AxisNumber;

                // Get external axis plane
                _posedExternalAxisPlanes[i] = externalAxis.CalculatePositionSave(_externalJointPosition[logic]);

                // Check if it is an external linear axis: the first external linear axis
                // External axes that move the robot: this updates the position of the robot
                if (externalAxis is ExternalLinearAxis && count == 0)
                {
                    ExternalLinearAxis externalLinearAxis = externalAxis as ExternalLinearAxis;
                    _positionPlane = externalLinearAxis.CalculatePosition(_externalJointPosition[logic], out bool inLimits);
                    count += 1;
                }
            }

            // Move relative to base
            Transform transNow;
            transNow = Transform.PlaneToPlane(_robotInfo.BasePlane, _positionPlane);

            // Calculates internal axes
            // First caculate all tansformations (rotations)
            // Axis 1
            Transform rot1;
            Plane planeAxis1 = new Plane(_robotInfo.InternalAxisPlanes[0]);
            rot1 = Transform.Rotation(_robotJointPosition[0] * Math.PI / 180, planeAxis1.ZAxis, planeAxis1.Origin);
            // Axis 2
            Transform rot2;
            Plane planeAxis2 = new Plane(_robotInfo.InternalAxisPlanes[1]);
            planeAxis2.Transform(rot1);
            rot2 = Transform.Rotation(_robotJointPosition[1] * Math.PI / 180, planeAxis2.ZAxis, planeAxis2.Origin);
            // Axis 3
            Transform rot3;
            Plane planeAxis3 = new Plane(_robotInfo.InternalAxisPlanes[2]);
            planeAxis3.Transform(rot2 * rot1);
            rot3 = Transform.Rotation(_robotJointPosition[2] * Math.PI / 180, planeAxis3.ZAxis, planeAxis3.Origin);
            // Axis 4
            Transform rot4;
            Plane planeAxis4 = new Plane(_robotInfo.InternalAxisPlanes[3]);
            planeAxis4.Transform(rot3 * rot2 * rot1);
            rot4 = Transform.Rotation(_robotJointPosition[3] * Math.PI / 180, planeAxis4.ZAxis, planeAxis4.Origin);
            // Axis 5
            Transform rot5;
            Plane planeAxis5 = new Plane(_robotInfo.InternalAxisPlanes[4]);
            planeAxis5.Transform(rot4 * rot3 * rot2 * rot1);
            rot5 = Transform.Rotation(_robotJointPosition[4] * Math.PI / 180, planeAxis5.ZAxis, planeAxis5.Origin);
            // Axis 6
            Transform rot6;
            Plane planeAxis6 = new Plane(_robotInfo.InternalAxisPlanes[5]);
            planeAxis6.Transform(rot5 * rot4 * rot3 * rot2 * rot1);
            rot6 = Transform.Rotation(_robotJointPosition[5] * Math.PI / 180, planeAxis6.ZAxis, planeAxis6.Origin);

            // Apply transformations on tcp plane
            _tcpPlane = new Plane(_robotInfo.ToolPlane);
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
                for (int i = 0; i < _robotInfo.ExternalAxis.Count; i++)
                {
                    int logic = (int)_robotInfo.ExternalAxis[i].AxisNumber;
                    _robotInfo.ExternalAxis[i].PoseMeshes(_externalJointPosition[logic]);
                    _posedExternalAxisMeshes.Add(_robotInfo.ExternalAxis[i].PosedMeshes.ConvertAll(mesh => mesh.DuplicateMesh()));
                }
            }
        }

        /// <summary>
        /// Sets new axis values and calculates the new solution.  
        /// </summary>
        /// <param name="internalAxisValues">List of internal axis values in degree. The length of the list should be equal to 6.</param>
        /// <param name="externalAxisValues">List of external axis values in meter. The length of the list should be (for now) equal to 1.</param>
        public void Calculate(List<double> internalAxisValues, List<double> externalAxisValues)
        {
            _robotJointPosition = new RobotJointPosition(internalAxisValues);
            _externalJointPosition = new ExternalJointPosition(externalAxisValues);
            Calculate();
        }

        /// <summary>
        /// Sets new axis values and calculates the new solution.  
        /// </summary>
        /// <param name="robotJointPosition">The internal axis values as a Robot Joint Position.</param>
        /// <param name="externalJointPosition">The external axis values as an External Joint Position.</param>
        public void Calculate(RobotJointPosition robotJointPosition, ExternalJointPosition externalJointPosition)
        {
            _robotJointPosition = robotJointPosition;
            _externalJointPosition = externalJointPosition;
            Calculate();
        }

        /// <summary>
        /// Clears the current solution
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
        /// Checks if the interal axis values are outside its limits.
        /// </summary>
        private void CheckForInternalAxisLimits()
        {
            for (int i = 0; i < 6; i++)
            {
                if (_robotInfo.InternalAxisLimits[i].IncludesParameter(_robotJointPosition[i], false) == false)
                {
                    _errorText.Add("Internal axis value " + (i + 1).ToString() + " is not in range.");
                    _inLimits = false;
                }
            }
        }

        /// <summary>
        /// Checks if the external axis values are outside its limits.
        /// </summary>
        private void CheckForExternalAxisLimits()
        {
            for (int i = 0; i < _robotInfo.ExternalAxis.Count; i++)
            {
                int logic = (int)_robotInfo.ExternalAxis[i].AxisNumber;

                if (_externalJointPosition[logic] == 9e9)
                {
                    _errorText.Add("External axis value " + (i + 1).ToString() + " is not definied by the user.");
                    _inLimits = false;
                }

                else if (_robotInfo.ExternalAxis[i].AxisLimits.IncludesParameter(_externalJointPosition[logic], false) == false)
                {
                    _errorText.Add("External axis value " + (i + 1).ToString() + " is not in range.");
                    _inLimits = false;
                }
            }
        }
        #endregion

        #region properties
        /// <summary>
        /// A boolean that indicates if the Forward Kinematics object is valid. 
        /// </summary>
        public bool IsValid
        {
            get
            {
                if (RobotInfo.IsValid == false) { return false; }
                if (RobotJointPosition == null) { return false; }
                if (RobotJointPosition.IsValid == false) { return false; }
                if (ExternalJointPosition == null) { return false; }
                if (ExternalJointPosition.IsValid == false) { return false; }
                return true;
            }
        }

        /// <summary>
        /// RobotInformation the FK should be calculated for.
        /// </summary>
        public Robot RobotInfo
        {
            get { return _robotInfo; }
            set { _robotInfo = value; }
        }

        /// <summary>
        /// Calcualte robot pose meshes
        /// </summary>
        public List<Mesh> PosedInternalAxisMeshes
        {
            get { return _posedInternalAxisMeshes; }
        }

        /// <summary>
        /// A List of meshes in pose for the external axis.
        /// </summary>
        public List<List<Mesh>> PosedExternalAxisMeshes
        {
            get { return _posedExternalAxisMeshes; }
        }

        /// <summary>
        /// Calculated posed TCP plane of the robot tool. 
        /// </summary>
        public Plane TCPPlane
        {
            get { return _tcpPlane; }
        }

        /// <summary>
        /// Bool that indicates if the internal and external values are within their limits
        /// </summary>
        public bool InLimits
        {
            get { return _inLimits; }
        }

        /// <summary>
        /// List of internal axis values in degrees.
        /// </summary>
        [Obsolete("This property is obsolete. Instead, use the property RobotJointPosition", false)]
        public List<double> InternalAxisValues
        {
            get { return _robotJointPosition.ToList(); }
            set { _robotJointPosition = new RobotJointPosition(value); }
        }

        /// <summary>
        /// List of external axis values in ?. A external axis can be meter or degree
        /// </summary>
        [Obsolete("This property is obsolete. Instead, use the property ExternalJointPosition", false)]
        public List<double> ExternalAxisValues
        {
            get 
            {
                List<double> values = _externalJointPosition.ToList();
                values.RemoveAll(val => val == 9e9);
                return values; 
            }
            set 
            { 
                _externalJointPosition = new ExternalJointPosition(value); 
            }
        }

        /// <summary>
        /// Defines the Robot Joint Position
        /// </summary>
        public RobotJointPosition RobotJointPosition
        {
            get { return _robotJointPosition; }
            set { _robotJointPosition = value; }
        }

        /// <summary>
        /// Defines the External Joint Position
        /// </summary>
        public ExternalJointPosition ExternalJointPosition
        {
            get { return _externalJointPosition; }
            set { _externalJointPosition = value; }
        }

        /// <summary>
        /// Array of external axis planes.
        /// </summary>
        public Plane[] PosedExternalAxisPlanes 
        {
            get { return _posedExternalAxisPlanes; }
        }

        /// <summary>
        /// List of strings with collected error messages. 
        /// </summary>
        public List<string> ErrorText
        {
            get { return _errorText; }
        }

        /// <summary>
        /// 
        /// A boolean that indicates if the posed mesh is or will be calculated
        /// </summary>
        public bool HideMesh
        {
            get { return _hideMesh; }
            set { _hideMesh = value; }
        }
        #endregion
    }

}

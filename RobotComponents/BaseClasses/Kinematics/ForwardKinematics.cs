// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/EDEK-UniKassel/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.BaseClasses.Definitions;

namespace RobotComponents.BaseClasses.Kinematics
{
    /// <summary>
    /// Forward Kinematics class
    /// </summary>
    public class ForwardKinematics
    {
        #region fields
        private Robot _robotInfo; // Robot info
        private Plane _positionPlane = Plane.Unset; // Robot Position Plane: needed for external linear axis
        private List<double> _internalAxisValues = new List<double>(); // Internal Axis Values in Degrees
        private double[] _internalAxisRads; // Internal Axis Values in Radiants
        private Plane[] _posedExternalAxisPlanes; // External Axis Planes 
        private List<double> _externalAxisValues = new List<double>(); // External Axis Values in degrees or meters
        private readonly List<string> _errorText = new List<string>(); // Error text
        private List<Mesh> _posedInternalAxisMeshes = new List<Mesh>(); // Posed Robot Meshes
        private List<List<Mesh>> _posedExternalAxisMeshes = new List<List<Mesh>>(); //Posed Axis Meshes
        private Plane _tcpPlane = Plane.Unset; // TCP Plane of end effector
        private bool _inLimits = true; // Indicates if the axis values are in limits 
        private bool _hideMesh;
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
            _internalAxisValues = internalAxisValues;
            _externalAxisValues = externalAxisValues;
            UpdateInternalAxisValuesRadians();
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
            _internalAxisValues = new List<double>(InternalAxisValues);
            _externalAxisValues = new List<double>(ExternalAxisValues);
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
            _internalAxisRads = new List<double>(forwardKinematics.InternalAxisRads).ToArray();
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

                // Get external axis plane
                _posedExternalAxisPlanes[i] = externalAxis.CalculatePositionSave(_externalAxisValues[i]);

                // Check if it is an external linear axis: the first external linear axis
                // External axes that move the robot: this updates the position of the robot
                if (externalAxis is ExternalLinearAxis && count == 0)
                {
                    ExternalLinearAxis externalLinearAxis = externalAxis as ExternalLinearAxis;
                    _positionPlane = externalLinearAxis.CalculatePosition(_externalAxisValues[i], out bool inLimits);
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
            rot1 = Transform.Rotation(_internalAxisRads[0], planeAxis1.ZAxis, planeAxis1.Origin);
            // Axis 2
            Transform rot2;
            Plane planeAxis2 = new Plane(_robotInfo.InternalAxisPlanes[1]);
            planeAxis2.Transform(rot1);
            rot2 = Transform.Rotation(_internalAxisRads[1], planeAxis2.ZAxis, planeAxis2.Origin);
            // Axis 3
            Transform rot3;
            Plane planeAxis3 = new Plane(_robotInfo.InternalAxisPlanes[2]);
            planeAxis3.Transform(rot2 * rot1);
            rot3 = Transform.Rotation(_internalAxisRads[2], planeAxis3.ZAxis, planeAxis3.Origin);
            // Axis 4
            Transform rot4;
            Plane planeAxis4 = new Plane(_robotInfo.InternalAxisPlanes[3]);
            planeAxis4.Transform(rot3 * rot2 * rot1);
            rot4 = Transform.Rotation(_internalAxisRads[3], planeAxis4.ZAxis, planeAxis4.Origin);
            // Axis 5
            Transform rot5;
            Plane planeAxis5 = new Plane(_robotInfo.InternalAxisPlanes[4]);
            planeAxis5.Transform(rot4 * rot3 * rot2 * rot1);
            rot5 = Transform.Rotation(_internalAxisRads[4], planeAxis5.ZAxis, planeAxis5.Origin);
            // Axis 6
            Transform rot6;
            Plane planeAxis6 = new Plane(_robotInfo.InternalAxisPlanes[5]);
            planeAxis6.Transform(rot5 * rot4 * rot3 * rot2 * rot1);
            rot6 = Transform.Rotation(_internalAxisRads[5], planeAxis6.ZAxis, planeAxis6.Origin);

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
                    // Update the mesh pose of the axis
                    _robotInfo.ExternalAxis[i].PoseMeshes(_externalAxisValues[i]);
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
            _internalAxisValues = internalAxisValues;
            _externalAxisValues = externalAxisValues;
            UpdateInternalAxisValuesRadians();
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
        /// Updates the array with internal axis values in radians based from the list withi internal axis values in degrees. 
        /// </summary>
        private void UpdateInternalAxisValuesRadians()
        {
            _internalAxisRads = new double[_internalAxisValues.Count];
            _internalAxisRads[0] = (_internalAxisValues[0] / 180) * Math.PI;
            _internalAxisRads[1] = (_internalAxisValues[1] / 180) * Math.PI;
            _internalAxisRads[2] = (_internalAxisValues[2] / 180) * Math.PI;
            _internalAxisRads[3] = (_internalAxisValues[3] / 180) * Math.PI;
            _internalAxisRads[4] = (_internalAxisValues[4] / 180) * Math.PI;
            _internalAxisRads[5] = (_internalAxisValues[5] / 180) * Math.PI;
        }

        /// <summary>
        /// Checks if the interal axis values are outside its limits.
        /// </summary>
        private void CheckForInternalAxisLimits()
        {
            for (int i = 0; i < _internalAxisValues.Count; i++)
            {
                if (_robotInfo.InternalAxisLimits[i].IncludesParameter(_internalAxisValues[i], false) == false)
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
                if (_externalAxisValues[i] == 9e9)
                {
                    _errorText.Add("External axis value " + (i + 1).ToString() + " is not definied by the user.");
                    _inLimits = false;
                }

                else if (_robotInfo.ExternalAxis[i].AxisLimits.IncludesParameter(_externalAxisValues[i], false) == false)
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
                if (InternalAxisValues == null) { return false; }
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
        public List<double> InternalAxisValues
        {
            get 
            { 
                return _internalAxisValues; 
            }
            set 
            { 
                _internalAxisValues = value;
                UpdateInternalAxisValuesRadians();
            }
        }

        /// <summary>
        /// Array of internal axis values in radians.
        /// </summary>
        public double[] InternalAxisRads
        {
            get { return _internalAxisRads; }
        }

        /// <summary>
        /// List of external axis values in ?. A external axis can be meter or degree
        /// </summary>
        public List<double> ExternalAxisValues
        {
            get { return _externalAxisValues; }
            set { _externalAxisValues = value; }
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

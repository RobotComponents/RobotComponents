using System;
using System.Collections.Generic;

using Rhino.Geometry;

using RobotComponents.BaseClasses.Definitions;

namespace RobotComponents.BaseClasses.Kinematics
{
    /// <summary>
    /// Forward Kinematics class
    /// </summary>
    public class ForwardKinematics
    {
        #region fields
        private RobotInfo _robotInfo; // Robot info

        private Plane _basePlane; // Robot Base Plane
        private Plane[] _internalAxisPlanes; // Internal Axis Planes 
        private List<double> _internalAxisValues = new List<double>(); // Internal Axis Values in Degrees
        private double[] _internalAxisRads; // Internal Axis Values in Radiants
        private List<Interval> _internalAxisLimits; // Internal Axis Value Limits
        private List<bool> _internalAxisInLimit = new List<bool>(); // Internal Axis in Limit?: Bool List

        private Plane[] _externalAxisPlanes; // External Axis Planes 
        private List<double> _externalAxisValues = new List<double>(); // External Axis Values in degrees or meters
        private List<Interval> _externalAxisLimits; // External Axis Value Limits
        private List<bool> _externalAxisInLimit = new List<bool>(); // External Axis in Limit?: Bool List

        private readonly List<string> _errorText = new List<string>(); // Error text

        private List<Mesh> _posedInternalAxisMeshes; // Posed Robot Meshes
        private List<List<Mesh>> _posedExternalAxisMeshes; //Posed Axis Meshes
        private Plane _tcpPlane; // TCP Plane of effector

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
        public ForwardKinematics(RobotInfo robotInfo, bool hideMesh = false)
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
        public ForwardKinematics(RobotInfo robotInfo, List<double> internalAxisValues, List<double> externalAxisValues, bool hideMesh = false)
        {
            _robotInfo = robotInfo;
            _hideMesh = hideMesh;
            Update(internalAxisValues, externalAxisValues);
        }

        /// <summary>
        /// A method to duplicate the Forward Kinematics object.
        /// </summary>
        /// <returns> Returns a deep copy of the Forward Kinematics object. </returns>
        public ForwardKinematics Duplicate()
        {
            //TODO: make a method that duplicates all the used properties
            ForwardKinematics dup = new ForwardKinematics(RobotInfo, InternalAxisValues, ExternalAxisValues, HideMesh);
            return dup;
        }
        #endregion

        #region methods
        /// <summary>
        /// Calculates Forward Kinematics based on the internal and external axis values.
        /// </summary>
        public void Calculate()
        {
            // Deep copy the mehses if the pose should be calculated
            if (_hideMesh == false)
            {
                _posedInternalAxisMeshes = _robotInfo.Meshes.ConvertAll(mesh => mesh.DuplicateMesh());
                _posedExternalAxisMeshes = new List<List<Mesh>>();
            }
            else
            {
                _posedInternalAxisMeshes = new List<Mesh>();
                _posedExternalAxisMeshes = new List<List<Mesh>>();
            }

            // Calculates external axes position
            for (int i = 0; i < _robotInfo.ExternalAxis.Count; i++)
            {
                // Get the external axis
                ExternalAxis externalAxis = _robotInfo.ExternalAxis[i];

                // Check if it is an external linear axis
                if (externalAxis is ExternalLinearAxis)
                {
                    ExternalLinearAxis externalLinearAxis = externalAxis as ExternalLinearAxis;
                    _basePlane = externalLinearAxis.CalculatePosition(_externalAxisValues[i], out bool inLimits);
                }

                // Check if the axis is an external rotational axis
                else if (externalAxis is ExternalRotationalAxis)
                {
                    ExternalRotationalAxis externalRotationalAxis = externalAxis as ExternalRotationalAxis;
                    //TODO: ... 
                }

                // Calculate the mesh pose of the external axes
                if (_hideMesh == false)
                {
                    // Update the mesh pose of the axis
                    externalAxis.PoseMeshes(_externalAxisValues[i]);
                    _posedExternalAxisMeshes.Add(externalAxis.PosedMeshes.ConvertAll(mesh => mesh.DuplicateMesh()));
                }
            }

            // Move relative to base
            Transform transNow;
            transNow = Transform.ChangeBasis(_basePlane, Plane.WorldXY);

            // Calculates internal axes
            // First caculate all tansformations (rotations)
            // Axis 1
            Transform rot1;
            Plane planeAxis1 = new Plane(_internalAxisPlanes[0]);
            rot1 = Transform.Rotation(_internalAxisRads[0], planeAxis1.ZAxis, planeAxis1.Origin);
            // Axis 2
            Transform rot2;
            Plane planeAxis2 = new Plane(_internalAxisPlanes[1]);
            planeAxis2.Transform(rot1);
            rot2 = Transform.Rotation(_internalAxisRads[1], planeAxis2.ZAxis, planeAxis2.Origin);
            // Axis 3
            Transform rot3;
            Plane planeAxis3 = new Plane(_internalAxisPlanes[2]);
            planeAxis3.Transform(rot2 * rot1);
            rot3 = Transform.Rotation(_internalAxisRads[2], planeAxis3.ZAxis, planeAxis3.Origin);
            // Axis 4
            Transform rot4;
            Plane planeAxis4 = new Plane(_internalAxisPlanes[3]);
            planeAxis4.Transform(rot3 * rot2 * rot1);
            rot4 = Transform.Rotation(_internalAxisRads[3], planeAxis4.ZAxis, planeAxis4.Origin);
            // Axis 5
            Transform rot5;
            Plane planeAxis5 = new Plane(_internalAxisPlanes[4]);
            planeAxis5.Transform(rot4 * rot3 * rot2 * rot1);
            rot5 = Transform.Rotation(_internalAxisRads[4], planeAxis5.ZAxis, planeAxis5.Origin);
            // Axis 6
            Transform rot6;
            Plane planeAxis6 = new Plane(_internalAxisPlanes[5]);
            planeAxis6.Transform(rot5 * rot4 * rot3 * rot2 * rot1);
            rot6 = Transform.Rotation(_internalAxisRads[5], planeAxis6.ZAxis, planeAxis6.Origin);

            // Apply transformations on tcp plane
            _tcpPlane.Transform(transNow * rot6 * rot5 * rot4 * rot3 * rot2 * rot1);

            // Transpose the robot mesh
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
            }
        }

        /// <summary>
        /// Updates the Internal and external axis values for the Forward Kinematic. 
        /// This methods does not recaculate the object. 
        /// </summary>
        /// <param name="internalAxisValues">List of internal axis values in degree. The length of the list should be equal to 6.</param>
        /// <param name="externalAxisValues">List of external axis values in meter. The length of the list should be (for now) equal to 1.</param>
        public void Update(List<double> internalAxisValues, List<double> externalAxisValues)
        {
            // Clear: remove data of possible old solution
            _internalAxisValues.Clear();
            _internalAxisInLimit.Clear();
            _externalAxisValues.Clear();
            _externalAxisInLimit.Clear();
            _errorText.Clear();

            // Update robot data and set the internal axis values
            _basePlane = _robotInfo.BasePlane;
            _tcpPlane = _robotInfo.ToolPlane;
            _internalAxisPlanes = _robotInfo.InternalAxisPlanes.ToArray();
            _internalAxisLimits = _robotInfo.InternalAxisLimits;
            _internalAxisValues = internalAxisValues;
            UpdateInternalAxisValuesRadians();

            // Update external axis data and set the external axis values
            _externalAxisPlanes = new Plane[_robotInfo.ExternalAxis.Count];
            _externalAxisLimits = _robotInfo.ExternalAxisLimits;
            _externalAxisValues = externalAxisValues;

            // Check axis limits
            CheckForInternalAxisLimits();
            CheckForExternalAxisLimits();

            // Get the current location of the attachment plane of the external axes
            for (int i = 0; i < _robotInfo.ExternalAxis.Count; i++)
            {
                _externalAxisPlanes[i] = _robotInfo.ExternalAxis[i].CalculatePositionSave(_externalAxisValues[i]);
            }
        }

        /// <summary>
        /// Updates the list with internal axis values in degrees based from the array with internal axis values in radians. 
        /// </summary>
        private void UpdateInternalAxisValuesDegrees()
        {
            _internalAxisValues = new List<double>() { 0, 0, 0, 0, 0, 0};
            _internalAxisValues[0] = (_internalAxisRads[0] * 180) / Math.PI;
            _internalAxisValues[1] = (_internalAxisRads[1] * 180) / Math.PI;
            _internalAxisValues[2] = (_internalAxisRads[2] * 180) / Math.PI;
            _internalAxisValues[3] = (_internalAxisRads[3] * 180) / Math.PI;
            _internalAxisValues[4] = (_internalAxisRads[4] * 180) / Math.PI;
            _internalAxisValues[5] = (_internalAxisRads[5] * 180) / Math.PI;
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
        public void CheckForInternalAxisLimits()
        {
            _errorText.Clear();
            _internalAxisInLimit.Clear();

            for (int i = 0; i < _internalAxisValues.Count; i++)
            {
                if (_internalAxisLimits[i].IncludesParameter(_internalAxisValues[i]))
                {
                    _internalAxisInLimit.Add(true);
                }
                else
                {
                    _errorText.Add("Internal Axis Value " + i + " is not in Range.");
                    _internalAxisInLimit.Add(false);
                }

            }
        }

        /// <summary>
        /// Checks if the external axis values are outside its limits.
        /// </summary>
        public void CheckForExternalAxisLimits()
        {
            _errorText.Clear();
            _externalAxisInLimit.Clear();

            for (int i = 0; i < _externalAxisValues.Count; i++)
            {
                if (_externalAxisLimits[i].IncludesParameter(_externalAxisValues[i]))
                {
                    _externalAxisInLimit.Add(true);
                }
                else
                {
                    _errorText.Add("External Axis Value " + i + " is not in Range.");
                    _externalAxisInLimit.Add(false);
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
        public RobotInfo RobotInfo
        {
            get 
            { 
                return _robotInfo; 
            }
            set 
            {
                _robotInfo = value;
                Update(_internalAxisValues, _externalAxisValues);
            }
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
        /// Calculated posed TCP plane of the robot tool
        /// </summary>
        public Plane TCPPlane
        {
            get { return _tcpPlane; }
        }

        /// <summary>
        /// List of boolean defining whether or not the robot is outside of there axis limits.
        /// </summary>
        public List<bool> InternalAxisInLimit
        {
            get { return _internalAxisInLimit; }
        }

        /// <summary>
        /// List of bools defining if the external axes are in their limit.
        /// </summary>
        public List<bool> ExternalAxisInLimit
        {
            get { return _externalAxisInLimit; }
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
        public Double[] InternalAxisRads
        {
            get 
            { 
                return _internalAxisRads; 
            }
            set 
            { 
                _internalAxisRads = value;
                UpdateInternalAxisValuesDegrees();
            }
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
        public Plane[] ExternalAxisPlanes 
        {
            get { return _externalAxisPlanes; }
        }

        /// <summary>
        /// List of strings collecting error messages which can be displayed in a grasshopper component.
        /// </summary>
        public List<string> ErrorText
        {
            get { return _errorText; }
        }

        /// <summary>
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

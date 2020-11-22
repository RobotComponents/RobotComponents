// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
// Rhino Libs
using Rhino.Geometry;
// Robot Components Libs
using RobotComponents.Enumerations;
using RobotComponents.Utils;

namespace RobotComponents.Definitions
{
    /// <summary>
    /// Represents a Work Object.
    /// </summary>
    [Serializable()]
    public class WorkObject : ISerializable
    {
        #region fields
        private ReferenceType _referenceType; // reference type
        private string _name; // The work object name
        private Plane _plane; // The work object coordinate system
        private Quaternion _orientation; // The orientation of the work object coordinate system
        private ExternalAxis _externalAxis; // The coupled mechanical unit
        private bool _robotHold; // Bool that indicates if the robot holds the work object
        private bool _fixedFrame; // Bool that indicates if the workobject is fixed (true) or movable (false)
        private Plane _userFrame; // The user frame coordinate system
        private Quaternion _userFrameOrientation; // the orienation of the user frame coordinate system
        private Plane _globalPlane; // global work object plane
        #endregion

        #region (de)serialization
        /// <summary>
        /// Protected constructor needed for deserialization of the object.  
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization. </param>
        protected WorkObject(SerializationInfo info, StreamingContext context)
        {
            // int version = (int)info.GetValue("Version", typeof(int)); // <-- use this if the (de)serialization changes
            _referenceType = (ReferenceType)info.GetValue("Reference Type", typeof(ReferenceType));
            _name = (string)info.GetValue("Name", typeof(string));
            _plane = (Plane)info.GetValue("Plane", typeof(Plane));
            _externalAxis = (ExternalAxis)info.GetValue("External Axis", typeof(ExternalAxis));
            _robotHold = (bool)info.GetValue("Robot Hold", typeof(bool));
            _userFrame = (Plane)info.GetValue("User Frame", typeof(Plane));

            Initialize();
        }

        /// <summary>
        /// Populates a SerializationInfo with the data needed to serialize the object.
        /// </summary>
        /// <param name="info"> The SerializationInfo to populate with data. </param>
        /// <param name="context"> The destination for this serialization. </param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Version", VersionNumbering.CurrentVersionAsInt, typeof(int));
            info.AddValue("Reference Type", _referenceType, typeof(ReferenceType));
            info.AddValue("Name", _name, typeof(string));
            info.AddValue("Plane", _plane, typeof(Plane));
            info.AddValue("External Axis", _externalAxis, typeof(ExternalAxis));
            info.AddValue("Robot Hold", _robotHold, typeof(bool));
            info.AddValue("User Frame", _userFrame , typeof(Plane));
        }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes a new instance of the Work Object class with the default work object wobj0. 
        /// </summary>
        public WorkObject()
        {
            _referenceType = ReferenceType.PERS;
            _name = "wobj0";
            _plane = Plane.WorldXY;
            _externalAxis = null;
            _robotHold = false;
            _userFrame = Plane.WorldXY;

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the Work Object class with a fixed work object.
        /// </summary>
        /// <param name="name"> The work object name, must be unique. </param>
        /// <param name="plane"> The work object coordinate system. </param>
        public WorkObject(string name, Plane plane)
        {
            _referenceType = ReferenceType.PERS;
            _name = name;
            _plane = plane;
            _externalAxis = null;
            _robotHold = false;
            _userFrame = Plane.WorldXY;

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the Work Object class with a movable work object.
        /// </summary>
        /// <param name="name"> The work object name, must be unique. </param>
        /// <param name="plane"> The work object coordinate system. </param>
        /// <param name="externalAxis"> The coupled external axis (mechanical unit) that moves the work object. </param>
        public WorkObject(string name, Plane plane, ExternalAxis externalAxis)
        {
            _referenceType = ReferenceType.PERS;
            _name = name;
            _plane = plane;
            _externalAxis = externalAxis;
            _robotHold = false;
            _userFrame = Plane.WorldXY;

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the Work Object class by duplicating an existing Work Object instance. 
        /// </summary>
        /// <param name="workObject"> The Work Object instance to duplicate. </param>
        /// <param name="duplicateMesh"> Specifies whether the meshes should be duplicated. </param>
        public WorkObject(WorkObject workObject, bool duplicateMesh = true)
        {
            _referenceType = workObject.ReferenceType;
            _name = workObject.Name;
            _plane = new Plane(workObject.Plane);
            _userFrame = new Plane(workObject.UserFrame);
            _globalPlane = new Plane(workObject.GlobalWorkObjectPlane);
            _userFrameOrientation = workObject.UserFrameOrientation;
            _orientation = workObject.Orientation;
            _robotHold = workObject.RobotHold;
            _fixedFrame = workObject.FixedFrame;

            if (workObject.ExternalAxis == null) { _externalAxis = null; }
            else if (duplicateMesh == true) { _externalAxis = workObject.ExternalAxis.DuplicateExternalAxis(); }
            else { _externalAxis = workObject.ExternalAxis.DuplicateExternalAxisWithoutMesh(); }          
        }

        /// <summary>
        /// Returns an exact duplicate of this Work Object instance.
        /// </summary>
        /// <returns> A deep copy of the Work Object instance. </returns>
        public WorkObject Duplicate()
        {
            return new WorkObject(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Work Object instance without meshes.
        /// </summary>
        /// <returns> A deep copy of the Work Object instance without meshes. </returns>
        public WorkObject DuplicateWithoutMesh()
        {
            return new WorkObject(this, false);
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
                return "Invalid Work Object";
            }
            else if (this.FixedFrame == false)
            {
                return "Movable Work Object (" + this.Name + ")";
            }
            else
            {
                return "Work Object (" + this.Name + ")";
            }
        }

        /// <summary>
        /// Calculates and returns the quaternion orientation of the work object coordinate system. 
        /// </summary>
        /// <returns> The quaternion orientation of the work object. </returns>
        public Quaternion CalculateOrientation()
        {
            _orientation = HelperMethods.PlaneToQuaternion(_plane);
            return _orientation;
        }

        /// <summary>
        /// Calculates and returns the quaternion orientation of the user frame coordinate system. 
        /// </summary>
        /// <returns> The quaternion orientation of the user frame. </returns>
        public Quaternion CalculateUserFrameOrientation()
        {
            _userFrameOrientation = HelperMethods.PlaneToQuaternion(_userFrame);
            return _userFrameOrientation;
        }

        /// <summary>
        /// Calculates and returns the global work object plane. 
        /// </summary>
        /// <returns> The global work object plane. </returns>
        public Plane CalculateGlobalWorkObjectPlane()
        {
            // Create a deep copy of the work object plane
            _globalPlane = new Plane(_plane);

            // Re-orient the plane
            Transform orient1 = Transform.PlaneToPlane(Plane.WorldXY, _userFrame);
            _globalPlane.Transform(orient1);

            // Re-orient again if an external axis is used
            if (_externalAxis != null)
            {
                // For an external axis the coordinate system of the work object plane
                // is definied in the coordinate system of the external axis.
                Transform orient2 = Transform.PlaneToPlane(Plane.WorldXY, _externalAxis.AttachmentPlane);
                _globalPlane.Transform(orient2);
            }

            return _globalPlane;
        }

        /// <summary>
        /// Initializes the fields and properties to construct a valid Work Object instance. 
        /// </summary>
        private void Initialize()
        {
            CalculateOrientation();
            CalculateUserFrameOrientation();
            CalculateGlobalWorkObjectPlane();

            // Set to a movable frame if an exernal axes is coupled
            if (_externalAxis != null)
            {
                _fixedFrame = false;
            }
            else
            {
                _fixedFrame = true;
            }

            // Check if the external axis moves the robot
            if (_externalAxis != null)
            {
                if (_externalAxis.MovesRobot == true)
                {
                    throw new Exception("An external axis that moves the robot cannot move a work object.");
                }
            }
        }

        /// <summary>
        /// Reinitializes the fields and properties to construct a valid Work Object instance.
        /// </summary>
        public void ReInitialize()
        {
            Initialize();
        }

        /// <summary>
        /// Returns the RAPID declaration code line of the this Work Object.
        /// </summary>
        /// <returns> The RAPID code line. </returns>
        public string ToRAPIDDeclaration()
        {
            string result = "";

            // Adds variable type
            result += Enum.GetName(typeof(ReferenceType), _referenceType);
            result += " wobjdata ";

            // Adds work object name
            result += $"{_name} := ";

            // Add robot hold < robhold of bool >
            if (_robotHold)
            {
                result += "[TRUE, ";
            }
            else
            {
                result += "[FALSE, ";
            }

            // Add User frame type < ufprog of bool >
            if (_fixedFrame)
            {
                result += "TRUE, ";
            }
            else
            {
                result += "FALSE, ";
            }

            // Add mechanical unit (an external axis or robot) < ufmec of string >            
            if (_externalAxis != null)
            {
                result += $"\"{_externalAxis.Name}\", ";
            }
            else
            {
                result += "\"\", ";
            }
            
            // Add user frame coordinate < uframe of pose > < trans of pos >
            result += $"[[{_userFrame.Origin.X:0.####}, {_userFrame.Origin.Y:0.####}, {_userFrame.Origin.Z:0.####}], ";

            // Add user frame orientation < uframe of pose > < rot of orient >
            result += $"[{_userFrameOrientation.A:0.#######}, {_userFrameOrientation.B:0.#######}, " +
                $"{_userFrameOrientation.C:0.#######}, {_userFrameOrientation.D:0.#######}]], ";

            // Add object frame coordinate < oframe of pose > < trans of pos >
            result += $"[[{_plane.Origin.X:0.####}, {_plane.Origin.Y:0.####}, {_plane.Origin.Z:0.####}], ";

            // Add object frame orientation < oframe of pose > < rot of orient >
            result += $"[{_orientation.A:0.#######}, {_orientation.B:0.#######}, " +
                $"{_orientation.C:0.#######}, {_orientation.D:0.#######}]]];";

            return result;
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
                if (Name == null) { return false;  }
                if (Name == "") { return false; }
                if (Plane == null) { return false; }
                if (Plane == Plane.Unset) { return false; }
                if (UserFrame == null) {return false; }
                if (UserFrame == Plane.Unset) { return false;  }
                return true;
            }
        }

        /// <summary>
        /// Gets or sets the Reference Type. 
        /// </summary>
        public ReferenceType ReferenceType
        {
            get { return _referenceType; }
            set { _referenceType = value; }
        }

        /// <summary>
        /// Gets or sets the name of the workobject.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets a value indicating whether or not the robot is holding the work object. 
        /// </summary>
        public bool RobotHold
        {
            get { return _robotHold; }
            set { _robotHold = value; }
        }

        /// <summary>
        /// Gets the global work object plane.
        /// </summary>
        public Plane GlobalWorkObjectPlane
        {
            get { return _globalPlane; }
        }

        /// <summary>
        /// Gets or sets the user coordinate system, i.e. the position of the current work surface or fixture.
        /// If the robot is holding the tool, the user coordinate system is defined in the world coordinate system (in the wrist coordinate system if a stationary tool is used). 
        /// For movable user frame (FixedFrame = false), the user frame is continuously defined by the system.
        /// </summary>
        public Plane UserFrame
        {
            get 
            { 
                return _userFrame; 
            }
            set 
            { 
                _userFrame = value;
                ReInitialize();
            }
        }

        /// <summary>
        /// Gets or set the work object coordinate system as a plane (e.g. the position of the current work object).
        /// The object coordinate system is defined in the user coordinate system.
        /// </summary>
        public Plane Plane
        {
            get 
            { 
                return _plane; 
            }
            set 
            { 
                _plane = value;
                ReInitialize();
            }
        }

        /// <summary>
        /// Gets or sets the external axis (mechanical unit) with which the robot movements are coordinated. 
        /// Only specified in the case of movable user coordinate systems.
        /// </summary>
        public ExternalAxis ExternalAxis
        {
            get 
            { 
                return _externalAxis; 
            }
            set 
            { 
                _externalAxis = value;
                ReInitialize();
            }
        }

        /// <summary>
        /// Gets the Quaternion orientation of the work object coordinate system.
        /// </summary>
        public Quaternion Orientation
        {
            get { return _orientation; }
        }

        /// <summary>
        /// Gets the Quaternion orientation of the user frame coordinate system.
        /// </summary>
        public Quaternion UserFrameOrientation
        {
            get { return _userFrameOrientation; }
        }

        /// <summary>
        /// Gets a value indicating whether or not a fixed user coordinate system is used.
        /// True indicates that the user frame is fixed. 
        /// False indicates that the user coordinate system is movable (e.g. coordinated external axes).
        /// </summary>
        public bool FixedFrame
        {
            get { return _fixedFrame; }
            set { _fixedFrame = value; }
        }
        #endregion
    }
}

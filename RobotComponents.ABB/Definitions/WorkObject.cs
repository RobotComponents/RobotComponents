// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components
// Project: https://github.com/RobotComponents/RobotComponents
//
// Copyright (c) 2018-2020 EDEK Uni Kassel
// Copyright (c) 2020-2025 Arjen Deetman
//
// Authors:
//   - Gabriel Rumph (2018-2020)
//   - Benedikt Wannemacher (2018-2020)
//   - Arjen Deetman (2019-2025)
//
// For license details, see the LICENSE file in the project root.

// System Libs
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
// Rhino Libs
using Rhino.Geometry;
// Robot Components Libs
using RobotComponents.ABB.Actions.Declarations;
using RobotComponents.ABB.Actions;
using RobotComponents.ABB.Enumerations;
using RobotComponents.ABB.Utils;

namespace RobotComponents.ABB.Definitions
{
    /// <summary>
    /// Represents a Work Object.
    /// </summary>
    [Serializable()]
    public class WorkObject : ISerializable, IDeclaration
    {
        #region fields
        private Scope _scope = Scope.GLOBAL;
        private VariableType _variableType = VariableType.PERS;
        private const string _datatype = "wobjdata";
        private string _name = "";
        private bool _robotHold;
        private bool _fixedFrame;
        private Plane _objectFrame;
        private Plane _userFrame;
        private Plane _globalPlane;
        private Quaternion _objectFrameOrientation;
        private Quaternion _userFrameOrientation;
        private IExternalAxis _externalAxis;
        #endregion

        #region (de)serialization
        /// <summary>
        /// Protected constructor needed for deserialization of the object.  
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization. </param>
        protected WorkObject(SerializationInfo info, StreamingContext context)
        {
            Version version = (Version)info.GetValue("Version", typeof(Version)); // <-- use this if the (de)serialization changes
            _scope = (Scope)info.GetValue("Scope", typeof(Scope));
            _variableType = (VariableType)info.GetValue("Variable Type", typeof(VariableType));
            _name = (string)info.GetValue("Name", typeof(string));
            _userFrame = (Plane)info.GetValue("User Frame", typeof(Plane));
            _objectFrame = version < new Version(4, 0, 0) ? (Plane)info.GetValue("Plane", typeof(Plane)) : (Plane)info.GetValue("Object Frame", typeof(Plane));
            _externalAxis = (IExternalAxis)info.GetValue("External Axis", typeof(IExternalAxis));
            _robotHold = (bool)info.GetValue("Robot Hold", typeof(bool));

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
            info.AddValue("Version", VersionNumbering.Version, typeof(Version));
            info.AddValue("Scope", _scope, typeof(Scope));
            info.AddValue("Variable Type", _variableType, typeof(VariableType));
            info.AddValue("Name", _name, typeof(string));
            info.AddValue("User Frame", _userFrame, typeof(Plane));
            info.AddValue("Object Frame", _objectFrame, typeof(Plane));
            info.AddValue("External Axis", _externalAxis, typeof(IExternalAxis));
            info.AddValue("Robot Hold", _robotHold, typeof(bool));
        }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes a new instance of the Work Object class with the default work object wobj0. 
        /// </summary>
        public WorkObject()
        {
            _name = "wobj0";
            _objectFrame = Plane.WorldXY;
            _externalAxis = null;
            _robotHold = false;
            _userFrame = Plane.WorldXY;

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the Work Object class with a fixed work object.
        /// </summary>
        /// <param name="name"> The work object name, must be unique. </param>
        /// <param name="objectFrame"> The object frame as a Plane. </param>
        public WorkObject(string name, Plane objectFrame)
        {
            _name = name;
            _objectFrame = objectFrame;
            _externalAxis = null;
            _robotHold = false;
            _userFrame = Plane.WorldXY;

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the Work Object class with a fixed work object.
        /// </summary>
        /// <param name="name"> The work object name, must be unique. </param>
        /// <param name="userFrame"> The user frame as a Plane. </param>
        /// <param name="objectFrame"> The object frame as a Plane. </param>
        public WorkObject(string name, Plane userFrame, Plane objectFrame)
        {
            _name = name;
            _objectFrame = objectFrame;
            _externalAxis = null;
            _robotHold = false;
            _userFrame = userFrame;

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the Work Object class with a movable work object.
        /// </summary>
        /// <param name="name"> The work object name, must be unique. </param>
        /// <param name="userFrame"> The user frame as a Plane. </param>
        /// <param name="objectFrame"> The object frame as a Plane.  </param>
        /// <param name="externalAxis"> The coupled external axis (mechanical unit) that moves the work object. </param>
        public WorkObject(string name, Plane userFrame, Plane objectFrame, IExternalAxis externalAxis)
        {
            _name = name;
            _objectFrame = objectFrame;
            _externalAxis = externalAxis;
            _robotHold = false;
            _userFrame = userFrame;

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the Work Object class with a movable work object.
        /// </summary>
        /// <param name="name"> The work object name, must be unique. </param>
        /// <param name="objectFrame"> The object frame as a Plane.  </param>
        /// <param name="externalAxis"> The coupled external axis (mechanical unit) that moves the work object. </param>
        public WorkObject(string name, Plane objectFrame, IExternalAxis externalAxis)
        {
            _name = name;
            _objectFrame = objectFrame;
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
            _scope = workObject.Scope;
            _variableType = workObject.VariableType;
            _name = workObject.Name;
            _objectFrame = new Plane(workObject.ObjectFrame);
            _userFrame = new Plane(workObject.UserFrame);
            _globalPlane = new Plane(workObject.GlobalWorkObjectPlane);
            _robotHold = workObject.RobotHold;
            _fixedFrame = workObject.FixedFrame;

            if (workObject.ExternalAxis == null) { _externalAxis = null; }
            else if (duplicateMesh == true) { _externalAxis = workObject.ExternalAxis.DuplicateExternalAxis(); }
            else { _externalAxis = workObject.ExternalAxis.DuplicateExternalAxisWithoutMesh(); }

            Initialize();
        }

        /// <summary>
        /// Returns an exact duplicate of this Work Object instance.
        /// </summary>
        /// <returns> 
        /// A deep copy of the Work Object instance. 
        /// </returns>
        public WorkObject Duplicate()
        {
            return new WorkObject(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Work Object instance without meshes.
        /// </summary>
        /// <returns> 
        /// A deep copy of the Work Object instance without meshes. 
        /// </returns>
        public WorkObject DuplicateWithoutMesh()
        {
            return new WorkObject(this, false);
        }

        /// <summary>
        /// Returns an exact duplicate of this Work Object instance as an IDeclaration.
        /// </summary>
        /// <returns> 
        /// A deep copy of the Work Object instance as an IDeclaration. 
        /// </returns>
        public IDeclaration DuplicateDeclaration()
        {
            return new WorkObject(this);
        }
        #endregion

        #region parse
        /// <summary>
        /// Initializes a new instance of the Work Object class from a rapid data string.
        /// </summary>
        /// <remarks>
        /// Only used for the Parse and TryParse methods. Therefore, this constructor is private. 
        /// </remarks>
        /// <param name="rapidData"> The RAPID data string. </param>
        private WorkObject(string rapidData)
        {
            this.SetRapidDataFromString(rapidData, out string[] values);

            if (values.Length == 17)
            {
                _robotHold = values[0] == "TRUE";
                _fixedFrame = values[1] == "TRUE";

                // External axes are ignored. 
                _externalAxis = null;

                double x = double.Parse(values[3]);
                double y = double.Parse(values[4]);
                double z = double.Parse(values[5]);
                double a = double.Parse(values[6]);
                double b = double.Parse(values[7]);
                double c = double.Parse(values[8]);
                double d = double.Parse(values[9]);
                _userFrame = HelperMethods.QuaternionToPlane(x, y, z, a, b, c, d);

                x = double.Parse(values[10]);
                y = double.Parse(values[11]);
                z = double.Parse(values[12]);
                a = double.Parse(values[13]);
                b = double.Parse(values[14]);
                c = double.Parse(values[15]);
                d = double.Parse(values[16]);
                _objectFrame = HelperMethods.QuaternionToPlane(x, y, z, a, b, c, d);

                CalculateOrientation();
                CalculateUserFrameOrientation();
                CalculateGlobalWorkObjectPlane();
            }
            else
            {
                throw new InvalidCastException("Invalid RAPID data string: The number of values does not match.");
            }
        }

        /// <summary>
        /// Returns a Work Object instance constructed from a RAPID data string. 
        /// </summary>
        /// <param name="rapidData"> The RAPID data string. s</param>
        public static WorkObject Parse(string rapidData)
        {
            return new WorkObject(rapidData);
        }

        /// <summary>
        /// Attempts to parse a RAPID data string into a Work Object instance.  
        /// </summary>
        /// <param name="rapidData"> The RAPID data string. </param>
        /// <param name="workObject"> The Work Object intance. </param>
        /// <returns> 
        /// True on success, false on failure. 
        /// </returns>
        public static bool TryParse(string rapidData, out WorkObject workObject)
        {
            try
            {
                workObject = new WorkObject(rapidData);
                return true;
            }
            catch
            {
                workObject = new WorkObject();
                return false;
            }
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
                return "Invalid Work Object";
            }
            else if (_fixedFrame == false)
            {
                return $"Movable Work Object ({_name})";
            }
            else
            {
                return $"Work Object ({_name})";
            }
        }

        /// <summary>
        /// Calculates and returns the quaternion orientation of the work object coordinate system. 
        /// </summary>
        private void CalculateOrientation()
        {
            _objectFrameOrientation = HelperMethods.PlaneToQuaternion(_objectFrame);
        }

        /// <summary>
        /// Calculates and returns the quaternion orientation of the user frame coordinate system. 
        /// </summary>
        private void CalculateUserFrameOrientation()
        {
            _userFrameOrientation = HelperMethods.PlaneToQuaternion(_userFrame);
        }

        /// <summary>
        /// Calculates and returns the global work object plane. 
        /// </summary>
        private void CalculateGlobalWorkObjectPlane()
        {
            // Create a deep copy of the work object plane
            _globalPlane = new Plane(_objectFrame);

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
            _fixedFrame = _externalAxis == null;

            // Check if the external axis moves the robot
            if (_externalAxis != null && _externalAxis.MovesRobot == true)
            {
                throw new Exception("An external axis that moves the robot cannot move a work object.");
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
        /// Returns the Configuration Data in RAPID code format.
        /// </summary>
        /// <remarks>
        /// An example output is 
        /// "[FALSE, TRUE, "", [[0, 0, 0], [1, 0, 0, 0]], [[0.0009, -0.0082, 8.0304], [0.9999999, 0.0005131, 0.0000556, 0]]]"
        /// </remarks>
        /// <returns> 
        /// The RAPID data string with work object values. 
        /// </returns>
        public string ToRAPID()
        {
            string result = "";

            // Add robot hold < robhold of bool >
            result += _robotHold ? "[TRUE, " : "[FALSE, ";

            // Add User frame type < ufprog of bool >
            result += _fixedFrame ? "TRUE, " : "FALSE, ";

            // Add mechanical unit (an external axis or robot) < ufmec of string >
            result += _externalAxis != null ? $"\"{_externalAxis.Name}\", " : "\"\", ";

            // Add user frame coordinate < uframe of pose > < trans of pos >
            result += $"[[{_userFrame.Origin.X:0.####}, {_userFrame.Origin.Y:0.####}, {_userFrame.Origin.Z:0.####}], ";

            // Add user frame orientation < uframe of pose > < rot of orient >
            result += $"[{_userFrameOrientation.A:0.#######}, {_userFrameOrientation.B:0.#######}, " +
                $"{_userFrameOrientation.C:0.#######}, {_userFrameOrientation.D:0.#######}]], ";

            // Add object frame coordinate < oframe of pose > < trans of pos >
            result += $"[[{_objectFrame.Origin.X:0.####}, {_objectFrame.Origin.Y:0.####}, {_objectFrame.Origin.Z:0.####}], ";

            // Add object frame orientation < oframe of pose > < rot of orient >
            result += $"[{_objectFrameOrientation.A:0.#######}, {_objectFrameOrientation.B:0.#######}, " +
                $"{_objectFrameOrientation.C:0.#######}, {_objectFrameOrientation.D:0.#######}]]]";

            return result;
        }

        /// <summary>
        /// Returns the RAPID declaration code line of the this Work Object.
        /// </summary>
        /// <returns> 
        /// The RAPID code line. 
        /// </returns>
        public string ToRAPIDDeclaration()
        {
            string result = _scope == Scope.GLOBAL ? "" : $"{Enum.GetName(typeof(Scope), _scope)} ";
            result += $"{Enum.GetName(typeof(VariableType), _variableType)} {_datatype} {_name} := {ToRAPID()};";

            return result;
        }

        /// <summary>
        /// Returns the RAPID declaration code line of the this action.
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> 
        /// The RAPID code line in case a variable name is defined. 
        /// </returns>
        public string ToRAPIDDeclaration(Robot robot)
        {
            if (_name != "" && _name != "wobj0")
            {
                return ToRAPIDDeclaration();
            }

            return string.Empty;
        }

        /// <summary>
        /// Creates declarations and instructions in the RAPID program module inside the RAPID Generator.
        /// </summary>
        /// <remarks>
        /// This method is called inside the RAPID generator.
        /// </remarks>
        /// <param name="RAPIDGenerator"> The RAPID Generator. </param>
        public void ToRAPIDGenerator(RAPIDGenerator RAPIDGenerator)
        {
            if (_name != "" && _name != "wobj0")
            {
                if (!RAPIDGenerator.WorkObjects.ContainsKey(_name))
                {
                    RAPIDGenerator.WorkObjects.Add(_name, this);
                    RAPIDGenerator.ProgramDeclarationsWorkObjectData.Add("    " + ToRAPIDDeclaration(RAPIDGenerator.Robot));
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
                if (_name == null) { return false; }
                if (_name == "") { return false; }
                if (_objectFrame == null) { return false; }
                if (_objectFrame == Plane.Unset) { return false; }
                if (_userFrame == null) { return false; }
                if (_userFrame == Plane.Unset) { return false; }
                return true;
            }
        }

        /// <summary>
        /// Gets or sets the scope. 
        /// </summary>
        public Scope Scope
        {
            get { return _scope; }
            set { _scope = value; }
        }

        /// <summary>
        /// Gets or sets the variable type. 
        /// </summary>
        public VariableType VariableType
        {
            get { return _variableType; }
            set { _variableType = value; }
        }

        /// <summary>
        /// Gets the RAPID datatype. 
        /// </summary>
        public string Datatype
        {
            get { return _datatype; }
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
        /// Gets or sets the user coordinate system, i.e. the position of the current work surface or fixture.
        /// </summary>
        /// <remarks>
        /// If the robot is holding the tool, the user coordinate system is defined in the 
        /// world coordinate system (in the wrist coordinate system if a stationary tool is used). 
        /// For movable user frame (FixedFrame = false), the user frame is continuously defined by the system.
        /// </remarks>
        public Plane UserFrame
        {
            get { return _userFrame; }
            set { _userFrame = value; ReInitialize(); }
        }

        /// <summary>
        /// Gets or set the work object coordinate system as a plane (e.g. the position of the current work object).
        /// </summary>
        /// <remarks>
        /// The object coordinate system is defined in the user coordinate system.
        /// </remarks>
        public Plane ObjectFrame
        {
            get { return _objectFrame; }
            set { _objectFrame = value; ReInitialize(); }
        }

        /// <summary>
        /// Gets or sets the external axis (mechanical unit) with which the robot movements are coordinated. 
        /// </summary>
        /// <remarks>
        /// Only specified in the case of movable user coordinate systems.
        /// </remarks>
        public IExternalAxis ExternalAxis
        {
            get { return _externalAxis; }
            set { _externalAxis = value; ReInitialize(); }
        }

        /// <summary>
        /// Gets a value indicating whether or not a fixed user coordinate system is used.
        /// </summary>
        /// <remarks>
        /// True indicates that the user frame is fixed. 
        /// False indicates that the user coordinate system is movable (e.g. coordinated external axes).s
        /// </remarks>
        public bool FixedFrame
        {
            get { return _fixedFrame; }
            set { _fixedFrame = value; }
        }

        /// <summary>
        /// Gets the global work object plane.
        /// </summary>
        public Plane GlobalWorkObjectPlane
        {
            get { return _globalPlane; }
        }
        #endregion

        #region obsolete
        /// <summary>
        /// Gets or set the work object coordinate system as a plane (e.g. the position of the current work object).
        /// </summary>
        /// <remarks>
        /// The object coordinate system is defined in the user coordinate system.
        /// </remarks>
        [Obsolete("This method is OBSOLETE and will be removed in vesion 5. Use ObjectFrame instead.", false)]
        public Plane Plane
        {
            get { return _objectFrame; }
            set { _objectFrame = value; ReInitialize(); }
        }
        #endregion
    }
}
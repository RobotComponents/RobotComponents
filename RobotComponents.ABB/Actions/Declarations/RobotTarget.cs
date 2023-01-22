// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.ABB.Definitions;
using RobotComponents.ABB.Enumerations;
using RobotComponents.ABB.Utils;
using RobotComponents.ABB.Actions.Interfaces;

namespace RobotComponents.ABB.Actions.Declarations
{
    /// <summary>
    /// Represents the Robot Target declaration. 
    /// This action is used to define the pose of the robot and the external axes.
    /// </summary>
    [Serializable()]
    public class RobotTarget : Action, ITarget, IDeclaration, ISerializable
    {
        #region fields
        private Scope _scope;
        private VariableType _variableType; // variable type
        private string _name; // robot target variable name
        private Plane _plane; // target plane (defines the required position and orientation of the tool)
        private Quaternion _quat; // target plane orientation (as quarternion)
        private int _axisConfig; // the axis configuration of the robot 
        private ExternalJointPosition _externalJointPosition; // the position of the external logical axes
        #endregion

        #region (de)serialization
        /// <summary>
        /// Protected constructor needed for deserialization of the object.  
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization. </param>
        protected RobotTarget(SerializationInfo info, StreamingContext context)
        {
            int version = (int)info.GetValue("Version", typeof(int)); // <-- use this if the (de)serialization changes
            _scope = version >= 2000000 ? (Scope)info.GetValue("Scope", typeof(Scope)) : Scope.GLOBAL;
            _variableType = version >= 2000000 ? (VariableType)info.GetValue("Variable Type", typeof(VariableType)) : (VariableType)info.GetValue("Reference Type", typeof(VariableType));
            _name = (string)info.GetValue("Name", typeof(string));
            _plane = (Plane)info.GetValue("Plane", typeof(Plane));
            _axisConfig = (int)info.GetValue("Axis Configuration", typeof(int));
            _externalJointPosition = (ExternalJointPosition)info.GetValue("External Joint Position", typeof(ExternalJointPosition));
            _quat = HelperMethods.PlaneToQuaternion(_plane);
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
            info.AddValue("Scope", _scope, typeof(Scope));
            info.AddValue("Variable Type", _variableType, typeof(VariableType));
            info.AddValue("Name", _name, typeof(string));
            info.AddValue("Plane", _plane, typeof(Plane));
            info.AddValue("Axis Configuration", _axisConfig, typeof(int));
            info.AddValue("External Joint Position", _externalJointPosition, typeof(ExternalJointPosition));
        }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the Robot Target class.
        /// </summary>
        public RobotTarget()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Robot Target class with an axis conguration set to zero and an undefined External Joint Position.
        /// </summary>
        /// <param name="plane"> The target plane. </param>
        public RobotTarget(Plane plane)
        {
            _scope = Scope.GLOBAL;
            _variableType = VariableType.VAR;
            _name = "";
            _plane = plane;
            _axisConfig = 0;
            _externalJointPosition = new ExternalJointPosition();
            _quat = HelperMethods.PlaneToQuaternion(_plane);
        }

        /// <summary>
        /// Initializes a new instance of the Robot Target class with an axis conguration set to zero and an undefined External Joint Position.
        /// </summary>
        /// <param name="name"> The target name, must be unique. </param>
        /// <param name="plane"> The target plane. </param>
        public RobotTarget(string name, Plane plane)
        {
            _scope = Scope.GLOBAL;
            _variableType = VariableType.VAR;
            _name = name;
            _plane = plane;
            _axisConfig = 0;
            _externalJointPosition = new ExternalJointPosition();
            _quat = HelperMethods.PlaneToQuaternion(_plane);
        }

        /// <summary>
        /// Initializes a new instance of the Robot Target class with an undefined External Joint Position.
        /// </summary>
        /// <param name="plane"> Thr target plane. </param>
        /// <param name="axisConfig"> The axis configuration as a number (0-7). </param>
        public RobotTarget(Plane plane, int axisConfig)
        {
            _scope = Scope.GLOBAL;
            _variableType = VariableType.VAR;
            _name = "";
            _plane = plane;
            _axisConfig = axisConfig;
            _externalJointPosition = new ExternalJointPosition();
            _quat = HelperMethods.PlaneToQuaternion(_plane);
        }

        /// <summary>
        /// Initializes a new instance of the Robot Target class with an undefined External Joint Position.
        /// </summary>
        /// <param name="name"> The target name, must be unique. </param>
        /// <param name="plane"> Thr target plane. </param>
        /// <param name="axisConfig"> The axis configuration as a number (0-7). </param>
        public RobotTarget(string name, Plane plane, int axisConfig)
        {
            _scope = Scope.GLOBAL;
            _variableType = VariableType.VAR;
            _name = name;
            _plane = plane;
            _axisConfig = axisConfig;
            _externalJointPosition = new ExternalJointPosition();
            _quat = HelperMethods.PlaneToQuaternion(_plane);
        }

        /// <summary>
        /// Initializes a new instance of the Robot Target class with an undefined Extenal Joint Position.
        /// The target planes will be reoriented from the reference plane to the world xy-plane.
        /// </summary>
        /// <param name="plane"> The target plane. </param>
        /// <param name="referencePlane"> The reference plane. </param>
        /// <param name="axisConfig"> The axis configuration as a number (0-7). </param>
        public RobotTarget(Plane plane, Plane referencePlane, int axisConfig)
        {
            _scope = Scope.GLOBAL;
            _variableType = VariableType.VAR;
            _name = "";
            _plane = plane;
            _axisConfig = axisConfig;
            _externalJointPosition = new ExternalJointPosition();
            _quat = HelperMethods.PlaneToQuaternion(referencePlane, _plane);

            // Re-orient the plane from the reference coordinate system to the world coordinate system
            Transform orient = Transform.PlaneToPlane(referencePlane, Plane.WorldXY);
            _plane.Transform(orient);
        }

        /// <summary>
        /// Initializes a new instance of the Robot Target class with an undefined Extenal Joint Position.
        /// The target planes will be reoriented from the reference plane to the world xy-plane.
        /// </summary>
        /// <param name="name"> The target name, must be unique. </param>
        /// <param name="plane"> The target plane. </param>
        /// <param name="referencePlane"> The reference plane. </param>
        /// <param name="axisConfig"> The axis configuration as a number (0-7). </param>
        public RobotTarget(string name, Plane plane, Plane referencePlane, int axisConfig)
        {
            _scope = Scope.GLOBAL;
            _variableType = VariableType.VAR;
            _name = name;
            _plane = plane;            
            _axisConfig = axisConfig;
            _externalJointPosition = new ExternalJointPosition();
            _quat = HelperMethods.PlaneToQuaternion(referencePlane, _plane);

            // Re-orient the plane from the reference coordinate system to the world coordinate system
            Transform orient = Transform.PlaneToPlane(referencePlane, Plane.WorldXY);
            _plane.Transform(orient);
        }

        /// <summary>
        /// Initializes a new instance of the Robot Target class.
        /// </summary>
        /// <param name="plane"> The target plane.</param>
        /// <param name="axisConfig"> The axis configuration as a number (0-7). </param>
        /// <param name="externalJointPosition"> The External Joint Position. </param>
        public RobotTarget(Plane plane, int axisConfig, ExternalJointPosition externalJointPosition)
        {
            _scope = Scope.GLOBAL;
            _variableType = VariableType.VAR;
            _name = "";
            _plane = plane;
            _axisConfig = axisConfig;
            _externalJointPosition = externalJointPosition;
            _quat = HelperMethods.PlaneToQuaternion(_plane);
        }

        /// <summary>
        /// Initializes a new instance of the Robot Target class.
        /// </summary>
        /// <param name="name"> The target name, must be unique. </param>
        /// <param name="plane"> The target plane.</param>
        /// <param name="axisConfig"> The axis configuration as a number (0-7). </param>
        /// <param name="externalJointPosition"> The External Joint Position. </param>
        public RobotTarget(string name, Plane plane, int axisConfig, ExternalJointPosition externalJointPosition)
        {
            _scope = Scope.GLOBAL;
            _variableType = VariableType.VAR;
            _name = name;
            _plane = plane;
            _axisConfig = axisConfig;
            _externalJointPosition = externalJointPosition;
            _quat = HelperMethods.PlaneToQuaternion(_plane);
        }

        /// <summary>
        /// Initializes a new instance of the Robot Target class.
        /// The target planes will be reoriented from the reference plane to the world xy-plane.
        /// </summary>
        /// <param name="plane"> The target plane.</param>
        /// <param name="referencePlane"> The Reference plane. </param>
        /// <param name="axisConfig"> The axis configuration as a number (0-7).</param>
        /// <param name="externalJointPosition"> The External Joint Position. </param>
        public RobotTarget(Plane plane, Plane referencePlane, int axisConfig, ExternalJointPosition externalJointPosition)
        {
            _scope = Scope.GLOBAL;
            _variableType = VariableType.VAR;
            _name = "";
            _plane = plane;
            _axisConfig = axisConfig;
            _externalJointPosition = externalJointPosition;
            _quat = HelperMethods.PlaneToQuaternion(referencePlane, _plane);

            // Re-orient the plane to the reference plane
            Transform orient = Transform.PlaneToPlane(referencePlane, Plane.WorldXY);
            _plane.Transform(orient);
        }

        /// <summary>
        /// Initializes a new instance of the Robot Target class.
        /// The target planes will be reoriented from the reference plane to the world xy-plane.
        /// </summary>
        /// <param name="name"> The target name, must be unique.</param>
        /// <param name="plane"> The target plane.</param>
        /// <param name="referencePlane"> The Reference plane. </param>
        /// <param name="axisConfig"> The axis configuration as a number (0-7).</param>
        /// <param name="externalJointPosition"> The External Joint Position. </param>
        public RobotTarget(string name, Plane plane, Plane referencePlane, int axisConfig, ExternalJointPosition externalJointPosition)
        {
            _scope = Scope.GLOBAL;
            _variableType = VariableType.VAR;
            _name = name;
            _plane = plane;
            _axisConfig = axisConfig;
            _externalJointPosition = externalJointPosition;
            _quat = HelperMethods.PlaneToQuaternion(referencePlane, _plane);

            // Re-orient the plane to the reference plane
            Transform orient = Transform.PlaneToPlane(referencePlane, Plane.WorldXY);
            _plane.Transform(orient);
        }

        /// <summary>
        /// Initializes a new instance of the Robot Target class by duplicating an existing Robot Target instance. 
        /// </summary>
        /// <param name="target"> The Robot Target instance to duplicate. </param>
        public RobotTarget(RobotTarget target)
        {
            _scope = target.Scope;
            _variableType = target.VariableType;
            _name = target.Name;
            _plane = new Plane(target.Plane);
            _axisConfig = target.AxisConfig;
            _externalJointPosition = target.ExternalJointPosition.Duplicate();
            _quat = target.Quat;
        }

        /// <summary>
        /// Returns an exact duplicate of this Robot Target instance.
        /// </summary>
        /// <returns> A deep copy of the Robot Target instance. </returns>
        public RobotTarget Duplicate()
        {
            return new RobotTarget(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Robot Target instance as an ITarget. 
        /// </summary>
        /// <returns> A deep copy of the Robot Target instance as an ITarget. </returns>
        public ITarget DuplicateTarget()
        {
            return new RobotTarget(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Robot Target instance as an IDeclaration.
        /// </summary>
        /// <returns> A deep copy of the Robot Target instance as an IDeclaration. </returns>
        public IDeclaration DuplicateDeclaration()
        {
            return new RobotTarget(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Robot Target instance as an Action. 
        /// </summary>
        /// <returns> A deep copy of the Robot Target instance as an Action. </returns>
        public override Action DuplicateAction()
        {
            return new RobotTarget(this);
        }
        #endregion

        #region method
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns> A string that represents the current object. </returns>
        public override string ToString()
        {
            if (!IsValid)
            {
                return "Invalid Robot Target";
            }
            else if (_name != "")
            {
                return $"Robot Target ({_name})";
            }
            else
            {
                return "Robot Target";
            }
        }

        /// <summary>
        /// Returns the Robot Target in RAPID code format, e.g. "[[300, 600, 250], [1, 0, 0, 0], [0, 0, 0, 1] [1000, 9E9, 9E9, 9E9, 9E9, 9E9]]".
        /// </summary>
        /// <returns> The string with robot target values. </returns>
        public string ToRAPID()
        {
            string externalJointPosition = _externalJointPosition.Name;

            if (externalJointPosition == "")
            {
                externalJointPosition = _externalJointPosition.ToRAPID();
            }

            string code = $"[";
            code += $"[{_plane.OriginX:0.##}, ";
            code += $"{_plane.OriginY:0.##}, ";
            code += $"{_plane.OriginZ:0.##}], ";
            code += $"[{_quat.A:0.######}, ";
            code += $"{_quat.B:0.######}, ";
            code += $"{_quat.C:0.######}, ";
            code += $"{_quat.D:0.######}], ";
            code += $"[0,0,0,{_axisConfig}], ";
            code += $"{externalJointPosition}]";

            return code;
        }

        /// <summary>
        /// Returns the RAPID declaration code line of the this action.
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> The RAPID code line. </returns>
        public override string ToRAPIDDeclaration(Robot robot)
        {
            if (_name != "")
            {
                string result = _scope == Scope.GLOBAL ? "" : $"{Enum.GetName(typeof(Scope), _scope)} ";
                result += $"{Enum.GetName(typeof(VariableType), _variableType)} robtarget {_name} := {ToRAPID()};";

                return result;
            }

            return string.Empty;
        }

        /// <summary>
        /// Returns the RAPID instruction code line of the this action. 
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> An emptry string. </returns>
        public override string ToRAPIDInstruction(Robot robot)
        {
            return string.Empty;
        }

        /// <summary>
        /// Creates declarations in the RAPID program module inside the RAPID Generator. 
        /// This method is called inside the RAPID generator.
        /// </summary>
        /// <param name="RAPIDGenerator"> The RAPID Generator. </param>
        public override void ToRAPIDDeclaration(RAPIDGenerator RAPIDGenerator)
        {
            _externalJointPosition.ToRAPIDDeclaration(RAPIDGenerator);

            if (_name != "")
            {
                if (!RAPIDGenerator.Targets.ContainsKey(_name))
                {
                    RAPIDGenerator.Targets.Add(_name, this);
                    RAPIDGenerator.ProgramDeclarations.Add("    " + ToRAPIDDeclaration(RAPIDGenerator.Robot));
                }
            }
        }

        /// <summary>
        /// Creates instructions in the RAPID program module inside the RAPID Generator.
        /// This method is called inside the RAPID generator.
        /// </summary>
        /// <param name="RAPIDGenerator"> The RAPID Generator. </param>
        public override void ToRAPIDInstruction(RAPIDGenerator RAPIDGenerator)
        {
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets a value indicating whether or not the object is valid.
        /// </summary>
        public override bool IsValid
        {
            get
            {
                if (_plane == null) { return false; }
                if (_plane == Plane.Unset) { return false; }
                if (_axisConfig < 0) { return false; }
                if (_axisConfig > 7) { return false; }
                if (_externalJointPosition == null) { return false; }
                if (_externalJointPosition.IsValid == false) { return false; }
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
        /// Gets or sets the Robot Target variable name.
        /// Each Target variable name has to be unique. 
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets or sets the desired position and orientation of the tool center point.
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
                _quat = HelperMethods.PlaneToQuaternion(_plane);
            }
        }

        /// <summary>
        /// Gets or sets the desired orientation of the tool center point.
        /// </summary>
        public Quaternion Quat
        {
            get 
            { 
                return _quat; 
            }
            set 
            { 
                _quat = value; 
                _plane = HelperMethods.QuaternionToPlane(_plane.Origin, _quat);
            }
        }

        /// <summary>
        /// Gets or set the axis configuration.
        /// Min. value 0. Max. value 7.
        /// </summary>
        public int AxisConfig
        {
            get { return _axisConfig; }
            set { _axisConfig = value; }
        }

        /// <summary>
        /// Gets or sets the External Joint Position.
        /// </summary>
        public ExternalJointPosition ExternalJointPosition
        {
            get { return _externalJointPosition; }
            set { _externalJointPosition = value; }
        }
        #endregion
    }

}

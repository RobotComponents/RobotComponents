// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text.RegularExpressions;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.ABB.Enumerations;
using RobotComponents.ABB.Utils;

namespace RobotComponents.ABB.Definitions
{
    /// <summary>
    /// Represent Load Data.
    /// </summary>
    public class LoadData : ISerializable
    {
        #region fields
        private Scope _scope;
        private VariableType _variableType;
        private const string _datatype = "loaddata";
        private string _name;
        private double _mass;
        private Point3d _centerOfGravity;
        private Quaternion _axesOfMoment;
        private Vector3d _inertialMoments;
        #endregion

        #region (de)serialization
        /// <summary>
        /// Protected constructor needed for deserialization of the object.  
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization. </param>
        protected LoadData(SerializationInfo info, StreamingContext context)
        {
            //int version = (int)info.GetValue("Version", typeof(int)); // <-- use this if the (de)serialization changes
            _scope = (Scope)info.GetValue("Scope", typeof(Scope));
            _variableType = (VariableType)info.GetValue("Variable Type", typeof(VariableType)); 
            _name = (string)info.GetValue("Name", typeof(string));
            _mass = (double)info.GetValue("Mass", typeof(double));
            _centerOfGravity = (Point3d)info.GetValue("Center of Gravity", typeof(Point3d));
            _axesOfMoment = (Quaternion)info.GetValue("Axes of Moment", typeof(Quaternion));
            _inertialMoments = (Vector3d)info.GetValue("Inertial Moments", typeof(Vector3d));
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
            info.AddValue("Mass", _mass, typeof(double));
            info.AddValue("Center of Gravity", _centerOfGravity, typeof(Point3d));
            info.AddValue("Axes of Moment", _axesOfMoment, typeof(Quaternion));
            info.AddValue("Inertial Moments", _inertialMoments, typeof(Vector3d));
        }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes a new instance of the Load Data class with the default load load0.
        /// </summary>
        public LoadData()
        {
            _scope = Scope.GLOBAL;
            _variableType = VariableType.PERS;
            _name = "load0";
            _mass = 0.001;
            _centerOfGravity = new Point3d(0, 0, 0.001);
            _axesOfMoment = new Quaternion(1, 0, 0, 0);
            _inertialMoments = new Vector3d(0, 0, 0);
        }

        /// <summary>
        /// Initializes a new instance of the Load Data class.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="mass"></param>
        /// <param name="centerOfGravity"></param>
        /// <param name="axesOfMoment"></param>
        /// <param name="inertialMoments"></param>
        public LoadData(string name, double mass, Point3d centerOfGravity, Quaternion axesOfMoment, Vector3d inertialMoments)
        {
            _scope = Scope.GLOBAL;
            _variableType = VariableType.PERS;
            _name = name;
            _mass = mass;
            _centerOfGravity = centerOfGravity;
            _axesOfMoment = axesOfMoment;
            _inertialMoments = inertialMoments;
        }

        /// <summary>
        /// Initializes a new instance of the Load Data class by duplicating an existing Load Data instance. 
        /// </summary>
        /// <param name="loadData"> The Load Data instance to duplicate. </param>
        public LoadData(LoadData loadData)
        {
            _scope = loadData.Scope;
            _variableType = loadData.VariableType;
            _name = loadData.Name;
            _mass = loadData.Mass;
            _centerOfGravity = loadData.CenterOfGravity;
            _axesOfMoment = loadData.AxesOfMoment;
            _inertialMoments = loadData.InertialMoments;
        }

        /// <summary>
        /// Returns an exact duplicate of this Load Data instance.
        /// </summary>
        /// <returns> A deep copy of the Load Data instance. </returns>
        public LoadData Duplicate()
        {
            return new LoadData(this);
        }
        #endregion

        #region parse
        /// <summary>
        /// Initializes a new instance of the Load DAta class from a rapid data string.
        /// </summary>
        /// <remarks>
        /// Only used for the Parse and TryParse methods. Therefore, this constructor is private. 
        /// </remarks>
        /// <param name="rapidData"></param>
        private LoadData(string rapidData)
        {
            string clean = Regex.Replace(rapidData, @"[\s;:\[\]\(\){}]", "");

            string[] split = clean.Split('=');
            string type;
            string value;

            // Check for equal signs
            switch (split.Length)
            {
                case 1:
                    type = $"VAR{_datatype}";
                    value = split[0];
                    break;
                case 2:
                    type = split[0];
                    value = split[1];
                    break;
                default:
                    throw new InvalidCastException("Invalid RAPID data string: More than one equal sign defined.");
            }

            // Scope
            switch (type)
            {
                case string t when t.StartsWith("LOCAL"):
                    _scope = Scope.LOCAL;
                    type = type.ReplaceFirst("LOCAL", "");
                    break;
                case string t when t.StartsWith("TASK"):
                    _scope = Scope.TASK;
                    type = type.ReplaceFirst("TASK", "");
                    break;
                default:
                    _scope = Scope.GLOBAL;
                    break;
            }

            // Variable type
            switch (type)
            {
                case string t when t.StartsWith("VAR"):
                    _variableType = VariableType.VAR;
                    type = type.ReplaceFirst("VAR", "");
                    break;
                case string t when t.StartsWith("CONST"):
                    _variableType = VariableType.CONST;
                    type = type.ReplaceFirst("CONST", "");
                    break;
                case string t when t.StartsWith("PERS"):
                    _variableType = VariableType.PERS;
                    type = type.ReplaceFirst("PERS", "");
                    break;
                default:
                    throw new InvalidCastException("Invalid RAPID data string: The scope or variable type is incorrect.");
            }

            // Datatype
            if (type.StartsWith(_datatype) == false)
            {
                throw new InvalidCastException("Invalid RAPID data string: The datatype does not match.");
            }

            // Name
            _name = type.ReplaceFirst(_datatype, "");

            // Value
            string[] values = value.Split(',');

            if (values.Length == 11)
            {
                _mass = double.Parse(values[0]);

                _centerOfGravity = new Point3d
                {
                    X = double.Parse(values[1]),
                    Y = double.Parse(values[2]),
                    Z = double.Parse(values[3])
                };

                _axesOfMoment = new Quaternion
                {
                    A = double.Parse(values[4]),
                    B = double.Parse(values[5]),
                    C = double.Parse(values[6]),
                    D = double.Parse(values[7])
                };

                _inertialMoments = new Vector3d
                {
                    X = double.Parse(values[8]),
                    Y = double.Parse(values[9]),
                    Z = double.Parse(values[10])
                };
            }
            else
            {
                throw new InvalidCastException("Invalid RAPID data string: The number of values does not match.");
            }
        }

        /// <summary>
        /// Returns a Load Data instance constructed from a RAPID data string. 
        /// </summary>
        /// <param name="rapidData"> The RAPID data string. s</param>
        public static LoadData Parse(string rapidData)
        {
            return new LoadData(rapidData);
        }

        /// <summary>
        /// Attempts to parse a RAPID data string into a Load Data instance.  
        /// </summary>
        /// <param name="rapidData"> The RAPID data string. </param>
        /// <param name="loadData"> The Load Data intance. </param>
        /// <returns> 
        /// True on success, false on failure. 
        /// </returns>
        public static bool TryParse(string rapidData, out LoadData loadData)
        {
            try
            {
                loadData = new LoadData(rapidData);
                return true;
            }
            catch
            {
                loadData = new LoadData();
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
                return "Invalid Load Data";
            }
            else if (_name != "")
            {
                return $"Load Data ({_name})";
            }
            else
            {
                return $"Load Data";
            }
        }

        /// <summary>
        /// Returns the loaddata in RAPID code format.
        /// </summary>
        /// <remarks>
        /// An exmaple output is "[ 5, [50, 0, 50], [1, 0, 0, 0], 0, 0, 0]".
        /// </remarks>
        /// <returns> 
        /// The string with loaddata values. 
        /// </returns>
        public string ToRAPID()
        {
            string result = $"[{_mass:0.######}, ";
            result += $"[{_centerOfGravity.X:0.######}, {_centerOfGravity.Y:0.######}, {_centerOfGravity.Z:0.######}], ";
            result += $"[{_axesOfMoment.A:0.######}, {_axesOfMoment.B:0.######}, ";
            result += $"{_axesOfMoment.C:0.######}, {_axesOfMoment.D:0.######}], ";
            result += $"{_inertialMoments.X:0.######}, {_inertialMoments.Y:0.######}, {_inertialMoments.Z:0.######}]";

            return result;
        }

        /// <summary>
        /// Returns the RAPID declaration code line of the this Load Data.
        /// </summary>
        /// <returns> 
        /// The RAPID code line. 
        /// </returns>
        public string ToRAPIDDeclaration()
        {
            string result = _scope == Scope.GLOBAL ? "" : $"{Enum.GetName(typeof(Scope), _scope)} ";
            result += $"{Enum.GetName(typeof(VariableType), _variableType)} {_datatype} {_name} := {this.ToRAPID()};";

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
                if (_mass < 0) { return false; }
                if (_centerOfGravity == Point3d.Unset) { return false; }
                if (_axesOfMoment.IsZero == true) { return false; }
                if (_axesOfMoment.IsValid == false) { return false; }
                if (_inertialMoments == Vector3d.Unset) { return false; }
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
        /// Gets the RAPID datatype. 
        /// </summary>
        public string DataType
        {
            get { return _datatype; }
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
        /// Gets or sets the load data name.
        /// </summary>
        /// <remarks>
        /// Each name has to be unique.
        /// </remarks>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets or sets the weight of the load in kg.
        /// </summary>
        public double Mass
        {
            get { return _mass; }
            set { _mass = value; }
        }

        /// <summary>
        /// Gets the position of the center of gravity of the load.
        /// </summary>
        public Point3d CenterOfGravity
        {
            get { return _centerOfGravity; }
            set { _centerOfGravity = value; }
        }

        /// <summary>
        /// Gets the orientation of the load coordinate system defined by the principal inertial axes of the tool load. 
        /// </summary>
        /// <remarks>
        /// Expressed in the wrist coordinate system as a quaternion (q1, q2, q3, q4).
        /// </remarks>
        public Quaternion AxesOfMoment
        {
            get { return _axesOfMoment; }
            set { _axesOfMoment = value; }
        }

        /// <summary>
        /// Gets or set the moment of inertia of the load in kgm2.
        /// </summary>
        public Vector3d InertialMoments
        {
            get { return _inertialMoments; }
            set { _inertialMoments = value; }
        }
        #endregion
    }
}

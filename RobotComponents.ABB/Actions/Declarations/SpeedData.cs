// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Collections.Generic;
// RobotComponents Libs
using RobotComponents.ABB.Definitions;
using RobotComponents.ABB.Enumerations;
using RobotComponents.ABB.Utils;

namespace RobotComponents.ABB.Actions.Declarations
{
    /// <summary>
    /// Represents a predefined or user definied Speed Data declaration.
    /// </summary>
    /// <remarks>
    /// This action is used to specify the velocity at which both the robot and the external axes move.
    /// </remarks>
    [Serializable()]
    public class SpeedData : IAction, IDeclaration, ISerializable
    {
        #region fields
        private Scope _scope = Scope.GLOBAL;
        private VariableType _variableType = VariableType.VAR;
        private const string _datatype = "speeddata";
        private string _name = "";
        private double _v_tcp;
        private double _v_ori;
        private double _v_leax;
        private double _v_reax;
        private bool _isPredefined;
        private readonly bool _isExactPredefinedValue;

        private static readonly string[] _validPredefinedNames = new string[] { "v5", "v10", "v20", "v30", "v40", "v50", "v60", "v80", "v100", "v150", "v200", "v300", "v400", "v500", "v600", "v800", "v1000", "v1500", "v2000", "v2500", "v3000", "v4000", "v5000", "v6000", "v7000" };
        private static readonly double[] _validPredefinedValues = new double[] { 5, 10, 20, 30, 40, 50, 60, 80, 100, 150, 200, 300, 400, 500, 600, 800, 1000, 1500, 2000, 2500, 3000, 4000, 5000, 6000, 7000 };

        #endregion

        #region (de)serialization
        /// <summary>
        /// Protected constructor needed for deserialization of the object.  
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization. </param>
        protected SpeedData(SerializationInfo info, StreamingContext context)
        {
            //Version version = (Version)info.GetValue("Version", typeof(Version)); // <-- use this if the (de)serialization changes
            _scope = (Scope)info.GetValue("Scope", typeof(Scope));
            _variableType = (VariableType)info.GetValue("Variable Type", typeof(VariableType));
            _name = (string)info.GetValue("Name", typeof(string));
            _v_tcp = (double)info.GetValue("v_tcp", typeof(double));
            _v_ori = (double)info.GetValue("v_ori", typeof(double));
            _v_leax = (double)info.GetValue("v_leax", typeof(double));
            _v_reax = (double)info.GetValue("v_reax", typeof(double));
            _isPredefined = (bool)info.GetValue("Predefined", typeof(bool));
            _isExactPredefinedValue = (bool)info.GetValue("Exact Predefined Value", typeof(bool));
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
            info.AddValue("Variable Type", _variableType, typeof(VariableType));
            info.AddValue("Scope", _scope, typeof(Scope));
            info.AddValue("Name", _name, typeof(string));
            info.AddValue("v_tcp", _v_tcp, typeof(double));
            info.AddValue("v_ori", _v_ori, typeof(double));
            info.AddValue("v_leax", _v_leax, typeof(double));
            info.AddValue("v_reax", _v_reax, typeof(double));
            info.AddValue("Predefined", _isPredefined, typeof(bool));
            info.AddValue("Exact Predefined Value", _isExactPredefinedValue, typeof(bool));
        }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the Speed Data class.
        /// </summary>
        public SpeedData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Speed Data class with predefined values.
        /// </summary>
        /// <param name="v_tcp"> The velocity of the tool center point (TCP) in mm/s. </param>
        public SpeedData(double v_tcp)
        {
            // Get nearest predefined speeddata value
            double tcp = _validPredefinedValues.Aggregate((x, y) => Math.Abs(x - v_tcp) < Math.Abs(y - v_tcp) ? x : y);
            _isExactPredefinedValue = (v_tcp - tcp) == 0;

            // Set other fields
            _name = $"v{tcp}";
            _v_tcp = tcp;
            _v_ori = 500;
            _v_leax = 5000;
            _v_reax = 1000;
            _isPredefined = true;
        }

        /// <summary>
        /// Initializes a new instance of the Speed Data class with predefined values.
        /// </summary>
        /// <param name="v_tcp"> The velocity of the tool center point (TCP) in mm/s. </param>
        public SpeedData(int v_tcp)
        {
            // Get nearest predefined speeddata value
            double tcp = _validPredefinedValues.Aggregate((x, y) => Math.Abs(x - v_tcp) < Math.Abs(y - v_tcp) ? x : y);
            _isExactPredefinedValue = (v_tcp - tcp) == 0;

            // Set other fields
            _name = $"v{tcp}";
            _v_tcp = tcp;
            _v_ori = 500;
            _v_leax = 5000;
            _v_reax = 1000;
            _isPredefined = true;
        }

        /// <summary>
        /// Initializes a new instance of the Speed Data class with custom values.
        /// </summary>
        /// <param name="v_tcp"> The velocity of the tool center point (TCP) in mm/s. </param>
        /// <param name="v_ori"> The reorientation velocity of the TCP expressed in degrees/s. </param>
        /// <param name="v_leax"> The velocity of linear external axes in mm/s. </param>
        /// <param name="v_reax"> The velocity of rotating external axes in degrees/s. </param>
        public SpeedData(double v_tcp, double v_ori = 500, double v_leax = 5000, double v_reax = 1000)
        {
            _v_tcp = v_tcp;
            _v_ori = v_ori;
            _v_leax = v_leax;
            _v_reax = v_reax;
            _isPredefined = false;
            _isExactPredefinedValue = false;
        }

        /// <summary>
        /// Initializes a new instance of the Speed Data class with custom values.
        /// </summary>
        /// <param name="name"> The Speed Data variable name, must be unique. </param>
        /// <param name="v_tcp"> The velocity of the tool center point (TCP) in mm/s. </param>
        /// <param name="v_ori"> The reorientation velocity of the TCP expressed in degrees/s. </param>
        /// <param name="v_leax"> The velocity of linear external axes in mm/s. </param>
        /// <param name="v_reax"> The velocity of rotating external axes in degrees/s. </param>
        public SpeedData(string name, double v_tcp, double v_ori = 500, double v_leax = 5000, double v_reax = 1000)
        {
            _name = name;
            _v_tcp = v_tcp;
            _v_ori = v_ori;
            _v_leax = v_leax;
            _v_reax = v_reax;
            _isPredefined = false;
            _isExactPredefinedValue = false;
        }

        /// <summary>
        /// Initializes a new instance of the Speed Data class by duplicating an existing Speed Data instance. 
        /// </summary>
        /// <param name="speeddata"> The Speed Data instance to duplicate. </param>
        public SpeedData(SpeedData speeddata)
        {
            _scope = speeddata.Scope;
            _variableType = speeddata.VariableType;
            _name = speeddata.Name;
            _v_tcp = speeddata.V_TCP;
            _v_ori = speeddata.V_ORI;
            _v_leax = speeddata.V_LEAX;
            _v_reax = speeddata.V_REAX;
            _isPredefined = speeddata.IsPreDefined;
            _isExactPredefinedValue = speeddata.IsExactPredefinedValue;
        }

        /// <summary>
        /// Initializes a new instance of the Speed Data clas from an enumeration.
        /// </summary>
        /// <param name="predefinedSpeedData"> Predefined speeddata as an enumeration </param>
        public static SpeedData GetPredefinedSpeedData(PredefinedSpeedData predefinedSpeedData)
        {
            return new SpeedData((double)predefinedSpeedData);
        }

        /// <summary>
        /// Returns an exact duplicate of this Speed Data instance.
        /// </summary>
        /// <returns> 
        /// A deep copy of the Speed Data instance. 
        /// </returns>
        public SpeedData Duplicate()
        {
            return new SpeedData(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Speed Data instance as an IDeclaration.
        /// </summary>
        /// <returns> 
        /// A deep copy of the SpeedData instance as an IDeclaration. 
        /// </returns>
        public IDeclaration DuplicateDeclaration()
        {
            return new SpeedData(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Spee Data instance as an Action. 
        /// </summary>
        /// <returns> 
        /// A deep copy of the Speed Data instance as an Action. 
        /// </returns>
        public IAction DuplicateAction()
        {
            return new SpeedData(this);
        }
        #endregion

        #region parse
        /// <summary>
        /// Initializes a new instance of the Speed Data class from a rapid data string.
        /// </summary>
        /// <remarks>
        /// Only used for the Parse and TryParse methods. Therefore, this constructor is private. 
        /// </remarks>
        /// <param name="rapidData"></param>
        private SpeedData(string rapidData)
        {
            this.SetRapidDataFromString(rapidData, out string[] values);

            _isPredefined = _validPredefinedNames.Contains(_name);

            if (values.Length == 4)
            {
                _v_tcp = double.Parse(values[0]);
                _v_ori = double.Parse(values[1]);
                _v_leax = double.Parse(values[2]);
                _v_reax = double.Parse(values[3]);
            }
            else
            {
                throw new InvalidCastException("Invalid RAPID data string: The number of values does not match.");
            }
        }

        /// <summary>
        /// Returns a Speed Data instance constructed from a RAPID data string. 
        /// </summary>
        /// <param name="rapidData"> The RAPID data string. s</param>
        public static SpeedData Parse(string rapidData)
        {
            return new SpeedData(rapidData);
        }

        /// <summary>
        /// Attempts to parse a RAPID data string into a Speed Data instance.  
        /// </summary>
        /// <param name="rapidData"> The RAPID data string. </param>
        /// <param name="speedData"> The Speed Data intance. </param>
        /// <returns> 
        /// True on success, false on failure. 
        /// </returns>
        public static bool TryParse(string rapidData, out SpeedData speedData)
        {
            try
            {
                speedData = new SpeedData(rapidData);
                return true;
            }
            catch
            {
                speedData = new SpeedData();
                return false;
            }
        }
        #endregion

        #region method
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
                return "Invalid Speed Data";
            }
            else if (_isPredefined == true)
            {
                return $"Predefined Speed Data ({_name})";
            }
            else if (_name != "")
            {
                return $"Custom Speed Data ({_name})";
            }
            else
            {
                return "Custom Speed Data";
            }
        }

        /// <summary>
        /// Returns the Speed Data in RAPID code format.
        /// </summary>
        /// <remarks>
        /// An example output is "[200, 500, 5000, 1000]".s
        /// </remarks>
        /// <returns> The string with speed data values. </returns>
        public string ToRAPID()
        {
            return $"[{_v_tcp}, {_v_ori}, {_v_leax}, {_v_reax}]";
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
            if (_isPredefined == false && _name != "")
            {
                string result = _scope == Scope.GLOBAL ? "" : $"{Enum.GetName(typeof(Scope), _scope)} ";
                result += $"{Enum.GetName(typeof(VariableType), _variableType)} {_datatype} {_name} := {ToRAPID()};";

                return result;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Returns the RAPID instruction code line of the this action. 
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> 
        /// An empty string. 
        /// </returns>
        public string ToRAPIDInstruction(Robot robot)
        {
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
            if (_isPredefined == false)
            {
                if (_name != "")
                {
                    if (!RAPIDGenerator.SpeedDatas.ContainsKey(_name))
                    {
                        RAPIDGenerator.SpeedDatas.Add(_name, this);
                        RAPIDGenerator.ProgramDeclarations.Add("    " + ToRAPIDDeclaration(RAPIDGenerator.Robot));
                    }
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
                if (_v_tcp <= 0) { return false; }
                if (_v_ori <= 0) { return false; }
                if (_v_leax <= 0) { return false; }
                if (_v_reax <= 0) { return false; }
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
        /// Gets or sets the speeddata variable name.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets or sets the velocity of the tool center point (TCP) in mm/s.
        /// </summary>
        /// <remarks>
        /// If a stationary tool or coordinated external axes are used, the velocity is specified relative to the work object.
        /// </remarks>
        public double V_TCP
        {
            get { return _v_tcp; }
            set { _v_tcp = value; }
        }

        /// <summary>
        /// Gets or sets the reorientation velocity of the TCP expressed in degrees/s. 
        /// </summary>
        /// <remarks>
        /// If a stationary tool or coordinated external axes are used, the velocity is specified relative to the work object.
        /// </remarks>
        public double V_ORI
        {
            get { return _v_ori; }
            set { _v_ori = value; }
        }

        /// <summary>
        /// Gets or sets the velocity of linear external axes in mm/s.
        /// </summary>
        public double V_LEAX
        {
            get { return _v_leax; }
            set { _v_leax = value; }
        }

        /// <summary>
        /// Gets or sets the velocity of rotating external axes in degrees/s.
        /// </summary>
        public double V_REAX
        {
            get { return _v_reax; }
            set { _v_reax = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this speeddata is a predefined speeddata. 
        /// </summary>
        public bool IsPreDefined
        {
            get { return _isPredefined; }
            set { _isPredefined = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this speeddata was constructed from an exact predefined speeddata value. 
        /// </summary>
        /// <remarks>
        /// If false, the nearest predefined speedata or a custom speeddata was used. 
        /// </remarks>
        public bool IsExactPredefinedValue
        {
            get { return _isExactPredefinedValue; }
        }

        /// <summary>
        /// Gets the valid predefined speeddata variable names.
        /// </summary>
        public static string[] ValidPredefinedNames
        {
            get { return _validPredefinedNames; }
        }

        /// <summary>
        /// Gets the valid predefined speeddata values.
        /// </summary>
        public static double[] ValidPredefinedValues
        {
            get { return _validPredefinedValues; }
        }

        /// <summary>
        /// Gets the valid predefined data as a dictionary.
        /// </summary>
        public static Dictionary<string, double> ValidPredefinedData
        {
            get { return _validPredefinedNames.Zip(_validPredefinedValues, (s, i) => new { s, i }).ToDictionary(item => item.s, item => item.i); }
        }
        #endregion
    }
}
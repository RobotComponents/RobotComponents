// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
// RobotComponents Libs
using RobotComponents.ABB.Definitions;
using RobotComponents.ABB.Enumerations;
using RobotComponents.ABB.Utils;

namespace RobotComponents.ABB.Actions.Declarations
{
    /// <summary>
    /// Represents the Configuration Data declaration. 
    /// </summary>
    [Serializable()]
    public class ConfigurationData : IAction, IDeclaration, ISerializable
    {
        #region fields
        private Scope _scope;
        private VariableType _variableType;
        private static readonly string _datatype = "confdata";
        private string _name;
        private int _cf1;
        private int _cf4;
        private int _cf6;
        private int _cfx;
        #endregion

        #region (de)serialization
        /// <summary>
        /// Protected constructor needed for deserialization of the object.  
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization. </param>
        protected ConfigurationData(SerializationInfo info, StreamingContext context)
        {
            //Version version = (Version)info.GetValue("Version", typeof(Version)); // <-- use this if the (de)serialization changes
            _scope = (Scope)info.GetValue("Scope", typeof(Scope));
            _variableType = (VariableType)info.GetValue("Variable Type", typeof(VariableType));
            _name = (string)info.GetValue("Name", typeof(string));
            _cf1 = (int)info.GetValue("cf1", typeof(int));
            _cf4 = (int)info.GetValue("cf4", typeof(int));
            _cf6 = (int)info.GetValue("cf6", typeof(int));
            _cfx = (int)info.GetValue("cfx", typeof(int));
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
            info.AddValue("cf1", _cf1, typeof(int));
            info.AddValue("cf4", _cf4, typeof(int));
            info.AddValue("cf6", _cf6, typeof(int));
            info.AddValue("cfx", _cfx, typeof(int));
        }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the Configuration Data class.
        /// </summary>
        public ConfigurationData()
        {

        }

        /// <summary>
        /// Initializes a new instance of the Configuration Data class.
        /// </summary>
        /// <param name="cf1"> The current quadrant of axis 1. </param>
        /// <param name="cf4"> The current quadrant of axis 4. </param>
        /// <param name="cf6"> The current quadrant of axis 6. </param>
        /// <param name="cfx"> The current robot configuration. </param>
        public ConfigurationData(int cf1, int cf4, int cf6, int cfx)
        {
            _scope = Scope.GLOBAL;
            _variableType = VariableType.VAR;
            _name = "";
            _cf1 = cf1;
            _cf4 = cf4;
            _cf6 = cf6;
            _cfx = cfx;
        }

        /// <summary>
        /// Initializes a new instance of the Configuration Data class.
        /// </summary>
        /// <param name="name"> The confiruation data name, must be unique. </param>
        /// <param name="cf1"> The current quadrant of axis 1. </param>
        /// <param name="cf4"> The current quadrant of axis 4. </param>
        /// <param name="cf6"> The current quadrant of axis 6. </param>
        /// <param name="cfx"> The current robot configuration. </param>
        public ConfigurationData(string name, int cf1, int cf4, int cf6, int cfx)
        {
            _scope = Scope.GLOBAL;
            _variableType = VariableType.VAR;
            _name = name;
            _cf1 = cf1;
            _cf4 = cf4;
            _cf6 = cf6;
            _cfx = cfx;
        }

        /// <summary>
        /// Initializes a new instance of the Configuration Data class by duplicating an existing Configuration Data instance. 
        /// </summary>
        /// <param name="configurationData"> The Configuration Data instance to duplicate. </param>
        public ConfigurationData(ConfigurationData configurationData)
        {
            _scope = configurationData.Scope;
            _variableType = configurationData.VariableType;
            _name = configurationData.Name;
            _cf1 = configurationData.Cf1;
            _cf4 = configurationData.Cf4;
            _cf6 = configurationData.Cf6;
            _cfx = configurationData.Cfx;
        }

        /// <summary>
        /// Returns an exact duplicate of this Configuration Data instance.
        /// </summary>
        /// <returns> A deep copy of the Configuration Data instance. </returns>
        public ConfigurationData Duplicate()
        {
            return new ConfigurationData(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Configuration Data instance as an IDeclaration.
        /// </summary>
        /// <returns> 
        /// A deep copy of the Configuration Data instance as an IDeclaration.
        /// </returns>
        public IDeclaration DuplicateDeclaration()
        {
            return new ConfigurationData(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Configuration Data instance as an Action. 
        /// </summary>
        /// <returns> 
        /// A deep copy of the Configuration Data instance as an Action. 
        /// </returns>
        public IAction DuplicateAction()
        {
            return new ConfigurationData(this);
        }
        #endregion

        #region parse
        /// <summary>
        /// Initializes a new instance of the Configuration Data class from a rapid data string.
        /// </summary>
        /// <remarks>
        /// Only used for the Parse and TryParse methods. Therefore, this constructor is private. 
        /// </remarks>
        /// <param name="rapidData"> The RAPID data string. </param>
        private ConfigurationData(string rapidData)
        {
            this.SetRapidDataFromString(rapidData, out string[] values);

            if (values.Length == 4)
            {
                _cf1 = int.Parse(values[0]);
                _cf4 = int.Parse(values[1]);
                _cf6 = int.Parse(values[2]);
                _cfx = int.Parse(values[3]);
            }
            else
            {
                throw new InvalidCastException("Invalid RAPID data string: The number of values does not match.");
            }
        }

        /// <summary>
        /// Returns a Configuration Data instance constructed from a RAPID data string. 
        /// </summary>
        /// <param name="rapidData"> The RAPID data string. s</param>
        public static ConfigurationData Parse(string rapidData)
        {
            return new ConfigurationData(rapidData);
        }

        /// <summary>
        /// Attempts to parse a RAPID data string into a Configuration Data instance.  
        /// </summary>
        /// <param name="rapidData"> The RAPID data string. </param>
        /// <param name="configurationData"> The Configuration Data intance. </param>
        /// <returns> 
        /// True on success, false on failure. 
        /// </returns>
        public static bool TryParse(string rapidData, out ConfigurationData configurationData)
        {
            try
            {
                configurationData = new ConfigurationData(rapidData);
                return true;
            }
            catch
            {
                configurationData = new ConfigurationData();
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
                return "Invalid Configuration Data";
            }
            else if (_name != "")
            {
                return $"Configuration Data ({_name})";
            }
            else
            {
                return "Configuration Data";
            }
        }

        /// <summary>
        /// Returns the Configuration Data in RAPID code format.
        /// </summary>
        /// <remarks>
        /// An example output is "[0, 0, 0, 1]".
        /// </remarks>
        /// <returns> 
        /// The string with robot target values. 
        /// </returns>
        public string ToRAPID()
        {
            return $"[{_cf1}, {_cf4}, {_cf6}, {_cfx}]";
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
            if (_name != "")
            {
                string result = _scope == Scope.GLOBAL ? "" : $"{Enum.GetName(typeof(Scope), _scope)} ";
                result += $"{Enum.GetName(typeof(VariableType), _variableType)} {_datatype} {_name} := {ToRAPID()};";

                return result;
            }

            return string.Empty;
        }

        /// <summary>
        /// Returns the RAPID instruction code line of the this action. 
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> 
        /// An emptry string. 
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
            if (_name != "")
            {
                if (!RAPIDGenerator.ConfigurationDatas.ContainsKey(_name))
                {
                    RAPIDGenerator.ConfigurationDatas.Add(_name, this);
                    RAPIDGenerator.ProgramDeclarations.Add("    " + ToRAPIDDeclaration(RAPIDGenerator.Robot));
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
        /// Gets or sets the Configuration Data variable name. 
        /// </summary>
        /// <remaks>
        /// Each variable name has to be unique.
        /// </remaks>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets ors sets the current quadrant of axis 1.
        /// </summary>
        public int Cf1
        {
            get { return _cf1; }
            set { _cf1 = value; }
        }

        /// <summary>
        /// Gets ors sets the current quadrant of axis 4.
        /// </summary>
        public int Cf4
        {
            get { return _cf4; }
            set { _cf4 = value; }
        }

        /// <summary>
        /// Gets ors sets the current quadrant of axis 6.
        /// </summary>
        public int Cf6
        {
            get { return _cf6; }
            set { _cf6 = value; }
        }

        /// <summary>
        /// Gets or sets the current robot configuration.
        /// </summary>
        public int Cfx
        {
            get { return _cfx; }
            set { _cfx = value; }
        }
        #endregion
    }
}

// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
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
using RobotComponents.ABB.Actions.Interfaces;
using RobotComponents.ABB.Utils;

namespace RobotComponents.ABB.Actions.Declarations
{
    /// <summary>
    /// Represents a predefined or user definied Zone Data declaration.
    /// </summary>
    /// <remarks>
    /// This action is used to specify how a position is to be terminated.
    /// </remarks>
    [Serializable()]
    public class ZoneData : Action, IDeclaration, ISerializable
    {
        #region fields
        private Scope _scope;
        private VariableType _variableType;
        private static readonly string _datatype = "zonedata";
        private string _name;
        private bool _finep;
        private double _pzone_tcp; 
        private double _pzone_ori;
        private double _pzone_eax;
        private double _zone_ori;
        private double _zone_leax;
        private double _zone_reax;
        private bool _isPredefined; 
        private readonly bool _isExactPredefinedValue; 

        private static readonly string[] _validPredefinedNames = new string[] { "fine", "z0", "z1", "z5", "z10", "z15", "z20", "z30", "z40", "z50", "z60", "z80", "z100", "z150", "z200" };
        private static readonly double[] _validPredefinedValues = new double[] { -1, 0, 1, 5, 10, 15, 20, 30, 40, 50, 60, 80, 100, 150, 200 };
        private static readonly double[] _predefinedPathZoneTCP = new double[] { 0, 0.3, 1, 5, 10, 15, 20, 30, 40, 50, 60, 80, 100, 150, 200 };
        private static readonly double[] _predefinedPathZoneOri = new double[] { 0, 0.3, 1, 8, 15, 23, 30, 45, 60, 75, 90, 120, 150, 225, 300 };
        private static readonly double[] _predefinedPathZoneEax = new double[] { 0, 0.3, 1, 8, 15, 23, 30, 45, 60, 75, 90, 120, 150, 225, 300 };
        private static readonly double[] _predefinedZoneOri = new double[] { 0, 0.03, 0.1, 0.8, 1.5, 2.3, 3, 4.5, 6, 7.5, 9, 12, 15, 23, 30 };
        private static readonly double[] _predefinedZoneLeax = new double[] { 0, 0.3, 1, 8, 15, 23, 30, 45, 60, 75, 90, 120, 150, 225, 300 };
        private static readonly double[] _predefinedZoneReax = new double[] { 0, 0.03, 0.1, 0.8, 1.5, 2.3, 3, 4.5, 6, 7.5, 9, 12, 15, 23, 30 };
        #endregion

        #region (de)serialization
        /// <summary>
        /// Protected constructor needed for deserialization of the object.  
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization. </param>
        protected ZoneData(SerializationInfo info, StreamingContext context)
        {
            int version = (int)info.GetValue("Version", typeof(int)); // <-- use this if the (de)serialization changes
            _scope = version >= 2000000 ? (Scope)info.GetValue("Scope", typeof(Scope)) : Scope.GLOBAL;
            _variableType = version >= 2000000 ? (VariableType)info.GetValue("Variable Type", typeof(VariableType)) : (VariableType)info.GetValue("Reference Type", typeof(VariableType));
            _name = (string)info.GetValue("Name", typeof(string));
            _pzone_tcp = (double)info.GetValue("pzone_tcp", typeof(double));
            _pzone_ori = (double)info.GetValue("pzone_ori", typeof(double));
            _pzone_eax = (double)info.GetValue("pzone_eax", typeof(double));
            _zone_ori = (double)info.GetValue("zone_ori", typeof(double));
            _zone_leax = (double)info.GetValue("zone_leax", typeof(double));
            _zone_reax = (double)info.GetValue("zone_reax", typeof(double));
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
            info.AddValue("Version", VersionNumbering.CurrentVersionAsInt, typeof(int));
            info.AddValue("Scope", _scope, typeof(Scope));
            info.AddValue("Variable Type", _variableType, typeof(VariableType));
            info.AddValue("Name", _name, typeof(string));
            info.AddValue("pzone_tcp", _pzone_tcp, typeof(double));
            info.AddValue("pzone_ori", _pzone_ori, typeof(double));
            info.AddValue("pzone_eax", _pzone_eax, typeof(double));
            info.AddValue("zone_ori", _zone_ori, typeof(double));
            info.AddValue("zone_leax", _zone_leax, typeof(double));
            info.AddValue("zone_reax", _zone_reax, typeof(double));
            info.AddValue("Predefined", _isPredefined, typeof(bool));
            info.AddValue("Exact Predefined Value", _isExactPredefinedValue, typeof(bool));
        }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the Zone Data class.
        /// </summary>
        public ZoneData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Zone Data class with predefined values.
        /// </summary>
        /// <remarks>
        /// Use -1 to define a fine point.
        /// </remarks>
        /// <param name="zone"> The size (the radius) of the TCP zone in mm. </param>
        public ZoneData(double zone)
        {
            _scope = Scope.GLOBAL;
            _variableType = VariableType.VAR;

            // Get nearest predefined zonedata value
            double tcp = _validPredefinedValues.Aggregate((x, y) => Math.Abs(x - zone) < Math.Abs(y - zone) ? x : y);
            _isExactPredefinedValue = (zone - tcp) == 0;

            // Check if it is a fly-by-point or a fine-point
            _name = tcp == -1 ? "fine" : $"z{tcp}";
            _finep = tcp == -1;

            // Check if pre-defined name is valid
            int pos = Array.IndexOf(_validPredefinedNames, _name);
            if (pos == -1)
            {
                // Set values: this makes this object invalid (predefined zonedata variable name is not defined)
                _pzone_tcp = -1;
                _pzone_ori = -1;
                _pzone_eax = -1;
                _zone_ori = -1;
                _zone_leax = -1;
                _zone_reax = -1;
                _isPredefined = true;
            }
            else
            {
                // Get predefined zonedata values
                _pzone_tcp = _predefinedPathZoneTCP[pos];
                _pzone_ori = _predefinedPathZoneOri[pos];
                _pzone_eax = _predefinedPathZoneEax[pos];
                _zone_ori = _predefinedZoneOri[pos];
                _zone_leax = _predefinedZoneLeax[pos];
                _zone_reax = _predefinedZoneReax[pos];
                _isPredefined = true;
            }
        }

        /// <summary>
        /// Initializes a new instance of the Zone Data class with predefined values.
        /// </summary>
        /// <remarks>
        /// Use -1 to define a fine point.
        /// </remarks>
        /// <param name="zone"> The size (the radius) of the TCP zone in mm. </param>
        public ZoneData(int zone)
        {
            _scope = Scope.GLOBAL;
            _variableType = VariableType.VAR;

            // Get nearest predefined zonedata value
            double tcp = _validPredefinedValues.Aggregate((x, y) => Math.Abs(x - zone) < Math.Abs(y - zone) ? x : y);
            _isExactPredefinedValue = (zone - tcp) == 0;

            // Check if it is a fly-by-point or a fine-point
            _name = tcp == -1 ? "fine" : $"z{tcp}";
            _finep = tcp == -1;

            // Check if pre-defined name is valid
            int pos = Array.IndexOf(_validPredefinedNames, _name);
            if (pos == -1)
            {
                // Set values: this makes this object invalid (predefined zonedata variable name is not defined)
                _pzone_tcp = -1;
                _pzone_ori = -1;
                _pzone_eax = -1;
                _zone_ori = -1;
                _zone_leax = -1;
                _zone_reax = -1;
                _isPredefined = true;
            }
            else
            {
                // Get predefined zonedata values
                _pzone_tcp = _predefinedPathZoneTCP[pos];
                _pzone_ori = _predefinedPathZoneOri[pos];
                _pzone_eax = _predefinedPathZoneEax[pos];
                _zone_ori = _predefinedZoneOri[pos];
                _zone_leax = _predefinedZoneLeax[pos];
                _zone_reax = _predefinedZoneReax[pos];
                _isPredefined = true;
            }
        }

        /// <summary>
        /// Initializes a new instance of the Zone Data class with with custom values. 
        /// </summary>
        /// <param name="finep"> Defines whether the movement is to terminate as a stop point (fine point) or as a fly-by point. </param>
        /// <param name="pzone_tcp"> The size (the radius) of the TCP zone in mm. </param>
        /// <param name="pzone_ori"> The zone size (the radius) for the tool reorientation. </param>
        /// <param name="pzone_eax"> The zone size (the radius) for external axes. </param>
        /// <param name="zone_ori"> The zone size for the tool reorientation in degrees. </param>
        /// <param name="zone_leax"> The zone size for linear external axes in mm. </param>
        /// <param name="zone_reax"> he zone size for rotating external axes in degrees. </param>
        public ZoneData(bool finep, double pzone_tcp = 0, double pzone_ori = 0, double pzone_eax = 0,
            double zone_ori = 0, double zone_leax = 0, double zone_reax = 0)
        {
            _scope = Scope.GLOBAL;
            _variableType = VariableType.VAR;
            _name = "";
            _finep = finep;
            _pzone_tcp = pzone_tcp;
            _pzone_ori = pzone_ori;
            _pzone_eax = pzone_eax;
            _zone_ori = zone_ori;
            _zone_leax = zone_leax;
            _zone_reax = zone_reax;
            _isPredefined = false;
            _isExactPredefinedValue = false;
        }

        /// <summary>
        /// Initializes a new instance of the Zone Data class with custom values.
        /// </summary>
        /// <param name="name"> The Zone Data variable name, must be unique. </param>
        /// <param name="finep"> Defines whether the movement is to terminate as a stop point (fine point) or as a fly-by point. </param>
        /// <param name="pzone_tcp"> The size (the radius) of the TCP zone in mm. </param>
        /// <param name="pzone_ori"> The zone size (the radius) for the tool reorientation. </param>
        /// <param name="pzone_eax"> The zone size (the radius) for external axes. </param>
        /// <param name="zone_ori"> The zone size for the tool reorientation in degrees. </param>
        /// <param name="zone_leax"> The zone size for linear external axes in mm. </param>
        /// <param name="zone_reax"> he zone size for rotating external axes in degrees. </param>
        public ZoneData(string name, bool finep, double pzone_tcp = 0, double pzone_ori = 0, double pzone_eax = 0,
            double zone_ori = 0, double zone_leax = 0, double zone_reax = 0)
        {
            _scope = Scope.GLOBAL;
            _variableType = VariableType.VAR;
            _name = name;
            _finep = finep;
            _pzone_tcp = pzone_tcp;
            _pzone_ori = pzone_ori;
            _pzone_eax = pzone_eax;
            _zone_ori = zone_ori;
            _zone_leax = zone_leax;
            _zone_reax = zone_reax;
            _isPredefined = false;
            _isExactPredefinedValue = false;
        }

        /// <summary>
        /// Initializes a new instance of the Zone Data class by duplicating an existing Zone Data instance. 
        /// </summary>
        /// <param name="zonedata"> The Zone Data instance to duplicate. </param>
        public ZoneData(ZoneData zonedata)
        {
            _scope = zonedata.Scope;
            _variableType = zonedata.VariableType;
            _name = zonedata.Name;
            _finep = zonedata.FinePoint;
            _pzone_tcp = zonedata.PathZoneTCP;
            _pzone_ori = zonedata.PathZoneOrientation;
            _pzone_eax = zonedata.PathZoneExternalAxes;
            _zone_ori = zonedata.ZoneOrientation;
            _zone_leax = zonedata.ZoneExternalLinearAxes;
            _zone_reax = zonedata.ZoneExternalRotationalAxes;
            _isPredefined = zonedata.IsPreDefined;
            _isExactPredefinedValue = zonedata.IsExactPredefinedValue;
        }

        /// <summary>
        /// Initializes a new instance of the Zone Data clas from an enumeration.
        /// </summary>
        /// <param name="predefinedZoneData"> Predefined zonedata as an enumeration </param>
        public static ZoneData GetPredefinedZoneData(PredefinedZoneData predefinedZoneData)
        {
            return new ZoneData((double)predefinedZoneData); ;
        }

        /// <summary>
        /// Returns an exact duplicate of this Zone Data instance.
        /// </summary>
        /// <returns> 
        /// A deep copy of the Zone Data instance.
        /// </returns>
        public ZoneData Duplicate()
        {
            return new ZoneData(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Zone Data instance as an IDeclaration.
        /// </summary>
        /// <returns> 
        /// A deep copy of the Zone Data instance as an IDeclaration. 
        /// </returns>
        public IDeclaration DuplicateDeclaration()
        {
            return new ZoneData(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Zone Data instance as an Action. 
        /// </summary>
        /// <returns> 
        /// A deep copy of the Zone Data instance as an Action. 
        /// </returns>
        public override Action DuplicateAction()
        {
            return new ZoneData(this);
        }
        #endregion

        #region parse
        /// <summary>
        /// Initializes a new instance of the Zone Data class from a rapid data string.
        /// </summary>
        /// <remarks>
        /// Only used for the Parse and TryParse methods. Therefore, this constructor is private. 
        /// </remarks>
        /// <param name="rapidData"> The RAPID data string. </param>
        private ZoneData(string rapidData)
        {
            this.SetDataFromString(rapidData, out string[] values);

            _isPredefined = _validPredefinedNames.Contains(_name);

            if (values.Length == 7)
            {
                _finep = values[0] == "TRUE";
                _pzone_tcp = double.Parse(values[1]);
                _pzone_ori = double.Parse(values[2]);
                _pzone_eax = double.Parse(values[3]);
                _zone_ori = double.Parse(values[4]);
                _zone_leax = double.Parse(values[5]);
                _zone_reax = double.Parse(values[6]);
            }
            else
            {
                throw new InvalidCastException("Invalid RAPID data string: The number of values does not match.");
            }
        }

        /// <summary>
        /// Returns a Zone Data instance constructed from a RAPID data string. 
        /// </summary>
        /// <param name="rapidData"> The RAPID data string. s</param>
        public static ZoneData Parse(string rapidData)
        {
            return new ZoneData(rapidData);
        }

        /// <summary>
        /// Attempts to parse a RAPID data string into a Zone Data instance.  
        /// </summary>
        /// <param name="rapidData"> The RAPID data string. </param>
        /// <param name="zoneData"> The Zone Data intance. </param>
        /// <returns> 
        /// True on success, false on failure. 
        /// </returns>
        public static bool TryParse(string rapidData, out ZoneData zoneData)
        {
            try
            {
                zoneData = new ZoneData(rapidData);
                return true;
            }
            catch
            {
                zoneData = new ZoneData();
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
                return "Invalid Zone Data";
            }
            else if (_isPredefined == true)
            {
                return $"Predefined Zone Data ({_name})";
            }
            else if (_name != "")
            {
                return $"Custom Zone Data ({_name})";
            }
            else
            {
                return "Custom Zone Data";
            }
        }

        /// <summary>
        /// Returns the Zone Data in RAPID code format.
        /// </summary>
        /// <remarks>
        /// An example output is "[FALSE, 0, 0.3, 0.3, 0.3, 0.3, 0.03]".
        /// </remarks>
        /// <returns> 
        /// The RAPID data string. 
        /// </returns>
        public string ToRAPID()
        {
            string code = "";

            code += _finep == false ? "[FALSE, " : "[TRUE, ";
            code += $"{_pzone_tcp}, ";
            code += $"{_pzone_ori}, ";
            code += $"{_pzone_eax}, ";
            code += $"{_zone_ori}, ";
            code += $"{_zone_leax}, ";
            code += $"{_zone_reax}]";

            return code;
        }

        /// <summary>
        /// Returns the RAPID declaration code line of the this action.
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> 
        /// The RAPID code line in case a variable name is defined. 
        /// </returns>
        public override string ToRAPIDDeclaration(Robot robot)
        {
            if (_isPredefined == false & _name != "")
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
        public override string ToRAPIDInstruction(Robot robot)
        {
            return string.Empty;
        }

        /// <summary>
        /// Creates declarations in the RAPID program module inside the RAPID Generator. 
        /// </summary>
        /// <remarks>
        /// This method is called inside the RAPID generator.
        /// </remarks>
        /// <param name="RAPIDGenerator"> The RAPID Generator. </param>
        public override void ToRAPIDDeclaration(RAPIDGenerator RAPIDGenerator)
        {
            if (_isPredefined == false)
            {
                if (_name != "")
                {
                    if (!RAPIDGenerator.ZoneDatas.ContainsKey(_name))
                    {
                        RAPIDGenerator.ZoneDatas.Add(_name, this);
                        RAPIDGenerator.ProgramDeclarations.Add("    " + ToRAPIDDeclaration(RAPIDGenerator.Robot));
                    }
                }
            }
        }

        /// <summary>
        /// Creates instructions in the RAPID program module inside the RAPID Generator.
        /// </summary>
        /// <remarks>
        /// This method is called inside the RAPID generator.
        /// </remarks>
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
                if (_pzone_tcp < 0) { return false; }
                if (_pzone_ori < 0) { return false; }
                if (_pzone_eax < 0) { return false; }
                if (_zone_ori < 0) { return false; }
                if (_zone_leax < 0) { return false; }
                if (_zone_reax < 0) { return false; }
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
        /// Gets or sets the ZoneData variable name. 
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the movement is to terminate as a stop point (fine point) or as a fly-by point.
        /// </summary>
        public bool FinePoint
        {
            get { return _finep; }
            set { _finep = value; }
        }

        /// <summary>
        /// Gets or sets the size (the radius) of the TCP zone in mm.
        /// </summary>
        public double PathZoneTCP
        {
            get { return _pzone_tcp; }
            set { _pzone_tcp = value; }
        }

        /// <summary>
        /// Gets or sets the zone size (the radius) for the tool reorientation. 
        /// </summary>
        /// <remarks>
        /// The size is defined as the distance of the TCP from the programmed point in mm.
        /// </remarks>
        public double PathZoneOrientation
        {
            get { return _pzone_ori; }
            set { _pzone_ori = value; }
        }

        /// <summary>
        /// Gets or sets the zone size (the radius) for external axes. 
        /// </summary>
        /// <remarks>
        /// The size is defined as the distance of the TCP from the programmed point in mm.
        /// </remarks>
        public double PathZoneExternalAxes
        {
            get { return _pzone_eax; }
            set { _pzone_eax = value; }
        }

        /// <summary>
        /// Gets or sets the zone size for the tool reorientation in degrees. 
        /// </summary>
        /// <remarks>
        /// If the robot is holding the work object, this means an angle of rotation for the work object.
        /// </remarks>
        public double ZoneOrientation
        {
            get { return _zone_ori; }
            set { _zone_ori = value; }
        }

        /// <summary>
        /// Gets or sets the zone size for linear external axes in mm. 
        /// </summary>
        public double ZoneExternalLinearAxes
        {
            get { return _zone_leax; }
            set { _zone_leax = value; }
        }

        /// <summary>
        /// Gets or sets the zone size for rotating external axes in degrees.
        /// </summary>
        public double ZoneExternalRotationalAxes
        {
            get { return _zone_reax; }
            set { _zone_reax = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this zonedata is a predefined zonedata. 
        /// </summary>
        public bool IsPreDefined
        {
            get { return _isPredefined; }
            set { _isPredefined = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this zonedata was constructed from an exact predefined zonedata value. 
        /// </summary>
        /// <remarks>
        /// If false the nearest predefined zoneata or a custom zonedata was used.
        /// </remarks>
        public bool IsExactPredefinedValue
        {
            get { return _isExactPredefinedValue; }
        }

        /// <summary>
        /// Gets the valid predefined zonedata variable names.
        /// </summary>
        public static string[] ValidPredefinedNames
        {
            get { return _validPredefinedNames; }
        }

        /// <summary>
        /// Gets the valid predefined zonedata values.
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

        #region obsolete
        /// <summary>
        /// Gets or sets a value indicating whether this zonedata is a predefined zonedata. 
        /// </summary>
        [Obsolete("This property is obsolete and will be removed in v3. Use IsPredefined instead.", false)]
        public bool PreDefined
        {
            get { return _isPredefined; }
            set { _isPredefined = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this zonedata is a user definied zonedata. 
        /// </summary>
        [Obsolete("This property is obsolete and will be removed in v3. Use IsPredefined instead.", false)]
        public bool UserDefinied
        {
            get { return !_isPredefined; }
            set { _isPredefined = !value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this zonedata was constructed from an exact predefined zonedata value. 
        /// </summary>
        /// <remarks>
        /// If false the nearest predefined zoneata or a custom zonedata was used.
        /// </remarks>
        [Obsolete("This property is obsolete and will be removed in v3. Use IsExactPredefinedValue instead.", false)]
        public bool ExactPredefinedValue
        {
            get { return _isExactPredefinedValue; }
        }
        #endregion
    }
}
// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Linq;
// RobotComponents Libs
using RobotComponents.Definitions;

namespace RobotComponents.Actions
{
    /// <summary>
    /// ZoneData class. Zoned Data is used to specify how a position is to be terminated, 
    /// i.e. how close to the programmed position the axes must be before moving towards 
    /// the next position.
    /// </summary>
    public class ZoneData : Action
    {
        #region fields
        private string _name; // ZoneData variable name
        private bool _finep; // Fine point
        private double _pzone_tcp; // Path zone TCP
        private double _pzone_ori; // Path zone orientation
        private double _pzone_eax; // Path zone external axes
        private double _zone_ori; // Zone orientation
        private double _zone_leax; // Zone linear external axes
        private double _zone_reax; // Zone rotational external axes
        private bool _predefined; // ABB predefinied data (e.g. fine, z1, z5, z10 etc.)?
        private readonly bool _exactPredefinedValue; // field that indicates if the exact predefined value was selected

        private static readonly string[] _validPredefinedNames = new string[] { "fine", "z0", "z1", "z5", "z10", "z15", "z20", "z30", "z40", "z50", "z60", "z80", "z100", "z150", "z200" };
        private static readonly double[] _validPredefinedValues = new double[] { -1, 0, 1, 5, 10, 15, 20, 30, 40, 50, 60, 80, 100, 150, 200 };
        private static readonly double[] _predefinedPathZoneTCP = new double[] { 0, 0.3, 1, 5, 10, 15, 20, 30, 40, 50, 60, 80, 100, 150, 200 };
        private static readonly double[] _predefinedPathZoneOri = new double[] { 0, 0.3, 1, 8, 15, 23, 30, 45, 60, 75, 90, 120, 150, 225, 300 };
        private static readonly double[] _predefinedPathZoneEax = new double[] { 0, 0.3, 1, 8, 15, 23, 30, 45, 60, 75, 90, 120, 150, 225, 300 };
        private static readonly double[] _predefinedZoneOri = new double[] { 0, 0.03, 0.1, 0.8, 1.5, 2.3, 3, 4.5, 6, 7.5, 9, 12, 15, 23, 30 };
        private static readonly double[] _predefinedZoneLeax = new double[] { 0, 0.3, 1, 8, 15, 23, 30, 45, 60, 75, 90, 120, 150, 225, 300 };
        private static readonly double[] _predefinedZoneReax = new double[] { 0, 0.03, 0.1, 0.8, 1.5, 2.3, 3, 4.5, 6, 7.5, 9, 12, 15, 23, 30 };
        #endregion

        #region constructors
        /// <summary>
        /// Defines an empty ZoneData object. 
        /// </summary>
        public ZoneData()
        {
        }

        /// <summary>
        /// Constructor for creating a predefined ZoneData. 
        /// ABB defined already a number of zone data in the system module.
        /// Use -1 to define a fine point. 
        /// </summary>
        /// <param name="zone"> The size (the radius) of the TCP zone in mm. </param>
        public ZoneData(double zone)
        {
            // Get nearest predefined zonedata value
            double tcp = _validPredefinedValues.Aggregate((x, y) => Math.Abs(x - zone) < Math.Abs(y - zone) ? x : y);

            // Check if the exact predefined value is used or the nearest one
            if (zone - tcp == 0) 
            { 
                _exactPredefinedValue = true; 
            }
            else 
            {
                _exactPredefinedValue = false; 
            }

            // Check if it is a fly-by-point or a fine-point
            if (tcp == -1)
            {
                _name = "fine";
                _finep = true;
            }
            else 
            {
                _name = "z" + tcp.ToString();
                _finep = false;
            }

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
                _predefined = true;
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
                _predefined = true;
            }
        }

        /// <summary>
        /// Constructor for creating a predefined ZoneData. 
        /// ABB defined already a number of zone data in the system module.
        /// Use -1 to define a fine point. 
        /// </summary>
        /// <param name="zone"> The size (the radius) of the TCP zone in mm. </param>
        public ZoneData(int zone)
        {
            // Get nearest predefined zonedata value
            double tcp = _validPredefinedValues.Aggregate((x, y) => Math.Abs(x - zone) < Math.Abs(y - zone) ? x : y);

            // Check if the exact predefined value is used or the nearest one
            if (zone - tcp == 0)
            {
                _exactPredefinedValue = true;
            }
            else
            {
                _exactPredefinedValue = false;
            }

            // Check if it is a fly-by-point or a fine-point
            if (tcp == -1)
            {
                _name = "fine";
                _finep = true;
            }
            else
            {
                _name = "z" + tcp.ToString();
                _finep = false;
            }

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
                _predefined = true;
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
                _predefined = true;
            }
        }

        /// <summary>
        /// Defines a custom ZoneData.
        /// </summary>
        /// <param name="name"> The ZoneData variable name. </param>
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
            _name = name;
            _finep = finep;
            _pzone_tcp = pzone_tcp;
            _pzone_ori = pzone_ori;
            _pzone_eax = pzone_eax;
            _zone_ori = zone_ori;
            _zone_leax = zone_leax;
            _zone_reax = zone_reax;
            _predefined = false;
            _exactPredefinedValue = false;
        }

        /// <summary>
        /// Creates a new zonedata by duplicating an existing zonedata. 
        /// This creates a deep copy of the existing zonedata. 
        /// </summary>
        /// <param name="zonedata"> The speeddata that should be duplicated. </param>
        public ZoneData(ZoneData zonedata)
        {
            _name = zonedata.Name;
            _finep = zonedata.FinePoint;
            _pzone_tcp = zonedata.PathZoneTCP;
            _pzone_ori = zonedata.PathZoneOrientation;
            _pzone_eax = zonedata.PathZoneExternalAxes;
            _zone_ori = zonedata.ZoneOrientation;
            _zone_leax = zonedata.ZoneExternalLinearAxes;
            _zone_reax = zonedata.ZoneExternalRotationalAxes;
            _predefined = zonedata.PreDefinied;
            _exactPredefinedValue = zonedata.ExactPredefinedValue;
        }

        /// <summary>
        /// Duplicates a ZoneData object
        /// </summary>
        /// <returns> Returns a deep copy of the ZoneData object. </returns>
        public ZoneData Duplicate()
        {
            return new ZoneData(this);
        }

        /// <summary>
        /// A method to duplicate the ZoneData object to an Action object. 
        /// </summary>
        /// <returns> Returns a deep copy of the ZoneData object as an Action object. </returns>
        public override Action DuplicateAction()
        {
            return new ZoneData(this) as Action;
        }
        #endregion

        #region method
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns> A string that represents the current object. </returns>
        public override string ToString()
        {
            if (!this.IsValid)
            {
                return "Invalid Zone Data";
            }
            else if (this.PreDefinied == true)
            {
                return "Predefined Zone Data (" + _name + ")";
            }
            else
            {
                return "Custom Zone Data (" + _name + ")";
            }
        }

        /// <summary>
        /// Used to create variable definition code of this action. 
        /// </summary>
        /// <param name="robot"> Defines the Robot were the code is generated for. </param>
        /// <returns> Returns the RAPID code line as a string. </returns>
        public override string ToRAPIDDeclaration(Robot robot)
        {
            if (_predefined == false)
            {
                string code = "VAR zonedata ";
                code += _name + " := [";
                
                if (_finep == false) { code +=  "FALSE, "; }
                else  { code += "TRUE, "; }

                code += _pzone_tcp + ", ";
                code += _pzone_ori + ", ";
                code += _pzone_eax + ", ";
                code += _zone_ori + ", ";
                code += _zone_leax + ", ";
                code += _zone_reax + "];";

                return code;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Used to create action instruction code line. 
        /// </summary>
        /// <param name="robot"> Defines the Robot were the code is generated for. </param>
        /// <returns> Returns the RAPID code line as a string. </returns>
        public override string ToRAPIDInstruction(Robot robot)
        {
            return string.Empty;
        }

        /// <summary>
        /// Used to create variable definitions in the RAPID Code. It is typically called inside the CreateRAPIDCode() method of the RAPIDGenerator class.
        /// </summary>
        /// <param name="RAPIDGenerator"> Defines the RAPIDGenerator. </param>
        public override void ToRAPIDDeclaration(RAPIDGenerator RAPIDGenerator)
        {
            if (_predefined == false)
            {
                // Only adds speedData Variable if not already in RAPID Code
                if (!RAPIDGenerator.ZoneDatas.ContainsKey(this.Name))
                {
                    RAPIDGenerator.ZoneDatas.Add(this.Name, this);
                    RAPIDGenerator.StringBuilder.Append(Environment.NewLine + "\t" + this.ToRAPIDDeclaration(RAPIDGenerator.Robot));
                }
            }
        }

        /// <summary>
        /// Used to create action instructions in the RAPID Code. It is typically called inside the CreateRAPIDCode() method of the RAPIDGenerator class.
        /// </summary>
        /// <param name="RAPIDGenerator"> Defines the RAPIDGenerator. </param>
        public override void ToRAPIDInstruction(RAPIDGenerator RAPIDGenerator)
        {
        }
        #endregion

        #region properties
        /// <summary>
        /// A boolean that indicates if the SpeedData is valid. 
        /// </summary>
        public override bool IsValid
        {
            get
            {
                if (Name == "") { return false; }
                if (Name == null) { return false; }
                if (PathZoneTCP < 0) { return false; }
                if (PathZoneOrientation < 0) { return false; }
                if (PathZoneExternalAxes < 0) { return false; }
                if (ZoneOrientation < 0) { return false; }
                if (ZoneExternalLinearAxes < 0) { return false; }
                if (ZoneExternalRotationalAxes < 0) { return false; }
                return true;
            }
        }

        /// <summary>
        /// The ZoneData variable name. 
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Defines whether the movement is to terminate as a stop point (fine point) or as a fly-by point.
        /// </summary>
        public bool FinePoint
        {
            get { return _finep; }
            set { _finep = value; }
        }

        /// <summary>
        /// The size (the radius) of the TCP zone in mm.
        /// </summary>
        public double PathZoneTCP
        {
            get { return _pzone_tcp; }
            set { _pzone_tcp = value; }
        }

        /// <summary>
        /// The zone size (the radius) for the tool reorientation. 
        /// The size is defined as the distance of the TCP from the programmed point in mm.
        /// </summary>
        public double PathZoneOrientation
        {
            get { return _pzone_ori; }
            set { _pzone_ori = value; }
        }

        /// <summary>
        /// The zone size (the radius) for external axes. 
        /// The size is defined as the distance of the TCP from the programmed point in mm.
        /// </summary>
        public double PathZoneExternalAxes
        {
            get { return _pzone_eax; }
            set { _pzone_eax = value; }
        }

        /// <summary>
        /// The zone size for the tool reorientation in degrees. 
        /// If the robot is holding the work object, this means an angle of rotation for the work object.
        /// </summary>
        public double ZoneOrientation
        {
            get { return _zone_ori; }
            set { _zone_ori = value; }
        }

        /// <summary>
        /// The zone size for linear external axes in mm. 
        /// </summary>
        public double ZoneExternalLinearAxes
        {
            get { return _zone_leax; }
            set { _zone_leax = value; }
        }

        /// <summary>
        /// The zone size for rotating external axes in degrees.
        /// </summary>
        public double ZoneExternalRotationalAxes
        {
            get { return _zone_reax; }
            set { _zone_reax = value; }
        }

        /// <summary>
        /// A boolean that indicates if the zonedata is predefined by ABB. 
        /// </summary>
        public bool PreDefinied
        {
            get { return _predefined; }
            set { _predefined = value; }
        }

        /// <summary>
        /// Indicates if the exact predefined zonedata value is used.
        /// If false the nearest predefined zonedata or a custom zonedata is used.
        /// </summary>
        public bool ExactPredefinedValue
        {
            get { return _exactPredefinedValue; }
        }

        /// <summary>
        /// Defines an array with valid predefined zonedata variable names
        /// </summary>
        public static string[] ValidPredefinedNames
        {
            get { return _validPredefinedNames; }
        }

        /// <summary>
        /// Defines an array with valid predefined zonedata values
        /// </summary>
        public static double[] ValidPredefinedValues
        {
            get { return _validPredefinedValues; }
        }
        #endregion
    }
}
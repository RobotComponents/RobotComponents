// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
// RobotComponents Libs
using RobotComponents.Definitions;
using RobotComponents.Enumerations;
using RobotComponents.Utils;

namespace RobotComponents.Actions
{
    /// <summary>
    /// Represents a predefined or user definied Speed Data declaration.
    /// This action is used to specify the velocity at which both the robot and the external axes move.
    /// </summary>
    [Serializable()]
    public class SpeedData : Action, IDeclaration, ISerializable
    {
        #region fields
        private ReferenceType _referenceType; // reference type
        private string _name; // SpeeData variable name
        private double _v_tcp; // Tool center point speed
        private double _v_ori; // Re-orientation speed
        private double _v_leax; // External linear axis speed
        private double _v_reax; // External rotational axis speed
        private bool _predefined; // ABB predefinied data (e.g. v5, v10, v20, v30)?
        private readonly bool _exactPredefinedValue; // field that indicates if the exact predefined value was selected

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
            // int version = (int)info.GetValue("Version", typeof(int)); // <-- use this if the (de)serialization changes
            _referenceType = (ReferenceType)info.GetValue("Reference Type", typeof(ReferenceType));
            _name = (string)info.GetValue("Name", typeof(string));
            _v_tcp = (double)info.GetValue("v_tcp", typeof(double));
            _v_ori = (double)info.GetValue("v_ori", typeof(double));
            _v_leax = (double)info.GetValue("v_leax", typeof(double));
            _v_reax = (double)info.GetValue("v_reax", typeof(double));
            _predefined = (bool)info.GetValue("Predefined", typeof(bool));
            _exactPredefinedValue = (bool)info.GetValue("Exact Predefined Value", typeof(bool));
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
            info.AddValue("v_tcp", _v_tcp, typeof(double));
            info.AddValue("v_ori", _v_ori, typeof(double));
            info.AddValue("v_leax", _v_leax, typeof(double));
            info.AddValue("v_reax", _v_reax, typeof(double));
            info.AddValue("Predefined", _predefined, typeof(bool));
            info.AddValue("Exact Predefined Value", _exactPredefinedValue, typeof(bool));
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

            // Check if the exact predefined value is used or the nearest one
            if (v_tcp - tcp == 0)
            {
                _exactPredefinedValue = true;
            }
            else
            {
                _exactPredefinedValue = false;
            }


            // Set other fields
            _referenceType = ReferenceType.VAR;
            _name = "v" + tcp.ToString();
            _v_tcp = tcp;
            _v_ori = 500;
            _v_leax = 5000;
            _v_reax = 1000;
            _predefined = true;
        }

        /// <summary>
        /// Initializes a new instance of the Speed Data class with predefined values.
        /// </summary>
        /// <param name="v_tcp"> The velocity of the tool center point (TCP) in mm/s. </param>
        public SpeedData(int v_tcp)
        {
            // Get nearest predefined speeddata value
            double tcp = _validPredefinedValues.Aggregate((x, y) => Math.Abs(x - v_tcp) < Math.Abs(y - v_tcp) ? x : y);

            // Check if the exact predefined value is used or the nearest one
            if (v_tcp - tcp == 0)
            {
                _exactPredefinedValue = true;
            }
            else
            {
                _exactPredefinedValue = false;
            }

            // Set other fields
            _referenceType = ReferenceType.VAR;
            _name = "v" + tcp.ToString();
            _v_tcp = tcp;
            _v_ori = 500;
            _v_leax = 5000;
            _v_reax = 1000;
            _predefined = true;
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
            _referenceType = ReferenceType.VAR;
            _name = name;
            _v_tcp = v_tcp;
            _v_ori = v_ori;
            _v_leax = v_leax;
            _v_reax = v_reax;
            _predefined = false;
            _exactPredefinedValue = false;
        }

        /// <summary>
        /// Initializes a new instance of the Speed Data class by duplicating an existing Speed Data instance. 
        /// </summary>
        /// <param name="speeddata"> The Speed Data instance to duplicate. </param>
        public SpeedData(SpeedData speeddata)
        {
            _referenceType = speeddata.ReferenceType;
            _name = speeddata.Name;
            _v_tcp = speeddata.V_TCP;
            _v_ori = speeddata.V_ORI;
            _v_leax = speeddata.V_LEAX;
            _v_reax = speeddata.V_REAX;
            _predefined = speeddata.PreDefinied;
            _exactPredefinedValue = speeddata.ExactPredefinedValue;
        }

        /// <summary>
        /// Returns an exact duplicate of this Speed Data instance.
        /// </summary>
        /// <returns> A deep copy of the Speed Data instance. </returns>
        public SpeedData Duplicate()
        {
            return new SpeedData(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Speed Data instance as an IDeclaration.
        /// </summary>
        /// <returns> A deep copy of the SpeedData instance as an IDeclaration. </returns>
        public IDeclaration DuplicateDeclaration()
        {
            return new SpeedData(this) as IDeclaration;
        }

        /// <summary>
        /// Returns an exact duplicate of this Spee Data instance as an Action. 
        /// </summary>
        /// <returns> A deep copy of the Speed Data instance as an Action. </returns>
        public override Action DuplicateAction()
        {
            return new SpeedData(this) as Action;
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
                return "Invalid Speed Data";
            }
            else if (this.PreDefinied == true)
            {
                return "Predefined Speed Data ("+ _name + ")";
            }
            else
            {
                return "Custom Speed Data (" + _name + ")";
            }
        }

        /// <summary>
        /// Returns the RAPID declaration code line of the this action.
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> The RAPID code line. </returns>
        public override string ToRAPIDDeclaration(Robot robot)
        {
            if (_predefined == false)
            {
                return Enum.GetName(typeof(ReferenceType), _referenceType) + " speeddata " + _name + " := [" + _v_tcp + ", " + _v_ori + ", " + _v_leax + ", " + _v_reax + "];";
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
        /// <returns> An empty string. </returns>
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
            if (_predefined == false)
            {
                // Only adds speedData Variable if not already in RAPID Code
                if (!RAPIDGenerator.SpeedDatas.ContainsKey(this.Name))
                {
                    RAPIDGenerator.SpeedDatas.Add(this.Name, this);
                    RAPIDGenerator.StringBuilder.Append(Environment.NewLine + "\t" + this.ToRAPIDDeclaration(RAPIDGenerator.Robot));
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
                if (Name == "") { return false; }
                if (Name == null) { return false; }
                if (V_TCP <= 0) { return false; }
                if (V_ORI <= 0) { return false; }
                if (V_LEAX <= 0) { return false; }
                if (V_REAX <= 0) { return false; }
                return true;
            }
        }

        /// <summary>
        /// Gets or sets the reference type.
        /// </summary>
        public ReferenceType ReferenceType
        {
            get { return _referenceType; }
            set { _referenceType = value; }
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
        /// If a stationary tool or coordinated external axes are used, the velocity is specified relative to the work object.
        /// </summary>
        public double V_TCP
        {
            get { return _v_tcp; }
            set { _v_tcp = value; }
        }

        /// <summary>
        /// Gets or sets the reorientation velocity of the TCP expressed in degrees/s. 
        /// If a stationary tool or coordinated external axes are used, the velocity is specified relative to the work object.
        /// </summary>
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
        public bool PreDefinied
        {
            get { return _predefined; }
            set { _predefined = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this speeddata is a user definied speeddata. 
        /// </summary>
        public bool UserDefinied
        {
            get { return !_predefined; }
            set { _predefined = !value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this speeddata was constructed from an exact predefined speeddata value. 
        /// If false the nearest predefined speedata or a custom speeddata was used. 
        /// </summary>
        public bool ExactPredefinedValue
        {
            get { return _exactPredefinedValue; }
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
        #endregion

    }
}

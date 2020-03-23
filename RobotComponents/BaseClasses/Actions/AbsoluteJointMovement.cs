// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/EDEK-UniKassel/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
// RobotComponents Libs
using RobotComponents.BaseClasses.Definitions;

namespace RobotComponents.BaseClasses.Actions
{
    /// <summary>
    /// Absolute Joint Movement class
    /// </summary>
    public class AbsoluteJointMovement : Action
    {
        #region fields
        // Fixed fields
        private string _name; // target variable name
        private List<double> _internalAxisValues;
        private List<double> _externalAxisValues;
        private SpeedData _speedData;
        private readonly int _movementType;
        private int _precision;

        // Variable fields
        private RobotTool _robotTool;
        #endregion

        #region constructors
        /// <summary>
        /// An empty absolute joint movement constructor.
        /// </summary>
        public AbsoluteJointMovement()
        {
        }

        /// <summary>
        /// Method to create an absolute joint movement with a minimum number of arguments. 
        /// </summary>
        /// <param name="name">Name of joint target, must be unique.</param>
        /// <param name="internalAxisValues">List of internal axis values. The length of the list should be equal to 6.</param>
        public AbsoluteJointMovement(string name, List<double> internalAxisValues)
        {
            _name = name;
            _internalAxisValues = internalAxisValues;
            _externalAxisValues = new List<double>() { };
            _speedData = new SpeedData(5); // Slowest predefined tcp speed
            _movementType = 0; // The movementType is always an Absolute Joint Movement
            _precision = 0;
            _robotTool = new RobotTool(); // Default Robot Tool tool0
            _robotTool.Clear(); // Empty Robot Tool
        }

        /// <summary>
        /// Method to create an absolute joint movement with internal and external axis values.
        /// </summary>
        /// <param name="name">Name of joint target, must be unique.</param>
        /// <param name="internalAxisValues">List of internal axis values. The length of the list should be equal to 6.</param>
        /// <param name="externalAxisValues">List of external axis values. </param>
        public AbsoluteJointMovement(string name, List<double> internalAxisValues, List<double> externalAxisValues)
        {
            _name = name;
            _internalAxisValues = internalAxisValues;
            _externalAxisValues = externalAxisValues;
            _speedData = new SpeedData(5); // Slowest predefined tcp speed
            _movementType = 0; // The movementType is always an Absolute Joint Movement
            _precision = 0;
            _robotTool = new RobotTool(); // Default Robot Tool tool0
            _robotTool.Clear(); // Empty Robot Tool
        }

        /// <summary>
        /// Method to create a absolute joint movement with an empty robot tool (no override).
        /// </summary>
        /// <param name="name">Name of joint target, must be unique.</param>
        /// <param name="internalAxisValues">List of internal axis values. The length of the list should be equal to 6.</param>
        /// <param name="externalAxisValues">List of external axis values. </param>
        /// <param name="speedData"> The SpeedData as a SpeedData </param>
        /// <param name="precision"> Robot movement precision. If this value is -1 the robot will go to exactly the specified position. This means its ZoneData in RAPID code is set to fine. </param>
        public AbsoluteJointMovement(string name, List<double> internalAxisValues, List<double> externalAxisValues, SpeedData speedData, int precision)
        {
            _name = name;
            _internalAxisValues = internalAxisValues;
            _externalAxisValues = externalAxisValues;
            _speedData = speedData;
            _movementType = 0; // The movement type is always an Absolute Joint Movement
            _precision = precision;
            _robotTool = new RobotTool(); // Default Robot Tool tool0
            _robotTool.Clear(); // Empty Robot Tool
        }

        /// <summary>
        /// Method to create an absolute joint movement. 
        /// </summary>
        /// <param name="name">Name of joint target, must be unique.</param>
        /// <param name="internalAxisValues">List of internal axis values. The length of the list should be equal to 6.</param>
        /// <param name="externalAxisValues">List of external axis values. The length of the list should be (for now) equal to 1.</param>
        /// <param name="speedData"> The SpeedData as a SpeedData </param>
        /// <param name="precision"> Robot movement precision. If this value is -1 the robot will go to exactly the specified position. This means its ZoneData in RAPID code is set to fine. </param>
        /// <param name="robotTool"> The Robot Tool. This will override the set default tool. </param>
        public AbsoluteJointMovement(string name, List<double> internalAxisValues, List<double> externalAxisValues, SpeedData speedData, int precision, RobotTool robotTool)
        {
            _name = name;
            _internalAxisValues = internalAxisValues;
            _externalAxisValues = externalAxisValues;
            _speedData = speedData;
            _movementType = 0; // The movement type is always an Absolute Joint Movement
            _precision = precision;
            _robotTool = robotTool;
        }

        /// <summary>
        /// Creates a new absolute joint movement by duplicating an existing absolute joint movement. 
        /// This creates a deep copy of the existing absolute joint movement. 
        /// </summary>
        /// <param name="jointMovement"> The absolute joint movement that should be duplicated. </param>
        /// <param name="duplicateMesh"> A boolean that indicates if the meshes should be duplicated. </param>
        public AbsoluteJointMovement(AbsoluteJointMovement jointMovement, bool duplicateMesh = true)
        {
            _name = jointMovement.Name;
            _internalAxisValues = new List<double>(jointMovement.InternalAxisValues);
            _externalAxisValues = new List<double>(jointMovement.ExternalAxisValues);
            _speedData = jointMovement.SpeedData.Duplicate();
            _movementType = jointMovement.MovementType;
            _precision = jointMovement.Precision;

            if (duplicateMesh == true) { _robotTool = jointMovement.RobotTool.Duplicate(); }
            else { _robotTool = jointMovement.RobotTool.DuplicateWithoutMesh(); }
        }

        /// <summary>
        /// Duplicates a robot movement.
        /// </summary>
        /// <returns></returns>
        public AbsoluteJointMovement Duplicate()
        {
            return new AbsoluteJointMovement(this);
        }

        /// <summary>
        /// Duplicates a robot movement without attached robot tool meshes.
        /// </summary>
        /// <returns> Returns a deep copy of the AbsoluteJointMovement object without attached RobotTool meshes. </returns>/returns>
        public AbsoluteJointMovement DuplicateWithoutMesh()
        {
            return new AbsoluteJointMovement(this, false);
        }

        /// <summary>
        /// A method to duplicate the AbsoluteJointMovement object to an Action object. 
        /// </summary>
        /// <returns> Returns a deep copy of the AbsoluteJointMovement object as an Action object. </returns>
        public override Action DuplicateAction()
        {
            return new AbsoluteJointMovement(this) as Action;
        }
        #endregion

        #region method
        /// <summary>
        /// Used to create variable definition code of this action. 
        /// </summary>
        /// <param name="robotInfo"> Defines the Robot Info were the code is generated for. </param>
        /// <returns> Returns the RAPID code line as a string. </returns>
        public override string InitRAPIDVar(RobotInfo robotInfo)
        {
            // Creates Code Variable
            string code = "CONST jointtarget " + JointTargetName + " := [[";

            // Adds all Internal Axis Values
            for (int i = 0; i < this._internalAxisValues.Count; i++)
            {
                code += this._internalAxisValues[i].ToString("0.##") + ", ";
            }
            code = code.Remove(code.Length - 2);

            // Adds all External Axis Values
            code += "], [";
            for (int i = 0; i < this._externalAxisValues.Count; i++)
            {
                code += this._externalAxisValues[i].ToString("0.##") + ", ";
            }

            // Adds 9E9 for all missing external Axis Values
            for (int i = this._externalAxisValues.Count; i < 6; i++)
            {
                code += "9E9" + ", ";
            }
            code = code.Remove(code.Length - 2);
            code += "]];";

            return code;
        }

        /// <summary>
        /// Used to create action instruction code line. 
        /// </summary>
        /// <param name="robotInfo"> Defines the Robot Info were the code is generated for. </param>
        /// <returns> Returns the RAPID code line as a string. </returns>
        public override string ToRAPIDFunction(RobotInfo robotInfo)
        {
            // Set tool name
            string toolName;

            // Check first if a tool is set
            if (_robotTool == null) 
            { 
                toolName = robotInfo.Tool.Name; 
            }
            // Check if a tool is set by checking the name (tool can be empty)
            else if (_robotTool.Name == "" || _robotTool.Name == null)
            { 
                toolName = robotInfo.Tool.Name; 
            } 
            // Otherwise don't set a tool. Last overwrite is used that is combined with the movement.
            else 
            { 
                toolName = _robotTool.Name; 
            }

            // Set zone data text (precision value)
            string zoneName;
            if (_precision < 0)
            {
                zoneName = ", fine, ";
            }
            else
            {
                zoneName = ", z" + _precision.ToString() + ", ";
            }

            // MoveAbsJ
            string code = "MoveAbsJ " + JointTargetName + ", " + _speedData.Name + zoneName + toolName + ";";

            return code;
        }

        /// <summary>
        /// Used to create variable definitions in the RAPID Code. It is typically called inside the CreateRAPIDCode() method of the RAPIDGenerator class.
        /// </summary>
        /// <param name="RAPIDGenerator"> Defines the RAPIDGenerator. </param>
        public override void InitRAPIDVar(RAPIDGenerator RAPIDGenerator)
        {
            // Creates SpeedData Variable Code
            _speedData.InitRAPIDVar(RAPIDGenerator);

            // Only adds target code if target is not already defined
            if (!RAPIDGenerator.Targets.ContainsKey(JointTargetName))
            {
                // Adds Target to RAPIDGenerator SpeedDatasDictionary
                RAPIDGenerator.Targets.Add(JointTargetName, new Target());

                // Add variable code line to the RAPID generator string builder
                RAPIDGenerator.StringBuilder.Append(Environment.NewLine + "\t" + this.InitRAPIDVar(RAPIDGenerator.RobotInfo));
            }
        }

        /// <summary>
        /// Used to create action instructions in the RAPID Code. It is typically called inside the CreateRAPIDCode() method of the RAPIDGenerator class.
        /// </summary>
        /// <param name="RAPIDGenerator"> Defines the RAPIDGenerator. </param>
        public override void ToRAPIDFunction(RAPIDGenerator RAPIDGenerator)
        {
            RAPIDGenerator.StringBuilder.Append(Environment.NewLine + "\t\t" + this.ToRAPIDFunction(RAPIDGenerator.RobotInfo));
        }
        #endregion

        #region properties
        /// <summary>
        /// A boolean that indicates if the Absolute Joint Movement object is valid.
        /// </summary>
        public override bool IsValid
        {
            get
            {
                if (Name == null) { return false; }
                if (Name == "") { return false; }
                if (InternalAxisValues == null) { return false; }
                if (ExternalAxisValues == null) { return false; }
                if (SpeedData == null) { return false; }
                return true;
            }
        }

        /// <summary>
        /// The target variable name, must be unique.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// The target name when it is used as a joint target.
        /// </summary>
        public string JointTargetName
        {
            get { return Name + "_jm"; }
        }

        /// <summary>
        /// Defines the pose of the internal axis values in degree. 
        /// </summary>
        public List<double> InternalAxisValues
        {
            get { return _internalAxisValues; }
            set { _internalAxisValues = value; }
        }

        /// <summary>
        /// Defines the pose of the external axis values in degrees or in meters.
        /// </summary>
        public List<double> ExternalAxisValues
        {
            get { return _externalAxisValues; }
            set { _externalAxisValues = value; }
        }

        /// <summary>
        /// The speed data that applies to movements. Speed data defines the velocity 
        /// for the tool center point, the tool reorientation, and external axes.
        /// </summary>
        public SpeedData SpeedData
        {
            get { return _speedData; }
            set { _speedData = value; }
        }

        /// <summary>
        /// The movement type.
        /// One is used for absolute joint movements with jointtargets (MoveAbsJ).
        /// For the Absolute Joint Movement Class the MovementType can not be set.
        /// </summary>
        public int MovementType
        {
            get { return _movementType; }
        }

        /// <summary>
        /// Precision for the movement that describes the ABB zondedata. 
        /// It defines the size of the generated corner path.
        /// </summary>
        public int Precision
        {
            get { return _precision; }
            set { _precision = value; }
        }

        /// <summary>
        /// The tool in use when the robot moves. 
        /// </summary>
        public RobotTool RobotTool
        {
            get { return _robotTool; }
            set { _robotTool = value; }
        }
        #endregion
    }
}

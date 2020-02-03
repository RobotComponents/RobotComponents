using System.Collections.Generic;

using Rhino.Geometry;

using RobotComponents.BaseClasses.Definitions;
using RobotComponents.BaseClasses.Kinematics;

namespace RobotComponents.BaseClasses.Actions
{
    /// <summary>
    /// Joint Movement class
    /// </summary>
    public class AbsoluteJointMovement : Action
    {
        #region fields
        // Fixed fields
        private string _name; // target variable name
        private List<double> _internalAxisValues;
        private List<double> _externalAxisValues;
        private SpeedData _speedData;
        private int _movementType;
        private int _precision;

        // Variable fields
        RobotTool _robotTool;
        #endregion

        #region constructors
        /// <summary>
        /// An empty movement constructor.
        /// </summary>
        public AbsoluteJointMovement()
        {
        }

        /// <summary>
        /// Method to create a absolute joint movement with as only minimum arguments. 
        /// </summary>
        /// <param name="name">Name of joint target, must be unique.</param>
        /// <param name="internalAxisValues">List of internal axis values. The length of the list should be equal to 6.</param>
        /// <param name="externalAxisValues">List of external axis values. The length of the list should be (for now) equal to 1.</param>
        public AbsoluteJointMovement(string name, List<double> internalAxisValues, List<double> externalAxisValues)
        {
            _name = name;
            _internalAxisValues = internalAxisValues;
            _externalAxisValues = externalAxisValues;
            _speedData = new SpeedData(5); // Slowest predefined tcp speed
            _movementType = 0; // The movementType is always JoinMovement
            _precision = 0;
            _robotTool = new RobotTool(); // Default Robot Tool tool0
            _robotTool.Clear(); // Empty Robot Tool
        }

        /// <summary>
        /// Method to create a absolute joint movement with an empty robot tool (no override).
        /// </summary>
        /// <param name="name">Name of joint target, must be unique.</param>
        /// <param name="internalAxisValues">List of internal axis values. The length of the list should be equal to 6.</param>
        /// <param name="externalAxisValues">List of external axis values. The length of the list should be (for now) equal to 1.</param>
        /// <param name="speedData"> The SpeedData as a SpeedData </param>
        /// <param name="precision"> Robot movement precision. If this value is -1 the robot will go to exactly the specified position. This means its ZoneData in RAPID code is set to fine. </param>
        public AbsoluteJointMovement(string name, List<double> internalAxisValues, List<double> externalAxisValues, SpeedData speedData, int precision)
        {
            _name = name;
            _internalAxisValues = internalAxisValues;
            _externalAxisValues = externalAxisValues;
            _speedData = speedData;
            _movementType = 0; // The movementType is always JoinMovement
            _precision = precision;
            _robotTool = new RobotTool(); // Default Robot Tool tool0
            _robotTool.Clear(); // Empty Robot Tool
        }

        /// <summary>
        /// Method to create a absolute joint movement. 
        /// </summary>
        /// <param name="name">Name of joint target, must be unique.</param>
        /// <param name="internalAxisValues">List of internal axis values. The length of the list should be equal to 6.</param>
        /// <param name="externalAxisValues">List of external axis values. The length of the list should be (for now) equal to 1.</param>
        /// <param name="speedData"> The SpeedData as a SpeedData </param>
        /// <param name="precision"> Robot movement precision. If this value is -1 the robot will go to exactly the specified position. This means its ZoneData in RAPID code is set to fine. </param>
        public AbsoluteJointMovement(string name, List<double> internalAxisValues, List<double> externalAxisValues, SpeedData speedData, int precision, DigitalOutput digitalOutput)
        {
            _name = name;
            _internalAxisValues = internalAxisValues;
            _externalAxisValues = externalAxisValues;
            _speedData = speedData;
            _movementType = 0; // The movementType is always JoinMovement
            _precision = precision;
            _robotTool = new RobotTool(); // Default Robot Tool tool0
            _robotTool.Clear(); // Empty Robot Tool
        }

        /// <summary>
        /// Method to create a absolute joint movement. 
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
            _movementType = 0; // The movementType is always JoinMovement
            _precision = precision;
            _robotTool = robotTool;
        }

        /// <summary>
        /// Duplicates a robot movement.
        /// </summary>
        /// <returns></returns>
        public AbsoluteJointMovement Duplicate()
        {
            AbsoluteJointMovement dup = new AbsoluteJointMovement(Name, new List<double>(InternalAxisValues), new List<double>(ExternalAxisValues), SpeedData.Duplicate(), Precision);
            return dup;
        }
        #endregion

        #region method

        /// <summary>
        /// Used to create variable definitions in the RAPID Code. It is typically called inside the CreateRAPIDCode() method of the RAPIDGenerator class.
        /// </summary>
        /// <param name="robotInfo">Defines the RobotInfo for the action.</param>
        /// <param name="RAPIDcode">Defines the RAPID Code the variable entries are added to.</param>
        /// <returns>Return the RAPID variable code.</returns>
        public override string InitRAPIDVar(RobotInfo robotInfo, string RAPIDcode)
        {
            string tempCode = "";

            // Creates Speed Data Variable Code
            string speedDataCode = _speedData.InitRAPIDVar(robotInfo, RAPIDcode);

            // Only adds speedData Variable if not already in RAPID Code
            if (!RAPIDcode.Contains(speedDataCode))
            {
                tempCode += speedDataCode;
            }

            // Creates targetName variables to check if they already exist 
            string jointTargetVar = "CONST jointtarget " + JointTargetName;

            // Only adds target code if target is not already defined
            if (!RAPIDcode.Contains(jointTargetVar))
            {
                // Creates Code Variable
                tempCode += "@" + "\t" + jointTargetVar + ":=[[";

                // Adds all Internal Axis Values
                for (int i = 0; i < this._internalAxisValues.Count; i++)
                {
                    tempCode += this._internalAxisValues[i].ToString("0.##") + ", ";
                }
                tempCode = tempCode.Remove(tempCode.Length - 2);

                // Adds all External Axis Values
                tempCode += "], [";
                for (int i = 0; i < this._externalAxisValues.Count; i++)
                {
                    tempCode += this._externalAxisValues[i].ToString("0.##") + ", ";
                }
                // Adds 9E9 for all missing external Axis Values
                for (int i = this._externalAxisValues.Count; i < 6; i++)
                {
                    tempCode += "9E9" + ", ";
                }
                tempCode = tempCode.Remove(tempCode.Length - 2);
                tempCode += "]];";
            }


            // returns RAPID code
            return tempCode;
        }

        /// <summary>
        /// Used to create action instructions in the RAPID Code. It is typically called inside the CreateRAPIDCode() method of the RAPIDGenerator class.
        /// </summary>
        /// <param name="robotToolName">Defines the robot rool name.</param>
        /// <returns>Returns the RAPID main code.</returns>
        public override string ToRAPIDFunction(string robotToolName)
        {
            // Set tool name
            string toolName;
            if (_robotTool.Name == "" || _robotTool.Name == null)
            {
                toolName = robotToolName;
            }
            else
            {
                toolName = _robotTool.Name;
            }

            // Set zone data text (precision value)
            string zoneName;
            if (_precision < 0) 
            { 
                zoneName = @", fine, "; 
            }
            else 
            { 
                zoneName = @", z" + _precision.ToString() + @", "; 
            }

            // MoveAbsJ
            return ("@" + "\t" + "MoveAbsJ " + JointTargetName + @", " + _speedData.Name + zoneName + toolName + ";");

        }
        #endregion

        #region properties
        /// <summary>
        /// A boolean that indicuate if the Movement object is valid.
        /// </summary>
        public bool IsValid
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
        /// The robot target name when it is used as a joint target.
        /// </summary>
        public string JointTargetName
        {
            get { return Name + "_jm"; }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<double> InternalAxisValues
        {
            get { return _internalAxisValues; }
            set { _internalAxisValues = value; }
        }

        /// <summary>
        /// 
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
        /// For the JointMovement Class the MovementType can not be set.
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

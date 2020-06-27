// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Linq;
using System.Collections.Generic;
// RobotComponents Libs
using RobotComponents.BaseClasses.Definitions;

namespace RobotComponents.BaseClasses.Actions
{
    /// <summary>
    /// Absolute Joint Movement class
    /// </summary>
    [Obsolete("The Absolute Joint Movement class will be removed in the future. Instead, combine a Joint Target object with a Movement object.", false)]
    public class AbsoluteJointMovement : Action
    {
        #region fields
        // Fixed fields
        private string _name; // target variable name
        private List<double> _internalAxisValues;
        private List<double> _externalAxisValues;
        private SpeedData _speedData;
        private readonly int _movementType;
        private ZoneData _zoneData;
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
            _internalAxisValues = CheckInternalAxisValues(internalAxisValues);
            _externalAxisValues = Enumerable.Repeat(9e9, 6).ToList();
            _speedData = new SpeedData(5); // Slowest predefined tcp speed
            _movementType = 0; // The movementType is always an Absolute Joint Movement
            _zoneData = new ZoneData(0);
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
            _internalAxisValues = CheckInternalAxisValues(internalAxisValues);
            _externalAxisValues = CheckExternalAxisValues(externalAxisValues);
            _speedData = new SpeedData(5); // Slowest predefined tcp speed
            _movementType = 0; // The movementType is always an Absolute Joint Movement
            _zoneData = new ZoneData(0);
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
        /// <param name="precision"> Robot movement precision. This value will be casted to the nearest predefined zonedata value. Use -1 for fine. </param>
        public AbsoluteJointMovement(string name, List<double> internalAxisValues, List<double> externalAxisValues, SpeedData speedData, int precision)
        {
            _name = name;
            _internalAxisValues = CheckInternalAxisValues(internalAxisValues);
            _externalAxisValues = CheckExternalAxisValues(externalAxisValues);
            _speedData = speedData;
            _movementType = 0; // The movement type is always an Absolute Joint Movement
            _zoneData = new ZoneData(precision);
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
        /// <param name="zoneData"> The ZoneData as a ZoneData </param>
        public AbsoluteJointMovement(string name, List<double> internalAxisValues, List<double> externalAxisValues, SpeedData speedData, ZoneData zoneData)
        {
            _name = name;
            _internalAxisValues = CheckInternalAxisValues(internalAxisValues);
            _externalAxisValues = CheckExternalAxisValues(externalAxisValues);
            _speedData = speedData;
            _movementType = 0; // The movement type is always an Absolute Joint Movement
            _zoneData = zoneData;
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
        /// <param name="precision"> Robot movement precision. This value will be casted to the nearest predefined zonedata value. Use -1 for fine. </param>
        /// <param name="robotTool"> The Robot Tool. This will override the set default tool. </param>
        public AbsoluteJointMovement(string name, List<double> internalAxisValues, List<double> externalAxisValues, SpeedData speedData, int precision, RobotTool robotTool)
        {
            _name = name;
            _internalAxisValues = CheckInternalAxisValues(internalAxisValues);
            _externalAxisValues = CheckExternalAxisValues(externalAxisValues);
            _speedData = speedData;
            _movementType = 0; // The movement type is always an Absolute Joint Movement
            _zoneData = new ZoneData(precision);
            _robotTool = robotTool;
        }

        /// <summary>
        /// Method to create an absolute joint movement. 
        /// </summary>
        /// <param name="name">Name of joint target, must be unique.</param>
        /// <param name="internalAxisValues">List of internal axis values. The length of the list should be equal to 6.</param>
        /// <param name="externalAxisValues">List of external axis values. The length of the list should be (for now) equal to 1.</param>
        /// <param name="speedData"> The SpeedData as a SpeedData </param>
        /// <param name="zoneData"> The ZoneData as a ZoneData </param>
        /// <param name="robotTool"> The Robot Tool. This will override the set default tool. </param>
        public AbsoluteJointMovement(string name, List<double> internalAxisValues, List<double> externalAxisValues, SpeedData speedData, ZoneData zoneData, RobotTool robotTool)
        {
            _name = name;
            _internalAxisValues = CheckInternalAxisValues(internalAxisValues);
            _externalAxisValues = CheckExternalAxisValues(externalAxisValues);
            _speedData = speedData;
            _movementType = 0; // The movement type is always an Absolute Joint Movement
            _zoneData = zoneData;
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
            _zoneData = jointMovement.ZoneData.Duplicate();

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
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns> A string that represents the current object. </returns>
        public override string ToString()
        {
            if (!this.IsValid)
            {
                return "Invalid Absolute Joint Movement";
            }
            else
            {
                return "Absolute Joint Movement (" + this.Name + ")";
            }
        }

        /// <summary>
        /// Method that checks the list with internal axis values. 
        /// Always returns a list with 6 external axis values. 
        /// Missing values will be filled with 0.  
        /// </summary>
        /// <param name="axisValues">A list with the internal axis values.</param>
        /// <returns>Returns a list with 6 internal axis values.</returns>
        private List<double> CheckInternalAxisValues(List<double> axisValues)
        {
            List<double> result = new List<double>();
            int n = Math.Min(axisValues.Count, 6);

            // Copy definied internal axis values
            for (int i = 0; i < n; i++)
            {
                result.Add(axisValues[i]);
            }

            // Add up missing internal axis values
            for (int i = n; i < 6; i++)
            {
                result.Add(0);
            }

            return result;
        }

        /// <summary>
        /// Method that checks the list with external axis values. 
        /// Always returns a list with 6 external axis values. 
        /// For missing values 9e9 (not connected) will be used. 
        /// </summary>
        /// <param name="axisValues">A list with the external axis values.</param>
        /// <returns>Returns a list with 6 external axis values.</returns>
        private List<double> CheckExternalAxisValues(List<double> axisValues)
        {
            List<double> result = new List<double>();
            int n = Math.Min(axisValues.Count, 6);

            // Copy definied external axis values
            for (int i = 0; i < n; i++)
            {
                result.Add(axisValues[i]);
            }

            // Add missing external axis values
            for (int i = n; i < 6; i++)
            {
                result.Add(9e9);
            }

            return result;
        }

        /// <summary>
        /// Cheks for the axis limits and returns a list with possible errors messages. 
        /// </summary>
        /// <param name="robot"> The robot info to check the axis values for. </param>
        /// <returns> Returns a list with error messages. </returns>
        public List<string> CheckForAxisLimits(Robot robot)
        {
            // Initiate list
            List<string> errors = new List<string>();

            // Check for internal axis values
            for (int i = 0; i < _internalAxisValues.Count; i++)
            {
                if (robot.InternalAxisLimits[i].IncludesParameter(_internalAxisValues[i], false) == false)
                {
                    errors.Add("Movement " + _name + "\\wobj0: Internal axis value " + (i + 1).ToString() + " is not in range.");
                }
            }

            // Check for external axis values
            for (int i = 0; i < robot.ExternalAxis.Count; i++)
            {
                int logicNumber = (int)robot.ExternalAxis[i].AxisNumber;

                if (_externalAxisValues[logicNumber] == 9e9)
                {
                    errors.Add("Movement " + _name + "\\wobj0: External axis value " + (i + 1).ToString() + " is not definied by the user.");
                }

                else if (robot.ExternalAxis[i].AxisLimits.IncludesParameter(_externalAxisValues[logicNumber], false) == false)
                {
                    errors.Add("Movement " + _name + "\\wobj0: External axis value " + (i + 1).ToString() + " is not in range.");
                }
            }

            return errors;
        }

        /// <summary>
        /// Used to create variable definition code of this action. 
        /// </summary>
        /// <param name="robot"> Defines the Robot were the code is generated for. </param>
        /// <returns> Returns the RAPID code line as a string. </returns>
        public override string ToRAPIDDeclaration(Robot robot)
        {
            // Creates Code Variable
            string code = "CONST jointtarget " + _name + " := [[";

            // Adds all Internal Axis Values
            for (int i = 0; i < this._internalAxisValues.Count; i++)
            {
                code += this._internalAxisValues[i].ToString("0.##") + ", ";
            }
            code = code.Remove(code.Length - 2);

            // Adds all External Axis Values
            code += "], [";
            for (int i = 0; i < 6; i++)
            {
                if (this._externalAxisValues[i] == 9e9)
                {
                    code += "9E9" + ", ";
                }
                else
                {
                    code += this._externalAxisValues[i].ToString("0.##") + ", ";
                }
            }

            code = code.Remove(code.Length - 2);
            code += "]];";

            return code;
        }

        /// <summary>
        /// Used to create action instruction code line. 
        /// </summary>
        /// <param name="robot"> Defines the Robot were the code is generated for. </param>
        /// <returns> Returns the RAPID code line as a string. </returns>
        public override string ToRAPIDInstruction(Robot robot)
        {
            // Set tool name
            string toolName;

            // Check first if a tool is set
            if (_robotTool == null) 
            { 
                toolName = robot.Tool.Name; 
            }
            // Check if a tool is set by checking the name (tool can be empty)
            else if (_robotTool.Name == "" || _robotTool.Name == null)
            { 
                toolName = robot.Tool.Name; 
            } 
            // Otherwise don't set a tool. Last overwrite is used that is combined with the movement.
            else 
            { 
                toolName = _robotTool.Name; 
            }

            // MoveAbsJ
            string code = "MoveAbsJ " + _name + ", " + _speedData.Name + ", " + _zoneData.Name + ", " + toolName + ";";

            return code;
        }

        /// <summary>
        /// Used to create variable definitions in the RAPID Code. It is typically called inside the CreateRAPIDCode() method of the RAPIDGenerator class.
        /// </summary>
        /// <param name="RAPIDGenerator"> Defines the RAPIDGenerator. </param>
        public override void ToRAPIDDeclaration(RAPIDGenerator RAPIDGenerator)
        {
            // Creates SpeedData Variable Code
            _speedData.ToRAPIDDeclaration(RAPIDGenerator);

            // Creates ZoneData Variable Code
            _zoneData.ToRAPIDDeclaration(RAPIDGenerator);

            // Only adds target code if target is not already defined
            if (!RAPIDGenerator.JointTargets.ContainsKey(_name))
            {
                RAPIDGenerator.JointTargets.Add(_name, this.ConvertToJointTarget());
                RAPIDGenerator.StringBuilder.Append(Environment.NewLine + "\t" + this.ToRAPIDDeclaration(RAPIDGenerator.Robot));
                RAPIDGenerator.ErrorText.AddRange(this.CheckForAxisLimits(RAPIDGenerator.Robot));
            }
        }

        /// <summary>
        /// Used to create action instructions in the RAPID Code. It is typically called inside the CreateRAPIDCode() method of the RAPIDGenerator class.
        /// </summary>
        /// <param name="RAPIDGenerator"> Defines the RAPIDGenerator. </param>
        public override void ToRAPIDInstruction(RAPIDGenerator RAPIDGenerator)
        {
            RAPIDGenerator.StringBuilder.Append(Environment.NewLine + "\t\t" + this.ToRAPIDInstruction(RAPIDGenerator.Robot));
        }
        #endregion

        #region convert methods: this class is obsolete, however, with these methods it can be converted to new objects
        /// <summary>
        /// Defines the External Joint Position of this Absolute Joint Movement
        /// </summary>
        /// <returns> The External Joint Position of this Absolute Joint Movement. </returns>
        public Movement ConvertToMovement()
        {
            return new Movement(ConvertToJointTarget(), _speedData, _movementType, _zoneData, _robotTool);
        }

        /// <summary>
        /// Defines the Joint Target of this Absolute Joint Movement
        /// </summary>
        /// <returns> The Joint Target of this Absolute Joint Movement. </returns>
        public JointTarget ConvertToJointTarget()
        {
            return new JointTarget(_name, this.ConvertToRobotJointPosition(), this.ConvertToExternalJointPosition());
        }

        /// <summary>
        /// Defines the Robot Joint Position of this Absolute Joint Movement
        /// </summary>
        /// <returns> The Robot Joint Position of this Absolute Joint Movement. </returns>
        public RobotJointPosition ConvertToRobotJointPosition()
        {
            return new RobotJointPosition(_internalAxisValues);
        }

        /// <summary>
        /// Defines the External Joint Position of this Absolute Joint Movement
        /// </summary>
        /// <returns> The External Joint Position of this Absolute Joint Movement. </returns>
        public ExternalJointPosition ConvertToExternalJointPosition()
        {
            return new ExternalJointPosition(_externalAxisValues);
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
                if (SpeedData.IsValid == false) { return false; }
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
        /// Defines the pose of the internal axis values in degree. 
        /// </summary>
        public List<double> InternalAxisValues
        {
            get { return _internalAxisValues; }
            set { _internalAxisValues = CheckInternalAxisValues(value); }
        }

        /// <summary>
        /// Defines the pose of the external axis values in degrees or in meters.
        /// </summary>
        public List<double> ExternalAxisValues
        {
            get { return _externalAxisValues; }
            set { _externalAxisValues = CheckExternalAxisValues(value); }
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
        /// The zone data that applies to movements.
        /// It defines the size of the generated corner path.
        /// </summary>
        public ZoneData ZoneData
        {
            get { return _zoneData; }
            set { _zoneData = value; }
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

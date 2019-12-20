using System.Collections.Generic;

namespace RobotComponents.BaseClasses
{
    /// <summary>
    /// Movement class
    /// </summary>
    public class Movement : Action
    {
        #region fields
        // Fixed fields
        Target _target;
        SpeedData _speedData;
        int _movementType;
        int _precision;

        // Variable fields: can be null
        RobotTool _robotTool;
        WorkObject _workObject;
        DigitalOutput _digitalOutput;
        #endregion

        #region constructors
        /// <summary>
        /// An empty movement constructor.
        /// </summary>
        public Movement()
        {
        }

        /// <summary>
        /// Method to create a robot movement. 
        /// </summary>
        /// <param name="target"> The target as a Target. </param>
        /// <param name="speedData"> The SpeedData as a SpeedData </param>
        /// <param name="movementType"> The movement type as an integer (0, 1 or 2). </param>
        /// <param name="precision"> Robot movement precision. If this value is -1 the robot will go to exactly the specified position. This means its ZoneData in RAPID code is set to fine. </param>
        public Movement(Target target, SpeedData speedData, int movementType, int precision)
        {
            _target = target;
            _speedData = speedData;
            _movementType = movementType;
            _precision = precision;
            _robotTool = null;
            _workObject = new WorkObject(); //wobj0
            _digitalOutput = null;
        }

        /// <summary>
        /// Method to create a robot movement. 
        /// </summary>
        /// <param name="target"> The target as a Target. </param>
        /// <param name="speedData"> The SpeedData as a SpeedData </param>
        /// <param name="movementType"> The movement type as an integer (0, 1 or 2). </param>
        /// <param name="precision"> Robot movement precision. If this value is -1 the robot will go to exactly the specified position. This means its ZoneData in RAPID code is set to fine. </param>
        /// <param name="robotTool"> The Robot Tool. This will override the set default tool. </param>
        /// <param name="workObject"> The Work Object as a Work Object </param>
        public Movement(Target target, SpeedData speedData, int movementType, int precision, RobotTool robotTool, WorkObject workObject)
        {
            _target = target;
            _speedData = speedData;
            _movementType = movementType;
            _precision = 0;
            _robotTool = robotTool;
            _workObject = workObject;
            _digitalOutput = null;
        }

        /// <summary>
        /// Method to create a robot movement. 
        /// </summary>
        /// <param name="target"> The target as a Target. </param>
        /// <param name="speedData"> The SpeedData as a SpeedData </param>
        /// <param name="movementType"> The movement type as an integer (0, 1 or 2). </param>
        /// <param name="precision"> Robot movement precision. If this value is -1 the robot will go to exactly the specified position. This means its ZoneData in RAPID code is set to fine. </param>
        /// <param name="robotTool"> The Robot Tool. This will override the set default tool. </param>
        /// <param name="workObject"> The Work Object as a Work Object </param>
        /// <param name="digitalOutput"> A Digital Output as a Digital Output. When set this will define a MoveLDO or a MoveJDO. </param>
        public Movement(Target target, SpeedData speedData, int movementType, int precision, RobotTool robotTool, WorkObject workObject, DigitalOutput digitalOutput)
        {
            _target = target;
            _speedData = speedData;
            _movementType = movementType;
            _precision = 0;
            _robotTool = robotTool;
            _workObject = workObject;
            _digitalOutput = digitalOutput;
        }

        /// <summary>
        /// Duplicates a robot movement.
        /// </summary>
        /// <returns></returns>
        public Movement Duplicate()
        {
            Movement dup = new Movement(Target, SpeedData, MovementType, Precision, RobotTool, WorkObject, DigitalOutput);
            return dup;
        }
        #endregion

        #region method
        public override string InitRAPIDVar(RobotInfo robotInfo, string RAPIDcode)
        {
            string tempCode = "";

            // Creates Speed Data Variable Code
            string speedDataCode = _speedData.InitRAPIDVar();

            // Only adds speedData Variable if not already in RAPID Code
            if (!RAPIDcode.Contains(speedDataCode))
            {
                tempCode += speedDataCode;
            }

            // Creates targetName variables to check if they already exist 
            string robTargetVar = "VAR robtarget " + _target.RobTargetName;
            string jointTargetVar = "CONST jointtarget " + _target.JointTargetName;

            // Create a robtarget if  the movement type is MoveL (1) or MoveJ (2)
            if (MovementType == 1 || MovementType == 2)
            {
                // Only adds target code if target is not already defined
                if (!RAPIDcode.Contains(robTargetVar))
                {
                    tempCode += ("@" + "\t" + robTargetVar + ":=[[" 
                        + _target.Plane.Origin.X.ToString("0.##") + ", " 
                        + _target.Plane.Origin.Y.ToString("0.##") + ", " 
                        + _target.Plane.Origin.Z.ToString("0.##") + "], ["
                        + _target.Quat.A.ToString("0.######") + ", " 
                        + _target.Quat.B.ToString("0.######") + ", " 
                        + _target.Quat.C.ToString("0.######") + ", " 
                        + _target.Quat.D.ToString("0.######") + "]," 
                        + "[0,0,0," + _target.AxisConfig);

                    // Adds all External Axis Values
                    InverseKinematics inverseKinematics = new InverseKinematics(_target, robotInfo);
                    inverseKinematics.Calculate();
                    List<double> externalAxisValues = inverseKinematics.ExternalAxisValues;
                    tempCode += "], [";
                    for (int i = 0; i < externalAxisValues.Count; i++)
                    {
                        tempCode += externalAxisValues[i].ToString("0.##") + ", ";
                    }

                    // Adds 9E9 for all missing external Axis Values
                    for (int i = externalAxisValues.Count; i < 6; i++)
                    {
                        tempCode += "9E9" + ", ";
                    }
                    tempCode = tempCode.Remove(tempCode.Length - 2);
                    tempCode += "]];";
                }
            }

            // Create a jointtarget if the movement type is MoveAbsJ (0)
            else
            {
                // Only adds target code if target is not already defined
                if (!RAPIDcode.Contains(jointTargetVar))
                {
                    // Calculates AxisValues
                    InverseKinematics inverseKinematics = new InverseKinematics(_target, robotInfo);
                    inverseKinematics.Calculate();
                    List<double> internalAxisValues = inverseKinematics.InternalAxisValues;
                    List<double> externalAxisValues = inverseKinematics.ExternalAxisValues;

                    // Creates Code Variable
                    tempCode += "@" + "\t" + jointTargetVar + ":=[[";

                    // Adds all Internal Axis Values
                    for (int i = 0; i < internalAxisValues.Count; i++)
                    {
                        tempCode += internalAxisValues[i].ToString("0.##") + ", ";
                    }
                    tempCode = tempCode.Remove(tempCode.Length - 2);

                    // Adds all External Axis Values
                    tempCode += "], [";
                    for (int i = 0; i < externalAxisValues.Count; i++)
                    {
                        tempCode += externalAxisValues[i].ToString("0.##") + ", ";
                    }
                    // Adds 9E9 for all missing external Axis Values
                    for (int i = externalAxisValues.Count; i < 6; i++)
                    {
                        tempCode += "9E9" + ", ";
                    }
                    tempCode = tempCode.Remove(tempCode.Length - 2);
                    tempCode += "]];";
                }
            }

            // returns RAPID code
            return tempCode;
        }

        public override string ToRAPIDFunction(string robotToolName)
        {
            // Set tool name
            string toolName;
            if (_robotTool == null) { toolName = robotToolName; }
            else { toolName = _robotTool.Name; }

            // Set zone data text (precision value)
            string zoneName;
            if (_precision < 0) { zoneName = @", fine, "; }
            else { zoneName = @", z" + _precision.ToString() + ", "; }

            // MoveAbsJ
            if (_movementType == 0)
            {
                return ("@" + "\t" + "MoveAbsJ " + _target.JointTargetName + @", " + _speedData.Name + zoneName + toolName + "\\WObj:=" + _workObject.Name + ";");
            }
            // MoveL
            else if (_movementType == 1 && _digitalOutput == null)
            {
                return ("@" + "\t" + "MoveL " + _target.RobTargetName + @", " + _speedData.Name + zoneName + toolName + "\\WObj:=" + _workObject.Name + ";");
            }
            // MoveJ
            else if (_movementType == 2 && _digitalOutput == null)
            {
                return ("@" + "\t" + "MoveJ " + _target.RobTargetName + @", " + _speedData.Name + zoneName + toolName + "\\WObj:=" + _workObject.Name + ";");
            }
            // MoveL
            else if (_movementType == 1 && _digitalOutput != null)
            {
                return ("@" + "\t" + "MoveLDO " + _target.RobTargetName + @", " + _speedData.Name + zoneName + toolName + "\\WObj:=" + _workObject.Name + @", " + _digitalOutput.Name + @", " + (_digitalOutput.IsActive ? 1 : 0) + ";");
            }
            // MoveJ
            else if (_movementType == 2 && _digitalOutput != null)
            {
                return ("@" + "\t" + "MoveJDO " + _target.RobTargetName + @", " + _speedData.Name + zoneName + toolName + "\\WObj:=" + _workObject.Name + @", " + _digitalOutput.Name + @", " + (_digitalOutput.IsActive ? 1 : 0) + ";");
            }
            // Return nothing if a wrong movement type is used
            else
            {
                return "";
            }
        }
        #endregion

        #region properties
        public bool IsValid
        {
            get
            {
                if (Target == null) { return false; }
                if (SpeedData == null) { return false; }
                return true;
            }
        }

        public Target Target
        {
            get { return _target; }
            set { _target = value; }
        }

        public SpeedData SpeedData
        {
            get { return _speedData; }
            set { _speedData = value; }
        }

        public int MovementType
        {
            get { return _movementType; }
            set { _movementType = value; }
        }

        public int Precision
        {
            get { return _precision; }
            set { _precision = value; }
        }

        public RobotTool RobotTool
        {
            get { return _robotTool; }
            set { _robotTool = value; }
        }

        public WorkObject WorkObject
        {
            get { return _workObject; }
            set { _workObject = value; }
        }

        public DigitalOutput DigitalOutput
        {
            get { return _digitalOutput; }
            set { _digitalOutput = value; }
        }
        #endregion
    }
}

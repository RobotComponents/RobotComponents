using System.Collections.Generic;

using RobotComponents.BaseClasses.Definitions;

namespace RobotComponents.BaseClasses.Actions
{
    /// <summary>
    /// Joint Movement class
    /// </summary>
    public class JointMovement : Action
    {
        #region fields
        // Fixed fields
        private string _name; // target variable name
        private List<double> _internalAxisValues;
        private List<double> _externalAxisValues;
        private SpeedData _speedData;
        private int _movementType;
        private int _precision;
        //private Plane _globalTargetPlane;

        // Variable fields
        RobotTool _robotTool;
        //WorkObject _workObject;
        DigitalOutput _digitalOutput;
        #endregion

        #region constructors
        /// <summary>
        /// An empty movement constructor.
        /// </summary>
        public JointMovement()
        {
        }

        /// <summary>
        /// Method to create a robot movement with as only argument an robot target. 
        /// This construct is made to automatically cast a robot target to a robot movement. 
        /// </summary>
        /// <param name="name">Robot target name, must be unique.</param>
        /// <param name="target"> The target as a Target. </param>
        public JointMovement(string name, List<double> internalAxisValues, List<double> externalAxisValues)
        {
            _name = name;
            _internalAxisValues = internalAxisValues;
            _externalAxisValues = externalAxisValues;
            _speedData = new SpeedData(5); // Slowest predefined tcp speed
            _movementType = 0; // The movementType is always JoinMovement
            _precision = 0;
            _robotTool = new RobotTool(); // Default Robot Tool tool0
            _robotTool.Clear(); // Empty Robot Tool
            //_workObject = new WorkObject(); // Default work object wobj0
            _digitalOutput = new DigitalOutput(); // InValid / empty DO

            Initialize();
        }

        /// <summary>
        /// Method to create a robot movement with an empty robot tool (no override), a default work object (wobj0) and an empty digital output. 
        /// </summary>
        /// <param name="target"> The target as a Target. </param>
        /// <param name="speedData"> The SpeedData as a SpeedData </param>
        /// <param name="movementType"> The movement type as an integer (0, 1 or 2). </param>
        /// <param name="precision"> Robot movement precision. If this value is -1 the robot will go to exactly the specified position. This means its ZoneData in RAPID code is set to fine. </param>
        public JointMovement(string name, List<double> internalAxisValues, List<double> externalAxisValues, SpeedData speedData, int precision)
        {
            _name = name;
            _internalAxisValues = internalAxisValues;
            _externalAxisValues = externalAxisValues;
            _speedData = speedData;
            _movementType = 0; // The movementType is always JoinMovement
            _precision = precision;
            _robotTool = new RobotTool(); // Default Robot Tool tool0
            _robotTool.Clear(); // Empty Robot Tool
            //_workObject = new WorkObject(); // Default work object wobj0
            _digitalOutput = new DigitalOutput(); // InValid / empty DO
            
            Initialize();
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
        public JointMovement(string name, List<double> internalAxisValues, List<double> externalAxisValues, SpeedData speedData, int precision, DigitalOutput digitalOutput)
        {
            _name = name;
            _internalAxisValues = internalAxisValues;
            _externalAxisValues = externalAxisValues;
            _speedData = speedData;
            _movementType = 0; // The movementType is always JoinMovement
            _precision = precision;
            _robotTool = new RobotTool(); // Default Robot Tool tool0
            _robotTool.Clear(); // Empty Robot Tool
            //_workObject = workObject;
            _digitalOutput = digitalOutput;

            Initialize();
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
        public JointMovement(string name, List<double> internalAxisValues, List<double> externalAxisValues, SpeedData speedData, int precision, RobotTool robotTool, DigitalOutput digitalOutput)
        {
            _name = name;
            _internalAxisValues = internalAxisValues;
            _externalAxisValues = externalAxisValues;
            _speedData = speedData;
            _movementType = 0; // The movementType is always JoinMovement
            _precision = precision;
            _robotTool = robotTool;
            //_workObject = workObject;
            _digitalOutput = digitalOutput;

            Initialize();
        }

        /// <summary>
        /// Duplicates a robot movement.
        /// </summary>
        /// <returns></returns>
        public JointMovement Duplicate()
        {
            JointMovement dup = new JointMovement(Name, InternalAxisValues, ExternalAxisValues, SpeedData, Precision, DigitalOutput);
            return dup;
        }
        #endregion

        #region method
        /// <summary>
        /// A method that calls all the other methods that are needed to initialize the data that is needed to construct a valid movement object. 
        /// </summary>
        private void Initialize()
        {
            //CalculateGlobalTargetPlane();
        }

        /// <summary>
        /// A method that can be called to reinitialize all the data that is needed to construct a valid movement object.
        /// </summary>
        public void ReInitialize()
        {
            Initialize();
        }

        ///// <summary>
        ///// Calculate the position and the orientation of the target in the global coordinate system. 
        ///// </summary>
        //private void CalculateGlobalTargetPlane()
        //{
        //    // Deep copy the target plane
        //    _globalTargetPlane = new Plane(Target.Plane);

        //    // Re-orient the target plane to the work object plane
        //    Transform orient = Transform.PlaneToPlane(Plane.WorldXY, WorkObject.GlobalWorkObjectPlane);
        //    _globalTargetPlane.Transform(orient);
        //}

        /// <summary>
        /// Used to create variable definitions in the RAPID Code. It is typically called inside the CreateRAPIDCode() method of the RAPIDGenerator class.
        /// </summary>
        /// <param name="RAPIDGenerator"> Defines the RAPIDGenerator. </param>
        /// <returns>Return the RAPID variable code.</returns>
        public override void InitRAPIDVar(RAPIDGenerator RAPIDGenerator)
        {
            string tempCode = "";

            // Only adds speedData Variable if not already in RAPID Code
            if (!RAPIDGenerator.SpeedDatas.ContainsKey(_speedData.Name))
            {
                // Creates SpeedData Variable Code and adds it to the tempCoode
                _speedData.InitRAPIDVar(RAPIDGenerator);
                // Adds SpeedData to RAPIDGenerator SpeedDatasDictionary
                RAPIDGenerator.SpeedDatas.Add(_speedData.Name, _speedData);
            }


            // Target with global plane (for ik)
            //Target globalTarget = _target.Duplicate();
            //globalTarget.Plane = GlobalTargetPlane;

            // Create a robtarget if  the movement type is MoveL (1) or MoveJ (2)
            if (MovementType == 1 || MovementType == 2)
            {
                //// Only adds target code if target is not already defined
                //if (!RAPIDcode.Contains(robTargetVar))
                //{
                //    tempCode += ("@" + "\t" + robTargetVar + " := [[" 
                //        + _target.Plane.Origin.X.ToString("0.##") + ", " 
                //        + _target.Plane.Origin.Y.ToString("0.##") + ", " 
                //        + _target.Plane.Origin.Z.ToString("0.##") + "], ["
                //        + _target.Quat.A.ToString("0.######") + ", " 
                //        + _target.Quat.B.ToString("0.######") + ", " 
                //        + _target.Quat.C.ToString("0.######") + ", " 
                //        + _target.Quat.D.ToString("0.######") + "]," 
                //        + "[0,0,0," + _target.AxisConfig);

                //    // Adds all External Axis Values
                //    InverseKinematics inverseKinematics = new InverseKinematics(globalTarget, robotInfo);
                //    inverseKinematics.Calculate();
                //    List<double> externalAxisValues = inverseKinematics.ExternalAxisValues;
                //    tempCode += "], [";
                //    for (int i = 0; i < externalAxisValues.Count; i++)
                //    {
                //        tempCode += externalAxisValues[i].ToString("0.##") + ", ";
                //    }

                //    // Adds 9E9 for all missing external Axis Values
                //    for (int i = externalAxisValues.Count; i < 6; i++)
                //    {
                //        if (Target.ExternalAxisValues[i] == 9e9)
                //        {
                //            tempCode += "9E9" + ", ";
                //        }
                //        else
                //        {
                //            tempCode += Target.ExternalAxisValues[i].ToString("0.##") + ", ";
                //        }
                        
                //    }
                //    tempCode = tempCode.Remove(tempCode.Length - 2);
                //    tempCode += "]];";
                //}
            }
            else // Create a jointtarget if the movement type is MoveAbsJ (0)
            {
                // Only adds target code if target is not already defined
                if (!RAPIDGenerator.Targets.ContainsKey(JointTargetName))
                {
                    //// Calculates AxisValues
                    //InverseKinematics inverseKinematics = new InverseKinematics(globalTarget, robotInfo);
                    //inverseKinematics.Calculate();
                    //List<double> internalAxisValues = inverseKinematics.InternalAxisValues;
                    //List<double> externalAxisValues = inverseKinematics.ExternalAxisValues;


                    // Adds Target to RAPIDGenerator SpeedDatasDictionary
                    RAPIDGenerator.Targets.Add(JointTargetName, new Target());

                    // Creates targetName variables to check if they already exist 
                    //string robTargetVar = "VAR robtarget " + _target.RobTargetName;
                    string jointTargetVar = "CONST jointtarget " + JointTargetName;

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
                        //if (Target.ExternalAxisValues[i] == 9e9)
                        //{
                        //    tempCode += "9E9" + ", ";
                        //}
                        //else
                        //{
                        //    tempCode += Target.ExternalAxisValues[i].ToString("0.##") + ", ";
                        //}
                    }
                    tempCode = tempCode.Remove(tempCode.Length - 2);
                    tempCode += "]];";
                }
            }
        }

        /// <summary>
        /// Used to create action instructions in the RAPID Code. It is typically called inside the CreateRAPIDCode() method of the RAPIDGenerator class.
        /// </summary>
        /// <param name="RAPIDGenerator"> Defines the RAPIDGenerator. </param>
        /// <returns>Returns the RAPID main code.</returns>
        public override void ToRAPIDFunction(RAPIDGenerator RAPIDGenerator)
        {
            // Set tool name
            string toolName = _robotTool.Name;
      
            // Set zone data text (precision value)
            string zoneName;
            if (_precision < 0) { zoneName = @", fine, "; }
            else { zoneName = @", z" + _precision.ToString() + @", "; }

            // Digital output bool
            bool moveDO = false;
            if (_digitalOutput.IsValid == true) { moveDO = true; }

            // A movement not combined with a digital output
            if (moveDO == false)
            {
                // MoveAbsJ
                if (_movementType == 0)
                {
                    RAPIDGenerator.StringBuilder.Append("@" + "\t" + "MoveAbsJ " + JointTargetName + @", " + _speedData.Name + zoneName + "toolName" + "\\WObj:=" + "wobj0" + ";");
                }

                //// MoveL
                //else if (_movementType == 1)
                //{
                //    return ("@" + "\t" + "MoveL " + _target.RobTargetName + @", " + _speedData.Name + zoneName + toolName + "\\WObj:=" + _workObject.Name + ";");
                //}

                //// MoveJ
                //else if (_movementType == 2)
                //{
                //    return ("@" + "\t" + "MoveJ " + _target.RobTargetName + @", " + _speedData.Name + zoneName + toolName + "\\WObj:=" + _workObject.Name + ";");
                //}

                // Return nothing if a wrong movement type is used

            }

            // A movement combined with a digital output
            else
            {
                // MoveAbsJ + SetDO: There is no RAPDID function that combines the an absolute joint movement and a DO.
                // Therefore, we write two separate RAPID code lines for an aboslute joint momvement combined with a DO. 
                if (_movementType == 0)
                {
                    // Empty string
                    string tempCode = "";
                    // Add the code line for the absolute joint movement
                    RAPIDGenerator.StringBuilder.Append("@" + "\t" + "MoveAbsJ " + JointTargetName + @", " + _speedData.Name + zoneName + toolName + "\\WObj:=" + "wobj0" + ";");
                    // Add the code line for the digital output
                    _digitalOutput.ToRAPIDFunction(RAPIDGenerator);
                }

                //// MoveLDO
                //else if (_movementType == 1)
                //{
                //    return ("@" + "\t" + "MoveLDO " + _target.RobTargetName + @", " + _speedData.Name + zoneName + toolName + "\\WObj:=" + _workObject.Name + @", " + _digitalOutput.Name + @", " + (_digitalOutput.IsActive ? 1 : 0) + ";");
                //}

                //// MoveJDO
                //else if (_movementType == 2)
                //{
                //    return ("@" + "\t" + "MoveJDO " + _target.RobTargetName + @", " + _speedData.Name + zoneName + toolName + "\\WObj:=" + _workObject.Name + @", " + _digitalOutput.Name + @", " + (_digitalOutput.IsActive ? 1 : 0) + ";");
                //}

    
            }
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

        ///// <summary>
        ///// The position and the orientation of the used target in the global coordinate system. 
        ///// </summary>
        //public Plane GlobalTargetPlane
        //{
        //    get { return _globalTargetPlane; }
        //    set { _globalTargetPlane = value; }
        //}

        /// <summary>
        /// The tool in use when the robot moves. 
        /// </summary>
        public RobotTool RobotTool
        {
            get { return _robotTool; }
            set { _robotTool = value; }
        }

        ///// <summary>
        ///// The work object (coordinate system) to which the robot position in the instruction is related.
        ///// </summary>
        //public WorkObject WorkObject
        //{
        //    get { return _workObject; }
        //    set { _workObject = value; }
        //}

        /// <summary>
        /// The digital output. If an empty digital output is set a normal movement will be set (MoveAbsJ, MoveL or MoveJ). 
        /// If a valid digital output is combined movement will be created (MoveLDO or MoveJDO). 
        /// In case an absolute joint movement is set an extra code line will be added that sets the digital output (SetDO).
        /// </summary>
        public DigitalOutput DigitalOutput
        {
            get { return _digitalOutput; }
            set { _digitalOutput = value; }
        }
        #endregion
    }
}

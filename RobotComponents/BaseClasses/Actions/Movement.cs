// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/EDEK-UniKassel/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.BaseClasses.Definitions;
using RobotComponents.BaseClasses.Kinematics;

namespace RobotComponents.BaseClasses.Actions
{
    /// <summary>
    /// Movement class
    /// </summary>
    public class Movement : Action
    {
        #region fields
        // Fixed fields
        private Target _target;
        private SpeedData _speedData;
        private int _movementType; //TODO: convert to enum
        private int _precision;
        private Plane _globalTargetPlane;

        // Variable fields
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
        /// Method to create a robot movement with as only argument an robot target. 
        /// This construct is made to automatically cast a robot target to a robot movement. 
        /// </summary>
        /// <param name="target"> The target as a Target. </param>
        public Movement(Target target)
        {
            _target = target;
            _speedData = new SpeedData(5); // Slowest predefined tcp speed
            _movementType = 0;
            _precision = 0;
            _robotTool = new RobotTool(); // Default Robot Tool tool0
            _robotTool.Clear(); // Empty Robot Tool
            _workObject = new WorkObject(); // Default work object wobj0
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
        public Movement(Target target, SpeedData speedData, int movementType, int precision)
        {
            _target = target;
            _speedData = speedData;
            _movementType = movementType;
            _precision = precision;
            _robotTool = new RobotTool(); // Default Robot Tool tool0
            _robotTool.Clear(); // Empty Robot Tool
            _workObject = new WorkObject(); // Default work object wobj0
            _digitalOutput = new DigitalOutput(); // InValid / empty DO
            
            Initialize();
        }

        /// <summary>
        /// Method to create a robot movement with an empty digital output.
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
            _precision = precision;
            _robotTool = robotTool;
            _workObject = workObject;
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
        public Movement(Target target, SpeedData speedData, int movementType, int precision, RobotTool robotTool, WorkObject workObject, DigitalOutput digitalOutput)
        {
            _target = target;
            _speedData = speedData;
            _movementType = movementType;
            _precision = precision;
            _robotTool = robotTool;
            _workObject = workObject;
            _digitalOutput = digitalOutput;

            Initialize();
        }

        /// <summary>
        /// Creates a new movement by duplicating an existing movement. 
        /// This creates a deep copy of the existing movement. 
        /// </summary>
        /// <param name="movement"> The movement that should be duplicated. </param>
        /// <param name="duplicateMesh"> A boolean that indicates if the mesh should be duplicated. </param>
        public Movement(Movement movement, bool duplicateMesh = true)
        {
            _target = movement.Target.Duplicate();
            _speedData = movement.SpeedData.Duplicate();
            _movementType = movement.MovementType;
            _precision = movement.Precision;
            _digitalOutput = movement.DigitalOutput.Duplicate();
            _globalTargetPlane = new Plane(movement.GlobalTargetPlane);

            if (duplicateMesh == true)
            {
                _robotTool = movement.RobotTool.Duplicate();
                _workObject = movement.WorkObject.Duplicate();
            }
            else
            {
                _robotTool = movement.RobotTool.DuplicateWithoutMesh();
                _workObject = movement.WorkObject.DuplicateWithoutMesh();
            }
        }

        /// <summary>
        /// Duplicates a robot movement.
        /// </summary>
        /// <returns> Returns a deep copy of the Movement object. </returns>
        public Movement Duplicate()
        {
            return new Movement(this);
        }

        /// <summary>
        /// Duplicates a robot movement without meshes that are part of the properties.
        /// Such as the robot tool meshes and the meshes of the external axis that can be attached to the work object. 
        /// </summary>
        /// <returns> Returns a deep copy of the Movement object. </returns>
        public Movement DuplicateWithoutMesh()
        {
            return new Movement(this, false);
        }

        /// <summary>
        /// A method to duplicate the Movement object to an Action object. 
        /// </summary>
        /// <returns> Returns a deep copy of the Movement object as an Action object. </returns>
        public override Action DuplicateAction()
        {
            return new Movement(this) as Action;
        }
        #endregion

        #region method
        /// <summary>
        /// A method that calls all the other methods that are needed to initialize the data that is needed to construct a valid movement object. 
        /// </summary>
        private void Initialize()
        {
            _globalTargetPlane = GetGlobalTargetPlane();
        }

        /// <summary>
        /// A method that can be called to reinitialize all the data that is needed to construct a valid movement object.
        /// </summary>
        public void ReInitialize()
        {
            Initialize();
        }

        /// <summary>
        /// Calculates the position and the orientation of the target in the world coordinate system. 
        /// If an external axis is attached to the work object this returns the pose of the 
        /// target plane in the world coorinate system for axis values equal to zero.
        /// </summary>
        /// <returns> The the target plane in the world coordinate system. </returns>
        public Plane GetGlobalTargetPlane()
        {
            // Deep copy the target plane
            Plane plane = new Plane(Target.Plane);

            // Re-orient the target plane to the work object plane
            Transform orient = Transform.PlaneToPlane(Plane.WorldXY, WorkObject.GlobalWorkObjectPlane);
            plane.Transform(orient);

            return plane;
        }

        /// <summary>
        /// Calculates the tranformed global target plane for the defined Robot Info with attached external axes.
        /// </summary>
        /// <param name="robotInfo"> The robot info with the external axes that defined the axis logic. </param>
        /// <param name="logic"> Retuns the axis logic number as an int. </param>
        /// <returns> The posed target plane in the word coordinate system. </returns>
        public Plane GetPosedGlobalTargetPlane(RobotInfo robotInfo, out int logic)
        {
            // Not transformed global target plane
            Plane plane = new Plane(_globalTargetPlane);

            // Initiate axis logic
            int axisLogic = -1; // dummy value

            // Re-orient the target plane if an external axis is attached to the work object
            if (_workObject.ExternalAxis != null)
            {
                // Check if the axis is attached to the robot and get the axis logic number
                axisLogic = robotInfo.ExternalAxis.FindIndex(p => p.Name == _workObject.ExternalAxis.Name);
                ExternalAxis externalAxis = robotInfo.ExternalAxis[axisLogic];

                // Get external axis value
                double axisValue = _target.ExternalAxisValues[axisLogic];
                if (axisValue == 9e9) { axisValue = 0; } // If the user does not define an axis value we set it to zero. 

                // External rotatioanal axis
                if (_workObject.ExternalAxis is ExternalRotationalAxis)
                {
                    // To radians
                    axisValue = (axisValue / 180) * Math.PI;

                    // Rotate
                    Transform rotate = Transform.Rotation(axisValue, externalAxis.AxisPlane.ZAxis, externalAxis.AxisPlane.Origin);
                    plane.Transform(rotate);
                }

                // External linear axis
                if (_workObject.ExternalAxis is ExternalLinearAxis)
                {
                    // Translate
                    Vector3d axis = new Vector3d(externalAxis.AxisPlane.ZAxis);
                    axis.Unitize();
                    Transform translate = Transform.Translation(axis * axisValue);
                    plane.Transform(translate);
                }
            }

            logic = axisLogic;
            return plane;
        }

        /// <summary>
        /// Used to create variable definition code of this action. 
        /// </summary>
        /// <param name="robotInfo"> Defines the Robot Info were the code is generated for. </param>
        /// <returns> Returns the RAPID code line as a string. </returns>
        public override string InitRAPIDVar(RobotInfo robotInfo)
        {
            // Initialize code line
            string code = "";
            
            // Target with global plane (for ik) 
            Target globalTarget = _target.Duplicate();
            globalTarget.Plane = GetPosedGlobalTargetPlane(robotInfo, out int logic);

            // Create a robtarget if  the movement type is MoveL (1) or MoveJ (2)
            if (_movementType == 1 || _movementType == 2)
            {
                // Creates targetName variable
                string robTargetVar = "VAR robtarget " + _target.RobTargetName;

                code += robTargetVar + " := [["
                    + _target.Plane.Origin.X.ToString("0.##") + ", "
                    + _target.Plane.Origin.Y.ToString("0.##") + ", "
                    + _target.Plane.Origin.Z.ToString("0.##") + "], ["
                    + _target.Quat.A.ToString("0.######") + ", "
                    + _target.Quat.B.ToString("0.######") + ", "
                    + _target.Quat.C.ToString("0.######") + ", "
                    + _target.Quat.D.ToString("0.######") + "],"
                    + "[0,0,0," + _target.AxisConfig;

                // Adds all External Axis Values
                InverseKinematics inverseKinematics = new InverseKinematics(globalTarget, robotInfo);
                inverseKinematics.Calculate();
                List<double> externalAxisValues = inverseKinematics.ExternalAxisValues;
                code += "], [";
                for (int i = 0; i < externalAxisValues.Count; i++)
                {
                    code += externalAxisValues[i].ToString("0.##") + ", ";
                }

                // Adds 9E9 for all missing external Axis Values
                for (int i = externalAxisValues.Count; i < 6; i++)
                {
                    if (Target.ExternalAxisValues[i] == 9e9)
                    {
                        code += "9E9" + ", ";
                    }
                    else
                    {
                        code += Target.ExternalAxisValues[i].ToString("0.##") + ", ";
                    }

                }
                code.Remove(code.Length - 2, 2);
                code += "]];";
            }

            // Create a jointtarget if the movement type is MoveAbsJ (0)
            else
            {
                // Creates targetName variable
                string jointTargetVar = "CONST jointtarget " + _target.JointTargetName;
                // Calculates AxisValues
                InverseKinematics inverseKinematics = new InverseKinematics(globalTarget, robotInfo);
                inverseKinematics.Calculate();
                List<double> internalAxisValues = inverseKinematics.InternalAxisValues;
                List<double> externalAxisValues = inverseKinematics.ExternalAxisValues;

                // Creates Code Variable
                code += jointTargetVar + " := [[";

                // Adds all Internal Axis Values
                for (int i = 0; i < internalAxisValues.Count; i++)
                {
                    code += internalAxisValues[i].ToString("0.##") + ", ";
                }
                code.Remove(code.Length - 2, 2);

                // Adds all External Axis Values
                code += "], [";
                for (int i = 0; i < externalAxisValues.Count; i++)
                {
                    code += externalAxisValues[i].ToString("0.##") + ", ";
                }
                // Adds 9E9 for all missing external Axis Values
                for (int i = externalAxisValues.Count; i < 6; i++)
                {
                    if (Target.ExternalAxisValues[i] == 9e9)
                    {
                        code += "9E9" + ", ";
                    }
                    else
                    {
                        code += Target.ExternalAxisValues[i].ToString("0.##") + ", ";
                    }
                }
                code.Remove(code.Length - 2, 2);
                code += "]];";
            }

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
            else { toolName = _robotTool.Name; }

            // Set zone data text (precision value)
            string zoneName;
            if (_precision < 0) { zoneName = ", fine, "; }
            else { zoneName = ", z" + _precision.ToString() + ", "; }

            // Digital output bool
            bool moveDO = false;
            if (_digitalOutput.IsValid == true) { moveDO = true; }

            // A movement not combined with a digital output
            if (moveDO == false)
            {
                // MoveAbsJ
                if (_movementType == 0)
                {
                    return "MoveAbsJ " + _target.JointTargetName + ", " + _speedData.Name + zoneName + toolName + "\\WObj:=" + _workObject.Name + ";";
                }

                // MoveL
                else if (_movementType == 1)
                {
                    return "MoveL " + _target.RobTargetName + ", " + _speedData.Name + zoneName + toolName + "\\WObj:=" + _workObject.Name + ";";
                }

                // MoveJ
                else if (_movementType == 2)
                {
                    return "MoveJ " + _target.RobTargetName + ", " + _speedData.Name + zoneName + toolName + "\\WObj:=" + _workObject.Name + ";";
                }

                // Wrong movement type
                else
                {
                    return string.Empty;
                }
            }

            // A movement combined with a digital output
            else
            {
                // MoveAbsJ + SetDO: There is no RAPDID function that combines the an absolute joint movement and a DO.
                // Therefore, we write two separate RAPID code lines for an aboslute joint momvement combined with a DO. 
                if (_movementType == 0)
                {
                    // Add the code line for the absolute joint movement
                    string code = "MoveAbsJ " + _target.JointTargetName + ", " + _speedData.Name + zoneName + toolName + "\\WObj:=" + _workObject.Name + ";";
                    // Add the code line for the digital output
                    code += " ";
                    code += _digitalOutput.ToRAPIDFunction(robotInfo);

                    return code;
                }

                // MoveLDO
                else if (_movementType == 1)
                {
                    return "MoveLDO " + _target.RobTargetName + ", " + _speedData.Name + zoneName + toolName + "\\WObj:=" + _workObject.Name + ", " + _digitalOutput.Name + ", " + (_digitalOutput.IsActive ? 1 : 0) + ";";
                }

                // MoveJDO
                else if (_movementType == 2)
                {
                    return "MoveJDO " + _target.RobTargetName + ", " + _speedData.Name + zoneName + toolName + "\\WObj:=" + _workObject.Name + ", " + _digitalOutput.Name + ", " + (_digitalOutput.IsActive ? 1 : 0) + ";";
                }

                // Wrong movement type
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Used to create variable definitions in the RAPID Code. It is typically called inside the CreateRAPIDCode() method of the RAPIDGenerator class.
        /// </summary>
        /// <param name="RAPIDGenerator"> Defines the RAPIDGenerator. </param>
        public override void InitRAPIDVar(RAPIDGenerator RAPIDGenerator)
        {
            // Creates SpeedData Variable Code 
            _speedData.InitRAPIDVar(RAPIDGenerator);

            // Target with global plane (for ik) 
            Target globalTarget = _target.Duplicate();
            globalTarget.Plane = GetPosedGlobalTargetPlane(RAPIDGenerator.RobotInfo, out int logic);

            // Create a robtarget if  the movement type is MoveL (1) or MoveJ (2)
            if (_movementType == 1 || _movementType == 2)
            {
                // Only adds target code if target is not already defined
                if (!RAPIDGenerator.Targets.ContainsKey(_target.RobTargetName))
                {
                    // Adds target to RAPIDGenrator targets dictionary
                    RAPIDGenerator.Targets.Add(_target.RobTargetName, _target);
                    // Creates targetName variable
                    string robTargetVar = "VAR robtarget " + _target.RobTargetName;
                    RAPIDGenerator.StringBuilder.Append((Environment.NewLine + "\t" + robTargetVar + " := [["
                        + _target.Plane.Origin.X.ToString("0.##") + ", "
                        + _target.Plane.Origin.Y.ToString("0.##") + ", "
                        + _target.Plane.Origin.Z.ToString("0.##") + "], ["
                        + _target.Quat.A.ToString("0.######") + ", "
                        + _target.Quat.B.ToString("0.######") + ", "
                        + _target.Quat.C.ToString("0.######") + ", "
                        + _target.Quat.D.ToString("0.######") + "],"
                        + "[0,0,0," + _target.AxisConfig));

                    // Adds all External Axis Values
                    RAPIDGenerator.InverseKinematics.Movement.Target = globalTarget;
                    RAPIDGenerator.InverseKinematics.ReInitialize();
                    RAPIDGenerator.InverseKinematics.Calculate();
                    List<double> externalAxisValues = RAPIDGenerator.InverseKinematics.ExternalAxisValues;
                    RAPIDGenerator.StringBuilder.Append("], [");
                    for (int i = 0; i < externalAxisValues.Count; i++)
                    {
                        RAPIDGenerator.StringBuilder.Append(externalAxisValues[i].ToString("0.##") + ", ");
                    }

                    // Adds 9E9 for all missing external Axis Values
                    for (int i = externalAxisValues.Count; i < 6; i++)
                    {
                        if (Target.ExternalAxisValues[i] == 9e9)
                        {
                            RAPIDGenerator.StringBuilder.Append("9E9" + ", ");
                        }
                        else
                        {
                            RAPIDGenerator.StringBuilder.Append(Target.ExternalAxisValues[i].ToString("0.##") + ", ");
                        }

                    }
                    RAPIDGenerator.StringBuilder.Remove(RAPIDGenerator.StringBuilder.Length - 2, 2);
                    RAPIDGenerator.StringBuilder.Append("]];");
                }
            }

            // Create a jointtarget if the movement type is MoveAbsJ (0)
            else
            {
                // Only adds target code if target is not already defined
                if (!RAPIDGenerator.Targets.ContainsKey(_target.JointTargetName))
                {
                    // Adds target to RAPIDGenrator targets dictionary
                    RAPIDGenerator.Targets.Add(_target.JointTargetName, _target);
                    // Creates targetName variable
                    string jointTargetVar = "CONST jointtarget " + _target.JointTargetName;
                    // Calculates AxisValues
                    RAPIDGenerator.InverseKinematics.Movement.Target = globalTarget;
                    RAPIDGenerator.InverseKinematics.ReInitialize();
                    RAPIDGenerator.InverseKinematics.Calculate();
                    List<double> internalAxisValues = RAPIDGenerator.InverseKinematics.InternalAxisValues;
                    List<double> externalAxisValues = RAPIDGenerator.InverseKinematics.ExternalAxisValues;

                    // Creates Code Variable
                    RAPIDGenerator.StringBuilder.Append(Environment.NewLine + "\t" + jointTargetVar + " := [[");

                    // Adds all Internal Axis Values
                    for (int i = 0; i < internalAxisValues.Count; i++)
                    {
                        RAPIDGenerator.StringBuilder.Append(internalAxisValues[i].ToString("0.##") + ", ");
                    }
                    RAPIDGenerator.StringBuilder.Remove(RAPIDGenerator.StringBuilder.Length - 2, 2);

                    // Adds all External Axis Values
                    RAPIDGenerator.StringBuilder.Append("], [");
                    for (int i = 0; i < externalAxisValues.Count; i++)
                    {
                        RAPIDGenerator.StringBuilder.Append(externalAxisValues[i].ToString("0.##") + ", ");
                    }
                    // Adds 9E9 for all missing external Axis Values
                    for (int i = externalAxisValues.Count; i < 6; i++)
                    {
                        if (Target.ExternalAxisValues[i] == 9e9)
                        {
                            RAPIDGenerator.StringBuilder.Append("9E9" + ", ");
                        }
                        else
                        {
                            RAPIDGenerator.StringBuilder.Append(Target.ExternalAxisValues[i].ToString("0.##") + ", ");
                        }
                    }
                    RAPIDGenerator.StringBuilder.Remove(RAPIDGenerator.StringBuilder.Length - 2, 2);
                    RAPIDGenerator.StringBuilder.Append("]];");
                }
            }
        }

        /// <summary>
        /// Used to create action instructions in the RAPID Code. It is typically called inside the CreateRAPIDCode() method of the RAPIDGenerator class.
        /// </summary>
        /// <param name="RAPIDGenerator"> Defines the RAPIDGenerator. </param>
        public override void ToRAPIDFunction(RAPIDGenerator RAPIDGenerator)
        {
            // Set tool name
            string toolName;

            // Check first if a tool is set
            if (_robotTool == null) { toolName = RAPIDGenerator.RobotInfo.Tool.Name; }
            // Check if a tool is set by checking the name (tool can be empty)
            else if (_robotTool.Name == "" || _robotTool.Name == null) { toolName = RAPIDGenerator.RobotInfo.Tool.Name; }
            // Otherwise don't set a tool. Last overwrite is used that is combined with the movement.
            else { toolName = _robotTool.Name; }

            // Set zone data text (precision value)
            string zoneName;
            if (_precision < 0) { zoneName = ", fine, "; }
            else { zoneName = ", z" + _precision.ToString() + ", "; }

            // Digital output bool
            bool moveDO = false;
            if (_digitalOutput.IsValid == true) { moveDO = true; }

            // A movement not combined with a digital output
            if (moveDO == false)
            {
                // MoveAbsJ
                if (_movementType == 0)
                {
                    RAPIDGenerator.StringBuilder.Append(Environment.NewLine + "\t\t" + "MoveAbsJ " + _target.JointTargetName + ", " + _speedData.Name + zoneName + toolName + "\\WObj:=" + _workObject.Name + ";");
                }

                // MoveL
                else if (_movementType == 1)
                {
                    RAPIDGenerator.StringBuilder.Append(Environment.NewLine + "\t\t" + "MoveL " + _target.RobTargetName + ", " + _speedData.Name + zoneName + toolName + "\\WObj:=" + _workObject.Name + ";");
                }

                // MoveJ
                else if (_movementType == 2)
                {
                    RAPIDGenerator.StringBuilder.Append(Environment.NewLine + "\t\t" + "MoveJ " + _target.RobTargetName + ", " + _speedData.Name + zoneName + toolName + "\\WObj:=" + _workObject.Name + ";");
                }
            }

            // A movement combined with a digital output
            else
            {
                // MoveAbsJ + SetDO: There is no RAPDID function that combines the an absolute joint movement and a DO.
                // Therefore, we write two separate RAPID code lines for an aboslute joint momvement combined with a DO. 
                if (_movementType == 0)
                {
                    // Add the code line for the absolute joint movement
                    RAPIDGenerator.StringBuilder.Append(Environment.NewLine + "\t\t" + "MoveAbsJ " + _target.JointTargetName + ", " + _speedData.Name + zoneName + toolName + "\\WObj:=" + _workObject.Name + ";");
                    // Add the code line for the digital output
                    _digitalOutput.ToRAPIDFunction(RAPIDGenerator);
                }

                // MoveLDO
                else if (_movementType == 1)
                {
                    RAPIDGenerator.StringBuilder.Append(Environment.NewLine + "\t\t" + "MoveLDO " + _target.RobTargetName + ", " + _speedData.Name + zoneName + toolName + "\\WObj:=" + _workObject.Name + ", " + _digitalOutput.Name + ", " + (_digitalOutput.IsActive ? 1 : 0) + ";");
                }

                // MoveJDO
                else if (_movementType == 2)
                {
                    RAPIDGenerator.StringBuilder.Append(Environment.NewLine + "\t\t" + "MoveJDO " + _target.RobTargetName + ", " + _speedData.Name + zoneName + toolName + "\\WObj:=" + _workObject.Name + ", " + _digitalOutput.Name + ", " + (_digitalOutput.IsActive ? 1 : 0) + ";");
                }

            }
        }
        #endregion

        #region properties
        /// <summary>
        /// A boolean that indicuate if the Movement object is valid.
        /// </summary>
        public override bool IsValid
        {
            get
            {
                if (Target == null) { return false; }
                if (SpeedData == null) { return false; }
                if (WorkObject == null) { return false;  }
                return true;
            }
        }

        /// <summary>
        /// The destination target of the robot and external axes.
        /// </summary>
        public Target Target
        {
            get 
            { 
                return _target; 
            }
            set 
            { 
                _target = value;
                ReInitialize();
            }
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
        /// Two is used for linear movements with robtarget (MoveL)
        /// Three is used for joint movements with robtargets (MoveJ).
        /// </summary>
        public int MovementType
        {
            get { return _movementType; }
            set { _movementType = value; }
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
        /// The position and the orientation of the used target in the global coordinate system.
        /// In case an external axis is connected to the work object this the position of the 
        /// target plane if the external axis values are zero. 
        /// </summary>
        public Plane GlobalTargetPlane
        {
            get { return _globalTargetPlane; }
        }

        /// <summary>
        /// The tool in use when the robot moves. 
        /// </summary>
        public RobotTool RobotTool
        {
            get { return _robotTool; }
            set { _robotTool = value; }
        }

        /// <summary>
        /// The work object (coordinate system) to which the robot position in the instruction is related.
        /// </summary>
        public WorkObject WorkObject
        {
            get 
            { 
                return _workObject; 
            }
            set 
            { 
                _workObject = value;
                ReInitialize();
            }
        }

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

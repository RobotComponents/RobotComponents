// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.ABB.Definitions;
using RobotComponents.ABB.Enumerations;
using RobotComponents.ABB.Actions.Interfaces;
using RobotComponents.ABB.Actions.Declarations;

namespace RobotComponents.ABB.Actions.Instructions
{
    /// <summary>
    /// Represents several Move instructions (MoveAbsJ, MoveL, MoveJ, MoveC, MoveLDO, MoveJDO and MoveC). 
    /// </summary>
    [Serializable()]
    public class Movement : Action, IInstruction, ISerializable
    {
        #region fields
        private MovementType _movementType;
        private RobotTarget _cirPoint;
        private ITarget _target;
        private int _id;
        private SpeedData _speedData;
        private double _time;
        private ZoneData _zoneData;
        private RobotTool _robotTool;
        private WorkObject _workObject;
        private SetDigitalOutput _setDigitalOutput;

        // For RAPID generator
        private ITarget _convertedTarget;
        #endregion

        #region (de)serialization
        /// <summary>
        /// Protected constructor needed for deserialization of the object.  
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization. </param>
        protected Movement(SerializationInfo info, StreamingContext context)
        {
            int version = (int)info.GetValue("Version", typeof(int));
            _movementType = (MovementType)info.GetValue("Movement Type", typeof(MovementType));
            _cirPoint = version >= 2001000 ? (RobotTarget)info.GetValue("Circle Point", typeof(RobotTarget)) : new RobotTarget();
            _target = (ITarget)info.GetValue("Target", typeof(ITarget));
            _id = (int)info.GetValue("ID", typeof(int));
            _speedData = (SpeedData)info.GetValue("Speed Data", typeof(SpeedData));
            _time = version >= 1004000 ? (double)info.GetValue("Time", typeof(double)) : -1;
            _zoneData = (ZoneData)info.GetValue("Zone Data", typeof(ZoneData));
            _robotTool = (RobotTool)info.GetValue("Robot Tool", typeof(RobotTool));
            _workObject = (WorkObject)info.GetValue("Work Object", typeof(WorkObject));

            if (version >= 2001000)
            {
                _setDigitalOutput = (SetDigitalOutput)info.GetValue("Set Digital Output", typeof(SetDigitalOutput));
            }
            else
            {
                DigitalOutput digitalOutput = (DigitalOutput)info.GetValue("Digital Output", typeof(DigitalOutput));
                _setDigitalOutput = new SetDigitalOutput(digitalOutput.Name, digitalOutput.IsActive);
            }
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
            info.AddValue("Movement Type", _movementType, typeof(MovementType));
            info.AddValue("Circle Point", _cirPoint, typeof(RobotTarget));
            info.AddValue("Target", _target, typeof(ITarget));
            info.AddValue("ID", _id, typeof(int));
            info.AddValue("Speed Data", _speedData, typeof(SpeedData));
            info.AddValue("Zone Data", _zoneData, typeof(ZoneData));
            info.AddValue("Time", _time, typeof(double));
            info.AddValue("Robot Tool", _robotTool, typeof(RobotTool));
            info.AddValue("Work Object", _workObject, typeof(WorkObject));
            info.AddValue("Set Digital Output", _setDigitalOutput, typeof(SetDigitalOutput));
        }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the Movement class.
        /// </summary>
        public Movement()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Movement class.
        /// </summary>
        /// <remarks>
        /// This constructor is typically used to cast a Plane to a Movement. 
        /// </remarks>
        /// <param name="plane"> The target plane. </param>
        public Movement(Plane plane)
        {
            _movementType = MovementType.MoveJ;
            _cirPoint = new RobotTarget(Plane.Unset);
            _target = new RobotTarget(plane);
            _id = -1;
            _speedData = new SpeedData(5); // Slowest predefined tcp speed
            _time = -1;
            _zoneData = new ZoneData(0);
            _robotTool = RobotTool.GetEmptyRobotTool(); // Empty Robot Tool
            _workObject = new WorkObject(); // Default work object wobj0
            _setDigitalOutput = new SetDigitalOutput(); // InValid / empty DO
        }

        /// <summary>
        /// Initializes a new instance of the Movement class.
        /// </summary>
        /// <remarks>
        /// This constructor is typically used to cast a Robot Target to a Movement.
        /// </remarks>
        /// <param name="target"> The Target. </param>
        public Movement(ITarget target)
        {
            _movementType = target is JointTarget ? MovementType.MoveAbsJ : MovementType.MoveJ;
            _cirPoint = new RobotTarget(Plane.Unset);
            _target = target;
            _id = -1;
            _speedData = new SpeedData(5); // Slowest predefined tcp speed
            _time = -1;
            _zoneData = new ZoneData(0);
            _robotTool = RobotTool.GetEmptyRobotTool(); // Empty Robot Tool
            _workObject = new WorkObject(); // Default work object wobj0
            _setDigitalOutput = new SetDigitalOutput(); // InValid / empty DO
        }

        /// <summary>
        /// Initializes a new instance of the Movement class with an empty Robot Tool (no override), a default Work Object (wobj0) and an empty Digital Output. 
        /// </summary>
        /// <param name="movementType"> The Movmement Type. </param>
        /// <param name="target"> The Target. </param>
        /// <param name="speedData"> The Speed Data. </param>
        public Movement(MovementType movementType, ITarget target, SpeedData speedData)
        {
            _movementType = movementType;
            _cirPoint = new RobotTarget(Plane.Unset);
            _target = target;
            _id = -1;
            _speedData = speedData;
            _time = -1;
            _zoneData = new ZoneData(0);
            _robotTool = RobotTool.GetEmptyRobotTool(); // Empty Robot Tool
            _workObject = new WorkObject(); // Default work object wobj0
            _setDigitalOutput = new SetDigitalOutput(); // InValid / empty DO
            CheckCombination();
        }

        /// <summary>
        /// Initializes a new instance of the Movement class with an empty Robot Tool (no override), a default Work Object (wobj0) and an empty Digital Output. 
        /// </summary>
        /// <param name="movementType"> The Movement Type. </param>
        /// <param name="target"> The Target. </param>
        /// <param name="speedData"> The Speed Data. </param>
        /// <param name="zoneData"> The Zone Data. </param>
        public Movement(MovementType movementType, ITarget target, SpeedData speedData, ZoneData zoneData)
        {
            _movementType = movementType;
            _cirPoint = new RobotTarget(Plane.Unset);
            _target = target;
            _id = -1;
            _speedData = speedData;
            _time = -1;
            _zoneData = zoneData;
            _robotTool = RobotTool.GetEmptyRobotTool(); // Empty Robot Tool
            _workObject = new WorkObject(); // Default work object wobj0
            _setDigitalOutput = new SetDigitalOutput(); // InValid / empty DO
            CheckCombination();
        }

        /// <summary>
        /// Initializes a new instance of the Movement class with a default Work object (wobj0) and an empty Digital Output.
        /// </summary>
        /// <param name="movementType"> The Movement Type. </param>
        /// <param name="target"> The Target. </param>
        /// <param name="speedData"> The Speed Data. </param>
        /// <param name="zoneData"> The Zone Data. </param>
        /// <param name="robotTool"> The Robot Tool. This will override the set default tool. </param>
        public Movement(MovementType movementType, ITarget target, SpeedData speedData, ZoneData zoneData, RobotTool robotTool)
        {
            _movementType = movementType;
            _cirPoint = new RobotTarget(Plane.Unset);
            _target = target;
            _id = -1;
            _speedData = speedData;
            _time = -1;
            _zoneData = zoneData;
            _robotTool = robotTool;
            _workObject = new WorkObject(); // Default work object wobj0
            _setDigitalOutput = new SetDigitalOutput(); // InValid / empty DO
            CheckCombination();
        }

        /// <summary>
        /// Initializes a new instance of the Movement class with an empty Robot Tool (no override) and an empty Digital Output.
        /// </summary>
        /// <param name="movementType"> The Movement Type. </param>
        /// <param name="target"> The Target. </param>
        /// <param name="speedData"> The Speed Data. </param>
        /// <param name="zoneData"> The Zone Data. </param>
        /// <param name="workObject"> The Work Object. </param>
        public Movement(MovementType movementType, ITarget target, SpeedData speedData, ZoneData zoneData, WorkObject workObject)
        {
            _movementType = movementType;
            _cirPoint = new RobotTarget(Plane.Unset);
            _target = target;
            _id = -1;
            _speedData = speedData;
            _time = -1;
            _zoneData = zoneData;
            _robotTool = RobotTool.GetEmptyRobotTool(); // Empty Robot Tool
            _workObject = workObject;
            _setDigitalOutput = new SetDigitalOutput(); // InValid / empty DO
            CheckCombination();
        }

        /// <summary>
        /// Initializes a new instance of the Movement class with an empty Digital Output.
        /// </summary>
        /// <param name="movementType"> The Movement Type. </param>
        /// <param name="target"> The Taret. </param>
        /// <param name="speedData"> The Speed Data. </param>
        /// <param name="zoneData"> The Zone Data. </param>
        /// <param name="robotTool"> The Robot Tool. This will override the set default tool. </param>
        /// <param name="workObject"> The Work Object. </param>
        public Movement(MovementType movementType, ITarget target, SpeedData speedData, ZoneData zoneData, RobotTool robotTool, WorkObject workObject)
        {
            _movementType = movementType;
            _cirPoint = new RobotTarget(Plane.Unset);
            _target = target;
            _id = -1;
            _speedData = speedData;
            _time = -1;
            _zoneData = zoneData;
            _robotTool = robotTool;
            _workObject = workObject;
            _setDigitalOutput = new SetDigitalOutput(); // InValid / empty DO
            CheckCombination();
        }

        /// <summary>
        /// Initializes a new instance of the Movement class with an empty Robot Tool (no override) and a default Work Object (wobj0)
        /// </summary>
        /// <param name="movementType"> The Movement Type. </param>
        /// <param name="target"> The Target. </param>
        /// <param name="speedData"> The Speed Data.</param>
        /// <param name="zoneData"> The Zone Data. </param>
        /// <param name="setDigitalOutput"> The Digital Output. When set this will define a MoveLDO, MoveJDO or MoveCDO instruction. </param>
        public Movement(MovementType movementType, ITarget target, SpeedData speedData, ZoneData zoneData, SetDigitalOutput setDigitalOutput)
        {
            _movementType = movementType;
            _cirPoint = new RobotTarget(Plane.Unset);
            _target = target;
            _id = -1;
            _speedData = speedData;
            _time = -1;
            _zoneData = zoneData;
            _robotTool = RobotTool.GetEmptyRobotTool(); // Empty Robot Tool
            _workObject = new WorkObject(); // Default work object wobj0
            _setDigitalOutput = setDigitalOutput;
            CheckCombination();
        }

        /// <summary>
        /// Initializes a new instance of the Movement class with a default Work Object (wobj0). 
        /// </summary>
        /// <param name="movementType"> The Movement Type. </param>
        /// <param name="target"> The Target. </param>
        /// <param name="speedData"> The Speed Data. </param>
        /// <param name="zoneData"> The Zone Data. </param>
        /// <param name="robotTool"> The Robot Tool. This will override the set default tool. </param>
        /// <param name="setDigitalOutput"> The Digital Output. When set this will define a MoveLDO, MoveJDO or MoveCDO instruction. </param>
        public Movement(MovementType movementType, ITarget target, SpeedData speedData, ZoneData zoneData, RobotTool robotTool, SetDigitalOutput setDigitalOutput)
        {
            _movementType = movementType;
            _cirPoint = new RobotTarget(Plane.Unset);
            _target = target;
            _id = -1;
            _speedData = speedData;
            _time = -1;
            _zoneData = zoneData;
            _robotTool = robotTool;
            _workObject = new WorkObject(); // Default work object wobj0
            _setDigitalOutput = setDigitalOutput;
            CheckCombination();
        }

        /// <summary>
        /// Initializes a new instance of the Movement class.
        /// </summary>
        /// <param name="movementType"> The Movement Type. </param>
        /// <param name="target"> The Target. </param>
        /// <param name="speedData"> The Speed Data. </param>
        /// <param name="zoneData"> The Zone Data. </param>
        /// <param name="robotTool"> The Robot Tool. This will override the set default tool. </param>
        /// <param name="workObject"> The Work Object. </param>
        /// <param name="setDigitalOutput"> The Digital Output. When set this will define a MoveLDO, MoveJDO or MoveCDO instruction. </param>
        public Movement(MovementType movementType, ITarget target, SpeedData speedData, ZoneData zoneData, RobotTool robotTool, WorkObject workObject, SetDigitalOutput setDigitalOutput)
        {
            _movementType = movementType;
            _cirPoint = new RobotTarget(Plane.Unset);
            _target = target;
            _id = -1;
            _speedData = speedData;
            _time = -1;
            _zoneData = zoneData;
            _robotTool = robotTool;
            _workObject = workObject;
            _setDigitalOutput = setDigitalOutput;
            CheckCombination();
        }

        /// <summary>
        /// Initializes a new instance of the Movement class by duplicating an existing Movement instance. 
        /// </summary>
        /// <param name="movement"> The Movement instance to duplicate. </param>
        /// <param name="duplicateMesh"> Specifies whether the meshes should be duplicated. </param>
        public Movement(Movement movement, bool duplicateMesh = true)
        {
            _movementType = movement.MovementType;
            _cirPoint = movement.CircularPoint.Duplicate();
            _target = movement.Target.DuplicateTarget();
            _id = movement.SyncID;
            _speedData = movement.SpeedData.Duplicate();
            _time = movement.Time;
            _zoneData = movement.ZoneData.Duplicate();
            _setDigitalOutput = movement.SetDigitalOutput.Duplicate();
            _target = _target.DuplicateTarget();

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
        /// Returns an exact duplicate of this Movement instance.
        /// </summary>
        /// <returns> 
        /// A deep copy of the Movement instance. 
        /// </returns>
        public Movement Duplicate()
        {
            return new Movement(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Movement instance as IInstruction.
        /// </summary>
        /// <returns>
        /// A deep copy of the Movement instance as an IInstruction. 
        /// </returns>
        public IInstruction DuplicateInstruction()
        {
            return new Movement(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Movement instance without meshes.
        /// </summary>
        /// <returns> 
        /// A deep copy of the Movement instance. 
        /// </returns>
        public Movement DuplicateWithoutMesh()
        {
            return new Movement(this, false);
        }

        /// <summary>
        /// Returns an exact duplicate of this Movement instance as an Action. 
        /// </summary>
        /// <returns> 
        /// A deep copy of the Movement instance as an Action. 
        /// </returns>
        public override Action DuplicateAction()
        {
            return new Movement(this);
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
                return "Invalid Movement";
            }

            else if (_id < 0)
            {
                if (_movementType == MovementType.MoveAbsJ)
                {
                    return $"Absolute Joint Movement ({_target.Name}\\{_workObject.Name})";
                }
                else if (_movementType == MovementType.MoveL)
                {
                    return $"Linear Movement ({_target.Name}\\{_workObject.Name})";
                }
                else if (_movementType == MovementType.MoveJ)
                {
                    return $"Joint Movement ({_target.Name}\\{_workObject.Name})";
                }
                else if (_movementType == MovementType.MoveC)
                {
                    return $"Circular Movement ({_target.Name}\\{_workObject.Name})";
                }
                else
                {
                    return "Movement";
                }
            }

            else
            {
                if (_movementType == MovementType.MoveAbsJ)
                {
                    return $"Coordinated synchronized Absolute Joint Movement ({_target.Name}\\{_workObject.Name})";
                }
                else if (_movementType == MovementType.MoveL)
                {
                    return $"Coordinated synchronized Linear Movement ({_target.Name}\\{_workObject.Name})";
                }
                else if (_movementType == MovementType.MoveJ)
                {
                return $"Coordinated synchronized Joint Movement ({_target.Name}\\{_workObject.Name})";
                }
                else if (_movementType == MovementType.MoveC)
                {
                    return $"Coordinated synchronized Circular Movement ({_target.Name}\\{_workObject.Name})";
                }
                else
                {
                    return "Coordinated synchronized Movement";
                }
            }
        }

        /// <summary>
        /// Checks the combination between the movement type and the target type.
        /// </summary>
        /// <remarks>
        /// Throws an exception if the combination is not valid. 
        /// </remarks>
        private void CheckCombination()
        {
            if (_movementType != MovementType.MoveAbsJ && _target is JointTarget)
            {
                throw new InvalidOperationException("Invalid Move instruction: A Joint Target cannot be combined with a MoveL, MoveJ or MoveC instruction.");
            }
        }

        /// <summary>
        /// Calculates the position and the orientation of the target in world coordinate space. 
        /// </summary>
        /// <remarks>
        /// If an external axis is attached to the work object this method returns the pose of the 
        /// target plane in the world coorinate space for axis values equal to zero.
        /// </remarks>
        /// <returns> 
        /// The the target plane in world coordinate space. 
        /// </returns>
        public Plane GetGlobalTargetPlane()
        {
            if (_target is RobotTarget robotTarget)
            {
                Plane plane = new Plane(robotTarget.Plane); // Deep copy
                Transform orient = Transform.PlaneToPlane(Plane.WorldXY, WorkObject.GlobalWorkObjectPlane);
                plane.Transform(orient);
                return plane;
            }

            else
            {
                return Plane.Unset;
            }
        } 

        /// <summary>
        /// Calculates the posed target plane for the defined Robot with attached external axes in world coordinate space.
        /// </summary>
        /// <returns> 
        /// The posed target plane in world coordinate space. 
        /// </returns>
        public Plane GetPosedGlobalTargetPlane()
        {
            if (_target is RobotTarget robotTarget)
            {
                // Not transformed global target plane
                Plane plane = GetGlobalTargetPlane();

                // Re-orient the target plane if an external axis is attached to the work object
                if (_workObject.ExternalAxis != null)
                {
                    Transform transform = _workObject.ExternalAxis.CalculateTransformationMatrix(robotTarget.ExternalJointPosition, out _);
                    plane.Transform(transform);
                }

                return plane;
            }

            else
            {
                return Plane.Unset;
            }
        }

        /// <summary>
        /// Converts the target for the RAPID generator.
        /// </summary>
        /// <param name="RAPIDGenerator"> The RAPID Generator. </param>
        private void ConvertTarget(RAPIDGenerator RAPIDGenerator)
        {
            // Duplicate original target
            _convertedTarget = _target.DuplicateTarget();

            // Robot target
            if (_convertedTarget is RobotTarget robotTarget)
            {
                // Duplicate original target
                _convertedTarget = _target.DuplicateTarget();

                // Update the movement of the inverse kinematics
                RAPIDGenerator.Robot.InverseKinematics.Movement = this;

                // Convert the robot target to a joint target
                if (_movementType == MovementType.MoveAbsJ)
                {
                    // Calculate the axis values from the robot target
                    RAPIDGenerator.Robot.InverseKinematics.Calculate();
                    RAPIDGenerator.ErrorText.AddRange(new List<string>(RAPIDGenerator.Robot.InverseKinematics.ErrorText));

                    // Create a joint target from the axis values
                    _convertedTarget = new JointTarget(robotTarget.Name, RAPIDGenerator.Robot.InverseKinematics.RobotJointPosition.Duplicate(), RAPIDGenerator.Robot.InverseKinematics.ExternalJointPosition.Duplicate());
                    _convertedTarget.ExternalJointPosition.Name = _target.ExternalJointPosition.Name;
                    _convertedTarget.VariableType = _target.VariableType;

                    if (_convertedTarget.Name != "")
                    {
                        _convertedTarget.Name += "_jt";
                    }
                }

                // Calculate the external joint position for the robot target
                else
                {
                    RAPIDGenerator.Robot.InverseKinematics.CalculateExternalJointPosition();
                    _convertedTarget.ExternalJointPosition = RAPIDGenerator.Robot.InverseKinematics.ExternalJointPosition.Duplicate();
                    _convertedTarget.ExternalJointPosition.Name = _target.ExternalJointPosition.Name;
                    _convertedTarget.Name = robotTarget.Name;
                    _convertedTarget.VariableType = _target.VariableType;
                }
            }

            // Joint target
            else if (_convertedTarget is JointTarget jointTarget)
            {
                RAPIDGenerator.ErrorText.AddRange(jointTarget.CheckAxisLimits(RAPIDGenerator.Robot));
            }
        }

        /// <summary>
        /// Returns the RAPID declaration code line of the this action.
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> 
        /// An empty string. 
        /// </returns>
        public override string ToRAPIDDeclaration(Robot robot)
        {
            return string.Empty;
        }

        /// <summary>
        /// Returns the RAPID instruction code line of the this action. 
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> 
        /// The RAPID code line. 
        /// </returns>
        public override string ToRAPIDInstruction(Robot robot)
        {
            // Set tool name
            string toolName;

            // Check first if a tool is set
            if (_robotTool == null || _robotTool.Name == null ||_robotTool.Name == "") 
            { 
                toolName = robot.Tool.Name; 
            }
            // Otherwise: Last overwrite is used that is combined with the movement.
            else 
            { 
                toolName = _robotTool.Name; 
            }

            // Check the movement and target type
            if (_target is JointTarget & _movementType != MovementType.MoveAbsJ)
            {
                throw new InvalidOperationException("Invalid Move instruction: A Joint Target cannot be combined with a MoveLm MoveJ or MoveC instruction.");
            }

            // Check circular point
            if (_movementType == MovementType.MoveC && _cirPoint.Plane == Plane.Unset)
            {
                throw new Exception("The circular point for the MoveC instruction is not defined.");
            }

            // Declaration RAPID code
            string toPoint = _movementType == MovementType.MoveC ? (_cirPoint.Name != "" ? _cirPoint.Name : _cirPoint.ToRAPID()) : "";
            string target = _convertedTarget.Name != "" ? _convertedTarget.Name : _convertedTarget.ToRAPID();
            string speedData = _speedData.Name != "" ? _speedData.Name : _speedData.ToRAPID();
            string zoneData = _zoneData.Name != "" ? _zoneData.Name : _zoneData.ToRAPID();
            target += _id > -1 ? string.Format("\\ID:={0}", _id) : "";
            speedData += _time > 0 ? string.Format("\\T:={0}", _time) : "";

            // A movement not combined with a digital output
            if (_setDigitalOutput == null || _setDigitalOutput.IsValid == false)
            {
                if (_movementType == MovementType.MoveC)
                {
                    string code = $"{Enum.GetName(typeof(MovementType), _movementType)} {toPoint}, ";
                    code += $"{target}, {speedData}, {zoneData}, {toolName}\\WObj:={_workObject.Name};";
                    return code;
                }
                else
                {
                    string code = $"{Enum.GetName(typeof(MovementType), _movementType)} ";
                    code += $"{target}, {speedData}, {zoneData}, {toolName}\\WObj:={_workObject.Name};";
                    return code;
                }
            }

            // A movement combined with a digital output
            else
            {
                // MoveAbsJ + SetDO: There is no RAPID function that combines the an absolute joint movement and a DO.
                // Therefore, we write two separate RAPID code lines for an aboslute joint momvement combined with a DO. 
                if (_movementType == MovementType.MoveAbsJ)
                {
                    string code = $"{Enum.GetName(typeof(MovementType), _movementType)} ";
                    code += $"{target}, {speedData}, {zoneData}, {toolName}\\WObj:={_workObject.Name}; ";
                    code += _setDigitalOutput.ToRAPIDInstruction(robot);
                    return code;
                }

                // MoveCDO
                else if (_movementType == MovementType.MoveC)
                {
                    string code = $"{Enum.GetName(typeof(MovementType), _movementType)}DO {toPoint}, ";
                    code += $"{target}, {speedData}, {zoneData}, {toolName}\\WObj:={_workObject.Name}, ";
                    code += $"{_setDigitalOutput.Name}, {(_setDigitalOutput.Value ? 1 : 0)};";
                    return code;
                }

                // MoveLDO and MoveJDO
                else
                {
                    string code = $"{Enum.GetName(typeof(MovementType), _movementType)}DO ";
                    code += $"{target}, {speedData}, {zoneData}, {toolName}\\WObj:={_workObject.Name}, ";
                    code += $"{_setDigitalOutput.Name}, {(_setDigitalOutput.Value ? 1 : 0)};";
                    return code;
                }
            }
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
            ConvertTarget(RAPIDGenerator);

            _convertedTarget.ToRAPIDDeclaration(RAPIDGenerator);
            _speedData.ToRAPIDDeclaration(RAPIDGenerator);
            _zoneData.ToRAPIDDeclaration(RAPIDGenerator);

            if (_movementType == MovementType.MoveC)
            {
                _cirPoint.ToRAPIDDeclaration(RAPIDGenerator);
            }
        }

        /// <summary>
        /// Creates instructions in the RAPID program module inside the RAPID Generator.
        /// </summary>
        /// <remarks>
        /// This method is called inside the RAPID generator.s
        /// </remarks>
        /// <param name="RAPIDGenerator"> The RAPID Generator. </param>
        public override void ToRAPIDInstruction(RAPIDGenerator RAPIDGenerator)
        {
            RAPIDGenerator.ProgramInstructions.Add("    " + "    " + ToRAPIDInstruction(RAPIDGenerator.Robot));

            // Collect unique loaddatas
            if (!RAPIDGenerator.LoadDatas.ContainsKey(_robotTool.LoadData.Name))
            {
                RAPIDGenerator.LoadDatas.Add(_robotTool.LoadData.Name, _robotTool.LoadData);
            }

            // Collect unique robot tools
            if (!RAPIDGenerator.RobotTools.ContainsKey(_robotTool.Name))
            {
                RAPIDGenerator.RobotTools.Add(_robotTool.Name, _robotTool);
            }

            // Collect unique work objects
            if (!RAPIDGenerator.WorkObjects.ContainsKey(_workObject.Name))
            {
                RAPIDGenerator.WorkObjects.Add(_workObject.Name, _workObject);
            }
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
                if (_target == null) { return false; }
                if (_target.IsValid == false) { return false; }
                if (_speedData == null) { return false; }
                if (_speedData.IsValid == false) { return false; }
                if (_zoneData == null) { return false; }
                if (_zoneData.IsValid == false) { return false; }
                if (_workObject == null) { return false;  }
                if (_workObject.IsValid == false) { return false; }
                if (_target is JointTarget && MovementType == MovementType.MoveL) { return false; }
                if (_target is JointTarget && MovementType == MovementType.MoveJ) { return false; }
                return true;
            }
        }

        /// <summary>
        /// Gets or sets the Movement Type.
        /// </summary>
        public MovementType MovementType
        {
            get { return _movementType; }
            set { _movementType = value; }
        }

        /// <summary>
        /// Gets or sets the circular point as a Robot Target. 
        /// </summary>
        /// <remarks>
        /// Defines the circular point for a MoveC instruction.
        /// Positions of external axes are ignored. 
        /// </remarks>
        public RobotTarget CircularPoint
        {
            get { return _cirPoint; }
            set { _cirPoint = value; }
        }

        /// <summary>
        /// Gets or sets the Target.
        /// </summary>
        /// <remarks>
        /// Defines the destination target of the robot and external axes.
        /// </remarks>
        public ITarget Target
        {
            get { return _target; }
            set { _target = value; }
        }

        /// <summary>
        /// Gets or sets the synchronization id for multi move programming. 
        /// </summary>
        /// <remarks>
        /// This ID number must be defined for coordinated synchronized movements in multi move systems. 
        /// Set this property to -1 to define normal movements (not coordinated / not synchronized).
        /// </remarks>
        public int SyncID
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// Gets or sets the Speed Data. 
        /// </summary>
        public SpeedData SpeedData
        {
            get { return _speedData; }
            set { _speedData = value; }
        }

        /// <summary>
        /// Gets the the total time which the robot will move in seconds.  
        /// </summary>
        /// <remarks>
        /// This overwrites the defined speeddata value.
        /// Set this property to a negative value to not overwrite the speeddata value.
        /// </remarks>
        public double Time
        {
            get { return _time; }
            set { _time = value; }
        }

        /// <summary>
        /// Gets or sets the Zone Data.
        /// </summary>
        public ZoneData ZoneData
        {
            get { return _zoneData; }
            set { _zoneData = value; }
        }

        /// <summary>
        /// Gets or sets the Robot Tool.
        /// </summary>
        /// <remarks>
        /// If an empty or no Robot Tool is used, the Robot Tool set at the Robot will be used. 
        /// </remarks>
        public RobotTool RobotTool
        {
            get { return _robotTool; }
            set { _robotTool = value; }
        }

        /// <summary>
        /// Gets or sets the Work Object. 
        /// </summary>
        public WorkObject WorkObject
        {
            get { return _workObject; }
            set { _workObject = value; }
        }

        /// <summary>
        /// Gets or sets the Digital Output. 
        /// </summary>
        /// <remarks>
        /// If an empty or invalid Digital Output is set a normal movement will be set (MoveAbsJ, MoveL or MoveJ). 
        /// If a valid Digital Output is combined movement will be created (MoveLDO or MoveJDO). 
        /// If as Movement Type an MoveAbsJ is set an extra RAPID code line will be added that sets the Digital Output (SetDO).
        /// </remarks>
        public SetDigitalOutput SetDigitalOutput
        {
            get { return _setDigitalOutput; }
            set { _setDigitalOutput = value; }
        }
        #endregion

        #region obsolete
        /// <summary>
        /// Initializes a new instance of the Movement class with an empty Robot Tool (no override) and a default Work Object (wobj0)
        /// </summary>
        /// <param name="movementType"> The Movement Type. </param>
        /// <param name="target"> The Target. </param>
        /// <param name="speedData"> The Speed Data.</param>
        /// <param name="zoneData"> The Zone Data. </param>
        /// <param name="digitalOutput"> The Digital Output. When set this will define a MoveLDO or a MoveJDO instruction. </param>
        [Obsolete("This constructor is obsolete and will be removed in v3.", false)]
        public Movement(MovementType movementType, ITarget target, SpeedData speedData, ZoneData zoneData, DigitalOutput digitalOutput)
        {
            _movementType = movementType;
            _cirPoint = new RobotTarget(Plane.Unset);
            _target = target;
            _id = -1;
            _speedData = speedData;
            _time = -1;
            _zoneData = zoneData;
            _robotTool = RobotTool.GetEmptyRobotTool(); // Empty Robot Tool
            _workObject = new WorkObject(); // Default work object wobj0
            _setDigitalOutput = new SetDigitalOutput(digitalOutput.Name, digitalOutput.IsActive);
            CheckCombination();
        }

        /// <summary>
        /// Initializes a new instance of the Movement class with a default Work Object (wobj0). 
        /// </summary>
        /// <param name="movementType"> The Movement Type. </param>
        /// <param name="target"> The Target. </param>
        /// <param name="speedData"> The Speed Data. </param>
        /// <param name="zoneData"> The Zone Data. </param>
        /// <param name="robotTool"> The Robot Tool. This will override the set default tool. </param>
        /// <param name="digitalOutput"> The Digital Output. When set this will define a MoveLDO or a MoveJDO instruction. </param>
        [Obsolete("This constructor is obsolete and will be removed in v3.", false)]
        public Movement(MovementType movementType, ITarget target, SpeedData speedData, ZoneData zoneData, RobotTool robotTool, DigitalOutput digitalOutput)
        {
            _movementType = movementType;
            _cirPoint = new RobotTarget(Plane.Unset);
            _target = target;
            _id = -1;
            _speedData = speedData;
            _time = -1;
            _zoneData = zoneData;
            _robotTool = robotTool;
            _workObject = new WorkObject(); // Default work object wobj0
            _setDigitalOutput = new SetDigitalOutput(digitalOutput.Name, digitalOutput.IsActive);
            CheckCombination();
        }

        /// <summary>
        /// Initializes a new instance of the Movement class.
        /// </summary>
        /// <param name="movementType"> The Movement Type. </param>
        /// <param name="target"> The Target. </param>
        /// <param name="speedData"> The Speed Data. </param>
        /// <param name="zoneData"> The Zone Data. </param>
        /// <param name="robotTool"> The Robot Tool. This will override the set default tool. </param>
        /// <param name="workObject"> The Work Object. </param>
        /// <param name="digitalOutput"> The Digital Output. When set this will define a MoveLDO or a MoveJDO instruction. </param>
        [Obsolete("This constructor is obsolete and will be removed in v3.", false)]
        public Movement(MovementType movementType, ITarget target, SpeedData speedData, ZoneData zoneData, RobotTool robotTool, WorkObject workObject, DigitalOutput digitalOutput)
        {
            _movementType = movementType;
            _cirPoint = new RobotTarget(Plane.Unset);
            _target = target;
            _id = -1;
            _speedData = speedData;
            _time = -1;
            _zoneData = zoneData;
            _robotTool = robotTool;
            _workObject = workObject;
            _setDigitalOutput = new SetDigitalOutput(digitalOutput.Name, digitalOutput.IsActive);
            CheckCombination();
        }

        /// <summary>
        /// Gets or sets the Digital Output. 
        /// </summary>
        /// <remarks>
        /// If an empty or invalid Digital Output is set a normal movement will be set (MoveAbsJ, MoveL or MoveJ). 
        /// If a valid Digital Output is combined movement will be created (MoveLDO or MoveJDO). 
        /// If as Movement Type an MoveAbsJ is set an extra RAPID code line will be added that sets the Digital Output (SetDO).
        /// </remarks>
        [Obsolete("This property is obsolete and will be removed in v3. Use SetDigitalOutput instead.", false)]
        public DigitalOutput DigitalOutput
        {
            get { return new DigitalOutput(_setDigitalOutput.Name, _setDigitalOutput.Value); }
            set { _setDigitalOutput = new SetDigitalOutput(value.Name, value.IsActive); }
        }
        #endregion
    }
}

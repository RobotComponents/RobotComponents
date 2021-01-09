// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.Definitions;
using RobotComponents.Enumerations;
using RobotComponents.Utils;

namespace RobotComponents.Actions
{
    /// <summary>
    /// Represents several Move instructions (MoveAbsJ, MoveL, MoveJ, MoveLDO and MoveJDO). 
    /// </summary>
    [Serializable()]
    public class Movement : Action, IInstruction, ISerializable
    {
        #region fields
        // Fixed fields
        private MovementType _movementType;
        private ITarget _target;
        private int _id; // Synchronization id (for multi move programming)
        private SpeedData _speedData;
        private ZoneData _zoneData;

        // Variable fields
        RobotTool _robotTool;
        WorkObject _workObject;
        DigitalOutput _digitalOutput;
        #endregion

        #region (de)serialization
        /// <summary>
        /// Protected constructor needed for deserialization of the object.  
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization. </param>
        protected Movement(SerializationInfo info, StreamingContext context)
        {
            // int version = (int)info.GetValue("Version", typeof(int)); // <-- use this if the (de)serialization changes
            _movementType = (MovementType)info.GetValue("Movement Type", typeof(MovementType));
            _target = (ITarget)info.GetValue("Target", typeof(ITarget));
            _id = (int)info.GetValue("ID", typeof(int));
            _speedData = (SpeedData)info.GetValue("Speed Data", typeof(SpeedData));
            _zoneData = (ZoneData)info.GetValue("Zone Data", typeof(ZoneData));
            _robotTool = (RobotTool)info.GetValue("Robot Tool", typeof(RobotTool));
            _workObject = (WorkObject)info.GetValue("Work Object", typeof(WorkObject));
            _digitalOutput = (DigitalOutput)info.GetValue("Digital Output", typeof(DigitalOutput));
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
            info.AddValue("Target", _target, typeof(ITarget));
            info.AddValue("ID", _id, typeof(int));
            info.AddValue("Speed Data", _speedData, typeof(SpeedData));
            info.AddValue("Zone Data", _zoneData, typeof(ZoneData));
            info.AddValue("Robot Tool", _robotTool, typeof(RobotTool));
            info.AddValue("Work Object", _workObject, typeof(WorkObject));
            info.AddValue("Digital Output", _digitalOutput, typeof(DigitalOutput));
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
        /// This constructor is typically used to cast a Robot Target to a movement. 
        /// </summary>
        /// <param name="target"> The Target. </param>
        public Movement(ITarget target)
        {
            _movementType = 0;
            _target = target;
            _id = -1;
            _speedData = new SpeedData(5); // Slowest predefined tcp speed
            _zoneData = new ZoneData(0);
            _robotTool = new RobotTool(); // Default Robot Tool tool0
            _robotTool.Clear(); // Empty Robot Tool
            _workObject = new WorkObject(); // Default work object wobj0
            _digitalOutput = new DigitalOutput(); // InValid / empty DO
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
            _target = target;
            _id = -1;
            _speedData = speedData;
            _zoneData = new ZoneData(0);
            _robotTool = new RobotTool(); // Default Robot Tool tool0
            _robotTool.Clear(); // Empty Robot Tool
            _workObject = new WorkObject(); // Default work object wobj0
            _digitalOutput = new DigitalOutput(); // InValid / empty DO
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
            _target = target;
            _id = -1;
            _speedData = speedData;
            _zoneData = zoneData;
            _robotTool = new RobotTool(); // Default Robot Tool tool0
            _robotTool.Clear(); // Empty Robot Tool
            _workObject = new WorkObject(); // Default work object wobj0
            _digitalOutput = new DigitalOutput(); // InValid / empty DO
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
            _target = target;
            _id = -1;
            _speedData = speedData;
            _zoneData = zoneData;
            _robotTool = robotTool;
            _workObject = new WorkObject(); // Default work object wobj0
            _digitalOutput = new DigitalOutput(); // InValid / empty DO
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
            _target = target;
            _id = -1;
            _speedData = speedData;
            _zoneData = zoneData;
            _robotTool = new RobotTool(); // Default Robot Tool tool0
            _robotTool.Clear(); // Empty Robot Tool
            _workObject = workObject;
            _digitalOutput = new DigitalOutput(); // InValid / empty DO
            CheckCombination();
        }

        /// <summary>
        /// Initializes a new instance of the Movement class with an empty Robot Tool (no override) and a default Work Object (wobj0)
        /// </summary>
        /// <param name="movementType"> The Movement Type. </param>
        /// <param name="target"> The Target. </param>
        /// <param name="speedData"> The Speed Data.</param>
        /// <param name="zoneData"> The Zone Data. </param>
        /// <param name="digitalOutput"> The Digital Output. When set this will define a MoveLDO or a MoveJDO instruction. </param>
        public Movement(MovementType movementType, ITarget target, SpeedData speedData, ZoneData zoneData, DigitalOutput digitalOutput)
        {
            _movementType = movementType;
            _target = target;
            _id = -1;
            _speedData = speedData;
            _zoneData = zoneData;
            _robotTool = new RobotTool(); // Default Robot Tool tool0
            _robotTool.Clear(); // Empty Robot Tool
            _workObject = new WorkObject(); // Default work object wobj0
            _digitalOutput = digitalOutput;
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
            _target = target;
            _id = -1;
            _speedData = speedData;
            _zoneData = zoneData;
            _robotTool = robotTool;
            _workObject = workObject;
            _digitalOutput = new DigitalOutput(); // InValid / empty DO
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
        public Movement(MovementType movementType, ITarget target, SpeedData speedData, ZoneData zoneData, RobotTool robotTool, DigitalOutput digitalOutput)
        {
            _movementType = movementType;
            _target = target;
            _id = -1;
            _speedData = speedData;
            _zoneData = zoneData;
            _robotTool = robotTool;
            _workObject = new WorkObject(); // Default work object wobj0
            _digitalOutput = digitalOutput;
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
        public Movement(MovementType movementType, ITarget target, SpeedData speedData, ZoneData zoneData, RobotTool robotTool, WorkObject workObject, DigitalOutput digitalOutput)
        {
            _movementType = movementType;
            _target = target;
            _id = -1;
            _speedData = speedData;
            _zoneData = zoneData;
            _robotTool = robotTool;
            _workObject = workObject;
            _digitalOutput = digitalOutput;
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
            _target = movement.Target.DuplicateTarget();
            _id = movement.SyncID;
            _speedData = movement.SpeedData.Duplicate();
            _zoneData = movement.ZoneData.Duplicate();
            _digitalOutput = movement.DigitalOutput.Duplicate();

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
        /// <returns> A deep copy of the Movement instance. </returns>
        public Movement Duplicate()
        {
            return new Movement(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Movement instance as IInstruction.
        /// </summary>
        /// <returns> A deep copy of the Movement instance as an IInstruction. </returns>
        public IInstruction DuplicateInstruction()
        {
            return new Movement(this) as IInstruction;
        }

        /// <summary>
        /// Returns an exact duplicate of this Movement instance without meshes.
        /// </summary>
        /// <returns> A deep copy of the Movement instance. </returns>
        public Movement DuplicateWithoutMesh()
        {
            return new Movement(this, false);
        }

        /// <summary>
        /// Returns an exact duplicate of this Movement instance as an Action. 
        /// </summary>
        /// <returns> A deep copy of the Movement instance as an Action. </returns>
        public override Action DuplicateAction()
        {
            return new Movement(this) as Action;
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
                return "Invalid Movement";
            }

            else if (_id < 0)
            {
                if (this.MovementType == MovementType.MoveAbsJ)
                {
                    return "Absolute Joint Movement (" + this.Target.Name + "\\" + this.WorkObject.Name + ")";
                }
                else if (this.MovementType == MovementType.MoveL)
                {
                    return "Linear Movement (" + this.Target.Name + "\\" + this.WorkObject.Name + ")";
                }
                else if (this.MovementType == MovementType.MoveJ)
                {
                    return "Joint Movement (" + this.Target.Name + "\\" + this.WorkObject.Name + ")";
                }
                else
                {
                    return "Movement";
                }
            }

            else
            {
                if (this.MovementType == MovementType.MoveAbsJ)
                {
                    return "Coordinated synchronized Absolute Joint Movement (" + this.Target.Name + "\\" + this.WorkObject.Name + ")";
                }
                else if (this.MovementType == MovementType.MoveL)
                {
                    return "Coordinated synchronized Linear Movement (" + this.Target.Name + "\\" + this.WorkObject.Name + ")";
                }
                else if (this.MovementType == MovementType.MoveJ)
                {
                    return "Coordinated synchronized Joint Movement (" + this.Target.Name + "\\" + this.WorkObject.Name + ")";
                }
                else
                {
                    return "Coordinated synchronized Movement";
                }
            }
        }

        /// <summary>
        /// Checks the combination between the movement type and the target type.
        /// Throws an exception if the combination is not valid. 
        /// </summary>
        private void CheckCombination()
        {
            if (_movementType != MovementType.MoveAbsJ && _target is JointTarget)
            {
                throw new InvalidOperationException("Invalid Move instruction: A Joint Target cannot be combined with a MoveL or MoveJ instruction.");
            }
        }

        /// <summary>
        /// Calculates the position and the orientation of the target in world coordinate space. 
        /// If an external axis is attached to the work object this method returns the pose of the 
        /// target plane in the world coorinate space for axis values equal to zero.
        /// </summary>
        /// <returns> The the target plane in world coordinate space. </returns>
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
        /// <returns> The posed target plane in world coordinate space. </returns>
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
        /// Returns the RAPID declaration code line of the this action.
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> An empty string. </returns>
        public override string ToRAPIDDeclaration(Robot robot)
        {
            return String.Empty;
        }

        /// <summary>
        /// Returns the RAPID instruction code line of the this action. 
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> The RAPID code line. </returns>
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

            // A movement not combined with a digital output
            if (_digitalOutput.IsValid == false)
            {
                // MoveAbsJ
                if (_movementType == MovementType.MoveAbsJ)
                {
                    // If a robot target is converted to a joint target we add the suffix _jt to the target name.
                    string name = _target.Name;

                    if (_target is RobotTarget)
                    {
                        name += "_jt";
                    }

                    string code = "MoveAbsJ ";
                    code += name;

                    if (_id > -1)
                    {
                        code += "\\ID:=" + _id;
                    }

                    code += ", ";
                    code += _speedData.Name + ", ";
                    code += _zoneData.Name + ", ";
                    code += toolName;
                    code += "\\WObj:=" + _workObject.Name + ";";

                    return code;
                }

                // MoveL
                else if (_movementType == MovementType.MoveL && _target is RobotTarget)
                {
                    string code = "MoveL ";
                    code += _target.Name;

                    if (_id > -1)
                    {
                        code += "\\ID:=" + _id;
                    }

                    code += ", ";
                    code += _speedData.Name + ", ";
                    code += _zoneData.Name + ", ";
                    code += toolName;
                    code += "\\WObj:=" + _workObject.Name + ";";

                    return code;
                }

                // MoveJ
                else if (_movementType == MovementType.MoveJ && _target is RobotTarget)
                {
                    string code = "MoveJ ";
                    code += _target.Name;

                    if (_id > -1)
                    {
                        code += "\\ID:=" + _id;
                    }

                    code += ", ";
                    code += _speedData.Name + ", ";
                    code += _zoneData.Name + ", ";
                    code += toolName;
                    code += "\\WObj:=" + _workObject.Name + ";";

                    return code;
                }

                // Wrong movement type or combination
                else
                {
                    throw new InvalidOperationException("Invalid Move instruction: A Joint Target cannot be combined with a MoveL or MoveJ instruction.");
                }
            }

            // A movement combined with a digital output
            else
            {
                // MoveAbsJ + SetDO: There is no RAPID function that combines the an absolute joint movement and a DO.
                // Therefore, we write two separate RAPID code lines for an aboslute joint momvement combined with a DO. 
                if (_movementType == MovementType.MoveAbsJ)
                {
                    // If a robot target is converted to a joint target we add the suffix _jt to the target name.
                    string name = _target.Name;

                    if (_target is RobotTarget) 
                    { 
                        name += "_jt"; 
                    }

                    string code = "MoveAbsJ ";
                    code += name;

                    if (_id > -1)
                    {
                        code += "\\ID:=" + _id;
                    }

                    code += ", ";
                    code += _speedData.Name + ", ";
                    code += _zoneData.Name + ", ";
                    code += toolName;
                    code += "\\WObj:=" + _workObject.Name + "; ";
                    code += _digitalOutput.ToRAPIDInstruction(robot);

                    return code;
                }

                // MoveLDO
                else if (_movementType == MovementType.MoveL && _target is RobotTarget)
                {
                    string code = "MoveLDO ";
                    code += _target.Name;

                    if (_id > -1)
                    {
                        code += "\\ID:=" + _id;
                    }

                    code += ", ";
                    code += _speedData.Name + ", ";
                    code += _zoneData.Name + ", ";
                    code += toolName;
                    code += "\\WObj:=" + _workObject.Name + ", ";
                    code += _digitalOutput.Name + ", ";
                    code += (_digitalOutput.IsActive ? 1 : 0) + ";";

                    return code;
                }

                // MoveJDO
                else if (_movementType == MovementType.MoveJ && _target is RobotTarget)
                {
                    string code = "MoveJDO ";
                    code += _target.Name;

                    if (_id > -1)
                    {
                        code += "\\ID:=" + _id;
                    }

                    code += ", ";
                    code += _speedData.Name + ", ";
                    code += _zoneData.Name + ", ";
                    code +=  toolName;
                    code += "\\WObj:=" + _workObject.Name + ", ";
                    code += _digitalOutput.Name + ", ";
                    code += (_digitalOutput.IsActive ? 1 : 0) + ";";

                    return code;
                }

                // Wrong movement type or combination
                else
                {
                    throw new InvalidOperationException("Invalid Move instruction: A Joint Target cannot be combined with a MoveL or MoveJ instruction.");
                }
            }
        }

        /// <summary>
        /// Creates declarations in the RAPID program module inside the RAPID Generator. 
        /// This method is called inside the RAPID generator.
        /// </summary>
        /// <param name="RAPIDGenerator"> The RAPID Generator. </param>
        public override void ToRAPIDDeclaration(RAPIDGenerator RAPIDGenerator)
        {
            // Generate the code for the zone and speeddata
            _speedData.ToRAPIDDeclaration(RAPIDGenerator);
            _zoneData.ToRAPIDDeclaration(RAPIDGenerator);

            // Generate code from robot targets
            if (_target is RobotTarget robotTarget)
            {
                // Update the movement of the inverse kinematics
                RAPIDGenerator.Robot.InverseKinematics.Movement = this;

                // Generates the robot target variable for a MoveL or MoveJ instruction
                if (_movementType == MovementType.MoveL || _movementType == MovementType.MoveJ)
                {
                    RAPIDGenerator.Robot.InverseKinematics.CalculateExternalJointPosition();
                    robotTarget.ToRAPIDDeclaration(RAPIDGenerator);
                }

                // Generates the joint target variable from a robot target for a MoveAbsJ instruction
                else
                {
                    if (!RAPIDGenerator.Targets.ContainsKey(robotTarget.Name + "_jt"))
                    {
                        // Calculate the axis values from the robot target
                        RAPIDGenerator.Robot.InverseKinematics.Calculate();
                        RAPIDGenerator.ErrorText.AddRange(new List<string>(RAPIDGenerator.Robot.InverseKinematics.ErrorText));

                        // Create a joint target from the axis values
                        RobotJointPosition robJointPosition = RAPIDGenerator.Robot.InverseKinematics.RobotJointPosition.Duplicate();
                        ExternalJointPosition extJointPosition = RAPIDGenerator.Robot.InverseKinematics.ExternalJointPosition.Duplicate();
                        JointTarget jointTarget = new JointTarget(robotTarget.Name + "_jt", robJointPosition, extJointPosition);
                        jointTarget.ReferenceType = _target.ReferenceType;

                        // Create the RAPID code
                        jointTarget.ToRAPIDDeclaration(RAPIDGenerator);
                    }
                }
            }

            // Generate code from joint targets
            else if (_target is JointTarget jointTarget)
            {
                // JointTarget with MoveAbsJ
                if (_movementType == MovementType.MoveAbsJ)
                {
                    jointTarget.ToRAPIDDeclaration(RAPIDGenerator);
                    RAPIDGenerator.ErrorText.AddRange(jointTarget.CheckAxisLimits(RAPIDGenerator.Robot));
                }

                // Joint Target combined with MoveL or MoveJ
                else
                {
                    throw new InvalidOperationException("Invalid Move instruction: A Joint Target cannot be combined with a MoveL or MoveJ instruction.");
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
            RAPIDGenerator.StringBuilder.Append(Environment.NewLine + "\t\t" + this.ToRAPIDInstruction(RAPIDGenerator.Robot));

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
                if (Target == null) { return false; }
                if (Target.IsValid == false) { return false; }
                if (SpeedData == null) { return false; }
                if (SpeedData.IsValid == false) { return false; }
                if (ZoneData == null) { return false; }
                if (ZoneData.IsValid == false) { return false; }
                if (WorkObject == null) { return false;  }
                if (WorkObject.IsValid == false) { return false; }
                if (Target is JointTarget && MovementType == MovementType.MoveL) { return false; }
                if (Target is JointTarget && MovementType == MovementType.MoveJ) { return false; }
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
        /// Gets or sets the Target.
        /// Defines the destination target of the robot and external axes.
        /// </summary>
        public ITarget Target
        {
            get { return _target; }
            set { _target = value; }
        }

        /// <summary>
        /// Gets or sets the synchronization id for multi move programming. 
        /// This ID number must be defined for coordinated synchronized movements in multi move systems. 
        /// Set this property to -1 to define normal movements (not coordinated / not synchronized).
        /// </summary>
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
        /// Gets or sets the Zone Data.
        /// </summary>
        public ZoneData ZoneData
        {
            get { return _zoneData; }
            set { _zoneData = value; }
        }

        /// <summary>
        /// Gets or sets the Robot Tool.
        /// If an empty or no Robot Tool is used, the Robot Tool set at the Robot will be used. 
        /// </summary>
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
        /// Gets or set the Digital Output. 
        /// If an empty or invalid Digital Output is set a normal movement will be set (MoveAbsJ, MoveL or MoveJ). 
        /// If a valid Digital oOutput is combined movement will be created (MoveLDO or MoveJDO). 
        /// If as Movement Type an MoveAbsJ is set an extra RAPID code line will be added that sets the Digital Output (SetDO).
        /// </summary>
        public DigitalOutput DigitalOutput
        {
            get { return _digitalOutput; }
            set { _digitalOutput = value; }
        }
        #endregion
    }
}

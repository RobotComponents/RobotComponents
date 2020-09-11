// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.Definitions;
using RobotComponents.Enumerations;

namespace RobotComponents.Actions
{
    /// <summary>
    /// Movement class
    /// </summary>
    public class Movement : Action
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

        #region constructors
        /// <summary>
        /// An empty movement constructor.
        /// </summary>
        public Movement()
        {
        }

        /// <summary>
        /// Constructs robot movement with as only argument a robot target. 
        /// This constructor is typically used to cast a robot target to a robot movement. 
        /// </summary>
        /// <param name="target"> The target as a Target. </param>
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
        /// Constructs a robot movement with an empty robot tool (no override), a default work object (wobj0) and an empty digital output. 
        /// </summary>
        /// <param name="movementType"> The movement type as a MovementType. </param>
        /// <param name="target"> The target as a Target. </param>
        /// <param name="speedData"> The SpeedData as a SpeedData </param>
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
        /// Constructs a robot movement with an empty robot tool (no override), a default work object (wobj0) and an empty digital output. 
        /// </summary>
        /// <param name="movementType"> The movement type as a MovementType. </param>
        /// <param name="target"> The target as a Target. </param>
        /// <param name="speedData"> The SpeedData as a SpeedData </param>
        /// <param name="zoneData"> The ZoneData as a ZoneData </param>
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
        /// Constructs a robot movement with an empty digital output, a default work object (wobj0) and a empty robot tool (no override).
        /// </summary>
        /// <param name="movementType"> The movement type as a MovemenType. </param>
        /// <param name="target"> The target as a Target. </param>
        /// <param name="speedData"> The SpeedData as a SpeedData </param>
        /// <param name="zoneData"> The ZoneData as a ZoneData </param>
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
        /// Constructs a robot movement with an empty digital output, an empty robot tool (no override) and a user definied work object. 
        /// </summary>
        /// <param name="movementType"> The movement type as a MovementType. </param>
        /// <param name="target"> The target as a Target. </param>
        /// <param name="speedData"> The SpeedData as a SpeedData </param>
        /// <param name="zoneData"> The ZoneData as a ZoneData </param>
        /// <param name="workObject"> The Work Object as a Work Object </param>
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
        /// Constructs a robot movement with an empty robot tool (no override), a default work object (wobj0) and user definied digital output. 
        /// </summary>
        /// <param name="movementType"> The movement type as a MovementType. </param>
        /// <param name="target"> The target as a Target. </param>
        /// <param name="speedData"> The SpeedData as a SpeedData </param>
        /// <param name="zoneData"> The ZoneData as a ZoneData </param>
        /// <param name="digitalOutput"> A Digital Output as a Digital Output. When set this will define a MoveLDO or a MoveJDO. </param>
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
        /// Constructs a robot movement with an empty digital output.
        /// </summary>
        /// <param name="movementType"> The movement type as a MovementType. </param>
        /// <param name="target"> The target as a Target. </param>
        /// <param name="speedData"> The SpeedData as a SpeedData </param>
        /// <param name="zoneData"> The ZoneData as a ZoneData </param>
        /// <param name="robotTool"> The Robot Tool. This will override the set default tool. </param>
        /// <param name="workObject"> The Work Object as a Work Object </param>
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
        /// Constructs a robot movement with a default work object (wobj0). 
        /// </summary>
        /// <param name="target"> The target as a Target. </param>
        /// <param name="speedData"> The SpeedData as a SpeedData </param>
        /// <param name="movementType"> The movement type as a MovementType. </param>
        /// <param name="zoneData"> The ZoneData as a ZoneData </param>
        /// <param name="robotTool"> The Robot Tool. This will override the set default tool. </param>
        /// <param name="digitalOutput"> A Digital Output as a Digital Output. When set this will define a MoveLDO or a MoveJDO. </param>
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
        /// Constructs a robot movement. 
        /// </summary>
        /// <param name="movementType"> The movement type as a MovementType. </param>
        /// <param name="target"> The target as a Target. </param>
        /// <param name="speedData"> The SpeedData as a SpeedData </param>
        /// <param name="zoneData"> The ZoneData as a ZoneData </param>
        /// <param name="robotTool"> The Robot Tool. This will override the set default tool. </param>
        /// <param name="workObject"> The Work Object as a Work Object </param>
        /// <param name="digitalOutput"> A Digital Output as a Digital Output. When set this will define a MoveLDO or a MoveJDO. </param>
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
        /// Creates a new movement by duplicating an existing movement. 
        /// This creates a deep copy of the existing movement. 
        /// </summary>
        /// <param name="movement"> The movement that should be duplicated. </param>
        /// <param name="duplicateMesh"> A boolean that indicates if the mesh should be duplicated. </param>
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
        /// Calculates the position and the orientation of the target in the world coordinate system. 
        /// If an external axis is attached to the work object this returns the pose of the 
        /// target plane in the world coorinate space for axis values equal to zero.
        /// </summary>
        /// <returns> The the target plane in the world coordinate system. </returns>
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
        /// Calculates the posed target plane for the defined Robot with attached external axes in world coorindate space.
        /// </summary>
        /// <param name="robot"> The robot info with the external axes that defined the axis logic. </param>
        /// <param name="logic"> Retuns the axis logic number as an int. </param>
        /// <returns> The posed target plane in the word coordinate system. </returns>
        public Plane GetPosedGlobalTargetPlane(Robot robot, out int logic)
        {
            // Initiate axis logic
            logic = -1; // dummy value

            if (_target is RobotTarget robotTarget)
            {
                // Not transformed global target plane
                Plane plane = GetGlobalTargetPlane();

                // Re-orient the target plane if an external axis is attached to the work object
                if (_workObject.ExternalAxis != null)
                {
                    // Check if the axis is attached to the robot and get the axis logic number
                    logic = robot.ExternalAxis.FindIndex(p => p.Name == _workObject.ExternalAxis.Name); // TODO: use _workObject.ExternalAxis.AxisNumber;

                    // Check axis logic
                    if (logic == -1)
                    {
                        throw new InvalidOperationException("The external axis that is attached to the work object could not be found in the list with external axes that are attached to the Robot. Did you attach the external axis to the Robot?");
                    }

                    // Get external axis value
                    double axisValue = robotTarget.ExternalJointPosition[logic];
                    if (axisValue == 9e9) { axisValue = 0; } // If the user does not define an axis value we set it to zero. 

                    // Transform
                    Transform transform = _workObject.ExternalAxis.CalculateTransformationMatrix(axisValue, out bool inLimits);
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
        /// Used to create variable definition code of this action. 
        /// </summary>
        /// <param name="robot"> Defines the Robot were the code is generated for. </param>
        /// <returns> Returns the RAPID code line as a string. </returns>
        public override string ToRAPIDDeclaration(Robot robot)
        {
            return String.Empty;
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
        /// Used to create variable definitions in the RAPID Code. It is typically called inside the CreateRAPIDCode() method of the RAPIDGenerator class.
        /// </summary>
        /// <param name="RAPIDGenerator"> Defines the RAPIDGenerator. </param>
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
                    RAPIDGenerator.Robot.InverseKinematics.CalculateExternalAxisValues();
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
                    RAPIDGenerator.ErrorText.AddRange(jointTarget.CheckForAxisLimits(RAPIDGenerator.Robot));
                }

                // Joint Target combined with MoveL or MoveJ
                else
                {
                    throw new InvalidOperationException("Invalid Move instruction: A Joint Target cannot be combined with a MoveL or MoveJ instruction.");
                }
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

        #region properties
        /// <summary>
        /// A boolean that indicates if the Movement object is valid.
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
        /// The movement type.
        /// </summary>
        public MovementType MovementType
        {
            get { return _movementType; }
            set { _movementType = value; }
        }

        /// <summary>
        /// Defines the destination target of the robot and external axes for this movement.
        /// </summary>
        public ITarget Target
        {
            get { return _target; }
            set { _target = value; }
        }

        /// <summary>
        /// The Synchronization id for multi move programming. 
        /// This ID number must be defined for coordinated synchronized movements in multi move systems. 
        /// Set this property to -1 to define normal movements (not coordinated / not synchronized).
        /// </summary>
        public int SyncID
        {
            get { return _id; }
            set { _id = value; }
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

        /// <summary>
        /// The work object (coordinate system) to which the robot position in the instruction is related.
        /// </summary>
        public WorkObject WorkObject
        {
            get { return _workObject; }
            set { _workObject = value; }
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

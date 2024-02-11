// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
// RobotComponents Libs
using RobotComponents.ABB.Enumerations;
using RobotComponents.ABB.Definitions;
using RobotComponents.ABB.Actions.Declarations;

namespace RobotComponents.ABB.Actions.Instructions
{
    /// <summary>
    /// Represents the WaitSyncTask instruction to synchronize several program tasks at a special point in each program.
    /// </summary>
    [Serializable()]
    public class WaitSyncTask : IAction, IInstruction, ISyncident, ISerializable
    {
        #region fields
        private VariableType _variableType;
        private bool _inPosition;
        private string _syncident;
        private TaskList _taskList;
        private double _timeOut;
        #endregion

        #region (de)serialization
        /// <summary>
        /// Protected constructor needed for deserialization of the object.  
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization. </param>
        protected WaitSyncTask(SerializationInfo info, StreamingContext context)
        {
            //Version version = (Version)info.GetValue("Version", typeof(Version)); // <-- use this if the (de)serialization changes
            _variableType = (VariableType)info.GetValue("Variable Type", typeof(VariableType));
            _inPosition = (bool)info.GetValue("In Position", typeof(bool));
            _syncident = (string)info.GetValue("Sync ID", typeof(string));
            _taskList = (TaskList)info.GetValue("Task List", typeof(TaskList));
            _timeOut = (double)info.GetValue("Time Out", typeof(double));
        }

        /// <summary>
        /// Populates a SerializationInfo with the data needed to serialize the object.
        /// </summary>
        /// <param name="info"> The SerializationInfo to populate with data. </param>
        /// <param name="context"> The destination for this serialization. </param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Version", VersionNumbering.Version, typeof(Version));
            info.AddValue("Variable Type", _variableType, typeof(VariableType));
            info.AddValue("In Position", _inPosition, typeof(bool));
            info.AddValue("Sync ID", _syncident, typeof(string));
            info.AddValue("Task List", _taskList, typeof(TaskList));
            info.AddValue("Time Out", _timeOut, typeof(double));
        }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the WaitSyncTask class.
        /// </summary>
        public WaitSyncTask()
        {
        }

        /// <summary>
        /// Initializes a new instance of the WaitSyncTask class.
        /// </summary>
        /// <param name="name"> The name of the synchronization point. </param>
        /// <param name="tasks"> The program tasks that should meet in the synchronization point. </param>
        /// <param name="inPosition"> Specifies whether or not the robot and external axes must have come to a standstill in its meeting point. </param>
        /// <param name="timeOut"> The max. time to wait for the other program tasks to reach the synchronization point. </param>
        public WaitSyncTask(string name, TaskList tasks, bool inPosition = false, double timeOut = -1)
        {
            _variableType = VariableType.VAR;
            _syncident = name;
            _taskList = tasks;
            _inPosition = inPosition;
            _timeOut = timeOut;
        }

        /// <summary>
        /// Initializes a new instance of the WaitSyncTask class by duplicating an existing WaitSyncTask instance. 
        /// </summary>
        /// <param name="waitSyncTask"> The WaitSyncTask instance to duplicate. </param>
        public WaitSyncTask(WaitSyncTask waitSyncTask)
        {
            _variableType = waitSyncTask.VariableType;
            _syncident = waitSyncTask.SyncID;
            _taskList = waitSyncTask.TaskList.Duplicate();
            _inPosition = waitSyncTask.InPosition;
            _timeOut = waitSyncTask.TimeOut;
        }

        /// <summary>
        /// Returns an exact duplicate of this WaitSyncTask instance.
        /// </summary>
        /// <returns> 
        /// A deep copy of the WaitSyncTask instance. 
        /// </returns>
        public WaitSyncTask Duplicate()
        {
            return new WaitSyncTask(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this WaitSyncTask instance as IInstruction.
        /// </summary>
        /// <returns> 
        /// A deep copy of the WaitSyncTask instance as an IDeclaration. 
        /// </returns>
        public IInstruction DuplicateInstruction()
        {
            return new WaitSyncTask(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this WaitSyncTask instance as ISyncident.
        /// </summary>
        /// <returns> 
        /// A deep copy of the WaitSyncTask instance as an ISyncident. 
        /// </returns>
        public ISyncident DuplicateSyncident()
        {
            return new WaitSyncTask(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Tasks instance as an Action. 
        /// </summary>
        /// <returns> 
        /// A deep copy of the Tasks instance as an Action. 
        /// </returns>
        public IAction DuplicateAction()
        {
            return new WaitSyncTask(this);
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
                return "Invalid Wait Sync Task";
            }
            else
            {
                return $"Wait Sync Task ({_syncident})";
            }
        }

        /// <summary>
        /// Returns the RAPID declaration code line of the this action.
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> 
        /// The RAPID code line. 
        /// </returns>
        public string ToRAPIDDeclaration(Robot robot)
        {
            return $"{Enum.GetName(typeof(VariableType), _variableType)} syncident {_syncident};";
        }

        /// <summary>
        /// Returns the RAPID instruction code line of the this action. 
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> 
        /// The RAPID code line. 
        /// </returns>
        public string ToRAPIDInstruction(Robot robot)
        {
            return $"WaitSyncTask {(_inPosition ? "\\InPos, " : "")}{_syncident}, {_taskList.Name}" +
                $"{(_timeOut > 0 ? $"\\TimeOut:={_timeOut:0.###}" : "")};";
        }

        /// <summary>
        /// Creates declarations and instructions in the RAPID program module inside the RAPID Generator.
        /// </summary>
        /// <remarks>
        /// This method is called inside the RAPID generator.
        /// </remarks>
        /// <param name="RAPIDGenerator"> The RAPID Generator. </param>
        public void ToRAPIDGenerator(RAPIDGenerator RAPIDGenerator)
        {
            _taskList.ToRAPIDGenerator(RAPIDGenerator);

            if (!RAPIDGenerator.Syncidents.ContainsKey(_syncident))
            {
                RAPIDGenerator.Syncidents.Add(_syncident, this);
                RAPIDGenerator.ProgramDeclarationsMultiMove.Add("    " + ToRAPIDDeclaration(RAPIDGenerator.Robot));
            }

            RAPIDGenerator.ProgramInstructions.Add("    " + "    " + ToRAPIDInstruction(RAPIDGenerator.Robot));
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets a value indicating whether or not the object is valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                if (_syncident == null) { return false; }
                if (_syncident == "") { return false; }
                if (_taskList.IsValid == false) { return false; }
                return true;
            }
        }

        /// <summary>
        /// Gets or sets the variable type of the syncident.
        /// </summary>
        public VariableType VariableType
        {
            get { return _variableType; }
            set { _variableType = value; }
        }

        /// <summary>
        /// Gets or sets the name of the synchronization (meeting) point (syncident).
        /// </summary>
        public string SyncID
        {
            get { return _syncident; }
            set { _syncident = value; }
        }

        /// <summary>
        /// Gets or sets the program tasks that should meet in the synchronization point.
        /// </summary>
        public TaskList TaskList
        {
            get { return _taskList; }
            set { _taskList = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the robot and external axes must have come to a standstill 
        /// before this program task starts waiting for other program tasks to reach its meeting point.
        /// </summary>
        public bool InPosition
        {
            get { return _inPosition; }
            set { _inPosition = value; }
        }

        /// <summary>
        /// Gets te max. time to wait for the other program tasks to reach the synchronization point.
        /// </summary>
        /// <remarks>
        /// Set a negative value to wait for ever(default is -1).
        /// </remarks>
        public double TimeOut
        {
            get { return _timeOut; }
            set { _timeOut = value; }
        }
        #endregion
    }
}
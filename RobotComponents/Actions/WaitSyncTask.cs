// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
// RobotComponents Libs
using RobotComponents.Enumerations;
using RobotComponents.Definitions;
using RobotComponents.Utils;

namespace RobotComponents.Actions
{
    /// <summary>
    /// Represents the WaitSyncTask instruction to synchronize several program tasks at a special point in each program.
    /// </summary>
    [Serializable()]
    public class WaitSyncTask : Action, IInstruction, ISyncident, ISerializable
    {
        #region fields
        private ReferenceType _referenceType; // reference type for sync identification
        private bool _inPosition; // if true the mechanical unit comes to still stand at the sync point
        private string _syncident; // the sync identification name
        private TaskList _taskList; // the set with tasks to syncronize
        #endregion

        #region (de)serialization
        /// <summary>
        /// Protected constructor needed for deserialization of the object.  
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization. </param>
        protected WaitSyncTask(SerializationInfo info, StreamingContext context)
        {
            // int version = (int)info.GetValue("Version", typeof(int)); // <-- use this if the (de)serialization changes
            _referenceType = (ReferenceType)info.GetValue("Reference Type", typeof(ReferenceType));
            _inPosition = (bool)info.GetValue("In Position", typeof(bool));
            _syncident = (string)info.GetValue("Sync ID", typeof(string));
            _taskList = (TaskList)info.GetValue("Task List", typeof(TaskList));
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
            info.AddValue("Reference Type", _referenceType, typeof(ReferenceType));
            info.AddValue("In Position", _inPosition, typeof(bool));
            info.AddValue("Sync ID", _syncident, typeof(string));
            info.AddValue("Task List", _taskList, typeof(TaskList));
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
        public WaitSyncTask(string name, TaskList tasks, bool inPosition = false)
        {
            _referenceType = ReferenceType.VAR;
            _syncident = name;
            _taskList = tasks;
            _inPosition = inPosition;
        }

        /// <summary>
        /// Initializes a new instance of the WaitSyncTask class by duplicating an existing WaitSyncTask instance. 
        /// </summary>
        /// <param name="waitSyncTask"> The WaitSyncTask instance to duplicate. </param>
        public WaitSyncTask(WaitSyncTask waitSyncTask)
        {
            _referenceType = waitSyncTask.ReferenceType;
            _syncident = waitSyncTask.SyncID;
            _taskList = waitSyncTask.TaskList.Duplicate();
            _inPosition = waitSyncTask.InPosition;
        }

        /// <summary>
        /// Returns an exact duplicate of this WaitSyncTask instance.
        /// </summary>
        /// <returns> A deep copy of the WaitSyncTask instance. </returns>
        public WaitSyncTask Duplicate()
        {
            return new WaitSyncTask(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this WaitSyncTask instance as IInstruction.
        /// </summary>
        /// <returns> A deep copy of the WaitSyncTask instance as an IDeclaration. </returns>
        public IInstruction DuplicateInstruction()
        {
            return new WaitSyncTask(this) as IInstruction;
        }

        /// <summary>
        /// Returns an exact duplicate of this WaitSyncTask instance as ISyncident.
        /// </summary>
        /// <returns> A deep copy of the WaitSyncTask instance as an ISyncident. </returns>
        public ISyncident DuplicateSyncident()
        {
            return new WaitSyncTask(this) as ISyncident;
        }

        /// <summary>
        /// Returns an exact duplicate of this Tasks instance as an Action. 
        /// </summary>
        /// <returns> A deep copy of the Tasks instance as an Action. </returns>
        public override Action DuplicateAction()
        {
            return new WaitSyncTask(this) as Action;
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
                return "Invalid Wait Sync Task";
            }
            else
            {
                return "Wait Sync Task (" + this.SyncID + ")";
            }
        }

        /// <summary>
        /// Returns the RAPID declaration code line of the this action.
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> An empty string. </returns>
        public override string ToRAPIDDeclaration(Robot robot)
        {
            return Enum.GetName(typeof(ReferenceType), _referenceType) + " syncident " + _syncident + ";";
        }

        /// <summary>
        /// Returns the RAPID instruction code line of the this action. 
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> The RAPID code line. </returns>
        public override string ToRAPIDInstruction(Robot robot)
        {
            if (_inPosition == false)
            {
                return "WaitSyncTask " + _syncident + ", " + _taskList.Name + ";";
            }
            else
            {
                return "WaitSyncTask \\InPos, " + _syncident + ", " + _taskList.Name + ";";
            }
        }

        /// <summary>
        /// Creates declarations in the RAPID program module inside the RAPID Generator. 
        /// This method is called inside the RAPID generator.
        /// </summary>
        /// <param name="RAPIDGenerator"> The RAPID Generator. </param>
        public override void ToRAPIDDeclaration(RAPIDGenerator RAPIDGenerator)
        {
            _taskList.ToRAPIDDeclaration(RAPIDGenerator);

            if (!RAPIDGenerator.Syncidents.ContainsKey(_syncident))
            {
                RAPIDGenerator.Syncidents.Add(_syncident, this);
                RAPIDGenerator.ProgramDeclarationsMultiMove.Add("    " + this.ToRAPIDDeclaration(RAPIDGenerator.Robot));
            }
        }

        /// <summary>
        /// Creates instructions in the RAPID program module inside the RAPID Generator.
        /// This method is called inside the RAPID generator.
        /// </summary>
        /// <param name="RAPIDGenerator"> The RAPID Generator. </param>
        public override void ToRAPIDInstruction(RAPIDGenerator RAPIDGenerator)
        {
            RAPIDGenerator.ProgramInstructions.Add("    " + "    " + this.ToRAPIDInstruction(RAPIDGenerator.Robot));
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
                if (_syncident == null) { return false; }
                if (_syncident == "") { return false; }
                if (_taskList.IsValid == false ) { return false; }
                return true;
            }
        }

        /// <summary>
        /// Gets or sets the reference type of the syncident.
        /// </summary>
        public ReferenceType ReferenceType
        {
            get { return _referenceType; }
            set { _referenceType = value; }
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
        #endregion
    }
}

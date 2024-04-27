﻿// This file is part of Robot Components. Robot Components is licensed 
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
    /// Represents the SyncMoveOn instruction that starts a sequence of synchronized movements.
    /// </summary>
    [Serializable()]
    public class SyncMoveOn : IAction, IInstruction, ISyncident, ISerializable
    {
        #region fields
        private VariableType _variableType;
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
        protected SyncMoveOn(SerializationInfo info, StreamingContext context)
        {
            //Version version = (Version)info.GetValue("Version", typeof(Version)); // <-- use this if the (de)serialization changes
            _variableType = (VariableType)info.GetValue("Variable Type", typeof(VariableType));
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
            info.AddValue("Sync ID", _syncident, typeof(string));
            info.AddValue("Task List", _taskList, typeof(TaskList));
            info.AddValue("Time Out", _timeOut, typeof(double));
        }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the SyncMoveOn class.
        /// </summary>
        public SyncMoveOn()
        {
        }

        /// <summary>
        /// Initializes a new instance of the SyncMoveOn class.
        /// </summary>
        /// <param name="name"> The name of the synchronization point. </param>
        /// <param name="tasks"> The program tasks that should meet in the synchronization point. </param>
        /// <param name="timeOut"> The max. time to wait for the other program tasks to reach the synchronization point. </param>
        public SyncMoveOn(string name, TaskList tasks, double timeOut = -1)
        {
            _variableType = VariableType.VAR;
            _syncident = name;
            _taskList = tasks;
            _timeOut = timeOut;
        }

        /// <summary>
        /// Initializes a new instance of the SyncMoveOn class by duplicating an existing SyncMoveOn instance. 
        /// </summary>
        /// <param name="syncMoveOn"> The SyncMoveOn instance to duplicate. </param>
        public SyncMoveOn(SyncMoveOn syncMoveOn)
        {
            _variableType = syncMoveOn.VariableType;
            _syncident = syncMoveOn.SyncID;
            _taskList = syncMoveOn.TaskList.Duplicate();
            _timeOut = syncMoveOn.TimeOut;
        }

        /// <summary>
        /// Returns an exact duplicate of this SyncMoveOn instance.
        /// </summary>
        /// <returns> 
        /// A deep copy of the SyncMoveOn instance. 
        /// </returns>
        public SyncMoveOn Duplicate()
        {
            return new SyncMoveOn(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this SyncMoveOn instance as IInstruction.
        /// </summary>
        /// <returns> 
        /// A deep copy of the SyncMoveOn instance as an IInstructions. 
        /// </returns>
        public IInstruction DuplicateInstruction()
        {
            return new SyncMoveOn(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this SyncMoveOn instance as ISyncident.
        /// </summary>
        /// <returns> 
        /// A deep copy of the SyncMoveOn instance as an ISyncident. 
        /// </returns>
        public ISyncident DuplicateSyncident()
        {
            return new SyncMoveOn(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this SyncMoveOn instance as an Action. 
        /// </summary>
        /// <returns> 
        /// A deep copy of the SyncMoveOn instance as an Action. </returns>
        public IAction DuplicateAction()
        {
            return new SyncMoveOn(this);
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
                return "Invalid Sync Move On";
            }
            else
            {
                return $"Sync Move On ({_syncident})";
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
            return $"SyncMoveOn {_syncident}, {_taskList.Name}{(_timeOut > 0 ? $"\\TimeOut:={_timeOut:0.###}" : "")};";
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
            RAPIDGenerator.IsSynchronized = true;
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
        /// Gets or sets te max. time to wait for the other program tasks to reach the synchronization point.
        /// </summary>
        /// <remarks>
        /// Set a negative value to wait for ever (default is -1).
        /// </remarks>
        public double TimeOut
        {
            get { return _timeOut; }
            set { _timeOut = value; }
        }
        #endregion
    }
}
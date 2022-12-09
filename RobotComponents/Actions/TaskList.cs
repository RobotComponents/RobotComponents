// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
// RobotComponents Libs
using RobotComponents.Enumerations;
using RobotComponents.Definitions;
using RobotComponents.Utils;

namespace RobotComponents.Actions
{
    /// <summary>
    /// Represents a collection that specifies several RAPID program tasks.
    /// </summary>
    [Serializable()]
    public class TaskList : Action, IDeclaration, ISerializable
    {
        #region fields
        private ReferenceType _referenceType; // reference type
        private string _name; // the name of the set with tasks
        private readonly List<string> _taskNames; // the set with tasks as a list with task names
        #endregion

        #region (de)serialization
        /// <summary>
        /// Protected constructor needed for deserialization of the object.  
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization. </param>
        protected TaskList(SerializationInfo info, StreamingContext context)
        {
            // int version = (int)info.GetValue("Version", typeof(int)); // <-- use this if the (de)serialization changes
            _referenceType = (ReferenceType)info.GetValue("Reference Type", typeof(ReferenceType));
            _name = (string)info.GetValue("Name", typeof(string));
            _taskNames = (List<string>)info.GetValue("Task Names", typeof(List<string>));
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
            info.AddValue("Name", _name, typeof(string));
            info.AddValue("Task Names", _taskNames, typeof(List<string>));
        }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the Tasks class.
        /// </summary>
        public TaskList()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Tasks class.
        /// </summary>
        /// <param name="name"> The name of the set with tasks. </param>
        /// <param name="tasks"> The tasks names as a collection with strings. </param>
        public TaskList(string name, IList<string> tasks)
        {
            _referenceType = ReferenceType.PERS;
            _name = name;
            _taskNames = new List<string>(tasks);
        }

        /// <summary>
        /// Initializes a new instance of the Tasks class by duplicating an existing Tasks instance. 
        /// </summary>
        /// <param name="tasks"> The Tasks instance to duplicate. </param>
        public TaskList(TaskList tasks)
        {
            _referenceType = tasks.ReferenceType;
            _name = tasks.Name;
            _taskNames = tasks.ToList();
        }

        /// <summary>
        /// Returns an exact duplicate of this Tasks instance.
        /// </summary>
        /// <returns> A deep copy of the Tasks instance. </returns>
        public TaskList Duplicate()
        {
            return new TaskList(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Tasks instance as IDeclaration.
        /// </summary>
        /// <returns> A deep copy of the Tasks instance as an IDelcaration. </returns>
        public IDeclaration DuplicateDeclaration()
        {
            return new TaskList(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Tasks instance as an Action. 
        /// </summary>
        /// <returns> A deep copy of the Tasks instance as an Action. </returns>
        public override Action DuplicateAction()
        {
            return new TaskList(this);
        }
        #endregion

        #region method
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns> A string that represents the current object. </returns>
        public override string ToString()
        {
            if (!IsValid)
            {
                return "Invalid Tasks";
            }
            else
            {
                return $"Tasks ({_name})";
            }
        }

        /// <summary>
        /// Returns the Tasks as an array with task names.
        /// </summary>
        /// <returns> The array containing the task names. </returns>
        public string[] ToArray()
        {
            return _taskNames.ToArray();
        }

        /// <summary>
        /// Returns the Tasks as an array with task names.
        /// </summary>
        /// <returns> The array containing the task names. </returns>
        public List<string> ToList()
        {
            return _taskNames.ConvertAll(item => item);
        }

        /// <summary>
        /// Returns the RAPID declaration code line of the this action.
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> An empty string. </returns>
        public override string ToRAPIDDeclaration(Robot robot)
        {
            string result = Enum.GetName(typeof(ReferenceType), _referenceType);
            result += " tasks ";
            result += _name;
            result += "{" + _taskNames.Count.ToString() + "} := [";

            for (int i = 0; i < _taskNames.Count; i++)
            {
                result += "[\"" + _taskNames[i] + "\"]";

                if (i < _taskNames.Count - 1)
                {
                    result += ", ";
                }
            }
            
            result += "];";

            return result;
        }

        /// <summary>
        /// Returns the RAPID instruction code line of the this action. 
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> The RAPID code line. </returns>
        public override string ToRAPIDInstruction(Robot robot)
        {
            return string.Empty;
        }

        /// <summary>
        /// Creates declarations in the RAPID program module inside the RAPID Generator. 
        /// This method is called inside the RAPID generator.
        /// </summary>
        /// <param name="RAPIDGenerator"> The RAPID Generator. </param>
        public override void ToRAPIDDeclaration(RAPIDGenerator RAPIDGenerator)
        {
            if (!RAPIDGenerator.TaskLists.ContainsKey(_name))
            {
                RAPIDGenerator.TaskLists.Add(_name, this);
                RAPIDGenerator.ProgramDeclarationsMultiMove.Add("    " + ToRAPIDDeclaration(RAPIDGenerator.Robot));
            }
        }

        /// <summary>
        /// Creates instructions in the RAPID program module inside the RAPID Generator.
        /// This method is called inside the RAPID generator.
        /// </summary>
        /// <param name="RAPIDGenerator"> The RAPID Generator. </param>
        public override void ToRAPIDInstruction(RAPIDGenerator RAPIDGenerator)
        {
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
                if (_name == null) { return false; }
                if (_name == "") { return false; }
                if (_taskNames == null) { return false; }
                if (_taskNames.Count == 0) { return false; }
                return true;
            }
        }

        /// <summary>
        /// Gets or sets the reference type.
        /// </summary>
        public ReferenceType ReferenceType
        {
            get { return _referenceType; }
            set { _referenceType = value; }
        }

        /// <summary>
        /// Gets or sets the name of the set with tasks.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets the number of elements in this Tasks collection.
        /// </summary>
        public int Count
        {
            get { return _taskNames.Count; }
        }

        /// <summary>
        /// Gets or sets the task names through the indexer. 
        /// </summary>
        /// <param name="index"> The index number. </param>
        /// <returns> The task name located at the given index. </returns>
        public string this[int index]
        {
            get { return _taskNames[index]; }
            set { _taskNames[index] = value; }
        }

        /// <summary>
        /// Gets a value indicating whether or not the task collection has a fixed size.
        /// </summary>
        public bool IsFixedSize
        {
            get { return false; }
        }
        #endregion
    }
}

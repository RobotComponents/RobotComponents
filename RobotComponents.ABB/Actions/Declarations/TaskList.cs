// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text.RegularExpressions;
// RobotComponents Libs
using RobotComponents.ABB.Enumerations;
using RobotComponents.ABB.Definitions;
using RobotComponents.ABB.Actions.Interfaces;
using RobotComponents.ABB.Utils;

namespace RobotComponents.ABB.Actions.Declarations
{
    /// <summary>
    /// Represents a collection that specifies several RAPID program tasks.
    /// </summary>
    [Serializable()]
    public class TaskList : Action, IDeclaration, ISerializable
    {
        #region fields
        private Scope _scope;
        private VariableType _variableType;
        private static readonly string _datatype = "tasks";
        private string _name; 
        private readonly List<string> _taskNames; 
        #endregion

        #region (de)serialization
        /// <summary>
        /// Protected constructor needed for deserialization of the object.  
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization. </param>
        protected TaskList(SerializationInfo info, StreamingContext context)
        {
            //Version version = (Version)info.GetValue("Version", typeof(Version)); // <-- use this if the (de)serialization changes
            _scope = (Scope)info.GetValue("Scope", typeof(Scope));
            _variableType = (VariableType)info.GetValue("Variable Type", typeof(VariableType));
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
            info.AddValue("Version", VersionNumbering.Version, typeof(Version));
            info.AddValue("Scope", _scope, typeof(Scope));
            info.AddValue("Variable Type", _variableType, typeof(VariableType));
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
            _scope = Scope.GLOBAL;
            _variableType = VariableType.PERS;
            _name = name;
            _taskNames = new List<string>(tasks);
        }

        /// <summary>
        /// Initializes a new instance of the Tasks class by duplicating an existing Tasks instance. 
        /// </summary>
        /// <param name="tasks"> The Tasks instance to duplicate. </param>
        public TaskList(TaskList tasks)
        {
            _scope = tasks.Scope;
            _variableType = tasks.VariableType;
            _name = tasks.Name;
            _taskNames = tasks.ToList();
        }

        /// <summary>
        /// Returns an exact duplicate of this Tasks instance.
        /// </summary>
        /// <returns> 
        /// A deep copy of the Tasks instance. 
        /// </returns>
        public TaskList Duplicate()
        {
            return new TaskList(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Tasks instance as IDeclaration.
        /// </summary>
        /// <returns> 
        /// A deep copy of the Tasks instance as an IDelcaration. 
        /// </returns>
        public IDeclaration DuplicateDeclaration()
        {
            return new TaskList(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Tasks instance as an Action. 
        /// </summary>
        /// <returns> 
        /// A deep copy of the Tasks instance as an Action. 
        /// </returns>
        public override Action DuplicateAction()
        {
            return new TaskList(this);
        }
        #endregion

        #region parse
        /// <summary>
        /// Initializes a new instance of the Task List class from a rapid data string.
        /// </summary>
        /// <remarks>
        /// Only used for the Parse and TryParse methods. Therefore, this constructor is private. 
        /// </remarks>
        /// <param name="rapidData"> The RAPID data string. </param>
        private TaskList(string rapidData)
        {
            // Replace value between curly braces
            rapidData = Regex.Replace(rapidData, @"\{.+?\}", "");

            this.SetDataFromString(rapidData, out string[] values);

            if (values.Length == 1 & values[0] == "")
            {
                throw new InvalidCastException("Invalid RAPID data string: No task names defined.");
            }
            else
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = values[i].Replace('"', '\0');
                }
                
                _taskNames = values.ToList();
            }
        }


        /// <summary>
        /// Returns a Task List instance constructed from a RAPID data string. 
        /// </summary>
        /// <param name="rapidData"> The RAPID data string. s</param>
        public static TaskList Parse(string rapidData)
        {
            return new TaskList(rapidData);
        }

        /// <summary>
        /// Attempts to parse a RAPID data string into a Task List instance.  
        /// </summary>
        /// <param name="rapidData"> The RAPID data string. </param>
        /// <param name="taskList"> The Task List intance. </param>
        /// <returns> 
        /// True on success, false on failure. 
        /// </returns>
        public static bool TryParse(string rapidData, out TaskList taskList)
        {
            try
            {
                taskList = new TaskList(rapidData);
                return true;
            }
            catch
            {
                taskList = new TaskList();
                return false;
            }
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
        /// <returns> 
        /// The array containing the task names. 
        /// </returns>
        public string[] ToArray()
        {
            return _taskNames.ToArray();
        }

        /// <summary>
        /// Returns the Tasks as an array with task names.
        /// </summary>
        /// <returns> 
        /// The array containing the task names. 
        /// </returns>
        public List<string> ToList()
        {
            return _taskNames.ConvertAll(item => item);
        }

        /// <summary>
        /// Returns the Task List in RAPID code format. 
        /// </summary>
        /// <remarks>
        /// An example output is "["T_ROB1", "T_ROB2"]".
        /// </remarks>
        /// <returns> 
        /// The RAPID data string. 
        /// </returns>
        public string ToRAPID()
        {
            string code = "[";

            for (int i = 0; i < _taskNames.Count; i++)
            {
                code += "[\"" + _taskNames[i] + "\"]";

                if (i < _taskNames.Count - 1)
                {
                    code += ", ";
                }
                else
                {
                    code += "]";
                }
            }

            return code;
        }

        /// <summary>
        /// Returns the RAPID declaration code line of the this action.
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> 
        /// The RAPID code line. 
        /// </returns>
        public override string ToRAPIDDeclaration(Robot robot)
        {
            string result = _scope == Scope.GLOBAL ? "" : $"{Enum.GetName(typeof(Scope), _scope)} ";
            result += $"{ Enum.GetName(typeof(VariableType), _variableType)} {_datatype} {_name}";
            result += "{" + _taskNames.Count.ToString() + "} := ";
            result += $"{ ToRAPID()};";

            return result;
        }

        /// <summary>
        /// Returns the RAPID instruction code line of the this action. 
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> 
        /// An empty string.  
        /// </returns>
        public override string ToRAPIDInstruction(Robot robot)
        {
            return string.Empty;
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
            if (!RAPIDGenerator.TaskLists.ContainsKey(_name))
            {
                RAPIDGenerator.TaskLists.Add(_name, this);
                RAPIDGenerator.ProgramDeclarationsMultiMove.Add("    " + ToRAPIDDeclaration(RAPIDGenerator.Robot));
            }
        }

        /// <summary>
        /// Creates instructions in the RAPID program module inside the RAPID Generator.
        /// </summary>
        /// <remarks>
        /// This method is called inside the RAPID generator.
        /// </remarks>
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
        /// Gets or sets the scope. 
        /// </summary>
        public Scope Scope
        {
            get { return _scope; }
            set { _scope = value; }
        }

        /// <summary>
        /// Gets or sets the variable type.
        /// </summary>
        public VariableType VariableType
        {
            get { return _variableType; }
            set { _variableType = value; }
        }

        /// <summary>
        /// Gets the RAPID datatype. 
        /// </summary>
        public string Datatype
        {
            get { return _datatype; }
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
        /// <returns> 
        /// The task name located at the given index. 
        /// </returns>
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
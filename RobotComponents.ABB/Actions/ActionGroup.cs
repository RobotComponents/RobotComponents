// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
// RobotComponents Libs
using RobotComponents.ABB.Definitions;

namespace RobotComponents.ABB.Actions
{
    /// <summary>
    /// Represents a group of Actions.
    /// </summary>
    [Serializable()]
    public class ActionGroup : Action, ISerializable
    {
        #region fields
        private string _name; // the name of the signal to be changed.
        private List<Action> _actions; // the list with actions
        #endregion

        #region (de)serialization
        /// <summary>
        /// Protected constructor needed for deserialization of the object.  
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization. </param>
        protected ActionGroup(SerializationInfo info, StreamingContext context)
        {
            // int version = (int)info.GetValue("Version", typeof(int)); // <-- use this if the (de)serialization changes
            _name = (string)info.GetValue("Name", typeof(string));
            _actions = (List<Action>)info.GetValue("Actions", typeof(List<Action>));
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
            info.AddValue("Name", _name, typeof(string));
            info.AddValue("Actions", _actions, typeof(List<Action>));
        }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the Action Group class.
        /// </summary>
        public ActionGroup()
        {
            _name = "";
            _actions = new List<Action>() { };
        }

        /// <summary>
        /// Initializes a new instance of the Action Group class with an empty name.
        /// </summary>
        /// <param name="actions"> The list with actions. </param>
        public ActionGroup(IList<Action> actions)
        {
            _name = "";
            _actions = new List<Action>(actions);
        }

        /// <summary>
        /// Initializes a new instance of the Action Group class.
        /// </summary>
        /// <param name="name"> The name of the Action Group. </param>
        /// <param name="actions"> The list with actions. </param>
        public ActionGroup(string name, IList<Action> actions)
        {
            _name = name;
            _actions = new List<Action>(actions);
        }

        /// <summary>
        /// Initializes a new instance of the Action Group class by duplicating an existing Action Group instance. 
        /// </summary>
        /// <param name="group"> The Action Group instance to duplicate. </param>
        public ActionGroup(ActionGroup group)
        {
            _name = group.Name;
            _actions = group.Actions;
        }

        /// <summary>
        /// Returns an exact duplicate of this Action Group instance.
        /// </summary>
        /// <returns> A deep copy of the Action Group instance. </returns>
        public ActionGroup Duplicate()
        {
            return new ActionGroup(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Action Group instance as an Action. 
        /// </summary>
        /// <returns> A deep copy of the Action Group instance as an Action. </returns>
        public override Action DuplicateAction()
        {
            return new ActionGroup(this);
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
                return "Invalid Action Group";
            }
            if (_actions.Count == 0)
            {
                return "Empty Action Group";
            }
            else if (_name != "")
            {
                return $"Action Group ({_name})";
            }
            else
            {
                return "Action Group";
            }
        }

        /// <summary>
        /// Returns a duplicate of the list with Actions as a list.
        /// </summary>
        /// <returns> The duplicate of the list with actions. </returns>
        public List<Action> DuplicateToList()
        {
            return _actions.ConvertAll(action => action.DuplicateAction());
        }

        /// <summary>
        /// Returns a duplicate of the list with Actions as an array.
        /// </summary>
        /// <returns> The duplicate of the list with actions as an array. </returns>
        public Action[] DuplicateToArray()
        {
            return _actions.ConvertAll(action => action.DuplicateAction()).ToArray();
        }

        /// <summary>
        /// Adds and action to the end of this action group.
        /// </summary>
        /// <param name="action"></param>
        public void Add(Action action)
        {
            _actions.Add(action);
        }

        /// <summary>
        /// Adds the elements of the specified collection with actions to the end of this action group.
        /// </summary>
        /// <param name="collection"> The colleciton with actions. </param>
        public void AddRange(IList<Action> collection)
        {
            _actions.AddRange(new List<Action>(collection));
        }

        /// <summary>
        /// Removes all elements from the action group.
        /// </summary>
        public void Clear()
        {
            _actions.Clear();
        }

        /// <summary>
        /// Determines the index of a specific item in the list.
        /// </summary>
        /// <param name="action"> The object to locate in the list. </param>
        /// <returns> The index of value if found in the list; otherwise, -1. </returns>
        public int IndexOf(Action action)
        {
            return _actions.IndexOf(action);
        }

        /// <summary>
        /// Inserts an item to the list at the specified index.
        /// </summary>
        /// <param name="index"> The zero-based index at which value should be inserted. </param>
        /// <param name="action"> The object to insert into the list. </param>
        public void Insert(int index, Action action)
        {
            _actions.Insert(index, action);
        }

        /// <summary>
        /// Determines whether the list contains a specific value.
        /// </summary>
        /// <param name="action"> The object to locate in the List. </param>
        /// <returns> True if the Action is found in the list; otherwise, false. </returns>
        public bool Contains(Action action)
        {
            return _actions.Contains(action);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the List.
        /// </summary>
        /// <param name="action"> The object to remove from the IList. </param>
        public void Remove(Action action)
        {
            _actions.Remove(action);
        }

        /// <summary>
        /// Removes the list item at the specified index.
        /// </summary>
        /// <param name="index"> The zero-based index of the item to remove. </param>
        public void RemoveAt(int index)
        {
            _actions.RemoveAt(index);
        }

        /// <summary>
        /// Copies the elements of the ICollection to an Array, starting at a particular Array index.
        /// </summary>
        /// <param name="array"> The one-dimensional Array that is the destination of the elements copied from ICollection. The Array must have zero-based indexing. </param>
        /// <param name="index"> The zero-based index in array at which copying begins. </param>
        public void CopyTo(Action[] array, int index)
        {
            _actions.CopyTo(array, index);
        }

        /// <summary>
        /// Returns the actions inside this group including the actions of the groups that are inside this group.
        /// </summary>
        /// <returns> List with Actions. </returns>
        public List<Action> Ungroup()
        {
            List<Action> result = new List<Action>() { };

            for (int i = 0; i < _actions.Count; i++)
            {
                if (_actions[i] is ActionGroup group)
                {
                    result.AddRange(group.Ungroup());
                }
                else
                {
                    result.Add(_actions[i]);
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the RAPID declaration code line of the this action.
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> An empty string. </returns>
        public override string ToRAPIDDeclaration(Robot robot)
        {
            string result = "";

            for (int i = 0; i != _actions.Count; i++)
            {
                result += _actions[i].ToRAPIDDeclaration(robot);

                if (_actions[i].ToRAPIDDeclaration(robot) != string.Empty)
                {
                    result += Environment.NewLine;
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the RAPID instruction code line of the this action. 
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> The RAPID code line. </returns>
        public override string ToRAPIDInstruction(Robot robot)
        {
            string result = "";

            for (int i = 0; i != _actions.Count; i++)
            {
                result += _actions[i].ToRAPIDInstruction(robot);

                if (_actions[i].ToRAPIDInstruction(robot) != string.Empty)
                {
                    result += Environment.NewLine;
                }
            }

            return result;
        }

        /// <summary>
        /// Creates declarations in the RAPID program module inside the RAPID Generator. 
        /// This method is called inside the RAPID generator.
        /// </summary>
        /// <param name="RAPIDGenerator"> The RAPID Generator. </param>
        public override void ToRAPIDDeclaration(RAPIDGenerator RAPIDGenerator)
        {
            for (int i = 0; i < _actions.Count; i++)
            {
                _actions[i].ToRAPIDDeclaration(RAPIDGenerator);
            }
        }

        /// <summary>
        /// Creates instructions in the RAPID program module inside the RAPID Generator.
        /// This method is called inside the RAPID generator.
        /// </summary>
        /// <param name="RAPIDGenerator"> The RAPID Generator. </param>
        public override void ToRAPIDInstruction(RAPIDGenerator RAPIDGenerator)
        {
            if (_name != "")
            {
                RAPIDGenerator.ProgramInstructions.Add("    " + "    " + $"! Start of group: {_name}");
            }

            for (int i = 0; i < _actions.Count; i++)
            {
                _actions[i].ToRAPIDInstruction(RAPIDGenerator);
            }

            if (_name != "")
            {
                RAPIDGenerator.ProgramInstructions.Add("    " + "    " + $"! End of group: {_name}");
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
                if (_name == null) { return false; }
                if (_actions == null) { return false; }
                return true;
            }
        }

        /// <summary>
        /// Gets or sets the name of the Action Group.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets or sets the collection with Actions.
        /// </summary>
        public List<Action> Actions
        {
            get { return _actions; }
            set { _actions = value; }
        }

        /// <summary>
        /// Gets the number of Actions that are grouped. 
        /// </summary>
        public int Count
        {
            get { return _actions.Count; }
        }

        /// <summary>
        /// Gets or sets the Actions through the indexer. 
        /// </summary>
        /// <param name="index"> The index number. </param>
        /// <returns> The Action located at the given index. </returns>
        public Action this[int index]
        {
            get { return _actions[index]; }
            set { _actions[index] = value; }
        }

        /// <summary>
        /// Gets a value indicating whether or not the action collection has a fixed size.
        /// </summary>
        public bool IsFixedSize
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the list is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }
        #endregion
    }
}

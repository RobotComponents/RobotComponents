﻿// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// Grasshopper Libs
using Grasshopper.Kernel.Types;
using GH_IO;
using GH_IO.Serialization;
// RobotComponents Libs
using RobotComponents.ABB.Actions;
using RobotComponents.ABB.Actions.Declarations;
using RobotComponents.Utils;

namespace RobotComponents.ABB.Gh.Goos.Actions.Declarations
{
    /// <summary>
    /// Task List Goo wrapper class, makes sure the Task List class can be used in Grasshopper.
    /// </summary>
    public class GH_TaskList : GH_Goo<TaskList>, GH_ISerializable
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_TaskList()
        {
            this.Value = null;
        }

        /// <summary>
        /// Data constructor: Creates a Task List Goo instance from Task List instance.
        /// </summary>
        /// <param name="taskList"> Task List Value to store inside this Goo instance. </param>
        public GH_TaskList(TaskList taskList)
        {
            this.Value = taskList;
        }

        /// <summary>
        /// Data constructor: Creates a Task List Goo instance from another Task List Goo instance.
        /// This creates a shallow copy of the passed Task List Goo instance. 
        /// </summary>
        /// <param name="taskListGoo"> Task List Goo instance to copy. </param>
        public GH_TaskList(GH_TaskList taskListGoo)
        {
            if (taskListGoo == null)
            {
                taskListGoo = new GH_TaskList();
            }

            this.Value = taskListGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this Goo instance. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the Task List Goo. </returns>
        public override IGH_Goo Duplicate()
        {
            return new GH_TaskList(Value == null ? new TaskList() : Value.Duplicate());
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets a value indicating whether or not the current value is valid.
        /// </summary>
        public override bool IsValid
        {
            get
            {
                if (Value == null) { return false; }
                return Value.IsValid;
            }
        }

        /// <summary>
        /// Gets a string describing the state of "invalidness". 
        /// If the instance is valid, then this property should return Nothing or string.Empty.
        /// </summary>
        public override string IsValidWhyNot
        {
            get
            {
                if (Value == null) { return "No internal Task List instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid Task List instance: Did you define the Task List name and values?";
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null) { return "Null Task List"; }
            else { return Value.ToString(); }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return "Task List"; }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return "Defines a Task List"; }
        }
        #endregion

        #region casting methods
        /// <summary>
        /// Attempt a cast to type Q.
        /// </summary>
        /// <typeparam name="Q"> Type to cast to.  </typeparam>
        /// <param name="target"> Pointer to target of cast. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool CastTo<Q>(ref Q target)
        {
            //Cast to Task List Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_TaskList)))
            {
                if (Value == null) { target = (Q)(object)new GH_TaskList(); }
                else { target = (Q)(object)new GH_TaskList(Value); }
                return true;
            }

            //Cast to Task List
            if (typeof(Q).IsAssignableFrom(typeof(TaskList)))
            {
                if (Value == null) { target = (Q)(object)null; }
                else { target = (Q)(object)Value; }
                return true;
            }

            //Cast to Action Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_Action)))
            {
                if (Value == null) { target = (Q)(object)new GH_Action(); }
                else { target = (Q)(object)new GH_Action(Value); }
                return true;
            }

            //Cast to Action
            if (typeof(Q).IsAssignableFrom(typeof(IAction)))
            {
                if (Value == null) { target = (Q)(object)null; }
                else { target = (Q)(object)Value; }
                return true;
            }

            //Cast to Declaration Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_Declaration)))
            {
                if (Value == null) { target = (Q)(object)new GH_Declaration(); }
                else { target = (Q)(object)new GH_Declaration(Value); }
                return true;
            }

            //Cast to Declaration
            if (typeof(Q).IsAssignableFrom(typeof(IDeclaration)))
            {
                if (Value == null) { target = (Q)(object)null; }
                else { target = (Q)(object)Value; }
                return true;
            }

            target = default;
            return false;
        }

        /// <summary>
        /// Attempt a cast from generic object.
        /// </summary>
        /// <param name="source"> Reference to source of cast. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool CastFrom(object source)
        {
            if (source == null) { return false; }

            //Cast from Task List
            if (typeof(TaskList).IsAssignableFrom(source.GetType()))
            {
                Value = source as TaskList;
                return true;
            }

            //Cast from Action
            if (typeof(IAction).IsAssignableFrom(source.GetType()))
            {
                if (source is TaskList action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Action Goo
            if (typeof(GH_Action).IsAssignableFrom(source.GetType()))
            {
                GH_Action actionGoo = source as GH_Action;
                if (actionGoo.Value is TaskList action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Declaration
            if (typeof(IDeclaration).IsAssignableFrom(source.GetType()))
            {
                if (source is TaskList declaration)
                {
                    Value = declaration;
                    return true;
                }
            }

            //Cast from Declaration Goo
            if (typeof(GH_Declaration).IsAssignableFrom(source.GetType()))
            {
                GH_Declaration declarationGoo = source as GH_Declaration;
                if (declarationGoo.Value is TaskList declaration)
                {
                    Value = declaration;
                    return true;
                }
            }

            //Cast from Text
            if (typeof(GH_String).IsAssignableFrom(source.GetType()))
            {
                string text = (source as GH_String).Value;

                try
                {
                    Value = TaskList.Parse(text);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }
        #endregion

        #region (de)serialisation
        /// <summary>
        /// IO key for (de)serialisation of the value inside this Goo.
        /// </summary>
        private const string IoKey = "Task List";

        /// <summary>
        /// This method is called whenever the instance is required to serialize itself.
        /// </summary>
        /// <param name="writer"> Writer object to serialize with. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Write(GH_IWriter writer)
        {
            if (this.Value != null)
            {
                byte[] array = Serialization.ObjectToByteArray(this.Value);
                writer.SetByteArray(IoKey, array);
            }

            return true;
        }

        /// <summary>
        /// This method is called whenever the instance is required to deserialize itself.
        /// </summary>
        /// <param name="reader"> Reader object to deserialize from. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Read(GH_IReader reader)
        {
            if (!reader.ItemExists(IoKey))
            {
                this.Value = null;
                return true;
            }

            byte[] array = reader.GetByteArray(IoKey);
            this.Value = (TaskList)Serialization.ByteArrayToObject(array);

            return true;
        }
        #endregion
    }
}

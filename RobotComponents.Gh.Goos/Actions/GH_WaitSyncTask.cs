// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// Grasshopper Libs
using Grasshopper.Kernel.Types;
using GH_IO;
using GH_IO.Serialization;
// RobotComponents Libs
using RobotComponents.ABB.Actions;
using RobotComponents.ABB.Utils;

namespace RobotComponents.Gh.Goos.Actions
{
    /// <summary>
    /// Wait Sync Task Goo wrapper class, makes sure the Wait Sync Task class can be used in Grasshopper.
    /// </summary>
    public class GH_WaitSyncTask : GH_Goo<WaitSyncTask>, GH_ISerializable
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_WaitSyncTask()
        {
            this.Value = null;
        }

        /// <summary>
        /// Data constructor: Creates a Wait Sync Task Goo instance from Wait Sync Task instance.
        /// </summary>
        /// <param name="WaitSyncTask"> Wait Sync Task Value to store inside this Goo instance. </param>
        public GH_WaitSyncTask(WaitSyncTask waitSyncTask)
        {
            this.Value = waitSyncTask;
        }

        /// <summary>
        /// Data constructor: Creates a Wait Sync Task Goo instance from another Wait Sync Task Goo instance.
        /// This creates a shallow copy of the passed Wait Sync Task Goo instance. 
        /// </summary>
        /// <param name="waitSyncTaskGoo"> Wait Sync Task Goo instance to copy. </param>
        public GH_WaitSyncTask(GH_WaitSyncTask waitSyncTaskGoo)
        {
            if (waitSyncTaskGoo == null)
            {
                waitSyncTaskGoo = new GH_WaitSyncTask();
            }

            this.Value = waitSyncTaskGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this Goo instance. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the Wait Sync Task Goo. </returns>
        public override IGH_Goo Duplicate()
        {
            return new GH_WaitSyncTask(Value == null ? new WaitSyncTask() : Value.Duplicate());
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
                if (Value == null) { return "No internal Wait Sync Task instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid Wait Sync Task instance: Did you define the Wait Sync Task name and values?";
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null) { return "Null Wait Sync Task"; }
            else { return Value.ToString(); }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return "Wait Sync Task"; }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return "Defines a Wait Sync Task"; }
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
            //Cast to Wait Sync Task Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_WaitSyncTask)))
            {
                if (Value == null) { target = (Q)(object)new GH_WaitSyncTask(); }
                else { target = (Q)(object)new GH_WaitSyncTask(Value); }
                return true;
            }

            //Cast to Wait Sync Task
            if (typeof(Q).IsAssignableFrom(typeof(WaitSyncTask)))
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
            if (typeof(Q).IsAssignableFrom(typeof(Action)))
            {
                if (Value == null) { target = (Q)(object)null; }
                else { target = (Q)(object)Value; }
                return true;
            }

            //Cast to Instruction Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_Instruction)))
            {
                if (Value == null) { target = (Q)(object)new GH_Instruction(); }
                else { target = (Q)(object)new GH_Instruction(Value); }
                return true;
            }

            //Cast to Instruction
            if (typeof(Q).IsAssignableFrom(typeof(IInstruction)))
            {
                if (Value == null) { target = (Q)(object)null; }
                else { target = (Q)(object)Value; }
                return true;
            }

            //Cast to Syncident Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_Syncident)))
            {
                if (Value == null) { target = (Q)(object)(Q)(object)new GH_Syncident(); }
                else { target = (Q)(object)new GH_Syncident(Value); }
                return true;
            }

            //Cast to Syncident
            if (typeof(Q).IsAssignableFrom(typeof(ISyncident)))
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

            //Cast from Wait Sync Task
            if (typeof(WaitSyncTask).IsAssignableFrom(source.GetType()))
            {
                Value = source as WaitSyncTask;
                return true;
            }

            //Cast from Action
            if (typeof(Action).IsAssignableFrom(source.GetType()))
            {
                if (source is WaitSyncTask action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Action Goo
            if (typeof(GH_Action).IsAssignableFrom(source.GetType()))
            {
                GH_Action actionGoo = source as GH_Action;
                if (actionGoo.Value is WaitSyncTask action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Instruction
            if (typeof(IInstruction).IsAssignableFrom(source.GetType()))
            {
                if (source is WaitSyncTask instruction)
                {
                    Value = instruction;
                    return true;
                }
            }

            //Cast from Instruction Goo
            if (typeof(GH_Instruction).IsAssignableFrom(source.GetType()))
            {
                GH_Instruction instructionGoo = source as GH_Instruction;
                if (instructionGoo.Value is WaitSyncTask instruction)
                {
                    Value = instruction;
                    return true;
                }
            }

            //Cast from Syncident
            if (typeof(ISyncident).IsAssignableFrom(source.GetType()))
            {
                if (source is WaitSyncTask syncident)
                {
                    Value = syncident;
                    return true;
                }
            }

            //Cast from Syncident Goo
            if (typeof(GH_Syncident).IsAssignableFrom(source.GetType()))
            {
                GH_Syncident syncidentGoo = source as GH_Syncident;
                if (syncidentGoo.Value is WaitSyncTask syncident)
                {
                    Value = syncident;
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region (de)serialisation
        /// <summary>
        /// IO key for (de)serialisation of the value inside this Goo.
        /// </summary>
        private const string IoKey = "Wait Sync Task";

        /// <summary>
        /// This method is called whenever the instance is required to serialize itself.
        /// </summary>
        /// <param name="writer"> Writer object to serialize with. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Write(GH_IWriter writer)
        {
            if (this.Value != null)
            {
                byte[] array = HelperMethods.ObjectToByteArray(this.Value);
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
            this.Value = (WaitSyncTask)HelperMethods.ByteArrayToObject(array);

            return true;
        }
        #endregion
    }
}

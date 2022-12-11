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
using RobotComponents.Utils;

namespace RobotComponents.ABB.Gh.Goos.Actions
{
    /// <summary>
    /// Sync Move Off Goo wrapper class, makes sure the Sync Move Off class can be used in Grasshopper.
    /// </summary>
    public class GH_SyncMoveOff : GH_Goo<SyncMoveOff>, GH_ISerializable
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_SyncMoveOff()
        {
            this.Value = null;
        }

        /// <summary>
        /// Data constructor: Creates a Sync Move Off Goo instance from Sync Move Off instance.
        /// </summary>
        /// <param name="SyncMoveOff"> Sync Move Off Value to store inside this Goo instance. </param>
        public GH_SyncMoveOff(SyncMoveOff syncMoveOff)
        {
            this.Value = syncMoveOff;
        }

        /// <summary>
        /// Data constructor: Creates a Sync Move Off Goo instance from another Sync Move Off Goo instance.
        /// This creates a shallow copy of the passed Sync Move Off Goo instance. 
        /// </summary>
        /// <param name="syncMoveOffGoo"> Sync Move Off Goo instance to copy. </param>
        public GH_SyncMoveOff(GH_SyncMoveOff syncMoveOffGoo)
        {
            if (syncMoveOffGoo == null)
            {
                syncMoveOffGoo = new GH_SyncMoveOff();
            }

            this.Value = syncMoveOffGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this Goo instance. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the Sync Move Off Goo. </returns>
        public override IGH_Goo Duplicate()
        {
            return new GH_SyncMoveOff(Value == null ? new SyncMoveOff() : Value.Duplicate());
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
                if (Value == null) { return "No internal Sync Move Off instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid Sync Move Off instance: Did you define the Sync Move Off name and values?";
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null) { return "Null Sync Move Off"; }
            else { return Value.ToString(); }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return "Sync Move Off"; }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return "Defines a Sync Move Off"; }
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
            //Cast to Sync Move Off Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_SyncMoveOff)))
            {
                if (Value == null) { target = (Q)(object)new GH_SyncMoveOff(); }
                else { target = (Q)(object)new GH_SyncMoveOff(Value); }
                return true;
            }

            //Cast to Sync Move Off
            if (typeof(Q).IsAssignableFrom(typeof(SyncMoveOff)))
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

            //Cast from Sync Move Off
            if (typeof(SyncMoveOff).IsAssignableFrom(source.GetType()))
            {
                Value = source as SyncMoveOff;
                return true;
            }

            //Cast from Action
            if (typeof(Action).IsAssignableFrom(source.GetType()))
            {
                if (source is SyncMoveOff action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Action Goo
            if (typeof(GH_Action).IsAssignableFrom(source.GetType()))
            {
                GH_Action actionGoo = source as GH_Action;
                if (actionGoo.Value is SyncMoveOff action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Instruction
            if (typeof(IInstruction).IsAssignableFrom(source.GetType()))
            {
                if (source is SyncMoveOff instruction)
                {
                    Value = instruction;
                    return true;
                }
            }

            //Cast from Instruction Goo
            if (typeof(GH_Instruction).IsAssignableFrom(source.GetType()))
            {
                GH_Instruction instructionGoo = source as GH_Instruction;
                if (instructionGoo.Value is SyncMoveOff instruction)
                {
                    Value = instruction;
                    return true;
                }
            }

            //Cast from Syncident
            if (typeof(ISyncident).IsAssignableFrom(source.GetType()))
            {
                if (source is SyncMoveOff syncident)
                {
                    Value = syncident;
                    return true;
                }
            }

            //Cast from Syncident Goo
            if (typeof(GH_Syncident).IsAssignableFrom(source.GetType()))
            {
                GH_Syncident syncidentGoo = source as GH_Syncident;
                if (syncidentGoo.Value is SyncMoveOff syncident)
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
        private const string IoKey = "Sync Move Off";

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
            this.Value = (SyncMoveOff)Serialization.ByteArrayToObject(array);

            return true;
        }
        #endregion
    }
}

// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// Grasshopper Libs
using Grasshopper.Kernel.Types;
using GH_IO;
using GH_IO.Serialization;
// RobotComponents Libs
using RobotComponents.Actions;
using RobotComponents.Utils;

namespace RobotComponents.Gh.Goos.Actions
{
    /// <summary>
    /// Sync Move On Goo wrapper class, makes sure the Sync Move On class can be used in Grasshopper.
    /// </summary>
    public class GH_SyncMoveOn : GH_Goo<SyncMoveOn>, GH_ISerializable
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_SyncMoveOn()
        {
            this.Value = null;
        }

        /// <summary>
        /// Data constructor: Creates a Sync Move On Goo instance from Sync Move On instance.
        /// </summary>
        /// <param name="SyncMoveOn"> Sync Move On Value to store inside this Goo instance. </param>
        public GH_SyncMoveOn(SyncMoveOn syncMoveOn)
        {
            this.Value = syncMoveOn;
        }

        /// <summary>
        /// Data constructor: Creates a Sync Move On Goo instance from another Sync Move On Goo instance.
        /// This creates a shallow copy of the passed Sync Move On Goo instance. 
        /// </summary>
        /// <param name="syncMoveOnGoo"> Sync Move On Goo instance to copy. </param>
        public GH_SyncMoveOn(GH_SyncMoveOn syncMoveOnGoo)
        {
            if (syncMoveOnGoo == null)
            {
                syncMoveOnGoo = new GH_SyncMoveOn();
            }

            this.Value = syncMoveOnGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this Goo instance. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the Sync Move On Goo. </returns>
        public override IGH_Goo Duplicate()
        {
            return new GH_SyncMoveOn(Value == null ? new SyncMoveOn() : Value.Duplicate());
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
                if (Value == null) { return "No internal Sync Move On instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid Sync Move On instance: Did you define the Sync Move On name and values?";
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null) { return "Null Sync Move On"; }
            else { return Value.ToString(); }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return "Sync Move On"; }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return "Defines a Sync Move On"; }
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
            //Cast to Sync Move On
            if (typeof(Q).IsAssignableFrom(typeof(SyncMoveOn)))
            {
                if (Value == null) { target = default; }
                else { target = (Q)(object)Value; }
                return true;
            }

            //Cast to Sync Move On Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_SyncMoveOn)))
            {
                if (Value == null) { target = default; }
                else { target = (Q)(object)new GH_SyncMoveOn(Value); }
                return true;
            }

            //Cast to Action
            if (typeof(Q).IsAssignableFrom(typeof(Action)))
            {
                if (Value == null) { target = default; }
                else { target = (Q)(object)Value; }
                return true;
            }

            //Cast to Action Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_Action)))
            {
                if (Value == null) { target = default; }
                else { target = (Q)(object)new GH_Action(Value); }
                return true;
            }

            //Cast to Instruction
            if (typeof(Q).IsAssignableFrom(typeof(IInstruction)))
            {
                if (Value == null) { target = default; }
                else { target = (Q)(object)Value; }
                return true;
            }

            //Cast to Instruction Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_Instruction)))
            {
                if (Value == null) { target = default; }
                else { target = (Q)(object)new GH_Instruction(Value); }
                return true;
            }

            //Cast to Syncident
            if (typeof(Q).IsAssignableFrom(typeof(ISyncident)))
            {
                if (Value == null) { target = default; }
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

            //Cast from Sync Move On
            if (typeof(SyncMoveOn).IsAssignableFrom(source.GetType()))
            {
                Value = source as SyncMoveOn;
                return true;
            }

            //Cast from Action
            if (typeof(Action).IsAssignableFrom(source.GetType()))
            {
                if (source is SyncMoveOn action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Action Goo
            if (typeof(GH_Action).IsAssignableFrom(source.GetType()))
            {
                GH_Action actionGoo = source as GH_Action;
                if (actionGoo.Value is SyncMoveOn action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Instruction
            if (typeof(IInstruction).IsAssignableFrom(source.GetType()))
            {
                if (source is SyncMoveOn instruction)
                {
                    Value = instruction;
                    return true;
                }
            }

            //Cast from Instruction Goo
            if (typeof(GH_Instruction).IsAssignableFrom(source.GetType()))
            {
                GH_Instruction instructionGoo = source as GH_Instruction;
                if (instructionGoo.Value is SyncMoveOn instruction)
                {
                    Value = instruction;
                    return true;
                }
            }

            //Cast from Syncident
            if (typeof(ISyncident).IsAssignableFrom(source.GetType()))
            {
                if (source is SyncMoveOn syncident)
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
        private const string IoKey = "Sync Move On";

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
            this.Value = (SyncMoveOn)HelperMethods.ByteArrayToObject(array);

            return true;
        }
        #endregion
    }
}

// This file is part of RobotComponents. RobotComponents is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

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
    /// Syncident Goo wrapper class, makes sure the Syncident interface can be used in Grasshopper.
    /// </summary>
    public class GH_Syncident : GH_Goo<ISyncident>, GH_ISerializable
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_Syncident()
        {
            this.Value = null;
        }

        /// <summary>
        /// Data constructor: Creates a Syncident Goo instance from a Syncident instance.
        /// </summary>
        /// <param name="syncident"> Syncident Value to store inside this Goo instance. </param>
        public GH_Syncident(ISyncident syncident)
        {
            this.Value = syncident;
        }

        /// <summary>
        /// Data constructor: Creates a Syncident Goo instance from another Syncident Goo instance.
        /// This creates a shallow copy of the passed Syncident Goo instance. 
        /// </summary>
        /// <param name="syncidentGoo"> Syncident Goo instance to copy. </param>
        public GH_Syncident(GH_Syncident syncidentGoo)
        {
            if (syncidentGoo == null)
            {
                syncidentGoo = new GH_Syncident();
            }

            this.Value = syncidentGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this Goo instance. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the Syncident Goo. </returns>
        public override IGH_Goo Duplicate()
        {
            return new GH_Syncident(Value?.DuplicateSyncident());
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
                if (Value == null) { return "No internal Syncident instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid Syncident instance: Did you define the Syncident name and values?";
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null) { return "Null Syncident"; }
            else { return Value.ToString(); }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return "Syncident"; }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return "Defines a Syncident"; }
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
            //Cast to Syncident Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_Syncident)))
            {
                if (Value == null) { target = (Q)(object)new GH_Syncident(); }
                else { target = (Q)(object)new GH_Syncident(Value); }
                return true;
            }

            //Cast to Syncident
            if (typeof(Q).IsAssignableFrom(typeof(ISyncident)))
            {
                if (Value == null) { target = (Q)(object)null; }
                else { target = (Q)Value; }
                return true;
            }

            //Cast to Action Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_Action)))
            {
                if (Value == null) { target = (Q)(object)new GH_Action(); }
                else { target = (Q)(object)new GH_Action(Value as RobotComponents.Actions.Action); }
                return true;
            }

            //Cast to Action
            if (typeof(Q).IsAssignableFrom(typeof(RobotComponents.Actions.Action)))
            {
                if (Value == null) { target = (Q)(object)null; }
                else { target = (Q)Value; }
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

            //Cast from Syncident
            if (typeof(ISyncident).IsAssignableFrom(source.GetType()))
            {
                Value = source as ISyncident;
                return true;
            }

            //Cast from Action Goo
            if (typeof(GH_Action).IsAssignableFrom(source.GetType()))
            {
                GH_Action actionGoo = source as GH_Action;
                if (actionGoo.Value is ISyncident action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Action
            if (typeof(Action).IsAssignableFrom(source.GetType()))
            {
                if (source is ISyncident action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Instruction Goo
            if (typeof(GH_Instruction).IsAssignableFrom(source.GetType()))
            {
                GH_Instruction instructionGoo = source as GH_Instruction;
                if (instructionGoo.Value is ISyncident instruction)
                {
                    Value = instruction;
                    return true;
                }
            }

            //Cast from Instruction
            if (typeof(IInstruction).IsAssignableFrom(source.GetType()))
            {
                if (source is ISyncident instruction)
                {
                    Value = instruction;
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
        private const string IoKey = "Syncident";

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
            this.Value = (ISyncident)HelperMethods.ByteArrayToObject(array);

            return true;
        }
        #endregion
    }
}

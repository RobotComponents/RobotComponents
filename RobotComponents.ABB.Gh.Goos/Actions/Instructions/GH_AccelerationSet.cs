// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// Grasshopper Libs
using Grasshopper.Kernel.Types;
using GH_IO;
using GH_IO.Serialization;
// RobotComponents Libs
using RobotComponents.ABB.Actions;
using RobotComponents.ABB.Actions.Interfaces;
using RobotComponents.ABB.Actions.Instructions;
using RobotComponents.Utils;

namespace RobotComponents.ABB.Gh.Goos.Actions.Instructions
{
    /// <summary>
    /// Acceleration Set Goo wrapper class, makes sure the Acceleration Set class can be used in Grasshopper.
    /// </summary>
    public class GH_AccelerationSet : GH_Goo<AccelerationSet>, GH_ISerializable
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_AccelerationSet()
        {
            this.Value = null;
        }

        /// <summary>
        /// Data constructor: Creates a Acceleration Set Goo instance from a Acceleration Set instance.
        /// </summary>
        /// <param name="accelerationSet"> Acceleration Set Value to store inside this Goo instance. </param>
        public GH_AccelerationSet(AccelerationSet accelerationSet)
        {
            this.Value = accelerationSet;
        }

        /// <summary>
        /// Data constructor: Creates an Acceleration Set Goo instance from another Acceleration Set Goo instance.
        /// This creates a shallow copy of the passed Acceleration Set Goo instance. 
        /// </summary>
        /// <param name="accelerationSetGoo"> Acceleration Set Goo instance to copy. </param>
        public GH_AccelerationSet(GH_AccelerationSet accelerationSetGoo)
        {
            if (accelerationSetGoo == null)
            {
                accelerationSetGoo = new GH_AccelerationSet();
            }

            this.Value = accelerationSetGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this Goo instance. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the AccelerationSet Goo. </returns>
        public override IGH_Goo Duplicate()
        {
            return new GH_AccelerationSet(Value == null ? new AccelerationSet() : Value.Duplicate());
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
                if (Value == null) { return "No internal Acceleration Set instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid Acceleration Set instance: Did you define the acceleration and ramp value?";  
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null) { return "Null Acceleration Set"; }
            else { return Value.ToString(); }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return ("Acceleration Set"); }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return ("Defines a Acceleration Set."); }
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
            //Cast to Acceleration Set Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_AccelerationSet)))
            {
                if (Value == null) { target = (Q)(object)new GH_AccelerationSet(); }
                else { target = (Q)(object)new GH_AccelerationSet(Value); }
                return true;
            }

            //Cast to Acceleration Set
            if (typeof(Q).IsAssignableFrom(typeof(AccelerationSet)))
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

            //Cast from Acceleration Set
            if (typeof(AccelerationSet).IsAssignableFrom(source.GetType()))
            {
                Value = source as AccelerationSet;
                return true;
            }

            //Cast from Action
            if (typeof(Action).IsAssignableFrom(source.GetType()))
            {
                if (source is AccelerationSet action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Action Goo
            if (typeof(GH_Action).IsAssignableFrom(source.GetType()))
            {
                GH_Action actionGoo = source as GH_Action;
                if (actionGoo.Value is AccelerationSet action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Instruction
            if (typeof(IInstruction).IsAssignableFrom(source.GetType()))
            {
                if (source is AccelerationSet instruction)
                {
                    Value = instruction;
                    return true;
                }
            }

            //Cast from Instruction Goo
            if (typeof(GH_Instruction).IsAssignableFrom(source.GetType()))
            {
                GH_Instruction instructionGoo = source as GH_Instruction;
                if (instructionGoo.Value is AccelerationSet instruction)
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
        private const string IoKey = "Acceleration Set";
        
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
            this.Value = (AccelerationSet)Serialization.ByteArrayToObject(array);

            return true;
        }
        #endregion
    }
}

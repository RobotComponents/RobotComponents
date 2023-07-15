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
using RobotComponents.ABB.Actions.Interfaces;
using RobotComponents.ABB.Actions.Instructions;
using RobotComponents.Utils;

namespace RobotComponents.ABB.Gh.Goos.Actions.Instructions
{
    /// <summary>
    /// Set Digital Output Goo wrapper class, makes sure the Set Digital Output class can be used in Grasshopper.
    /// </summary>
    public class GH_SetDigitalOutput : GH_Goo<SetDigitalOutput>, GH_ISerializable
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_SetDigitalOutput()
        {
            this.Value = null;
        }

        /// <summary>
        /// Data constructor: Creates a Set Digital Output Goo instance from Set Digital Ouput instance.
        /// </summary>
        /// <param name="digitalOutput"> Set Digital Output Value to store inside this Goo instance. </param>
        public GH_SetDigitalOutput(SetDigitalOutput digitalOutput)
        {
            this.Value = digitalOutput;
        }

        /// <summary>
        /// Data constructor: Creates a Set Digital Output Goo instance from another Set Digital Output Goo instance.
        /// This creates a shallow copy of the passed Digital Output Goo instance. 
        /// </summary>
        /// <param name="digitalOutputGoo"> Set Digital Output Goo instance to copy. </param>
        public GH_SetDigitalOutput(GH_SetDigitalOutput digitalOutputGoo)
        {
            if (digitalOutputGoo == null)
            {
                digitalOutputGoo = new GH_SetDigitalOutput();
            }

            this.Value = digitalOutputGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this Goo instance. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the Set Digital Output Goo. </returns>
        public override IGH_Goo Duplicate()
        {
            return new GH_SetDigitalOutput(Value == null ? new SetDigitalOutput() : Value.Duplicate());
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
                if (Value == null) { return "No internal Set Digital Output instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid Set Digital Output instance: Did you define the digital output name and state?";
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null) { return "Null Set Digital Output"; }
            else { return Value.ToString(); }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return "Set Digital Output"; }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return "Defines a Set Digital Output"; }
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
            //Cast to Set Digital Output Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_SetDigitalOutput)))
            {
                if (Value == null) { target = (Q)(object)new GH_SetDigitalOutput(); }
                else { target = (Q)(object)new GH_SetDigitalOutput(Value); }
                return true;
            }

            //Cast to Digital Output
            if (typeof(Q).IsAssignableFrom(typeof(SetDigitalOutput)))
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

            //Cast to Boolean
            if (typeof(Q).IsAssignableFrom(typeof(GH_Boolean)))
            {
                if (Value == null) { target = default; }
                else { target = (Q)(object)new GH_Boolean(Value.Value); }
                return true;
            }

            //Cast to Digital Output
            if (typeof(Q).IsAssignableFrom(typeof(DigitalOutput)))
            {
                if (Value == null) { target = (Q)(object)null; }
                else { target = (Q)(object)new DigitalOutput(Value.Name, Value.Value); }
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

            //Cast from Set Digital Output Goo
            if (typeof(GH_SetDigitalOutput).IsAssignableFrom(source.GetType()))
            {
                GH_SetDigitalOutput setDigitalOutputGoo = source as GH_SetDigitalOutput;
                Value = setDigitalOutputGoo.Value;
                return true;
            }

            //Cast from Set Digital Output
            if (typeof(SetDigitalOutput).IsAssignableFrom(source.GetType()))
            {
                Value = source as SetDigitalOutput;
                return true;
            }

            //Cast from Action
            if (typeof(Action).IsAssignableFrom(source.GetType()))
            {
                if (source is SetDigitalOutput action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Action Goo
            if (typeof(GH_Action).IsAssignableFrom(source.GetType()))
            {
                GH_Action actionGoo = source as GH_Action;
                if (actionGoo.Value is SetDigitalOutput action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Instruction
            if (typeof(IInstruction).IsAssignableFrom(source.GetType()))
            {
                if (source is SetDigitalOutput instruction)
                {
                    Value = instruction;
                    return true;
                }
            }

            //Cast from Instruction Goo
            if (typeof(GH_Instruction).IsAssignableFrom(source.GetType()))
            {
                GH_Instruction instructionGoo = source as GH_Instruction;
                if (instructionGoo.Value is SetDigitalOutput instruction)
                {
                    Value = instruction;
                    return true;
                }
            }

            //Cast from Digital Output Goo
            if (typeof(GH_DigitalOutput).IsAssignableFrom(source.GetType()))
            {
                GH_DigitalOutput digitalOutputGoo = source as GH_DigitalOutput;
                Value = new SetDigitalOutput(digitalOutputGoo.Value.Name, digitalOutputGoo.Value.IsActive);
                return true;
            }

            //Cast from Digital Output
            if (typeof(DigitalOutput).IsAssignableFrom(source.GetType()))
            {
                DigitalOutput digitalOutput = source as DigitalOutput;
                Value = new SetDigitalOutput(digitalOutput.Name, digitalOutput.IsActive);
                return true;
            }

            return false;
        }
        #endregion

        #region (de)serialisation
        /// <summary>
        /// IO key for (de)serialisation of the value inside this Goo.
        /// </summary>
        private const string IoKey = "Set Digital Output";

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
            this.Value = (SetDigitalOutput)Serialization.ByteArrayToObject(array);

            return true;
        }
        #endregion
    }
}

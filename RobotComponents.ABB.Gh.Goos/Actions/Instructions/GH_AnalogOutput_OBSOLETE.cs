// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
// Grasshopper Libs
using Grasshopper.Kernel.Types;
using GH_IO;
using GH_IO.Serialization;
// RobotComponents Libs
using RobotComponents.ABB.Actions.Interfaces;
using RobotComponents.ABB.Actions.Instructions;
using RobotComponents.Utils;

namespace RobotComponents.ABB.Gh.Goos.Actions.Instructions
{
    /// <summary>
    /// Analog Output Goo wrapper class, makes sure the Analog Output class can be used in Grasshopper.
    /// </summary>
    [Obsolete("This class is obsolete and will be removed in v3. Use GH_SetAnalogOutput instead.", false)]
    public class GH_AnalogOutput : GH_Goo<AnalogOutput>, GH_ISerializable
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_AnalogOutput()
        {
            this.Value = null;
        }

        /// <summary>
        /// Data constructor: Creates a Analog Output Goo instance from Analog Ouput instance.
        /// </summary>
        /// <param name="analogOutput"> Analog Output Value to store inside this Goo instance. </param>
        public GH_AnalogOutput(AnalogOutput analogOutput)
        {
            this.Value = analogOutput;
        }

        /// <summary>
        /// Data constructor: Creates a Analog Output Goo instance from another Analog Output Goo instance.
        /// This creates a shallow copy of the passed Analog Output Goo instance. 
        /// </summary>
        /// <param name="analogOutputGoo"> Analog Output Goo instance to copy. </param>
        public GH_AnalogOutput(GH_AnalogOutput analogOutputGoo)
        {
            if (analogOutputGoo == null)
            {
                analogOutputGoo = new GH_AnalogOutput();
            }

            this.Value = analogOutputGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this Goo instance. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the Analog Output Goo. </returns>
        public override IGH_Goo Duplicate()
        {
            return new GH_AnalogOutput(Value == null ? new AnalogOutput() : Value.Duplicate());
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
                if (Value == null) { return "No internal Analog Output instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid Analog Output instance: Did you define the analog output name and value?";
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null) { return "Null Analog Output"; }
            else { return Value.ToString(); }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return "Analog Output"; }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return "Defines a Analog Output"; }
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
            //Cast to Analog Output Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_AnalogOutput)))
            {
                if (Value == null) { target = (Q)(object)new GH_AnalogOutput(); }
                else { target = (Q)(object)new GH_AnalogOutput(Value); }
                return true;
            }

            //Cast to Analog Output
            if (typeof(Q).IsAssignableFrom(typeof(AnalogOutput)))
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
            if (typeof(Q).IsAssignableFrom(typeof(ABB.Actions.Action)))
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

            //Cast to Number
            if (typeof(Q).IsAssignableFrom(typeof(GH_Number)))
            {
                if (Value == null) { target = default; }
                else { target = (Q)(object)new GH_Number(Value.Value); }
                return true;
            }

            //Cast to Set Analog Output
            if (typeof(Q).IsAssignableFrom(typeof(SetAnalogOutput)))
            {
                if (Value == null) { target = (Q)(object)null; }
                else { target = (Q)(object)new SetAnalogOutput(Value.Name, Value.Value); }
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

            //Cast from Analog Output Goo
            if (typeof(GH_AnalogOutput).IsAssignableFrom(source.GetType()))
            {
                GH_AnalogOutput analogOutputGoo = source as GH_AnalogOutput;
                Value = analogOutputGoo.Value;
                return true;
            }

            //Cast from Analog Output
            if (typeof(AnalogOutput).IsAssignableFrom(source.GetType()))
            {
                Value = source as AnalogOutput;
                return true;
            }

            //Cast from Action
            if (typeof(ABB.Actions.Action).IsAssignableFrom(source.GetType()))
            {
                if (source is AnalogOutput action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Action Goo
            if (typeof(GH_Action).IsAssignableFrom(source.GetType()))
            {
                GH_Action actionGoo = source as GH_Action;
                if (actionGoo.Value is AnalogOutput action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Instruction
            if (typeof(IInstruction).IsAssignableFrom(source.GetType()))
            {
                if (source is AnalogOutput instruction)
                {
                    Value = instruction;
                    return true;
                }
            }

            //Cast from Instruction Goo
            if (typeof(GH_Instruction).IsAssignableFrom(source.GetType()))
            {
                GH_Instruction instructionGoo = source as GH_Instruction;
                if (instructionGoo.Value is AnalogOutput instruction)
                {
                    Value = instruction;
                    return true;
                }
            }

            //Cast from Set Analog Output Goo
            if (typeof(GH_SetAnalogOutput).IsAssignableFrom(source.GetType()))
            {
                GH_SetAnalogOutput setAnalogOutputGoo = source as GH_SetAnalogOutput;
                Value = new AnalogOutput(setAnalogOutputGoo.Value.Name, setAnalogOutputGoo.Value.Value);
                return true;
            }

            //Cast from Set Analog Output
            if (typeof(SetAnalogOutput).IsAssignableFrom(source.GetType()))
            {
                SetAnalogOutput setAnalogOutput = source as SetAnalogOutput;
                Value = new AnalogOutput(setAnalogOutput.Name, setAnalogOutput.Value);
                return true;
            }

            return false;
        }
        #endregion

        #region (de)serialisation
        /// <summary>
        /// IO key for (de)serialisation of the value inside this Goo.
        /// </summary>
        private const string IoKey = "Analog Output";

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
            this.Value = (AnalogOutput)Serialization.ByteArrayToObject(array);

            return true;
        }
        #endregion
    }
}

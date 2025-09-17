// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components
// Project: https://github.com/RobotComponents/RobotComponents
//
// Copyright (c) 2020-2025 Arjen Deetman
//
// Authors:
//   - Arjen Deetman (2020-2025)
//
// For license details, see the LICENSE file in the project root.

// Grasshopper Libs
using Grasshopper.Kernel.Types;
using GH_IO;
using GH_IO.Serialization;
// RobotComponents Libs
using RobotComponents.ABB.Actions;
using RobotComponents.ABB.Actions.Instructions;

namespace RobotComponents.ABB.Gh.Goos.Actions.Instructions
{
    /// <summary>
    /// Set Analog Output Goo wrapper class, makes sure the Set Analog Output class can be used in Grasshopper.
    /// </summary>
    public class GH_SetAnalogOutput : GH_Goo<SetAnalogOutput>, GH_ISerializable
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_SetAnalogOutput()
        {
            this.Value = null;
        }

        /// <summary>
        /// Data constructor: Creates a Set Analog Output Goo instance from Analog Ouput instance.
        /// </summary>
        /// <param name="analogOutput"> Set Analog Output Value to store inside this Goo instance. </param>
        public GH_SetAnalogOutput(SetAnalogOutput analogOutput)
        {
            this.Value = analogOutput;
        }

        /// <summary>
        /// Data constructor: Creates a Set Analog Output Goo instance from another Set Analog Output Goo instance.
        /// This creates a shallow copy of the passed Analog Output Goo instance. 
        /// </summary>
        /// <param name="analogOutputGoo"> Set Analog Output Goo instance to copy. </param>
        public GH_SetAnalogOutput(GH_SetAnalogOutput analogOutputGoo)
        {
            if (analogOutputGoo == null)
            {
                analogOutputGoo = new GH_SetAnalogOutput();
            }

            this.Value = analogOutputGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this Goo instance. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the Analog Output Goo. </returns>
        public override IGH_Goo Duplicate()
        {
            return new GH_SetAnalogOutput(Value == null ? new SetAnalogOutput() : Value.Duplicate());
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
                if (Value == null) { return "No internal Set Analog Output instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid Set Analog Output instance: Did you define the analog output name and value?";
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null) { return "Null Set Analog Output"; }
            else { return Value.ToString(); }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return "Set Analog Output"; }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return "Defines a Set Analog Output"; }
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
            //Cast to Set Analog Output Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_SetAnalogOutput)))
            {
                if (Value == null) { target = (Q)(object)new GH_SetAnalogOutput(); }
                else { target = (Q)(object)new GH_SetAnalogOutput(Value); }
                return true;
            }

            //Cast to Set Analog Output
            if (typeof(Q).IsAssignableFrom(typeof(SetAnalogOutput)))
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

            //Cast to Analog Output
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

            //Cast from Set Analog Output Goo
            if (typeof(GH_SetAnalogOutput).IsAssignableFrom(source.GetType()))
            {
                GH_SetAnalogOutput setAnalogOutputGoo = source as GH_SetAnalogOutput;
                Value = setAnalogOutputGoo.Value;
                return true;
            }

            //Cast from Set Analog Output
            if (typeof(SetAnalogOutput).IsAssignableFrom(source.GetType()))
            {
                Value = source as SetAnalogOutput;
                return true;
            }

            //Cast from Action
            if (typeof(IAction).IsAssignableFrom(source.GetType()))
            {
                if (source is SetAnalogOutput action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Action Goo
            if (typeof(GH_Action).IsAssignableFrom(source.GetType()))
            {
                GH_Action actionGoo = source as GH_Action;
                if (actionGoo.Value is SetAnalogOutput action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Instruction
            if (typeof(IInstruction).IsAssignableFrom(source.GetType()))
            {
                if (source is SetAnalogOutput instruction)
                {
                    Value = instruction;
                    return true;
                }
            }

            //Cast from Instruction Goo
            if (typeof(GH_Instruction).IsAssignableFrom(source.GetType()))
            {
                GH_Instruction instructionGoo = source as GH_Instruction;
                if (instructionGoo.Value is SetAnalogOutput instruction)
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
        private const string IoKey = "Set Analog Output";

        /// <summary>
        /// This method is called whenever the instance is required to serialize itself.
        /// </summary>
        /// <param name="writer"> Writer object to serialize with. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Write(GH_IWriter writer)
        {
            if (this.Value != null)
            {
                byte[] array = RobotComponents.Utils.Serialization.ObjectToByteArray(this.Value);
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
            this.Value = (SetAnalogOutput)RobotComponents.Utils.Serialization.ByteArrayToObject(array);

            return true;
        }
        #endregion
    }
}

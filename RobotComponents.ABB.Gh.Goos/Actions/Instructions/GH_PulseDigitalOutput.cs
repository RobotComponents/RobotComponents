// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components
// Project: https://github.com/RobotComponents/RobotComponents
//
// Copyright (c) 2023-2025 Arjen Deetman
//
// Authors:
//   - Arjen Deetman (2023-2025)
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
    /// Pulse Digital Output Goo wrapper class, makes sure the Pulse Digital Output class can be used in Grasshopper.
    /// </summary>
    public class GH_PulseDigitalOutput : GH_Goo<PulseDigitalOutput>, GH_ISerializable
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_PulseDigitalOutput()
        {
            this.Value = null;
        }

        /// <summary>
        /// Data constructor: Creates a Pulse Digital Output Goo instance from Set Digital Ouput instance.
        /// </summary>
        /// <param name="digitalOutput"> Pulse Digital Output Value to store inside this Goo instance. </param>
        public GH_PulseDigitalOutput(PulseDigitalOutput digitalOutput)
        {
            this.Value = digitalOutput;
        }

        /// <summary>
        /// Data constructor: Creates a Pulse Digital Output Goo instance from another Pulse Digital Output Goo instance.
        /// This creates a shallow copy of the passed Digital Output Goo instance. 
        /// </summary>
        /// <param name="digitalOutputGoo"> Pulse Digital Output Goo instance to copy. </param>
        public GH_PulseDigitalOutput(GH_PulseDigitalOutput digitalOutputGoo)
        {
            if (digitalOutputGoo == null)
            {
                digitalOutputGoo = new GH_PulseDigitalOutput();
            }

            this.Value = digitalOutputGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this Goo instance. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the Pulse Digital Output Goo. </returns>
        public override IGH_Goo Duplicate()
        {
            return new GH_PulseDigitalOutput(Value == null ? new PulseDigitalOutput() : Value.Duplicate());
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
                if (Value == null) { return "No internal Pulse Digital Output instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid Pulse Digital Output instance: Did you define the digital output name and pulse length?";
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null) { return "Null Pulse Digital Output"; }
            else { return Value.ToString(); }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return "Pulse Digital Output"; }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return "Defines a Pulse Digital Output"; }
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
            //Cast to Pulse Digital Output Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_PulseDigitalOutput)))
            {
                if (Value == null) { target = (Q)(object)new GH_PulseDigitalOutput(); }
                else { target = (Q)(object)new GH_PulseDigitalOutput(Value); }
                return true;
            }

            //Cast to Digital Output
            if (typeof(Q).IsAssignableFrom(typeof(PulseDigitalOutput)))
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

            //Cast to Boolean
            if (typeof(Q).IsAssignableFrom(typeof(GH_Boolean)))
            {
                if (Value == null) { target = default; }
                else { target = (Q)(object)new GH_Boolean(Value.High); }
                return true;
            }

            //Cast to Number
            if (typeof(Q).IsAssignableFrom(typeof(GH_Number)))
            {
                if (Value == null) { target = default; }
                else { target = (Q)(object)new GH_Number(Value.Length); }
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

            //Cast from Pulse Digital Output Goo
            if (typeof(GH_PulseDigitalOutput).IsAssignableFrom(source.GetType()))
            {
                GH_PulseDigitalOutput pulseDigitalOutputGoo = source as GH_PulseDigitalOutput;
                Value = pulseDigitalOutputGoo.Value;
                return true;
            }

            //Cast from Pulse Digital Output
            if (typeof(PulseDigitalOutput).IsAssignableFrom(source.GetType()))
            {
                Value = source as PulseDigitalOutput;
                return true;
            }

            //Cast from Action
            if (typeof(IAction).IsAssignableFrom(source.GetType()))
            {
                if (source is PulseDigitalOutput action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Action Goo
            if (typeof(GH_Action).IsAssignableFrom(source.GetType()))
            {
                GH_Action actionGoo = source as GH_Action;
                if (actionGoo.Value is PulseDigitalOutput action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Instruction
            if (typeof(IInstruction).IsAssignableFrom(source.GetType()))
            {
                if (source is PulseDigitalOutput instruction)
                {
                    Value = instruction;
                    return true;
                }
            }

            //Cast from Instruction Goo
            if (typeof(GH_Instruction).IsAssignableFrom(source.GetType()))
            {
                GH_Instruction instructionGoo = source as GH_Instruction;
                if (instructionGoo.Value is PulseDigitalOutput instruction)
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
        private const string IoKey = "Pulse Digital Output";

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
            this.Value = (PulseDigitalOutput)RobotComponents.Utils.Serialization.ByteArrayToObject(array);

            return true;
        }
        #endregion
    }
}

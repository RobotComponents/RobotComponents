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
using RobotComponents.ABB.Enumerations;

namespace RobotComponents.ABB.Gh.Goos.Actions.Instructions
{
    /// <summary>
    /// Circle Path Mode Goo wrapper class, makes sure the Circle Path Mode class can be used in Grasshopper.
    /// </summary>
    public class GH_CirclePathMode : GH_Goo<CirclePathMode>, GH_ISerializable
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_CirclePathMode()
        {
            this.Value = null;
        }

        /// <summary>
        /// Data constructor: Creates a Circle Path Mode Goo instance from Circle Path Mode instance.
        /// </summary>
        /// <param name="circlePathMode"> Circle Path Mode Value to store inside this Goo instance. </param>
        public GH_CirclePathMode(CirclePathMode circlePathMode)
        {
            this.Value = circlePathMode;
        }

        /// <summary>
        /// Data constructor: Creates a Circle Path Mode Goo instance from another Circle Path Mode Goo instance.
        /// This creates a shallow copy of the passed Circle Path Mode Goo instance. 
        /// </summary>
        /// <param name="circlePathModeGoo"> Circle Path Mode Goo instance to copy. </param>
        public GH_CirclePathMode(GH_CirclePathMode circlePathModeGoo)
        {
            if (circlePathModeGoo == null)
            {
                circlePathModeGoo = new GH_CirclePathMode();
            }

            this.Value = circlePathModeGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this Goo instance. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the Circle Path Mode Goo. </returns>
        public override IGH_Goo Duplicate()
        {
            return new GH_CirclePathMode(Value == null ? new CirclePathMode() : Value.Duplicate());
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
                if (Value == null) { return "No internal Circle Path Mode instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid Circle Path Mode instance: Did you define the Circle Path Mode value?";
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null) { return "Null Circle Path Mode"; }
            else { return Value.ToString(); }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return "Circle Path Mode"; }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return "Defines a Circle Path Mode"; }
        }
        #endregion

        #region casting methods
        /// <summary>
        /// Attempt a cast to type Q.
        /// </summary>
        /// <typeparam name="Q"> Type to cast to. </typeparam>
        /// <param name="target"> Pointer to target of cast. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool CastTo<Q>(ref Q target)
        {
            //Cast to Circle Path Mode Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_CirclePathMode)))
            {
                if (Value == null) { target = (Q)(object)new GH_CirclePathMode(); }
                else { target = (Q)(object)new GH_CirclePathMode(Value); }
                return true;
            }

            //Cast to Circle Path Mode
            if (typeof(Q).IsAssignableFrom(typeof(CirclePathMode)))
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
                if (Value == null) { target = default; }
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

            //Cast from Circle Path Mode
            if (typeof(CirclePathMode).IsAssignableFrom(source.GetType()))
            {
                Value = source as CirclePathMode;
                return true;
            }

            //Cast from Action
            if (typeof(IAction).IsAssignableFrom(source.GetType()))
            {
                if (source is CirclePathMode action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Action Goo
            if (typeof(GH_Action).IsAssignableFrom(source.GetType()))
            {
                GH_Action actionGoo = source as GH_Action;
                if (actionGoo.Value is CirclePathMode action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Instruction
            if (typeof(IInstruction).IsAssignableFrom(source.GetType()))
            {
                if (source is CirclePathMode instruction)
                {
                    Value = instruction;
                    return true;
                }
            }

            //Cast from Instruction Goo
            if (typeof(GH_Instruction).IsAssignableFrom(source.GetType()))
            {
                GH_Instruction instructionGoo = source as GH_Instruction;
                if (instructionGoo.Value is CirclePathMode instruction)
                {
                    Value = instruction;
                    return true;
                }
            }

            //Cast from Integer
            if (typeof(GH_Integer).IsAssignableFrom(source.GetType()))
            {
                int mode = (source as GH_Integer).Value;
                Value = new CirclePathMode((CirPathMode)mode);
                return true;
            }

            return false;
        }
        #endregion

        #region (de)serialisation
        /// <summary>
        /// IO key for (de)serialisation of the value inside this Goo.
        /// </summary>
        private const string IoKey = "Circle Path Mode";

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
            this.Value = (CirclePathMode)RobotComponents.Utils.Serialization.ByteArrayToObject(array);

            return true;
        }
        #endregion
    }
}

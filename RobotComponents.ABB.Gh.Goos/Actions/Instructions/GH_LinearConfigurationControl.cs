﻿// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// Grasshopper Libs
using Grasshopper.Kernel.Types;
using GH_IO;
using GH_IO.Serialization;
// RobotComponents Libs
using RobotComponents.ABB.Actions;
using RobotComponents.ABB.Actions.Instructions;
using RobotComponents.Utils;

namespace RobotComponents.ABB.Gh.Goos.Actions.Instructions
{
    /// <summary>
    /// Linear Configuration Control Goo wrapper class, makes sure the Linear Configuration Control class can be used in Grasshopper.
    /// </summary>
    public class GH_LinearConfigurationControl : GH_Goo<LinearConfigurationControl>, GH_ISerializable
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_LinearConfigurationControl()
        {
            this.Value = null;
        }

        /// <summary>
        /// Data constructor: Creates a Linear Configuration Control Goo instance from a Linear Configuration Control instance.
        /// </summary>
        /// <param name="linearConfigurationControl"> Linear Configuration Control Value to store inside this Goo instance. </param>
        public GH_LinearConfigurationControl(LinearConfigurationControl linearConfigurationControl)
        {
            this.Value = linearConfigurationControl;
        }

        /// <summary>
        /// Data constructor: Creates an Linear Configuration Control Goo instance from another Linear Configuration Control Goo instance.
        /// This creates a shallow copy of the passed Linear Configuration Control Goo instance. 
        /// </summary>
        /// <param name="linearConfigurationControlGoo"> Linear Configuration Control Goo instance to copy. </param>
        public GH_LinearConfigurationControl(GH_LinearConfigurationControl linearConfigurationControlGoo)
        {
            if (linearConfigurationControlGoo == null)
            {
                linearConfigurationControlGoo = new GH_LinearConfigurationControl();
            }

            this.Value = linearConfigurationControlGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this Goo instance. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the LinearConfigurationControl Goo. </returns>
        public override IGH_Goo Duplicate()
        {
            return new GH_LinearConfigurationControl(Value == null ? new LinearConfigurationControl() : Value.Duplicate());
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
                if (Value == null) { return "No internal Linear Configuration Control instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid Linear Configuration Control instance: Did you set a bool?";
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null) { return "Null Linear Configuration Control"; }
            else { return Value.ToString(); }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return "Linear Configuration Control"; }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return "Defines a Linear Configuration Control."; }
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
            //Cast to Linear Configuration Control Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_LinearConfigurationControl)))
            {
                if (Value == null) { target = (Q)(object)new GH_LinearConfigurationControl(); }
                else { target = (Q)(object)new GH_LinearConfigurationControl(Value); }
                return true;
            }

            //Cast to Linear Configuration Control
            if (typeof(Q).IsAssignableFrom(typeof(LinearConfigurationControl)))
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
                if (Value == null) { target = (Q)(object)null; ; }
                else { target = (Q)(object)Value; }
                return true;
            }

            //Cast to Boolean
            if (typeof(Q).IsAssignableFrom(typeof(GH_Boolean)))
            {
                if (Value == null) { target = default; }
                else { target = (Q)(object)new GH_Boolean(Value.IsActive); }
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

            //Cast from Linear Configuration Control
            if (typeof(LinearConfigurationControl).IsAssignableFrom(source.GetType()))
            {
                Value = source as LinearConfigurationControl;
                return true;
            }

            //Cast from Boolean
            if (typeof(GH_Boolean).IsAssignableFrom(source.GetType()))
            {
                GH_Boolean ghBoolean = source as GH_Boolean;
                Value = new LinearConfigurationControl(ghBoolean.Value);
                return true;
            }

            //Cast from Action
            if (typeof(IAction).IsAssignableFrom(source.GetType()))
            {
                if (source is LinearConfigurationControl action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Action Goo
            if (typeof(GH_Action).IsAssignableFrom(source.GetType()))
            {
                GH_Action actionGoo = source as GH_Action;
                if (actionGoo.Value is LinearConfigurationControl action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Instruction
            if (typeof(IInstruction).IsAssignableFrom(source.GetType()))
            {
                if (source is LinearConfigurationControl instruction)
                {
                    Value = instruction;
                    return true;
                }
            }

            //Cast from Instruction Goo
            if (typeof(GH_Instruction).IsAssignableFrom(source.GetType()))
            {
                GH_Instruction instructionGoo = source as GH_Instruction;
                if (instructionGoo.Value is LinearConfigurationControl instruction)
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
        private const string IoKey = "Linear Configuration Control";

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
            this.Value = (LinearConfigurationControl)Serialization.ByteArrayToObject(array);

            return true;
        }
        #endregion
    }
}

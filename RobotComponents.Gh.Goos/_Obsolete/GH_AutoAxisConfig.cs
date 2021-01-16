// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
// Grasshopper Libs
using Grasshopper.Kernel.Types;
using GH_IO;
using GH_IO.Serialization;
// RobotComponents Libs
using RobotComponents.Actions;
using RobotComponents.Utils;
using RobotComponents.Gh.Goos.Actions;

namespace RobotComponents.Gh.Goos.Obsolete
{
    /// <summary>
    /// Auto Axis Configuration Goo wrapper class, makes sure the Auto Axis Configuration class can be used in Grasshopper.
    /// </summary>
    [Obsolete("This class is obsolete and will be removed in the future.", false)]
    public class GH_AutoAxisConfig : GH_Goo<AutoAxisConfig>, GH_ISerializable
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_AutoAxisConfig()
        {
            this.Value = null;
        }

        /// <summary>
        /// Data constructor: Creates an Auto Axis Configuration Goo instance from an Auto Axis Configuration instance.
        /// </summary>
        /// <param name="config"> Auto Axis Config Value to store inside this Goo instance. </param>
        public GH_AutoAxisConfig(AutoAxisConfig config)
        {
            this.Value = config;
        }

        /// <summary>
        /// Data constructor: Creates an Auto Axis Configuration Goo instance from another Auto Axis Configuration Goo instance.
        /// This creates a shallow copy of the passed Auto Axis Configuration Goo instance. 
        /// </summary>
        /// <param name="configGoo"> Auto Axis Config Goo instance to copy. </param>
        public GH_AutoAxisConfig(GH_AutoAxisConfig configGoo)
        {
            if (configGoo == null)
            {
                configGoo = new GH_AutoAxisConfig();
            }

            this.Value = configGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this Goo instance. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the AutoAxisConfigGoo. </returns>
        public override IGH_Goo Duplicate()
        {
            return new GH_AutoAxisConfig(Value == null ? new AutoAxisConfig() : Value.Duplicate());
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
        /// If the instance is valid, then this property should return Nothing or String.Empty.
        /// </summary>
        public override string IsValidWhyNot
        {
            get
            {
                if (Value == null) { return "No internal Auto Axis Configuration instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid Auto Axis Configuration instance: Did you set a bool?"; //Todo: beef this up to be more informative.
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null) { return "Null Auto Axis Configuration"; }
            else { return Value.ToString(); }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return ("Auto Axis Configuration"); }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return ("Defines an Auto Axis Configuration."); }
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
            //Cast to Auto Axis Config
            if (typeof(Q).IsAssignableFrom(typeof(AutoAxisConfig)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)Value; }
                return true;
            }

            //Cast to Auto Axis Config Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_AutoAxisConfig)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)new GH_AutoAxisConfig(Value); }
                return true;
            }

            //Cast to Action
            if (typeof(Q).IsAssignableFrom(typeof(RobotComponents.Actions.Action)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)Value; }
                return true;
            }

            //Cast to Action Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_Action)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)new GH_Action(Value); }
                return true;
            }

            //Cast to Instruction
            if (typeof(Q).IsAssignableFrom(typeof(IInstruction)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)Value; }
                return true;
            }

            //Cast to Instruction Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_Instruction)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)new GH_Instruction(Value); }
                return true;
            }

            //Cast to Boolean
            if (typeof(Q).IsAssignableFrom(typeof(GH_Boolean)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)new GH_Boolean(Value.IsActive); }
                return true;
            }

            target = default(Q);
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

            //Cast from Auto Axis Config
            if (typeof(AutoAxisConfig).IsAssignableFrom(source.GetType()))
            {
                Value = source as AutoAxisConfig;
                return true;
            }

            //Cast from Boolean
            if (typeof(GH_Boolean).IsAssignableFrom(source.GetType()))
            {
                GH_Boolean ghBoolean = source as GH_Boolean;
                Value = new AutoAxisConfig(ghBoolean.Value);
                return true;
            }

            //Cast from Action
            if (typeof(RobotComponents.Actions.Action).IsAssignableFrom(source.GetType()))
            {
                if (source is AutoAxisConfig action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Action Goo
            if (typeof(GH_Action).IsAssignableFrom(source.GetType()))
            {
                GH_Action actionGoo = source as GH_Action;
                if (actionGoo.Value is AutoAxisConfig action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Instruction
            if (typeof(IInstruction).IsAssignableFrom(source.GetType()))
            {
                if (source is AutoAxisConfig instruction)
                {
                    Value = instruction;
                    return true;
                }
            }

            //Cast from Instruction Goo
            if (typeof(GH_Instruction).IsAssignableFrom(source.GetType()))
            {
                GH_Instruction instructionGoo = source as GH_Instruction;
                if (instructionGoo.Value is AutoAxisConfig instruction)
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
        private const string IoKey = "Auto Axis Config";
        
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
            this.Value = (AutoAxisConfig)HelperMethods.ByteArrayToObject(array);

            return true;
        }
        #endregion
    }
}

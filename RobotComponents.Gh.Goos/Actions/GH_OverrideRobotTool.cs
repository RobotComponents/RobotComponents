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
using RobotComponents.Gh.Goos.Definitions;

namespace RobotComponents.Gh.Goos.Actions
{
    /// <summary>
    /// Override Robot Tool Goo wrapper class, makes sure the Override Robot Tool class can be used in Grasshopper.
    /// </summary>
    public class GH_OverrideRobotTool : GH_Goo<OverrideRobotTool>, GH_ISerializable
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_OverrideRobotTool()
        {
            this.Value = null;
        }

        /// <summary>
        /// Data constructor: Create an Override Robot Tool Goo instance from an Override Robot Tool instance.
        /// </summary>
        /// <param name="overrideRobotTool"> Override Robot Tool Value to store inside this Goo instance. </param>
        public GH_OverrideRobotTool(OverrideRobotTool overrideRobotTool)
        {
            this.Value = overrideRobotTool;
        }

        /// <summary>
        /// Data constructor: Creates an Override Robot Tool Goo instance from another Override Robot Tool Goo instance.
        /// This creates a shallow copy of the passed OVerride Robot Tool Goo instance. 
        /// </summary>
        /// <param name="overrideRobotToolGoo"> Override Robot Tool Goo to copy. </param>
        public GH_OverrideRobotTool(GH_OverrideRobotTool overrideRobotToolGoo)
        {
            if (overrideRobotToolGoo == null)
            {
                overrideRobotToolGoo = new GH_OverrideRobotTool();
            }
                
            this.Value = overrideRobotToolGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this Goo istance. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the Override Robot Tool Goo. </returns>
        public override IGH_Goo Duplicate()
        {
            return new GH_OverrideRobotTool(Value == null ? new OverrideRobotTool() : Value.Duplicate());
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
                if (Value == null) { return "No internal Override Robot Tool instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid Override Robot Tool instance: Did you define a Robot Tool?";
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null) { return "Null Override Robot Tool"; }
            else { return Value.ToString(); }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return "Override Robot Tool"; }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return "Defines a Override Robot Tool"; }
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
            //Cast to Override Robot Tool
            if (typeof(Q).IsAssignableFrom(typeof(OverrideRobotTool)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)Value; }
                return true;
            }

            //Cast to Override Robot Tool Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_OverrideRobotTool)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)new GH_OverrideRobotTool(Value); }
                return true;
            }

            //Cast to Action
            if (typeof(Q).IsAssignableFrom(typeof(Action)))
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

            //Cast to Robot Tool Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_RobotTool)))
            {
                if (Value == null) { target = default(Q); }
                else if (Value.RobotTool == null) { target = default(Q); }
                else { target = (Q)(object)new GH_RobotTool(Value.RobotTool); }
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

            //Cast from Override Robot Tool
            if (typeof(OverrideRobotTool).IsAssignableFrom(source.GetType()))
            {
                Value = source as OverrideRobotTool;
                return true;
            }

            //Cast from Robot Tool Goo
            if (typeof(GH_RobotTool).IsAssignableFrom(source.GetType()))
            {
                GH_RobotTool robotToolGoo = source as GH_RobotTool;
                Value = new OverrideRobotTool(robotToolGoo.Value);
                return true;
            }

            //Cast from Action
            if (typeof(Action).IsAssignableFrom(source.GetType()))
            {
                if (source is OverrideRobotTool action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Action Goo
            if (typeof(GH_Action).IsAssignableFrom(source.GetType()))
            {
                GH_Action actionGoo = source as GH_Action;
                if (actionGoo.Value is OverrideRobotTool action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Instruction
            if (typeof(IInstruction).IsAssignableFrom(source.GetType()))
            {
                if (source is OverrideRobotTool instruction)
                {
                    Value = instruction;
                    return true;
                }
            }

            //Cast from Instruction Goo
            if (typeof(GH_Instruction).IsAssignableFrom(source.GetType()))
            {
                GH_Instruction instructionGoo = source as GH_Instruction;
                if (instructionGoo.Value is OverrideRobotTool instruction)
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
        private const string IoKey = "Override Robot Tool";

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
            this.Value = (OverrideRobotTool)HelperMethods.ByteArrayToObject(array);

            return true;
        }
        #endregion
    }

}

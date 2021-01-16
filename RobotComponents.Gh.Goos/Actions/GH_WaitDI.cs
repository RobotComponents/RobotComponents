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

namespace RobotComponents.Gh.Goos.Actions
{
    /// <summary>
    /// Wait DI Goo wrapper class, makes sure the Wait DI class can be used in Grasshopper.
    /// </summary>
    public class GH_WaitDI : GH_Goo<WaitDI>, GH_ISerializable
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_WaitDI()
        {
            this.Value = null;
        }

        /// <summary>
        /// Data constructor: Create a Wait for Digital Input Goo instance from a Wait for Digital Input instance.
        /// </summary>
        /// <param name="waitDI"> Wait DI Value to store inside this Goo instance. </param>
        public GH_WaitDI(WaitDI waitDI)
        {
            this.Value = waitDI;
        }

        /// <summary>
        /// Data constructor: Creates a Wait for Digital Input Goo instance from another Wait for Digital Input Goo instance.
        /// This creates a shallow copy of the passed Wait for Digital Input Goo instance. 
        /// </summary>
        /// <param name="waitDIGoo"> Wait DI Goo to copy. </param>
        public GH_WaitDI(GH_WaitDI waitDIGoo)
        {
            if (waitDIGoo == null)
            {
                waitDIGoo = new GH_WaitDI();
            }

            this.Value = waitDIGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this Goo instance. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the WaitDIGoo. </returns>
        public override IGH_Goo Duplicate()
        {
            return new GH_WaitDI(Value == null ? new WaitDI() : Value.Duplicate());
        }
        #endregion

        #region properties
        public override bool IsValid
        /// <summary>
        /// Gets a value indicating whether or not the current value is valid.
        /// </summary>
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
                if (Value == null) { return "No internal Wait DI instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid Wait DI instance: Did you define the digital input name and value?"; 
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null) { return "Null Wait DI"; }
            else { return Value.ToString(); }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return "Wait for Digital Input"; }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return "Defines a Wait for Digital Input"; }
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
            // Cast to Wait DI
            if (typeof(Q).IsAssignableFrom(typeof(WaitDI)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)Value; }
                return true;
            }

            // Cast to Wait DI Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_WaitDI)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)new GH_WaitDI(Value); }
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

            //Cast to Boolean
            if (typeof(Q).IsAssignableFrom(typeof(GH_Boolean)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)new GH_Boolean(Value.Value); }
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

            // Cast from Wait DI
            if (typeof(WaitDI).IsAssignableFrom(source.GetType()))
            {
                Value = source as WaitDI;
                return true;
            }

            //Cast from Action
            if (typeof(Action).IsAssignableFrom(source.GetType()))
            {
                if (source is WaitDI action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Action Goo
            if (typeof(GH_Action).IsAssignableFrom(source.GetType()))
            {
                GH_Action actionGoo = source as GH_Action;
                if (actionGoo.Value is WaitDI action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Instruction
            if (typeof(IInstruction).IsAssignableFrom(source.GetType()))
            {
                if (source is WaitDI instruction)
                {
                    Value = instruction;
                    return true;
                }
            }

            //Cast from Instruction Goo
            if (typeof(GH_Instruction).IsAssignableFrom(source.GetType()))
            {
                GH_Instruction instructionGoo = source as GH_Instruction;
                if (instructionGoo.Value is WaitDI instruction)
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
        private const string IoKey = "Wait DI";

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
            this.Value = (WaitDI)HelperMethods.ByteArrayToObject(array);

            return true;
        }
        #endregion
    }
}

// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Lib
using System;
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
    /// External Joint Position Goo wrapper class, makes sure the External Joint Position class can be used in Grasshopper.
    /// </summary>
    public class GH_ExternalJointPosition : GH_Goo<ExternalJointPosition>, GH_ISerializable
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_ExternalJointPosition()
        {
            this.Value = null;
        }

        /// <summary>
        /// Data constructor: Creates an External Joint Position Goo instance from an External Joint Position instance. 
        /// </summary>
        /// <param name="externalJointPosition"> External Joint Position Value to store inside this Goo instance. </param>
        public GH_ExternalJointPosition(ExternalJointPosition externalJointPosition)
        {
            this.Value = externalJointPosition;
        }

        /// <summary>
        /// Data constructor: Creates an External Joint Position Goo instance from another External Joint Position Goo instance.
        /// This creates a shallow copy of the passed External Joint Position Goo instance. 
        /// </summary>
        /// <param name="externalJointPositionGoo"> External Joint Position Goo instance to copy. </param>
        public GH_ExternalJointPosition(GH_ExternalJointPosition externalJointPositionGoo)
        {
            if (externalJointPositionGoo == null)
            {
                externalJointPositionGoo = new GH_ExternalJointPosition();
            }

            this.Value = externalJointPositionGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this Goo instance. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the this Goo instance. </returns>
        public override IGH_Goo Duplicate()
        {
            return new GH_ExternalJointPosition(Value == null ? new ExternalJointPosition() : Value.Duplicate());
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
                if (Value == null) { return "No internal ExternalJointPosition instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid ExternalJointPosition instance: Did you define external axis values?";
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null) { return "Null External Joint Position"; }
            else { return Value.ToString(); }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return "External Joint Position"; }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return ("Defines an External Joint Position"); }
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
            //Cast to ExternalJointPosition
            if (typeof(Q).IsAssignableFrom(typeof(ExternalJointPosition)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)Value; }
                return true;
            }

            //Cast to ExternalJointPositionGoo
            if (typeof(Q).IsAssignableFrom(typeof(GH_ExternalJointPosition)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)new GH_ExternalJointPosition(Value); }
                return true;
            }

            //Cast to Action
            if (typeof(Q).IsAssignableFrom(typeof(RobotComponents.Actions.Action)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)Value; } 
                return true;
            }

            //Cast to ActionGoo
            if (typeof(Q).IsAssignableFrom(typeof(GH_Action)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)new GH_Action(Value); }
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

            //Cast from Number: The user only defines the first external axis value
            if (typeof(GH_Number).IsAssignableFrom(source.GetType()))
            {
                Value = new ExternalJointPosition((source as GH_Number).Value);
                return true;
            }

            //Cast from Text
            if (typeof(GH_String).IsAssignableFrom(source.GetType()))
            {
                string text = (source as GH_String).Value;

                string[] values = text.Split(',');

                try
                {
                    ExternalJointPosition externalJointPosition = new ExternalJointPosition();

                    for (int i = 0; i < Math.Min(values.Length, 6); i++)
                    {
                        externalJointPosition[i] = System.Convert.ToDouble(values[i]);
                    }

                    Value = externalJointPosition;
                    return true;
                }

                catch
                {
                    return false;
                }
            }

            //Cast from ExternalJointPosition
            if (typeof(ExternalJointPosition).IsAssignableFrom(source.GetType()))
            {
                Value = source as ExternalJointPosition;
                return true;
            }

            //Cast from Action
            if (typeof(RobotComponents.Actions.Action).IsAssignableFrom(source.GetType()))
            {
                if (source is ExternalJointPosition action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from ActionGoo
            if (typeof(GH_Action).IsAssignableFrom(source.GetType()))
            {
                GH_Action actionGoo = source as GH_Action;
                if (actionGoo.Value is ExternalJointPosition action)
                {
                    Value = action;
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
        private const string IoKey = "External Joint Position";

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
            this.Value = (ExternalJointPosition)HelperMethods.ByteArrayToObject(array);

            return true;
        }
        #endregion
    }
}

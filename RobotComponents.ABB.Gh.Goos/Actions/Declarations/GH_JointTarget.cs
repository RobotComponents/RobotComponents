﻿// This file is part of Robot Components. Robot Components is licensed 
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
using RobotComponents.ABB.Actions.Declarations;
using RobotComponents.Utils;

namespace RobotComponents.ABB.Gh.Goos.Actions.Declarations
{
    /// <summary>
    /// Joint Target Goo wrapper class, makes sure the Joint Target class can be used in Grasshopper.
    /// </summary>
    public class GH_JointTarget : GH_Goo<JointTarget>, GH_ISerializable
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_JointTarget()
        {
            this.Value = null;
        }

        /// <summary>
        /// Data constructor: Create a Joint Target Goo instance from a Joint Target instance.
        /// </summary>
        /// <param name="jointTarget"> Joint Target Value to store inside this Goo instance. </param>
        public GH_JointTarget(JointTarget jointTarget)
        {
            this.Value = jointTarget;
        }

        /// <summary>
        /// Data constructor: Creates a Joint Target Goo instance from another Joint Target instance.
        /// This creates a shallow copy of the passed Joint Target Goo instance. 
        /// </summary>
        /// <param name="jointTargetGoo"> Joint Target Goo instance to copy. </param>
        public GH_JointTarget(GH_JointTarget jointTargetGoo)
        {
            if (jointTargetGoo == null)
            {
                jointTargetGoo = new GH_JointTarget();
            }

            this.Value = jointTargetGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this Goo instance. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the JointTargetGoo. </returns>
        public override IGH_Goo Duplicate()
        {
            return new GH_JointTarget(Value == null ? new JointTarget() : Value.Duplicate());
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
                if (Value == null) { return "No internal Joint Target instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid Joint Target instance: Did you define a robot and externanl joint position?";
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null) { return "Null Joint Target"; }
            else { return Value.ToString(); }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return "Joint Target"; }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return "Defines a single Joint Target"; }
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
            //Cast to Target Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_Target)))
            {
                if (Value == null) { target = (Q)(object)new GH_Target(); }
                else { target = (Q)(object)new GH_Target(Value); }
                return true;
            }

            //Cast to Target
            if (typeof(Q).IsAssignableFrom(typeof(ITarget)))
            {
                if (Value == null) { target = (Q)(object)null; }
                else { target = (Q)(object)Value; }
                return true;
            }

            //Cast to Joint Target Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_JointTarget)))
            {
                if (Value == null) { target = (Q)(object)new GH_JointTarget(); }
                else { target = (Q)(object)new GH_JointTarget(Value); }
                return true;
            }

            //Cast to Joint Target
            if (typeof(Q).IsAssignableFrom(typeof(JointTarget)))
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
            if (typeof(Q).IsAssignableFrom(typeof(RobotComponents.ABB.Actions.IAction)))
            {
                if (Value == null) { target = (Q)(object)null; }
                else { target = (Q)(object)Value; }
                return true;
            }

            //Cast to Declaration Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_Declaration)))
            {
                if (Value == null) { target = (Q)(object)new GH_Declaration(); }
                else { target = (Q)(object)new GH_Declaration(Value); }
                return true;
            }

            //Cast to Declaration
            if (typeof(Q).IsAssignableFrom(typeof(IDeclaration)))
            {
                if (Value == null) { target = (Q)(object)null; }
                else { target = (Q)(object)Value; }
                return true;
            }

            //Cast to Robot Joint Position Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_RobotJointPosition)))
            {
                if (Value == null) { target = (Q)(object)new GH_RobotJointPosition(); }
                else { target = (Q)(object)new GH_RobotJointPosition(Value.RobotJointPosition); }
                return true;
            }

            //Cast to Robot Joint Position
            if (typeof(Q).IsAssignableFrom(typeof(RobotJointPosition)))
            {
                if (Value == null) { target = (Q)(object)null; }
                else { target = (Q)(object)Value.RobotJointPosition; }
                return true;
            }

            //Cast to External Joint Position Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_ExternalJointPosition)))
            {
                if (Value == null) { target = (Q)(object)new GH_ExternalJointPosition(); }
                else { target = (Q)(object)new GH_ExternalJointPosition(Value.ExternalJointPosition); }
                return true;
            }

            //Cast to External Joint Position
            if (typeof(Q).IsAssignableFrom(typeof(ExternalJointPosition)))
            {
                if (Value == null) { target = (Q)(object)null; }
                else { target = (Q)(object)Value.ExternalJointPosition; }
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

            //Cast from Target
            if (typeof(ITarget).IsAssignableFrom(source.GetType()))
            {
                if (source is JointTarget target)
                {
                    Value = target;
                    return true;
                }
            }

            //Cast from Target Goo
            if (typeof(GH_Target).IsAssignableFrom(source.GetType()))
            {
                GH_Target targetGoo = source as GH_Target;
                if (targetGoo.Value is JointTarget target)
                {
                    Value = target;
                    return true;
                }
            }

            //Cast from Action
            if (typeof(RobotComponents.ABB.Actions.IAction).IsAssignableFrom(source.GetType()))
            {
                if (source is JointTarget action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Action Goo
            if (typeof(GH_Action).IsAssignableFrom(source.GetType()))
            {
                GH_Action actionGoo = source as GH_Action;
                if (actionGoo.Value is JointTarget action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Joint Target
            if (typeof(JointTarget).IsAssignableFrom(source.GetType()))
            {
                Value = source as JointTarget;
                return true;
            }

            //Cast from Joint Target Goo
            if (typeof(GH_JointTarget).IsAssignableFrom(source.GetType()))
            {
                GH_JointTarget targetGoo = source as GH_JointTarget;
                Value = targetGoo.Value;
                return true;
            }

            //Cast from Declaration
            if (typeof(IDeclaration).IsAssignableFrom(source.GetType()))
            {
                if (source is JointTarget target)
                {
                    Value = target;
                    return true;
                }
            }

            //Cast from Declaration Goo
            if (typeof(GH_Declaration).IsAssignableFrom(source.GetType()))
            {
                GH_Declaration declarationGoo = source as GH_Declaration;
                if (declarationGoo.Value is JointTarget target)
                {
                    Value = target;
                    return true;
                }
            }

            //Cast from Robot Joint Position
            if (typeof(RobotJointPosition).IsAssignableFrom(source.GetType()))
            {
                RobotJointPosition jointPosition = (RobotJointPosition)source;
                Value = new JointTarget(jointPosition);
                return true;
            }

            //Cast from Robot Joint Position Goo
            if (typeof(GH_RobotJointPosition).IsAssignableFrom(source.GetType()))
            {
                GH_RobotJointPosition jointPositionGoo = (GH_RobotJointPosition)source;
                Value = new JointTarget(jointPositionGoo.Value);
                return true;
            }

            //Cast from Text
            if (typeof(GH_String).IsAssignableFrom(source.GetType()))
            {
                string text = (source as GH_String).Value;

                try
                {
                    Value = JointTarget.Parse(text);
                    return true;
                }
                catch
                {
                    string clean = text.Replace("[", "");
                    clean = clean.Replace("]", "");
                    clean = clean.Replace("(", "");
                    clean = clean.Replace(")", "");
                    clean = clean.Replace("{", "");
                    clean = clean.Replace("}", "");
                    clean = clean.Replace(" ", "");

                    string[] values = clean.Split(',');

                    if (values.Length == 6)
                    {
                        try
                        {
                            RobotJointPosition robotJointPosition = new RobotJointPosition();

                            for (int i = 0; i < Math.Min(values.Length, 6); i++)
                            {
                                robotJointPosition[i] = System.Convert.ToDouble(values[i]);
                            }

                            Value = new JointTarget(robotJointPosition);
                            return true;
                        }

                        catch
                        {
                            return false;
                        }
                    }
                }
            }

            return false;
        }
        #endregion

        #region (de)serialisation
        /// <summary>
        /// IO key for (de)serialisation of the value inside this Goo.
        /// </summary>
        private const string IoKey = "Joint Target";

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
            this.Value = (JointTarget)Serialization.ByteArrayToObject(array);

            return true;
        }
        #endregion
    }
}

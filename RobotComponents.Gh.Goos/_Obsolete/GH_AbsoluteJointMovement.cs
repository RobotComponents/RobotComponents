// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
// Grasshopper Libs
using Grasshopper.Kernel.Types;
// RobotComponents Libs
using RobotComponents.Gh.Goos.Actions;
using RobotComponents.Gh.Goos.Definitions;
using RobotComponents.Actions;

namespace RobotComponents.Gh.Goos.Obsolete
{
    /// <summary>
    /// Absolute Joint Movement Goo wrapper class, makes sure the Absolute Joint Movement class can be used in Grasshopper.
    /// </summary>
    [Obsolete("This class is obsolete and will be removed in the future.", false)]
    public class GH_AbsoluteJointMovement : GH_Goo<AbsoluteJointMovement>
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_AbsoluteJointMovement()
        {
            this.Value = null;
        }

        /// <summary>
        /// Data constructor: Creates an Absolute Joint Momvement Goo instance from an Absolute Joint Movement instance.
        /// </summary>
        /// <param name="absoluteJointMovement"> Absolute Joint Movement Value to store inside this Goo instance. </param>
        public GH_AbsoluteJointMovement(AbsoluteJointMovement absoluteJointMovement)
        {
            this.Value = absoluteJointMovement;
        }

        /// <summary>
        /// Data constructor: Creates an Absolute Joint Movement Goo instance from another Absolute Joint Movement Goo instance.
        /// This creates a shallow copy of the passed Absolute Joint Movement Goo instance. 
        /// </summary>
        /// <param name="absoluteJointMovementGoo"> Absolute Joint Movement Goo instance to copy. </param>
        public GH_AbsoluteJointMovement(GH_AbsoluteJointMovement absoluteJointMovementGoo)
        {
            if (absoluteJointMovementGoo == null)
            {
                absoluteJointMovementGoo = new GH_AbsoluteJointMovement();
            }

            this.Value = absoluteJointMovementGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this Goo instance. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the Movement Goo. </returns>
        public override IGH_Goo Duplicate()
        {
            return new GH_AbsoluteJointMovement(Value == null ? new AbsoluteJointMovement() : Value.Duplicate());
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
                if (Value == null) { return "No internal Absolute Joint Movement instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid Absolute Joint Movement instance: Did you define a name, speed and axis values?";
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null) { return "Null Absolute Joint Movement"; }
            else { return Value.ToString(); }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return "Absolute Joint Movement"; }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return "Defines an Absolute Joint Movement"; }
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
            //Cast to Absolute Joint Movement
            if (typeof(Q).IsAssignableFrom(typeof(AbsoluteJointMovement)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)Value; }
                return true;
            }

            //Cast to Absolute Joint Movement Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_AbsoluteJointMovement)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)new GH_AbsoluteJointMovement(Value); }
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

            //Cast to Speed Data
            if (typeof(Q).IsAssignableFrom(typeof(GH_SpeedData)))
            {
                if (Value == null) { target = default(Q); }
                else if (Value.SpeedData == null) { target = default(Q); }
                else { target = (Q)(object)new GH_SpeedData(Value.SpeedData); }
                return true;
            }

            //Cast to Zone Data
            if (typeof(Q).IsAssignableFrom(typeof(GH_ZoneData)))
            {
                if (Value == null) { target = default(Q); }
                else if (Value.ZoneData == null) { target = default(Q); }
                else { target = (Q)(object)new GH_ZoneData(Value.ZoneData); }
                return true;
            }

            //Cast to Robot Tool
            if (typeof(Q).IsAssignableFrom(typeof(GH_RobotTool)))
            {
                if (Value == null) { target = default(Q); }
                else if (Value.RobotTool == null) { target = default(Q); }
                else { target = (Q)(object)new GH_RobotTool(Value.RobotTool); }
                return true;
            }

            #region casting methods to new datatypes
            //Cast to Movement Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_Movement)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)new GH_Movement(Value.ConvertToMovement()); }
                return true;
            }

            //Cast to Joint Target Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_JointTarget)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)new GH_JointTarget(Value.ConvertToJointTarget()); }
                return true;
            }

            //Cast to Robot Joint Position Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_RobotJointPosition)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)new GH_RobotJointPosition(Value.ConvertToRobotJointPosition()); }
                return true;
            }

            //Cast to External Joint Position Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_ExternalJointPosition)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)new GH_ExternalJointPosition(Value.ConvertToExternalJointPosition()); }
                return true;
            }
            #endregion

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

            //Cast from Aboslute Joint Movement
            if (typeof(AbsoluteJointMovement).IsAssignableFrom(source.GetType()))
            {
                Value = source as AbsoluteJointMovement;
                return true;
            }

            //Cast from Aboslute Joint Movement Goo
            if (typeof(GH_AbsoluteJointMovement).IsAssignableFrom(source.GetType()))
            {
                GH_AbsoluteJointMovement absoluteJointMovementGoo = source as GH_AbsoluteJointMovement;
                Value = absoluteJointMovementGoo.Value;
                return true;
            }

            //Cast from Action
            if (typeof(RobotComponents.Actions.Action).IsAssignableFrom(source.GetType()))
            {
                if (source is AbsoluteJointMovement action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Action Goo
            if (typeof(GH_Action).IsAssignableFrom(source.GetType()))
            {
                GH_Action actionGoo = source as GH_Action;
                if (actionGoo.Value is AbsoluteJointMovement action)
                {
                    Value = action;
                    return true;
                }
            }

            return false;
        }
        #endregion
    }
}

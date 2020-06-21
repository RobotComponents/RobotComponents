// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/EDEK-UniKassel/RobotComponents>.

// Grasshopper Libs
using Grasshopper.Kernel.Types;
// RobotComponents Libs
using RobotComponents.BaseClasses.Actions;

// NOTE: The namespace is missing '.Declarations' to keep the access to the actions simple. 
namespace RobotComponentsGoos.Actions
{
    /// <summary>
    /// Joint Target Goo wrapper class, makes sure the Joint Target class can be used in Grasshopper.
    /// </summary>
    public class GH_JointTarget : GH_Goo<JointTarget>
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
        /// If the instance is valid, then this property should return Nothing or String.Empty.
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
            get { return ("Defines a single Joint Target"); }
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
            //Cast to Joint Target
            if (typeof(Q).IsAssignableFrom(typeof(JointTarget)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)Value; }
                return true;
            }

            //Cast to JointTargetGoo
            if (typeof(Q).IsAssignableFrom(typeof(GH_JointTarget)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)new GH_JointTarget(Value); }
                return true;
            }

            //Cast to Action
            if (typeof(Q).IsAssignableFrom(typeof(Action)))
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

            //Cast to RobJointPositionGoo
            if (typeof(Q).IsAssignableFrom(typeof(GH_RobotJointPosition)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)new GH_RobotJointPosition(Value.RobotJointPosition); }
                return true;
            }

            //Cast to ExtJointPositionGoo
            if (typeof(Q).IsAssignableFrom(typeof(GH_ExternalJointPosition)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)new GH_ExternalJointPosition(Value.ExternalJointPosition); }
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

            //Cast from Joint Target
            if (typeof(JointTarget).IsAssignableFrom(source.GetType()))
            {
                Value = source as JointTarget;
                return true;
            }

            //Cast from Action
            if (typeof(Action).IsAssignableFrom(source.GetType()))
            {
                if (source is JointTarget action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from ActionGoo
            if (typeof(GH_Action).IsAssignableFrom(source.GetType()))
            {
                GH_Action actionGoo = source as GH_Action;
                if (actionGoo.Value is JointTarget action)
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

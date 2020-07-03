// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// Grasshopper Libs
using Grasshopper.Kernel.Types;
// RobotComponents Libs
using RobotComponents.Actions;

namespace RobotComponentsGoos.Actions
{
    /// <summary>
    /// Robot Target Goo wrapper class, makes sure the Robot Target class can be used in Grasshopper.
    /// </summary>
    public class GH_RobotTarget : GH_Goo<RobotTarget>
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_RobotTarget() 
        {
            this.Value = null;
        }

        /// <summary>
        /// Data constructor: Create a Robot Target Goo instance from a Robot Target instance.
        /// </summary>
        /// <param name="target"> Robot Target Value to store inside this Goo instance. </param>
        public GH_RobotTarget(RobotTarget target)
        {
            this.Value = target;
        }

        /// <summary>
        /// Data constructor: Creates a Robot Target Goo instance from another Robot Target Goo instance.
        /// This creates a shallow copy of the passed Robot Target Goo instance. 
        /// </summary>
        /// <param name="targetGoo"> Robot Target Goo to copy. </param>
        public GH_RobotTarget(GH_RobotTarget targetGoo)
        {
            if (targetGoo == null)
            {
                targetGoo = new GH_RobotTarget();
            }

            this.Value = targetGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this Goo instance. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the Robot Target Goo. </returns>
        public override IGH_Goo Duplicate()
        {
            return new GH_RobotTarget(Value == null ? new RobotTarget() : Value.Duplicate());
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
                if (Value == null) { return "No internal Robot Target instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid Robot Target instance: Did you define a name and target plane?";
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null) { return "Null Robot Target"; }
            else { return Value.ToString(); }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return "Robot Target"; }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return "Defines a Robot Target"; }
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
            //Cast to Target
            if (typeof(Q).IsAssignableFrom(typeof(ITarget)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)Value; }
                return true;
            }

            //Cast to Target Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_Target)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)new GH_Target(Value); }
                return true;
            }

            //Cast to Robot Target
            if (typeof(Q).IsAssignableFrom(typeof(RobotTarget)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)Value; }
                return true;
            }

            //Cast to Robot Target Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_RobotTarget)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)new GH_Target(Value); }
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

            //Cast to Plane
            if (typeof(Q).IsAssignableFrom(typeof(GH_Plane)))
            {
                if (Value == null) { target = default(Q); }
                else if (Value.Plane == null) { target = default(Q); }
                else { target = (Q)(object)new GH_Plane(Value.Plane); }
                return true;
            }

            //Cast to Point
            if (typeof(Q).IsAssignableFrom(typeof(GH_Point)))
            {
                if (Value == null) { target = default(Q); }
                else if (Value.Plane == null) { target = default(Q); }
                else { target = (Q)(object)new GH_Point(Value.Plane.Origin); }
                return true;
            }

            //Cast to External Joint Position Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_ExternalJointPosition)))
            {
                if (Value == null) { target = default(Q); }
                else if (Value.ExternalJointPosition == null) { target = default(Q); }
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

            //Cast from Target
            if (typeof(ITarget).IsAssignableFrom(source.GetType()))
            {
                if (source is RobotTarget target)
                {
                    Value = target;
                    return true;
                }
            }

            //Cast from Target Goo
            if (typeof(GH_Target).IsAssignableFrom(source.GetType()))
            {
                GH_Target targetGoo = source as GH_Target;
                if (targetGoo.Value is RobotTarget target)
                {
                    Value = target;
                    return true;
                }
            }

            //Cast from Action
            if (typeof(Action).IsAssignableFrom(source.GetType()))
            {
                if (source is RobotTarget action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Action Goo
            if (typeof(GH_Action).IsAssignableFrom(source.GetType()))
            {
                GH_Action actionGoo = source as GH_Action;
                if (actionGoo.Value is RobotTarget action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Robot Target
            if (typeof(RobotTarget).IsAssignableFrom(source.GetType()))
            {
                Value = source as RobotTarget;
                return true;
            }

            //Cast from Robot Target Goo
            if (typeof(GH_RobotTarget).IsAssignableFrom(source.GetType()))
            {
                GH_RobotTarget targetGoo = source as GH_RobotTarget;
                Value = targetGoo.Value as RobotTarget;
                return true;
            }

            return false;
        }
        #endregion
    }
}

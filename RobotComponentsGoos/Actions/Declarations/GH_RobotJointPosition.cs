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
    /// Robot Joint Position Goo wrapper class, makes sure the Robot Joint Position class can be used in Grasshopper.
    /// </summary>
    public class GH_RobotJointPosition : GH_Goo<RobotJointPosition>
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_RobotJointPosition()
        {
            this.Value = null;
        }

        /// <summary>
        /// Data constructor
        /// </summary>
        /// <param name="RobotJointPosition"> Robot Joint Position Value to store inside this Goo instance. </param>
        public GH_RobotJointPosition(RobotJointPosition robotJointPosition)
        {
            this.Value = robotJointPosition;
        }

        /// <summary>
        /// Data constructor: Creates a Robot Joint Position instance from another Robot Joint Position Goo instance.
        /// This creates a shallow copy of the passed Robot Joint Position Goo instance. 
        /// </summary>
        /// <param name="robotJointPositionGoo"> Robot Joint Position Goo instance to copy. </param>
        public GH_RobotJointPosition(GH_RobotJointPosition robotJointPositionGoo)
        {
            if (robotJointPositionGoo == null)
            {
                robotJointPositionGoo = new GH_RobotJointPosition();
            }

            this.Value = robotJointPositionGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this Goo instance. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of this Goo instance. </returns>
        public override IGH_Goo Duplicate()
        {
            return new GH_RobotJointPosition(Value == null ? new RobotJointPosition() : Value.Duplicate());
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
                if (Value == null) { return "No internal RobotJointPosition instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid RobotJointPosition instance: Did you define axis values?";
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null) { return "Null Robot Joint Position"; }
            else { return Value.ToString(); }  
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return "Robot Joint Position"; }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return "Defines a Robot Joint Position"; }
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
            //Cast to RobotJointPosition
            if (typeof(Q).IsAssignableFrom(typeof(RobotJointPosition)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)Value; }
                return true;
            }

            //Cast to RobotJointPositionGoo
            if (typeof(Q).IsAssignableFrom(typeof(GH_RobotJointPosition)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)new GH_RobotJointPosition(Value); }
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

            //Cast from RobotJointPosition
            if (typeof(RobotJointPosition).IsAssignableFrom(source.GetType()))
            {
                Value = source as RobotJointPosition;
                return true;
            }

            //Cast from Action
            if (typeof(Action).IsAssignableFrom(source.GetType()))
            {
                if (source is RobotJointPosition action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from ActionGoo
            if (typeof(GH_Action).IsAssignableFrom(source.GetType()))
            {
                GH_Action actionGoo = source as GH_Action;
                if (actionGoo.Value is RobotJointPosition action)
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

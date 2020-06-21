// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/EDEK-UniKassel/RobotComponents>.

// Grasshopper Libs
using Grasshopper.Kernel.Types;
// RobotComponents Libs
using RobotComponents.BaseClasses.Actions;

namespace RobotComponentsGoos.Actions
{
    /// <summary>
    /// Wait Time Goo wrapper class, makes sure the Wait Time class can be used in Grasshopper.
    /// </summary>
    public class GH_WaitTime : GH_Goo<WaitTime>
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_WaitTime()
        {
            this.Value = null;
        }

        /// <summary>
        /// Data constructor: Creates a Wait Time Goo instance from a Wait Time instance.
        /// </summary>
        /// <param name="waitTime"> WaitTime Value to store inside this Goo instance. </param>
        public GH_WaitTime(WaitTime waitTime)
        {
            this.Value = waitTime;
        }

        /// <summary>
        /// Data constructor: Creates a Wait Time Goo instance from another Wait Time Goo instance.
        /// This creates a shallow copy of the passed Wait Time Goo instance. 
        /// </summary>
        /// <param name="waitTimeGoo"> Wait Time Goo instacne to copy. </param>
        public GH_WaitTime(GH_WaitTime waitTimeGoo)
        {
            if (waitTimeGoo == null)
            {
                waitTimeGoo = new GH_WaitTime();
            }

            this.Value = waitTimeGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this Goo instance. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the WaitTimeGoo. </returns>
        public override IGH_Goo Duplicate()
        {
            return new GH_WaitTime(Value == null ? new WaitTime() : Value.Duplicate());
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
                if (Value == null) { return "No internal Wait Time instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid Wait Time instance: Did you define a duration?";
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null) { return "Null Wait Time"; }
            else { return Value.ToString(); }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return "Wait Time"; }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return "Defines a Wait Time."; }
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
            //Cast to Wait Time
            if (typeof(Q).IsAssignableFrom(typeof(WaitTime)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)Value; }
                return true;
            }

            //Cast to Wait Time Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_WaitTime)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)new GH_WaitTime(Value); }
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

            //Cast to Number
            if (typeof(Q).IsAssignableFrom(typeof(GH_Number)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)new GH_Number(Value.Duration); }
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

            //Cast from Wait Time
            if (typeof(WaitTime).IsAssignableFrom(source.GetType()))
            {
                Value = source as WaitTime;
                return true;
            }

            //Cast from Action
            if (typeof(Action).IsAssignableFrom(source.GetType()))
            {
                if (source is WaitTime action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Action Goo
            if (typeof(GH_Action).IsAssignableFrom(source.GetType()))
            {
                GH_Action actionGoo = source as GH_Action;
                if (actionGoo.Value is WaitTime action)
                {
                    Value = action;
                    return true;
                }
            }

            // Cast from Number
            if (typeof(GH_Number).IsAssignableFrom(source.GetType()))
            {
                GH_Number ghNumber = (GH_Number)source;
                Value = new WaitTime(ghNumber.Value);
                return true;
            }
            return false;
        }
        #endregion
    }
}

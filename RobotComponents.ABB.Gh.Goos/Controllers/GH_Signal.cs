// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
// Grasshopper Libs
using Grasshopper.Kernel.Types;
// Rhino Libs
using Rhino.Geometry;
// Robot Components Libs
using RobotComponents.ABB.Controllers;

namespace RobotComponents.ABB.Gh.Goos.Controllers
{
    /// <summary>
    /// Signal Goo wrapper class, makes sure the Signal class can be used in Grasshopper.
    /// </summary>
    public class GH_Signal : GH_Goo<Signal>
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_Signal()
        {
            this.Value = null;
        }

        /// <summary>
        /// Data constructor: Creates an Signal Goo instance from an Signal instance.
        /// </summary>
        /// <param name="signal"> Signal to store inside this Goo instance. </param>
        public GH_Signal(Signal signal)
        {
            this.Value = signal;
        }

        /// <summary>
        /// Data constructor: Creates a Signal Goo instance from another Signal Goo instance.
        /// This creates a shallow copy of the passed Signal Goo instance. 
        /// </summary>
        /// <param name="signalGoo"> Signal Goo instance to copy. </param>
        public GH_Signal(GH_Signal signalGoo)
        {
            if (signalGoo == null)
            {
                signalGoo = new GH_Signal();
            }

            this.Value = signalGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this Goo instance. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the Comment Goo. </returns>
        public override IGH_Goo Duplicate()
        {
            return new GH_Signal(Value == null ? new Signal() : Value);
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
                if (Value == null) { return "No internal Signal instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid Signal instance";
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null) { return "Null Signal"; }
            else { return Value.ToString(); }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return "Signal"; }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return "Defines a Signal."; }
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
            //Cast to Signal Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_Signal)))
            {
                if (Value == null) { target = (Q)(object)new GH_Signal(); }
                else { target = (Q)(object)new GH_Signal(Value); }
                return true;
            }

            //Cast to Signal
            if (typeof(Q).IsAssignableFrom(typeof(Signal)))
            {
                if (Value == null) { target = (Q)(object)null; }
                else { target = (Q)(object)Value; }
                return true;
            }

            //Cast to bool
            if (typeof(Q).IsAssignableFrom(typeof(bool)))
            {
                if (Value == null)
                    target = default;
                else
                    target = (Q)(object)(Value.Value != 0);
                return true;
            }

            //Cast to GH_Boolean
            if (typeof(Q).IsAssignableFrom(typeof(GH_Boolean)))
            {
                if (Value == null)
                    target = default;
                else
                    target = (Q)(object)new GH_Boolean(Value.Value != 0);
                return true;
            }

            //Cast to integer
            if (typeof(Q).IsAssignableFrom(typeof(int)))
            {
                if (Value == null)
                    target = default;
                else
                    target = (Q)(object)Convert.ToInt32(Value.Value);
                return true;
            }

            //Cast to GH_Integer
            if (typeof(Q).IsAssignableFrom(typeof(GH_Integer)))
            {
                if (Value == null)
                    target = default;
                else
                    target = (Q)(object)new GH_Integer(Convert.ToInt32(Value.Value));
                return true;
            }

            //Cast to double
            if (typeof(Q).IsAssignableFrom(typeof(double)))
            {
                if (Value == null)
                    target = default;
                else
                    target = (Q)(object)Value.Value;
                return true;
            }

            //Cast to GH_Number
            if (typeof(Q).IsAssignableFrom(typeof(GH_Number)))
            {
                if (Value == null)
                    target = default;
                else
                    target = (Q)(object)new GH_Number(Value.Value);
                return true;
            }

            //Cast to Interval
            if (typeof(Q).IsAssignableFrom(typeof(Interval)))
            {
                if (Value == null)
                    target = default;
                else
                    target = (Q)(object)Value.Limits;
                return true;
            }

            //Cast to GH_Interval
            if (typeof(Q).IsAssignableFrom(typeof(GH_Interval)))
            {
                if (Value == null)
                    target = default;
                else
                    target = (Q)(object)new GH_Interval(Value.Limits);
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

            //Cast from Signal
            if (typeof(Signal).IsAssignableFrom(source.GetType()))
            {
                Value = source as Signal;
                return true;
            }

            //Cast from Signal Goo
            if (typeof(GH_Signal).IsAssignableFrom(source.GetType()))
            {
                GH_Signal SignalGoo = source as GH_Signal;
                Value = SignalGoo.Value;
                return true;
            }

            return false;
        }
        #endregion
    }
}

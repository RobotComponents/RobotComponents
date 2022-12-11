// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// Grasshopper Libs
using Grasshopper.Kernel.Types;
// Robot Components Libs
using RobotComponents.ABB.Controllers;

namespace RobotComponents.ABB.Gh.Goos.Controllers
{
    /// <summary>
    /// Controller Goo wrapper class, makes sure the Controller class can be used in Grasshopper.
    /// </summary>
    public class GH_Controller : GH_Goo<Controller>
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_Controller()
        {
            this.Value = null;
        }

        /// <summary>
        /// Data constructor: Creates an Controller Goo instance from an Controller instance.
        /// </summary>
        /// <param name="controller"> Controller Value to store inside this Goo instance. </param>
        public GH_Controller(Controller controller)
        {
            this.Value = controller;
        }

        /// <summary>
        /// Data constructor: Creates a Controller Goo instance from another Controller Goo instance.
        /// This creates a shallow copy of the passed Controller Goo instance. 
        /// </summary>
        /// <param name="controllerGoo"> Controller Goo instance to copy. </param>
        public GH_Controller(GH_Controller controllerGoo)
        {
            if (controllerGoo == null)
            {
                controllerGoo = new GH_Controller();
            }

            this.Value = controllerGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this Goo instance. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the Comment Goo. </returns>
        public override IGH_Goo Duplicate()
        {
            return new GH_Controller(Value == null ? new Controller() : Value);
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
                if (Value == null) { return "No internal Controller instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid Controller instance";
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null) { return "Null Controller"; }
            else { return Value.ToString(); }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return "Controller"; }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return "Defines a Controller."; }
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
            //Cast to Controller Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_Controller)))
            {
                if (Value == null) { target = (Q)(object)new GH_Controller(); }
                else { target = (Q)(object)new GH_Controller(Value); }
                return true;
            }

            //Cast to Controller
            if (typeof(Q).IsAssignableFrom(typeof(Controller)))
            {
                if (Value == null) { target = (Q)(object)null; }
                else { target = (Q)(object)Value; }
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

            //Cast from Controller
            if (typeof(Controller).IsAssignableFrom(source.GetType()))
            {
                Value = source as Controller;
                return true;
            }

            //Cast from Controller Goo
            if (typeof(GH_Controller).IsAssignableFrom(source.GetType()))
            {
                GH_Controller ControllerGoo = source as GH_Controller;
                Value = ControllerGoo.Value;
                return true;
            }

            return false;
        }
        #endregion
    }
}

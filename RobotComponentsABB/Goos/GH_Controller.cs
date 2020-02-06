// Grasshopper Libs
using Grasshopper.Kernel.Types;
// ABB Robotic Libs
using ABB.Robotics.Controllers;

namespace RobotComponentsABB.Goos
{
    /// <summary>
    /// Controller wrapper class, makes sure the controller can be used in Grasshopper.
    /// </summary>
    public class GH_Controller : GH_Goo<Controller>, IGH_Goo
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_Controller()
        {
            this.Value = new Controller();
        }

        /// <summary>
        /// Data constructor, m_value will be set to internal_data.
        /// </summary>
        /// <param name="controller"> Controller Value to store inside this Goo instance. </param>
        public GH_Controller(Controller controller)
        {
            if (controller == null)
                controller = new Controller();
            this.Value = controller;
        }

        /// <summary>
        /// Data constructor, m_value will be set to internal_data.
        /// </summary>
        /// <param name="controllerGoo"> Controller Value to store inside this Goo instance. </param>
        public GH_Controller(GH_Controller controllerGoo)
        {
            if (controllerGoo == null)
                controllerGoo = new GH_Controller();
            this.Value = controllerGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this geometry. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the ControllerGoo. </returns>
        public override IGH_Goo Duplicate()
        {
            return DuplicateController();
        }

        /// <summary>
        /// Make a complete duplicate of this geometry. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the CommentGoo. </returns>
        public GH_Controller DuplicateController()
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
                return true;
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null)
            {
                return "Null Controller";
            }
            else
            {
                return "Name : " + Value.Name + "\nSystem Name : " + Value.SystemName + "\nIsVirtual : " + Value.IsVirtual.ToString();
            }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return ("Controller"); }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get
            {
                return "Defines a ABB Controller";
            }
        }
        #endregion
    }
}

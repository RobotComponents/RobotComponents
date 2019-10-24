// ----- Grasshopper Libs -----
using Grasshopper.Kernel.Types;
// ----- ABB Robotic Libs -----
using ABB.Robotics.Controllers;

namespace RobotComponents.Goos
{
    public class ControllerGoo : GH_Goo<ABB.Robotics.Controllers.Controller>, IGH_Goo
    {
        #region constructors
        public ControllerGoo()
        {
            this.Value = new ABB.Robotics.Controllers.Controller();
        }

        public ControllerGoo(ABB.Robotics.Controllers.Controller controller)
        {
            if (controller == null)
                controller = new Controller();
            this.Value = controller;
        }

        public override IGH_Goo Duplicate()
        {
            return DublicateController();
        }

        public ControllerGoo DublicateController()
        {
            return new ControllerGoo(Value == null ? new Controller() : Value);
        }
        #endregion

        #region properties
        public override bool IsValid
        {
            get
            {
                if (Value == null) { return false; }
                return true;
            }
        }

        public override string TypeDescription
        {
            get
            {
                return "Defines a ABB Controller";
            }
        }

        public override string TypeName
        {
            get { return ("Controller"); }
        }

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
        #endregion
    }
}

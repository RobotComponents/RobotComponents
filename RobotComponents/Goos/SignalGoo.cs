// ----- Grasshopper Libs -----
using Grasshopper.Kernel.Types;

namespace RobotComponents.Goos
{
    /// <summary>
    /// Signal Goo wrapper class, makes sure Signal can be used in Grasshopper.
    /// </summary>
    public class SignalGoo : GH_Goo<ABB.Robotics.Controllers.IOSystemDomain.DigitalSignal>, IGH_Goo
    {
        #region constructors
        public SignalGoo()
        {
            this.Value = null;
        }

        public SignalGoo(ABB.Robotics.Controllers.IOSystemDomain.DigitalSignal signal)
        {
            this.Value = signal;
        }

        public override IGH_Goo Duplicate()
        {
            return DublicateSignal();
        }

        public SignalGoo DublicateSignal()
        {
            return new SignalGoo(Value);
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
                return "Defines a ABB Signal";
            }
        }

        public override string TypeName
        {
            get { return ("Signal"); }
        }

        public override string ToString()
        {
            if (Value == null)
            {
                return "Null Signal";
            }
            else
            {
                return "SignalName : " + Value.Name + "\nValue : " + Value.Value;
            }
        }
        #endregion
    }
}

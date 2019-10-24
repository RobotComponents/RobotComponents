using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

using RobotComponents.BaseClasses;

namespace RobotComponents.Goos
{
    /// <summary>
    /// Timer Goo wrapper class, makes sure Target can be used in Grasshopper.
    /// </summary>
    public class TimerGoo : GH_GeometricGoo<Timer>, IGH_PreviewData
    {
        #region constructors
        public TimerGoo()
        {
            this.Value = new Timer();
        }

        public TimerGoo(Timer timer)
        {
            if (timer == null)
                timer = new Timer();
            this.Value = timer;
        }

        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return DuplicateTimerGoo();
        }

        public TimerGoo DuplicateTimerGoo()
        {
            return new TimerGoo(Value == null ? new Timer() : Value.Duplicate());
        }
        #endregion

        #region properties
        public override bool IsValid
        {
            get
            {
                if (Value == null) { return false; }
                return Value.IsValid;
            }
        }
        public override string IsValidWhyNot
        {
            get
            {
                if (Value == null) { return "No internal Timer instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid Timer instance: Did you define a duration?"; //Todo: beef this up to be more informative.
            }
        }
        public override string ToString()
        {
            if (Value == null)
                return "Null Timer";
            else
                return "Timer";
        }
        public override string TypeName
        {
            get { return ("Timer"); }
        }
        public override string TypeDescription
        {
            get { return ("Defines a Timer."); }
        }

        public override BoundingBox Boundingbox
        {
            get
            {
                return BoundingBox.Empty; //Note: beef this up if needed
            }
        }
        public override BoundingBox GetBoundingBox(Transform xform)
        {
            return BoundingBox.Empty; //Note: beef this up if needed
        }
        #endregion

        #region casting methods
        public override bool CastTo<Q>(out Q wait)
        {
            //Cast to Wait.
            if (typeof(Q).IsAssignableFrom(typeof(Timer)))
            {
                if (Value == null)
                    wait = default(Q);
                else
                    wait = (Q)(object)Value;
                return true;
            }

            wait = default(Q);
            return false;
        }
        public override bool CastFrom(object source)
        {
            if (source == null) { return false; }

            //Cast from Wait
            if (typeof(Timer).IsAssignableFrom(source.GetType()))
            {
                Value = (Timer)source;
                return true;
            }

            return false;
        }
        #endregion

        #region transformation methods
        public override IGH_GeometricGoo Transform(Transform xform)
        {
            return null;
        }
        public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
        {
            return null;
        }
        #endregion

        #region drawing methods
        public BoundingBox ClippingBox
        {
            get { return Boundingbox; }
        }

        public void DrawViewportMeshes(GH_PreviewMeshArgs args)
        {
        }

        public void DrawViewportWires(GH_PreviewWireArgs args)
        {
        }
        #endregion
    }
}

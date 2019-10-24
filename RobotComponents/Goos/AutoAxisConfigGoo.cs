using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

using RobotComponents.BaseClasses;

namespace RobotComponents.Goos
{
    /// <summary>
    /// Timer Goo wrapper class, makes sure Target can be used in Grasshopper.
    /// </summary>
    public class AutoAxisConfigGoo : GH_GeometricGoo<AutoAxisConfig>, IGH_PreviewData
    {
        #region constructors
        public AutoAxisConfigGoo()
        {
            this.Value = new AutoAxisConfig();
        }

        public AutoAxisConfigGoo(AutoAxisConfig timer)
        {
            if (timer == null)
                timer = new AutoAxisConfig();
            this.Value = timer;
        }

        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return DuplicateAutoAxisConfigGoo();
        }

        public AutoAxisConfigGoo DuplicateAutoAxisConfigGoo()
        {
            return new AutoAxisConfigGoo(Value == null ? new AutoAxisConfig() : Value.Duplicate());
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
                if (Value == null) { return "No internal Auto Axis Configuration instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid Auto Axis Configuration instance: Did you set a bool?"; //Todo: beef this up to be more informative.
            }
        }
        public override string ToString()
        {
            if (Value == null)
                return "Null Auto Axis Configuration";
            else
                return "Auto Axis Configuration";
        }
        public override string TypeName
        {
            get { return ("Auto Axis Configuration"); }
        }
        public override string TypeDescription
        {
            get { return ("Defines a Auto Axis Configuration."); }
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
            if (typeof(AutoAxisConfig).IsAssignableFrom(source.GetType()))
            {
                Value = (AutoAxisConfig)source;
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

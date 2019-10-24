using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

using RobotComponents.BaseClasses;

namespace RobotComponents.Goos
{
    /// <summary>
    /// ExternalRotationalAxis Goo wrapper class, makes sure ExternalRotationalAxis can be used in Grasshopper.
    /// </summary>
    
    // Todo: make inherent from abstract class external axis
    public class ExternalRotationalAxisGoo : GH_GeometricGoo<ExternalRotationalAxis>, IGH_PreviewData
    {
        #region constructors
        public ExternalRotationalAxisGoo()
        {
            this.Value = new ExternalRotationalAxis();
        }

        public ExternalRotationalAxisGoo(ExternalRotationalAxis externalLinearAxis)
        {
            if (externalLinearAxis == null)
                externalLinearAxis = new ExternalRotationalAxis();
            this.Value = externalLinearAxis;
        }

        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return DuplicateExternalLinearAxisGoo();
        }

        public ExternalRotationalAxisGoo DuplicateExternalLinearAxisGoo()
        {
            return new ExternalRotationalAxisGoo(Value == null ? new ExternalRotationalAxis() : Value.Duplicate());
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
                if (Value == null) { return "No internal ExternalRotationalAxis instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid ExternalRotationalAxis instance: Did you define an interval?"; //Todo: beef this up to be more informative.
            }
        }
        public override string ToString()
        {
            if (Value == null)
                return "Null ExternalRotationalAxis";
            else
                return "External Rotational Axis";
        }
        public override string TypeName
        {
            get { return ("ExternalRotationalAxis"); }
        }
        public override string TypeDescription
        {
            get { return ("Defines a ExternalRotationalAxis."); }
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
            if (typeof(ExternalRotationalAxis).IsAssignableFrom(source.GetType()))
            {
                Value = (ExternalRotationalAxis)source;
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
            //args.Pipeline.DrawCurve(Value.AxisCurve, System.Drawing.Color.Red, 1);
        }

        public void DrawViewportWires(GH_PreviewWireArgs args)
        {
            args.Pipeline.DrawCurve(Value.AxisCurve, args.Color, 1);
        }
        #endregion
    }
}

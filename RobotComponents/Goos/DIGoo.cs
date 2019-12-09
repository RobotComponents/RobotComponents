using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

using RobotComponents.BaseClasses;

namespace RobotComponents.Goos
{
    /// <summary>
    /// Wait Digital Input wrapper class, makes sure the WaitDI can be used in Grasshopper.
    /// </summary>
    public class WaitDIGoo : GH_GeometricGoo<WaitDI>, IGH_PreviewData
    {
        #region constructors
        public WaitDIGoo()
        {
            this.Value = new WaitDI();
        }

        public WaitDIGoo(WaitDI comment)
        {
            if (comment == null)
                comment = new WaitDI();
            this.Value = comment;
        }

        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return null;
        }

        public WaitDIGoo DuplicateTarget()
        {
            return new WaitDIGoo(Value == null ? new WaitDI() : Value.Duplicate());
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
                if (Value == null) { return "No internal WaitDI instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid WaitDI instance: Did you define the DIName and Value?"; //Todo: beef this up to be more informative.
            }
        }
        public override string ToString()
        {
            if (Value == null)
                return "Null WaitDI";
            else
                return "Digital Input";
        }
        public override string TypeName
        {
            get { return ("WaitDI"); }
        }
        public override string TypeDescription
        {
            get { return ("Defines a single WaitDI."); }
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
        public override bool CastTo<Q>(out Q waitDI)
        {
            // Cast to WaitDI
            if (typeof(Q).IsAssignableFrom(typeof(WaitDI)))
            {
                if (Value == null)
                    waitDI = default(Q);
                else
                    waitDI = (Q)(object)Value;
                return true;
            }

            waitDI = default(Q);
            return false;
        }

        public override bool CastFrom(object source)
        {
            if (source == null) { return false; }

            // Cast from WaitDI
            if (typeof(WaitDI).IsAssignableFrom(source.GetType()))
            {
                Value = (WaitDI)source;
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

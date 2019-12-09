using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

using RobotComponents.BaseClasses;

namespace RobotComponents.Goos
{
    /// <summary>
    /// Target Goo wrapper class, makes sure Target can be used in Grasshopper.
    /// </summary>
    public class TargetGoo : GH_GeometricGoo<Target>, IGH_PreviewData
    {
        #region constructors
        public TargetGoo() 
        {
            this.Value = new Target();
        }

        public TargetGoo(Target target)
        {
            if (target == null)
                target = new Target();
            this.Value = target;
        }

        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return null;
        }

        public TargetGoo DuplicateTarget()
        {
            return new TargetGoo(Value == null ? new Target() : Value.Duplicate());
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
                if (Value == null) { return "No internal Target instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid JointMovement instance: Did you define a Plane?"; //Todo: beef this up to be more informative.
            }
        }

        public override string ToString()
        {
            if (Value == null)
                return "Null Target";
            else
                return "Target";
        }

        public override string TypeName
        {
            get { return ("Target"); }
        }

        public override string TypeDescription
        {
            get { return ("Defines a single Target"); }
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
        public override bool CastTo<Q>(out Q target)
        {
            //Cast to JointMovement.
            if (typeof(Q).IsAssignableFrom(typeof(Target)))
            {
                if (Value == null)
                    target = default(Q);
                else
                    target = (Q)(object)Value;
                return true;
            }

            target = default(Q);
            return false;
        }

        public override bool CastFrom(object source)
        {
            if (source == null) { return false; }

            //Cast from JointMovement
            if (typeof(Target).IsAssignableFrom(source.GetType()))
            {
                Value = (Target)source;
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

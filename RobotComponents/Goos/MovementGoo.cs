using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

using RobotComponents.BaseClasses;

namespace RobotComponents.Goos
{
    /// <summary>
    /// Movement Goo wrapper class, makes sure the Movement can be used in Grasshopper.
    /// </summary>
    public class MovementGoo : GH_GeometricGoo<Movement>, IGH_PreviewData
    {
        #region constructors
        public MovementGoo()
        {
            this.Value = new Movement();
        }

        public MovementGoo(Movement movement)
        {
            if (movement == null)
                movement = new Movement();
            this.Value = movement;
        }

        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return DuplicateMovementGoo();
        }

        public MovementGoo DuplicateMovementGoo()
        {
            return new MovementGoo(Value == null ? new Movement() : Value.Duplicate());
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
                if (Value == null) { return "No internal Movement instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid Movement instance: Did you define a Plane?"; //Todo: beef this up to be more informative.
            }
        }

        public override string ToString()
        {
            if (Value == null)
                return "Null Movement";
            else
                return "Movement";
        }

        public override string TypeName
        {
            get { return ("Movement"); }
        }

        public override string TypeDescription
        {
            get { return ("Defines a single Movement"); }
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
        public override bool CastTo<Q>(out Q movement)
        {
            //Cast to Movement.
            if (typeof(Q).IsAssignableFrom(typeof(Movement)))
            {
                if (Value == null)
                    movement = default(Q);
                else
                    movement = (Q)(object)Value;
                return true;
            }

            movement = default(Q);
            return false;
        }

        public override bool CastFrom(object source)
        {
            if (source == null) { return false; }

            //Cast from Movement
            if (typeof(Movement).IsAssignableFrom(source.GetType()))
            {
                Value = (Movement)source;
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

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

using RobotComponents.BaseClasses;

namespace RobotComponents.Goos
{
    class SpeedDataGoo : GH_GeometricGoo<SpeedData>, IGH_PreviewData
    {
        #region constructors
        public SpeedDataGoo()
        {
            this.Value = new SpeedData();
        }

        public SpeedDataGoo(SpeedData speedData)
        {
            if (speedData == null)
                speedData = new SpeedData();
            this.Value = speedData;
        }

        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return DuplicateSpeedData();
        }

        public SpeedDataGoo DuplicateSpeedData()
        {
            return new SpeedDataGoo(Value == null ? new SpeedData() : Value.Duplicate());
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
                if (Value == null) { return "No internal SpeedData instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid SpeedData instance: Did you define a name, v_tcp, v_ori, v_leax and v_reax?"; 
            }
        }
        public override string ToString()
        {
            if (Value == null)
                return "Null SpeedData";
            else
                return "Speed Data";
        }
        public override string TypeName
        {
            get { return ("SpeedData"); }
        }
        public override string TypeDescription
        {
            get { return ("Defines a single SpeedData"); }
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
            //Cast to SpeedData.
            if (typeof(Q).IsAssignableFrom(typeof(SpeedData)))
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

            //Cast from SpeedData
            if (typeof(SpeedData).IsAssignableFrom(source.GetType()))
            {
                Value = (SpeedData)source;
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

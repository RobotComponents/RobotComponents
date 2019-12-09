using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

using RobotComponents.BaseClasses;

namespace RobotComponents.Goos
{
    /// <summary>
    /// Digital Output wrapper class, makes sure the Digital Output can be used in Grasshopper.
    /// </summary>
    public class DigitalOutputGoo : GH_GeometricGoo<DigitalOutput>, IGH_PreviewData
    {
        #region constructors
        public DigitalOutputGoo()
        {
            this.Value = new DigitalOutput();
        }

        public DigitalOutputGoo(DigitalOutput digitalOutput)
        {
            if (digitalOutput == null)
                digitalOutput = new DigitalOutput();
            this.Value = digitalOutput;
        }

        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return DuplicateRobotTool();
        }

        public DigitalOutputGoo DuplicateRobotTool()
        {
            return new DigitalOutputGoo(Value == null ? new DigitalOutput() : Value.Duplicate());
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
                if (Value == null) { return "No internal DigitalOutput instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid DigitalOutput instance: Did you define a name and output?"; //Todo: beef this up to be more informative.
            }
        }
        public override string ToString()
        {
            if (Value == null)
                return "Null DigitalOutput";
            else
                return "Digital Output";
        }

        public override string TypeName
        {
            get { return ("DigitalOutput"); }
        }

        public override string TypeDescription
        {
            get { return ("Defines a single JointMovement"); }
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
        public override bool CastTo<Q>(out Q digitalOutput)
        {
            //Cast to DigitalOutput.
            if (typeof(Q).IsAssignableFrom(typeof(DigitalOutput)))
            {
                if (Value == null)
                    digitalOutput = default(Q);
                else
                    digitalOutput = (Q)(object)Value;
                return true;
            }

            digitalOutput = default(Q);
            return false;
        }

        public override bool CastFrom(object source)
        {
            if (source == null) { return false; }

            //Cast from JointMovement
            if (typeof(DigitalOutput).IsAssignableFrom(source.GetType()))
            {
                Value = (DigitalOutput)source;
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

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

using RobotComponents.BaseClasses;

namespace RobotComponents.Goos
{
    public class CodeLineGo : GH_GeometricGoo<CodeLine>, IGH_PreviewData
    {
        #region constructors
        public CodeLineGo()
        {
            this.Value = new CodeLine();
        }

        public CodeLineGo(CodeLine codeLine)
        {
            if (codeLine == null)
                codeLine = new CodeLine();
            this.Value = codeLine;
        }

        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return null;
        }

        public CodeLineGo DuplicateCodeLine()
        {
            return new CodeLineGo(Value == null ? new CodeLine() : Value.Duplicate());
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
                if (Value == null) { return "No internal CodeLine instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid CodeLine instance: Did you define a String?"; //Todo: beef this up to be more informative.
            }
        }
        public override string ToString()
        {
            if (Value == null)
                return "Null CodeLine";
            else
                return "Code Line";
        }
        public override string TypeName
        {
            get { return ("CodeLine"); }
        }
        public override string TypeDescription
        {
            get { return ("Defines a single CodeLine."); }
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
            //Cast to CodeLine.
            if (typeof(Q).IsAssignableFrom(typeof(CodeLine)))
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

            //Cast from CodeLine
            if (typeof(CodeLine).IsAssignableFrom(source.GetType()))
            {
                Value = (CodeLine)source;
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

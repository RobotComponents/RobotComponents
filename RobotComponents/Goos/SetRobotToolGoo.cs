using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

using RobotComponents.BaseClasses;

namespace RobotComponents.Goos
{
    /// <summary>
    /// Movement Goo wrapper class, makes sure Target can be used in Grasshopper.
    /// </summary>
    public class SetRobotToolGoo : GH_GeometricGoo<SetRobotTool>, IGH_PreviewData
    {
        #region constructors
        public SetRobotToolGoo()
        {
            this.Value = new SetRobotTool();
        }

        public SetRobotToolGoo(SetRobotTool setRobotTool)
        {
            if (setRobotTool == null)
                setRobotTool = new SetRobotTool();
            this.Value = setRobotTool;
        }

        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return DuplicateSetRobotTool();
        }

        public SetRobotToolGoo DuplicateSetRobotTool()
        {
            return new SetRobotToolGoo(Value == null ? new SetRobotTool() : Value.Duplicate());
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
                if (Value == null) { return "No internal Set Robot Tool instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid SetRobotTool instance: Did you define a Name?"; //Todo: beef this up to be more informative.
            }
        }

        public override string ToString()
        {
            if (Value == null)
                return "Null Set Robot Tool";
            else
                return "Set Robot Tool";
        }

        public override string TypeName
        {
            get { return ("ChangeTool"); }
        }

        public override string TypeDescription
        {
            get { return ("Defines a single ChangeTool"); }
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
        public override bool CastTo<Q>(out Q changeTool)
        {
            //Cast to ChangeTool.
            if (typeof(Q).IsAssignableFrom(typeof(SetRobotTool)))
            {
                if (Value == null)
                    changeTool = default(Q);
                else
                    changeTool = (Q)(object)Value;
                return true;
            }

            changeTool = default(Q);
            return false;
        }

        public override bool CastFrom(object source)
        {
            if (source == null) { return false; }

            //Cast from ChangeTool
            if (typeof(SetRobotTool).IsAssignableFrom(source.GetType()))
            {
                Value = (SetRobotTool)source;
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

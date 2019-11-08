using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

using RobotComponents.BaseClasses;

namespace RobotComponents.Goos
{

    /// <summary>
    /// RobotTool Goo wrapper class, makes sure RobotTool can be used in Grasshopper.
    /// </summary>
    public class RobotToolGoo : GH_GeometricGoo<RobotTool>, IGH_PreviewData
    {
        #region constructors
        public RobotToolGoo()
        {
            this.Value = new RobotTool();
        }

        public RobotToolGoo(RobotTool robotTool)
        {
            if (robotTool == null)
                robotTool = new RobotTool();
            this.Value = robotTool;
        }

        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return DuplicateRobotTool();
        }

        public RobotToolGoo DuplicateRobotTool()
        {
            return new RobotToolGoo(Value == null ? new RobotTool() : Value.Duplicate());
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
                if (Value == null) { return "No internal RobotTool instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid RobotTool instance: Did you define an AttachmentPlane and ToolPlane?"; //Todo: beef this up to be more informative.
            }
        }
        public override string ToString()
        {
            if (Value == null)
                return "Null RobotTool";
            else
                return "Robot Tool";
        }
        public override string TypeName
        {
            get { return ("RobotTool"); }
        }
        public override string TypeDescription
        {
            get { return ("Defines a single RobotTool"); }
        }

        public override BoundingBox Boundingbox
        {
            get
            {
                if (Value == null) { return BoundingBox.Empty; }
                else if (Value.Mesh == null) { return BoundingBox.Empty; }
                else { return Value.Mesh.GetBoundingBox(true); }
            }
        }
        public override BoundingBox GetBoundingBox(Transform xform)
        {
            return Boundingbox;
        }
        #endregion

        #region casting methods
        public override bool CastTo<Q>(out Q target)
        {
            //Cast to RobotTool.
            if (typeof(Q).IsAssignableFrom(typeof(RobotTool)))
            {
                if (Value == null)
                    target = default(Q);
                else
                    target = (Q)(object)Value;
                return true;
            }

            //Cast to Mesh.
            if (typeof(Q).IsAssignableFrom(typeof(Mesh)))
            {
                if (Value == null)
                    target = default(Q);
                else if (Value.Mesh == null)
                    target = default(Q);
                else
                    target = (Q)(object)Value.Mesh.DuplicateShallow();
                return true;
            }

            //Todo: cast to point, number, mesh, curve?

            target = default(Q);
            return false;
        }
        public override bool CastFrom(object source)
        {
            if (source == null) { return false; }

            //Cast from RobotTool
            if (typeof(RobotTool).IsAssignableFrom(source.GetType()))
            {
                Value = (RobotTool)source;
                return true;
            }

            return false;
        }
        #endregion

        #region transformation methods
        public override IGH_GeometricGoo Transform(Transform xform)
        {
            //It's debatable whether you should maintain a RobotTool through transformations. 
            //It might not be easy/make sense to apply scaling/rotations, shears etc.
            //In this example, I'll convert the RobotTool to a mesh.
            //Perhaps you will want to check for translations/rotations only, operations that make sense.

            //if (Value == null) { return null; }
            //if (Value.Mesh == null) { return null; }

            //Mesh mesh = Value.Mesh.DuplicateMesh();
            //mesh.Transform(xform);
            //return new GH_Mesh(mesh);

            return null;
        }
        public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
        {
            //if (Value == null) { return null; }
            //if (Value.Shape == null) { return null; }

            //Brep brep = Value.Shape.DuplicateBrep();
            //xmorph.Morph(brep);
            //return new GH_Brep(brep);

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
            if (Value == null) { return; }
            if (Value.Mesh != null)
            {
                args.Pipeline.DrawMeshShaded(Value.Mesh, args.Material);
            }
        }
        public void DrawViewportWires(GH_PreviewWireArgs args)
        {
            if (Value == null) { return; }

            //Draw hull shape.
            if (Value.Mesh != null)
            {
                args.Pipeline.DrawMeshWires(Value.Mesh, args.Color, -1);
            }
        }
        #endregion
    }
}

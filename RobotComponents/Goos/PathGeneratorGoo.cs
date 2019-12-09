using System.Collections.Generic;

using Rhino.Geometry;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using RobotComponents.BaseClasses;

namespace RobotComponents.Goos
{
    /// <summary>
    /// PathGenerator Goo wrapper class, makes sure PathGenerator can be used in Grasshopper.
    /// </summary>
    public class PathGeneratorGoo : GH_GeometricGoo<PathGenerator>, IGH_PreviewData
    {
        #region constructors
        public PathGeneratorGoo()
        {
            this.Value = new PathGenerator();
        }

        public PathGeneratorGoo(PathGenerator pathGenerator)
        {
            if (pathGenerator == null)
                pathGenerator = new PathGenerator();
            this.Value = pathGenerator;
        }

        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return DuplicatePathGenerator();
        }

        public PathGeneratorGoo DuplicatePathGenerator()
        {
            return new PathGeneratorGoo(Value == null ? new PathGenerator() : Value.Duplicate());
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
                if (Value == null) { return "No internal PathGenerator instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid PathGenerator instance: Did you define ActionList, Interpolations, Step and RobotInfo"; //Todo: beef this up to be more informative.
            }
        }

        public override string ToString()
        {
            if (Value == null)
                return "Null PathGenerator";
            else
                return "Path Generator";
        }

        public override string TypeName
        {
            get { return ("PathGenerator"); }
        }

        public override string TypeDescription
        {
            get { return ("Defines a single PathGenerator"); }
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
        public override bool CastTo<Q>(out Q pathGenerator)
        {
            //Cast to PathGenerator.
            if (typeof(Q).IsAssignableFrom(typeof(PathGenerator)))
            {
                if (Value == null)
                    pathGenerator = default(Q);
                else
                    pathGenerator = (Q)(object)Value;
                return true;
            }

            //Cast to Mesh.
            if (typeof(Q).IsAssignableFrom(typeof(List<Mesh>)))
            {
            }

            pathGenerator = default(Q);
            return false;
        }

        public override bool CastFrom(object source)
        {
            if (source == null) { return false; }

            //Cast from RobotInfo
            if (typeof(PathGenerator).IsAssignableFrom(source.GetType()))
            {
                Value = (PathGenerator)source;
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

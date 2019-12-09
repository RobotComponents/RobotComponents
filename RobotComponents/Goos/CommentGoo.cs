using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

using RobotComponents.BaseClasses;

namespace RobotComponents.Goos
{
    /// <summary>
    /// Comment wrapper class, makes sure the comment can be used in Grasshopper.
    /// </summary>
    public class CommentGo : GH_GeometricGoo<Comment>, IGH_PreviewData
    {
        #region constructors
        public CommentGo()
        {
            this.Value = new Comment();
        }

        public CommentGo(Comment comment)
        {
            if (comment == null)
                comment = new Comment();
            this.Value = comment;
        }

        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return null;
        }

        public CommentGo DuplicateTarget()
        {
            return new CommentGo(Value == null ? new Comment() : Value.Duplicate());
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
                if (Value == null) { return "No internal Comment instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid Comment instance: Did you define a String?"; //Todo: beef this up to be more informative.
            }
        }

        public override string ToString()
        {
            if (Value == null)
                return "Null Comment";
            else
                return "Comment";
        }

        public override string TypeName
        {
            get { return ("Comment"); }
        }

        public override string TypeDescription
        {
            get { return ("Defines a single Comment."); }
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
        public override bool CastTo<Q>(out Q comment)
        {
            //Cast to Comment.
            if (typeof(Q).IsAssignableFrom(typeof(Comment)))
            {
                if (Value == null)
                    comment = default(Q);
                else
                    comment = (Q)(object)Value;
                return true;
            }

            comment = default(Q);
            return false;
        }

        public override bool CastFrom(object source)
        {
            if (source == null) { return false; }

            //Cast from Comment
            if (typeof(Comment).IsAssignableFrom(source.GetType()))
            {
                Value = (Comment)source;
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

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

using RobotComponents.BaseClasses.Definitions;

namespace RobotComponentsABB.Goos
{
    /// <summary>
    /// RobotTool Goo wrapper class, makes sure RobotTool can be used in Grasshopper.
    /// </summary>
    public class RobotToolGoo : GH_GeometricGoo<RobotTool>, IGH_PreviewData
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public RobotToolGoo()
        {
            this.Value = new RobotTool();
        }

        /// <summary>
        /// Data constructor, m_value will be set to internal_data.
        /// </summary>
        /// <param name="robotTool"> RobotTool Value to store inside this Goo instance. </param>
        public RobotToolGoo(RobotTool robotTool)
        {
            if (robotTool == null)
                robotTool = new RobotTool();
            this.Value = robotTool;
        }

        /// <summary>
        /// Data constructor, m_value will be set to internal_data.
        /// </summary>
        /// <param name="robotToolGoo"> RobotToolGoo to store inside this Goo instance. </param>
        public RobotToolGoo(RobotToolGoo robotToolGoo)
        {
            if (robotToolGoo == null)
                robotToolGoo = new RobotToolGoo();
            this.Value = robotToolGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this geometry. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the RobotToolGoo. </returns>
        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return DuplicateRobotToolGoo();
        }

        /// <summary>
        /// Make a complete duplicate of this geometry. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the RobotToolGoo. </returns>
        public RobotToolGoo DuplicateRobotToolGoo()
        {
            return new RobotToolGoo(Value == null ? new RobotTool() : Value.Duplicate());
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets a value indicating whether or not the current value is valid.
        /// </summary>
        public override bool IsValid
        {
            get
            {
                if (Value == null) { return false; }
                return Value.IsValid;
            }
        }

        /// <summary>
        /// ets a string describing the state of "invalidness". 
        /// If the instance is valid, then this property should return Nothing or String.Empty.
        /// </summary>
        public override string IsValidWhyNot
        {
            get
            {
                if (Value == null) { return "No internal RobotTool instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid RobotTool instance: Did you define an AttachmentPlane and ToolPlane?"; //Todo: beef this up to be more informative.
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null)
                return "Null RobotTool";
            if (Value.Name == "" || Value.Name == null)
                return "Empty Robot Tool";
            else
                return "Robot Tool";
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return ("RobotTool"); }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return ("Defines a single RobotTool"); }
        }

        /// <summary>
        /// Gets the boundingbox for this geometry.
        /// </summary>
        public override BoundingBox Boundingbox
        {
            get
            {
                if (Value == null) { return BoundingBox.Empty; }
                else if (Value.Mesh == null) { return BoundingBox.Empty; }
                else { return Value.Mesh.GetBoundingBox(true); }
            }
        }

        /// <summary>
        /// Compute an aligned boundingbox.
        /// </summary>
        /// <param name="xform"> Transformation to apply to geometry for BoundingBox computation. </param>
        /// <returns> The world aligned boundingbox of the transformed geometry. </returns>
        public override BoundingBox GetBoundingBox(Transform xform)
        {
            return Boundingbox;
        }
        #endregion

        #region casting methods
        /// <summary>
        /// Attempt a cast to type Q.
        /// </summary>
        /// <typeparam name="Q"> Type to cast to.  </typeparam>
        /// <param name="target"> Pointer to target of cast. </param>
        /// <returns> True on success, false on failure. </returns>
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
            if (typeof(Q).IsAssignableFrom(typeof(GH_Mesh)))
            {
                if (Value == null)
                    target = default(Q);
                else if (Value.Mesh == null)
                    target = default(Q);
                else
                    target = (Q)(object) new GH_Mesh(Value.Mesh);
                return true;
            }

            target = default(Q);
            return false;
        }

        /// <summary>
        /// Attempt a cast from generic object.
        /// </summary>
        /// <param name="source"> Reference to source of cast. </param>
        /// <returns> True on success, false on failure. </returns>
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
        /// <summary>
        /// Transforms the object or a deformable representation of the object.
        /// </summary>
        /// <param name="xform"> Transformation matrix. </param>
        /// <returns> Transformed geometry. If the local geometry can be transformed accurately, 
        /// then the returned instance equals this instance. Not all geometry types can be accurately 
        /// transformed under all circumstances though, if this is the case, this function will 
        /// return an instance of another IGH_GeometricGoo derived type which can be transformed.</returns>
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

        /// <summary>
        /// Morph the object or a deformable representation of the object.
        /// </summary>
        /// <param name="xmorph"> Spatial deform. </param>
        /// <returns> Deformed geometry. If the local geometry can be deformed accurately, then the returned 
        /// instance equals this instance. Not all geometry types can be accurately deformed though, if 
        /// this is the case, this function will return an instance of another IGH_GeometricGoo derived 
        /// type which can be deformed.</returns>
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
        /// <summary>
        /// Gets the clipping box for this data. The clipping box is typically the same as the boundingbox.
        /// </summary>
        public BoundingBox ClippingBox
        {
            get { return Boundingbox; }
        }

        /// <summary>
        /// Implement this function to draw all shaded meshes. 
        /// If the viewport does not support shading, this function will not be called.
        /// </summary>
        /// <param name="args"> Drawing arguments. </param>
        public void DrawViewportMeshes(GH_PreviewMeshArgs args)
        {
            if (Value == null) { return; }
            if (Value.Mesh != null)
            {
                args.Pipeline.DrawMeshShaded(Value.Mesh, args.Material);
            }
        }

        /// <summary>
        /// Implement this function to draw all wire and point previews.
        /// </summary>
        /// <param name="args"> Drawing arguments. </param>
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

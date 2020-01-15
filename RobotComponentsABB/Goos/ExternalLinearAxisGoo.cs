using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

using RobotComponents.BaseClasses.Definitions;

namespace RobotComponentsABB.Goos
{
    /// <summary>
    /// ExternalLinearAxis Goo wrapper class, makes sure ExternalLinearAxis can be used in Grasshopper.
    /// </summary>
    public class ExternalLinearAxisGoo : GH_GeometricGoo<ExternalLinearAxis>, IGH_PreviewData
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public ExternalLinearAxisGoo()
        {
            this.Value = new ExternalLinearAxis();
        }

        /// <summary>
        /// Data constructor, m_value will be set to internal_data.
        /// </summary>
        /// <param name="externalLinearAxis"> ExternalLinearAxis Value to store inside this Goo instance. </param>
        public ExternalLinearAxisGoo(ExternalLinearAxis externalLinearAxis)
        {
            if (externalLinearAxis == null)
                externalLinearAxis = new ExternalLinearAxis();
            this.Value = externalLinearAxis;
        }

        /// <summary>
        /// Data constructor, m_value will be set to internal_data.
        /// </summary>
        /// <param name="externalLinearAxisGoo"> ExternalLinearAxisGooe to store inside this Goo instance. </param>
        public ExternalLinearAxisGoo(ExternalLinearAxisGoo externalLinearAxisGoo)
        {
            if (externalLinearAxisGoo == null)
                externalLinearAxisGoo = new ExternalLinearAxisGoo();
            this.Value = externalLinearAxisGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this geometry. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the ExternalLinearAxisGoo. </returns>
        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return DuplicateExternalLinearAxisGoo();
        }

        /// <summary>
        /// Make a complete duplicate of this geometry. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the ExternalLinearAxisGoo. </returns>
        public ExternalLinearAxisGoo DuplicateExternalLinearAxisGoo()
        {
            return new ExternalLinearAxisGoo(Value == null ? new ExternalLinearAxis() : Value.Duplicate());
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
                if (Value == null) { return "No internal ExternalLinearAxis instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid ExternalLinearAxis instance: Did you define an interval?"; //Todo: beef this up to be more informative.
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null)
                return "Null ExternalLinearAxis";
            else
                return "External Linear Axis";
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return ("ExternalLinearAxis"); }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return ("Defines a ExternalLinearAxis."); }
        }

        /// <summary>
        /// Gets the boundingbox for this geometry.
        /// </summary>
        public override BoundingBox Boundingbox
        {
            get
            {
                if (Value == null)
                {
                    return BoundingBox.Empty;
                }

                else
                {
                    BoundingBox MeshBoundingBox = BoundingBox.Empty;

                    // Base mesh
                    if (Value.BaseMesh != null)
                    {
                        MeshBoundingBox.Union(Value.BaseMesh.GetBoundingBox(true));
                    }

                    // Link mesh
                    if (Value.LinkMesh != null)
                    {
                        MeshBoundingBox.Union(Value.BaseMesh.GetBoundingBox(true));
                    }

                    // Axis curve
                    if (Value.AxisCurve != null)
                    {
                        MeshBoundingBox.Union(Value.AxisCurve.GetBoundingBox(true));
                    }

                    return MeshBoundingBox;
                }
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
            //Cast to ExternalLinearAxis.
            if (typeof(Q).IsAssignableFrom(typeof(ExternalLinearAxis)))
            {
                if (Value == null)
                    target = default(Q);
                else
                    target = (Q)(object)Value;
                return true;
            }

            //Cast to Curve.
            if (typeof(Q).IsAssignableFrom(typeof(GH_Curve)))
            {
                if (Value == null)
                    target = default(Q);
                else if (Value.AxisCurve == null)
                    target = default(Q);
                else
                    target = (Q)(object) new GH_Curve(Value.AxisCurve);
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

            //Cast from Wait
            if (typeof(ExternalLinearAxis).IsAssignableFrom(source.GetType()))
            {
                Value = (ExternalLinearAxis)source;
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

            if (Value.BaseMesh != null)
            {
                args.Pipeline.DrawMeshShaded(Value.BaseMesh, new Rhino.Display.DisplayMaterial(System.Drawing.Color.FromArgb(225, 225, 225), 0));
            }

            if (Value.LinkMesh != null)
            {
                args.Pipeline.DrawMeshShaded(Value.LinkMesh, new Rhino.Display.DisplayMaterial(System.Drawing.Color.FromArgb(225, 225, 225), 0));
            }
        }

        /// <summary>
        /// Implement this function to draw all wire and point previews.
        /// </summary>
        /// <param name="args"> Drawing arguments. </param>
        public void DrawViewportWires(GH_PreviewWireArgs args)
        {
            if (Value == null) { return; }

            if (Value.BaseMesh != null)
            {
                args.Pipeline.DrawMeshWires(Value.BaseMesh, args.Color, -1);
            }

            if (Value.LinkMesh != null)
            {
                args.Pipeline.DrawMeshWires(Value.LinkMesh, args.Color, -1);
            }

            if (Value.AxisCurve.IsValid == true && Value.BaseMesh.IsValid == false && Value.LinkMesh.IsValid == false)
            {
                args.Pipeline.DrawCurve(Value.AxisCurve, args.Color, 1);
            }
        }
        #endregion
    }
}

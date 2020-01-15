using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

using RobotComponents.BaseClasses.Actions;

namespace RobotComponentsABB.Goos
{
    /// <summary>
    /// Movement Goo wrapper class, makes sure Target can be used in Grasshopper.
    /// </summary>
    public class OverrideRobotToolGoo : GH_GeometricGoo<OverrideRobotTool>, IGH_PreviewData
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public OverrideRobotToolGoo()
        {
            this.Value = new OverrideRobotTool();
        }

        /// <summary>
        /// Data constructor, m_value will be set to internal_data.
        /// </summary>
        /// <param name="overrideRobotTool"> OverrideRobotTool Value to store inside this Goo instance. </param>
        public OverrideRobotToolGoo(OverrideRobotTool overrideRobotTool)
        {
            if (overrideRobotTool == null)
                overrideRobotTool = new OverrideRobotTool();
            this.Value = overrideRobotTool;
        }

        /// <summary>
        /// Data constructor, m_value will be set to internal_data.
        /// </summary>
        /// <param name="overrideRobotToolGoo"> OverrideRobotToolGoo to store inside this Goo instance. </param>
        public OverrideRobotToolGoo(OverrideRobotToolGoo overrideRobotToolGoo)
        {
            if (overrideRobotToolGoo == null)
                overrideRobotToolGoo = new OverrideRobotToolGoo();
            this.Value = overrideRobotToolGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this geometry. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the OverrideRobotToolGoo. </returns>
        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return DuplicateOverrideRobotToolGoo();
        }

        /// <summary>
        /// Make a complete duplicate of this geometry. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the OverrideRobotToolGoo. </returns>
        public OverrideRobotToolGoo DuplicateOverrideRobotToolGoo()
        {
            return new OverrideRobotToolGoo(Value == null ? new OverrideRobotTool() : Value.Duplicate());
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
                if (Value == null) { return "No internal Set Robot Tool instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid OverrideRobotTool instance: Did you define a Name?"; //Todo: beef this up to be more informative.
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null)
                return "Null Set Robot Tool";
            else
                return "Set Robot Tool";
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return ("ChangeTool"); }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return ("Defines a single ChangeTool"); }
        }

        /// <summary>
        /// Gets the boundingbox for this geometry.
        /// </summary>
        public override BoundingBox Boundingbox
        {
            get
            {
                return BoundingBox.Empty; //Note: beef this up if needed
            }
        }

        /// <summary>
        /// Compute an aligned boundingbox.
        /// </summary>
        /// <param name="xform"> Transformation to apply to geometry for BoundingBox computation. </param>
        /// <returns> The world aligned boundingbox of the transformed geometry. </returns>
        public override BoundingBox GetBoundingBox(Transform xform)
        {
            return BoundingBox.Empty; //Note: beef this up if needed
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
            //Cast to OverrideRobotTool.
            if (typeof(Q).IsAssignableFrom(typeof(OverrideRobotTool)))
            {
                if (Value == null)
                    target = default(Q);
                else
                    target = (Q)(object)Value;
                return true;
            }

            //Cast to RobotToolGoo
            if (typeof(Q).IsAssignableFrom(typeof(RobotToolGoo)))
            {
                if (Value == null)
                    target = default(Q);
                else if (Value.RobotTool == null)
                    target = default(Q);
                else
                    target = (Q)(object) new RobotToolGoo(Value.RobotTool);
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

            //Cast from OverrideRobotTool.
            if (typeof(OverrideRobotTool).IsAssignableFrom(source.GetType()))
            {
                Value = (OverrideRobotTool)source;
                return true;
            }

            //Cast from RobotToolGoo
            if (typeof(RobotToolGoo).IsAssignableFrom(source.GetType()))
            {
                RobotToolGoo robotToolGoo = (RobotToolGoo)source;
                Value = new OverrideRobotTool(robotToolGoo.Value);
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
        }

        /// <summary>
        /// Implement this function to draw all wire and point previews.
        /// </summary>
        /// <param name="args"> Drawing arguments. </param>
        public void DrawViewportWires(GH_PreviewWireArgs args)
        {
        }
        #endregion
    }

}

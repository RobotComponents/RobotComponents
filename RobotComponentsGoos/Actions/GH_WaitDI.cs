// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// Grasshopper Libs
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.BaseClasses.Actions;

namespace RobotComponentsGoos.Actions
{
    /// <summary>
    /// Wait Digital Input wrapper class, makes sure the WaitDI can be used in Grasshopper.
    /// </summary>
    public class GH_WaitDI : GH_GeometricGoo<WaitDI>, IGH_PreviewData
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_WaitDI()
        {
            this.Value = new WaitDI();
        }

        /// <summary>
        /// Data constructor, m_value will be set to internal_data.
        /// </summary>
        /// <param name="waitDI"> WaitDI Value to store inside this Goo instance. </param>
        public GH_WaitDI(WaitDI waitDI)
        {
            if (waitDI == null)
                waitDI = new WaitDI();
            this.Value = waitDI;
        }

        /// <summary>
        /// Data constructor, m_value will be set to internal_data.
        /// </summary>
        /// <param name="waitDIGoo"> WaitDIGoo to store inside this Goo instance. </param>
        public GH_WaitDI(GH_WaitDI waitDIGoo)
        {
            if (waitDIGoo == null)
                waitDIGoo = new GH_WaitDI();
            this.Value = waitDIGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this geometry. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the WaitDIGoo. </returns>
        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return DuplicateWaitDIGoo();
        }

        /// <summary>
        /// Make a complete duplicate of this geometry. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the WaitDIGoo. </returns>
        public GH_WaitDI DuplicateWaitDIGoo()
        {
            return new GH_WaitDI(Value == null ? new WaitDI() : Value.Duplicate());
        }
        #endregion

        #region properties
        public override bool IsValid
        /// <summary>
        /// Gets a value indicating whether or not the current value is valid.
        /// </summary>
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
                if (Value == null) { return "No internal WaitDI instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid WaitDI instance: Did you define the digital input name and value?"; //Todo: beef this up to be more informative.
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null)
                return "Null WaitDI";
            else
                return Value.ToString();
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return ("Wait for Digital Input"); }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return ("Defines a single Wait for Digital Input data."); }
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
            // Cast to WaitDI
            if (typeof(Q).IsAssignableFrom(typeof(WaitDI)))
            {
                if (Value == null)
                    target = default(Q);
                else
                    target = (Q)(object)Value;
                return true;
            }

            // Cast to WaitDIGoo
            if (typeof(Q).IsAssignableFrom(typeof(GH_WaitDI)))
            {
                if (Value == null)
                    target = default(Q);
                else
                    target = (Q)(object)new GH_WaitDI(Value);
                return true;
            }

            //Cast to Action
            if (typeof(Q).IsAssignableFrom(typeof(Action)))
            {
                if (Value == null)
                    target = default(Q);
                else
                    target = (Q)(object)Value;
                return true;
            }

            //Cast to ActionGoo
            if (typeof(Q).IsAssignableFrom(typeof(GH_Action)))
            {
                if (Value == null)
                    target = default(Q);
                else
                    target = (Q)(object)new GH_Action(Value);
                return true;
            }

            //Cast to Boolean
            if (typeof(Q).IsAssignableFrom(typeof(GH_Boolean)))
            {
                if (Value == null)
                    target = default(Q);
                else
                    target = (Q)(object)new GH_Boolean(Value.Value);
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

            // Cast from WaitDI
            if (typeof(WaitDI).IsAssignableFrom(source.GetType()))
            {
                Value = source as WaitDI;
                return true;
            }

            //Cast from Action
            if (typeof(Action).IsAssignableFrom(source.GetType()))
            {
                if (source is WaitDI action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from ActionGoo
            if (typeof(GH_Action).IsAssignableFrom(source.GetType()))
            {
                GH_Action actionGoo = source as GH_Action;
                if (actionGoo.Value is WaitDI action)
                {
                    Value = action;
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region transformation methods
        /// <summary>
        /// Transforms the object or a deformable representation of the object.
        /// </summary>
        /// <param name="xform"> Transformation matrix. </param>
        /// <returns> Returns a null item since this goo instance has no geometry. </returns>
        public override IGH_GeometricGoo Transform(Transform xform)
        {
            return null;
        }

        /// <summary>
        /// Morph the object or a deformable representation of the object.
        /// </summary>
        /// <param name="xmorph"> Spatial deform. </param>
        /// <returns> Returns a null item since this goo instance has no geometry. </returns>
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

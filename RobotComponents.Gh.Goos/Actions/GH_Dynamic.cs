// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// Grasshopper Libs
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GH_IO;
using GH_IO.Serialization;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.Actions;
using RobotComponents.Utils;

namespace RobotComponents.Gh.Goos.Actions
{
    /// <summary>
    /// Dynamic Goo wrapper class, makes sure the Dynamic class can be used in Grasshopper.
    /// </summary>
    public class GH_Dynamic : GH_GeometricGoo<IDynamic>, IGH_PreviewData, GH_ISerializable
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_Dynamic()
        {
            this.Value = null;
        }

        /// <summary>
        /// Data constructor: Creates an Dynamic Goo instance from an Dynamic instance.
        /// </summary>
        /// <param name="dynamic"> Dynamic Value to store inside this Goo instance. </param>
        public GH_Dynamic(IDynamic dynamic)
        {
            this.Value = dynamic;
        }

        /// <summary>
        /// Data constructor: Creates a Dynamic Goo instance from another Dynamic Goo instance.
        /// This creates a shallow copy of the passed Dynamic Goo instance. 
        /// </summary>
        /// <param name="dynamicGoo"> Dynamic Goo instance to copy. </param>
        public GH_Dynamic(GH_Dynamic dynamicGoo)
        {
            if (dynamicGoo == null)
            {
                dynamicGoo = new GH_Dynamic();
            }

            this.Value = dynamicGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this Goo instance. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the Dynamic Goo. </returns>
        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return DuplicateDynamicGoo();
        }

        /// <summary>
        /// Make a complete duplicate of this Goo instance. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the Dynamic Goo. </returns>
        public GH_Dynamic DuplicateDynamicGoo()
        {
            if (Value == null) { return null; }
            else if (Value is IDynamic) { return new GH_Dynamic(Value.DuplicateDynamic()); }
            else { return null; }
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
        /// Gets a string describing the state of "invalidness". 
        /// If the instance is valid, then this property should return Nothing or String.Empty.
        /// </summary>
        public override string IsValidWhyNot
        {
            get
            {
                if (Value == null) { return "No internal Dynamic instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid Dynamic instance";
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null) { return "Null Dynamic"; }
            else { return Value.ToString(); }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return "Dynamic"; }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return "Defines a Dynamic."; }
        }

        /// <summary>
        /// Gets the boundingbox for this geometry.
        /// </summary>
        public override BoundingBox Boundingbox
        {
            get { return BoundingBox.Empty; }
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
        /// <typeparam name="Q"> Type to cast to. </typeparam>
        /// <param name="target"> Pointer to target of cast. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool CastTo<Q>(out Q target)
        {
            //Cast to Action
            if (typeof(Q).IsAssignableFrom(typeof(Action)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)Value; }
                return true;
            }

            //Cast to Action Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_Action)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)new GH_Action(Value as Action); }
                return true;
            }

            //Cast to Dynamic
            if (typeof(Q).IsAssignableFrom(typeof(IDynamic)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)Value; }
                return true;
            }

            //Cast to Dynamic Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_Dynamic)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)new GH_Dynamic(Value as IDynamic); }
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

            //Cast from Dynamic
            if (typeof(IDynamic).IsAssignableFrom(source.GetType()))
            {
                Value = source as IDynamic;
                return true;
            }

            //Cast from Dynamic Goo
            if (typeof(GH_Dynamic).IsAssignableFrom(source.GetType()))
            {
                GH_Dynamic dynamicGoo = source as GH_Dynamic;
                Value = dynamicGoo.Value as IDynamic;
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

        #region (de)serialisation
        /// <summary>
        /// IO key for (de)serialisation of the value inside this Goo.
        /// </summary>
        private const string IoKey = "Dynamic";

        /// <summary>
        /// This method is called whenever the instance is required to serialize itself.
        /// </summary>
        /// <param name="writer"> Writer object to serialize with. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Write(GH_IWriter writer)
        {
            if (this.Value != null)
            {
                byte[] array = HelperMethods.ObjectToByteArray(this.Value);
                writer.SetByteArray(IoKey, array);
            }

            return true;
        }

        /// <summary>
        /// This method is called whenever the instance is required to deserialize itself.
        /// </summary>
        /// <param name="reader"> Reader object to deserialize from. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Read(GH_IReader reader)
        {
            if (!reader.ItemExists(IoKey))
            {
                this.Value = null;
                return true;
            }

            byte[] array = reader.GetByteArray(IoKey);
            this.Value = (IDynamic)HelperMethods.ByteArrayToObject(array);

            return true;
        }
        #endregion
    }
}

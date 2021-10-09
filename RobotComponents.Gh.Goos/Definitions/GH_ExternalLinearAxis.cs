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
using RobotComponents.Definitions;
using RobotComponents.Utils;

namespace RobotComponents.Gh.Goos.Definitions
{
    /// <summary>
    /// External Linear Axis Goo wrapper class, makes sure the External Linear Axis class can be used in Grasshopper.
    /// </summary>
    public class GH_ExternalLinearAxis : GH_GeometricGoo<ExternalLinearAxis>, IGH_PreviewData, GH_ISerializable
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_ExternalLinearAxis()
        {
            this.Value = null;
        }

        /// <summary>
        /// Data constructor: Creates an External Linear Axis Goo instance from an External Linear Axis instance.
        /// </summary>
        /// <param name="externalLinearAxis"> External Linear Axis Value to store inside this Goo instance. </param>
        public GH_ExternalLinearAxis(ExternalLinearAxis externalLinearAxis)
        {
            this.Value = externalLinearAxis;
        }

        /// <summary>
        /// Data constructor: Creates External Linear Axis Goo instance from another External Linear Axis Goo instance.
        /// This creates a shallow copy of the passed External Linear Axis Goo instance. 
        /// </summary>
        /// <param name="externalLinearAxisGoo"> External Linear Axis Goo instance to copy. </param>
        public GH_ExternalLinearAxis(GH_ExternalLinearAxis externalLinearAxisGoo)
        {
            if (externalLinearAxisGoo == null)
            {
                externalLinearAxisGoo = new GH_ExternalLinearAxis();
            }

            this.Value = externalLinearAxisGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this Goo instance. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the External Linear Axis Goo. </returns>
        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return DuplicateExternalLinearAxisGoo();
        }

        /// <summary>
        /// Make a complete duplicate of this Goo instance. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the External Linear Axis Goo. </returns>
        public GH_ExternalLinearAxis DuplicateExternalLinearAxisGoo()
        {
            return new GH_ExternalLinearAxis(Value == null ? new ExternalLinearAxis() : Value.Duplicate());
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
                if (Value == null) { return "No internal External Linear Axis instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid External Linear Axis instance: Did you define an interval, attachment plane and axis vector?";
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null) { return "Null External Linear Axis"; }
            else { return Value.ToString(); }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return "External Linear Axis"; }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return "Defines an External Linear Axis."; }
        }

        /// <summary>
        /// Gets the boundingbox for this geometry.
        /// </summary>
        public override BoundingBox Boundingbox
        {
            get { return GetBoundingBox(new Transform()); }
        }

        /// <summary>
        /// Compute an aligned boundingbox.
        /// </summary>
        /// <param name="xform"> Transformation to apply to geometry for BoundingBox computation. </param>
        /// <returns> The world aligned boundingbox of the transformed geometry. </returns>
        public override BoundingBox GetBoundingBox(Transform xform)
        {
            if (Value == null) { return BoundingBox.Empty; }
            else { return Value.GetBoundingBox(true); }
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
            //Cast to External Linear Axis
            if (typeof(Q).IsAssignableFrom(typeof(ExternalLinearAxis)))
            {
                if (Value == null) { target = default(Q); }
                else if (Value.IsValid == false) { target = default(Q); }
                else { target = (Q)(object)Value; }
                return true;
            }

            //Cast to External Linear Axis Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_ExternalLinearAxis)))
            {
                if (Value == null) { target = default(Q); }
                else if (Value.IsValid == false) { target = default(Q); }
                else { target = (Q)(object)new GH_ExternalLinearAxis(Value); }
                return true;
            }

            //Cast to External Axis Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_ExternalAxis)))
            {
                if (Value == null) { target = default(Q); }
                else if (Value.IsValid == false) { target = default(Q); }
                else { target = (Q)(object)new GH_ExternalAxis(Value); }
                return true;
            }

            //Cast to Plane
            if (typeof(Q).IsAssignableFrom(typeof(GH_Plane)))
            {
                if (Value == null) { target = default(Q); }
                else if (Value.AttachmentPlane == null) { target = default(Q); }
                else { target = (Q)(object)new GH_Plane(Value.AttachmentPlane); }
                return true;
            }

            //Cast to Point
            if (typeof(Q).IsAssignableFrom(typeof(GH_Point)))
            {
                if (Value == null) { target = default(Q); }
                else if (Value.AttachmentPlane == null) { target = default(Q); }
                else { target = (Q)(object)new GH_Point(Value.AttachmentPlane.Origin); }
                return true;
            }

            //Cast to Interval
            if (typeof(Q).IsAssignableFrom(typeof(GH_Interval)))
            {
                if (Value == null) { target = default(Q); }
                else if (Value.AxisLimits == null) { target = default(Q); }
                else { target = (Q)(object)new GH_Interval(Value.AxisLimits); }
                return true;
            }

            //Cast to Axis vector (positive movement direction)
            if (typeof(Q).IsAssignableFrom(typeof(GH_Vector)))
            { 
                if (Value == null) { target = default(Q); }
                else if (Value.AxisPlane == null) { target = default(Q); }
                else { target = (Q)(object)new GH_Vector(Value.AxisPlane.ZAxis); }
                return true;
            }

            //Cast to Curve
            if (typeof(Q).IsAssignableFrom(typeof(GH_Curve)))
            {
                if (Value == null)
                {
                    target = default(Q);
                }
                else if (Value.AxisCurve == null)
                {
                    target = default(Q);
                }
                else
                {
                    Curve curve = Value.AxisCurve.DuplicateCurve();
                    curve.Domain = Value.AxisLimits;
                    target = (Q)(object)new GH_Curve(curve);
                }
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

            //Cast from External Linear Axis
            if (typeof(ExternalLinearAxis).IsAssignableFrom(source.GetType()))
            {
                ExternalLinearAxis externalLinearAxis = source as ExternalLinearAxis;
                Value = externalLinearAxis;
                return true;
            }

            //Cast from External Linear Axis Goo
            if (typeof(GH_ExternalLinearAxis).IsAssignableFrom(source.GetType()))
            {
                GH_ExternalLinearAxis externalLinearAxisGoo = source as GH_ExternalLinearAxis;
                Value = externalLinearAxisGoo.Value;
                return true;
            }

            //Cast from External Axis
            if (typeof(ExternalAxis).IsAssignableFrom(source.GetType()))
            {
                if (source is ExternalLinearAxis externalLinearAxis)
                {
                    Value = externalLinearAxis;
                    return true;
                }
            }

            //Cast from External Axis Goo
            if (typeof(GH_ExternalAxis).IsAssignableFrom(source.GetType()))
            {
                GH_ExternalAxis externalAxisGoo = source as GH_ExternalAxis;                 
                if (externalAxisGoo.Value is ExternalLinearAxis externalLinearAxis)
                {
                    Value = externalLinearAxis;
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
        /// <returns> Transformed geometry. If the local geometry can be transformed accurately, 
        /// then the returned instance equals this instance. Not all geometry types can be accurately 
        /// transformed under all circumstances though, if this is the case, this function will 
        /// return an instance of another IGH_GeometricGoo derived type which can be transformed.</returns>
        public override IGH_GeometricGoo Transform(Transform xform)
        {
            if (Value == null)
            {
                return null;
            }

            else if (Value.IsValid == false)
            {
                return null;
            }

            else
            {
                // Duplicate value
                ExternalLinearAxis externalLinearAxis = Value.Duplicate();
                // Transform
                externalLinearAxis.Transform(xform);
                // Make new Goo instance
                GH_ExternalLinearAxis externalLinearAxisGoo = new GH_ExternalLinearAxis(externalLinearAxis);
                // Return
                return externalLinearAxisGoo;
            }
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
            if (Value == null) 
            { 
                return; 
            }

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

        }
        #endregion

        #region (de)serialisation
        /// <summary>
        /// IO key for (de)serialisation of the value inside this Goo.
        /// </summary>
        private const string IoKey = "External Linear Axis";

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
            this.Value = (ExternalLinearAxis)HelperMethods.ByteArrayToObject(array);

            return true;
        }
        #endregion
    }
}

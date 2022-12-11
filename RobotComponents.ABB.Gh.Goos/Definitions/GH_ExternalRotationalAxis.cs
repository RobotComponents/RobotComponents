// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// Grasshopper Libs
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GH_IO;
using GH_IO.Serialization;
// Rhino Libs
using Rhino.Geometry;
// RobotComponets Libs
using RobotComponents.ABB.Definitions;
using RobotComponents.Utils;

namespace RobotComponents.ABB.Gh.Goos.Definitions
{
    /// <summary>
    /// External Rotational Axis Goo wrapper class, makes sure ExternalRotationalAxis can be used in Grasshopper.
    /// </summary>
    public class GH_ExternalRotationalAxis : GH_GeometricGoo<ExternalRotationalAxis>, IGH_PreviewData, GH_ISerializable
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_ExternalRotationalAxis()
        {
            this.Value = null;
        }

        /// <summary>
        /// Data constructor: Creates an External Rotational Axis Goo instance from a External Rotational Axis.
        /// </summary>
        /// <param name="externalRotationalAxis"> External Rotational Axis Value to store inside this Goo instance. </param>
        public GH_ExternalRotationalAxis(ExternalRotationalAxis externalRotationalAxis)
        {
            this.Value = externalRotationalAxis;
        }

        /// <summary>
        /// Data constructor: Creates an External Rotational Axis Goo instance from another External Rotational Axis Goo instance.
        /// This creates a shallow copy of the passed External Rotational Axis Goo instance. 
        /// </summary>
        /// <param name="externalRotationalAxisGoo"> External Rotational Axis Goo instance to copy. </param>
        public GH_ExternalRotationalAxis(GH_ExternalRotationalAxis externalRotationalAxisGoo)
        {
            if (externalRotationalAxisGoo == null)
            {
                externalRotationalAxisGoo = new GH_ExternalRotationalAxis();
            } 
               
            this.Value = externalRotationalAxisGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this Goo instance. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the External Rational Axis Goo. </returns>
        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return DuplicateExternalRotationalAxisGoo();
        }

        /// <summary>
        /// Make a complete duplicate of this Goo instance. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the External Rational Axis Goo. </returns>
        public GH_ExternalRotationalAxis DuplicateExternalRotationalAxisGoo()
        {
            return new GH_ExternalRotationalAxis(Value == null ? new ExternalRotationalAxis() : Value.Duplicate());
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
        /// If the instance is valid, then this property should return Nothing or string.Empty.
        /// </summary>
        public override string IsValidWhyNot
        {
            get
            {
                if (Value == null) { return "No internal External Rotational Axis instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid External Rotational Axis instance: Did you define an interval, attachment plane and axis plane?";
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null) { return "Null External Rotational Axis"; }
            else { return Value.ToString(); }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return ("External Rotational Axis"); }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return "Defines an External Rotational Axis."; }
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
            //Cast to External Rotational Axis Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_ExternalRotationalAxis)))
            {
                if (Value == null) { target = (Q)(object)new GH_ExternalRotationalAxis(); }
                else { target = (Q)(object)new GH_ExternalRotationalAxis(Value); }
                return true;
            }

            //Cast to External Rotational Axis
            if (typeof(Q).IsAssignableFrom(typeof(ExternalRotationalAxis)))
            {
                if (Value == null) { target = (Q)(object)null; }
                else { target = (Q)(object)Value; }
                return true;
            }

            //Cast to External Axis Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_ExternalAxis)))
            {
                if (Value == null) { target = (Q)(object)new GH_ExternalAxis(); }
                else { target = (Q)(object)new GH_ExternalAxis(Value); }
                return true;
            }

            //Cast to External Axis
            if (typeof(Q).IsAssignableFrom(typeof(ExternalAxis)))
            {
                if (Value == null) { target = (Q)(object)null; }
                else { target = (Q)(object)Value; }
                return true;
            }

            //Cast to Plane
            if (typeof(Q).IsAssignableFrom(typeof(GH_Plane)))
            {
                if (Value == null) { target = default; }
                else { target = (Q)(object)new GH_Plane(Value.AttachmentPlane); }
                return true;
            }

            //Cast to Point
            if (typeof(Q).IsAssignableFrom(typeof(GH_Point)))
            {
                if (Value == null) { target = default; }
                else { target = (Q)(object)new GH_Point(Value.AttachmentPlane.Origin); }
                return true;
            }

            //Cast to Interval
            if (typeof(Q).IsAssignableFrom(typeof(GH_Interval)))
            {
                if (Value == null) { target = default; }
                else { target = (Q)(object)new GH_Interval(Value.AxisLimits); }
                return true;
            }

            //Cast to Bool
            if (typeof(Q).IsAssignableFrom(typeof(GH_Boolean)))
            {
                if (Value == null) { target = default; }
                else { target = (Q)(object)new GH_Boolean(Value.MovesRobot); }
                return true;
            }

            //Cast to Number
            if (typeof(Q).IsAssignableFrom(typeof(GH_Number)))
            {
                if (Value == null) { target = default; }
                else { target = (Q)(object)new GH_Number(Value.AxisNumber); }
                return true;
            }

            target = default;
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

            //Cast from External Rotational Axis Goo
            if (typeof(GH_ExternalRotationalAxis).IsAssignableFrom(source.GetType()))
            {
                GH_ExternalRotationalAxis externalRotationalAxisGoo = source as GH_ExternalRotationalAxis;
                Value = externalRotationalAxisGoo.Value;
                return true;
            }

            //Cast from External Rotational Axis
            if (typeof(ExternalRotationalAxis).IsAssignableFrom(source.GetType()))
            {
                Value = source as ExternalRotationalAxis;
                return true;
            }

            //Cast from External Axis Goo
            if (typeof(GH_ExternalAxis).IsAssignableFrom(source.GetType()))
            {
                GH_ExternalAxis externalAxisGoo = source as GH_ExternalAxis;
                if (externalAxisGoo.Value is ExternalRotationalAxis externalRotationalAxis)
                {
                    Value = externalRotationalAxis;
                    return true;
                }
            }

            //Cast from External Axis
            if (typeof(ExternalAxis).IsAssignableFrom(source.GetType()))
            {
                if (source is ExternalRotationalAxis externalRotationalAxis)
                {
                    Value = externalRotationalAxis;
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
                ExternalRotationalAxis externalRotationalAxis = Value.Duplicate();
                // Transform
                externalRotationalAxis.Transform(xform);
                // Make new Goo instance
                GH_ExternalRotationalAxis externalRotationalAxisGoo = new GH_ExternalRotationalAxis(externalRotationalAxis);
                // Return
                return externalRotationalAxisGoo;
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
        private const string IoKey = "External Rotational Axiw";

        /// <summary>
        /// This method is called whenever the instance is required to serialize itself.
        /// </summary>
        /// <param name="writer"> Writer object to serialize with. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Write(GH_IWriter writer)
        {
            if (this.Value != null)
            {
                byte[] array = Serialization.ObjectToByteArray(this.Value);
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
            this.Value = (ExternalRotationalAxis)Serialization.ByteArrayToObject(array);

            return true;
        }
        #endregion
    }
}

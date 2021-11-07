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
    /// Robot Tool Goo wrapper class, makes sure the Robot Tool class can be used in Grasshopper.
    /// </summary>
    public class GH_RobotTool : GH_GeometricGoo<RobotTool>, IGH_PreviewData, GH_ISerializable
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_RobotTool()
        {
            this.Value = null;
        }

        /// <summary>
        /// Data constructor: Create a Robot Tool instance from a Robot Tool.
        /// </summary>
        /// <param name="robotTool"> Robot Tool Value to store inside this Goo instance. </param>
        public GH_RobotTool(RobotTool robotTool)
        {
            this.Value = robotTool;
        }

        /// <summary>
        /// Data constructor: Creates a Robot Tool Goo instance from another Robot Tool Goo instance.
        /// This creates a shallow copy of the passed Robot Tool Goo instance. 
        /// </summary>
        /// <param name="robotToolGoo"> Robot Tool Goo instance to copy. </param>
        public GH_RobotTool(GH_RobotTool robotToolGoo)
        {
            if (robotToolGoo == null)
            {
                robotToolGoo = new GH_RobotTool();
            }

            this.Value = robotToolGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this Goo insance. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the Robot Tool Goo. </returns>
        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return DuplicateRobotToolGoo();
        }

        /// <summary>
        /// Make a complete duplicate of this geometry. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the Robot Tool Goo. </returns>
        public GH_RobotTool DuplicateRobotToolGoo()
        {
            return new GH_RobotTool(Value == null ? new RobotTool() : Value.Duplicate());
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
                if (Value == null) { return "No internal Robot Tool instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid Robot Tool instance: Did you define the attachment plane and TCP plane?";
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null) { return "Null Robot Tool"; }
            if (Value.Name == "" || Value.Name == null) { return "Empty Robot Tool"; }
            else { return Value.ToString(); }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return "RobotTool"; }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return "Defines a single RobotTool"; }
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
            //Cast to Robot Tool
            if (typeof(Q).IsAssignableFrom(typeof(RobotTool)))
            {
                if (Value == null) { target = default; }
                else { target = (Q)(object)Value; }
                return true;
            }

            //Cast to Mesh
            if (typeof(Q).IsAssignableFrom(typeof(GH_Mesh)))
            {
                if (Value == null) { target = default; }
                else if (Value.Mesh == null) { target = default; }
                else { target = (Q)(object)new GH_Mesh(Value.Mesh); }
                return true;
            }

            //Cast to Plane
            if (typeof(Q).IsAssignableFrom(typeof(GH_Plane)))
            {
                if (Value == null) { target = default; }
                else if (Value.ToolPlane == null) { target = default; }
                else { target = (Q)(object)new GH_Plane(Value.ToolPlane); }
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

            //Cast from Robot Tool
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
                RobotTool robotTool = Value.Duplicate();
                // Transform
                robotTool.Transform(xform);
                // Make new Goo instance
                GH_RobotTool robotToolGoo = new GH_RobotTool(robotTool);
                // Return
                return robotToolGoo;
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

            if (Value.Mesh != null)
            {
                args.Pipeline.DrawMeshShaded(Value.Mesh, new Rhino.Display.DisplayMaterial(System.Drawing.Color.FromArgb(225, 225, 225), 0));
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
        private const string IoKey = "Robot Tool";

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
            this.Value = (RobotTool)HelperMethods.ByteArrayToObject(array);

            return true;
        }
        #endregion
    }
}

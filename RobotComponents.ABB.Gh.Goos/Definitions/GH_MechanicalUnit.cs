// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// Grasshopper Libs
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GH_IO;
using GH_IO.Serialization;
// Rhino Libs
using Rhino.Geometry;
// RbootComponents Libs
using RobotComponents.ABB.Definitions;
using RobotComponents.Utils;

namespace RobotComponents.ABB.Gh.Goos.Definitions
{
    /// <summary>
    /// Mechanical Unit Goo wrapper class, makes sure the Mechanical Unit class can be used in Grasshopper.
    /// </summary>
    public class GH_MechanicalUnit : GH_GeometricGoo<IMechanicalUnit>, IGH_PreviewData, GH_ISerializable
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_MechanicalUnit()
        {
            this.Value = null;
        }

        /// <summary>
        /// Data constructor: Creates an Mechanical Unit Goo instance from an Mechanical Unit instance.
        /// </summary>
        /// <param name="mechanicalUnit"> Mechanical Unit Value to store inside this Goo instance. </param>
        public GH_MechanicalUnit(IMechanicalUnit mechanicalUnit)
        {
            this.Value = mechanicalUnit;
        }

        /// <summary>
        /// Data constructor: Creates a Mechanical Unit Goo instance from another Mechanical Unit Goo instance.
        /// This creates a shallow copy of the passed Mechanical Unit Goo instance. 
        /// </summary>
        /// <param name="mechanicalUnitGoo"> Mechanical Unit Goo instance to copy </param>
        public GH_MechanicalUnit(GH_MechanicalUnit mechanicalUnitGoo)
        {
            if (mechanicalUnitGoo == null)
            {
                mechanicalUnitGoo = new GH_MechanicalUnit();
            }

            this.Value = mechanicalUnitGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this Goo instance. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the Mechanical Unit Goo. </returns>
        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return DuplicateMechanicalUnitGoo();
        }

        /// <summary>
        /// Make a complete duplicate of this Goo instance. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the Mechanical Unit Goo. </returns>
        public GH_MechanicalUnit DuplicateMechanicalUnitGoo()
        {
            if (Value == null) { return new GH_MechanicalUnit(); }
            else  { return new GH_MechanicalUnit(Value.DuplicateMechanicalUnit()); }
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
                if (Value == null) { return "No internal Mechanical Unit instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid Mechanical Unit instance.";
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null) { return "Null Mechanical Unit"; }
            else { return Value.ToString(); }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return "Mechanical Unit"; }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return "Defines an Mechanical Unit."; }
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
            //Cast to Mechanical Unit Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_MechanicalUnit)))
            {
                if (Value == null) { target = (Q)(object)new GH_MechanicalUnit(); }
                else { target = (Q)(object)new GH_MechanicalUnit(Value); }
                return true;
            }

            //Cast to Mechanical Unit
            if (typeof(Q).IsAssignableFrom(typeof(IMechanicalUnit)))
            {
                if (Value == null) { target = (Q)(object)null; }
                else { target = (Q)(object)Value; }
                return true;
            }

            //Cast to Robot Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_Robot)))
            {
                if (Value == null) { target = (Q)(object)new GH_Robot(); }
                else if (Value is Robot) { target = (Q)(object)new GH_Robot(Value as Robot); }
                else { target = default; }
                return true;
            }

            //Cast to Robot
            if (typeof(Q).IsAssignableFrom(typeof(Robot)))
            {
                if (Value == null) { target = (Q)(object)null; }
                else if (Value is Robot) { target = (Q)(object)Value; }
                else { target = default; }
                return true;
            }

            //Cast to External Linear Axis Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_ExternalLinearAxis)))
            {
                if (Value == null) { target = (Q)(object)new GH_ExternalLinearAxis(); }
                else if (Value is ExternalLinearAxis) { target = (Q)(object)new GH_ExternalLinearAxis(Value as ExternalLinearAxis); }
                else { target = default; }
                return true;
            }

            //Cast to External Linear Axis
            if (typeof(Q).IsAssignableFrom(typeof(ExternalLinearAxis)))
            {
                if (Value == null) { target = (Q)(object)null; }
                else if (Value is ExternalLinearAxis) { target = (Q)(object)Value; }
                else { target = default; }
                return true;
            }

            //Cast to External Rotational Axis Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_ExternalRotationalAxis)))
            {
                if (Value == null) { target = (Q)(object)new GH_ExternalRotationalAxis(); }
                else if (Value is ExternalRotationalAxis) { target = (Q)(object)new GH_ExternalRotationalAxis(Value as ExternalRotationalAxis); }
                else { target = default; }
                return true;
            }

            //Cast to External Rotational Axis
            if (typeof(Q).IsAssignableFrom(typeof(ExternalRotationalAxis)))
            {
                if (Value == null) { target = (Q)(object)null; }
                else if (Value is ExternalRotationalAxis) { target = (Q)(object)Value; }
                else { target = default; }
                return true;
            }

            //Cast to External Axis Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_ExternalAxis)))
            {
                if (Value == null) { target = (Q)(object)new GH_ExternalAxis(); }
                else if (Value is ExternalAxis) { target = (Q)(object)new GH_ExternalAxis(Value as ExternalAxis); }
                else { target = default; }
                return true;
            }

            //Cast to External Axis
            if (typeof(Q).IsAssignableFrom(typeof(ExternalAxis)))
            {
                if (Value == null) { target = (Q)(object)null; }
                else if (Value is ExternalAxis) { target = (Q)(object)Value; }
                else { target = default; }
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

            //Cast from Mechanical Unit Goo
            if (typeof(GH_MechanicalUnit).IsAssignableFrom(source.GetType()))
            {
                GH_MechanicalUnit mechanicalUnitGoo = source as GH_MechanicalUnit;
                Value = mechanicalUnitGoo.Value;
                return true;
            }

            //Cast from Mechanical Unit
            if (typeof(IMechanicalUnit).IsAssignableFrom(source.GetType()))
            {
                Value = source as IMechanicalUnit;
                return true;
            }

            //Cast from Robot Goo
            if (typeof(GH_Robot).IsAssignableFrom(source.GetType()))
            {
                GH_Robot robotGoo = source as GH_Robot;
                Value = robotGoo.Value;
                return true;
            }

            //Cast from Robot
            if (typeof(Robot).IsAssignableFrom(source.GetType()))
            {
                Value = source as IMechanicalUnit;
                return true;
            }

            //Cast from External Linear Axis Goo
            if (typeof(GH_ExternalLinearAxis).IsAssignableFrom(source.GetType()))
            {
                GH_ExternalLinearAxis externalLinearAxisGoo = source as GH_ExternalLinearAxis;
                Value = externalLinearAxisGoo.Value;
                return true;
            }

            //Cast from External Linear Axis
            if (typeof(ExternalLinearAxis).IsAssignableFrom(source.GetType()))
            {
                Value = source as IMechanicalUnit;
                return true;
            }

            //Cast from External Rotational Axis Goo
            if (typeof(GH_ExternalRotationalAxis).IsAssignableFrom(source.GetType()))
            {
                GH_ExternalRotationalAxis externalRotationalAxisGoo = source as GH_ExternalRotationalAxis;
                Value = externalRotationalAxisGoo.Value;
                return true;
            }

            //Cast from External Rotatioanl Axis
            if (typeof(ExternalRotationalAxis).IsAssignableFrom(source.GetType()))
            {
                Value = source as IMechanicalUnit;
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

            else if (Value is IMechanicalUnit)
            {
                // Get value and duplicate
                IMechanicalUnit mechanicalUnit = Value.DuplicateMechanicalUnit();
                // Transform
                mechanicalUnit.Transform(xform);
                // Make new Goo instance
                GH_MechanicalUnit mechanicalUnitGoo = new GH_MechanicalUnit(mechanicalUnit);
                // Return
                return mechanicalUnitGoo;
            }

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
            if (Value == null) 
            { 
                return; 
            }

            if (Value is Robot robot)
            {
                // Robot meshes
                if (robot.Meshes != null)
                {
                    for (int i = 0; i != robot.Meshes.Count; i++)
                    {
                        args.Pipeline.DrawMeshShaded(robot.Meshes[i], new Rhino.Display.DisplayMaterial(System.Drawing.Color.FromArgb(225, 225, 225), 0));
                    }
                }

                // External axis meshes
                for (int i = 0; i != robot.ExternalAxes.Count; i++)
                {
                    if (robot.ExternalAxes[i].IsValid == true)
                    {
                        if (robot.ExternalAxes[i].BaseMesh != null)
                        {
                            args.Pipeline.DrawMeshShaded(robot.ExternalAxes[i].BaseMesh, new Rhino.Display.DisplayMaterial(System.Drawing.Color.FromArgb(225, 225, 225), 0));
                        }

                        if (robot.ExternalAxes[i].LinkMesh != null)
                        {
                            args.Pipeline.DrawMeshShaded(robot.ExternalAxes[i].LinkMesh, new Rhino.Display.DisplayMaterial(System.Drawing.Color.FromArgb(225, 225, 225), 0));
                        }
                    }
                }
            }
            else if (Value is ExternalAxis externalAxis)
            {
                if (externalAxis.BaseMesh != null)
                {
                    args.Pipeline.DrawMeshShaded(externalAxis.BaseMesh, new Rhino.Display.DisplayMaterial(System.Drawing.Color.FromArgb(225, 225, 225), 0));
                }

                if (externalAxis.LinkMesh != null)
                {
                    args.Pipeline.DrawMeshShaded(externalAxis.LinkMesh, new Rhino.Display.DisplayMaterial(System.Drawing.Color.FromArgb(225, 225, 225), 0));
                }
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
        private const string IoKey = "Mechanical Unit";

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
            this.Value = (IMechanicalUnit)Serialization.ByteArrayToObject(array);

            return true;
        }
        #endregion
    }
}

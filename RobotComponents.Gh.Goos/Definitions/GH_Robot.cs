// This file is part of RobotComponents. RobotComponents is licensed under 
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
// RobotComponents Libs
using RobotComponents.Definitions;
using RobotComponents.Utils;

namespace RobotComponents.Gh.Goos.Definitions
{
    /// <summary>
    /// Robot Goo wrapper class, makes sure the Robot class can be used in Grasshopper.
    /// </summary>
    public class GH_Robot : GH_GeometricGoo<Robot>, IGH_PreviewData, GH_ISerializable
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_Robot()
        {
            this.Value = null;
        }

        /// <summary>
        /// Data constructor: Creates a Robot Goo instance from a Robot instance.
        /// </summary>
        /// <param name="robot"> Robot to store inside this Goo instance. </param>
        public GH_Robot(Robot robot)
        {
            this.Value = robot;
        }

        /// <summary>
        /// Data constructor: Creates a Robot Goo instance from another Robot Goo instance.
        /// This creates a shallow copy of the passed Robot Goo instance. 
        /// </summary>
        /// <param name="robotGoo"> Robot Goo instance to copy. </param>
        public GH_Robot(GH_Robot robotGoo)
        {
            if (robotGoo == null)
            {
                robotGoo = new GH_Robot();
            }

            this.Value = robotGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this Goo insance. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the Robot Goo instance. </returns>
        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return DuplicateRobotInfoGoo();
        }

        /// <summary>
        /// Make a complete duplicate of this Goo instance. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the Robot Goo instance. </returns>
        public GH_Robot DuplicateRobotInfoGoo()
        {
            return new GH_Robot(Value == null ? new Robot() : Value.Duplicate());
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
                if (Value == null) { return "No internal Robot instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid Robot instance: Did you define the axis limits, position plane, axis planes and tool attachment plane?";
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null) { return "Null Robot"; }
            else { return Value.ToString(); }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return "Robot"; }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return "Defines a Robot"; }
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
            //Cast to Robot Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_Robot)))
            {
                if (Value == null) { target = (Q)(object)new GH_Robot(); }
                else { target = (Q)(object)new GH_Robot(Value); }
                return true;
            }

            //Cast to Robot
            if (typeof(Q).IsAssignableFrom(typeof(Robot)))
            {
                if (Value == null) { target = (Q)(object)null; }
                else { target = (Q)(object)Value; }
                return true;
            }

            //Cast to Robot Tool Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_RobotTool)))
            {
                if (Value == null) { target = (Q)(object)new GH_RobotTool(); }
                else { target = (Q)(object)new GH_RobotTool(Value.Tool); }
                return true;
            }

            //Cast to Robot Tool
            if (typeof(Q).IsAssignableFrom(typeof(GH_RobotTool)))
            {
                if (Value == null) { target = (Q)(object)null; }
                else { target = (Q)(object)Value.Tool; }
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
                Value = source as Robot;
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
                Robot robot = Value.Duplicate();
                // Transform
                robot.Transform(xform);
                // Transform external axes that move the robot
                for (int i = 0; i < robot.ExternalAxes.Count; i++)
                {
                    if (robot.ExternalAxes[i].MovesRobot == true)
                    {
                        robot.ExternalAxes[i].Transform(xform);
                    }
                }
                // Make new Goo instance
                GH_Robot robotGoo = new GH_Robot(robot);
                // Return
                return robotGoo;
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

            // Robot meshes
            if (Value.Meshes != null)
            {
                for (int i = 0; i != Value.Meshes.Count; i++)
                {
                    args.Pipeline.DrawMeshShaded(Value.Meshes[i], new Rhino.Display.DisplayMaterial(System.Drawing.Color.FromArgb(225, 225, 225), 0));
                }
            }

            // External axis meshes
            for (int i = 0; i != Value.ExternalAxes.Count; i++)
            {
                if (Value.ExternalAxes[i].IsValid == true)
                {
                    if (Value.ExternalAxes[i].BaseMesh != null)
                    {
                        args.Pipeline.DrawMeshShaded(Value.ExternalAxes[i].BaseMesh, new Rhino.Display.DisplayMaterial(System.Drawing.Color.FromArgb(225, 225, 225), 0));
                    }

                    if (Value.ExternalAxes[i].LinkMesh != null)
                    {
                        args.Pipeline.DrawMeshShaded(Value.ExternalAxes[i].LinkMesh, new Rhino.Display.DisplayMaterial(System.Drawing.Color.FromArgb(225, 225, 225), 0));
                    }
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
        private const string IoKey = "Robot";

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
            this.Value = (Robot)HelperMethods.ByteArrayToObject(array);

            return true;
        }
        #endregion
    }
}

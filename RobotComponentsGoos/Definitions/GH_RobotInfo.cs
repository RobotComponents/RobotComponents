// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/EDEK-UniKassel/RobotComponents>.

// Grasshopper Libs
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.BaseClasses.Definitions;

namespace RobotComponentsGoos.Definitions
{
    /// <summary>
    /// RobotInfo Goo wrapper class, makes sure RobotInfo can be used in Grasshopper.
    /// </summary>
    public class GH_RobotInfo : GH_GeometricGoo<RobotInfo>, IGH_PreviewData
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_RobotInfo()
        {
            this.Value = new RobotInfo();
        }

        /// <summary>
        /// Data constructor, m_value will be set to internal_data.
        /// </summary>
        /// <param name="robotInfo"> RobotInfo Value to store inside this Goo instance. </param>
        public GH_RobotInfo(RobotInfo robotInfo)
        {
            if (robotInfo == null)
                robotInfo = new RobotInfo();
            this.Value = robotInfo;
        }

        /// <summary>
        /// Data constructor, m_value will be set to internal_data.
        /// </summary>
        /// <param name="robotInfoGoo"> RobotInfoGoo to store inside this Goo instance. </param>
        public GH_RobotInfo(GH_RobotInfo robotInfoGoo)
        {
            if (robotInfoGoo == null)
                robotInfoGoo = new GH_RobotInfo();
            this.Value = robotInfoGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this geometry. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the RobotInfoGoo. </returns>
        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return DuplicateRobotInfoGoo();
        }

        /// <summary>
        /// Make a complete duplicate of this geometry. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the RobotInfoGoo. </returns>
        public GH_RobotInfo DuplicateRobotInfoGoo()
        {
            return new GH_RobotInfo(Value == null ? new RobotInfo() : Value.Duplicate());
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
                if (Value == null) { return "No internal RobotInfo instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid RobotInfo instance: Did you define the axis limits, position plane, axis planes and tool attachment plane?"; //Todo: beef this up to be more informative.
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null)
                return "Null RobotInfo";
            else
                return Value.ToString();
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return ("RobotInfo"); }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return ("Defines a single RobotInfo"); }
        }

        /// <summary>
        /// Gets the boundingbox for this geometry.
        /// </summary>
        public override BoundingBox Boundingbox
        {
            get
            {
                if (Value == null) { return BoundingBox.Empty; }
                if (Value.Meshes == null) { return BoundingBox.Empty; }
                else
                {
                    // Make an empty bounding box
                    BoundingBox MeshBoundingBox = BoundingBox.Empty;

                    // Make the bounding box of the robot meshes
                    for (int i = 0; i != Value.Meshes.Count; i++)
                    {
                        MeshBoundingBox.Union(Value.Meshes[i].GetBoundingBox(true));
                    }

                    // Make the bounding box of the external axes
                    for (int i = 0; i != Value.ExternalAxis.Count; i++)
                    {
                        if (Value.ExternalAxis[i].IsValid == true)
                        {
                            if (Value.ExternalAxis[i].BaseMesh != null)
                            {
                                MeshBoundingBox.Union(Value.ExternalAxis[i].BaseMesh.GetBoundingBox(true));
                            }

                            if (Value.ExternalAxis[i].LinkMesh != null)
                            {
                                MeshBoundingBox.Union(Value.ExternalAxis[i].LinkMesh.GetBoundingBox(true));
                            }
                        }
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
            //Cast to RobotInfo.
            if (typeof(Q).IsAssignableFrom(typeof(RobotInfo)))
            {
                if (Value == null)
                    target = default(Q);
                else
                    target = (Q)(object)Value;
                return true;
            }

            //Cast to RobotTool
            if (typeof(Q).IsAssignableFrom(typeof(GH_RobotTool)))
            {
                if (Value == null)
                    target = default(Q);
                else if (Value.Tool == null)
                    target = default(Q);
                else
                    target = (Q)(object)new GH_RobotTool(Value.Tool);
                return true;
            }

            //Cast to Mesh.

            // Casting is only possible from one to one (does not work with lists)
            // The Robot Info mesh is stored in a list.
            // It can only be returned as one single mesh and not as list with a mesh for the separate links. 
            // Therefore, casting to a mesh is not implemented. 
            // If the users wants to get the robot mesh they can use the Deconstuct Robot Info Component. 

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

            //Cast from RobotInfo
            if (typeof(RobotInfo).IsAssignableFrom(source.GetType()))
            {
                Value = (RobotInfo)source;
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

            // Robot meshes
            if (Value.Meshes != null)
            {
                for (int i = 0; i != Value.Meshes.Count; i++)
                {
                    args.Pipeline.DrawMeshShaded(Value.Meshes[i], new Rhino.Display.DisplayMaterial(System.Drawing.Color.FromArgb(225, 225, 225), 0));
                }
            }

            // External axis meshes
            for (int i = 0; i != Value.ExternalAxis.Count; i++)
            {
                if (Value.ExternalAxis[i].IsValid == true)
                {
                    if (Value.ExternalAxis[i].BaseMesh != null)
                    {
                        args.Pipeline.DrawMeshShaded(Value.ExternalAxis[i].BaseMesh, new Rhino.Display.DisplayMaterial(System.Drawing.Color.FromArgb(225, 225, 225), 0));
                    }

                    if (Value.ExternalAxis[i].LinkMesh != null)
                    {
                        args.Pipeline.DrawMeshShaded(Value.ExternalAxis[i].LinkMesh, new Rhino.Display.DisplayMaterial(System.Drawing.Color.FromArgb(225, 225, 225), 0));
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
    }
}

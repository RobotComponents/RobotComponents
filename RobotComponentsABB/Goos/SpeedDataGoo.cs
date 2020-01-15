using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

using RobotComponents.BaseClasses.Actions;

namespace RobotComponentsABB.Goos
{
    /// <summary>
    /// SpeedData wrapper class, makes sure SpeedData can be used in Grasshopper.
    /// </summary>
    class SpeedDataGoo : GH_GeometricGoo<SpeedData>, IGH_PreviewData
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public SpeedDataGoo()
        {
            this.Value = new SpeedData();
        }

        /// <summary>
        /// Data constructor, m_value will be set to internal_data.
        /// </summary>
        /// <param name="speedData"> SpeedData Value to store inside this Goo instance. </param>
        public SpeedDataGoo(SpeedData speedData)
        {
            if (speedData == null)
                speedData = new SpeedData();
            this.Value = speedData;
        }

        /// <summary>
        /// Data constructor, m_value will be set to internal_data.
        /// </summary>
        /// <param name="speedDataGoo"> SpeedDataGoo to store inside this Goo instance. </param>
        public SpeedDataGoo(SpeedDataGoo speedDataGoo)
        {
            if (speedDataGoo == null)
                speedDataGoo = new SpeedDataGoo();
            this.Value = speedDataGoo.Value;
        }

        /// <summary>
        /// Data constructor, m_value will be set to internal_data.
        /// </summary>
        /// <param name="tcpSpeed"> The tool center point speed in mm/s to create 
        /// the the SpeedData value stored inside this Goo instance. </param>
        public SpeedDataGoo(GH_Number tcpSpeed)
        {
            this.Value = new SpeedData(tcpSpeed.Value);
        }

        /// <summary>
        /// Data constructor, m_value will be set to internal_data.
        /// </summary>
        /// <param name="tcpSpeed"> The tool center point speed in mm/s to create 
        /// the the SpeedData value stored inside this Goo instance. </param>
        public SpeedDataGoo(double tcpSpeed)
        {
            this.Value = new SpeedData(tcpSpeed);
        }

        /// <summary>
        /// Make a complete duplicate of this geometry. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the SpeedDataGoo. </returns>
        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return DuplicateSpeedDataGoo();
        }

        /// <summary>
        /// Make a complete duplicate of this geometry. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the SpeedDataGoo. </returns>
        public SpeedDataGoo DuplicateSpeedDataGoo()
        {
            return new SpeedDataGoo(Value == null ? new SpeedData() : Value.Duplicate());
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
                if (Value == null) { return "No internal SpeedData instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid SpeedData instance: Did you define a name, v_tcp, v_ori, v_leax and v_reax?"; 
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null)
            {
                return "Null SpeedData";
            }
            else if (Value.PreDefinied == true)
            {
                return "Predefined Speed Data";
            }
            else
            {
                return "Custom Speed Data";
            }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return ("SpeedData"); }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return ("Defines a single SpeedData"); }
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
        /// <typeparam name="Q"> Type to cast to. </typeparam>
        /// <param name="target"> Pointer to target of cast. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool CastTo<Q>(out Q target)
        {
            //Cast to SpeedData.
            if (typeof(Q).IsAssignableFrom(typeof(SpeedData)))
            {
                if (Value == null)
                    target = default(Q);
                else
                    target = (Q)(object)Value;
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

            //Cast from SpeedData: Custom SpeedData
            if (typeof(SpeedData).IsAssignableFrom(source.GetType()))
            {
                Value = (SpeedData)source;
                return true;
            }

            //Cast from number: Predefined SpeedData
            if (typeof(GH_Number).IsAssignableFrom(source.GetType()))
            {
                Value = new SpeedData((source as GH_Number).Value);
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

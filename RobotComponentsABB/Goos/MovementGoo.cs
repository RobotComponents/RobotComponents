using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

using RobotComponents.BaseClasses;

namespace RobotComponentsABB.Goos
{
    /// <summary>
    /// Movement Goo wrapper class, makes sure the Movement can be used in Grasshopper.
    /// </summary>
    public class MovementGoo : GH_GeometricGoo<Movement>, IGH_PreviewData
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public MovementGoo()
        {
            this.Value = new Movement();
        }

        /// <summary>
        /// Data constructor, m_value will be set to internal_data.
        /// </summary>
        /// <param name="movement"> Movement Value to store inside this Goo instance. </param>
        public MovementGoo(Movement movement)
        {
            if (movement == null)
                movement = new Movement();
            this.Value = movement;
        }

        /// <summary>
        /// Data constructor, m_value will be set to internal_data.
        /// </summary>
        /// <param name="movementGoo"> MovementGoo to store inside this Goo instance. </param>
        public MovementGoo(MovementGoo movementGoo)
        {
            if (movementGoo == null)
                movementGoo = new MovementGoo();
            this.Value = movementGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this geometry. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the MovementGoo. </returns>
        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return DuplicateMovementGoo();
        }

        /// <summary>
        /// Make a complete duplicate of this geometry. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the MovementGoo. </returns>
        public MovementGoo DuplicateMovementGoo()
        {
            return new MovementGoo(Value == null ? new Movement() : Value.Duplicate());
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
                if (Value == null) { return "No internal Movement instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid Movement instance: Did you define a Plane?"; //Todo: beef this up to be more informative.
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
                return "Null Movement";
            }

            else if (Value.MovementType == 0)
            {
                return "Absolute Joint Movement";
            }

            else if (Value.MovementType == 1)
            {
                return "Linear Movement";
            }

            else if (Value.MovementType == 2)
            {
                return "Joint Movement";
            }

            else
            {
                return "Movement";
            }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return ("Movement"); }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return ("Defines a single Movement"); }
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
        /// <param name="movement"> Pointer to target of cast. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool CastTo<Q>(out Q movement)
        {
            //Cast to Movement.
            if (typeof(Q).IsAssignableFrom(typeof(Movement)))
            {
                if (Value == null)
                    movement = default(Q);
                else
                    movement = (Q)(object)Value;
                return true;
            }

            movement = default(Q);
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

            //Cast from Movement
            if (typeof(Movement).IsAssignableFrom(source.GetType()))
            {
                Value = (Movement)source;
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
            Plane plane = Value.GlobalTargetPlane;
            args.Pipeline.DrawDirectionArrow(plane.Origin, plane.ZAxis, System.Drawing.Color.Blue);
            args.Pipeline.DrawDirectionArrow(plane.Origin, plane.XAxis, System.Drawing.Color.Red);
            args.Pipeline.DrawDirectionArrow(plane.Origin, plane.YAxis, System.Drawing.Color.Green);
        }

        /// <summary>
        /// Implement this function to draw all wire and point previews.
        /// </summary>
        /// <param name="args"> Drawing arguments. </param>
        public void DrawViewportWires(GH_PreviewWireArgs args)
        {
            // GH_Plane.DrawPlane(args.Pipeline, Value.GlobalTargetPlane);
        }
        #endregion
    }
}

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
using RobotComponents.BaseClasses.Actions;
using RobotComponentsGoos.Definitions;

namespace RobotComponentsGoos.Actions
{
    /// <summary>
    /// Movement Goo wrapper class, makes sure the Movement can be used in Grasshopper.
    /// </summary>
    public class GH_Movement : GH_GeometricGoo<Movement>, IGH_PreviewData
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_Movement()
        {
            this.Value = new Movement();
        }

        /// <summary>
        /// Data constructor, m_value will be set to internal_data.
        /// </summary>
        /// <param name="movement"> Movement Value to store inside this Goo instance. </param>
        public GH_Movement(Movement movement)
        {
            if (movement == null)
                movement = new Movement();
            this.Value = movement;
        }

        /// <summary>
        /// Data constructor, m_value will be set to internal_data.
        /// </summary>
        /// <param name="movementGoo"> MovementGoo to store inside this Goo instance. </param>
        public GH_Movement(GH_Movement movementGoo)
        {
            if (movementGoo == null)
                movementGoo = new GH_Movement();
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
        public GH_Movement DuplicateMovementGoo()
        {
            return new GH_Movement(Value == null ? new Movement() : Value.Duplicate());
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
                return "Invalid Movement instance: Did you define a target?"; //Todo: beef this up to be more informative.
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null)
                return "Null Movement";

            else
                return Value.ToString();
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
        /// <param name="target"> Pointer to target of cast. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool CastTo<Q>(out Q target)
        {
            //Cast to Movement.
            if (typeof(Q).IsAssignableFrom(typeof(Movement)))
            {
                if (Value == null)
                    target = default(Q);
                else
                    target = (Q)(object)Value;
                return true;
            }

            //Cast to Movement.
            if (typeof(Q).IsAssignableFrom(typeof(GH_Movement)))
            {
                if (Value == null)
                    target = default(Q);
                else
                    target = (Q)(object)new GH_Movement(Value);
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

            //Cast to Target
            if (typeof(Q).IsAssignableFrom(typeof(GH_Target)))
            {
                if (Value == null)
                    target = default(Q);
                else if (Value.Target == null)
                    target = default(Q);
                else
                    target = (Q)(object)new GH_Target(Value.Target);
                return true;
            }

            //Cast to Plane
            if (typeof(Q).IsAssignableFrom(typeof(GH_Plane)))
            {
                if (Value == null)
                    target = default(Q);
                else if (Value.GlobalTargetPlane == null)
                    target = default(Q);
                else
                    target = (Q)(object)new GH_Plane(Value.GlobalTargetPlane);
                return true;
            }

            //Cast to Point
            if (typeof(Q).IsAssignableFrom(typeof(GH_Point)))
            {
                if (Value == null)
                    target = default(Q);
                else if (Value.GlobalTargetPlane == null)
                    target = default(Q);
                else
                    target = (Q)(object)new GH_Point(Value.GlobalTargetPlane.Origin);
                return true;
            }

            //Cast to SpeedData
            if (typeof(Q).IsAssignableFrom(typeof(GH_SpeedData)))
            {
                if (Value == null)
                    target = default(Q);
                else if (Value.SpeedData == null)
                    target = default(Q);
                else
                    target = (Q)(object)new GH_SpeedData(Value.SpeedData);
                return true;
            }

            //Cast to ZoneData
            if (typeof(Q).IsAssignableFrom(typeof(GH_ZoneData)))
            {
                if (Value == null)
                    target = default(Q);
                else if (Value.ZoneData == null)
                    target = default(Q);
                else
                    target = (Q)(object)new GH_ZoneData(Value.ZoneData);
                return true;
            }

            //Cast to RobotTool
            if (typeof(Q).IsAssignableFrom(typeof(GH_RobotTool)))
            {
                if (Value == null)
                    target = default(Q);
                else if (Value.RobotTool == null)
                    target = default(Q);
                else
                    target = (Q)(object)new GH_RobotTool(Value.RobotTool);
                return true;
            }

            //Cast to WorkObject
            if (typeof(Q).IsAssignableFrom(typeof(GH_WorkObject)))
            {
                if (Value == null)
                    target = default(Q);
                else if (Value.WorkObject == null)
                    target = default(Q);
                else
                    target = (Q)(object)new GH_WorkObject(Value.WorkObject);
                return true;
            }

            //Cast to DigitalOutput
            if (typeof(Q).IsAssignableFrom(typeof(GH_DigitalOutput)))
            {
                if (Value == null)
                    target = default(Q);
                else if (Value.DigitalOutput == null)
                    target = default(Q);
                else
                    target = (Q)(object)new GH_DigitalOutput(Value.DigitalOutput);
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

            //Cast from Movement
            if (typeof(Movement).IsAssignableFrom(source.GetType()))
            {
                Value = source as Movement;
                return true;
            }

            //Cast from TargetGoo
            if (typeof(GH_Target).IsAssignableFrom(source.GetType()))
            {
                GH_Target targetGoo = source as GH_Target;
                Value = new Movement(targetGoo.Value);
                return true;
            }

            //Cast from Target
            if (typeof(Target).IsAssignableFrom(source.GetType()))
            {
                Target target = (Target)source;
                Value = new Movement(target);
                return true;
            }

            //Cast from Action
            if (typeof(Action).IsAssignableFrom(source.GetType()))
            {
                if (source is Movement action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from ActionGoo
            if (typeof(GH_Action).IsAssignableFrom(source.GetType()))
            {
                GH_Action actionGoo = source as GH_Action;
                if (actionGoo.Value is Movement action)
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

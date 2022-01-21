// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System.Collections.Generic;
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
    /// Action Goo wrapper class, makes sure the Action Group class can be used in Grasshopper.
    /// </summary>
    public class GH_ActionGroup : GH_GeometricGoo<ActionGroup>, IGH_PreviewData, GH_ISerializable
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_ActionGroup()
        {
            this.Value = null;
        }

        /// <summary>
        /// Data constructor: Creates an Action Goo instance from an Action instance.
        /// </summary>
        /// <param name="group"> Action Group Value to store inside this Goo instance. </param>
        public GH_ActionGroup(ActionGroup group)
        {
            this.Value = group;
        }

        /// <summary>
        /// Data constructor: Creates a Action Group Goo instance from another Action Group Goo instance.
        /// This creates a shallow copy of the passed Action Goo instance. 
        /// </summary>
        /// <param name="groupGoo"> Action Group Goo instance to copy. </param>
        public GH_ActionGroup(GH_ActionGroup groupGoo)
        {
            if (groupGoo == null)
            {
                groupGoo = new GH_ActionGroup();
            }

            this.Value = groupGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this Goo instance. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the Action Group Goo. </returns>
        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return DuplicateActionGroupGoo();
        }

        /// <summary>
        /// Make a complete duplicate of this Goo instance. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the Action Group Goo. </returns>
        public GH_ActionGroup DuplicateActionGroupGoo()
        {
            if (Value == null) { return null; }
            else if (Value is ActionGroup) { return new GH_ActionGroup(Value.Duplicate()); }
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
        /// If the instance is valid, then this property should return Nothing or string.Empty.
        /// </summary>
        public override string IsValidWhyNot
        {
            get
            {
                if (Value == null) { return "No internal Action Group instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid Action Group instance";
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null) { return "Null Action Group"; }
            else { return Value.ToString(); }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return "Action Group"; }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return "Defines an Action Group."; }
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
            //Cast to Action Group Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_ActionGroup)))
            {
                if (Value == null) { target = (Q)(object)new GH_ActionGroup(); }
                else { target = (Q)(object)new GH_ActionGroup(Value); }
                return true;
            }

            //Cast to Action Group
            if (typeof(Q).IsAssignableFrom(typeof(ActionGroup)))
            {
                if (Value == null) { target = (Q)(object)null; }
                else { target = (Q)(object)Value; }
                return true;
            }

            //Cast to Action Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_Action)))
            {
                if (Value == null) { target = (Q)(object)new GH_Action(); }
                else { target = (Q)(object)new GH_Action(Value); }
                return true;
            }

            //Cast to Action
            if (typeof(Q).IsAssignableFrom(typeof(Action)))
            {
                if (Value == null) { target = (Q)(object)null; }
                else { target = (Q)(object)Value; }
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

            //Cast from Action Group
            if (typeof(ActionGroup).IsAssignableFrom(source.GetType()))
            {
                Value = source as ActionGroup;
                return true;
            }

            //Cast from Action Group Goo
            if (typeof(GH_ActionGroup).IsAssignableFrom(source.GetType()))
            {
                GH_ActionGroup groupGoo = source as GH_ActionGroup;
                Value = groupGoo.Value;
                return true;
            }

            //Cast from Action
            if (typeof(Action).IsAssignableFrom(source.GetType()))
            {
                if (source is ActionGroup action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Action Goo
            if (typeof(GH_Action).IsAssignableFrom(source.GetType()))
            {
                GH_Action actionGoo = source as GH_Action;
                if (actionGoo.Value is ActionGroup action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Geometry Group Goo
            if (typeof(GH_GeometryGroup).IsAssignableFrom(source.GetType()))
            {
                GH_GeometryGroup groupGoo = source as GH_GeometryGroup;
                List<IGH_GeometricGoo> goos = groupGoo.Objects;
                List<Action> actions = new List<Action>() { };
                bool nonAction = false;

                for (int i = 0; i < goos.Count; i++)
                {
                    if (goos[i] is GH_GeometricGoo<Action> geometricGoo)
                    {
                        actions.Add(geometricGoo.Value);
                    }
                    else if (goos[i] is GH_Goo<Action> goo)
                    {
                        actions.Add(goo.Value);
                    }
                    else
                    {
                        nonAction = true;
                        break;
                    }
                }

                if (nonAction == false)
                {
                    Value = new ActionGroup(actions);
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
            for (int i = 0; i < this.Value.Actions.Count; i++)
            {
                if (this.Value.Actions[i] is Movement movement)
                {
                    Plane plane = movement.GetGlobalTargetPlane();

                    if (plane != Plane.Unset)
                    {
                        args.Pipeline.DrawDirectionArrow(plane.Origin, plane.ZAxis, System.Drawing.Color.Blue);
                        args.Pipeline.DrawDirectionArrow(plane.Origin, plane.XAxis, System.Drawing.Color.Red);
                        args.Pipeline.DrawDirectionArrow(plane.Origin, plane.YAxis, System.Drawing.Color.Green);
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
        private const string IoKey = "ActionGroup";

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
            this.Value = (ActionGroup)HelperMethods.ByteArrayToObject(array);

            return true;
        }
        #endregion
    }
}

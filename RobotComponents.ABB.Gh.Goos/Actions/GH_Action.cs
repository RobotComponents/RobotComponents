﻿// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

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
using RobotComponents.ABB.Actions;
using RobotComponents.ABB.Actions.Instructions;
using RobotComponents.Utils;

namespace RobotComponents.ABB.Gh.Goos.Actions
{
    /// <summary>
    /// Action Goo wrapper class, makes sure the Action class can be used in Grasshopper.
    /// </summary>
    public class GH_Action : GH_GeometricGoo<IAction>, IGH_PreviewData, GH_ISerializable
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_Action()
        {
            this.Value = null;
        }

        /// <summary>
        /// Data constructor: Creates an Action Goo instance from an Action instance.
        /// </summary>
        /// <param name="action"> Action Value to store inside this Goo instance. </param>
        public GH_Action(IAction action)
        {
            this.Value = action;
        }

        /// <summary>
        /// Data constructor: Creates a Action Goo instance from another Action Goo instance.
        /// This creates a shallow copy of the passed Action Goo instance. 
        /// </summary>
        /// <param name="actionGoo"> Action Goo instance to copy. </param>
        public GH_Action(GH_Action actionGoo)
        {
            if (actionGoo == null)
            {
                actionGoo = new GH_Action();
            }

            this.Value = actionGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this Goo instance. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the Action Goo. </returns>
        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return DuplicateActionGoo();
        }

        /// <summary>
        /// Make a complete duplicate of this Goo instance. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the Action Goo. </returns>
        public GH_Action DuplicateActionGoo()
        {
            if (Value == null) { return null; }
            else if (Value is IAction) { return new GH_Action(Value.DuplicateAction()); }
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
                if (Value == null) { return "No internal Action instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid Action instance";
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null) { return "Null Action"; }
            else { return Value.ToString(); }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return "Action"; }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return "Defines an Action."; }
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
            //Cast to Action Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_Action)))
            {
                if (Value == null) { target = (Q)(object)new GH_Action(); }
                else { target = (Q)(object)new GH_Action(Value); }
                return true;
            }
            //Cast to Action
            if (typeof(Q).IsAssignableFrom(typeof(IAction)))
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

            //Cast from Action
            if (typeof(IAction).IsAssignableFrom(source.GetType()))
            {
                Value = source as IAction;
                return true;
            }

            //Cast from Action Goo
            if (typeof(GH_Action).IsAssignableFrom(source.GetType()))
            {
                GH_Action actionGoo = source as GH_Action;
                Value = actionGoo.Value;
                return true;
            }

            //Cast from Geometry Group Goo
            if (typeof(GH_GeometryGroup).IsAssignableFrom(source.GetType()))
            {
                GH_GeometryGroup groupGoo = source as GH_GeometryGroup;
                List<IGH_GeometricGoo> goos = groupGoo.Objects;
                List<IAction> actions = new List<IAction>() { };
                bool nonAction = false;

                for (int i = 0; i < goos.Count; i++)
                {
                    if (goos[i] is GH_GeometricGoo<IAction> geometricGoo)
                    {
                        actions.Add(geometricGoo.Value);
                    }
                    else if (goos[i] is GH_Goo<IAction> goo)
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
            if (this.Value is Movement movement1)
            {
                Plane plane = movement1.GetGlobalTargetPlane();

                if (plane != Plane.Unset)
                {
                    args.Pipeline.DrawDirectionArrow(plane.Origin, plane.ZAxis, System.Drawing.Color.Blue);
                    args.Pipeline.DrawDirectionArrow(plane.Origin, plane.XAxis, System.Drawing.Color.Red);
                    args.Pipeline.DrawDirectionArrow(plane.Origin, plane.YAxis, System.Drawing.Color.Green);
                }
            }

            else if (this.Value is ActionGroup group)
            {
                for (int i = 0; i < group.Actions.Count; i++)
                {
                    if (group.Actions[i] is Movement movement2)
                    {
                        Plane plane = movement2.GetGlobalTargetPlane();

                        if (plane != Plane.Unset)
                        {
                            args.Pipeline.DrawDirectionArrow(plane.Origin, plane.ZAxis, System.Drawing.Color.Blue);
                            args.Pipeline.DrawDirectionArrow(plane.Origin, plane.XAxis, System.Drawing.Color.Red);
                            args.Pipeline.DrawDirectionArrow(plane.Origin, plane.YAxis, System.Drawing.Color.Green);
                        }
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
        private const string IoKey = "Action";

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
            this.Value = (IAction)Serialization.ByteArrayToObject(array);

            return true;
        }
        #endregion
    }
}

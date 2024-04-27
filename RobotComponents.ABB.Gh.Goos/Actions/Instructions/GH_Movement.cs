﻿// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
// Grasshopper Libs
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GH_IO;
using GH_IO.Serialization;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.ABB.Actions.Instructions;
using RobotComponents.ABB.Actions.Declarations;
using RobotComponents.ABB.Definitions;
using RobotComponents.Utils;
using RobotComponents.ABB.Gh.Goos.Definitions;
using RobotComponents.ABB.Gh.Goos.Actions.Declarations;

namespace RobotComponents.ABB.Gh.Goos.Actions.Instructions
{
    /// <summary>
    /// Movement Goo wrapper class, makes sure the Movement class can be used in Grasshopper.
    /// </summary>
    public class GH_Movement : GH_GeometricGoo<Movement>, IGH_PreviewData, GH_ISerializable
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_Movement()
        {
            this.Value = null;
        }

        /// <summary>
        /// Data constructor: Creates a Movement Goo instance from a Movement instance.
        /// </summary>
        /// <param name="movement"> Movement Value to store inside this Goo instance. </param>
        public GH_Movement(Movement movement)
        {
            this.Value = movement;
        }

        /// <summary>
        /// Data constructor: Creates a Momvement Goo instance from another Movement Goo instance.
        /// This creates a shallow copy of the passed Momvement Goo instance. 
        /// </summary>
        /// <param name="movementGoo"> Movement Goo instance to copy. </param>
        public GH_Movement(GH_Movement movementGoo)
        {
            if (movementGoo == null)
            {
                movementGoo = new GH_Movement();
            }

            this.Value = movementGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this Goo instance. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the Movement Goo. </returns>
        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return DuplicateMovementGoo();
        }

        /// <summary>
        /// Make a complete duplicate of this Goo instance. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the Movement Goo. </returns>
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
        /// Gets a string describing the state of "invalidness". 
        /// If the instance is valid, then this property should return Nothing or string.Empty.
        /// </summary>
        public override string IsValidWhyNot
        {
            get
            {
                if (Value == null) { return "No internal Movement instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid Movement instance: Did you define a Target?";
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null) { return "Null Movement"; }
            else { return Value.ToString(); }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return "Movement"; }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return "Defines a Movement"; }
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
            return BoundingBox.Empty;
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
            //Cast to Movement Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_Movement)))
            {
                if (Value == null) { target = (Q)(object)new GH_Movement(); }
                else { target = (Q)(object)new GH_Movement(Value); }
                return true;
            }

            //Cast to Movement
            if (typeof(Q).IsAssignableFrom(typeof(Movement)))
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
            if (typeof(Q).IsAssignableFrom(typeof(RobotComponents.ABB.Actions.IAction)))
            {
                if (Value == null) { target = (Q)(object)null; }
                else { target = (Q)(object)Value; }
                return true;
            }

            //Cast to Instruction Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_Instruction)))
            {
                if (Value == null) { target = (Q)(object)new GH_Instruction(); }
                else { target = (Q)(object)new GH_Instruction(Value); }
                return true;
            }

            //Cast to Instruction
            if (typeof(Q).IsAssignableFrom(typeof(IInstruction)))
            {
                if (Value == null) { target = (Q)(object)null; }
                else { target = (Q)(object)Value; }
                return true;
            }

            //Cast to Target Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_Target)))
            {
                if (Value == null) { target = (Q)(object)new GH_Target(); }
                else { target = (Q)(object)new GH_Target(Value.Target); }
                return true;
            }

            //Cast to Target
            if (typeof(Q).IsAssignableFrom(typeof(ITarget)))
            {
                if (Value == null) { target = (Q)(object)null; }
                else { target = (Q)Value.Target; }
                return true;
            }

            //Cast to Robot Target Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_RobotTarget)))
            {
                if (Value == null) { target = (Q)(object)new GH_RobotTarget(); }
                else if (Value.Target is RobotTarget) { target = (Q)(object)new GH_RobotTarget(Value.Target as RobotTarget); }
                else if (Value.Target is JointTarget) { target = (Q)(object)new GH_RobotTarget(); }
                else { target = default; }
                return true;
            }

            //Cast to Robot Target
            if (typeof(Q).IsAssignableFrom(typeof(RobotTarget)))
            {
                if (Value == null) { target = (Q)(object)null; }
                else if (Value.Target is RobotTarget) { target = (Q)Value.Target; }
                else if (Value.Target is JointTarget) { target = (Q)(object)null; }
                else { target = default; }
                return true;
            }

            //Cast to Joint Target Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_JointTarget)))
            {
                if (Value == null) { target = (Q)(object)new GH_JointTarget(); }
                else if (Value.Target is JointTarget) { target = (Q)(object)new GH_JointTarget(Value.Target as JointTarget); }
                else if (Value.Target is RobotTarget) { { target = (Q)(object)new GH_JointTarget(); } }
                else { target = default; }
                return true;
            }

            //Cast to Joint Target
            if (typeof(Q).IsAssignableFrom(typeof(JointTarget)))
            {
                if (Value == null) { target = (Q)(object)null; }
                else if (Value.Target is JointTarget) { target = (Q)Value.Target; }
                else if (Value.Target is RobotTarget) { { target = (Q)(object)null; } }
                else { target = default; }
                return true;
            }

            //Cast to External Joint Position Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_ExternalJointPosition)))
            {
                if (Value == null) { target = (Q)(object)new GH_ExternalJointPosition(); }
                else { target = (Q)(object)new GH_ExternalJointPosition(Value.Target.ExternalJointPosition); }
                return true;
            }

            //Cast to External Joint Position
            if (typeof(Q).IsAssignableFrom(typeof(ExternalJointPosition)))
            {
                if (Value == null) { target = (Q)(object)null; }
                else { target = (Q)(object)Value.Target.ExternalJointPosition; }
                return true;
            }

            //Cast to Speed Data Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_SpeedData)))
            {
                if (Value == null) { target = (Q)(object)new GH_SpeedData(); }
                else { target = (Q)(object)new GH_SpeedData(Value.SpeedData); }
                return true;
            }

            //Cast to Speed Data
            if (typeof(Q).IsAssignableFrom(typeof(SpeedData)))
            {
                if (Value == null) { target = (Q)(object)null; }
                else { target = (Q)(object)Value.SpeedData; }
                return true;
            }

            //Cast to Zone Data Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_ZoneData)))
            {
                if (Value == null) { target = (Q)(object)new GH_ZoneData(); }
                else { target = (Q)(object)new GH_ZoneData(Value.ZoneData); }
                return true;
            }

            //Cast to Zone Data
            if (typeof(Q).IsAssignableFrom(typeof(ZoneData)))
            {
                if (Value == null) { target = (Q)(object)null; }
                else { target = (Q)(object)Value.ZoneData; }
                return true;
            }

            //Cast to Robot Tool Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_RobotTool)))
            {
                if (Value == null) { target = (Q)(object)new GH_RobotTool(); }
                else { target = (Q)(object)new GH_RobotTool(Value.RobotTool); }
                return true;
            }

            //Cast to Robot Tool
            if (typeof(Q).IsAssignableFrom(typeof(RobotTool)))
            {
                if (Value == null) { target = (Q)(object)null; }
                else { target = (Q)(object)Value.RobotTool; }
                return true;
            }

            //Cast to Work Object Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_WorkObject)))
            {
                if (Value == null) { target = (Q)(object)new GH_WorkObject(); }
                else { target = (Q)(object)new GH_WorkObject(Value.WorkObject); }
                return true;
            }

            //Cast to Work Object
            if (typeof(Q).IsAssignableFrom(typeof(WorkObject)))
            {
                if (Value == null) { target = (Q)(object)null; }
                else { target = (Q)(object)Value.WorkObject; }
                return true;
            }

            //Cast to Set Digital Output Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_SetDigitalOutput)))
            {
                if (Value == null) { target = (Q)(object)new GH_SetDigitalOutput(); }
                else { target = (Q)(object)new GH_SetDigitalOutput(Value.SetDigitalOutput); }
                return true;
            }

            //Cast to Set Digital Output
            if (typeof(Q).IsAssignableFrom(typeof(SetDigitalOutput)))
            {
                if (Value == null) { target = (Q)(object)null; }
                else { target = (Q)(object)Value.SetDigitalOutput; }
                return true;
            }

            //Cast to Plane
            if (typeof(Q).IsAssignableFrom(typeof(GH_Plane)))
            {
                if (Value == null) { target = default; }
                else { target = (Q)(object)new GH_Plane(Value.GetGlobalTargetPlane()); }
                return true;
            }

            //Cast to Point
            if (typeof(Q).IsAssignableFrom(typeof(GH_Point)))
            {
                if (Value == null) { target = default; }
                else { target = (Q)(object)new GH_Point(Value.GetGlobalTargetPlane().Origin); }
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

            //Cast from Movement
            if (typeof(Movement).IsAssignableFrom(source.GetType()))
            {
                Value = source as Movement;
                return true;
            }

            //Cast from Movement Goo
            if (typeof(GH_Movement).IsAssignableFrom(source.GetType()))
            {
                GH_Movement movementGoo = source as GH_Movement;
                Value = movementGoo.Value;
                return true;
            }

            //Cast from Action
            if (typeof(RobotComponents.ABB.Actions.IAction).IsAssignableFrom(source.GetType()))
            {
                if (source is Movement action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Action Goo
            if (typeof(GH_Action).IsAssignableFrom(source.GetType()))
            {
                GH_Action actionGoo = source as GH_Action;
                if (actionGoo.Value is Movement action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Target
            if (typeof(ITarget).IsAssignableFrom(source.GetType()))
            {
                Value = new Movement(source as ITarget);
                return true;
            }

            //Cast from Target Goo
            if (typeof(GH_Target).IsAssignableFrom(source.GetType()))
            {
                GH_Target targetGoo = source as GH_Target;
                Value = new Movement(targetGoo.Value);
                return true;
            }

            //Cast from Robot Target
            if (typeof(RobotTarget).IsAssignableFrom(source.GetType()))
            {
                RobotTarget target = (RobotTarget)source;
                Value = new Movement(target);
                return true;
            }

            //Cast from Robot Target Goo
            if (typeof(GH_RobotTarget).IsAssignableFrom(source.GetType()))
            {
                GH_RobotTarget targetGoo = (GH_RobotTarget)source;
                Value = new Movement(targetGoo.Value);
                return true;
            }

            //Cast from Joint Target
            if (typeof(JointTarget).IsAssignableFrom(source.GetType()))
            {
                JointTarget target = (JointTarget)source;
                Value = new Movement(target);
                return true;
            }

            //Cast from Joint Target Goo
            if (typeof(GH_JointTarget).IsAssignableFrom(source.GetType()))
            {
                GH_JointTarget targetGoo = (GH_JointTarget)source;
                Value = new Movement(targetGoo.Value);
                return true;
            }

            //Cast from Instruction
            if (typeof(IInstruction).IsAssignableFrom(source.GetType()))
            {
                if (source is Movement instruction)
                {
                    Value = instruction;
                    return true;
                }
            }

            //Cast from Instruction Goo
            if (typeof(GH_Instruction).IsAssignableFrom(source.GetType()))
            {
                GH_Instruction instructionGoo = source as GH_Instruction;
                if (instructionGoo.Value is Movement instruction)
                {
                    Value = instruction;
                    return true;
                }
            }

            //Cast from Plane
            if (typeof(Plane).IsAssignableFrom(source.GetType()))
            {
                Plane plane = (Plane)source;
                Value = new Movement(plane);
                return true;
            }

            //Cast from Plane Goo
            if (typeof(GH_Plane).IsAssignableFrom(source.GetType()))
            {
                GH_Plane planeGoo = (GH_Plane)source;
                Value = new Movement(planeGoo.Value);
                return true;
            }

            //Cast from Robot Joint Position
            if (typeof(RobotJointPosition).IsAssignableFrom(source.GetType()))
            {
                RobotJointPosition jointPosition = (RobotJointPosition)source;
                Value = new Movement(new JointTarget(jointPosition));
                return true;
            }

            //Cast from Robot Joint Position Goo
            if (typeof(GH_RobotJointPosition).IsAssignableFrom(source.GetType()))
            {
                GH_RobotJointPosition jointPositionGoo = (GH_RobotJointPosition)source;
                Value = new Movement(new JointTarget(jointPositionGoo.Value));
                return true;
            }

            //Cast from Text
            if (typeof(GH_String).IsAssignableFrom(source.GetType()))
            {
                string text = (source as GH_String).Value;

                try
                {
                    Value = new Movement(JointTarget.Parse(text));
                    return true;
                }
                catch
                {
                    try
                    {
                        Value = new Movement(RobotTarget.Parse(text));
                        return true;
                    }
                    catch
                    {
                        string clean = text.Replace("[", "");
                        clean = clean.Replace("]", "");
                        clean = clean.Replace("(", "");
                        clean = clean.Replace(")", "");
                        clean = clean.Replace("{", "");
                        clean = clean.Replace("}", "");
                        clean = clean.Replace(" ", "");

                        string[] values = clean.Split(',');

                        if (values.Length == 6)
                        {
                            try
                            {
                                RobotJointPosition robotJointPosition = new RobotJointPosition();

                                for (int i = 0; i < values.Length; i++)
                                {
                                    robotJointPosition[i] = Convert.ToDouble(values[i]);
                                }

                                Value = new Movement(new JointTarget(robotJointPosition));
                                return true;
                            }

                            catch
                            {
                                return false;
                            }
                        }
                    }
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
            Plane plane = Value.GetGlobalTargetPlane();

            if (plane != Plane.Unset)
            {
                args.Pipeline.DrawDirectionArrow(plane.Origin, plane.ZAxis, System.Drawing.Color.Blue);
                args.Pipeline.DrawDirectionArrow(plane.Origin, plane.XAxis, System.Drawing.Color.Red);
                args.Pipeline.DrawDirectionArrow(plane.Origin, plane.YAxis, System.Drawing.Color.Green);
            }
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

        #region (de)serialisation
        /// <summary>
        /// IO key for (de)serialisation of the value inside this Goo.
        /// </summary>
        private const string IoKey = "Movement";

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
            this.Value = (Movement)Serialization.ByteArrayToObject(array);

            return true;
        }
        #endregion
    }
}

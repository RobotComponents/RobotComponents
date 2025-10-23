// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components
// Project: https://github.com/RobotComponents/RobotComponents
//
// Copyright (c) 2020 EDEK Uni Kassel
// Copyright (c) 2020-2025 Arjen Deetman
//
// Authors:
//   - Arjen Deetman (2020-2025)
//
// For license details, see the LICENSE file in the project root.

// System Libs
using System;
// Grasshopper Libs
using Grasshopper.Kernel.Types;
using GH_IO;
using GH_IO.Serialization;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.ABB.Actions.Declarations;

namespace RobotComponents.ABB.Gh.Goos.Actions.Declarations
{
    /// <summary>
    /// Target Goo wrapper class, makes sure the Target interface can be used in Grasshopper.
    /// </summary>
    public class GH_Target : GH_Goo<ITarget>, GH_ISerializable
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_Target()
        {
            this.Value = null;
        }

        /// <summary>
        /// Data constructor: Create a Target Goo instance from a Target instance.
        /// </summary>
        /// <param name="target"> Target Value to store inside this Goo instance. </param>
        public GH_Target(ITarget target)
        {
            this.Value = target;
        }

        /// <summary>
        /// Data constructor: Creates a Target Goo instance from another Target Goo instance.
        /// This creates a shallow copy of the passed Target Goo instance. 
        /// </summary>
        /// <param name="targetGoo"> Target Goo to copy. </param>
        public GH_Target(GH_Target targetGoo)
        {
            if (targetGoo == null)
            {
                targetGoo = new GH_Target();
            }

            this.Value = targetGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this Goo instance. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the Target Goo. </returns>
        public override IGH_Goo Duplicate()
        {
            return new GH_Target(Value?.DuplicateTarget());
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
                if (Value == null) { return "No internal Target instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid Target instance: Did you define a name?";
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null) { return "Null Target"; }
            else { return Value.ToString(); }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return "Target"; }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return "Defines a Target"; }
        }
        #endregion

        #region casting methods
        /// <summary>
        /// Attempt a cast to type Q.
        /// </summary>
        /// <typeparam name="Q"> Type to cast to.  </typeparam>
        /// <param name="target"> Pointer to target of cast. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool CastTo<Q>(ref Q target)
        {
            //Cast to Target Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_Target)))
            {
                if (Value == null) { target = (Q)(object)new GH_Target(); }
                else { target = (Q)(object)new GH_Target(Value); }
                return true;
            }

            //Cast to Target
            if (typeof(Q).IsAssignableFrom(typeof(ITarget)))
            {
                if (Value == null) { target = (Q)(object)null; }
                else { target = (Q)Value; }
                return true;
            }

            //Cast to Joint Target Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_JointTarget)))
            {
                if (Value == null) { target = (Q)(object)new GH_JointTarget(); }
                else if (Value is JointTarget) { target = (Q)(object)new GH_JointTarget(Value as JointTarget); }
                else if (Value is RobotTarget) { { target = (Q)(object)new GH_JointTarget(); } }
                else { target = default; }
                return true;
            }

            //Cast to Joint Target
            if (typeof(Q).IsAssignableFrom(typeof(JointTarget)))
            {
                if (Value == null) { target = (Q)(object)null; }
                else if (Value is JointTarget) { target = (Q)Value; }
                else if (Value is RobotTarget) { { target = (Q)(object)null; } }
                else { target = default; }
                return true;
            }

            //Cast to Robot Target Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_RobotTarget)))
            {
                if (Value == null) { target = (Q)(object)new GH_RobotTarget(); ; }
                else if (Value is RobotTarget) { target = (Q)(object)new GH_RobotTarget(Value as RobotTarget); }
                else if (Value is JointTarget) { target = (Q)(object)new GH_RobotTarget(); }
                else { target = default; }
                return true;
            }

            //Cast to Robot Target Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_RobotTarget)))
            {
                if (Value == null) { target = (Q)(object)null; ; }
                else if (Value is RobotTarget) { target = (Q)Value; }
                else if (Value is JointTarget) { target = (Q)(object)null; }
                else { target = default; }
                return true;
            }

            //Cast to Action Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_Action)))
            {
                if (Value == null) { target = (Q)(object)new GH_Action(); }
                else { target = (Q)(object)new GH_Action(Value as RobotComponents.ABB.Actions.IAction); }
                return true;
            }

            //Cast to Action
            if (typeof(Q).IsAssignableFrom(typeof(RobotComponents.ABB.Actions.IAction)))
            {
                if (Value == null) { target = (Q)(object)null; }
                else { target = (Q)Value; }
                return true;
            }

            //Cast to Declaration Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_Declaration)))
            {
                if (Value == null) { target = (Q)(object)new GH_Declaration(); }
                else { target = (Q)(object)new GH_Declaration(Value as IDeclaration); }
                return true;
            }

            //Cast to Declaration
            if (typeof(Q).IsAssignableFrom(typeof(IDeclaration)))
            {
                if (Value == null) { target = (Q)(object)null; }
                else { target = (Q)Value; }
                return true;
            }

            //Cast to External Joint Position Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_ExternalJointPosition)))
            {
                if (Value == null) { target = (Q)(object)new GH_ExternalJointPosition(); }
                else { target = (Q)(object)new GH_ExternalJointPosition(Value.ExternalJointPosition); }
                return true;
            }

            //Cast to External Joint Position Goo
            if (typeof(Q).IsAssignableFrom(typeof(ExternalJointPosition)))
            {
                if (Value == null) { target = (Q)(object)null; }
                else { target = (Q)(object)Value.ExternalJointPosition; }
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

            //Cast from Target
            if (typeof(ITarget).IsAssignableFrom(source.GetType()))
            {
                Value = source as ITarget;
                return true;
            }

            //Cast from Target Goo
            if (typeof(ITarget).IsAssignableFrom(source.GetType()))
            {
                GH_Target targetGoo = source as GH_Target;
                if (targetGoo.Value is ITarget target)
                {
                    Value = target;
                    return true;
                }
            }

            //Cast from Action
            if (typeof(RobotComponents.ABB.Actions.IAction).IsAssignableFrom(source.GetType()))
            {
                if (source is ITarget action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Action Goo
            if (typeof(GH_Action).IsAssignableFrom(source.GetType()))
            {
                GH_Action actionGoo = source as GH_Action;
                if (actionGoo.Value is ITarget action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Declaration
            if (typeof(IDeclaration).IsAssignableFrom(source.GetType()))
            {
                if (source is ITarget target)
                {
                    Value = target;
                    return true;
                }
            }

            //Cast from Declaration Goo
            if (typeof(GH_Declaration).IsAssignableFrom(source.GetType()))
            {
                GH_Declaration declarationGoo = source as GH_Declaration;
                if (declarationGoo.Value is ITarget target)
                {
                    Value = target;
                    return true;
                }
            }

            //Cast from Plane
            if (typeof(Plane).IsAssignableFrom(source.GetType()))
            {
                Plane plane = (Plane)source;
                Value = new RobotTarget(plane);
                return true;
            }

            //Cast from Plane Goo
            if (typeof(GH_Plane).IsAssignableFrom(source.GetType()))
            {
                GH_Plane planeGoo = (GH_Plane)source;
                Value = new RobotTarget(planeGoo.Value);
                return true;
            }

            //Cast from Point
            if (typeof(Point3d).IsAssignableFrom(source.GetType()))
            {
                Point3d point = (Point3d)source;
                Value = new RobotTarget(new Plane(point, new Vector3d(1, 0, 0), new Vector3d(0, 1, 0)));
                return true;
            }

            //Cast from Point Goo
            if (typeof(GH_Point).IsAssignableFrom(source.GetType()))
            {
                GH_Point pointGoo = (GH_Point)source;
                Value = new RobotTarget(new Plane(pointGoo.Value, new Vector3d(1, 0, 0), new Vector3d(0, 1, 0)));
                return true;
            }

            //Cast from Robot Joint Position
            if (typeof(RobotJointPosition).IsAssignableFrom(source.GetType()))
            {
                RobotJointPosition jointPosition = (RobotJointPosition)source;
                Value = new JointTarget(jointPosition);
                return true;
            }

            //Cast from Robot Joint Position Goo
            if (typeof(GH_RobotJointPosition).IsAssignableFrom(source.GetType()))
            {
                GH_RobotJointPosition jointPositionGoo = (GH_RobotJointPosition)source;
                Value = new JointTarget(jointPositionGoo.Value);
                return true;
            }

            //Cast from Text
            if (typeof(GH_String).IsAssignableFrom(source.GetType()))
            {
                string text = (source as GH_String).Value;

                try
                {
                    Value = JointTarget.Parse(text);
                    return true;
                }
                catch
                {
                    try
                    {
                        Value = RobotTarget.Parse(text);
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

                                Value = new JointTarget(robotJointPosition);
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

        #region (de)serialisation
        /// <summary>
        /// IO key for (de)serialisation of the value inside this Goo.
        /// </summary>
        private const string IoKey = "Target";

        /// <summary>
        /// This method is called whenever the instance is required to serialize itself.
        /// </summary>
        /// <param name="writer"> Writer object to serialize with. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Write(GH_IWriter writer)
        {
            if (this.Value != null)
            {
                byte[] array = RobotComponents.Utils.Serialization.ObjectToByteArray(this.Value);
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
            this.Value = (ITarget)RobotComponents.Utils.Serialization.ByteArrayToObject(array);

            return true;
        }
        #endregion
    }
}

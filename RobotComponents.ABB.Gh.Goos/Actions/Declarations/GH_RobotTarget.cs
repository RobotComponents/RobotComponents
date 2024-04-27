﻿// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// Grasshopper Libs
using Grasshopper.Kernel.Types;
using GH_IO;
using GH_IO.Serialization;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.ABB.Actions;
using RobotComponents.ABB.Actions.Declarations;
using RobotComponents.Utils;

namespace RobotComponents.ABB.Gh.Goos.Actions.Declarations
{
    /// <summary>
    /// Robot Target Goo wrapper class, makes sure the Robot Target class can be used in Grasshopper.
    /// </summary>
    public class GH_RobotTarget : GH_Goo<RobotTarget>, GH_ISerializable
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_RobotTarget()
        {
            this.Value = null;
        }

        /// <summary>
        /// Data constructor: Create a Robot Target Goo instance from a Robot Target instance.
        /// </summary>
        /// <param name="target"> Robot Target Value to store inside this Goo instance. </param>
        public GH_RobotTarget(RobotTarget target)
        {
            this.Value = target;
        }

        /// <summary>
        /// Data constructor: Creates a Robot Target Goo instance from another Robot Target Goo instance.
        /// This creates a shallow copy of the passed Robot Target Goo instance. 
        /// </summary>
        /// <param name="targetGoo"> Robot Target Goo to copy. </param>
        public GH_RobotTarget(GH_RobotTarget targetGoo)
        {
            if (targetGoo == null)
            {
                targetGoo = new GH_RobotTarget();
            }

            this.Value = targetGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this Goo instance. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the Robot Target Goo. </returns>
        public override IGH_Goo Duplicate()
        {
            return new GH_RobotTarget(Value == null ? new RobotTarget() : Value.Duplicate());
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
                if (Value == null) { return "No internal Robot Target instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid Robot Target instance: Did you define a name and target plane?";
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null) { return "Null Robot Target"; }
            else { return Value.ToString(); }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return "Robot Target"; }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return "Defines a Robot Target"; }
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
                else { target = (Q)(object)Value; }
                return true;
            }

            //Cast to Robot Target Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_RobotTarget)))
            {
                if (Value == null) { target = (Q)(object)new GH_Target(); }
                else { target = (Q)(object)new GH_Target(Value); }
                return true;
            }

            //Cast to Robot Target
            if (typeof(Q).IsAssignableFrom(typeof(RobotTarget)))
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
            if (typeof(Q).IsAssignableFrom(typeof(IAction)))
            {
                if (Value == null) { target = (Q)(object)null; }
                else { target = (Q)(object)Value; }
                return true;
            }

            //Cast to Declaration Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_Declaration)))
            {
                if (Value == null) { target = (Q)(object)new GH_Declaration(); }
                else { target = (Q)(object)new GH_Declaration(Value); }
                return true;
            }

            //Cast to Declaration
            if (typeof(Q).IsAssignableFrom(typeof(IDeclaration)))
            {
                if (Value == null) { target = (Q)(object)null; }
                else { target = (Q)(object)Value; }
                return true;
            }

            //Cast to Configuration Data Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_ConfigurationData)))
            {
                if (Value == null) { target = (Q)(object)new GH_ConfigurationData(); }
                else { target = (Q)(object)new GH_ConfigurationData(Value.ConfigurationData); }
                return true;
            }

            //Cast to Configuration Data
            if (typeof(Q).IsAssignableFrom(typeof(ConfigurationData)))
            {
                if (Value == null) { target = (Q)(object)null; }
                else { target = (Q)(object)Value.ConfigurationData; }
                return true;
            }

            //Cast to External Joint Position Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_ExternalJointPosition)))
            {
                if (Value == null) { target = (Q)(object)new GH_ExternalJointPosition(); }
                else { target = (Q)(object)new GH_ExternalJointPosition(Value.ExternalJointPosition); }
                return true;
            }

            //Cast to External Joint Position
            if (typeof(Q).IsAssignableFrom(typeof(ExternalJointPosition)))
            {
                if (Value == null) { target = (Q)(object)null; }
                else { target = (Q)(object)Value.ExternalJointPosition; }
                return true;
            }

            //Cast to Plane
            if (typeof(Q).IsAssignableFrom(typeof(GH_Plane)))
            {
                if (Value == null) { target = default; }
                else { target = (Q)(object)new GH_Plane(Value.Plane); }
                return true;
            }

            //Cast to Point
            if (typeof(Q).IsAssignableFrom(typeof(GH_Point)))
            {
                if (Value == null) { target = default; }
                else { target = (Q)(object)new GH_Point(Value.Plane.Origin); }
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
                if (source is RobotTarget target)
                {
                    Value = target;
                    return true;
                }
            }

            //Cast from Target Goo
            if (typeof(GH_Target).IsAssignableFrom(source.GetType()))
            {
                GH_Target targetGoo = source as GH_Target;
                if (targetGoo.Value is RobotTarget target)
                {
                    Value = target;
                    return true;
                }
            }

            //Cast from Action
            if (typeof(IAction).IsAssignableFrom(source.GetType()))
            {
                if (source is RobotTarget action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Action Goo
            if (typeof(GH_Action).IsAssignableFrom(source.GetType()))
            {
                GH_Action actionGoo = source as GH_Action;
                if (actionGoo.Value is RobotTarget action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Robot Target
            if (typeof(RobotTarget).IsAssignableFrom(source.GetType()))
            {
                Value = source as RobotTarget;
                return true;
            }

            //Cast from Robot Target Goo
            if (typeof(GH_RobotTarget).IsAssignableFrom(source.GetType()))
            {
                GH_RobotTarget targetGoo = source as GH_RobotTarget;
                Value = targetGoo.Value as RobotTarget;
                return true;
            }

            //Cast from Declaration
            if (typeof(IDeclaration).IsAssignableFrom(source.GetType()))
            {
                if (source is RobotTarget target)
                {
                    Value = target;
                    return true;
                }
            }

            //Cast from Declaration Goo
            if (typeof(GH_Declaration).IsAssignableFrom(source.GetType()))
            {
                GH_Declaration declarationGoo = source as GH_Declaration;
                if (declarationGoo.Value is RobotTarget target)
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

            //Cast from Text
            if (typeof(GH_String).IsAssignableFrom(source.GetType()))
            {
                string text = (source as GH_String).Value;

                try
                {
                    Value = RobotTarget.Parse(text);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }
        #endregion

        #region (de)serialisation
        /// <summary>
        /// IO key for (de)serialisation of the value inside this Goo.
        /// </summary>
        private const string IoKey = "Robot Target";

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
            this.Value = (RobotTarget)Serialization.ByteArrayToObject(array);

            return true;
        }
        #endregion
    }
}

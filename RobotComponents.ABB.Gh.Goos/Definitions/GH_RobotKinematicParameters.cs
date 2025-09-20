// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components
// Project: https://github.com/RobotComponents/RobotComponents
//
// Copyright (c) 2025 Arjen Deetman
//
// Authors:
//   - Arjen Deetman (2025)
//
// For license details, see the LICENSE file in the project root.

// Grasshopper Libs
using Grasshopper.Kernel.Types;
using GH_IO;
using GH_IO.Serialization;
// RobotComponents Libs
using RobotComponents.ABB.Definitions;

namespace RobotComponents.ABB.Gh.Goos.Definitions
{
    /// <summary>
    /// Robot Kinematic Parameters Goo wrapper class, makes sure the Robot Kinematic Parameters class can be used in Grasshopper.
    /// </summary>
    public class GH_RobotKinematicParameters : GH_Goo<RobotKinematicParameters>, GH_ISerializable
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_RobotKinematicParameters()
        {
            this.Value = null;
        }

        /// <summary>
        /// Data constructor: Create a Robot Kinematic Parameters instance from a Robot Kinematic Parameters.
        /// </summary>
        /// <param name="RobotKinematicParameters"> Robot Kinematic Parameters Value to store inside this Goo instance. </param>
        public GH_RobotKinematicParameters(RobotKinematicParameters RobotKinematicParameters)
        {
            this.Value = RobotKinematicParameters;
        }

        /// <summary>
        /// Data constructor: Creates a Robot Kinematic Parameters Goo instance from another Robot Kinematic Parameters Goo instance.
        /// This creates a shallow copy of the passed Robot Kinematic Parameters Goo instance. 
        /// </summary>
        /// <param name="RobotKinematicParametersGoo"> Robot Kinematic Parameters Goo instance to copy. </param>
        public GH_RobotKinematicParameters(GH_RobotKinematicParameters RobotKinematicParametersGoo)
        {
            if (RobotKinematicParametersGoo == null)
            {
                RobotKinematicParametersGoo = new GH_RobotKinematicParameters();
            }

            this.Value = RobotKinematicParametersGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this Goo instance. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the Speed Data Goo. </returns>
        public override IGH_Goo Duplicate()
        {
            return new GH_RobotKinematicParameters(Value == null ? new RobotKinematicParameters() : Value.Duplicate());
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
                if (Value == null) { return "No internal Robot Kinematic Parameters instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid Robot Kinematic Parameters instance: Did you define the Robot Kinematic Parameters properties?";
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null) { return "Null Robot Kinematic Parameters"; }
            else { return Value.ToString(); }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return "Robot Kinematic Parameters"; }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return "Defines a single Robot Kinematic Parameters"; }
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
            //Cast to Robot Kinematic Parameters Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_RobotKinematicParameters)))
            {
                if (Value == null) { target = (Q)(object)new GH_RobotKinematicParameters(); }
                else { target = (Q)(object)new GH_RobotKinematicParameters(Value); }
                return true;
            }

            //Cast to Robot Kinematic Parameters
            if (typeof(Q).IsAssignableFrom(typeof(RobotKinematicParameters)))
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

            //Cast from Robot Kinematic Parameters Goo
            if (typeof(GH_RobotKinematicParameters).IsAssignableFrom(source.GetType()))
            {
                GH_RobotKinematicParameters RobotKinematicParametersGoo = source as GH_RobotKinematicParameters;
                Value = RobotKinematicParametersGoo.Value;
                return true;
            }

            //Cast from Robot Kinematic Parameters
            if (typeof(RobotKinematicParameters).IsAssignableFrom(source.GetType()))
            {
                Value = source as RobotKinematicParameters;
                return true;
            }

            return false;
        }
        #endregion

        #region (de)serialisation
        /// <summary>
        /// IO key for (de)serialisation of the value inside this Goo.
        /// </summary>
        private const string IoKey = "Robot Kinematic Parameters";

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
            this.Value = (RobotKinematicParameters)RobotComponents.Utils.Serialization.ByteArrayToObject(array);

            return true;
        }
        #endregion
    }
}

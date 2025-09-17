// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components
// Project: https://github.com/RobotComponents/RobotComponents
//
// Copyright (c) 2023-2025 Arjen Deetman
//
// Authors:
//   - Arjen Deetman (2023-2025)
//
// For license details, see the LICENSE file in the project root.

// Grasshopper Libs
using Grasshopper.Kernel.Types;
using GH_IO;
using GH_IO.Serialization;
// RobotComponents Libs
using RobotComponents.ABB.Actions.Declarations;
using RobotComponents.ABB.Definitions;
using RobotComponents.ABB.Gh.Goos.Actions.Declarations;

namespace RobotComponents.ABB.Gh.Goos.Definitions
{
    /// <summary>
    /// Load Data Goo wrapper class, makes sure the Load Data class can be used in Grasshopper.
    /// </summary>
    public class GH_LoadData : GH_Goo<LoadData>, GH_ISerializable
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_LoadData()
        {
            this.Value = null;
        }

        /// <summary>
        /// Data constructor: Create a Load Data instance from a Load Data.
        /// </summary>
        /// <param name="loadData"> Load Data Value to store inside this Goo instance. </param>
        public GH_LoadData(LoadData loadData)
        {
            this.Value = loadData;
        }

        /// <summary>
        /// Data constructor: Creates a Load Data Goo instance from another Load Data Goo instance.
        /// This creates a shallow copy of the passed Load Data Goo instance. 
        /// </summary>
        /// <param name="loadDataGoo"> Load Data Goo instance to copy. </param>
        public GH_LoadData(GH_LoadData loadDataGoo)
        {
            if (loadDataGoo == null)
            {
                loadDataGoo = new GH_LoadData();
            }

            this.Value = loadDataGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this Goo instance. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the Speed Data Goo. </returns>
        public override IGH_Goo Duplicate()
        {
            return new GH_LoadData(Value == null ? new LoadData() : Value.Duplicate());
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
                if (Value == null) { return "No internal Load Data instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid Load Data instance: Did you define the load data properties?";
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null) { return "Null Load Data"; }
            else { return Value.ToString(); }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return "Load Data"; }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return "Defines a single Load Data"; }
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
            //Cast to Load Data Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_LoadData)))
            {
                if (Value == null) { target = (Q)(object)new GH_LoadData(); }
                else { target = (Q)(object)new GH_LoadData(Value); }
                return true;
            }

            //Cast to Load Data
            if (typeof(Q).IsAssignableFrom(typeof(LoadData)))
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

            //Cast to Number
            if (typeof(Q).IsAssignableFrom(typeof(GH_Number)))
            {
                if (Value == null) { target = default; }
                else { target = (Q)(object)new GH_Number(Value.Mass); }
                return true;
            }

            //Cast to Point
            if (typeof(Q).IsAssignableFrom(typeof(GH_Point)))
            {
                if (Value == null) { target = default; }
                else { target = (Q)(object)new GH_Point(Value.CenterOfGravity); }
                return true;
            }

            //Cast to Vector
            if (typeof(Q).IsAssignableFrom(typeof(GH_Vector)))
            {
                if (Value == null) { target = default; }
                else { target = (Q)(object)new GH_Vector(Value.InertialMoments); }
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

            //Cast from Load Data Goo
            if (typeof(GH_LoadData).IsAssignableFrom(source.GetType()))
            {
                GH_LoadData loadDataGoo = source as GH_LoadData;
                Value = loadDataGoo.Value;
                return true;
            }

            //Cast from Load Data
            if (typeof(LoadData).IsAssignableFrom(source.GetType()))
            {
                Value = source as LoadData;
                return true;
            }

            //Cast from Declaration
            if (typeof(IDeclaration).IsAssignableFrom(source.GetType()))
            {
                if (source is LoadData loadData)
                {
                    Value = loadData;
                    return true;
                }
            }

            //Cast from Declaration Goo
            if (typeof(GH_Declaration).IsAssignableFrom(source.GetType()))
            {
                GH_Declaration declarationGoo = source as GH_Declaration;
                if (declarationGoo.Value is LoadData loadData)
                {
                    Value = loadData;
                    return true;
                }
            }

            //Cast from Text
            if (typeof(GH_String).IsAssignableFrom(source.GetType()))
            {
                string text = (source as GH_String).Value;

                try
                {
                    Value = LoadData.Parse(text);
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
        private const string IoKey = "Load Data";

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
            this.Value = (LoadData)RobotComponents.Utils.Serialization.ByteArrayToObject(array);

            return true;
        }
        #endregion
    }
}

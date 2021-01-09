// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Linq;
// Grasshopper Libs
using Grasshopper.Kernel.Types;
using GH_IO;
using GH_IO.Serialization;
// RobotComponents Libs
using RobotComponents.Actions;
using RobotComponents.Utils;

namespace RobotComponents.Gh.Goos.Actions
{
    /// <summary>
    /// Zone Data Goo wrapper class, makes sure the Zone Data class can be used in Grasshopper.
    /// </summary>
    public class GH_ZoneData : GH_Goo<ZoneData>, GH_ISerializable
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_ZoneData()
        {
            this.Value = null;
        }

        /// <summary>
        /// Data constructor: Create a Zone Data Goo instance from a Zone Data instance.
        /// </summary>
        /// <param name="zoneData"> Zone Data Value to store inside this Goo instance. </param>
        public GH_ZoneData(ZoneData zoneData)
        {
            this.Value = zoneData;
        }

        /// <summary>
        /// Data constructor: Creates a Zone Data Goo instance from another Zone Data Goo instance.
        /// This creates a shallow copy of the passed Zone Data Goo instance. 
        /// </summary>
        /// <param name="zoneDataGoo"> Zone Data Goo instance to copy.  </param>
        public GH_ZoneData(GH_ZoneData zoneDataGoo)
        {
            if (zoneDataGoo == null)
            {
                zoneDataGoo = new GH_ZoneData();
            }

            this.Value = zoneDataGoo.Value;
        }

        /// <summary>
        /// Data constructor: Creates a Zone Data Goo instance from a Number Goo instance. 
        /// This result a Predefined Zone Data Value. 
        /// </summary>
        /// <param name="zone"> The size (the radius) of the TCP zone in mm.  </param>
        public GH_ZoneData(GH_Number zone)
        {
            this.Value = new ZoneData(zone.Value);
        }

        /// <summary>
        /// Data constructor: Creates a Zone Data Goo instance from an Integer Goo instance. 
        /// This result a Predefined Zone Data Value. 
        /// </summary>
        /// <param name="zone"> The size (the radius) of the TCP zone in mm. </param>
        public GH_ZoneData(GH_Integer zone)
        {
            this.Value = new ZoneData(zone.Value);
        }

        /// <summary>
        /// Data constructor: Creates a Zone Data Goo instance from a Double.
        /// This result a Predefined Zone Data Value. 
        /// </summary>
        /// <param name="zone"> The size (the radius) of the TCP zone in mm. </param>
        public GH_ZoneData(double zone)
        {
            this.Value = new ZoneData(zone);
        }

        /// <summary>
        /// Data constructor: Creates a Zone Data Goo instance from an Integer. 
        /// This result a Predefined Zone Data Value. 
        /// </summary>
        /// <param name="zone"> The size (the radius) of the TCP zone in mm. </param>
        public GH_ZoneData(int zone)
        {
            this.Value = new ZoneData(zone);
        }

        /// <summary>
        /// Make a complete duplicate of this Goo instance. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the Zone Data Goo. </returns>
        public override IGH_Goo Duplicate()
        {
            return new GH_ZoneData(Value == null ? new ZoneData() : Value.Duplicate());
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
                if (Value == null) { return "No internal ZoneData instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid ZoneData instance: Did you define a name and pzone_tcp?"; 
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value
        /// </summary>
        /// <returns> Description of the the current instance value. </returns>
        public override string ToString()
        {
            if (Value == null) { return "Null Zone Data"; }
            else { return Value.ToString(); }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return "Zone Data"; }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return "Defines a Zone Data"; }
        }
        #endregion

        #region casting methods
        /// <summary>
        /// Attempt a cast to type Q.
        /// </summary>
        /// <typeparam name="Q"> Type to cast to. </typeparam>
        /// <param name="target"> Pointer to target of cast. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool CastTo<Q>(ref Q target)
        {
            //Cast to Zone Data
            if (typeof(Q).IsAssignableFrom(typeof(ZoneData)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)Value; }
                return true;
            }

            //Cast to Zone Data Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_ZoneData)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)new GH_ZoneData(Value); }
                return true;
            }

            //Cast to Action
            if (typeof(Q).IsAssignableFrom(typeof(RobotComponents.Actions.Action)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)Value; }
                return true;
            }

            //Cast to Action Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_Action)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)new GH_Action(Value); }
                return true;
            }

            //Cast to Declaration
            if (typeof(Q).IsAssignableFrom(typeof(IDeclaration)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)Value; }
                return true;
            }

            //Cast to Declaration Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_Declaration)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)new GH_Declaration(Value); }
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

            //Cast from Zone Data: Custom Zone Data
            if (typeof(ZoneData).IsAssignableFrom(source.GetType()))
            {
                Value = (ZoneData)source;
                return true;
            }

            //Cast from Number: Predefined Zone Data
            if (typeof(GH_Number).IsAssignableFrom(source.GetType()))
            {
                Value = new ZoneData((source as GH_Number).Value);
                return true;
            }

            //Cast from Integer: Predefined Zone Data
            if (typeof(GH_Integer).IsAssignableFrom(source.GetType()))
            {
                Value = new ZoneData((source as GH_Integer).Value);
                return true;
            }

            //Cast from Action
            if (typeof(RobotComponents.Actions.Action).IsAssignableFrom(source.GetType()))
            {
                if (source is ZoneData action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Action Goo
            if (typeof(GH_Action).IsAssignableFrom(source.GetType()))
            {
                GH_Action actionGoo = source as GH_Action;
                if (actionGoo.Value is ZoneData action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Declaration
            if (typeof(IDeclaration).IsAssignableFrom(source.GetType()))
            {
                if (source is ZoneData zonedata)
                {
                    Value = zonedata;
                    return true;
                }
            }

            //Cast from Declaration Goo
            if (typeof(GH_Declaration).IsAssignableFrom(source.GetType()))
            {
                GH_Declaration declarationGoo = source as GH_Declaration;
                if (declarationGoo.Value is ZoneData zonedata)
                {
                    Value = zonedata;
                    return true;
                }
            }

            //Cast from Text: Predefined Zone Data
            if (typeof(GH_String).IsAssignableFrom(source.GetType()))
            {
                string text = (source as GH_String).Value;

                if (text == "fine")
                {
                    Value = new ZoneData(-1);
                    return true;
                }

                else if (text.Contains("z"))
                {
                    if (ZoneData.ValidPredefinedNames.Contains(text))
                    {
                        try
                        {
                            text = text.Replace("z", String.Empty); // Changes z1 to 1, z5 to 5 etc. 
                            double number = System.Convert.ToDouble(text);
                            Value = new ZoneData(number);
                            return true;
                        }
                        catch
                        {
                            return false;
                        }
                    }
                }
                
                else
                {
                    try
                    {
                        double number = System.Convert.ToDouble(text);
                        Value = new ZoneData(number);
                        return true;
                    }
                    catch
                    {
                        return false;
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
        private const string IoKey = "Zone Data";

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
            this.Value = (ZoneData)HelperMethods.ByteArrayToObject(array);

            return true;
        }
        #endregion
    }
}

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
    /// Speed Data Goo wrapper class, makes sure the Speed Data class can be used in Grasshopper.
    /// </summary>
    public class GH_SpeedData : GH_Goo<SpeedData>, GH_ISerializable
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_SpeedData()
        {
            this.Value = null;
        }

        /// <summary>
        /// Data constructor: Create a Speed Data Goo instance from a Speed Data instance.
        /// </summary>
        /// <param name="speedData"> Speed Data Value to store inside this Goo instance. </param>
        public GH_SpeedData(SpeedData speedData)
        {
            this.Value = speedData;
        }

        /// <summary>
        /// Data constructor: Creates a Speed Data Goo instance from another Speed Data Goo instance.
        /// This creates a shallow copy of the passed Speed Data Goo instance. 
        /// </summary>
        /// <param name="speedDataGoo"> Speed Data Goo instance to copy. </param>
        public GH_SpeedData(GH_SpeedData speedDataGoo)
        {
            if (speedDataGoo == null)
            {
                speedDataGoo = new GH_SpeedData();
            }

            this.Value = speedDataGoo.Value;
        }

        /// <summary>
        /// Data constructor: Creates a Speed Data Goo instance from a Number Goo instance. 
        /// This result a Predefined Speed Data Value. 
        /// </summary>
        /// <param name="tcpSpeed"> The tool center point speed in mm/s to create 
        /// the the Speed Data value stored inside this Goo instance. </param>
        public GH_SpeedData(GH_Number tcpSpeed)
        {
            this.Value = new SpeedData(tcpSpeed.Value);
        }

        /// <summary>
        /// Data constructor: Creates a Speed Data Goo instance from an Integer Goo instance. 
        /// This result a Predefined Speed Data Value. 
        /// </summary>
        /// <param name="tcpSpeed"> The tool center point speed in mm/s to create 
        /// the the SpeedData value stored inside this Goo instance. </param>
        public GH_SpeedData(GH_Integer tcpSpeed)
        {
            this.Value = new SpeedData(tcpSpeed.Value);
        }

        /// <summary>
        /// Data constructor: Creates a Speed Data Goo instance from a Double. 
        /// This result a Predefined Speed Data Value. 
        /// </summary>
        /// <param name="tcpSpeed"> The tool center point speed in mm/s to create 
        /// the the SpeedData value stored inside this Goo instance. </param>
        public GH_SpeedData(double tcpSpeed)
        {
            this.Value = new SpeedData(tcpSpeed);
        }

        /// <summary>
        /// Data constructor: Creates a Speed Data Goo instance from an Integer. 
        /// This result a Predefined Speed Data Value. 
        /// </summary>
        /// <param name="tcpSpeed"> The tool center point speed in mm/s to create 
        /// the the SpeedData value stored inside this Goo instance. </param>
        public GH_SpeedData(int tcpSpeed)
        {
            this.Value = new SpeedData(tcpSpeed);
        }

        /// <summary>
        /// Make a complete duplicate of this Goo instance. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the Speed Data Goo. </returns>
        public override IGH_Goo Duplicate()
        {
            return new GH_SpeedData(Value == null ? new SpeedData() : Value.Duplicate());
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
        /// If the instance is valid, then this property should return Nothing or String.Empty.
        /// </summary>
        public override string IsValidWhyNot
        {
            get
            {
                if (Value == null) { return "No internal Speed Data instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid Speed Data instance: Did you define a name, v_tcp, v_ori, v_leax and v_reax?"; 
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null) { return "Null Speed Data"; }
            else { return Value.ToString(); }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return ("Speed Data"); }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return ("Defines a Speed Data"); }
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
            //Cast to Speed Data
            if (typeof(Q).IsAssignableFrom(typeof(SpeedData)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)Value; }
                return true;
            }

            //Cast to Speed Data Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_SpeedData)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)new GH_SpeedData(Value); }
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

            //Cast from SpeedData: Custom SpeedData
            if (typeof(SpeedData).IsAssignableFrom(source.GetType()))
            {
                Value = (SpeedData)source;
                return true;
            }

            //Cast from Number: Predefined Speed Data
            if (typeof(GH_Number).IsAssignableFrom(source.GetType()))
            {
                Value = new SpeedData((source as GH_Number).Value);
                return true;
            }

            //Cast from Integer: Predefined Speed Data
            if (typeof(GH_Integer).IsAssignableFrom(source.GetType()))
            {
                Value = new SpeedData((source as GH_Integer).Value);
                return true;
            }

            //Cast from Action
            if (typeof(RobotComponents.Actions.Action).IsAssignableFrom(source.GetType()))
            {
                if (source is SpeedData action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Action Goo
            if (typeof(GH_Action).IsAssignableFrom(source.GetType()))
            {
                GH_Action actionGoo = source as GH_Action;
                if (actionGoo.Value is SpeedData action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Declaration
            if (typeof(IDeclaration).IsAssignableFrom(source.GetType()))
            {
                if (source is SpeedData action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Declaration Goo
            if (typeof(GH_Declaration).IsAssignableFrom(source.GetType()))
            {
                GH_Declaration declarationGoo = source as GH_Declaration;
                if (declarationGoo.Value is SpeedData speeddata)
                {
                    Value = speeddata;
                    return true;
                }
            }

            //Cast from Text: Predefined Speed Data
            if (typeof(GH_String).IsAssignableFrom(source.GetType()))
            {
                string text = (source as GH_String).Value;

                if (text.Contains("v"))
                {
                    if (SpeedData.ValidPredefinedNames.Contains(text))
                    {
                        try
                        {
                            text = text.Replace("v", String.Empty); // Changes v5 to 5, v10 to 10 etc. 
                            double number = System.Convert.ToDouble(text);
                            Value = new SpeedData(number);
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
                        Value = new SpeedData(number);
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
        private const string IoKey = "Speed Data";

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
            this.Value = (SpeedData)HelperMethods.ByteArrayToObject(array);

            return true;
        }
        #endregion
    }
}

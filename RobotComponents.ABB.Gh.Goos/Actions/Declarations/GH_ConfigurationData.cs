// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// Grasshopper Libs
using Grasshopper.Kernel.Types;
using GH_IO;
using GH_IO.Serialization;
// RobotComponents Libs
using RobotComponents.ABB.Actions;
using RobotComponents.ABB.Actions.Declarations;
using RobotComponents.Utils;

namespace RobotComponents.ABB.Gh.Goos.Actions.Declarations
{
    /// <summary>
    /// Configuration Data Goo wrapper class, makes sure the Configuration Data class can be used in Grasshopper.
    /// </summary>
    public class GH_ConfigurationData : GH_Goo<ConfigurationData>, GH_ISerializable
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_ConfigurationData()
        {
            this.Value = null;
        }

        /// <summary>
        /// Data constructor: Create a Configuration Data Goo instance from a Configuration Data instance.
        /// </summary>
        /// <param name="configurationData"> Configuration Data Value to store inside this Goo instance. </param>
        public GH_ConfigurationData(ConfigurationData configurationData)
        {
            this.Value = configurationData;
        }

        /// <summary>
        /// Data constructor: Creates a Configuration Data Goo instance from another Configuration Data Goo instance.
        /// This creates a shallow copy of the passed Configuration Data Goo instance. 
        /// </summary>
        /// <param name="configurationDataGoo"> Configuration Data Goo instance to copy. </param>
        public GH_ConfigurationData(GH_ConfigurationData configurationDataGoo)
        {
            if (configurationDataGoo == null)
            {
                configurationDataGoo = new GH_ConfigurationData();
            }

            this.Value = configurationDataGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this Goo instance. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the Configuration Data Goo. </returns>
        public override IGH_Goo Duplicate()
        {
            return new GH_ConfigurationData(Value == null ? new ConfigurationData() : Value.Duplicate());
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
                if (Value == null) { return "No internal Configuration Data instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid Configuration Data instance: Did you define the robot axis configurations?";
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null) { return "Null Configuration Data"; }
            else { return Value.ToString(); }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return "Configuration Data"; }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return "Defines a Configuration Data"; }
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
            //Cast to Configuration Data Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_ConfigurationData)))
            {
                if (Value == null) { target = (Q)(object)new GH_ConfigurationData(); }
                else { target = (Q)(object)new GH_ConfigurationData(Value); }
                return true;
            }

            //Cast to Configuration Data
            if (typeof(Q).IsAssignableFrom(typeof(ConfigurationData)))
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

            //Cast to Number
            if (typeof(Q).IsAssignableFrom(typeof(GH_Number)))
            {
                if (Value == null) { target = (Q)(object)new GH_Number(); }
                else { target = (Q)(object)new GH_Number(Value.Cfx); }
                return true;
            }

            //Cast to Integer
            if (typeof(Q).IsAssignableFrom(typeof(GH_Integer)))
            {
                if (Value == null) { target = (Q)(object)new GH_Integer(); }
                else { target = (Q)(object)new GH_Integer(Value.Cfx); }
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

            //Cast from Configuration Data Goo
            if (typeof(GH_ConfigurationData).IsAssignableFrom(source.GetType()))
            {
                GH_ConfigurationData configurationDataGoo = source as GH_ConfigurationData;
                Value = configurationDataGoo.Value;
                return true;
            }

            //Cast from Configuration Data
            if (typeof(ConfigurationData).IsAssignableFrom(source.GetType()))
            {
                Value = (ConfigurationData)source;
                return true;
            }

            //Cast from Integer
            if (typeof(GH_Integer).IsAssignableFrom(source.GetType()))
            {
                int cfx = (source as GH_Integer).Value;
                Value = new ConfigurationData(0, 0, 0, cfx);
                return true;
            }

            //Cast from Number
            if (typeof(GH_Number).IsAssignableFrom(source.GetType()))
            {
                double cfx = (source as GH_Number).Value;
                Value = new ConfigurationData(0, 0, 0, (int)cfx);
                return true;
            }

            //Cast from Action
            if (typeof(IAction).IsAssignableFrom(source.GetType()))
            {
                if (source is ConfigurationData action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Action Goo
            if (typeof(GH_Action).IsAssignableFrom(source.GetType()))
            {
                GH_Action actionGoo = source as GH_Action;
                if (actionGoo.Value is ConfigurationData action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Declaration
            if (typeof(IDeclaration).IsAssignableFrom(source.GetType()))
            {
                if (source is ConfigurationData action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Declaration Goo
            if (typeof(GH_Declaration).IsAssignableFrom(source.GetType()))
            {
                GH_Declaration declarationGoo = source as GH_Declaration;
                if (declarationGoo.Value is ConfigurationData configurationdata)
                {
                    Value = configurationdata;
                    return true;
                }
            }

            //Cast from Text
            if (typeof(GH_String).IsAssignableFrom(source.GetType()))
            {
                string text = (source as GH_String).Value;

                try
                {
                    Value = ConfigurationData.Parse(text);
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
        private const string IoKey = "Configuration Data";

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
            this.Value = (ConfigurationData)Serialization.ByteArrayToObject(array);

            return true;
        }
        #endregion
    }
}

// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
// RobotComponents Libs
using RobotComponents.Definitions;
using RobotComponents.Utils;

namespace RobotComponents.Actions
{
    /// <summary>
    /// Represents the Configuration Linear instruction. 
    /// This action is used to switch on or off the monitoring of linear movements.  
    /// </summary>
    [Serializable()]
    public class LinearConfigurationControl : Action, IInstruction, ISerializable
    {
        #region fields
        private bool _isActive; // boolean that indicates if the linear configuration control is active
        #endregion

        #region (de)serialization
        /// <summary>
        /// Protected constructor needed for deserialization of the object. 
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization. </param>
        protected LinearConfigurationControl(SerializationInfo info, StreamingContext context)
        {
            // int version = (int)info.GetValue("Version", typeof(int)); // <-- use this if the (de)serialization changes
            _isActive = (bool)info.GetValue("Is Active", typeof(bool));
        }

        /// <summary>
        /// Populates a SerializationInfo with the data needed to serialize the object.
        /// </summary>
        /// <param name="info"> The SerializationInfo to populate with data. </param>
        /// <param name="context"> The destination for this serialization. </param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Version", VersionNumbering.CurrentVersionAsInt, typeof(int));
            info.AddValue("Is Active", _isActive, typeof(bool));
        }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the Linear Configuration Control class. 
        /// </summary>
        public LinearConfigurationControl()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Linear Configuration Control class. 
        /// </summary>
        /// <param name="isActive"> Specifies whether the Linear Configuration Control is enabled. </param>
        public LinearConfigurationControl(bool isActive)
        {
            _isActive = isActive;
        }

        /// <summary>
        /// Initializes a new instance of the Linear Configuration Control class by duplicating an existing Linear Configuration Control instance. 
        /// </summary>
        /// <param name="config"> The Linear Configuration Control instance to duplicate. </param>
        public LinearConfigurationControl(LinearConfigurationControl config)
        {
            _isActive = config.IsActive;
        }

        /// <summary>
        /// Returns an exact duplicate of this Linear Configuration Control instance.
        /// </summary>
        /// <returns> A deep copy of the Linear Configuration Control instance. </returns>
        public LinearConfigurationControl Duplicate()
        {
            return new LinearConfigurationControl(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Linear Configuration Control instance as IInstruction.
        /// </summary>
        /// <returns> A deep copy of the Linear Configuration Control instance as an IInstruction. </returns>
        public IInstruction DuplicateInstruction()
        {
            return new LinearConfigurationControl(this) as IInstruction;
        }

        /// <summary>
        /// Returns an exact duplicate of this Linear Configuration Control instance as an Action. 
        /// </summary>
        /// <returns> A deep copy of the Linear Configuration Control instance as an Action. </returns>
        public override Action DuplicateAction()
        {
            return new LinearConfigurationControl(this) as Action;
        }
        #endregion

        #region method
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns> A string that represents the current object. </returns>
        public override string ToString()
        {
            if (!this.IsValid)
            {
                return "Invalid Linear Configuration Control";
            }
            else if (this.IsActive)
            {
                return "Enable Linear Configuration Control";
            }
            else
            {
                return "Disable Linear Configuration Control";
            }
        }

        /// <summary>
        /// Returns the RAPID declaration code line of the this action.
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> An empty string. </returns>
        public override string ToRAPIDDeclaration(Robot robot)
        {
            return string.Empty;
        }

        /// <summary>
        /// Returns the RAPID instruction code line of the this action. 
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> The RAPID code line. </returns>
        public override string ToRAPIDInstruction(Robot robot)
        {
            if (_isActive == true)
            {
                return "ConfL\\on;";
            }
            else
            {
                return "ConfL\\off;";
            }
        }

        /// <summary>
        /// Creates declarations in the RAPID program module inside the RAPID Generator. 
        /// This method is called inside the RAPID generator.
        /// </summary>
        /// <param name="RAPIDGenerator"> The RAPID Generator. </param>
        public override void ToRAPIDDeclaration(RAPIDGenerator RAPIDGenerator)
        {
        }

        /// <summary>
        /// Creates instructions in the RAPID program module inside the RAPID Generator.
        /// This method is called inside the RAPID generator.
        /// </summary>
        /// <param name="RAPIDGenerator"> The RAPID Generator. </param>
        public override void ToRAPIDInstruction(RAPIDGenerator RAPIDGenerator)
        {
            if (_isActive == true)
            {
                RAPIDGenerator.StringBuilder.Append(Environment.NewLine + "\t\t" + "ConfL\\on;");
            }
            else
            {
                RAPIDGenerator.StringBuilder.Append(Environment.NewLine + "\t\t" + "ConfL\\off;");
            }
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets a value indicating whether or not the object is valid.
        /// </summary>
        public override bool IsValid
        {
            get { return true; }
        }

        /// <summary>
        /// Gets or set a value indicating whether Linear Configuration Control is enabled. 
        /// </summary>
        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; }
        }
        #endregion
    }

}

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
    /// Represents the Configuration Joint instruction. 
    /// This action is used to switch on or off the monitoring of joint movements.  
    /// </summary>
    [Serializable()]
    public class JointConfigurationControl : Action, IInstruction, ISerializable
    {
        #region fields
        private bool _isActive; // boolean that indicates if the joint configuration control is active
        #endregion

        #region (de)serialization
        /// <summary>
        /// Protected constructor needed for deserialization of the object. 
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization. </param>
        protected JointConfigurationControl(SerializationInfo info, StreamingContext context)
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
        /// Initializes an empty instance of the Joint Configuration Control class. 
        /// </summary>
        public JointConfigurationControl()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Joint Configuration Control class. 
        /// </summary>
        /// <param name="isActive"> Specifies whether the Joint Configuration Control is enabled. </param>
        public JointConfigurationControl(bool isActive)
        {
            _isActive = isActive;
        }

        /// <summary>
        /// Initializes a new instance of the Joint Configuration Control class by duplicating an existing Joint Configuration Control instance. 
        /// </summary>
        /// <param name="config"> The Joint Configuration Control instance to duplicate. </param>
        public JointConfigurationControl(JointConfigurationControl config)
        {
            _isActive = config.IsActive;
        }

        /// <summary>
        /// Returns an exact duplicate of this Joint Configuration Control instance.
        /// </summary>
        /// <returns> A deep copy of the Joint Configuration Control instance. </returns>
        public JointConfigurationControl Duplicate()
        {
            return new JointConfigurationControl(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Joint Configuration Control instance as IInstruction.
        /// </summary>
        /// <returns> A deep copy of the Joint Configuration Control instance as an IInstruction. </returns>
        public IInstruction DuplicateInstruction()
        {
            return new JointConfigurationControl(this) as IInstruction;
        }

        /// <summary>
        /// Returns an exact duplicate of this Joint Configuration Control instance as an Action. 
        /// </summary>
        /// <returns> A deep copy of the Joint Configuration Control instance as an Action. </returns>
        public override Action DuplicateAction()
        {
            return new JointConfigurationControl(this) as Action;
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
                return "Invalid Joint Configuration Control";
            }
            else if (this.IsActive)
            {
                return "Enable Joint Configuration Control";
            }
            else
            {
                return "Disable Joint Configuration Control";
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
                return "ConfJ\\on;";
            }
            else
            {
                return "ConfJ\\off;";
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
                RAPIDGenerator.StringBuilder.Append(Environment.NewLine + "\t\t" + "ConfJ\\on;");
            }
            else
            {
                RAPIDGenerator.StringBuilder.Append(Environment.NewLine + "\t\t" + "ConfJ\\off;");
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
        /// Gets or set a value indicating whether Joint Configuration Control is enabled. 
        /// </summary>
        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; }
        }
        #endregion
    }

}

// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
// RobotComponents Libs
using RobotComponents.Definitions;

namespace RobotComponents.Actions
{
    /// <summary>
    /// Auto Axis Configuration Class, sets Auto Axis Configuration to True or False.
    /// </summary>
    public class AutoAxisConfig : Action
    {
        #region fields
        private bool _isActive; // boolean that indicates if the auto axis configuration is active
        #endregion

        #region constructors
        /// <summary>
        /// Defines an empty Auto Axis Configuration object.
        /// </summary>
        public AutoAxisConfig()
        {
        }

        /// <summary>
        /// Defines an Auto Axis configuration.
        /// </summary>
        /// <param name="isActive">Bool that enables (true) or disables (false) the auto axis configuration.</param>
        public AutoAxisConfig(bool isActive)
        {
            _isActive = isActive;
        }

        /// <summary>
        /// Creates a new auto axis configuration by duplicating an existing auto axis configuration. 
        /// This creates a deep copy of the existing auto axis configuration 
        /// </summary>
        /// <param name="config"> The auto axis configuration that should be duplicated. </param>
        public AutoAxisConfig(AutoAxisConfig config)
        {
            _isActive = config.IsActive;
        }

        /// <summary>
        /// A method to duplicate the AutoAxisConfiguration object.
        /// </summary>
        /// <returns> Returns a deep copy of the AutoAxisConfiguration object. </returns>
        public AutoAxisConfig Duplicate()
        {
            return new AutoAxisConfig(this);
        }

        /// <summary>
        /// A method to duplicate the AutoAxisConfiguration object to an Action object. 
        /// </summary>
        /// <returns> Returns a deep copy of the AutoAxisConfiguration object as an Action object. </returns>
        public override Action DuplicateAction()
        {
            return new AutoAxisConfig(this) as Action;
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
                return "Invalid Auto Axis Configuration";
            }
            else if (this.IsActive)
            {
                return "Enable Auto Axis Configuration";
            }
            else
            {
                return "Disable Auto Axis Configuration";
            }
        }

        /// <summary>
        /// Used to create variable definition code of this action. 
        /// </summary>
        /// <param name="robot"> Defines the Robot were the code is generated for. </param>
        /// <returns> Returns the RAPID code line as a string. </returns>
        public override string ToRAPIDDeclaration(Robot robot)
        {
            return string.Empty;
        }

        /// <summary>
        /// Used to create action instruction code line. 
        /// </summary>
        /// <param name="robot"> Defines the Robot were the code is generated for. </param>
        /// <returns> Returns the RAPID code line as a string. </returns>
        public override string ToRAPIDInstruction(Robot robot)
        {
            if (_isActive == true)
            {
                return "ConfJ\\off; ConfL\\off;";
            }
            else
            {
                return "ConfJ\\on; ConfL\\on;";
            }
        }

        /// <summary>
        /// Used to create variable definitions in the RAPID Code. It is typically called inside the CreateRAPIDCode() method of the RAPIDGenerator class.
        /// </summary>
        /// <param name="RAPIDGenerator"> Defines the RAPIDGenerator. </param>
        public override void ToRAPIDDeclaration(RAPIDGenerator RAPIDGenerator)
        {
        }

        /// <summary>
        /// Used to create action instructions in the RAPID Code. It is typically called inside the CreateRAPIDCode() method of the RAPIDGenerator class.
        /// </summary>
        /// <param name="RAPIDGenerator"> Defines the RAPIDGenerator. </param>
        public override void ToRAPIDInstruction(RAPIDGenerator RAPIDGenerator)
        {
            if (_isActive == true)
            {
                RAPIDGenerator.StringBuilder.Append(Environment.NewLine + "\t\t" + "ConfJ\\off;");
                RAPIDGenerator.StringBuilder.Append(Environment.NewLine + "\t\t" + "ConfL\\off;");
            }
            else
            {
                RAPIDGenerator.StringBuilder.Append(Environment.NewLine + "\t\t" + "ConfJ\\on;");
                RAPIDGenerator.StringBuilder.Append(Environment.NewLine + "\t\t" + "ConfL\\on;");
            }
        }
        #endregion

        #region properties
        /// <summary>
        /// A boolean that indicates if the AutoAxisConfiguration object is valid.
        /// </summary>
        public override bool IsValid
        {
            get { return true; }
        }

        /// <summary>
        /// A boolean that indicates if the auto axis configruation is enabled.
        /// </summary>
        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; }
        }
        #endregion
    }

}

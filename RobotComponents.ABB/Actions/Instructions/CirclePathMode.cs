// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
// RobotComponents Libs
using RobotComponents.ABB.Definitions;
using RobotComponents.ABB.Actions.Interfaces;
using RobotComponents.ABB.Enumerations;

namespace RobotComponents.ABB.Actions.Instructions
{
    /// <summary>
    /// Represents the Circle Path Mode instruction. 
    /// </summary>
    /// <remarks>
    /// This action makes it possible to select different modes to reorientate the tool during circular movements.
    /// </remarks>
    [Serializable()]
    public class CirclePathMode : Action, IInstruction, ISerializable
    {
        #region fields
        private CirPathMode _mode;
        #endregion

        #region (de)serialization
        /// <summary>
        /// Protected constructor needed for deserialization of the object. 
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization. </param>
        protected CirclePathMode(SerializationInfo info, StreamingContext context)
        {
            // int version = (int)info.GetValue("Version", typeof(int)); // <-- use this if the (de)serialization changes
            _mode = (CirPathMode)info.GetValue("Mode", typeof(CirPathMode));
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
            info.AddValue("Mode", _mode, typeof(CirPathMode));
        }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the Circle Path Mode class. 
        /// </summary>
        public CirclePathMode()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Circle Path Mode class. 
        /// </summary>
        /// <param name="mode"> The circle path mode. </param>
        public CirclePathMode(CirPathMode mode)
        {
            _mode = mode;
        }

        /// <summary>
        /// Initializes a new instance of the Circle Path Mode class by duplicating an existing Circle Path Mode instance. 
        /// </summary>
        /// <param name="circlePathMode"> The Circle Path Mode instance to duplicate. </param>
        public CirclePathMode(CirclePathMode circlePathMode)
        {
            _mode = circlePathMode.Mode;
        }

        /// <summary>
        /// Returns an exact duplicate of this Circle Path Mode instance.
        /// </summary>
        /// <returns> 
        /// A deep copy of the Circle Path Mode instance. 
        /// </returns>
        public CirclePathMode Duplicate()
        {
            return new CirclePathMode(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Circle Path Mode instance as IInstruction.
        /// </summary>
        /// <returns> 
        /// A deep copy of the Circle Path Mode instance as an IInstruction. 
        /// </returns>
        public IInstruction DuplicateInstruction()
        {
            return new CirclePathMode(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Circle Path Mode instance as an Action. 
        /// </summary>
        /// <returns> 
        /// A deep copy of the Circle Path Mode instance as an Action. 
        /// </returns>
        public override Action DuplicateAction()
        {
            return new CirclePathMode(this);
        }
        #endregion

        #region method
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns> 
        /// A string that represents the current object. 
        /// </returns>
        public override string ToString()
        {
            if (!IsValid)
            {
                return "Invalid Circle Path Mode";
            }
            else
            {
                return "Circle Path Mode";
            }
        }

        /// <summary>
        /// Returns the RAPID declaration code line of the this action.
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> 
        /// An empty string. 
        /// </returns>
        public override string ToRAPIDDeclaration(Robot robot)
        {
            return string.Empty;
        }

        /// <summary>
        /// Returns the RAPID instruction code line of the this action. 
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> 
        /// The RAPID code line. 
        /// </returns>
        public override string ToRAPIDInstruction(Robot robot)
        {
            return $"CirPathMode \\{Enum.GetName(typeof(CirPathMode), _mode)};";
        }

        /// <summary>
        /// Creates declarations in the RAPID program module inside the RAPID Generator. 
        /// </summary>
        /// <remarks>
        /// This method is called inside the RAPID generator.
        /// </remarks>
        /// <param name="RAPIDGenerator"> The RAPID Generator. </param>
        public override void ToRAPIDDeclaration(RAPIDGenerator RAPIDGenerator)
        {
        }

        /// <summary>
        /// Creates instructions in the RAPID program module inside the RAPID Generator.
        /// </summary>
        /// <remarks>
        /// This method is called inside the RAPID generator.
        /// </remarks>
        /// <param name="RAPIDGenerator"> The RAPID Generator. </param>
        public override void ToRAPIDInstruction(RAPIDGenerator RAPIDGenerator)
        {
            RAPIDGenerator.ProgramInstructions.Add("    " + "    " + ToRAPIDInstruction(RAPIDGenerator.Robot));
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
        /// Gets or set the circular path mode.
        /// </summary>
        public CirPathMode Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }
        #endregion
    }
}

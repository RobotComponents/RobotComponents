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

namespace RobotComponents.ABB.Actions.Instructions
{
    /// <summary>
    /// Represent the Velocity Set instruction.
    /// </summary>
    /// <remarks>
    /// This action is used to override and limit the speed. 
    /// </remarks>
    [Serializable()]
    [Obsolete("This class is a work in progress and could undergo changes in the current major release.", false)]
    public class VelocitySet : Action, IInstruction, ISerializable
    {
        #region fields
        private double _override;
        private double _max;
        #endregion

        #region (de)serialization
        /// <summary>
        /// Protected constructor needed for deserialization of the object.  
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization. </param>
        protected VelocitySet(SerializationInfo info, StreamingContext context)
        {
            //Version version = (Version)info.GetValue("Version", typeof(Version)); // <-- use this if the (de)serialization changes
            _override = (double)info.GetValue("Override", typeof(double));
            _max = (double)info.GetValue("Max", typeof(double));
        }

        /// <summary>
        /// Populates a SerializationInfo with the data needed to serialize the object.
        /// </summary>
        /// <param name="info"> The SerializationInfo to populate with data. </param>
        /// <param name="context"> The destination for this serialization. </param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Version", VersionNumbering.Version, typeof(Version));
            info.AddValue("Override", _override, typeof(double));
            info.AddValue("Max", _max, typeof(double));
        }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the Velocity Set class.
        /// </summary>
        public VelocitySet()
        {
        }

        /// <summary>
        /// Initializes an empty instance of the Velocity Set class.
        /// </summary>
        /// <param name="override"> The desired velocity as a percentage of programmed velocity (0-100). </param>
        /// <param name="max"> The maximum TCP velocity in mm/s. </param>
        public VelocitySet(double @override, double max)
        {
            _override = @override;
            _max = max;
        }

        /// <summary>
        /// Initializes a new instance of the Velocity Set class by duplicating an existing Velocity Set instance. 
        /// </summary>
        /// <param name="velocitySet"> The Velocity Set instance to duplicate. </param>
        public VelocitySet(VelocitySet velocitySet)
        {
            _override = velocitySet.Override;
            _max = velocitySet.Max;
        }

        /// <summary>
        /// Returns an exact duplicate of this Velocity Set instance.
        /// </summary>
        /// <returns> 
        /// A deep copy of the Velocity Set instance. 
        /// </returns>
        public VelocitySet Duplicate()
        {
            return new VelocitySet(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Velocity Set instance as IInstruction.
        /// </summary>
        /// <returns> 
        /// A deep copy of the Velocity Set instance as an IInstruction. 
        /// </returns>
        public IInstruction DuplicateInstruction()
        {
            return new VelocitySet(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Velocity Set instance as an Action. 
        /// </summary>
        /// <returns> 
        /// A deep copy of the Velocity Set instance as an Action. 
        /// </returns>
        public override Action DuplicateAction()
        {
            return new VelocitySet(this);
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
                return "Invalid Velocity Set";
            }
            else
            {
                return $"Velocity Set";
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
            return $"VelSet {_override:0.###}, {_max:0.###};";
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
            get
            {
                if (_override < 0) { return false; }
                if (_override > 100) { return false; }
                if (_max <= 0) { return false; }
                else { return true; }
            }
        }

        /// <summary>
        /// Gets or sets the desired velocity as a percentage of programmed velocity.
        /// </summary>
        /// <remarks>
        /// Use values from 0 till 100. 
        /// </remarks>
        public double Override
        {
            get { return _override; }
            set { _override = value; }
        }

        /// <summary>
        /// Gets or sets the maximum TCP velocity in mm/s.
        /// </summary>
        public double Max
        {
            get { return _max; }
            set { _max = value; }
        }
        #endregion
    }
}

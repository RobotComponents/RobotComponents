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
    /// Represent the Acceleration Set instruction.
    /// </summary>
    /// <remarks>
    /// This action is used to adjust the acceleration and decceleration values.
    /// </remarks>
    [Serializable()]
    public class AccelerationSet : Action, IInstruction, ISerializable
    {
        #region fields
        private double _acceleration;
        private double _ramp;
        #endregion

        #region (de)serialization
        /// <summary>
        /// Protected constructor needed for deserialization of the object.  
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization. </param>
        protected AccelerationSet(SerializationInfo info, StreamingContext context)
        {
            //Version version = (Version)info.GetValue("Version", typeof(Version)); // <-- use this if the (de)serialization changes
            _acceleration = (double)info.GetValue("Acceleration", typeof(double));
            _ramp = (double)info.GetValue("Ramp", typeof(double));
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
            info.AddValue("Acceleration", _acceleration, typeof(double));
            info.AddValue("Ramp", _ramp, typeof(double));
        }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the Acceleration Set class.
        /// </summary>
        public AccelerationSet()
        {
        }

        /// <summary>
        /// Initializes an empty instance of the Acceleration Set class.
        /// </summary>
        /// <param name="acceleration"> The acceleration and deceleration as a percentage of the normal values (20-100). </param>
        /// <param name="ramp"> The rate at which acceleration and deceleration increases as a percentage of the normal values (10-100) </param>
        public AccelerationSet(double acceleration, double ramp)
        {
            _acceleration = acceleration;
            _ramp = ramp;
        }

        /// <summary>
        /// Initializes a new instance of the Acceleration Set class by duplicating an existing Acceleration Set instance. 
        /// </summary>
        /// <param name="accelerationSet"> The Acceleration Set instance to duplicate. </param>
        public AccelerationSet(AccelerationSet accelerationSet)
        {
            _acceleration = accelerationSet.Acceleration;
            _ramp = accelerationSet.Ramp;
        }

        /// <summary>
        /// Returns an exact duplicate of this Acceleration Set instance.
        /// </summary>
        /// <returns> 
        /// A deep copy of the Acceleration Set instance. 
        /// </returns>
        public AccelerationSet Duplicate()
        {
            return new AccelerationSet(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Acceleration Set instance as IInstruction.
        /// </summary>
        /// <returns> 
        /// A deep copy of the Acceleration Set instance as an IInstruction. 
        /// </returns>
        public IInstruction DuplicateInstruction()
        {
            return new AccelerationSet(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Acceleration Set instance as an Action. 
        /// </summary>
        /// <returns> 
        /// A deep copy of the Acceleration Set instance as an Action. 
        /// </returns>
        public override Action DuplicateAction()
        {
            return new AccelerationSet(this);
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
                return "Invalid Acceleration Set";
            }
            else
            {
                return $"Acceleration Set";
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
            return $"AccSet {_acceleration:0.###}, {_ramp:0.###};";
        }

        /// <summary>
        /// Creates declarations and instructions in the RAPID program module inside the RAPID Generator.
        /// </summary>
        /// <remarks>
        /// This method is called inside the RAPID generator.
        /// </remarks>
        /// <param name="RAPIDGenerator"> The RAPID Generator. </param>
        public override void ToRAPIDGenerator(RAPIDGenerator RAPIDGenerator)
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
                if (_acceleration < 20) { return false; }
                if (_acceleration > 100) { return false; }
                if (_ramp < 10) { return false; }
                if (_ramp > 100) { return false; }
                else { return true; }
            }
        }

        /// <summary>
        /// Gets or sets acceleration and deceleration as a percentage of the normal values.
        /// </summary>
        /// <remarks>
        /// Use values from 20 till 100. 
        /// </remarks>
        public double Acceleration
        {
            get { return _acceleration; }
            set { _acceleration = value; }
        }

        /// <summary>
        /// Gets or sets the rate at which acceleration and deceleration increases as a percentage of the normal values..
        /// </summary>
        /// <remarks>
        /// Use values from 10 till 100. 
        /// </remarks>
        public double Ramp
        {
            get { return _ramp; }
            set { _ramp = value; }
        }
        #endregion
    }
}
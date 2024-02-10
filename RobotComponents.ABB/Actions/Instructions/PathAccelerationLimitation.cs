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
    /// Represent the Path Acceleration Limitation instruction.
    /// </summary>
    /// <remarks>
    /// This action is used to set or reset limitations on TCP 
    /// acceleration and/or TCP deceleration along the movement path.
    /// </remarks>
    [Serializable()]
    public class PathAccelerationLimitation : Action, IInstruction, ISerializable
    {
        #region fields
        private bool _accelerationLimitation;
        private double _accelerationMax;
        private bool _decelerationLimitation;
        private double _decelerationMax;
        #endregion

        #region (de)serialization
        /// <summary>
        /// Protected constructor needed for deserialization of the object.  
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization. </param>
        protected PathAccelerationLimitation(SerializationInfo info, StreamingContext context)
        {
            //Version version = (Version)info.GetValue("Version", typeof(Version)); // <-- use this if the (de)serialization changes
            _accelerationLimitation = (bool)info.GetValue("Acceleration Limitation", typeof(bool));
            _accelerationMax = (double)info.GetValue("Acceleration Max", typeof(double));
            _decelerationLimitation = (bool)info.GetValue("Deceleration Limitation", typeof(bool));
            _decelerationMax = (double)info.GetValue("Deceleration Max", typeof(double));
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
            info.AddValue("Acceleration Limitation", _accelerationLimitation, typeof(bool));
            info.AddValue("Acceleration Max", _accelerationMax, typeof(double));
            info.AddValue("Deceleration Limitation", _accelerationLimitation, typeof(bool));
            info.AddValue("Deceleration Max", _accelerationMax, typeof(double));
        }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the Override Robot Tool class.
        /// </summary>
        public PathAccelerationLimitation()
        {
        }

        /// <summary>
        /// Initializes an empty instance of the Path Acceleration Limitation class.
        /// </summary>
        /// <param name="accelerationLimitation"> Specifies whether or not the acceleration is limited. </param>
        /// <param name="accelerationMax">  The absolute value of the acceleration limitation in m/s^2. s</param>
        /// <param name="decelerationLimitation"> Specifies whether or not the deceleration is limited. </param>
        /// <param name="decelerationMax"> The absolute value of the deceleration limitation in m/s^2. </param>
        public PathAccelerationLimitation(bool accelerationLimitation, double accelerationMax, bool decelerationLimitation, double decelerationMax)
        {
            _accelerationLimitation = accelerationLimitation;
            _accelerationMax = accelerationMax;
            _decelerationLimitation = decelerationLimitation;
            _decelerationMax = decelerationMax;
        }

        /// <summary>
        /// Initializes a new instance of the Path Acceleration Limitation class by duplicating an existing Path Acceleration Limitation instance. 
        /// </summary>
        /// <param name="pathAccelerationLimitation"> The Path Acceleration Limitation instance to duplicate. </param>
        public PathAccelerationLimitation(PathAccelerationLimitation pathAccelerationLimitation)
        {
            _accelerationLimitation = pathAccelerationLimitation.AccelerationLimitation;
            _accelerationMax = pathAccelerationLimitation.AccelerationMax;
            _decelerationLimitation = pathAccelerationLimitation.DecelerationLimitation;
            _decelerationMax = pathAccelerationLimitation.DecelerationMax;
        }

        /// <summary>
        /// Returns an exact duplicate of this Path Acceleration Limitation instance.
        /// </summary>
        /// <returns> 
        /// A deep copy of the Path Acceleration Limitation instance. 
        /// </returns>
        public PathAccelerationLimitation Duplicate()
        {
            return new PathAccelerationLimitation(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Path Acceleration Limitation instance as IInstruction.
        /// </summary>
        /// <returns> 
        /// A deep copy of the Path Acceleration Limitation instance as an IInstruction. 
        /// </returns>
        public IInstruction DuplicateInstruction()
        {
            return new PathAccelerationLimitation(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Path Acceleration Limitation instance as an Action. 
        /// </summary>
        /// <returns> 
        /// A deep copy of the Path Acceleration Limitation instance as an Action. 
        /// </returns>
        public override Action DuplicateAction()
        {
            return new PathAccelerationLimitation(this);
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
                return "Invalid Path Acceleration Limitation";
            }
            else
            {
                return $"Path Acceleration Limitation";
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
            string acceleration = _accelerationLimitation == false ? "FALSE" : $"TRUE\\AccMax:={_accelerationMax}";
            string deceleration = _decelerationLimitation == false ? "FALSE" : $"TRUE\\DecelMax:={_decelerationMax}";
            return $"PathAccelerationLimitation {acceleration}, {deceleration};";
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
                if (_accelerationMax < 0) { return false; }
                if (_decelerationMax < 0) { return false; }
                else { return true; }
            }
        }

        /// <summary>
        /// Gets or sets a value whether or not the acceleration is limited.
        /// </summary>
        public bool AccelerationLimitation
        {
            get { return _accelerationLimitation; }
            set { _accelerationLimitation = value; }
        }

        /// <summary>
        /// Gets or sets the absolute value of the acceleration limitation in m/s^2.
        /// </summary>
        public double AccelerationMax
        {
            get { return _accelerationMax; }
            set { _accelerationMax = value; }
        }

        /// <summary>
        /// Gets or sets a value whether or not the deceleration is limited.
        /// </summary>
        public bool DecelerationLimitation
        {
            get { return _decelerationLimitation; }
            set { _decelerationLimitation = value; }
        }

        /// <summary>
        ///  Gets or sets the absolute value of the deceleration limitation in m/s^2.
        /// </summary>
        public double DecelerationMax
        {
            get { return _decelerationMax; }
            set { _decelerationMax = value; }
        }
        #endregion
    }
}
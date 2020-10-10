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
    /// WaitTime class, defines waiting time between two actions. This command is used to wait a given amount of time.
    /// </summary>
    [Serializable()]
    public class WaitTime : Action, ISerializable
    {
        #region fields
        private double _duration; // the time expressed in seconds
        #endregion

        #region (de)serialization
        /// <summary>
        /// Special contructor needed for deserialization of the object. 
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization. </param>
        protected WaitTime(SerializationInfo info, StreamingContext context)
        {
            // int version = (int)info.GetValue("Version", typeof(int)); // <-- use this if the (de)serialization changes
            _duration = (double)info.GetValue("Duration", typeof(double));
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
            info.AddValue("Duration", _duration, typeof(double));
        }
        #endregion

        #region constructors
        /// <summary>
        /// Defines an empty WaitTime object. 
        /// </summary>
        public WaitTime()
        {
        }

        /// <summary>
        /// Constructor to create a WaitTime object. 
        /// </summary>
        /// <param name="Duration"> The time, expressed in seconds, that program execution is to wait. </param>
        public WaitTime(double Duration)
        {
            this._duration = Duration;
        }

        /// <summary>
        /// Creates a new WaitTime object by duplicating an existing WaitTime object. 
        /// This creates a deep copy of the existing WaitTime object. 
        /// </summary>
        /// <param name="waitTime"> The WaitTime that should be duplicated. </param>
        public WaitTime(WaitTime waitTime)
        {
            _duration = waitTime.Duration;
        }

        /// <summary>
        /// Method to duplicate the WaitTime object.
        /// </summary>
        /// <returns> Returns a deep copy of the WaitTime object.</returns>
        public WaitTime Duplicate()
        {
            return new WaitTime(this);
        }

        /// <summary>
        /// A method to duplicate the WaitTime object to an Action object. 
        /// </summary>
        /// <returns> Returns a deep copy of the WaitTime object as an Action object. </returns>
        public override Action DuplicateAction()
        {
            return new WaitTime(this) as Action;
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
                return "Invalid Wait Time";
            }
            else
            {
                return "Wait Time (" + this.Duration.ToString("0.##") + " sec.)";
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
            return "WaitTime " + _duration + ";";
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
            RAPIDGenerator.StringBuilder.Append(Environment.NewLine + "\t\t" + this.ToRAPIDInstruction(RAPIDGenerator.Robot)); 
        }
        #endregion

        #region properties
        /// <summary>
        /// A boolean that indicates if the WaitTime object is valid.
        /// </summary>
        public override bool IsValid
        {
            get
            {
                if (Duration < 0) { return false; }
                else { return true; }
            }
        }

        /// <summary>
        /// The time, expressed in seconds, that program execution is to wait. Min. value 0 seconds. Max. value no limit. Resolution 0.001 seconds.
        /// </summary>
        public double Duration
        {
            get { return _duration; }
            set { _duration = value; }
        }
        #endregion
    }

}

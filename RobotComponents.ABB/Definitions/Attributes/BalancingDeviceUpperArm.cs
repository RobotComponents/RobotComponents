// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components
// Project: https://github.com/RobotComponents/RobotComponents
//
// Copyright (c) 2025 Arjen Deetman
//
// Authors:
//   - Arjen Deetman (2025)
//
// For license details, see the LICENSE file in the project root.

// System Libs
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
// Rhino Libs
using Rhino.Geometry;

namespace RobotComponents.ABB.Definitions.Attributes
{
    /// <summary>
    /// Class included to define the required serialization schema for version 4.  
    /// It currently contains only the data fields needed for serialization; implementation will be added later.
    /// </summary>
    [Serializable()]
    internal class BalancingDeviceUpperArm : ISerializable
    {
        #region fields
        private Robot _robot;

        private Plane _planeA = Plane.Unset;
        private Plane _planeB = Plane.Unset;
        private Plane _planeC = Plane.Unset;

        private Mesh _meshBody = new Mesh();
        private Mesh _meshPiston = new Mesh();

        private Mesh _posedMeshBody = new Mesh();
        private Mesh _posedMeshPiston = new Mesh();
        #endregion

        #region (de)serialization
        /// <summary>
        /// Protected constructor needed for deserialization of the object.  
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization. </param>
        protected BalancingDeviceUpperArm(SerializationInfo info, StreamingContext context)
        {
            // Version version = (int)info.GetValue("Version", typeof(Version)); // <-- use this if the (de)serialization changes
            _planeA = (Plane)info.GetValue("Plane A", typeof(Plane));
            _planeB = (Plane)info.GetValue("Plane B", typeof(Plane));
            _planeC = (Plane)info.GetValue("Plane C", typeof(Plane));
            _meshBody = (Mesh)info.GetValue("Mesh Body", typeof(Mesh));
            _meshPiston = (Mesh)info.GetValue("Mesh Piston", typeof(Mesh));
            _posedMeshBody = (Mesh)info.GetValue("Posed Mesh Body", typeof(Mesh));
            _posedMeshPiston = (Mesh)info.GetValue("Posed Mesh Piston", typeof(Mesh));
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
            info.AddValue("Plane A", _planeA, typeof(Plane));
            info.AddValue("Plane B", _planeB, typeof(Plane));
            info.AddValue("Plane C", _planeC, typeof(Plane));
            info.AddValue("Mesh Body", _meshBody, typeof(Mesh));
            info.AddValue("Mesh Piston", _meshPiston, typeof(Mesh));
            info.AddValue("Posed Mesh Body", _posedMeshBody, typeof(Mesh));
            info.AddValue("Posed Mesh Piston", _posedMeshPiston, typeof(Mesh));
        }
        #endregion

        #region constructors
        /// <summary>
        /// Empty constructor.
        /// </summary>
        internal BalancingDeviceUpperArm()
        {
        }
        #endregion
    }
}

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
    internal class ParallelRod : ISerializable
    {
        #region fields
        private Plane _mountingPlaneRod = Plane.Unset;

        private Mesh _meshCounterWeight = new Mesh();
        private Mesh _meshRod = new Mesh();

        private Mesh _posedMeshCounterWeight = new Mesh();
        private Mesh _posedMeshRod = new Mesh();
        #endregion

        #region (de)serialization
        /// <summary>
        /// Protected constructor needed for deserialization of the object.  
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization. </param>
        protected ParallelRod(SerializationInfo info, StreamingContext context)
        {
            // Version version = (int)info.GetValue("Version", typeof(Version)); // <-- use this if the (de)serialization changes
            _mountingPlaneRod = (Plane)info.GetValue("Mounting Plane Rod", typeof(Plane));
            _meshCounterWeight = (Mesh)info.GetValue("Mesh Counter Weight", typeof(Mesh));
            _meshRod = (Mesh)info.GetValue("Mesh Rod", typeof(Mesh));
            _posedMeshCounterWeight = (Mesh)info.GetValue("Posed Mesh Counter Weight", typeof(Mesh));
            _posedMeshRod = (Mesh)info.GetValue("Posed Mesh Rod", typeof(Mesh));
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
            info.AddValue("Mounting Plane Rod", _mountingPlaneRod, typeof(Plane));
            info.AddValue("Mesh Counter Weight", _meshCounterWeight, typeof(Mesh));
            info.AddValue("Mesh Rod", _meshRod, typeof(Mesh));
            info.AddValue("Posed Mesh Counter Weight", _posedMeshCounterWeight, typeof(Mesh));
            info.AddValue("Posed Mesh Rod", _posedMeshRod, typeof(Mesh));
        }
        #endregion

        #region constructors
        /// <summary>
        /// Empty constructor.
        /// </summary>
        internal ParallelRod()
        {
        }
        #endregion
    }
}
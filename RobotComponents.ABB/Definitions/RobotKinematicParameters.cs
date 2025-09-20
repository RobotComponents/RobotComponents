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
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
// Rhino Libs
using Rhino.Geometry;

namespace RobotComponents.ABB.Definitions
{
    /// <summary>
    /// Represents the robot kinematics parameters.
    /// </summary>
    [Serializable()]
    public class RobotKinematicParameters : ISerializable
    {
        #region fields
        private readonly double _a1 = 0;
        private readonly double _a2 = 0;
        private readonly double _a3 = 0;
        private readonly double _b = 0;
        private readonly double _c1 = 0;
        private readonly double _c2 = 0;
        private readonly double _c3 = 0;
        private readonly double _c4 = 0;
        #endregion

        #region (de)serialization
        /// <summary>
        /// Protected constructor needed for deserialization of the object.  
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization. </param>
        protected RobotKinematicParameters(SerializationInfo info, StreamingContext context)
        {
            //Version version = (Version)info.GetValue("Version", typeof(Version)); // <-- use this if the (de)serialization changes
            _a1 = (double)info.GetValue("A1", typeof(double));
            _a2 = (double)info.GetValue("A2", typeof(double));
            _a3 = (double)info.GetValue("A3", typeof(double));
            _b = (double)info.GetValue("B", typeof(double));
            _c1 = (double)info.GetValue("C1", typeof(double));
            _c2 = (double)info.GetValue("C2", typeof(double));
            _c3 = (double)info.GetValue("C3", typeof(double));
            _c4 = (double)info.GetValue("C4", typeof(double));
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
            info.AddValue("A1", _a1, typeof(double));
            info.AddValue("A2", _a2, typeof(double));
            info.AddValue("A3", _a3, typeof(double));
            info.AddValue("B", _b, typeof(double));
            info.AddValue("C1", _c1, typeof(double));
            info.AddValue("C2", _c2, typeof(double));
            info.AddValue("C3", _c3, typeof(double));
            info.AddValue("C4", _c4, typeof(double));
        }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the Robot Kinematic Parameters class.
        /// </summary>
        public RobotKinematicParameters()
        {
            _a1 = double.NaN;
            _a2 = double.NaN;
            _a3 = double.NaN;
            _b = double.NaN;
            _c1 = double.NaN;
            _c2 = double.NaN;
            _c3 = double.NaN;
            _c4 = double.NaN;
        }

        /// <summary>
        /// Initializes a new instance of the Robot Kinematics parameters class.
        /// </summary>
        /// <param name="a1"> The shoulder offset A1. </param>
        /// <param name="a2"> The elbow offset A2. </param>
        /// <param name="a3"> The wrist offset A3. </param>
        /// <param name="b"> The lateral offset B. </param>
        /// <param name="c1"> The link length C1. </param>
        /// <param name="c2"> The link length C2. </param>
        /// <param name="c3"> The link length C3. </param>
        /// <param name="c4"> The link length C4. </param>
        public RobotKinematicParameters(double a1, double a2, double a3, double b, double c1, double c2, double c3, double c4)
        {
            _a1 = a1;
            _a2 = a2;   
            _a3 = a3;
            _b = b;
            _c1 = c1;
            _c2 = c2;
            _c3 = c3;
            _c4 = c4;
        }

        /// <summary>
        /// Initializes a new instance of the Robot Kinematics parameters class from given axis planes.
        /// </summary>
        /// <param name="basePlane"> The robot base plane. </param>
        /// <param name="axisPlanes"> The robot internal axis planes. </param>
        public RobotKinematicParameters(Plane basePlane, IList<Plane> axisPlanes)
        {
            Transform orient = Transform.PlaneToPlane(basePlane, Plane.WorldXY);
            List<Plane> planes = new List<Plane>();

            for (int i = 0; i < axisPlanes.Count; i++)
            {
                Plane plane = new Plane(axisPlanes[i]);
                plane.Transform(orient);
                planes.Add(plane);
            }

            _a1 = planes[1].Origin.X;
            _a2 = -(planes[4].Origin.Z - planes[2].Origin.Z);
            _a3 = -(planes[5].Origin.Z - planes[4].Origin.Z);
            _b = planes[0].Origin.Y - planes[5].Origin.Y;
            _c1 = planes[1].Origin.Z;
            _c2 = planes[2].Origin.Z - planes[1].Origin.Z;
            _c3 = planes[4].Origin.X - planes[2].Origin.X;
            _c4 = planes[5].Origin.X - planes[4].Origin.X;
        }

        /// <summary>
        /// Initializes a new instance of the Robot Kinematic Parameters class by duplicating an existing Robot Kinematic Parameters instance. 
        /// </summary>
        /// <param name="parameters"> The Robot instance to duplicate. </param>
        public RobotKinematicParameters(RobotKinematicParameters parameters)
        {
            _a1 = parameters.A1;
            _a2 = parameters.A2;
            _a3 = parameters.A3;
            _b = parameters.B;
            _c1 = parameters.C1;
            _c2 = parameters.C2;
            _c3 = parameters.C3;
            _c4 = parameters.C4;
        }

        /// <summary>
        /// Returns an exact duplicate of this Robot instance.
        /// </summary>
        /// <returns> 
        /// A deep copy of the Robot instance. 
        /// </returns>
        public RobotKinematicParameters Duplicate()
        {
            return new RobotKinematicParameters(this);
        }
        #endregion

        #region methods
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns> 
        /// A string that represents the current object. 
        /// </returns>
        public override string ToString()
        {
            return IsValid ? "Robot Kinematics Parameters" : "Invalid Robot Kinematics Parameters";
        }

        /// <summary>
        /// Returns the axis planes for a given position plane.
        /// </summary>
        /// <param name="basePlane"> The robot position plane </param>
        /// <param name="mountingFrame"> The tool mounting frame. </param>
        /// <returns> The array with axis planes. </returns>
        public Plane[] GetAxisPlanes(Plane basePlane, out Plane mountingFrame)
        {
            Plane[] planes = new Plane[6];

            planes[0] = new Plane(new Point3d(0, 0, 0), new Vector3d(0, 0, 1));
            planes[1] = new Plane(new Point3d(_a1, 0, _c1), new Vector3d(0, 1, 0));
            planes[2] = new Plane(new Point3d(_a1, 0, _c1 + _c2), new Vector3d(0, 1, 0));
            planes[3] = new Plane(new Point3d(_a1, -_b, _c1 + _c2 - _a2), new Vector3d(1, 0, 0));
            planes[4] = new Plane(new Point3d(_a1 + _c3, -_b, _c1 + _c2 - _a2), new Vector3d(0, 1, 0));
            planes[5] = new Plane(new Point3d(_a1 + _c3 + _c4, -_b, _c1 + _c2 - _a2 - _a3), new Vector3d(1, 0, 0));
            mountingFrame = new Plane(new Point3d(planes[5].Origin), -Vector3d.ZAxis, Vector3d.YAxis);

            Transform orient = Transform.PlaneToPlane(Plane.WorldXY, basePlane);

            for (int i = 0; i < planes.Length; i++)
            {
                planes[i].Transform(orient);
            }

            return planes;
        }

        #endregion

        #region properties
        /// <summary>
        /// Gets a value indicating whether or not the object is valid.
        /// </summary>
        public bool IsValid
        {
            get 
            {
                if (_a1 == double.NaN) { return false; }
                if (_a2 == double.NaN) { return false; }
                if (_a3 == double.NaN) { return false; }
                if (_b == double.NaN) { return false; }
                if (_c1 == double.NaN) { return false; }
                if (_c2 == double.NaN) { return false; }
                if (_c3 == double.NaN) { return false; }
                if (_c4 == double.NaN) { return false; }
                return true; 
            }
        }

        /// <summary>
        /// Gets the shoulder offset A1.
        /// </summary>
        public double A1
        {
            get { return _a1; }
        }

        /// <summary>
        /// Gets the elbow offset A2.
        /// </summary>
        public double A2
        {
            get { return _a2; }
        }

        /// <summary>
        /// Gets the wrist offset A3.
        /// </summary>
        public double A3
        {
            get { return _a3; }
        }

        /// <summary>
        /// Gets the lateral offset B.
        /// </summary>
        public double B
        {
            get { return _b; }
        }

        /// <summary>
        /// Gets the link length C1.
        /// </summary>
        public double C1
        {
            get { return _c1; }
        }

        /// <summary>
        /// Gets the link length C2.
        /// </summary>
        public double C2
        {
            get { return _c2; }
        }

        /// <summary>
        /// Gets the link length C3.
        /// </summary>
        public double C3
        {
            get { return _c3; }
        }

        /// <summary>
        /// Gets the link length C4.
        /// </summary>
        public double C4
        {
            get { return _c4; }
        }
        #endregion
    }
}

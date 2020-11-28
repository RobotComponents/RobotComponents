// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.Actions;
using RobotComponents.Enumerations;
using RobotComponents.Utils;

namespace RobotComponents.Definitions
{
    /// <summary>
    /// Represents an External Linear Axis.
    /// </summary>
    [Serializable()]
    public class ExternalLinearAxis : ExternalAxis, ISerializable
    {
        #region fields
        private string _name; // The name of the external axis
        private Plane _attachmentPlane; // The plane where the robot or the work object is attached
        private Plane _axisPlane; // Z-Axis of the _axisPlane is the linear axis
        private Interval _axisLimits; // The movement limits
        private int _axisNumber; // The axis logic number
        private bool _movesRobot;
        private Mesh _baseMesh; // The base mesh (fixed)
        private Mesh _linkMesh; // The link mesh posed for axis value 0
        private Curve _axisCurve; // The axis curve
        private List<Mesh> _posedMeshes; // The mesh posed for a certain axis value
        #endregion

        #region (de)serialization
        /// <summary>
        /// Protected constructor needed for deserialization of the object.  
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization. </param>
        protected ExternalLinearAxis(SerializationInfo info, StreamingContext context)
        {
            // int version = (int)info.GetValue("Version", typeof(int)); // <-- use this if the (de)serialization changes
            _name = (string)info.GetValue("Name", typeof(string));
            _attachmentPlane = (Plane)info.GetValue("Attachment Plane", typeof(Plane));
            _axisPlane = (Plane)info.GetValue("Axis Plane", typeof(Plane));
            _axisLimits = (Interval)info.GetValue("Axis Limits", typeof(Interval));
            _axisNumber = (int)info.GetValue("Axis Number", typeof(int));
            _movesRobot = (bool)info.GetValue("Moves Robot", typeof(bool));
            _baseMesh = (Mesh)info.GetValue("Base Mesh", typeof(Mesh));
            _linkMesh = (Mesh)info.GetValue("Link Mesh", typeof(Mesh));
            _axisCurve = (Curve)info.GetValue("Axis Curve", typeof(Curve));
            _posedMeshes = (List<Mesh>)info.GetValue("Posed Meshed", typeof(List<Mesh>));
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
            info.AddValue("Name", _name, typeof(string));
            info.AddValue("Attachment Plane", _attachmentPlane, typeof(Plane));
            info.AddValue("Axis Plane", _axisPlane , typeof(Plane));
            info.AddValue("Axis Limits", _axisLimits, typeof(Interval));
            info.AddValue("Axis Number", _axisNumber, typeof(int));
            info.AddValue("Moves Robot", _movesRobot, typeof(bool));
            info.AddValue("Base Mesh", _baseMesh, typeof(Mesh));
            info.AddValue("Link Mesh", _linkMesh, typeof(Mesh));
            info.AddValue("Axis Curve", _axisCurve, typeof(Curve));
            info.AddValue("Posed Meshed", _posedMeshes, typeof(List<Mesh>));
        }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the External Linear Axis class.
        /// </summary>
        public ExternalLinearAxis()
        {
            _name = "";
            _baseMesh = new Mesh();
            _linkMesh = new Mesh();
            _posedMeshes = new List<Mesh>();
            _axisNumber = -1;
            _movesRobot = true;
        }

        /// <summary>
        /// Initializes a new instance of the External Linear Axis class with empty meshes.
        /// </summary>
        /// <param name="attachmentPlane" > The attachment plane posed at the location for axis value 0. </param>
        /// <param name="axis"> The positive movement direction. </param>
        /// <param name="axisLimits"> The movement limits of the external linear axis. </param>
        public ExternalLinearAxis(Plane attachmentPlane, Vector3d axis, Interval axisLimits)
        {
            axis.Unitize();

            _name = "";
            _attachmentPlane = attachmentPlane;
            _axisPlane = new Plane(attachmentPlane.Origin, axis);
            _axisLimits = axisLimits;
            _axisNumber = -1;
            _movesRobot = true;
            _baseMesh = new Mesh();
            _linkMesh = new Mesh();
            _posedMeshes = new List<Mesh>();

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the External Linear Axis class.
        /// </summary>
        /// <param name="attachmentPlane" > The attachment plane posed at the location for axis value 0. </param>
        /// <param name="axis"> The positive movement direction. </param>
        /// <param name="axisLimits"> The movement limits of the external linear axis. </param>
        /// <param name="baseMesh"> The base mesh. </param>
        /// <param name="linkMesh"> The link mesh posed for an external axis value set to 0. </param>
        public ExternalLinearAxis(Plane attachmentPlane, Vector3d axis, Interval axisLimits, Mesh baseMesh, Mesh linkMesh)
        {
            axis.Unitize();

            _name = "";
            _attachmentPlane = attachmentPlane;
            _axisPlane = new Plane(attachmentPlane.Origin, axis);
            _axisLimits = axisLimits;
            _axisNumber = -1;
            _movesRobot = true;
            _baseMesh = baseMesh;
            _linkMesh = linkMesh;
            _posedMeshes = new List<Mesh>();

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the External Linear Axis class.
        /// </summary>
        /// <param name="attachmentPlane" > The attachment plane posed at the location for axis value 0. </param>
        /// <param name="axisPlane"> The axis plane. The Z-axis defines the positive movement direction of the axis. </param>
        /// <param name="axisLimits"> The movement limits of the external linear axis. </param>
        /// <param name="baseMesh"> The base mesh. </param>
        /// <param name="linkMesh"> The link mesh posed for an external axis value set to 0. </param>
        public ExternalLinearAxis(Plane attachmentPlane, Plane axisPlane, Interval axisLimits, Mesh baseMesh, Mesh linkMesh)
        {
            _name = "";
            _attachmentPlane = attachmentPlane;
            _axisPlane = axisPlane;
            _axisLimits = axisLimits;
            _axisNumber = -1;
            _movesRobot = true;
            _baseMesh = baseMesh;
            _linkMesh = linkMesh;
            _posedMeshes = new List<Mesh>();

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the External Linear Axis class.
        /// </summary>
        /// <param name="name"> The External Axis name. </param>
        /// <param name="attachmentPlane" > The attachment plane posed at the location for axis value 0. </param>
        /// <param name="axis"> The positive movement direction. </param>
        /// <param name="axisLimits"> The movement limits of the external linear axis. </param>
        /// <param name="baseMesh"> The base mesh. </param>
        /// <param name="linkMesh"> The link mesh posed for an external axis value set to 0. </param>
        public ExternalLinearAxis(string name, Plane attachmentPlane, Vector3d axis, Interval axisLimits, Mesh baseMesh, Mesh linkMesh)
        {
            axis.Unitize();

            _name = name;
            _attachmentPlane = attachmentPlane;
            _axisPlane = new Plane(attachmentPlane.Origin, axis);
            _axisLimits = axisLimits;
            _axisNumber = -1;
            _movesRobot = true;
            _baseMesh = baseMesh;
            _linkMesh = linkMesh;
            _posedMeshes = new List<Mesh>();

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the External Linear Axis class.
        /// </summary>
        /// <param name="name"> The External Axis name. </param>
        /// <param name="attachmentPlane" > The attachment plane posed at the location for axis value 0. </param>
        /// <param name="axis"> The positive movement direction. </param>
        /// <param name="axisLimits"> The movement limits of the external linear axis. </param>
        /// <param name="baseMeshes"> The base mesh. </param>
        /// <param name="linkMeshes"> The link mesh posed for an external axis value set to 0. </param>
        public ExternalLinearAxis(string name, Plane attachmentPlane, Vector3d axis, Interval axisLimits, List<Mesh> baseMeshes, List<Mesh> linkMeshes)
        {
            axis.Unitize();

            _name = name;
            _attachmentPlane = attachmentPlane;
            _axisPlane = new Plane(attachmentPlane.Origin, axis);
            _axisLimits = axisLimits;
            _axisNumber = -1;
            _movesRobot = true;
            _baseMesh = new Mesh();
            _linkMesh = new Mesh();
            _posedMeshes = new List<Mesh>();

            for (int i = 0; i < baseMeshes.Count; i++) { _baseMesh.Append(baseMeshes[i]); }
            for (int i = 0; i < linkMeshes.Count; i++) { _linkMesh.Append(linkMeshes[i]); }

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the External Linear Axis class.
        /// </summary>
        /// <param name="name"> The External Axis name. </param>
        /// <param name="attachmentPlane" > The attachment plane posed at the location for axis value 0. </param>
        /// <param name="axisPlane"> The axis plane. The Z-axis defines the positive movement direction of the axis. </param>
        /// <param name="axisLimits"> The movement limits of the external linear axis. </param>
        /// <param name="baseMesh"> The base mesh. </param>
        /// <param name="linkMesh"> The link mesh posed for an external axis value set to 0. </param>
        public ExternalLinearAxis(string name, Plane attachmentPlane, Plane axisPlane, Interval axisLimits, Mesh baseMesh, Mesh linkMesh)
        {
            _name = name;
            _attachmentPlane = attachmentPlane;
            _axisPlane = axisPlane;
            _axisLimits = axisLimits;
            _axisNumber = -1;
            _movesRobot = true;
            _baseMesh = baseMesh;
            _linkMesh = linkMesh;
            _posedMeshes = new List<Mesh>();

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the External Linear Axis class.
        /// </summary>
        /// <param name="name"> The External Axis name. </param>
        /// <param name="attachmentPlane" > The attachment plane posed at the location for axis value 0. </param>
        /// <param name="axisPlane"> The axis plane. The Z-axis defines the positive movement direction of the axis. </param>
        /// <param name="axisLimits"> The movement limits of the external linear axis. </param>
        /// <param name="baseMeshes"> The base mesh. </param>
        /// <param name="linkMeshes"> The link mesh posed for an external axis value set to 0. </param>
        public ExternalLinearAxis(string name, Plane attachmentPlane, Plane axisPlane, Interval axisLimits, List<Mesh> baseMeshes, List<Mesh> linkMeshes)
        {
            _name = name;
            _attachmentPlane = attachmentPlane;
            _axisPlane = axisPlane;
            _axisLimits = axisLimits;
            _axisNumber = -1;
            _movesRobot = true;
            _baseMesh = new Mesh();
            _linkMesh = new Mesh();
            _posedMeshes = new List<Mesh>();

            for (int i = 0; i < baseMeshes.Count; i++) { _baseMesh.Append(baseMeshes[i]); }
            for (int i = 0; i < linkMeshes.Count; i++) { _linkMesh.Append(linkMeshes[i]); }

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the External Linear Axis class with empty meshes.
        /// </summary>
        /// <param name="attachmentPlane" > The attachment plane posed at the location for axis value 0. </param>
        /// <param name="axis"> The positive movement direction. </param>
        /// <param name="axisLimits"> The movement limits of the external linear axis. </param>
        /// <param name="axisLogic"> The axis logic number. </param>
        /// <param name="movesRobot"> Specifies whether the external axis moves a robot. </param>
        public ExternalLinearAxis(Plane attachmentPlane, Vector3d axis, Interval axisLimits, string axisLogic, bool movesRobot = true)
        {
            axis.Unitize();

            _name = "";
            _attachmentPlane = attachmentPlane;
            _axisPlane = new Plane(attachmentPlane.Origin, axis);
            _axisLimits = axisLimits;
            _axisNumber = -1;
            _movesRobot = movesRobot;
            _baseMesh = new Mesh();
            _linkMesh = new Mesh();
            _posedMeshes = new List<Mesh>();

            SetAxisNumberFromString(axisLogic);
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the External Linear Axis class.
        /// </summary>
        /// <param name="attachmentPlane" > The attachment plane posed at the location for axis value 0. </param>
        /// <param name="axis"> The positive movement direction. </param>
        /// <param name="axisLimits"> The movement limits of the external linear axis. </param>
        /// <param name="baseMesh"> The base mesh. </param>
        /// <param name="linkMesh"> The link mesh posed for an external axis value set to 0. </param>
        /// <param name="axisLogic"> The axis logic number. </param>
        /// <param name="movesRobot"> Specifies whether the external axis moves a robot. </param>
        public ExternalLinearAxis(Plane attachmentPlane, Vector3d axis, Interval axisLimits, Mesh baseMesh, Mesh linkMesh, string axisLogic, bool movesRobot = true)
        {
            axis.Unitize();

            _name = "";
            _attachmentPlane = attachmentPlane;
            _axisPlane = new Plane(attachmentPlane.Origin, axis);
            _axisLimits = axisLimits;
            _axisNumber = -1;
            _movesRobot = movesRobot;
            _baseMesh = baseMesh;
            _linkMesh = linkMesh;
            _posedMeshes = new List<Mesh>();

            SetAxisNumberFromString(axisLogic);
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the External Linear Axis class.
        /// </summary>
        /// <param name="attachmentPlane" > The attachment plane posed at the location for axis value 0. </param>
        /// <param name="axisPlane"> The axis plane. The Z-axis defines the positive movement direction of the axis. </param>
        /// <param name="axisLimits"> The movement limits of the external linear axis. </param>
        /// <param name="baseMesh"> The base mesh. </param>
        /// <param name="linkMesh"> The link mesh posed for an external axis value set to 0. </param>
        /// <param name="axisLogic"> The axis logic number. </param>
        /// <param name="movesRobot"> Specifies whether the external axis moves a robot. </param>
        public ExternalLinearAxis(Plane attachmentPlane, Plane axisPlane, Interval axisLimits, Mesh baseMesh, Mesh linkMesh, string axisLogic, bool movesRobot = true)
        {
            _name = "";
            _attachmentPlane = attachmentPlane;
            _axisPlane = axisPlane;
            _axisLimits = axisLimits;
            _axisNumber = -1;
            _movesRobot = movesRobot;
            _baseMesh = baseMesh;
            _linkMesh = linkMesh;
            _posedMeshes = new List<Mesh>();

            SetAxisNumberFromString(axisLogic);
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the External Linear Axis class.
        /// </summary>
        /// <param name="name"> The External Axis name. </param>
        /// <param name="attachmentPlane" > The attachment plane posed at the location for axis value 0. </param>
        /// <param name="axis"> The positive movement direction. </param>
        /// <param name="axisLimits"> The movement limits of the external linear axis. </param>
        /// <param name="baseMesh"> The base mesh. </param>
        /// <param name="linkMesh"> The link mesh posed for an external axis value set to 0. </param>
        /// <param name="axisLogic"> The axis logic number. </param>
        /// <param name="movesRobot"> Specifies whether the external axis moves a robot. </param>
        public ExternalLinearAxis(string name, Plane attachmentPlane, Vector3d axis, Interval axisLimits, Mesh baseMesh, Mesh linkMesh, string axisLogic, bool movesRobot = true)
        {
            axis.Unitize();

            _name = name;
            _attachmentPlane = attachmentPlane;
            _axisPlane = new Plane(attachmentPlane.Origin, axis);
            _axisLimits = axisLimits;
            _axisNumber = -1;
            _movesRobot = movesRobot;
            _baseMesh = baseMesh;
            _linkMesh = linkMesh;
            _posedMeshes = new List<Mesh>();

            SetAxisNumberFromString(axisLogic);
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the External Linear Axis class.
        /// </summary>
        /// <param name="name"> The External Axis name. </param>
        /// <param name="attachmentPlane" > The attachment plane posed at the location for axis value 0. </param>
        /// <param name="axis"> The positive movement direction. </param>
        /// <param name="axisLimits"> The movement limits of the external linear axis. </param>
        /// <param name="baseMeshes"> The base mesh. </param>
        /// <param name="linkMeshes"> The link mesh posed for an external axis value set to 0. </param>
        /// <param name="axisLogic"> The axis logic number. </param>
        /// <param name="movesRobot"> Specifies whether the external axis moves a robot. </param>
        public ExternalLinearAxis(string name, Plane attachmentPlane, Vector3d axis, Interval axisLimits, List<Mesh> baseMeshes, List<Mesh> linkMeshes, string axisLogic, bool movesRobot = true)
        {
            axis.Unitize();

            _name = name;
            _attachmentPlane = attachmentPlane;
            _axisPlane = new Plane(attachmentPlane.Origin, axis);
            _axisLimits = axisLimits;
            _axisNumber = -1;
            _movesRobot = movesRobot;
            _baseMesh = new Mesh();
            _linkMesh = new Mesh();
            _posedMeshes = new List<Mesh>();

            for (int i = 0; i < baseMeshes.Count; i++) { _baseMesh.Append(baseMeshes[i]); }
            for (int i = 0; i < linkMeshes.Count; i++) { _linkMesh.Append(linkMeshes[i]); }

            SetAxisNumberFromString(axisLogic);
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the External Linear Axis class.
        /// </summary>
        /// <param name="name"> The External Axis name. </param>
        /// <param name="attachmentPlane" > The attachment plane posed at the location for axis value 0. </param>
        /// <param name="axisPlane"> The axis plane. The Z-axis defines the positive movement direction of the axis. </param>
        /// <param name="axisLimits"> The movement limits of the external linear axis. </param>
        /// <param name="baseMesh"> The base mesh. </param>
        /// <param name="linkMesh"> The link mesh posed for an external axis value set to 0. </param>
        /// <param name="axisLogic"> The axis logic number. </param>
        /// <param name="movesRobot"> Specifies whether the external axis moves a robot. </param>
        public ExternalLinearAxis(string name, Plane attachmentPlane, Plane axisPlane, Interval axisLimits, Mesh baseMesh, Mesh linkMesh, string axisLogic, bool movesRobot = true)
        {
            _name = name;
            _attachmentPlane = attachmentPlane;
            _axisPlane = axisPlane;
            _axisLimits = axisLimits;
            _axisNumber = -1;
            _movesRobot = movesRobot;
            _baseMesh = baseMesh;
            _linkMesh = linkMesh;
            _posedMeshes = new List<Mesh>();

            SetAxisNumberFromString(axisLogic);
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the External Linear Axis class.
        /// </summary>
        /// <param name="name"> The External Axis name. </param>
        /// <param name="attachmentPlane" > The attachment plane posed at the location for axis value 0. </param>
        /// <param name="axisPlane"> The axis plane. The Z-axis defines the positive movement direction of the axis. </param>
        /// <param name="axisLimits"> The movement limits of the external linear axis. </param>
        /// <param name="baseMeshes"> The base mesh. </param>
        /// <param name="linkMeshes"> The link mesh posed for an external axis value set to 0. </param>
        /// <param name="axisLogic"> The axis logic number. </param>
        /// <param name="movesRobot"> Specifies whether the external axis moves a robot. </param>
        public ExternalLinearAxis(string name, Plane attachmentPlane, Plane axisPlane, Interval axisLimits, List<Mesh> baseMeshes, List<Mesh> linkMeshes, string axisLogic, bool movesRobot = true)
        {
            _name = name;
            _attachmentPlane = attachmentPlane;
            _axisPlane = axisPlane;
            _axisLimits = axisLimits;
            _axisNumber = -1;
            _movesRobot = movesRobot;
            _baseMesh = new Mesh();
            _linkMesh = new Mesh();
            _posedMeshes = new List<Mesh>();

            for (int i = 0; i < baseMeshes.Count; i++) { _baseMesh.Append(baseMeshes[i]); }
            for (int i = 0; i < linkMeshes.Count; i++) { _linkMesh.Append(linkMeshes[i]); }

            SetAxisNumberFromString(axisLogic);
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the External Linear Axis class with empty meshes.
        /// </summary>
        /// <param name="attachmentPlane" > The attachment plane posed at the location for axis value 0. </param>
        /// <param name="axis"> The positive movement direction. </param>
        /// <param name="axisLimits"> The movement limits of the external linear axis. </param>
        /// <param name="axisLogic"> The axis logic number. </param>
        /// <param name="movesRobot"> Specifies whether the external axis moves a robot. </param>
        public ExternalLinearAxis(Plane attachmentPlane, Vector3d axis, Interval axisLimits, int axisLogic, bool movesRobot = true)
        {
            axis.Unitize();

            _name = "";
            _attachmentPlane = attachmentPlane;
            _axisPlane = new Plane(attachmentPlane.Origin, axis);
            _axisLimits = axisLimits;
            _axisNumber = axisLogic;
            _movesRobot = movesRobot;
            _baseMesh = new Mesh();
            _linkMesh = new Mesh();
            _posedMeshes = new List<Mesh>();

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the External Linear Axis class.
        /// </summary>
        /// <param name="attachmentPlane" > The attachment plane posed at the location for axis value 0. </param>
        /// <param name="axis"> The positive movement direction. </param>
        /// <param name="axisLimits"> The movement limits of the external linear axis. </param>
        /// <param name="baseMesh"> The base mesh. </param>
        /// <param name="linkMesh"> The link mesh posed for an external axis value set to 0. </param>
        /// <param name="axisLogic"> The axis logic number. </param>
        /// <param name="movesRobot"> Specifies whether the external axis moves a robot. </param>
        public ExternalLinearAxis(Plane attachmentPlane, Vector3d axis, Interval axisLimits, Mesh baseMesh, Mesh linkMesh, int axisLogic, bool movesRobot = true)
        {
            axis.Unitize();

            _name = "";
            _attachmentPlane = attachmentPlane;
            _axisPlane = new Plane(attachmentPlane.Origin, axis);
            _axisLimits = axisLimits;
            _axisNumber = axisLogic;
            _movesRobot = movesRobot;
            _baseMesh = baseMesh;
            _linkMesh = linkMesh;
            _posedMeshes = new List<Mesh>();

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the External Linear Axis class.
        /// </summary>
        /// <param name="name"> The External Axis name. </param>
        /// <param name="attachmentPlane" > The attachment plane posed at the location for axis value 0. </param>
        /// <param name="axis"> The positive movement direction. </param>
        /// <param name="axisLimits"> The movement limits of the external linear axis. </param>
        /// <param name="baseMesh"> The base mesh. </param>
        /// <param name="linkMesh"> The link mesh posed for an external axis value set to 0. </param>
        /// <param name="axisLogic"> The axis logic number. </param>
        /// <param name="movesRobot"> Specifies whether the external axis moves a robot. </param>
        public ExternalLinearAxis(string name, Plane attachmentPlane, Vector3d axis, Interval axisLimits, Mesh baseMesh, Mesh linkMesh, int axisLogic, bool movesRobot = true)
        {
            axis.Unitize();

            _name = name;
            _attachmentPlane = attachmentPlane;
            _axisPlane = new Plane(attachmentPlane.Origin, axis);
            _axisLimits = axisLimits;
            _axisNumber = axisLogic;
            _movesRobot = movesRobot;
            _baseMesh = baseMesh;
            _linkMesh = linkMesh;
            _posedMeshes = new List<Mesh>();

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the External Linear Axis class.
        /// </summary>
        /// <param name="name"> The External Axis name. </param>
        /// <param name="attachmentPlane" > The attachment plane posed at the location for axis value 0. </param>
        /// <param name="axis"> The positive movement direction. </param>
        /// <param name="axisLimits"> The movement limits of the external linear axis. </param>
        /// <param name="baseMeshes"> The base mesh. </param>
        /// <param name="linkMeshes"> The link mesh posed for an external axis value set to 0. </param>
        /// <param name="axisLogic"> The axis logic number. </param>
        /// <param name="movesRobot"> Specifies whether the external axis moves a robot. </param>
        public ExternalLinearAxis(string name, Plane attachmentPlane, Vector3d axis, Interval axisLimits, List<Mesh> baseMeshes, List<Mesh> linkMeshes, int axisLogic, bool movesRobot = true)
        {
            axis.Unitize();

            _name = name;
            _attachmentPlane = attachmentPlane;
            _axisPlane = new Plane(attachmentPlane.Origin, axis);
            _axisLimits = axisLimits;
            _axisNumber = axisLogic;
            _movesRobot = movesRobot;
            _baseMesh = new Mesh();
            _linkMesh = new Mesh();
            _posedMeshes = new List<Mesh>();

            for (int i = 0; i < baseMeshes.Count; i++) { _baseMesh.Append(baseMeshes[i]); }
            for (int i = 0; i < linkMeshes.Count; i++) { _linkMesh.Append(linkMeshes[i]); }

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the External Linear Axis class.
        /// </summary>
        /// <param name="name"> The External Axis name. </param>
        /// <param name="attachmentPlane" > The attachment plane posed at the location for axis value 0. </param>
        /// <param name="axisPlane"> The axis plane. The Z-axis defines the positive movement direction of the axis. </param>
        /// <param name="axisLimits"> The movement limits of the external linear axis. </param>
        /// <param name="baseMesh"> The base mesh. </param>
        /// <param name="linkMesh"> The link mesh posed for an external axis value set to 0. </param>
        /// <param name="axisLogic"> The axis logic number. </param>
        /// <param name="movesRobot"> Specifies whether the external axis moves a robot. </param>
        public ExternalLinearAxis(string name, Plane attachmentPlane, Plane axisPlane, Interval axisLimits, Mesh baseMesh, Mesh linkMesh, int axisLogic, bool movesRobot = true)
        {
            _name = name;
            _attachmentPlane = attachmentPlane;
            _axisPlane = axisPlane;
            _axisLimits = axisLimits;
            _axisNumber = axisLogic;
            _movesRobot = movesRobot;
            _baseMesh = baseMesh;
            _linkMesh = linkMesh;
            _posedMeshes = new List<Mesh>();

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the External Linear Axis class.
        /// </summary>
        /// <param name="name"> The External Axis name. </param>
        /// <param name="attachmentPlane" > The attachment plane posed at the location for axis value 0. </param>
        /// <param name="axisPlane"> The axis plane. The Z-axis defines the positive movement direction of the axis. </param>
        /// <param name="axisLimits"> The movement limits of the external linear axis. </param>
        /// <param name="baseMeshes"> The base mesh. </param>
        /// <param name="linkMeshes"> The link mesh posed for an external axis value set to 0. </param>
        /// <param name="axisLogic"> The axis logic number. </param>
        /// <param name="movesRobot"> Specifies whether the external axis moves a robot. </param>
        public ExternalLinearAxis(string name, Plane attachmentPlane, Plane axisPlane, Interval axisLimits, List<Mesh> baseMeshes, List<Mesh> linkMeshes, int axisLogic, bool movesRobot = true)
        {
            _name = name;
            _attachmentPlane = attachmentPlane;
            _axisPlane = axisPlane;
            _axisLimits = axisLimits;
            _axisNumber = axisLogic;
            _movesRobot = movesRobot;
            _baseMesh = new Mesh();
            _linkMesh = new Mesh();
            _posedMeshes = new List<Mesh>();

            for (int i = 0; i < baseMeshes.Count; i++) { _baseMesh.Append(baseMeshes[i]); }
            for (int i = 0; i < linkMeshes.Count; i++) { _linkMesh.Append(linkMeshes[i]); }

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the External Linear Axis class by duplicating an existing External Linear Axis instance. 
        /// </summary>
        /// <param name="externalLinearAxis"> The External Linear Axis instance to duplicate. </param>
        /// <param name="duplicateMesh"> Specifies whether the meshes should be duplicated. </param>
        public ExternalLinearAxis(ExternalLinearAxis externalLinearAxis, bool duplicateMesh = true)
        {
            _name = externalLinearAxis.Name;
            _axisPlane = new Plane(externalLinearAxis.AxisPlane);
            _attachmentPlane = new Plane(externalLinearAxis.AttachmentPlane);
            _axisLimits = new Interval(externalLinearAxis.AxisLimits);
            _axisNumber = externalLinearAxis.AxisNumber;
            _movesRobot = externalLinearAxis.MovesRobot;
            _axisCurve = externalLinearAxis.AxisCurve;

            if (duplicateMesh == true)
            {
                _baseMesh = externalLinearAxis.BaseMesh.DuplicateMesh();
                _linkMesh = externalLinearAxis.LinkMesh.DuplicateMesh();
                _posedMeshes = externalLinearAxis.PosedMeshes.ConvertAll(mesh => mesh.DuplicateMesh());
            }
            else
            {
                _baseMesh = new Mesh();
                _linkMesh = new Mesh();
                _posedMeshes = new List<Mesh>();
            }
        }

        /// <summary>
        /// Returns an exact duplicate of this External Linear Axis instance.
        /// </summary>
        /// <returns> A deep copy of the External Linear Axis instance. </returns>
        public ExternalLinearAxis Duplicate()
        {
            return new ExternalLinearAxis(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this External Linear Axis instance without meshes.
        /// </summary>
        /// <returns> A deep copy of the External Linear Axis instance without meshes. </returns>
        public ExternalLinearAxis DuplicateWithoutMesh()
        {
            return new ExternalLinearAxis(this, false);
        }

        /// <summary>
        /// Returns an exact duplicate of this External Linear Axis instance as an External Axis.
        /// </summary>
        /// <returns> A deep copy of the External Linear Axis instance as an External Axis. </returns>
        public override ExternalAxis DuplicateExternalAxis()
        {
            return new ExternalLinearAxis(this) as ExternalAxis;
        }

        /// <summary>
        /// Returns an exact duplicate of this External Linear Axis instance as an External Axis without meshes.
        /// </summary>
        /// <returns> A deep copy of the External Linear Axis instance as an External Axis without meshes. </returns>
        public override ExternalAxis DuplicateExternalAxisWithoutMesh()
        {
            return new ExternalLinearAxis(this, false) as ExternalAxis;
        }
        #endregion

        #region methods
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns> A string that represents the current object. </returns>
        public override string ToString()
        {
            if (!this.IsValid)
            {
                return "Invalid External Linear Axis";
            }
            else
            {
                return "External Linear Axis (" + this.Name + ")";
            }
        }

        /// <summary>
        /// Sets the axis logic number from a string. 
        /// Only used in constructors.
        /// </summary>
        /// <param name="text"> The string with the axis logic number. </param>
        private void SetAxisNumberFromString(string text)
        {
            text = text.Replace(" ", "");
            text = text.Replace("\t", "");
            text = text.Replace("\n", "");
            text = text.Replace("\r", "");

            List<string> validNumbers = new List<string> { "-1", "0", "1", "2", "3", "4", "5" };
            List<string> validCharacters = new List<string> { "a", "b", "c", "d", "e", "f", "A", "B", "C", "D", "E", "F" };

            if (validNumbers.Contains(text))
            {
                _axisNumber = Convert.ToInt32(text);
            }
            else if (validCharacters.Contains(text))
            {
                switch (text)
                {
                    case "a": _axisNumber = 0; break;
                    case "b": _axisNumber = 1; break;
                    case "c": _axisNumber = 2; break;
                    case "d": _axisNumber = 3; break;
                    case "e": _axisNumber = 4; break;
                    case "f": _axisNumber = 5; break;

                    case "A": _axisNumber = 0; break;
                    case "B": _axisNumber = 1; break;
                    case "C": _axisNumber = 2; break;
                    case "D": _axisNumber = 3; break;
                    case "E": _axisNumber = 4; break;
                    case "F": _axisNumber = 5; break;

                    default: _axisNumber = -1; break;
                }
            }
            else
            {
                throw new Exception("Invalid Axis Logic Number: Allowed values are -1, 0, 1, 2, 3, 4, 5, a, b, c, d, e, f, A, B, C, D, E and F.");
            }
        }

        /// <summary>
        /// Gets the axis curve. The direction of the curve defines the movement direction and the spatial limits of the attachement plane.
        /// </summary>
        /// <returns> The axis curve. </returns>
        public Curve GetAxisCurve()
        {
            Line line = new Line(_attachmentPlane.Origin + _axisPlane.ZAxis * _axisLimits.Min, _attachmentPlane.Origin + _axisPlane.ZAxis * _axisLimits.Max);
            _axisCurve = line.ToNurbsCurve();
            _axisCurve.Domain = _axisLimits;

            return _axisCurve;
        }

        /// <summary>
        /// Returns the position of the attachment plane for a given External Joint Position.
        /// This calculation does not take into account the axis limits. 
        /// </summary>
        /// <param name="externalJointPosition"> The External Joint Position. </param>
        /// <param name="inLimits"> Specifies whether the External Joint Position is inside its limits. </param>
        /// <returns> The posed attachement plane. </returns>
        public override Plane CalculatePosition(ExternalJointPosition externalJointPosition, out bool inLimits)
        {
            Transform translateNow = CalculateTransformationMatrix(externalJointPosition, out bool isInLimits);
            Plane positionPlane = new Plane(AttachmentPlane);
            positionPlane.Transform(translateNow);

            inLimits = isInLimits;
            return positionPlane;
        }

        /// <summary>
        /// Returns the the transformation matrix for a given External Joint Position.
        /// This calculation does not take into account the axis limits. 
        /// </summary>
        /// <param name="externalJointPosition"> The External Joint Position. </param>
        /// <param name="inLimits"> Specifies whether the External Joint Position is inside its limits. </param>
        /// <returns> The transformation matrix. </returns>
        public override Transform CalculateTransformationMatrix(ExternalJointPosition externalJointPosition, out bool inLimits)
        {
            double axisValue = externalJointPosition[_axisNumber];
            bool isInLimits;

            if (axisValue == 9e9) { axisValue = Math.Max(0, Math.Min(_axisLimits.Min, _axisLimits.Max)); }

            if (axisValue < AxisLimits.Min)
            {
                isInLimits = false;
            }
            else if (axisValue > AxisLimits.Max)
            {
                isInLimits = false;
            }
            else
            {
                isInLimits = true;
            }

            Transform transform = Rhino.Geometry.Transform.Translation(_axisPlane.ZAxis * axisValue);
            inLimits = isInLimits;

            return transform;
        }

        /// <summary>
        /// Returns the position of the attachment plane for a given External Joint Position.
        /// This calculations takes into account the external axis limits. 
        /// If the defined External Joint Posiiton is outside its limits the closest valid external axis value will be used.  
        /// </summary>
        /// <param name="externalJointPosition"> The External Joint Position. </param>
        /// <returns> The posed attachement plane. </returns>
        public override Plane CalculatePositionSave(ExternalJointPosition externalJointPosition)
        {
            Transform translateNow = CalculateTransformationMatrixSave(externalJointPosition);
            Plane positionPlane = new Plane(AttachmentPlane);
            positionPlane.Transform(translateNow);

            return positionPlane;
        }

        /// <summary>
        /// Returns the the transformation matrix for a given External Joint Position.
        /// This calculations takes into account the external axis limits. 
        /// If the defined External Joint Posiiton is outside its limits the closest valid external axis value will be used. 
        /// </summary>
        /// <param name="externalJointPosition"> The External Joint Position. </param>
        /// <returns> The transformation matrix. </returns>
        public override Transform CalculateTransformationMatrixSave(ExternalJointPosition externalJointPosition)
        {
            double axisValue = externalJointPosition[_axisNumber];
            double value;

            if (axisValue == 9e9) { axisValue = Math.Max(0, Math.Min(_axisLimits.Min, _axisLimits.Max)); }

            if (axisValue < _axisLimits.Min)
            {
                value = _axisLimits.Min;
            }
            else if (axisValue > _axisLimits.Max)
            {
                value = _axisLimits.Max;
            }
            else
            {
                value = axisValue;
            }

            Transform transform = Rhino.Geometry.Transform.Translation(_axisPlane.ZAxis * value);

            return transform;
        }

        /// <summary>
        /// Calculates and returns the position of the external axis meshes for a given External Joint Position.
        /// </summary>
        /// <param name="externalJointPosition"> The External Joint Position. </param>
        /// <returns> The posed meshes. </returns>
        public override List<Mesh> PoseMeshes(ExternalJointPosition externalJointPosition)
        {
            _posedMeshes.Clear();
            _posedMeshes.Add(_baseMesh.DuplicateMesh());
            _posedMeshes.Add(_linkMesh.DuplicateMesh());

            Transform translateNow = CalculateTransformationMatrix(externalJointPosition, out _);
            _posedMeshes[1].Transform(translateNow);

            return _posedMeshes;
        }

        /// <summary>
        /// Initializes the fields and properties to construct a valid External Linear Axis instance. 
        /// </summary>
        private void Initialize()
        {
            GetAxisCurve();
        }

        /// <summary>
        /// Reinitializes the fields and properties to construct a valid External Linear Axis instance. 
        /// </summary>
        public override void ReInitialize()
        {
            Initialize();
            _posedMeshes.Clear();
        }

        /// <summary>
        /// Transforms the external axis spatial properties (planes and meshes). 
        /// </summary>
        /// <param name="xform"> The spatial deform. </param>
        public override void Transform(Transform xform)
        {
            _attachmentPlane.Transform(xform);
            _axisPlane.Transform(xform);
            _baseMesh.Transform(xform);
            _linkMesh.Transform(xform);

            for (int i = 0; i < _posedMeshes.Count; i++)
            {
                _posedMeshes[i].Transform(xform);
            }

            GetAxisCurve();
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
                if (AttachmentPlane == null) { return false; }
                if (AttachmentPlane == Plane.Unset) { return false; }
                if (AxisPlane == null) { return false; }
                if (AxisPlane == Plane.Unset) { return false; }
                if (AxisLimits == null) { return false; }
                if (AxisNumber < -1) { return false; }
                if (AxisNumber > 5) { return false; }
                return true;
            }
        }

        /// <summary>
        /// Gets or sets the external axis name. 
        /// </summary>
        public override string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets or sets the attachment plane to attach a robot or work object.
        /// </summary>
        public override Plane AttachmentPlane 
        {
            get 
            { 
                return _attachmentPlane; 
            }
            set 
            { 
                _attachmentPlane = value;
                ReInitialize();
            }
        }

        /// <summary>
        /// Gets or sets the axis plane.
        /// The z-axis of the place defines the positive movement direction of the external linear axis.
        /// </summary>
        public override Plane AxisPlane 
        { 
            get 
            { 
                return _axisPlane; 
            }
            set 
            { 
                _axisPlane = value;
                ReInitialize();
            }
        }

        /// <summary>
        /// Gets or sets the axis limits in meters.
        /// </summary>
        public override Interval AxisLimits 
        { 
            get 
            { 
                return _axisLimits; 
            }
            set 
            {
                _axisLimits = value;
                ReInitialize();
            }
        }

        /// <summary>
        /// Gets or sets the axis logic as a number (-1, 0, 1, 2, 3, 4, 5).
        /// </summary>
        public override int AxisNumber 
        { 
            get { return _axisNumber; }
            set { _axisNumber = value; }
        }

        /// <summary>
        /// Gets the axis logic as a char (-, A, B, C, E, E, F).
        /// </summary>
        public override char AxisLogic
        {
            get
            {
                switch (_axisNumber)
                {
                    case -1: return '-';
                    case 0: return 'A';
                    case 1: return 'B';
                    case 2: return 'C';
                    case 3: return 'D';
                    case 4: return 'E';
                    case 5: return 'F';
                    default: return '-';
                }
            }
            set
            {
                switch(value)
                {
                    case 'a': _axisNumber = 0; break;
                    case 'b': _axisNumber = 1; break;
                    case 'c': _axisNumber = 2; break;
                    case 'd': _axisNumber = 3; break;
                    case 'e': _axisNumber = 4; break;
                    case 'f': _axisNumber = 5; break;

                    case 'A': _axisNumber = 0; break;
                    case 'B': _axisNumber = 1; break;
                    case 'C': _axisNumber = 2; break;
                    case 'D': _axisNumber = 3; break;
                    case 'E': _axisNumber = 4; break;
                    case 'F': _axisNumber = 5; break;

                    default: _axisNumber = -1; break;
                }
            }
        }

        /// <summary>
        /// Gets the Axis Type.
        /// </summary>
        public override AxisType AxisType 
        { 
            get { return AxisType.LINEAR; }
        }

        /// <summary>
        /// Gets or sets the fixed base mesh of the external axis. 
        /// </summary>
        public override Mesh BaseMesh 
        { 
            get 
            { 
                return _baseMesh; 
            }
            set 
            { 
                _baseMesh = value;
                _posedMeshes = new List<Mesh>();
            }
        }

        /// <summary>
        /// Gets or sets the movable link mesh of the external axis posed for external axis value set to 0.
        /// </summary>
        public override Mesh LinkMesh 
        { 
            get 
            { 
                return _linkMesh; 
            }
            set 
            { 
                _linkMesh = value;
                _posedMeshes = new List<Mesh>();
            }
        }

        /// <summary>
        /// Gets the axis curve. 
        /// The direction of the curve defines the movement direction and the spatial limits of the attachement plane.
        /// </summary>
        public Curve AxisCurve 
        { 
            get { return _axisCurve; }
        }

        /// <summary>
        /// Gets latest calculated posed axis meshes.
        /// </summary>
        public override List<Mesh> PosedMeshes 
        { 
            get { return _posedMeshes; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not this External Linear Axis moves the Robot.
        /// </summary>
        public override bool MovesRobot 
        { 
            get { return _movesRobot; }
            set { _movesRobot = value; }
        }
        #endregion
    }
}

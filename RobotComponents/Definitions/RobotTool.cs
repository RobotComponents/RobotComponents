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
using RobotComponents.Enumerations;
using RobotComponents.Utils;

namespace RobotComponents.Definitions
{
    /// <summary>
    /// Represents a Robot Tool.
    /// </summary>
    [Serializable()]
    public class RobotTool : ISerializable
    {
        #region fields
        private ReferenceType _referenceType; // reference type
        private string _name; // tool name
        private Mesh _mesh; // tool mesh
        private Plane _attachmentPlane; // mounting frame
        private Plane _toolPlane; // tool center point and orientation
        private bool _robotHold;
        private Point3d _position;
        private Quaternion _orientation;
        private double _mass;
        private Plane _centerOfGravity;
        private Point3d _centerOfGravityPosition;
        private Quaternion _centerOfGravityOrientation;
        private Vector3d _inertia;
        #endregion

        #region (de)serialization
        /// <summary>
        /// Protected constructor needed for deserialization of the object.  
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization. </param>
        protected RobotTool(SerializationInfo info, StreamingContext context)
        {
            // int version = (int)info.GetValue("Version", typeof(int)); // <-- use this if the (de)serialization changes
            _referenceType = (ReferenceType)info.GetValue("Reference Type", typeof(ReferenceType));
            _name = (string)info.GetValue("Name", typeof(string));
            _mesh = (Mesh)info.GetValue("Mesh", typeof(Mesh));
            _attachmentPlane = (Plane)info.GetValue("Attachment Plane", typeof(Plane));
            _toolPlane = (Plane)info.GetValue("Tool Plane", typeof(Plane));
            _robotHold = (bool)info.GetValue("Robot Hold", typeof(bool));
            _mass = (double)info.GetValue("Mass", typeof(double));
            _centerOfGravity = (Plane)info.GetValue("Center Of Gravity", typeof(Plane));
            _centerOfGravityPosition = (Point3d)info.GetValue("Center Of Gravity Position", typeof(Point3d));
            _centerOfGravityOrientation = (Quaternion)info.GetValue("Center Of Gravity Orientation", typeof(Quaternion)); ;
            _inertia = (Vector3d)info.GetValue("Inertia", typeof(Vector3d));

            Initialize();
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
            info.AddValue("Reference Type", _referenceType, typeof(ReferenceType));
            info.AddValue("Name", _name, typeof(string));
            info.AddValue("Mesh", _mesh, typeof(Mesh));
            info.AddValue("Attachment Plane", _attachmentPlane, typeof(Plane));
            info.AddValue("Tool Plane", _toolPlane, typeof(Plane));
            info.AddValue("Robot Hold", _robotHold, typeof(bool));
            info.AddValue("Mass", _mass, typeof(double));
            info.AddValue("Center Of Gravity", _centerOfGravity, typeof(Plane));
            info.AddValue("Center Of Gravity Position", _centerOfGravityPosition, typeof(Point3d));
            info.AddValue("Center Of Gravity Orientation", _centerOfGravityOrientation, typeof(Quaternion));
            info.AddValue("Inertia", _inertia, typeof(Vector3d));
        }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes a new instance of the Robot Tool class with the default tool tool0.
        /// </summary>
        public RobotTool()
        {
            _referenceType = ReferenceType.PERS;
            _name = "tool0";
            _mesh = new Mesh();
            _attachmentPlane = Plane.WorldXY;
            _toolPlane = Plane.WorldXY;

            _robotHold = true;
            _mass = 0.001;
            _centerOfGravity = new Plane(new Point3d(0, 0, 0.001), new Vector3d(1, 0, 0), new Vector3d(0, 1, 0));
            _inertia = new Vector3d(0, 0, 0);

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the Robot Tool class from planes.
        /// Sets the load data as defined for the default tool tool0.
        /// </summary>
        /// <param name="name"> The tool name, must be unique. </param>
        /// <param name="mesh"> The tool mesh. </param>
        /// <param name="attachmentPlane"> The attachement plane. </param>
        /// <param name="toolPlane"> The tool center point and tool orientation as a plane. </param>
        public RobotTool(string name, Mesh mesh, Plane attachmentPlane, Plane toolPlane)
        {
            _referenceType = ReferenceType.PERS;
            _name = name;
            _mesh = mesh;
            _attachmentPlane = attachmentPlane;
            _toolPlane = toolPlane;

            _robotHold = true;
            _mass = 0.001;
            _centerOfGravity = new Plane(new Point3d(0, 0, 0.001), new Vector3d(1, 0, 0), new Vector3d(0, 1, 0));
            _inertia = new Vector3d(0, 0, 0);

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the Robot Tool class from planes.
        /// Sets the load data as defined for the default tool tool0.
        /// </summary>
        /// <param name="name"> The tool name, must be unique. </param>
        /// <param name="meshes"> The tool mesh as The list with robot meshes. </param>
        /// <param name="attachmentPlane"> The attachement plane. </param>
        /// <param name="toolPlane"> The tool center point and tool orientation as a plane. </param>
        public RobotTool(string name, List<Mesh> meshes, Plane attachmentPlane, Plane toolPlane)
        {
            _referenceType = ReferenceType.PERS;
            _name = name;
            _mesh = new Mesh();
            for (int i = 0; i < meshes.Count; i++) { _mesh.Append(meshes[i]); }
            _attachmentPlane = attachmentPlane;
            _toolPlane = toolPlane;

            _robotHold = true;
            _mass = 0.001;
            _centerOfGravity = new Plane(new Point3d(0, 0, 0.001), new Vector3d(1, 0, 0), new Vector3d(0, 1, 0));
            _inertia = new Vector3d(0, 0, 0);

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the Robot Tool class from Euler data.
        /// Sets the attachtment plane equal to the world xy-plane. 
        /// Sets the load data as defined for the default tool tool0.
        /// </summary>
        /// <param name="name"> The tool name, must be unique. </param>
        /// <param name="mesh"> The tool mesh. </param>
        /// <param name="toolTransX"> The tool center point translation in x-direction. </param>
        /// <param name="toolTransY"> The tool center point translation in y-direction. </param>
        /// <param name="toolTransZ"> The tool center point translation in z-direction. </param>
        /// <param name="toolRotX"> The orientation around the x-axis in radians. </param>
        /// <param name="toolRotY"> The orientation around the y-axis in radians. </param>
        /// <param name="toolRotZ"> The orientation around the y-axis in radians. </param>
        public RobotTool(string name, Mesh mesh, double toolTransX, double toolTransY, 
            double toolTransZ, double toolRotX, double toolRotY, double toolRotZ)
        {
            _referenceType = ReferenceType.PERS;
            _name = name;
            _mesh = mesh;
            _attachmentPlane = Plane.WorldXY;
            _toolPlane = Plane.WorldXY;

            _toolPlane.Translate(new Vector3d(toolTransX, toolTransY, toolTransZ));
            _toolPlane.Transform(Rhino.Geometry.Transform.Rotation(toolRotX, new Vector3d(1, 0, 0), _toolPlane.Origin));
            _toolPlane.Transform(Rhino.Geometry.Transform.Rotation(toolRotY, new Vector3d(0, 1, 0), _toolPlane.Origin));
            _toolPlane.Transform(Rhino.Geometry.Transform.Rotation(toolRotZ, new Vector3d(0, 0, 1), _toolPlane.Origin));

            _robotHold = true;
            _mass = 0.001;
            _centerOfGravity = new Plane(new Point3d(0, 0, 0.001), new Vector3d(1, 0, 0), new Vector3d(0, 1, 0));
            _inertia = new Vector3d(0, 0, 0);

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the Robot Tool class from Euler data.
        /// Sets the attachtment plane equal to the world xy-plane. 
        /// Sets the load data as defined for the default tool tool0.
        /// </summary>
        /// <param name="name"> The tool name, must be unique. </param>
        /// <param name="meshes"> The tool mesh. </param>
        /// <param name="toolTransX"> The tool center point translation in x-direction. </param>
        /// <param name="toolTransY"> The tool center point translation in y-direction. </param>
        /// <param name="toolTransZ"> The tool center point translation in z-direction. </param>
        /// <param name="toolRotX"> The orientation around the x-axis in radians. </param>
        /// <param name="toolRotY"> The orientation around the y-axis in radians. </param>
        /// <param name="toolRotZ"> The orientation around the y-axis in radians. </param>
        public RobotTool(string name, List<Mesh> meshes, double toolTransX, double toolTransY,
            double toolTransZ, double toolRotX, double toolRotY, double toolRotZ)
        {
            _referenceType = ReferenceType.PERS;
            _name = name;
            _mesh = new Mesh();
            for (int i = 0; i < meshes.Count; i++) { _mesh.Append(meshes[i]); }
            _attachmentPlane = Plane.WorldXY;
            _toolPlane = Plane.WorldXY;

            _toolPlane.Translate(new Vector3d(toolTransX, toolTransY, toolTransZ));
            _toolPlane.Transform(Rhino.Geometry.Transform.Rotation(toolRotX, new Vector3d(1, 0, 0), _toolPlane.Origin));
            _toolPlane.Transform(Rhino.Geometry.Transform.Rotation(toolRotY, new Vector3d(0, 1, 0), _toolPlane.Origin));
            _toolPlane.Transform(Rhino.Geometry.Transform.Rotation(toolRotZ, new Vector3d(0, 0, 1), _toolPlane.Origin));

            _robotHold = true;
            _mass = 0.001;
            _centerOfGravity = new Plane(new Point3d(0, 0, 0.001), new Vector3d(1, 0, 0), new Vector3d(0, 1, 0));
            _inertia = new Vector3d(0, 0, 0);

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the Robot Tool class from the x, y and z coordinate and the four quarternion values of the TCP point. 
        /// Sets the attachtment plane equal to the world xy-plane. 
        /// Sets the load data as defined for the default tool tool0.
        /// </summary>
        /// <param name="name"> The tool name, must be unique. </param>
        /// <param name="mesh"> The tool mesh. </param>
        /// <param name="x"> The x coordinate of the TCP point. </param>
        /// <param name="y"> The y coordinate of the TCP point. </param>
        /// <param name="z"> The z coordinate of the TCP point.</param>
        /// <param name="q1"> The real part of the quaternion. </param>
        /// <param name="q2"> The first imaginary coefficient of the quaternion. </param>
        /// <param name="q3"> The second imaginary coefficient of the quaternion. </param>
        /// <param name="q4"> The third imaginary coefficient of the quaternion. </param>
        public RobotTool(string name, Mesh mesh, double x, double y,
            double z, double q1, double q2, double q3, double q4)
        {
            _referenceType = ReferenceType.PERS;
            _name = name;
            _mesh = mesh;
            _attachmentPlane = Plane.WorldXY;

            _robotHold = true;
            _mass = 0.001;
            _centerOfGravity = new Plane(new Point3d(0, 0, 0.001), new Vector3d(1, 0, 0), new Vector3d(0, 1, 0));
            _inertia = new Vector3d(0, 0, 0);

            _toolPlane = HelperMethods.QuaternionToPlane(x, y, z, q1, q2, q3, q4);

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the Robot Tool class from the x, y and z coordinate and the four quarternion values of the TCP point. 
        /// Sets the attachtment plane equal to the world xy-plane. 
        /// Sets the load data as defined for the default tool tool0.
        /// </summary>
        /// <param name="name"> The tool name, must be unique. </param>
        /// <param name="meshes"> The tool mesh. </param>
        /// <param name="x"> The x coordinate of the TCP point. </param>
        /// <param name="y"> The y coordinate of the TCP point. </param>
        /// <param name="z"> The z coordinate of the TCP point.</param>
        /// <param name="q1"> The real part of the quaternion. </param>
        /// <param name="q2"> The first imaginary coefficient of the quaternion. </param>
        /// <param name="q3"> The second imaginary coefficient of the quaternion. </param>
        /// <param name="q4"> The third imaginary coefficient of the quaternion. </param>
        public RobotTool(string name, List<Mesh> meshes, double x, double y,
            double z, double q1, double q2, double q3, double q4)
        {
            _referenceType = ReferenceType.PERS;
            _name = name;
            _mesh = new Mesh();
            for (int i = 0; i < meshes.Count; i++) { _mesh.Append(meshes[i]); }
            _attachmentPlane = Plane.WorldXY;

            _robotHold = true;
            _mass = 0.001;
            _centerOfGravity = new Plane(new Point3d(0, 0, 0.001), new Vector3d(1, 0, 0), new Vector3d(0, 1, 0));
            _inertia = new Vector3d(0, 0, 0);

            _toolPlane = HelperMethods.QuaternionToPlane(x, y, z, q1, q2, q3, q4);

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the Robot Tool class from the x, y and z coordinate and the four quarternion values of the TCP point. 
        /// Sets the load data as defined for the default tool tool0.
        /// </summary>
        /// <param name="name"> The tool name, must be unique. </param>
        /// <param name="meshes"> The tool mesh. </param>
        /// <param name="attachmentPlane"> The attachement plane. </param>
        /// <param name="x"> The x coordinate of the TCP point. </param>
        /// <param name="y"> The y coordinate of the TCP point. </param>
        /// <param name="z"> The z coordinate of the TCP point.</param>
        /// <param name="q1"> The real part of the quaternion. </param>
        /// <param name="q2"> The first imaginary coefficient of the quaternion. </param>
        /// <param name="q3"> The second imaginary coefficient of the quaternion. </param>
        /// <param name="q4"> The third imaginary coefficient of the quaternion. </param>
        public RobotTool(string name, List<Mesh> meshes, Plane attachmentPlane, double x, 
            double y, double z, double q1, double q2, double q3, double q4)
        {
            _referenceType = ReferenceType.PERS;
            _name = name;
            _mesh = new Mesh();
            for (int i = 0; i < meshes.Count; i++) { _mesh.Append(meshes[i]); }
            _attachmentPlane = attachmentPlane;

            _robotHold = true;
            _mass = 0.001;
            _centerOfGravity = new Plane(new Point3d(0, 0, 0.001), new Vector3d(1, 0, 0), new Vector3d(0, 1, 0));
            _inertia = new Vector3d(0, 0, 0);

            _toolPlane = HelperMethods.QuaternionToPlane(_attachmentPlane, x, y, z, q1, q2, q3, q4);

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the Robot Tool class from a point and a quarternion.
        /// Sets the attachtment plane equal to the world xy-plane. 
        /// Sets the load data as defined for the default tool tool0.
        /// </summary>
        /// <param name="name"> The tool name, must be unique. </param>
        /// <param name="mesh"> The tool mesh defined in the tool coordinate space. </param>
        /// <param name="point"> The point of the TCP point. </param>
        /// <param name="quat"> The orientation of TCP point. </param>
        public RobotTool(string name, Mesh mesh, Point3d point, Quaternion quat)
        {
            _referenceType = ReferenceType.PERS;
            _name = name;
            _mesh = mesh;
            _attachmentPlane = Plane.WorldXY;

            _robotHold = true;
            _mass = 0.001;
            _centerOfGravity = new Plane(new Point3d(0, 0, 0.001), new Vector3d(1, 0, 0), new Vector3d(0, 1, 0));
            _inertia = new Vector3d(0, 0, 0);

            _toolPlane = HelperMethods.QuaternionToPlane(_attachmentPlane, point, quat);

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the Robot Tool class.
        /// </summary>
        /// <param name="name"> The tool name, must be unique. </param>
        /// <param name="mesh"> The tool mesh. </param>
        /// <param name="attachmentPlane"> The attachement plane. </param>
        /// <param name="toolPlane"> The tool center point and tool orientation. </param>
        /// <param name="robotHold"> Specifies whether the robot is holding the tool. </param>
        /// <param name="mass"> The weight of the tool in kg. </param>
        /// <param name="centerOfGravityPosition"> The position of the  center of gravity of the tool load. </param>
        /// <param name="centerOfGravityOrientation"> The orientation of the tool load coordinate system defined by the principal inertial axes of the 
        /// tool load. Expressed in the wrist coordinate system as a quaternion (q1, q2, q3, q4). </param>
        /// <param name="inertia"> The moment of inertia of the load in kgm2. </param>
        public RobotTool(string name, Mesh mesh, Plane attachmentPlane, Plane toolPlane, bool robotHold, 
            double mass, Point3d centerOfGravityPosition, Quaternion centerOfGravityOrientation, Vector3d inertia)
        {
            _referenceType = ReferenceType.PERS;
            _name = name;
            _mesh = mesh;
            _attachmentPlane = attachmentPlane;
            _toolPlane = toolPlane;

            _robotHold = robotHold;
            _mass = mass;
            _centerOfGravityPosition = centerOfGravityPosition;
            _centerOfGravityOrientation = centerOfGravityOrientation;
            _inertia = inertia;

            _centerOfGravity = HelperMethods.QuaternionToPlane(_attachmentPlane, _centerOfGravityPosition, _centerOfGravityOrientation);

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the Robot Tool class.
        /// </summary>
        /// <param name="name"> The tool name, must be unique. </param>
        /// <param name="meshes"> The tool mesh. </param>
        /// <param name="attachmentPlane"> The attachement plane. </param>
        /// <param name="toolPlane"> The tool center point and tool orientation. </param>
        /// <param name="robotHold"> Specifies whether the robot is holding the tool. </param>
        /// <param name="mass"> The weight of the tool in kg. </param>
        /// <param name="centerOfGravityPosition"> The position of the center of gravity of the tool load. </param>
        /// <param name="centerOfGravityOrientation"> The orientation of the tool load coordinate system defined by the principal inertial axes of the 
        /// tool load. Expressed in the wrist coordinate system as a quaternion (q1, q2, q3, q4). </param>
        /// <param name="inertia"> The moment of inertia of the load in kgm2. </param>
        public RobotTool(string name, List<Mesh> meshes, Plane attachmentPlane, Plane toolPlane, bool robotHold,
            double mass, Point3d centerOfGravityPosition, Quaternion centerOfGravityOrientation, Vector3d inertia)
        {
            _referenceType = ReferenceType.PERS;
            _name = name;
            _mesh = new Mesh();
            for (int i = 0; i < meshes.Count; i++) { _mesh.Append(meshes[i]); }
            _attachmentPlane = attachmentPlane;
            _toolPlane = toolPlane;

            _robotHold = robotHold;
            _mass = mass;
            _centerOfGravityPosition = centerOfGravityPosition;
            _centerOfGravityOrientation = centerOfGravityOrientation;
            _inertia = inertia;

            _centerOfGravity = HelperMethods.QuaternionToPlane(_attachmentPlane, _centerOfGravityPosition, _centerOfGravityOrientation);

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the Robot Tool class.
        /// </summary>
        /// <param name="name"> The tool name, must be unique. </param>
        /// <param name="mesh"> The tool mesh. </param>
        /// <param name="attachmentPlane"> The attachement plane. </param>
        /// <param name="toolPlane"> The tool center point and tool orientation. </param>
        /// <param name="robotHold"> Specifies whether the robot is holding the tool. </param>
        /// <param name="mass"> The weight of the tool in kg. </param>
        /// <param name="centerOfGravity"> The position and orientation of the center of gravity of the tool load. </param>
        /// <param name="inertia"> The moment of inertia of the load in kgm2. </param>
        public RobotTool(string name, Mesh mesh, Plane attachmentPlane, Plane toolPlane, bool robotHold, double mass, Plane centerOfGravity, Vector3d inertia)
        {
            _referenceType = ReferenceType.PERS;
            _name = name;
            _mesh = mesh;
            _attachmentPlane = attachmentPlane;
            _toolPlane = toolPlane;

            _robotHold = robotHold;
            _mass = mass;
            _centerOfGravity = centerOfGravity;
            _inertia = inertia;

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the Robot Tool class.
        /// </summary>
        /// <param name="name"> The tool name, must be unique. </param>
        /// <param name="meshes"> The tool mesh. </param>
        /// <param name="attachmentPlane"> The attachement plane. </param>
        /// <param name="toolPlane"> The tool center point and tool orientation. </param>
        /// <param name="robotHold"> Specifies whether the robot is holding the tool. </param>
        /// <param name="mass"> The weight of the tool in kg. </param>
        /// <param name="centerOfGravity"> The position and orientation of the center of gravity of the tool load. </param>
        /// <param name="inertia"> The moment of inertia of the load in kgm2. </param>
        public RobotTool(string name, List<Mesh> meshes, Plane attachmentPlane, Plane toolPlane, bool robotHold, double mass, Plane centerOfGravity, Vector3d inertia)
        {
            _referenceType = ReferenceType.PERS;
            _name = name;
            _mesh = new Mesh();
            for (int i = 0; i < meshes.Count; i++) { _mesh.Append(meshes[i]); }
            _attachmentPlane = attachmentPlane;
            _toolPlane = toolPlane;

            _robotHold = robotHold;
            _mass = mass;
            _centerOfGravity = centerOfGravity;
            _inertia = inertia;

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the Robot Tool class by duplicating an existing Robot Tool instance. 
        /// </summary>
        /// <param name="robotTool"> The Robot Tool instance to duplicate. </param>
        /// <param name="duplicateMesh"> Specifies whether the meshes should be duplicated. </param>
        public RobotTool(RobotTool robotTool, bool duplicateMesh = true)
        {
            _referenceType = robotTool.ReferenceType;
            _name = robotTool.Name;
            _attachmentPlane = new Plane(robotTool.AttachmentPlane);
            _toolPlane = new Plane(robotTool.ToolPlane);

            _robotHold = robotTool.RobotHold;
            _mass = robotTool.Mass;
            _centerOfGravity = new Plane(robotTool.CenterOfGravity);
            _centerOfGravityPosition = new Point3d(robotTool.CenterOfGravityPosition);
            _centerOfGravityOrientation = robotTool.CenterOfGravityOrientation;
            _inertia = new Vector3d(robotTool.Inertia);

            _position = new Point3d(robotTool.Position);
            _orientation = robotTool.Orientation;

            if (duplicateMesh == true) { _mesh = robotTool.Mesh.DuplicateMesh(); }
            else { }//_mesh = new Mesh(); }

        }

        /// <summary>
        /// Returns an exact duplicate of this Robot Tool instance.
        /// </summary>
        /// <returns> A deep copy of the Robot Tool instance. </returns>
        public RobotTool Duplicate()
        {
            return new RobotTool(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Robot Tool instance without meshes.
        /// </summary>
        /// <returns> A deep copy of the Robot Tool instance without meshes. </returns>
        public RobotTool DuplicateWithoutMesh()
        {
            return new RobotTool(this, false);
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
                return "Invalid Robot Tool";
            }
            else
            {
                return "Robot Tool (" + this.Name + ")";
            }
        }

        /// <summary>
        /// Initializes the fields and properties to construct a valid Robot Tool istance.
        /// </summary>
        private void Initialize()
        {
            CalculateToolPosition();
            CalculateToolOrientation();
            CalculateCenterOfGravityPosition();
            CalculateCenterOfGravityOrientation();
        }

        /// <summary>
        /// Reinitializes the fields and properties to construct a valid Robot Tool instance. 
        /// </summary>
        public void ReInitialize()
        {
            Initialize();
        }

        /// <summary>
        /// Calculates and returns the tool center point relative to the defined attachment plane. 
        /// </summary>
        /// <returns> The tool center point. </returns>
        public Point3d CalculateToolPosition()
        {
            Plane toolPlane = new Plane(_toolPlane);
            Transform orient = Rhino.Geometry.Transform.PlaneToPlane(_attachmentPlane, Plane.WorldXY);
            toolPlane.Transform(orient);

            _position = new Point3d(toolPlane.Origin);
            
            return _position;
        }

        /// <summary>
        /// Calculates and returns the tool center orientation relative to the defined attachment plane. 
        /// </summary>
        /// <returns> The quaternion orientation of the tool center plane. </returns>
        public Quaternion CalculateToolOrientation()
        {
            _orientation = HelperMethods.PlaneToQuaternion(_attachmentPlane, _toolPlane);

            return _orientation;
        }

        /// <summary>
        /// Calculates and returns the tool center of gravity relative to the defined attachment plane. 
        /// </summary>
        /// <returns> The center of gravity point. </returns>
        public Point3d CalculateCenterOfGravityPosition()
        {
            Plane centerOfGravity = new Plane(_centerOfGravity);
            Transform orient = Rhino.Geometry.Transform.PlaneToPlane(_attachmentPlane, Plane.WorldXY);
            centerOfGravity.Transform(orient);

            _centerOfGravityPosition = new Point3d(centerOfGravity.Origin);

            return _centerOfGravityPosition;
        }

        /// <summary>
        /// Calculates and returns the tool center of gravity orientation relative to the defined attachment plane. 
        /// </summary>
        /// <returns> The quaternion orientation of the tool center of gravity. </returns>
        public Quaternion CalculateCenterOfGravityOrientation()
        {
            _centerOfGravityOrientation = HelperMethods.PlaneToQuaternion(_attachmentPlane, _centerOfGravity);

            return _centerOfGravityOrientation;
        }

        /// <summary>
        /// Returns the RAPID declaration code line of the this Robot Tool.
        /// </summary>
        /// <returns> The RAPID code line. </returns>
        public string ToRAPIDDeclaration()
        {
            // Add robot tool name
            string result = Enum.GetName(typeof(ReferenceType), _referenceType);
            result += " tooldata " + _name + " := ";

            // Add robot hold < robhold of bool >
            if (_robotHold)
            {
                result += "[TRUE, [[";
            }
            else
            {
                result += "[FALSE, [[";
            }

            // Add coordinate of toolframe < tframe of pose > < trans of pos >
            result += _position.X.ToString("0.###") 
                + ", " + _position.Y.ToString("0.###") 
                + ", " + _position.Z.ToString("0.###") + "], [";

            // Add orientation of tool frame < tframe of pose > < rot of orient >
            result += _orientation.A.ToString("0.######") 
                + ", " + _orientation.B.ToString("0.######") 
                + ", " + _orientation.C.ToString("0.######") 
                + ", " + _orientation.D.ToString("0.######") + "]], [";

            // Add tool load < tload of loaddata >
            result += _mass.ToString("0.######") + ", [" 

                + _centerOfGravityPosition.X.ToString("0.######") + ", " 
                + _centerOfGravityPosition.Y.ToString("0.######") + ", " 
                + _centerOfGravityPosition.Z.ToString("0.######") + "], ["

                + _centerOfGravityOrientation.A.ToString("0.######") + ", " 
                + _centerOfGravityOrientation.B.ToString("0.######") + ", " 
                + _centerOfGravityOrientation.C.ToString("0.######") + ", " 
                + _centerOfGravityOrientation.D.ToString("0.######") + "], " 

                + _inertia.X.ToString("0.######") + ", " 
                + _inertia.Y.ToString("0.######") + ", " 
                + _inertia.Z.ToString("0.######") + "]];";

            return result;
        }

        /// <summary>
        /// Clears all the fields and properties of the current instance.
        /// Typically used for defining an empty Robot Tool instance 
        /// (since the empty constructor creates the default Robot Tool tool0).
        /// </summary>
        public void Clear()
        {
            _name = "";
            _mesh = new Mesh();
            _attachmentPlane = Plane.Unset;
            _toolPlane = Plane.Unset;
            _robotHold = false;
            _position = Point3d.Unset;
            _orientation = Quaternion.Zero;
            _mass = 0;
            _centerOfGravity = Plane.Unset;
            _centerOfGravityPosition = Point3d.Unset;
            _centerOfGravityOrientation = Quaternion.Zero;
            _inertia = Vector3d.Unset;
        }

        /// <summary>
        /// Transforms the Robot Tool spatial properties (planes and meshes). 
        /// </summary>
        /// <param name="xform"> The spatial deform. </param>
        public void Transform(Transform xform)
        {
            _mesh.Transform(xform);
            _attachmentPlane.Transform(xform);
            _toolPlane.Transform(xform);
            _centerOfGravity.Transform(xform);

            ReInitialize();
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
                if (Name == null) { return false; }
                if (Name == "") { return false; }
                if (AttachmentPlane == null) { return false; }
                if (AttachmentPlane == Plane.Unset) { return false; }
                if (ToolPlane == null) { return false; }
                if (ToolPlane == Plane.Unset) { return false; }
                if (CenterOfGravity == null) { return false; }
                if (CenterOfGravity == Plane.Unset) { return false; }
                if (Mass < 0.0) { return false; }
                return true;
            }
        }

        /// <summary>
        /// Gets or sets the Reference Type. 
        /// </summary>
        public ReferenceType ReferenceType
        {
            get { return _referenceType; }
            set { _referenceType = value; }
        }

        /// <summary>
        /// Gets or sets the tool name.
        /// Each tool name has to be unique.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets or sets the mesh.
        /// </summary>
        public Mesh Mesh
        {
            get { return _mesh; }
            set { _mesh = value; }
        }

        /// <summary>
        /// Gets or sets the tool attachment plane.
        /// </summary>
        public Plane AttachmentPlane
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
        /// Gets or sets the position and orientaton of the tool center point. 
        /// </summary>
        public Plane ToolPlane
        {
            get 
            {
                return _toolPlane; 
            }
            set 
            { 
                _toolPlane = value;
                ReInitialize();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the robot is holding the tool.
        /// Use true if the robot is holding the tool. 
        /// Use false if the robot is not holding the tool (e.g. stationary tool).
        /// </summary>
        public bool RobotHold
        {
            get { return _robotHold; }
            set { _robotHold = value; }
        }

        /// <summary>
        /// Gets the position of the the tool center point which is the offset between the tool center plane and the attachment plane.
        /// </summary>
        public Point3d Position
        {
            get { return _position; }
        }

        /// <summary>
        /// Gets the orientation of the tool center point as a Quaternion.
        /// </summary>
        public Quaternion Orientation
        {
            get { return _orientation; }
        }

        /// <summary>
        /// Gets or sets the weight of the load in kg.
        /// </summary>
        public double Mass
        {
            get { return _mass; }
            set { _mass = value; }
        }

        /// <summary>
        /// Gets or sets the position and orientation of the center of gravity of the tool load as a plane.
        /// </summary>
        public Plane CenterOfGravity
        {
            get 
            { 
                return _centerOfGravity; 
            }
            set 
            { 
                _centerOfGravity = value;
                ReInitialize();
            }
        }

        /// <summary>
        /// Gets the position of the center of gravity of the tool load.
        /// </summary>
        public Point3d CenterOfGravityPosition
        {
            get { return _centerOfGravityPosition; }
        }

        /// <summary>
        /// Gets the orientation of the tool load coordinate system defined by the principal inertial axes of the tool load. 
        /// Expressed in the wrist coordinate system as a quaternion (q1, q2, q3, q4).
        /// </summary>
        public Quaternion CenterOfGravityOrientation
        {
            get { return _centerOfGravityOrientation; }
        }

        /// <summary>
        /// Gets or set the moment of inertia of the load in kgm2.
        /// </summary>
        public Vector3d Inertia
        {
            get { return _inertia; }
            set { _inertia = value; }
        }
        #endregion
    }

}

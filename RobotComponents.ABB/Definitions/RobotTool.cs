// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.ABB.Enumerations;
using RobotComponents.ABB.Utils;

namespace RobotComponents.ABB.Definitions
{
    /// <summary>
    /// Represents a Robot Tool.
    /// </summary>
    [Serializable()]
    public class RobotTool : ISerializable
    {
        #region fields
        private Scope _scope;
        private VariableType _variableType; // variable type
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
            int version = (int)info.GetValue("Version", typeof(int)); // <-- use this if the (de)serialization changes
            _scope = version >= 2000000 ? (Scope)info.GetValue("Scope", typeof(Scope)) : Scope.GLOBAL;
            _variableType = version >= 2000000 ? (VariableType)info.GetValue("Variable Type", typeof(VariableType)) : (VariableType)info.GetValue("Reference Type", typeof(VariableType));
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
            info.AddValue("Scope", _scope, typeof(Scope));
            info.AddValue("Variable Type", _variableType, typeof(VariableType));
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
            _scope = Scope.GLOBAL;
            _variableType = VariableType.PERS;
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
            _scope = Scope.GLOBAL;
            _variableType = VariableType.PERS;
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
        public RobotTool(string name, IList<Mesh> meshes, Plane attachmentPlane, Plane toolPlane)
        {
            _scope = Scope.GLOBAL;
            _variableType = VariableType.PERS;
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
            _scope = Scope.GLOBAL;
            _variableType = VariableType.PERS;
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
        public RobotTool(string name, IList<Mesh> meshes, double toolTransX, double toolTransY,
            double toolTransZ, double toolRotX, double toolRotY, double toolRotZ)
        {
            _scope = Scope.GLOBAL;
            _variableType = VariableType.PERS;
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
            _scope = Scope.GLOBAL;
            _variableType = VariableType.PERS;
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
        public RobotTool(string name, IList<Mesh> meshes, double x, double y,
            double z, double q1, double q2, double q3, double q4)
        {
            _scope = Scope.GLOBAL;
            _variableType = VariableType.PERS;
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
        public RobotTool(string name, IList<Mesh> meshes, Plane attachmentPlane, double x, 
            double y, double z, double q1, double q2, double q3, double q4)
        {
            _scope = Scope.GLOBAL;
            _variableType = VariableType.PERS;
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
            _scope = Scope.GLOBAL;
            _variableType = VariableType.PERS;
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
            _scope = Scope.GLOBAL;
            _variableType = VariableType.PERS;
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
        public RobotTool(string name, IList<Mesh> meshes, Plane attachmentPlane, Plane toolPlane, bool robotHold,
            double mass, Point3d centerOfGravityPosition, Quaternion centerOfGravityOrientation, Vector3d inertia)
        {
            _scope = Scope.GLOBAL;
            _variableType = VariableType.PERS;
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
            _scope = Scope.GLOBAL;
            _variableType = VariableType.PERS;
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
        public RobotTool(string name, IList<Mesh> meshes, Plane attachmentPlane, Plane toolPlane, bool robotHold, double mass, Plane centerOfGravity, Vector3d inertia)
        {
            _scope = Scope.GLOBAL;
            _variableType = VariableType.PERS;
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
            _scope = robotTool.Scope;
            _variableType = robotTool.ReferenceType;
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

        /// <summary>
        /// Returns an empty Robot Tool instance.
        /// </summary>
        /// <returns> The empty Robot Tool. </returns>
        public static RobotTool GetEmptyRobotTool()
        {
            RobotTool robotTool = new RobotTool();
            robotTool.Clear();

            return robotTool;
        }
        #endregion

        #region parse
        /// <summary>
        /// Initializes a new instance of the Robot Tool class from a rapid data string.
        /// </summary>
        /// <remarks>
        /// Only used for the Parse and TryParse methods. Therefore, this constructor is private. 
        /// </remarks>
        /// <param name="rapidData"></param>
        private RobotTool(string rapidData)
        {
            string clean = rapidData;
            clean = clean.Replace(" ", "");
            clean = clean.Replace("\t", "");
            clean = clean.Replace("\n", "");
            clean = clean.Replace(";", "");
            clean = clean.Replace(":", "");
            clean = clean.Replace("[", "");
            clean = clean.Replace("]", "");
            clean = clean.Replace("(", "");
            clean = clean.Replace(")", "");
            clean = clean.Replace("{", "");
            clean = clean.Replace("}", "");

            string[] split = clean.Split('=');
            string type;
            string value;

            if (split.Length == 1)
            {
                type = "PERStooldata"; // default: GLOBAL scope and PERS variable type
                value = split[0];
            }
            else if (split.Length == 2)
            {
                type = split[0];
                value = split[1];
            }
            else
            {
                throw new InvalidCastException("Invalid RAPID data string: More than one equal sign defined.");
            }

            // Scope
            if (type.StartsWith("LOCAL"))
            {
                _scope = Scope.LOCAL;
                type = type.ReplaceFirst("LOCAL", "");
            }
            else if (type.StartsWith("TASK"))
            {
                _scope = Scope.TASK;
                type = type.ReplaceFirst("TASK", "");
            }
            else
            {
                _scope = Scope.GLOBAL;
            }

            // Variable type
            if (type.StartsWith("VAR"))
            {
                _variableType = VariableType.VAR;
                type = type.ReplaceFirst("VAR", "");
            }
            else if (type.StartsWith("CONST"))
            {
                _variableType = VariableType.CONST;
                type = type.ReplaceFirst("CONST", "");
            }
            else if (type.StartsWith("PERS"))
            {
                _variableType = VariableType.PERS;
                type = type.ReplaceFirst("PERS", "");
            }
            else
            {
                throw new InvalidCastException("Invalid RAPID data string: The scope or variable type is incorrect.");
            }

            // Datatype
            if (type.StartsWith("tooldata") == false)
            {
                throw new InvalidCastException("Invalid RAPID data string: The datatype does not match.");
            }

            type = type.ReplaceFirst("tooldata", "");

            // Name
            _name = type;

            // Value
            string[] values = value.Split(',');

            if (values.Length == 19)
            {
                _robotHold = values[0] == "TRUE" ? true : false;

                _position = new Point3d();
                _position.X = Convert.ToDouble(values[1]);
                _position.X = Convert.ToDouble(values[2]);
                _position.X = Convert.ToDouble(values[3]);

                _orientation = new Quaternion();
                _orientation.A = Convert.ToDouble(values[4]);
                _orientation.B = Convert.ToDouble(values[5]);
                _orientation.C = Convert.ToDouble(values[6]);
                _orientation.D = Convert.ToDouble(values[7]);

                _mass = Convert.ToDouble(values[8]);

                _centerOfGravityPosition = new Point3d();
                _centerOfGravityPosition.X = Convert.ToDouble(values[9]);
                _centerOfGravityPosition.Y = Convert.ToDouble(values[10]);
                _centerOfGravityPosition.Z = Convert.ToDouble(values[11]);

                _centerOfGravityOrientation = new Quaternion();
                _centerOfGravityOrientation.A = Convert.ToDouble(values[12]);
                _centerOfGravityOrientation.B = Convert.ToDouble(values[13]);
                _centerOfGravityOrientation.C = Convert.ToDouble(values[14]);
                _centerOfGravityOrientation.D = Convert.ToDouble(values[15]);

                _inertia = new Vector3d();
                _inertia.X = Convert.ToDouble(values[16]);
                _inertia.Y = Convert.ToDouble(values[17]);
                _inertia.Z = Convert.ToDouble(values[18]);
            }
            else
            {
                throw new InvalidCastException("Invalid RAPID data string: The number of values does not match.");
            }

            _mesh = new Mesh();
            _attachmentPlane = Plane.WorldXY;
            _toolPlane = HelperMethods.QuaternionToPlane(_attachmentPlane, _position, _orientation);
        }

        /// <summary>
        /// Returns a Robot Tool instance constructed from a RAPID data string. 
        /// </summary>
        /// <param name="rapidData"> The RAPID data string. s</param>
        public static RobotTool Parse(string rapidData)
        {
            return new RobotTool(rapidData);
        }

        /// <summary>
        /// Attempts to parse a RAPID data string into a Robot Tool instance.  
        /// </summary>
        /// <param name="rapidData"> The RAPID data string. </param>
        /// <param name="robotTool"> The Robot Tool intance. </param>
        /// <returns> True on success, false on failure. </returns>
        public static bool TryParse(string rapidData, out RobotTool robotTool)
        {
            try
            {
                robotTool = new RobotTool(rapidData);
                return true;
            }
            catch
            {
                robotTool = new RobotTool();
                return false;
            }
        }
        #endregion

        #region methods
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns> A string that represents the current object. </returns>
        public override string ToString()
        {
            if (!IsValid)
            {
                return "Invalid Robot Tool";
            }
            else
            {
                return $"Robot Tool ({_name})";
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
            // Scope
            string result = _scope == Scope.GLOBAL ? "" : $"{Enum.GetName(typeof(Scope), _scope)} ";

            // Adds variable type
            result += Enum.GetName(typeof(VariableType), _variableType);

            // Add robot tool name
            result += " tooldata " + _name + " := ";

            // Add robot hold < robhold of bool >
            result += _robotHold ? "[TRUE, [[" : "[FALSE, [[";

            // Add coordinate of toolframe < tframe of pose > < trans of pos >
            result += $"{_position.X:0.###}, {_position.Y:0.###}, {_position.Z:0.###}], [";
            
            // Add orientation of tool frame < tframe of pose > < rot of orient >
            result += $"{_orientation.A:0.######}, {_orientation.B:0.######}, " +
                $"{_orientation.C:0.######}, {_orientation.D:0.######}]], [";

            // Add tool load < tload of loaddata >
            result += $"{_mass:0.######}, [";
            result += $"{_centerOfGravityPosition.X:0.######}, {_centerOfGravityPosition.Y:0.######}, {_centerOfGravityPosition.Z:0.######}], [";
            result += $"{_centerOfGravityOrientation.A:0.######}, {_centerOfGravityOrientation.B:0.######}, ";
            result += $"{_centerOfGravityOrientation.C:0.######}, {_centerOfGravityOrientation.D:0.######}], "; 
            result += $"{_inertia.X:0.######}, {_inertia.Y:0.######}, {_inertia.Z:0.######}" + "]];";

            return result;
        }

        /// <summary>
        /// Clears all the fields and properties of the current instance.
        /// Typically used for defining an empty Robot Tool instance 
        /// (since the empty constructor creates the default Robot Tool tool0).
        /// </summary>
        private void Clear()
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

        /// <summary>
        /// Returns the Bounding Box of the object.
        /// </summary>
        /// <param name="accurate"> If true, a physically accurate bounding box will be computed. If not, a bounding box estimate will be computed. </param>
        /// <returns> The Bounding Box. </returns>
        public BoundingBox GetBoundingBox(bool accurate)
        {
            {
                BoundingBox boundingBox = BoundingBox.Empty;

                if (_mesh != null)
                {
                    boundingBox.Union(_mesh.GetBoundingBox(accurate));
                }

                return boundingBox;
            }
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
                if (_name == null) { return false; }
                if (_name == "") { return false; }
                if (_attachmentPlane == null) { return false; }
                if (_attachmentPlane == Plane.Unset) { return false; }
                if (_toolPlane == null) { return false; }
                if (_toolPlane == Plane.Unset) { return false; }
                if (_centerOfGravity == null) { return false; }
                if (_centerOfGravity == Plane.Unset) { return false; }
                if (_mass < 0.0) { return false; }
                return true;
            }
        }

        /// <summary>
        /// Gets or sets the scope. 
        /// </summary>
        public Scope Scope
        {
            get { return _scope; }
            set { _scope = value; }
        }

        /// <summary>
        /// Gets or sets the variable type. 
        /// </summary>
        public VariableType ReferenceType
        {
            get { return _variableType; }
            set { _variableType = value; }
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

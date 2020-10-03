// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.Enumerations;
using RobotComponents.Utils;

namespace RobotComponents.Definitions
{
    /// <summary>
    /// RobotTool class, defines the basic properties and methods for any Robot Tool.
    /// </summary>
    public class RobotTool
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
        private Vector3d _centerOfGravity;
        private Quaternion _centerOfGravityOrientation;
        private Vector3d _inertia;
        #endregion

        #region constructors
        /// <summary>
        /// An empty constuctor that create the default robot tool tool0. 
        /// The tool tool0 defines the wrist coordinate system, with the 
        /// origin being the center of the mounting flange.
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
            _centerOfGravity = new Vector3d(0, 0, 0.001);
            _centerOfGravityOrientation = new Quaternion(1, 0, 0, 0);
            _inertia = new Vector3d(0, 0, 0);

            Initialize();
        }

        /// <summary>
        /// Defines a robot tool from planes with load data as defined for the default tool tool0.
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
            _centerOfGravity = new Vector3d(0, 0, 0.001);
            _centerOfGravityOrientation = new Quaternion(1, 0, 0, 0);
            _inertia = new Vector3d(0, 0, 0);

            Initialize();
        }

        /// <summary>
        /// Defines a robot tool from planes with load data as defined for the default tool tool0.
        /// </summary>
        /// <param name="name"> The tool name, must be unique. </param>
        /// <param name="meshes"> The tool mesh as a list with meshes. </param>
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
            _centerOfGravity = new Vector3d(0, 0, 0.001);
            _centerOfGravityOrientation = new Quaternion(1, 0, 0, 0);
            _inertia = new Vector3d(0, 0, 0);

            Initialize();
        }

        /// <summary>
        /// Defines a robot tool from Euler data with load data as defined for the default tool tool0.
        /// The attachment plane is set to WorldXY.
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
            _centerOfGravity = new Vector3d(0, 0, 0.001);
            _centerOfGravityOrientation = new Quaternion(1, 0, 0, 0);
            _inertia = new Vector3d(0, 0, 0);

            Initialize();
        }

        /// <summary>
        /// Defines a robot tool from Euler data with load data as defined for the default tool tool0.
        /// The attachment plane is set to WorldXY.
        /// </summary>
        /// <param name="name"> The tool name, must be unique. </param>
        /// <param name="meshes"> The tool mesh as a list with meshes. </param>
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
            _centerOfGravity = new Vector3d(0, 0, 0.001);
            _centerOfGravityOrientation = new Quaternion(1, 0, 0, 0);
            _inertia = new Vector3d(0, 0, 0);

            Initialize();
        }

        /// <summary>
        /// Defines a robot tool from the x, y and z coordinate and the four quarternion values. 
        /// With load data as defined for the default tool tool0.
        /// The attachment plane is set to WorldXY.
        /// </summary>
        /// <param name="name"> The tool name, must be unique. </param>
        /// <param name="mesh"> The tool mesh defined in the tool coordinate space. </param>
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
            _centerOfGravity = new Vector3d(0, 0, 0.001);
            _centerOfGravityOrientation = new Quaternion(1, 0, 0, 0);
            _inertia = new Vector3d(0, 0, 0);

            _toolPlane = HelperMethods.QuaternionToPlane(x, y, z, q1, q2, q3, q4);

            Initialize();
        }

        /// <summary>
        /// Defines a robot tool from the x, y and z coordinate and the four quarternion values. 
        /// With load data as defined for the default tool tool0.
        /// The attachment plane is set to WorldXY.
        /// </summary>
        /// <param name="name"> The tool name, must be unique. </param>
        /// <param name="meshes"> The tool mesh defined in the tool coordinate space as a list with meshes </param>
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
            _centerOfGravity = new Vector3d(0, 0, 0.001);
            _centerOfGravityOrientation = new Quaternion(1, 0, 0, 0);
            _inertia = new Vector3d(0, 0, 0);

            _toolPlane = HelperMethods.QuaternionToPlane(x, y, z, q1, q2, q3, q4);

            Initialize();
        }

        /// <summary>
        /// Defines a robot tool from the x, y and z coordinate and the four quarternion values. 
        /// With load data as defined for the default tool tool0.
        /// </summary>
        /// <param name="name"> The tool name, must be unique. </param>
        /// <param name="meshes"> The tool mesh defined in the tool coordinate space as a list with meshes </param>
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
            _centerOfGravity = new Vector3d(0, 0, 0.001);
            _centerOfGravityOrientation = new Quaternion(1, 0, 0, 0);
            _inertia = new Vector3d(0, 0, 0);

            _toolPlane = HelperMethods.QuaternionToPlane(_attachmentPlane, x, y, z, q1, q2, q3, q4);

            Initialize();
        }

        /// <summary>
        /// Defines a robot tool from the TCP point and the TCP orientation defined as a quatertion.
        /// With load data as defined for the default tool tool0. 
        /// The attachment plane is set to WorldXY.
        /// </summary>
        /// <param name="name"> The tool name, must be unique. </param>
        /// <param name="mesh"> The tool mesh defined in the tool coordinate space. </param>
        /// <param name="point"> The point of the tool center as point </param>
        /// <param name="quat"> The orientation of the tool center point as quarternion </param>
        public RobotTool(string name, Mesh mesh, Point3d point, Quaternion quat)
        {
            _referenceType = ReferenceType.PERS;
            _name = name;
            _mesh = mesh;
            _attachmentPlane = Plane.WorldXY;

            _robotHold = true;
            _mass = 0.001;
            _centerOfGravity = new Vector3d(0, 0, 0.001);
            _centerOfGravityOrientation = new Quaternion(1, 0, 0, 0);
            _inertia = new Vector3d(0, 0, 0);

            // Get tool plane from point and quarternion
            quat.GetRotation(out Plane plane);
            _toolPlane = new Plane(point, plane.XAxis, plane.YAxis);

            Initialize();
        }

        /// <summary>
        /// Defines a robot tool with user defined load data. 
        /// </summary>
        /// <param name="name"> The tool name, must be unique. </param>
        /// <param name="mesh"> The tool mesh. </param>
        /// <param name="attachmentPlane"> The attachement plane. </param>
        /// <param name="toolPlane"> The tool center point and tool orientation as a plane. </param>
        /// <param name="robotHold"> Boolean that indicates if the robot is holding the tool or if the tool is stationary. </param>
        /// <param name="mass"> The weight of the tool in kg. </param>
        /// <param name="centerOfGravity"> The center of gravity of the tool load. </param>
        /// <param name="centerOfGravityOrientation"> The orientation of the tool load coordinate system defined by the principal inertial axes of the 
        /// tool load. Expressed in the wrist coordinate system as a quaternion (q1, q2, q3, q4). </param>
        /// <param name="inertia"> The moment of inertia of the load in kgm2. </param>
        public RobotTool(string name, Mesh mesh, Plane attachmentPlane, Plane toolPlane, bool robotHold, 
            double mass, Vector3d centerOfGravity, Quaternion centerOfGravityOrientation, Vector3d inertia)
        {
            _referenceType = ReferenceType.PERS;
            _name = name;
            _mesh = mesh;
            _attachmentPlane = attachmentPlane;
            _toolPlane = toolPlane;

            _robotHold = robotHold;
            _mass = mass;
            _centerOfGravity = centerOfGravity;
            _centerOfGravityOrientation = centerOfGravityOrientation;
            _inertia = inertia;

            Initialize();
        }

        /// <summary>
        /// Creates a new robot tool by duplicating an existing robot tool.
        /// This creates a deep copy of the existing robot tool.
        /// </summary>
        /// <param name="robotTool"> The robot tool that should be duplicated. </param>
        /// <param name="duplicateMesh"> A boolean that indicates if the mesh should be duplicated. </param>
        public RobotTool(RobotTool robotTool, bool duplicateMesh = true)
        {
            _referenceType = robotTool.ReferenceType;
            _name = robotTool.Name;
            _attachmentPlane = new Plane(robotTool.AttachmentPlane);
            _toolPlane = new Plane(robotTool.ToolPlane);

            _robotHold = robotTool.RobotHold;
            _mass = robotTool.Mass;
            _centerOfGravity = new Vector3d(robotTool.CenterOfGravity);
            _centerOfGravityOrientation = robotTool.CenterOfGravityOrientation;
            _inertia = new Vector3d(robotTool.Inertia);

            _position = new Point3d(robotTool.Position);
            _orientation = robotTool.Orientation;

            if (duplicateMesh == true) { _mesh = robotTool.Mesh.DuplicateMesh(); }
            else { }//_mesh = new Mesh(); }

        }

        /// <summary>
        /// A method to duplicate the RobotTool object. 
        /// </summary>
        /// <returns> Returns a deep copy for the RobotTool object. </returns>
        public RobotTool Duplicate()
        {
            return new RobotTool(this);
        }

        /// <summary>
        /// A method to duplicate the RobotTool object without duplicating the mesh. It will set an empty mesh. 
        /// </summary>
        /// <returns> Returns a deep copy for the RobotTool object without a mesh. </returns>
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
        /// A method that calls all the other methods that are needed to initialize the data that is needed to construct a valid robot tool object. 
        /// </summary>
        private void Initialize()
        {
            GetToolPosition();
            GetToolOrientation();
        }

        /// <summary>
        /// A method that can be called to reinitialize all the data that is needed to construct a valid robot tool object. 
        /// </summary>
        public void ReInitialize()
        {
            Initialize();
        }

        /// <summary>
        /// Calculates the local tool center point relative to the defined attachment plane. 
        /// </summary>
        /// <returns> Returns the local tool center point coordinates. </returns>
        public Point3d GetToolPosition()
        {
            Plane toolPlane = new Plane(_toolPlane);
            Transform orient = Rhino.Geometry.Transform.PlaneToPlane(_attachmentPlane, Plane.WorldXY);
            toolPlane.Transform(orient);

            _position = new Point3d(toolPlane.Origin);
            
            return _position;
        }

        /// <summary>
        /// Calculates the tool center orientatin relative to the defined attachment plane. 
        /// </summary>
        /// <returns> Returns the quaternion orientation of the tool center plane. </returns>
        public Quaternion GetToolOrientation()
        {
            _orientation = Quaternion.Rotation(_attachmentPlane, _toolPlane);
            return _orientation;
        }

        /// <summary>
        /// Method for creating the robot tool data for the system BASE code. 
        /// </summary>
        /// <returns> Returns the robot tool BASE code as a string. </returns>
        public string GetRSToolData()
        {
            string result;
            result = CreateToolDataString();
            return result;
        }

        /// <summary>
        /// Private method for creating the robot tool data for the system BASE code. 
        /// </summary>
        /// <returns> Returns the robot tool BASE code as a string. </returns>
        private string CreateToolDataString()
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

                + _centerOfGravity.X.ToString("0.######") + ", " 
                + _centerOfGravity.Y.ToString("0.######") + ", " 
                + _centerOfGravity.Z.ToString("0.######") + "], ["

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
        /// Clear / unset all the fields and properties of the object.
        /// Used for constructing an empty Robot Tool object. 
        /// Since the empty constructor creates the default Robot Tool tool0.
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
            _centerOfGravity = Vector3d.Unset;
            _centerOfGravityOrientation = Quaternion.Zero;
            _inertia = Vector3d.Unset;
        }

        /// <summary>
        /// Transforms the Robot Tool spatial properties (planes and meshes). 
        /// </summary>
        /// <param name="xform"> Spatial deform. </param>
        public void Transform(Transform xform)
        {
            _mesh.Transform(xform);
            _attachmentPlane.Transform(xform);
            _toolPlane.Transform(xform);

            ReInitialize();
        }
        #endregion

        #region properties
        /// <summary>
        /// A boolean that indicates if the Robot Tool object is valid. 
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
                return true;
            }
        }

        /// <summary>
        /// Defines the reference type (PERS, VAR or CONST)
        /// </summary>
        public ReferenceType ReferenceType
        {
            get { return _referenceType; }
            set { _referenceType = value; }
        }

        /// <summary>
        /// The tobot tool name, must be unique. 
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// The Robot Tool mesh
        /// </summary>
        public Mesh Mesh
        {
            get { return _mesh; }
            set { _mesh = value; }
        }

        /// <summary>
        /// The robot tool attachment plane
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
        /// The robot tool center point and orientation defined as a plane
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
        /// Defines whether or not the robot is holding the tool.
        /// Use true if the robot is holding the tool. 
        /// Use false if the robot is not holding the tool (e.g. stationary tool).
        /// </summary>
        public bool RobotHold
        {
            get { return _robotHold; }
            set { _robotHold = value; }
        }

        /// <summary>
        /// The position of the the tool center point which is the offset between
        /// the tool center plane and the attachment plane
        /// </summary>
        public Point3d Position
        {
            get { return _position; }
        }

        /// <summary>
        /// The orientation of the tool center point as a Quaternion
        /// </summary>
        public Quaternion Orientation
        {
            get { return _orientation; }
        }

        /// <summary>
        /// The weight of the load in kg.
        /// </summary>
        public double Mass
        {
            get { return _mass; }
            set { _mass = value; }
        }

        /// <summary>
        /// The center of gravity of the tool load.
        /// </summary>
        public Vector3d CenterOfGravity
        {
            get { return _centerOfGravity; }
            set { _centerOfGravity = value; }
        }

        /// <summary>
        /// The orientation of the tool load coordinate system defined by the principal inertial axes of the 
        /// tool load. Expressed in the wrist coordinate system as a quaternion (q1, q2, q3, q4).
        /// </summary>
        public Quaternion CenterOfGravityOrientation
        {
            get { return _centerOfGravityOrientation; }
            set { _centerOfGravityOrientation = value; }
        }

        /// <summary>
        /// The moment of inertia of the load in kgm2.
        /// </summary>
        public Vector3d Inertia
        {
            get { return _inertia; }
            set { _inertia = value; }
        }

        /// <summary>
        /// The moment of inertia of the load around the x-axis of the tool load or payload coordinate system in kgm2.
        /// </summary>
        public double InertiaX
        {
            get { return _inertia[0]; }
            set { _inertia[0] = value; }
        }

        /// <summary>
        /// The moment of inertia of the load around the y-axis of the tool load or payload coordinate system in kgm2.
        /// </summary>
        public double InertiaY
        {
            get { return _inertia[1]; }
            set { _inertia[1] = value; }
        }

        /// <summary>
        /// The moment of inertia of the load around the z-axis of the tool load or payload coordinate system in kgm2.
        /// </summary>
        public double InertiaZ
        {
            get { return _inertia[2]; }
            set { _inertia[2] = value; }
        }
        #endregion
    }

}

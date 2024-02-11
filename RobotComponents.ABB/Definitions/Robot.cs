// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.ABB.Kinematics;
using RobotComponents.ABB.Actions.Declarations;

namespace RobotComponents.ABB.Definitions
{
    /// <summary>
    /// Represents a 6-axis spherical Robot.
    /// </summary>
    [Serializable()]
    public class Robot : ISerializable, IMechanicalUnit
    {
        #region fields
        private string _name; // The name of the robot
        private readonly List<Mesh> _meshes; // The robot mesh
        private List<Plane> _internalAxisPlanes; // The internal axis planes
        private List<Interval> _internalAxisLimits; // The internal axis limits
        private Plane _basePlane; // The base plane (position) of the robot
        private Plane _mountingFrame; // The tool mounting frame
        private RobotTool _tool; // The attached robot tool
        private Plane _toolPlane; // The TCP plane
        private List<IExternalAxis> _externalAxes; // The attached external axes
        private readonly InverseKinematics _inverseKinematics; // Robot inverse kinematics
        private readonly ForwardKinematics _forwardKinematics; // Robot forward kinematics
        private List<Plane> _externalAxisPlanes; // The external axis planes
        private List<Interval> _externalAxisLimits; // The external axis limit

        // Kinematics properties
        private double _upperArmLength;
        private double _lowerArmLength;
        private double _elbowLength;
        private double _a1;
        private double _a2;
        private double _a3;
        private double _b;
        private double _c1;
        private double _c2;
        private double _c3;
        private double _c4;
        #endregion

        #region (de)serialization
        /// <summary>
        /// Protected constructor needed for deserialization of the object.  
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization. </param>
        protected Robot(SerializationInfo info, StreamingContext context)
        {
            _name = (string)info.GetValue("Name", typeof(string));
            _meshes = (List<Mesh>)info.GetValue("Meshes", typeof(List<Mesh>));
            _internalAxisPlanes = (List<Plane>)info.GetValue("Internal Axis Planes", typeof(List<Plane>));
            _internalAxisLimits = (List<Interval>)info.GetValue("Internal Axis Limits", typeof(List<Interval>));
            _basePlane = (Plane)info.GetValue("Base Plane", typeof(Plane));
            _mountingFrame = (Plane)info.GetValue("Mounting Frame", typeof(Plane));
            _tool = (RobotTool)info.GetValue("RobotTool", typeof(RobotTool));
            _toolPlane = (Plane)info.GetValue("Tool Plane", typeof(Plane));
            _externalAxes = (List<IExternalAxis>)info.GetValue("External Axis", typeof(List<IExternalAxis>));
            _externalAxisPlanes = (List<Plane>)info.GetValue("External Axis Planes", typeof(List<Plane>));
            _externalAxisLimits = (List<Interval>)info.GetValue("External Axis Limits", typeof(List<Interval>));

            _inverseKinematics = new InverseKinematics(this);
            _forwardKinematics = new ForwardKinematics(this);

            UpdateKinematics();
        }

        /// <summary>
        /// Populates a SerializationInfo with the data needed to serialize the object.
        /// </summary>
        /// <param name="info"> The SerializationInfo to populate with data. </param>
        /// <param name="context"> The destination for this serialization. </param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // // Version version = (int)info.GetValue("Version", typeof(Version)); // <-- use this if the (de)serialization changes
            info.AddValue("Version", VersionNumbering.Version, typeof(Version));
            info.AddValue("Name", _name, typeof(string));
            info.AddValue("Meshes", _meshes, typeof(List<Mesh>));
            info.AddValue("Internal Axis Planes", _internalAxisPlanes, typeof(List<Plane>));
            info.AddValue("Internal Axis Limits", _internalAxisLimits, typeof(List<Interval>));
            info.AddValue("Base Plane", _basePlane, typeof(Plane));
            info.AddValue("Mounting Frame", _mountingFrame, typeof(Plane));
            info.AddValue("RobotTool", _tool, typeof(RobotTool));
            info.AddValue("Tool Plane", _toolPlane, typeof(Plane));
            info.AddValue("External Axis", _externalAxes, typeof(List<IExternalAxis>));
            info.AddValue("External Axis Planes", _externalAxisPlanes, typeof(List<Plane>));
            info.AddValue("External Axis Limits", _externalAxisLimits, typeof(List<Interval>));
        }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the Robot class.
        /// </summary>
        public Robot()
        {
            _name = "Empty Robot";
        }

        /// <summary>
        /// Initializes a new instance of the Robot class without attached external axes.
        /// </summary>
        /// <param name="name"> The name. </param>
        /// <param name="meshes"> The base and links meshes defined in the world coorindate space. </param>
        /// <param name="internalAxisPlanes"> The internal axes planes defined in the world coorindate space. </param>
        /// <param name="internalAxisLimits"> The internal axes limit. </param>
        /// <param name="basePlane"> The position and orientation of the robot base in the world coordinate space. </param>
        /// <param name="mountingFrame"> The tool mounting frame definied in the world coordinate space. </param>
        /// <param name="tool"> The Robot Tool. </param>
        public Robot(string name, IList<Mesh> meshes, IList<Plane> internalAxisPlanes, IList<Interval> internalAxisLimits, Plane basePlane, Plane mountingFrame, RobotTool tool)
        {
            // Update robot related fields
            _name = name;
            _meshes = new List<Mesh>(meshes);
            _internalAxisPlanes = new List<Plane>(internalAxisPlanes);
            _internalAxisLimits = new List<Interval>(internalAxisLimits);
            _basePlane = basePlane;
            _mountingFrame = mountingFrame;

            // Update tool related fields
            _tool = tool.Duplicate(); // Make a deep copy since we transform it later
            _meshes.Add(GetAttachedToolMesh());
            CalculateAttachedToolPlane();

            // Update external axis related fields
            _externalAxes = new List<IExternalAxis>();
            _externalAxisPlanes = Enumerable.Repeat(Plane.Unset, 6).ToList();
            _externalAxisLimits = Enumerable.Repeat(new Interval(), 6).ToList();

            // Transform Robot Tool to Mounting Frame
            Transform trans = Rhino.Geometry.Transform.PlaneToPlane(_tool.AttachmentPlane, _mountingFrame);
            _tool.Transform(trans);

            // Set kinematics
            _inverseKinematics = new InverseKinematics(this);
            _forwardKinematics = new ForwardKinematics(this);
            UpdateKinematics();
        }

        /// <summary>
        /// Initializes a new instance of the Robot class with attached external axes.
        /// </summary>
        /// <param name="name"> The name. </param>
        /// <param name="meshes"> The base and links meshes defined in the world coorindate space. </param>
        /// <param name="internalAxisPlanes"> The internal axes planes defined in the world coorindate space. </param>
        /// <param name="internalAxisLimits"> The internal axes limit. </param>
        /// <param name="basePlane"> The position and orientation of the robot base in the world coordinate space. </param>
        /// <param name="mountingFrame"> The tool mounting frame definied in the world coordinate space. </param>
        /// <param name="tool"> The Robot Tool. </param>
        /// <param name="externalAxes"> The attached external axes. </param>
        public Robot(string name, IList<Mesh> meshes, IList<Plane> internalAxisPlanes, IList<Interval> internalAxisLimits, Plane basePlane, Plane mountingFrame, RobotTool tool, IList<IExternalAxis> externalAxes)
        {
            // Robot related fields
            _name = name;
            _meshes = new List<Mesh>(meshes);
            _internalAxisPlanes = new List<Plane>(internalAxisPlanes);
            _internalAxisLimits = new List<Interval>(internalAxisLimits);
            _basePlane = basePlane;
            _mountingFrame = mountingFrame;

            // Tool related fields
            _tool = tool.Duplicate(); // Make a deep copy since we transform it later
            _meshes.Add(GetAttachedToolMesh());
            CalculateAttachedToolPlane();

            // External axis related fields
            _externalAxes = new List<IExternalAxis>(externalAxes);
            _externalAxisPlanes = new List<Plane>();
            _externalAxisLimits = new List<Interval>();
            UpdateExternalAxisFields();

            // Transform Robot Tool to Mounting Frame
            Transform trans = Rhino.Geometry.Transform.PlaneToPlane(_tool.AttachmentPlane, _mountingFrame);
            _tool.Transform(trans);

            // Set kinematics
            _inverseKinematics = new InverseKinematics(this);
            _forwardKinematics = new ForwardKinematics(this);
            UpdateKinematics();
        }

        /// <summary>
        /// Initializes a new instance of the Robot class by duplicating an existing Robot instance. 
        /// </summary>
        /// <param name="robot"> The Robot instance to duplicate. </param>
        /// <param name="duplicateMesh"> Specifies whether the meshes should be duplicated. </param>
        public Robot(Robot robot, bool duplicateMesh = true)
        {
            // Robot related fields
            _name = robot.Name;
            _internalAxisPlanes = robot.InternalAxisPlanes.ConvertAll(item => new Plane(item));
            _internalAxisLimits = robot.InternalAxisLimits.ConvertAll(item => new Interval(item));
            _basePlane = new Plane(robot.BasePlane);
            _mountingFrame = new Plane(robot.MountingFrame);

            // Mesh related fields
            if (duplicateMesh == true)
            {
                _meshes = robot.Meshes.ConvertAll(mesh => mesh.DuplicateMesh()); // This includes the tool mesh
                _tool = robot.Tool.Duplicate();
                _externalAxes = robot.ExternalAxes.ConvertAll(item => item.DuplicateExternalAxis());
            }
            else
            {
                _meshes = new List<Mesh>() { new Mesh(), new Mesh(), new Mesh(), new Mesh(), new Mesh(), new Mesh(), new Mesh() };
                _tool = robot.Tool.DuplicateWithoutMesh();
                _externalAxes = robot.ExternalAxes.ConvertAll(item => item.DuplicateExternalAxisWithoutMesh());
            }

            // Tool related fields
            _toolPlane = new Plane(robot.ToolPlane);

            // External axis related fields
            _externalAxisPlanes = robot.ExternalAxisPlanes.ConvertAll(item => new Plane(item));
            _externalAxisLimits = robot.ExternalAxisLimits.ConvertAll(item => new Interval(item));

            // Kinematics
            _inverseKinematics = new InverseKinematics(this);
            _forwardKinematics = new ForwardKinematics(this, robot.ForwardKinematics.HideMesh);
            UpdateKinematics();
        }

        /// <summary>
        /// Returns an exact duplicate of this Robot instance.
        /// </summary>
        /// <returns> 
        /// A deep copy of the Robot instance. 
        /// </returns>
        public Robot Duplicate()
        {
            return new Robot(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Robot as a Mechanical Unit.
        /// </summary>
        /// <returns> 
        /// A deep copy of the Mechanical Unit. 
        /// </returns>
        public IMechanicalUnit DuplicateMechanicalUnit()
        {
            return new Robot(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Robot as Mechanical Unit without meshes.
        /// </summary>
        /// <returns> 
        /// A deep copy of the Mechanical Unit without meshes. 
        /// </returns>
        public IMechanicalUnit DuplicateMechanicalUnitWithoutMesh()
        {
            return new Robot(this, false);
        }
        #endregion

        #region static methods
        /// <summary>
        /// Returns the axis planes for the given kinematics parameters in world coordinate space. 
        /// </summary>
        /// <param name="basePlane"> The position and orientation of the robot base in the world coordinate space. </param>
        /// <param name="a1"> Kinematics parameter A1. </param>
        /// <param name="a2"> Kinematics parameter A2. </param>
        /// <param name="a3"> Kinematics parameter A3. </param>
        /// <param name="b"> Kinematics parameter B. </param>
        /// <param name="c1"> Kinematics parameter C1. </param>
        /// <param name="c2"> Kinematics parameter C2. </param>
        /// <param name="c3"> Kinematics parameter C3. </param>
        /// <param name="c4"> Kinematics parameter C4. </param>
        /// <param name="mountingFrame"> The tool mounting frame. </param>
        /// <returns> The axis planes. </returns>
        public static Plane[] GetAxisPlanesFromKinematicsParameters(Plane basePlane, double a1, double a2, double a3, double b, double c1, double c2, double c3, double c4, out Plane mountingFrame)
        {
            Plane[] planes = GetAxisPlanesFromKinematicsParameters(a1, a2, a3, b, c1, c2, c3, c4, out mountingFrame);
            Transform orient = Rhino.Geometry.Transform.PlaneToPlane(Plane.WorldXY, basePlane);

            for (int i = 0; i < planes.Length; i++)
            {
                planes[i].Transform(orient);
            }

            return planes;
        }

        /// <summary>
        /// Returns the axis planes for the given kinematics parameters in local coordinate space. 
        /// </summary>
        /// <param name="a1"> Kinematics parameter A1. </param>
        /// <param name="a2"> Kinematics parameter A2. </param>
        /// <param name="a3"> Kinematics parameter A3. </param>
        /// <param name="b"> Kinematics parameter B. </param>
        /// <param name="c1"> Kinematics parameter C1. </param>
        /// <param name="c2"> Kinematics parameter C2. </param>
        /// <param name="c3"> Kinematics parameter C3. </param>
        /// <param name="c4"> Kinematics parameter C4. </param>
        /// <param name="mountingFrame"> The tool mounting frame. </param>
        /// <returns> The axis planes. </returns>
        public static Plane[] GetAxisPlanesFromKinematicsParameters(double a1, double a2, double a3, double b, double c1, double c2, double c3, double c4, out Plane mountingFrame)
        {
            Plane[] planes = new Plane[6];

            planes[0] = new Plane(new Point3d(0, 0, 0), new Vector3d(0, 0, 1));
            planes[1] = new Plane(new Point3d(a1, 0, c1), new Vector3d(0, 1, 0));
            planes[2] = new Plane(new Point3d(a1, 0, c1 + c2), new Vector3d(0, 1, 0));
            planes[3] = new Plane(new Point3d(a1, -b, c1 + c2 - a2), new Vector3d(1, 0, 0));
            planes[4] = new Plane(new Point3d(a1 + c3, -b, c1 + c2 - a2), new Vector3d(0, 1, 0));
            planes[5] = new Plane(new Point3d(a1 + c3 + c4, -b, c1 + c2 - a2 - a3), new Vector3d(1, 0, 0));

            mountingFrame = new Plane(new Point3d(planes[5].Origin), -Vector3d.ZAxis, Vector3d.YAxis);

            return planes;
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
            if (!IsValid)
            {
                return "Invalid Robot";
            }
            else
            {
                return $"Robot ({_name})";
            }
        }

        /// <summary>
        /// Reinitializes the fields that are related to the attached external axes.
        /// </summary>
        private void UpdateExternalAxisFields()
        {
            // Check the number of external axes
            if (_externalAxes.Count > 6)
            {
                throw new ArgumentException("More than six external axes are defined. A maximum of 6 external axes can be attached to a Robot.");
            }

            // Check list with external axes: maximum of one external linear axis is allowed at the moment
            if (_externalAxes.Count(item => item.MovesRobot is true) > 1)
            {
                throw new ArgumentException("More than one external axis is defined that moves the robot.");
            }

            _externalAxisPlanes.Clear();
            _externalAxisLimits.Clear();
            _externalAxisLimits = Enumerable.Repeat(new Interval(), 6).ToList();
            _externalAxisPlanes = Enumerable.Repeat(Plane.Unset, 6).ToList();

            for (int i = 0; i < _externalAxes.Count; i++)
            {
                if (_externalAxes[i].AxisNumber == -1)
                {
                    _externalAxes[i].AxisNumber = i;
                }
                else if (!_externalAxes[i].IsValid)
                {
                    throw new ArgumentException($"External Axis {_externalAxes[i].AxisLogic} ({_externalAxes[i].Name}): The set attached External Axis is not valid.");
                }

                _externalAxisLimits[_externalAxes[i].AxisNumber] = _externalAxes[i].AxisLimits;
                _externalAxisPlanes[_externalAxes[i].AxisNumber] = _externalAxes[i].AxisPlane;
            }

            // Check for duplicate axis logic numbers
            List<int> duplicates = _externalAxes.GroupBy(x => x.AxisNumber).Where(g => g.Count() > 1).Select(y => y.Key).ToList();
            if (duplicates.Count > 0)
            {
                throw new ArgumentException("Some of the axis logic numbers are used multiple times");
            }
        }

        /// <summary>
        /// Reinitializes the fields that are related to the kinematics.
        /// </summary>
        private void UpdateKinematics()
        {
            // Get values in World XY plane
            Transform orient = Rhino.Geometry.Transform.PlaneToPlane(_basePlane, Plane.WorldXY);
            List<Plane> planes = new List<Plane>();

            for (int i = 0; i < _internalAxisPlanes.Count; i++)
            {
                Plane plane = new Plane(_internalAxisPlanes[i]);
                plane.Transform(orient);
                planes.Add(plane);
            }

            // Elbow
            _lowerArmLength = planes[1].Origin.DistanceTo(planes[2].Origin);
            _upperArmLength = planes[2].Origin.DistanceTo(planes[4].Origin);
            _elbowLength = _lowerArmLength + _upperArmLength;

            // OPW Kinematics parameters
            _a1 = planes[1].Origin.X;
            _a2 = -(planes[4].Origin.Z - planes[2].Origin.Z);
            _a3 = -(planes[5].Origin.Z - planes[4].Origin.Z);
            _b = planes[0].Origin.Y - planes[5].Origin.Y;
            _c1 = planes[1].Origin.Z;
            _c2 = planes[2].Origin.Z - planes[1].Origin.Z;
            _c3 = planes[4].Origin.X - planes[2].Origin.X;
            _c4 = planes[5].Origin.X - planes[4].Origin.X;

            // Update kinematics solvers
            _inverseKinematics.ReInitialize();
        }

        /// <summary>
        /// Returns the attached Robot Tool mesh in robot coordinate space.
        /// </summary>
        /// <returns> 
        /// The tool mesh in the robot coordinate space. 
        /// </returns>
        public Mesh GetAttachedToolMesh()
        {
            Mesh toolMesh = _tool.Mesh.DuplicateMesh();
            Transform trans = Rhino.Geometry.Transform.PlaneToPlane(_tool.AttachmentPlane, _mountingFrame);
            toolMesh.Transform(trans);
            return toolMesh;
        }

        /// <summary>
        /// Calculates and returns the TCP plane of the attached Robot Tool in robot coordinate space.
        /// </summary>
        /// <returns> 
        /// The TCP plane in robot coordinate space. 
        /// </returns>
        private Plane CalculateAttachedToolPlane()
        {
            _toolPlane = new Plane(_tool.ToolPlane);
            Transform trans = Rhino.Geometry.Transform.PlaneToPlane(_tool.AttachmentPlane, _mountingFrame);
            _toolPlane.Transform(trans);

            return _toolPlane;
        }

        /// <summary>
        /// Calculates and returns the position of the meshes for a given Joint Target.
        /// </summary>
        /// <param name="jointTarget"> The Joint Target. </param>
        /// <returns> 
        /// The posed meshes. 
        /// </returns>
        public List<Mesh> PoseMeshes(JointTarget jointTarget)
        {
            List<Mesh> meshes = new List<Mesh>();

            _forwardKinematics.Calculate(jointTarget.RobotJointPosition, jointTarget.ExternalJointPosition);

            meshes.AddRange(_forwardKinematics.PosedRobotMeshes);

            for (int i = 0; i < _forwardKinematics.PosedExternalAxisMeshes.Count; i++)
            {
                _meshes.AddRange(_forwardKinematics.PosedExternalAxisMeshes[i]);
            }

            return meshes;
        }

        /// <summary>
        /// Transforms the robot spatial properties (planes and meshes).
        /// </summary>
        /// <remarks>
        /// The attached external axes will not be transformed.
        /// </remarks>
        /// <param name="xform"> Spatial deform. </param>
        public void Transform(Transform xform)
        {
            _basePlane.Transform(xform);
            _mountingFrame.Transform(xform);
            _tool.Transform(xform);

            for (int i = 0; i < _meshes.Count; i++)
            {
                _meshes[i].Transform(xform);
            }

            for (int i = 0; i < _internalAxisPlanes.Count; i++)
            {
                Plane transformedPlane = new Plane(_internalAxisPlanes[i]);
                transformedPlane.Transform(xform);
                _internalAxisPlanes[i] = new Plane(transformedPlane);
            }

            CalculateAttachedToolPlane();
        }

        /// <summary>
        /// Returns the Bounding Box of the object.
        /// </summary>
        /// <param name="accurate"> If true, a physically accurate bounding box will be computed. If not, a bounding box estimate will be computed. </param>
        /// <returns> 
        /// The Bounding Box. 
        /// </returns>
        public BoundingBox GetBoundingBox(bool accurate)
        {
            if (_meshes == null)
            {
                return BoundingBox.Empty;
            }

            else
            {
                // Make an empty bounding box
                BoundingBox boundingBox = BoundingBox.Empty;

                // Make the bounding box of the robot meshes
                for (int i = 0; i != _meshes.Count; i++)
                {
                    if (_meshes[i] != null)
                    {
                        if (_meshes[i].IsValid)
                        {
                            boundingBox.Union(_meshes[i].GetBoundingBox(accurate));
                        }
                    }
                }

                // Make the bounding box of the external axes
                for (int i = 0; i != _externalAxes.Count; i++)
                {
                    if (_externalAxes[i].IsValid == true)
                    {
                        boundingBox.Union(_externalAxes[i].GetBoundingBox(accurate));
                    }
                }

                // Make the bounding box of the robot tool
                if (_tool.IsValid)
                {
                    boundingBox.Union(_tool.GetBoundingBox(accurate));
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
                if (_internalAxisPlanes == null) { return false; }
                if (_internalAxisLimits == null) { return false; }
                if (_basePlane == null) { return false; }
                if (_basePlane == Plane.Unset) { return false; }
                if (_mountingFrame == null) { return false; }
                if (_mountingFrame == Plane.Unset) { return false; }
                if (_internalAxisPlanes.Count != 6) { return false; }
                if (_meshes.Count != 8) { return false; }
                if (_tool == null) { return false; }
                if (_tool.IsValid == false) { return false; }
                return true;
            }
        }

        /// <summary>
        /// Gets or sets the name of the Robot.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets the Robot meshes including the mesh of the attached tool.
        /// </summary>
        public List<Mesh> Meshes
        {
            get { return _meshes; }
        }

        /// <summary>
        /// Gets or sets the internal axis planes.
        /// </summary>
        /// <remarks>
        /// The Z-axes of the planes define the rotation centers.
        /// </remarks>
        public List<Plane> InternalAxisPlanes
        {
            get { return _internalAxisPlanes; }
            set { _internalAxisPlanes = value; UpdateKinematics(); }
        }

        /// <summary>
        /// Gets or sets the axis limits in degrees.
        /// </summary>
        public List<Interval> InternalAxisLimits
        {
            get { return _internalAxisLimits; }
            set { _internalAxisLimits = value; }
        }

        /// <summary>
        /// Gets or sets the position and orientation of the robot in world coordinate space. 
        /// </summary>
        public Plane BasePlane
        {
            get { return _basePlane; }
            set { _basePlane = value; }
        }

        /// <summary>
        /// Gets or sets the tool mounting frame in world coordinate space.
        /// </summary>
        public Plane MountingFrame
        {
            get { return _mountingFrame; }
            set { _mountingFrame = value; CalculateAttachedToolPlane(); }
        }

        /// <summary>
        /// Gets the TCP plane in world coordinate space.
        /// </summary>
        public Plane ToolPlane
        {
            get { return _toolPlane; }
        }

        /// <summary>
        /// Gets or sets the Robot Tool.
        /// </summary>
        public RobotTool Tool
        {
            get { return _tool; }
            set
            {
                _tool = value;
                CalculateAttachedToolPlane();
            }
        }

        /// <summary>
        /// Gets or sets the attached external axes.
        /// </summary>
        public List<IExternalAxis> ExternalAxes
        {
            get { return _externalAxes; }
            set { _externalAxes = value; UpdateExternalAxisFields(); }
        }

        /// <summary>
        /// Gets the Inverse Kinematics of this Robot. 
        /// </summary>
        public InverseKinematics InverseKinematics
        {
            get { return _inverseKinematics; }
        }

        /// <summary>
        /// Gets the Forward Kinimatics of this Robot.
        /// </summary>
        public ForwardKinematics ForwardKinematics
        {
            get { return _forwardKinematics; }
        }

        /// <summary>
        /// Gets the external axis planes.
        /// </summary>
        public List<Plane> ExternalAxisPlanes
        {
            get { return _externalAxisPlanes; }
        }

        /// <summary>
        /// Gets the external axis limits.
        /// </summary>
        public List<Interval> ExternalAxisLimits
        {
            get { return _externalAxisLimits; }
        }

        /// <summary>
        /// Gets the number of axes for the mechanical unit.
        /// </summary>
        public int NumberOfAxes
        {
            get { return _internalAxisPlanes.Count; }
        }

        /// <summary>
        /// Gets the length of the lower arm.
        /// </summary>
        /// <returns> The length of the lower arm. </returns>
        public double LowerArmLength
        {
            get { return _lowerArmLength; }
        }

        /// <summary>
        /// Gets the length of the upper arm.
        /// </summary>
        /// <returns> The length of the upper arm. </returns>
        public double UpperArmLength
        {
            get { return _upperArmLength; }
        }

        /// <summary>
        /// Gets the total length of the elbow.
        /// </summary>
        public double ElbowLength
        {
            get { return _elbowLength; }
        }

        /// <summary>
        /// Gets the OPW kinematics parameter A1.
        /// </summary>
        public double A1
        {
            get { return _a1; }
        }

        /// <summary>
        /// Gets the OPW kinematics parameter A2.
        /// </summary>
        public double A2
        {
            get { return _a2; }
        }

        /// <summary>
        /// Gets the wrist offset kinematics parameter A3.
        /// </summary>
        public double A3
        {
            get { return _a3; }
        }

        /// <summary>
        /// Gets the OPW kinematics parameter B.
        /// </summary>
        public double B
        {
            get { return _b; }
        }

        /// <summary>
        /// Gets the OPW kinematics parameter C1.
        /// </summary>
        public double C1
        {
            get { return _c1; }
        }

        /// <summary>
        /// Gets the OPW kinematics parameter C2.
        /// </summary>
        public double C2
        {
            get { return _c2; }
        }

        /// <summary>
        /// Gets the OPW kinematics parameter C3.
        /// </summary>
        public double C3
        {
            get { return _c3; }
        }

        /// <summary>
        /// Gets the OPW kinematics parameter C4.
        /// </summary>
        public double C4
        {
            get { return _c4; }
        }
        #endregion
    }
}
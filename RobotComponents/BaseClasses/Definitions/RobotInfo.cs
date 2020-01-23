using System.Linq;
using System.Collections.Generic;

using Rhino.Geometry;

namespace RobotComponents.BaseClasses.Definitions
{
    /// <summary>
    /// RobotInfo class, defines the basic properties and methods for any Robot Info.
    /// </summary>
    public class RobotInfo
    {
        #region fields
        private string _name; // The name of the robot
        private List<Mesh> _meshes; // The robot mesh
        private List<Plane> _internalAxisPlanes; // The internal axis planes
        private List<Interval> _internalAxisLimits; // The internal axis limits
        private Plane _basePlane; // The base plane (position) of the robot
        private Plane _mountingFrame; // The tool mounting frame
        private RobotTool _tool; // The attached robot tool
        private Plane _toolPlane; // The TCP plane
        private List<ExternalAxis> _externalAxis; // The attached external axes
        private List<Plane> _externalAxisPlanes; // The external axis planes
        private List<Interval> _externalAxisLimits; // The external axis limit
        #endregion

        #region constructors
        /// <summary>
        /// An empty constuctor that creates an robot info.
        /// </summary>
        public RobotInfo()
        {
            _name = "Empty RobotInfo";
        }

        /// <summary>
        /// Defines a robot info without external axes. 
        /// </summary>
        /// <param name="name"> The robot name. </param>
        /// <param name="meshes"> The robot base and links meshes as a list defined in the robot coorindate space. </param>
        /// <param name="internalAxisPlanes"> The internal axes planes as a list defined in the robot coordinate space. </param>
        /// <param name="internalAxisLimits"> The internal axes limit as intervals. </param>
        /// <param name="basePlane"> The position of the robot in the world coordinate space as a plane. </param>
        /// <param name="mountingFrame"> The tool mounting frame as plane in the robot coordinate space. </param>
        /// <param name="tool"> The attached robot tool as a Robot Tool. </param>
        public RobotInfo(string name, List<Mesh> meshes, List<Plane> internalAxisPlanes, List<Interval> internalAxisLimits, Plane basePlane, Plane mountingFrame, RobotTool tool)
        {
            // Update robot related fields
            _name = name;
            _meshes = meshes;
            _internalAxisPlanes = internalAxisPlanes;
            _internalAxisLimits = internalAxisLimits;
            _basePlane = basePlane;
            _mountingFrame = mountingFrame;

            // Update tool related fields
            _tool = tool;
            _meshes.Add(GetAttachedToolMesh(_tool));
            _toolPlane = GetAttachedToolPlane(_tool);

            // Update external axis related fields
            _externalAxis = new List<ExternalAxis>();
            _externalAxisPlanes = Enumerable.Repeat(Plane.Unset, 6).ToList();
            _externalAxisLimits = Enumerable.Repeat(new Interval(), 6).ToList();
        }

        /// <summary>
        /// Defines a robot info with attached external axes.
        /// </summary>
        /// <param name="name"> The robot name. </param>
        /// <param name="meshes"> The robot base and links meshes as a list defined in the robot coorindate space. </param>
        /// <param name="internalAxisPlanes"> The internal axes planes as a list defined in the robot coordinate space. </param>
        /// <param name="internalAxisLimits"> The internal axes limit as intervals. </param>
        /// <param name="basePlane"> The position of the robot in the world coordinate space as a plane. </param>
        /// <param name="mountingFrame"> The tool mounting frame as plane in the robot coordinate space. </param>
        /// <param name="tool"> The attached robot tool as a Robot Tool. </param>
        /// <param name="externalAxis"> The list with attached external axes. </param>
        public RobotInfo(string name, List<Mesh> meshes, List<Plane> internalAxisPlanes, List<Interval> internalAxisLimits, Plane basePlane, Plane mountingFrame, RobotTool tool, List<ExternalAxis> externalAxis)
        {
            // Robot related fields
            _name = name;
            _meshes = meshes;
            _internalAxisPlanes = internalAxisPlanes;
            _internalAxisLimits = internalAxisLimits;
            _basePlane = basePlane;
            _mountingFrame = mountingFrame;

            // Tool related fields
            _tool = tool;
            _meshes.Add(GetAttachedToolMesh(_tool));
            _toolPlane = GetAttachedToolPlane(_tool);

            // External axis related fields
            _externalAxis = externalAxis;
            _externalAxisPlanes = new List<Plane>();
            _externalAxisLimits = new List<Interval>();
            UpdateExternalAxisFields();
        }

        /// <summary>
        /// A method to duplicate the RobotInfo object. 
        /// </summary>
        /// <returns> Returns a deep copy for the RobotInfo object. </returns>
        public RobotInfo Duplicate()
        {
            RobotInfo dup = new RobotInfo(Name, Meshes, InternalAxisPlanes, InternalAxisLimits, BasePlane, MountingFrame, Tool, ExternalAxis);
            return dup;
        }
        #endregion

        #region methods
        /// <summary>
        /// Re-initializes the fields related to the list with external axes.
        /// </summary>
        private void UpdateExternalAxisFields()
        {
            // Clear the lists
            _externalAxisPlanes.Clear();
            _externalAxisLimits.Clear();

            // Fill the lists again
            for (int i = 0; i < 6; i++)
            {
                if (_externalAxis.Count > i && _externalAxis[i] != null)
                {
                    _externalAxisLimits.Add(_externalAxis[i].AxisLimits);
                    _externalAxisPlanes.Add(_externalAxis[i].AxisPlane);
                }
                else
                {
                    _externalAxisLimits.Add(new Interval());
                    _externalAxisPlanes.Add(Plane.Unset);
                }
            }
        }

        /// <summary>
        /// Defines the attached tool mesh in the robot coordinate space.
        /// </summary>
        /// <param name="tool"> Ther robot tool to take the mesh from. </param>
        /// <returns> The tool mesh in the robot coordinate space. </returns>
        public Mesh GetAttachedToolMesh(RobotTool tool)
        {
            Mesh toolMesh = tool.Mesh.DuplicateMesh();
            Transform trans = Transform.PlaneToPlane(tool.AttachmentPlane, _mountingFrame);
            toolMesh.Transform(trans);
            return toolMesh;
        }

        /// <summary>
        /// Defines the TCP plane of the attached robot tool in the robot coordinate space.
        /// </summary>
        /// <param name="tool"> Ther robot tool to take the mesh from. </param>
        /// <returns> The attached tool mesh in the robot coordinate space. </returns>
        public Plane GetAttachedToolPlane(RobotTool tool)
        {
            Plane toolPlane = new Plane(tool.ToolPlane);
            Transform trans = Transform.PlaneToPlane(tool.AttachmentPlane, _mountingFrame);
            toolPlane.Transform(trans);
            return toolPlane;
        }
        #endregion

        #region properties
        /// <summary>
        /// A boolean that indicates if the Robot Info object is valid. 
        /// </summary>
        public bool IsValid
        {
            get
            {
                if (InternalAxisPlanes == null) { return false; }
                if (InternalAxisLimits == null) { return false; }
                if (BasePlane == null) { return false; }
                if (BasePlane == Plane.Unset) { return false; }
                if (MountingFrame == null) { return false; }
                if (MountingFrame == Plane.Unset) { return false; }
                return true;
            }
        }

        /// <summary>
        /// The Robot Info name. 
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// A list with all the robot info meshes, including the mesh of the attached tool.
        /// </summary>
        public List<Mesh> Meshes
        {
            get { return _meshes; }
            set { _meshes = value; }
        }

        /// <summary>
        /// Defines the list with the internal axis planes. The Z-axis of the planes if defining the rotation centers. 
        /// </summary>
        public List<Plane> InternalAxisPlanes
        {
            get { return _internalAxisPlanes; }
            set { _internalAxisPlanes = value; }
        }

        /// <summary>
        /// Defines the list with internal axis limits in degrees. 
        /// </summary>
        public List<Interval> InternalAxisLimits
        {
            get { return _internalAxisLimits; }
            set { _internalAxisLimits = value; }
        }

        /// <summary>
        /// Defines the position plane of the robot in the world coordinate space. 
        /// </summary>
        public Plane BasePlane
        {
            get { return _basePlane; }
            set { _basePlane = value; }
        }

        /// <summary>
        /// Defines the mounting plane of the tool in the robot coordinate space. 
        /// </summary>
        public Plane MountingFrame
        {
            get
            {
                return _mountingFrame;
            }
            set
            {
                _mountingFrame = value;
                _toolPlane = GetAttachedToolPlane(_tool);
            }
        }

        /// <summary>
        /// Defines the TCP plane of the tool in the robot coordinate space. 
        /// </summary>
        public Plane ToolPlane
        {
            get { return _toolPlane; }
        }

        /// <summary>
        /// Defines the attached robot tool. 
        /// </summary>
        public RobotTool Tool
        {
            get
            {
                return _tool;
            }
            set
            {
                _tool = value;
                _toolPlane = GetAttachedToolPlane(_tool);
            }
        }

        /// <summary>
        /// Defines the list with attached external axes.
        /// </summary>
        public List<ExternalAxis> ExternalAxis
        {
            get
            {
                return _externalAxis;
            }
            set
            {
                _externalAxis = value;
                UpdateExternalAxisFields();
            }
        }

        /// <summary>
        /// Defines the list with external axis planes. 
        /// </summary>
        public List<Plane> ExternalAxisPlanes
        {
            get { return _externalAxisPlanes; }
        }

        /// <summary>
        /// Defines the list with external axis limits. 
        /// </summary>
        public List<Interval> ExternalAxisLimits
        {
            get { return _externalAxisLimits; }
        }
        #endregion
    }

}

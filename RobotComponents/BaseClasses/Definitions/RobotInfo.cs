using System;
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
        private string _name;
        private List<Mesh> _meshes;
        private List<Plane> _internalAxisPlanes;
        private List<Interval> _internalAxisLimits;
        private List<Plane> _externalAxisPlanes;
        private List<Interval> _externalAxisLimits;
        private Plane _basePlane;
        private Plane _mountingFrame;
        private Plane _toolPlane;
        private RobotTool _tool;
        private List<ExternalAxis> _externalAxis;
        #endregion

        #region constructors
        public RobotInfo()
        {
            _name = "Empty RobotInfo";
        }

        public RobotInfo(string name, List<Mesh> meshes, List<Plane> internalAxisPlanes, List<Interval> internalAxisLimits, Plane basePlane, Plane mountingFrame, RobotTool tool, List<ExternalAxis> externalAxis) 
        {
            _name = name;
            _meshes = meshes;

            _internalAxisPlanes = internalAxisPlanes;
            _internalAxisLimits = internalAxisLimits;
            _externalAxisPlanes = new List<Plane> { };
            _externalAxis = externalAxis;
            _externalAxisLimits = new List<Interval> { }; //improve this
            for (int i = 0; i < 6; i++)
            {
                if (_externalAxis.Count > i && _externalAxis[i] != null)
                {
                    _externalAxisLimits.Add(_externalAxis[i].AxisLimits);
                    _externalAxisPlanes.Add(_externalAxis[i].AxisPlane);
                }
                else
                {
                    _externalAxisLimits.Add(new Interval(0, 0));
                    _externalAxisPlanes.Add(Plane.WorldXY);
                }
            }
          
            
            _basePlane = basePlane;
            _mountingFrame = mountingFrame;

            _tool = tool;

            _meshes.Add(GetAttachedToolMesh(_tool, mountingFrame));
            _toolPlane = GetAttachedToolPlane(_tool, mountingFrame);
        }

        public RobotInfo(string name, List<Mesh> meshes, List<Plane> internalAxisPlanes, List<Interval> internalAxisLimits, Plane basePlane, Plane mountingFrame, RobotTool tool)
        {
            _name = name;
            _meshes = meshes;

            _internalAxisPlanes = internalAxisPlanes;
            _internalAxisLimits = internalAxisLimits;
            _externalAxisPlanes = Enumerable.Repeat(Plane.WorldXY, 6).ToList();
            _externalAxisLimits = Enumerable.Repeat(new Interval(0, 0), 6).ToList();

            _basePlane = basePlane;
            _mountingFrame = mountingFrame;

            _tool = tool;
            _externalAxis = new List<ExternalAxis>();

            _meshes.Add(GetAttachedToolMesh(_tool, mountingFrame));
            _toolPlane = GetAttachedToolPlane(_tool, mountingFrame);
        }

        public RobotInfo Duplicate()
        {
            RobotInfo dup = new RobotInfo(Name, Meshes, InternalAxisPlanes, InternalAxisLimits, BasePlane, MountingFrame, Tool, ExternalAxis);
            return dup;
        }
        #endregion

        #region methods
        public Mesh GetAttachedToolMesh(RobotTool tool, Plane mountingPlane)
        {
            Mesh toolMesh = tool.Mesh.DuplicateMesh();
            Transform trans = Transform.PlaneToPlane(tool.AttachmentPlane, mountingPlane);
            toolMesh.Transform(trans);
            return toolMesh;
        }

        public Plane GetAttachedToolPlane(RobotTool tool, Plane mountingPlane)
        {
            Plane toolPlane = new Plane(tool.ToolPlane);
            Transform trans = Transform.PlaneToPlane(tool.AttachmentPlane, mountingPlane);
            toolPlane.Transform(trans);
            return toolPlane;
        }
        #endregion

        #region properties
        public bool IsValid
        {
            get
            {
                if (InternalAxisPlanes == null) { return false; }
                if (InternalAxisLimits == null) { return false; }
                if (BasePlane == null) { return false; }
                if (MountingFrame == null) { return false; }
                return true;
            }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public List<Mesh> Meshes
        {
            get { return _meshes; }
            set { _meshes = value; }
        }

        public List<Plane> InternalAxisPlanes
        {
            get { return _internalAxisPlanes; }
            set { _internalAxisPlanes = value; }
        }

        public List<Interval> InternalAxisLimits
        {
            get { return _internalAxisLimits; }
            set { _internalAxisLimits = value; }
        }

        public Plane BasePlane
        {
            get { return _basePlane; }
            set { _basePlane = value; }
        }

        public Plane MountingFrame
        {
            get { return _mountingFrame; }
            set { _mountingFrame = value; }
        }

        public Plane ToolPlane
        {
            get { return _toolPlane; }
        }

        public RobotTool Tool
        {
            get 
            { 
                return _tool; 
            }
            set 
            { 
                _tool = value;
                _toolPlane = GetAttachedToolPlane(_tool, MountingFrame);
            }
        }

        public List<Plane> ExternalAxisPlanes 
        {
            get { return _externalAxisPlanes; }
            set { _externalAxisPlanes = value; }
        }
        public List<Interval> ExternalAxisLimits 
        {
            get { return _externalAxisLimits; }
            set { _externalAxisLimits = value; }
        }
        public List<ExternalAxis> ExternalAxis 
        {
            get { return _externalAxis; }
            set { _externalAxis = value; }
        }
        #endregion
    }

}

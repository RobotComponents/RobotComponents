using System;
using System.Collections.Generic;

using Rhino.Geometry;

namespace RobotComponents.BaseClasses
{
    /// <summary>
    /// ForwardKinematics class
    /// </summary>
    public class ForwardKinematics
    {
        #region fields

        RobotInfo _robotInfo;                                       //Robot Info Goo

        Plane _basePlane;                                           //Robot Base Plane
        Plane[] _internalAxisPlanes;                                //Internal Axis Planes 
        List<double> _internalAxisValues = new List<double>();      //Internal Axis Values in Degrees
        double[] _internalAxisRads;                                 //Internal Axis Values in Radiants
        List<Interval> _internalAxisLimits;                         //Internal Axis Value Limits
        List<bool> _internalAxisInLimit = new List<bool>();         //Internal Axis in Limit?: Bool List

        Plane[] _externalAxisPlanes;                                //External Axis Planes 
        List<double> _externalAxisValues = new List<double>();      //External Axis Values in Degrees
        double[] _externalAxisRads;                                 //External Axis Values in Radiants
        List<Interval> _externalAxisLimits;                         //External Axis Value Limits
        List<bool> _externalAxisInLimit = new List<bool>();         //External Axis in Limit?: Bool List

        List<string> _errorText = new List<string>();

        List<Mesh> _meshes;                                         //Robot Meshes
        List<Mesh> _posedMeshes;                                    //Posed Robot Meshes
        List<Mesh> _posedAxisMeshes;                                //Posed Axis Meshes
        Plane _endPlane;                                            //EndPlane placed on TargetPlane
        Plane _tcpPlane;                                            //TCP Plane of effector

        #endregion

        #region constructors
        public ForwardKinematics()
        {
        }

        public ForwardKinematics(RobotInfo robotInfo)
        {
        this._robotInfo = robotInfo;
        }

        public ForwardKinematics(RobotInfo robotInfo, List<double> internalAxisValues, List<double> externalAxisValues)
        {
            this._robotInfo = robotInfo;
            Update(internalAxisValues, externalAxisValues);
        }

        public ForwardKinematics Duplicate()
        {
            ForwardKinematics dup = new ForwardKinematics();
            return dup;
        }

        #endregion

        #region methods
        // Calculates Forward Kinematics
        public void Calculate()
        {
            this._posedMeshes = this._meshes.ConvertAll(mesh => mesh.DuplicateMesh());
            this.PosedAxisMeshes = new List<Mesh>();
            this._tcpPlane = this._robotInfo.ToolPlane;

            // Calculates external axes
            for (int i = 0; i < _robotInfo.ExternalAxis.Count; i++)
            {
                if (_robotInfo.ExternalAxis[i] is ExternalLinearAxis)
                {
                    ExternalLinearAxis externalLinearAxis = _robotInfo.ExternalAxis[i] as ExternalLinearAxis;
                    this._basePlane.Origin += externalLinearAxis.AxisPlane.ZAxis * _externalAxisValues[0]; //External Axis Offset: Use "CalculatePositionSave()" ?
                    externalLinearAxis.PoseMeshes(_externalAxisValues[0]);

                    for (int j = 0; j < externalLinearAxis.PosedMeshes.Count; j++)
                    {
                        PosedAxisMeshes.Add(externalLinearAxis.PosedMeshes[j]);
                    }
         
                }
            }
            
            // Calculates interal axes
            // First caculate all tansformations (rotations)
            // Axis 1
            Transform rot1;
            rot1 = Transform.Rotation(_internalAxisRads[0], this._internalAxisPlanes[0].ZAxis, this._internalAxisPlanes[0].Origin);
            // Axis 2
            Transform rot2;
            Plane planeAxis2 = new Plane(_internalAxisPlanes[1]);
            planeAxis2.Transform(rot1);
            rot2 = Transform.Rotation(_internalAxisRads[1], planeAxis2.ZAxis, planeAxis2.Origin);
            // Axis 3
            Transform rot3;
            Plane planeAxis3 = new Plane(_internalAxisPlanes[2]);
            planeAxis3.Transform(rot2 * rot1);
            rot3 = Transform.Rotation(_internalAxisRads[2], planeAxis3.ZAxis, planeAxis3.Origin);
            // Axis 4
            Transform rot4;
            Plane planeAxis4 = new Plane(_internalAxisPlanes[3]);
            planeAxis4.Transform(rot3 * rot2 * rot1);
            rot4 = Transform.Rotation(_internalAxisRads[3], planeAxis4.ZAxis, planeAxis4.Origin);
            // Axis 5
            Transform rot5;
            Plane planeAxis5 = new Plane(_internalAxisPlanes[4]);
            planeAxis5.Transform(rot4 * rot3 * rot2 * rot1);
            rot5 = Transform.Rotation(_internalAxisRads[4], planeAxis5.ZAxis, planeAxis5.Origin);
            // Axis 6
            Transform rot6;
            Plane planeAxis6 = new Plane(_internalAxisPlanes[5]);
            planeAxis6.Transform(rot5 * rot4 * rot3 * rot2 * rot1);
            rot6 = Transform.Rotation(_internalAxisRads[5], planeAxis6.ZAxis, planeAxis6.Origin);

            // Move relative to base
            Transform transNow;
            transNow = Transform.ChangeBasis(_basePlane, Plane.WorldXY);

            // Apply transformations
            // Base link transform
            _posedMeshes[0].Transform(transNow);
            // Link_1 tranfrom 
            _posedMeshes[1].Transform(transNow * rot1);
            // Link_2 tranfrom
            _posedMeshes[2].Transform(transNow * rot2 * rot1);
            // Link_3 tranfrom
            _posedMeshes[3].Transform(transNow * rot3 * rot2 * rot1);
            // Link_4 tranfrom
            _posedMeshes[4].Transform(transNow * rot4 * rot3 * rot2 * rot1);
            // Link_5 tranfrom
            _posedMeshes[5].Transform(transNow * rot5 * rot4 * rot3 * rot2 * rot1);
            // Link_6 tranfrom
            _posedMeshes[6].Transform(transNow * rot6 * rot5 * rot4 * rot3 * rot2 * rot1);
            // Endeffector transform
            _posedMeshes[7].Transform(transNow * rot6 * rot5 * rot4 * rot3 * rot2 * rot1);
            // TCP plane transform
            _tcpPlane.Transform(transNow * rot6 * rot5 * rot4 * rot3 * rot2 * rot1);
        }

        public void Update(List<double> internalAxisValues, List<double> externalAxisValues)
        {
            _internalAxisValues.Clear();
            _internalAxisInLimit.Clear();
            _externalAxisValues.Clear();
            _externalAxisInLimit.Clear();
            _errorText.Clear();

            this._basePlane = _robotInfo.BasePlane;

            this._internalAxisPlanes = _robotInfo.InternalAxisPlanes.ToArray();
            this._internalAxisLimits = _robotInfo.InternalAxisLimits;
            this._internalAxisValues = internalAxisValues;

            this._externalAxisPlanes = new Plane[_robotInfo.ExternalAxis.Count];

            this._externalAxisLimits = _robotInfo.ExternalAxisLimits;
            this._externalAxisValues = externalAxisValues;

            // "Deep dopy" mesh to new object
            this._meshes = _robotInfo.Meshes.ConvertAll(mesh => mesh.DuplicateMesh());
            this._posedMeshes = _robotInfo.Meshes.ConvertAll(mesh => mesh.DuplicateMesh());

            _tcpPlane = _robotInfo.ToolPlane;

            // Internal axis values in degrees converted to axis values in radiants
            this._internalAxisRads = new double[_internalAxisValues.Count];
            _internalAxisRads[0] = (_internalAxisValues[0] / 180) * Math.PI;
            _internalAxisRads[1] = (_internalAxisValues[1] / 180) * Math.PI;
            _internalAxisRads[2] = (_internalAxisValues[2] / 180) * Math.PI;
            _internalAxisRads[3] = (_internalAxisValues[3] / 180) * Math.PI;
            _internalAxisRads[4] = (_internalAxisValues[4] / 180) * Math.PI;
            _internalAxisRads[5] = (_internalAxisValues[5] / 180) * Math.PI;

            // Check axis limits
            CheckForInternalAxisLimits();
            CheckForExternalAxisLimits();

            // Get the current location of the attachment plane of the external axes
            for (int i = 0; i < _robotInfo.ExternalAxis.Count; i++)
            {
                _externalAxisPlanes[i] = _robotInfo.ExternalAxis[i].CalculatePositionSave(_externalAxisValues[i]);
            }
        }

        public void CheckForInternalAxisLimits()
        {
            _errorText.Clear();
            _internalAxisInLimit.Clear();

            for (int i = 0; i < _internalAxisValues.Count; i++)
            {
                if (_internalAxisLimits[i].IncludesParameter(_internalAxisValues[i]))
                {
                    _internalAxisInLimit.Add(true);
                }
                else
                {
                    _errorText.Add("Internal Axis Value " + i + " is not in Range.");
                    _internalAxisInLimit.Add(false);
                }

            }
        }

        public void CheckForExternalAxisLimits()
        {
            _errorText.Clear();
            _externalAxisInLimit.Clear();

            for (int i = 0; i < _externalAxisValues.Count; i++)
            {
                if (_externalAxisLimits[i].IncludesParameter(_externalAxisValues[i]))
                {
                    _externalAxisInLimit.Add(true);
                }
                else
                {
                    _errorText.Add("External Axis Value " + i + " is not in Range.");
                    _externalAxisInLimit.Add(false);
                }

            }
        }
        #endregion

        #region properties
        public bool IsValid
        {
            get
            {
                if (InternalAxisPlanes == null) { return false; }
                if (InternalAxisLimits == null) { return false; }
                if (InternalAxisValues == null) { return false; }
                return true;
            }
        }

        public RobotInfo RobotInfo
        {
            get { return _robotInfo; }
            set { _robotInfo = value; }
        }

        public List<Mesh> Meshes
        {
            get { return _meshes; }
            set { _meshes = value; }
        }

        public List<Mesh> PosedMeshes
        {
            get { return _posedMeshes; }
            set { _posedMeshes = value; }
        }

        public Plane[] InternalAxisPlanes
        {
            get { return _internalAxisPlanes; }
            set { _internalAxisPlanes = value; }
        }

        public Plane EndPlane
        {
            get { return _endPlane; }
            set { _endPlane = value; }
        }

        public Plane TCPPlane
        {
            get { return _tcpPlane; }
            set { _tcpPlane = value; }
        }

        public List<Interval> InternalAxisLimits
        {
            get { return _internalAxisLimits; }
            set { _internalAxisLimits = value; }
        }

        public List<bool> InternalAxisInLimit
        {
            get { return _internalAxisInLimit; }
            set { _internalAxisInLimit = value; }
        }

        public List<string> ErrorText
        {
            get { return _errorText; }
            set { _errorText = value; }
        }

        public List<double> InternalAxisValues
        {
            get { return _internalAxisValues; }
            set { _internalAxisValues = value; }
        }

        public Double[] InternalAxisRads
        {
            get { return _internalAxisRads; }
            set { _internalAxisRads = value; }
        }

        public Plane[] ExternalAxisPlanes 
        {
            get { return _externalAxisPlanes; }
            set { _externalAxisPlanes = value; }
        }

        public List<double> ExternalAxisValues 
        {
            get { return _externalAxisValues; }
            set { _externalAxisValues = value; }
        }

        public double[] ExternalAxisRads 
        {
            get { return _externalAxisRads; }
            set { _externalAxisRads = value; }
        }

        public List<Interval> ExternalAxisLimits 
        {
            get { return _externalAxisLimits; }
            set { _externalAxisLimits = value; }
        }
        
        public List<bool> ExternalAxisInLimit 
        {
            get { return _externalAxisInLimit; }
            set { _externalAxisInLimit = value; }
        }

        public Plane BasePlane 
        {
            get { return _basePlane; }
            set { _basePlane = value; }
        }

        public List<Mesh> PosedAxisMeshes 
        {
            get { return _posedAxisMeshes; }
            set { _posedAxisMeshes = value; }
        }
        #endregion
    }

}

using System.Collections.Generic;

using Rhino.Geometry;

namespace RobotComponents.BaseClasses
{
    public class ExternalLinearAxis : ExternalAxis
    {
        #region fields
        Plane _attachmentPlane;
        Plane _axisPlane;          // Z-Axis of the _axisPlane is the linear axis
        Interval _axisLimits;
        int? _axisNumber;
        Mesh _baseMesh;
        Mesh _linkMesh;
        Mesh _posedBaseMesh;
        Mesh _posedLinkMesh;
        Curve _axisCurve;
        List<Mesh> _posedMeshes;
        #endregion

        #region constructors
        public ExternalLinearAxis()
        {
            _posedMeshes = new List<Mesh>();
        }

        public ExternalLinearAxis(Plane attachementPlane, Plane axisPlane, Interval axisLimits)
        {
            _attachmentPlane = attachementPlane;
            _axisPlane = axisPlane;
            _axisLimits = axisLimits;
            _axisNumber = null; // to do
            BaseMesh = null;
            LinkMesh = null;
            _posedMeshes = new List<Mesh>();
            Initialize();
        }

        public ExternalLinearAxis(Plane attachementPlane, Vector3d axis, Interval axisLimits)
        {
            _attachmentPlane = attachementPlane;
            axis.Unitize();
            _axisPlane = new Plane(attachementPlane.Origin, axis);
            _axisLimits = axisLimits;
            _axisNumber = null; // to do
            BaseMesh = null;
            LinkMesh = null;
            _posedMeshes = new List<Mesh>();
            Initialize();
        }

        public ExternalLinearAxis(Plane attachementPlane, Vector3d axis, Interval axisLimits, Mesh baseMesh, Mesh linkMesh)
        {
            _attachmentPlane = attachementPlane;
            axis.Unitize();
            _axisPlane = new Plane(attachementPlane.Origin, axis);
            _axisLimits = axisLimits;
            _axisNumber = null; // to do
            BaseMesh = baseMesh;
            LinkMesh = linkMesh;
            _posedMeshes = new List<Mesh>();
            Initialize();
        }

        public ExternalLinearAxis(Plane attachementPlane, Plane axisPlane, Interval axisLimits, Mesh baseMesh, Mesh linkMesh)
        {
            _attachmentPlane = attachementPlane;
            _axisPlane = axisPlane;
            _axisLimits = axisLimits;
            _axisNumber = null; // to do
            BaseMesh = baseMesh;
            LinkMesh = linkMesh;
            _posedMeshes = new List<Mesh>();
            Initialize();
        }

        public ExternalLinearAxis Duplicate()
        {
            ExternalLinearAxis dup = new ExternalLinearAxis(AttachmentPlane, AxisPlane, AxisLimits, BaseMesh, LinkMesh);
            return dup;
        }
        #endregion

        #region methods
        public bool IsValid
        {
            get
            {
                if (AttachmentPlane == null) { return false; }
                if (AxisPlane == null) { return false; }
                if (AxisLimits == null) { return false; }
                return true;
            }
        }

        public void GetAxisCurve()
        {
            Line line = new Line(_attachmentPlane.Origin + _axisPlane.ZAxis * _axisLimits.Min, _attachmentPlane.Origin + _axisPlane.ZAxis * _axisLimits.Max);
            AxisCurve = line.ToNurbsCurve();
            AxisCurve.Domain = new Interval(0, 1);

        }

        public override Plane CalculatePosition(double axisValue, out bool inLimits)
        {
            bool isInLimits;

            // Check if value is within axis limits
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

            // Transform
            Transform translateNow = Transform.Translation(_axisPlane.ZAxis * axisValue);
            Plane positionPlane = new Plane(AttachmentPlane);
            positionPlane.Transform(translateNow);

            inLimits = isInLimits;
            return positionPlane;
        }

        public override Plane CalculatePositionSave(double axisValue)
        {
            double value;

            // Check if value is within axis limits
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

            // Transform
            Transform translateNow = Transform.Translation(_axisPlane.ZAxis * value);
            Plane positionPlane = new Plane(AttachmentPlane);
            positionPlane.Transform(translateNow);

            return positionPlane;
        }

        override public void PoseMeshes(double axisValue)
        {
            _posedMeshes.Clear();
            _posedMeshes.Add(_baseMesh.DuplicateMesh());
            _posedMeshes.Add(_linkMesh.DuplicateMesh());

            Transform translateNow = Transform.Translation(_axisPlane.ZAxis * axisValue);
            _posedMeshes[1].Transform(translateNow);
        }

        public override void Initialize()
        {
            GetAxisCurve();
        }

        public override void ReInitialize()
        {
            Initialize();
        }

        #endregion

        #region properties

        override public Plane AttachmentPlane 
        {
            get { return _attachmentPlane; }
            set { _attachmentPlane = value; }
        }

        override public Plane AxisPlane 
        { 
            get { return _axisPlane; }
            set { _axisPlane = value; }
        }

        override public Interval AxisLimits 
        { 
            get { return _axisLimits; }
            set { _axisLimits = value; }
        }

        override public int? AxisNumber 
        { 
            get { return _axisNumber; }
            set { _axisNumber = value; }
        }

        public override AxisType AxisType 
        { 
            get { return AxisType.LINEAR; }
        }

        public Mesh BaseMesh 
        { 
            get { return _baseMesh; }
            set { _baseMesh = value; }
        }

        public Mesh LinkMesh 
        { 
            get { return _linkMesh; }
            set { _linkMesh = value; }
        }

        public Curve AxisCurve 
        { 
            get { return _axisCurve; }
            set { _axisCurve = value; }
        }

        override public List<Mesh> PosedMeshes 
        { 
            get { return _posedMeshes; }
            set { _posedMeshes = value; }
        }
        #endregion
    }
}

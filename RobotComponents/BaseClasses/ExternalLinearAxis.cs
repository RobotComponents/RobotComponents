using System.Collections.Generic;

using Rhino.Geometry;

namespace RobotComponents.BaseClasses
{
    public class ExternalLinearAxis : ExternalAxis
    {
        #region fields
        private Point3d _startPoint;
        private Point3d _endPoint;
        private double _attachmentDistance;

        private Vector3d _axis;
        private Point3d _attachmentOrigin;
        private double _maxNegativeDistanceFromOrigin;
        private double _maxPositiveDistanceFromOrigin;
        private Interval _axisLimits;
        private Curve _axisCurve;

        private Plane _attachmentPlane;
        private Plane _axisPlane; // Todo: now only the attachement plane is copied
        private bool _isLinear;
        private List<Mesh> _meshes;
        private int? _axisNumber; // Todo
        #endregion

        #region constructors
        public ExternalLinearAxis()
        {
            _startPoint = new Point3d(0, 0, 0);
            _endPoint = new Point3d(0, 0, 0);
            _attachmentDistance = 0;
            _axisNumber = null; // Todo
            Initilize();
        }

        //public ExternalLinearAxis(Point3d startPoint, Point3d endPoint, double attachmentDistance)
        //{
        //    _startPoint = startPoint;
        //    _endPoint = endPoint;
        //    _attachmentDistance = attachmentDistance;
        //    _axisNumber = -1; 
        //   Initilize();
        //}

        public ExternalLinearAxis(Point3d startPoint, Point3d endPoint, Plane attachmentPlane)
        {
            _startPoint = startPoint;
            _endPoint = endPoint;
            _attachmentPlane = attachmentPlane;
            _axisNumber = null; // Todo
            _attachmentDistance = _startPoint.DistanceTo(_attachmentPlane.Origin);
            Initilize();
        }

        public ExternalLinearAxis Duplicate()
        {
            ExternalLinearAxis dup = new ExternalLinearAxis(StartPoint, EndPoint, AttachmentPlane);
            return dup;
        }
        #endregion

        #region methods
        public bool IsValid
        {
            get
            {
                if (StartPoint == null) { return false; }
                if (EndPoint == null) { return false; }
                return true;
            }
        }

        public void GetAxis()
        {
            Vector3d axis = _endPoint - _startPoint;
            axis.Unitize();
            _axis = axis;
        }

        public void GetAttachmentOrigin()
        {
            _attachmentOrigin = AttachmentPlane.Origin;
        }

        public void GetMaxPositiveDistanceFromOrigin()
        {
            _maxPositiveDistanceFromOrigin = _attachmentPlane.Origin.DistanceTo(_endPoint);
        }

        public void GetMaxNegativeDistanceFromOrigin()
        {
            _maxNegativeDistanceFromOrigin = _attachmentPlane.Origin.DistanceTo(_startPoint);
        }

        public void GetAxisLimit()
        {
            _axisLimits = new Interval(-_maxNegativeDistanceFromOrigin, _maxPositiveDistanceFromOrigin);
        }

        public void GetAxisCurve()
        {
            Line line = new Line(_startPoint, _endPoint);
            _axisCurve = line.ToNurbsCurve();
            _axisCurve.Domain = new Interval(0, 1);
            
        }
        public void GetAxisPlane()
        {
            // To do
            _axisPlane = _attachmentPlane;
        }

        public override Plane CalculatePosition(double axisValue)
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
            Transform translateNow = Transform.Translation(_axis * value);
            Plane position = _attachmentPlane; // deep copy?
            position.Transform(translateNow);

            return position;
        }

        public override void Initilize()
        {
            GetAxis();
            GetAttachmentOrigin();
            GetAxisCurve();
            GetMaxPositiveDistanceFromOrigin();
            GetMaxNegativeDistanceFromOrigin();
            GetAxisLimit();
        }

        public override void ReInitilize()
        {
            Initilize();
        }

        #endregion

        #region properties
        public override Interval AxisLimits { get => _axisLimits; set => _axisLimits = value; }
        public override AxisType AxisType { get => AxisType.LINEAR; }
        public override Plane AttachmentPlane { get => _attachmentPlane; set => _attachmentPlane = value; }
        public override Plane AxisPlane { get => _axisPlane; set => _axisPlane = value; }
        public override int? AxisNumber { get => _axisNumber; set => _axisNumber = value; }
        public Point3d StartPoint { get => _startPoint; set => _startPoint = value; }
        public Point3d EndPoint { get => _endPoint; set => _endPoint = value; }
        public double AttachmentDistance { get => _attachmentDistance; set => _attachmentDistance = value; }
        public Vector3d Axis { get => _axis; set => _axis = value; }
        public Point3d AttachmentOrigin { get => _attachmentOrigin; set => _attachmentOrigin = value; }
        public double MaxNegativeDistanceFromOrigin { get => _maxNegativeDistanceFromOrigin; set => _maxNegativeDistanceFromOrigin = value; }
        public double MaxPositiveDistanceFromOrigin { get => _maxPositiveDistanceFromOrigin; set => _maxPositiveDistanceFromOrigin = value; }
        public Curve AxisCurve { get => _axisCurve; set => _axisCurve = value; }
        public bool IsLinear { get => _isLinear; set => _isLinear = value; }
        public List<Mesh> Meshes { get => _meshes; set => _meshes = value; }
        #endregion
    }
}

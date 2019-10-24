using System;
using System.Collections.Generic;

using Rhino.Geometry;

namespace RobotComponents.BaseClasses
{

    /// <summary>
    /// PathGenerator class
    /// </summary>
    public class PathGenerator
    {
        #region fields
        private RobotInfo _robotInfo;
        private InverseKinematics _inverseKinematics;
        private ForwardKinematics _forwardKinematics;

        List<Target> _targets;
        List<List<Double>> _internalAxisValues;
        List<List<Double>> _externalAxisValues;

        #endregion

        #region properties
        public bool IsValid
        {
            get
            {
                if (_robotInfo == null) { return false; };
                return true;
            }
        }

        public InverseKinematics InverseKinematics
        {
            get { return _inverseKinematics; }
            set { _inverseKinematics = value; }
        }


        public RobotInfo RobotInfo
        {
            get { return _robotInfo; }
            set { _robotInfo = value; }
        }

        public List<Target> Targets { get => _targets; set => _targets = value; }
        public List<List<double>> InternalAxisValues { get => _internalAxisValues; set => _internalAxisValues = value; }
        public List<List<double>> ExternalAxisValues { get => _externalAxisValues; set => _externalAxisValues = value; }


        #endregion

        #region constructors
        public PathGenerator()
        {
            _targets = new List<Target>();
            _internalAxisValues = new List<List<double>>();
            _externalAxisValues = new List<List<double>>();

        }

        public PathGenerator(RobotInfo robotInfo)
        {
            _targets = new List<Target>();
            _internalAxisValues = new List<List<double>>();
            _externalAxisValues = new List<List<double>>();

            _robotInfo = robotInfo;

            _inverseKinematics = new InverseKinematics(robotInfo);
            _forwardKinematics = new ForwardKinematics(robotInfo);


            //GetMovements();
            //GetMainTargets();
            //GetMovementTypes();
            //GetSubTargets();
        }

        public PathGenerator(List<Target> targets, List<List<double>> internalAxisValues, List<List<double>> externalAxisValues)
        {
            _targets = new List<Target>();
            _internalAxisValues = new List<List<double>>();
            _externalAxisValues = new List<List<double>>();

            this._targets = targets;
            this._internalAxisValues = internalAxisValues;
            this._externalAxisValues = externalAxisValues;
        }

        public PathGenerator Duplicate()
        {
            PathGenerator dup = new PathGenerator(RobotInfo);
            return dup;
        }
        #endregion


        #region methods
        /// <summary>
        /// Generates SubTargetValue List from MainTargets based on Interpolation Count
        /// </summary>
        public List<Target> GetTargets(List<Movement> movements, int interpolations)
        {
            List<Target> targets = new List<Target>();

            if (movements.Count > 1)
            {
                for (int i = 0; i < movements.Count - 1; i++)
                {
                    targets.Add(movements[i].Target);

                    Target _target1 = movements[i].Target;
                    Target _target2 = movements[i + 1].Target;


                    if (movements[i + 1].IsLinear == false)
                    {
                        // _inverseKinematics.Update(_target1);
                        InverseKinematics IK1 = new InverseKinematics(_target1, _robotInfo);
                        IK1.Calculate();
                        List<double> _target1InternalAxisValues = IK1.InternalAxisValues;
                        List<double> _target1ExternalAxisValues = IK1.ExternalAxisValues;

                        //_inverseKinematics.Update(_target2);
                        InverseKinematics IK2 = new InverseKinematics(_target2, _robotInfo);
                        IK2.Calculate();
                        List<double> _target2InternalAxisValues = IK2.InternalAxisValues;
                        List<double> _target2ExternalAxisValues = IK2.ExternalAxisValues;

                        // Calculate Axis Value Difference between both Targets
                        List<double> _internalAxisValueDifferences = new List<double>();
                        for (int j = 0; j < _target1InternalAxisValues.Count; j++)
                        {
                            double _difference = _target2InternalAxisValues[j] - _target1InternalAxisValues[j];
                            _internalAxisValueDifferences.Add(_difference);
                        }

                        List<double> _externalAxisValueDifferences = new List<double>();
                        for (int j = 0; j < _target1ExternalAxisValues.Count; j++)
                        {
                            double _difference = _target2ExternalAxisValues[j] - _target1ExternalAxisValues[j];
                            _externalAxisValueDifferences.Add(_difference);
                        }

                        // Calculates Axis Value Change per Interpolation Step
                        List<double> _internalAxisValueChange = new List<double>();
                        for (int j = 0; j < _target1InternalAxisValues.Count; j++)
                        {
                            double _valueChange = _internalAxisValueDifferences[j] / interpolations;
                            _internalAxisValueChange.Add(_valueChange);
                        }

                        List<double> _externalAxisValueChange = new List<double>();
                        for (int j = 0; j < _target1ExternalAxisValues.Count; j++)
                        {
                            double _valueChange = _externalAxisValueDifferences[j] / interpolations;
                            _externalAxisValueChange.Add(_valueChange);
                        }

                        // Calculates new Subtarget AxisValues for each Interpolation Step
                        for (int j = 0; j < interpolations; j++)
                        {
                            List<double> _internalAxisValues = new List<double>();
                            for (int k = 0; k < _target1InternalAxisValues.Count; k++)
                            {
                                double _valueAddition = _target1InternalAxisValues[k] + _internalAxisValueChange[k] * j;
                                _internalAxisValues.Add(_valueAddition);
                            }

                            List<double> _externalAxisValues = new List<double>();
                            for (int k = 0; k < _target1ExternalAxisValues.Count; k++)
                            {
                                double _valueAddition = _target1ExternalAxisValues[k] + _externalAxisValueChange[k] * j;
                                _externalAxisValues.Add(_valueAddition);
                            }


                            _forwardKinematics.Update(_internalAxisValues, _externalAxisValues);
                            _forwardKinematics.Calculate();
                            targets.Add(new Target(_target1.Name + "_interpolation_" + j, _forwardKinematics.TCPPlane));
                        }

                        /*
                        _inverseKinematics.Update(_target1);
                        _inverseKinematics.Calculate();
                        _target1InternalAxisValues = _inverseKinematics.InternalAxisValues;
                        _target1ExternalAxisValues = _inverseKinematics.ExternalAxisValues;

                        _inverseKinematics.Update(_target2);
                        _inverseKinematics.Calculate();
                        _target2InternalAxisValues = _inverseKinematics.InternalAxisValues;
                        _target2ExternalAxisValues = _inverseKinematics.ExternalAxisValues;

                        // Calculate Axis Value Difference between both Targets
                        List<double> _internalAxisValueDifferences = new List<double>();

                        for (int j = 0; j < _target1InternalAxisValues.Count; j++)
                        {
                            double _internalDifference = _target2InternalAxisValues[j] - _target1InternalAxisValues[j];
                            _internalAxisValueDifferences.Add(_internalDifference);
                        }

                        List<double> _externalAxisValueDifferences = new List<double>();

                        for (int j = 0; j < _target1ExternalAxisValues.Count; j++)
                        {
                            double _externalDifference = _target2ExternalAxisValues[j] - _target1ExternalAxisValues[j];
                            _externalAxisValueDifferences.Add(_externalDifference);
                        }

                        // Calculates Axis Value Change per Interpolation Step
                        List<double> _internalAxisValueChange = new List<double>();

                        for (int j = 0; j < _target1InternalAxisValues.Count; j++)
                        {
                            double _internalValueChange = _internalAxisValueDifferences[j] / interpolations;
                            _internalAxisValueChange.Add(_internalValueChange);

                        }

                        List<double> _externalAxisValueChange = new List<double>();


                        for (int j = 0; j < _target1ExternalAxisValues.Count; j++)
                        {

                            double _externalValueChange = _externalAxisValueDifferences[j] / interpolations;
                            _externalAxisValueChange.Add(_externalValueChange);
                        }



                        // Calculates new Subtarget AxisValues for each Interpolation Step
                        for (int j = 0; j < interpolations; j++)
                        {
                            List<double> _internalAxisValues = new List<double>();

                            for (int k = 0; k < _target1InternalAxisValues.Count; k++)
                            {
                                double _internalValueAddition = _target1InternalAxisValues[k] + _internalAxisValueChange[k] * j;
                                _internalAxisValues.Add(_internalValueAddition);
                            }

                            List<double> _externalAxisValues = new List<double>();

                            for (int k = 0; k < _target1ExternalAxisValues.Count; k++)
                            {
                                double _externalValueAddition = _target1ExternalAxisValues[k] + _externalAxisValueChange[k] * j;
                                _externalAxisValues.Add(_externalValueAddition);
                            }


                            _forwardKinematics.Update(_internalAxisValues, _externalAxisValues);
                            _forwardKinematics.Calculate();
                            targets.Add(new Target(_target1.Name + "_interpolation_" + j, _forwardKinematics.TCPPlane));
                        }
                             */
                    }


                    else
                    {
                        // MainTarget Plane Position Difference
                        Vector3d _posDif = _target2.Plane.Origin - _target1.Plane.Origin;

                        // MeinTarget Plane Position Difference per Step
                        Vector3d _posChange = _posDif / interpolations;

                        // MainTarget Planes Axis Differences
                        Vector3d _xAxisDif = _target2.Plane.XAxis - _target1.Plane.XAxis;
                        Vector3d _yAxisDif = _target2.Plane.YAxis - _target1.Plane.YAxis;
                        Vector3d _zAxisDif = _target2.Plane.ZAxis - _target1.Plane.ZAxis;

                        // MainTarget Planes Axis Change per Interpolation Step
                        Vector3d _xAxisChange = _xAxisDif / interpolations;
                        Vector3d _yAxisChange = _yAxisDif / interpolations;
                        Vector3d _zAxisChange = _zAxisDif / interpolations;

                        // SubTarget Plane Points for each Interpolation Step
                        List<Point3d> _planePoints = new List<Point3d>();
                        for (int j = 0; j < interpolations; j++)
                        {
                            _planePoints.Add(_target1.Plane.Origin + _posChange * j);
                        }

                        // SubTarget Plane Axis Directions
                        List<List<Vector3d>> _axisDirections = new List<List<Vector3d>>();
                        for (int k = 0; k < interpolations; k++)
                        {
                            List<Vector3d> _axisDir = new List<Vector3d>();
                            _axisDir.Add(_target1.Plane.XAxis + _xAxisChange * k);
                            _axisDir.Add(_target1.Plane.YAxis + _yAxisChange * k);
                            _axisDir.Add(_target1.Plane.ZAxis + _zAxisChange * k);
                            _axisDirections.Add(_axisDir);
                        }

                        // SubTarget Planes
                        List<Plane> _subTargetPlanes = new List<Plane>();
                        for (int l = 0; l < interpolations; l++)
                        {
                            Plane plane = new Plane(_planePoints[l], _axisDirections[l][0], _axisDirections[l][1]);
                            _subTargetPlanes.Add(plane);
                            targets.Add(new Target(_target1.Name + "_interpolation_" + l, plane));
                        }
                    }
                }
            }

            // Get TargetAxisAngle for last MainTarget
            Target _lastTarget = movements[movements.Count - 1].Target;
            targets.Add(_lastTarget);

            return targets;
        }

        /// <summary>
        /// Calculates new Points from SubtargetValues and creates Path
        /// </summary>
        public void Calculate(List<Action> actions, int interpolations)
        {
            List<Movement> movements = GetMovementsFromActions(actions);

            _targets.Clear();

            for (int i = 0; i < _internalAxisValues.Count; i++)
            {
                _internalAxisValues[i].Clear();
                _externalAxisValues[i].Clear();
            }
            _internalAxisValues.Clear();
            _externalAxisValues.Clear();

            _targets = GetTargets(movements, interpolations);
        }

        public List<Curve> GeneratePathCurves(List<Target> targets)
        {
            List<Curve> paths = new List<Curve>();

            List<Point3d> points = new List<Point3d>();
            for (int i = 0; i < targets.Count; i++)
            {

                points.Add(targets[i].Plane.Origin);

            }

            paths.Add(Curve.CreateInterpolatedCurve(points, 3));

            return paths;
        }

        private List<Movement> GetMovementsFromActions(List<Action> actions)
        {
            List<Movement> movements = new List<Movement>();

            for (int i = 0; i < actions.Count; i++)
            {
                if (actions[i] is Movement)
                {
                    movements.Add(((Movement)actions[i]));
                }
            }
            return movements;
        }

        #endregion
    }

}





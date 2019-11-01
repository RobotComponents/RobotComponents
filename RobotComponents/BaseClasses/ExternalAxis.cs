﻿using Rhino.Geometry;

namespace RobotComponents.BaseClasses
{
    public enum AxisType { LINEAR, ROTATIONAL };
    /// <summary>
    /// External axis class, main Class for external axis.
    /// An external axis (linear and rotational) can be attahced to a robot or a workobject
    /// </summary>
    public abstract class ExternalAxis
    {
        #region fields
        #endregion

        #region constructors

        #endregion

        #region properties
        public abstract Interval AxisLimits { get; set; }
        public abstract AxisType AxisType { get; }
        public abstract Plane AttachmentPlane { get; set; }
        public abstract Plane AxisPlane { get; set; }
        public abstract int? AxisNumber { get; set; }
        #endregion

        #region methods
        public abstract Plane CalculatePosition(double axisValue);
        public abstract void Initilize();
        public abstract void ReInitilize();
        #endregion
    }
}
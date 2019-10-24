using System;

using Rhino.Geometry;

namespace RobotComponents.BaseClasses
{
    /// <summary>
    /// Target class, defines Target Data.
    /// </summary>
    /// 

    public class Target : Action
    {
        #region fields
        string _name;
        Guid _instanceGuid;
        private Plane _plane;
        private Quaternion _quat;
        private int _axisConfig;
        private bool _isRobTarget;
        string _jointTargetName;
        string _robTargetName;

        // External axis values:
        // To do in inverse kinematics:
        // If external axis values are null: 9.0e9
        // If external axis values are null and an external axis is connected: use our own inverse kinematics
        // If external axis values are defined and an external axis is connected: use the user defined axis position
        private double? _Eax_a;
        private double? _Eax_b;
        private double? _Eax_c;
        private double? _Eax_d;
        private double? _Eax_e;
        private double? _Eax_f;
        #endregion

        #region constructors
        public Target()
        {
        }

        /// <summary>
        ///  Defines a robot target.
        /// </summary>
        /// <param name="name">Robot target name, must be unique.</param>
        /// <param name="plane">Robot target plane.</param>
        /// <param name="axisConfig">Robot target axisConfiguration.</param>
        public Target(string name,  Plane plane, int axisConfig)
        {
            this._name = name;
            this._plane = plane;
            this._quat = CalcQuaternion();
            this._axisConfig = axisConfig;
            this._robTargetName = name + "_rt";
            this._jointTargetName = name + "_jt";

            // External axis values
            this._Eax_a = null;
            this._Eax_b = null;
            this._Eax_c = null;
            this._Eax_d = null;
            this._Eax_e = null;
            this._Eax_f = null;
        }

        /// <summary>
        ///  Defines a robot target with default axis configuration.
        /// </summary>
        /// <param name="name">Robot target name, must be unique.</param>
        /// <param name="plane">Robot target plane.</param>
        public Target(string name, Plane plane)
        {
            this._name = name;
            this._plane = plane;
            this._quat = CalcQuaternion();
            this._axisConfig = 0;
            this._robTargetName = name + "_rt";
            this._jointTargetName = name + "_jt";

            // External axis values
            this._Eax_a = null;
            this._Eax_b = null;
            this._Eax_c = null;
            this._Eax_d = null;
            this._Eax_e = null;
            this._Eax_f = null;
        }

        public Target(string name, Guid instanceGuid, Plane plane, int axisConfig, bool isRobTarget)
        {
            this._name = name;
            this._instanceGuid = instanceGuid;
            this._plane = plane;
            this._quat = CalcQuaternion();
            this._axisConfig = axisConfig;
            this._isRobTarget = isRobTarget;
            this._robTargetName = name + "_rt";
            this._jointTargetName = name + "_jt";

            // External axis values
            this._Eax_a = null;
            this._Eax_b = null;
            this._Eax_c = null;
            this._Eax_d = null;
            this._Eax_e = null;
            this._Eax_f = null;
        }

        public Target Duplicate()
        {
            Target dup = new Target(Name, InstanceGuid, Plane, AxisConfig, IsRobTarget);
            return dup;
        }
        #endregion

        #region properties
        public bool IsValid
        {
            get
            {
                if (Plane == null) { return false; }
                if (Name == null) { return false; }
                return true;
            }
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public Guid InstanceGuid
        {
            get { return _instanceGuid; }
            set { _instanceGuid = value; }
        }
        public Plane Plane
        {
            get { return _plane; }
            set { _plane = value; }
        }
        public Quaternion Quat
        {
            get { return this._quat; }
            set { this._quat = value; }
        }
        public int AxisConfig
        {
            get { return this._axisConfig; }
            set { this._axisConfig = value; }
        }
        public bool IsRobTarget
        {
            get { return this._isRobTarget; }
            set { this._isRobTarget = value; }
        }
        public string JointTargetName
        {
            get { return this._jointTargetName; }
            set { this._jointTargetName = value; }
        }
        public string RobTargetName
        {
            get { return this._robTargetName; }
            set { this._robTargetName = value; }
        }
        #endregion

        #region method
        public override string InitRAPIDVar(RobotInfo robotInfo, string RAPIDcode)
        {
            return "";
        }

        public override string ToRAPIDFunction()
        {
            return ("");
        }

        public Quaternion CalcQuaternion()
        {
            Plane refPlane = new Plane(Plane.WorldXY);
            Quaternion quat = Quaternion.Rotation(refPlane, _plane);
            double[] quaternion = new double[] { quat.A, quat.B, quat.C, quat.D };
            return quat;
        }
        #endregion

    }

}

using System;

using Rhino.Geometry;

namespace RobotComponents.BaseClasses
{
    /// <summary>
    /// Target class, defines Target Data.
    /// </summary>
    public class Target : Action
    {
        #region fields
        string _name;
        Guid _instanceGuid;
        Plane _plane;
        Quaternion _quat;
        int _axisConfig;
        bool _isRobTarget;
        string _jointTargetName;
        string _robTargetName;

        // External axis values:
        // To do in inverse kinematics:
        // If external axis values are null: 9.0e9
        // If external axis values are null and an external axis is connected: use our own inverse kinematics
        // If external axis values are defined and an external axis is connected: use the user defined axis position
        double? _Eax_a;
        double? _Eax_b;
        double? _Eax_c;
        double? _Eax_d;
        double? _Eax_e;
        double? _Eax_f;
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
        /// <param name="referencePlane">Reference plane. Target planes will be reoriented from this plane to the origon (WorldXY). </param>
        /// <param name="axisConfig">Robot target axisConfiguration.</param>
        public Target(string name, Plane plane, Plane referencePlane, int axisConfig)
        {
            _name = name;
            _plane = plane;

            // Re-orient the plane to the reference plane
            Transform orient = Transform.ChangeBasis(referencePlane, Plane.WorldXY);
            _plane.Transform(orient);
            
            _quat = CalcQuaternion();
            _axisConfig = axisConfig;
            _robTargetName = name + "_rt";
            _jointTargetName = name + "_jt";

            // External axis values
            _Eax_a = null;
            _Eax_b = null;
            _Eax_c = null;
            _Eax_d = null;
            _Eax_e = null;
            _Eax_f = null;
        }

        /// <summary>
        ///  Defines a robot target.
        /// </summary>
        /// <param name="name">Robot target name, must be unique.</param>
        /// <param name="plane">Robot target plane.</param>
        /// <param name="axisConfig">Robot target axisConfiguration.</param>
        public Target(string name,  Plane plane, int axisConfig)
        {
            _name = name;
            _plane = plane;
            _quat = CalcQuaternion();
            _axisConfig = axisConfig;
            _robTargetName = name + "_rt";
            _jointTargetName = name + "_jt";

            // External axis values
            _Eax_a = null;
            _Eax_b = null;
            _Eax_c = null;
            _Eax_d = null;
            _Eax_e = null;
            _Eax_f = null;
        }

        /// <summary>
        ///  Defines a robot target with default axis configuration.
        /// </summary>
        /// <param name="name">Robot target name, must be unique.</param>
        /// <param name="plane">Robot target plane.</param>
        public Target(string name, Plane plane)
        {
            _name = name;
            _plane = plane;
            _quat = CalcQuaternion();
            _axisConfig = 0;
            _robTargetName = name + "_rt";
            _jointTargetName = name + "_jt";

            // External axis values
            _Eax_a = null;
            _Eax_b = null;
            _Eax_c = null;
            _Eax_d = null;
            _Eax_e = null;
            _Eax_f = null;
        }

        public Target(string name, Plane plane, int axisConfig, bool isRobTarget)
        {
            _name = name;
            _plane = plane;
            _quat = CalcQuaternion();
            _axisConfig = axisConfig;
            _isRobTarget = isRobTarget;
            _robTargetName = name + "_rt";
            _jointTargetName = name + "_jt";

            // External axis values
            _Eax_a = null;
            _Eax_b = null;
            _Eax_c = null;
            _Eax_d = null;
            _Eax_e = null;
            _Eax_f = null;
        }

        public Target Duplicate()
        {
            Target dup = new Target(Name, Plane, AxisConfig, IsRobTarget);
            return dup;
        }
        #endregion

        #region method
        public override string InitRAPIDVar(RobotInfo robotInfo, string RAPIDcode)
        {
            return "";
        }

        public override string ToRAPIDFunction(string robotToolName)
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
            get { return _quat; }
            set { _quat = value; }
        }
        public int AxisConfig
        {
            get { return _axisConfig; }
            set { _axisConfig = value; }
        }
        public bool IsRobTarget
        {
            get { return _isRobTarget; }
            set { _isRobTarget = value; }
        }
        public string JointTargetName
        {
            get { return _jointTargetName; }
            set { _jointTargetName = value; }
        }
        public string RobTargetName
        {
            get { return _robTargetName; }
            set { _robTargetName = value; }
        }
        #endregion
    }

}

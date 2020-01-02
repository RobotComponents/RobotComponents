using System;
using System.Collections.Generic;

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
        string _jointTargetName;
        string _robTargetName;

        // External axis values:
        // To do in inverse kinematics:
        // If external axis values are null: 9.0e9
        // If external axis values are null and an external axis is connected: use our own inverse kinematics
        // If external axis values are defined and an external axis is connected: use the user defined axis position
        double _Eax_a;
        double _Eax_b;
        double _Eax_c;
        double _Eax_d;
        double _Eax_e;
        double _Eax_f;
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
            
            _axisConfig = axisConfig;
            _robTargetName = name + "_rt";
            _jointTargetName = name + "_jt";

            // External axis values
            _Eax_a = 9e9;
            _Eax_b = 9e9;
            _Eax_c = 9e9;
            _Eax_d = 9e9;
            _Eax_e = 9e9;
            _Eax_f = 9e9;

            Initialize();
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
            _axisConfig = axisConfig;
            _robTargetName = name + "_rt";
            _jointTargetName = name + "_jt";

            // External axis values
            _Eax_a = 9e9;
            _Eax_b = 9e9;
            _Eax_c = 9e9;
            _Eax_d = 9e9;
            _Eax_e = 9e9;
            _Eax_f = 9e9;

            Initialize();
        }

        /// <summary>
        /// Defines a robot target with default axis configuration.
        /// </summary>
        /// <param name="name">Robot target name, must be unique.</param>
        /// <param name="plane">Robot target plane.</param>
        public Target(string name, Plane plane)
        {
            _name = name;
            _plane = plane;
            _axisConfig = 0;
            _robTargetName = name + "_rt";
            _jointTargetName = name + "_jt";

            // External axis values
            _Eax_a = 9e9;
            _Eax_b = 9e9;
            _Eax_c = 9e9;
            _Eax_d = 9e9;
            _Eax_e = 9e9;
            _Eax_f = 9e9;

            Initialize();
        }

        /// <summary>
        /// Defines a robot target with default axis configuration.
        /// </summary>
        /// <param name="name">Robot target name, must be unique.</param>
        /// <param name="plane">Robot target plane.</param>
        public Target(string name, Plane plane, List<double> Eax)
        {
            _name = name; 
            _plane = plane;
            _axisConfig = 0;
            _robTargetName = name + "_rt";
            _jointTargetName = name + "_jt";

            Eax = CheckExternalAxisValues(Eax);

            // External axis values
            _Eax_a = Eax[0];
            _Eax_b = Eax[1];
            _Eax_c = Eax[2];
            _Eax_d = Eax[3];
            _Eax_e = Eax[4];
            _Eax_f = Eax[5];

            Initialize();
        }


        public Target(string name, Plane plane, Plane referencePlane, int axisConfig, double Eax_a, double Eax_b, double Eax_c, double Eax_d, double Eax_e, double Eax_f)
        {
            _name = name;
            _plane = plane;

            // Re-orient the plane to the reference plane
            Transform orient = Transform.ChangeBasis(referencePlane, Plane.WorldXY);
            _plane.Transform(orient);

            _axisConfig = axisConfig;
            _robTargetName = name + "_rt";
            _jointTargetName = name + "_jt";

            // External axis values
            _Eax_a = Eax_a;
            _Eax_b = Eax_b;
            _Eax_c = Eax_c;
            _Eax_d = Eax_d;
            _Eax_e = Eax_e;
            _Eax_f = Eax_f;

            Initialize();
        }

        public Target(string name, Plane plane, Plane referencePlane, int axisConfig, List<double> Eax)
        {
            _name = name;
            _plane = plane;

            // Re-orient the plane to the reference plane
            Transform orient = Transform.ChangeBasis(referencePlane, Plane.WorldXY);
            _plane.Transform(orient);

            _axisConfig = axisConfig;
            _robTargetName = name + "_rt";
            _jointTargetName = name + "_jt";

            Eax = CheckExternalAxisValues(Eax);

            // External axis values
            _Eax_a = Eax[0];
            _Eax_b = Eax[1];
            _Eax_c = Eax[2];
            _Eax_d = Eax[3];
            _Eax_e = Eax[4];
            _Eax_f = Eax[5];

            Initialize();
        }

        public Target(string name, Plane plane, Plane referencePlane, int axisConfig, double[] Eax)
        {
            _name = name;
            _plane = plane;

            // Re-orient the plane to the reference plane
            Transform orient = Transform.ChangeBasis(referencePlane, Plane.WorldXY);
            _plane.Transform(orient);

            _axisConfig = axisConfig;
            _robTargetName = name + "_rt";
            _jointTargetName = name + "_jt";

            Eax = CheckExternalAxisValues(Eax);

            // External axis values
            _Eax_a = Eax[0];
            _Eax_b = Eax[1];
            _Eax_c = Eax[2];
            _Eax_d = Eax[3];
            _Eax_e = Eax[4];
            _Eax_f = Eax[5];

            Initialize();
        }

        public Target(string name, Plane plane, int axisConfig, List<double> Eax)
        {
            _name = name;
            _plane = plane;
            _axisConfig = axisConfig;
            _robTargetName = name + "_rt";
            _jointTargetName = name + "_jt";

            Eax = CheckExternalAxisValues(Eax);

            // External axis values
            _Eax_a = Eax[0];
            _Eax_b = Eax[1];
            _Eax_c = Eax[2];
            _Eax_d = Eax[3];
            _Eax_e = Eax[4];
            _Eax_f = Eax[5];

            Initialize();
        }

        public Target Duplicate()
        {
            Target dup = new Target(Name, Plane, AxisConfig, ExternalAxisValues);
            return dup;
        }
        #endregion

        #region method
        public void Initialize()
        {
            _quat = CalcQuaternion();
        }

        public void ReInitialize()
        {
            Initialize();
        }

        private List<double> CheckExternalAxisValues(List<double> axisValues)
        {
            List<double> result = new List<double>();
            int n = Math.Min(axisValues.Count, 6);

            // Copy definied external axis values
            for (int i = 0; i < n; i++)
            {
                result.Add(axisValues[i]);
            }

            // Add up missing external axisValues
            for (int i = n; i < 6; i++)
            {
                result.Add(9e9);
            }

            return result;
        }

        private double[] CheckExternalAxisValues(double[] axisValues)
        {
            double[] result = new double[6];
            int n = Math.Min(axisValues.Length, 6);

            // Copy definied external axis values
            for (int i = 0; i < n; i++)
            {
                result[i] = axisValues[i];
            }

            // Add up missing external axisValues
            for (int i = n; i < 6; i++)
            {
                result[i] = 9e9;
            }

            return result;
        }

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

        public List<double> ExternalAxisValues
        {
            get
            {
                List<double> ExternalAxisValues = new List<double> { _Eax_a, _Eax_b, _Eax_c, _Eax_d, _Eax_e, _Eax_f };
                return ExternalAxisValues;
            }
        }

        public double ExternalAxisValueA
        {
            get { return _Eax_a; }
            set { _Eax_a = value; }
        }

        public double ExternalAxisValueB
        {
            get { return _Eax_b; }
            set { _Eax_b = value; }
        }

        public double ExternalAxisValueC
        {
            get { return _Eax_c; }
            set { _Eax_c = value; }
        }

        public double ExternalAxisValueD
        {
            get { return _Eax_d; }
            set { _Eax_d = value; }
        }

        public double ExternalAxisValueE
        {
            get { return _Eax_e; }
            set { _Eax_e = value; }
        }

        public double ExternalAxisValueF
        {
            get { return _Eax_f; }
            set { _Eax_f = value; }
        }
        #endregion
    }

}

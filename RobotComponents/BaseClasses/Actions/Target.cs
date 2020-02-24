// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/EDEK-UniKassel/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
// Rhino Libs
using Rhino.Geometry;

namespace RobotComponents.BaseClasses.Actions
{
    /// <summary>
    /// Target class, defines target data. The target data is used to define the position of the robot and external axes.
    /// </summary>
    public class Target : Action
    {
        #region fields
        private string _name; // target variable name
        private Plane _plane; // target plane (defines the required position and orientation of the tool)
        private Quaternion _quat; // target plane orientation (as quarternion)
        private int _axisConfig; // the axis configuration of the robot 

        private double _Eax_a; // the user override position of the external logical axis “a” expressed in degrees or mm 
        private double _Eax_b; // the user override position of the external logical axis “b” expressed in degrees or mm
        private double _Eax_c; // the user override position of the external logical axis “c” expressed in degrees or mm
        private double _Eax_d; // the user override position of the external logical axis “d” expressed in degrees or mm
        private double _Eax_e; // the user override position of the external logical axis “e” expressed in degrees or mm
        private double _Eax_f; // the user override position of the external logical axis “f” expressed in degrees or mm
        #endregion

        #region constructors
        /// <summary>
        /// Defines an empty Target object.
        /// </summary>
        public Target()
        {
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
        /// Defines a robot target with a user defined axis configuration.
        /// </summary>
        /// <param name="name">Robot target name, must be unique.</param>
        /// <param name="plane">Robot target plane.</param>
        /// <param name="axisConfig">Robot axis configuration as a number (0-7).</param>
        public Target(string name, Plane plane, int axisConfig)
        {
            _name = name;
            _plane = plane;
            _axisConfig = axisConfig;

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
        /// Defines a robot target that will be re-oriented from the reference coordinate system to the global coordinate system.
        /// </summary>
        /// <param name="name">Robot target name, must be unique.</param>
        /// <param name="plane">Robot target plane.</param>
        /// <param name="referencePlane">Reference plane. Target planes will be reoriented from this plane to the origon (WorldXY). </param>
        /// <param name="axisConfig">Robot axis configuration as a number (0-7).</param>
        public Target(string name, Plane plane, Plane referencePlane, int axisConfig)
        {
            _name = name;
            _plane = plane;            
            _axisConfig = axisConfig;

            // Re-orient the plane to the reference plane
            Transform orient = Transform.PlaneToPlane(referencePlane, Plane.WorldXY);
            _plane.Transform(orient);

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
        /// Defines a robot target with user defined (override) external axis values. 
        /// </summary>
        /// <param name="name">Robot target name, must be unique.</param>
        /// <param name="plane">Robot target plane.</param>
        /// <param name="referencePlane">Reference plane. Target planes will be reoriented from this plane to the origin (WorldXY). </param>
        /// <param name="axisConfig">Robot axis configuration as a number (0-7).</param>
        /// <param name="Eax_a"></param>
        /// <param name="Eax_b"></param>
        /// <param name="Eax_c"></param>
        /// <param name="Eax_d"></param>
        /// <param name="Eax_e"></param>
        /// <param name="Eax_f"></param>
        public Target(string name, Plane plane, Plane referencePlane, int axisConfig, double Eax_a, double Eax_b = 9e9, double Eax_c = 9e9, double Eax_d = 9e9, double Eax_e = 9e9, double Eax_f = 9e9)
        {
            _name = name;
            _plane = plane;

            // Re-orient the plane to the reference plane
            Transform orient = Transform.PlaneToPlane(referencePlane, Plane.WorldXY);
            _plane.Transform(orient);

            _axisConfig = axisConfig;

            // External axis values
            _Eax_a = Eax_a;
            _Eax_b = Eax_b;
            _Eax_c = Eax_c;
            _Eax_d = Eax_d;
            _Eax_e = Eax_e;
            _Eax_f = Eax_f;

            Initialize();
        }

        /// <summary>
        /// Defines a robot target with user defined (override) external axis values. 
        /// </summary>
        /// <param name="name">Robot target name, must be unique.</param>
        /// <param name="plane">Robot target plane.</param>
        /// <param name="axisConfig">Robot target axis configuration as a number (0-7).</param>
        /// <param name="Eax">The user defined external axis values as a list.</param>
        public Target(string name, Plane plane, int axisConfig, List<double> Eax)
        {
            _name = name;
            _plane = plane;
            _axisConfig = axisConfig;

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

        /// <summary>
        /// Defines a robot target with user defined (override) external axis values.
        /// Target planes will be re-oriented from the reference coordinate system to the global coordinate system.
        /// </summary>
        /// <param name="name">Robot target name, must be unique.</param>
        /// <param name="plane">Robot target plane.</param>
        /// <param name="referencePlane">Reference plane. Target planes will be reoriented from this plane to the origon (WorldXY). </param>
        /// <param name="axisConfig">Robot target axis configuration as a number (0-7).</param>
        /// <param name="Eax">The user defined external axis values as a list.</param>
        public Target(string name, Plane plane, Plane referencePlane, int axisConfig, List<double> Eax)
        {
            _name = name;
            _plane = plane;
            _axisConfig = axisConfig;

            // Re-orient the plane to the reference plane
            Transform orient = Transform.PlaneToPlane(referencePlane, Plane.WorldXY);
            _plane.Transform(orient);

            // Check the length of the list with external axis values
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

        /// <summary>
        /// Defines a robot target with user defined (override) external axis values.
        /// Target planes will be re-oriented from the reference coordinate system to the global coordinate system.
        /// </summary>
        /// <param name="name">Robot target name, must be unique.</param>
        /// <param name="plane">Robot target plane.</param>
        /// <param name="referencePlane">Reference plane. Target planes will be reoriented from this plane to the origon (WorldXY). </param>
        /// <param name="axisConfig">Robot target axis configuration as a number (0-7).</param>
        /// <param name="Eax">The user defined external axis values as an array.</param>
        public Target(string name, Plane plane, Plane referencePlane, int axisConfig, double[] Eax)
        {
            _name = name;
            _plane = plane;
            _axisConfig = axisConfig;

            // Re-orient the plane to the reference plane
            Transform orient = Transform.PlaneToPlane(referencePlane, Plane.WorldXY);
            _plane.Transform(orient);

            // Check the length of the araay with external axis values
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

        /// <summary>
        /// Creates a new target by duplicating an existing target. 
        /// This creates a deep copy of the existing target. 
        /// </summary>
        /// <param name="target"> The target that should be duplicated. </param>
        public Target(Target target)
        {
            _name = target.Name;
            _plane = new Plane(target.Plane);
            _axisConfig = target.AxisConfig;
            _quat = target.Quat;

            // External axis values
            _Eax_a = target.ExternalAxisValueA;
            _Eax_b = target.ExternalAxisValueB;
            _Eax_c = target.ExternalAxisValueC;
            _Eax_d = target.ExternalAxisValueD;
            _Eax_e = target.ExternalAxisValueE;
            _Eax_f = target.ExternalAxisValueF;
        }

        /// <summary>
        /// Method to duplicate the Target object.
        /// </summary>
        /// <returns>Returns a deep copy of the Target object.</returns>
        public Target Duplicate()
        {
            return new Target(this);
        }

        /// <summary>
        /// A method to duplicate the Target object to an Action object. 
        /// </summary>
        /// <returns> Returns a deep copy of the Target object as an Action object. </returns>
        public override Action DuplicateAction()
        {
            return new Target(this) as Action;
        }
        #endregion

        #region method
        /// <summary>
        /// A method that calls all the other methods that are needed to initialize the data that is needed to construct a valid target object. 
        /// </summary>
        private void Initialize()
        {
            _quat = CalcQuaternion();
        }

        /// <summary>
        /// A method that can be called to reinitialize all the data that is needed to construct a valid target object.
        /// </summary>
        public void ReInitialize()
        {
            Initialize();
        }

        /// <summary>
        /// Method that checks the list with external axis values. 
        /// Always returns a list with 6 external axis values. 
        /// For missing values 9e9 (not connected) will be used. 
        /// </summary>
        /// <param name="axisValues">A list with the external axis values.</param>
        /// <returns>Returns a list with 6 external axis values.</returns>
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

        /// <summary></summary>
        /// Method that checks the array with external axis values. 
        /// Always returns a list with 6 external axis values. 
        /// For missing values 9e9 (not connected) will be used. 
        /// <param name="axisValues">A list with the external axis values.</param>
        /// <returns>Returns an array with 6 external axis values.</returns>
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

        /// <summary>
        /// Calculate the four quarternion coefficients of the target plane needed for writing the RAPID code. 
        /// </summary>
        /// <returns>The four quarternion coefficients of the target plane.</returns>
        private Quaternion CalcQuaternion()
        {
            Plane refPlane = new Plane(Plane.WorldXY);
            Quaternion quat = Quaternion.Rotation(refPlane, _plane);
            return quat;
        }

        /// <summary>
        /// Used to create variable definitions in the RAPID Code. It is typically called inside the CreateRAPIDCode() method of the RAPIDGenerator class.
        /// </summary>
        /// <param name="RAPIDGenerator"> Defines the RAPIDGenerator. </param>
        public override void InitRAPIDVar(RAPIDGenerator RAPIDGenerator)
        {
        }

        /// <summary>
        /// Used to create action instructions in the RAPID Code. It is typically called inside the CreateRAPIDCode() method of the RAPIDGenerator class.
        /// </summary>
        /// <param name="RAPIDGenerator"> Defines the RAPIDGenerator. </param>
        public override void ToRAPIDFunction(RAPIDGenerator RAPIDGenerator)
        {
        }
        #endregion

        #region properties
        /// <summary>
        /// A boolean that indicuate if the Target object is valid.
        /// </summary>
        public override bool IsValid
        {
            get
            {
                if (Plane == null) { return false; }
                if (Plane == Plane.Unset) { return false; }
                if (Name == null) { return false; }
                if (Name == "") { return false; }
                if (AxisConfig < 0) { return false; }
                if (AxisConfig > 7) { return false; }
                return true;
            }
        }

        /// <summary>
        /// The target variable name, must be unique.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// The position and orientation of the tool center as a plane. 
        /// </summary>
        public Plane Plane
        {
            get 
            { 
                return _plane; 
            }
            set 
            { 
                _plane = value;
                ReInitialize();
            }
        }

        /// <summary>
        /// The orientation of the tool, expressed in the form of a quaternion (q1, q2, q3, and q4). 
        /// </summary>
        public Quaternion Quat
        {
            get { return _quat; }
            set { _quat = value; }
        }

        /// <summary>
        /// The axis configuration of the robot (0-7).
        /// </summary>
        public int AxisConfig
        {
            get { return _axisConfig; }
            set { _axisConfig = value; }
        }

        /// <summary>
        /// The robot target name when it is used as a joint target.
        /// </summary>
        public string JointTargetName
        {
            get { return Name + "_jt"; }
        }

        /// <summary>
        /// The robot target name when it is used as a robot target.
        /// </summary>
        public string RobTargetName
        {
            get { return Name + "_rt"; }
        }

        /// <summary>
        /// The external axis values as a list.
        /// </summary>
        public List<double> ExternalAxisValues
        {
            get
            {
                List<double> ExternalAxisValues = new List<double> { _Eax_a, _Eax_b, _Eax_c, _Eax_d, _Eax_e, _Eax_f };
                return ExternalAxisValues;
            }
            set 
            {
                // Check the length of the araay with external axis values
                List<double> Eax = CheckExternalAxisValues(value);

                // External axis values
                _Eax_a = Eax[0];
                _Eax_b = Eax[1];
                _Eax_c = Eax[2];
                _Eax_d = Eax[3];
                _Eax_e = Eax[4];
                _Eax_f = Eax[5];
            }
        }

        /// <summary>
        /// The position of the external logical axis “a” expressed in degrees or mm (depending on the type of axis).
        /// If 9e9 is used the inverse kinematics will calculate the axis value.
        /// </summary>
        public double ExternalAxisValueA
        {
            get { return _Eax_a; }
            set { _Eax_a = value; }
        }

        /// <summary>
        /// The position of the external logical axis “b” expressed in degrees or mm (depending on the type of axis).
        /// If 9e9 is used the inverse kinematics will calculate the axis value.
        /// </summary>
        public double ExternalAxisValueB
        {
            get { return _Eax_b; }
            set { _Eax_b = value; }
        }

        /// <summary>
        /// The position of the external logical axis “c” expressed in degrees or mm (depending on the type of axis).
        /// If 9e9 is used the inverse kinematics will calculate the axis value.
        /// </summary>
        public double ExternalAxisValueC
        {
            get { return _Eax_c; }
            set { _Eax_c = value; }
        }

        /// <summary>
        /// The position of the external logical axis “d” expressed in degrees or mm (depending on the type of axis).
        /// If 9e9 is used the inverse kinematics will calculate the axis value.
        /// </summary>
        public double ExternalAxisValueD
        {
            get { return _Eax_d; }
            set { _Eax_d = value; }
        }

        /// <summary>
        /// The position of the external logical axis “e” expressed in degrees or mm (depending on the type of axis).
        /// If 9e9 is used the inverse kinematics will calculate the axis value.
        /// </summary>
        public double ExternalAxisValueE
        {
            get { return _Eax_e; }
            set { _Eax_e = value; }
        }

        /// <summary>
        /// The position of the external logical axis “f” expressed in degrees or mm (depending on the type of axis).
        /// If 9e9 is used the inverse kinematics will calculate the axis value.
        /// </summary>
        public double ExternalAxisValueF
        {
            get { return _Eax_f; }
            set { _Eax_f = value; }
        }
        #endregion
    }

}

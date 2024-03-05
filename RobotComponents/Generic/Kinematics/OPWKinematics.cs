// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs 
using System;
using System.Linq;
using System.Collections.Generic;
// Rhino Libs
using Rhino.Geometry;

namespace RobotComponents.Generic.Kinematics
{
    /// <summary>
    /// Represents the OPW Kinematics class.
    /// </summary>
    /// <remarks>
    /// Based on the paper 'An Analytical Solution of the Inverse Kinematics Problem of 
    /// Industrial Serial Manipulators with an Ortho-parallel Basis and a Spherical Wrist' 
    /// by Mathias Brandstötter, Arthur Angerer, and Michael Hofbaur.
    /// 
    /// Solution order:
    /// 
    /// Sol.    Wrist center            Wrist center            Axis 5 angle
    ///         relative to axis 1      relative to lower arm
    ///         
    /// 0       In front of             In front of             Positive
    /// 1       In front of             Behind                  Positive
    /// 2       Behind                  In front of             Positive
    /// 3       Behind                  Behind                  Positive
    /// 4       In front of             In front of             Negative
    /// 5       In front of             Behind                  Negative 
    /// 6       Behind                  In front of             Negative
    /// 7       Behind                  Behind                  Negative        
    /// </remarks>
    public class OPWKinematics
    {
        #region fields
        private readonly double[][] _solutions = Enumerable.Range(0, 8).Select(item => new double[6]).ToArray();
        private readonly bool[] _shoulderSingularities = Enumerable.Repeat(false, 8).ToArray();
        private readonly bool[] _elbowSingularities = Enumerable.Repeat(false, 8).ToArray();
        private readonly bool[] _wristSingularities = Enumerable.Repeat(false, 8).ToArray();
        private double[] _offsets = Enumerable.Repeat(0.0, 6).ToArray();
        private int[] _signs = Enumerable.Repeat(1, 6).ToArray();

        // Robot parameters
        private double _a1 = 0;
        private double _a2 = 0;
        private double _b = 0;
        private double _c1 = 0;
        private double _c2 = 0;
        private double _c3 = 0;
        private double _c4 = 0;
        private double _k = 0;
        private double _k2 = 0;
        private double _psi3 = 0;

        // Constants
        private const double _pi = Math.PI;
        private const double _twoPi = 2 * Math.PI;
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the OPW Kinematics class
        /// </summary>
        public OPWKinematics()
        {

        }
        #endregion

        #region methods
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns> A string that represents the current object. </returns>
        public override string ToString()
        {
            if (!IsValid)
            {
                return "Invalid OPW Kinematics";
            }
            else
            {
                return "OPW Kinematics";
            }
        }

        /// <summary>
        /// Updates the robot parameters.
        /// </summary>
        /// <remarks>
        /// Method is called when the property A2 or C3 changes. 
        /// </remarks>
        private void UpdateRobotParameters()
        {
            _psi3 = Math.Atan2(_a2, _c3);
            _k2 = (_a2 * _a2) + (_c3 * _c3);
            _k = Math.Sqrt(_k2);
        }

        /// <summary>
        /// Calculates the end plane of joint 6 for a given pose.
        /// </summary>
        /// <param name="pose"> The pose as a collection with 6 rotations in radians. </param>
        /// <returns> The end plane of the 6th joint. </returns>
        public Plane Forward(IList<double> pose)
        {
            return Forward(pose, out _);
        }

        /// <summary>
        /// Calculates the end plane of joint 6 for a given pose.
        /// </summary>
        /// <param name="pose"> The pose as a collection with 6 rotations in radians. </param>
        /// <param name="wristPosition"> The wrist position for the given pose. </param>
        /// <returns> The end plane of the 6th joint. </returns>
        public Plane Forward(IList<double> pose, out Point3d wristPosition)
        {
            // Check
            if (pose.Count < 6) { throw new Exception("Pose does not contain six rotation values."); }

            // Pose
            double[] theta = pose.ToArray();

            // Corrections
            theta[0] = (theta[0] * _signs[0]) - _offsets[0];
            theta[1] = (theta[1] * _signs[1]) - _offsets[1];
            theta[2] = (theta[2] * _signs[2]) - _offsets[2];
            theta[3] = (theta[3] * _signs[3]) - _offsets[3];
            theta[4] = (theta[4] * _signs[4]) - _offsets[4];
            theta[5] = (theta[5] * _signs[5]) - _offsets[5];

            // Sine values
            double sin1 = Math.Sin(theta[0]);
            double sin2 = Math.Sin(theta[1]);
            double sin3 = Math.Sin(theta[2]);
            double sin4 = Math.Sin(theta[3]);
            double sin5 = Math.Sin(theta[4]);
            double sin6 = Math.Sin(theta[5]);

            // Cosine values
            double cos1 = Math.Cos(theta[0]);
            double cos2 = Math.Cos(theta[1]);
            double cos3 = Math.Cos(theta[2]);
            double cos4 = Math.Cos(theta[3]);
            double cos5 = Math.Cos(theta[4]);
            double cos6 = Math.Cos(theta[5]);

            // Wrist position on plane
            double cx1 = (_c2 * sin2) + (_k * Math.Sin(theta[1] + theta[2] + _psi3)) + _a1;
            double cy1 = _b;
            double cz1 = (_c2 * cos2) + (_k * Math.Cos(theta[1] + theta[2] + _psi3));

            // Wrist position
            double cx0 = (cx1 * cos1) - (cy1 * sin2);
            double cy0 = (cx1 * sin1) + (cy1 * cos2);
            double cz0 = cz1 + _c1;

            // Matrix Rce: Wrist orientation
            Matrix rce = new Matrix(3, 3);
            rce[0, 0] = (cos4 * cos5 * cos6) - (sin4 * sin6);
            rce[0, 1] = (-cos4 * cos5 * sin6) - (sin4 * cos6);
            rce[0, 2] = cos4 * sin5;
            rce[1, 0] = (sin4 * cos5 * cos6) + (cos4 * sin6);
            rce[1, 1] = (-sin4 * cos5 * sin6) + (cos4 * cos6);
            rce[1, 2] = sin4 * sin5;
            rce[2, 0] = -sin5 * cos6;
            rce[2, 1] = sin5 * sin6;
            rce[2, 2] = cos5;

            // Matrix Roc
            Matrix roc = new Matrix(3, 3);                              // Alternative form
            roc[0, 0] = (cos1 * cos2 * cos3) - (cos1 * sin2 * sin3);    // cos(x1) cos(x2 + x3)
            roc[0, 1] = -sin1;
            roc[0, 2] = (cos1 * cos2 * sin3) + (cos1 * sin2 * cos3);    // cos(x1) sin(x2 + x3)
            roc[1, 0] = (sin1 * cos2 * cos3) - (sin1 * sin2 * sin3);    // sin(x1) cos(x2 + x3)
            roc[1, 1] = cos1;
            roc[1, 2] = (sin1 * cos2 * sin3) + (sin1 * sin2 * cos3);    // sin(x1) sin(x2 + x3)
            roc[2, 0] = (-sin2 * cos3) - (cos2 * sin3);                 // -sin(x2 + x3)
            roc[2, 1] = 0;
            roc[2, 2] = (-sin2 * sin3) + (cos2 * cos3);                 // cos(x2 + x3)

            // Matrix Roe: End plane orientation
            Matrix roe = roc * rce;

            // End plane position
            double ux0 = cx0 + (_c4 * roe[0, 2]);
            double uy0 = cy0 + (_c4 * roe[1, 2]);
            double uz0 = cz0 + (_c4 * roe[2, 2]);

            // Return wrist position via out param
            wristPosition = new Point3d(cx0, cy0, cz0);

            // Return end plane
            Point3d origin = new Point3d(ux0, uy0, uz0);
            Vector3d xAxis = new Vector3d(roe[0, 0], roe[1, 0], roe[2, 0]);
            Vector3d yAxis = new Vector3d(roe[0, 1], roe[1, 1], roe[2, 1]);
            Plane endPlane = new Plane(origin, xAxis, yAxis);

            return endPlane;
        }

        /// <summary>
        /// Calculates the inverse kinematic solutions.
        /// </summary>
        /// <param name="endPlane"> The end plane of joint 6. </param>
        public void Inverse(Plane endPlane)
        {
            // Position
            double ux0 = endPlane.Origin.X;
            double uy0 = endPlane.Origin.Y;
            double uz0 = endPlane.Origin.Z;

            // Orientation
            double e11 = endPlane.XAxis.X;
            double e12 = endPlane.YAxis.X;
            double e13 = endPlane.ZAxis.X;
            double e21 = endPlane.XAxis.Y;
            double e22 = endPlane.YAxis.Y;
            double e23 = endPlane.ZAxis.Y;
            double e31 = endPlane.XAxis.Z;
            double e32 = endPlane.YAxis.Z;
            double e33 = endPlane.ZAxis.Z;

            // Wrist position
            double cx0 = ux0 - (e13 * _c4);
            double cy0 = uy0 - (e23 * _c4);
            double cz0 = uz0 - (e33 * _c4);

            // Positioning parameters for joint 1, 2 and 3
            double nx1 = Math.Sqrt((cx0 * cx0) + (cy0 * cy0) - (_b * _b)) - _a1;
            double s1_2 = (nx1 * nx1) + ((cz0 - _c1) * (cz0 - _c1));
            double s2_2 = ((nx1 + (2.0 * _a1)) * (nx1 + (2.0 * _a1))) + ((cz0 - _c1) * (cz0 - _c1));
            double s1 = Math.Sqrt(s1_2);
            double s2 = Math.Sqrt(s2_2);
            double psi1 = Math.Atan2(nx1, cz0 - _c1);
            double psi2_i = Math.Acos((s1_2 + (_c2 * _c2) - _k2) / (2.0 * s1 * _c2));
            double psi2_ii = Math.Acos((s2_2 + (_c2 * _c2) - _k2) / (2.0 * s2 * _c2));
            double acos1 = Math.Acos((s1_2 - (_c2 * _c2) - _k2) / (2.0 * _k * _c2));
            double acos2 = Math.Acos((s2_2 - (_c2 * _c2) - _k2) / (2.0 * _k * _c2));
            double atan1 = Math.Atan2(cy0, cx0);
            double atan2 = Math.Atan2(_b, nx1 + _a1);
            double atan3 = Math.Atan2(nx1 + (2.0 * _a1), cz0 - _c1);

            // Check for Nan values (singularities)
            if (double.IsNaN(acos1)) { acos1 = 0; }
            if (double.IsNaN(acos2)) { acos2 = 0; }
            if (double.IsNaN(psi2_i)) { psi2_i = 0; }
            if (double.IsNaN(psi2_ii)) { psi2_ii = 0; }

            // Joint position 1
            double theta1_i = atan1 - atan2;
            double theta1_ii = atan1 + atan2 - _pi;
            _solutions[0][0] = theta1_i;
            _solutions[1][0] = theta1_i;
            _solutions[2][0] = theta1_ii;
            _solutions[3][0] = theta1_ii;
            _solutions[4][0] = theta1_i;
            _solutions[5][0] = theta1_i;
            _solutions[6][0] = theta1_ii;
            _solutions[7][0] = theta1_ii;

            // Joint position 2
            double theta2_i = -psi2_i + psi1;
            double theta2_ii = psi2_i + psi1;
            double theta2_iii = -psi2_ii - atan3;
            double theta2_iv = psi2_ii - atan3;
            _solutions[0][1] = theta2_i;
            _solutions[1][1] = theta2_ii;
            _solutions[2][1] = theta2_iii;
            _solutions[3][1] = theta2_iv;
            _solutions[4][1] = theta2_i;
            _solutions[5][1] = theta2_ii;
            _solutions[6][1] = theta2_iii;
            _solutions[7][1] = theta2_iv;

            // Joint position 3
            double theta3_i = acos1 - _psi3;
            double theta3_ii = -acos1 - _psi3;
            double theta3_iii = acos2 - _psi3;
            double theta3_iv = -acos2 - _psi3;
            _solutions[0][2] = theta3_i;
            _solutions[1][2] = theta3_ii;
            _solutions[2][2] = theta3_iii;
            _solutions[3][2] = theta3_iv;
            _solutions[4][2] = theta3_i;
            _solutions[5][2] = theta3_ii;
            _solutions[6][2] = theta3_iii;
            _solutions[7][2] = theta3_iv;

            // Calculate joint postion 4, 5 and 6
            for (int i = 0; i < 8; i++)
            {
                double sin1 = Math.Sin(_solutions[i][0]);
                double sin23 = Math.Sin(_solutions[i][1] + _solutions[i][2]);
                double cos1 = Math.Cos(_solutions[i][0]);
                double cos23 = Math.Cos(_solutions[i][1] + _solutions[i][2]);
                double m = (e13 * sin23 * cos1) + (e23 * sin23 * sin1) + (e33 * cos23);

                // Joint 4
                double theta4_p = Math.Atan2((e23 * cos1) - (e13 * sin1), (e13 * cos23 * cos1) + (e23 * cos23 * sin1) - (e33 * sin23));
                _solutions[i][3] = i < 4 ? theta4_p : theta4_p + _pi;

                // Joint 5
                double theta5_p = Math.Atan2(Math.Sqrt(1 - (m * m)), m);
                _solutions[i][4] = i < 4 ? theta5_p : -theta5_p;
                _wristSingularities[i] = Math.Abs(_solutions[i][4]) < 1e-3;

                // Joint 6
                double theta6_p = Math.Atan2((e12 * sin23 * cos1) + (e22 * sin23 * sin1) + (e32 * cos23), (-e11 * sin23 * cos1) - (e21 * sin23 * sin1) - (e31 * cos23));
                _solutions[i][5] = i < 4 ? theta6_p : theta6_p - _pi;
            }

            // Elbow singularities
            bool singularity1 = psi2_i == 0;
            bool singularity2 = psi2_ii == 0;

            _elbowSingularities[0] = singularity1;
            _elbowSingularities[1] = singularity1;
            _elbowSingularities[2] = singularity2;
            _elbowSingularities[3] = singularity2;
            _elbowSingularities[4] = singularity1;
            _elbowSingularities[5] = singularity1;
            _elbowSingularities[6] = singularity2;
            _elbowSingularities[7] = singularity2;

            // Shoulder singularity
            if ((Math.Abs(cx0) < 1e-3) & (Math.Abs(cy0) < 1e-3))
            {
                _shoulderSingularities.Select(item => item = true);
            }
            else
            {
                _shoulderSingularities.Select(item => item = false);
            }

            // Corrections
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    // Check if the values are within - pi till pi
                    _solutions[i][j] = _solutions[i][j] > _pi ? _solutions[i][j] - _twoPi : _solutions[i][j];
                    _solutions[i][j] = _solutions[i][j] < -_pi ? _solutions[i][j] + _twoPi : _solutions[i][j];

                    // Offset and sign corrections
                    _solutions[i][j] = _signs[j] * (_solutions[i][j] + _offsets[j]);
                }
            }
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets a value indicating whether or not the object is valid.
        /// </summary>
        public bool IsValid
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the inverse kinematics solutions in radians.
        /// </summary>
        public double[][] Solutions
        {
            get { return _solutions; }
        }

        /// <summary>
        /// Gets a value indicating whether or not the solutions have a shoulder singularity.
        /// </summary>>
        public bool[] IsShoulderSingularity
        {
            get { return _shoulderSingularities; }
        }

        /// <summary>
        /// Gets a value indicating whether or not the solutions have an elbow singularity.
        /// </summary>
        public bool[] IsElbowSingularity
        {
            get { return _elbowSingularities; }
        }

        /// <summary>
        /// Gets a value indicating whether or not the solutions have a wrist singularity.
        /// </summary>
        public bool[] IsWristSingularity
        {
            get { return _wristSingularities; }
        }

        /// <summary>
        /// Gets or sets the offset corrections.
        /// </summary>
        public IList<double> Offsets
        {
            get { return _offsets; }
            set { _offsets = value.ToArray(); }
        }

        /// <summary>
        /// Gets or sets the sign corrections.
        /// </summary>
        /// <remarks>
        /// This property also checks the input values and adjusts them to -1 or 1 if needed.
        /// </remarks>
        public IList<int> Signs
        {
            get { return _signs; }
            set { _signs = value.ToList().ConvertAll(x => Math.Sign(x)).ToArray(); }
        }

        /// <summary>
        /// Gets or sets the OPW kinematics parameter A1.
        /// </summary>
        public double A1
        {
            get { return _a1; }
            set { _a1 = value; }
        }

        /// <summary>
        /// Gets or sets the OPW kinematics parameter A2.
        /// </summary>
        public double A2
        {
            get { return _a2; }
            set { _a2 = value; UpdateRobotParameters(); }
        }

        /// <summary>
        /// Gets or sets the OPW kinematics parameter B.
        /// </summary>
        public double B
        {
            get { return _b; }
            set { _b = value; }
        }

        /// <summary>
        /// Gets or sets the OPW kinematics parameter C1.
        /// </summary>
        public double C1
        {
            get { return _c1; }
            set { _c1 = value; }
        }

        /// <summary>
        /// Gets or sets the OPW kinematics parameter C2.
        /// </summary>
        public double C2
        {
            get { return _c2; }
            set { _c2 = value; }
        }

        /// <summary>
        /// Gets or sets the OPW kinematics parameter C3.
        /// </summary>
        public double C3
        {
            get { return _c3; }
            set { _c3 = value; UpdateRobotParameters(); }
        }

        /// <summary>
        /// Gets or sets the OPW kinematics parameter C4.
        /// </summary>
        public double C4
        {
            get { return _c4; }
            set { _c4 = value; }
        }
        #endregion
    }
}

// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components
// Project: https://github.com/RobotComponents/RobotComponents
//
// Copyright (c) 2024–2026 Arjen Deetman
//
// Authors:
//   - Arjen Deetman (2024–2026)
//
// For license details, see the LICENSE file in the project root.

// System Libs 
using System;
using System.Linq;
using System.Collections.Generic;
// Rhino Libs
using Rhino.Geometry;

namespace RobotComponents.Generic.Kinematics
{
    /// <summary>
    /// Represents the Wristy Twisty Solver class, a solver for the inverse and forward
    /// kinematics problem of industrial serial manipulators with an offset wrist.
    /// </summary>
    /// <remarks> 
    /// For more information see https://github.com/RobotComponents/WristyTwistySolver.
    /// 
    /// Solution order:
    /// 
    /// Sol.    Wrist center            Wrist center            Axis 5 angle
    ///         relative to axis 1      relative to lower arm
    ///         
    /// 0       In front of             In front of             theta5_0 > theta5_4
    /// 1       In front of             Behind                  theta5_1 > theta5_5
    /// 2       Behind                  In front of             theta5_2 > theta5_6
    /// 3       Behind                  Behind                  theta5_3 > theta5_7
    /// 4       In front of             In front of             theta5_0 > theta5_4
    /// 5       In front of             Behind                  theta5_1 > theta5_5 
    /// 6       Behind                  In front of             theta5_2 > theta5_6
    /// 7       Behind                  Behind                  theta5_3 > theta5_7 
    /// </remarks>
    public class WristyTwistySolver
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
        private double _a3 = 0;
        private double _b = 0;
        private double _c1 = 0;
        private double _c2 = 0;
        private double _c3 = 0;
        private double _c4 = 0;
        private double _k = 0;
        private double _k2 = 0;
        private double _psi3 = 0;
        private double _r = 0;

        // Position parameters
        private double _ux0 = 0;
        private double _uy0 = 0;
        private double _uz0 = 0;

        // Orientation parameters
        private readonly Matrix _roe = new Matrix(3, 3);

        // Wrist data
        private double _dx0 = 0;
        private double _dy0 = 0;
        private double _dz0 = 0;

        // Solver settings
        private const int _steps = 64; // DISCUSSION: Make this a public property?
        private const int _kmax = 3; // DISCUSSION: Make this a public property?
        private const double _stepSize = _twoPi / _steps;
        private const double _dt = 1e-6;

        // Constants
        private const double _halfPi = Math.PI / 2;
        private const double _pi = Math.PI;
        private const double _twoPi = 2 * Math.PI;
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the Wristy Twisty Solver class
        /// </summary>
        public WristyTwistySolver()
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
                return "Invalid Wristy Twisty Solver";
            }
            else
            {
                return "Wristy Twisty Solver";
            }
        }

        /// <summary>
        /// Updates the robot parameters.
        /// </summary>
        /// <remarks>
        /// Method is called when the property A2, A3 or C3 changes. 
        /// </remarks>
        private void UpdateRobotParameters()
        {
            _psi3 = Math.Atan2(_a2, _c3);
            _k2 = (_a2 * _a2) + (_c3 * _c3);
            _k = Math.Sqrt(_k2);
            _r = Math.Abs(_a3);
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
        /// <param name="c"> The wrist position for the given pose. </param>
        /// <returns> The end plane of the 6th joint. </returns>
        public Plane Forward(IList<double> pose, out Point3d c)
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
            double cx0 = (cx1 * cos1) - (cy1 * sin1);
            double cy0 = (cx1 * sin1) + (cy1 * cos1);
            double cz0 = cz1 + _c1;

            // Matrix Rce
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

            // Wrist offset direction
            double wox = (roc[0, 0] * cos4 * cos5) + (roc[0, 1] * sin4 * cos5) - (roc[0, 2] * sin5);
            double woy = (roc[1, 0] * cos4 * cos5) + (roc[1, 1] * sin4 * cos5) - (roc[1, 2] * sin5);
            double woz = (roc[2, 0] * cos4 * cos5) + (roc[2, 1] * sin4 * cos5) - (roc[2, 2] * sin5);

            // End plane position
            double ux0 = cx0 + (_c4 * roe[0, 2]) - (_r * wox);
            double uy0 = cy0 + (_c4 * roe[1, 2]) - (_r * woy);
            double uz0 = cz0 + (_c4 * roe[2, 2]) - (_r * woz);

            // Return wrist position via out param
            c = new Point3d(cx0, cy0, cz0);

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
            // Position parameters 
            _ux0 = endPlane.Origin.X;
            _uy0 = endPlane.Origin.Y;
            _uz0 = endPlane.Origin.Z;

            // Orientation parameters
            _roe[0, 0] = endPlane.XAxis.X;
            _roe[0, 1] = endPlane.YAxis.X;
            _roe[0, 2] = endPlane.ZAxis.X;
            _roe[1, 0] = endPlane.XAxis.Y;
            _roe[1, 1] = endPlane.YAxis.Y;
            _roe[1, 2] = endPlane.ZAxis.Y;
            _roe[2, 0] = endPlane.XAxis.Z;
            _roe[2, 1] = endPlane.YAxis.Z;
            _roe[2, 2] = endPlane.ZAxis.Z;

            // Wrist rotation center (circle center)
            _dx0 = _ux0 - (_roe[0, 2] * _c4);
            _dy0 = _uy0 - (_roe[1, 2] * _c4);
            _dz0 = _uz0 - (_roe[2, 2] * _c4);

            for (int i = 0; i < 4; i++)
            {
                // Scan the solution space
                List<double> parameters = ScanSolutionDomain(i);

                // Find the exact wrist parameters
                for (int j = 0; j < parameters.Count; j++)
                {
                    parameters[j] = FindWristParameter(parameters[j], i);
                }

                // Select parameters
                bool wristSingularity;
                double t1;
                double t2;

                if (parameters.Count == 2)
                {
                    wristSingularity = false;
                    t1 = parameters[0];
                    t2 = parameters[1];
                }
                else
                {
                    wristSingularity = true;
                    double dist1 = CalculateCircularDistance(parameters[0], parameters[2]);
                    double dist2 = CalculateCircularDistance(parameters[1], parameters[3]);

                    if (dist1 > dist2)
                    {
                        t1 = parameters[0];
                        t2 = parameters[2];
                    }
                    else
                    {
                        t1 = parameters[1];
                        t2 = parameters[3];
                    }
                }

                // Calculate two poses
                double[] pose1 = CalculateJointValues(t1, i, true, out _, out bool elbowSingularity1, out bool shoulderSingularity1);
                double[] pose2 = CalculateJointValues(t2, i, true, out _, out bool elbowSingularity2, out bool shoulderSingularity2);

                // Corrections
                for (int j = 0; j < 6; j++)
                {
                    // Check if the values are within - pi till pi
                    pose1[j] = pose1[j] > _pi ? pose1[j] - _twoPi : pose1[j];
                    pose1[j] = pose1[j] < -_pi ? pose1[j] + _twoPi : pose1[j];
                    pose2[j] = pose2[j] > _pi ? pose2[j] - _twoPi : pose2[j];
                    pose2[j] = pose2[j] < -_pi ? pose2[j] + _twoPi : pose2[j];
                }

                // Axis configuration matching based on joint angle 5
                if (pose1[4] >= pose2[4])
                {
                    Array.Copy(pose1, _solutions[i], 6);
                    Array.Copy(pose2, _solutions[i + 4], 6);
                    _elbowSingularities[i] = elbowSingularity1;
                    _elbowSingularities[i + 4] = elbowSingularity2;
                    _shoulderSingularities[i] = shoulderSingularity1;
                    _shoulderSingularities[i + 4] = shoulderSingularity2;
                }
                else
                {
                    Array.Copy(pose1, _solutions[i + 4], 6);
                    Array.Copy(pose2, _solutions[i], 6);
                    _elbowSingularities[i + 4] = elbowSingularity1;
                    _elbowSingularities[i] = elbowSingularity2;
                    _shoulderSingularities[i + 4] = shoulderSingularity1;
                    _shoulderSingularities[i] = shoulderSingularity2;
                }

                // Wrist singularity
                _wristSingularities[i] = wristSingularity;
                _wristSingularities[i + 4] = wristSingularity;

                // Offset and sign corrections
                for (int j = 0; j < 6; j++)
                {
                    _solutions[i][j] = _signs[j] * (_solutions[i][j] + _offsets[j]);
                    _solutions[i + 4][j] = _signs[j] * (_solutions[i + 4][j] + _offsets[j]);
                }
            }
        }

        /// <summary>
        /// Scans the solution space for the two initial starting parameters.
        /// </summary>
        /// <param name="config"> The axis configuration number. </param>
        private List<double> ScanSolutionDomain(int config)
        {
            // Initiate output
            List<double> parameters = new List<double>() { };

            // Start parameter
            double t1 = -_pi;
            double t2 = t1 + _stepSize;

            // Initial function value
            CalculateJointValues(t1, config, false, out double fval1, out _, out _);

            // Counts the number of parameters found
            int found = 0;

            // Finds the two roots of the function
            for (int i = 0; i < _steps; i++)
            {
                // Get function value
                CalculateJointValues(t2, config, false, out double fval2, out _, out _);

                // Check if the sign of the two function values is different 
                // If the sign is different, the root is in the domain [start, start + step]
                if ((fval1 <= 0 & fval2 > 0) | (fval1 >= 0 & fval2 < 0))
                {
                    // Estimate the root by linear interpolation between the two values
                    parameters.Add(t1 - fval1 * (t2 - t1) / (fval2 - fval1));
                    found += 1;
                }

                // Update parameters for next step
                t1 = t2;
                t2 += _stepSize;
                fval1 = fval2;

                // Stop the scan when the two parameters are found
                if (found == 4) { break; }
            }

            return parameters;
        }

        /// <summary>
        /// Returns the wrist parameter.
        /// </summary>
        /// <param name="start"> Initial parameter. </param>
        /// <param name="config"> The axis configuration number. </param>
        /// <returns> The wrist parameter. </returns>
        private double FindWristParameter(double start, int config)
        {
            // Start parameter
            double t = start;

            // Find the param with Newton's method
            for (int j = 0; j < _kmax; j++)
            {
                CalculateJointValues(t, config, false, out double fval, out _, out _);
                CalculateJointValues(t - _dt, config, false, out double fval1, out _, out _);
                CalculateJointValues(t + _dt, config, false, out double fval2, out _, out _);
                double derivative = (fval2 - fval1) / (2 * _dt);

                if (derivative != 0)
                {
                    t -= fval / derivative;
                }
                else
                {
                    break;
                }
            }

            return t;
        }

        /// <summary>
        /// Calculates the joint values for a given wrist position and axis configuration. 
        /// </summary>
        /// <param name="t"> Wrist parameter [0 till 2 * PI]. </param>
        /// <param name="config"> Axis configuration number. </param>
        /// <param name="all"> Indicates if all joint values should be calculated or only the function value. </param>
        /// <param name="fval"> Function value (error) in radians. </param>
        /// <param name="elbowSingularity"> Indicates if the solution has a elbow singularity. </param>
        /// <param name="shoulderSingularity"> Indicates if the solution has a shoulder singularity. </param>
        /// <returns> The joint values in radians. </returns>
        private double[] CalculateJointValues(double t, int config, bool all, out double fval, out bool elbowSingularity, out bool shoulderSingularity)
        {
            // Initialize output
            elbowSingularity = false;
            shoulderSingularity = false;
            double theta1;
            double theta2;
            double theta3;
            double theta4;
            double theta5;
            double theta6;

            // Sine and cosine values of param t
            double cost = Math.Cos(t);
            double sint = Math.Sin(t);

            // Wrist position for given parameter t
            double cx0 = _dx0 + (_r * _roe[0, 0] * cost) + (_r * _roe[0, 1] * sint);
            double cy0 = _dy0 + (_r * _roe[1, 0] * cost) + (_r * _roe[1, 1] * sint);
            double cz0 = _dz0 + (_r * _roe[2, 0] * cost) + (_r * _roe[2, 1] * sint);

            // Check for shoulder singularity
            if ((Math.Abs(cx0) < 1e-3) & (Math.Abs(cy0) < 1e-3)) { shoulderSingularity = true; }

            // General positioning parameters
            double nx1 = Math.Sqrt((cx0 * cx0) + (cy0 * cy0) - (_b * _b)) - _a1;
            double atan1 = Math.Atan2(cy0, cx0);
            double atan2 = Math.Atan2(_b, nx1 + _a1);

            // Joint position 1, 2 and 3
            if (config == 0)
            {
                // Specific positioning parameters
                double s1_2 = (nx1 * nx1) + ((cz0 - _c1) * (cz0 - _c1));
                double s1 = Math.Sqrt(s1_2);
                double psi1 = Math.Atan2(nx1, cz0 - _c1);
                double psi2_i = Math.Acos((s1_2 + (_c2 * _c2) - _k2) / (2.0 * s1 * _c2));
                double acos1 = Math.Acos((s1_2 - (_c2 * _c2) - _k2) / (2.0 * _c2 * _k));

                // Check for Nan values (singularities)
                if (double.IsNaN(psi2_i)) { psi2_i = 0; elbowSingularity = true; }
                if (double.IsNaN(acos1)) { acos1 = 0; }

                theta1 = atan1 - atan2;
                theta2 = -psi2_i + psi1;
                theta3 = acos1 - _psi3;
            }
            else if (config == 1)
            {
                // Specific positioning parameters
                double s1_2 = (nx1 * nx1) + ((cz0 - _c1) * (cz0 - _c1));
                double s1 = Math.Sqrt(s1_2);
                double psi1 = Math.Atan2(nx1, cz0 - _c1);
                double psi2_i = Math.Acos((s1_2 + (_c2 * _c2) - _k2) / (2.0 * s1 * _c2));
                double acos1 = Math.Acos((s1_2 - (_c2 * _c2) - _k2) / (2.0 * _c2 * _k));

                // Check for Nan values (singularities)
                if (double.IsNaN(psi2_i)) { psi2_i = 0; elbowSingularity = true; }
                if (double.IsNaN(acos1)) { acos1 = 0; }

                theta1 = atan1 - atan2;
                theta2 = psi2_i + psi1;
                theta3 = -acos1 - _psi3;
            }
            else if (config == 2)
            {
                // Specific positioning parameters
                double s2_2 = ((nx1 + (2.0 * _a1)) * (nx1 + (2.0 * _a1))) + ((cz0 - _c1) * (cz0 - _c1));
                double s2 = Math.Sqrt(s2_2);
                double psi2_ii = Math.Acos((s2_2 + (_c2 * _c2) - _k2) / (2.0 * s2 * _c2));
                double acos2 = Math.Acos((s2_2 - (_c2 * _c2) - _k2) / (2.0 * _c2 * _k));
                double atan3 = Math.Atan2(nx1 + (2.0 * _a1), cz0 - _c1);

                // Check for Nan values (singularities)
                if (double.IsNaN(psi2_ii)) { psi2_ii = 0; elbowSingularity = true; }
                if (double.IsNaN(acos2)) { acos2 = 0; }

                theta1 = atan1 + atan2 - _pi;
                theta2 = -psi2_ii - atan3;
                theta3 = acos2 - _psi3;
            }
            else if (config == 3)
            {
                // Specific positioning parameters
                double s2_2 = ((nx1 + (2.0 * _a1)) * (nx1 + (2.0 * _a1))) + ((cz0 - _c1) * (cz0 - _c1));
                double s2 = Math.Sqrt(s2_2);
                double psi2_ii = Math.Acos((s2_2 + (_c2 * _c2) - _k2) / (2.0 * s2 * _c2));
                double acos2 = Math.Acos((s2_2 - (_c2 * _c2) - _k2) / (2.0 * _c2 * _k));
                double atan3 = Math.Atan2(nx1 + (2.0 * _a1), cz0 - _c1);

                // Check for Nan values (singularities)
                if (double.IsNaN(psi2_ii)) { psi2_ii = 0; elbowSingularity = true; }
                if (double.IsNaN(acos2)) { acos2 = 0; }

                theta1 = atan1 + atan2 - _pi;
                theta2 = psi2_ii - atan3;
                theta3 = -acos2 - _psi3;
            }
            else
            {
                throw new Exception("Configuration parameter is outside range.");
            }

            // Resulting sine and cosine values
            double sin1 = Math.Sin(theta1);
            double sin2 = Math.Sin(theta2);
            double sin3 = Math.Sin(theta3);
            double cos1 = Math.Cos(theta1);
            double cos2 = Math.Cos(theta2);
            double cos3 = Math.Cos(theta3);

            // Matrix Roc transposed
            Matrix roct = new Matrix(3, 3);                             // Alternative form
            roct[0, 0] = (cos1 * cos2 * cos3) - (cos1 * sin2 * sin3);   // cos(x1) cos(x2 + x3)
            roct[0, 1] = (sin1 * cos2 * cos3) - (sin1 * sin2 * sin3);   // sin(x1) cos(x2 + x3)
            roct[0, 2] = (-sin2 * cos3) - (cos2 * sin3);                // -sin(x2 + x3)
            roct[1, 0] = -sin1;
            roct[1, 1] = cos1;
            roct[1, 2] = 0;
            roct[2, 0] = (cos1 * cos2 * sin3) + (cos1 * sin2 * cos3);   // cos(x1) sin(x2 + x3)
            roct[2, 1] = (sin1 * cos2 * sin3) + (sin1 * sin2 * cos3);   // sin(x1) sin(x2 + x3)
            roct[2, 2] = (-sin2 * sin3) + (cos2 * cos3);                // cos(x2 + x3)

            // Rotation matrix Rz
            Matrix rz = new Matrix(3, 3);
            rz[0, 0] = cost;
            rz[0, 1] = -sint;
            rz[0, 2] = 0;
            rz[1, 0] = sint;
            rz[1, 1] = cost;
            rz[1, 2] = 0;
            rz[2, 0] = 0;
            rz[2, 1] = 0;
            rz[2, 2] = 1;

            // Matrix Rod: Orientation of wrist plane parallel to endplane
            Matrix rod = _roe * rz;

            // Wrist orientation Rod relative to wrist orientation Roc
            Matrix rcd = roct * rod;

            // Function value (error)
            fval = Math.Acos(rcd[2, 1]) - _halfPi;

            // Calculate remaining joint positions (4, 5 and 6) if needed
            if (all == true)
            {
                // Joint position 4
                theta4 = Math.Atan2(rcd[1, 1], rcd[0, 1]) - _halfPi;

                // Joint position 5
                theta5 = rcd[2, 0] >= 0 ? -Math.Acos(rcd[2, 2]) : Math.Acos(rcd[2, 2]) - _twoPi;
                if (double.IsNaN(theta5)) { theta5 = 0; }

                // Joint Position 6
                theta6 = -t;
            }
            else
            {
                theta4 = 0;
                theta5 = 0;
                theta6 = 0;
            }

            // Return pose
            return new double[6] { theta1, theta2, theta3, theta4, theta5, theta6 };
        }

        /// <summary>
        /// Calculates the circular distance between two angles.
        /// </summary>
        /// <param name="param1">First angle in radians.</param>
        /// <param name="param2">Second angle in radians.</param>
        /// <returns>Circular distance between the angles.</returns>
        private static double CalculateCircularDistance(double param1, double param2)
        {
            double diff = Math.Abs(param1 - param2);
            return Math.Min(diff, _twoPi - diff);
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
        /// Gets or sets the Wristy Twisty parameter A1.
        /// </summary>
        public double A1
        {
            get { return _a1; }
            set { _a1 = value; }
        }

        /// <summary>
        /// Gets or sets the Wristy Twisty parameter A2.
        /// </summary>
        public double A2
        {
            get { return _a2; }
            set { _a2 = value; UpdateRobotParameters(); }
        }

        /// <summary>
        /// Gets or sets the Wristy Twisty parameter A3.
        /// </summary>
        public double A3
        {
            get { return _a3; }
            set { _a3 = value; UpdateRobotParameters(); }
        }

        /// <summary>
        /// Gets or sets the Wristy Twisty parameter B.
        /// </summary>
        public double B
        {
            get { return _b; }
            set { _b = value; }
        }

        /// <summary>
        /// Gets or sets the Wristy Twisty parameter C1.
        /// </summary>
        public double C1
        {
            get { return _c1; }
            set { _c1 = value; }
        }

        /// <summary>
        /// Gets or sets the Wristy Twisty parameter C2.
        /// </summary>
        public double C2
        {
            get { return _c2; }
            set { _c2 = value; }
        }

        /// <summary>
        /// Gets or sets the Wristy Twisty parameter C3.
        /// </summary>
        public double C3
        {
            get { return _c3; }
            set { _c3 = value; UpdateRobotParameters(); }
        }

        /// <summary>
        /// Gets or sets the Wristy Twisty parameter C4.
        /// </summary>
        public double C4
        {
            get { return _c4; }
            set { _c4 = value; }
        }
        #endregion
    }
}
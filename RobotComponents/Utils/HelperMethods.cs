﻿// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components
// Project: https://github.com/RobotComponents/RobotComponents
//
// Copyright (c) 2024 Arjen Deetman
//
// Authors:
//   - Arjen Deetman (2024)
//
// For license details, see the LICENSE file in the project root.

// Rhino Libs
using Rhino.Geometry;

namespace RobotComponents.Utils
{
    /// <summary>
    /// Represents the class with internal helper methods.
    /// </summary>
    internal static class HelperMethods
    {
        /// <summary>
        /// Writes a 3x3 matrix to the Rhinoceros command line for debugging purposes.
        /// </summary>
        /// <param name="m"> The matrix to be written to the command line. </param>
        public static void WriteMatrix3x3(Matrix m)
        {
            if (m.ColumnCount != 3 || m.RowCount != 3)
            {
                Rhino.RhinoApp.WriteLine("Matrix dimensions do not match the expected size.");
                return;
            }

            Rhino.RhinoApp.WriteLine($"{m[0, 0]}, {m[0, 1]}, {m[0, 2]}");
            Rhino.RhinoApp.WriteLine($"{m[1, 0]}, {m[1, 1]}, {m[1, 2]}");
            Rhino.RhinoApp.WriteLine($"{m[2, 0]}, {m[2, 1]}, {m[2, 2]}");
        }

        /// <summary>
        /// Creates a plane from a 3x3 rotation matrix.
        /// </summary>
        /// <param name="m"> The 3x3 rotation matrix. </param>
        /// <returns> The plane. </returns>
        public static Plane Matrix3x3ToPlane(Matrix m) 
        {
            if (m.ColumnCount != 3 || m.RowCount != 3)
            {
                Rhino.RhinoApp.WriteLine("Matrix dimensions do not match the expected size.");
                return Plane.Unset;
            }

            Vector3d xAxis = new Vector3d(m[0, 0], m[1, 0], m[2, 0]);
            Vector3d yAxis = new Vector3d(m[0, 1], m[1, 1], m[2, 1]);

            return new Plane(new Point3d(0, 0, 0), xAxis, yAxis);
        }
    }
}

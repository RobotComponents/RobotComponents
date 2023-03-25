// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Text.RegularExpressions;
// Rhino Libs
using Rhino.Geometry;
// Robot Components Libs
using RobotComponents.ABB.Enumerations;
using RobotComponents.ABB.Actions.Interfaces;

namespace RobotComponents.ABB.Utils
{
    /// <summary>
    /// Represents general helper methods.
    /// </summary>
    public static class HelperMethods
    {
        #region fields
        private static readonly Regex _rapidDataRegex = new Regex(@"[\s;:\[\]\(\){}]", RegexOptions.Compiled);
        #endregion

        #region methods
        /// <summary>
        /// Flips a plane normal to the oposite direction by setting it's x-axis negative.
        /// </summary>
        /// <param name="plane"> The plane that needs to be flipped. </param>
        /// <returns> 
        /// The flipped plane. 
        /// </returns>
        public static Plane FlipPlaneX(Plane plane)
        {
            return new Plane(plane.Origin, -plane.XAxis, plane.YAxis);
        }

        /// <summary>
        /// Flips a plane normal to the oposite direction by setting it's y-axis negative.
        /// </summary>
        /// <param name="plane"> The plane that needs to be flipped. </param>
        /// <returns> 
        /// The flipped plane. 
        /// </returns>
        public static Plane FlipPlaneY(Plane plane)
        {
            return new Plane(plane.Origin, plane.XAxis, -plane.YAxis);
        }

        /// <summary>
        /// Transforms a Quarternion to a Plane.
        /// </summary>
        /// <param name="origin"> The origin of the plane. </param>
        /// <param name="quat"> The quarternion. </param>
        /// <returns> 
        /// The plane obtained with the orientation defined by the quarternion values. 
        /// </returns>
        public static Plane QuaternionToPlane(Point3d origin, Quaternion quat)
        {
            quat.GetRotation(out Plane plane);
            plane = new Plane(origin, plane.XAxis, plane.YAxis);
            return plane;
        }

        /// <summary>
        /// Transforms a Quarternion to a Plane.
        /// </summary>
        /// <param name="refPlane"> The reference plane. </param>
        /// <param name="origin"> The origin of the new plane. </param>
        /// <param name="quat"> The quarternion. </param>
        /// <returns> 
        /// The plane obtained with the orientation defined by the quarternion values. 
        /// </returns>
        public static Plane QuaternionToPlane(Plane refPlane, Point3d origin, Quaternion quat)
        {
            quat.GetRotation(out Plane plane);
            plane = new Plane(origin, plane.XAxis, plane.YAxis);

            Transform transform = Transform.PlaneToPlane(Plane.WorldXY, refPlane);
            plane.Transform(transform);

            return plane;
        }

        /// <summary>
        /// Transforms a Quarternion to a Plane.
        /// </summary>
        /// <param name="origin"> The origin of the plane. </param>
        /// <param name="A"> The real part of the quaternion. </param>
        /// <param name="B"> The first imaginary coefficient of the quaternion. </param>
        /// <param name="C"> The second imaginary coefficient of the quaternion. </param>
        /// <param name="D"> The third imaginary coefficient of the quaternion. </param>
        /// <returns> 
        /// The plane obtained with the orientation defined by the quarternion values. 
        /// </returns>
        public static Plane QuaternionToPlane(Point3d origin, double A, double B, double C, double D)
        {
            Quaternion quat = new Quaternion(A, B, C, D);
            Plane plane = QuaternionToPlane(origin, quat);
            return plane;
        }

        /// <summary>
        /// Transforms a Quarternion to a Plane.
        /// </summary>
        /// <param name="refPlane"> The reference plane. </param>
        /// <param name="origin"> The origin of the plane. </param>
        /// <param name="A"> The real part of the quaternion. </param>
        /// <param name="B"> The first imaginary coefficient of the quaternion. </param>
        /// <param name="C"> The second imaginary coefficient of the quaternion. </param>
        /// <param name="D"> The third imaginary coefficient of the quaternion. </param>
        /// <returns> 
        /// The plane obtained with the orientation defined by the quarternion values.
        /// </returns>
        public static Plane QuaternionToPlane(Plane refPlane, Point3d origin, double A, double B, double C, double D)
        {
            Quaternion quat = new Quaternion(A, B, C, D);
            Plane plane = QuaternionToPlane(refPlane, origin, quat);
            return plane;
        }

        /// <summary>
        /// Transforms a Quarternion to a Plane.
        /// </summary>
        /// <param name="x"> The x coordinate of the plane origin. </param>
        /// <param name="y"> The y coordinate of the plane origin. </param>
        /// <param name="z"> The z coordinate of the plane origin.</param>
        /// <param name="A"> The real part of the quaternion. </param>
        /// <param name="B"> The first imaginary coefficient of the quaternion. </param>
        /// <param name="C"> The second imaginary coefficient of the quaternion. </param>
        /// <param name="D"> The third imaginary coefficient of the quaternion. </param>
        /// <returns> 
        /// The plane obtained with the orientation defined by the quarternion values. 
        /// </returns>
        public static Plane QuaternionToPlane(double x, double y, double z, double A, double B, double C, double D)
        {
            Point3d point = new Point3d(x, y, z);
            Plane plane = QuaternionToPlane(point, A, B, C, D);
            return plane;
        }

        /// <summary>
        /// Transforms a Quarternion to a Plane.
        /// </summary>
        /// <param name="refPlane"> The reference plane. </param>
        /// <param name="x"> The x coordinate of the plane origin. </param>
        /// <param name="y"> The y coordinate of the plane origin. </param>
        /// <param name="z"> The z coordinate of the plane origin.</param>
        /// <param name="A"> The real part of the quaternion. </param>
        /// <param name="B"> The first imaginary coefficient of the quaternion. </param>
        /// <param name="C"> The second imaginary coefficient of the quaternion. </param>
        /// <param name="D"> The third imaginary coefficient of the quaternion. </param>
        /// <returns> 
        /// The plane obtained with the orientation defined by the quarternion values. 
        /// </returns>
        public static Plane QuaternionToPlane(Plane refPlane, double x, double y, double z, double A, double B, double C, double D)
        {
            Point3d point = new Point3d(x, y, z);
            Plane plane = QuaternionToPlane(refPlane, point, A, B, C, D);
            return plane;
        }

        /// <summary>
        /// Transforms a plane to a quarternion.
        /// </summary>
        /// <param name="refPlane"> The reference plane. </param>
        /// <param name="plane"> The plane that should be transformed. </param>
        /// <returns> 
        /// The quaternion as a Quaternion. 
        /// </returns>
        public static Quaternion PlaneToQuaternion(Plane refPlane, Plane plane)
        {
            Transform orient = Transform.PlaneToPlane(refPlane, Plane.WorldXY);

            Plane dum = new Plane(plane);
            dum.Transform(orient);

            Quaternion quat = Quaternion.Rotation(Plane.WorldXY, dum);

            return quat;
        }

        /// <summary>
        /// Transforms a plane to a quarternion.
        /// </summary>
        /// <param name="refPlane"> The reference plane. </param>
        /// <param name="plane"> The plane that should be transformed. </param>
        /// <param name="origin"> The origin of the plane oriented based on the reference plane. </param>
        /// <returns> 
        /// The quaternion as a Quaternion. 
        /// </returns>
        public static Quaternion PlaneToQuaternion(Plane refPlane, Plane plane, out Point3d origin)
        {
            Transform orient = Transform.PlaneToPlane(refPlane, Plane.WorldXY);

            Plane dum = new Plane(plane);
            dum.Transform(orient);

            Quaternion quat = Quaternion.Rotation(Plane.WorldXY, dum);

            origin = new Point3d(plane.Origin);
            origin.Transform(orient);

            return quat;
        }

        /// <summary>
        /// Transforms a plane to a quarternion with as reference plane WorldXY.
        /// </summary>
        /// <param name="plane"> The plane to should be transformed. </param>
        /// <returns> 
        /// The quaternion. 
        /// </returns>
        public static Quaternion PlaneToQuaternion(Plane plane)
        {
            Plane refPlane = Plane.WorldXY;
            Quaternion quat = Quaternion.Rotation(refPlane, plane);
            return quat;
        }

        /// <summary>
        /// Replaces the first occurence in a string with a new text. 
        /// </summary>
        /// <param name="text"> The text the search and replace in. </param>
        /// <param name="search"> The text to search for. </param>
        /// <param name="replace"> The new text. </param>
        /// <returns> 
        /// Returns a new string with replaced text. 
        /// </returns>
        public static string ReplaceFirst(this string text, string search, string replace)
        {
            int position = text.IndexOf(search);

            if (position < 0)
            {
                return text;
            }

            return text.Substring(0, position) + replace + text.Substring(position + search.Length);
        }

        /// <summary>
        /// Sets the scope, variable type and variable name from a RAPID data string and outputs the values. 
        /// </summary>
        /// <remarks>
        /// This method is used to parse declarations from a RAPID data string. 
        /// The values are processed inside the different IDeclaration classes. 
        /// </remarks>
        /// <param name="declaration"> The declaration to set the values. </param>
        /// <param name="rapidData"> The RAPID data string. </param>
        /// <param name="values"> The values from the RAPID data string. </param>
        public static void SetDataFromString(this IDeclaration declaration, string rapidData, out string[] values)
        {
            string clean = _rapidDataRegex.Replace(rapidData, "");

            string[] split = clean.Split('=');
            string type;
            string value;

            // Check for equal signs
            switch (split.Length)
            {
                case 1:
                    type = $"VAR{declaration.Datatype}";
                    value = split[0];
                    break;
                case 2:
                    type = split[0];
                    value = split[1];
                    break;
                default:
                    throw new InvalidCastException("Invalid RAPID data string: More than one equal sign defined.");
            }

            // Scope
            switch (type)
            {
                case string t when t.StartsWith("LOCAL"):
                    declaration.Scope = Scope.LOCAL;
                    type = type.ReplaceFirst("LOCAL", "");
                    break;
                case string t when t.StartsWith("TASK"):
                    declaration.Scope = Scope.TASK;
                    type = type.ReplaceFirst("TASK", "");
                    break;
                default:
                    declaration.Scope = Scope.GLOBAL;
                    break;
            }

            // Variable type
            switch (type)
            {
                case string t when t.StartsWith("VAR"):
                    declaration.VariableType = VariableType.VAR;
                    type = type.ReplaceFirst("VAR", "");
                    break;
                case string t when t.StartsWith("CONST"):
                    declaration.VariableType = VariableType.CONST;
                    type = type.ReplaceFirst("CONST", "");
                    break;
                case string t when t.StartsWith("PERS"):
                    declaration.VariableType = VariableType.PERS;
                    type = type.ReplaceFirst("PERS", "");
                    break;
                default:
                    throw new InvalidCastException("Invalid RAPID data string: The scope or variable type is incorrect.");
            }

            // Datatype
            if (type.StartsWith(declaration.Datatype) == false)
            {
                throw new InvalidCastException("Invalid RAPID data string: The datatype does not match.");
            }

            // Name
            declaration.Name = type.ReplaceFirst(declaration.Datatype, "");

            // Values
            values = value.Split(',');
        }
        #endregion

        #region properties

        #endregion
    }
}

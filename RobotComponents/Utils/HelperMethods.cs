// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
// Rhino Libs
using Rhino.Geometry;

namespace RobotComponents.Utils
{
    /// <summary>
    /// General Helper methods
    /// </summary>
    public static class HelperMethods
    {
        /// <summary>
        /// Serializes a common object to a byte array. 
        /// Typically used for serializing robot meshes. 
        /// </summary>
        /// <param name="obj"> The common object. </param>
        /// <returns> Returns the byte array. </returns>
        public static byte[] ObjectToByteArray(Object obj)
        {
            if (obj == null) { return null; }

            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, obj);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Deserializes a byte array to a common object. 
        /// Typically used for deserializing robot meshes. 
        /// </summary>
        /// <param name="data"> The byte array. </param>
        /// <returns> Returns the common object. </returns>
        public static Object ByteArrayToObject(byte[] data)
        {
            using (MemoryStream stream = new MemoryStream(data))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                stream.Write(data, 0, data.Length);
                stream.Seek(0, SeekOrigin.Begin);
                return (Object)formatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// Flips the plane to the oposite direction by setting it's x-axis negative.
        /// </summary>
        /// <param name="plane"> The plane that needs to be flipped. </param>
        /// <returns> Returns the flipped plane. </returns>
        public static Plane FlipPlaneX(Plane plane)
        {
            return new Plane(plane.Origin, -plane.XAxis, plane.YAxis);
        }

        /// <summary>
        /// Flips the plane to the oposite direction by setting it's y-axis negative.
        /// </summary>
        /// <param name="plane"> The plane that needs to be flipped. </param>
        /// <returns> Returns the flipped plane. </returns>
        public static Plane FlipPlaneY(Plane plane)
        {
            return new Plane(plane.Origin, plane.XAxis, -plane.YAxis);
        }

        /// <summary>
        /// Transforms a Quarternion to a Plane.
        /// </summary>
        /// <param name="origin"> The origin of the plane. </param>
        /// <param name="quat"> The quarternion </param>
        /// <returns> The plane obtained with the orientation defined by the quarternion values. </returns>
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
        /// <param name="origin"> The origin of the plane. </param>
        /// <param name="quat"> The quarternion </param>
        /// <returns> The plane obtained with the orientation defined by the quarternion values. </returns>
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
        /// <returns> The plane obtained with the orientation defined by the quarternion values. </returns>
        public static Plane QuaternionToPlane(Point3d origin, double A, double B, double C, double D)
        {
            Quaternion quat = new Quaternion(A, B, C, D);
            Plane plane = QuaternionToPlane(origin, quat);
            return plane;
        }

        /// <summary>
        /// Transforms a Quarternion to a Plane.
        /// </summary
        /// <param name="refPlane"> The reference plane. </param>
        /// <param name="origin"> The origin of the plane. </param>
        /// <param name="A"> The real part of the quaternion. </param>
        /// <param name="B"> The first imaginary coefficient of the quaternion. </param>
        /// <param name="C"> The second imaginary coefficient of the quaternion. </param>
        /// <param name="D"> The third imaginary coefficient of the quaternion. </param>
        /// <returns> The plane obtained with the orientation defined by the quarternion values. </returns>
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
        /// <returns> The plane obtained with the orientation defined by the quarternion values. </returns>
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
        /// <returns> The plane obtained with the orientation defined by the quarternion values. </returns>
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
        /// <returns> The quaternion as a Quaternion. </returns>
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
        /// <returns> The quaternion as a Quaternion. </returns>
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
        /// <returns> The quaternion as a Quaternion.</returns>
        public static Quaternion PlaneToQuaternion(Plane plane)
        {
            Plane refPlane = Plane.WorldXY;
            Quaternion quat = Quaternion.Rotation(refPlane, plane);
            return quat;
        }
    }
}

// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

namespace RobotComponents.ABB.Kinematics.IKFast.Geometry
{
    /// <summary>
    /// Represents the internal Vector3d class.
    /// </summary>
    internal struct Vector3d
    {
        public Vector3d(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3d(Rhino.Geometry.Point3d point)
        {
            x = point.X;
            y = point.Y;
            z = point.Z;
        }

        public Vector3d(Rhino.Geometry.Vector3d vector)
        {
            x = vector.X;
            y = vector.Y;
            z = vector.Z;
        }

        public Rhino.Geometry.Point3d ToRhinoPoint3d()
        {
            return new Rhino.Geometry.Point3d(x, y, z);
        }

        public Rhino.Geometry.Vector3d ToRhinoVector3d()
        {
            return new Rhino.Geometry.Vector3d(x, y, z);
        }

        public double x, y, z;
    }
}


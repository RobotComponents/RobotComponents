// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

namespace RobotComponents.ABB.Kinematics.IkFast.Geometry
{
    /// <summary>
    /// Represents the internal Quaternion class.
    /// </summary>
    /// <remarks>
    /// Be careful with quaternion naming conventions. 
    /// There is a difference between ikfast and Rhino. 
    /// The conversion is as follows; A=w, B=x, C=y, D=z.
    /// </remarks>
    internal struct Quaternion
    {
        public Quaternion(double x, double y, double z, double w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public Quaternion(Rhino.Geometry.Quaternion quaternion)
        {
            x = quaternion.B;
            y = quaternion.C;
            z = quaternion.D;
            w = quaternion.A;
        }

        public Rhino.Geometry.Quaternion ToRhinoQuaternion()
        {
            return new Rhino.Geometry.Quaternion(w, x, y, z);
        }

        public double x, y, z, w;
    }
}


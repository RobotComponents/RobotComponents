// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System.Collections.Generic;
// Robot Components Libs
using RobotComponents.ABB.Actions.Declarations;

namespace RobotComponents.ABB.Kinematics.IkFast.Geometry
{
    /// <summary>
    /// Represents the internal Vector6d class.
    /// </summary>
    internal struct Vector6d
    {
        public Vector6d(double x1, double x2, double x3, double x4, double x5, double x6)
        {
            this.x1 = x1;
            this.x2 = x2;
            this.x3 = x3;
            this.x4 = x4;
            this.x5 = x5;
            this.x6 = x6;
        }

        public double[] ToArray()
        {
            return new double[6]{ x1, x2, x3, x4, x5, x6 };
        }

        public List<double> ToList()
        {
            return new List<double>() { x1, x2, x3, x4, x5, x6 };
        }

        public RobotJointPosition ToRobotJointPosition()
        {
            return new RobotJointPosition(x1, x2, x3, x4, x5, x6);
        }

        public double x1, x2, x3, x4, x5, x6;
    }
}


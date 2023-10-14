// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
// Rhino Libs
using Rhino.Geometry;
// Robot Components Libs
using RobotComponents.ABB.Definitions;
using RobotComponents.ABB.Presets.Robots;
using RobotComponents.ABB.Presets.Enumerations;

namespace RobotComponents.ABB.Presets
{
    /// <summary>
    /// Represents the presets factory. 
    /// </summary>
    public static class Factory
    {
        /// <summary>
        /// Returns a predefined ABB Robot preset. 
        /// </summary>
        /// <param name="preset"> The Robot preset type. </param>
        /// <param name="positionPlane"> The position and orientation of the Robot in world coordinate space. </param>
        /// <param name="tool"> The Robot Tool. </param>
        /// <param name="externalAxes"> The external axes attached to the Robot. </param>
        /// <returns></returns>
        public static Robot GetRobotPreset(RobotPreset preset, Plane positionPlane, RobotTool tool, IList<ExternalAxis> externalAxes = null)
        {
            // Check Robot Tool data
            if (tool == null)
            {
                tool = new RobotTool();
            }

            // Check External Axes 
            if (externalAxes == null)
            {
                externalAxes = new List<ExternalAxis>() { };
            }

            else if (preset == RobotPreset.EMPTY)
            {
                return new Robot();
            }
            if (preset == RobotPreset.IRB1010_1_5_037)
            {
                return IRB1010_15_037.GetRobot(positionPlane, tool, externalAxes);
            }
            if (preset == RobotPreset.IRB1100_4_0475)
            {
                return IRB1100_4_0475.GetRobot(positionPlane, tool, externalAxes);
            }
            else if (preset == RobotPreset.IRB1100_4_058)
            {
                return IRB1100_4_058.GetRobot(positionPlane, tool, externalAxes);
            }
            else if (preset == RobotPreset.IRB120_3_058)
            {
                return IRB120_3_058.GetRobot(positionPlane, tool, externalAxes);
            }
            else if (preset == RobotPreset.IRB1200_5_090)
            {
                return IRB1200_5_090.GetRobot(positionPlane, tool, externalAxes);
            }
            else if (preset == RobotPreset.IRB1200_7_070)
            {
                return IRB1200_7_070.GetRobot(positionPlane, tool, externalAxes);
            }
            else if (preset == RobotPreset.IRB1300_10_115)
            {
                return IRB1300_10_115.GetRobot(positionPlane, tool, externalAxes);
            }
            else if (preset == RobotPreset.IRB1300_11_090)
            {
                return IRB1300_11_090.GetRobot(positionPlane, tool, externalAxes);
            }
            else if (preset == RobotPreset.IRB1300_7_140)
            {
                return IRB1300_7_140.GetRobot(positionPlane, tool, externalAxes);
            }
            else if (preset == RobotPreset.IRB140_6_081)
            {
                return IRB140_6_081.GetRobot(positionPlane, tool, externalAxes);
            }
            else if (preset == RobotPreset.IRB1520ID_4_150)
            {
                return IRB1520ID_4_150.GetRobot(positionPlane, tool, externalAxes);
            }
            else if (preset == RobotPreset.IRB1600_X_120)
            {
                return IRB1600_X_120.GetRobot(positionPlane, tool, externalAxes);
            }
            else if (preset == RobotPreset.IRB1600_X_145)
            {
                return IRB1600_X_145.GetRobot(positionPlane, tool, externalAxes);
            }
            else if (preset == RobotPreset.IRB1660ID_X_155)
            {
                return IRB1660ID_X_155.GetRobot(positionPlane, tool, externalAxes);
            }
            else if (preset == RobotPreset.IRB2600_12_185)
            {
                return IRB2600_12_185.GetRobot(positionPlane, tool, externalAxes);
            }
            else if (preset == RobotPreset.IRB2600_X_165)
            {
                return IRB2600_X_165.GetRobot(positionPlane, tool, externalAxes);
            }
            else if (preset == RobotPreset.IRB2600ID_15_185)
            {
                return IRB2600ID_15_185.GetRobot(positionPlane, tool, externalAxes);
            }
            else if (preset == RobotPreset.IRB2600ID_8_200)
            {
                return IRB2600ID_8_200.GetRobot(positionPlane, tool, externalAxes);
            }
            else if (preset == RobotPreset.IRB4600_20_250)
            {
                return IRB4600_20_250.GetRobot(positionPlane, tool, externalAxes);
            }
            else if (preset == RobotPreset.IRB4600_40_255)
            {
                return IRB4600_40_255.GetRobot(positionPlane, tool, externalAxes);
            }
            else if (preset == RobotPreset.IRB4600_X_205)
            {
                return IRB4600_X_205.GetRobot(positionPlane, tool, externalAxes);
            }
            else if (preset == RobotPreset.IRB6620_150_220)
            {
                return IRB6620_150_220.GetRobot(positionPlane, tool, externalAxes);
            }
            else if (preset == RobotPreset.IRB6640_235_255)
            {
                return IRB6640_235_255.GetRobot(positionPlane, tool, externalAxes);
            }
            else if (preset == RobotPreset.IRB6650_125_320)
            {
                return IRB6650_125_320.GetRobot(positionPlane, tool, externalAxes);
            }
            else if (preset == RobotPreset.IRB6650_200_275)
            {
                return IRB6650_200_275.GetRobot(positionPlane, tool, externalAxes);
            }
            else if (preset == RobotPreset.IRB6650S_125_350)
            {
                return IRB6650S_125_350.GetRobot(positionPlane, tool, externalAxes);
            }
            else if (preset == RobotPreset.IRB6650S_200_300)
            {
                return IRB6650S_200_300.GetRobot(positionPlane, tool, externalAxes);
            }
            else if (preset == RobotPreset.IRB6650S_90_390)
            {
                return IRB6650S_90_390.GetRobot(positionPlane, tool, externalAxes);
            }
            else if (preset == RobotPreset.IRB6700_150_320)
            {
                return IRB6700_150_320.GetRobot(positionPlane, tool, externalAxes);
            }
            else if (preset == RobotPreset.IRB6700_155_285)
            {
                return IRB6700_155_285.GetRobot(positionPlane, tool, externalAxes);
            }
            else if (preset == RobotPreset.IRB6700_175_305)
            {
                return IRB6700_175_305.GetRobot(positionPlane, tool, externalAxes);
            }
            else if (preset == RobotPreset.IRB6700_200_260)
            {
                return IRB6700_200_260.GetRobot(positionPlane, tool, externalAxes);
            }
            else if (preset == RobotPreset.IRB6700_205_280)
            {
                return IRB6700_205_280.GetRobot(positionPlane, tool, externalAxes);
            }
            else if (preset == RobotPreset.IRB6700_235_265)
            {
                return IRB6700_235_265.GetRobot(positionPlane, tool, externalAxes);
            }
            else if (preset == RobotPreset.IRB6700_245_300)
            {
                return IRB6700_245_300.GetRobot(positionPlane, tool, externalAxes);
            }
            else if (preset == RobotPreset.IRB6700_300_270)
            {
                return IRB6700_300_270.GetRobot(positionPlane, tool, externalAxes);
            }
            else if (preset == RobotPreset.IRB6790_205_280)
            {
                return IRB6790_205_280.GetRobot(positionPlane, tool, externalAxes);
            }
            else if (preset == RobotPreset.IRB6790_235_265)
            {
                return IRB6790_235_265.GetRobot(positionPlane, tool, externalAxes);
            }
            else if (preset == RobotPreset.IRB7600_150_350)
            {
                return IRB7600_150_350.GetRobot(positionPlane, tool, externalAxes);
            }
            else if (preset == RobotPreset.IRB7600_325_310)
            {
                return IRB7600_325_310.GetRobot(positionPlane, tool, externalAxes);
            }
            else if (preset == RobotPreset.IRB7600_340_280)
            {
                return IRB7600_340_280.GetRobot(positionPlane, tool, externalAxes);
            }
            else if (preset == RobotPreset.IRB7600_400_255)
            {
                return IRB7600_400_255.GetRobot(positionPlane, tool, externalAxes);
            }
            else if (preset == RobotPreset.IRB7600_500_255)
            {
                return IRB7600_500_255.GetRobot(positionPlane, tool, externalAxes);
            }
            else
            {
                throw new Exception("Could not find the data of the defined Robot preset type.");
            }
        }
    }
}

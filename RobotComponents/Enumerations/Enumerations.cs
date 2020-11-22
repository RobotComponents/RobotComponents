// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

namespace RobotComponents.Enumerations
{
    /// <summary>
    /// Defines if the axis moves linear or rotational.
    /// </summary>
    public enum AxisType : int
    { 
        /// <summary>
        /// Linear motion
        /// </summary>
        LINEAR = 0, 

        /// <summary>
        /// Rotational motion
        /// </summary>
        ROTATIONAL = 1
    }

    /// <summary>
    /// Defines the movement type.
    /// </summary>
    public enum MovementType : int
    {
        /// <summary>
        /// Absolute joint movement
        /// </summary>
        MoveAbsJ = 0,

        /// <summary>
        /// Linear Movement
        /// </summary>
        MoveL = 1,

        /// <summary>
        /// Joint Movement
        /// </summary>
        MoveJ = 2
    }

    /// <summary>
    /// Defines the code type.
    /// </summary>
    public enum CodeType : int
    {
        /// <summary>
        /// Instruction
        /// </summary>
        Instruction = 0,

        /// <summary>
        /// Declaration
        /// </summary>
        Declaration = 1
    }

    /// <summary>
    /// Defines the reference type of the declaration.
    /// </summary>
    public enum ReferenceType : int
    {
        /// <summary>
        /// Persistent data type
        /// </summary>
        PERS = 0,

        /// <summary>
        /// Variable data type
        /// </summary>
        VAR = 1,

        /// <summary>
        /// Constant data type
        /// </summary>
        CONST = 2
    }

    /// <summary>
    /// Defines the Robot preset
    /// </summary>
    public enum RobotPreset
    {
        /// <summary>
        /// Empty robot
        /// </summary>
        EMPTY,

        /// <summary>
        /// IRB1100-4/0.475
        /// </summary>
        IRB1100_4_0475,

        /// <summary>
        /// IRB1100-4/0.58
        /// </summary>
        IRB1100_4_058,

        /// <summary>
        /// IRB120-3/0.58
        /// </summary>
        IRB120_3_058,

        /// <summary>
        /// IRB1200-5/0.9
        /// </summary>
        IRB1200_5_090,

        /// <summary>
        /// IRB1200-7/0.7
        /// </summary>
        IRB1200_7_070,

        /// <summary>
        /// IRB1300-10/1.15
        /// </summary>
        IRB1300_10_115,

        /// <summary>
        /// IRB1300-11/0.9
        /// </summary>
        IRB1300_11_090,

        /// <summary>
        /// IRB1300-7/1.4
        /// </summary>
        IRB1300_7_140,

        /// <summary>
        /// IRB150-6/-.81
        /// </summary>
        IRB140_6_081,

        /// <summary>
        /// IRB1600-X/1.2
        /// </summary>
        IRB1600_X_120,

        /// <summary>
        /// IRB1600-X/1.45
        /// </summary>
        IRB1600_X_145,

        /// <summary>
        /// IRB1660ID-X/1.55
        /// </summary>
        IRB1660ID_X_155,

        /// <summary>
        /// IRB2600-12/1.85
        /// </summary>
        IRB2600_12_185,

        /// <summary>
        /// IRB2600-X/1.65
        /// </summary>
        IRB2600_X_165, 

        /// <summary>
        /// IRB2600ID-15/1.85
        /// </summary>
        IRB2600ID_15_185,

        /// <summary>
        /// IRB2600ID-8/2.0
        /// </summary>
        IRB2600ID_8_200,

        /// <summary>
        /// IRB4600-20/2.5
        /// </summary>
        IRB4600_20_250,

        /// <summary>
        /// IRB4600-40/2.55
        /// </summary>
        IRB4600_40_255,

        /// <summary>
        /// IRB4600-X/2.05
        /// </summary>
        IRB4600_X_205,

        /// <summary>
        /// IRB6620-150/2.2
        /// </summary>
        IRB6620_150_220,

        /// <summary>
        /// IRB6640-235/2.55
        /// </summary>
        IRB6640_235_255,

        /// <summary>
        /// IRB6650-125/3.2
        /// </summary>
        IRB6650_125_320,

        /// <summary>
        /// IRB6650-200/2.75
        /// </summary>
        IRB6650_200_275,

        /// <summary>
        /// IRB6650S-125/3.5
        /// </summary>
        IRB6650S_125_350,

        /// <summary>
        /// IRB6650S-200/3.0
        /// </summary>
        IRB6650S_200_300,

        /// <summary>
        /// IRB6650S-90/3.9
        /// </summary>
        IRB6650S_90_390,

        /// <summary>
        /// IRB6700-150/3.20
        /// </summary>
        IRB6700_150_320,

        /// <summary>
        /// IRB6700-1.55/2.85
        /// </summary>
        IRB6700_155_285,

        /// <summary>
        /// IRB6700-175/3.05
        /// </summary>
        IRB6700_175_305,

        /// <summary>
        /// IRB6700-200/2.60
        /// </summary>
        IRB6700_200_260,

        /// <summary>
        /// IRB6700-205/2.80
        /// </summary>
        IRB6700_205_280,

        /// <summary>
        /// IRB6700-235/2.65
        /// </summary>
        IRB6700_235_265,

        /// <summary>
        /// IRB6700-245/3.0
        /// </summary>
        IRB6700_245_300,

        /// <summary>
        /// IRB6700-300/2.7
        /// </summary>
        IRB6700_300_270,

        /// <summary>
        /// IRB6790-205/2.8
        /// </summary>
        IRB6790_205_280,

        /// <summary>
        /// IRB6790-235/2.65
        /// </summary>
        IRB6790_235_265,

        /// <summary>
        /// IRB7600-150/3.5
        /// </summary>
        IRB7600_150_350,

        /// <summary>
        /// IRB7600-325/3.1
        /// </summary>
        IRB7600_325_310,

        /// <summary>
        /// IRB7600-340/2.8
        /// </summary>
        IRB7600_340_280,

        /// <summary>
        /// IRB7600-400/2.55
        /// </summary>
        IRB7600_400_255,

        /// <summary>
        /// IRB7600-500/2.55
        /// </summary>
        IRB7600_500_255
    }
}

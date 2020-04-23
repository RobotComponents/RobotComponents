// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/EDEK-UniKassel/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
// RobotComponents Libs
using RobotComponentsABB.Components.CodeGeneration;
using RobotComponentsABB.Components.ControllerUtility;
using RobotComponentsABB.Components.Deconstruct;
using RobotComponentsABB.Components.Simulation;
using RobotComponentsABB.Components.Definitions;
using RobotComponentsABB.Components.Utilities;

namespace RobotComponentsABB.Utils
{
    /// <summary>
    /// Static class that contains all the data (e.g. links to pages) that is relevant for our documentation.
    /// </summary>
    public static class Documentation
    {
        /// <summary>
        /// Dictionary that contains all the links to the documention of the different component types.
        /// </summary>
        public readonly static Dictionary<Type, string> ComponentWeblinks = new Dictionary<Type, string>()
        {

            { typeof(Documentation), "https://edek-unikassel.github.io/RobotComponents-Documentation/" },

            #region Code generation
            { typeof(AutoAxisConfigComponent) , "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Code%20Generation/Action%20Auto%20Axis%20Configuration/" },
            { typeof(AbsoluteJointMovementComponent) , "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Code%20Generation/Action%20Absolute%20Joint%20Movement/" },
            { typeof(CodeLineComponent) , "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Code%20Generation/Action%20Code%20Line/" },
            { typeof(CommentComponent) , "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Code%20Generation/Action%20Comment/" },
            { typeof(DigitalOutputComponent) , "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Code%20Generation/Action%20Digital%20Output/" },
            { typeof(MovementComponent) , "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Code%20Generation/Action%20Movement/" },
            { typeof(OverrideRobotToolComponent) , "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Code%20Generation/Action%20Set%20Robot%20Tool/" },
            { typeof(RAPIDGeneratorComponent) , "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Code%20Generation/RAPID%20Generator/" },
            { typeof(SpeedDataComponent) , "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Code%20Generation/Action%20Speed%20Data/" },
            { typeof(TargetComponent) , "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Code%20Generation/Action%20Target/" },
            { typeof(TimerComponent) , "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Code%20Generation/Action%20Timer/" },
            { typeof(WaitDIComponent) , "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Code%20Generation/Action%20Wait%20for%20Digital%20Input/" },
            { typeof(ZoneDataComponent), "https://edek-unikassel.github.io/RobotComponents-Documentation/" }, // TODO: make webpage...
            #endregion

            #region Controller utility
            { typeof(GetAxisValuesComponent), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Controller%20Utility/Get%20Axis%20Values/" },
            { typeof(GetControllerComponent), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Controller%20Utility/Get%20Controller/" },
            { typeof(GetDigitalInputComponent), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Controller%20Utility/Get%20Digital%20Input/" },
            { typeof(GetDigitalOutputComponent), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Controller%20Utility/Get%20Digital%20Output/" },
            { typeof(GetPlaneComponent), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Controller%20Utility/Get%20Plane/" },
            { typeof(RemoteConnectionComponent), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Controller%20Utility/Remote%20Connection/" },
            { typeof(SetDigitalInputComponent), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Controller%20Utility/Set%20Digital%20Input/" },
            { typeof(SetDigitalOutputComponent), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Controller%20Utility/Set%20Digital%20Output/" },
            #endregion

            #region Deconstruct
            { typeof(DeconstructAbsoluteJointMovementComponent), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Deconstruct/Deconstruct%20Absolute%20Joint%20Movement/" },
            { typeof(DeconstructExternalLinearAxisComponent), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Deconstruct/Deconstruct%20External%20Linear%20Axis/" },
            { typeof(DeconstructExternalRotationalAxisComponent), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Deconstruct/Deconstruct%20External%20Rotational%20Axis/" },
            { typeof(DeconstructMovementComponent), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Deconstruct/Deconstruct%20Movement/" },
            { typeof(DeconstructRobotInfoComponent), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Deconstruct/Deconstruct%20Robot%20Info/" },
            { typeof(DeconstructRobotToolComponent), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Deconstruct/Deconstruct%20Robot%20Tool/" },
            { typeof(DeconstructSpeedDataComponent), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Deconstruct/Deconstruct%20Speed%20Data/" },
            { typeof(DeconstructTargetComponent), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Deconstruct/Deconstruct%20Target/" },
            { typeof(DeconstructWorkObjectComponent), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Deconstruct/Deconstruct%20Work%20Object/" },
            { typeof(DeconstructZoneDataComponent), "https://edek-unikassel.github.io/RobotComponents-Documentation/" }, // TODO: make webpage...
            #endregion

            #region Definitions
            { typeof(ExternalLinearAxisComponent), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Definitions/External%20Linear%20Axis/" },
            { typeof(ExternalRotationalAxisComponent), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Definitions/External%20Rotational%20Axis/" },
            { typeof(RobotInfoComponent), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Definitions/Robot%20Info/" },
            { typeof(RobotToolFromDataEulerComponent), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Definitions/Robot%20Tool%20from%20Data/" },
            { typeof(RobotToolFromPlanesComponent), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Definitions/Robot%20Tool%20from%20Planes/" },
            { typeof(RobotToolFromQuaternionComponent), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Definitions/Robot%20Tool%20from%20Quaternion%20Data/" },
            { typeof(WorkObjectComponent), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Definitions/Work%20Object/" },
            #endregion

            #region Definitions: Robot Info presets
            { typeof(IRB120_3_0_58_Component), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Definitions/Robot%20Info%20Presets/" },
            { typeof(IRB1200_5_90_Component), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Definitions/Robot%20Info%20Presets/" },
            { typeof(IRB1200_7_70_Component), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Definitions/Robot%20Info%20Presets/" },
            { typeof(IRB140_6_0_81_Component), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Definitions/Robot%20Info%20Presets/" },
            { typeof(IRB1600_X_1_20_Component), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Definitions/Robot%20Info%20Presets/" },
            { typeof(IRB1600_X_1_45_Component), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Definitions/Robot%20Info%20Presets/" },
            { typeof(IRB1660ID_Component), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Definitions/Robot%20Info%20Presets/" },
            { typeof(IRB2600_12_1_85_Component), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Definitions/Robot%20Info%20Presets/" },
            { typeof(IRB2600_X_1_65_Component), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Definitions/Robot%20Info%20Presets/" },
            { typeof(IRB2600ID_15_1_85_Component), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Definitions/Robot%20Info%20Presets/" },
            { typeof(IRB2600ID_8_2_00_Component), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Definitions/Robot%20Info%20Presets/" },
            { typeof(IRB4600_20_2_50_Component), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Definitions/Robot%20Info%20Presets/" },
            { typeof(IRB4600_40_2_55_Component), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Definitions/Robot%20Info%20Presets/" },
            { typeof(IRB4600_X_2_05_Component), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Definitions/Robot%20Info%20Presets/" },
            { typeof(IRB6620_150_2_20_Component), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Definitions/Robot%20Info%20Presets/" },
            { typeof(IRB6640_185_2_80_Component), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Definitions/Robot%20Info%20Presets/" },
            { typeof(IRB6640_235_2_55_Component), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Definitions/Robot%20Info%20Presets/" },
            { typeof(IRB6650_125_3_20_Component), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Definitions/Robot%20Info%20Presets/" },
            { typeof(IRB6650_200_2_75_Component), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Definitions/Robot%20Info%20Presets/" },
            { typeof(IRB6650S_125_3_50_Component), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Definitions/Robot%20Info%20Presets/" },
            { typeof(IRB6650S_200_3_00_Component), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Definitions/Robot%20Info%20Presets/" },
            { typeof(IRB6650S_90_3_90_Component), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Definitions/Robot%20Info%20Presets/" },
            { typeof(IRB6790_235_2_65_Component), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Definitions/Robot%20Info%20Presets/" },
            { typeof(IRB6790_205_2_80_Component), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Definitions/Robot%20Info%20Presets/" },
            { typeof(IRB7600_150_3_50_Component), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Definitions/Robot%20Info%20Presets/" },
            { typeof(IRB7600_325_3_10_Component), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Definitions/Robot%20Info%20Presets/" },
            { typeof(IRB7600_340_2_80_Component), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Definitions/Robot%20Info%20Presets/" },
            { typeof(IRB7600_400_2_55_Component), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Definitions/Robot%20Info%20Presets/" },
            { typeof(IRB7600_500_2_55_Component), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Definitions/Robot%20Info%20Presets/" },
            #endregion

            #region Simulation
            { typeof(ForwardKinematicsComponent), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Simulation/Forward%20Kinematics/" },
            { typeof(InverseKinematicsComponent), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Simulation/Inverse%20Kinematics/" },
            { typeof(PathGeneratorComponent), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Simulation/Path%20Generator/" },
            #endregion

            #region Utilities
            { typeof(FlipPlaneXComponent), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Utility/Flip%20Plane%20X/" },
            { typeof(FlipPlaneYComponent), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Utility/Flip%20Plane%20Y/" },
            { typeof(GetObjectManager), "https://edek-unikassel.github.io/RobotComponents-Documentation/" }, // TODO: make webpage...
            { typeof(NamingComponent), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Utility/Naming/" },
            { typeof(PlaneToQuaternion), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Utility/Plane%20to%20Quarternion/" },
            { typeof(PlaneVisualizerComponent), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Utility/Plane%20Visualizer/" },
            { typeof(QuaternionToPlane), "https://edek-unikassel.github.io/RobotComponents-Documentation/docs/Robot%20Components/Categories/Utility/Quaternion%20to%20Plane/" },
            { typeof(InfoComponent), "https://edek-unikassel.github.io/RobotComponents-Documentation/"},
            #endregion
        };
    }
}

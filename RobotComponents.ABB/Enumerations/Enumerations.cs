// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

namespace RobotComponents.ABB.Enumerations
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
    /// Defines the scope of the declaration.
    /// </summary>
    public enum Scope : int
    {
        /// <summary>
        /// Global level
        /// </summary>
        GLOBAL = 0,

        /// <summary>
        /// Local level
        /// </summary>
        LOCAL = 1,

        /// <summary>
        /// Task level
        /// </summary>
        TASK = 2
    }

    /// <summary>
    /// Defines the variable type of the declaration.
    /// </summary>
    public enum VariableType : int
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
    /// Defines inequalities (less than, greater than)
    /// </summary>
    public enum InequalitySymbol : int
    {
        /// <summary>
        /// Less than
        /// </summary>
        LT = 0,

        /// <summary>
        /// Greater than
        /// </summary>
        GT = 1
    }

    /// <summary>
    /// Defines predefined speeddata values.
    /// </summary>
    public enum PredefinedSpeedData : int
    {
        /// <summary>
        /// Predefined speeddata v5
        /// </summary>
        v5 = 5,

        /// <summary>
        /// Predefined speeddata v10
        /// </summary>
        v10 = 10,

        /// <summary>
        /// Predefined speeddata v20
        /// </summary>
        v20 = 20,

        /// <summary>
        /// Predefined speeddata v30
        /// </summary>
        v30 = 30,

        /// <summary>
        /// Predefined speeddata v40
        /// </summary>
        v40 = 40,

        /// <summary>
        /// Predefined speeddata v50
        /// </summary>
        v50 = 50,

        /// <summary>
        /// Predefined speeddata v60
        /// </summary>
        v60 = 60,

        /// <summary>
        /// Predefined speeddata v80
        /// </summary>
        v80 = 80,

        /// <summary>
        /// Predefined speeddata v100
        /// </summary>
        v100 = 100,

        /// <summary>
        /// Predefined speeddata v150
        /// </summary>
        v150 = 150,

        /// <summary>
        /// Predefined speeddata v200
        /// </summary>
        v200 = 200,

        /// <summary>
        /// Predefined speeddata v300
        /// </summary>
        v300 = 300,

        /// <summary>
        /// Predefined speeddata v400
        /// </summary>
        v400 = 400,

        /// <summary>
        /// Predefined speeddata v500
        /// </summary>
        v500 = 500,

        /// <summary>
        /// Predefined speeddata v600
        /// </summary>
        v600 = 600,

        /// <summary>
        /// Predefined speeddata v800
        /// </summary>
        v800 = 800,

        /// <summary>
        /// Predefined speeddata v1000
        /// </summary>
        v1000 = 1000,

        /// <summary>
        /// Predefined speeddata v1500
        /// </summary>
        v1500 = 1500,

        /// <summary>
        /// Predefined speeddata v2000
        /// </summary>
        v2000 = 2000,

        /// <summary>
        /// Predefined speeddata v2500
        /// </summary>
        v2500 = 2500,

        /// <summary>
        /// Predefined speeddata v3000
        /// </summary>
        v3000 = 3000,

        /// <summary>
        /// Predefined speeddata v4000
        /// </summary>
        v4000 = 4000,

        /// <summary>
        /// Predefined speeddata v5000
        /// </summary>
        v5000 = 5000,

        /// <summary>
        /// Predefined speeddata v6000
        /// </summary>
        v6000 = 6000,

        /// <summary>
        /// Predefined speeddata v7000
        /// </summary>
        v7000 = 7000
    }

    /// <summary>
    /// Defines predefined zonedata values.
    /// </summary>
    public enum PredefinedZoneData : int
    {
        /// <summary>
        /// Predefined zonedata fine
        /// </summary>
        fine = -1,

        /// <summary>
        /// Predefined zonedata z0
        /// </summary>
        z0 = 0,

        /// <summary>
        /// Predefined zonedata z1
        /// </summary>
        z1 = 1,

        /// <summary>
        /// Predefined zonedata z5
        /// </summary>
        z5 = 5,

        /// <summary>
        /// Predefined zonedata z10
        /// </summary>
        z10 = 10,

        /// <summary>
        /// Predefined zonedata z15
        /// </summary>
        z15 = 15,

        /// <summary>
        /// Predefined zonedata z20
        /// </summary>
        z20 = 20,

        /// <summary>
        /// Predefined zonedata z30
        /// </summary>
        z30 = 30,

        /// <summary>
        /// Predefined zonedata z40
        /// </summary>
        z40 = 40,

        /// <summary>
        /// Predefined zonedata z50
        /// </summary>
        z50 = 50,

        /// <summary>
        /// Predefined zonedata z60
        /// </summary>
        z60 = 60,

        /// <summary>
        /// Predefined zonedata z80
        /// </summary>
        z80 = 80,

        /// <summary>
        /// Predefined zonedata z100
        /// </summary>
        z100 = 100,

        /// <summary>
        /// Predefined zonedata z15-
        /// </summary>
        z150 = 150,

        /// <summary>
        /// Predefined zonedata z200
        /// </summary>
        z200 = 200
    }
}

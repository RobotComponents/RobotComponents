// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
// RobotComponents Libs
using RobotComponents.Definitions;

namespace RobotComponents.Actions
{
    /// <summary>
    /// Robot Joint Position class, defines the robot axis positions of the in degrees.
    /// </summary>
    public class RobotJointPosition : Action, IJointPosition
    {
        #region fields
        private double _val1;
        private double _val2;
        private double _val3;
        private double _val4;
        private double _val5;
        private double _val6;

        private const double _defaultValue = 0.0;
        #endregion

        #region constructors
        /// <summary>
        /// An undefined Robot joint position. 
        /// All Robot axis values will be set to 0.0.
        /// </summary>
        public RobotJointPosition()
        {
            _val1 = _defaultValue;
            _val2 = _defaultValue;
            _val3 = _defaultValue;
            _val4 = _defaultValue;
            _val5 = _defaultValue;
            _val6 = _defaultValue;
        }

        /// <summary>
        /// Defines an Robot joint position. 
        /// </summary>
        /// <param name="rax_1"> The position of robot axis 1 in degrees from the calibration position. </param>
        /// <param name="rax_2"> The position of robot axis 2 in degrees from the calibration position.</param>
        /// <param name="rax_3"> The position of robot axis 3 in degrees from the calibration position.</param>
        /// <param name="rax_4"> The position of robot axis 4 in degrees from the calibration position.</param>
        /// <param name="rax_5"> The position of robot axis 5 in degrees from the calibration position.</param>
        /// <param name="rax_6"> The position of robot axis 6 in degrees from the calibration position.</param>
        public RobotJointPosition(double rax_1, double rax_2 = _defaultValue, double rax_3 = _defaultValue, double rax_4 = _defaultValue, double rax_5 = _defaultValue, double rax_6 = _defaultValue)
        {
            _val1 = rax_1;
            _val2 = rax_2;
            _val3 = rax_3;
            _val4 = rax_4;
            _val5 = rax_5;
            _val6 = rax_6;
        }

        /// <summary>
        /// Defines an robot joint position from a list with values.
        /// </summary>
        /// <param name="internalAxisValues"> The user defined internal axis values as a list.</param>
        public RobotJointPosition(List<double> internalAxisValues)
        {
            double[] values = CheckAxisValues(internalAxisValues.ToArray());

            _val1 = values[0];
            _val2 = values[1];
            _val3 = values[2];
            _val4 = values[3];
            _val5 = values[4];
            _val6 = values[5];
        }

        /// <summary>
        /// Defines an robot joint position from an array wth values.
        /// </summary>
        /// <param name="internalAxisValues">The user defined internal axis values as an array.</param>
        public RobotJointPosition(double[] internalAxisValues)
        {
            double[] values = CheckAxisValues(internalAxisValues);

            _val1 = values[0];
            _val2 = values[1];
            _val3 = values[2];
            _val4 = values[3];
            _val5 = values[4];
            _val6 = values[5];
        }

        /// <summary>
        /// Creates a new robot joint position by duplicating an robot joint position. 
        /// This creates a deep copy of the existing robot joint position. 
        /// </summary>
        /// <param name="robJointPosition"> The Robot joint position that should be duplicated. </param>
        public RobotJointPosition(RobotJointPosition robJointPosition)
        {
            _val1 = robJointPosition[0];
            _val2 = robJointPosition[1];
            _val3 = robJointPosition[2];
            _val4 = robJointPosition[3];
            _val5 = robJointPosition[4];
            _val6 = robJointPosition[5];
        }

        /// <summary>ot
        /// Method to duplicate the Robot Joint Position object.
        /// </summary>
        /// <returns>Returns a deep copy of the Robot Joint Position object.</returns>
        public RobotJointPosition Duplicate()
        {
            return new RobotJointPosition(this);
        }

        /// <summary>
        /// A method to duplicate the Robot Joint Position object to an Action object. 
        /// </summary>
        /// <returns> Returns a deep copy of the Robot Joint Position object as an Action object. </returns>
        public override Action DuplicateAction()
        {
            return new RobotJointPosition(this) as Action;
        }
        #endregion

        #region method
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns> A string that represents the current object. </returns>
        public override string ToString()
        {
            if (!this.IsValid)
            {
                return "Invalid Robot Joint Position";
            }
            else
            {
                return "Robot Joint Position";
            }
        }

        /// <summary>
        /// Copies the axis values of the joint position to a new array
        /// </summary>
        /// <returns> An array containing the axis values of the robot joint position. </returns>
        public double[] ToArray()
        {
            return new double[] { _val1, _val2, _val3, _val4, _val5, _val6 };
        }

        /// <summary>
        /// Copies the axis values of the joint position to a new list
        /// </summary>
        /// <returns> A list containing the axis values of the robot joint position. </returns>
        public List<double> ToList()
        {
            return new List<double>() { _val1, _val2, _val3, _val4, _val5, _val6 };
        }

        /// <summary>
        /// Computes the sum of the values in this joint position.
        /// </summary>
        /// <returns> The sum of the values in the joint position. </returns>
        public double Sum()
        {
            return _val1 + _val2 + _val3 + _val4 + _val5 + _val6;
        }

        /// <summary>
        /// Computes the norm of this joint position.
        /// </summary>
        /// <returns> The norm of the joint position. </returns>
        public double Norm()
        {
            return Math.Sqrt(this.NormSq());
        }

        /// <summary>
        /// Computes the square norm of this joint position.
        /// </summary>
        /// <returns> he square norm of this joint position. </returns>
        public double NormSq()
        {
            return (_val1 * _val1) + (_val2 * _val2) + (_val3 * _val3) + (_val4 * _val4) + (_val5 * _val5) + (_val6 * _val6);
        }

        /// <summary>
        /// Sets all the elements in the joint position back to its default value (0.0).
        /// </summary>
        public void Reset()
        {
            _val1 = _defaultValue;
            _val2 = _defaultValue;
            _val3 = _defaultValue;
            _val4 = _defaultValue;
            _val5 = _defaultValue;
            _val6 = _defaultValue;
        }


        /// <summary>
        /// Adds a constant number to all the values inside this Joint Position
        /// </summary>
        /// <param name="value"> The number that should be added. </param>
        public void Add(double value)
        {
            for (int i = 0; i < 6; i++)
            {
                this[i] += value;
            }
        }

        /// <summary>
        /// Adds the values of an Robot Joint Position to the values inside this Joint Position. 
        /// Value 1 + value 1, value 2 + value 2, value 3 + value 3 etc.
        /// </summary>
        /// <param name="jointPosition"> The Robot Joint Position that should be added. </param>
        public void Add(RobotJointPosition jointPosition)
        {
            for (int i = 0; i < 6; i++)
            {
                this[i] += jointPosition[i];
            }
        }

        /// <summary>
        /// Substracts a constant number from the values inside this Joint Position
        /// </summary>
        /// <param name="value"> The number that should be substracted. </param>
        public void Substract(double value)
        {
            for (int i = 0; i < 6; i++)
            {
                this[i] -= value;
            }
        }

        /// <summary>
        /// Substracts the values of an Robot Joint Position from the values inside this Joint Position. 
        /// Value 1 - value 1, value 2 - value 2, value 3 - value 3 etc.
        /// </summary>
        /// <param name="jointPosition"> The Robot Joint Position that should be substracted. </param>
        public void Substract(RobotJointPosition jointPosition)
        {
            for (int i = 0; i < 6; i++)
            {
                this[i] -= jointPosition[i];
            }
        }

        /// <summary>
        /// Multiplies the values inside this Joint Position with a constant number.
        /// </summary>
        /// <param name="value"> The multiplier as a double. </param>
        public void Multiply(double value)
        {
            for (int i = 0; i < 6; i++)
            {
                this[i] *= value;
            }
        }

        /// <summary>
        /// Multiplies the values inside this Joint Position with the values from another Robot Joint Position.
        /// Value 1 * value 1, value 2 * value 2, value 3 * value 3 etc.
        /// </summary>
        /// <param name="jointPosition"> The multiplier as an Robot Joint Position. </param>
        public void Multiply(RobotJointPosition jointPosition)
        {
            for (int i = 0; i < 6; i++)
            {
                this[i] *= jointPosition[i];
            }
        }

        /// <summary>
        /// Divides the values inside this Joint Position with a constant number.
        /// </summary>
        /// <param name="value"> The divider as a double. </param>
        public void Divide(double value)
        {
            if (value == 0)
            {
                new DivideByZeroException();
            }

            for (int i = 0; i < 6; i++)
            {
                this[i] /= value;
            }
        }

        /// <summary>
        /// Divides the values inside this Joint Position with the values from another Robot Joint Position.
        /// Value 1 / value 1, value 2 / value 2, value 3 / value 3 etc.
        /// </summary>
        /// <param name="jointPosition"> The divider as an Robot Joint Position. </param>
        public void Divide(RobotJointPosition jointPosition)
        {
            for (int i = 0; i < 6; i++)
            {
                if (jointPosition[i] == 0)
                {
                    new DivideByZeroException();
                }
                
                this[i] /= jointPosition[i];
            }
        }

        /// <summary></summary>
        /// Method that checks the array with internal axis values. 
        /// Always returns a list with 6 internal axis values. 
        /// For missing values 0.0 will be used. 
        /// <param name="axisValues">A list with the internal axis values.</param>
        /// <returns> Returns an array with 6 Robot axis values.</returns>
        private double[] CheckAxisValues(double[] axisValues)
        {
            double[] result = new double[6];
            int n = Math.Min(axisValues.Length, 6);

            // Copy definied axis values
            for (int i = 0; i < n; i++)
            {
                result[i] = axisValues[i];
            }

            // Add missing axis values
            for (int i = n; i < 6; i++)
            {
                result[i] = _defaultValue;
            }

            return result;
        }

        /// <summary>
        /// Used to create variable definition code of this action. 
        /// </summary>
        /// <param name="robot"> Defines the Robot were the code is generated for. </param>
        /// <returns> Returns the RAPID code line as a string.  </returns>
        public override string ToRAPIDDeclaration(Robot robot)
        {
            string code = "[";

            code += _val1.ToString("0.##") + ", ";
            code += _val2.ToString("0.##") + ", ";
            code += _val3.ToString("0.##") + ", ";
            code += _val4.ToString("0.##") + ", ";
            code += _val5.ToString("0.##") + ", ";
            code += _val6.ToString("0.##") + "]";

            return code;
        }

        /// <summary>
        /// Used to create action instruction code line. 
        /// </summary>
        /// <param name="robot"> Defines the Robot were the code is generated for. </param>
        /// <returns> Returns an empty string.  </returns>
        public override string ToRAPIDInstruction(Robot robot)
        {
            return string.Empty;
        }

        /// <summary>
        /// Used to create variable definitions in the RAPID Code. It is typically called inside the CreateRAPIDCode() method of the RAPIDGenerator class.
        /// </summary>
        /// <param name="RAPIDGenerator"> Defines the RAPIDGenerator. </param>
        public override void ToRAPIDDeclaration(RAPIDGenerator RAPIDGenerator)
        {
        }

        /// <summary>
        /// Used to create action instructions in the RAPID Code. It is typically called inside the CreateRAPIDCode() method of the RAPIDGenerator class.
        /// </summary>
        /// <param name="RAPIDGenerator"> Defines the RAPIDGenerator. </param>
        public override void ToRAPIDInstruction(RAPIDGenerator RAPIDGenerator)
        {
        }
        #endregion

        #region properties
        /// <summary>
        /// Defines if the Robot Joint Position object is valid.
        /// </summary>
        public override bool IsValid
        {
            get { return true; }
        }

        /// <summary>
        /// Defines the number of elements in the Robot Joint Position
        /// </summary>
        public int Length
        {
            get { return 6; }
        }

        /// <summary>
        /// Get or set axis values through the indexer. 
        /// </summary>
        /// <param name="index"> The index number. </param>
        /// <returns> The axis value located at the given index. </returns>
        public double this[int index]
        {
            get
            {
                if (index == 0) { return _val1; }
                else if (index == 1) { return _val2; }
                else if (index == 2) { return _val3; }
                else if (index == 3) { return _val4; }
                else if (index == 4) { return _val5; }
                else if (index == 5) { return _val6; }
                else { throw new IndexOutOfRangeException(); }
            }
            set
            {
                if (index == 0) { _val1 = value; }
                else if (index == 1) { _val2 = value; }
                else if (index == 2) { _val3 = value; }
                else if (index == 3) { _val4 = value; }
                else if (index == 4) { _val5 = value; }
                else if (index == 5) { _val6 = value; }
                else { throw new IndexOutOfRangeException(); }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the joint position array has a fixed size.
        /// </summary>
        public bool IsFixedSize
        {
            get { return true; }
        }
        #endregion

        #region operators
        /// <summary>
        /// Adds a number to all the values inside the Robot Joint Position.
        /// </summary>
        /// <param name="p"> The Robot Joint Position. </param>
        /// <param name="num"> The value to add. </param>
        /// <returns> The Robot Joint Position with added values. </returns>
        public static RobotJointPosition operator +(RobotJointPosition p, double num)
        {
            return new RobotJointPosition(p[0] + num, p[1] + num, p[2] + num, p[3] + num, p[4] + num, p[5] + num);
        }

        /// <summary>
        /// Substracts a number from all the values inside the Robot Joint Position.
        /// </summary>
        /// <param name="p"> The Robot Joint Position. </param>
        /// <param name="num"> The number to substract. </param>
        /// <returns> The Robot Joint Position with divide values. </returns>
        public static RobotJointPosition operator -(RobotJointPosition p, double num)
        {
            return new RobotJointPosition(p[0] - num, p[1] - num, p[2] - num, p[3] - num, p[4] - num, p[5] - num);
        }

        /// <summary>
        /// Mutliplies all the values inside the Robot Joint Position by a number.
        /// </summary>
        /// <param name="p"> The Robot Joint Position. </param>
        /// <param name="num"> The value to multiply by. </param>
        /// <returns> The Robot Joint Position with multuplied values. </returns>
        public static RobotJointPosition operator *(RobotJointPosition p, double num)
        {
            return new RobotJointPosition(p[0] * num, p[1] * num, p[2] * num, p[3] * num, p[4] * num, p[5] * num);
        }

        /// <summary>
        /// Divides all the values inside the Robot Joint Position by a number. 
        /// </summary>
        /// <param name="p"> The Robot Joint Position. </param>
        /// <param name="num"> The number to divide by. </param>
        /// <returns> The Robot Joint Position with divide values. </returns>
        public static RobotJointPosition operator /(RobotJointPosition p, double num)
        {
            if (num == 0)
            {
                throw new DivideByZeroException();
            }

            return new RobotJointPosition(p[0] / num, p[1] / num, p[2] / num, p[3] / num, p[4] / num, p[5] / num);
        }

        /// <summary>
        /// Addition of two Robot Joint Position
        /// </summary>
        /// <param name="p1"> The first Robot Joint Position. </param>
        /// <param name="p2"> The second Robot Joint Position. </param>
        /// <returns> The addition of the two Robot Joint Poistion</returns>
        public static RobotJointPosition operator +(RobotJointPosition p1, RobotJointPosition p2)
        {
            return new RobotJointPosition(p1[0] + p2[0], p1[1] + p2[1], p1[2] + p2[2], p1[3] + p2[3], p1[4] + p2[4], p1[5] + p2[5]);
        }

        /// <summary>
        /// Substraction of two Robot Joint Position
        /// </summary>
        /// <param name="p1"> The first Robot Joint Position. </param>
        /// <param name="p2"> The second Robot Joint Position. </param>
        /// <returns> The first robot Joint Position minus the second Robot Joint Position. </returns>
        public static RobotJointPosition operator -(RobotJointPosition p1, RobotJointPosition p2)
        {
            return new RobotJointPosition(p1[0] - p2[0], p1[1] - p2[1], p1[2] - p2[2], p1[3] - p2[3], p1[4] - p2[4], p1[5] - p2[5]);
        }

        /// <summary>
        /// Multiplication of two Robot Joint Positions
        /// </summary>
        /// <param name="p1"> The first Robot Joint Position. </param>
        /// <param name="p2"> The second Robot Joint Position. </param>
        /// <returns> The multiplicaton of the two Robot Joint Positions. </returns>
        public static RobotJointPosition operator *(RobotJointPosition p1, RobotJointPosition p2)
        {
            return new RobotJointPosition(p1[0] * p2[0], p1[1] * p2[1], p1[2] * p2[2], p1[3] * p2[3], p1[4] * p2[4], p1[5] * p2[5]);
        }

        /// <summary>
        /// Division of a Robot Joint Position by another Robot Joint Position
        /// </summary>
        /// <param name="p1"> The first Robot Joint Position. </param>
        /// <param name="p2"> The second Robot Joint Position. </param>
        /// <returns> The first Robot Joint Position divided by the second Robot Joint Position. </returns>
        public static RobotJointPosition operator /(RobotJointPosition p1, RobotJointPosition p2)
        {
            if (p2[0] == 0 || p2[1] == 0 || p2[2] == 0 || p2[3] == 0 || p2[4] == 0 || p2[5] == 0)
            {
                throw new DivideByZeroException();
            }

            RobotJointPosition result = new RobotJointPosition();

            for (int i = 0; i < 6; i++)
            {
                result[i] = p1[i] / p2[i];
            }

            return result;
        }
        #endregion
    }

}

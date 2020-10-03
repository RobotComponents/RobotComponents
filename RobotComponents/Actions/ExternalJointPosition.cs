// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
// RobotComponents Libs
using RobotComponents.Definitions;

namespace RobotComponents.Actions
{
    /// <summary>
    /// External Joint Position class, defines external joint position data. Is used to define 
    /// the axis positions of external axes, positioners and workpiece manipulators. 
    /// </summary>
    [Serializable()]
    public class ExternalJointPosition : Action, IJointPosition, ISerializable
    {
        #region fields
        private double _val1;
        private double _val2;
        private double _val3;
        private double _val4;
        private double _val5;
        private double _val6;

        private const double _defaultValue = 9e9;
        #endregion

        #region (de)serialization
        /// <summary>
        /// Special contructor needed for deserialization of the object. 
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization.</param>
        protected ExternalJointPosition(SerializationInfo info, StreamingContext context)
        {
            _val1 = (double)info.GetValue("Axis value 1", typeof(double));
            _val2 = (double)info.GetValue("Axis value 2", typeof(double));
            _val3 = (double)info.GetValue("Axis value 3", typeof(double));
            _val4 = (double)info.GetValue("Axis value 4", typeof(double));
            _val5 = (double)info.GetValue("Axis value 5", typeof(double));
            _val6 = (double)info.GetValue("Axis value 6", typeof(double));
        }

        /// <summary>
        /// Populates a SerializationInfo with the data needed to serialize the object.
        /// </summary>
        /// <param name="info"> The SerializationInfo to populate with data. </param>
        /// <param name="context"> The destination for this serialization. </param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Axis value 1", _val1, typeof(double));
            info.AddValue("Axis value 2", _val2, typeof(double));
            info.AddValue("Axis value 3", _val3, typeof(double));
            info.AddValue("Axis value 4", _val4, typeof(double));
            info.AddValue("Axis value 5", _val5, typeof(double));
            info.AddValue("Axis value 6", _val6, typeof(double));
        }
        #endregion

        #region constructors
        /// <summary>
        /// An undefined external joint position. 
        /// All external axis values will be set to 9E9.
        /// </summary>
        public ExternalJointPosition()
        {
            _val1 = _defaultValue;
            _val2 = _defaultValue;
            _val3 = _defaultValue;
            _val4 = _defaultValue;
            _val5 = _defaultValue;
            _val6 = _defaultValue;
        }


        /// <summary>
        /// Defines an external joint position. 
        /// </summary>
        /// <param name="Eax_a"> The position of the external logical axis “a” expressed in degrees or mm. </param>
        /// <param name="Eax_b"> The position of the external logical axis “b” expressed in degrees or mm.</param>
        /// <param name="Eax_c"> The position of the external logical axis “c” expressed in degrees or mm.</param>
        /// <param name="Eax_d"> The position of the external logical axis “d” expressed in degrees or mm.</param>
        /// <param name="Eax_e"> The position of the external logical axis “3” expressed in degrees or mm.</param>
        /// <param name="Eax_f"> The position of the external logical axis “f” expressed in degrees or mm.</param>
        public ExternalJointPosition(double Eax_a, double Eax_b = _defaultValue, double Eax_c = _defaultValue, double Eax_d = _defaultValue, double Eax_e = _defaultValue, double Eax_f = _defaultValue)
        {
            _val1 = Eax_a;
            _val2 = Eax_b;
            _val3 = Eax_c;
            _val4 = Eax_d;
            _val5 = Eax_e;
            _val6 = Eax_f;
        }

        /// <summary>
        /// Defines an external joint position from a list with values.
        /// </summary>
        /// <param name="externalAxisValues"> The user defined external axis values as a list.</param>
        public ExternalJointPosition(List<double> externalAxisValues)
        {
            double[] values = CheckAxisValues(externalAxisValues.ToArray());

            _val1 = values[0];
            _val2 = values[1];
            _val3 = values[2];
            _val4 = values[3];
            _val5 = values[4];
            _val6 = values[5];
        }

        /// <summary>
        /// Defines an external joint position from an array wth values.
        /// </summary>
        /// <param name="externalAxisValues">The user defined external axis values as an array.</param>
        public ExternalJointPosition(double[] externalAxisValues)
        {
            double[] values = CheckAxisValues(externalAxisValues);

            _val1 = values[0];
            _val2 = values[1];
            _val3 = values[2];
            _val4 = values[3];
            _val5 = values[4];
            _val6 = values[5];
        }

        /// <summary>
        /// Creates a new external joint position by duplicating an existing external joint position. 
        /// This creates a deep copy of the existing external joint position. 
        /// </summary>
        /// <param name="extJointPosition"> The external joint position that should be duplicated. </param>
        public ExternalJointPosition(ExternalJointPosition extJointPosition)
        {
            _val1 = extJointPosition[0];
            _val2 = extJointPosition[1];
            _val3 = extJointPosition[2];
            _val4 = extJointPosition[3];
            _val5 = extJointPosition[4];
            _val6 = extJointPosition[5];
        }

        /// <summary>
        /// Method to duplicate the Ext Joint Position object.
        /// </summary>
        /// <returns>Returns a deep copy of the Ext Joint Position object.</returns>
        public ExternalJointPosition Duplicate()
        {
            return new ExternalJointPosition(this);
        }

        /// <summary>
        /// A method to duplicate the Ext Joint Position object to an Action object. 
        /// </summary>
        /// <returns> Returns a deep copy of the Ext Joint Position object as an Action object. </returns>
        public override Action DuplicateAction()
        {
            return new ExternalJointPosition(this) as Action;
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
                return "Invalid External Joint Position";
            }
            else
            {
                string result = "External Joint Position (";

                if (_val1 == _defaultValue) { result += "9E9, "; }
                else { result += _val1.ToString("0.##") + ", "; }

                if (_val2 == _defaultValue) { result += "9E9, "; }
                else { result += _val2.ToString("0.##") + ", "; }

                if (_val3 == _defaultValue) { result += "9E9, "; }
                else { result += _val3.ToString("0.##") + ", "; }

                if (_val4 == _defaultValue) { result += "9E9, "; }
                else { result += _val4.ToString("0.##") + ", "; }

                if (_val5 == _defaultValue) { result += "9E9, "; }
                else { result += _val5.ToString("0.##") + ", "; }

                if (_val6 == _defaultValue) { result += "9E9)"; }
                else { result += _val6.ToString("0.##") + ")"; }

                return result;
            }
        }

        /// <summary>
        /// Copies the axis values of the joint position to a new array
        /// </summary>
        /// <returns> An array containing the axis values of the external joint position. </returns>
        public double[] ToArray()
        {
            return new double[] { _val1, _val2, _val3, _val4, _val5, _val6 };
        }

        /// <summary>
        /// Copies the axis values of the joint position to a new list
        /// </summary>
        /// <returns> A list containing the axis values of the external joint position. </returns>
        public List<double> ToList()
        {
            return new List<double>() { _val1, _val2, _val3, _val4, _val5, _val6 };
        }

        /// <summary>
        /// Sets all the elements in the joint position back to its default value (9E9).
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
                if (this[i] != _defaultValue)
                {
                    this[i] += value;
                }
            }
        }

        /// <summary>
        /// Adds the values of an External Joint Position to the values inside this Joint Position. 
        /// Value 1 + value 1, value 2 + value 2, value 3 + value 3 etc.
        /// </summary>
        /// <param name="jointPosition"> The External Joint Position that should be added. </param>
        public void Add(ExternalJointPosition jointPosition)
        {
            for (int i = 0; i < 6; i++)
            {
                if (this[i] != _defaultValue && jointPosition[i] != _defaultValue)
                {
                    this[i] += jointPosition[i];
                }
                else if (this[i] == _defaultValue && this[i] == _defaultValue)
                {
                    this[i] = _defaultValue;
                }
                else
                {
                    throw new InvalidOperationException(String.Format("Mismatch between two External Joint Positions. A definied axis value [on index {0}] is combined with an undefinied axis value.", i));
                }
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
                if (this[i] != _defaultValue)
                {
                    this[i] -= value;
                }
            }
        }

        /// <summary>
        /// Substracts the values of an External Joint Position from the values inside this Joint Position. 
        /// Value 1 - value 1, value 2 - value 2, value 3 - value 3 etc.
        /// </summary>
        /// <param name="jointPosition"> The External Joint Position that should be substracted. </param>
        public void Substract(ExternalJointPosition jointPosition)
        {
            for (int i = 0; i < 6; i++)
            {
                if (this[i] != _defaultValue && jointPosition[i] != _defaultValue)
                {
                    this[i] -= jointPosition[i];
                }
                else if (this[i] == _defaultValue && this[i] == _defaultValue)
                {
                    this[i] = _defaultValue;
                }
                else
                {
                    throw new InvalidOperationException(String.Format("Mismatch between two External Joint Positions. A definied axis value [on index {0}] is combined with an undefinied axis value.", i));
                }
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
                if (this[i] != _defaultValue)
                {
                    this[i] *= value;
                }
            }
        }

        /// <summary>
        /// Multiplies the values inside this Joint Position with the values from another External Joint Position.
        /// Value 1 * value 1, value 2 * value 2, value 3 * value 3 etc.
        /// </summary>
        /// <param name="jointPosition"> The multiplier as an External Joint Position. </param>
        public void Multiply(ExternalJointPosition jointPosition)
        {
            for (int i = 0; i < 6; i++)
            {
                if (this[i] != _defaultValue && jointPosition[i] != _defaultValue)
                {
                    this[i] *= jointPosition[i];
                }
                else if (this[i] == _defaultValue && this[i] == _defaultValue)
                {
                    this[i] = _defaultValue;
                }
                else
                {
                    throw new InvalidOperationException(String.Format("Mismatch between two External Joint Positions. A definied axis value [on index {0}] is combined with an undefinied axis value.", i));
                }
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
                if (this[i] != _defaultValue)
                {
                    this[i] /= value;
                }
            }
        }

        /// <summary>
        /// Divides the values inside this Joint Position with the values from another External Joint Position.
        /// Value 1 / value 1, value 2 / value 2, value 3 / value 3 etc.
        /// </summary>
        /// <param name="jointPosition"> The divider as an External Joint Position. </param>
        public void Divide(ExternalJointPosition jointPosition)
        {
            for (int i = 0; i < 6; i++)
            {
                if (jointPosition[i] == 0)
                {
                    new DivideByZeroException();
                }
                else if (this[i] != _defaultValue && jointPosition[i] != _defaultValue)
                {
                    this[i] /= jointPosition[i];
                }
                else if (this[i] == _defaultValue && this[i] == _defaultValue)
                {
                    this[i] = _defaultValue;
                }
                else
                {
                    throw new InvalidOperationException(String.Format("Mismatch between two External Joint Positions. A definied axis value [on index {0}] is combined with an undefinied axis value.", i));
                }
            }
        }

        /// <summary></summary>
        /// Method that checks the array with external axis values. 
        /// Always returns a list with 6 external axis values. 
        /// For missing values 9E9 (not connected) will be used. 
        /// <param name="axisValues">A list with the external axis values.</param>
        /// <returns> Returns an array with 6 external axis values.</returns>
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

            if (_val1 == _defaultValue) { code += "9E9, "; }
            else { code += _val1.ToString("0.##") + ", "; }

            if (_val2 == _defaultValue) { code += "9E9, "; }
            else { code += _val2.ToString("0.##") + ", "; }

            if (_val3 == _defaultValue) { code += "9E9, "; }
            else { code += _val3.ToString("0.##") + ", "; }

            if (_val4 == _defaultValue) { code += "9E9, "; }
            else { code += _val4.ToString("0.##") + ", "; }

            if (_val5 == _defaultValue) { code += "9E9, "; }
            else { code += _val5.ToString("0.##") + ", "; }

            if (_val6 == _defaultValue) { code += "9E9]"; }
            else { code += _val6.ToString("0.##") + "]"; }

            return code;
        }

        /// <summary>
        /// Used to create action instruction code line. 
        /// </summary>
        /// <param name="robot"> Defines the Robot were the code is generated for. </param>
        /// <returns> Returns an empty string. </returns>
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
        /// Defines if the External Joint Position object is valid.
        /// </summary>
        public override bool IsValid
        {
            get { return true; }
        }

        /// <summary>
        /// Defines the number of elements in the External Joint Position
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
        /// Adds a number to all the values inside the External Joint Position.
        /// </summary>
        /// <param name="p"> The External Joint Position. </param>
        /// <param name="num"> The value to add. </param>
        /// <returns> The External Joint Position with added values. </returns>
        public static ExternalJointPosition operator +(ExternalJointPosition p, double num)
        {
            ExternalJointPosition externalJointPosition = new ExternalJointPosition();

            for (int i = 0; i < 6; i++)
            {
                if (p[i] != _defaultValue) { externalJointPosition[i] = p[i] + num; }
            }

            return externalJointPosition;
        }

        /// <summary>
        /// Substracts a number from all the values inside the External Joint Position.
        /// </summary>
        /// <param name="p"> The External Joint Position. </param>
        /// <param name="num"> The number to substract. </param>
        /// <returns> The External Joint Position with divide values. </returns>
        public static ExternalJointPosition operator -(ExternalJointPosition p, double num)
        {
            ExternalJointPosition externalJointPosition = new ExternalJointPosition();

            for (int i = 0; i < 6; i++)
            {
                if (p[i] != _defaultValue) { externalJointPosition[i] = p[i] - num; }
            }

            return externalJointPosition;
        }

        /// <summary>
        /// Mutliplies all the values inside the External Joint Position by a number.
        /// </summary>
        /// <param name="p"> The External Joint Position. </param>
        /// <param name="num"> The value to multiply by. </param>
        /// <returns> The External Joint Position with multuplied values. </returns>
        public static ExternalJointPosition operator *(ExternalJointPosition p, double num)
        {
            ExternalJointPosition externalJointPosition = new ExternalJointPosition();

            for (int i = 0; i < 6; i++)
            {
                if (p[i] != _defaultValue) { externalJointPosition[i] = p[i] * num; }
            }

            return externalJointPosition;
        }

        /// <summary>
        /// Divides all the values inside the External Joint Position by a number. 
        /// </summary>
        /// <param name="p"> The External Joint Position. </param>
        /// <param name="num"> The number to divide by. </param>
        /// <returns> The External Joint Position with divide values. </returns>
        public static ExternalJointPosition operator /(ExternalJointPosition p, double num)
        {
            if (num == 0)
            {
                throw new DivideByZeroException();
            }

            ExternalJointPosition externalJointPosition = new ExternalJointPosition();

            for (int i = 0; i < 6; i++)
            {
                if (p[i] != _defaultValue) { externalJointPosition[i] = p[i] / num; }
            }

            return externalJointPosition;
        }

        /// <summary>
        /// Addition of two External Joint Position
        /// </summary>
        /// <param name="p1"> The first External Joint Position. </param>
        /// <param name="p2"> The second External Joint Position. </param>
        /// <returns> The addition of the two Robot Joint Poistion</returns>
        public static ExternalJointPosition operator +(ExternalJointPosition p1, ExternalJointPosition p2)
        {
            ExternalJointPosition result = new ExternalJointPosition();

            for (int i = 0; i < 6; i++)
            {
                if (p1[i] != _defaultValue && p2[i] != _defaultValue)
                {
                    result[i] = p1[i] + p2[i];
                }
                else if (p1[i] == _defaultValue && p2[i] == _defaultValue)
                {
                    result[i] = _defaultValue;
                }
                else
                {
                    throw new InvalidOperationException(String.Format("Mismatch between two External Joint Positions. A definied axis value [on index {0}] is combined with an undefinied axis value.", i));
                }
            }

            return result;
        }

        /// <summary>
        /// Substraction of two External Joint Position
        /// </summary>
        /// <param name="p1"> The first External Joint Position. </param>
        /// <param name="p2"> The second External Joint Position. </param>
        /// <returns> The first External Joint Position minus the second External Joint Position. </returns>
        public static ExternalJointPosition operator -(ExternalJointPosition p1, ExternalJointPosition p2)
        {
            ExternalJointPosition result = new ExternalJointPosition();

            for (int i = 0; i < 6; i++)
            {
                if (p1[i] != _defaultValue && p2[i] != _defaultValue)
                {
                    result[i] = p1[i] - p2[i];
                }
                else if (p1[i] == _defaultValue && p2[i] == _defaultValue)
                {
                    result[i] = _defaultValue;
                }
                else
                {
                    throw new InvalidOperationException(String.Format("Mismatch between two External Joint Positions. A definied axis value [on index {0}] is combined with an undefinied axis value.", i));
                }
            }

            return result;
        }

        /// <summary>
        /// Multiplication of two External Joint Positions
        /// </summary>
        /// <param name="p1"> The first External Joint Position. </param>
        /// <param name="p2"> The second External Joint Position. </param>
        /// <returns> The multiplicaton of the two External Joint Positions. </returns>
        public static ExternalJointPosition operator *(ExternalJointPosition p1, ExternalJointPosition p2)
        {
            ExternalJointPosition result = new ExternalJointPosition();

            for (int i = 0; i < 6; i++)
            {
                if (p1[i] != _defaultValue && p2[i] != _defaultValue)
                {
                    result[i] = p1[i] * p2[i];
                }
                else if (p1[i] == _defaultValue && p2[i] == _defaultValue)
                {
                    result[i] = _defaultValue;
                }
                else
                {
                    throw new InvalidOperationException(String.Format("Mismatch between two External Joint Positions. A definied axis value [on index {0}] is combined with an undefinied axis value.", i));
                }
            }

            return result;
        }

        /// <summary>
        /// Division of a External Joint Position by another External Joint Position
        /// </summary>
        /// <param name="p1"> The first External Joint Position. </param>
        /// <param name="p2"> The second External Joint Position. </param>
        /// <returns> The first External Joint Position divided by the second External Joint Position. </returns>
        public static ExternalJointPosition operator /(ExternalJointPosition p1, ExternalJointPosition p2)
        {
            if (p2[0] == 0 || p2[1] == 0 || p2[2] == 0 || p2[3] == 0 || p2[4] == 0 || p2[5] == 0)
            {
                throw new DivideByZeroException();
            }

            ExternalJointPosition result = new ExternalJointPosition();

            for (int i = 0; i < 6; i++)
            {
                if (p1[i] != _defaultValue && p2[i] != _defaultValue)
                {
                    result[i] = p1[i] / p2[i];
                }
                else if (p1[i] == _defaultValue && p2[i] == _defaultValue)
                {
                    result[i] = _defaultValue;
                }
                else
                {
                    throw new InvalidOperationException(String.Format("Mismatch between two External Joint Positions. A definied axis value [on index {0}] is combined with an undefinied axis value.", i));
                }
            }

            return result;
        }
        #endregion
    }

}

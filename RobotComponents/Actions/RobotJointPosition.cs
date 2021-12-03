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
using RobotComponents.Enumerations;
using RobotComponents.Definitions;
using RobotComponents.Utils;

namespace RobotComponents.Actions
{
    /// <summary>
    /// Represents the Robot Joint Position declaration.
    /// This action is used to define the robot axis positions in degrees.
    /// </summary>
    [Serializable()]
    public class RobotJointPosition : Action, IDeclaration, IJointPosition, ISerializable
    {
        #region fields
        private ReferenceType _referenceType;
        private string _name;
        private double _val1;
        private double _val2;
        private double _val3;
        private double _val4;
        private double _val5;
        private double _val6;

        private const double _defaultValue = 0.0;
        #endregion

        #region (de)serialization
        /// <summary>
        /// Protected constructor needed for deserialization of the object.  
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization. </param>
        protected RobotJointPosition(SerializationInfo info, StreamingContext context)
        {
            int version = (int)info.GetValue("Version", typeof(int)); // <-- use this if the (de)serialization changes

            if (version <= 16000)
            {
                _referenceType = ReferenceType.CONST;
                _name = String.Empty;
            }
            else
            {
                _referenceType = (ReferenceType)info.GetValue("Reference Type", typeof(ReferenceType));
                _name = (string)info.GetValue("Name", typeof(string));
            }

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
            info.AddValue("Version", VersionNumbering.CurrentVersionAsInt, typeof(int));
            info.AddValue("Reference Type", _referenceType, typeof(ReferenceType));
            info.AddValue("Name", _name, typeof(string));
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
        /// Initializes a new instance of the Robot Joint Position class with an empty name an all axis values set to zero. 
        /// </summary>
        public RobotJointPosition()
        {
            _referenceType = ReferenceType.CONST;
            _name = string.Empty;

            _val1 = _defaultValue;
            _val2 = _defaultValue;
            _val3 = _defaultValue;
            _val4 = _defaultValue;
            _val5 = _defaultValue;
            _val6 = _defaultValue;
        }

        /// <summary>
        /// Initializes a new instance of the Robot Joint Position class with an empty name.
        /// </summary>
        /// <param name="rax_1"> The position of robot axis 1 in degrees from the calibration position. </param>
        /// <param name="rax_2"> The position of robot axis 2 in degrees from the calibration position.</param>
        /// <param name="rax_3"> The position of robot axis 3 in degrees from the calibration position.</param>
        /// <param name="rax_4"> The position of robot axis 4 in degrees from the calibration position.</param>
        /// <param name="rax_5"> The position of robot axis 5 in degrees from the calibration position.</param>
        /// <param name="rax_6"> The position of robot axis 6 in degrees from the calibration position.</param>
        public RobotJointPosition(double rax_1, double rax_2 = _defaultValue, double rax_3 = _defaultValue, double rax_4 = _defaultValue, double rax_5 = _defaultValue, double rax_6 = _defaultValue)
        {
            _referenceType = ReferenceType.CONST;
            _name = string.Empty;

            _val1 = rax_1;
            _val2 = rax_2;
            _val3 = rax_3;
            _val4 = rax_4;
            _val5 = rax_5;
            _val6 = rax_6;
        }

        /// <summary>
        /// Initializes a new instance of the Robot Joint Position class with an empty name from a list with values 
        /// </summary>
        /// <param name="internalAxisValues"> The user defined internal axis values as a list.</param>
        public RobotJointPosition(List<double> internalAxisValues)
        {
            _referenceType = ReferenceType.CONST;
            _name = string.Empty;

            double[] values = CheckAxisValues(internalAxisValues.ToArray());

            _val1 = values[0];
            _val2 = values[1];
            _val3 = values[2];
            _val4 = values[3];
            _val5 = values[4];
            _val6 = values[5];
        }

        /// <summary>
        /// Initializes a new instance of the Robot Joint Position class with an empty name from an array. 
        /// </summary>
        /// <param name="internalAxisValues">The user defined internal axis values as an array.</param>
        public RobotJointPosition(double[] internalAxisValues)
        {
            _referenceType = ReferenceType.CONST;
            _name = string.Empty;

            double[] values = CheckAxisValues(internalAxisValues);

            _val1 = values[0];
            _val2 = values[1];
            _val3 = values[2];
            _val4 = values[3];
            _val5 = values[4];
            _val6 = values[5];
        }

        /// <summary>
        /// Initializes a new instance of the Robot Joint Position class with all axis values set to zero.
        /// </summary>
        /// <param name="name"> The robot joint position name, must be unique. </param>
        public RobotJointPosition(string name)
        {
            _referenceType = ReferenceType.CONST;
            _name = name;

            _val1 = _defaultValue;
            _val2 = _defaultValue;
            _val3 = _defaultValue;
            _val4 = _defaultValue;
            _val5 = _defaultValue;
            _val6 = _defaultValue;
        }

        /// <summary>
        /// Initializes a new instance of the Robot Joint Position class.
        /// </summary>
        /// <param name="name"> The robot joint position name, must be unique. </param>
        /// <param name="rax_1"> The position of robot axis 1 in degrees from the calibration position. </param>
        /// <param name="rax_2"> The position of robot axis 2 in degrees from the calibration position.</param>
        /// <param name="rax_3"> The position of robot axis 3 in degrees from the calibration position.</param>
        /// <param name="rax_4"> The position of robot axis 4 in degrees from the calibration position.</param>
        /// <param name="rax_5"> The position of robot axis 5 in degrees from the calibration position.</param>
        /// <param name="rax_6"> The position of robot axis 6 in degrees from the calibration position.</param>
        public RobotJointPosition(string name, double rax_1, double rax_2 = _defaultValue, double rax_3 = _defaultValue, double rax_4 = _defaultValue, double rax_5 = _defaultValue, double rax_6 = _defaultValue)
        {
            _referenceType = ReferenceType.CONST;
            _name = name;

            _val1 = rax_1;
            _val2 = rax_2;
            _val3 = rax_3;
            _val4 = rax_4;
            _val5 = rax_5;
            _val6 = rax_6;
        }

        /// <summary>
        /// Initializes a new instance of the Robot Joint Position class from a list with values.
        /// </summary>
        /// <param name="name"> The robot joint position name, must be unique. </param>
        /// <param name="internalAxisValues"> The user defined internal axis values as a list.</param>
        public RobotJointPosition(string name, List<double> internalAxisValues)
        {
            _referenceType = ReferenceType.CONST;
            _name = name;

            double[] values = CheckAxisValues(internalAxisValues.ToArray());

            _val1 = values[0];
            _val2 = values[1];
            _val3 = values[2];
            _val4 = values[3];
            _val5 = values[4];
            _val6 = values[5];
        }

        /// <summary>
        /// Initializes a new instance of the Robot Joint Position class from an array.
        /// </summary>
        /// <param name="name"> The robot joint position name, must be unique. </param>
        /// <param name="internalAxisValues">The user defined internal axis values as an array.</param>
        public RobotJointPosition(string name, double[] internalAxisValues)
        {
            _referenceType = ReferenceType.CONST;
            _name = name;

            double[] values = CheckAxisValues(internalAxisValues);

            _val1 = values[0];
            _val2 = values[1];
            _val3 = values[2];
            _val4 = values[3];
            _val5 = values[4];
            _val6 = values[5];
        }

        /// <summary>
        /// Initializes a new instance of the Robot Joint Position class by duplicating an existing Robot Joint Position instance. 
        /// </summary>
        /// <param name="robotJointPosition"> The Robot Joint Position instance to duplicate. </param>
        public RobotJointPosition(RobotJointPosition robotJointPosition)
        {
            _referenceType = robotJointPosition.ReferenceType;
            _name = robotJointPosition.Name;
            _val1 = robotJointPosition[0];
            _val2 = robotJointPosition[1];
            _val3 = robotJointPosition[2];
            _val4 = robotJointPosition[3];
            _val5 = robotJointPosition[4];
            _val6 = robotJointPosition[5];
        }

        /// <summary>
        /// Returns an exact duplicate of this Robot Joint Position instance.
        /// </summary>
        /// <returns> A deep copy of the Robot Joint Position instance. </returns>
        public RobotJointPosition Duplicate()
        {
            return new RobotJointPosition(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Robot Joint Position instance as an Action. 
        /// </summary>
        /// <returns> A deep copy of the Robot Joint Position instance as an Action. </returns>
        public override Action DuplicateAction()
        {
            return new RobotJointPosition(this) as Action;
        }

        /// <summary>
        /// Returns an exact duplicate of this Robot Joint Position instance as an IDeclaration.
        /// </summary>
        /// <returns> A deep copy of the Robot Joint Position instance as an IDeclaration. </returns>
        public IDeclaration DuplicateDeclaration()
        {
            return new RobotJointPosition(this) as IDeclaration;
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
                string result = "Robot Joint Position (";

                result += _val1.ToString("0.##") + ", ";
                result += _val2.ToString("0.##") + ", ";
                result += _val3.ToString("0.##") + ", ";
                result += _val4.ToString("0.##") + ", ";
                result += _val5.ToString("0.##") + ", ";
                result += _val6.ToString("0.##") + ")";

                return result;
            }
        }

        /// <summary>
        /// Returns the Robot Joint Position as an array with axis values.
        /// </summary>
        /// <returns> The array containing the axis values of the robot joint position. </returns>
        public double[] ToArray()
        {
            return new double[] { _val1, _val2, _val3, _val4, _val5, _val6 };
        }

        /// <summary>
        /// Returns the Robot Joint Position as an array with axis values.
        /// </summary>
        /// <returns> The list containing the axis values of the robot joint position. </returns>
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
        /// Adds a constant number to all the values inside this Joint Position.
        /// </summary>
        /// <param name="value"> The number to be added. </param>
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
        /// <param name="jointPosition"> The Robot Joint Position to be added. </param>
        public void Add(RobotJointPosition jointPosition)
        {
            for (int i = 0; i < 6; i++)
            {
                this[i] += jointPosition[i];
            }
        }

        /// <summary>
        /// Substracts a constant number from the values inside this Joint Position.
        /// </summary>
        /// <param name="value"> The number to be substracted. </param>
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
        /// <param name="jointPosition"> The Robot Joint Position to be substracted. </param>
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
        /// <param name="jointPosition"> The multiplier as a Robot Joint Position. </param>
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
        /// Cecks the array with internal axis values. 
        /// Always returns a list with 6 internal axis values. 
        /// For missing values 0.0 is used. 
        /// <param name="axisValues"> The list with the internal axis values. </param>
        /// <returns> The array with 6 axis values. </returns>
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
        /// Returns the Joint Position in RAPID code format, e.g. "[0, 0, 0, 0, 45, 0]".
        /// </summary>
        /// <returns> The string with axis values. </returns>
        public string ToRAPID()
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
        /// Returns the RAPID declaration code line of the this action.
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> An empty string. </returns>
        public override string ToRAPIDDeclaration(Robot robot)
        {
            if (_name != String.Empty)
            {
                string code = Enum.GetName(typeof(ReferenceType), _referenceType);
                code += " robjoint ";
                code += _name;
                code += " := ";
                code += this.ToRAPID();
                code += ";";

                return code;
            }

            return String.Empty;
        }

        /// <summary>
        /// Returns the RAPID instruction code line of the this action. 
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> An empty string. </returns>
        public override string ToRAPIDInstruction(Robot robot)
        {
            return string.Empty;
        }

        /// <summary>
        /// Creates declarations in the RAPID program module inside the RAPID Generator. 
        /// This method is called inside the RAPID generator.
        /// </summary>
        /// <param name="RAPIDGenerator"> The RAPID Generator. </param>
        public override void ToRAPIDDeclaration(RAPIDGenerator RAPIDGenerator)
        {
            if (_name != String.Empty)
            {
                if (!RAPIDGenerator.JointPositions.ContainsKey(_name))
                {
                    RAPIDGenerator.JointPositions.Add(_name, this);
                    RAPIDGenerator.ProgramDeclarations.Add("    " + this.ToRAPIDDeclaration(RAPIDGenerator.Robot));
                }
            }
        }

        /// <summary>
        /// Creates instructions in the RAPID program module inside the RAPID Generator.
        /// This method is called inside the RAPID generator.
        /// </summary>
        /// <param name="RAPIDGenerator"> The RAPID Generator. </param>
        public override void ToRAPIDInstruction(RAPIDGenerator RAPIDGenerator)
        {
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets a value indicating whether or not the object is valid.
        /// </summary>
        public override bool IsValid
        {
            get { return true; }
        }

        /// <summary>
        /// Gets or sets the Robot Joint Position variable name.
        /// Each Robot Joint Position variable name has to be unique.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets or sets the Reference Type. 
        /// </summary>
        public ReferenceType ReferenceType
        {
            get { return _referenceType; }
            set { _referenceType = value; }
        }

        /// <summary>
        /// Gets the number of elements in the Robot Joint Position.
        /// </summary>
        public int Length
        {
            get { return 6; }
        }

        /// <summary>
        /// Gets or sets the axis values through the indexer. 
        /// </summary>
        /// <param name="index"> The index number. </param>
        /// <returns> The axis value located at the given index. </returns>
        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return _val1;
                    case 1: return _val2;
                    case 2: return _val3;
                    case 3: return _val4;
                    case 4: return _val5;
                    case 5: return _val6;
                    default: throw new IndexOutOfRangeException();
                }
            }

            set
            {
                switch (index)
                {
                    case 0: _val1 = value; break;
                    case 1: _val2 = value; break;
                    case 2: _val3 = value; break;
                    case 3: _val4 = value; break;
                    case 4: _val5 = value; break;
                    case 5: _val6 = value; break;
                    default: throw new IndexOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the joint position array has a fixed size.
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
        /// <returns> The Robot Joint Position with multiplied values. </returns>
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
        /// Adds a Robot Joint Position to an other Robot Joint Position.
        /// </summary>
        /// <param name="p1"> The first Robot Joint Position. </param>
        /// <param name="p2"> The second Robot Joint Position. </param>
        /// <returns> The addition of the two Robot Joint Poistion</returns>
        public static RobotJointPosition operator +(RobotJointPosition p1, RobotJointPosition p2)
        {
            return new RobotJointPosition(p1[0] + p2[0], p1[1] + p2[1], p1[2] + p2[2], p1[3] + p2[3], p1[4] + p2[4], p1[5] + p2[5]);
        }

        /// <summary>
        /// Substracts a Robot Joint Position from an other Robot Joint Position.
        /// </summary>
        /// <param name="p1"> The first Robot Joint Position. </param>
        /// <param name="p2"> The second Robot Joint Position. </param>
        /// <returns> The first robot Joint Position minus the second Robot Joint Position. </returns>
        public static RobotJointPosition operator -(RobotJointPosition p1, RobotJointPosition p2)
        {
            return new RobotJointPosition(p1[0] - p2[0], p1[1] - p2[1], p1[2] - p2[2], p1[3] - p2[3], p1[4] - p2[4], p1[5] - p2[5]);
        }

        /// <summary>
        /// Multiplies a Robot Joint Position woth an other Robot Joint Position.
        /// </summary>
        /// <param name="p1"> The first Robot Joint Position. </param>
        /// <param name="p2"> The second Robot Joint Position. </param>
        /// <returns> The multiplicaton of the two Robot Joint Positions. </returns>
        public static RobotJointPosition operator *(RobotJointPosition p1, RobotJointPosition p2)
        {
            return new RobotJointPosition(p1[0] * p2[0], p1[1] * p2[1], p1[2] * p2[2], p1[3] * p2[3], p1[4] * p2[4], p1[5] * p2[5]);
        }

        /// <summary>
        /// Divides a Robot Joint Position with an other Robot Joint Position.
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

            return new RobotJointPosition(p1[0] / p2[0], p1[1] / p2[1], p1[2] / p2[2], p1[3] / p2[3], p1[4] / p2[4], p1[5] / p2[5]);
        }
        #endregion
    }

}

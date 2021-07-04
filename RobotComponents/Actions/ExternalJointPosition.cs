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
    /// Represents an External Joint Position declaration.
    /// This action is used to defined define the axis positions of external axes, positioners and workpiece manipulators. 
    /// </summary>
    [Serializable()]
    public class ExternalJointPosition : Action, IDeclaration, IJointPosition, ISerializable
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

        private const double _defaultValue = 9e9;
        #endregion

        #region (de)serialization
        /// <summary>
        /// Protected constructor needed for deserialization of the object.  
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization.</param>
        protected ExternalJointPosition(SerializationInfo info, StreamingContext context)
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

            _val1 = (double)info.GetValue("Value 1", typeof(double));
            _val2 = (double)info.GetValue("Value 2", typeof(double));
            _val3 = (double)info.GetValue("Value 3", typeof(double));
            _val4 = (double)info.GetValue("Value 4", typeof(double));
            _val5 = (double)info.GetValue("Value 5", typeof(double));
            _val6 = (double)info.GetValue("Value 6", typeof(double));
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
            info.AddValue("Value 1", _val1, typeof(double));
            info.AddValue("Value 2", _val2, typeof(double));
            info.AddValue("Value 3", _val3, typeof(double));
            info.AddValue("Value 4", _val4, typeof(double));
            info.AddValue("Value 5", _val5, typeof(double));
            info.AddValue("Value 6", _val6, typeof(double));
        }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes a new instance of the External Joint Position class with an empty name and undefinied positions of the external logical axes.
        /// </summary>
        public ExternalJointPosition()
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
        /// Initializes a new instance of the External Joint Position class with an empty name.
        /// </summary>
        /// <param name="Eax_a"> The position of the external logical axis “a” expressed in degrees or mm. </param>
        /// <param name="Eax_b"> The position of the external logical axis “b” expressed in degrees or mm. </param>
        /// <param name="Eax_c"> The position of the external logical axis “c” expressed in degrees or mm. </param>
        /// <param name="Eax_d"> The position of the external logical axis “d” expressed in degrees or mm. </param>
        /// <param name="Eax_e"> The position of the external logical axis “3” expressed in degrees or mm. </param>
        /// <param name="Eax_f"> The position of the external logical axis “f” expressed in degrees or mm. </param>
        public ExternalJointPosition(double Eax_a, double Eax_b = _defaultValue, double Eax_c = _defaultValue, double Eax_d = _defaultValue, double Eax_e = _defaultValue, double Eax_f = _defaultValue)
        {
            _referenceType = ReferenceType.CONST;
            _name = string.Empty;

            _val1 = Eax_a;
            _val2 = Eax_b;
            _val3 = Eax_c;
            _val4 = Eax_d;
            _val5 = Eax_e;
            _val6 = Eax_f;
        }

        /// <summary>
        /// Initializes a new instance of the External Joint Position class with an empty name from a list with values.
        /// </summary>
        /// <param name="externalAxisValues"> The position of the external logical axes. </param>
        public ExternalJointPosition(List<double> externalAxisValues)
        {
            _referenceType = ReferenceType.CONST;
            _name = string.Empty;

            double[] values = CheckAxisValues(externalAxisValues.ToArray());

            _val1 = values[0];
            _val2 = values[1];
            _val3 = values[2];
            _val4 = values[3];
            _val5 = values[4];
            _val6 = values[5];
        }

        /// <summary>
        /// Initializes a new instance of the External Joint Position class with an empty name from an array with values.
        /// </summary>
        /// <param name="externalAxisValues"> The position of the external logical axes. </param>
        public ExternalJointPosition(double[] externalAxisValues)
        {
            _referenceType = ReferenceType.CONST;
            _name = string.Empty;

            double[] values = CheckAxisValues(externalAxisValues);

            _val1 = values[0];
            _val2 = values[1];
            _val3 = values[2];
            _val4 = values[3];
            _val5 = values[4];
            _val6 = values[5];
        }

        /// <summary>
        /// Initializes a new instance of the External Joint Position class with undefinied positions of the external logical axes.
        /// </summary>
        /// <param name="name"> The external joint position name, must be unique. </param>
        public ExternalJointPosition(string name)
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
        /// Initializes a new instance of the External Joint Position class.
        /// </summary>
        /// <param name="name"> The external joint position name, must be unique. </param>
        /// <param name="Eax_a"> The position of the external logical axis “a” expressed in degrees or mm. </param>
        /// <param name="Eax_b"> The position of the external logical axis “b” expressed in degrees or mm. </param>
        /// <param name="Eax_c"> The position of the external logical axis “c” expressed in degrees or mm. </param>
        /// <param name="Eax_d"> The position of the external logical axis “d” expressed in degrees or mm. </param>
        /// <param name="Eax_e"> The position of the external logical axis “3” expressed in degrees or mm. </param>
        /// <param name="Eax_f"> The position of the external logical axis “f” expressed in degrees or mm. </param>
        public ExternalJointPosition(string name, double Eax_a, double Eax_b = _defaultValue, double Eax_c = _defaultValue, double Eax_d = _defaultValue, double Eax_e = _defaultValue, double Eax_f = _defaultValue)
        {
            _referenceType = ReferenceType.CONST;
            _name = name;

            _val1 = Eax_a;
            _val2 = Eax_b;
            _val3 = Eax_c;
            _val4 = Eax_d;
            _val5 = Eax_e;
            _val6 = Eax_f;
        }

        /// <summary>
        /// Initializes a new instance of the External Joint Position class from a list with values.
        /// </summary>
        /// <param name="name"> The external joint position name, must be unique. </param>
        /// <param name="externalAxisValues"> The position of the external logical axes. </param>
        public ExternalJointPosition(string name, List<double> externalAxisValues)
        {
            _referenceType = ReferenceType.CONST;
            _name = name;

            double[] values = CheckAxisValues(externalAxisValues.ToArray());

            _val1 = values[0];
            _val2 = values[1];
            _val3 = values[2];
            _val4 = values[3];
            _val5 = values[4];
            _val6 = values[5];
        }

        /// <summary>
        /// Initializes a new instance of the External Joint Position class from an array with values.
        /// </summary>
        /// <param name="name"> The external joint position name, must be unique. </param>
        /// <param name="externalAxisValues"> The position of the external logical axes. </param>
        public ExternalJointPosition(string name, double[] externalAxisValues)
        {
            _referenceType = ReferenceType.CONST;
            _name = name;

            double[] values = CheckAxisValues(externalAxisValues);

            _val1 = values[0];
            _val2 = values[1];
            _val3 = values[2];
            _val4 = values[3];
            _val5 = values[4];
            _val6 = values[5];
        }

        /// <summary>
        /// Initializes a new instance of the External Joint Position class by duplicating an existing External Joint Position instance. 
        /// </summary>
        /// <param name="externalJointPosition"> The External Joint Position instance to duplicate. </param>
        public ExternalJointPosition(ExternalJointPosition externalJointPosition)
        {
            _referenceType = externalJointPosition.ReferenceType;
            _name = externalJointPosition.Name;
            _val1 = externalJointPosition[0];
            _val2 = externalJointPosition[1];
            _val3 = externalJointPosition[2];
            _val4 = externalJointPosition[3];
            _val5 = externalJointPosition[4];
            _val6 = externalJointPosition[5];
        }

        /// <summary>
        /// Returns an exact duplicate of this Digital Output instance.
        /// </summary>
        /// <returns> A deep copy of the Digital Output instance. </returns>
        public ExternalJointPosition Duplicate()
        {
            return new ExternalJointPosition(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this External Joint Position instance as an Action. 
        /// </summary>
        /// <returns> A deep copy of the External Joint Position instance as an Action. </returns>
        public override Action DuplicateAction()
        {
            return new ExternalJointPosition(this) as Action;
        }

        /// <summary>
        /// Returns an exact duplicate of this External Joint Position instance as an IDeclaration.
        /// </summary>
        /// <returns> A deep copy of the External Joint Position instance as an IDeclaration. </returns>
        public IDeclaration DuplicateDeclaration()
        {
            return new ExternalJointPosition(this) as IDeclaration;
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
        /// Returns the External Joint Position as an array with the positions of the external logical axes.
        /// </summary>
        /// <returns> An array containing the positions of the external logical axes. </returns>
        public double[] ToArray()
        {
            return new double[] { _val1, _val2, _val3, _val4, _val5, _val6 };
        }

        /// <summary>
        /// Returns the External Joint Position as a list with the positions of the external logical axes.
        /// </summary>
        /// <returns> A list containing the positions of the external logical axes. </returns>
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
        /// Adds a constant number to all the values inside this Joint Position.
        /// </summary>
        /// <param name="value"> The number to be added. </param>
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
        /// <param name="jointPosition"> The External Joint Position to be added. </param>
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
                    throw new InvalidOperationException(String.Format("Mismatch between two External Joint Positions. A definied joint position [on index {0}] is combined with an undefinied joint position.", i));
                }
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
        /// <param name="jointPosition"> The External Joint Position to be substracted. </param>
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
                    throw new InvalidOperationException(String.Format("Mismatch between two External Joint Positions. A definied joint position [on index {0}] is combined with an undefinied joint position.", i));
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
                    throw new InvalidOperationException(String.Format("Mismatch between two External Joint Positions. A definied joint position [on index {0}] is combined with an undefinied joint position.", i));
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
                    throw new InvalidOperationException(String.Format("Mismatch between two External Joint Positions. A definied joint position [on index {0}] is combined with an undefinied joint position.", i));
                }
            }
        }

        /// <summary></summary>
        /// Checks the array the positions of the external logical axes. 
        /// Always returns an array with 6 joint positions. 
        /// For missing values 9E9 (not connected) will be used. 
        /// <param name="axisValues"> The array with the positions of the external logical axes. </param>
        /// <returns> The array with the 6 positions of the external axes. </returns>
        private double[] CheckAxisValues(double[] axisValues)
        {
            double[] result = new double[6];
            int n = Math.Min(axisValues.Length, 6);

            // Copy definied joint positions
            for (int i = 0; i < n; i++)
            {
                result[i] = axisValues[i];
            }

            // Add missing joint positions
            for (int i = n; i < 6; i++)
            {
                result[i] = _defaultValue;
            }

            return result;
        }

        /// <summary>
        /// Returns the Joint Position in RAPID code format, e.g. "[100, 9E9, 9E9, 9E9, 9E9, 9E9]".
        /// </summary>
        /// <returns> The string with the positions of the external axes. </returns>
        public string ToRAPID()
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
        /// Returns the RAPID declaration code line of the this action.
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> An empty string. </returns>
        public override string ToRAPIDDeclaration(Robot robot)
        {
            if (_name != String.Empty)
            {
                string code = Enum.GetName(typeof(ReferenceType), _referenceType);
                code += " extjoint ";
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
                    RAPIDGenerator.StringBuilder.Append(Environment.NewLine + "\t" + this.ToRAPIDDeclaration(RAPIDGenerator.Robot));
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
        /// Gets or sets the External Joint Position variable name.
        /// Each External Joint Position variable name has to be unique.
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
        /// Gets the number of elements in the External Joint Position.
        /// </summary>
        public int Length
        {
            get { return 6; }
        }

        /// <summary>
        /// Gets or sets the position of the external logical axis through the indexer. 
        /// </summary>
        /// <param name="index"> The index number. </param>
        /// <returns> The position of the external logical axis located at the given index. </returns>
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
        /// Gets or sets the position of the external logical axis through the indexer. 
        /// </summary>
        /// <param name="index"> The index character. </param>
        /// <returns> The position of the external logical axis located at the given index. </returns>
        public double this[char index]
        {
            get
            {
                switch (index)
                {
                    case 'a': return _val1;
                    case 'b': return _val2;
                    case 'c': return _val3;
                    case 'd': return _val4;
                    case 'e': return _val5;
                    case 'f': return _val6;

                    case 'A': return _val1;
                    case 'B': return _val2;
                    case 'C': return _val3;
                    case 'D': return _val4;
                    case 'E': return _val5;
                    case 'F': return _val6;

                    default: throw new IndexOutOfRangeException();
                }
            }

            set
            {
                switch (index)
                {
                    case 'a': _val1 = value; break;
                    case 'b': _val2 = value; break;
                    case 'c': _val3 = value; break;
                    case 'd': _val4 = value; break;
                    case 'e': _val5 = value; break;
                    case 'f': _val6 = value; break;

                    case 'A': _val1 = value; break;
                    case 'B': _val2 = value; break;
                    case 'C': _val3 = value; break;
                    case 'D': _val4 = value; break;
                    case 'E': _val5 = value; break;
                    case 'F': _val6 = value; break;

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
        /// Adds an External Joint Position with an other External Joint Position.
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
                    throw new InvalidOperationException(String.Format("Mismatch between two External Joint Positions. A definied joint position [on index {0}] is combined with an undefinied joint position.", i));
                }
            }

            return result;
        }

        /// <summary>
        /// Substracts an External Joint Position from an other External Joint Position.
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
                    throw new InvalidOperationException(String.Format("Mismatch between two External Joint Positions. A definied joint position [on index {0}] is combined with an undefinied joint position.", i));
                }
            }

            return result;
        }

        /// <summary>
        /// Multiplies an External Joint Position with an other External Joint Position.
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
                    throw new InvalidOperationException(String.Format("Mismatch between two External Joint Positions. A definied joint position [on index {0}] is combined with an undefinied joint position.", i));
                }
            }

            return result;
        }

        /// <summary>
        /// Divides an External Joint Positin with by an other External Joint Position
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
                    throw new InvalidOperationException(String.Format("Mismatch between two External Joint Positions. A definied joint position [on index {0}] is combined with an undefinied joint position.", i));
                }
            }

            return result;
        }
        #endregion
    }

}

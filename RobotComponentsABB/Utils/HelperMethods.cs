// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/EDEK-UniKassel/RobotComponents>.

// System Libs
using System.Collections.Generic;

namespace RobotComponentsABB.Utils
{
    /// <summary>
    /// Helper methods and properties
    /// </summary>
    public static class HelperMethods
    {
        #region fields
        /// <summary>
        /// List with the pre-defined precision values that can be used for defining the precision of a robot movement. 
        /// </summary>
        private static readonly int[] _validPrecisionValues = new int[] { -1, 0, 1, 5, 10, 15, 20, 30, 40, 50, 60, 80, 100, 150, 200 };

        /// <summary>
        /// List with the pre-defined tcp speed values that can be used for defining the speed of a robot movement. 
        /// </summary>
        private static readonly double[] _validPredefiniedSpeedValues = new double[] { 5, 10, 20, 30, 40, 50, 60, 80, 100, 150, 200, 300,
            400, 500, 600, 800, 1000, 1500, 2000, 2500, 3000, 4000, 5000, 6000, 7000 };
        #endregion

        #region methods
        /// <summary>
        /// Method to check if a string variable is longer then 32 characters. 
        /// </summary>
        /// <param name="variable"> The string to check for its length. </param>
        /// <returns> Returns true if the string longer then 32 characters. Otherwise the return value is false. </returns>
        public static bool VariableExeedsCharacterLimit32(string variable)
        {
            int length = variable.Length;

            if (length < 32)
            {
                return false;
            }
            else return true;
        }

        /// <summary>
        /// A method to check if a string starts with a digit. 
        /// </summary>
        /// <param name="variable"> The string variable to check for. </param>
        /// <returns> Returns a boolean that defines if the string starts with a digit. </returns>
        public static bool VariableStartsWithNumber(string variable)
        {
            bool isDigit = char.IsNumber(variable[0]); ;
            return isDigit;
        }

        /// <summary>
        /// A method to check if the precision value is valid to use a pre-defined precision value (zonedata). 
        /// </summary>
        /// <param name="value"> The precision value (double) to check for. </param>
        /// <returns> Returns if the precision value (zonedata) is valid. </returns>
        public static bool PrecisionValueIsValid(int value)
        {
            bool isValid = false;

            for (int i = 0; i < _validPrecisionValues.Length; i++)
            {
                if (value == _validPrecisionValues[i])
                {
                    isValid = true;
                    break;
                }
            }
            return isValid;
        }

        /// <summary>
        /// A method to check if the speeddata value is valid to use a pre-defined speeddata. 
        /// </summary>
        /// <param name="value"> The speeddata value (double) to check for. </param>
        /// <returns> Returns if the speeddata value is valid. </returns>
        public static bool PredefinedSpeedValueIsValid(double value)
        {
            bool isValid = false;

            for (int i = 0; i < _validPredefiniedSpeedValues.Length; i++)
            {
                if (value == _validPredefiniedSpeedValues[i])
                {
                    isValid = true;
                    break;
                }
            }
            return isValid;
        }

        /// <summary>
        /// This method gives the biggest value from an array or a list with doubles. 
        /// </summary>
        /// <param name="values"> The list with doubles. </param>
        /// <returns> Returns the biggest double from the list. </returns>
        public static double GetBiggestValue(List<double> values)
        {
            double value = 0;

            for (int i = 0; i < values.Count; i++)
            {
                if (values[i] > value)
                {
                    value = values[i];
                }
            }

            return value;
        }

        /// <summary>
        /// This method gives the biggest value from an array or a list with doubles. 
        /// </summary>
        /// <param name="values"> The list with doubles. </param>
        /// <returns> Returns the biggest double from the list. </returns>
        public static double GetBiggestValue(double[] values)
        {
            double value = 0;

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] > value)
                {
                    value = values[i];
                }
            }

            return value;
        }

        /// <summary>
        /// This method gives the biggest value from an array or a list with integers. 
        /// </summary>
        /// <param name="values"> The list with integers. </param>
        /// <returns> Returns the biggest integer from the list. </returns>
        public static int GetBiggestValue(List<int> values)
        {
            int value = 0;

            for (int i = 0; i < values.Count; i++)
            {
                if (values[i] > value)
                {
                    value = values[i];
                }
            }

            return value;
        }

        /// <summary>
        /// This method gives the biggest value from an array or a list with integers. 
        /// </summary>
        /// <param name="values"> The array with integers. </param>
        /// <returns> Returns the biggest integer from the array. </returns>
        public static int GetBiggestValue(int[] values)
        {
            int value = 0;

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] > value)
                {
                    value = values[i];
                }
            }

            return value;
        }
        #endregion

        #region properties
        /// <summary>
        /// List with the pre-defined precision values that can be used for defining the precision of a robot movement. 
        /// </summary>
        public static int[] ValidPrecisionValues
        {
            get { return _validPrecisionValues; }
        }

        /// <summary>
        /// List with the pre-defined tcp speed values that can be used for defining the speed of a robot movement. 
        /// </summary>
        public static double[] ValidPredefiniedSpeedValues
        {
            get { return _validPredefiniedSpeedValues; }
        }
        #endregion
    }
}

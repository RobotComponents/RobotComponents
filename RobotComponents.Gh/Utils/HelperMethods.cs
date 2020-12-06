// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System.Collections.Generic;
// Grasshopper Libs
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

namespace RobotComponents.Gh.Utils
{
    /// <summary>
    /// Helper methods and properties
    /// </summary>
    public static class HelperMethods
    {
        #region fields
        /// <summary>
        /// OBSOLETE: Is only uaed in obsolete components. Use the ZoneData base class to acces this data. 
        /// List with the pre-defined precision values that can be used for defining the precision of a robot movement. 
        /// </summary>
        private static readonly int[] _validPrecisionValues = new int[] { -1, 0, 1, 5, 10, 15, 20, 30, 40, 50, 60, 80, 100, 150, 200 };
        #endregion

        #region methods
        /// <summary>
        /// Creates a datatree with the same structure as the one use for the input.
        /// It data tree with uniques names based on the given name and the path of the item. 
        /// </summary>
        /// <param name="name"> The name that should be used. </param>
        /// <param name="data"> The tree structure. </param>
        /// <returns> The datatree filled with unique names. </returns>
        public static GH_Structure<GH_String> DataTreeNaming(string name, GH_Structure<IGH_Goo> data)
        {
            // Output
            GH_Structure<GH_String> names = new GH_Structure<GH_String>();

            // Paths
            var paths = data.Paths;

            // Make the output datatree with names
            for (int i = 0; i < data.Branches.Count; i++)
            {
                var branches = data.Branches[i];
                GH_Path iPath = paths[i];
                string pathString = iPath.ToString();
                string newPath = pathString.Replace("{", "").Replace(";", "_").Replace("}", "");

                for (int j = 0; j < branches.Count; j++)
                {
                    string myName = name + "_" + newPath + "_" + j;
                    GH_String converted = new GH_String(myName);
                    names.Append(converted, iPath);
                }
            }

            return names;
        }

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
        /// OBSOLETE: Is only used in obsolete components. Use the ZoneData base class to acces predefined values.
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

        /// <summary>
        /// Replaces the spaces with an underscore and removes new lines in a list with strings. 
        /// Typically used to edit variable names of declarations.
        /// </summary>
        /// <param name="strings"> The list with strings. </param>
        /// <returns> The list with strings. </returns>
        public static List<string> ReplaceSpacesAndRemoveNewLines(List<string> strings)
        {
            List<string> result = new List<string>() { };

            for (int i = 0; i < strings.Count; i++)
            {
                result.Add(ReplaceSpacesAndRemoveNewLines(strings[i]));
            }

            return result;
        }

        /// <summary>
        /// Replaces the spaces with an underscore and removes new lines in a string. 
        /// Typically used to edit variable names of declarations.
        /// </summary>
        /// <param name="text"> The string. </param>
        /// <returns> The new string. </returns>
        public static string ReplaceSpacesAndRemoveNewLines(string text)
        {
            string result = text.Replace(" ", "_");
            result = result.Replace("\n", "");
            result = result.Replace("\r", "");

            return result;
        }

        /// <summary>
        /// Compares are string with an other string and checks if these are equal. 
        /// </summary>
        /// <param name="text1"> The first string to compare with. </param>
        /// <param name="text2"> The second string to compare with. </param>
        /// <returns> Specifies wethers the two strings are equal </returns>
        public static bool Equality(this string text1, string text2)
        {
            char[] chars1 = text1.ToCharArray();
            char[] chars2 = text2.ToCharArray();

            if (chars1.Length != chars2.Length)
            {
                return false;
            }

            for (int i = 0; i < chars1.Length; i++)
            {
                if (chars1[i] != chars2[i])
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region properties
        #endregion
    }
}

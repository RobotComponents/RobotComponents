using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotComponents.Utils
{
    public static class HelperMethods
    {
        private static int[] _validPrecisionValues = new int[]{-1 ,0, 1, 5, 10, 15, 20, 30, 40, 50, 60, 80, 100, 150, 200};

        private static double[] _validPredefiniedSpeedValues = new double[] { 5, 10, 20, 30, 40, 50, 60, 80, 100, 150, 200, 300,
            400, 500, 600, 800, 1000, 1500, 2000, 2500, 3000, 4000, 5000, 6000, 7000 };

        public static bool VariableExeedsCharacterLimit32(string variable)
        {
            int length = variable.Length;

            if (length < 32)
            {
                return false;
            }
            else return true;
        }

        public static bool VariableStartsWithNumber(string variable)
        {
            bool isDigit = char.IsNumber(variable[0]); ;
            return isDigit;
        }

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

        public static double GetBiggestValue(List<double> values)
        {
            double value = 0;

            for (int i = 0; i < values.Count; i++)
            {
                if(values[i] > value)
                {
                    value = values[i];
                }
            }

            return value;
        }

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
    }
}

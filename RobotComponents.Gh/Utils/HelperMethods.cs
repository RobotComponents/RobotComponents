// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Drawing;
// Grasshopper Libs
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Special;

namespace RobotComponents.Gh.Utils
{
    /// <summary>
    /// Helper methods and properties
    /// </summary>
    public static class HelperMethods
    {
        #region fields
        #endregion

        #region methods
        /// <summary>
        /// Creates a datatree with the same structure as the one use for the input.
        /// It data tree with uniques names based on the given name and the path of the item. 
        /// </summary>
        /// <param name="name"> The name that should be used. </param>
        /// <param name="data"> The tree structure. </param>
        /// <returns> The datatree filled with unique names. </returns>
        [Obsolete("This method is OBSOLETE and will be removed in the future.", false)]
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
        /// Method to check if a string is longer then 32 characters. 
        /// </summary>
        /// <param name="variable"> The string to check for its length. </param>
        /// <returns> Returns true if the string longer then 32 characters. Otherwise the return value is false. </returns>
        public static bool StringExeedsCharacterLimit32(string variable)
        {
            int length = variable.Length;

            if (length < 32)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// A method to check if a string starts with a digit. 
        /// </summary>
        /// <param name="variable"> The string to check for. </param>
        /// <returns> Returns a boolean that defines if the string starts with a digit. </returns>
        public static bool StringStartsWithNumber(string variable)
        {
            if (variable.Length > 0)
            {
                bool isDigit = char.IsNumber(variable[0]); ;
                return isDigit;
            }

            return false;
        }

        /// <summary>
        /// A method to check if a string contains special characters.
        /// </summary>
        /// <param name="variable"> The string to check for. </param>
        /// <returns> Returns a boolean that defines if the string has special characters. </returns>
        public static bool StringHasSpecialCharacters(string variable)
        {
            if (variable.Length > 0)
            {
                Regex rgx = new Regex("[^A-Za-z0-9:_]");
                return rgx.IsMatch(variable);
            }

            return false;
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

        /// <summary>
        /// Creates the panel with the signal name and connects it to the input parameter
        /// </summary>
        /// <param name="text"> Text to place inside the panel. </param>
        /// <param name="component"> Component to connect to. </param>
        /// <param name="text"> Text to place inside the panel. </param>
        /// <param name="index"> Index of the input to connect the panel to. </param>
        /// <returns> True, if created. </returns>
        public static bool CreatePanel(GH_Component component, string text, int index)
        {
            try
            {
                // Parameter
                var parameter = component.Params.Input[index];

                // Create the  panel
                GH_Panel panel = new GH_Panel();
                panel.SetUserText(text);
                panel.Properties.Colour = Color.White;

                // Add the panel to the active canvas
                Instances.ActiveCanvas.Document.AddObject(panel, false);
                panel.Attributes.ExpireLayout();
                panel.Attributes.PerformLayout();

                // Set the location and size of the panel
                panel.Attributes.Bounds = new RectangleF(0.0f, 0.0f, 160.0f, 20.0f);
                float x = component.Attributes.DocObject.Attributes.Bounds.Left - panel.Attributes.Bounds.Width - 30;
                float y = parameter.Attributes.Bounds.Y;
                panel.Attributes.Pivot = new System.Drawing.PointF(x, y);

                // Connect the panel to the input parameter
                parameter.RemoveAllSources();
                parameter.AddSource(panel);

                // Return that it's created
                return true;
            }
            catch
            {
                // Return that it isn't created
                return false;
            }
        }

        /// <summary>
        /// Creates a Grasshopper value list from a list with strings and connects it to an input parameter.  
        /// Returns true if it's created.
        /// </summary>
        /// <param name="component"> Component to connect to. </param>
        /// <param name="names"> List with strings. </param>
        /// <param name="index"> Index of the input to connect the value list to. </param>
        /// <returns> True, if created. </returns>
        public static bool CreateValueList(GH_Component component, List<string> names, int index)
        {
            try
            {
                var parameter = component.Params.Input[index];

                // Create the value list
                GH_ValueList obj = CreateValueList(names);

                // Add the value list to the active canvas
                Instances.ActiveCanvas.Document.AddObject(obj, false);
                obj.Attributes.ExpireLayout();
                obj.Attributes.PerformLayout();

                // Make point where the valuelist should be created on the canvas                
                float dx = obj.Attributes.OutputGrip.X - parameter.Attributes.InputGrip.X;
                float dy = obj.Attributes.OutputGrip.Y - parameter.Attributes.InputGrip.Y;
                obj.Attributes.Pivot = new PointF(obj.Attributes.Pivot.X - dx - 25, obj.Attributes.Pivot.Y - dy);
                obj.Attributes.ExpireLayout();

                // Connect the value list to the input parameter
                parameter.AddSource(obj);

                // Collect data
                parameter.CollectData();

                // Expire value list
                obj.ExpireSolution(true);

                //Return that it's created
                return true;
            }
            catch
            {
                //Return that it isn't created
                return false;
            }
        }

        /// <summary>
        /// Return a value list populated with names.
        /// </summary>
        /// <param name="names"> List with strings to take the names from. </param>
        /// <returns> The value list with data. </returns>
        private static GH_ValueList CreateValueList(List<string> names)
        {
            // Creates the empty value list
            GH_ValueList obj = new GH_ValueList();
            obj.CreateAttributes();
            obj.ListMode = GH_ValueListMode.DropDown;
            obj.ListItems.Clear();

            names.Sort();

            for (int i = 0; i < names.Count; i++)
            {
                obj.ListItems.Add(new GH_ValueListItem(names[i], string.Format("\"{0}\"", names[i])));
            }

            return obj;
        }

        /// <summary>
        /// Creates a Grasshopper value list from an enum and connects it to an input parameter.  
        /// Returns true if it's created.
        /// </summary>
        /// <param name="component"> Component to connect to. </param>
        /// <param name="enumType"> Enumeration to take values from. </param>
        /// <param name="index"> Index of the input to connect the value list to. </param>
        /// <returns> True, if created. </returns>
        public static bool CreateValueList(GH_Component component, Type enumType, int index)
        {
            try
            {
                var parameter = component.Params.Input[index];

                // Create the value list
                GH_ValueList obj = CreateValueList(enumType);

                // Add the value list to the active canvas
                Instances.ActiveCanvas.Document.AddObject(obj, false);
                obj.Attributes.ExpireLayout();
                obj.Attributes.PerformLayout();

                // Make point where the valuelist should be created on the canvas
                float dx = obj.Attributes.OutputGrip.X - parameter.Attributes.InputGrip.X;
                float dy = obj.Attributes.OutputGrip.Y - parameter.Attributes.InputGrip.Y;
                obj.Attributes.Pivot = new PointF(obj.Attributes.Pivot.X - dx - 25, obj.Attributes.Pivot.Y - dy);
                obj.Attributes.ExpireLayout();

                // Connect the value list to the input parameter
                parameter.AddSource(obj);

                // Collect data
                parameter.CollectData();

                // Expire value list
                obj.ExpireSolution(true);

                //Return that it's created
                return true; 
            }
            catch
            {
                //Return that it isn't created
                return false;
            }
        }

        /// <summary>
        /// Return a value list populated with data from an enum.
        /// </summary>
        /// <param name="enumType"> Enumeration to take values from. </param>
        /// <returns> The value list with data. </returns>
        private static GH_ValueList CreateValueList(Type enumType)
        {
            // Creates the empty value list
            GH_ValueList obj = new GH_ValueList();
            obj.CreateAttributes();
            obj.ListMode = GH_ValueListMode.DropDown;
            obj.ListItems.Clear();

            // Add the items to the value list
            string[] names = Enum.GetNames(enumType);
            int[] values = (int[])Enum.GetValues(enumType);

            for (int i = 0; i < names.Length; i++)
            {
                obj.ListItems.Add(new GH_ValueListItem(names[i], values[i].ToString()));
            }

            return obj;
        }

        /// <summary>
        /// Places a value list on a given location on the canvas. 
        /// Returns true if it's created.
        /// </summary>
        /// <param name="obj"> Value list to place on the canvas. </param>
        /// <param name="location"> Location on the canvas. </param>
        /// <returns> True, if created. </returns>
        private static bool CreateValueList(GH_ValueList obj, PointF location)
        {
            try
            {
                obj.Attributes.Pivot = new PointF(location.X - obj.Attributes.Bounds.Width / 4, location.Y - obj.Attributes.Bounds.Height / 2);
                Instances.ActiveCanvas.Document.AddObject(obj, false);
                obj.Attributes.ExpireLayout();
                obj.Attributes.PerformLayout();

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a value list populated with data from a dictionary.
        /// </summary>
        /// <param name="data"> Data to populate the valuelist with. </param>
        /// <returns> The value list with data. </returns>
        private static GH_ValueList CreateValueList(Dictionary<string, double> data)
        {
            // Creates the empty value list
            GH_ValueList obj = new GH_ValueList();
            obj.CreateAttributes();
            obj.ListMode = GH_ValueListMode.DropDown;
            obj.ListItems.Clear();

            // Add the items to the value list
            foreach (KeyValuePair<string, double> entry in data)
            {
                obj.ListItems.Add(new GH_ValueListItem(entry.Key, entry.Value.ToString()));
            }

            return obj;
        }

        /// <summary>
        /// Creates a Grasshopper value list from an enum and places it on the given location on the canvas. 
        /// Returns true if it's created.
        /// </summary>
        /// <param name="enumType"> Enumeration to take values from. </param>
        /// <param name="location"> Location on the canvas. </param>
        /// <returns> True, if created. </returns>
        public static bool CreateValueList(Type enumType, PointF location)
        {
            return CreateValueList(CreateValueList(enumType), location);
        }

        /// <summary>
        /// Creates a Grasshopper value list from a dictionary and places it on the given location on the canvas. 
        /// Returns true if it's created.
        /// </summary>
        /// <param name="data"> Data to populate the valuelist with. </param>
        /// <param name="location"> Location on the canvas. </param>
        /// <returns> True, if created. </returns>
        public static bool CreateValueList(Dictionary<string, double> data, PointF location)
        {
            return CreateValueList(CreateValueList(data), location);
        }
        #endregion

        #region properties
        #endregion
    }
}

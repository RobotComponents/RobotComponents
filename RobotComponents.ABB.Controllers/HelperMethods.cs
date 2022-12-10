// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

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

namespace RobotComponents.ABB.Controllers
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
        /// <param name="inputIndex"> Index of the input to connect the value list to. </param>
        /// <returns> True, if created. </returns>
        public static bool CreateValueList(GH_Component component, List<string> names, int inputIndex)
        {
            try
            {
                var parameter = component.Params.Input[inputIndex];

                // Create the value list
                GH_ValueList obj = CreateValueList(names);

                // Make point where the valuelist should be created on the canvas
                float width = obj.Attributes.Bounds.Width;
                obj.Attributes.Pivot = new PointF(parameter.Attributes.InputGrip.X - width - 15, parameter.Attributes.InputGrip.Y - 11);

                // Add the value list to the active canvas
                Instances.ActiveCanvas.Document.AddObject(obj, false);

                // Connect the value list to the input parameter
                parameter.AddSource(obj);

                // Collect data
                parameter.CollectData();

                // Expire value list
                obj.ExpireSolution(true);

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
        /// <param name="inputIndex"> Index of the input to connect the value list to. </param>
        /// <returns> True, if created. </returns>
        public static bool CreateValueList(GH_Component component, Type enumType, int inputIndex)
        {
            try
            {
                var parameter = component.Params.Input[inputIndex];

                // Create the value list
                GH_ValueList obj = CreateValueList(enumType);

                // Make point where the valuelist should be created on the canvas
                float width = obj.Attributes.Bounds.Width;
                obj.Attributes.Pivot = new PointF(parameter.Attributes.InputGrip.X - width - 15, parameter.Attributes.InputGrip.Y - 11);

                // Add the value list to the active canvas
                Instances.ActiveCanvas.Document.AddObject(obj, false);

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

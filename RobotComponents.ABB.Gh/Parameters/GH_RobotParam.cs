// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System.Collections.Generic;
// Grasshopper Libs
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

namespace RobotComponents.ABB.Gh.Parameters
{
    /// <summary>
    /// An abstract base class for Robot Components parameters. 
    /// Inherits from GH_PersistentParam and provides a foundation for strongly typed, persistent parameters that handle data implementing IGH_Goo.
    /// </summary>
    /// <typeparam name="T">The type of data stored in the parameter, which must be a reference type implementing IGH_Goo.</typeparam>
    public abstract class GH_RobotParam<T> : GH_PersistentParam<T> where T : class, IGH_Goo
    {
        /// <summary>
        /// Constructs a generic Robot Components parameter. 
        /// </summary>
        /// <param name="name"> Name of the parameter. </param>
        /// <param name="nickname"> Nickname of the parameter. </param>
        /// <param name="category"> Category in which this parameter belongs. </param>
        /// <param name="description"> Description of the parameter. </param>
        protected GH_RobotParam(string name, string nickname, string category, string description)
        : base(name, nickname, description + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.VersionNumbering.CurrentVersion, "Robot Components ABB", category)
        {
        }

        #region disable pick parameters
        /// <summary>
        /// Disables picking of multiple values through the Grasshopper UI.
        /// Always returns GH_GetterResult.cancel to block user interaction.
        /// </summary>
        /// <param name="values"> The list intended to store picked values (unused). </param>
        /// <returns> GH_GetterResult.cancel to indicate the operation is canceled. </returns>
        protected override GH_GetterResult Prompt_Plural(ref List<T> values)
        {
            return GH_GetterResult.cancel;
        }

        /// <summary>
        /// Disables picking of a single value through the Grasshopper UI.
        /// Always returns GH_GetterResult.cancel to block user interaction.
        /// </summary>
        /// <param name="value"> The variable intended to store the picked value (unused). </param>
        /// <returns> GH_GetterResult.cancel to indicate the operation is canceled. </returns>
        protected override GH_GetterResult Prompt_Singular(ref T value)
        {
            return GH_GetterResult.cancel;
        }

        /// <summary>
        /// Overrides the context menu item for setting a single value.
        /// Returns a hidden menu item labeled "Not available".
        /// </summary>
        /// <returns> A hidden ToolStripMenuItem instance. </returns>
        protected override System.Windows.Forms.ToolStripMenuItem Menu_CustomSingleValueItem()
        {
            System.Windows.Forms.ToolStripMenuItem item = new System.Windows.Forms.ToolStripMenuItem
            {
                Text = "Not available",
                Visible = false
            };

            return item;
        }

        /// <summary>
        /// Overrides the context menu item for setting multiple values.
        /// Returns a hidden menu item labeled "Not available".
        /// </summary>
        /// <returns> A hidden ToolStripMenuItem instance. </returns>
        protected override System.Windows.Forms.ToolStripMenuItem Menu_CustomMultiValueItem()
        {
            System.Windows.Forms.ToolStripMenuItem item = new System.Windows.Forms.ToolStripMenuItem
            {
                Text = "Not available",
                Visible = false
            };

            return item;
        }
        #endregion
    }
}
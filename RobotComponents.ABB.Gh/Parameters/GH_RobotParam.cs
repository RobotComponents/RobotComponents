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
        protected override GH_GetterResult Prompt_Plural(ref List<T> values)
        {
            return GH_GetterResult.cancel;
        }

        protected override GH_GetterResult Prompt_Singular(ref T value)
        {
            return GH_GetterResult.cancel;
        }

        protected override System.Windows.Forms.ToolStripMenuItem Menu_CustomSingleValueItem()
        {
            System.Windows.Forms.ToolStripMenuItem item = new System.Windows.Forms.ToolStripMenuItem
            {
                Text = "Not available",
                Visible = false
            };

            return item;
        }

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
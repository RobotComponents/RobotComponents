// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components
// Project: https://github.com/RobotComponents/RobotComponents

namespace RobotComponents.ABB.Gh
{
    /// <summary>
    /// Derive from this class if you wish to perform additional steps before any of your components are loaded. 
    /// Any class in your project which inherits from GH_AssemblyPriority and which has an empty constructor
    /// will be instantiated prior to any GH_Component or IGH_DocumentObject classes.
    /// </summary>
    public class RobotComponentsCategoryIcon : Grasshopper.Kernel.GH_AssemblyPriority
    {
        /// <summary>
        /// This method will be called exactly once before any of the Components in your project are loaded.
        /// </summary>
        /// <returns> Loading instruction. </returns>
        public override Grasshopper.Kernel.GH_LoadingInstruction PriorityLoad()
        {
            Grasshopper.Instances.ComponentServer.AddCategoryIcon("Robot Components ABB", Properties.Resources.RobotComponents_Icon24);
            Grasshopper.Instances.ComponentServer.AddCategoryShortName("Robot Components ABB", "RC ABB");
            Grasshopper.Instances.ComponentServer.AddCategorySymbolName("Robot Components ABB", 'R');
            return Grasshopper.Kernel.GH_LoadingInstruction.Proceed;
        }
    }
}
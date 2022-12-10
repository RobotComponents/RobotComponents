// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

namespace RobotComponents.ABB.Gh
{
    public class RobotComponentsCategoryIcon : Grasshopper.Kernel.GH_AssemblyPriority
    {
        public override Grasshopper.Kernel.GH_LoadingInstruction PriorityLoad()
        {
            Grasshopper.Instances.ComponentServer.AddCategoryIcon("Robot Components ABB", Properties.Resources.RobotComponents_Icon24);
            Grasshopper.Instances.ComponentServer.AddCategoryShortName("Robot Components ABB", "RC ABB");
            Grasshopper.Instances.ComponentServer.AddCategorySymbolName("Robot Components ABB", 'R');
            return Grasshopper.Kernel.GH_LoadingInstruction.Proceed;
        }
    }
}
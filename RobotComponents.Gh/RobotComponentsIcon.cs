// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

namespace RobotComponents.Gh
{
    public class RobotComponentsCategoryIcon : Grasshopper.Kernel.GH_AssemblyPriority
    {
        public override Grasshopper.Kernel.GH_LoadingInstruction PriorityLoad()
        {
            Grasshopper.Instances.ComponentServer.AddCategoryIcon("RobotComponents", Properties.Resources.RobotComponents_Icon24);
            Grasshopper.Instances.ComponentServer.AddCategoryShortName("RobotComponents", "RC");
            Grasshopper.Instances.ComponentServer.AddCategorySymbolName("RobotComponents", 'R');
            return Grasshopper.Kernel.GH_LoadingInstruction.Proceed;
        }
    }
}
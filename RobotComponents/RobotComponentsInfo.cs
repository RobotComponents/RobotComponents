using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace RobotComponents
{
    public class RobotComponentsInfo : GH_AssemblyInfo
    {

        public override string Name
        {
            get
            {
                return "RobotComponents";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "A plugin for intuitively programming ABB Robots inside Rhinos Grasshopper.";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("37a01880-be52-49b3-afdb-27e4dcb3f346");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "Department of Experimental and Digital Design and Construction of the University of Kassel: Philipp Eversmann, Gabriel Rumpf, Benedikt Wannemacher, Arjen Deetman, Mohamed Dawod, Zuardin Akbar, Andrea Rossi.";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "gabriel.rumpf@posteo.de, wannemacher@asl.uni-kassel.de, eversmann@asl.uni-kassel.de ";
            }
        }
    }
}

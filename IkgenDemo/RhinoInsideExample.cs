using System;

using Rhino.Runtime.InProcess;
using Rhino.Geometry;
using Rhino;

namespace IkgenDemo
{
    class RhinoInsideExample
    {
        #region Program static constructor
   
        static RhinoInsideExample()
        {
            RhinoInside.Resolver.Initialize();
        }
        #endregion

        [System.STAThread]
        static void Main(string[] args)
        {
            try
            {
                using (new RhinoCore(args))
                {
                    RunExample();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }

            Console.WriteLine("press any key to exit");
            Console.ReadKey();
        }

        static void RunExample()
        {
        Console.WriteLine("Rhino Inside Example");
        }
    }
}

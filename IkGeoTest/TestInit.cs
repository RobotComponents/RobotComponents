using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Reflection;

using Microsoft.Win32;

namespace IkGeoTest
{
    /// <summary>
    /// Shared VS test assembly class using Rhino.Inside
    /// </summary>
    [TestClass]
    public static class TestInit
    {
        private static bool initialized = false;
        private static string rhinoDir;


        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            //get the correct rhino 7 installation directory
            rhinoDir = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\McNeel\Rhinoceros\7.0\Install", "Path", null) as string ?? string.Empty;
            Assert.IsTrue(System.IO.Directory.Exists(rhinoDir), "Rhino system dir not found: {0}", rhinoDir);
            context.WriteLine(" The current Rhino 7 installation is " + rhinoDir);

            if (initialized)
            {
                throw new InvalidOperationException("Initialize Rhino.Inside once");
            }
            else
            {
                RhinoInside.Resolver.Initialize();
                initialized = true;
                context.WriteLine("Rhino.Inside init has started");
            }

            // Ensure we are running the tests in x64
            Assert.IsTrue(Environment.Is64BitProcess, "Tests must be run as x64");

            // Set path to rhino system directory
            string envPath = Environment.GetEnvironmentVariable("path");
            Environment.SetEnvironmentVariable("path", envPath + ";" + rhinoDir);

            // Start a headless rhino instance using Rhino.Inside
            StartRhino();

            // We have to load grasshopper.dll on the current AppDomain manually for some reason
            AppDomain.CurrentDomain.AssemblyResolve += ResolveGrasshopper;
        }

        /// <summary>
        /// Starting Rhino - loading the relevant libraries
        /// </summary>
        [STAThread]
        public static void StartRhino()
        {
            var _rhinoCore = new Rhino.Runtime.InProcess.RhinoCore(null, Rhino.Runtime.InProcess.WindowStyle.NoWindow);
        }


        /// <summary>
        /// Add Grasshopper.dll to the current Appdomain
        /// </summary>
        private static Assembly ResolveGrasshopper(object sender, ResolveEventArgs args)
        {
            var name = args.Name;

            if (!name.StartsWith("Grasshopper"))
            {
                return null;
            }

            var path = Path.Combine(Path.GetFullPath(Path.Combine(rhinoDir, @"..\")), "Plug-ins\\Grasshopper\\Grasshopper.dll");
            return Assembly.LoadFrom(path);
        }
    }
}


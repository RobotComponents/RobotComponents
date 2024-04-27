// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// Syste Libs
using System;
// Rhino Libs
using Rhino.Geometry;
using Rhino.Runtime.InProcess;
// Robot Components Libs
using RobotComponents.ABB.Presets.Enumerations;
using RobotComponents.ABB.Kinematics;
using RobotComponents.ABB.Definitions;
using RobotComponents.ABB.Actions.Declarations;
using RobotComponents.ABB.Actions.Instructions;
using RobotComponents.ABB.Presets;

namespace IkfastDemo
{
    class InverseKinematicsDemo
    {
        #region Program static constructor
        static InverseKinematicsDemo()
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
                    RunDemo();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Program error");
                Console.Error.WriteLine(ex.Message);
                Console.WriteLine("");
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static void RunDemo()
        {
            Console.WriteLine("*** Inverse Kinematics Demo ***");
            Console.WriteLine("");

            /*
            Plane tcp = new Plane(new Point3d(0, 0, 0), Vector3d.XAxis, Vector3d.YAxis);
            RobotTool tool1 = new RobotTool("tool1", new Mesh(), Plane.WorldXY, tcp);
            */

            // Set non trivial tcp and tool
            Plane tcp = new Plane(new Point3d(20, 40, 80), Vector3d.XAxis, Vector3d.YAxis);
            RobotTool tool1 = new RobotTool("tool1", new Mesh(), Plane.WorldXY, tcp);

            // Compute IK of robots with non-trivial base pose.
            ShowRobotIK(Factory.GetRobotPreset(RobotPreset.CRB15000_5_095, Plane.WorldYZ, tool1));
            //ShowRobotIK(CRB15000_10_152.GetRobot(Plane.WorldYZ, tool1));  // Preset seems to be not implemented yet
            //ShowRobotIK(Factory.GetRobotPreset(RobotPreset.IRB1010_1_5_037, Plane.WorldXY, tool1));
            
        }

        static void ShowRobotIK(Robot robot)
        {
            Console.WriteLine($"Robot {robot.Name}");
            Console.WriteLine("");

            // Initialize kinematics solvers
            ForwardKinematics forwardKinematics = new ForwardKinematics(robot);

            // Create a robot target from a given robot joint position
            RobotJointPosition robotJointPosition = new RobotJointPosition(-40, 30, -60, 70, 50, 45);
            //RobotJointPosition robotJointPosition = new RobotJointPosition(40, 30, 60, 70, 50, 45);
            //RobotJointPosition robotJointPosition = new RobotJointPosition(0, 0, 0, 0, 0, 0);

            forwardKinematics.Calculate(robotJointPosition);
            Plane toolEndPlane = forwardKinematics.TCPPlane;
            RobotTarget robotTarget = new RobotTarget(toolEndPlane);

            // Calcuate solutions
            // TODO debug late target passing
            // Passing the movement target after the simple instructor call seems fail in the 
            // computation of target planes. I guess the order of method calls inside InverseKinematics
            // needs to be modified. 
            // I.e. these two lines fail:
            //InverseKinematics inverseKinematics = new InverseKinematics(robot);
            //inverseKinematics.Movement.Target = robotTarget; 

            InverseKinematics inverseKinematics = new InverseKinematics(robot);
            inverseKinematics.Calculate(new Movement(robotTarget));

            // Write IK error text
            if (inverseKinematics.ErrorText.Count != 0)
            {
                Console.WriteLine("IK error text");

                for (int i = 0; i < inverseKinematics.ErrorText.Count; i++)
                {
                    Console.WriteLine(inverseKinematics.ErrorText[i]);
                }

                Console.WriteLine("");
            }

            // Write data to console
            Console.WriteLine("Set Target Plane");
            Console.WriteLine($"Position    :   {robotTarget.Plane.Origin}");
            Console.WriteLine($"Orientation :   {robotTarget.Quat}");
            Console.WriteLine("");

            Console.WriteLine("Set Robot Joint Position");
            Console.WriteLine(robotJointPosition.ToString());
            Console.WriteLine("");

            Console.WriteLine("Calculated Robot Joint Positions");
            
            for (int i = 0; i < inverseKinematics.RobotJointPositions.Count; i++)
            {
                Console.WriteLine(inverseKinematics.RobotJointPositions[i].ToString());
            }

            Console.WriteLine("");
        }
    }
}

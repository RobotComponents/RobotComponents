using System;

using Rhino;
using Rhino.Geometry;
using Rhino.Runtime.InProcess;

using RobotComponents.ABB.Presets.Robots;
using RobotComponents.ABB.Kinematics;
using RobotComponents.ABB.Definitions;
using RobotComponents.ABB.Actions.Declarations;

namespace IkgenDemo
{
    class Robot_CRB15000_5_095
    {
        #region Program static constructor
        static Robot_CRB15000_5_095()
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
                    InverseKinematicsDemo();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }

            Console.WriteLine("press any key to exit");
            Console.ReadKey();
        }

        static void InverseKinematicsDemo()
        {
            Console.WriteLine("*** Inverse Kinematics Demo ***");

            Plane robotBasePose = new Plane(new Point3d(0, 0, 0),
                new Rhino.Geometry.Vector3d(1, 0, 0), new Rhino.Geometry.Vector3d(0, 1, 0));
            RobotTool tool = new RobotTool("TCP", new Mesh(), 0.0, 0.0, 0.0, 0.0, 0.0, 0.0);

            Robot robot = CRB15000_5_095.GetRobot(robotBasePose, tool, null);

            ForwardKinematics fkrc = new ForwardKinematics(robot, true);
            RobotComponents.ABB.Kinematics.Ikgen.InverseKinematics ikgen = 
                new RobotComponents.ABB.Kinematics.Ikgen.InverseKinematics(robot);

            Plane targetPose;
            Point3d targetPos;
            double x, y, z, w;

            targetPos = new Point3d(100, 130, 240);
            x = 0.4; y = 0.3; z = 0.8; w = -0.15;
            Quaternion targetOri = new Quaternion(w, x, y, z);

            targetOri.GetRotation(out targetPose);
            targetPose.Origin = targetPos;

            //targetPose = new Plane(targetPos, new Vector3d(1, 0, 0), new Vector3d(0, 1, 0));

            Console.WriteLine($"Target end-effector plane (plane origin, plane normal): " +
                $"{targetPose.Origin:F2}\t{targetPose.Normal:F2}");

            ikgen.TargetPlane = targetPose;
            ikgen.Compute();

            Console.WriteLine("ikgen found {0} solutions (joint positions in deg) .", ikgen.NumSolutions);

            for (int i = 0; i<ikgen.NumSolutions; i++)
            {
                Console.Write("{0}: ", i);

                PrintJointPositions(ikgen.RobotJointPositions[i]);

                // Recompute end-effector pose
                fkrc.Calculate(ikgen.RobotJointPositions[i]);

                Console.WriteLine($"Recomputed end-effector plane: " +
                    $"{fkrc.TCPPlane.Origin:F2}\t{fkrc.TCPPlane.Normal:F2}");
            }


        }

        static void PrintJointPositions(RobotJointPosition joints)
        {
            //Console.WriteLine(String.Join("\t", joints.ToArray()));

            foreach (var val in joints.ToArray())
            {
                Console.Write($"{val:F3} ");
            }
            Console.Write("\n");
        }

    }
}

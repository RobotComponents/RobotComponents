// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
// Microsoft Libs
using Microsoft.VisualStudio.TestTools.UnitTesting;
// Robot Components Libs
using RobotComponents.ABB.Kinematics;
using RobotComponents.ABB.Kinematics.IkGeo;
using RobotComponents.ABB.Definitions;
using RobotComponents.ABB.Actions.Declarations;
using RobotComponents.ABB.Presets;
using RobotComponents.ABB.Presets.Robots;
using RobotComponents.ABB.Presets.Enumerations;
// Rhino Libs
using Rhino;
using Rhino.Geometry;
//using RobotComponents.ABB.Actions.Interfaces;


namespace IkGeoTest
{
    [TestClass]
    public class IkGeoSolverTest
    {

        /// <summary>
        /// Test that solutions are found and correct.
        /// </summary>
        [TestMethod]
        public void Test_InverseKinematicsSolutions()
        {
            const int n_runs = 4;
            const double tol = 0.0001;

            Plane eePose;
            Plane eePose_recomputed;

            Random rnd = new Random();

            RobotTool tool = new RobotTool("tool", new Mesh(), Rhino.Geometry.Plane.WorldXY, Plane.WorldXY);

            Robot robot = Factory.GetRobotPreset(RobotPreset.CRB15000_5_095, Plane.WorldXY, tool);
            //Robot robot = CRB15000_5_095.GetRobot(robotBasePose, tool, null);

            ForwardKinematics fk = new ForwardKinematics(robot);
            IkGeoSolver ikgeo = new IkGeoSolver(robot);

            List<double> jointsRnd;

            for (int i = 0; i < n_runs; i++)
            {
                jointsRnd = new List<double>(6) { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };

                // Generate random joint positions within +- 180 deg
                jointsRnd[0] = ((rnd.NextDouble() * 2.0 - 1.0) * 180);
                jointsRnd[1] = ((rnd.NextDouble() * 2.0 - 1.0) * 180);
                jointsRnd[2] = ((rnd.NextDouble() * 2.0 - 1.0) * 180);
                jointsRnd[3] = ((rnd.NextDouble() * 2.0 - 1.0) * 180);
                jointsRnd[4] = ((rnd.NextDouble() * 2.0 - 1.0) * 180);
                jointsRnd[5] = ((rnd.NextDouble() * 2.0 - 1.0) * 180);

                // Compute the forward kinematics
                fk.Calculate(new RobotJointPosition(
                    jointsRnd[0],
                    jointsRnd[1],
                    jointsRnd[2],
                    jointsRnd[3],
                    jointsRnd[4],
                    jointsRnd[5]));

                eePose = fk.TCPPlane;

                // Compute the inverse kinematics
                ikgeo.Compute_CRB15000_5_095(eePose);

                Assert.IsTrue(ikgeo.NumSolutions > 0, "IkFast found no solution");

                // Recompute EE pose (forward kinematics) for all solutions and compare
                for (int j = 0; j < ikgeo.RobotJointPositions.Count; j++)
                {

                    // Skip places with missing solution
                    if ( Enumerable.SequenceEqual(
                        ikgeo.RobotJointPositions[j].ToArray(), 
                        new[] { 9e9, 9e9, 9e9, 9e9, 9e9, 9e9}) )
                    {
                        continue;
                    }

                    // Recompute EE pose
                    fk.Calculate(new RobotJointPosition(
                        ikgeo.RobotJointPositions[j][0],
                        ikgeo.RobotJointPositions[j][1],
                        ikgeo.RobotJointPositions[j][2],
                        ikgeo.RobotJointPositions[j][3],
                        ikgeo.RobotJointPositions[j][4],
                        ikgeo.RobotJointPositions[j][5]));

                    eePose_recomputed = fk.TCPPlane;

                    // Compare precomputed EE pose with EE pose recomputed through ikgen
                    Assert.AreEqual(eePose.OriginX, eePose_recomputed.OriginX, tol, "Pos x");
                    Assert.AreEqual(eePose.OriginY, eePose_recomputed.OriginY, tol, "Pos y");
                    Assert.AreEqual(eePose.OriginZ, eePose_recomputed.OriginZ, tol, "Pos z");
                    Assert.AreEqual(eePose.Normal.X, eePose_recomputed.Normal.X, tol, "Normal x");
                    Assert.AreEqual(eePose.Normal.Y, eePose_recomputed.Normal.Y, tol, "Normal y");
                    Assert.AreEqual(eePose.Normal.Z, eePose_recomputed.Normal.Z, tol, "Normal z");
                }

                Console.WriteLine($"Run {i}: all {ikgeo.NumSolutions} ik solutions are correct");
            }
            
        }

        /// <summary>
        /// Test that the ConfigurationComparer sorts joint positions in desired order.
        /// </summary>
		[Ignore]
        [TestMethod]
        public void Test_ConfigurationComparer()
        {
            // Test only the public comparer class
            RobotComponents.ABB.Kinematics.IkGeo.ConfigurationComparer comparer = 
                new RobotComponents.ABB.Kinematics.IkGeo.ConfigurationComparer();

            const int  list_size = 6;

            List<RobotJointPosition> posList = new List<RobotJointPosition>();

            posList.Add(new RobotJointPosition(180.0, 10.0, -100.0, 10.0, 10.0, 10.0));
            posList.Add(new RobotJointPosition(-10.0, 10.0, 10.0, 0.0, 0.0, 0.0));
            posList.Add(new RobotJointPosition(-100.0, 0.0, 0.0, 100.0, 0.0, 101.0));
            posList.Add(new RobotJointPosition(-100.0, 0.0, 0.0, 100.0, 0.0, 100.0));
            posList.Add(new RobotJointPosition(30.0, 0.0, 0.0, 100.0, 0.0, 100.0));
            posList.Add(new RobotJointPosition(180.0, 10.0, -100.0, -90.0, 10.0, 10.0));

            // Manually sort the robot positions
            List<RobotJointPosition> posList_sorted = new List<RobotJointPosition>();

            posList_sorted.Add(new RobotJointPosition(-100.0, 0.0, 0.0, 100.0, 0.0, 101.0));
            posList_sorted.Add(new RobotJointPosition(-100.0, 0.0, 0.0, 100.0, 0.0, 100.0));
            posList_sorted.Add(new RobotJointPosition(-10.0, 10.0, 10.0, 0.0, 0.0, 0.0));
            posList_sorted.Add(new RobotJointPosition(30.0, 0.0, 0.0, 100.0, 0.0, 100.0));
            posList_sorted.Add(new RobotJointPosition(180.0, 10.0, -100.0, -90.0, 10.0, 10.0));
            posList_sorted.Add(new RobotJointPosition(180.0, 10.0, -100.0, 10.0, 10.0, 10.0));

            // TEST equality of list sequence after sorting
            posList.Sort(comparer);

            //bool res = posList.SequenceEqual(posList_sorted); // Appears not to work

            for (int i = 0; i < list_size; i++)
            {
                // SequenceEqual seems to work only on array level
                Assert.IsTrue(posList[i].ToArray().SequenceEqual(posList_sorted[i].ToArray()));
            }
        }

        /// <summary>
        /// Test that arranged joint positions are complete.
        /// </summary>
        [Ignore] // Requires modification in ArrangeJointPositions.
        [TestMethod]
        public void Test_ArrangedJointPositions()
        {
            const int n_runs = 4;

            int solution_count = 0;

            Random rnd = new Random();

            RobotTool tool = new RobotTool("tool", new Mesh(), Rhino.Geometry.Plane.WorldXY, Plane.WorldXY);

            Robot robot = Factory.GetRobotPreset(RobotPreset.CRB15000_5_095, Plane.WorldXY, tool);

            ForwardKinematics fk = new ForwardKinematics(robot);
            IkGeoSolver ikgeo = new IkGeoSolver(robot);

            List<double> jointsRnd;

            for (int i = 0; i < n_runs; i++)
            {
                jointsRnd = new List<double>(6) { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };

                // Generate random joint positions within +- 180 deg
                jointsRnd[0] = ((rnd.NextDouble() * 2.0 - 1.0) * 180);
                jointsRnd[1] = ((rnd.NextDouble() * 2.0 - 1.0) * 180);
                jointsRnd[2] = ((rnd.NextDouble() * 2.0 - 1.0) * 180);
                jointsRnd[3] = ((rnd.NextDouble() * 2.0 - 1.0) * 180);
                jointsRnd[4] = ((rnd.NextDouble() * 2.0 - 1.0) * 180);
                jointsRnd[5] = ((rnd.NextDouble() * 2.0 - 1.0) * 180);

                // Compute the forward kinematics
                fk.Calculate(new RobotJointPosition(
                    jointsRnd[0],
                    jointsRnd[1],
                    jointsRnd[2],
                    jointsRnd[3],
                    jointsRnd[4],
                    jointsRnd[5]));

                // Compute the inverse kinematics
                ikgeo.Compute_CRB15000_5_095(fk.TCPPlane);

                // Count the number of solutions
                for (int j = 0; j < ikgeo.RobotJointPositions.Count; j++)
                {

                    // Look for places with missing solution
                    if (! Enumerable.SequenceEqual(
                        ikgeo.RobotJointPositions[j].ToArray(),
                        new[] { 9e9, 9e9, 9e9, 9e9, 9e9, 9e9 }))
                    {
                        solution_count++;
                    }
                }

                Assert.AreEqual(ikgeo.NumSolutions, solution_count, "Num ikgeo solutions is inconsistent.");
            }

        }
    }
}

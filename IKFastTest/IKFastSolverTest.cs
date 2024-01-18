using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Security.Claims;

using RobotComponents.ABB.Kinematics;
using RobotComponents.ABB.Actions.Declarations;
using System.Collections.Generic;
using System.Linq;

namespace IKFastTest
{
    [TestClass]
    public class IKFastSolverTest
    {
        [TestMethod]
        public void Test_ConfigurationComparer()
        {
            // Test only the public comparer class
            RobotComponents.ABB.Kinematics.IKFast.ConfigurationComparer comparer = 
                new RobotComponents.ABB.Kinematics.IKFast.ConfigurationComparer();

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
    }
}

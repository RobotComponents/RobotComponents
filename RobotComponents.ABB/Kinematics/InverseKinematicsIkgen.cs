
using Rhino;
using Rhino.Geometry;

using RobotComponents.ABB.Actions.Declarations;
using RobotComponents.ABB.Actions.Instructions;
using RobotComponents.ABB.Actions.Interfaces;
using RobotComponents.ABB.Definitions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace RobotComponents.ABB.Kinematics.Ikgen
{
    internal struct Vector6d
    {
        public double x1, x2, x3, x4, x5, x6;
    }

    internal struct Vector3d
    {
        public double x, y, z;
    }

    internal struct Quaternion
    {
        public double x, y, z, w;
    }

    /// <summary>
    /// Inverse kinematics for the CRB15000_5_095 robot.
    /// </summary>
    /// <remarks>
    /// This class wraps the ikfast library generated for the CRB15000_5_095 robot. 
    /// So far, this is the only robot supported.
    /// </remarks>
    public class InverseKinematics
    {

        private int _n_solutions;

        private Robot _robot;
        private RobotTool _robotTool;
        private ITarget _target;

        private Plane _endPlane;
        private Plane _robotBasePose;

        private RobotJointPosition _robotJointPosition = new RobotJointPosition();
        private List<RobotJointPosition> _robotJointPositions = 
            new List<RobotJointPosition>();


        #region constructor
        /// <summary>
        /// Initializes a new instance of the Inverse Kinematics class from a Robot.
        /// </summary>
        /// <param name="robot"> The Robot. </param>
        public InverseKinematics(Robot robot)
        {
            // Check for supported robots
            if (robot.Name != "CRB15000-5/0.95")
            {
                throw new ArgumentException($"Robot {robot.Name} not supported", nameof(robot));
            }

            _robot = robot;
            _endPlane = new Plane();

            // TODO take from robot
            _robotBasePose = new Plane(new Point3d(0, 0, 0), new Rhino.Geometry.Vector3d(1, 0, 0), new Rhino.Geometry.Vector3d(0, 1, 0));

            _target = new RobotTarget();

            /*
            _target = _movement.Target;

            // Check robot tool: override if the movement contains a robot tool
            if (_movement.RobotTool == null || _movement.RobotTool.Name == null || _movement.RobotTool.Name == "")
            {
                _robotTool = _robot.Tool;
            }
            else
            {
                _robotTool = _movement.RobotTool;
            }
            */
        }
        #endregion

        /// <summary>
        /// Computes the inverse kinematics.
        /// </summary>
        public void Compute()
        {
            CalculateRobotJointPosition();
        }

        /// <summary>
        /// Calculates the Robot Joint Position for end effector plane in robot coordinates.
        /// </summary>
        /// <remarks>
        /// Internal axis limits are not taken into account.
        /// </remarks>
        public void CalculateRobotJointPosition()
        {
            _n_solutions = 0;

            _robotJointPositions.Clear();
            _robotJointPosition.Reset();

            if (_target is RobotTarget robotTarget)
            {

                Vector3d pos_end = new Vector3d
                {
                    x = _endPlane.Origin.X,
                    y = _endPlane.Origin.Y,
                    z = _endPlane.Origin.Z
                };

                // Convert plane orientation to quaternion
                Rhino.Geometry.Quaternion q_end = new Rhino.Geometry.Quaternion();
                q_end.SetRotation(_robotBasePose, _endPlane);

                // Correct for an (unknown) end effector rotation transformation.
                // This is a 90 deg rotation around the y axis.
                Rhino.Geometry.Quaternion q_gap = new Rhino.Geometry.Quaternion();
                q_gap.B = 0.0;
                q_gap.C = 0.707;
                q_gap.D = 0.0;
                q_gap.A = 0.707;

                Rhino.Geometry.Quaternion q_end_closed = q_end * q_gap.Inverse;

                Quaternion ori_end = new Quaternion // ikgen.Quaternion
                {
                    // Be careful with quaternion naming conventions
                    x = q_end_closed.B,
                    y = q_end_closed.C,
                    z = q_end_closed.D,
                    w = q_end_closed.A
                };

                RobotJointPosition jointPos;

                unsafe
                {
                    Vector6d* joints_ikgen = computeInverseKinematics(
                        ref pos_end, ref ori_end, out _n_solutions);

                    for (int i = 0; i < _n_solutions; i++)
                    {
                        jointPos = new RobotJointPosition();

                        jointPos[0] = RhinoMath.ToDegrees(joints_ikgen[i].x1);
                        jointPos[1] = RhinoMath.ToDegrees(joints_ikgen[i].x2);
                        jointPos[2] = RhinoMath.ToDegrees(joints_ikgen[i].x3);
                        jointPos[3] = RhinoMath.ToDegrees(joints_ikgen[i].x4);
                        jointPos[4] = RhinoMath.ToDegrees(joints_ikgen[i].x5);
                        jointPos[5] = RhinoMath.ToDegrees(joints_ikgen[i].x6);

                        _robotJointPositions.Add(jointPos);
                    }
                }

                _robotJointPosition = _robotJointPositions.DefaultIfEmpty(new RobotJointPosition()).Last();
            }
            else if (_target is JointTarget jointTarget)
            {
                _robotJointPosition = jointTarget.RobotJointPosition;
            }
            else
            {
                _robotJointPosition = new RobotJointPosition();
            }
        }

        /// <summary>
        /// Set the targeted end effector plane.
        /// </summary>
        public Plane TargetPlane
        {
            set { _endPlane = value; }
        }

        /// <summary>
        /// Gets the latest calculated Robot Joint Position.
        /// </summary>
        public RobotJointPosition RobotJointPosition
        {
            get { return _robotJointPosition; }
        }

        /// <summary>
        /// Gets all calculated Robot Joint Position.
        /// </summary>
        public List<RobotJointPosition> RobotJointPositions
        {
            get { return _robotJointPositions; }
        }

        /// <summary>
        /// Get number of computed inverse kinematics solutions.
        /// </summary>
        public int NumSolutions
        {
            get { return _n_solutions; }
        }

        //[DllImport("rcik.dll")]
        //private static extern void computeInverseKinematics(
        //    ref Vector3d eePos, ref Quaternion eeOri, out Vector6d jointPos);

        [DllImport("rcik.dll", EntryPoint = "computeInverseKinematics")]
        private static unsafe extern Vector6d* computeInverseKinematics(
            ref Vector3d eePos, ref Quaternion eeOri, out int n_sol);

    }
}


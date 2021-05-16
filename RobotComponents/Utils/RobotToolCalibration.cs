// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Linq;
using System.Collections.Generic;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.Actions;
using RobotComponents.Definitions;

namespace RobotComponents.Utils
{
	/// <summary>
	/// Represents the Robot Tool Calibration class.
	/// This class uses Newton's method and numerical differentiation to calculate the TCP from given Joint Positions. 
	/// If desired, the parameters for this optimization can be changed via the properties of this class.
	/// </summary>
	public class RobotToolCalibration
	{
		#region fields
		private Robot _robot;
		private List<RobotJointPosition> _robotJointPositions = new List<RobotJointPosition>();
		private List<ExternalJointPosition> _externalJointPositions = new List<ExternalJointPosition>();
		private Plane[] _frames;
		private Transform[] _transformations;
		private Point3d _averagePoint;
		private double _x;
		private double _y;
		private double _z;
		private double _xInitial = 0.0;
		private double _yInitial = 0.0;
		private double _zInitial = 0.0;
		private int _iterations = 400000;
		private double _precision = 1e-2;
		private double _delta = 1e-12;
		private double _damping = 0.01;
		private double[] _errorsX;
		private double[] _errorsY;
		private double[] _errorsZ;
		#endregion

		#region constructors
		/// <summary>
		/// Initializes an empty instance of the Robot Tool Calibration class.
		/// </summary>
		public RobotToolCalibration()
        {
        }

		/// <summary>
		/// Initializes a new instance of the Robot Tool Calibration class from given Robot Joint Positions.
		/// The External Joint Positions are set with default values (9e9).
		/// </summary>
		/// <param name="robot"> The Robot the Joint Positions are given for. </param>
		/// <param name="robotJointPositions"> The Robot Joint Positions as a list. </param>
		public RobotToolCalibration(Robot robot, List<RobotJointPosition> robotJointPositions)
		{
			_robot = robot.Duplicate();
			_robotJointPositions = robotJointPositions;
			_externalJointPositions = Enumerable.Repeat(new ExternalJointPosition(), _robotJointPositions.Count).ToList();

			Initialize();
		}

		/// <summary>
		/// Initializes a new instance of the Robot Tool Calibration class.
		/// </summary>
		/// <param name="robot"> The Robot the Joint Positions are given for. </param>
		/// <param name="robotJointPositions"> The Robot Joint Positions as a list. </param>
		/// <param name="externalJointPositions"> The External Joint Positions as a list. </param>
		public RobotToolCalibration(Robot robot, List<RobotJointPosition> robotJointPositions, List<ExternalJointPosition> externalJointPositions)
        {
			_robot = robot.Duplicate();
			_robotJointPositions = robotJointPositions;
			_externalJointPositions = externalJointPositions;

			Initialize();
        }

		/// <summary>
		/// Initializes a new instance of the Robot Tool Calibration class by duplicating an existing Robot Tool Calibration instance.
		/// </summary>
		/// <param name="calibration"> The Robot Tool Calibration instance to duplicate. </param>
		public RobotToolCalibration(RobotToolCalibration calibration)
        {
			_robot = calibration.Robot.Duplicate();
			_robotJointPositions = calibration.RobotJointPositions.ConvertAll(position => position.Duplicate());
			_externalJointPositions = calibration.ExternalJointPositions.ConvertAll(position => position.Duplicate());
			_averagePoint = new Point3d(calibration.TargetPoint);
			_x = calibration.X;
			_y = calibration.Y;
			_z = calibration.Z;
			_xInitial = calibration.XInitial;
			_yInitial = calibration.YInitial;
			_zInitial = calibration.ZInitial;
			_iterations = calibration.Iterations;
			_precision = calibration.Precision;
			_delta = calibration.Delta;
			_damping = calibration.Damping;
			_errorsX = calibration.ErrorsX.Select(error => error).ToArray();
			_errorsY = calibration.ErrorsY.Select(error => error).ToArray();
			_errorsZ = calibration.ErrorsZ.Select(error => error).ToArray();

			CreateFrames();
		}

		/// <summary>
		/// Returns an exact duplicate of this Robot Tool Calibration instance.
		/// </summary>
		/// <returns> A deep copy of the Robot Tool Calibration instance. </returns>
		public RobotToolCalibration Duplicate()
        {
			return new RobotToolCalibration(this);
        }
		#endregion

		#region methods
		/// <summary>
		/// Initializes the fields and properties to construct a valid Robot Tool Calibration instance. 
		/// </summary>
		private void Initialize()
        {
			// Check list length with joint positions
			if (_externalJointPositions.Count == 0 && _robotJointPositions.Count != 0)
            {
				for (int i = 0; i < _robotJointPositions.Count; i++)
                {
					_externalJointPositions.Add(new ExternalJointPosition());
                }
            }
			else if (_robotJointPositions.Count < _externalJointPositions.Count)
            {
				int n = _externalJointPositions.Count - _robotJointPositions.Count;
				for (int i = 0; i < n; i++)
                {
					_robotJointPositions.Add(_robotJointPositions[_robotJointPositions.Count - 1].Duplicate());
				}
            }
			else if (_externalJointPositions.Count < _robotJointPositions.Count)
            {
				int n = _robotJointPositions.Count - _externalJointPositions.Count;
				for (int i = 0; i < n; i++)
				{
					_externalJointPositions.Add(_externalJointPositions[_externalJointPositions.Count - 1].Duplicate());
				}
			}

			// Initiate arrays
			_frames = new Plane[_robotJointPositions.Count];
			_transformations = new Transform[_robotJointPositions.Count];
			_errorsX = new double[_robotJointPositions.Count];
			_errorsY = new double[_robotJointPositions.Count];
			_errorsZ = new double[_robotJointPositions.Count];
		}

		/// <summary>
		/// Reinitializes the fields and properties to construct a valid Robot Tool Calibration instance. 
		/// </summary>
		public void ReInitialize()
        {
			Initialize();

			// Reset values
			_averagePoint = new Point3d(0, 0, 0);
			_x = _xInitial;
			_y = _yInitial;
			_z = _zInitial;
		}

		/// <summary>
		/// Checks if the given Joint Positions are within the axis limits of the robot.
		/// </summary>
		/// <returns> A list with erros messages. When the list is empty, all the joint positions are within the limits. </returns>
		public List<string> CheckJointPositionsAxisLimits()
        {
			List<string> errors = new List<string>();

			for (int i = 0; i < _robotJointPositions.Count; i++)
            {
				JointTarget jointTarget = new JointTarget((i+1).ToString(), _robotJointPositions[i], _externalJointPositions[i]);
				errors.AddRange(jointTarget.CheckAxisLimits(_robot));
			}

			return errors;
        }

		/// <summary>
		/// Calculates the Tool Center Point.
		/// </summary>
		public void Calculate()
        {
			_x = _xInitial;
			_y = _yInitial;
			_z = _zInitial;

			double fvalx;
			double fvaly;
			double fvalz;
			double fval1;
			double fval2;
			double changeX;
			double changeY;
			double changeZ;

			CreateFrames();

			for (int i = 0; i < _iterations; i++)
            {
				double xOld = _x;
				double yOld = _y;
				double zOld = _z;

				CalculateFunctionValues(xOld, yOld, zOld);
				fvalx = _errorsX.Average();
				fvaly = _errorsY.Average();
				fvalz = _errorsZ.Average();

				#region X
				CalculateFunctionValues(xOld + _delta, yOld, zOld);
				fval1 = _errorsX.Average();

				CalculateFunctionValues(xOld - _delta, yOld, zOld);
				fval2 = _errorsX.Average();

				if (fval1 - fval2 != 0)
                {
					_x -= _damping * (fvalx / ((fval1 - fval2) / (2 * _delta)));
				}
				#endregion

				#region Y
				CalculateFunctionValues(xOld, yOld + _delta, zOld);
				fval1 = _errorsY.Average();

				CalculateFunctionValues(xOld, yOld - _delta, zOld);
				fval2 = _errorsY.Average();

				if (fval1 - fval2 != 0)
				{
					_y -= _damping * (fvaly / ((fval1 - fval2) / (2 * _delta)));
				}
				#endregion

				#region Z
				CalculateFunctionValues(xOld, yOld, zOld + _delta);
				fval1 = _errorsZ.Average();

				CalculateFunctionValues(xOld, yOld, zOld - _delta);
				fval2 = _errorsZ.Average();

				if (fval1 - fval2 != 0)
				{
					_z -= _damping * (fvalz / ((fval1 - fval2) / (2 * _delta)));
				}
				#endregion

				changeX = Math.Abs(xOld - _x);
				changeY = Math.Abs(yOld - _y);
				changeZ = Math.Abs(zOld - _z);

				if (changeX < _precision && changeY < _precision && changeZ < _precision)
                {
					break;
                }
			}

			CalculateFunctionValues(_x, _y, _z);
		}

		/// <summary>
		/// Creates the reference frames (the end planes of the 6th axis for the different positions).
		/// </summary>
		private void CreateFrames()
        {
			_robot.Tool = new RobotTool();
			_robot.ForwardKinematics.HideMesh = true;

			for (int i = 0; i < _frames.Length; i++)
            {
				_robot.ForwardKinematics.Calculate(_robotJointPositions[i], _externalJointPositions[i]);
				_frames[i] = new Plane(_robot.ForwardKinematics.TCPPlane);
				_transformations[i] = Transform.PlaneToPlane(Plane.WorldXY, _frames[i]);
			}
        }

		/// <summary>
		/// Calculates the fuctions values (the errors) for the optimization.
		/// </summary>
		/// <param name="x"> The x-coordinate of the TCP point. </param>
		/// <param name="y"> The y-coordinate of the TCP point. </param>
		/// <param name="z"> The z-coordinate of the TCP point.</param>
		private void CalculateFunctionValues(double x, double y, double z)
        {
			_averagePoint = new Point3d(0, 0, 0);
			Point3d[] points = new Point3d[_frames.Length];
			int n = _frames.Length;

			for (int i = 0; i < n; i++)
            {
				Point3d point = new Point3d(x, y, z);
				point.Transform(_transformations[i]);
				points[i] = point;
				_averagePoint += point / n;
			}

			for (int i = 0; i < n; i++)
            {
				_errorsX[i] = Math.Abs(_averagePoint.X - points[i].X);
				_errorsY[i] = Math.Abs(_averagePoint.Y - points[i].Y);
				_errorsZ[i] = Math.Abs(_averagePoint.Z - points[i].Z);
			}
		}
		#endregion

		#region properties
		/// <summary>
		/// Gets a value indicating whether or not the object is valid.
		/// </summary>
		public bool IsValid
		{
			get 
			{
				if (_robot.IsValid != true) { return false; }
				if (_robotJointPositions.Count < 4) { return false; }
				if (_externalJointPositions.Count < 4) { return false; }
				if (_robotJointPositions.Count != _externalJointPositions.Count) { return false; }
				if (_iterations < 1) { return false; }
				if (_delta < 0) { return false; }
				if (_damping < 0) { return false; }
				return true; 
			}
		}

		/// <summary>
		/// Gets or sets the Robot that is used for the tool calibration.
		/// </summary>
		public Robot Robot
        {
            get { return _robot; }
			set { _robot = value.Duplicate(); }
        }

		/// <summary>
		/// Gets or sets the Robot Joint Positions to calculate the TCP from.
		/// </summary>
		public List<RobotJointPosition> RobotJointPositions
        {
            get { return _robotJointPositions; }
			set { _robotJointPositions = value; }
        }

		/// <summary>
		/// Gets or sets the External Joint Positions to calculate the tool center point from.
		/// </summary>
		public List<ExternalJointPosition> ExternalJointPositions
		{
			get	{ return _externalJointPositions; }
			set { _externalJointPositions = value; }
		}

		/// <summary>
		/// Gets or sets the initial x-coordinate of the tool center point to start the optimization with.
		/// </summary>
		public double XInitial
        {
			get { return _xInitial; }
			set { _xInitial = value; }
        }

		/// <summary>
		/// Gets or sets the initial y-coordinate of the tool center point to start the optimization with.
		/// </summary>
		public double YInitial
		{
			get { return _yInitial; }
			set { _yInitial = value; }
		}

		/// <summary>
		/// Gets or sets the initial z-coordinate of the tool center point to start the optimization with.
		/// </summary>
		public double ZInitial
		{
			get { return _zInitial; }
			set { _zInitial = value; }
		}

		/// <summary>
		/// Gets or sets the maximum number of iterations of the tool calibation optimization.
		/// </summary>
		public int Iterations
        {
			get { return _iterations; }
			set { _iterations = value; }
        }

		/// <summary>
		/// Gets or sets the desired precision of the tool calibraton optimization.
		/// </summary>
		public double Precision
        {
			get { return _precision; }
			set { _precision = value; }
        }

		/// <summary>
		/// Gets or sets the damping factor of the tool calibraton optimization.
		/// </summary>
		public double Damping
        {
			get { return _damping; }
            set { _damping = value; }
        }

		/// <summary>
		/// Gets or sets the delta of the tool calibraton optimization.
		/// </summary>
		public double Delta
        {
            get { return _delta; }
			set { _delta = value; }
        }

		/// <summary>
		/// Gets the x-coordindate of the tool center point.
		/// </summary>
		public double X
        {
            get { return _x; }
        }

		/// <summary>
		/// Gets the y-coordindate of the tool center point.
		/// </summary>
		public double Y
		{
			get { return _y; }
		}

		/// <summary>
		/// Gets the z-coordindate of the tool center point.
		/// </summary>
		public double Z
		{
			get { return _z; }
		}

		/// <summary>
		/// Gets the tool center point.
		/// </summary>
		public Point3d TcpPoint
        {
			get { return new Point3d(_x, _y, _z); }
        }

		/// <summary>
		/// Gets the target point in world coordinate space. 
		/// This is the average point that is obtained from the given Robot Joint Positions and the calculated TCP.
		/// </summary>
		public Point3d TargetPoint
        {
            get { return _averagePoint; }
        }

		/// <summary>
		/// Gets the resulting errors in x-direction for each given Joint Position.
		/// </summary>
		public double[] ErrorsX
        {
            get { return _errorsX; }
        }

		/// <summary>
		/// Gets the resulting errors in y-direction for each given Joint Position.
		/// </summary>
		public double[] ErrorsY
		{
			get { return _errorsY; }
		}

		/// <summary>
		/// Gets the resulting errors in z-direction for each given Joint Position.
		/// </summary>
		public double[] ErrorsZ
		{
			get { return _errorsZ; }
		}

		/// <summary>
		/// Gets the maximum errors in the x, y and z direction as a vector. 
		/// </summary>
		public Vector3d MaximumError
        {
            get { return new Vector3d(_errorsX.Max(), _errorsY.Max(), _errorsZ.Max()); }
        }
		#endregion
	}
}
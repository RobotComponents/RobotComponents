// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.Definitions;
using RobotComponents.Utils;

namespace RobotComponents.Actions
{
    /// <summary>
    /// Robot Target class, defines robot target data. The robt target data is used to define the position of the robot and external axes.
    /// </summary>
    public class RobotTarget : Action, ITarget
    {
        #region fields
        private string _name; // robot target variable name
        private Plane _plane; // target plane (defines the required position and orientation of the tool)
        private Quaternion _quat; // target plane orientation (as quarternion)
        private int _axisConfig; // the axis configuration of the robot 
        private ExternalJointPosition _externalJointPosition; // the position of the external axes
        #endregion

        #region constructors
        /// <summary>
        /// Defines an empty Robot Target object.
        /// </summary>
        public RobotTarget()
        {
        }

        /// <summary>
        /// Defines a robot target with default axis configuration.
        /// </summary>
        /// <param name="name">Robot target name, must be unique.</param>
        /// <param name="plane">Robot target plane.</param>
        public RobotTarget(string name, Plane plane)
        {
            _name = name;
            _plane = plane;
            _axisConfig = 0;
            _externalJointPosition = new ExternalJointPosition();
            _quat = HelperMethods.PlaneToQuaternion(_plane);
        }

        /// <summary>
        /// Defines a robot target with a user defined axis configuration.
        /// </summary>
        /// <param name="name">Robot target name, must be unique.</param>
        /// <param name="plane">Robot target plane.</param>
        /// <param name="axisConfig">Robot axis configuration as a number (0-7).</param>
        public RobotTarget(string name, Plane plane, int axisConfig)
        {
            _name = name;
            _plane = plane;
            _axisConfig = axisConfig;
            _externalJointPosition = new ExternalJointPosition();
            _quat = HelperMethods.PlaneToQuaternion(_plane);
        }

        /// <summary>
        /// Defines a robot target that will be re-oriented from the reference coordinate system to the global coordinate system.
        /// </summary>
        /// <param name="name">Robot target name, must be unique.</param>
        /// <param name="plane">Robot target plane.</param>
        /// <param name="referencePlane">Reference plane. Target planes will be reoriented from this plane to the origon (WorldXY). </param>
        /// <param name="axisConfig">Robot axis configuration as a number (0-7).</param>
        public RobotTarget(string name, Plane plane, Plane referencePlane, int axisConfig)
        {
            _name = name;
            _plane = plane;            
            _axisConfig = axisConfig;
            _externalJointPosition = new ExternalJointPosition();
            _quat = HelperMethods.PlaneToQuaternion(referencePlane, _plane);

            // Re-orient the plane from the reference coordinate system to the world coordinate system
            Transform orient = Transform.PlaneToPlane(referencePlane, Plane.WorldXY);
            _plane.Transform(orient);
        }

        /// <summary>
        /// Defines a robot target with user defined (override) external axis values. 
        /// </summary>
        /// <param name="name">Robot target name, must be unique.</param>
        /// <param name="plane">Robot target plane.</param>
        /// <param name="referencePlane">Reference plane. Target planes will be reoriented from this plane to the origin (WorldXY). </param>
        /// <param name="axisConfig">Robot axis configuration as a number (0-7).</param>
        /// <param name="Eax_a"> The position of the external logical axis “a” expressed in degrees or mm. </param>
        /// <param name="Eax_b"> The position of the external logical axis “b” expressed in degrees or mm. </param>
        /// <param name="Eax_c"> The position of the external logical axis “c” expressed in degrees or mm. </param>
        /// <param name="Eax_d"> The position of the external logical axis “d” expressed in degrees or mm. </param>
        /// <param name="Eax_e"> The position of the external logical axis “e” expressed in degrees or mm. </param>
        /// <param name="Eax_f"> The position of the external logical axis “f” expressed in degrees or mm. </param>
        public RobotTarget(string name, Plane plane, Plane referencePlane, int axisConfig, double Eax_a, double Eax_b = 9e9, double Eax_c = 9e9, double Eax_d = 9e9, double Eax_e = 9e9, double Eax_f = 9e9)
        {
            _name = name;
            _plane = plane;
            _axisConfig = axisConfig;
            _externalJointPosition = new ExternalJointPosition(Eax_a, Eax_b, Eax_c, Eax_d, Eax_e, Eax_f);
            _quat = HelperMethods.PlaneToQuaternion(referencePlane, _plane);

            // Re-orient the plane from the reference coordinate system to the world coordinate system
            Transform orient = Transform.PlaneToPlane(referencePlane, Plane.WorldXY);
            _plane.Transform(orient);
        }

        /// <summary>
        /// Defines a robot target with user defined (override) external axis values. 
        /// </summary>
        /// <param name="name">Robot target name, must be unique.</param>
        /// <param name="plane">Robot target plane.</param>
        /// <param name="axisConfig">Robot target axis configuration as a number (0-7).</param>
        /// <param name="Eax">The user defined external joint positions as a list with axis values.</param>
        public RobotTarget(string name, Plane plane, int axisConfig, List<double> Eax)
        {
            _name = name;
            _plane = plane;
            _axisConfig = axisConfig;
            _externalJointPosition = new ExternalJointPosition(Eax);
            _quat = HelperMethods.PlaneToQuaternion(_plane);
        }

        /// <summary>
        /// Defines a robot target with a user defined external joint position.
        /// </summary>
        /// <param name="name">Robot target name, must be unique.</param>
        /// <param name="plane">Robot target plane.</param>
        /// <param name="axisConfig">Robot target axis configuration as a number (0-7).</param>
        /// <param name="externalJointPosition">The user defined external joint position.</param>
        public RobotTarget(string name, Plane plane, int axisConfig, ExternalJointPosition externalJointPosition)
        {
            _name = name;
            _plane = plane;
            _axisConfig = axisConfig;
            _externalJointPosition = externalJointPosition;
            _quat = HelperMethods.PlaneToQuaternion(_plane);
        }

        /// <summary>
        /// Defines a robot target with user defined (override) external axis values.
        /// Target planes will be re-oriented from the reference coordinate system to the global coordinate system.
        /// </summary>
        /// <param name="name">Robot target name, must be unique.</param>
        /// <param name="plane">Robot target plane.</param>
        /// <param name="referencePlane">Reference plane. Target planes will be reoriented from this plane to the origon (WorldXY). </param>
        /// <param name="axisConfig">Robot target axis configuration as a number (0-7).</param>
        /// <param name="Eax">The user defined external axis values as a list.</param>
        public RobotTarget(string name, Plane plane, Plane referencePlane, int axisConfig, List<double> Eax)
        {
            _name = name;
            _plane = plane;
            _axisConfig = axisConfig;
            _externalJointPosition = new ExternalJointPosition(Eax);
            _quat = HelperMethods.PlaneToQuaternion(referencePlane, _plane);

            // Re-orient the plane from the reference coordinate system to the world coordinate system
            Transform orient = Transform.PlaneToPlane(referencePlane, Plane.WorldXY);
            _plane.Transform(orient);
        }

        /// <summary>
        /// Defines a robot target with user defined (override) external axis values.
        /// Target planes will be re-oriented from the reference coordinate system to the global coordinate system.
        /// </summary>
        /// <param name="name">Robot target name, must be unique.</param>
        /// <param name="plane">Robot target plane.</param>
        /// <param name="referencePlane">Reference plane. Target planes will be reoriented from this plane to the origon (WorldXY). </param>
        /// <param name="axisConfig">Robot target axis configuration as a number (0-7).</param>
        /// <param name="Eax">The user defined external axis values as an array.</param>
        public RobotTarget(string name, Plane plane, Plane referencePlane, int axisConfig, double[] Eax)
        {
            _name = name;
            _plane = plane;
            _axisConfig = axisConfig;
            _externalJointPosition = new ExternalJointPosition(Eax);
            _quat = HelperMethods.PlaneToQuaternion(referencePlane, _plane);

            // Re-orient the plane to the reference plane
            Transform orient = Transform.PlaneToPlane(referencePlane, Plane.WorldXY);
            _plane.Transform(orient);
        }


        /// <summary>
        /// Defines a robot target with a user defined external joint position.
        /// Target planes will be re-oriented from the reference coordinate system to the global coordinate system.
        /// </summary>
        /// <param name="name">Robot target name, must be unique.</param>
        /// <param name="plane">Robot target plane.</param>
        /// <param name="referencePlane">Reference plane. Target planes will be reoriented from this plane to the origon (WorldXY). </param>
        /// <param name="axisConfig">Robot target axis configuration as a number (0-7).</param>
        /// <param name="externalJointPosition">The user defined external joint position.</param>
        public RobotTarget(string name, Plane plane, Plane referencePlane, int axisConfig, ExternalJointPosition externalJointPosition)
        {
            _name = name;
            _plane = plane;
            _axisConfig = axisConfig;
            _externalJointPosition = externalJointPosition;
            _quat = HelperMethods.PlaneToQuaternion(referencePlane, _plane);

            // Re-orient the plane to the reference plane
            Transform orient = Transform.PlaneToPlane(referencePlane, Plane.WorldXY);
            _plane.Transform(orient);
        }

        /// <summary>
        /// Creates a new target by duplicating an existing target. 
        /// This creates a deep copy of the existing target. 
        /// </summary>
        /// <param name="target"> The target that should be duplicated. </param>
        public RobotTarget(RobotTarget target)
        {
            _name = target.Name;
            _plane = new Plane(target.Plane);
            _axisConfig = target.AxisConfig;
            _externalJointPosition = target.ExternalJointPosition;
            _quat = target.Quat;
        }

        /// <summary>
        /// Method to duplicate the Target object.
        /// </summary>
        /// <returns>Returns a deep copy of the Target object.</returns>
        public RobotTarget Duplicate()
        {
            return new RobotTarget(this);
        }

        /// <summary>
        /// A method to duplicate the Robot Target object to an ITarget object. 
        /// </summary>
        /// <returns> Returns a deep copy of the Robot Target object as an ITarget object. </returns>
        public ITarget DuplicateTarget()
        {
            return new RobotTarget(this) as ITarget;
        }

        /// <summary>
        /// A method to duplicate the Target object to an Action object. 
        /// </summary>
        /// <returns> Returns a deep copy of the Target object as an Action object. </returns>
        public override Action DuplicateAction()
        {
            return new RobotTarget(this) as Action;
        }
        #endregion

        #region method
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns> A string that represents the current object. </returns>
        public override string ToString()
        {
            if (!this.IsValid)
            {
                return "Invalid Robot Target";
            }
            else
            {
                return "Robot Target (" + this.Name + ")";
            }
        }

        /// <summary>
        /// Used to create variable definition code of this action. 
        /// </summary>
        /// <param name="robot"> Defines the Robot were the code is generated for. </param>
        /// <returns> Returns the RAPID code line as a string. </returns>
        public override string ToRAPIDDeclaration(Robot robot)
        {
            string code = "VAR robtarget "; 

            code += _name;
            code += " := [";
            code += "[" + _plane.Origin.X.ToString("0.##") + ", ";           
            code += _plane.Origin.Y.ToString("0.##") + ", ";
            code += _plane.Origin.Z.ToString("0.##") + "]";
            code += ", ";
            code += "[" + _quat.A.ToString("0.######") + ", ";
            code += _quat.B.ToString("0.######") + ", ";
            code += _quat.C.ToString("0.######") + ", ";
            code += _quat.D.ToString("0.######") + "]";
            code += ", ";
            code += "[0,0,0," + _axisConfig + "]";
            code += ", ";
            code += _externalJointPosition.ToRAPIDDeclaration(robot); //TODO: Use the external axis values one from the IK of the robot? 
            code += "];";

            return code;
        }

        /// <summary>
        /// Used to create action instruction code line. 
        /// </summary>
        /// <param name="robot"> Defines the Robot were the code is generated for. </param>
        /// <returns> Returns the RAPID code line as a string. </returns>
        public override string ToRAPIDInstruction(Robot robot)
        {
            return string.Empty;
        }

        /// <summary>
        /// Used to create variable definitions in the RAPID Code. It is typically called inside the CreateRAPIDCode() method of the RAPIDGenerator class.
        /// </summary>
        /// <param name="RAPIDGenerator"> Defines the RAPIDGenerator. </param>
        public override void ToRAPIDDeclaration(RAPIDGenerator RAPIDGenerator)
        {
            // Only adds target code if target is not already defined
            if (!RAPIDGenerator.Targets.ContainsKey(_name))
            {
                // Add to dictionary
                RAPIDGenerator.Targets.Add(_name, this);

                // Generate code
                string code = "VAR robtarget ";
                code += _name;
                code += " := [";
                code += "[" + _plane.Origin.X.ToString("0.##") + ", ";
                code += _plane.Origin.Y.ToString("0.##") + ", ";
                code += _plane.Origin.Z.ToString("0.##") + "]";
                code += ", ";
                code += "[" + _quat.A.ToString("0.######") + ", ";
                code += _quat.B.ToString("0.######") + ", ";
                code += _quat.C.ToString("0.######") + ", ";
                code += _quat.D.ToString("0.######") + "]";
                code += ", ";
                code += "[0,0,0," + _axisConfig + "]";
                code += ", ";
                code += RAPIDGenerator.Robot.InverseKinematics.ExternalJointPosition.ToRAPIDDeclaration(RAPIDGenerator.Robot);
                code += "];";

                // Add to stringbuilder
                RAPIDGenerator.StringBuilder.Append(Environment.NewLine + "\t" + code);
            }
        }

        /// <summary>
        /// Used to create action instructions in the RAPID Code. It is typically called inside the CreateRAPIDCode() method of the RAPIDGenerator class.
        /// </summary>
        /// <param name="RAPIDGenerator"> Defines the RAPIDGenerator. </param>
        public override void ToRAPIDInstruction(RAPIDGenerator RAPIDGenerator)
        {
        }
        #endregion

        #region properties
        /// <summary>
        /// A boolean that indicuate if the Robot Target object is valid.
        /// </summary>
        public override bool IsValid
        {
            get
            {
                if (Name == null) { return false; }
                if (Name == "") { return false; }
                if (Plane == null) { return false; }
                if (Plane == Plane.Unset) { return false; }
                if (Name == null) { return false; }
                if (Name == "") { return false; }
                if (AxisConfig < 0) { return false; }
                if (AxisConfig > 7) { return false; }
                if (ExternalJointPosition.IsValid == false) { return false; }
                return true;
            }
        }
        
        /// <summary>
        /// The robot target variable name, must be unique.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// The position and orientation of the tool center as a plane. 
        /// </summary>
        public Plane Plane
        {
            get 
            { 
                return _plane; 
            }
            set 
            { 
                _plane = value;
                _quat = HelperMethods.PlaneToQuaternion(_plane);
            }
        }

        /// <summary>
        /// The orientation of the tool, expressed in the form of a quaternion (q1, q2, q3, and q4). 
        /// </summary>
        public Quaternion Quat
        {
            get 
            { 
                return _quat; 
            }
            set 
            { 
                _quat = value; 
                _plane = HelperMethods.QuaternionToPlane(_plane.Origin, _quat);
            }
        }

        /// <summary>
        /// The axis configuration of the robot (0-7).
        /// </summary>
        public int AxisConfig
        {
            get { return _axisConfig; }
            set { _axisConfig = value; }
        }

        /// <summary>
        /// Defines the External Joint Position
        /// </summary>
        public ExternalJointPosition ExternalJointPosition
        {
            get { return _externalJointPosition; }
            set { _externalJointPosition = value; }
        }
        #endregion
    }

}

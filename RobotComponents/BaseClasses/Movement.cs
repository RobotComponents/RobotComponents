using System.Collections.Generic;
using System;

namespace RobotComponents.BaseClasses
{
    /// <summary>
    /// Movement class
    /// </summary>
    public class Movement : Action
    {
        #region fields
        private Target _target;
        private SpeedData _speedData;
        int _movementType;
        int _precision;
        Guid _documentGUID;
        ObjectManager _objectManager;
        #endregion

        #region constructors
        public Movement()
        {
        }

        /// <summary>
        /// Defines a robot movement.
        /// </summary>
        /// <param name="target">Robot movement target.</param>
        /// <param name="speedData">Robot movement speedData.</param>
        /// <param name="movementType">Robot movement type.</param>
        /// <param name="precision">Robot movement Precision. If this value is 0 the robot will go to exactly the specified position. This means its ZoneData in RAPID code is set to fine.</param>
        public Movement(Target target, SpeedData speedData, int movementType, int precision, Guid documentGUID)
        {
            this._target = target;
            this._speedData = speedData;
            this._movementType = movementType;
            this._precision = precision;
            this._documentGUID = documentGUID;


            // Checks if ObjectManager for this document already exists. If not it creates a new one
            if (!DocumentManager.ObjectManagers.ContainsKey(_documentGUID))
            {
                DocumentManager.ObjectManagers.Add(_documentGUID, new ObjectManager());
            }

            // Gets ObjectManager of this document
            _objectManager = DocumentManager.ObjectManagers[_documentGUID];
        }

        /// <summary>
        /// Defines a robot movement with fine precision.
        /// </summary>
        /// <param name="target">Robot movement target.</param>
        /// <param name="speedData">Robot movement speedData.</param>
        /// <param name="movementType">Robot movement type.</param>
        public Movement(Target target, SpeedData speedData, int movementType, Guid documentGUID)
        {
            this._target = target;
            this._speedData = speedData;
            this._movementType = movementType;
            this._precision = 0;
            this._documentGUID = documentGUID;


            // Checks if ObjectManager for this document already exists. If not it creates a new one
            if (!DocumentManager.ObjectManagers.ContainsKey(_documentGUID))
            {
                DocumentManager.ObjectManagers.Add(_documentGUID, new ObjectManager());
            }

            // Gets ObjectManager of this document
            _objectManager = DocumentManager.ObjectManagers[_documentGUID];
        }

        /// <summary>
        /// Duplicates a robot movement.
        /// </summary>
        /// <returns></returns>
        public Movement Duplicate()
        {
            Movement dup = new Movement(Target, SpeedData, MovementType, Precision, DocumentGUID);
            return dup;
        }
        #endregion

        #region method
        public override string InitRAPIDVar(RobotInfo robotInfo, string RAPIDcode)
        {
            string tempCode = "";

            // Creates Speed Data Variable Code
            string speedDataCode = _speedData.InitRAPIDVar();

            // Only adds speedData Variable if not already in RAPID Code
            if (!RAPIDcode.Contains(speedDataCode))
            {
                tempCode += speedDataCode;
            }

            // Creates targetName variables to check if they already exist 
            string robTargetVar = "VAR robtarget " + _target.RobTargetName;
            string jointTargetVar = "CONST jointtarget " + _target.JointTargetName;

            // Create a robtarget if  the movement type is MoveL (1) or MoveJ (2)
            if (MovementType == 1 || MovementType == 2)
            {
                // Only adds target code if target is not already defined
                if (!RAPIDcode.Contains(robTargetVar))
                {
                    tempCode += ("@" + "\t" + robTargetVar + ":=[[" 
                        + _target.Plane.Origin.X.ToString("0.##") + ", " 
                        + _target.Plane.Origin.Y.ToString("0.##") + ", " 
                        + _target.Plane.Origin.Z.ToString("0.##") + "], ["
                        + _target.Quat.A.ToString("0.######") + ", " 
                        + _target.Quat.B.ToString("0.######") + ", " 
                        + _target.Quat.C.ToString("0.######") + ", " 
                        + _target.Quat.D.ToString("0.######") + "]," 
                        + "[0,0,0," + _target.AxisConfig);

                    // Adds all External Axis Values
                    InverseKinematics inverseKinematics = new InverseKinematics(_target, robotInfo);
                    inverseKinematics.Calculate();
                    List<double> externalAxisValues = inverseKinematics.ExternalAxisValues;
                    tempCode += "], [";
                    for (int i = 0; i < externalAxisValues.Count; i++)
                    {
                        tempCode += externalAxisValues[i].ToString("0.##") + ", ";
                    }

                    // Adds 9E9 for all missing external Axis Values
                    for (int i = externalAxisValues.Count; i < 6; i++)
                    {
                        tempCode += "9E9" + ", ";
                    }
                    tempCode = tempCode.Remove(tempCode.Length - 2);
                    tempCode += "]];";
                }
            }

            // Create a jointtarget if the movement type is MoveAbsJ (0)
            else
            {
                // Only adds target code if target is not already defined
                if (!RAPIDcode.Contains(jointTargetVar))
                {
                    // Calculates AxisValues
                    InverseKinematics inverseKinematics = new InverseKinematics(_target, robotInfo);
                    inverseKinematics.Calculate();
                    List<double> internalAxisValues = inverseKinematics.InternalAxisValues;
                    List<double> externalAxisValues = inverseKinematics.ExternalAxisValues;

                    // Creates Code Variable
                    tempCode += "@" + "\t" + jointTargetVar + ":=[[";

                    // Adds all Internal Axis Values
                    for (int i = 0; i < internalAxisValues.Count; i++)
                    {
                        tempCode += internalAxisValues[i].ToString("0.##") + ", ";
                    }
                    tempCode = tempCode.Remove(tempCode.Length - 2);

                    // Adds all External Axis Values
                    tempCode += "], [";
                    for (int i = 0; i < externalAxisValues.Count; i++)
                    {
                        tempCode += externalAxisValues[i].ToString("0.##") + ", ";
                    }
                    // Adds 9E9 for all missing external Axis Values
                    for (int i = externalAxisValues.Count; i < 6; i++)
                    {
                        tempCode += "9E9" + ", ";
                    }
                    tempCode = tempCode.Remove(tempCode.Length - 2);
                    tempCode += "]];";
                }
            }

            // returns RAPID code
            return tempCode;
        }

        public override string ToRAPIDFunction()
        {
            // MoveAbsJ
            if (MovementType == 0)
            {
                if (_precision < 0)
                {
                    return ("@" + "\t" + "MoveAbsJ " + _target.JointTargetName + @", " + _speedData.Name + ", fine, " + _objectManager.CurrentTool + "\\WObj:=wobj0;");
                }
                else
                {
                    return ("@" + "\t" + "MoveAbsJ " + _target.JointTargetName + @", " + _speedData.Name + ", z" + Precision.ToString() + ", " + _objectManager.CurrentTool + "\\WObj:=wobj0;");
                }
            }
            // MoveL
            else if (MovementType == 1)
            {
                if (_precision < 0)
                {
                    return ("@" + "\t" + "MoveL " + _target.RobTargetName + @", " + _speedData.Name + ", fine, " + _objectManager.CurrentTool + "\\WObj:=wobj0;");
                }
                else
                {
                    return ("@" + "\t" + "MoveL " + _target.RobTargetName + @", " + _speedData.Name + ", z" + Precision.ToString() + ", " + _objectManager.CurrentTool + "\\WObj:=wobj0;");
                }
            }
            // MoveJ
            else if (MovementType == 2)
            {
                if (_precision < 0)
                {
                    return ("@" + "\t" + "MoveJ " + _target.RobTargetName + @", " + _speedData.Name + ", fine, " + _objectManager.CurrentTool + "\\WObj:=wobj0;");
                }
                else
                {
                    return ("@" + "\t" + "MoveJ " + _target.RobTargetName + @", " + _speedData.Name + ", z" + Precision.ToString() + ", " + _objectManager.CurrentTool + "\\WObj:=wobj0;");
                }
            }
            // Return nothing if a wrong movement type is used
            else
            {
                return "";
            }
        }
        #endregion

        #region properties
        public bool IsValid
        {
            get
            {
                if (Target == null) { return false; }
                if (SpeedData == null) { return false; }
                return true;
            }
        }
        public Target Target
        {
            get { return _target; }
            set { _target = value; }
        }
        public SpeedData SpeedData
        {
            get { return _speedData; }
            set { _speedData = value; }
        }
        public int MovementType
        {
            get { return _movementType; }
            set { _movementType = value; }
        }
        public int Precision
        {
            get { return _precision; }
            set { _precision = value; }
        }

        public Guid DocumentGUID
        {
            get => _documentGUID;
            set => _documentGUID = value;
        }

        public ObjectManager ObjectManager
        {
            get => _objectManager;
            set => _objectManager = value;
        }
        #endregion
    }
}

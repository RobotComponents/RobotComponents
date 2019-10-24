using System;

namespace RobotComponents.BaseClasses
{
    public class SpeedData : Action
    {
        #region fields
        private string _name;
        private double _v_tcp;
        private double _v_ori;
        private double _v_leax;
        private double _v_reax;
        private bool _predefined; // ABB predefinied data: e.g. v5, v10, v20, v30
        #endregion

        #region constructors
        public SpeedData()
        {
        }
        public SpeedData(double v_tcp)
        {
            double tcp = Math.Round(v_tcp, 0);
            this._name = "v" + tcp.ToString();
            this._v_tcp = tcp;
            this._v_ori = 500;
            this._v_leax = 5000;
            this._v_reax = 1000;
            this._predefined = true;
        }
        public SpeedData(string name, double v_tcp, double v_ori, double v_leax, double v_reax)
        {
            this._name = name;
            this._v_tcp = v_tcp;
            this._v_ori = v_ori;
            this._v_leax = v_leax;
            this._v_reax = v_reax;
            this._predefined = false;
        }
        public SpeedData(string name, double v_tcp, double v_ori, double v_leax, double v_reax, bool predefined)
        {
            this._name = name;
            this._v_tcp = v_tcp;
            this._v_ori = v_ori;
            this._v_leax = v_leax;
            this._v_reax = v_reax;
            this._predefined = predefined;
        }

        /// <summary>
        /// Duplicates a robot movement.
        /// </summary>
        /// <returns></returns>
        public SpeedData Duplicate()
        {
            SpeedData dup = new SpeedData(Name, V_TCP, V_ORI, V_LEAX, V_REAX, PreDefinied);
            return dup;
        }
        #endregion

        #region properties
        public bool IsValid
        {
            get
            {
                if (Name == "") { return false; }
                if (V_TCP <= 0) { return false; }
                if (V_ORI <= 0) { return false; }
                if (V_LEAX <= 0) { return false; }
                if (V_REAX <= 0) { return false; }
                return true;
            }
        }
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public double V_TCP
        {
            get { return _v_tcp; }
            set { _v_tcp = value; }
        }
        public double V_ORI
        {
            get { return _v_ori; }
            set { _v_ori = value; }
        }
        public double V_LEAX
        {
            get { return _v_leax; }
            set { _v_leax = value; }
        }
        public double V_REAX
        {
            get { return _v_reax; }
            set { _v_reax = value; }
        }
        public bool PreDefinied
        {
            get { return _predefined; }
            set { _predefined = value; }
        }
        #endregion

        #region method
        // never used in RAPID Generator
        public override string InitRAPIDVar(RobotInfo robotInfo, string RAPIDcode)
        {
            if (_predefined == false) 
            {
                return ("@" + "\t" + "VAR speeddata " + _name + ":= [" + _v_tcp + ", " + _v_ori + ", " + _v_leax + ", " + _v_reax + "];");
            }
            else
            {
                return "";
            }
        }

        // uses instead this method in InitRAPIDVar method of movement class
        public string InitRAPIDVar()
        {
            if (_predefined == false)
            {
                return ("@" + "\t" + "VAR speeddata " + _name + ":= [" + _v_tcp + ", " + _v_ori + ", " + _v_leax + ", " + _v_reax + "];");
            }
            else
            {
                return "";
            }
        }

        public override string ToRAPIDFunction()
        {
            return "";
        }
        #endregion

    }
}

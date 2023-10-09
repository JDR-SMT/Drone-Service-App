using System.Text.RegularExpressions;

namespace DroneServiceApp
{
    public class Drone
    {
        // private variables
        private string _name;
        private string _drone;
        private string _problem;
        private decimal _cost;
        private int _tag;

        #region Getters & Setters
        public string GetName()
        {
            return _name;
        }

        public void SetName(string aName)
        {
            // title case
            _name = Regex.Replace(aName, @"\b[a-z]", match => match.Value.ToUpper());
        }

        public string GetDrone()
        {
            return _drone;
        }

        public void SetDrone(string aDrone)
        {
            _drone = aDrone;
        }

        public string GetProblem()
        {
            return _problem;
        }

        public void SetProblem(string aProblem)
        {
            // sentence case
            _problem = Regex.Replace(aProblem, @"(^[a-z])|\.\s+(.)", match => match.Value.ToUpper());
        }

        public decimal GetCost()
        {
            return _cost;
        }

        public void SetCost(decimal aCost)
        {
            _cost = aCost;
        }

        public int getTag()
        {
            return _tag;
        }

        public void SetTag(int aTag)
        {
            _tag = aTag;
        }
        #endregion

        // display name and cost
        override
        public string ToString()
        {
            return GetName() + " $" + GetCost();
        }
    }
}

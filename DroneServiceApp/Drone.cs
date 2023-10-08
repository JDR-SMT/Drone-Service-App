using System.Text.RegularExpressions;

namespace DroneServiceApp
{
    public class Drone
    {
        // private variables
        private string name;
        private string drone;
        private string problem;
        private decimal cost;
        private int tag;

        #region Getters & Setters
        public string GetName() { return name; }
        public void SetName(string aName) { name = Regex.Replace(aName, @"\b[a-z]", match => match.Value.ToUpper()); } // title case
        public string GetDrone() { return drone; }
        public void SetDrone(string aDrone) { drone = aDrone; }
        public string GetProblem() { return problem; }
        public void SetProblem(string aProblem) { problem = Regex.Replace(aProblem, @"(^[a-z])|\.\s+(.)", match => match.Value.ToUpper()); } // sentence case
        public decimal GetCost() { return cost; }
        public void SetCost(decimal aCost) { cost = aCost; }
        public int getTag() { return tag; }
        public void SetTag(int aTag) { tag = aTag; }
        #endregion

        // display name and cost
        public string Display()
        {
            return GetName() + " $" + GetCost();
        }
    }
}

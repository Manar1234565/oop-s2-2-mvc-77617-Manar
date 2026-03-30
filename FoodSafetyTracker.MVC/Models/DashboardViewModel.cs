namespace FoodSafetyTracker.MVC.Models
{
    public class DashboardViewModel
    {
        public int InspectionsThisMonth { get; set; }
        public int FailedInspectionsThisMonth { get; set; }
        public int OverdueFollowUps { get; set; }

        public string TownFilter { get; set; }
        public string RiskFilter { get; set; }
    }
}
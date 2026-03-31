using System;

namespace FoodSafetyTracker.Domain
{
    public class FollowUp
    {
        public int Id { get; set; }

        //  FK
        public int InspectionId { get; set; }

        public DateTime DueDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime? ClosedDate { get; set; }

        //  nullable
        public Inspection? Inspection { get; set; }
    }
}
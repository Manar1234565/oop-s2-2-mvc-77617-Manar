using System;

namespace FoodSafetyTracker.Domain
{
    public class FollowUp
    {
        public int Id { get; set; }

        // 🔹 FK
        public int InspectionId { get; set; }

        public DateTime DueDate { get; set; }
        public string Status { get; set; } = "";
        public DateTime? ClosedDate { get; set; }

        // 🔹 RELACIÓN (at the end)
        public Inspection Inspection { get; set; }
    }
}
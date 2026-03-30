using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // 🔥 AÑADE ESTO

namespace FoodSafetyTracker.Domain
{
    public class Inspection
    {
        public int Id { get; set; }

        // 🔹 FK
        public int PremisesId { get; set; }

        public DateTime InspectionDate { get; set; }

        [Range(0, 99, ErrorMessage = "Score must be between 0 and 99")] // 🔥 ESTO ES LO IMPORTANTE
        public int Score { get; set; }

        public string Outcome { get; set; } = "";
        public string Notes { get; set; } = "";

        // 🔹 RELACIONES
        public Premises Premises { get; set; }
        public List<FollowUp> FollowUps { get; set; } = new();
    }
}
using FoodSafetyTracker.Domain;
using Microsoft.AspNetCore.Identity;

namespace FoodSafetyTracker.MVC.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<AppDbContext>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            context.Database.EnsureCreated();

            //  AVOID DUPLICATE DATA
            if (context.Premises.Any())
                return;

            // =========================
            // ROLES
            // =========================
            string[] roles = { "Admin", "Inspector", "Viewer" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // =========================
            // 👤 ADMIN USER
            // =========================
            var adminEmail = "manar@admin.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var user = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(user, "Admin123!");
                await userManager.AddToRoleAsync(user, "Admin");
            }

            // =========================
            // 🟢 PREMISES
            // =========================
            var premises = new List<Premises>
            {
                new Premises { Name = "Temple Bar Cafe", Address="Temple Bar", Town="Dublin", RiskRating="High" },
                new Premises { Name = "Guinness Storehouse Restaurant", Address="St James's Gate", Town="Dublin", RiskRating="Medium" },
                new Premises { Name = "Avoca Cafe", Address="Suffolk Street", Town="Dublin", RiskRating="Low" },
                new Premises { Name = "Leo Burdock Fish & Chips", Address="Christchurch", Town="Dublin", RiskRating="High" },
                new Premises { Name = "The Woollen Mills", Address="Ormond Quay", Town="Dublin", RiskRating="Medium" },

                new Premises { Name = "Market Lane", Address="Oliver Plunkett St", Town="Cork", RiskRating="Medium" },
                new Premises { Name = "Electric Cork", Address="South Mall", Town="Cork", RiskRating="High" },

                new Premises { Name = "Ard Bia at Nimmos", Address="Spanish Arch", Town="Galway", RiskRating="Low" },
                new Premises { Name = "McDonagh's", Address="Quay Street", Town="Galway", RiskRating="High" },

                new Premises { Name = "The House Limerick", Address="O'Connell Street", Town="Limerick", RiskRating="Medium" },
                new Premises { Name = "Freddy’s Bistro", Address="Catherine Street", Town="Limerick", RiskRating="Low" },

                new Premises { Name = "The Fat Fox", Address="Kildare Town", Town="Kildare", RiskRating="Medium" }
            };

            context.Premises.AddRange(premises);
            context.SaveChanges();

            // =========================
            // 🟢 INSPECTIONS
            // =========================
            var inspections = new List<Inspection>();
            var rand = new Random();

            foreach (var p in context.Premises.ToList())
            {
                for (int i = 0; i < 2; i++)
                {
                    var score = rand.Next(50, 100);
                    inspections.Add(new Inspection
                    {
                        PremisesId = p.Id,
                        InspectionDate = DateTime.Now.AddDays(-rand.Next(1, 60)),
                        Score = score,
                        Outcome = score < 70 ? "Fail" : "Pass",
                        Notes = "Routine inspection"
                    });
                }
            }

            context.Inspections.AddRange(inspections);
            context.SaveChanges();

            // =========================
            // 🟢 FOLLOW UPS
            // =========================
            var followUps = new List<FollowUp>();

            var inspectionsList = context.Inspections.Take(10).ToList();

            foreach (var i in inspectionsList)
            {
                followUps.Add(new FollowUp
                {
                    InspectionId = i.Id,
                    DueDate = DateTime.Now.AddDays(-rand.Next(1, 10)),
                    Status = rand.Next(0, 2) == 0 ? "Open" : "Closed",
                    ClosedDate = null
                });
            }

            context.FollowUps.AddRange(followUps);
            context.SaveChanges();
        }
    }
}
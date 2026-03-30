using FoodSafetyTracker.Domain;
using Microsoft.EntityFrameworkCore;

namespace FoodSafetyTracker.MVC.Data
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Premises> Premises { get; set; }
        public DbSet<Inspection> Inspections { get; set; }
        public DbSet<FollowUp> FollowUps { get; set; }
    }
}
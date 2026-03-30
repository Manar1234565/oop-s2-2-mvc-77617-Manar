using FoodSafetyTracker.Domain;
using FoodSafetyTracker.MVC.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Xunit;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class Tests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public void OverdueFollowUps_ReturnsCorrectItems()
    {
        var context = GetDbContext();

        context.FollowUps.Add(new FollowUp
        {
            DueDate = DateTime.Now.AddDays(-5),
            Status = "Open"
        });

        context.SaveChanges();

        var result = context.FollowUps
            .Where(f => f.Status == "Open" && f.DueDate < DateTime.Now)
            .ToList();

        Assert.Single(result);
    }

    [Fact]
    public void FollowUp_CannotBeClosedWithoutClosedDate()
    {
        var followUp = new FollowUp
        {
            Status = "Closed",
            ClosedDate = null
        };

        bool isValid = followUp.Status != "Closed" || followUp.ClosedDate != null;

        Assert.False(isValid);
    }

    [Fact]
    public void DashboardCounts_AreCorrect()
    {
        var context = GetDbContext();

        var premises = new Premises { Name = "Test", Town = "Dublin", RiskRating = "High" };
        context.Premises.Add(premises);
        context.SaveChanges();

        context.Inspections.Add(new Inspection
        {
            PremisesId = premises.Id,
            InspectionDate = DateTime.Now,
            Score = 60,
            Outcome = "Fail"
        });

        context.SaveChanges();

        var count = context.Inspections.Count();

        Assert.Equal(1, count);
    }

    [Fact]
    public void InspectionScore_ValidRange()
    {
        var inspection = new Inspection
        {
            Score = 150
        };

        bool valid = inspection.Score >= 0 && inspection.Score <= 100;

        Assert.False(valid);
    }
}

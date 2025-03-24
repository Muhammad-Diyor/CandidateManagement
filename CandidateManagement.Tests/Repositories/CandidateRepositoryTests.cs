using Microsoft.EntityFrameworkCore;
using CandidateManagement.Infrastructure.Data;
using CandidateManagement.Infrastructure.Repositories;
using CandidateManagement.Domain.Entities;

namespace CandidateManagement.Tests.Repositories;

public class CandidateRepositoryTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnCandidate_WhenExists()
    {
        await using var db = GetDbContext();
        var repo = new CandidateRepository(db);
        var candidate = new Candidate { Email = "a@test.com", FirstName = "Test" };
        db.Candidates.Add(candidate);
        await db.SaveChangesAsync();

        var result = await repo.GetByEmailAsync("a@test.com");

        Assert.NotNull(result);
        Assert.Equal("Test", result!.FirstName);
    }
    
    [Fact]
    public async Task GetByEmailAsync_ShouldReturnNull_WhenEmailDoesNotExist()
    {
        await using var db = GetDbContext();
        var repo = new CandidateRepository(db);
        var candidate = new Candidate 
        {
            Email = "exists@test.com", 
            FirstName = "Test" 
        };
        db.Candidates.Add(candidate);
        await db.SaveChangesAsync();
    
        var result = await repo.GetByEmailAsync("doesnotexist@test.com");
    
        Assert.Null(result);
    }
}
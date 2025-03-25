using Moq;
using Xunit;
using CandidateManagement.Application.DTOs;
using CandidateManagement.Application.Repositories;
using CandidateManagement.Application.Caching;
using CandidateManagement.Infrastructure.Services;
using CandidateManagement.Domain.Entities;

namespace CandidateManagement.Tests.Services;

public class CandidateServiceTests
{
    [Fact]
    public async Task AddOrUpdateCandidateAsync_ShouldCreateNewCandidate_WhenEmailDoesNotExist()
    {
        var dto = new CandidateDto
        {
            FirstName = "Ali",
            LastName = "Valiyev",
            PhoneNumber = "123456",
            Email = "ali@example.com",
            CallStartTime = "08:00",
            CallEndTime = "09:00",
            LinkedInUrl = "https://linkedin.com/ali",
            GitHubUrl = "https://github.com/ali",
            Comment = "New candidate"
        };

        var repoMock = new Mock<ICandidateRepository>();
        var cacheMock = new Mock<ICacheService>();

        // No cache hit
        cacheMock.Setup(c => c.GetAsync<Candidate>(dto.Email)).ReturnsAsync((Candidate?)null);
        repoMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync((Candidate?)null);

        Candidate? added = null;
        repoMock.Setup(x => x.AddCandidateAsync(It.IsAny<Candidate>()))
            .Callback<Candidate>(c => added = c)
            .Returns(Task.CompletedTask);

        var service = new CandidateService(repoMock.Object, cacheMock.Object);

        var result = await service.AddOrUpdateCandidateAsync(dto);

        Assert.NotNull(added);
        Assert.Equal(dto.Email, result.Email);

        // Cache set should be called
        cacheMock.Verify(c => c.SetAsync(dto.Email, It.IsAny<Candidate>(), It.IsAny<TimeSpan>()), Times.Once);
    }

    [Fact]
    public async Task AddOrUpdateCandidateAsync_ShouldUpdate_WhenCandidateExists()
    {
        var existing = new Candidate
        {
            FirstName = "Old",
            LastName = "Name",
            Email = "update@example.com",
            PhoneNumber = "0000",
            CallStartTime = new TimeOnly(9, 0),
            CallEndTime = new TimeOnly(10, 0),
            LinkedInUrl = "old",
            GitHubUrl = "old",
            Comment = "old"
        };

        var dto = new CandidateDto
        {
            FirstName = "New",
            LastName = "Name",
            Email = existing.Email,
            CallStartTime = "11:00",
            CallEndTime = "12:00"
        };

        var repoMock = new Mock<ICandidateRepository>();
        var cacheMock = new Mock<ICacheService>();

        // No cache hit
        cacheMock.Setup(c => c.GetAsync<Candidate>(dto.Email)).ReturnsAsync((Candidate?)null);
        repoMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(existing);
        repoMock.Setup(r => r.UpdateCandidateAsync(It.IsAny<Candidate>())).Returns(Task.CompletedTask);

        var service = new CandidateService(repoMock.Object, cacheMock.Object);
        var result = await service.AddOrUpdateCandidateAsync(dto);

        Assert.Equal("New", result.FirstName);
        Assert.Equal(new TimeOnly(11, 0), result.CallStartTime);

        // Cache should be updated after update
        cacheMock.Verify(c => c.SetAsync(dto.Email, It.IsAny<Candidate>(), It.IsAny<TimeSpan>()), Times.Once);
    }

    [Fact]
    public async Task AddOrUpdateCandidateAsync_ShouldReturnFromCache_WhenCandidateIsCached()
    {
        var cachedCandidate = new Candidate
        {
            FirstName = "FromCache",
            LastName = "Test",
            Email = "cache@example.com"
        };

        var dto = new CandidateDto
        {
            Email = cachedCandidate.Email
        };

        var repoMock = new Mock<ICandidateRepository>();
        var cacheMock = new Mock<ICacheService>();

        cacheMock.Setup(c => c.GetAsync<Candidate>(dto.Email)).ReturnsAsync(cachedCandidate);

        var service = new CandidateService(repoMock.Object, cacheMock.Object);
        var result = await service.AddOrUpdateCandidateAsync(dto);

        Assert.Equal("FromCache", result.FirstName);

        // No DB calls
        repoMock.Verify(r => r.GetByEmailAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task AddOrUpdateCandidateAsync_ShouldUpdateOnlyProvidedFields_WhenUpdatingExistingCandidate()
    {
        var existing = new Candidate
        {
            FirstName = "Old",
            LastName = "Name",
            Email = "test@example.com",
            PhoneNumber = "1234567",
            LinkedInUrl = "https://linkedin.com/old",
            GitHubUrl = "https://github.com/old",
            Comment = "Old comment"
        };

        var dto = new CandidateDto
        {
            Email = existing.Email,
            FirstName = "New"
        };

        var repoMock = new Mock<ICandidateRepository>();
        var cacheMock = new Mock<ICacheService>();

        cacheMock.Setup(c => c.GetAsync<Candidate>(dto.Email)).ReturnsAsync((Candidate?)null);
        repoMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(existing);
        repoMock.Setup(r => r.UpdateCandidateAsync(It.IsAny<Candidate>())).Returns(Task.CompletedTask);

        var service = new CandidateService(repoMock.Object, cacheMock.Object);

        var result = await service.AddOrUpdateCandidateAsync(dto);

        Assert.Equal("New", result.FirstName);
        Assert.Equal("Name", result.LastName);
        Assert.Equal("1234567", result.PhoneNumber);

        cacheMock.Verify(c => c.SetAsync(dto.Email, It.IsAny<Candidate>(), It.IsAny<TimeSpan>()), Times.Once);
    }

    [Fact]
    public async Task AddOrUpdateCandidateAsync_ShouldThrowException_WhenTimeFormatInvalid()
    {
        var dto = new CandidateDto
        {
            FirstName = "Ali",
            LastName = "Test",
            Email = "a@test.com",
            CallStartTime = "not-a-time"
        };

        var repoMock = new Mock<ICandidateRepository>();
        var cacheMock = new Mock<ICacheService>();

        cacheMock.Setup(c => c.GetAsync<Candidate>(dto.Email)).ReturnsAsync((Candidate?)null);
        repoMock.Setup(x => x.GetByEmailAsync(dto.Email)).ReturnsAsync((Candidate?)null);

        var service = new CandidateService(repoMock.Object, cacheMock.Object);

        await Assert.ThrowsAsync<FormatException>(() =>
            service.AddOrUpdateCandidateAsync(dto));
    }
}
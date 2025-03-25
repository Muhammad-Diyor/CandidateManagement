using CandidateManagement.Application.Caching;
using CandidateManagement.Application.DTOs;
using CandidateManagement.Application.Repositories;
using CandidateManagement.Application.Services;
using CandidateManagement.Domain.Entities;

namespace CandidateManagement.Infrastructure.Services;

/// <summary>
/// Service responsible for managing candidate information with caching support.
/// Provides functionality to add or update candidate records while maintaining 
/// a caching layer for improved performance.
/// </summary>
/// <remarks>
/// This service implements a two-level lookup strategy:
/// 1. Check cache first
/// 2. Fallback to database if not in cache
/// 3. Update both cache and database on modifications
/// </remarks>
public class CandidateService : ICandidateService
{
    /// <summary>
    /// Repository for persistent candidate data storage.
    /// </summary>
    private readonly ICandidateRepository _candidateRepository;

    /// <summary>
    /// Caching service for temporary and quick candidate data retrieval.
    /// </summary>
    private readonly ICacheService _cacheService;

    /// <summary>
    /// Initializes a new instance of the CandidateService.
    /// </summary>
    /// <param name="candidateRepository">Repository for candidate data operations</param>
    /// <param name="cacheService">Caching service for performance optimization</param>
    public CandidateService(
        ICandidateRepository candidateRepository, 
        ICacheService cacheService)
    {
        // Validate input dependencies
        _candidateRepository = candidateRepository 
            ?? throw new ArgumentNullException(nameof(candidateRepository));
        _cacheService = cacheService 
            ?? throw new ArgumentNullException(nameof(cacheService));
    }

    /// <summary>
    /// Adds a new candidate or updates an existing candidate's information.
    /// </summary>
    /// <param name="dto">Data Transfer Object containing candidate information</param>
    /// <returns>The created or updated Candidate entity</returns>
    /// <remarks>
    /// Processing Order:
    /// 1. Check cache for existing candidate
    /// 2. If not in cache, check database
    /// 3. Update existing or create new candidate
    /// 4. Update cache with new/modified candidate
    /// </remarks>
    public async Task<Candidate> AddOrUpdateCandidateAsync(CandidateDto dto)
    {
        // Step 1: Check cache first for existing candidate
        var existingCandidate = await _cacheService.GetAsync<Candidate>(dto.Email);

        // Step 2: If not in cache, retrieve from database
        if (existingCandidate == null)
        {
            existingCandidate = await _candidateRepository.GetByEmailAsync(dto.Email);
        }

        // Existing candidate update logic
        if (existingCandidate != null)
        {
            // Selective update: only modify provided fields
            UpdateCandidateFields(existingCandidate, dto);

            // Persist changes to database
            await _candidateRepository.UpdateCandidateAsync(existingCandidate);

            // Update cache with modified candidate
            await _cacheService.SetAsync(
                existingCandidate.Email, 
                existingCandidate, 
                TimeSpan.FromMinutes(10)
            );

            return existingCandidate;
        }

        // Create new candidate if not exists
        var newCandidate = CreateNewCandidate(dto);

        // Save to database
        await _candidateRepository.AddCandidateAsync(newCandidate);

        // Cache the new candidate
        await _cacheService.SetAsync(
            newCandidate.Email, 
            newCandidate, 
            TimeSpan.FromMinutes(10)
        );

        return newCandidate;
    }

    /// <summary>
    /// Updates existing candidate fields with non-null values from DTO.
    /// </summary>
    /// <param name="candidate">Existing candidate to update</param>
    /// <param name="dto">DTO containing potential updates</param>
    private void UpdateCandidateFields(Candidate candidate, CandidateDto dto)
    {
        candidate.FirstName = string.IsNullOrWhiteSpace(dto.FirstName) 
            ? candidate.FirstName 
            : dto.FirstName;

        candidate.LastName = string.IsNullOrWhiteSpace(dto.LastName) 
            ? candidate.LastName 
            : dto.LastName;

        // Similar pattern for other fields...
        candidate.PhoneNumber = string.IsNullOrWhiteSpace(dto.PhoneNumber) 
            ? candidate.PhoneNumber 
            : dto.PhoneNumber;

        // Time parsing with null checks
        candidate.CallStartTime = string.IsNullOrWhiteSpace(dto.CallStartTime) 
            ? candidate.CallStartTime 
            : TimeOnly.Parse(dto.CallStartTime);

        candidate.CallEndTime = string.IsNullOrWhiteSpace(dto.CallEndTime) 
            ? candidate.CallEndTime 
            : TimeOnly.Parse(dto.CallEndTime);

        candidate.LinkedInUrl = string.IsNullOrWhiteSpace(dto.LinkedInUrl) 
            ? candidate.LinkedInUrl 
            : dto.LinkedInUrl;

        candidate.GitHubUrl = string.IsNullOrWhiteSpace(dto.GitHubUrl) 
            ? candidate.GitHubUrl 
            : dto.GitHubUrl;

        candidate.Comment = string.IsNullOrWhiteSpace(dto.Comment) 
            ? candidate.Comment 
            : dto.Comment;
    }

    /// <summary>
    /// Creates a new Candidate entity from DTO.
    /// </summary>
    /// <param name="dto">Data Transfer Object for candidate creation</param>
    /// <returns>A new Candidate instance</returns>
    private Candidate CreateNewCandidate(CandidateDto dto)
    {
        return new Candidate
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PhoneNumber = dto.PhoneNumber,
            Email = dto.Email,
            CallStartTime = string.IsNullOrWhiteSpace(dto.CallStartTime)
                ? default
                : TimeOnly.Parse(dto.CallStartTime),
            CallEndTime = string.IsNullOrWhiteSpace(dto.CallEndTime)
                ? default
                : TimeOnly.Parse(dto.CallEndTime),
            LinkedInUrl = dto.LinkedInUrl,
            GitHubUrl = dto.GitHubUrl,
            Comment = dto.Comment
        };
    }
}
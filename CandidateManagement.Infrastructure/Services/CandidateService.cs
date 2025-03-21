using CandidateManagement.Application.DTOs;
using CandidateManagement.Application.Repositories;
using CandidateManagement.Domain.Entities;
using CandidateManagement.Application.Services;

namespace CandidateManagement.Infrastructure.Services;
public class CandidateService : ICandidateService
{
    private readonly ICandidateRepository _candidateRepository;

    public CandidateService(ICandidateRepository candidateRepository)
    {
        _candidateRepository = candidateRepository;
    }

    public async Task<Candidate> AddOrUpdateCandidateAsync(CandidateDto dto)
    {
        var existingCandidate = await _candidateRepository.GetByEmailAsync(dto.Email);

        if (existingCandidate != null)
        {
            existingCandidate.FirstName = dto.FirstName;
            existingCandidate.LastName = dto.LastName;
            existingCandidate.PhoneNumber = dto.PhoneNumber;
            existingCandidate.CallStartTime = TimeOnly.Parse(dto.CallStartTime);
            existingCandidate.CallEndTime = TimeOnly.Parse(dto.CallEndTime);
            existingCandidate.LinkedInUrl = dto.LinkedInUrl;
            existingCandidate.GitHubUrl = dto.GitHubUrl;
            existingCandidate.Comment = dto.Comment;

            await _candidateRepository.UpdateCandidateAsync(existingCandidate);
            return existingCandidate;
        }

        var newCandidate = new Candidate
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PhoneNumber = dto.PhoneNumber,
            Email = dto.Email,
            CallStartTime = TimeOnly.Parse(dto.CallStartTime),
            CallEndTime = TimeOnly.Parse(dto.CallEndTime),
            LinkedInUrl = dto.LinkedInUrl,
            GitHubUrl = dto.GitHubUrl,
            Comment = dto.Comment
        };

        await _candidateRepository.AddCandidateAsync(newCandidate);
        return newCandidate;
    }
}
﻿using CandidateManagement.Application.DTOs;
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
            existingCandidate.FirstName = string.IsNullOrWhiteSpace(dto.FirstName) ? existingCandidate.FirstName : dto.FirstName;
            existingCandidate.LastName = string.IsNullOrWhiteSpace(dto.LastName) ? existingCandidate.LastName : dto.LastName;
            existingCandidate.PhoneNumber = string.IsNullOrWhiteSpace(dto.PhoneNumber) ? existingCandidate.PhoneNumber : dto.PhoneNumber;
            existingCandidate.CallStartTime = string.IsNullOrWhiteSpace(dto.CallStartTime) ? existingCandidate.CallStartTime : TimeOnly.Parse(dto.CallStartTime);
            existingCandidate.CallEndTime = string.IsNullOrWhiteSpace(dto.CallEndTime) ? existingCandidate.CallEndTime : TimeOnly.Parse(dto.CallEndTime);
            existingCandidate.LinkedInUrl = string.IsNullOrWhiteSpace(dto.LinkedInUrl) ? existingCandidate.LinkedInUrl : dto.LinkedInUrl;
            existingCandidate.GitHubUrl = string.IsNullOrWhiteSpace(dto.GitHubUrl) ? existingCandidate.GitHubUrl : dto.GitHubUrl;
            existingCandidate.Comment = string.IsNullOrWhiteSpace(dto.Comment) ? existingCandidate.Comment : dto.Comment;


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
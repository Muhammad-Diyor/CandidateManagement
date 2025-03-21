using CandidateManagement.Application.DTOs;
using CandidateManagement.Domain.Entities;

namespace CandidateManagement.Application.Services;
public interface ICandidateService
{
    Task<Candidate> AddOrUpdateCandidateAsync(CandidateDto dto);
}

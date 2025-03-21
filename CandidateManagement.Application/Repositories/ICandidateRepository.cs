using CandidateManagement.Domain.Entities;

namespace CandidateManagement.Application.Repositories;

public interface ICandidateRepository
{
    Task<Candidate?> GetByEmailAsync(string email);
    Task AddCandidateAsync(Candidate candidate);
    Task UpdateCandidateAsync(Candidate candidate);
}

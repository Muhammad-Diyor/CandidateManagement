using CandidateManagement.Application.Repositories;
using CandidateManagement.Domain.Entities;
using CandidateManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CandidateManagement.Infrastructure.Repositories;

public class CandidateRepository : ICandidateRepository
{
    private readonly AppDbContext _context;

    public CandidateRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Candidate?> GetByEmailAsync(string email)
    {
        return await _context.Candidates.FirstOrDefaultAsync(c => c.Email == email);
    }

    public async Task AddOrUpdateAsync(Candidate candidate)
    {
        var existingCandidate = await GetByEmailAsync(candidate.Email);
        if (existingCandidate != null)
        {
            _context.Entry(existingCandidate).CurrentValues.SetValues(candidate);
        }
        else
        {
            await _context.Candidates.AddAsync(candidate);
        }

        await _context.SaveChangesAsync();
    }
}

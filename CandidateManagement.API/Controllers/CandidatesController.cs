using CandidateManagement.Application.DTOs;
using CandidateManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CandidateManagement.API.Controllers;

[ApiController]
[Route("api/candidates")]
public class CandidatesController : ControllerBase
{
    private readonly ICandidateService _candidateService;

    public CandidatesController(ICandidateService candidateService)
    {
        _candidateService = candidateService;
    }

    [HttpPost]
    public async Task<IActionResult> AddOrUpdateCandidate([FromBody] CandidateDto dto)
    {
        var candidate = await _candidateService.AddOrUpdateCandidateAsync(dto);
        return Ok(candidate);
    }
}
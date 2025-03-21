namespace CandidateManagement.Application.DTOs;

public class CandidateDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string CallStartTime { get; set; } = string.Empty;
    public string CallEndTime { get; set; } = string.Empty;
    public string LinkedInUrl { get; set; } = string.Empty;
    public string GitHubUrl { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
}

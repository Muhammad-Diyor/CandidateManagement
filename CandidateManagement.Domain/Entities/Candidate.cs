namespace CandidateManagement.Domain.Entities;

public class Candidate
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // For the time interval when it’s better to call
    public TimeOnly CallStartTime { get; set; }  
    public TimeOnly CallEndTime { get; set; }

    public string LinkedInUrl { get; set; } = string.Empty;
    public string GitHubUrl { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
}

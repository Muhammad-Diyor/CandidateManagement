using CandidateManagement.Application.DTOs;
using FluentValidation;

namespace CandidateManagement.API.Validators;

public class CandidateDtoValidator : AbstractValidator<CandidateDto>
{
    public CandidateDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email format is invalid");

        RuleFor(x => x.FirstName)
            .Must(name => string.IsNullOrWhiteSpace(name) || name.Trim().Length > 0)
            .WithMessage("First name cannot be blank if provided");

        RuleFor(x => x.LastName)
            .Must(name => string.IsNullOrWhiteSpace(name) || name.Trim().Length > 0)
            .WithMessage("Last name cannot be blank if provided");

        RuleFor(x => x.PhoneNumber)
            .Must(phone => string.IsNullOrWhiteSpace(phone) || phone.Trim().Length > 0)
            .WithMessage("Phone number cannot be blank if provided");

        RuleFor(x => x.CallStartTime)
            .Must(BeValidTimeOnly)
            .When(x => !string.IsNullOrWhiteSpace(x.CallStartTime))
            .WithMessage("CallStartTime must be valid time in HH:mm format");

        RuleFor(x => x.CallEndTime)
            .Must(BeValidTimeOnly)
            .When(x => !string.IsNullOrWhiteSpace(x.CallEndTime))
            .WithMessage("CallEndTime must be valid time in HH:mm format");

        RuleFor(x => x.LinkedInUrl)
            .Must(uri => string.IsNullOrWhiteSpace(uri) || Uri.IsWellFormedUriString(uri, UriKind.Absolute))
            .WithMessage("LinkedIn URL must be a valid URI, including \"https:// \"");
        
        RuleFor(x => x.GitHubUrl)
            .Must(uri => string.IsNullOrWhiteSpace(uri) || Uri.IsWellFormedUriString(uri, UriKind.Absolute))
            .WithMessage("GitHub URL must be a valid URI, including \"https:// \" ");
    }

    private bool BeValidTimeOnly(string time)
    {
        return TimeOnly.TryParse(time, out _);
    }
}

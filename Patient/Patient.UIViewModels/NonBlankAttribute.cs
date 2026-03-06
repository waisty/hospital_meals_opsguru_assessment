using System.ComponentModel.DataAnnotations;

namespace Hospital.Patient.ViewModels;

/// <summary>
/// Validates that a string is not null, empty, or whitespace-only.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class NonBlankAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string s)
            return new ValidationResult(ErrorMessage ?? "Value must be a non-blank string.");

        if (string.IsNullOrWhiteSpace(s))
            return new ValidationResult(ErrorMessage ?? "Value cannot be blank.");

        return ValidationResult.Success;
    }
}

using System.ComponentModel.DataAnnotations;

namespace Hospital.Patient.WebApi.Validation;

/// <summary>
/// Validates request arguments that have validation attributes (e.g. DataAnnotations) and returns 400 if invalid.
/// </summary>
public sealed class ValidationEndpointFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        foreach (var argument in context.Arguments)
        {
            if (argument is null) continue;
            if (argument is CancellationToken) continue;

            var type = argument.GetType();
            if (type.IsPrimitive || type == typeof(string)) continue;

            if (!HasValidationAttributes(type)) continue;

            var results = new List<ValidationResult>();
            var validationContext = new ValidationContext(argument);
            if (Validator.TryValidateObject(argument, validationContext, results, validateAllProperties: true))
                continue;

            var errors = results
                .SelectMany(r => r.MemberNames.Select(member => new KeyValuePair<string, string[]>(member, [r.ErrorMessage ?? "Invalid value."])))
                .GroupBy(kv => kv.Key, kv => kv.Value[0])
                .ToDictionary(g => g.Key, g => g.ToArray());

            return Results.ValidationProblem(new Dictionary<string, string[]>(errors));
        }

        return await next(context);
    }

    private static bool HasValidationAttributes(Type type)
    {
        foreach (var prop in type.GetProperties())
        {
            if (Attribute.IsDefined(prop, typeof(ValidationAttribute)))
                return true;
        }
        return false;
    }
}

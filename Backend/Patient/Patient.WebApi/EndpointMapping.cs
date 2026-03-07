using Hospital.Patient.WebApi.EndpointMappings;
using Hospital.Patient.WebApi.Validation;

namespace Hospital.Patient.WebApi;

public static class EndpointMapping
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapGet("/", () => Results.Ok(new { service = "Hospital.Patient.WebApi", status = "running" }));

        var api = app.MapGroup("/api/v1").AddEndpointFilter<ValidationEndpointFilter>();

        api.MapPatientEndpointMappings();
        api.MapAllergyEndpointMappings();
        api.MapClinicalStateEndpointMappings();
        api.MapDietTypeEndpointMappings();
    }
}

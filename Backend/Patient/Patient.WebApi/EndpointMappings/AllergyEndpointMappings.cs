using Hospital.Patient.Core.Contracts;
using Hospital.Patient.ViewModels;
using Hospital.Patient.WebApi.Authentication;
using Microsoft.AspNetCore.Routing;

namespace Hospital.Patient.WebApi.EndpointMappings;

public static class AllergyEndpointMappings
{
    public static RouteGroupBuilder MapAllergyEndpointMappings(this RouteGroupBuilder group)
    {
        group.MapPost("/allergies", async (AllergyCreateRequest request, IAllergyHandler handler, CancellationToken ct) =>
        {
            var allergyId = await handler.AddAllergyAsync(request, ct);
            return Results.Created($"/api/v1/allergies/{allergyId}", new AllergyCreateResponse() { Id = allergyId });
        }).RequireAuthorization(JwtAuthenticationExtensions.AdminPolicyName);

        group.MapPut("/allergies/{id}", async (string id, AllergyUpdateRequest request, IAllergyHandler handler, CancellationToken ct) =>
        {
            var updated = await handler.UpdateAllergyAsync(id, request, ct);
            return updated ? Results.NoContent() : Results.NotFound();
        }).RequireAuthorization(JwtAuthenticationExtensions.AdminPolicyName);

        group.MapGet("/allergies/{id}", async (string id, IAllergyHandler handler, CancellationToken ct) =>
        {
            var allergy = await handler.GetAllergyByIdAsync(id, ct);
            return allergy is null ? Results.NotFound() : Results.Ok(allergy);
        }).RequireAuthorization(JwtAuthenticationExtensions.PatientAdminPolicyName);

        group.MapGet("/allergies", async (IAllergyHandler handler, CancellationToken ct) =>
        {
            var list = await handler.ListAllergiesAsync(ct);
            return Results.Ok(list);
        }).RequireAuthorization(JwtAuthenticationExtensions.PatientAdminPolicyName);

        return group;
    }
}

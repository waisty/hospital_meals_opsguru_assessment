using Hospital.Patient.Core.Contracts;
using Hospital.Patient.ViewModels;
using Hospital.Patient.WebApi.Authentication;
using Microsoft.AspNetCore.Routing;

namespace Hospital.Patient.WebApi.EndpointMappings;

public static class ClinicalStateEndpointMappings
{
    public static RouteGroupBuilder MapClinicalStateEndpointMappings(this RouteGroupBuilder group)
    {
        group.MapPost("/clinical-states", async (ClinicalStateCreateRequest request, IClinicalStateHandler handler, CancellationToken ct) =>
        {
            string clinicalStateId = await handler.AddClinicalStateAsync(request, ct);
            return Results.Created($"/api/v1/clinical-states/{clinicalStateId}", new ClinicalStateCreateResponse() { Id = clinicalStateId });
        }).RequireAuthorization(JwtAuthenticationExtensions.AdminPolicyName);

        group.MapPut("/clinical-states/{id}", async (string id, ClinicalStateUpdateRequest request, IClinicalStateHandler handler, CancellationToken ct) =>
        {
            var updated = await handler.UpdateClinicalStateAsync(id, request, ct);
            return updated ? Results.NoContent() : Results.NotFound();
        }).RequireAuthorization(JwtAuthenticationExtensions.AdminPolicyName);

        group.MapGet("/clinical-states/{id}", async (string id, IClinicalStateHandler handler, CancellationToken ct) =>
        {
            var clinicalState = await handler.GetClinicalStateByIdAsync(id, ct);
            return clinicalState is null ? Results.NotFound() : Results.Ok(clinicalState);
        }).RequireAuthorization(JwtAuthenticationExtensions.PatientAdminPolicyName);

        group.MapGet("/clinical-states", async (IClinicalStateHandler handler, CancellationToken ct) =>
        {
            var list = await handler.ListClinicalStatesAsync(ct);
            return Results.Ok(list);
        }).RequireAuthorization(JwtAuthenticationExtensions.PatientAdminPolicyName);

        return group;
    }
}

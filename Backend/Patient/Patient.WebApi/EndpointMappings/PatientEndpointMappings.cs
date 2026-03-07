using Hospital.Patient.Core.Contracts;
using Hospital.Patient.ViewModels;
using Hospital.Patient.WebApi.Authentication;
using Microsoft.AspNetCore.Routing;

namespace Hospital.Patient.WebApi.EndpointMappings;

public static class PatientEndpointMappings
{
    public static RouteGroupBuilder MapPatientEndpointMappings(this RouteGroupBuilder group)
    {
        group.MapPost("/patients", async (PatientCreateRequest request, IPatientHandler handler, CancellationToken ct) =>
        {
            var id = await handler.AddPatientAsync(request, ct);
            return Results.Created($"/api/v1/patients/{id}", new PatientCreateResponse { Id = id.ToString() });
        }).RequireAuthorization(JwtAuthenticationExtensions.PatientAdminOrMealsUserPolicyName);

        group.MapGet("/patients/{id}", async (string id, IPatientHandler handler, CancellationToken ct) =>
        {
            var patient = await handler.GetPatientByIdAsync(id, ct);
            return patient is null ? Results.NotFound() : Results.Ok(patient);
        }).RequireAuthorization(JwtAuthenticationExtensions.PatientAdminPolicyName);

        group.MapPut("/patients/{id}", async (string id, PatientUpdateRequest request, IPatientHandler handler, CancellationToken ct) =>
        {
            var updated = await handler.UpdatePatientAsync(id, request, ct);
            return updated ? Results.NoContent() : Results.NotFound();
        }).RequireAuthorization(JwtAuthenticationExtensions.PatientAdminPolicyName);

        group.MapGet("/patients/{id}/detail", async (string id, IPatientHandler handler, CancellationToken ct) =>
        {
            var detail = await handler.GetPatientDetailByIdAsync(id, ct);
            return detail is null ? Results.NotFound() : Results.Ok(detail);
        }).RequireAuthorization(JwtAuthenticationExtensions.PatientAdminPolicyName);

        group.MapGet("/patients/{id}/service-detail", async (string id, IPatientHandler handler, CancellationToken ct) =>
        {
            var detail = await handler.GetPatientServiceDetailByIdAsync(id, ct);
            return detail is null ? Results.NotFound() : Results.Ok(detail);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsServicePolicyName);

        group.MapGet("/patients", async (int page, int pageSize, IPatientHandler handler, CancellationToken ct) =>
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;
            var result = await handler.ListPatientsAsync(page, pageSize, ct);
            return Results.Ok(result);
        }).RequireAuthorization(JwtAuthenticationExtensions.PatientAdminPolicyName);

        group.MapGet("/patients/{patientId}/allergies", async (string patientId, IPatientHandler handler, CancellationToken ct) =>
        {
            var ids = await handler.GetAllergyIdsByPatientIdAsync(patientId, ct);
            return Results.Ok(ids);
        }).RequireAuthorization(JwtAuthenticationExtensions.PatientAdminPolicyName);

        group.MapPut("/patients/{patientId}/allergies", async (string patientId, PatientAllergiesUpdateRequest request, IPatientHandler handler, CancellationToken ct) =>
        {
            var updated = await handler.UpdatePatientAllergiesAsync(patientId, request, ct);
            return updated ? Results.NoContent() : Results.NotFound();
        }).RequireAuthorization(JwtAuthenticationExtensions.PatientAdminPolicyName);

        group.MapGet("/patients/{patientId}/clinical-states", async (string patientId, IPatientHandler handler, CancellationToken ct) =>
        {
            var ids = await handler.GetClinicalStateIdsByPatientIdAsync(patientId, ct);
            return Results.Ok(ids);
        }).RequireAuthorization(JwtAuthenticationExtensions.PatientAdminPolicyName);

        group.MapPut("/patients/{patientId}/clinical-states", async (string patientId, PatientClinicalStatesUpdateRequest request, IPatientHandler handler, CancellationToken ct) =>
        {
            var updated = await handler.UpdatePatientClinicalStatesAsync(patientId, request, ct);
            return updated ? Results.NoContent() : Results.NotFound();
        }).RequireAuthorization(JwtAuthenticationExtensions.PatientAdminPolicyName);

        return group;
    }
}

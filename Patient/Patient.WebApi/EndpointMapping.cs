using Hospital.Patient.Core.Contracts;
using Hospital.Patient.UIViewModels;
using Hospital.Patient.WebApi.Authentication;
using Hospital.Patient.WebApi.Validation;

namespace Hospital.Patient.WebApi;

public static class EndpointMapping
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapGet("/", () => Results.Ok(new { service = "Hospital.Patient.WebApi", status = "running" }));

        var api = app.MapGroup("/api/v1").AddEndpointFilter<ValidationEndpointFilter>();

        // Patient
        api.MapPost("/patients", async (PatientCreateRequest request, IPatientHandler handler, CancellationToken ct) =>
        {
            var id = await handler.AddPatientAsync(request, ct);
            return Results.Created($"/api/v1/patients/{id}", new PatientCreateResponse { Id = id.ToString() });
        }).RequireAuthorization(JwtAuthenticationExtensions.PatientAdminPolicyName);

        api.MapGet("/patients/{id}", async (string id, IPatientHandler handler, CancellationToken ct) =>
        {
            var patient = await handler.GetPatientByIdAsync(id, ct);
            return patient is null ? Results.NotFound() : Results.Ok(patient);
        }).RequireAuthorization(JwtAuthenticationExtensions.PatientAdminPolicyName);

        api.MapGet("/patients/{id}/detail", async (string id, IPatientHandler handler, CancellationToken ct) =>
        {
            var detail = await handler.GetPatientDetailByIdAsync(id, ct);
            return detail is null ? Results.NotFound() : Results.Ok(detail);
        }).RequireAuthorization(JwtAuthenticationExtensions.PatientAdminOrMealsServicePolicyName);

        api.MapGet("/patients", async (int page, int pageSize, IPatientHandler handler, CancellationToken ct) =>
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;
            var result = await handler.ListPatientsAsync(page, pageSize, ct);
            return Results.Ok(result);
        }).RequireAuthorization(JwtAuthenticationExtensions.PatientAdminPolicyName);

        // Allergy
        api.MapPost("/allergies", async (AllergyCreateRequest request, IPatientHandler handler, CancellationToken ct) =>
        {
            var allergyId = await handler.AddAllergyAsync(request, ct);
            return Results.Created($"/api/v1/allergies/{allergyId}", new AllergyCreateResponse() { Id = allergyId });
        }).RequireAuthorization(JwtAuthenticationExtensions.AdminPolicyName);

        api.MapPut("/allergies/{id}", async (string id, AllergyUpdateRequest request, IPatientHandler handler, CancellationToken ct) =>
        {
            var updated = await handler.UpdateAllergyAsync(id, request, ct);
            return updated ? Results.NoContent() : Results.NotFound();
        }).RequireAuthorization(JwtAuthenticationExtensions.AdminPolicyName);

        api.MapGet("/allergies/{id}", async (string id, IPatientHandler handler, CancellationToken ct) =>
        {
            var allergy = await handler.GetAllergyByIdAsync(id, ct);
            return allergy is null ? Results.NotFound() : Results.Ok(allergy);
        }).RequireAuthorization(JwtAuthenticationExtensions.PatientAdminPolicyName);

        api.MapGet("/allergies", async (IPatientHandler handler, CancellationToken ct) =>
        {
            var list = await handler.ListAllergiesAsync(ct);
            return Results.Ok(list);
        }).RequireAuthorization(JwtAuthenticationExtensions.PatientAdminPolicyName);

        api.MapGet("/patients/{patientId}/allergies", async (string patientId, IPatientHandler handler, CancellationToken ct) =>
        {
            var ids = await handler.GetAllergyIdsByPatientIdAsync(patientId, ct);
            return Results.Ok(ids);
        }).RequireAuthorization(JwtAuthenticationExtensions.PatientAdminPolicyName);

        api.MapPut("/patients/{patientId}/allergies", async (string patientId, PatientAllergiesUpdateRequest request, IPatientHandler handler, CancellationToken ct) =>
        {
            var updated = await handler.UpdatePatientAllergiesAsync(patientId, request, ct);
            return updated ? Results.NoContent() : Results.NotFound();
        }).RequireAuthorization(JwtAuthenticationExtensions.PatientAdminPolicyName);

        // Clinical state
        api.MapPost("/clinical-states", async (ClinicalStateCreateRequest request, IPatientHandler handler, CancellationToken ct) =>
        {
            string clinicalStateId = await handler.AddClinicalStateAsync(request, ct);
            return Results.Created($"/api/v1/clinical-states/{clinicalStateId}", new ClinicalStateCreateResponse() { Id = clinicalStateId });
        }).RequireAuthorization(JwtAuthenticationExtensions.AdminPolicyName);

        api.MapPut("/clinical-states/{id}", async (string id, ClinicalStateUpdateRequest request, IPatientHandler handler, CancellationToken ct) =>
        {
            var updated = await handler.UpdateClinicalStateAsync(id, request, ct);
            return updated ? Results.NoContent() : Results.NotFound();
        }).RequireAuthorization(JwtAuthenticationExtensions.AdminPolicyName);

        api.MapGet("/clinical-states/{id}", async (string id, IPatientHandler handler, CancellationToken ct) =>
        {
            var clinicalState = await handler.GetClinicalStateByIdAsync(id, ct);
            return clinicalState is null ? Results.NotFound() : Results.Ok(clinicalState);
        }).RequireAuthorization(JwtAuthenticationExtensions.PatientAdminPolicyName);

        api.MapGet("/clinical-states", async (IPatientHandler handler, CancellationToken ct) =>
        {
            var list = await handler.ListClinicalStatesAsync(ct);
            return Results.Ok(list);
        }).RequireAuthorization(JwtAuthenticationExtensions.PatientAdminPolicyName);

        api.MapGet("/patients/{patientId}/clinical-states", async (string patientId, IPatientHandler handler, CancellationToken ct) =>
        {
            var ids = await handler.GetClinicalStateIdsByPatientIdAsync(patientId, ct);
            return Results.Ok(ids);
        }).RequireAuthorization(JwtAuthenticationExtensions.PatientAdminPolicyName);

        api.MapPut("/patients/{patientId}/clinical-states", async (string patientId, PatientClinicalStatesUpdateRequest request, IPatientHandler handler, CancellationToken ct) =>
        {
            var updated = await handler.UpdatePatientClinicalStatesAsync(patientId, request, ct);
            return updated ? Results.NoContent() : Results.NotFound();
        }).RequireAuthorization(JwtAuthenticationExtensions.PatientAdminPolicyName);

        // Diet type
        api.MapPost("/diet-types", async (DietTypeCreateRequest request, IPatientHandler handler, CancellationToken ct) =>
        {
            string dietTypeId = await handler.AddDietTypeAsync(request, ct);
            return Results.Created($"/api/v1/diet-types/{dietTypeId}", new DietTypeCreateResponse() { Id = dietTypeId });
        }).RequireAuthorization(JwtAuthenticationExtensions.AdminPolicyName);

        api.MapPut("/diet-types/{id}", async (string id, DietTypeUpdateRequest request, IPatientHandler handler, CancellationToken ct) =>
        {
            var updated = await handler.UpdateDietTypeAsync(id, request, ct);
            return updated ? Results.NoContent() : Results.NotFound();
        }).RequireAuthorization(JwtAuthenticationExtensions.AdminPolicyName);

        api.MapGet("/diet-types/{id}", async (string id, IPatientHandler handler, CancellationToken ct) =>
        {
            var dietType = await handler.GetDietTypeByIdAsync(id, ct);
            return dietType is null ? Results.NotFound() : Results.Ok(dietType);
        }).RequireAuthorization(JwtAuthenticationExtensions.PatientAdminPolicyName);

        api.MapGet("/diet-types", async (IPatientHandler handler, CancellationToken ct) =>
        {
            var list = await handler.ListDietTypesAsync(ct);
            return Results.Ok(list);
        }).RequireAuthorization(JwtAuthenticationExtensions.PatientAdminPolicyName);
    }
}

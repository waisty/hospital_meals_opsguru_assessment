using Hospital.Meals.Core.Contracts;
using Hospital.Meals.ViewModels;
using Hospital.Meals.WebApi.Authentication;
using Microsoft.AspNetCore.Routing;

namespace Hospital.Meals.WebApi.EndpointMappings;

public static class ReferenceDataEndpointMappings
{
    public static RouteGroupBuilder MapReferenceDataEndpointMappings(this RouteGroupBuilder group)
    {
        // Allergy (reference data synced from Patient)
        group.MapPost("/allergies", async (AllergyCreateRequest request, IReferenceDataHandler handler, CancellationToken ct) =>
        {
            await handler.AddAllergyAsync(request, ct);
            return Results.Created($"/api/v1/allergies/{request.Id}", null);
        }).RequireAuthorization(JwtAuthenticationExtensions.PatientServicePolicyName);

        group.MapPut("/allergies/{id}", async (string id, AllergyUpdateRequest request, IReferenceDataHandler handler, CancellationToken ct) =>
        {
            var updated = await handler.UpdateAllergyAsync(id, request, ct);
            return updated ? Results.NoContent() : Results.NotFound();
        }).RequireAuthorization(JwtAuthenticationExtensions.PatientServicePolicyName);

        group.MapGet("/allergies/{id}", async (string id, IReferenceDataHandler handler, CancellationToken ct) =>
        {
            var allergy = await handler.GetAllergyByIdAsync(id, ct);
            return allergy is null ? Results.NotFound() : Results.Ok(allergy);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsUserPolicyName);

        group.MapGet("/allergies", async (IReferenceDataHandler handler, CancellationToken ct) =>
        {
            var list = await handler.ListAllergiesAsync(ct);
            return Results.Ok(list);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsUserPolicyName);

        // Clinical state (reference data)
        group.MapPost("/clinical-states", async (ClinicalStateCreateRequest request, IReferenceDataHandler handler, CancellationToken ct) =>
        {
            await handler.AddClinicalStateAsync(request, ct);
            return Results.Created($"/api/v1/clinical-states/{request.Id}", null);
        }).RequireAuthorization(JwtAuthenticationExtensions.PatientServicePolicyName);

        group.MapPut("/clinical-states/{id}", async (string id, ClinicalStateUpdateRequest request, IReferenceDataHandler handler, CancellationToken ct) =>
        {
            var updated = await handler.UpdateClinicalStateAsync(id, request, ct);
            return updated ? Results.NoContent() : Results.NotFound();
        }).RequireAuthorization(JwtAuthenticationExtensions.PatientServicePolicyName);

        group.MapGet("/clinical-states/{id}", async (string id, IReferenceDataHandler handler, CancellationToken ct) =>
        {
            var clinicalState = await handler.GetClinicalStateByIdAsync(id, ct);
            return clinicalState is null ? Results.NotFound() : Results.Ok(clinicalState);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsUserPolicyName);

        group.MapGet("/clinical-states", async (IReferenceDataHandler handler, CancellationToken ct) =>
        {
            var list = await handler.ListClinicalStatesAsync(ct);
            return Results.Ok(list);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsUserPolicyName);

        // Diet type (reference data)
        group.MapPost("/diet-types", async (DietTypeCreateRequest request, IReferenceDataHandler handler, CancellationToken ct) =>
        {
            await handler.AddDietTypeAsync(request, ct);
            return Results.Created($"/api/v1/diet-types/{request.Id}", null);
        }).RequireAuthorization(JwtAuthenticationExtensions.PatientServicePolicyName);

        group.MapPut("/diet-types/{id}", async (string id, DietTypeUpdateRequest request, IReferenceDataHandler handler, CancellationToken ct) =>
        {
            var updated = await handler.UpdateDietTypeAsync(id, request, ct);
            return updated ? Results.NoContent() : Results.NotFound();
        }).RequireAuthorization(JwtAuthenticationExtensions.PatientServicePolicyName);

        group.MapGet("/diet-types/{id}", async (string id, IReferenceDataHandler handler, CancellationToken ct) =>
        {
            var dietType = await handler.GetDietTypeByIdAsync(id, ct);
            return dietType is null ? Results.NotFound() : Results.Ok(dietType);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsUserPolicyName);

        group.MapGet("/diet-types", async (IReferenceDataHandler handler, CancellationToken ct) =>
        {
            var list = await handler.ListDietTypesAsync(ct);
            return Results.Ok(list);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsUserPolicyName);

        return group;
    }
}

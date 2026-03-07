using Hospital.Patient.Core.Contracts;
using Hospital.Patient.ViewModels;
using Hospital.Patient.WebApi.Authentication;
using Microsoft.AspNetCore.Routing;

namespace Hospital.Patient.WebApi.EndpointMappings;

public static class DietTypeEndpointMappings
{
    public static RouteGroupBuilder MapDietTypeEndpointMappings(this RouteGroupBuilder group)
    {
        group.MapPost("/diet-types", async (DietTypeCreateRequest request, IDietTypeHandler handler, CancellationToken ct) =>
        {
            string dietTypeId = await handler.AddDietTypeAsync(request, ct);
            return Results.Created($"/api/v1/diet-types/{dietTypeId}", new DietTypeCreateResponse() { Id = dietTypeId });
        }).RequireAuthorization(JwtAuthenticationExtensions.AdminPolicyName);

        group.MapPut("/diet-types/{id}", async (string id, DietTypeUpdateRequest request, IDietTypeHandler handler, CancellationToken ct) =>
        {
            var updated = await handler.UpdateDietTypeAsync(id, request, ct);
            return updated ? Results.NoContent() : Results.NotFound();
        }).RequireAuthorization(JwtAuthenticationExtensions.AdminPolicyName);

        group.MapGet("/diet-types/{id}", async (string id, IDietTypeHandler handler, CancellationToken ct) =>
        {
            var dietType = await handler.GetDietTypeByIdAsync(id, ct);
            return dietType is null ? Results.NotFound() : Results.Ok(dietType);
        }).RequireAuthorization(JwtAuthenticationExtensions.PatientAdminPolicyName);

        group.MapGet("/diet-types", async (IDietTypeHandler handler, CancellationToken ct) =>
        {
            var list = await handler.ListDietTypesAsync(ct);
            return Results.Ok(list);
        }).RequireAuthorization(JwtAuthenticationExtensions.PatientAdminPolicyName);

        return group;
    }
}

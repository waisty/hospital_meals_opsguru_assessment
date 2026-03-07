using static Hospital.Meals.Core.Contracts.Enums;
using Hospital.Meals.Core.Contracts;
using Hospital.Meals.ViewModels;
using Hospital.Meals.WebApi.Authentication;
using Microsoft.AspNetCore.Routing;

namespace Hospital.Meals.WebApi.EndpointMappings;

public static class PatientRequestEndpointMappings
{
    public static RouteGroupBuilder MapPatientRequestEndpointMappings(this RouteGroupBuilder group)
    {
        group.MapGet("/patient-requests/safety-check", async (HttpContext context, IRecipeSafetyService safetyService, CancellationToken ct) =>
        {
            var patientId = context.Request.Query["patientId"].ToString();
            var recipeId = context.Request.Query["recipeId"].ToString();
            if (string.IsNullOrWhiteSpace(patientId) || string.IsNullOrWhiteSpace(recipeId))
                return Results.BadRequest();
            var result = await safetyService.CheckRecipeSafetyAsync(patientId, recipeId, ct);
            return Results.Ok(result);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsUserPolicyName);

        group.MapPost("/patient-requests", async (PatientRequestCreateRequest request, IPatientRequestHandler handler, CancellationToken ct) =>
        {
            var requestInfo = await handler.AddPatientRequestAsync(request, ct);
            var response = new PatientCreateRequestResponse()
            {
                Id = requestInfo.requestId.ToString(),
                StatusReason = requestInfo.statusReason ?? "",
                StatusString = requestInfo.status.ToString(),
                UnsafeIngredientId = requestInfo.unsafeIngredientId
            };

            return requestInfo.status == MealRequestAppprovalStatus.Rejected ? Results.Conflict(response) : Results.Created($"/api/v1/patient-requests/{response.Id}", response);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsUserPolicyName);

        group.MapGet("/patient-requests/{id}", async (string id, IPatientRequestHandler handler, CancellationToken ct) =>
        {
            var request = await handler.GetPatientRequestByIdAsync(id, ct);
            return request is null ? Results.NotFound() : Results.Ok(request);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsUserPolicyName);

        group.MapGet("/patient-requests", async (int page, int pageSize, string? search, IPatientRequestHandler handler, CancellationToken ct) =>
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;
            var result = await handler.ListPatientRequestsAsync(page, pageSize, search, ct);
            return Results.Ok(result);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsUserPolicyName);

        return group;
    }
}

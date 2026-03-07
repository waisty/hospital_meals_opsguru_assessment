using Hospital.Meals.WebApi.EndpointMappings;
using Hospital.Meals.WebApi.Validation;

namespace Hospital.Meals.WebApi;

public static class EndpointMapping
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapGet("/", () => Results.Ok(new { service = "Hospital.Meals.WebApi", status = "running" }));

        var api = app.MapGroup("/api/v1").AddEndpointFilter<ValidationEndpointFilter>();

        api.MapMealEndpointMappings();
        api.MapRecipeEndpointMappings();
        api.MapIngredientEndpointMappings();
        api.MapPatientRequestEndpointMappings();
        api.MapReferenceDataEndpointMappings();
    }
}

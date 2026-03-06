using Hospital.Patient.Core;
using Hospital.Patient.Core.Contracts;
using Hospital.Patient.UIViewModels;
using Hospital.Patient.WebApi.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPatientServices(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);

var app = builder.Build();

app.UseJwtAuthentication();

// Patient
app.MapPost("/patients", async (PatientCreateRequest request, IPatientHandler handler, CancellationToken ct) =>
{
    await handler.AddPatientAsync(request, ct);
    return Results.Created($"/patients/{request.Id}", null);
}).RequireAuthorization();

app.MapGet("/patients/{id}", async (string id, IPatientHandler handler, CancellationToken ct) =>
{
    var patient = await handler.GetPatientByIdAsync(id, ct);
    return patient is null ? Results.NotFound() : Results.Ok(patient);
}).RequireAuthorization();

app.MapGet("/patients/{id}/detail", async (string id, IPatientHandler handler, CancellationToken ct) =>
{
    var detail = await handler.GetPatientDetailByIdAsync(id, ct);
    return detail is null ? Results.NotFound() : Results.Ok(detail);
}).RequireAuthorization();

app.MapGet("/patients", async (int page, int pageSize, IPatientHandler handler, CancellationToken ct) =>
{
    if (page < 1) page = 1;
    if (pageSize < 1 || pageSize > 100) pageSize = 10;
    var result = await handler.ListPatientsAsync(page, pageSize, ct);
    return Results.Ok(result);
}).RequireAuthorization();

// Allergy
app.MapPost("/allergies", async (AllergyCreateRequest request, IPatientHandler handler, CancellationToken ct) =>
{
    await handler.AddAllergyAsync(request, ct);
    return Results.Created($"/allergies/{request.Id}", null);
}).RequireAuthorization();

app.MapGet("/allergies/{id}", async (string id, IPatientHandler handler, CancellationToken ct) =>
{
    var allergy = await handler.GetAllergyByIdAsync(id, ct);
    return allergy is null ? Results.NotFound() : Results.Ok(allergy);
}).RequireAuthorization();

app.MapGet("/allergies", async (IPatientHandler handler, CancellationToken ct) =>
{
    var list = await handler.ListAllergiesAsync(ct);
    return Results.Ok(list);
}).RequireAuthorization();

app.MapGet("/patients/{patientId}/allergies", async (string patientId, IPatientHandler handler, CancellationToken ct) =>
{
    var ids = await handler.GetAllergyIdsByPatientIdAsync(patientId, ct);
    return Results.Ok(ids);
}).RequireAuthorization();

// Clinical state
app.MapPost("/clinical-states", async (ClinicalStateCreateRequest request, IPatientHandler handler, CancellationToken ct) =>
{
    await handler.AddClinicalStateAsync(request, ct);
    return Results.Created($"/clinical-states/{request.Id}", null);
}).RequireAuthorization();

app.MapGet("/clinical-states/{id}", async (string id, IPatientHandler handler, CancellationToken ct) =>
{
    var clinicalState = await handler.GetClinicalStateByIdAsync(id, ct);
    return clinicalState is null ? Results.NotFound() : Results.Ok(clinicalState);
}).RequireAuthorization();

app.MapGet("/clinical-states", async (IPatientHandler handler, CancellationToken ct) =>
{
    var list = await handler.ListClinicalStatesAsync(ct);
    return Results.Ok(list);
}).RequireAuthorization();

app.MapGet("/patients/{patientId}/clinical-states", async (string patientId, IPatientHandler handler, CancellationToken ct) =>
{
    var ids = await handler.GetClinicalStateIdsByPatientIdAsync(patientId, ct);
    return Results.Ok(ids);
}).RequireAuthorization();

// Diet type
app.MapPost("/diet-types", async (DietTypeCreateRequest request, IPatientHandler handler, CancellationToken ct) =>
{
    await handler.AddDietTypeAsync(request, ct);
    return Results.Created($"/diet-types/{request.Id}", null);
}).RequireAuthorization();

app.MapGet("/diet-types/{id}", async (string id, IPatientHandler handler, CancellationToken ct) =>
{
    var dietType = await handler.GetDietTypeByIdAsync(id, ct);
    return dietType is null ? Results.NotFound() : Results.Ok(dietType);
}).RequireAuthorization();

app.MapGet("/diet-types", async (IPatientHandler handler, CancellationToken ct) =>
{
    var list = await handler.ListDietTypesAsync(ct);
    return Results.Ok(list);
}).RequireAuthorization();

app.Run();

/// <summary>Entry point type for integration tests (WebApplicationFactory).</summary>
public partial class Program { }

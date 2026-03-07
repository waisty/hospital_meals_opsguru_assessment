using Hospital.Meals.Core.Contracts;
using Hospital.Meals.Core.InternalModels;
using Hospital.Meals.ViewModels;
using static Hospital.Meals.Core.Contracts.Enums;

namespace Hospital.Meals.Core.Implementation
{
    internal sealed class PatientRequestHandler : IPatientRequestHandler
    {
        private readonly IPatientRequestRepo _patientRequestRepo;
        private readonly IRecipeRepo _recipeRepo;
        private readonly IPatientApiClient _patientApiClient;
        private readonly IKitchenApiClient _kitchenApiClient;
        private readonly IRecipeSafetyService _recipeSafetyService;

        public PatientRequestHandler(
            IPatientRequestRepo patientRequestRepo,
            IRecipeRepo recipeRepo,
            IPatientApiClient patientApiClient,
            IKitchenApiClient kitchenApiClient,
            IRecipeSafetyService recipeSafetyService)
        {
            _patientRequestRepo = patientRequestRepo;
            _recipeRepo = recipeRepo;
            _patientApiClient = patientApiClient;
            _kitchenApiClient = kitchenApiClient;
            _recipeSafetyService = recipeSafetyService;
        }

        public async Task<(Guid requestId, MealRequestAppprovalStatus status, string? statusReason, string unsafeIngredientId)> AddPatientRequestAsync(PatientRequestCreateRequest request, CancellationToken cancellationToken = default)
        {
            var patientState = await _patientApiClient.GetPatientDetailAsync(request.PatientId, cancellationToken).ConfigureAwait(false)
                ?? throw new Exception($"Patient '{request.PatientId}' not found");

            var patientName = string.Join(" ", new[] { patientState.FirstName, patientState.MiddleName, patientState.LastName }.Where(s => !string.IsNullOrWhiteSpace(s))).Trim();
            var patientRequest = new PatientRequest
            {
                PatientId = request.PatientId,
                PatientName = string.IsNullOrEmpty(patientName) ? "-" : patientName,
                FirstName = patientState.FirstName,
                MiddleName = patientState.MiddleName ?? "",
                LastName = patientState.LastName,
                RecipeId = request.RecipeId,
                RequestedDateTime = DateTime.UtcNow,
                ApprovalStatus = MealRequestAppprovalStatus.Pending
            };

            var recipe = await _recipeRepo.GetRecipeByIdAsync(request.RecipeId, cancellationToken).ConfigureAwait(false)
                ?? throw new Exception($"Recipe '{request.RecipeId}' not found");
            var recipeIngredients = await _recipeRepo.GetRecipeIngredientsByRecipeIdAsync(request.RecipeId, cancellationToken).ConfigureAwait(false);

            await _patientRequestRepo.AddPatientRequestAsync(patientRequest, cancellationToken).ConfigureAwait(false);

            var publishTrayRequest = recipe.ToKitchenPublishTrayRequest(recipeIngredients, patientRequest);
            await _kitchenApiClient.PublishTrayAsync(publishTrayRequest, cancellationToken).ConfigureAwait(false);

            try
            {
                var safetyResult = await _recipeSafetyService.CheckRecipeSafetyAsync(request.PatientId, request.RecipeId, cancellationToken).ConfigureAwait(false);
                if (!safetyResult.IsSafe)
                {
                    patientRequest.ApprovalStatus = MealRequestAppprovalStatus.Rejected;
                    patientRequest.StatusReason = safetyResult.StatusReason;
                    patientRequest.UnsafeIngredientId = safetyResult.UnsafeIngredientId;
                }
                else
                {
                    patientRequest.ApprovalStatus = MealRequestAppprovalStatus.Accepted;
                    patientRequest.StatusReason = null;
                    patientRequest.UnsafeIngredientId = null;
                    patientRequest.FinalizedDateTime = DateTime.UtcNow;
                }
                await _patientRequestRepo.UpdatePatientRequestAsync(patientRequest, cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                patientRequest.ApprovalStatus = MealRequestAppprovalStatus.Rejected;
                patientRequest.StatusReason = "Error while performing safety validation";
                await _patientRequestRepo.UpdatePatientRequestAsync(patientRequest, cancellationToken).ConfigureAwait(false);
            }

            return (patientRequest.Id, patientRequest.ApprovalStatus, patientRequest.StatusReason, patientRequest.UnsafeIngredientId ?? "");
        }

        public async Task<PatientRequestViewModel?> GetPatientRequestByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            if (!Guid.TryParse(id, out var guid))
                return null;
            var request = await _patientRequestRepo.GetPatientRequestByIdAsync(guid, cancellationToken).ConfigureAwait(false);
            return request == null ? null : request.ToPatientRequestViewModel();
        }

        public async Task<PagedResult<PatientRequestViewModel>> ListPatientRequestsAsync(int page, int pageSize, string? search = null, CancellationToken cancellationToken = default)
        {
            var paged = await _patientRequestRepo.ListPatientRequestsAsync(page, pageSize, search, cancellationToken).ConfigureAwait(false);
            return new PagedResult<PatientRequestViewModel>
            {
                Items = paged.Items.Select(r => r.ToPatientRequestViewModel()).ToList(),
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            };
        }
    }
}

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
        private readonly IIngredientRepo _ingredientRepo;
        private readonly IPatientApiClient _patientApiClient;
        private readonly IKitchenApiClient _kitchenApiClient;

        public PatientRequestHandler(
            IPatientRequestRepo patientRequestRepo,
            IRecipeRepo recipeRepo,
            IIngredientRepo ingredientRepo,
            IPatientApiClient patientApiClient,
            IKitchenApiClient kitchenApiClient)
        {
            _patientRequestRepo = patientRequestRepo;
            _recipeRepo = recipeRepo;
            _ingredientRepo = ingredientRepo;
            _patientApiClient = patientApiClient;
            _kitchenApiClient = kitchenApiClient;
        }

        public async Task<(Guid requestId, MealRequestAppprovalStatus status, string? statusReason, string unsafeIngredientId)> AddPatientRequestAsync(PatientRequestCreateRequest request, CancellationToken cancellationToken = default)
        {
            var patientState = await _patientApiClient.GetPatientDetailAsync(request.PatientId, cancellationToken).ConfigureAwait(false)
                ?? throw new Exception($"Patient '{request.PatientId}' not found");

            var patientRequest = new PatientRequest
            {
                PatientId = request.PatientId,
                PatientName = patientState.Name,
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
                await VerifyRecipeSafetyAsync(patientRequest, patientState.AllergyIds, patientState.ClinicalStateIds, patientState.DietTypeId, cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                patientRequest.ApprovalStatus = MealRequestAppprovalStatus.Rejected;
                patientRequest.StatusReason = "Error while performing safety validation";
                await _patientRequestRepo.UpdatePatientRequestAsync(patientRequest, cancellationToken).ConfigureAwait(false);
            }

            return (patientRequest.Id, patientRequest.ApprovalStatus, patientRequest.StatusReason, patientRequest.UnsafeIngredientId ?? "");
        }

        private async Task VerifyRecipeSafetyAsync(
            PatientRequest patientRequest,
            IReadOnlyList<string> allergyIds,
            IReadOnlyList<string> clinicalStateIds,
            string? dietTypeId,
            CancellationToken cancellationToken = default)
        {
            var recipeIngredients = await _recipeRepo.GetRecipeIngredientsByRecipeIdAsync(patientRequest.RecipeId, cancellationToken).ConfigureAwait(false);

            foreach (var recipeIngredient in recipeIngredients)
            {
                var ingredientId = recipeIngredient.IngredientId;

                if (allergyIds.Count > 0)
                {
                    var allergyExclusionIds = await _ingredientRepo.GetAllergyIdsByIngredientIdAsync(ingredientId, cancellationToken).ConfigureAwait(false);
                    if (allergyIds.Any(aid => allergyExclusionIds.Contains(aid)))
                    {
                        patientRequest.ApprovalStatus = MealRequestAppprovalStatus.Rejected;
                        patientRequest.StatusReason = "Allergen detected";
                        patientRequest.UnsafeIngredientId = ingredientId;
                        await _patientRequestRepo.UpdatePatientRequestAsync(patientRequest, cancellationToken).ConfigureAwait(false);
                        return;
                    }
                }

                if (clinicalStateIds.Count > 0)
                {
                    var clinicalStateExclusionIds = await _ingredientRepo.GetClinicalStateIdsByIngredientIdAsync(ingredientId, cancellationToken).ConfigureAwait(false);
                    if (clinicalStateIds.Any(cid => clinicalStateExclusionIds.Contains(cid)))
                    {
                        patientRequest.ApprovalStatus = MealRequestAppprovalStatus.Rejected;
                        patientRequest.StatusReason = "Clinical state contraindication";
                        patientRequest.UnsafeIngredientId = ingredientId;
                        await _patientRequestRepo.UpdatePatientRequestAsync(patientRequest, cancellationToken).ConfigureAwait(false);
                        return;
                    }
                }

                if (!string.IsNullOrEmpty(dietTypeId))
                {
                    var dietTypeExclusionIds = await _ingredientRepo.GetDietTypeExclusionIdsByIngredientIdAsync(ingredientId, cancellationToken).ConfigureAwait(false);
                    if (dietTypeExclusionIds.Contains(dietTypeId))
                    {
                        patientRequest.ApprovalStatus = MealRequestAppprovalStatus.Rejected;
                        patientRequest.StatusReason = "Diet type exclusion";
                        patientRequest.UnsafeIngredientId = ingredientId;
                        await _patientRequestRepo.UpdatePatientRequestAsync(patientRequest, cancellationToken).ConfigureAwait(false);
                        return;
                    }
                }
            }

            patientRequest.ApprovalStatus = MealRequestAppprovalStatus.Accepted;
            patientRequest.StatusReason = null;
            patientRequest.UnsafeIngredientId = null;
            patientRequest.FinalizedDateTime = DateTime.UtcNow;
            await _patientRequestRepo.UpdatePatientRequestAsync(patientRequest, cancellationToken).ConfigureAwait(false);
        }

        public async Task<PatientRequestViewModel?> GetPatientRequestByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            if (!Guid.TryParse(id, out var guid))
                return null;
            var request = await _patientRequestRepo.GetPatientRequestByIdAsync(guid, cancellationToken).ConfigureAwait(false);
            return request == null ? null : request.ToPatientRequestViewModel();
        }

        public async Task<PagedResult<PatientRequestViewModel>> ListPatientRequestsAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var paged = await _patientRequestRepo.ListPatientRequestsAsync(page, pageSize, cancellationToken).ConfigureAwait(false);
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

using Hospital.Meals.ViewModels;
using static Hospital.Meals.Core.Contracts.Enums;

namespace Hospital.Meals.Core.Contracts
{
    public interface IPatientRequestHandler
    {
        Task<(Guid requestId, MealRequestAppprovalStatus status, string? statusReason, string unsafeIngredientId)> AddPatientRequestAsync(PatientRequestCreateRequest request, CancellationToken cancellationToken = default);
        Task<PatientRequestViewModel?> GetPatientRequestByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<PagedResult<PatientRequestViewModel>> ListPatientRequestsAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    }
}

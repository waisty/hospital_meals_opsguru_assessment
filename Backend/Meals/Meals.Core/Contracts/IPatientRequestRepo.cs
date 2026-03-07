using Hospital.Meals.Core.InternalModels;
using Hospital.Meals.ViewModels;

namespace Hospital.Meals.Core.Contracts
{
    internal interface IPatientRequestRepo
    {
        Task AddPatientRequestAsync(PatientRequest request, CancellationToken cancellationToken = default);
        Task UpdatePatientRequestAsync(PatientRequest request, CancellationToken cancellationToken = default);
        Task<PatientRequest?> GetPatientRequestByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PagedResult<PatientRequest>> ListPatientRequestsAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    }
}

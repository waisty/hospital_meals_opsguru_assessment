using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Meals.Core.InternalModels
{
    //NOTE: this will only be populated via the patient service and should not have a UI view model
    internal class Allergy
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
    }
}

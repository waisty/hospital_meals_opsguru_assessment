using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Meals.Core.InternalModels
{
    internal class Allergy
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
    }
}

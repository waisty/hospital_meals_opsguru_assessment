namespace Hospital.Meals.Core.InternalModels
{
    internal class Recipe
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public string? DietTypeId { get; set; }
    }
}

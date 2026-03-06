namespace Hospital.Meals.Core.InternalModels
{
    internal class Meal
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string RecipeId { get; set; } = "";
        public string? DietTypeId { get; set; }
    }
}

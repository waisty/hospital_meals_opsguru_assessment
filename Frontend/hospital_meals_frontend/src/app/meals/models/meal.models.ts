export interface MealViewModel {
  id: string;
  name: string;
  recipeId: string;
  dietTypeId: string | null;
  disabled: boolean;
}

export interface MealCreateRequest {
  id: string;
  name: string;
  recipeId: string;
  dietTypeId?: string | null;
}

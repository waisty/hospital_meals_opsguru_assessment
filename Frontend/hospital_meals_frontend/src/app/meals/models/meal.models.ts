export interface MealViewModel {
  id: string;
  name: string;
  recipeId: string;
  disabled: boolean;
}

export interface MealCreateRequest {
  id: string;
  name: string;
  recipeId: string;
}

export interface MealUpdateRequest {
  name: string;
  recipeId: string;
}

export interface MealRecipeViewModel {
  recipeId: string;
  recipeName: string;
  disabled: boolean;
}

export interface MealViewModel {
  id: string;
  name: string;
  description: string | null;
  disabled: boolean;
  recipes: MealRecipeViewModel[];
  recipeCount: number;
}

export interface MealCreateRequest {
  id: string;
  name: string;
  description?: string | null;
}

export interface MealUpdateRequest {
  name: string;
  description?: string | null;
}

export interface AddRecipeToMealRequest {
  recipeId: string;
}

export interface SetMealRecipeDisabledRequest {
  disabled: boolean;
}

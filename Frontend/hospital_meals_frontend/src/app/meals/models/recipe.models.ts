export interface RecipeViewModel {
  id: string;
  name: string;
  description: string | null;
  disabled: boolean;
}

export interface RecipeDetailViewModel {
  id: string;
  name: string;
  description: string | null;
  disabled: boolean;
  ingredients: RecipeIngredientViewModel[];
}

export interface RecipeCreateRequest {
  id: string;
  name: string;
  description?: string | null;
}

export interface RecipeIngredientViewModel {
  ingredientId: string;
  quantity: number;
  unit: string | null;
}

export interface SetRecipeIngredientsRequest {
  ingredients: RecipeIngredientViewModel[];
}

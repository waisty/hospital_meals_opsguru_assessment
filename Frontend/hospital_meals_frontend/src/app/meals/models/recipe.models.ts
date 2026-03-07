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
  /** Name of the meal this recipe is mapped to, if any. */
  mappedMealName?: string | null;
  ingredients: RecipeIngredientViewModel[];
}

export interface RecipeCreateRequest {
  id: string;
  name: string;
  description?: string | null;
}

export interface RecipeUpdateRequest {
  name: string;
  description?: string | null;
}

export interface RecipeIngredientViewModel {
  ingredientId: string;
  quantity: number;
  unit: string | null;
  ingredientName: string;
}

export interface SetRecipeIngredientsRequest {
  ingredients: RecipeIngredientViewModel[];
}

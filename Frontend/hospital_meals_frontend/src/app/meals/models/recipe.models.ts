import type { IngredientExclusionNamesItem } from './ingredient.models';

export interface RecipeViewModel {
  id: string;
  name: string;
  description: string | null;
  disabled: boolean;
  /** Name of the meal this recipe is mapped to, if any. */
  mappedMealName?: string | null;
}

/** Request for exclusion names by recipe IDs (subsequent call after list recipes). */
export interface RecipeExclusionNamesRequest {
  recipeIds: string[];
}

/** Aggregated exclusion names for one recipe. */
export interface RecipeExclusionNamesItemViewModel {
  recipeId: string;
  allergyNames: string[];
  clinicalStateNames: string[];
  dietTypeNames: string[];
}

export interface RecipeExclusionNamesResponseViewModel {
  items: RecipeExclusionNamesItemViewModel[];
}

export interface RecipeDetailViewModel {
  id: string;
  name: string;
  description: string | null;
  disabled: boolean;
  /** Name of the meal this recipe is mapped to, if any. */
  mappedMealName?: string | null;
  ingredients: RecipeIngredientViewModel[];
  /** Ingredient ID to exclusion names (for request summary and reusable display). */
  exclusionNamesByIngredientId?: Record<string, IngredientExclusionNamesItem> | null;
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

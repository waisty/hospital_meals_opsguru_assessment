export interface IngredientViewModel {
  id: string;
  name: string;
  description: string | null;
}

export interface IngredientDetailViewModel {
  id: string;
  name: string;
  description: string | null;
  allergyExclusionIds: string[];
  clinicalStateExclusionIds: string[];
  dietTypeExclusionIds: string[];
}

export interface IngredientCreateRequest {
  id: string;
  name: string;
  description?: string | null;
}

export interface SetIngredientAllergyExclusionsRequest {
  allergyIds: string[];
}

export interface SetIngredientClinicalStateExclusionsRequest {
  clinicalStateIds: string[];
}

export interface SetIngredientDietTypeExclusionsRequest {
  dietTypeIds: string[];
}

export interface IngredientExclusionNamesRequest {
  ingredientIds: string[];
}

export interface IngredientExclusionNamesItem {
  ingredientId: string;
  allergyNames: string[];
  clinicalStateNames: string[];
  dietTypeNames: string[];
}

export interface IngredientExclusionNamesResponse {
  items: IngredientExclusionNamesItem[];
}

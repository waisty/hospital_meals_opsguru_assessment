import type { IngredientExclusionNamesItem } from '../models/ingredient.models';

/** Aggregated exclusion names for a recipe (distinct across all its ingredients). */
export interface RecipeExclusionSummary {
  allergyNames: string[];
  clinicalStateNames: string[];
  dietTypeNames: string[];
}

/**
 * Computes the aggregated exclusion summary for a recipe from ingredient IDs and per-ingredient exclusion names.
 * Reusable for recipe list (from list response) and request summary (from recipe detail).
 */
export function computeRecipeExclusionSummary(
  recipeId: string,
  recipeIngredientIds: Record<string, string[]>,
  exclusionNamesByIngredientId: Record<string, IngredientExclusionNamesItem>
): RecipeExclusionSummary {
  const ingredientIds = recipeIngredientIds[recipeId] ?? [];
  const allergySet = new Set<string>();
  const clinicalSet = new Set<string>();
  const dietSet = new Set<string>();
  for (const ingId of ingredientIds) {
    const ex = exclusionNamesByIngredientId[ingId];
    if (!ex) continue;
    for (const n of ex.allergyNames ?? []) allergySet.add(n);
    for (const n of ex.clinicalStateNames ?? []) clinicalSet.add(n);
    for (const n of ex.dietTypeNames ?? []) dietSet.add(n);
  }
  return {
    allergyNames: [...allergySet],
    clinicalStateNames: [...clinicalSet],
    dietTypeNames: [...dietSet],
  };
}

export function hasRecipeExclusions(summary: RecipeExclusionSummary): boolean {
  return (
    summary.allergyNames.length > 0 ||
    summary.clinicalStateNames.length > 0 ||
    summary.dietTypeNames.length > 0
  );
}

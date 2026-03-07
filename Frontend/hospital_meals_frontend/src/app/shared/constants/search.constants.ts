/** Minimum length for full-text search (backend and frontend). Shorter strings are not sent to the API. */
export const MIN_SEARCH_LENGTH = 2;

export function isSearchLongEnough(search: string | null | undefined): boolean {
  const s = search?.trim();
  return (s?.length ?? 0) >= MIN_SEARCH_LENGTH;
}

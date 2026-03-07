/**
 * Simple cookie helpers for auth persistence.
 * Token is stored in a cookie so the user stays logged in across refreshes.
 */

const DEFAULT_MAX_AGE_SEC = 24 * 60 * 60; // 24 hours

export function setCookie(
  name: string,
  value: string,
  maxAgeSec: number = DEFAULT_MAX_AGE_SEC
): void {
  const secure = typeof window !== 'undefined' && window.location?.protocol === 'https:';
  let cookie = `${encodeURIComponent(name)}=${encodeURIComponent(value)}; path=/; max-age=${maxAgeSec}; SameSite=Lax`;
  if (secure) cookie += '; Secure';
  document.cookie = cookie;
}

export function getCookie(name: string): string | null {
  const encoded = encodeURIComponent(name);
  const match = document.cookie.match(
    new RegExp(`(?:^|;\\s*)${encoded.replace(/[.*+?^${}()|[\]\\]/g, '\\$&')}=([^;]*)`)
  );
  if (!match) return null;
  try {
    return decodeURIComponent(match[1]);
  } catch {
    return null;
  }
}

export function deleteCookie(name: string): void {
  document.cookie = `${encodeURIComponent(name)}=; path=/; max-age=0`;
}

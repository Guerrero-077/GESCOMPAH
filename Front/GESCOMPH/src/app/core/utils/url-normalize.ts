export function normalizeUrl(url: unknown): string {
  if (typeof url !== 'string') return ''; 
  // quita fragmento y query
  const noHash = url.split('#')[0];
  const noQuery = noHash.split('?')[0];
  // quita "/" inicial y espacios
  return noQuery.replace(/^\/+/, '').trim();
}
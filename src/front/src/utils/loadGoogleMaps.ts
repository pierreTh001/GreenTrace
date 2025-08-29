declare global {
  interface Window { google?: any }
}

export function loadGoogleMaps(apiKey: string, libraries: string[] = ['places']): Promise<void> {
  if (window.google && window.google.maps) return Promise.resolve()
  return new Promise((resolve, reject) => {
    const cbName = '__onGMapLoaded__' + Math.random().toString(36).slice(2)
    ;(window as any)[cbName] = () => {
      resolve()
      try { delete (window as any)[cbName] } catch {}
    }
    const script = document.createElement('script')
    const libs = libraries.join(',')
    script.src = `https://maps.googleapis.com/maps/api/js?key=${encodeURIComponent(apiKey)}&libraries=${encodeURIComponent(libs)}&callback=${cbName}`
    script.async = true
    script.defer = true
    script.onerror = (e) => reject(new Error('Failed to load Google Maps JS API'))
    document.head.appendChild(script)
  })
}


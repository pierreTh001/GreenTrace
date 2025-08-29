import { useEffect, useRef } from 'react'
import { loadGoogleMaps } from '../utils/loadGoogleMaps'

type Props = {
  placeholder?: string
  value?: string
  onChange?: (value: string) => void
  onSelected: (parts: { addressLine1?: string; postalCode?: string; city?: string; country?: string }) => void
}

export default function AddressAutocomplete({ placeholder = 'Rechercher une adresse', value, onChange, onSelected }: Props) {
  const inputRef = useRef<HTMLInputElement | null>(null)

  useEffect(() => {
    const key = import.meta.env.VITE_GOOGLE_MAPS_API_KEY as string | undefined
    let autocomplete: any
    let destroyed = false

    async function init() {
      if (!key) return
      try {
        await loadGoogleMaps(key)
        if (destroyed || !inputRef.current) return
        const gm = (window as any).google
        autocomplete = new gm.maps.places.Autocomplete(inputRef.current!, {
          types: ['address'],
          fields: ['address_components', 'formatted_address']
        })
        autocomplete.addListener('place_changed', () => {
          const place = autocomplete.getPlace()
          if (!place || !place.address_components) return
          const comps = place.address_components as Array<{ long_name: string; short_name: string; types: string[] }>
          const get = (type: string) => comps.find(c => c.types.includes(type))
          const streetNumber = get('street_number')?.long_name || ''
          const route = get('route')?.long_name || ''
          const postalCode = get('postal_code')?.long_name
          const city = get('locality')?.long_name || get('postal_town')?.long_name
          const country = get('country')?.long_name
          const addressLine1 = [streetNumber, route].filter(Boolean).join(' ').trim()
          onSelected({ addressLine1, postalCode: postalCode || undefined, city: city || undefined, country: country || undefined })
        })
      } catch {
        // ignore if API cannot load
      }
    }
    init()
    return () => { destroyed = true }
  }, [onSelected])

  return (
    <input
      ref={inputRef}
      className="input"
      placeholder={placeholder}
      value={value}
      onChange={(e)=> onChange?.(e.target.value)}
    />
  )
}

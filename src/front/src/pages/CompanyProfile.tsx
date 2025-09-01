import { useEffect, useState } from 'react'
import { CompanyService } from '../services/CompanyService'
import { useToast } from '../context/ToastContext'

export default function CompanyProfile() {
  const toast = useToast()
  const [c, setC] = useState<any>({...CompanyService.get()})
  const [suggestions, setSuggestions] = useState<any[]>([])
  const [showSuggest, setShowSuggest] = useState(false)
  const [loadingSuggest, setLoadingSuggest] = useState(false)
  const [errors, setErrors] = useState<Record<string, string>>({})

  function validate(next: any) {
    const e: Record<string,string> = {}
    if (!next.name || next.name.trim().length < 2) e.name = 'Raison sociale requise'
    if (next.city && /\d/.test(next.city)) e.city = 'La ville ne doit pas contenir de chiffres'
    if (next.postalCode) {
      const isFR = (next.hqCountry||'').toLowerCase().includes('fr')
      const re = isFR ? /^\d{5}$/ : /^[A-Za-z0-9][A-Za-z0-9 \-]{1,10}$/
      if (!re.test(next.postalCode)) e.postalCode = 'Code postal invalide'
    }
    // SIREN/SIRET: chiffres uniquement; si FR, 9 et 14 chiffres
    if (next.siren) {
      const digitsOnly = /^\d+$/.test(next.siren)
      const isFR = (next.hqCountry||'').toLowerCase().includes('fr')
      if (!digitsOnly) e.siren = 'SIREN: uniquement des chiffres'
      else if (isFR && next.siren.length !== 9) e.siren = 'SIREN: 9 chiffres requis en France'
    }
    if (next.siret) {
      const digitsOnly = /^\d+$/.test(next.siret)
      const isFR = (next.hqCountry||'').toLowerCase().includes('fr')
      if (!digitsOnly) e.siret = 'SIRET: uniquement des chiffres'
      else if (isFR && next.siret.length !== 14) e.siret = 'SIRET: 14 chiffres requis en France'
    }
    if (next.website && !/^((https?:\/\/)?[A-Za-z0-9.-]+\.[A-Za-z]{2,})$/.test(next.website)) e.website = 'Site web invalide (ex: exemple.fr)'
    if (next.esgContactEmail && !/^\S+@\S+\.\S+$/.test(next.esgContactEmail)) e.esgContactEmail = 'Email invalide'
    setErrors(e);
    return Object.keys(e).length === 0
  }
  useEffect(() => { /* compute initial validity */
    const ok = validate(c)
    // we don't store isValid here; we will compute it on render below
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [])
  const save = async () => {
    if (!validate(c)) return
    try {
      await CompanyService.save(c)
      toast.success('Enregistré')
    } catch (e:any) {
      const msg = String(e?.message || e || '')
      if (msg.startsWith('401')) toast.error('Session expirée ou invalide. Merci de vous reconnecter.')
      else if (msg.startsWith('403')) toast.warning('Accès refusé. Vérifiez vos droits sur cette entreprise.')
      else if (msg.startsWith('404')) toast.warning('Entreprise introuvable. Elle sera recréée au prochain enregistrement.')
      else toast.error('Erreur lors de la sauvegarde')
    }
  }

  return (
    <div className="max-w-4xl space-y-6">
      <div>
        <h1 className="text-2xl font-semibold">Fiche entreprise (ESRS 2)</h1>
        <p className="text-slate-600">Renseignez les informations générales de votre entreprise.</p>
      </div>

      <div className="card grid md:grid-cols-2 gap-4">
        <div className="relative md:col-span-2">
          <label className="label">Raison sociale <span className="text-red-600">*</span></label>
          <input className="input" value={c.name || ''} onChange={async e=>{
            const v = e.target.value
            const next = { ...c, name: v }
            setC(next); validate(next)
            if (!v || v.length < 2) { setSuggestions([]); setShowSuggest(false); return }
            try {
              setLoadingSuggest(true)
              const res = await fetch(`/api/public/companies/lookup?query=${encodeURIComponent(v)}`)
              if (res.ok) {
                const data = await res.json()
                setSuggestions(data)
                setShowSuggest(true)
              } else {
                // API répond mais pas OK → pas de suggestions
                setSuggestions([])
                setShowSuggest(false)
              }
            } catch {
              // API indisponible (ex: proxy ECONNREFUSED) → mode dégradé sans suggestions
              setSuggestions([])
              setShowSuggest(false)
            } finally {
              setLoadingSuggest(false)
            }
          }} />
          {showSuggest && suggestions.length > 0 && (
            <div className="absolute z-20 bg-white border border-slate-200 rounded-md mt-1 max-h-60 overflow-auto w-full shadow">
              {suggestions.map((s:any, idx:number) => (
                <button key={idx} type="button" className="w-full text-left px-3 py-2 hover:bg-slate-50" onClick={()=>{
                  setC({
                    ...c,
                    name: s.name,
                    legalForm: s.legalForm || c.legalForm,
                    siren: s.siren || c.siren,
                    siret: s.siret || c.siret,
                    vatNumber: s.vatNumber || c.vatNumber,
                    naceCode: s.naceCode || c.naceCode,
                    addressLine1: s.addressLine1 || c.addressLine1,
                    postalCode: s.postalCode || c.postalCode,
                    city: s.city || c.city,
                    hqCountry: s.country || c.hqCountry
                  })
                  setShowSuggest(false)
                }}>
                  <div className="font-medium">{s.name}</div>
                  <div className="text-xs text-slate-500">{s.siren} • {s.legalForm || ''} • {s.city || ''}</div>
                </button>
              ))}
            </div>
          )}
          {errors.name && <div className="text-red-600 text-xs mt-1">{errors.name}</div>}
          {loadingSuggest && <div className="text-xs text-slate-500 mt-1">Recherche…</div>}
        </div>
        <div className="md:col-span-2">
          <label className="label">Adresse (ligne 1) <span className="text-red-600">*</span></label>
          <input className="input" value={c.addressLine1 || ''} onChange={e=>{ const next = { ...c, addressLine1: e.target.value }; setC(next); validate(next) }} />
          {errors.addressLine1 && <div className="text-red-600 text-xs mt-1">{errors.addressLine1}</div>}
        </div>
        <div className="md:col-span-2">
          <label className="label">Adresse (ligne 2)</label>
          <input className="input" value={c.addressLine2 || ''} onChange={e=>{ const next = { ...c, addressLine2: e.target.value }; setC(next); validate(next) }} />
        </div>
        <div>
          <label className="label">Code postal</label>
          <input className="input" value={c.postalCode || ''} onChange={e=>{ const next = { ...c, postalCode: e.target.value }; setC(next); validate(next) }} />
          {errors.postalCode && <div className="text-red-600 text-xs mt-1">{errors.postalCode}</div>}
        </div>
        <div>
          <label className="label">Ville</label>
          <input className="input" value={c.city || ''} onChange={e=>{ const next = { ...c, city: e.target.value }; setC(next); validate(next) }} />
          {errors.city && <div className="text-red-600 text-xs mt-1">{errors.city}</div>}
        </div>
        <div>
          <label className="label">Pays du siège <span className="text-red-600">*</span></label>
          <input className="input" value={c.hqCountry || ''} onChange={e=>{ const next = { ...c, hqCountry: e.target.value }; setC(next); validate(next) }} />
          {errors.hqCountry && <div className="text-red-600 text-xs mt-1">{errors.hqCountry}</div>}
        </div>
        <div>
          <label className="label">Forme juridique <span className="text-red-600">*</span></label>
          <input className="input" value={c.legalForm || ''} onChange={e=>{ const next = { ...c, legalForm: e.target.value }; setC(next); validate(next) }} />
          {errors.legalForm && <div className="text-red-600 text-xs mt-1">{errors.legalForm}</div>}
        </div>
        <div>
          <label className="label">SIREN</label>
          <input className="input" value={c.siren || ''} onChange={e=>{ const next = { ...c, siren: e.target.value }; setC(next); validate(next) }} />
          {errors.siren && <div className="text-red-600 text-xs mt-1">{errors.siren}</div>}
        </div>
        <div>
          <label className="label">SIRET</label>
          <input className="input" value={c.siret || ''} onChange={e=>{ const next = { ...c, siret: e.target.value }; setC(next); validate(next) }} />
          {errors.siret && <div className="text-red-600 text-xs mt-1">{errors.siret}</div>}
        </div>
        <div>
          <label className="label">N° TVA <span className="text-red-600">*</span></label>
          <input className="input" value={c.vatNumber || ''} onChange={e=>{ const next = { ...c, vatNumber: e.target.value }; setC(next); validate(next) }} />
          {errors.vatNumber && <div className="text-red-600 text-xs mt-1">{errors.vatNumber}</div>}
        </div>
        
        <div>
          <label className="label">Code NACE <span className="text-red-600">*</span></label>
          <input className="input" value={c.naceCode || ''} onChange={e=>{ const next = { ...c, naceCode: e.target.value }; setC(next); validate(next) }} />
          {errors.naceCode && <div className="text-red-600 text-xs mt-1">{errors.naceCode}</div>}
        </div>
        <div>
          <label className="label">Site web <span className="text-red-600">*</span></label>
          <input className="input" value={c.website || ''} placeholder="ex: exemple.fr ou https://exemple.fr" onChange={e=>{ const next = { ...c, website: e.target.value }; setC(next); validate(next) }} />
          {errors.website && <div className="text-red-600 text-xs mt-1">{errors.website}</div>}
        </div>
        <div>
          <label className="label">Effectif (nombre de salariés) <span className="text-red-600">*</span></label>
          <input className="input" type="number" value={c.employeesCount ?? ''} onChange={e=>{ const next = { ...c, employeesCount: e.target.value === '' ? null : Number(e.target.value) }; setC(next); validate(next) }} />
        </div>
        <div>
          <label className="label">Méthode de consolidation <span className="text-red-600">*</span></label>
          <input className="input" value={c.consolidationMethod || ''} onChange={e=>{ const next = { ...c, consolidationMethod: e.target.value }; setC(next); validate(next) }} />
          {errors.consolidationMethod && <div className="text-red-600 text-xs mt-1">{errors.consolidationMethod}</div>}
        </div>
        <div className="md:col-span-2">
          <label className="label">Email contact ESG <span className="text-red-600">*</span></label>
          <input className="input" type="email" value={c.esgContactEmail || ''} onChange={e=>{ const next = { ...c, esgContactEmail: e.target.value }; setC(next); validate(next) }} />
          {errors.esgContactEmail && <div className="text-red-600 text-xs mt-1">{errors.esgContactEmail}</div>}
        </div>
      </div>

      <div className="flex gap-3">
        {/** Compute validity without triggering state changes */}
        {(() => {
          const ok = Object.keys(errors).length === 0
            && c.name && c.name.trim().length >= 2
            && c.legalForm && c.legalForm.trim()
            && c.siren && c.siren.trim()
            && c.siret && c.siret.trim()
            && c.vatNumber && c.vatNumber.trim()
            && c.naceCode && c.naceCode.trim()
            && c.website && c.website.trim()
            && c.addressLine1 && c.addressLine1.trim()
            // addressLine2 is optional
            && c.postalCode && c.postalCode.trim()
            && c.city && c.city.trim()
            && c.hqCountry && c.hqCountry.trim()
            && (c.employeesCount !== null && c.employeesCount !== undefined && c.employeesCount !== '')
            && c.consolidationMethod && c.consolidationMethod.trim()
            && c.esgContactEmail && c.esgContactEmail.trim();
          return (
            <button className="btn btn-primary disabled:opacity-50" disabled={!ok} onClick={save} title={!ok ? 'Complétez les champs requis' : undefined}>Enregistrer</button>
          )
        })()}
      </div>
    </div>
  )
}

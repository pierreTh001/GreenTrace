import { useState } from 'react'
import { CompanyService } from '../services/CompanyService'

export default function CompanyProfile() {
  const [c, setC] = useState<any>({...CompanyService.get()})
  const [suggestions, setSuggestions] = useState<any[]>([])
  const [showSuggest, setShowSuggest] = useState(false)
  const [loadingSuggest, setLoadingSuggest] = useState(false)
  const save = async () => {
    await CompanyService.save(c)
    alert('Enregistré')
  }

  return (
    <div className="max-w-4xl space-y-6">
      <div>
        <h1 className="text-2xl font-semibold">Fiche entreprise (ESRS 2)</h1>
        <p className="text-slate-600">Renseignez les informations générales de votre entreprise.</p>
      </div>

      <div className="card grid md:grid-cols-2 gap-4">
        <div className="relative">
          <label className="label">Raison sociale</label>
          <input className="input" value={c.name || ''} onChange={async e=>{
            const v = e.target.value
            setC({...c, name: v});
            if (!v || v.length < 2) { setSuggestions([]); setShowSuggest(false); return }
            try {
              setLoadingSuggest(true)
              const res = await fetch(`/api/public/companies/lookup?query=${encodeURIComponent(v)}`)
              if (res.ok) {
                const data = await res.json()
                setSuggestions(data)
                setShowSuggest(true)
              }
            } finally { setLoadingSuggest(false) }
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
          {loadingSuggest && <div className="text-xs text-slate-500 mt-1">Recherche…</div>}
        </div>
        <div className="md:col-span-2">
          <label className="label">Adresse (ligne 1)</label>
          <input className="input" value={c.addressLine1 || ''} onChange={e=>setC({...c, addressLine1: e.target.value})} />
        </div>
        <div className="md:col-span-2">
          <label className="label">Adresse (ligne 2)</label>
          <input className="input" value={c.addressLine2 || ''} onChange={e=>setC({...c, addressLine2: e.target.value})} />
        </div>
        <div>
          <label className="label">Code postal</label>
          <input className="input" value={c.postalCode || ''} onChange={e=>setC({...c, postalCode: e.target.value})} />
        </div>
        <div>
          <label className="label">Ville</label>
          <input className="input" value={c.city || ''} onChange={e=>setC({...c, city: e.target.value})} />
        </div>
        <div>
          <label className="label">Pays du siège</label>
          <input className="input" value={c.hqCountry || ''} onChange={e=>setC({...c, hqCountry: e.target.value})} />
        </div>
        <div>
          <label className="label">Forme juridique</label>
          <input className="input" value={c.legalForm || ''} onChange={e=>setC({...c, legalForm: e.target.value})} />
        </div>
        <div>
          <label className="label">SIREN</label>
          <input className="input" value={c.siren || ''} onChange={e=>setC({...c, siren: e.target.value})} />
        </div>
        <div>
          <label className="label">SIRET</label>
          <input className="input" value={c.siret || ''} onChange={e=>setC({...c, siret: e.target.value})} />
        </div>
        <div>
          <label className="label">N° TVA</label>
          <input className="input" value={c.vatNumber || ''} onChange={e=>setC({...c, vatNumber: e.target.value})} />
        </div>
        
        <div>
          <label className="label">Code NACE</label>
          <input className="input" value={c.naceCode || ''} onChange={e=>setC({...c, naceCode: e.target.value})} />
        </div>
        <div>
          <label className="label">Site web</label>
          <input className="input" value={c.website || ''} onChange={e=>setC({...c, website: e.target.value})} />
        </div>
        <div>
          <label className="label">Effectif (nombre de salariés)</label>
          <input className="input" type="number" value={c.employeesCount ?? ''} onChange={e=>setC({...c, employeesCount: e.target.value === '' ? null : Number(e.target.value)})} />
        </div>
        <div>
          <label className="label">Méthode de consolidation</label>
          <input className="input" value={c.consolidationMethod || ''} onChange={e=>setC({...c, consolidationMethod: e.target.value})} />
        </div>
        <div className="md:col-span-2">
          <label className="label">Email contact ESG</label>
          <input className="input" type="email" value={c.esgContactEmail || ''} onChange={e=>setC({...c, esgContactEmail: e.target.value})} />
        </div>
      </div>

      <div className="flex gap-3">
        <button className="btn btn-primary" onClick={save}>Enregistrer</button>
      </div>
    </div>
  )
}

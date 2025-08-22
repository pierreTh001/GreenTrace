import { useState } from 'react'
import { CompanyService } from '../services/CompanyService'

export default function CompanyProfile() {
  const [c, setC] = useState({...CompanyService.get()})
  const save = () => {
    CompanyService.save(c)
    alert('Enregistré')
  }

  return (
    <div className="max-w-3xl space-y-6">
      <div>
        <h1 className="text-2xl font-semibold">Fiche entreprise (ESRS 2)</h1>
        <p className="text-slate-600">Renseignez les informations générales obligatoires.</p>
      </div>

      <div className="card grid md:grid-cols-2 gap-4">
        <div>
          <label className="label">Raison sociale</label>
          <input className="input" value={c.name} onChange={e=>setC({...c, name: e.target.value})} />
        </div>
        <div>
          <label className="label">Secteur</label>
          <input className="input" value={c.sector} onChange={e=>setC({...c, sector: e.target.value})} />
        </div>
        <div>
          <label className="label">Effectif</label>
          <input className="input" type="number" value={c.employees} onChange={e=>setC({...c, employees: Number(e.target.value)})} />
        </div>
        <div>
          <label className="label">Chiffre d'affaires (€)</label>
          <input className="input" type="number" value={c.revenue} onChange={e=>setC({...c, revenue: Number(e.target.value)})} />
        </div>
        <div className="md:col-span-2">
          <label className="label">Siège social</label>
          <input className="input" value={c.headquarters} onChange={e=>setC({...c, headquarters: e.target.value})} />
        </div>
        <div>
          <label className="label">Entreprise cotée UE</label>
          <select className="input" value={c.euListed ? 'yes' : 'no'} onChange={e=>setC({...c, euListed: e.target.value==='yes'})}>
            <option value="no">Non</option>
            <option value="yes">Oui</option>
          </select>
        </div>
        <div>
          <label className="label">Année de reporting</label>
          <input className="input" type="number" value={c.reportingYear} onChange={e=>setC({...c, reportingYear: Number(e.target.value)})} />
        </div>
      </div>

      <div className="flex gap-3">
        <button className="btn btn-primary" onClick={save}>Enregistrer</button>
      </div>
    </div>
  )
}

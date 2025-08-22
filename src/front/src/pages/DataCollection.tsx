import { useState } from 'react'
import { CompanyService } from '../services/CompanyService'

export default function DataCollection() {
  const data = CompanyService.getDataCollection()
  const [policies, setPolicies] = useState<string>(data.policies ?? '')
  const [risks, setRisks] = useState<string>(data.risks ?? '')
  const [targets, setTargets] = useState<string>(data.targets ?? '')
  const [energyMWh, setEnergyMWh] = useState<number>(data.metrics?.energyMWh ?? 0)
  const [renewableShare, setRenewableShare] = useState<number>(data.metrics?.renewableShare ?? 0)

  const save = () => {
    CompanyService.saveDataCollection({ policies, risks, targets, metrics: { energyMWh, renewableShare } })
    alert('Collecte enregistrée')
  }

  return (
    <div className="max-w-3xl space-y-6">
      <div>
        <h1 className="text-2xl font-semibold">Collecte des données</h1>
        <p className="text-slate-600">Renseignez vos politiques, risques, objectifs et indicateurs clés (ex. énergie).</p>
      </div>

      <div className="card space-y-4">
        <div>
          <label className="label">Politiques</label>
          <textarea className="input h-28" value={policies} onChange={e=>setPolicies(e.target.value)} />
        </div>
        <div>
          <label className="label">Risques & opportunités</label>
          <textarea className="input h-28" value={risks} onChange={e=>setRisks(e.target.value)} />
        </div>
        <div>
          <label className="label">Objectifs</label>
          <textarea className="input h-28" value={targets} onChange={e=>setTargets(e.target.value)} />
        </div>
        <div className="grid sm:grid-cols-2 gap-4">
          <div>
            <label className="label">Consommation d'énergie (MWh)</label>
            <input className="input" type="number" value={energyMWh} onChange={e=>setEnergyMWh(Number(e.target.value))} />
          </div>
          <div>
            <label className="label">Part renouvelable</label>
            <input className="input" type="number" min={0} max={1} step="0.01" value={renewableShare} onChange={e=>setRenewableShare(Number(e.target.value))} />
            <div className="text-xs text-slate-500 mt-1">Ex: 0.35 = 35%</div>
          </div>
        </div>
      </div>

      <button className="btn btn-primary" onClick={save}>Enregistrer</button>
    </div>
  )
}

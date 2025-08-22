import { useState } from 'react'
import { EmissionsService } from '../services/EmissionsService'

type S1 = { category: string; tco2e: number }
type S2 = { method: 'market'|'location'; tco2e: number }
type S3 = { category: string; tco2e: number }

export default function Emissions() {
  const init = EmissionsService.get()
  const [scope1, setS1] = useState<S1[]>(init.scope1)
  const [scope2, setS2] = useState<S2[]>(init.scope2)
  const [scope3, setS3] = useState<S3[]>(init.scope3)

  const addS1 = () => setS1([...scope1, { category: 'Nouvelle source', tco2e: 0 }])
  const addS2 = () => setS2([...scope2, { method: 'market', tco2e: 0 }])
  const addS3 = () => setS3([...scope3, { category: 'Nouvelle catégorie', tco2e: 0 }])

  const save = () => {
    EmissionsService.save({ scope1, scope2, scope3 })
    alert('Emissions enregistrées')
  }

  const totals = EmissionsService.totals()

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-semibold">Bilans GES</h1>
        <p className="text-slate-600">Saisissez vos émissions en tCO₂e (méthodologie GHG Protocol simplifiée).</p>
      </div>

      <div className="grid lg:grid-cols-3 gap-5">
        <div className="card">
          <div className="flex items-center justify-between mb-3">
            <h3 className="font-semibold">Scope 1</h3>
            <button className="btn btn-outline" onClick={addS1}>Ajouter</button>
          </div>
          {scope1.map((row, idx) => (
            <div key={idx} className="grid grid-cols-[1fr_140px_40px] gap-2 mb-2">
              <input className="input" value={row.category} onChange={e=>{
                const arr = [...scope1]; arr[idx].category = e.target.value; setS1(arr)
              }} />
              <input className="input" type="number" value={row.tco2e} onChange={e=>{
                const arr = [...scope1]; arr[idx].tco2e = Number(e.target.value); setS1(arr)
              }} />
              <button className="btn btn-outline" onClick={()=>setS1(scope1.filter((_,i)=>i!==idx))}>×</button>
            </div>
          ))}
        </div>

        <div className="card">
          <div className="flex items-center justify-between mb-3">
            <h3 className="font-semibold">Scope 2</h3>
            <button className="btn btn-outline" onClick={addS2}>Ajouter</button>
          </div>
          {scope2.map((row, idx) => (
            <div key={idx} className="grid grid-cols-[160px_1fr_40px] gap-2 mb-2">
              <select className="input" value={row.method} onChange={e=>{
                const arr = [...scope2]; arr[idx].method = e.target.value as any; setS2(arr)
              }}>
                <option value="market">Market-based</option>
                <option value="location">Location-based</option>
              </select>
              <input className="input" type="number" value={row.tco2e} onChange={e=>{
                const arr = [...scope2]; arr[idx].tco2e = Number(e.target.value); setS2(arr)
              }} />
              <button className="btn btn-outline" onClick={()=>setS2(scope2.filter((_,i)=>i!==idx))}>×</button>
            </div>
          ))}
        </div>

        <div className="card">
          <div className="flex items-center justify-between mb-3">
            <h3 className="font-semibold">Scope 3</h3>
            <button className="btn btn-outline" onClick={addS3}>Ajouter</button>
          </div>
          {scope3.map((row, idx) => (
            <div key={idx} className="grid grid-cols-[1fr_140px_40px] gap-2 mb-2">
              <input className="input" value={row.category} onChange={e=>{
                const arr = [...scope3]; arr[idx].category = e.target.value; setS3(arr)
              }} />
              <input className="input" type="number" value={row.tco2e} onChange={e=>{
                const arr = [...scope3]; arr[idx].tco2e = Number(e.target.value); setS3(arr)
              }} />
              <button className="btn btn-outline" onClick={()=>setS3(scope3.filter((_,i)=>i!==idx))}>×</button>
            </div>
          ))}
        </div>
      </div>

      <div className="grid sm:grid-cols-4 gap-4">
        <div className="card"><div className="text-sm text-slate-500">Scope 1</div><div className="text-xl font-semibold">{totals.s1.toFixed(1)} tCO₂e</div></div>
        <div className="card"><div className="text-sm text-slate-500">Scope 2</div><div className="text-xl font-semibold">{totals.s2.toFixed(1)} tCO₂e</div></div>
        <div className="card"><div className="text-sm text-slate-500">Scope 3</div><div className="text-xl font-semibold">{totals.s3.toFixed(1)} tCO₂e</div></div>
        <div className="card"><div className="text-sm text-slate-500">Total</div><div className="text-xl font-semibold">{totals.total.toFixed(1)} tCO₂e</div></div>
      </div>

      <button className="btn btn-primary" onClick={save}>Enregistrer</button>
    </div>
  )
}

import { useState } from 'react'
import { CompanyService } from '../services/CompanyService'
import { useToast } from '../context/ToastContext'

const TOPICS = [
  'ESRS 2 - Informations générales',
  'ESRS E1 - Climat',
  'ESRS E2 - Pollution',
  'ESRS E3 - Eau et ressources marines',
  'ESRS E4 - Biodiversité',
  'ESRS E5 - Ressources et économie circulaire',
  'ESRS S1 - Propre personnel',
  'ESRS S2 - Travailleurs de la chaîne de valeur',
  'ESRS S3 - Communautés affectées',
  'ESRS S4 - Consommateurs et utilisateurs finaux',
  'ESRS G1 - Conduite des affaires'
]

export default function Materiality() {
  const toast = useToast()
  const [mat, setMat] = useState<Record<string, boolean>>({...CompanyService.getMateriality()})
  const toggle = (k: string) => setMat(m => ({...m, [k]: !m[k]}))
  const save = () => { CompanyService.saveMateriality(mat); toast.success('Matérialité enregistrée') }

  return (
    <div className="max-w-3xl space-y-6">
      <div>
        <h1 className="text-2xl font-semibold">Analyse de matérialité</h1>
        <p className="text-slate-600">Sélectionnez les normes ESRS pertinentes pour votre entreprise.</p>
      </div>

      <div className="card grid sm:grid-cols-2 gap-3">
        {TOPICS.map(t => (
          <label key={t} className={`flex items-center gap-3 rounded-xl border p-3 ${mat[t] ? 'border-brand-400 bg-brand-50' : 'border-slate-200'}`}>
            <input type="checkbox" checked={!!mat[t]} onChange={()=>toggle(t)} />
            <span className="text-sm">{t}</span>
          </label>
        ))}
      </div>

      <button className="btn btn-primary" onClick={save}>Enregistrer</button>
    </div>
  )
}

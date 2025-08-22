import { CompanyService } from '../services/CompanyService'

const ITEMS = [
  { id: 'esrs2-org', label: 'ESRS 2 — Informations générales complétées' },
  { id: 'e1-ges', label: 'ESRS E1 — Bilans GES renseignés (S1/S2/S3)' },
  { id: 'targets', label: 'Objectifs et trajectoires renseignés' },
  { id: 'policies', label: 'Politiques documentées' },
  { id: 'risks', label: 'Risques et opportunités documentés' }
]

export default function ComplianceChecklist() {
  const mat = CompanyService.getMateriality()
  const data = CompanyService.getDataCollection()
  const checks: Record<string, boolean> = {
    'esrs2-org': true,
    'e1-ges': true,
    'targets': !!data.targets,
    'policies': !!data.policies,
    'risks': !!data.risks
  }

  return (
    <div className="max-w-3xl space-y-6">
      <div>
        <h1 className="text-2xl font-semibold">Conformité — Checklist</h1>
        <p className="text-slate-600">Aperçu synthétique des points clés à compléter.</p>
      </div>

      <div className="card space-y-2">
        {ITEMS.map(i => (
          <label key={i.id} className="flex items-center gap-3">
            <input type="checkbox" checked={!!checks[i.id]} readOnly />
            <span className="text-sm">{i.label}</span>
          </label>
        ))}
      </div>

      <div className="card">
        <h3 className="font-semibold mb-2">Normes ESRS sélectionnées</h3>
        <ul className="text-sm text-slate-700">
          {Object.entries(mat).filter(([,v]) => v).map(([k]) => <li key={k}>• {k}</li>)}
        </ul>
      </div>
    </div>
  )
}

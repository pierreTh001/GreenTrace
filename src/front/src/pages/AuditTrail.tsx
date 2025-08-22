import { AuditService } from '../services/AuditService'

export default function AuditTrail() {
  const events = AuditService.list()

  return (
    <div className="max-w-3xl space-y-6">
      <div>
        <h1 className="text-2xl font-semibold">Journal d'audit</h1>
        <p className="text-slate-600">Historique des actions (mock).</p>
      </div>

      <div className="card">
        <table className="w-full text-sm">
          <thead>
            <tr className="text-left text-slate-500">
              <th className="py-2">Date</th>
              <th className="py-2">Acteur</th>
              <th className="py-2">Action</th>
            </tr>
          </thead>
          <tbody>
            {events.map(ev => (
              <tr key={ev.id} className="border-t">
                <td className="py-2">{new Date(ev.ts).toLocaleString('fr-FR')}</td>
                <td className="py-2">{ev.actor}</td>
                <td className="py-2">{ev.action}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  )
}

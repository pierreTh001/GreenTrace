import { useEffect, useMemo, useState } from 'react'
import { ApiClient } from '../services/ApiClient'
import { useToast } from '../context/ToastContext'
import { Plus, Pencil, Trash } from 'lucide-react'

type AdminUserRow = {
  id: string
  email: string
  firstName: string
  lastName: string
  companies: string
  planName: string | null
}

export default function Admin() {
  const toast = useToast()
  const [tab, setTab] = useState<'users'>('users')
  const [rows, setRows] = useState<AdminUserRow[]>([])
  const [loading, setLoading] = useState(false)
  const [q, setQ] = useState('')
  const [editId, setEditId] = useState<string | null>(null)
  const [fn, setFn] = useState('')
  const [ln, setLn] = useState('')
  const [eRole, setERole] = useState<'User'|'Admin'>('User')
  const [ePlanName, setEPlanName] = useState<string>('—')
  const [showCreate, setShowCreate] = useState(false)
  const [cEmail, setCEmail] = useState('')
  const [cPassword, setCPassword] = useState('')
  const [cFirst, setCFirst] = useState('')
  const [cLast, setCLast] = useState('')
  const [cRole, setCRole] = useState<'User'|'Admin'>('User')

  async function refresh() {
    try {
      setLoading(true)
      const data = await ApiClient.get<AdminUserRow[]>('/api/users/overview')
      setRows(data)
    } catch { toast.error('Chargement des utilisateurs impossible') }
    finally { setLoading(false) }
  }

  useEffect(() => { refresh() }, [])

  const filtered = useMemo(() => {
    const needle = q.trim().toLowerCase()
    if (!needle) return rows
    return rows.filter(r => [r.email, r.firstName, r.lastName, r.companies, r.planName || '']
      .some(v => (v || '').toLowerCase().includes(needle)))
  }, [rows, q])

  const onEdit = async (r: AdminUserRow) => {
    setEditId(r.id); setFn(r.firstName || ''); setLn(r.lastName || '')
    setEPlanName(r.planName || '—')
    try {
      const detail = await ApiClient.get<any>(`/api/users/${r.id}`)
      const roles = Array.isArray(detail.roles) ? detail.roles : []
      const isAdmin = roles.includes('Admin')
      setERole(isAdmin ? 'Admin' : 'User')
    } catch {}
  }
  const onSave = async () => {
    if (!editId) return
    try {
      await ApiClient.put(`/api/users/${editId}`, { firstName: fn, lastName: ln })
      await ApiClient.put(`/api/users/${editId}/role`, { role: eRole })
      toast.success('Utilisateur mis à jour')
      setEditId(null); refresh()
    }
    catch { toast.error('Échec de mise à jour') }
  }
  const onDelete = async (id: string) => {
    if (!confirm('Êtes-vous sûr ? Cette action supprime l’utilisateur et toutes ses données (entreprises, rapports…).')) return
    try { await ApiClient.del(`/api/users/${id}`); toast.success('Utilisateur supprimé'); refresh() }
    catch { toast.error('Suppression impossible') }
  }

  const onCreate = async () => {
    try {
      await ApiClient.post('/api/users', { email: cEmail, password: cPassword, firstName: cFirst, lastName: cLast, role: cRole })
      toast.success('Utilisateur créé')
      setShowCreate(false)
      setCEmail(''); setCPassword(''); setCFirst(''); setCLast(''); setCRole('User')
      refresh()
    } catch { toast.error('Création impossible') }
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-semibold">Administration</h1>
      </div>

      <div className="flex items-center justify-between border-b border-slate-200">
        <div className="flex gap-2">
          <button className={`px-3 py-2 ${tab==='users' ? 'border-b-2 border-brand-600 text-brand-700' : 'text-slate-600'}`} onClick={()=>setTab('users')}>Gestion des utilisateurs</button>
        </div>
      </div>

      {tab==='users' && (
        <div className="space-y-4">
      <div className="flex items-center justify-between gap-3">
        <div className="relative w-full max-w-md">
          <input className="input w-full pr-10" placeholder="Rechercher (nom, prénom, email, entreprise, abonnement)" value={q} onChange={e=>setQ(e.target.value)} />
          {q && (
            <button aria-label="Effacer" className="absolute right-2 top-1/2 -translate-y-1/2 text-slate-500 hover:text-slate-700" onClick={()=>setQ('')}>×</button>
          )}
        </div>
        <button className="btn btn-primary" onClick={()=>setShowCreate(true)}><Plus size={16} className="mr-2"/> Ajouter</button>
      </div>
          <div className="overflow-auto border border-slate-200 rounded-xl">
            <table className="min-w-full text-sm">
              <thead className="bg-slate-50 text-slate-600">
                <tr>
                  <th className="text-left px-3 py-2">Nom</th>
                  <th className="text-left px-3 py-2">Prénom</th>
                  <th className="text-left px-3 py-2">Email</th>
                  <th className="text-left px-3 py-2">Entreprise(s)</th>
                  <th className="px-3 py-2">Éditer</th>
                  <th className="px-3 py-2">Supprimer</th>
                </tr>
              </thead>
              <tbody>
                {loading && (
                  <tr><td className="px-3 py-3" colSpan={6}>Chargement…</td></tr>
                )}
                {!loading && filtered.map(r => (
                  <tr key={r.id} className="border-t border-slate-100">
                    <td className="px-3 py-2">{r.lastName || '—'}</td>
                    <td className="px-3 py-2">{r.firstName || '—'}</td>
                    <td className="px-3 py-2">{r.email || '—'}</td>
                    <td className="px-3 py-2">{r.companies || '—'}</td>
                    <td className="px-3 py-2 text-center">
                      <button className="text-slate-600 hover:text-slate-900" title="Éditer" onClick={()=>onEdit(r)}>
                        <Pencil size={16} />
                      </button>
                    </td>
                    <td className="px-3 py-2 text-center">
                      <button className="text-slate-600 hover:text-red-700" title="Supprimer" onClick={()=>onDelete(r.id)}>
                        <Trash size={16} />
                      </button>
                    </td>
                  </tr>
                ))}
                {!loading && filtered.length === 0 && (
                  <tr><td className="px-3 py-3 text-slate-500" colSpan={6}>Aucun résultat</td></tr>
                )}
              </tbody>
            </table>
          </div>

          {editId && (
            <div className="fixed inset-0 bg-black/30 flex items-center justify-center z-50">
              <div className="bg-white rounded-2xl p-5 w-full max-w-md shadow-lg">
                <h3 className="font-semibold mb-3">Éditer l’utilisateur</h3>
                <div className="grid gap-3">
                  <div>
                    <label className="label">Prénom</label>
                    <input className="input" value={fn} onChange={e=>setFn(e.target.value)} />
                  </div>
                  <div>
                    <label className="label">Nom</label>
                    <input className="input" value={ln} onChange={e=>setLn(e.target.value)} />
                  </div>
                  <div>
                    <label className="label">Rôle</label>
                    <select className="input" value={eRole} onChange={e=>setERole(e.target.value as any)}>
                      <option value="User">User</option>
                      <option value="Admin">Admin</option>
                    </select>
                  </div>
                  <div>
                    <label className="label">Abonnement</label>
                    <input className="input" value={ePlanName} disabled />
                  </div>
                </div>
                <div className="mt-4 flex justify-end gap-2">
                  <button className="btn btn-outline" onClick={()=>setEditId(null)}>Annuler</button>
                  <button className="btn btn-primary" onClick={onSave}>Enregistrer</button>
                </div>
              </div>
            </div>
          )}

          {showCreate && (
            <div className="fixed inset-0 bg-black/30 flex items-center justify-center z-50">
              <div className="bg-white rounded-2xl p-5 w-full max-w-md shadow-lg">
                <h3 className="font-semibold mb-3">Créer un utilisateur</h3>
                <div className="grid gap-3">
                  <div>
                    <label className="label">Email</label>
                    <input className="input" type="email" value={cEmail} onChange={e=>setCEmail(e.target.value)} />
                  </div>
                  <div>
                    <label className="label">Mot de passe</label>
                    <input className="input" type="password" value={cPassword} onChange={e=>setCPassword(e.target.value)} />
                  </div>
                  <div className="grid grid-cols-2 gap-3">
                    <div>
                      <label className="label">Prénom</label>
                      <input className="input" value={cFirst} onChange={e=>setCFirst(e.target.value)} />
                    </div>
                    <div>
                      <label className="label">Nom</label>
                      <input className="input" value={cLast} onChange={e=>setCLast(e.target.value)} />
                    </div>
                  </div>
                  <div>
                    <label className="label">Rôle</label>
                    <select className="input" value={cRole} onChange={e=>setCRole(e.target.value as any)}>
                      <option value="User">User</option>
                      <option value="Admin">Admin</option>
                    </select>
                  </div>
                </div>
                <div className="mt-4 flex justify-end gap-2">
                  <button className="btn btn-outline" onClick={()=>setShowCreate(false)}>Annuler</button>
                  <button className="btn btn-primary" onClick={onCreate} disabled={!cEmail || !cPassword || !cFirst || !cLast}>Créer</button>
                </div>
              </div>
            </div>
          )}
        </div>
      )}
    </div>
  )
}

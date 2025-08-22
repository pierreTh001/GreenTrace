import { useState } from 'react'
import { useAuth } from '../context/AuthContext'
import { UserService } from '../services/UserService'

export default function Users() {
  const { user } = useAuth()
  if (user?.role !== 'Admin') return <div className="text-red-600">Accès réservé aux administrateurs.</div>

  const [list, setList] = useState(UserService.list())
  const [email, setEmail] = useState('nouveau@exemple.com')
  const [role, setRole] = useState<'Admin'|'User'>('User')

  const refresh = () => setList(UserService.list())

  const add = () => { UserService.create(email, role); refresh() }
  const remove = (id: string) => { UserService.remove(id); refresh() }

  return (
    <div className="space-y-6 max-w-3xl">
      <div>
        <h1 className="text-2xl font-semibold">Utilisateurs</h1>
        <p className="text-slate-600">Gérez les accès à la plateforme.</p>
      </div>

      <div className="card grid sm:grid-cols-[1fr_160px_120px_auto] gap-2 items-start">
        <input className="input" value={email} onChange={e=>setEmail(e.target.value)} placeholder="email" />
        <select className="input" value={role} onChange={e=>setRole(e.target.value as any)}>
          <option value="User">User</option>
          <option value="Admin">Admin</option>
        </select>
        <button className="btn btn-primary" onClick={add}>Ajouter</button>
      </div>

      <div className="card">
        <table className="w-full text-sm">
          <thead>
            <tr className="text-left text-slate-500">
              <th className="py-2">Email</th>
              <th className="py-2">Rôle</th>
              <th className="py-2"></th>
            </tr>
          </thead>
          <tbody>
            {list.map(u => (
              <tr key={u.id} className="border-t">
                <td className="py-2">{u.email}</td>
                <td className="py-2">{u.role}</td>
                <td className="py-2 text-right">
                  <button className="btn btn-outline" onClick={()=>remove(u.id)}>Supprimer</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  )
}

import { useState } from 'react'
import { useAuth } from '../context/AuthContext'
import { UserService } from '../services/UserService'

export default function Settings() {
  const { user } = useAuth()
  const [password, setPassword] = useState('')
  const [message, setMessage] = useState<string | null>(null)

  const changePw = () => {
    if (!user) return
    UserService.update(user.id, { password })
    setMessage('Mot de passe mis à jour (mock).')
    setPassword('')
  }

  return (
    <div className="max-w-xl space-y-6">
      <div>
        <h1 className="text-2xl font-semibold">Paramètres du compte</h1>
        <p className="text-slate-600">Mettre à jour votre mot de passe (mock).</p>
      </div>

      <div className="card space-y-3">
        <label className="label">Nouveau mot de passe</label>
        <input className="input" type="password" value={password} onChange={e=>setPassword(e.target.value)} />
        <button className="btn btn-primary w-fit" onClick={changePw}>Mettre à jour</button>
        {message && <div className="text-green-700 text-sm">{message}</div>}
      </div>
    </div>
  )
}

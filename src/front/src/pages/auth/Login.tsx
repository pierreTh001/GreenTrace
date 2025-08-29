import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../../context/AuthContext'

export default function Login() {
  const [email, setEmail] = useState('admin@thiebert.me')
  const [password, setPassword] = useState('admin')
  const [error, setError] = useState<string | null>(null)
  const { login } = useAuth()
  const nav = useNavigate()

  const onSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    const ok = await login(email, password)
    if (!ok) setError("Identifiants invalides")
    else nav('/app')
  }

  return (
    <div className="min-h-screen grid md:grid-cols-2">
      <div className="hidden md:block bg-gradient-to-br from-brand-600 to-emerald-400 text-white p-10">
        <div className="max-w-lg mt-20">
          <h1 className="text-4xl font-bold">GreenTrace</h1>
          <p className="mt-4 text-white/90">Générez des rapports CSRD conformes, collectez vos données et pilotez vos indicateurs RSE. 100% mock API pour démo.</p>
          <ul className="mt-6 space-y-2 text-white/90">
            <li>• ESRS E/S/G</li>
            <li>• Bilans GES S1/S2/S3</li>
            <li>• Export PDF</li>
          </ul>
        </div>
      </div>
      <div className="flex items-center justify-center p-8">
        <form onSubmit={onSubmit} className="card w-full max-w-md">
          <h2 className="text-xl font-semibold">Connexion</h2>
          <p className="text-sm text-slate-500 mb-4">Utilisez le compte de démo: admin@thiebert.me / admin</p>
          <label className="label">Email</label>
          <input className="input mb-3" type="email" value={email} onChange={e=>setEmail(e.target.value)} required />
          <label className="label">Mot de passe</label>
          <input className="input mb-3" type="password" value={password} onChange={e=>setPassword(e.target.value)} required />
          {error && <div className="text-red-600 text-sm mb-3">{error}</div>}
          <button className="btn btn-primary w-full">Se connecter</button>
          <div className="mt-3 text-sm text-center">
            Pas de compte ? <Link to="/auth/register" className="text-brand-600 hover:underline">Créer un compte</Link>
          </div>
        </form>
      </div>
    </div>
  )
}

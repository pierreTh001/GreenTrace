import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../../context/AuthContext'

export default function Register() {
  const [email, setEmail] = useState('user@example.com')
  const [password, setPassword] = useState('user12345')
  const [error, setError] = useState<string | null>(null)
  const { register } = useAuth()
  const nav = useNavigate()

  const onSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    const ok = await register(email, password)
    if (!ok) setError("Un compte existe déjà avec cet email")
    else nav('/app')
  }

  return (
    <div className="min-h-screen grid md:grid-cols-2">
      <div className="hidden md:block bg-gradient-to-br from-brand-600 to-emerald-400 text-white p-10">
        <div className="max-w-lg mt-20">
          <h1 className="text-4xl font-bold">Bienvenue !</h1>
          <p className="mt-4 text-white/90">Créez votre espace et commencez la collecte de données RSE.</p>
        </div>
      </div>
      <div className="flex items-center justify-center p-8">
        <form onSubmit={onSubmit} className="card w-full max-w-md">
          <h2 className="text-xl font-semibold">Créer un compte</h2>
          <label className="label">Email</label>
          <input className="input mb-3" type="email" value={email} onChange={e=>setEmail(e.target.value)} required />
          <label className="label">Mot de passe</label>
          <input className="input mb-4" type="password" value={password} onChange={e=>setPassword(e.target.value)} required />
          {error && <div className="text-red-600 text-sm mb-3">{error}</div>}
          <button className="btn btn-primary w-full">Créer mon compte</button>
          <div className="mt-3 text-sm text-center">
            Déjà inscrit ? <Link to="/auth/login" className="text-brand-600 hover:underline">Connexion</Link>
          </div>
        </form>
      </div>
    </div>
  )
}

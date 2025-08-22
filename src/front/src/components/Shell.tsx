import { Outlet, NavLink, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'
import logo from '../assets/logo.svg'
import { LogOut, FileText, Home, Factory, Settings, Users, CheckCircle, BarChart3, ClipboardList } from 'lucide-react'

export default function Shell() {
  const { user, logout } = useAuth()
  const navigate = useNavigate()

  const nav = [
    { to: '/app', label: 'Tableau de bord', icon: Home },
    { to: '/app/company', label: 'Entreprise', icon: Factory },
    { to: '/app/materiality', label: 'Matérialité ESRS', icon: ClipboardList },
    { to: '/app/data', label: 'Collecte des données', icon: BarChart3 },
    { to: '/app/emissions', label: 'Bilans GES (S1/S2/S3)', icon: CheckCircle },
    { to: '/app/report', label: 'Rapport CSRD', icon: FileText },
    { to: '/app/users', label: 'Utilisateurs', icon: Users, admin: true },
    { to: '/app/compliance', label: 'Conformité', icon: CheckCircle },
    { to: '/app/audit', label: 'Journal', icon: FileText }
  ]

  return (
    <div className="flex min-h-screen">
      <aside className="w-[280px] border-r border-slate-200 bg-white hidden md:flex md:flex-col">
        <div className="flex items-center gap-3 p-5">
          <img src={logo} className="h-7 w-7" />
          <div className="font-semibold">GreenTrace</div>
        </div>
        <nav className="p-3 flex-1 space-y-1">
          {nav.filter(n => !n.admin || user?.role === 'Admin').map(item => {
            const Icon = item.icon
            return (
              <NavLink key={item.to} to={item.to} className={({isActive}) =>
                `flex items-center gap-3 rounded-xl px-3 py-2 text-sm ${isActive ? 'bg-brand-50 text-brand-700' : 'text-slate-700 hover:bg-slate-100'}`}>
                <Icon size={18} />
                {item.label}
              </NavLink>
            )
          })}
        </nav>
        <div className="p-4 border-t border-slate-200">
          <div className="mb-2 text-xs text-slate-500">Connecté en tant que</div>
          <div className="flex items-center justify-between">
            <div className="text-sm font-medium">{user?.email}</div>
            <button className="btn btn-outline" onClick={() => { logout(); navigate('/auth/login') }}>
              <LogOut size={16} className="mr-2" /> Quitter
            </button>
          </div>
        </div>
      </aside>
      <main className="flex-1">
        <header className="sticky top-0 z-10 flex items-center justify-between border-b border-slate-200 bg-white/70 backdrop-blur p-4 md:hidden">
          <div className="flex items-center gap-2">
            <img src={logo} className="h-6 w-6" />
            <span className="font-semibold">GreenTrace</span>
          </div>
          <button className="btn btn-outline" onClick={() => { logout(); navigate('/auth/login') }}>
            <LogOut size={16} className="mr-2"/> Quitter
          </button>
        </header>
        <div className="p-5 md:p-8">
          <Outlet />
        </div>
      </main>
    </div>
  )
}

import { Link } from 'react-router-dom'
import { useEffect, useState } from 'react'
import { SubscriptionsService } from '../services/SubscriptionsService'
import { EmissionsService } from '../services/EmissionsService'
import { CompanyService } from '../services/CompanyService'

export default function Dashboard() {
  const totals = EmissionsService.totals()
  const company = CompanyService.get()
  const [hasSub, setHasSub] = useState<boolean>(false)
  useEffect(() => { SubscriptionsService.me().then(s=>setHasSub(!!s)).catch(()=>setHasSub(false)) }, [])

  const items = [
    { title: 'Complétion fiche entreprise', value: `${Math.min(100, Math.round((['name','sector','employees','revenue','headquarters'].filter(k => (company as any)[k]) .length/5)*100))}%` },
    { title: 'Total GES', value: `${totals.total.toFixed(1)} tCO₂e` },
    { title: 'Matérialité active', value: 'ESRS E1, E5, S1, S4, G1' },
  ]

  if (!hasSub) {
    return (
      <div className="max-w-2xl">
        <h1 className="text-2xl font-semibold mb-2">Bienvenue sur GreenTrace</h1>
        <p className="text-slate-600 mb-4">Pour utiliser l’application, vous devez souscrire à une offre.</p>
        <Link to="/app/settings" className="btn btn-primary">Gérer mon abonnement</Link>
      </div>
    )
  }

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-semibold">Tableau de bord</h1>
        <p className="text-slate-500">Bienvenue dans GreenTrace. Continuez votre parcours ci-dessous.</p>
      </div>

      <div className="grid md:grid-cols-3 gap-4">
        {items.map((k) => (
          <div key={k.title} className="card">
            <div className="text-slate-500 text-sm">{k.title}</div>
            <div className="text-2xl font-semibold mt-2">{k.value}</div>
          </div>
        ))}
      </div>

      <div className="grid lg:grid-cols-3 gap-5">
        <div className="card">
          <h3 className="font-semibold mb-2">1) Fiche entreprise</h3>
          <p className="text-sm text-slate-600 mb-3">Renseignez les informations générales requises par ESRS 2.</p>
          <Link to="/app/company" className="btn btn-primary">Compléter</Link>
        </div>
        <div className="card">
          <h3 className="font-semibold mb-2">2) Matérialité</h3>
          <p className="text-sm text-slate-600 mb-3">Sélectionnez les normes ESRS pertinentes.</p>
          <Link to="/app/materiality" className="btn btn-primary">Analyser</Link>
        </div>
        <div className="card">
          <h3 className="font-semibold mb-2">3) Bilans GES</h3>
          <p className="text-sm text-slate-600 mb-3">Renseignez vos émissions (S1/S2/S3).</p>
          <Link to="/app/emissions" className="btn btn-primary">Saisir</Link>
        </div>
        <div className="card">
          <h3 className="font-semibold mb-2">4) Collecte des données</h3>
          <p className="text-sm text-slate-600 mb-3">Politiques, risques, objectifs et indicateurs.</p>
          <Link to="/app/data" className="btn btn-primary">Ouvrir</Link>
        </div>
        <div className="card">
          <h3 className="font-semibold mb-2">5) Conformité</h3>
          <p className="text-sm text-slate-600 mb-3">Checklist synthétique de conformité.</p>
          <Link to="/app/compliance" className="btn btn-primary">Vérifier</Link>
        </div>
        <div className="card">
          <h3 className="font-semibold mb-2">6) Rapport CSRD</h3>
          <p className="text-sm text-slate-600 mb-3">Prévisualiser et exporter en PDF.</p>
          <Link to="/app/report" className="btn btn-primary">Générer</Link>
        </div>
      </div>
    </div>
  )
}

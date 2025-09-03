import { useEffect, useState } from 'react'
import { useAuth } from '../context/AuthContext'
import { useToast } from '../context/ToastContext'
import { SubscriptionsService, type Subscription, type SubscriptionPlan } from '../services/SubscriptionsService'
import { ApiClient } from '../services/ApiClient'

export default function Settings() {
  const { user } = useAuth()
  const toast = useToast()

  const [firstName, setFirstName] = useState('')
  const [lastName, setLastName] = useState('')
  const [oldPw, setOldPw] = useState('')
  const [newPw, setNewPw] = useState('')
  const [confirmPw, setConfirmPw] = useState('')

  const [plans, setPlans] = useState<SubscriptionPlan[]>([])
  const [active, setActive] = useState<Subscription | null>(null)
  const [loadingPlans, setLoadingPlans] = useState(false)
  const [deletedAt, setDeletedAt] = useState<string | null>(null)
  const formatDate = (s?: string | null) => {
    if (!s) return '—'
    const d = new Date(s)
    return d.toLocaleDateString('fr-FR', { year: 'numeric', month: 'long', day: 'numeric' })
  }
  // Prochain renouvellement (mensuel)
  const nextMonthlyRenewal = (startedAt?: string): string => {
    if (!startedAt) return '—'
    const start = new Date(startedAt)
    const now = new Date()
    let next = new Date(start)
    // avancer par mois jusqu'à dépasser maintenant
    while (next <= now) {
      const y = next.getFullYear()
      const idx = next.getMonth() // 0-based
      const d = next.getDate()
      // dernier jour du mois suivant
      const lastDayNextMonth = new Date(y, idx + 2, 0).getDate()
      next = new Date(y, idx + 1, Math.min(d, lastDayNextMonth), next.getHours(), next.getMinutes(), next.getSeconds())
    }
    return next.toISOString()
  }
  // Fin d'engagement (annuel) — prochaine échéance annuelle > maintenant
  const annualTermEnd = (startedAt?: string): string => {
    if (!startedAt) return '—'
    const start = new Date(startedAt)
    const now = new Date()
    let end = new Date(start)
    while (end <= now) end = new Date(end.getFullYear() + 1, end.getMonth(), end.getDate(), end.getHours(), end.getMinutes(), end.getSeconds())
    return end.toISOString()
  }

  useEffect(() => {
    // Fetch subscription data
    (async () => {
      try {
        setLoadingPlans(true)
        const [pl, me] = await Promise.all([
          SubscriptionsService.plans().catch(()=>[]),
          SubscriptionsService.me().catch(()=>null)
        ])
        setPlans(pl || [])
        setActive(me || null)
      } finally { setLoadingPlans(false) }
    })()
    // Prefill profile (self)
    ;(async () => {
      try {
        const me = await ApiClient.get<any>(`/api/users/me`)
        setFirstName(me.firstName || '')
        setLastName(me.lastName || '')
        setDeletedAt(me.deletedAt || null)
      } catch {}
    })()
  }, [])

  const saveProfile = async () => {
    try {
      await ApiClient.put(`/api/users/me`, { firstName, lastName })
      toast.success('Profil mis à jour')
    } catch (e:any) {
      toast.error('Échec mise à jour du profil')
    }
  }

  const canChangePw = oldPw.length > 0 && newPw.length >= 6 && confirmPw.length > 0 && newPw === confirmPw

  const changePw = async () => {
    if (!canChangePw) return
    try {
      await ApiClient.post('/api/auth/change-password', { oldPassword: oldPw, newPassword: newPw })
      setOldPw(''); setNewPw(''); setConfirmPw('')
      toast.success('Mot de passe mis à jour')
    } catch (e:any) {
      const msg = String(e?.message || e || '')
      if (msg.startsWith('400')) toast.error('Ancien mot de passe invalide')
      else toast.error('Échec du changement de mot de passe')
    }
  }

  const subscribe = async (planId: string) => {
    try {
      const sub = await SubscriptionsService.subscribe(planId)
      setActive(sub)
      setDeletedAt(null) // subscribing reactivates account
      toast.success('Abonnement activé')
      // Notifier l'app pour rafraîchir le menu sans recharger la page
      window.dispatchEvent(new Event('greentrace:subscription:changed'))
    } catch { toast.error('Échec de la souscription') }
  }

  const cancel = async () => {
    try {
      const sub = await SubscriptionsService.cancel()
      setActive(sub)
      toast.warning("Renouvellement automatique désactivé")
      window.dispatchEvent(new Event('greentrace:subscription:changed'))
    } catch { toast.error('Échec de l’annulation') }
  }

  const deleteAccount = async () => {
    if (!user) return
    if (!confirm("Êtes-vous sûr ? Toutes les données (entreprise, rapports, etc.) seront supprimées.")) return
    try {
      const res = await ApiClient.del<any>(`/api/users/me`)
      // If API returns no content, request succeeds without JSON; our ApiClient returns undefined in that case
      if (!res) { toast.success('Compte supprimé. Vous pouvez vous déconnecter.'); setDeletedAt(null); return }
      if (res?.scheduledDeletionAt) {
        const when = new Date(res.scheduledDeletionAt).toLocaleDateString('fr-FR', { year:'numeric', month:'long', day:'numeric' })
        toast.warning(`Abonnement annulé. Le compte sera supprimé le ${when}.`)
        setDeletedAt(res.scheduledDeletionAt)
        // Refresh subscription panel state
        try { const s = await SubscriptionsService.me(); setActive(s) } catch {}
      } else {
        toast.warning('Abonnement annulé. Le compte sera supprimé à la fin de votre engagement.')
      }
    } catch {
      toast.error('Suppression impossible')
    }
  }

  const reactivateAccount = async () => {
    try {
      await ApiClient.post('/api/users/me/reactivate', {})
      setDeletedAt(null)
      toast.success('Compte réactivé')
    } catch { toast.error('Impossible de réactiver le compte') }
  }

  return (
    <div className="max-w-3xl space-y-8">
      <div>
        <h1 className="text-2xl font-semibold">Gestion du compte</h1>
        <p className="text-slate-600">Gérer vos informations, mot de passe, abonnement et suppression du compte.</p>
      </div>

      <section className="card space-y-4">
        <h2 className="font-semibold">Profil</h2>
        <div className="grid sm:grid-cols-2 gap-4">
          <div className="sm:col-span-2">
            <label className="label">Email</label>
            <input className="input" value={user?.email || ''} disabled />
          </div>
          <div>
            <label className="label">Prénom</label>
            <input className="input" value={firstName} onChange={e=>setFirstName(e.target.value)} placeholder="(admin requis pour sauvegarder)" />
          </div>
          <div>
            <label className="label">Nom</label>
            <input className="input" value={lastName} onChange={e=>setLastName(e.target.value)} placeholder="(admin requis pour sauvegarder)" />
          </div>
        </div>
        <button className="btn btn-primary w-fit" onClick={saveProfile}>Enregistrer</button>
      </section>

      <section className="card space-y-4">
        <h2 className="font-semibold">Mot de passe</h2>
        <div className="grid sm:grid-cols-3 gap-4">
          <div className="sm:col-span-1">
            <label className="label">Ancien mot de passe</label>
            <input className="input" type="password" value={oldPw} onChange={e=>setOldPw(e.target.value)} />
          </div>
          <div className="sm:col-span-1">
            <label className="label">Nouveau mot de passe</label>
            <input className="input" type="password" value={newPw} onChange={e=>setNewPw(e.target.value)} />
          </div>
          <div className="sm:col-span-1">
            <label className="label">Confirmer</label>
            <input className="input" type="password" value={confirmPw} onChange={e=>setConfirmPw(e.target.value)} />
          </div>
        </div>
        <div className="flex items-center gap-3">
          <button className="btn btn-primary disabled:opacity-50" disabled={!canChangePw} onClick={changePw}>Mettre à jour</button>
          {!canChangePw && <div className="text-xs text-slate-500">Remplir les 3 champs et assurer la concordance</div>}
        </div>
      </section>

      <section className="card space-y-4">
        <h2 className="font-semibold">Abonnement</h2>
        {active ? (() => {
          const ap = plans.find(p => p.id === active.planId)
          const name = ap?.label || ap?.code || active.planId
          const nextIso = nextMonthlyRenewal(active.startedAt)
          const isCanceled = active.status !== 'Active' && active.endsAt
          const end = active.endsAt || annualTermEnd(active.startedAt)
          let nextToShow: string | null = null
          if (nextIso && nextIso !== '—') {
            const show = !isCanceled || (new Date(nextIso) <= new Date(end))
            nextToShow = show ? nextIso : null
          }
          return (
            <div className="space-y-2 text-sm">
              <div>Abonnement: <b>{name}</b> — statut <b>{active.status}</b></div>
              <div>Date de souscription: <b>{formatDate(active.startedAt)}</b></div>
              <div>Prochain prélèvement: <b>{formatDate(nextToShow || undefined)}</b></div>
              <div>Engagé jusqu’au: <b>{formatDate(end)}</b></div>
              <div className="flex gap-2">
                <button className="btn btn-outline disabled:opacity-50" onClick={cancel} disabled={isCanceled}>Supprimer l’abonnement</button>
                {isCanceled && <div className="text-slate-600">Fin de l’abonnement le: <b>{formatDate(end)}</b></div>}
              </div>
            </div>
          )
        })() : (
          <div className="text-sm text-slate-500">Aucun abonnement actif</div>
        )}
        <div className="grid sm:grid-cols-3 gap-3">
          {loadingPlans && <div className="text-sm text-slate-500">Chargement des offres…</div>}
          {!loadingPlans && plans.map(p => (
            <button key={p.id} className={`btn ${active?.planId === p.id ? 'btn-primary' : 'btn-outline'}`} onClick={()=>subscribe(p.id)} disabled={active?.planId === p.id}>
              {p.code}
            </button>
          ))}
        </div>
      </section>

      <section className="card space-y-3">
        <h2 className="font-semibold text-red-700">Gestion du compte</h2>
        {!deletedAt ? (
          <>
            <div className="text-sm text-slate-600">Supprimer votre compte supprimera toutes vos données (entreprise, rapports, etc.).</div>
            <button className="btn btn-outline" onClick={deleteAccount}>Supprimer mon compte</button>
          </>
        ) : (
          <>
            <div className="text-sm text-slate-600">Compte supprimé: effectif le <b>{formatDate(deletedAt)}</b></div>
            <button className="btn btn-primary" onClick={reactivateAccount}>Réactiver mon compte</button>
          </>
        )}
      </section>
    </div>
  )
}

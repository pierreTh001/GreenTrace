import { ApiClient } from './ApiClient'
import { CompanyService } from './CompanyService'
import { AuditService } from './AuditService'

export type User = { id: string; email: string; role: 'Admin' | 'User' }

const CURRENT = 'greentrace-current-user'
const TOKEN = 'greentrace-token'
const listeners = new Set<(u: User | null) => void>()

function notify(u: User | null) { listeners.forEach(cb => { try { cb(u) } catch {} }) }

function setCurrent(user: User | null) {
  if (user) localStorage.setItem(CURRENT, JSON.stringify(user))
  else localStorage.removeItem(CURRENT)
  notify(user)
}

function setToken(token: string | null) {
  if (token) localStorage.setItem(TOKEN, token)
  else localStorage.removeItem(TOKEN)
}

async function ensureSubscribed() {
  // Try to subscribe to BASIC if no active subscription
  try {
    const me = await ApiClient.get<any>('/api/subscriptions/me')
    if (!me) {
      const plans = await ApiClient.get<any[]>('/api/subscriptions/plans')
      const basic = plans.find(p => p.code === 'BASIC') || plans[0]
      if (basic) await ApiClient.post<any>('/api/subscriptions/subscribe', { planId: basic.id })
    }
  } catch {}
}

export const AuthService = {
  subscribe(cb: (u: User | null) => void) {
    listeners.add(cb); return () => { listeners.delete(cb) }
  },
  currentUser(): User | null {
    try { return JSON.parse(localStorage.getItem(CURRENT) || 'null') } catch { return null }
  },
  token(): string | null { return localStorage.getItem(TOKEN) },
  async login(email: string, password: string): Promise<boolean> {
    try {
      const res = await ApiClient.post<{ token: string }>('/api/auth/login', { email, password })
      setToken(res.token)
      setCurrent({ id: 'me', email, role: 'User' })
      AuditService.log(email, 'Connexion')
      // Lancer l'initialisation en arrière-plan pour ne pas bloquer la connexion
      ensureSubscribed().catch(()=>{})
      CompanyService.bootstrapFromApi().catch(()=>{})
      return true
    } catch {
      return false
    }
  },
  async register(email: string, password: string, firstName: string, lastName: string): Promise<boolean> {
    try {
      const res = await ApiClient.post<{ token: string }>('/api/auth/register', { email, password, firstName, lastName })
      setToken(res.token)
      setCurrent({ id: 'me', email, role: 'User' })
      AuditService.log(email, 'Inscription')
      // Ne bloque pas la réussite d'inscription si l'init échoue
      ensureSubscribed().catch(()=>{})
      CompanyService.bootstrapFromApi().catch(()=>{})
      return true
    } catch {
      return false
    }
  },
  logout() {
    const u = AuthService.currentUser()
    if (u) AuditService.log(u.email, 'Déconnexion')
    setToken(null)
    setCurrent(null)
  }
}

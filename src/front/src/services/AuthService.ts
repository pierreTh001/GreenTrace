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

function decodeJwt(token: string): any | null {
  try {
    const payload = token.split('.')?.[1]
    if (!payload) return null
    const json = atob(payload.replace(/-/g, '+').replace(/_/g, '/'))
    return JSON.parse(decodeURIComponent(escape(json)))
  } catch { return null }
}

// No auto-subscription; users start without a plan

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
      const payload = decodeJwt(res.token)
      const roles: string[] = Array.isArray(payload?.["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]) ? payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] : []
      const role: 'Admin'|'User' = roles.includes('Admin') ? 'Admin' : 'User'
      const id: string = payload?.sub || 'me'
      setCurrent({ id, email, role })
      AuditService.log(email, 'Connexion')
      // Init (may be limited by subscription policies)
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
      const payload = decodeJwt(res.token)
      const roles: string[] = Array.isArray(payload?.["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]) ? payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] : []
      const role: 'Admin'|'User' = roles.includes('Admin') ? 'Admin' : 'User'
      const id: string = payload?.sub || 'me'
      setCurrent({ id, email, role })
      AuditService.log(email, 'Inscription')
      // Init (may be limited by subscription policies)
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

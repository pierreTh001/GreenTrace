import { loadDb, saveDb } from './Storage'
import { seed, type Db } from '../mock/db'
import { AuditService } from './AuditService'

export type User = { id: string; email: string; role: 'Admin' | 'User' }
type DbUser = User & { password: string }

const CURRENT = "greentrace-current-user"
const listeners = new Set<(u: User | null) => void>()

function notify(u: User | null) {
  listeners.forEach(cb => { try { cb(u) } catch {} })
}

function getDb(): Db {
  return loadDb<Db>() ?? seedAndSave()
}
function seedAndSave(): Db {
  const s = seed()
  saveDb(s)
  return s
}

function setCurrent(user: User | null) {
  if (user) localStorage.setItem(CURRENT, JSON.stringify(user))
  else localStorage.removeItem(CURRENT)
  notify(user)
}

export const AuthService = {
  subscribe(cb: (u: User | null) => void) {
    listeners.add(cb)
    return () => { listeners.delete(cb) }
  },
  currentUser(): User | null {
    try { return JSON.parse(localStorage.getItem(CURRENT) || 'null') } catch { return null }
  },
  async login(email: string, password: string): Promise<boolean> {
    const db = getDb()
    const found = db.users.find(u => u.email.toLowerCase() === email.toLowerCase() && (u as DbUser).password === password)
    if (!found) return false
    const { password: _pw, ...safe } = found as DbUser
    setCurrent(safe)
    AuditService.log(email, "Connexion")
    return true
  },
  async register(email: string, password: string): Promise<boolean> {
    const db = getDb()
    const exists = db.users.some(u => u.email.toLowerCase() === email.toLowerCase())
    if (exists) return false
    const u: DbUser = { id: crypto.randomUUID(), email, role: 'User', password }
    db.users.push(u)
    saveDb(db)
    const { password: _pw, ...safe } = u
    setCurrent(safe)
    AuditService.log(email, "Inscription")
    return true
  },
  logout() {
    const u = AuthService.currentUser()
    if (u) AuditService.log(u.email, "Déconnexion")
    setCurrent(null)
  }
}

export type { Db }

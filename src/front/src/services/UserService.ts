import { loadDb, saveDb } from './Storage'
import { seed, type Db } from '../mock/db'
import { AuditService } from './AuditService'

function getDb(): Db {
  return loadDb<Db>() ?? seedAndSave()
}
function seedAndSave(): Db {
  const s = seed()
  saveDb(s)
  return s
}

export const UserService = {
  list() {
    const db = getDb()
    return db.users.map(({ password, ...u }) => u)
  },
  create(email: string, role: 'Admin'|'User', password = 'changeme') {
    const db = getDb()
    const id = crypto.randomUUID()
    db.users.push({ id, email, role, password })
    saveDb(db)
    AuditService.log(email, `Création utilisateur (${role})`)
  },
  update(id: string, data: Partial<{ email: string; role: 'Admin'|'User'; password: string }>) {
    const db = getDb()
    const u = db.users.find(u => u.id === id)
    if (!u) return
    Object.assign(u, data)
    saveDb(db)
    AuditService.log(u.email, "Mise à jour utilisateur")
  },
  remove(id: string) {
    const db = getDb()
    const idx = db.users.findIndex(u => u.id === id)
    if (idx >= 0) {
      const email = db.users[idx].email
      db.users.splice(idx, 1)
      saveDb(db)
      AuditService.log(email, "Suppression utilisateur")
    }
  }
}

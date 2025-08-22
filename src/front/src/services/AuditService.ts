import { loadDb, saveDb } from './Storage'
import { seed, type Db } from '../mock/db'

function getDb(): Db {
  return loadDb<Db>() ?? seedAndSave()
}
function seedAndSave(): Db {
  const s = seed()
  saveDb(s)
  return s
}

export const AuditService = {
  log(actor: string, action: string) {
    const db = getDb()
    db.audit.unshift({ id: crypto.randomUUID(), ts: Date.now(), actor, action })
    saveDb(db)
  },
  list() {
    return getDb().audit
  }
}

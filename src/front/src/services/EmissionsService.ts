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

export const EmissionsService = {
  get() {
    return getDb().emissions
  },
  save(em: Db['emissions']) {
    const db = getDb()
    db.emissions = em
    saveDb(db)
    AuditService.log("system", "Mise à jour des émissions GES")
  },
  totals() {
    const e = EmissionsService.get()
    const s1 = e.scope1.reduce((a,c)=>a+c.tco2e,0)
    const s2 = e.scope2.reduce((a,c)=>a+c.tco2e,0)
    const s3 = e.scope3.reduce((a,c)=>a+c.tco2e,0)
    return { s1, s2, s3, total: s1+s2+s3 }
  }
}

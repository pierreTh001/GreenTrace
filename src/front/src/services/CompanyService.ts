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

export const CompanyService = {
  get() {
    return getDb().company
  },
  save(patch: Partial<Db['company']>) {
    const db = getDb()
    db.company = { ...db.company, ...patch }
    saveDb(db)
    AuditService.log("system", "Mise à jour fiche entreprise")
  },
  getMateriality() {
    return getDb().materiality
  },
  saveMateriality(mat: Record<string, boolean>) {
    const db = getDb()
    db.materiality = mat
    saveDb(db)
    AuditService.log("system", "Mise à jour analyse de matérialité")
  },
  getDataCollection() {
    return getDb().dataCollection
  },
  saveDataCollection(data: Record<string, any>) {
    const db = getDb()
    db.dataCollection = { ...db.dataCollection, ...data }
    saveDb(db)
    AuditService.log("system", "Mise à jour collecte de données")
  }
}

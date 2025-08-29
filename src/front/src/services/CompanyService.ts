import { loadDb, saveDb } from './Storage'
import { AuditService } from './AuditService'
import { ApiClient } from './ApiClient'

// Minimal local shape retained for UI-only fields
type LocalCompany = {
  id?: string
  name?: string
  sector?: string
  employees?: number
  revenue?: number
  headquarters?: string
  euListed?: boolean
  reportingYear?: number
}

type LocalDb = {
  company: LocalCompany
  materiality: Record<string, boolean>
  dataCollection: Record<string, any>
  currentCompanyId?: string
}

function ensureLocal(): LocalDb {
  const def: LocalDb = { company: {}, materiality: {}, dataCollection: {} }
  const db = loadDb<LocalDb>()
  if (!db) { saveDb(def); return def }
  return db
}

async function fetchCompanies(): Promise<any[]> {
  return await ApiClient.get<any[]>('/api/companies')
}

async function fetchCompany(id: string): Promise<any> {
  return await ApiClient.get<any>(`/api/companies/${id}`)
}

async function createCompany(name: string): Promise<any> {
  return await ApiClient.post<any>('/api/companies', { name })
}

async function updateCompany(id: string, patch: Partial<{ name: string; naceCode: string|null; employeesCount: number|null; hqCountry: string|null; website: string|null }>) {
  return await ApiClient.put<any>(`/api/companies/${id}`, {
    name: patch.name,
    naceCode: patch.naceCode ?? null,
    employeesCount: patch.employeesCount ?? null,
    hqCountry: patch.hqCountry ?? null,
    website: patch.website ?? null
  })
}

export const CompanyService = {
  // Called on login/register to sync from API
  async bootstrapFromApi() {
    const db = ensureLocal()
    const list = await fetchCompanies()
    let companyId = db.currentCompanyId
    if (!companyId) companyId = list[0]?.id
    if (!companyId) {
      // Create a placeholder company for the user
      const created = await createCompany('Mon Entreprise')
      companyId = created.id
    }
    const c = await fetchCompany(companyId)
    db.currentCompanyId = companyId
    // Map backend -> local
    db.company = {
      id: c.id,
      name: c.name,
      sector: c.naceCode ?? '',
      employees: c.employeesCount ?? 0,
      headquarters: c.hqCountry ?? ''
    }
    saveDb(db)
  },
  currentCompanyId(): string | undefined { return ensureLocal().currentCompanyId },
  get(): LocalCompany { return ensureLocal().company },
  async save(patch: Partial<LocalCompany>) {
    const db = ensureLocal()
    const id = db.currentCompanyId
    if (id) {
      await updateCompany(id, {
        name: patch.name ?? db.company.name ?? '',
        naceCode: patch.sector ?? db.company.sector ?? '',
        employeesCount: patch.employees ?? db.company.employees ?? 0,
        hqCountry: patch.headquarters ?? db.company.headquarters ?? ''
      })
    }
    db.company = { ...db.company, ...patch }
    saveDb(db)
    AuditService.log('system', 'Mise à jour fiche entreprise')
  },
  getMateriality() { return ensureLocal().materiality },
  saveMateriality(mat: Record<string, boolean>) {
    const db = ensureLocal(); db.materiality = mat; saveDb(db)
    AuditService.log('system', 'Mise à jour analyse de matérialité')
  },
  getDataCollection() { return ensureLocal().dataCollection },
  saveDataCollection(data: Record<string, any>) {
    const db = ensureLocal(); db.dataCollection = { ...db.dataCollection, ...data }; saveDb(db)
    AuditService.log('system', 'Mise à jour collecte de données')
  }
}

import { loadDb, saveDb } from './Storage'
import { AuditService } from './AuditService'
import { ApiClient } from './ApiClient'

// Minimal local shape retained for UI-only fields
type LocalCompany = {
  id?: string
  name?: string
  // Legacy/mock fields used by dashboard/report (kept optional)
  sector?: string | null
  employees?: number | null
  revenue?: number | null
  headquarters?: string | null
  euListed?: boolean | null
  reportingYear?: number | null
  addressLine1?: string | null
  addressLine2?: string | null
  postalCode?: string | null
  city?: string | null
  legalForm?: string | null
  siren?: string | null
  siret?: string | null
  vatNumber?: string | null
  naceCode?: string | null
  website?: string | null
  hqCountry?: string | null
  employeesCount?: number | null
  consolidationMethod?: string | null
  esgContactEmail?: string | null
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

async function createCompany(payload: LocalCompany): Promise<any> {
  return await ApiClient.post<any>('/api/companies', {
    name: payload.name,
    legalForm: payload.legalForm ?? '',
    siren: payload.siren ?? '',
    siret: payload.siret ?? '',
    vatNumber: payload.vatNumber ?? '',
    naceCode: payload.naceCode ?? '',
    website: payload.website ?? '',
    addressLine1: payload.addressLine1 ?? '',
    addressLine2: payload.addressLine2 ?? null,
    postalCode: payload.postalCode ?? '',
    city: payload.city ?? '',
    hqCountry: payload.hqCountry ?? '',
    employeesCount: payload.employeesCount ?? 0,
    consolidationMethod: payload.consolidationMethod ?? '',
    esgContactEmail: payload.esgContactEmail ?? ''
  })
}

async function ensureActiveSubscription() {
  try {
    const me = await ApiClient.get<any>('/api/subscriptions/me')
    if (me) return
    const plans = await ApiClient.get<any[]>('/api/subscriptions/plans')
    const basic = plans.find(p => p.code === 'BASIC') || plans[0]
    if (basic) await ApiClient.post('/api/subscriptions/subscribe', { planId: basic.id })
  } catch {
    // ignore; API will enforce policy if missing
  }
}

async function updateCompany(id: string, patch: Partial<LocalCompany>) {
  return await ApiClient.put<any>(`/api/companies/${id}`, {
    name: patch.name,
    legalForm: patch.legalForm ?? null,
    siren: patch.siren ?? null,
    siret: patch.siret ?? null,
    vatNumber: patch.vatNumber ?? null,
    naceCode: patch.naceCode ?? null,
    website: patch.website ?? null,
    addressLine1: patch.addressLine1 ?? null,
    addressLine2: patch.addressLine2 ?? null,
    postalCode: patch.postalCode ?? null,
    city: patch.city ?? null,
    hqCountry: patch.hqCountry ?? null,
    employeesCount: patch.employeesCount ?? null,
    consolidationMethod: patch.consolidationMethod ?? null,
    esgContactEmail: patch.esgContactEmail ?? null
  })
}

export const CompanyService = {
  // Called on login/register to sync from API
  async bootstrapFromApi() {
    const db = ensureLocal()
    const list = await fetchCompanies()
    // If local companyId is not part of allowed companies, drop it
    if (db.currentCompanyId && !list.some(c => c.id === db.currentCompanyId)) {
      db.currentCompanyId = undefined
      db.company = {}
      saveDb(db)
    }
    let companyId = db.currentCompanyId
    if (!companyId) companyId = list[0]?.id
    if (!companyId) {
      // Aucune entreprise: laisser le formulaire vide pour création manuelle
      db.company = {}
      db.currentCompanyId = undefined
      saveDb(db)
      return
    }
    const c = await fetchCompany(companyId)
    db.currentCompanyId = companyId
    // Map backend -> local (1:1 avec le modèle API)
    db.company = {
      id: c.id,
      name: c.name,
      addressLine1: c.addressLine1 ?? null,
      addressLine2: c.addressLine2 ?? null,
      postalCode: c.postalCode ?? null,
      city: c.city ?? null,
      legalForm: c.legalForm ?? null,
      siren: c.siren ?? null,
      siret: c.siret ?? null,
      vatNumber: c.vatNumber ?? null,
      naceCode: c.naceCode ?? null,
      website: c.website ?? null,
      hqCountry: c.hqCountry ?? null,
      employeesCount: c.employeesCount ?? null,
      consolidationMethod: c.consolidationMethod ?? null,
      esgContactEmail: c.esgContactEmail ?? null
    }
    saveDb(db)
  },
  currentCompanyId(): string | undefined { return ensureLocal().currentCompanyId },
  get(): LocalCompany { return ensureLocal().company },
  async save(patch: Partial<LocalCompany>) {
    const db = ensureLocal()
    const id = db.currentCompanyId
    if (id) {
      try {
        await updateCompany(id, {
          name: patch.name ?? db.company.name ?? '',
          addressLine1: patch.addressLine1 ?? db.company.addressLine1 ?? null,
          addressLine2: patch.addressLine2 ?? db.company.addressLine2 ?? null,
          postalCode: patch.postalCode ?? db.company.postalCode ?? null,
          city: patch.city ?? db.company.city ?? null,
          legalForm: patch.legalForm ?? db.company.legalForm ?? null,
          siren: patch.siren ?? db.company.siren ?? null,
          siret: patch.siret ?? db.company.siret ?? null,
          vatNumber: patch.vatNumber ?? db.company.vatNumber ?? null,
          naceCode: patch.naceCode ?? db.company.naceCode ?? null,
          website: patch.website ?? db.company.website ?? null,
          hqCountry: patch.hqCountry ?? db.company.hqCountry ?? null,
          employeesCount: patch.employeesCount ?? db.company.employeesCount ?? null,
          consolidationMethod: patch.consolidationMethod ?? db.company.consolidationMethod ?? null,
          esgContactEmail: patch.esgContactEmail ?? db.company.esgContactEmail ?? null
        })
      } catch (e:any) {
        const msg = String(e?.message || e || '')
        if (msg.startsWith('404') || msg.startsWith('403')) {
          // Company not found on server → create anew
          await ensureActiveSubscription()
          const payload: LocalCompany = {
            name: patch.name ?? db.company.name,
            legalForm: patch.legalForm ?? db.company.legalForm,
            siren: patch.siren ?? db.company.siren,
            siret: patch.siret ?? db.company.siret,
            vatNumber: patch.vatNumber ?? db.company.vatNumber,
            naceCode: patch.naceCode ?? db.company.naceCode,
            website: patch.website ?? db.company.website,
            addressLine1: patch.addressLine1 ?? db.company.addressLine1,
            addressLine2: patch.addressLine2 ?? db.company.addressLine2,
            postalCode: patch.postalCode ?? db.company.postalCode,
            city: patch.city ?? db.company.city,
            hqCountry: patch.hqCountry ?? db.company.hqCountry,
            employeesCount: patch.employeesCount ?? db.company.employeesCount,
            consolidationMethod: patch.consolidationMethod ?? db.company.consolidationMethod,
            esgContactEmail: patch.esgContactEmail ?? db.company.esgContactEmail
          }
          const created = await createCompany(payload)
          db.currentCompanyId = created.id
          db.company = { ...payload, id: created.id }
          saveDb(db)
          AuditService.log('system', 'Création de l\'entreprise (fallback 404)')
          return
        }
        throw e
      }
    } else {
      // Création de l'entreprise si aucune n'existe encore
      await ensureActiveSubscription()
      const payload: LocalCompany = {
        name: patch.name ?? db.company.name,
        legalForm: patch.legalForm ?? db.company.legalForm,
        siren: patch.siren ?? db.company.siren,
        siret: patch.siret ?? db.company.siret,
        vatNumber: patch.vatNumber ?? db.company.vatNumber,
        naceCode: patch.naceCode ?? db.company.naceCode,
        website: patch.website ?? db.company.website,
        addressLine1: patch.addressLine1 ?? db.company.addressLine1,
        addressLine2: patch.addressLine2 ?? db.company.addressLine2,
        postalCode: patch.postalCode ?? db.company.postalCode,
        city: patch.city ?? db.company.city,
        hqCountry: patch.hqCountry ?? db.company.hqCountry,
        employeesCount: patch.employeesCount ?? db.company.employeesCount,
        consolidationMethod: patch.consolidationMethod ?? db.company.consolidationMethod,
        esgContactEmail: patch.esgContactEmail ?? db.company.esgContactEmail
      }
      const created = await createCompany(payload)
      db.currentCompanyId = created.id
      // merge with id and save
      db.company = { ...payload, id: created.id }
      saveDb(db)
      AuditService.log('system', 'Création de l\'entreprise')
      return
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

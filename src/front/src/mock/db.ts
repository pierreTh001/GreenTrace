export type Db = {
  users: Array<{ id: string; email: string; role: 'Admin' | 'User'; password: string }>
  company: {
    name: string
    sector: string
    employees: number
    revenue: number
    headquarters: string
    euListed: boolean
    reportingYear: number
  },
  materiality: Record<string, boolean>,
  dataCollection: Record<string, any>,
  emissions: {
    scope1: Array<{ category: string; tco2e: number }>
    scope2: Array<{ method: 'market'|'location'; tco2e: number }>
    scope3: Array<{ category: string; tco2e: number }>
  },
  audit: Array<{ id: string; ts: number; actor: string; action: string }>
}

export function seed(): Db {
  return {
    users: [
      { id: 'u-1', email: 'admin@greentrace.io', role: 'Admin', password: 'admin123' },
      { id: 'u-2', email: 'user@greentrace.io', role: 'User', password: 'user12345' }
    ],
    company: {
      name: 'GreenTrace SAS',
      sector: 'Logiciels B2B',
      employees: 42,
      revenue: 3200000,
      headquarters: 'Paris, France',
      euListed: false,
      reportingYear: new Date().getFullYear() - 1
    },
    materiality: {
      'ESRS 2 - Informations générales': true,
      'ESRS E1 - Climat': true,
      'ESRS E2 - Pollution': false,
      'ESRS E3 - Eau et ressources marines': false,
      'ESRS E4 - Biodiversité': false,
      'ESRS E5 - Ressources et économie circulaire': true,
      'ESRS S1 - Propre personnel': true,
      'ESRS S2 - Travailleurs de la chaîne de valeur': false,
      'ESRS S3 - Communautés affectées': false,
      'ESRS S4 - Consommateurs et utilisateurs finaux': true,
      'ESRS G1 - Conduite des affaires': true
    },
    dataCollection: {
      policies: 'Politique climat alignée SBTi (en préparation).',
      risks: 'Risque principal: hausse du coût de l’énergie.',
      targets: 'Réduction -42% S1+S2 d’ici 2030 vs 2024.',
      metrics: { energyMWh: 180, renewableShare: 0.35 }
    },
    emissions: {
      scope1: [{ category: 'Véhicules de société', tco2e: 12 }],
      scope2: [{ method: 'market', tco2e: 28 }],
      scope3: [
        { category: 'Achats de biens et services', tco2e: 220 },
        { category: 'Déplacements professionnels', tco2e: 46 }
      ]
    },
    audit: []
  }
}

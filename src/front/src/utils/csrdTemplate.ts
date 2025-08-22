import { CompanyService } from '../services/CompanyService'
import { EmissionsService } from '../services/EmissionsService'

export function buildReportHtml() {
  const company = CompanyService.get()
  const materiality = CompanyService.getMateriality()
  const data = CompanyService.getDataCollection()
  const emissions = EmissionsService.get()
  const totals = EmissionsService.totals()

  const yes = (v: boolean) => v ? 'Oui' : 'Non'

  return `
    <div style="font-family: Inter, ui-sans-serif, system-ui; padding: 24px; width: 800px;">
      <h1 style="font-size: 28px; margin-bottom: 4px;">Rapport de durabilité (CSRD) — ${company.reportingYear}</h1>
      <div style="color:#64748b;">${company.name} — ${company.sector}, ${company.headquarters}</div>

      <h2 style="margin-top: 24px; font-size: 20px;">Informations générales (ESRS 2)</h2>
      <ul>
        <li>Employés: <b>${company.employees}</b></li>
        <li>Chiffre d'affaires: <b>${company.revenue.toLocaleString('fr-FR')} €</b></li>
        <li>Entreprise cotée UE: <b>${yes(company.euListed)}</b></li>
      </ul>

      <h2 style="margin-top: 16px; font-size: 20px;">Gouvernance & Modèle d'affaires (ESRS 2)</h2>
      <p><b>Politiques</b>: ${data.policies ?? '—'}</p>
      <p><b>Risques & opportunités</b>: ${data.risks ?? '—'}</p>
      <p><b>Objectifs</b>: ${data.targets ?? '—'}</p>

      <h2 style="margin-top: 16px; font-size: 20px;">Actes de matérialité</h2>
      <ul>
        ${Object.entries(materiality).map(([k,v]) => `<li>${k}: <b>${yes(!!v)}</b></li>`).join('')}
      </ul>

      <h2 style="margin-top: 16px; font-size: 20px;">Indicateurs climats (ESRS E1)</h2>
      <p>Consommation d'énergie: <b>${data.metrics?.energyMWh ?? '—'} MWh</b>, part renouvelable: <b>${Math.round((data.metrics?.renewableShare ?? 0)*100)}%</b></p>

      <h3 style="margin-top: 12px; font-size: 18px;">Bilans GES</h3>
      <ul>
        <li>Scope 1: <b>${totals.s1.toFixed(1)} tCO₂e</b></li>
        <li>Scope 2: <b>${totals.s2.toFixed(1)} tCO₂e</b></li>
        <li>Scope 3: <b>${totals.s3.toFixed(1)} tCO₂e</b></li>
        <li>Total: <b>${totals.total.toFixed(1)} tCO₂e</b></li>
      </ul>

      <h2 style="margin-top: 16px; font-size: 20px;">Méthodologies & Périmètre</h2>
      <p>Méthodologie simplifiée GHG Protocol pour S1/S2/S3. Périmètre: ${company.name} (consolidé: ${yes(false)}).</p>

      <hr style="margin: 24px 0;" />

      <small style="color:#64748b;">Ce modèle est fourni à des fins de test. La conformité finale CSRD/ESRS nécessite une revue d'expert et l'audit externe.</small>
    </div>
  `
}

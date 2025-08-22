# GreenTrace — Azure Starter (Terraform + GitHub Actions)

Ce dossier est un *starter pack* prêt à l’emploi pour créer un **environnement DEV sur Azure** pour GreenTrace :
- **App Service (Linux)** pour l’API .NET
- **Azure SQL Database** (serveur logique + base)
- **Storage Account** (Blob) pour les documents
- **Key Vault** pour stocker la chaîne de connexion SQL
- **Application Insights** pour les logs
- **Identité managée** (Managed Identity) sur l’API
- **Terraform** pour décrire l’infra et **GitHub Actions** pour automatiser

> Objectif : appliquer en DEV, puis dupliquer le dossier `infra/envs/dev` en `qa`, `prod` en changeant 2–3 variables.

---

## 0) Prérequis (sur ton PC)
1. **Créer un compte Azure** : https://azure.microsoft.com (il faut une CB, offre d’essai possible).
2. **Installer** :
   - Git : https://git-scm.com/downloads
   - Visual Studio Code : https://code.visualstudio.com/
   - Azure CLI : https://learn.microsoft.com/cli/azure/install-azure-cli
   - Terraform : https://developer.hashicorp.com/terraform/install
3. **Avoir un compte GitHub** et créer un **repo privé**.

> Si tu n’as *jamais* utilisé Git : ouvre VS Code → installe l’extension “GitHub” → *Clone Repository* et colle l’URL de ton repo.

---

## 1) Connexion Azure & création du “remote state” (une seule fois)
Le **state Terraform** doit être stocké dans un **Storage Account** dédié (sécurisé). On le crée **une fois** à la main en CLI.

Ouvre un **terminal** (PowerShell, Terminal macOS ou VS Code → Terminal) puis :

```bash
# 1) Se connecter
az login

# (Si plusieurs abonnements, lister et choisir l'ID voulu)
az account list --output table
az account set --subscription "<SUBSCRIPTION_ID>"

# 2) Créer un Resource Group pour le tfstate
az group create -n grt-tfstate-rg -l westeurope

# 3) Créer le Storage Account pour le state
TFSTATE_SA="grttfstate$RANDOM"
az storage account create -g grt-tfstate-rg -n $TFSTATE_SA -l westeurope --sku Standard_LRS

# 4) Créer le container 'tfstate'
az storage container create --account-name $TFSTATE_SA -n tfstate
echo "Ton Storage Account pour le state est: $TFSTATE_SA"
```

> **Note**: on utilisera **Azure AD** pour que Terraform accède au state, donc **pas** besoin de clé secrète du storage.

---

## 2) Créer un Service Principal (robot) pour Terraform
GitHub Actions utilisera un **compte service** (Service Principal) pour déployer.

```bash
# Remplace NOM lisible si tu veux
az ad sp create-for-rbac   --name "greentrace-tf"   --role "Contributor"   --scopes "/subscriptions/$(az account show --query id -o tsv)"   --sdk-auth
```

Garde la **sortie JSON** précieusement (clientId, clientSecret, tenantId, subscriptionId).

{
  "clientId": "5778632d-a602-4bb3-ad2f-b4e46263e312",
  "clientSecret": "lUE8Q~zpVjcj.c3NXDX_91YSsY8VfDywNhVgCbYL",
  "subscriptionId": "fa92e16c-805c-4b72-8a7e-861dc95b01c6",
  "tenantId": "ab66917a-dd61-4317-96e3-f96c5ebddf1d",
  "activeDirectoryEndpointUrl": "https://login.microsoftonline.com",
  "resourceManagerEndpointUrl": "https://management.azure.com/",
  "activeDirectoryGraphResourceId": "https://graph.windows.net/",
  "sqlManagementEndpointUrl": "https://management.core.windows.net:8443/",
  "galleryEndpointUrl": "https://gallery.azure.com/",
  "managementEndpointUrl": "https://management.core.windows.net/"
}



> On lui donnera aussi accès au stockage du state via **RBAC** :
```bash
# Donne 'Storage Blob Data Contributor' sur le Storage Account du state
SA_ID=$(az storage account show -g grt-tfstate-rg -n $TFSTATE_SA --query id -o tsv)
SP_APPID=$(az ad sp list --display-name "greentrace-tf" --query "[0].appId" -o tsv)
az role assignment create   --assignee $SP_APPID   --role "Storage Blob Data Contributor"   --scope $SA_ID


{
  "condition": null,
  "conditionVersion": null,
  "createdBy": null,
  "createdOn": "2025-08-22T12:42:42.003686+00:00",
  "delegatedManagedIdentityResourceId": null,
  "description": null,
  "id": "/subscriptions/fa92e16c-805c-4b72-8a7e-861dc95b01c6/resourceGroups/grt-tfstate-rg/providers/Microsoft.Storage/storageAccounts/grttfstate31508/providers/Microsoft.Authorization/roleAssignments/18078efc-42c3-4a79-9bf9-6883a2129267",
  "name": "18078efc-42c3-4a79-9bf9-6883a2129267",
  "principalId": "71f3069e-c0a2-49a9-95e9-dcd5cf9015be",
  "principalType": "ServicePrincipal",
  "resourceGroup": "grt-tfstate-rg",
  "roleDefinitionId": "/subscriptions/fa92e16c-805c-4b72-8a7e-861dc95b01c6/providers/Microsoft.Authorization/roleDefinitions/ba92f5b4-2d11-453d-a403-e96b0029c9fe",
  "scope": "/subscriptions/fa92e16c-805c-4b72-8a7e-861dc95b01c6/resourceGroups/grt-tfstate-rg/providers/Microsoft.Storage/storageAccounts/grttfstate31508",
  "type": "Microsoft.Authorization/roleAssignments",
  "updatedBy": "b96f799c-29bb-4df7-b865-1a3975ccf99d",
  "updatedOn": "2025-08-22T12:42:42.320689+00:00"
}



```

---

## 3) Préparer ton repo GitHub (secrets)
Sur GitHub → **Settings** → **Secrets and variables** → **Actions** → **New repository secret** :

- `AZURE_CREDENTIALS` → colle **le JSON** retourné par `--sdk-auth` ci‑dessus.
- `AZURE_SUBSCRIPTION_ID` → la valeur `subscriptionId` du JSON.
- `TFSTATE_RESOURCE_GROUP` → `grt-tfstate-rg`
- `TFSTATE_STORAGE_ACCOUNT` → **la valeur de `$TFSTATE_SA` affichée plus haut**
- `TFSTATE_CONTAINER` → `tfstate`

> Plus tard pour le déploiement de l’API, on ajoutera `AZUREAPPSERVICE_PUBLISHPROFILE_DEV` (voir § 7).

---

## 4) Configurer le backend Terraform (state)
Dans `infra/envs/dev/backend.tf`, **renseigne** la variable `storage_account_name` avec **ton** Storage Account créé à l’étape 1.

```hcl
terraform {
  backend "azurerm" {
    resource_group_name  = "grt-tfstate-rg"
    storage_account_name = "<REMPLACE-MOI>"
    container_name       = "tfstate"
    key                  = "greentrace-dev.tfstate"
    use_azuread_auth     = true
  }
}
```

> **À faire** : remplace `<REMPLACE-MOI>` par le nom exact (ex: `grttfstate12345`).

---

## 5) Lancer Terraform *en local* (une première fois)
Depuis la racine du repo cloné en local :

```bash
cd infra/envs/dev

# Initialise Terraform + backend
terraform init

# Prévisualise les créations
terraform plan

# Applique réellement (5-10 minutes)
terraform apply
```

À la fin, tu auras : Resource Group, Storage, Key Vault, SQL Server + DB, App Service (Linux), App Insights.

> **SQL** : le mot de passe admin est **généré**. La *connection string* est **mise en secret** dans Key Vault et **référencée** par l’App Service.

---

## 6) Activer l’automatisation (GitHub Actions)
À chaque changement dans `infra/**`, GitHub fera un **plan** (PR) puis un **apply** (push sur `main`).  
Vérifie/édite `.github/workflows/infra-dev.yml` si besoin.

---

## 7) Déployer ton API (optionnel maintenant)
- Récupère le **Publish Profile** de l’App Service `grt-dev-api` (Azure Portal → App Service → *Get publish profile*).
- Sur GitHub, secret **`AZUREAPPSERVICE_PUBLISHPROFILE_DEV`** : colle le contenu XML.
- Pousse du code sous `src/api/**` (ton API .NET).  
- Le workflow `.github/workflows/deploy-api-dev.yml` publiera automatiquement.

> Tu peux aussi déployer depuis VS / CLI au début, puis passer à l’automatique.

---

## 8) Créer un nouvel environnement (QA/PROD)
- Copie le dossier `infra/envs/dev` → `infra/envs/qa` (ou `prod`).
- Renomme les ressources dans `variables.tf` (env, suffixes).
- Dans `backend.tf` change `key = "greentrace-qa.tfstate"`.
- Duplique les workflows en changeant les chemins/ids/secrets.

---

## 9) Nettoyer (si besoin)
Pour **détruire** l’environnement DEV :
```bash
cd infra/envs/dev
terraform destroy
```

---

## Notes
- Coûts DEV typiques: App Service *B1*, SQL *S0*, Storage *LRS*, Insights: quelques dizaines d’€/mois.
- Production: envisager **Private Endpoints**, plans plus costauds, backups forts, alerting.

Bon déploiement 🚀

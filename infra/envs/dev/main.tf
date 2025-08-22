data "azurerm_client_config" "current" {}

# Resource Group
resource "azurerm_resource_group" "rg" {
  name     = "${var.name_prefix}-${var.env}-rg"
  location = var.location
}

# Storage Account (documents)
resource "azurerm_storage_account" "sa" {
  name                            = "${var.name_prefix}${var.env}sa"
  resource_group_name             = azurerm_resource_group.rg.name
  location                        = var.location
  account_tier                    = "Standard"
  account_replication_type        = "LRS"
  allow_nested_items_to_be_public = false


  blob_properties {
    versioning_enabled = true
    delete_retention_policy {
      days = 7
    }
    container_delete_retention_policy {
      days = 7
    }
  }
}

resource "azurerm_storage_container" "docs" {
  name                  = "documents"
  storage_account_name  = azurerm_storage_account.sa.name
  container_access_type = "private"
}

# Application Insights
resource "azurerm_application_insights" "appi" {
  name                = "${var.name_prefix}-${var.env}-appi"
  location            = var.location
  resource_group_name = azurerm_resource_group.rg.name
  application_type    = "web"

  lifecycle {
    ignore_changes = [workspace_id]
  }
}

# App Service Plan
resource "azurerm_service_plan" "plan" {
  name                = "${var.name_prefix}-${var.env}-plan"
  resource_group_name = azurerm_resource_group.rg.name
  location            = var.location
  os_type             = "Linux"
  sku_name            = "B1"
}

# SQL Server + DB
resource "random_password" "sql_admin" {
  length  = 24
  special = true
}

resource "azurerm_mssql_server" "sql" {
  name                         = "${var.name_prefix}-${var.env}-sql-fc" # <— nouveau nom
  resource_group_name          = azurerm_resource_group.rg.name
  location                     = var.sql_location # francecentral
  version                      = "12.0"
  administrator_login          = "sqladminuser"
  administrator_login_password = random_password.sql_admin.result
}

# Autoriser services Azure (dont App Service) à atteindre SQL
resource "azurerm_mssql_firewall_rule" "allow_azure_services" {
  name             = "AllowAzureServices"
  server_id        = azurerm_mssql_server.sql.id
  start_ip_address = "0.0.0.0"
  end_ip_address   = "0.0.0.0"
}

resource "azurerm_mssql_database" "db" {
  name      = "${var.name_prefix}_${var.env}_db"
  server_id = azurerm_mssql_server.sql.id
  sku_name  = "S0"
}

# Key Vault
resource "azurerm_key_vault" "kv" {
  name                       = "${var.name_prefix}-${var.env}-kv"
  location                   = var.location
  resource_group_name        = azurerm_resource_group.rg.name
  tenant_id                  = data.azurerm_client_config.current.tenant_id
  sku_name                   = "standard"
  purge_protection_enabled   = true
  soft_delete_retention_days = 7

  # Accès pour l'utilisateur courant (créateur) pour gérer les secrets
  access_policy {
    tenant_id = data.azurerm_client_config.current.tenant_id
    object_id = data.azurerm_client_config.current.object_id

    secret_permissions = ["Get", "List", "Set", "Delete", "Recover", "Purge"]
  }
}

# Connection string SQL (secret dans KV)
locals {
  sql_connection_string = "Server=tcp:${azurerm_mssql_server.sql.name}.database.windows.net,1433;Initial Catalog=${azurerm_mssql_database.db.name};Persist Security Info=False;User ID=${azurerm_mssql_server.sql.administrator_login};Password=${random_password.sql_admin.result};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
}

resource "azurerm_key_vault_secret" "sql_conn" {
  name         = "Sql--ConnectionString"
  value        = local.sql_connection_string
  key_vault_id = azurerm_key_vault.kv.id
}

# Web App (API)
resource "azurerm_linux_web_app" "api" {
  name                = "${var.name_prefix}-${var.env}-api"
  resource_group_name = azurerm_resource_group.rg.name
  location            = var.location
  service_plan_id     = azurerm_service_plan.plan.id

  identity { type = "SystemAssigned" }

  site_config {
    minimum_tls_version = "1.2"
    ftps_state          = "Disabled"
    application_stack {
      dotnet_version = "8.0"
    }
  }

  app_settings = {
    "WEBSITE_RUN_FROM_PACKAGE"              = "1"
    "APPLICATIONINSIGHTS_CONNECTION_STRING" = azurerm_application_insights.appi.connection_string

    # Key Vault Reference pour la chaîne de connexion SQL
    "ConnectionStrings__Default" = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.sql_conn.id})"

    # Storage (nom + container) - pour accès via SDK avec Managed Identity
    "Blob__AccountName" = azurerm_storage_account.sa.name
    "Blob__Container"   = azurerm_storage_container.docs.name
  }
}

# Autoriser l'application à lire les secrets du Key Vault
resource "azurerm_key_vault_access_policy" "api_kv_policy" {
  key_vault_id = azurerm_key_vault.kv.id
  tenant_id    = data.azurerm_client_config.current.tenant_id
  object_id    = azurerm_linux_web_app.api.identity[0].principal_id

  secret_permissions = ["Get", "List"]
}

# Donner à l'app les droits Blob (RBAC) sur le Storage Account
resource "azurerm_role_assignment" "api_storage_contrib" {
  scope                = azurerm_storage_account.sa.id
  role_definition_name = "Storage Blob Data Contributor"
  principal_id         = azurerm_linux_web_app.api.identity[0].principal_id
}

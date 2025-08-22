terraform {
  backend "azurerm" {
    resource_group_name  = "grt-tfstate-rg"
    storage_account_name = "grttfstate31508" # Renseigne le nom du Storage Account tfstate
    container_name       = "tfstate"
    key                  = "greentrace-dev.tfstate"
    use_azuread_auth     = true
  }
}

variable "env" {
  type    = string
  default = "dev"
}

variable "location" {
  type    = string
  default = "westeurope"
}

variable "name_prefix" {
  type    = string
  default = "grt" # Toutes les ressources commenceront par 'grt'
}

variable "sql_location" {
  type    = string
  default = "francecentral"
}

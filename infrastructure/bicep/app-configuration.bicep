// Parameters
@description('The name for the  app configuration')
param name string

@description('The location for the container app environment')
param location string = resourceGroup().location

@description('The SKU type for the app configuration')
@allowed(['free', 'standard'])
param sku string = 'free'

@description('The AAD-PrincipalIds of the developers, needing read-write access to the database')
param userPrincipalIds array = []

// Template
resource appConfiguration 'Microsoft.AppConfiguration/configurationStores@2022-05-01' = {
  name: name
  location: location
  sku: {
    name: sku
  }
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    encryption: {
    }
    disableLocalAuth: true
    enablePurgeProtection: false
    publicNetworkAccess: 'Enabled'
  }
}

// var appConfigurationDataReaderRole = resourceId('Microsoft.Authorization/roleDefinitions', '516239f1-63e1-4d78-a4de-a74fb236a071') // App Configuration Data Reader
var appConfigurationDataOwnerRole = resourceId('Microsoft.Authorization/roleDefinitions', '5ae67dd6-50cb-40e7-96ff-dc2bfa4b606b') // App Configuration Data Owner

resource roleAssignment 'Microsoft.Authorization/roleAssignments@2020-10-01-preview' = [for principalId in userPrincipalIds: {
  name: guid(principalId)
  scope: appConfiguration
  properties: {
    roleDefinitionId: appConfigurationDataOwnerRole
    principalId: principalId
    principalType: 'User'
  }
}]

output id string = appConfiguration.id
output name string = appConfiguration.name

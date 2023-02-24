// Parameters
@description('The name for the container app environment')
param containerAppEnvironmentName string = 'codebreakerenv'

@description('The location for the container app environment')
param containerAppEnvironmentLocation string = resourceGroup().location

@description('The name for the log analytics workspace')
param logAnalyticsWorkspaceName string

// Template
resource logAnalytics 'microsoft.operationalinsights/workspaces@2021-12-01-preview' existing = {
  name: logAnalyticsWorkspaceName
}

resource containerAppEnvironment 'Microsoft.App/managedEnvironments@2022-10-01' = {
  name: containerAppEnvironmentName
  location: containerAppEnvironmentLocation
  sku: {
    name: 'Consumption'
  }
  properties: {
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: logAnalytics.properties.customerId
        sharedKey: logAnalytics.listKeys().primarySharedKey
      }
    }
    zoneRedundant: false
    customDomainConfiguration: {
    }
  }
  dependsOn: [
    logAnalytics
  ]
}

output id string = containerAppEnvironment.id

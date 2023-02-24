// Parameters
@description('The name for the log analytics workspace')
param name string

@description('The location for the log analytics workspace')
param location string = resourceGroup().location

// Template
resource logAnalytics 'microsoft.operationalinsights/workspaces@2021-12-01-preview' = {
  name: name
  location: location
  properties: {
    sku: {
      name: 'pergb2018'
    }
    retentionInDays: 30
    features: {
      enableLogAccessUsingOnlyResourcePermissions: true
    }
    workspaceCapping: {
      dailyQuotaGb: -1
    }
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
  }
}

output name string = logAnalytics.name
output id string = logAnalytics.id

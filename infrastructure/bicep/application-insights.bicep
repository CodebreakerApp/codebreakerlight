// Parameters
@description('The name for application insights')
param name string

@description('The location for application insights')
param location string = resourceGroup().location

@description('The name for the log analytics workspace')
param logAnalyticsWorkspaceName string

resource logAnalytics 'microsoft.operationalinsights/workspaces@2021-12-01-preview' existing = {
  name: logAnalyticsWorkspaceName
}

var logWorkspaceExternalId = '/subscriptions/${subscription().subscriptionId}/resourceGroups/${resourceGroup().name}/providers/microsoft.operationalinsights/workspaces/${logAnalytics.name}'

// Template
resource appInsights 'microsoft.insights/components@2020-02-02' = {
  name: name
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    Flow_Type: 'Redfield'
    Request_Source: 'IbizaAIExtension'
    RetentionInDays: 90
    WorkspaceResourceId: logWorkspaceExternalId
    IngestionMode: 'LogAnalytics'
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
  }
  dependsOn: [ logAnalytics ]
}

output appInsights object = appInsights

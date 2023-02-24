@description('The PrincipalIds of the developers needing access to resources from their local machine')
param userPrincipalIds array

@description('Whether free tiers should be applied or not')
param applyFreeTiers bool

@description('The suffix used for resources, where a name collision might occur')
@minLength(2)
@maxLength(30)
param suffix string

module logAnalytics 'log-analytics.bicep' = {
  name: 'log-analytics'
  params: {
    name: 'codebreaker-logs'
  }
}

module applicationInsights 'application-insights.bicep' = {
  name: 'application-insights'
  params: {
    name: 'codebreaker-insights'
    logAnalyticsWorkspaceName: logAnalytics.outputs.name
  }
}

module appConfiguration 'app-configuration.bicep' = {
  name: 'app-configuration'
  params: {
    name: 'codebreaker-config-${suffix}' // After deleting an app configuration instance, the name may be still in use for some days. https://learn.microsoft.com/en-us/azure/azure-app-configuration/faq#why-can-t-i-create-an-app-configuration-store-with-the-same-name-as-one-that-i-just-deleted
    sku: applyFreeTiers ? 'free' : 'standard' // Only 1 free app configuration instance per subscription allowed
    userPrincipalIds: userPrincipalIds
  }
}

module containerRegistry 'container-registry.bicep' = {
  name: 'container-registry'
  params: {
    name: 'codebreakerContainerRegistry${suffix}'
  }
}

module cosmos 'cosmos.bicep' = {
  name: 'cosmos'
  params: {
    databaseAccountName: 'codebreaker-db-account-${suffix}'
    databaseName: 'codebreaker-db'
    gameContainerName: 'GameContainer'
    applyFreeTier: applyFreeTiers  // Only 1 free tier per subscription allowed
    userPrincipalIds: userPrincipalIds
  }
}

module containerAppEnvironment 'container-app-environment.bicep' = {
  name: 'container-app-environment'
  params: {
    logAnalyticsWorkspaceName: logAnalytics.outputs.name
  }
}

module apiContainerApp 'api-container-app.bicep' = {
  name: 'api-container-app'
  dependsOn: [ containerAppEnvironment ]
  params: {
    name: 'codebreaker-api'
    environmentId: containerAppEnvironment.outputs.id
    appConfigurationName: appConfiguration.outputs.name
    databaseAccountName: cosmos.outputs.accountName
    minReplicas: 1
    maxReplicas: 5
    port: 80
  }
}

// Parameters
@description('The id of the container app environment')
param environmentId string

@description('The name for the api container app')
param name string

@description('The location for the api container app')
param location string = resourceGroup().location

@description('The target port for the api container app')
param port int = 443

@description('Number of CPU cores the api container can use. Can be with a maximum of two decimals.')
param cpu string = '0.25'

@description('Amount of memory (in gibibytes, GiB) allocated to the api container up to 4GiB. Can be with a maximum of two decimals. Ratio with CPU cores must be equal to 2.')
param memory string = '0.5'

@description('Minimum number of replicas the api container app will be deployed')
@minValue(0)
@maxValue(30)
param minReplicas int = 0

@description('Maximum number of replicas the api container app will be deployed')
@minValue(1)
@maxValue(30)
param maxReplicas int = 10

@description('Specifies the docker container image to deploy for the api container app')
param initialImage string = 'mcr.microsoft.com/azuredocs/containerapps-helloworld:latest'

// Templates
resource apiContainerApp 'Microsoft.App/containerApps@2022-01-01-preview' = {
  name: name
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    managedEnvironmentId: environmentId
    configuration: {
      ingress: {
        external: true
        targetPort: port
        allowInsecure: false
        traffic: [
          {
            latestRevision: true
            weight: 100
          }
        ]
      }
    }
    template: {
      revisionSuffix: 'firstrevision'
      containers: [
        {
          name: name
          image: initialImage
          resources: {
            cpu: json(cpu)
            memory: '${memory}Gi'
          }
        }
      ]
      scale: {
        minReplicas: minReplicas
        maxReplicas: maxReplicas
      }
    }
  }
}


@description('Azure Cosmos DB account name')
@maxLength(44)
param databaseAccountName string

resource databaseAccount 'Microsoft.DocumentDB/databaseAccounts@2022-08-15' existing = {
  name: toLower(databaseAccountName)
}

resource cosmosDatacontributorRole 'Microsoft.DocumentDB/databaseAccounts/sqlRoleDefinitions@2022-08-15' existing = {
  parent: databaseAccount
  name: '00000000-0000-0000-0000-000000000002'  // Cosmos data contributor
}

resource apiContainerAppComsosDataContributorRoleAssignment 'Microsoft.DocumentDB/databaseAccounts/sqlRoleAssignments@2022-08-15' = {
  parent: databaseAccount
  name: guid(databaseAccount.id, name)
  properties: {
    roleDefinitionId: cosmosDatacontributorRole.id
    principalId: apiContainerApp.identity.principalId
    scope: databaseAccount.id
  }
}


@description('The name of the app configuration')
param appConfigurationName string

resource appConfiguration 'Microsoft.AppConfiguration/configurationStores@2022-05-01' existing = {
  name: appConfigurationName
}

var appConfigurationDataReaderRole = resourceId('Microsoft.Authorization/roleDefinitions', '516239f1-63e1-4d78-a4de-a74fb236a071') // App Configuration Data Reader

resource appConfigurationDataReaderRoleAssignment 'Microsoft.Authorization/roleAssignments@2020-10-01-preview' =  {
  name: guid(appConfiguration.id, name)
  scope: appConfiguration
  properties: {
    roleDefinitionId: appConfigurationDataReaderRole
    principalId: apiContainerApp.identity.principalId
    principalType: 'ServicePrincipal'
  }
}

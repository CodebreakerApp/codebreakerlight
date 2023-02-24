// Parameters
@description('Azure Cosmos DB account name')
@maxLength(44)
param databaseAccountName string

@description('The name for the database')
param databaseName string

@description('The name for the game-container')
param gameContainerName string = 'GameContainer'

@description('The location for the DB account')
param location string = resourceGroup().location
var primaryRegion = location

@description('Whether the free tier should be applied or not')
param applyFreeTier bool = true

@description('The throughput for the game container')
@minValue(400)
@maxValue(1000000)
param throughput int = 400

@description('The AAD-PrincipalIds of the developers, needing read-write access to the database')
param userPrincipalIds array = []

// Template
resource databaseAccount 'Microsoft.DocumentDB/databaseAccounts@2022-08-15' = {
  name: toLower(databaseAccountName)
  location: location
  kind: 'GlobalDocumentDB'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    publicNetworkAccess: 'Enabled'
    enableAutomaticFailover: false
    enableMultipleWriteLocations: false
    isVirtualNetworkFilterEnabled: false
    virtualNetworkRules: []
    disableKeyBasedMetadataWriteAccess: false
    enableFreeTier: applyFreeTier
    enableAnalyticalStorage: false
    analyticalStorageConfiguration: {
      schemaType: 'WellDefined'
    }
    databaseAccountOfferType: 'Standard'
    defaultIdentity: 'FirstPartyIdentity'
    networkAclBypass: 'None'
    disableLocalAuth: false
    enablePartitionMerge: false
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
      maxIntervalInSeconds: 5
      maxStalenessPrefix: 100
    }
    locations: [
      {
        locationName: primaryRegion
        failoverPriority: 0
        isZoneRedundant: false
      }
    ]
    cors: []
    capabilities: []
    ipRules: []
    backupPolicy: {
      type: 'Periodic'
      periodicModeProperties: {
        backupIntervalInMinutes: 1440
        backupRetentionIntervalInHours: 48
        backupStorageRedundancy: 'Geo'
      }
    }
    networkAclBypassResourceIds: []
  }
}

resource database 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2022-08-15' = {
  parent: databaseAccount
  name: databaseName
  properties: {
    resource: {
      id: databaseName
    }
  }
}

resource gameContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2022-08-15' = {
  parent: database
  name: gameContainerName
  properties: {
    resource: {
      id: gameContainerName
      indexingPolicy: {
        indexingMode: 'consistent'
        automatic: true
        includedPaths: [
          {
            path: '/*'
          }
        ]
        excludedPaths: [
          {
            path: '/"_etag"/?'
          }
        ]
      }
      partitionKey: {
        paths: [
          '/GameId'
        ]
        kind: 'Hash'
      }
      uniqueKeyPolicy: {
        uniqueKeys: []
      }
      conflictResolutionPolicy: {
        mode: 'LastWriterWins'
        conflictResolutionPath: '/_ts'
      }
    }
    options: {
      throughput: throughput
    }
  }
}

// resource cosmosDatareaderRole 'Microsoft.DocumentDB/databaseAccounts/sqlRoleDefinitions@2022-08-15' existing = {
//   parent: databaseAccount
//   name: '00000000-0000-0000-0000-000000000001'
// }

resource cosmosDatacontributorRole 'Microsoft.DocumentDB/databaseAccounts/sqlRoleDefinitions@2022-08-15' existing = {
  parent: databaseAccount
  name: '00000000-0000-0000-0000-000000000002'
}

resource developerRoleAssignment 'Microsoft.DocumentDB/databaseAccounts/sqlRoleAssignments@2022-08-15' = [for principalId in userPrincipalIds: {
  parent: databaseAccount
  name: principalId
  properties: {
    roleDefinitionId: cosmosDatacontributorRole.id
    principalId: principalId
    scope: databaseAccount.id
  }
}]

output accountName string = databaseAccountName
output dbName string = databaseName
output containerName string = gameContainerName

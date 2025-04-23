param envName string
param location string
param certName string
@secure()
param certValue string
@secure()
param certPassword string

resource env 'Microsoft.App/managedEnvironments@2024-10-02-preview' = {
  name: envName
  location: location
  properties: {
    zoneRedundant: false
    workloadProfiles: [
      {
        workloadProfileType: 'Consumption'
        name: 'Consumption'
        enableFips: false
      }
    ]
    peerAuthentication: {
      mtls: {
        enabled: false
      }
    }
    peerTrafficConfiguration: {
      encryption: {
        enabled: false
      }
    }
    publicNetworkAccess: 'Enabled'
  }
}

resource cert 'Microsoft.App/managedEnvironments/certificates@2024-10-02-preview' = {
  parent: env
  name: certName
  location: location
  properties: {
    certificateType: 'ServerSSLCertificate'
    password: certPassword
    value: certValue
  }
}

output containerAppEnvName string = env.name
output location string = env.location
output certId string = cert.id

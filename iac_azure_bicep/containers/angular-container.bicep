param location string
param envName string
param containerName string
param imageName string
param domainName string
param certName string

resource env 'Microsoft.App/managedEnvironments@2024-10-02-preview' existing = {
  name: envName
}

resource cert 'Microsoft.App/managedEnvironments/certificates@2024-10-02-preview' existing = {
  parent: env
  name: certName
}

resource containerapps_angular_app_name_resource 'Microsoft.App/containerapps@2024-10-02-preview' = {
  name: containerName
  location: location
  properties: {
    managedEnvironmentId: env.id
    configuration: {
      activeRevisionsMode: 'Single'
      ingress: {
        external: true
        targetPort: 8080
        traffic: [
          {
            weight: 100
            latestRevision: true
          }
        ]
        customDomains: [
          {
            name: domainName
            certificateId: cert.id
            bindingType: 'SniEnabled'
          }
        ]
        allowInsecure: false
        stickySessions: {
          affinity: 'none'
        }
      }
    }
    template: {
      containers: [
        {
          image: imageName
          name: containerName
          resources: {
            cpu: json('0.5')
            memory: '1Gi'
          }
        }
      ]
      scale: {
        minReplicas: 1
        maxReplicas: 10
        cooldownPeriod: 300
        pollingInterval: 30
      }
    }
  }
}

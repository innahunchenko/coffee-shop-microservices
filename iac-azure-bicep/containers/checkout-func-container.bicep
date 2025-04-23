param location string
param envName string
param containerName string
param imageName string
param orderApiUrl string

@secure()
param servicebusConnectionString string

resource env 'Microsoft.App/managedEnvironments@2024-10-02-preview' existing = {
  name: envName
}

resource containerapps_process_checkout_name_resource 'Microsoft.App/containerapps@2024-10-02-preview' = {
  name: containerName
  location: location
  
  properties: {
    managedEnvironmentId: env.id
    configuration: {
      secrets: [
        {
          name: 'sb-connection-string'
          value: servicebusConnectionString
        }
      ]
      activeRevisionsMode: 'Single'
      ingress: {
        external: true
        targetPort: 80
        traffic: [
          {
            weight: 100
            latestRevision: true
          }
        ]
        allowInsecure: false
      }
    }
    template: {
      containers: [
        {
          image: imageName
          name: containerName
          env: [
            {
              name: 'ServiceBusConnectionString'
              secretRef: 'sb-connection-string'
            }
            {
              name: 'orderApiUrl'
              value: orderApiUrl
            }
          ]
          resources: {
            cpu: json('0.25')
            memory: '0.5Gi'
          }
          probes: []
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

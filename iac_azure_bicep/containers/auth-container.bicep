param location string
param envName string
param containerName string
param imageName string

var authapiUrl = 'https://${containerName}.${envName}.azurecontainerapps.io'

resource env 'Microsoft.App/managedEnvironments@2024-10-02-preview' existing = {
  name: envName
}

resource authContainerApp 'Microsoft.App/containerapps@2024-10-02-preview' = {
  name: containerName
  location: location
  properties: {
    managedEnvironmentId: env.id
    configuration: {
      secrets: [
        {
          name: 'db-connection-sting'
        }
        {
          name: 'jwt-secret'
        }
      ]
      ingress: {
        external: true
        targetPort: 8080
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
              name: 'ConnectionStrings__IdentityConnection'
              secretRef: 'db-connection-sting'
            }
            {
              name: 'JwtOptions__Secret'
              secretRef: 'jwt-secret'
            }
            {
              name: 'JwtOptions__Issuer'
              value: authapiUrl
            }
            {
              name: 'JwtOptions__Audience'
              value: authapiUrl
            }
          ]
          resources: {
            cpu: json('0.25')
            memory: '0.5Gi'
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

output url string = 'https://${authContainerApp.properties.configuration.ingress.fqdn}'

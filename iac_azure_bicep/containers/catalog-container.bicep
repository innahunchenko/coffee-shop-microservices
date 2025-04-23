param location string
param containerName string
param envName string 
@secure()
param sqldbConnectionString string
param imageName string

resource env 'Microsoft.App/managedEnvironments@2024-10-02-preview' existing = {
  name: envName
}

resource catalogContainerApp 'Microsoft.App/containerapps@2024-10-02-preview' = {
  name: containerName
  location: location
  properties: {
    managedEnvironmentId: env.id
    configuration: {
      secrets: [
        {
          name: 'connection-string-db'
          value: sqldbConnectionString
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
      // registries: [
      //   {
      //     server: 'coffeshopbackend.azurecr.io'
      //     username: 'coffeshopbackend'
      //     passwordSecretRef: 'reg-pswd-b85486df-a7e8'
      //   }
      // ]
    }
    template: {
      containers: [
        {
          image: imageName
          name: containerName
          env: [
            {
              name: 'ConnectionStrings__ProductsConnection'
              secretRef: 'connection-string-db'
            }
          ]
          resources: {
            cpu: json('0.5')
            memory: '1Gi'
          }
        }
      ]
      scale: {
        minReplicas: 1
        maxReplicas: 5
      }
    }
  }
}


output url string = 'https://${catalogContainerApp.properties.configuration.ingress.fqdn}'


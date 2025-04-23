param location string
param containerName string
param envName string 
param imageName string

@secure()
param sqldbConnectionString string

@secure()
param servicebusConnectionString string

@secure()
param jwtSecret string

@secure()
param postgresdbConnectionString string

@secure()
param secureTokenSettings string

resource env 'Microsoft.App/managedEnvironments@2024-10-02-preview' existing = {
  name: envName
}

param apigatewayUrl string
param authapiUrl string

resource shoppingCartContainerApp 'Microsoft.App/containerapps@2024-10-02-preview' = {
  name: containerName
  location: location
  identity: {
    type: 'None'
  }
  properties: {
    managedEnvironmentId: env.id
    configuration: {
      secrets: [
        {
          name: 'servicebus-connection-string'
          value: servicebusConnectionString
        }
        {
          name: 'sqldb-connection-string'
          value: sqldbConnectionString
        }
        {
          name: 'jwt-secret'
          value: jwtSecret
        }
        {
          name: 'postgresdb-connection-string'
          value: postgresdbConnectionString
        }
        {
          name: 'reg-pswd-bec032da-9240'
        }
        {
          name: 'secure-token-settings'
          value: secureTokenSettings
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
      //     passwordSecretRef: 'reg-pswd-bec032da-9240'
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
              name: 'ConnectionStrings__Database'
              secretRef: 'postgresdb-connection-string'
            }
            {
              name: 'ConnectionStrings__SqlDatabase'
              secretRef: 'sqldb-connection-string'
            }
            {
              name: 'ApiSettings__GatewayAddress'
              value: apigatewayUrl
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
            {
              name: 'ServiceBus__ConnectionString'
              secretRef: 'servicebus-connection-string'
            }
            {
              name: 'SecureTokenSettings__CartKey'
              secretRef: 'secure-token-settings'
            }
            {
              name: 'ServiceBus__QueueName'
              value: 'checkout-msgs'
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
        maxReplicas: 10
        cooldownPeriod: 300
        pollingInterval: 30
      }
    }
  }
}

output url string = 'https://${shoppingCartContainerApp.properties.configuration.ingress.fqdn}'

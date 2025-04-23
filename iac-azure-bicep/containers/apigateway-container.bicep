param location string
param envName string
param containerName string
param imageName string
param authapiUrl string
param orderingUrl string
param catalogUrl string
param shoppingcartUrl string

resource env 'Microsoft.App/managedEnvironments@2024-10-02-preview' existing = {
  name: envName
}

resource apigatewayContainerApp 'Microsoft.App/containerapps@2024-10-02-preview' = {
  name: containerName
  location: location
  properties: {
    managedEnvironmentId: env.id
    configuration: {
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
              name: 'ReverseProxy__Clusters__catalog-cluster__Destinations__destination1__Address'
              value: catalogUrl
            }
            {
              name: 'ReverseProxy__Clusters__shoppingCart-cluster__Destinations__destination1__Address'
              value: shoppingcartUrl
            }
            {
              name: 'ReverseProxy__Clusters__ordering-cluster__Destinations__destination1__Address'
              value: orderingUrl
            }
            {
              name: 'ReverseProxy__Clusters__auth-cluster__Destinations__destination1__Address'
              value: authapiUrl
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
      volumes: []
    }
  }
}

output url string = 'https://${apigatewayContainerApp.properties.configuration.ingress.fqdn}'


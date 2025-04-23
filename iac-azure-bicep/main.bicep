param envName string = 'aca-anv'
param catalogContainerName string = 'catalog-api'
param apigatewayContainerName string = 'api-gateway'
param shoppingcartContainerName string = 'shopping-cart-api'
param orderingContainerName string = 'ordering-api'
param authContainerName string = 'auth-api'
//param angularContainerName string = 'angular-app'
param serviceBusName string = 'coffeeshop-sb'
param queueName string = 'coffeeshop'
param shoppingCartDbPostgresSqlName string = 'shoppingcart-db-postgresql'
param sqlServerName string = 'coffeeshop-sqlserver'
param sqlDbName string = 'coffeeshop-sqldb'
param checkoutFuncName string = 'process-checkout'
param certName string = 'coffeeshop-cert'
param location string = resourceGroup().location

param catalogImageName string
param shoppingCartImageName string
param authImageName string
param checkoutFuncImageName string
param apigatewayImageName string
param orderingImageName string

@secure()
param certValue string
@secure()
param certPassword string
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
@secure()
param administratorLogin string
@secure()
param administratorLoginPassword string

module env './containers/container-app-env.bicep' = {
  name: 'env'
  params: {
    location: location
    certName: certName
    envName: envName
    certPassword: certPassword
    certValue: certValue
  }
}

module sqlServer './infrastructure/sqlserver.bicep' = {
  name: 'sql'
  params:{
    administratorLogin: administratorLogin
    administratorLoginPassword: administratorLoginPassword
    location: location
    sqlServerName: sqlServerName
    dbName: sqlDbName
  }
}

module postgressql './infrastructure/postgresql.bicep' = {
  name: 'postgresql'
  params: {
    location: location
    administratorLogin: administratorLogin
    administratorLoginPassword: administratorLoginPassword
    postgresqlName: shoppingCartDbPostgresSqlName
  }
}

module servicebus './infrastructure/servicebus.bicep' = {
  name: 'servicebus'
  params: {
    location: location
    namespaceName: serviceBusName
    queueName: queueName
  }
}

module catalogContainer './containers/catalog-container.bicep' ={
  name: 'catalog-container'
  params: {
    containerName: catalogContainerName
    envName: env.name
    location: location
    sqldbConnectionString: sqldbConnectionString
    imageName: catalogImageName
  }
}

module orderingContainer './containers/ordering-container.bicep' = {
  name: 'ordering-container'
  params: {
    location: location
    authapiUrl: authContainer.outputs.url
    containerName: orderingContainerName
    envName: env.name
    imageName: orderingImageName
  }
}

module authContainer './containers/auth-container.bicep' = {
  name: 'auth-container'
  params: {
    location: location
    containerName: authContainerName
    envName: env.name
    imageName: authImageName
  }
}

module shoppingCartContainer './containers/shoppingcart-container.bicep' ={
  name: 'shopping-cart-container'
  params: {
    containerName: shoppingcartContainerName
    envName: env.name
    location: location
    sqldbConnectionString: sqldbConnectionString
    imageName: shoppingCartImageName
    jwtSecret: jwtSecret
    postgresdbConnectionString: postgresdbConnectionString
    secureTokenSettings: secureTokenSettings
    servicebusConnectionString: servicebusConnectionString
    apigatewayUrl: 'https://${apigatewayContainerName}.${env.outputs.location}.azurecontainerapps.io'
    authapiUrl: authContainer.outputs.url
  }
}

module checkoutFunc './containers/checkout-func-container.bicep' = {
  name: 'checkout-func-container'
  params: {
    containerName: checkoutFuncName
    envName: envName
    location: location
    imageName: checkoutFuncImageName
    servicebusConnectionString: servicebusConnectionString
    orderApiUrl: '${apigatewayContainer.outputs.url}/ordering/orders/create'
  }
}

module apigatewayContainer './containers/apigateway-container.bicep' = {
  name: 'apigateway-container'
  params: {
    location: location
    authapiUrl: authContainer.outputs.url
    catalogUrl: catalogContainer.outputs.url
    containerName: apigatewayContainerName
    envName: env.name
    imageName: apigatewayImageName
    orderingUrl: orderingContainer.outputs.url
    shoppingcartUrl: shoppingCartContainer.outputs.url
  }
}


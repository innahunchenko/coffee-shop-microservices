param namespaceName string
param location string
param queueName string

resource namespaces 'Microsoft.ServiceBus/namespaces@2024-01-01' = {
  name: namespaceName
  location: location
}

resource namespacesAuthRule 'Microsoft.ServiceBus/namespaces/authorizationrules@2024-01-01' = {
  parent: namespaces
  name: 'RootManageSharedAccessKey'
  properties: {
    rights: [
      'Listen'
      'Manage'
      'Send'
    ]
  }
}

resource defaultRule 'Microsoft.ServiceBus/namespaces/networkrulesets@2024-01-01' = {
  parent: namespaces
  name: 'default'
}

resource queue 'Microsoft.ServiceBus/namespaces/queues@2024-01-01' = {
  parent: namespaces
  name: queueName
}

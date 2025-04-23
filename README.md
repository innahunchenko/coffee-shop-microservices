This is the backend solution for the Coffee Shop app. It is built using ASP.NET Core and follows a microservices architecture. The project includes multiple Web API services, each responsible for a specific domain such as product catalog, orders, user management, and payments.
The project includes Bicep files for automating the deployment of Azure resources:

1. Azure Container Apps: Deploys microservices such as Catalog API, Ordering API, Authentication API, Shopping Cart API, and API Gateway.
2. SQL Server: Hosts the main relational database (coffeeshop-sqldb) for storing user and order data.
3. PostgreSQL: Manages the shopping cart data (shoppingcart-db-postgresql).
4. Azure Service Bus: Handles message queuing and communication between microservices (e.g., for order processing).
5. Azure Functions: Manages checkout processing via the process-checkout function.
6. TLS/SSL Certificate: Ensures secure connections with the coffeeshop-cert


External Dependencies and Links:

1. The API Gateway links all microservices together, routing requests from the frontend to the appropriate services.
2. The Authentication API is used for securing user access, with JWT tokens being validated across services.

The project utilizes Docker to containerize all backend services, Docker Compose is used for local development

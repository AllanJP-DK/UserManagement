# User Management API

A REST API for user management with role-based access control and comprehensive audit logging.

## 1. Overview

This project implements a CRUD REST API for user management that allows for creating, reading, updating, and deleting users, roles, access rights, and addresses. All actions are automatically logged for audit purposes.

### 1.1 Key Features

- **User Management**: Create, retrieve, update, and delete user accounts
- **Role-Based Access Control**: Assign roles with specific access rights to users
- **Address Management**: Associate addresses with users
- **Audit Logging**: Automatically track all operations (create, update, delete) with user attribution
- **Transaction Support**: Create complete user profiles in a single atomic transaction

## 2. Technology Stack

- **Framework**: ASP.NET Core 6.0
- **ORM**: Entity Framework Core 6.0
- **Database**: PostgreSQL
- **API Documentation**: Swagger/OpenAPI

## 3. Project Structure

The project follows a standard ASP.NET Core architecture:

```
UserManagement/
├─ Controllers/         # API endpoints
├─ Models/              # Domain entities
├─ DTOs/                # Data transfer objects
├─ Data/                # Database context and configurations
├─ Repositories/        # Data access layer
├─ Services/            # Business logic and services
├─ Program.cs           # Application entry point and configuration
├─ appsettings.json     # Application settings
```

## 4. Database Schema

The database includes the following tables:

- `users`: User accounts
- `roles`: User roles (e.g., Administrator, User)
- `accessrights`: Access permissions (e.g., Users:Read, Users:Write)
- `addresses`: User addresses
- `userroles`: Many-to-many relationship between users and roles
- `roleaccessrights`: Many-to-many relationship between roles and access rights
- `auditlogs`: Log of all operations performed on the system

## 5. API Endpoints

### 5.1 Users

- `GET /api/Users` - Get all users
- `GET /api/Users/{id}` - Get user by ID
- `POST /api/Users` - Create a user
- `POST /api/Users/CreateWithDetails` - Create a full user with address and roles in a transaction
- `PUT /api/Users/{id}` - Update a user
- `DELETE /api/Users/{id}` - Delete a user

### 5.2 Roles

- `GET /api/Roles` - Get all roles
- `GET /api/Roles/{id}` - Get role by ID
- `POST /api/Roles` - Create a role
- `PUT /api/Roles/{id}` - Update a role
- `DELETE /api/Roles/{id}` - Delete a role

### 5.3 Access Rights

- `GET /api/AccessRights` - Get all access rights
- `GET /api/AccessRights/{id}` - Get access right by ID
- `POST /api/AccessRights` - Create an access right
- `PUT /api/AccessRights/{id}` - Update an access right
- `DELETE /api/AccessRights/{id}` - Delete an access right

### 5.4 Addresses

- `GET /api/Addresses` - Get all addresses
- `GET /api/Addresses/{id}` - Get address by ID
- `POST /api/Addresses` - Create an address
- `PUT /api/Addresses/{id}` - Update an address
- `DELETE /api/Addresses/{id}` - Delete an address

### 5.5 Audit Logs

- `GET /api/AuditLogs` - Get all audit logs with optional filtering
- `GET /api/AuditLogs/{id}` - Get audit log by ID
- `GET /api/AuditLogs/ByUser/{userId}` - Get audit logs for a specific user
- `GET /api/AuditLogs/ByTimeInterval` - Get audit logs within a time range
- `GET /api/AuditLogs/ByUserAndTimeInterval/{userId}` - Get audit logs for a user within a time range
- `GET /api/AuditLogs/ByOperation/{operation}` - Get audit logs for a specific operation
- `GET /api/AuditLogs/ByTable/{tableName}` - Get audit logs for a specific table

## 6. Setup and Configuration

### 6.1 Prerequisites

- .NET 6.0 SDK or later
- PostgreSQL server
- IDE (Visual Studio, VS Code, or similar)

### 6.2 Database Configuration

1. Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Username=yourusername;Password=yourpassword;Database=usermanagement;"
  }
}
```

### 6.3 Running the Application

```bash
# Clone the repository
git clone https://github.com/yourusername/UserManagement.git

# Navigate to the project directory
cd UserManagement

# Run the application
dotnet run
```

The API will be available at:
- http://localhost:5164 (or similar port - check console output)

Swagger documentation will be available at:
- http://localhost:5164/swagger

## 7. Usage Examples

### 7.1 Creating a User with Transaction

```bash
curl -X 'POST' \
  'http://localhost:5164/api/Users/CreateWithDetails' \
  -H 'accept: text/plain' \
  -H 'Content-Type: application/json' \
  -d '{
  "username": "allanjp",
  "firstName": "Allan Juhl",
  "lastName": "Petersen",
  "street": "Api Vænget 13",
  "postalCode": "4000",
  "roleIds": [
    "01969bb5-9108-728f-87da-675da796454d"
  ]
}'
```

### 7.2 Querying Audit Logs

Get all actions performed by a specific user:
```
GET /api/AuditLogs/ByUser/01969bb5-90e0-755b-b6cb-7cfa4db50ad3
```

Get all actions within a date range:
```
GET /api/AuditLogs/ByTimeInterval?startDate=2025-05-04&endDate=2025-05-05
```

## 8. Key Implementation Details

### 8.1 Automatic Audit Logging

All write operations (POST, PUT, DELETE) are automatically logged using middleware that captures:
- The table affected
- The operation performed
- The user who performed the action
- The timestamp

### 8.2 Transaction Support

The API supports creating complex entities in a single transaction, ensuring data consistency:

```csharp
using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
{
    // Create address
    // Create user
    // Assign roles
    
    transactionScope.Complete();
}
```

### 8.3 Optimized Database Queries

The audit log queries are optimized for performance with:
- Proper indexing on userId and changedAt columns
- Filtered queries that leverage database indexes
- Result limiting to prevent large result sets

## 9. Initial Data

On first run, the application seeds the database with:
- Admin user
- Administrator role
- Basic access rights (Users:Read, Users:Write, Roles:Read, Roles:Write, AuditLogs:Read)

## 10. Troubleshooting

### Common Issues

1. **Connection string issues**: Make sure your PostgreSQL server is running and accessible using the connection string in appsettings.json.

2. **Port conflicts**: If the ports are already in use, you can change them in the Properties/launchSettings.json file.

## 11. License

[Your License Here]

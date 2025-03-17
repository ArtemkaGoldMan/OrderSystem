# Order Management System

A console application for processing orders with business rules implementation and clean architecture principles.

## Project Overview

This Order Management System is a console application built with C# and .NET, designed to handle order processing with specific business rules and constraints. The system implements clean architecture principles and various design patterns to ensure maintainability, testability, and scalability.

### Technologies Used
- C# / .NET
- Entity Framework Core
- SQLite Database
- xUnit for Testing

### Project Structure
```
└── 📁OrderSystem
    └── 📁Data
        └── DatabaseContext.cs
        └── 📁Implementations
            └── OrderRepository.cs
        └── 📁Interfaces
            └── IOrderRepository.cs
        └── 📁Migrations
            └── 20250315170509_InitialCreate.cs
            └── 20250315170509_InitialCreate.Designer.cs
            └── DatabaseContextModelSnapshot.cs
    └── 📁Factories
        └── 📁Implementations
            └── OrderFactory.cs
        └── 📁Interfaces
            └── IOrderFactory.cs
    └── 📁Models
        └── CustomerType.cs
        └── Order.cs
        └── OrderStatus.cs
        └── PaymentMethod.cs
    └── 📁Services
        └── 📁Implementations
            └── OrderService.cs
        └── 📁Interfaces
            └── IOrderService.cs
    └── orders.db
    └── OrderSystem.csproj
    └── Program.cs
```
```
└── 📁OrderSystem.Tests
    └── 📁Repositories
        └── OrderRepositoryTests.cs
    └── 📁Services
        └── OrderServiceTests.cs
    └── OrderSystem.Tests.csproj
```

## System Architecture
### Design Patterns Implemented
1. **Repository Pattern**
   - Abstracts data access layer
   - Provides CRUD operations for orders
   - Implemented through `IOrderRepository` and `OrderRepository`

2. **Factory Pattern**
   - Creates order objects with predefined settings
   - Implemented through `IOrderFactory` and `OrderFactory`
   - Ensures consistent order creation

3. **Dependency Injection**
   - Used throughout the application
   - Configured in `Program.cs`
   - Enables loose coupling and testability

### Key Components
- **Models**
  - `Order`: Core entity with properties
  - `CustomerType`: Enum (Company, Individual)
  - `OrderStatus`: Enum (New, InWarehouse, InShipping, etc.)
  - `PaymentMethod`: Enum (Card, CashOnDelivery)

- **Repositories**
  - `IOrderRepository`: Interface defining data operations
  - `OrderRepository`: Implementation of data access

- **Services**
  - `IOrderService`: Interface defining business operations
  - `OrderService`: Implementation of business logic

- **Factories**
  - `IOrderFactory`: Interface for order creation
  - `OrderFactory`: Implementation of order creation

- **Database**
  - `DatabaseContext`: Entity Framework Core context
  - SQLite database for data persistence

## Business Rules

### Order Statuses
1. New
2. InWarehouse
3. InShipping
4. ReturnedToCustomer
5. Error
6. Closed
7. Cancelled

### Business Constraints
1. Orders with amount ≥ 2500 and CashOnDelivery payment are returned to customer when sent to warehouse
2. Orders in shipping status change to closed after 5 seconds
3. Orders without delivery address result in error status
4. Orders can only be cancelled in specific states
5. Orders can only be deleted when closed or cancelled

### Order Properties
- Product Name (required, max 100 chars)
- Amount (required, range 0-999999.99)
- Customer Type (required: Company/Individual)
- Delivery Address (max 255 chars)
- Payment Method (required: Card/CashOnDelivery)
- Status (default: New)

## Features

1. Create new orders

2. Send orders to warehouse

3. Send orders to shipping

4. View orders

5. Cancel orders

6. Delete orders

## Testing

### Test Coverage
- Repository Tests
  - CRUD operations
  - Edge cases
  - Error handling

- Service Tests
  - Business rules
  - Status transitions
  - Input validation

## Getting Started

### Prerequisites
- .NET SDK
- Visual Studio or VS Code
- SQLite

### Installation
1. Clone the repository
2. Navigate to the project directory
3. Restore NuGet packages
4. Build the solution

### Running the Application
1. Navigate to the project directory
2. Run `dotnet run`
3. Follow the console menu

### Running Tests
1. Navigate to the test project directory
2. Run `dotnet test`

## Usage Examples

### Creating an Order
```
1. Select option 1 from main menu
2. Enter product name
3. Enter amount
4. Select customer type (1-Individual, 2-Company)
5. Enter delivery address
6. Select payment method (1-Card, 2-CashOnDelivery)
```

### Sending to Warehouse
```
1. Select option 2 from main menu
2. View available orders
3. Enter order ID
4. System validates and processes the request
```

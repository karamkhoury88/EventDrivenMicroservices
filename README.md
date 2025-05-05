# Simple .Net Event Driven Microservices Example

This project demonstrates a simple implementation of event-driven microservices using .NET. It includes two services: `OrderService` and `InventoryService`, which communicate asynchronously via RabbitMQ. The project is designed to showcase best practices for building scalable, maintainable, and loosely coupled microservices.

---

## Table of Contents
- [Features](#features)
- [Technologies Used](#technologies-used)
- [Installation](#installation)
- [Usage](#usage)
- [API Documentation](#api-documentation)
- [Event Flow](#event-flow)
- [Contributing](#contributing)

---

## Features
- Event-driven architecture using RabbitMQ.
- Two microservices: `OrderService` and `InventoryService`.
- DTOs for event communication.
- RESTful APIs for managing orders and products.
- Centralized common library for shared components.

---

## Technologies Used
- **.NET 9**: Framework for building microservices.
- **RabbitMQ**: Message broker for event-driven communication.
- **Entity Framework Core**: ORM for database interactions.
- **SQLite**: Database for persisting data.
- **Scalar**: API documentation and testing.

---

## Installation

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### Steps
1. Clone the repository:
   ```console
     git clone https://github.com/karamkhoury88/EventDrivenMicroservices.git
     cd EventDrivenMicroservices.
   ```

3. Update the connection strings for both `OrderService` and `InventoryService` to use SQLite.

4. Build and run the services using .NET Aspire:

## Usage

### Running the Services
1. Start the `OrderService` and `InventoryService` projects.
2. Use Scalar to interact with the APIs.

### Example Workflow
1. Create a product in the `InventoryService`.
2. Place an order in the `OrderService`.
3. Observe the event-driven updates between the services.

---

## API Documentation

### OrderService
- **GET /api/orders**: Retrieve all orders.
- **POST /api/orders**: Create a new order.
- **GET /api/products**: Retrieve all products.

### InventoryService
- **GET /api/products**: Retrieve all products.
- **POST /api/products**: Create a new product.

---

## Event Flow
1. **ProductCreatedEvent**: Published by `InventoryService` when a new product is created.
2. **OrderCreatedEvent**: Published by `OrderService` when a new order is placed.
3. **ProductUpdatedEvent**: Published by `InventoryService` when a product is updated.

---

## Contributing
Contributions are welcome! Please follow these steps:
1. Fork the repository.
2. Create a new branch: `git checkout -b feature-name`.
3. Commit your changes: `git commit -m "Add feature"`.
4. Push to the branch: `git push origin feature-name`.
5. Open a pull request.
   
   

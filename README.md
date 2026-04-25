# Subscription Billing System

A production-style **Subscription Billing System** built with **.NET 9**, following **Clean Architecture** and **Domain-Driven Design (DDD)** principles.

---

## 🚀 Features

- Customer Management  
- Subscription Lifecycle  
  - Create Subscription  
  - Activate Subscription  
  - Cancel Subscription  
- Invoice Management  
  - Automatic invoice generation  
  - Payment handling  
- Domain Events  
- Outbox Pattern (reliable event processing)  
- Background Job with retry (exponential backoff)  
- Unit tested domain layer  

---

## 🧱 Architecture

```
API → Application → Domain
             ↓
        Infrastructure
```

---

### Domain Layer
- Aggregates: Customer, Subscription, Invoice  
- Value Objects: Email, Money  
- Domain Events  
- Contains all business rules and invariants  

---

### Application Layer
- CQRS (Commands & Queries)  
- MediatR handlers  
- Validation  
- Pipeline behaviors  

---

### Infrastructure Layer
- EF Core (DbContext)  
- Outbox Pattern  
- Background processing  
- Retry mechanism  

---

### API Layer
- Minimal APIs  
- Swagger  
- Global exception handling  

---

## 🧠 Key Design Decisions

### Domain-Driven Design
Business logic is encapsulated inside aggregates.

---

### Outbox Pattern
Events are stored in database and processed asynchronously to ensure reliability and avoid data loss.

---

### Retry Mechanism
Includes:
- RetryCount  
- MaxRetryCount  
- NextRetryAt  
- LastError  

Uses exponential backoff for retries.

---

### Aggregate Root Pattern
Only aggregate roots modify internal collections to maintain consistency.

---

### EF Core Tracking Fix
Custom handling ensures correct INSERT/UPDATE behavior for child entities inside aggregates.

---

## ⚙️ How to Run

### Clone repository
```
git clone https://github.com/shahzadahmad/SubscriptionBillingSystem.git
cd SubscriptionBillingSystem
```

### Run API
```
dotnet run --project SubscriptionBillingSystem.Api
```

### Open Swagger
```
https://localhost:<port>/swagger
```

---

## 🧪 Running Tests

```
dotnet test
```

---

## 🔄 Sample Workflow

1. Create Customer  
2. Create Subscription  
3. Activate Subscription → generates Invoice  
4. Pay Invoice → triggers event  

---

## 🧰 Tech Stack

- .NET 9  
- EF Core  
- MediatR  
- FluentValidation  
- xUnit  
- FluentAssertions  

---

## 📈 Future Improvements

- Idempotent payment APIs  
- Dead letter queue  
- Distributed messaging (Kafka / RabbitMQ)  
- Authentication & authorization  

---

## 👨‍💻 Author

Shahzad Ahmad

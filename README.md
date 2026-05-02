# Subscription Billing System

A production-style **Subscription Billing System** built with **.NET 9**, following **Clean Architecture**, **Domain-Driven Design (DDD)**, and **event-driven architecture** principles.

---

## 🚀 Features

- Customer Management  
- Subscription Lifecycle  
  - Create Subscription  
  - Activate Subscription  
  - Cancel Subscription  
- Invoice Management  
  - Asynchronous invoice generation (event-driven)  
  - Payment handling  
- Domain Events (decoupled aggregates)  
- Outbox Pattern (reliable event processing)  
- Background Job with retry (exponential backoff)  
- CQRS (Commands & Queries separation)  
- Unit tested domain layer  

---

## 🧱 Architecture

API → Application → Domain  
             ↓  
        Infrastructure  

---

## 📦 Layers Overview

### Domain Layer
- Aggregates: Customer, Subscription, Invoice  
- Value Objects: Email, Money  
- Domain Events (pure, decoupled)  
- Contains all business rules and invariants  
- No cross-aggregate dependencies  

---

### Application Layer
- CQRS (Commands & Queries)  
- MediatR handlers  
- Event Handlers (application-level orchestration)  
- Transaction behavior (applies only to commands)  
- Validation  
- Depends only on abstractions (`IAggregateContext`, `IReadDbContext`)  
- **No direct dependency on Infrastructure layer (EF Core removed from Application layer)**  

---

### Infrastructure Layer
- EF Core (DbContext)  
- Implements `IAggregateContext` (write side)  
- Implements `ReadDbContext` / `IReadDbContext` (read side)  
- Outbox Pattern implementation  
- Background processing service  
- Retry mechanism with exponential backoff  
- Safe event serialization/deserialization  

---

### API Layer
- Minimal APIs  
- Swagger  
- Global exception handling  

---

## 🧠 Key Design Decisions

### ✅ Clean CQRS Separation (NEW)
- Application layer no longer depends on Infrastructure
- Introduced abstractions:
  - `IAggregateContext` → write side (aggregates + persistence)
  - `IReadDbContext` / `ReadDbContext` → query side
- Clear separation between command and query responsibilities

---

### ✅ Aggregate Decoupling
- Removed direct dependency between Subscription and Invoice  
- Subscription raises InvoiceGenerationRequestedEvent  
- Invoice creation handled in Application layer  

---

### ✅ Event-Driven Architecture
- Domain raises events → Application handles them  
- Enables loose coupling and scalability  

---

### ✅ Outbox Pattern
- Events stored in DB and processed asynchronously  
- Ensures reliability and eventual consistency  

---

### ✅ Serialization Fix
- Fixed serialization using runtime type  
- Prevented data loss during deserialization  
- Supports complex Value Objects like Money  

---

### ✅ Retry Mechanism
- RetryCount, MaxRetryCount  
- NextRetryAt, LastError  
- Exponential backoff  

---

### ✅ Transaction Handling
- Transactions only for Commands  
- Queries do not trigger SaveChanges  
- Controlled via Application-level Transaction Behavior  

---

## ⚙️ How to Run

```bash
git clone https://github.com/shahzadahmad/SubscriptionBillingSystem.git  
cd SubscriptionBillingSystem  

dotnet run --project SubscriptionBillingSystem.Api    

---

## 🧪 Running Tests

dotnet test  

---

## 🔄 Sample Workflow

1. Create Customer  
2. Create Subscription  
3. Activate Subscription  
4. Generate Invoice (event-driven)  
5. Pay Invoice  

---

## 🧰 Tech Stack

- .NET 9  
- EF Core  
- MediatR  
- FluentValidation  
- xUnit  
- FluentAssertions  

---

## 👨‍💻 Author

Shahzad Ahmad

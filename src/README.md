# Fitness Workout Tracker

A fitness tracking API built with ASP.NET Core that focuses on **rich domain modeling** and **Clean Architecture**.

The primary goal of this project is to demonstrate how complex business workflows can be modeled through behavior-rich domain objects while maintaining clear separation of concerns across application layers.

Unlike many CRUD-oriented applications where entities act as simple data containers and business logic is implemented through procedural service methods, this project models business behavior directly within domain entities. Workouts, scheduled workouts, and exercise progress objects own their state transitions and enforce their own business rules.

---

## Technical Highlights

- Rich Domain Model
- Clean Architecture
- Integration Testing With Test Containers
- FastEndpoints
- Entity Framework Core
- SQL Server
- JWT Authentication
- Specification Pattern
- Pagination, Filtering, and Sorting
- FluentValidation
- Health Checks
- Structured Logging

---

## Architecture

```text
Presentation (FastEndpoints)
          ↓
Application (Use Cases)
          ↓
Domain (Business Rules)
          ↓
Infrastructure (EF Core, SQL Server)
```

### Layer Responsibilities

#### Domain

The Domain layer contains the core business concepts and rules of the application.

Key entities include:

- User
- Workout
- Exercise
- ScheduledWorkout
- ExerciseProgress
- Note

Rather than exposing public setters and relying on service methods to manipulate state, entities encapsulate behavior through methods that enforce valid business operations.

Examples of domain behaviors include:

- Scheduling a workout
- Starting a scheduled workout
- Completing a scheduled workout
- Cancelling a scheduled workout
- Rescheduling a workout
- Starting exercise progress
- Completing exercise progress
- Skipping exercise progress

This approach helps maintain consistency by preventing invalid state transitions and keeping business rules close to the data they govern.

#### Application

The Application layer contains use cases that coordinate domain objects and infrastructure dependencies.

Responsibilities include:

- Retrieving aggregates
- Invoking domain behaviors
- Persisting changes
- Returning application results

Business rules remain inside the domain model rather than being duplicated across handlers and services.

#### Infrastructure

The Infrastructure layer contains implementation details such as:

- Entity Framework Core
- SQL Server persistence
- Repository implementations
- Authentication services
- Logging adapters

#### Public API

The API layer exposes functionality through FastEndpoints and remains intentionally thin.

Responsibilities include:

- Request handling
- Validation
- Authentication
- Response mapping

---

## Why Rich Domain Modeling?

Many applications follow an anemic domain model where entities are little more than DTOs and business rules are implemented through procedural service calls.

This project takes the opposite approach.

Instead of:

```csharp
scheduledWorkout.Status = ScheduledWorkoutStatus.Completed;
```

Business operations are performed through domain behaviors:

```csharp
scheduledWorkout.Complete();
```

The entity itself is responsible for determining whether the operation is valid and for enforcing all associated business rules.

This keeps business logic centralized, testable, and easier to evolve as the application grows.

---

## Features

### Authentication

- User registration
- User login
- JWT authentication

### Workout Management

- Create workouts
- Update workouts
- Delete workouts
- Retrieve workouts
- Retrieve workout details

### Exercise Management

- Create exercises
- Retrieve exercises

### Scheduled Workouts

- Schedule workouts
- Start workouts
- Complete workouts
- Cancel workouts
- Reschedule workouts
- Delete scheduled workouts
- Retrieve scheduled workouts
- Retrieve scheduled workout details

### Exercise Progress Tracking

- Start exercise progress
- Complete exercise progress
- Skip exercise progress
- Add notes
- Delete progress records
- Retrieve progress history

---

## Query Capabilities

The API supports:

- Pagination
- Filtering
- Sorting

using the Specification Pattern to keep query logic reusable and maintainable.

---

## Project Structure

```text
src/
├── PublicApi
├── Application
├── Domain
└── Infrastructure

tests/
└── PublicApiIntegrationTests
```

---

## Getting Started

### Prerequisites

- .NET 9
- SQL Server

### Configure the Connection String

Update `src/PublicApi/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=FitnessWorkoutTracker;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

### Build the Solution

```powershell
dotnet build FitnessWorkoutTracker.slnx
```

### Run the API

```powershell
dotnet run --project src/PublicApi/PublicApi.csproj
```

Swagger/OpenAPI documentation is available automatically in development mode.

---

## Testing

Integration tests verify:

- Authentication flows
- Endpoint behavior
- Domain workflows
- Request and response contracts
- Database interactions

Run tests using:

```powershell
dotnet test tests/PublicApiIntegrationTests/PublicApiIntegrationTests.csproj
```

---

## Health Checks

The application exposes a health check endpoint:

```text
/health
```

This endpoint can be used to verify that the API and its dependencies are operating correctly.

---

## What This Project Demonstrates

This project was built to demonstrate backend engineering practices beyond basic CRUD development:

- Designing behavior-rich domain models
- Encapsulating business rules inside entities
- Applying Clean Architecture principles
- Maintaining separation of concerns
- Building testable application layers
- Implementing integration-tested APIs
- Modeling real business workflows through domain behavior rather than procedural service logic

While the application focuses on fitness tracking, the primary objective is showcasing maintainable backend design and domain-driven thinking.

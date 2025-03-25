# Candidate Management API

A lightweight .NET 8 Web API for managing job candidate information.  
---

## 🚀 What It Does

- Accepts candidate data via a single API endpoint
- Uses **email** as the unique identifier
- If the candidate exists → updates them  
  If not → creates a new one
- Supports **partial updates** (only send what you want to change)
- Validates inputs (like time formats)
- Saves data to SQLite DB
- Caches candidates in memory for performance (Caches the updated/added entity, because it has higher probability of being changed in following 10 minutes)

---

## Why I Built It Like This

I tried to keep things clean and scalable. Here's the thinking behind the architecture and decisions:

### 🔹 Clean Architecture

I structured it into layers:
- **API** – presentation & middleware
- **Application** – interfaces, DTOs, services
- **Domain** – core entities
- **Infrastructure** – EF Core, caching, repositories
- **Tests** – xUnit + Moq

This makes it easy to test, replace things (like the DB or caching, experienced in my case), and grow over time.

---

### 🔹 Database: Why SQLite?

I actually started with **PostgreSQL**, but realized that for a recruiter or teammate to run the project easily, asking them to set up Postgres isn’t great.  
So I switched to **SQLite**, which:
- Works anywhere
- Requires no setup
- Just press "Run" in Visual Studio and it works

Migrations are applied automatically on app start.

---

### 🔹 Caching: First Redis, Then In-Memory

Initially I used **Redis** for caching recently used candidates (to avoid DB hits on repeat updates, and read the repeatedly requested candidates faster).  
That worked great — but Redis requires Docker or external setup.

To meet the task’s **“just run the app”** requirement, I, at the end, swapped it for **.NET’s built-in in-memory cache**.

And Clean Architecture came handy there — I didn’t have to touch anything except one line in `Program.cs` and a class file.  
Everything else stayed the same.

---

## 📦 Tech Stack

- .NET 8
- SQLite via Entity Framework Core
- In-Memory Caching (IMemoryCache)
- Clean Architecture
- FluentValidation
- xUnit + Moq

---

## 🛠 How to Run

> No setup, no Docker, no external DB — just run it.

- Clone the the repository into your device;
- Open it in Visual Studio and hit the run button;
- Or just type `dotnet run` in terminal;


<br>

## 💬 Assumptions Made
- No auth needed (not in requirements)

- One endpoint only — extendable in future

- Used caching for “recently created or updated” candidates to avoid repeated DB hits

- SQLite and in-memory caching used for 100% self-contained project

- Partial update logic prioritizes readability and simplicity

<br>

# Future Improvements

These are things I would improve or implement if I had more time, or if this project were going into production.

---

## 1. Better Exception Handling

Currently:
- We have a global exception middleware that returns a generic 500 error for unhandled exceptions.

Improvements:
- Introduce custom exception types (e.g., `ValidationException`, `NotFoundException`, etc.)
- Catch and return proper status codes (e.g., 400, 404)
- Wrap all responses with a consistent format (e.g., `{ statusCode, message, errors }`)
- Add logging categories + correlation IDs for traceability

---

## 2. Add Authentication/Authorization

Currently:
- No auth is implemented, since the task didn’t request it.

If needed:
- Add basic JWT token authentication
- Add role-based access (e.g., Admin can write, User can only view)

---

# 3. Add Integration Tests

Currently:
- All logic is unit-tested (service + repo), but we don’t test the full request pipeline.

Improvements:
- Use WebApplicationFactory to test full API endpoint behavior
- Assert actual HTTP status codes, DB writes, and validation responses

---

## 4. Add CI/CD Pipeline

Currently:
- Tests must be run manually.

Improvements:
- Add GitHub Actions to run tests on every push/PR

---

## 6. Redis Support (Prod-Ready Caching)

Currently:
- We swapped Redis out for in-memory cache to keep the project self-contained.

In production:
- Bring back Redis (already supported via `ICacheService`)
- Configure expiration policies (sliding vs absolute)
- Use it across other services too (e.g., lookup tables, rate-limiting, auth tokens)

---

## 6. General Refactors & Polish

- Add proper logging (structured logs via Serilog)
- Others

---

## 8. Improve Partial Update Logic

Right now:
- Partial update is handled manually with null/empty checks.

Could be improved by:
- Supporting PATCH with JSON Merge Patch or JSON Patch
- Using a dynamic update DTO pattern with optional fields

---

## Time spent coding: 8 hours, 50 minutes (Including writing documentation for some parts of the code, writing this readme file)
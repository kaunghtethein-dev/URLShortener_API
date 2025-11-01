# BitlyClone Backend

Backend service for a modern URL shortener, built with **ASP.NET Core Web API**, **Entity Framework Core**, and **SQL Server** using **layered architecture**.

---

## Architecture Overview

The solution follows layered architecture and is seperated into different layers:

- **API Layer** – Exposes RESTful endpoints and handles HTTP requests and responses.  
- **Application Layer** – Contains business logic and service classes.  
- **Infrastructure Layer** – Implements data access using EF Core and the repository pattern.  
- **Domain Layer** – Defines core entities and domain models.  
- **Shared Layer** – Holds shared DTOs, constants, and utilities used across layers.

---

## Tech Stack

- **Backend Framework:** ASP.NET Core Web API  
- **Database:** Microsoft SQL Server  
- **ORM:** Entity Framework Core (Code-First approach)  
- **Architecture Pattern:** Layered Architecture  
- **Language:** C#

---

## Project Goal

This project aims to replicate the core functionality of Bit.ly, demonstrating production-grade backend design with:
- Scalable architecture  
- Clean code practices  
- Repository and service patterns  
- Database migrations and seeding  

The frontend (Blazor WebAssembly) is developed in a **separate repository**.

---

## Status

Currently in active development.  


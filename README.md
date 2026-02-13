# Acceloka Ticket Management API

A robust Backend API for managing ticket bookings, with features of:
* available ticket searching
* booking ticket(s)
* ticket revocation
* updating a booked ticket

This project is built using **ASP.NET Core 10**, following the **MARVEL** architecture pattern. 

## 🚀 Architecture & Tech Stack

* **Pattern**: MARVEL (MediatR, FluentValidation, Clean Architecture)
* **Communication**: MediatR for Decoupled Command/Query handling
* **Validation**: FluentValidation with custom Business Logic
* **Database**: SQL Server with Entity Framework Core (Code First)
* **Logging**: Serilog with rolling daily file sinks
* **Error Handling**: Standardized RFC 7807 Problem Details

## 🛠 Prerequisites

* .NET 10 SDK
* SQL Server (LocalDB or SQLEXPRESS)

## 📥 Getting Started

### 1. Configuration

The application uses **User Secrets** for sensitive configurations like connection strings. To set up your local database:

```powershell
# Navigate to the API project
cd Acceloka.Api

# Initialize and set your local connection string
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(YOUR_AVAILABLE_SERVER_NAME);Database=(DATABASE_NAME);Trusted_Connection=True;TrustServerCertificate=True"

```

### 2. Database Migration & Seeding

The application automatically applies migrations as it is a code first type and seeds initial data (Categories and Tickets) upon startup through the `DbInitializer`, seeding the database with tests data especially for tickets so you can test with ease.

### 3. Running the App


```powershell
dotnet run --project Acceloka.Api

```

The API will be available at: `http://localhost:5225`

## 🛰 API Endpoints

### Ticket Discovery

* **GET** `/api/v1/get-available-tickets`
* **Features**: Supports Pagination (`page`, `pageSize`), Multi-column Searching (Category, Code, Name, Price, Date Range), and Dynamic Sorting.

  #### Usage Examples:
  
  * **Search by Category & Max Price:** `GET /api/v1/get-available-tickets?CategoryName=Concert&MaxPrice=5000000`
  * **Search by Ticket Name & Date Range:** `GET /api/v1/get-available-tickets?TicketName=Coldplay&StartEventDate=2026-01-01&EndEventDate=2026-12-31`
  * **Sorting (Ascending/Descending):** `GET /api/v1/get-available-tickets?OrderBy=Price&OrderState=desc`
  * (Options for OrderBy: *TicketName, Category, Price, TicketCode*)
  * **Pagination:** `GET /api/v1/get-available-tickets?page=2&pageSize=5`

### Booking Operations
* **POST** `/api/v1/book-ticket`: Create a new booking for multiple tickets.
* **GET** `/api/v1/get-booked-ticket/{bookedTicketId}`: Retrieve detailed breakdown of a booking.
* **PUT** `/api/v1/edit-booked-ticket/{bookedTicketId}`: Update quantities for existing booked tickets.
* **DELETE** `/api/v1/revoke-ticket/{bookedTicketId}/{ticketCode}/{quantity}`: Partially or fully revoke a booked ticket.

## 📝 Logging

Logs are automatically generated in the `/logs` folder with the naming convention `Log-yyyyMMdd.txt`.

* **Log Level**: Information
* **Details**: Captures request metadata, business validation failures, and database transactions.

## 🛡 Validation Rules

* **Quota Check**: Cannot book or edit quantity beyond available ticket quota.
* **Date Check**: Cannot book tickets for events that have already passed.
* **Quantity Check**: Minimum quantity for any booking or edit is 1.

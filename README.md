# SimpleClinicManager

**A secure, web-based patient workflow tracking application for clinics**

* Tracks patient referrals, counselling, treatments, and follow-ups.
* Built with **.NET 9**, **Azure SQL**, and **Microsoft Entra ID** authentication.
* Implements modern best practices: **Dependency Injection**, **Repository/Unit of Work patterns**, **EF Core**, and secure cloud hosting.

---

## Table of Contents

1. [Features](#features)
2. [Architecture](#architecture)
3. [Technology Stack](#technology-stack)
4. [Project Structure](#project-structure)
5. [Setup & Deployment](#setup--deployment)
6. [Authentication & Security](#authentication--security)
7. [Database](#database)
8. [Usage](#usage)
9. [Contributing](#contributing)
10. [License](#license)

---

## Features

* Secure Entra login with NHS Microsoft 365 account
* Add, update, and track patients through the referral → counselling → treatment workflow
* Display calculated fields for:

  * `WaitTimeReferralToCounselling`
  * `TreatTime` (Counselling → Dispensed)
* Filter/search patient lists
* Color-coded flags for overdue actions
* Export reports to CSV/Excel
* Role-based access support (`ClinicUser`, `ClinicAdmin`)
* Audit logging: CreatedBy/ModifiedBy and timestamps

---

## Architecture

```
[Frontend: Blazor/React] <--> [API: ASP.NET Core Web API] <--> [Database: Azure SQL]
                                ^
                                |
                        Dependency Injection
                                |
                        Repository / UnitOfWork
```

* **Frontend:** Browser-based UI for data entry, viewing, and reporting
* **API Layer:** Handles business logic and data access
* **Database Layer:** Azure SQL stores patient records; calculated fields handled in app or views
* **Authentication:** Microsoft Entra ID (Azure AD)

---

## Technology Stack

* **Framework:** .NET 9 / ASP.NET Core
* **Frontend:** Blazor Server / Blazor WebAssembly or React
* **Backend API:** ASP.NET Core Web API
* **Database:** Azure SQL Database
* **ORM:** Entity Framework Core
* **Authentication:** Microsoft Entra ID (Azure AD, MSAL)
* **Logging:** Serilog
* **Deployment:** Azure App Service
* **Patterns:** Dependency Injection, Repository, Unit of Work

---

## Project Structure

```
ClinicTracking/
├── ClinicTracking.API/           # API controllers, DTOs, services
├── ClinicTracking.Core/          # Domain models, interfaces
├── ClinicTracking.Infrastructure/ # EF Core, repositories, DB context
├── ClinicTracking.Client/        # Blazor/React frontend
└── ClinicTracking.Tests/         # Unit tests
```

---

## Setup & Deployment

1. **Clone repository:**

```bash
git clone https://github.com/your-org/ClinicTracking.git
```

2. **Configure Azure SQL Database:**

* Create database in UK South/West
* Enable **Entra ID authentication**
* Store connection string securely (Azure Key Vault recommended)

3. **Register app in Entra ID:**

* Create **App Registration**
* Configure redirect URI to frontend
* Set **“User assignment required”**
* Assign authorized users or groups

4. **Configure API:**

* Add connection string via `appsettings.json` or environment variables
* Register services, repositories, and DbContext in DI container

5. **Run migrations:**

```bash
dotnet ef database update --project ClinicTracking.Infrastructure
```

6. **Deploy:**

* Publish API and frontend to **Azure App Service**
* Ensure HTTPS enforced

---

## Authentication & Security

* **Entra ID Login**: Users log in with NHS Microsoft 365 credentials
* **Controlled Access**: Only explicitly assigned users can access
* **Role-based authorization**: Optional `ClinicUser` / `ClinicAdmin`
* **Database Security**:

  * TLS 1.2+
  * Transparent Data Encryption (TDE)
  * Managed identity for connection
  * Backups with geo-redundancy

---

## Database

* **Table:** `PatientTracking`

* **Calculated Fields:** Displayed at runtime or via SQL view:

  * `WaitTimeReferralToCounselling = DATEDIFF(day, ReferralDate, CounsellingDate)`
  * `TreatTime = DATEDIFF(day, CounsellingDate, DispensedDate)`

* **Sample SQL View:**

```sql
CREATE VIEW vwPatientTracking AS
SELECT *,
       DATEDIFF(day, ReferralDate, CounsellingDate) AS WaitTimeReferralToCounselling,
       DATEDIFF(day, CounsellingDate, DispensedDate) AS TreatTime
FROM PatientTracking;
```

---

## Usage

* Login using **NHS Microsoft 365 account**
* Add new patients via “Add Patient” screen
* Update workflow dates and treatment information
* View calculated wait times and treatment times
* Export patient list for reporting

---

## Contributing

* Fork repository
* Create feature branch
* Follow coding standards and patterns (DI, Repository/Unit of Work)
* Add unit tests
* Submit Pull Request

---

## License

* [MIT License](LICENSE) – free to use, modify, and distribute


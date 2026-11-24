# Health Care Center Smart Management API

An ASP.NET Core Web API backend for managing a smart healthcare complex, combining core administrative and clinical workflows with AI-assisted doctor note summarization and a patient guidance/booking chatbot.

---

## Key Features

* **Authentication & Role-Based Access Control (RBAC):** JWT Bearer authentication with password hashing (`PasswordHasher<User>`) and five system roles: `Admin`, `Doctor`, `Receptionist`, `Pharmacist`, and `Patient`.  
* **User & Staff Management:** Admin endpoints to create accounts, assign roles, and activate or deactivate users.  
* **Patient Management & Electronic Medical Records (EMR):** Create and update patient profiles, search patients, and retrieve unified medical records (appointments, visits, doctor notes, approved AI summaries, prescriptions, and invoices).  
* **Clinics, Doctors & Working Schedules:** Manage clinics, link doctor profiles to user accounts, and configure per-day working hours (`DoctorSchedules`).  
* **Smart Appointment Scheduling:** Book, filter, reschedule, and update appointment statuses (`Scheduled`, `Completed`, `Cancelled`) with automatic validation against doctor working hours and 30-minute overlap conflict detection.  
* **Clinical Visits, Prescriptions & Billing:** Record patient visits, issue multi-item prescriptions, manage pharmacy dispensing workflows, create visit invoices, and track payment statuses (`Paid`, `Unpaid`).  
* **Administrative Reporting:** Summary dashboard metrics covering total patients, total paid revenue, and appointment load per clinic.  
* **AI Capabilities (OpenAI-Compatible LLM Integration):**  
  * **Human-in-the-Loop Note Summarization:** Generates structured Arabic draft summaries (`الشكوى`, `الملاحظات الأساسية`, `الخططة المقترحة`) from doctor notes without inventing diagnoses, persisting to `Visits.AiSummary` only after explicit doctor approval.  
  * **Patient Guidance & Booking Chatbot:** Answers inquiries about clinics, doctors, working hours, and booking steps with strict non-diagnostic/emergency guardrails, logs conversations in `ChatMessages`, and triggers `IAppointmentService` when a patient requests a valid appointment slot.  
  * **Graceful Fallback:** Handles LLM API outages or timeouts cleanly with localized Arabic fallback messages so core operations remain unaffected.

---

## Tech Stack

* **Framework:** ASP.NET Core Web API (.NET)  
* **ORM & Database:** Entity Framework Core & Microsoft SQL Server  
* **Authentication:** JWT Bearer (`Microsoft.AspNetCore.Authentication.JwtBearer`)  
* **API Documentation:** Swagger / OpenAPI (`Swashbuckle.AspNetCore`)  
* **AI Integration:** HTTP Client (`IHttpClientFactory`) calling an OpenAI-compatible Chat Completions endpoint

---

## Project Structure

```
HealthCareCenter/
├── Controllers/
│   ├── AiController.cs                  # AI note summarization, approval & chatbot endpoints
│   ├── AppointmentsController.cs        # Appointment booking, filtering, rescheduling & status
│   ├── AuthController.cs                # JWT login endpoint
│   ├── ClinicalController.cs            # Visits, prescriptions, dispensing & invoices
│   ├── ClinicsAndDoctorsController.cs   # Clinics, doctors & working schedules
│   ├── PatientsController.cs            # Patient CRUD & unified medical record view
│   ├── ReportsController.cs             # Admin summary analytics
│   └── UsersController.cs               # Admin user account & role management
├── Data/
│   └── HealthCareCenterDBContext.cs     # EF Core DbContext & Fluent API schema configuration
├── DTOs/
│   └── BackendDtos.cs                   # Request and response record DTOs
├── Models/
│   ├── Appointment.cs
│   ├── ChatMessage.cs
│   ├── Clinic.cs
│   ├── Doctor.cs
│   ├── DoctorSchedule.cs
│   ├── Invoice.cs
│   ├── Patient.cs
│   ├── Prescription.cs
│   ├── PrescriptionItem.cs
│   ├── User.cs
│   └── Visit.cs
├── Services/
│   ├── Interfaces/                      # IAiService, IAppointmentService, IAuthService, IClinicalService, IPatientService
│   └── Implementations/                 # LlmAiService, AppointmentService, AuthService, ClinicalService, PatientService
├── appsettings.json
└── Program.cs                           # DI registration, JWT setup, Swagger & dev admin seeding
```

---

## Database Schema

`HealthCareCenterDBContext` manages 11 SQL Server tables:

* **`Users`** — User credentials (`Email` unique), `PasswordHash`, `Role`, and `IsActive`.  
* **`Patients`** — Patient demographics linked 1-to-1 with `Users`.  
* **`Clinics`** — Medical departments/clinics.  
* **`Doctors`** — Doctor profiles linked 1-to-1 with `Users` and many-to-1 with `Clinics`.  
* **`DoctorSchedules`** — Weekly working hours per doctor (`Unique(DoctorId, DayOfWeek)`).  
* **`Appointments`** — Scheduled 30-minute time slots linking `Patients` and `Doctors`.  
* **`Visits`** — Clinical visit records linked 1-to-1 with `Appointments`, storing `DoctorNotes` and doctor-approved `AiSummary`.  
* **`Prescriptions` & `PrescriptionItems`** — Electronic prescriptions and medication line items (`MedicineName`, `Dose`, `Duration`) with pharmacy dispensing tracking.  
* **`Invoices`** — Visit billing records (`Amount`, `PaymentStatus`, `PaymentDate`).  
* **`ChatMessages`** — Audit log of patient chatbot prompts and assistant replies.

---

## Getting Started

### 1\. Prerequisites

* .NET 8.0 SDK (or compatible .NET version supporting C\# 12 collection expressions)  
* SQL Server / SQL Server Express  
* Visual Studio 2022 or .NET CLI

### 2\. Configuration (`appsettings.json`)

Update `appsettings.json` (or use `dotnet user-secrets` in development) with your SQL Server connection string, JWT secret key, and LLM API credentials:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER\\SQLEXPRESS;Database=HealthCareCenterDBContext;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "YOUR_STRONG_SECRET_SIGNING_KEY_AT_LEAST_32_CHARS",
    "Issuer": "HealthCareCenterApi",
    "Audience": "HealthCareCenterClient"
  },
  "AiSettings": {
    "ProviderUrl": "https://api.openai.com/v1/chat/completions",
    "Model": "gpt-4o-mini",
    "ApiKey": "YOUR_LLM_API_KEY",
    "TimeoutSeconds": 20
  }
}
```

### 3\. Apply Database Migrations

Run EF Core migrations to create the database and tables:

```sh
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 4\. Run the Application

```sh
dotnet run
```

When running in the `Development` environment:

* Swagger UI is available at `/swagger`.  
* If the `Users` table is empty, a default administrator account is automatically seeded:  
  * **Email:** `admin@healthcomplex.local`  
  * **Password:** `Admin@12345`

---

## API Endpoints Overview

### Authentication & Users

| Method | Endpoint | Allowed Roles | Description |
| :---- | :---- | :---- | :---- |
| `POST` | `/api/Auth/login` | Anonymous | Authenticate and receive a JWT Bearer token |
| `GET` | `/api/Users` | `Admin` | List all user accounts |
| `POST` | `/api/Users` | `Admin` | Create a new user account with a role |
| `PUT` | `/api/Users/{id}` | `Admin` | Update a user's role or active status |

### Clinics, Doctors & Schedules

| Method | Endpoint | Allowed Roles | Description |
| :---- | :---- | :---- | :---- |
| `GET` | `/api/clinics` | Authenticated | List all clinics |
| `POST` | `/api/clinics` | `Admin` | Create a new clinic |
| `GET` | `/api/doctors?clinicId=` | Authenticated | List doctors and working schedules (optional clinic filter) |
| `POST` | `/api/doctors` | `Admin` | Create a doctor profile linked to a `Doctor` user |
| `PUT` | `/api/doctors/{doctorId}/schedules` | `Admin` | Add or update a doctor's working hours for a day of the week |

### Patients & Medical Records

| Method | Endpoint | Allowed Roles | Description |
| :---- | :---- | :---- | :---- |
| `GET` | `/api/Patients?search=` | `Admin`, `Receptionist`, `Doctor` | Search patients by name or phone |
| `POST` | `/api/Patients` | `Admin`, `Receptionist` | Create a patient profile linked to a `Patient` user |
| `PUT` | `/api/Patients/{id}` | `Admin`, `Receptionist` | Update patient demographic details |
| `GET` | `/api/Patients/{id}/medical-record` | `Admin`, `Doctor`, `Receptionist`, `Patient` | Retrieve full patient medical history, visits, prescriptions, and invoices |

### Appointments

| Method | Endpoint | Allowed Roles | Description |
| :---- | :---- | :---- | :---- |
| `GET` | `/api/Appointments` | Authenticated | Query appointments by `date`, `doctorId`, or `patientId` |
| `POST` | `/api/Appointments` | `Admin`, `Receptionist`, `Patient` | Book a 30-minute appointment (validates schedule & conflicts) |
| `PUT` | `/api/Appointments/{id}/reschedule` | `Admin`, `Receptionist`, `Patient` | Reschedule an active appointment with conflict validation |
| `PATCH` | `/api/Appointments/{id}/status` | `Admin`, `Receptionist`, `Doctor` | Update status (`Scheduled`, `Completed`, `Cancelled`) |

### Clinical Visits, Prescriptions & Invoices

| Method | Endpoint | Allowed Roles | Description |
| :---- | :---- | :---- | :---- |
| `POST` | `/api/Clinical/visits` | `Doctor`, `Admin` | Record a visit and mark its appointment `Completed` |
| `POST` | `/api/Clinical/prescriptions` | `Doctor` | Issue a prescription with medication items |
| `GET` | `/api/Clinical/prescriptions/pending` | `Pharmacist`, `Admin` | List all undispensed prescriptions |
| `PATCH` | `/api/Clinical/prescriptions/{id}/dispense` | `Pharmacist`, `Admin` | Mark a prescription as dispensed |
| `POST` | `/api/Clinical/invoices` | `Receptionist`, `Admin` | Generate an unpaid invoice for a visit |
| `PATCH` | `/api/Clinical/invoices/{id}/payment` | `Receptionist`, `Admin` | Update invoice payment status (`Paid` / `Unpaid`) |

### AI Services & Reports

| Method | Endpoint | Allowed Roles | Description |
| :---- | :---- | :---- | :---- |
| `POST` | `/api/Ai/summarize-note` | `Doctor` | Generate an Arabic draft summary from doctor notes |
| `PUT` | `/api/Ai/visits/{visitId}/approve-summary` | `Doctor` | Save the doctor-approved summary to `Visits.AiSummary` |
| `POST` | `/api/Ai/chat` | `Patient`, `Receptionist`, `Admin` | Chat with the AI assistant for guidance or appointment booking |
| `GET` | `/api/Reports/summary` | `Admin` | Get total patients, paid revenue, and clinic appointment load |

&nbsp;
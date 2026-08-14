# Assignment & Submission Management System

A full-stack, role-based web application for a school/college that allows **Admins** to manage users, classes, and subjects; **Teachers** to create and grade assignments; and **Students** to view and submit their work.

Built for the Assistant Software Engineer Recruitment Project — OnnoRokom Projukti Limited.

---

## 1. Project Overview

This system supports three roles with distinct capabilities:

- **Admin** — manages users, classes, subjects, teacher-subject-class assignments, and application settings.
- **Teacher** — creates assignments (draft/publish/close), reviews student submissions, and grades them with feedback.
- **Student** — views assignments for their class, submits answers (text and/or file upload), and views marks/feedback.

---

## 2. Technology Stack

| Layer | Technology |
|---|---|
| Frontend | Angular 18 (standalone components), Angular Material, TypeScript |
| Backend | ASP.NET Core 8 Web API, C#, Clean Architecture (Domain / Application / Infrastructure / API) |
| Database | PostgreSQL, Entity Framework Core (Code-First, Migrations) |
| Authentication | JWT Bearer tokens, ASP.NET Core Identity password hashing |
| Validation | FluentValidation |
| Logging | Serilog (console + rolling file logs) |
| Testing | xUnit, FluentAssertions, EF Core InMemory provider |
| API Docs | Swagger / OpenAPI |

---

## 3. Project Structure

```
AssignmentSystem/
├── AssignmentSystem.API/            # Controllers, Program.cs, appsettings, middleware
├── AssignmentSystem.Application/    # Services, DTOs, Interfaces, Validators, Exceptions
├── AssignmentSystem.Domain/         # Entities, Enums
├── AssignmentSystem.Infrastructure/ # EF Core DbContext, Migrations, Auth, File storage
├── AssignmentSystem.UnitTests/      # xUnit test suite
└── assignment-system-ui/            # Angular frontend (separate folder, own package.json)
```

---

## 4. Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/) (v13+)
- [Node.js](https://nodejs.org/) (v18+) and npm
- [Angular CLI](https://angular.dev/tools/cli): `npm install -g @angular/cli`
- Visual Studio 2022 (backend) and/or VS Code (frontend)

---

## 5. Backend Setup

### 5.1 Configure the database connection

Copy `AssignmentSystem.API/appsettings.example.json` to `appsettings.json` (or edit the existing one) and set your PostgreSQL credentials:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=AssignmentSystem;Username=postgres;Password=YOUR_PASSWORD"
  }
}
```

> A `.env.example` file is also provided listing all required environment-style configuration values. No real secrets are committed to the repository.

### 5.2 Apply migrations (creates the database and tables automatically)

Open **Package Manager Console** in Visual Studio (Default project: `AssignmentSystem.Infrastructure`) and run:

```powershell
Update-Database -Project AssignmentSystem.Infrastructure -StartupProject AssignmentSystem.API
```

This is EF Core **Code-First** — the database itself does not need to be created manually; `Update-Database` creates it if it doesn't exist and applies all migrations.

### 5.3 Run the API

Set `AssignmentSystem.API` as the startup project and press **F5** (or `dotnet run` from that folder). Swagger UI will open automatically at:

```
https://localhost:<port>/swagger
```

### 5.4 Seed data

On first run, the application automatically seeds three demo accounts (Admin, Teacher, Student) — see [Demo Credentials](#7-demo-credentials) below. The seeder is idempotent: it checks if any users exist and skips seeding on subsequent runs.

### 5.5 Run backend unit tests

```powershell
dotnet test
```

Expected: all tests pass (14 tests covering authentication, assignment ownership rules, submission deadlines/duplication, and teacher-assignment conflict rules).

---

## 6. Frontend Setup

```bash
cd assignment-system-ui
npm install
```

Update the API base URL if needed in `src/environments/environment.ts`:

```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:<your-backend-port>/api'
};
```

Run the frontend:

```bash
ng serve
```

Open the browser at:

```
http://localhost:4200
```

> **Important:** the backend must be running for the frontend to work, and CORS is configured on the backend to allow `http://localhost:4200` by default (see `Program.cs` / `appsettings.json` → `Cors:AllowedOrigins`).

---

## 7. Demo Credentials

| Role | Email | Password |
|---|---|---|
| Admin | `admin@demo.com` | `Admin@123` |
| Teacher | `teacher@demo.com` | `Teacher@123` |
| Student | `student@demo.com` | `Student@123` |

These are created automatically by the database seeder on first run.

### Creating additional users

This system does **not** support self-registration by design, consistent with the requirement that the Admin manages all users. To create a new Teacher or Student:

1. Log in as Admin.
2. Go to **Admin → Users → New User**.
3. Fill in the name, email, password, and role (Student additionally requires a Class).
4. Share the credentials with that person out-of-band (email, in person, etc.) — exactly as would happen in a real school office.

To let a new Teacher create assignments, an Admin must also assign them to a Class + Subject via **Admin → Teacher Assignments**.

---

## 8. Core Features

### Admin
- Manage Users (create, update, activate/deactivate, change password, filter/search, pagination)
- Manage Classes (create, update, soft-deactivate)
- Manage Subjects (create, update, soft-deactivate)
- Link/unlink Subjects to Classes
- Assign Teachers to a Class + Subject (with conflict prevention — only one active teacher per Class+Subject pair)
- Manage Application Settings (key-value store)

### Teacher
- Create assignments only for classes/subjects they are assigned to teach
- Save as Draft, Publish, or Close an assignment
- Edit assignment details before/after publishing
- View and grade student submissions (marks + feedback)
- Delete an assignment only if it has no submissions yet (otherwise must be closed instead)

### Student
- View only Published assignments for their own class
- Submit an answer as text, a file, or both
- Update a submission before the deadline (if the assignment allows it and it hasn't been graded yet)
- View marks and feedback once graded

---

## 9. Key Business Rules

- A Teacher can only create/manage assignments for a Class + Subject they are actively assigned to (`TeacherAssignments` table).
- A Student can only see/submit assignments belonging to their own class, and only once `Published`.
- A Student can submit **once** per assignment; further attempts update the existing submission (subject to the rules below).
- Submission updates are blocked once: the assignment's deadline has passed **and** `AllowUpdateAfterSubmit` is false, or the submission has already been graded.
- Submissions made after the deadline (when allowed) are marked `Late` rather than rejected outright.
- Only one **active** Teacher can be assigned to a given Class + Subject pair at a time; reactivating a deactivated assignment is blocked if it would create a duplicate.
- Deactivated user accounts cannot log in, even with correct credentials.
- Deleting a Class/Subject/User uses soft-delete (`IsActive` flag) to preserve historical Assignment/Submission records; deleting an Assignment is blocked once it has submissions (it must be Closed instead).

---

## 10. Assumptions

- A student belongs to exactly one class at a time (`User.ClassId`), not multiple.
- File uploads are stored on local disk (`FileStorage:RootPath` in `appsettings.json`), not cloud storage — acceptable for this assignment's scope.
- Password reset/forgot-password flows are out of scope; Admin can reset a user's password via **Change Password**.
- Email notifications (e.g., on grading) are out of scope.
- Self-registration is intentionally disabled — Admin creates all accounts, matching the "Admin manages users" requirement.

## 11. Known Limitations

- No pagination on the Class/Subject/TeacherAssignment lists (acceptable given the expected small dataset size for a single school).
- File upload size limit is enforced at 20MB per submission; no virus scanning is performed.
- No refresh-token flow — JWT expires after 2 hours and the user must log in again.

---

## 12. API Documentation

Once the backend is running, full interactive API documentation (all endpoints, request/response schemas, and the ability to test authenticated requests) is available via Swagger:

```
https://localhost:<port>/swagger
```

To test protected endpoints in Swagger: call `POST /api/auth/login`, copy the returned token, click **Authorize**, and enter `Bearer <token>`.

---

## 13. Logs

Application logs are written to both the console and a rolling daily file at:

```
AssignmentSystem.API/logs/log-YYYYMMDD.txt
```

This includes every HTTP request (method, path, status code, duration) plus key business events such as login attempts (success/failure) and account deactivation blocks.

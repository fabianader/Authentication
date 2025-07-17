# Authentication
ASP.NET Core MVC Identity Management System

# Overview
IdentityProject is an ASP.NET Core MVC web application focused on implementing a customizable, secure, and scalable user and role management system using ASP.NET Core Identity.
This system enables user registration, login, password reset via email confirmation codes, role assignment, and complete role-based access control.

# Features
#### ✅ ASP.NET Core Identity Integration
  - Custom ApplicationUser and ApplicationRole models
  - Supports tokens, claims, logins, and role relationships

#### ✅ User Authentication Flow
  - Register, Login, Logout
  - Password reset via email code
  - Email verification support

#### ✅ Role Management
  - Add/Edit/Delete roles
  - Assign/unassign users to roles
  - Role-based UI and navigation

#### ✅ User Management
  - Edit users, search
  - View assigned roles
  - Reset password and send verification code via email

#### ✅ Custom Tools
  - Persian validation error provider
  - Email validation attribute
  - Email sender service abstraction

#### ✅ ViewModels and DTOs
  - Ensures separation of concerns and improves security
  - Prevents over-posting from views

#### ✅ Razor Views
  - Strongly-typed views for login, register, reset, manage roles/users
  - Shared layouts and partials
  - Bootstrap UI (with jQuery validation)

# 🧱 Tech Stack
|Technology                |	Purpose                        |
| ------------------------ |:-------------------------------:|
|ASP.NET Core MVC          |  Web framework                  |
|ASP.NET Core Identity     |	Auth, user & role management   |
|Entity Framework Core     |	ORM & database access          |
|SQL Server                |  Database                       |
|Bootstrap 5               |	UI framework                   |
|jQuery                    |	DOM manipulation & validation  |
|Razor View Engine         |	Dynamic server-side rendering  |

# 📁 Project Structure
├── Controllers/
│   ├── Auth.cs              # Login/Register/ForgotPassword
│   ├── ManageUserController.cs
│   ├── ManageRoleController.cs
│   └── HomeController.cs
├── Models/
│   ├── AppCustomEntities/   # Identity entities (User, Role, Claim, etc.)
│   ├── ManageUserVM/        # User-related ViewModels
│   └── ViewModels/          # Auth ViewModels
├── DTOs/                    # For secure data transfer
├── Views/
│   ├── Auth/                # Login, Register, Forgot/Reset Password
│   ├── ManageUser/          # User list, edit, search
│   ├── ManageRole/          # Role list, add, edit
│   └── Shared/              # Layout, partials
├── Context/DbContext.cs     # EF Core DB Context
├── Migrations/              # EF migrations
├── Services/EmailSender.cs  # Email service
├── Tools/                   # Custom validation/tools
├── wwwroot/                 # Static files (CSS, JS, lib)
├── Program.cs               # App entry point
└── appsettings.json         # Configurations


# Getting Started
#### Prerequisites
  - .NET 8 SDK
  - SQL Server or LocalDB
  - Visual Studio 2022+

#### Setup
1. Clone the repository:
  - `git clone https://github.com/your-username/IdentityProject.git`
  - `cd IdentityProject`

2. Update the database:
  - `dotnet ef database update`

3. Run the app:
  - `dotnet run`
Or use `F5` in Visual Studio.

# 📧 Email Functionality
The project supports sending email tokens for actions like password reset and confirmation. You'll need to configure an SMTP server in appsettings.json:

# FarmShield – Digital Biosecurity Management System

> **Development of a Digital Farm Management Portal for Implementing Biosecurity Measures in Pig and Poultry Farms**

![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-MVC-512BD4?style=flat&logo=dotnet)
![.NET](https://img.shields.io/badge/.NET-10%20LTS-512BD4?style=flat&logo=dotnet)
![EF Core](https://img.shields.io/badge/Entity%20Framework-Core-512BD4?style=flat)
![SQLite](https://img.shields.io/badge/Database-SQLite%20%2F%20SQL%20Server-003B57?style=flat&logo=sqlite)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-7952B3?style=flat&logo=bootstrap)
![License](https://img.shields.io/badge/License-Academic-green?style=flat)

---

## 📋 Overview

**FarmShield** is an enterprise-grade digital farm management and biosecurity platform built specifically for modern pig and poultry operations. It automates compliance audits, gate controls, and real-time disease surveillance.

This is a **college academic capstone project** designed to look and function like a real-world agricultural technology SaaS platform.

---

## 🚀 Tech Stack

| Layer | Technology |
|-------|-----------|
| Backend | ASP.NET Core MVC (.NET 10 LTS) |
| Language | C# |
| ORM | Entity Framework Core (Code First) |
| Database | SQLite (default) / SQL Server |
| Authentication | ASP.NET Core Identity |
| Frontend | HTML5, Bootstrap 5, JavaScript |
| Charts | Chart.js 4 |
| Icons | Font Awesome 6 |

---

## ✨ Features

- 🏠 **Landing Page** – Professional public portal with live statistics
- 🔐 **Authentication** – Register, Login, Role-based access (4 roles)
- 📊 **Admin Dashboard** – Interactive Chart.js visualizations, real-time data
- 🐷 **Pig Management** – Individual animal tracking, health, vaccination
- 🐔 **Poultry Management** – Batch management, egg production, mortality
- 🛡️ **Biosecurity Engine** – 6-pillar framework (Visitors, Cleaning, Feed/Water, Pest Control, Waste, Health)
- 📋 **Digital Inspections** – Interactive checklists, auto compliance scoring
- 🦠 **Disease Surveillance** – Report, track, alert on suspected outbreaks
- 💉 **Vaccination Tracking** – Schedule, reminders, history
- 🔔 **Notification Center** – Real-time alerts for inspections, diseases, vaccinations
- 📈 **Reports & Analytics** – Filtered reports, PDF export

---

## 👥 User Roles

| Role | Access |
|------|--------|
| **Super Admin** | Full system access, all farms |
| **Farm Owner** | Own farm data, reports |
| **Farm Manager** | Day-to-day operations, biosecurity logs |
| **Veterinary Officer** | Health records, disease reports, vaccinations |

---

## ⚡ Quick Start

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Git

### Run Locally

```bash
git clone https://github.com/Vinayk289/FarmShield.git
cd FarmShield
dotnet run
```

The app will:
1. Build automatically
2. Create the SQLite database (`FarmShield.db`)
3. Seed all demo data
4. Start at **http://localhost:5215**

---

## 🔑 Demo Accounts

| Role | Email | Password |
|------|-------|----------|
| Super Admin | `admin@farmshield.com` | `FarmShield@2025` |
| Farm Owner | `owner@greenvalleyfarms.com` | `FarmShield@2025` |
| Farm Manager | `manager@highlandpigs.com` | `FarmShield@2025` |
| Vet Officer | `vet@biosecurevet.com` | `FarmShield@2025` |

---

## 🗄️ Database

By default the app uses **SQLite** (zero-config, file-based). To switch to **SQL Server**, update `appsettings.json`:

```json
{
  "DatabaseProvider": "SqlServer",
  "ConnectionStrings": {
    "SqlServerConnection": "Server=YOUR_SERVER;Database=FarmShield;Trusted_Connection=True;"
  }
}
```

---

## 📁 Project Structure

```
FarmShield/
├── Controllers/          # MVC Controllers
├── Data/                 # DbContext + Seeder
├── Models/
│   ├── Entities/         # EF Core entities
│   ├── Enums/            # Domain enums
│   └── ViewModels/       # View models
├── Services/             # Business logic layer
├── Views/                # Razor views
│   ├── Shared/           # Layouts (_Layout, _LandingLayout)
│   ├── Dashboard/
│   ├── Farms/
│   ├── Pigs/
│   ├── Poultry/
│   ├── Biosecurity/
│   ├── Inspections/
│   ├── Diseases/
│   ├── Vaccinations/
│   └── Reports/
└── wwwroot/              # Static assets (CSS, JS)
```

---

## 🎓 Academic Information

**Project Title:** Development of a Digital Farm Management Portal for Implementing Biosecurity Measures in Pig and Poultry Farms  
**Platform:** FarmShield – Digital Biosecurity Management System  
**Framework:** .NET 10 LTS Enterprise Web Architecture  
**Compliance:** Designed to WOAH (World Organisation for Animal Health) & FAO biosecurity standards

---

## 📄 License

This project is submitted as an academic capstone project. All rights reserved © 2025 FarmShield.

# Mini Cloud Note 📝

![Build Status](https://img.shields.io/badge/Build-Passing-success)
![.NET](https://img.shields.io/badge/Backend-.NET%209.0-purple)
![Flutter](https://img.shields.io/badge/Frontend-Flutter%203.x-02569B)
![Architecture](https://img.shields.io/badge/Architecture-Clean%20%26%20BLoC-green)
![Docker](https://img.shields.io/badge/Deployment-Docker-2496ED)

**Mini Cloud Note** is a comprehensive, cross-platform personal knowledge management system. It features a robust **.NET Backend** following Clean Architecture and a responsive **Flutter Frontend** (Mobile & Web) using the BLoC pattern.

This repository is organized as a **Monorepo**, representing a continuous journey of mastering full-stack software engineering, from low-level backend optimization to high-level UI/UX state management.

## 🚀 Project Overview

The goal is to build a production-ready system that seamlessly syncs notes across devices while maintaining strict security and high performance.

🌟 **Key Features**

🛡️ **Backend (.NET Core)**
- **🔐 Advanced Authentication**: Secure Identity system using JWT (Access Token & Refresh Token strategies).
- **📝 Note Management**: Full CRUD operations for notes with Markdown support.
- **☁️ Object Storage**: Efficient file handling using MinIO (S3 compatible) for attachments.
- **⚡ High Performance**: Caching strategy implemented with **Redis**.
- **⏱️ Background Jobs**: Asynchronous task processing (email sending, file cleanup) using **Hangfire**.
- **🔍 Observability**: Centralized logging and monitoring via **Serilog** and **Seq**.
- **🐳 Containerization**: Full Docker support for "One-click" deployment.

📱**Frontend (Flutter)**
- **Cross-Platform**: Single codebase running on **Android, iOS, and Web.**
- **State Management**: Predictable state management using **BLoC (Business Logic Component).**
- **Networking**: Robust API handling with **Dio** and Interceptors.
- **Responsive UI**: Adaptive design for both mobile screens and web dashboards.

## 🏗 System Architecture Diagram

The system operates on a client-server model within a containerized environment.

```mermaid
graph TD
    %% Define Styles
    classDef mobile fill:#e3f2fd,stroke:#1565c0,stroke-width:2px;
    classDef web fill:#e3f2fd,stroke:#0277bd,stroke-width:2px;
    classDef proxy fill:#fff9c4,stroke:#fbc02d,stroke-width:2px;
    classDef api fill:#e8f5e9,stroke:#2e7d32,stroke-width:2px;
    classDef db fill:#f3e5f5,stroke:#7b1fa2,stroke-width:2px;

    %% Clients
    subgraph "Frontend Layer (Flutter)"
        Mobile[Mobile App]:::mobile
        Web[Web Admin]:::web
    end

    %% Backend
    subgraph "Backend Infrastructure (Docker)"
        Mobile & Web -->|"REST API (HTTPS)"| NGINX["NGINX Reverse Proxy"]:::proxy
        NGINX -->|"Forward"| API["API Core (.NET 9)"]:::api
        
        API -->|"Persist"| PG[("PostgreSQL")]:::db
        API -->|"Cache"| Redis[("Redis")]:::db
        API -->|"Files"| MinIO[("MinIO S3")]:::db
        API -->|"Logs"| Seq[("Seq Logging")]:::db
    end
```
## 🛠 Tech Stack

This project utilizes a modern, industry-standard technology stack:

- **Backend:** ASP.NET Core 9, Entity Framework Core, Hangfire
- **Frontend:** Flutter (Dart), BLoC, Dio, Equatable
- **Database:** PostgreSQL (Relational), Redis (Cache)
- **Storage:** MinIO (S3 Compatible Object Storage)
- **DevOps:** Docker, Docker Compose, Jenkins, NGINX
- **Tools:** Visual Studio 2022, VS Code, Postman, Android Studio

## 📂 Project Structure

```text
MiniCloudNote/
├── backend/                  # .NET Core API Solution
│   ├── src/
│   │   ├── MiniCloudNote.API/            # Entry Point
│   │   ├── MiniCloudNote.Core/           # Domain & Logic
│   │   └── MiniCloudNote.Infrastructure/ # DB & External Services
│   ├── Dockerfile
│   └── MiniCloudNote.sln
│
├── frontend/                 # Flutter Application
│   ├── android/              # Android native code
│   ├── ios/                  # iOS native code
│   ├── lib/                  # Dart source code (Screens, BLoCs)
│   └── pubspec.yaml
│
├── docker-compose.prod.yml   # Production Orchestration
├── nginx/                    # Reverse Proxy Config
└── .github/                  # CI/CD Workflows
```

## 🧹 Clean Architecture
```mermaid
    graph TD
    API[MiniCloudNote.API] --> Core[MiniCloudNote.Core]
    Infrastructure[MiniCloudNote.Infrastructure] --> Core
    API --> Infrastructure
    
    %% Core KHÔNG ĐƯỢC chỉ ngược lại ai cả
```

## ⚡ Getting Started

Follow these steps to get **MiniCloudNote** running on your local machine.

### Prerequisites

Ensure you have the following installed:
- **[Docker Desktop](https://www.docker.com/products/docker-desktop)** (Required for Backend).
- **[Flutter SDK](https://docs.flutter.dev/install)** (Required for Frontend).

1. **Setup Backend (Docker)**

   ```bash
    # 1. Navigate to root
    cd MiniCloudNote

    # 2. Start Backend Infrastructure
    docker-compose -f docker-compose.prod.yml up -d --build
    ```
    Wait a few minutes for the containers to initialize.

2. **Setup Frontend (Flutter)** 
Open a new terminal to run the mobile/web app.
    
    ```bash
    # 1. Navigate to frontend folder
    cd frontend

    # 2. Get Dependencies
    flutter pub get

    # 3. Run the App
    flutter run
    ```

3. **Access the Application:**
    * **API Swagger:** `http://localhost:5265/swagger`
    * **MinIO Console:** `http://localhost:9001`
    * **Seq Logs:** `http://localhost:5341`
    * **Hangfire Dashboard:** `http://localhost:5265/hangfire`

    ---
    **Author:** PHAM HONG THAI
    ---






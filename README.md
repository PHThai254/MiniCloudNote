# Mini Cloud Note 📝

**Mini Cloud Note** is a comprehensive backend system for a personal note-taking application that supports file attachments (images, documents). This project is built to demonstrate strict adherence to software engineering principles, including SOLID, Clean Architecture, and a full DevOps pipeline.

This repository tracks my journey of mastering modern technologies over a **42-day coding challenge**.

## 🚀 Project Overview

The goal is to build a scalable, secure, and deployable backend system where users can:
- Securely register and login (JWT Authentication).
- Manage personal notes (Create, Read, Update, Delete).
- Upload and manage attachments using Object Storage (MinIO).

## MiniCloudNote System Architecture Diagram
```mermaid
graph TD
    %% Định nghĩa Style (Màu sắc)
    classDef client fill:#e1f5fe,stroke:#01579b,stroke-width:2px;
    classDef proxy fill:#fff9c4,stroke:#fbc02d,stroke-width:2px;
    classDef app fill:#e8f5e9,stroke:#2e7d32,stroke-width:2px;
    classDef db fill:#f3e5f5,stroke:#7b1fa2,stroke-width:2px;

    %% User và Client
    User((User)) -->|"1. Tương tác"| Browser["Browser / Client App"]:::client

    %% Entry Point
    Browser -->|"2. HTTP Request (RESTful API)"| NGINX["NGINX Reverse Proxy"]:::proxy

    %% Backend Application (Clean Architecture)
    subgraph "Docker Container: Backend API"
        NGINX -->|"3. Forward Request"| API_Layer["API Layer (Controllers)"]:::app
        
        subgraph "MiniCloudNote Application (ASP.NET Core)"
            API_Layer -->|"4. Gọi Service"| Core_Layer["Core Layer (Business Logic/Entities)"]:::app
            Core_Layer -->|"5. Gọi Interface"| Infra_Layer["Infrastructure Layer"]:::app
        end
    end

    %% External Infrastructure
    Infra_Layer -->|"6. SQL Query (EF Core)"| PG[("PostgreSQL Database")]:::db
    Infra_Layer -->|"7. S3 API (MinIO SDK)"| MinIO[("MinIO Object Storage")]:::db

    %% Style cho đường nối
    linkStyle 3,4,5 stroke:#2e7d32,stroke-width:2px;
```

## 🛠 Tech Stack

This project utilizes a modern, industry-standard technology stack:

- **Core Framework:** ASP.NET Core (.NET 9)
- **Database:** PostgreSQL
- **Object Storage:** MinIO (S3 Compatible) for file storage
- **Architecture:** Clean Architecture & Layered Architecture
- **Containerization:** Docker & Docker Compose
- **CI/CD:** Jenkins & GitHub Webhooks
- **Gateway/Proxy:** NGINX
- **Tunneling:** Ngrok (for public exposure)

## 📂 Project Structure

```text
MiniCloudNote/
├── src/
│   ├── MiniCloudNote.API/    # Main Entry Point (Web API)
│   ├── MiniCloudNote.Core/   # Domain Entities & Interfaces (Planned)
│   ├── MiniCloudNote.Infrastructure/  # Database & External Services (Planned)
└── docker-compose.yml        # Container Orchestration
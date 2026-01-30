# Sprintpad

Sprintpad is a Kanban board application in progress. This repository is the starting point for building a full-stack app and is currently in the early backend scaffolding phase.

## Current State

- ASP.NET Core minimal API project (`KanbanApi`) with the default `/weatherforecast` endpoint.
- Domain models for a Kanban app are present (`Board`, `Column`, `Card`, `BoardMember`, `BoardRole`, `ApplicationUser`).
- No persistence, authentication, authorization, or real endpoints wired up yet.

## Expected State

Sprintpad will become a complete Kanban board product with:

- .NET 8 minimal APIs for the backend
- Entity Framework Core 8 for data access
- ASP.NET Core Identity for authentication
- Authorization policies for roles and permissions
- CI/CD with GitHub Actions and deployment to Azure

## Product Description

Sprintpad is a hands-on build of a real Kanban board application from scratch. The goal is a portfolio-ready, full-stack product with authentication, authorization, and cloud deployment.

The plan mirrors a professional .NET stack: .NET 8 minimal APIs on the backend, Entity Framework Core 8 for data access, ASP.NET Core Identity for authentication, and Azure for hosting. Development includes C# 12, dependency injection, and authorization policies, with deployment automated through GitHub Actions.

Each sprint adds a real feature and reinforces practical skills. The outcome is a working application and a repeatable path to ship the next one faster.

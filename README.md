# MikrocopTask

## Used
- .NET 9.0
- EntityFramework
- Swagger
- Moq
- xUnit

## Prerequisites:
- DB (I used MSSQL - latest)


### How to install MSSQL via docker for a quick start
1. docker pull mcr.microsoft.com/mssql/server
2. docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=someSaPass12" -p 1433:1433 --name mssql-dev -d mcr.microsoft.com/mssql/server:latest

### DB Migration
1. CD to src/
2. Run: dotnet ef database update --project UserManagement.Persistence --startup-project UserManagement.API

The SQL server and DB should be ready to go now.

### How-to start
1. Open the project with IDE of choice and run the solution


## Testing

### Unit Tests:
1. CD to tests/UserManagement.API.Test
2. Run: dotnet test


### Integration Tests: (Won't run due to issue)
1. CD to tests/UserManagement.API.Test
2. Run: dotnet test

## Things I wish I had time for:
 - FluidValidation
 - Clean-er Architecture
 - Identity Framework
 - Code lists for Language and Culture
 - Fixed Integration tests


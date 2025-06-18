# PABC-API

## Documentatie
[De documentatie staat op readthedocs](https://pabc-api.readthedocs.io/)

## Ontwikkelen
- Zorg dat PABC.AppHost het startup project is
- De api key tijdens ontwikkelen is `unsafe-test-api-key`

## Database migration aanmaken
.NET CLI:
```bash
dotnet ef migrations add MyMigration --project PABC.Data --startup-project PABC.Server
```
Visual Studio Package Manager Console:
```powershell
Add-Migration MyMigration -Project PABC.Data -StartupProject PABC.Server
```

## API Specificatie


| API Specificatie |
|------------------|
| [Swagger](https://petstore.swagger.io/?url=https://raw.githubusercontent.com/PodiumD-Autorisatie-Beheer-Component/PABC-API/refs/heads/main/PABC.Server/PABC.Server.json) |

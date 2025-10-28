# PABC-API

Het Platform Autorisatie Beheer Component (PABC) is een component om binnen het Platform landschap autorisaties voor de verschillende componenten te beheren. 

## Documentatie
[De documentatie staat op readthedocs](https://pabc-api.readthedocs.io/)

## Ontwikkelen
- Zorg dat PABC.AppHost het startup project is
- De api key tijdens ontwikkelen is `unsafe-test-api-key`
- De database wordt bij opstarten  automatisch geleegd en opnieuw gevuld met de data in [`test-dataset.json`](./test-dataset.json)
- Om in de UI in te kunnen loggen, moet je in de user secrets van het PABC.Server project de benodigde configuratie invullen:
```json
{
  "Oidc": {
    "Authority": "",
    "ClientId": "",
    "ClientSecret": "",
    "FunctioneelBeheerderRole": ""
  }
}
```

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


| Applicatieversie | API-versie | API Specificatie |
|------------------|------------------|------------------|
| main | 1 | [Swagger](https://petstore.swagger.io/?url=https://raw.githubusercontent.com/Platform-Autorisatie-Beheer-Component/PABC-API/refs/heads/main/PABC.Server/PABC.Server.json), [ReDoc](https://redocly.github.io/redoc/?url=https://raw.githubusercontent.com/Platform-Autorisatie-Beheer-Component/PABC-API/refs/heads/main/PABC.Server/PABC.Server.json) |

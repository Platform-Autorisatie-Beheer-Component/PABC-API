# Changelog

## latest
- Refactor query implementation for GetApplicationRolesPerEntityType to be more readable
- [Zaaktypes uit Open Zaak importeren #148](https://github.com/Platform-Autorisatie-Beheer-Component/PABC-API/issues/148)
- [Functionele rollen importeren vanuit Keycloak #147](https://github.com/Platform-Autorisatie-Beheer-Component/PABC-API/issues/147)
- [Tekstfilter voor domeinen en functionele rollen #150](https://github.com/Platform-Autorisatie-Beheer-Component/PABC-API/issues/150)
- [Pre-fill applicaties en applicatierollen bij opstarten #149](https://github.com/Platform-Autorisatie-Beheer-Component/PABC-API/issues/149)

## v1.1.1

### Helm chart improvements
- fix: wait-for-migrations gebruikt job-wr i.p.v. job

## v1.1.0

### Helm chart improvements
- Support configurable initContainer image for wait-for containers in both API deployment and migrations job (enables ACR compatibility for restricted registry environments)
- Add nodeSelector support to migrations job for custom node scheduling

## v1.0.0
- [Allow mapping a functional role to all entity types #42 (wildcard)](https://github.com/Platform-Autorisatie-Beheer-Component/PABC-API/issues/42)
- [Allow Mappings without Domains #45](https://github.com/Platform-Autorisatie-Beheer-Component/PABC-API/issues/45)
- [UI – Domains #12](https://github.com/Platform-Autorisatie-Beheer-Component/PABC-API/issues/12)
- [UI – Functional roles #7](https://github.com/Platform-Autorisatie-Beheer-Component/PABC-API/issues/7)
- [UI – Entity types #13](https://github.com/Platform-Autorisatie-Beheer-Component/PABC-API/issues/13)
- [Authentication via OIDC for the user interface #10](https://github.com/Platform-Autorisatie-Beheer-Component/PABC-API/issues/10)
- [UI - Application roles #14](https://github.com/Platform-Autorisatie-Beheer-Component/PABC-API/issues/14)
- [UI – Link entity types and domains #15](https://github.com/Platform-Autorisatie-Beheer-Component/PABC-API/issues/15)
- [UI - Link functional roles to application roles within domains #3](https://github.com/Platform-Autorisatie-Beheer-Component/PABC-API/issues/3)
- [Create, modify, and delete applications separately #78](https://github.com/Platform-Autorisatie-Beheer-Component/PABC-API/issues/78)
- [UI - configure the two types of 'special' roles #89](https://github.com/Platform-Autorisatie-Beheer-Component/PABC-API/issues/89)
- [PABC lokaal kunnen gebruiken met een OIDC/OAuth provider over HTTP (i.p.v. HTTPS) #116](https://github.com/Platform-Autorisatie-Beheer-Component/PABC-API/issues/116)
- [Use snake_case naming convention in postgres table and column names](https://github.com/Platform-Autorisatie-Beheer-Component/PABC-API/issues/43)

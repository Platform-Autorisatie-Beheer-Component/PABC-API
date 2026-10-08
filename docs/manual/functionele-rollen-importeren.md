# Functionele rollen importeren

## Vereisten

De PABC moet gekoppeld zijn met Keycloak. Dit is standaard het geval wanneer de PABC correct is geïnstalleerd; er is geen extra configuratie nodig.

## Hoe werkt het?

Op de Beheer pagina verschijnt er een **Importeren** knop bij de Functionele rollen sectie.

1. Klik op de **Importeren** knop.
2. Er opent een dialoog met een toelichting. Klik op **Importeren** om de import te starten.
3. De PABC haalt alle realm roles op uit het geconfigureerde Keycloak realm.
4. Na afloop toont de dialoog het resultaat:
   - **Aangemaakt**: functionele rollen die nieuw zijn aangemaakt in de PABC.
   - **Overgeslagen**: functionele rollen die al bestonden in de PABC.
   - **Niet meer in Keycloak**: functionele rollen die wel in de PABC staan maar niet meer in Keycloak voorkomen. Deze worden *niet* automatisch verwijderd.

## Wat wordt er aangemaakt?

Voor elke realm role in Keycloak die nog niet bestaat in de PABC wordt een nieuwe functionele rol aangemaakt. De naam van de functionele rol komt overeen met de naam van de Keycloak realm role.

De match gebeurt op de exacte rolnaam (inclusief eventuele voor- en naloop spaties).

### Rollen uitsluiten van import

Keycloak maakt voor elk realm ook technische rollen aan die geen functionele rol vertegenwoordigen, bijvoorbeeld `default-roles-<realm>`, `offline_access` en `uma_authorization`. Dit soort rollen wil je meestal niet als functionele rol in de PABC hebben.

Met de omgevingsvariabele `KeycloakAdmin__ExcludedRoles__N` (in de helm chart: `settings.keycloakAdmin.excludedRoles`) kan een lijst van rolnamen geconfigureerd worden die nooit geïmporteerd worden. Rollen op deze lijst worden bij de import volledig genegeerd: ze verschijnen niet bij **Aangemaakt**, **Overgeslagen** of **Niet meer in Keycloak**. De match gebeurt op de exacte rolnaam (hoofdlettergevoelig), spaties in de rolnaam worden ondersteund. Zie [Omgevingsvariabelen](../installation/configuratie.md) voor meer details.

#### Configuratie voor PodiumD (integratieteam)

Voor een PodiumD-installatie moet het integratieteam in ieder geval de volgende rollen uitsluiten:

- `default-roles-<REALM_NAME>`, waarbij `<REALM_NAME>` de naam is van het PodiumD Keycloak realm (meestal `podiumd`, maar niet altijd). Deze naam wordt gezet door de PodiumD Helm Chart en moet doorgegeven worden aan de PABC Helm Chart.
- `offline_access`
- `uma_authorization`

In de values van de PABC Helm Chart:

```yaml
settings:
  keycloakAdmin:
    excludedRoles:
      - "default-roles-podiumd" # vervang 'podiumd' door de daadwerkelijke realm naam
      - "offline_access"
      - "uma_authorization"
```

Ter controle: start na installatie een import van functionele rollen. `default-roles-<REALM_NAME>`, `offline_access` en `uma_authorization` mogen dan niet verschijnen bij **Aangemaakt**, **Overgeslagen** of **Niet meer in Keycloak**.

## Belangrijke opmerkingen

- Functionele rollen die al bestaan in de PABC worden niet gewijzigd of bijgewerkt.
- Functionele rollen die niet meer in Keycloak voorkomen worden niet verwijderd uit de PABC.
- De import kan meerdere keren uitgevoerd worden; bestaande rollen worden overgeslagen.
- Als een rol pas ná een eerdere import aan de uitsluitlijst wordt toegevoegd, verschijnt deze bij een volgende import als **Niet meer in Keycloak** (omdat hij niet meer wordt meegenomen uit Keycloak). De PABC verwijdert deze niet automatisch; dit moet handmatig gebeuren.

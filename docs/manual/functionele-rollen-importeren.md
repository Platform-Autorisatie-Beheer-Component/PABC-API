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

## Belangrijke opmerkingen

- Functionele rollen die al bestaan in de PABC worden niet gewijzigd of bijgewerkt.
- Functionele rollen die niet meer in Keycloak voorkomen worden niet verwijderd uit de PABC.
- De import kan meerdere keren uitgevoerd worden; bestaande rollen worden overgeslagen.

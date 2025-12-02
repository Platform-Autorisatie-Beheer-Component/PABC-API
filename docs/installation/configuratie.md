

# Omgevingsvariabelen

Als je PABC installeert met de helm chart, worden deze variabelen geconfigureerd met de chart values. Zie [de helm chart in github](https://github.com/Platform-Autorisatie-Beheer-Component/PABC-API/tree/main/charts/pabc) voor meer informatie.
 
---------------------------------
| **Variabele**                      | **Waarde**                                                     |
|------------------------------------|----------------------------------------------------------------|
| ConnectionStrings__PabcConnection | Connectiestring voor de Postgres database. <details> <summary>Meer informatie </summary> Volgens het format `Host=my-hostname; Database=database-name; Username=my-username; Password=my-secret-password`. In de helm chart worden deze waardes individueel opgegeven onder `settings.database`. </details> |
| API_KEY__N            | Lijst van uitgegeven API keys. <details> <summary>Meer informatie </summary>Er kunnen meerdere API keys opgenomen worden in de configuratie: API_KEY__0, API_KEY__1, etc. De API keys kunnen elke willekeurige waarde hebben. Calls naar de api moeten voorzien zijn van een 'X-API-KEY' header met als waarde een van deze API keys. In de helm chart worden deze waardes opgegeven onder `settings.apiKeys` </details>         |
| JSON_DATASET_PATH            | Optioneel kunt u een pad opgeven naar een JSON-bestand met een dataset om de database te initializeren. **LET OP**: hierbij wordt alle bestaande data verwijderd. <details>   <summary>Meer informatie</summary>Het bestand moet geldig zijn volgens [het JSON-schema](https://raw.githubusercontent.com/Platform-Autorisatie-Beheer-Component/PABC-API/refs/heads/main/PABC.MigrationService/dataset.schema.json)</details>         |
| Oidc__Authority                                      | URL van de OpenID Connect Identity Provider <details> <summary>Meer informatie </summary>Bijvoorbeeld: `https://login.microsoftonline.com/ce1a3f2d-2265-4517-a8b4-3e4f381461ab/v2.0` </details>         |
| Oidc__ClientId                                       | Voor toegang tot de OpenID Connect Identity Provider <details> <summary>Meer informatie </summary>Bijvoorbeeld: `54f66f54-71e5-45f1-8634-9158c41f602a` </details>  |
| Oidc__ClientSecret                                   | Secret voor de OpenID Connect Identity Provider <details> <summary>Meer informatie </summary>Bijvoorbeeld: `VM2B!ccnebNe.M*gxH63*NXc8iTiAGhp` </details>    |  |
| Oidc__RequireHttps                                   | Optionele setting om een OpenID Connect Identity Provider die op http draait toe te staan  <details> <summary>Meer informatie </summary>Voeg deze variabele toe met de waarde `false` als de gebruikte identity provider communiceert via een http verbinding. Dit kan bijvoorbeeld handig zijn in een lokale ontwikkelomgeving. Als de identity provider een https verbinding heeft dan kan je deze variabele helemaal weglaten of de waarde `true` geven </details>    |  |
| Oidc__LogoutFromIdentityProvider                                   | Optionele instelling om de gebruiker automatisch af te melden bij de Identity Provider wanneer hij/zij zich afmeldt bij PABC.

<details> <summary>Meer informatie</summary> Wanneer deze instelling `true` is, worden gebruikers afgemeld bij de Identity Provider wanneer zij zich afmelden bij PABC. Dit is handig als je wilt zorgen dat gebruikers opnieuw moeten inloggen wanneer ze na het afmelden terugkeren naar PABC. Wanneer deze instelling `false` is, worden gebruikers alleen afgemeld bij PABC en kunnen ze bij de Identity Provider ingelogd blijven. </details>    |  |
| Oidc__FunctioneelBeheerderRole                       | De waarde van de role claim in het JWT token van de OpenID Connect Provider voor toegang de gebruikersinterface foor de beheerfuncties<details> <summary>Meer informatie </summary>Bijvoorbeeld: `PABC-Functioneel-Beheerder` </details>     |
| Oidc__NameClaimType                                  | De naam van de claim in het JWT token van de OpenID Connect Provider waarin de volledige naam van de ingelogde gebruiker staat <br/> (default waarde is `name`) |
| Oidc__RoleClaimType                                  | De naam van de claim in het JWT token van de OpenID Connect Provider waarin de rollen van de ingelogde gebruiker staan. <br/> (default waarde is `roles`)  |
| Oidc__EmailClaimType                                 | De naam van de claim in het JWT token van de OpenID Connect Provider waarin het e-mailadres van de ingelogde gebruiker staat. <br/> (default waarde is `email`)   |
| KeycloakAdmin__ClientId  |  Voor toegang tot de Admin REST API van Keycloak <details> <summary>Meer informatie </summary>Bijvoorbeeld: `pabc-admin-client` </details>    |  |
| KeycloakAdmin__ClientSecret                                   | Secret voor de Admin REST API van Keycloak <details> <summary>Meer informatie </summary>Bijvoorbeeld: `VM2B!ccnebNe.M*gxH63*NXc8iTiAGhp` </details>    |  |
|  |  |

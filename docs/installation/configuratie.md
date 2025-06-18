

# Omgevingsvariabelen

Als je PABC installeert met de helm chart, worden deze variabelen geconfigureerd met de chart values. Zie [de helm chart in github](https://github.com/PodiumD-Autorisatie-Beheer-Component/PABC-API/tree/main/charts/pabc) voor meer informatie.
 
---------------------------------
| **Variabele**                      | **Waarde**                                                     |
|------------------------------------|----------------------------------------------------------------|
| ConnectionStrings__PabcConnection | Connectiestring voor de Postgres database. <details> <summary>Meer informatie </summary> Volgens het format `Host=my-hostname; Database=database-name; Username=my-username; Password=my-secret-password`. In de helm chart worden deze waardes individueel opgegeven onder `settings.database`. </details> |
| API_KEY__N            | Lijst van uitgegeven API keys. <details> <summary>Meer informatie </summary>Er kunnen meerdere API keys opgenomen worden in de configuratie: API_KEY__0, API_KEY__1, etc. De API keys kunnen elke willekeurige waarde hebben. Calls naar de api moeten voorzien zijn van een 'X-API-KEY' header met als waarde een van deze API keys. In de helm chart worden deze waardes opgegeven onder `settings.apiKeys` </details>         |
|  |  |

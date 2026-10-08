# Applicaties en applicatierollen pre-fillen

## Wat is prefill?

Met de prefill-functionaliteit kun je de PABC zo configureren dat bij opstarten automatisch applicaties en bijbehorende applicatierollen worden aangemaakt. Dit is handig als je een vaste set applicatierollen wilt aanbieden zonder deze handmatig te hoeven invoeren.

De prefill is **generiek**: de PABC heeft geen kennis van specifieke applicaties. De te pre-fillen applicaties en rollen worden volledig via configuratie (Helm chart) opgegeven.

## Hoe werkt het?

Bij het opstarten van de PABC migratie-job wordt de prefill-configuratie ingelezen. Voor elke geconfigureerde applicatie geldt:

- **Als de applicatie nog niet bestaat**: de applicatie en al haar applicatierollen worden aangemaakt.
- **Als de applicatie al bestaat**: de applicatie wordt **niet** opnieuw aangemaakt of aangepast. Dit wordt gelogd op loglevel `INFO`.
- **Als er voor een bestaande applicatie al applicatierollen bestaan**: deze worden **niet** opnieuw aangemaakt, uitgebreid of aangepast. Dit wordt eveneens gelogd op loglevel `INFO`.

## Configuratie via Helm chart

In de Helm chart values kun je de prefill configureren onder `migrations.prefill.applications`:

```yaml
migrations:
  prefill:
    applications:
      - name: zaakafhandelcomponent
        roles:
          - raadpleger
          - behandelaar
          - coordinator
          - recordmanager
          - beheerder
          - brp_zoeken
          - zaakspecifiek_autorisatie_behandelaar
```

Je kunt meerdere applicaties configureren, elk met hun eigen set applicatierollen.

## Verschil met dataset laden (`JSON_DATASET_PATH`)

De PABC heeft twee mechanismen om data te laden bij opstarten. Het is belangrijk het verschil te kennen:

| | **Prefill** (`PREFILL_PATH`) | **Dataset** (`JSON_DATASET_PATH`) |
|---|---|---|
| **Doel** | Vaste basisinstellingen aanmaken als ze nog niet bestaan | Database volledig initialiseren met een complete dataset |
| **Bestaande data** | Wordt **behouden** — bestaande applicaties en rollen worden overgeslagen | Wordt **verwijderd** — alle bestaande data wordt gewist en vervangen |
| **Scope** | Alleen applicaties en applicatierollen | Alle entiteiten (applicaties, rollen, domeinen, zaaktypes, mappings) |
| **Idempotent** | Ja — kan veilig meerdere keren draaien | Nee — wist altijd alle data en laadt opnieuw |
| **Typisch gebruik** | Productieomgevingen waar basisrollen altijd aanwezig moeten zijn | Ontwikkel-/testomgevingen voor het laden van een bekende beginstatus |

### Volgorde van uitvoering

Wanneer beide zijn geconfigureerd, wordt eerst de dataset geladen en daarna de prefill uitgevoerd. Dit betekent dat in dit geval:

1. De dataset **wist alle bestaande data** en laadt de volledige dataset.
2. De prefill voegt vervolgens applicaties toe die niet in de dataset staan (of slaat ze over als ze al bestaan).

> **Let op**: als je `JSON_DATASET_PATH` gebruikt, wordt bij elke herstart alle bestaande data verwijderd — inclusief eerder ge-prefillde applicaties. De prefill zal deze vervolgens opnieuw aanmaken. Voor productieomgevingen wordt aanbevolen om alleen prefill te gebruiken en `JSON_DATASET_PATH` niet in te stellen.

## Belangrijk

- De prefill-configuratie is optioneel. Zonder configuratie wordt er niets ge-prefilled.
- De prefill is ontworpen om veilig en herhaaldelijk te kunnen draaien zonder dataverlies.
- Handmatig toegevoegde applicaties en rollen worden nooit door de prefill beïnvloed.

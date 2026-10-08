# Zaaktypes importeren

## Vereisten

Om zaaktypes te kunnen importeren moet de ZGW zaakregister koppeling geconfigureerd zijn. Zie [Omgevingsvariabelen](../installation/configuratie) voor de benodigde configuratie (`ZgwZaakregister__*` variabelen).

## Hoe werkt het?

Wanneer de koppeling is ingeschakeld, verschijnt er een **Importeren** knop bij de Entiteitstypes sectie op de Beheer pagina.

1. Klik op de **Importeren** knop.
2. Er opent een dialoog met een toelichting. Klik op **Importeren** om de import te starten.
3. De PABC haalt alle gepubliceerde zaaktypes op uit de geconfigureerde zaaktypecatalogus in het zaakregister.
4. Na afloop toont de dialoog het resultaat:
   - **Aangemaakt**: zaaktypes die nieuw zijn aangemaakt in de PABC.
   - **Overgeslagen**: zaaktypes die al bestonden in de PABC.
   - **Niet meer in zaakregister**: zaaktypes die wel in de PABC staan maar niet meer in het zaakregister voorkomen. Deze worden *niet* automatisch verwijderd.

## Wat wordt er aangemaakt?

Voor elk zaaktype in het zaakregister dat nog niet bestaat in de PABC wordt een nieuw entiteitstype aangemaakt met de volgende waarden:

| Veld             | Waarde                  |
|------------------|-------------------------|
| Entiteitstype    | ZAAKTYPE                |
| Entiteitstype ID | Zaaktype omschrijving   |
| Naam             | Zaaktype omschrijving   |
| URL              | *(niet gevuld)*         |

De match gebeurt op de exacte zaaktype omschrijving (inclusief eventuele voor- en naloop spaties).

## Belangrijke opmerkingen

- Alleen zaaktypes met minstens 1 gepubliceerde versie (`status=definitief`) worden geïmporteerd.
- Zaaktypes die al bestaan in de PABC worden niet gewijzigd of bijgewerkt.
- Zaaktypes die niet meer in het zaakregister voorkomen worden niet verwijderd uit de PABC.
- De import kan meerdere keren uitgevoerd worden; bestaande zaaktypes worden overgeslagen.
